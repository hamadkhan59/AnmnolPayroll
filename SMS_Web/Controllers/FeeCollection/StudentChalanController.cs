using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_DAL.ViewModel;
using SMS_Web.Helpers;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Controllers.SecurityAssurance;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.FeeCollection
{
    public class StudentChalanController : Controller
    {
        static int errorCode = 0;
        //
        // GET: /StudentChalan/

        
        IFeePlanRepository feePlanRepo;
        IClassSectionRepository classSecRepo;
        IClassRepository classRepo;
        ISectionRepository secRepo;
        IStudentRepository studentRepo;

        public StudentChalanController()
        {
            
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());;
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_FEE_INCREMENT) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewData["Error"] = errorCode;
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                errorCode = 0;
                voidSetSearchVeriables();

                if (Session[ConstHelper.SEARCH_CHALLAN_FLAG] != null && (bool)Session[ConstHelper.SEARCH_CHALLAN_FLAG] == true)
                {
                    Session[ConstHelper.SEARCH_CHALLAN_FLAG] = false;
                    int classId = (int)Session[ConstHelper.SEARCH_CHALLAN_CLASS_ID];
                    ViewBag.ChallanId = new SelectList(feePlanRepo.GetAllChallanByClassId(classId), "Id", "Name");
                    return View(SearchStudentChalan());
                }
                else
                {
                    ViewBag.ChallanId = new SelectList(SessionHelper.ChallanList(Session.SessionID), "Id", "Name");
                    return View("");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return View("");
            }
        }

        //
        // GET: /StudentChalan/Details/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchStudentChalan(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string chalanNo, string FatherCnic, string AdmissionNo, string FatherContact)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                SearchStudentParams(ClassId, SectionId, RollNo, Name, FatherName, FatherCnic, AdmissionNo, FatherContact);
                Session[ConstHelper.SEARCH_CHALLAN_FLAG] = true;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", new { id = -59 });

        }

        private List<StudentChallanViewModel> SearchStudentChalan()
        {
            List<StudentModel> studentList = new List<StudentModel>();
            List<StudentChallanViewModel> studentChalanList = new List<StudentChallanViewModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                string rollNumber = (string)Session[ConstHelper.SEARCH_CHALLAN_ROLL_NO];
                string name = (string)Session[ConstHelper.SEARCH_CHALLAN_NAME];
                string fatherName = (string)Session[ConstHelper.SEARCH_CHALLAN_FATHER_NAME];
                string fatherCnic = (string)Session[ConstHelper.SEARCH_CHALLAN_FATHER_CNIC];
                string admissionNo = (string)Session[ConstHelper.SEARCH_CHALLAN_ADMISSION_NO];
                string contactNo = (string)Session[ConstHelper.SEARCH_CHALLAN_CONTACT_NO];
                int classSectionId = (int)Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID];
                int classId = (int)Session[ConstHelper.SEARCH_CHALLAN_CLASS_ID];

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (classSectionId > 0)
                    studentList = studentRepo.SearchStudents(rollNumber, name, fatherName, classSectionId, fatherCnic, branchId, admissionNo, contactNo);
                else if (classId > 0)
                    studentList = studentRepo.SearchClassStudents(rollNumber, name, fatherName, classId, fatherCnic, branchId, admissionNo, contactNo);
                else
                    studentList = studentRepo.SearchStudents(rollNumber, name, fatherName, fatherCnic, branchId, admissionNo, contactNo);

                studentList = studentList.Where(x => x.LeavingStatus == "ACTIVE").ToList();
                foreach (var student in studentList)
                {
                    StudentChallanViewModel scvm = new StudentChallanViewModel();
                    scvm.Id = student.Id;
                    scvm.Name = student.Name;
                    scvm.FatherName = student.FatherName;
                    scvm.RollNumber = student.RollNumber;
                    var studentChalan = feePlanRepo.GetStudentChallanDetailByStudentId(student.Id);
                    if (studentChalan != null)
                    {
                        scvm.Chalan = studentChalan.Challan.Name;
                        //scvm.Chalan = feePlanRepo.GetChallanById(studentChalan.ChallanId).Name;
                        scvm.Amount = "" + feePlanRepo.GetChallanAmountByChallanId(studentChalan.ChallanId);
                        //scvm.Amount = "" + feePlanRepo.GetChallanAmountByChallanId(studentChalan.ChallanId);
                    }
                    else
                    {
                        scvm.Chalan = "";
                        scvm.Amount = "";
                    }
                    studentChalanList.Add(scvm);
                }
                LogWriter.WriteLog("Search Student Challan List Count : " + (studentChalanList == null ? 0 : studentChalanList.Count) );
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return studentChalanList;
        }

        public void SearchStudentParams(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo, string FatherContact)
        {
            int classId = int.Parse(string.IsNullOrEmpty(ClassId) == true  ? "0" : ClassId);
            int sectionId = int.Parse(string.IsNullOrEmpty(SectionId) == true ? "0" : SectionId);
            Session[ConstHelper.SEARCH_CHALLAN_ROLL_NO] = RollNo;
            Session[ConstHelper.SEARCH_CHALLAN_CLASS_ID] = classId;
            Session[ConstHelper.SEARCH_CHALLAN_NAME] = Name;
            Session[ConstHelper.SEARCH_CHALLAN_FATHER_NAME] = FatherName;
            Session[ConstHelper.SEARCH_CHALLAN_FATHER_CNIC] = FatherCnic;
            Session[ConstHelper.SEARCH_CHALLAN_ADMISSION_NO] = AdmissionNo;
            Session[ConstHelper.SEARCH_CHALLAN_CONTACT_NO] = FatherContact;
            Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
            Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
            if (classId > 0 && sectionId > 0)
                Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID] = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
            else
                Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID] = 0;
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
        public ActionResult SaveStudentChalan(int[] ChalanIds, string ChallanId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                int chalanId = int.Parse(ChallanId);
                foreach (var id in ChalanIds)
                {

                    var studentChalan = feePlanRepo.GetStudentChallanDetailByStudentId(id);
                    if (studentChalan != null)
                    {
                        studentChalan.ChallanId = chalanId;
                        feePlanRepo.UpdateStudentChallanDetail(studentChalan);
                    }
                    else
                    {
                        ChallanStudentDetail csd = new ChallanStudentDetail();
                        csd.ChallanId = chalanId;
                        csd.StdId = id;
                        feePlanRepo.AddStudentChallanDetail(csd);
                    }
                }
                errorCode = 4;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index", new { id = -59 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchByAdmissionNo(string admissionNo)
        {
            var temp = studentRepo.GetStudentBySrNo(admissionNo);

            return RedirectToAction("SingleStudentChallan", new { id = temp.id });

        }

        public ActionResult SingleStudentChallan(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_SINGLE_STUDENT_CHALLAN) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["nameList"] = SessionHelper.StudentNameList(branchId);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                if (id > 0)
                {
                    var studentDetail = feePlanRepo.GetStudentChallanDetailByStudentId(id);
                    var student = studentRepo.GetStudentById(id);
                    ViewData["vanFee"] = student.VanFee;
                    var clasecModel = classSecRepo.GetClassSectionsModelById((int)student.ClassSectionId);
                    int classId = (int)clasecModel.ClassId;
                    var classChallan = feePlanRepo.GetAllChallanByClassId(classId);
                    if (classChallan == null || classChallan.Count == 0)
                    {
                        ViewData["Error"] = 200;
                        ViewData["DefaultClass"] = classId;
                    }
                    ViewData["challan"] = feePlanRepo.GetChallDetailByClassIdId(classId);
                    Session["regularFeeStudentId"] = id;
                    ViewBag.Challans = new SelectList(classChallan, "Id", "Name", 0);
                    ViewData["RollNo"] = student.RollNumber;
                    ViewData["Name"] = student.Name;
                    ViewData["Father"] = student.FatherName;
                    if (studentDetail != null)
                    {
                        var feedetail = studentDetail;
                        var challanDetail = feePlanRepo.GetChallDetailByChallanId(feedetail.ChallanId);
                        if (challanDetail.Count > 0)
                        {

                            ViewBag.Challans = new SelectList(classChallan, "Id", "Name", studentDetail.ChallanId);
                            return View(challanDetail);
                        }
                    }
                    return View("");
                }
                else
                {
                    ViewBag.Challans = new SelectList(new List<Challan>());

                    return View("");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return View("");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveSingleStudentChallan(int ChallanId, string StopName, List<UserChallanModel> ChallanDetail)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            int regularFeeStudentId = (int)Session["regularFeeStudentId"];
            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            ChallanDetail.RemoveAt(ChallanDetail.Count - 1);
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (regularFeeStudentId > 0)
                {
                    //var stdList = db.ChallanStudentDetails.Where(x => x.StdId == regularFeeStudentId).ToList();
                    var stdList = feePlanRepo.GetStudentChallanDetailByStudentId(regularFeeStudentId);
                    ChallanId = IsNewChallan(ChallanId, ChallanDetail, regularFeeStudentId, branchId);
                    if (stdList == null)
                    {
                        ChallanStudentDetail detail = new ChallanStudentDetail();
                        detail.StdId = regularFeeStudentId;
                        detail.ChallanId = ChallanId;
                        detail.CreatedOn = DateTime.Now;
                        feePlanRepo.AddStudentChallanDetail(detail);
                    }
                    else
                    {
                        var detail = stdList;
                        detail.ChallanId = ChallanId;
                        feePlanRepo.UpdateStudentChallanDetail(detail);
                    }
                    errorCode = 10;

                    var vanFee = ChallanDetail.Where(x => x.FeeHead.ToUpper() == "VAN FEE").FirstOrDefault();
                    if (vanFee != null)
                    {
                        var student = studentRepo.GetStudentById(regularFeeStudentId);
                        student.StopName = StopName;
                        student.VanFee = vanFee.Amount;
                        studentRepo.UpdateStudent(student);
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return RedirectToAction("SingleStudentChallan", new { id = 0 });
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            //return Redirect(Request.UrlReferrer.ToString());
            return RedirectToAction("SingleStudentChallan", new { id = regularFeeStudentId });
        }

        private int IsNewChallan(int ChallanId, List<UserChallanModel> ChallanDetail, int studentId, int branchId)
        {
            int challanId = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                bool newChallannFLag = true;
                var oldChallan = feePlanRepo.GetChallanById(ChallanId);
                int newAmount = ChallanDetail.Sum(x => x.Amount);

                var oldChallanList = feePlanRepo.GetAllChallanByClassId((int)oldChallan.ClassId);
                LogWriter.WriteLog("Check if the challan is found in the class with the amount : " + newAmount);
                foreach (Challan chalan in oldChallanList)
                {
                    var tempDetail = feePlanRepo.GetChallDetailByChallanId(chalan.Id);
                    int oldAmount = tempDetail.Sum(x => x.Amount);
                    if (oldAmount == newAmount)
                    {
                        var updateList = ChallanDetail.Where(x => x.Amount > 0).ToList();

                        if (updateList != null && updateList.Count > 0)
                        {
                            int unMatchedCount = 0;
                            foreach (UserChallanModel model in updateList)
                            {
                                var tempObj = tempDetail.Where(x => x.Name == model.FeeHead).FirstOrDefault();
                                if (tempObj.Amount != model.Amount)
                                {
                                    unMatchedCount++;
                                    break;
                                }
                            }
                            if (unMatchedCount == 0)
                            {
                                newChallannFLag = false;
                                LogWriter.WriteLog("Challan is found with the amount");
                                challanId = chalan.Id;
                                break;
                            }
                        }
                    }
                }

                if (newChallannFLag)
                {
                    LogWriter.WriteLog("No challan is found with the amount");
                    LogWriter.WriteLog("Creating new challan with the amount");
                    Challan newChallan = new Challan();
                    newChallan.BranchId = oldChallan.BranchId;
                    newChallan.ClassId = oldChallan.ClassId;
                    string className = classRepo.GetClassById((int)oldChallan.ClassId).Name;
                    newChallan.Name = className + " Class Challan (" + newAmount + ")";
                    var challanList = feePlanRepo.GetAllChallan().Where(x => x.Name.Contains(newChallan.Name)).ToList();
                    if (challanList != null && challanList.Count > 0)
                        newChallan.Name = newChallan.Name + challanList.Count;
                    newChallan.Description = "Customise Challan for Class " + className + "Amount (" + newAmount + ")";
                    newChallan.CreatedOn = DateTime.Now;
                    newChallan.SystemGenerated = true;
                    newChallan.IsDefault = false;
                    feePlanRepo.AddChallan(newChallan);
                    SessionHelper.InvalidateChallanCache = false;

                    LogWriter.WriteLog("Adding fee heads in the challan");
                    var headList = feePlanRepo.GetAllFeeHeads(branchId);
                    foreach (FeeHead head in headList)
                    {
                        ChallanFeeHeadDetail detail = new ChallanFeeHeadDetail();
                        detail.HeadId = head.Id;
                        detail.ChallanId = newChallan.Id;
                        var tempData = ChallanDetail.Where(x => x.FeeHead == head.Name).FirstOrDefault();
                        detail.Amount = 0;
                        if (tempData != null)
                            detail.Amount = tempData.Amount;
                        feePlanRepo.AddChallanDetail(detail);
                    }

                    challanId = newChallan.Id;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return challanId;
        }
    }
}