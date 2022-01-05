using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_Web.Filters;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.StudentAdministration
{
    public class ClassSectionController : Controller
    {
        //private cs db = new cs();
        //private SC_WEBEntities2 db = SessionHelper.dbContext;
        private static int errorCode = 0;
        private static int studentErrorCode = 0;
        private IFinanceAccountRepository financeRepo;
        private IFeePlanRepository feeRepo;
        private IClassSectionRepository classSectionRepo;
        private ISectionRepository secRepo;
        private IClassRepository classRepo;
        IStudentRepository studentRepo;

        //
        // GET: /ClassSection/

        public ClassSectionController()
        {
            classSectionRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());;
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            feeRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());;
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2()); ;
        }

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID,ConstHelper.SA_CLASS_SECTIONS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<ClassSectionModel> classsections = new List<ClassSectionModel>();
            try
            {
                if (id > 0)
                    errorCode = 0;
                ViewData["Error"] = errorCode;
                errorCode = 0;
                ViewData["Operation"] = id;
                Session[ConstHelper.CLASS_SECTION_ID] = id;
                classsections = SessionHelper.ClassSectionList(Session.SessionID);
                var tempClassSection = classSectionRepo.GetClassSectionById(id);
                if (tempClassSection == null)
                {
                    ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                    ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                    ViewData["IsFinanceAccountOpen"] = false;
                }
                else
                {
                    ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name", tempClassSection.ClassId);
                    ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name", tempClassSection.SectionId);
                    ViewData["IsFinanceAccountOpen"] = tempClassSection.IsFinanceAccountOpen;
                }

            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(classsections);
        }



        //
        // POST: /ClassSection/Create

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClassSection classsection, bool? IsFinanceAccountOpen)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var tempObj = classSectionRepo.GetClassSectionByClassAndSectionId((int)classsection.ClassId, (int)classsection.SectionId);
                //classsection.IsFinanceAccountOpen = (bool)IsFinanceAccountOpen;
                classsection.IsFinanceAccountOpen = true;
                CreateFinanceAccount((int)classsection.ClassId, (int)classsection.SectionId, branchId);

                if (ModelState.IsValid && tempObj == null)
                {
                    classsection.CreatedOn = DateTime.Now;
                    int returnStatus = classSectionRepo.AddClassSection(classsection);
                    if (returnStatus == -1)
                        errorCode = 420;
                    else
                        errorCode = 2;
                    SessionHelper.InvalidateClassSectionCache = false;
                }
                else if (tempObj != null)
                    errorCode = 11;
                else
                    errorCode = 1;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClassSection classsection, bool? IsFinanceAccountOpen)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var secObj = classSectionRepo.GetClassSectionByClassAndSectionId((int)classsection.ClassId, (int)classsection.SectionId);
                classsection.ClassSectionId = (int)Session[ConstHelper.CLASS_SECTION_ID];
                //classsection.IsFinanceAccountOpen = (bool)IsFinanceAccountOpen;
                classsection.IsFinanceAccountOpen = true;
                //if (classsection.IsFinanceAccountOpen)
                //    CreateFinanceAccount((int)classsection.ClassId, (int)classsection.SectionId);

                if (ModelState.IsValid && secObj.ClassSectionId == classsection.ClassSectionId)
                {
                    classSectionRepo.UpdateClassSection(classsection);
                    errorCode = 2;
                    SessionHelper.InvalidateClassSectionCache = false;
                }
                else if (secObj != null)
                    errorCode = 11;
                else
                    errorCode = 1;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        private int CreateFinanceAccount(int classId, int sectionId, int branchId)
        {
            int accountId = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                string className = classRepo.GetClassById(classId).Name;
                string sectionName = secRepo.GetSectionById(sectionId).Name;
                string fourthLvlAccountName = className + ", Section : " + sectionName;
                FinanceFourthLvlAccount accounts = new FinanceFourthLvlAccount();
                var thirdLvlObj = financeRepo.GetFinanceThirdLvlAccountByName(className, branchId);
                if (thirdLvlObj == null)
                    return -59;
                else
                    accounts.ThirdLvlAccountId = thirdLvlObj.Id;

                var fourthLvlAccountTemp = financeRepo.GetFinanceFourthLvlAccountByName(fourthLvlAccountName, branchId);

                if (fourthLvlAccountTemp == null)
                {
                    accounts.AccountName = fourthLvlAccountName;
                    accounts.AccountDescription = "This account is created for section " + sectionName + " fee collection";
                    accounts.CreatedOn = DateTime.Now;
                    accounts.Value = 0;// int.Parse(this.txtValue.Text.ToString());
                    accounts.BranchId = branchId;
                    financeRepo.AddFinanceFourthLvlAccount(accounts);
                    SessionHelper.InvalidateFinanceFourthLvlCache = false;
                    List<FeeHead> feeHeadList = feeRepo.GetAllFeeHeads(branchId);
                    foreach (FeeHead head in feeHeadList)
                    {
                        FinanceFifthLvlAccount accountsFifthLvl = new FinanceFifthLvlAccount();
                        var fifthLvlObj = financeRepo.GetFinanceFifthLvlAccountByName(head.Name, branchId);

                        if (fifthLvlObj == null || fifthLvlObj.FourthLvlAccountId != accounts.Id)
                        {
                            accountsFifthLvl.AccountName = head.Name;
                            accountsFifthLvl.AccountDescription = "This account is created for fee head " + head.Name + " fee collection for " + accounts.AccountName;
                            accountsFifthLvl.CreatedOn = DateTime.Now;
                            accountsFifthLvl.Value = 0;
                            accountsFifthLvl.Count = 0;
                            accountsFifthLvl.FourthLvlAccountId = accounts.Id;
                            accountsFifthLvl.BranchId = branchId;
                            financeRepo.AddFinanceFifthLvlAccount(accountsFifthLvl);
                            SessionHelper.InvalidateFinanceFifthLvlCache = false;
                        }
                    }
                    return accounts.Id;
                }
                else
                {
                    List<FeeHead> feeHeadList = feeRepo.GetAllFeeHeads(branchId);
                    foreach (FeeHead head in feeHeadList)
                    {
                        FinanceFifthLvlAccount accountsFifthLvl = new FinanceFifthLvlAccount();
                        var fifthLvlObj = financeRepo.GetFinanceFifthLvlAccount(head.Name, fourthLvlAccountTemp.Id);

                        if (fifthLvlObj == null || fifthLvlObj.FourthLvlAccountId != accounts.Id)
                        {
                            accountsFifthLvl.AccountName = head.Name;
                            accountsFifthLvl.AccountDescription = "This account is created for fee head " + head.Name + " fee collection for " + accounts.AccountName;
                            accountsFifthLvl.CreatedOn = DateTime.Now;
                            accountsFifthLvl.Value = 0;
                            accountsFifthLvl.Count = 0;
                            accountsFifthLvl.FourthLvlAccountId = fourthLvlAccountTemp.Id;
                            financeRepo.AddFinanceFifthLvlAccount(accountsFifthLvl);
                            SessionHelper.InvalidateFinanceFifthLvlCache = false;
                        }
                    }
                }
                accountId = fourthLvlAccountTemp.Id;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return accountId;
        }

        //
        // GET: /ClassSection/Delete/5

        //[HttpGet]
        public ActionResult Delete(int id = 0)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ClassSection classsection = classSectionRepo.GetClassSectionById(id);
                if (classsection == null)
                {
                    return HttpNotFound();
                }
                int studentCount = classSectionRepo.GetStudentCount(classsection.ClassSectionId);
                if (studentCount == 0)
                {
                    classSectionRepo.DeleteClassSection(classsection);
                    errorCode = 4;
                    SessionHelper.InvalidateClassSectionCache = false;
                }
                else
                    errorCode = 40;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 3;
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        public ActionResult ChangeClasseSection()
        {
            List<StudentModel> list = new List<StudentModel>();
            try
            {
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewData["Error"] = studentErrorCode;
                studentErrorCode = 0;
                if (Session[ConstHelper.EXAM_PROMOTE_STUDENTSEARCH] != null)
                    list = (List<StudentModel>)Session[ConstHelper.EXAM_PROMOTE_STUDENTSEARCH];

                voidSetSearchVeriables();
                Session[ConstHelper.EXAM_PROMOTE_STUDENTSEARCH] = null;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(list);
        }

        private void voidSetSearchVeriables()
        {
            if (Session[ConstHelper.GLOBAL_CLASS_ID] != null)
            {
                ViewData["GlobalClassId"] = (int)Session[ConstHelper.GLOBAL_CLASS_ID];
                Session[ConstHelper.GLOBAL_CLASS_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_SECTION_ID] != null)
            {
                ViewData["GlobalSectionId"] = (int)Session[ConstHelper.GLOBAL_SECTION_ID];
                Session[ConstHelper.GLOBAL_SECTION_ID] = null;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchChangeClasseSection(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo, string FatherContact)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ClassId = string.IsNullOrEmpty(ClassId) == true ? "0" : ClassId;
                SectionId = string.IsNullOrEmpty(SectionId) == true ? "0" : SectionId;

                int classId = int.Parse(ClassId);
                int sectionId = int.Parse(SectionId);

                int classSectionId = 0;
                if (classId > 0 && sectionId > 0)
                    classSectionId = classSectionRepo.GetClassSectionId(classId, sectionId);

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (classSectionId > 0)
                    Session[ConstHelper.EXAM_PROMOTE_STUDENTSEARCH] = studentRepo.SearchStudents(RollNo, Name, FatherName, classSectionId, FatherCnic, branchId, AdmissionNo, FatherContact).Where(x => x.IsPromoted == false).ToList();
                else if (classId > 0)
                    Session[ConstHelper.EXAM_PROMOTE_STUDENTSEARCH] = studentRepo.SearchClassStudents(RollNo, Name, FatherName, classId, FatherCnic, branchId, AdmissionNo, FatherContact).Where(x => x.IsPromoted == false).ToList();
                else
                    Session[ConstHelper.EXAM_PROMOTE_STUDENTSEARCH] = studentRepo.SearchStudents(RollNo, Name, FatherName, FatherCnic, branchId, AdmissionNo, FatherContact).Where(x => x.IsPromoted == false).ToList();

                Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("ChangeClasseSection");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveChangeClassSection(int[] studentIds, int PromoteClassId, int PromoteSectionId, int MakeSequence, int OrderStudentId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (MakeSequence == 1)
                {
                    var student = studentRepo.GetStudentById(OrderStudentId);
                    studentRepo.MakeClassSequential((int)student.ClassSectionId);
                    studentErrorCode = 110;
                }
                else
                {
                    int clasSectionId = classSectionRepo.GetClassSectionId(PromoteClassId, PromoteSectionId);
                    if (clasSectionId > 0)
                    {
                        foreach (int id in studentIds)
                        {
                            var student = studentRepo.GetStudentById(id);
                            student.ClassSectionId = clasSectionId;
                            var classSec = classSectionRepo.GetClassSectionById(clasSectionId);
                            student.RollNumber = studentRepo.GetMaxRollNo((int)classSec.ClassId, (int)classSec.SectionId);
                            studentRepo.UpdateStudent(student);
                        }
                    }
                    studentErrorCode = 100;
                }

                SessionHelper.InvalidateStudentNameCache = false;
                SessionHelper.InvalidateFathernameCache = false;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                studentErrorCode = MakeSequence == 1 ? 421 : 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("ChangeClasseSection");
        }

    }
}