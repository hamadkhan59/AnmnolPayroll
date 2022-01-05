using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Web.Helpers;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.FeeCollection
{
    public class StudentExtraChargesController : Controller
    {
        //
        // GET: /StudentExtraCharges/
        
        IFeePlanRepository feePlanRepo;
        IClassSectionRepository classSecRepo;
        IClassRepository classRepo;
        ISectionRepository secRepo;
        IStudentRepository studentRepo;
        static int errorCode = 0;
        public StudentExtraChargesController()
        {
            
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());;
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_STD_EXT_CHARGES) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<StudentExtraChargesViewModel> chargesList = new List<StudentExtraChargesViewModel>();

            try
            {
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.MonthId = new SelectList(SessionHelper.MonthList, "Id", "Month1");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewBag.FeeHeadId = new SelectList(SessionHelper.FeeHeadListDD(Session.SessionID), "Id", "Name");
                ViewData["Error"] = errorCode;
                errorCode = 0;
                voidSetSearchVeriables();

                if (Session[ConstHelper.STUDENT_EXTRA_CHARGES_SEARCH_FLAG] != null && (bool)Session[ConstHelper.STUDENT_EXTRA_CHARGES_SEARCH_FLAG] == true)
                {
                    chargesList = SearchStudentExtraCharges();
                }
                else
                    chargesList = GetTopTwentyRecords();
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(chargesList);
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
        public ActionResult SavestudentExtraCharges(string MonthId, string YearId, string ClassId, string SectionId, string RollNo, string FeeHeadId, int Amount = 0, string Description = "")
        {
            try
            {
                if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_STD_EXT_CHARGES) == false)
                {
                    return RedirectToAction("Index", "NoPermission");
                }

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                string currentMonth = SessionHelper.GetMonthName(int.Parse(MonthId)) + "-" + (2016 + int.Parse(YearId) - 1);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                
                bool isSearch = false;
                if (Session[ConstHelper.STUDENT_EXTRA_CHARGES_SEARCH_FLAG] != null)
                    isSearch = (bool)Session[ConstHelper.STUDENT_EXTRA_CHARGES_SEARCH_FLAG];
                if (isSearch)
                {
                    SetSearchParams(currentMonth, ClassId, SectionId, RollNo, FeeHeadId, Amount);
                }
                else
                {
                    int issueChallanCount = 0;
                    int classSectionId = 0;
                    ClassId = ClassId.Length == 0 ? "0" : ClassId;
                    SectionId = SectionId.Length == 0 ? "0" : SectionId;
                    int classId = int.Parse(ClassId);
                    int sectionId = int.Parse(SectionId);
                    if (classId > 0 && sectionId > 0)
                    {
                        classSectionId = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                    }
                    if(classSectionId > 0)
                        issueChallanCount = feePlanRepo.GetIssueChallanCount(classSectionId, currentMonth, branchId);
                    else
                        issueChallanCount = feePlanRepo.GetIssuedChallanCount(currentMonth, branchId);
                    
                    if (issueChallanCount > 0)
                    {
                        errorCode = 421;
                    }
                    else
                    {
                        if (Amount == 0)
                        {
                            errorCode = 3420;
                            return RedirectToAction("Index");
                        }

                        if (ClassId.Length == 0)
                            ClassId = "0";
                        if (string.IsNullOrEmpty(SectionId))
                            SectionId = "0";

                        StudentExtraCharge charges = new StudentExtraCharge();
                        charges.ForMonth = currentMonth;
                        charges.ClassId = int.Parse(ClassId);
                        charges.SectionId = int.Parse(SectionId);
                        charges.RollNumber = RollNo;
                        charges.FeeHeadId = int.Parse(FeeHeadId);
                        charges.Status = 0;
                        charges.Description = Description;
                        charges.HeadAmount = Amount;
                        charges.CreatedOn = DateTime.Now;
                        charges.BranchId = branchId;

                        if (charges.RollNumber.Length == 0)
                            charges.RollNumber = "-1";
                        string clasName = "All", sectionName = "All";
                        if (int.Parse(ClassId) > 0)
                            clasName = classRepo.GetClassById(int.Parse(ClassId)).Name;
                        if (int.Parse(SectionId) > 0)
                            clasName = secRepo.GetSectionById(int.Parse(SectionId)).Name;
                        string criteria = "Student Extra Charges For Class : " + clasName + ", Section : " + sectionName;
                        if (RollNo.Length > 0)
                            criteria += ", RollNo : " + RollNo;
                        charges.Criteria = criteria;

                        feePlanRepo.SaveStudentEtraCharges(charges);
                        errorCode = 10;
                    }
                }
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

        private List<StudentExtraChargesViewModel> SearchStudentExtraCharges()
        {
            List<StudentExtraChargesViewModel> extraChargesaList = null;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Session[ConstHelper.STUDENT_EXTRA_CHARGES_SEARCH_FLAG] = false;
                string currentMonth = (string)Session[ConstHelper.STUDENT_EXTRA_CHARGES_FOR_MONTH];
                string ClassId = (string)Session[ConstHelper.STUDENT_EXTRA_CHARGES_CLASS_ID];
                string SectionId = (string)Session[ConstHelper.STUDENT_EXTRA_CHARGES_SECTION_ID];
                string RollNo = (string)Session[ConstHelper.STUDENT_EXTRA_CHARGES_ROLL_NO];
                string FeeHeadId = (string)Session[ConstHelper.STUDENT_EXTRA_CHARGES_FEE_HEAD_ID];
                int Amount = (int)Session[ConstHelper.STUDENT_EXTRA_CHARGES_AMOUNT];

                extraChargesaList = feePlanRepo.SearchStudentExtraCharges(currentMonth, int.Parse(ClassId), int.Parse(SectionId), RollNo, int.Parse(FeeHeadId), Amount);
                LogWriter.WriteLog("Search Extra Charges Count : " + (extraChargesaList == null ? 0 : extraChargesaList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 1420;
            }
            return extraChargesaList;
        }

        private List<StudentExtraChargesViewModel> GetTopTwentyRecords()
        {
            List<StudentExtraChargesViewModel> extraChargesaList = null;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                extraChargesaList = feePlanRepo.SearchStudentExtraCharges("", 0, 0, "-1", 0, 0);
                //extraChargesaList = extraChargesaList.Take(20).ToList();
                LogWriter.WriteLog("Search Extra Charges Count : " + (extraChargesaList == null ? 0 : extraChargesaList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 1420;
            }
            return extraChargesaList;
        }

        public ActionResult Delete(int id = 0)
        {

            try
            {
                if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_STD_EXT_CHARGES) == false)
                {
                    return RedirectToAction("Index", "NoPermission");
                }

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (id > 0)
                {
                    int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                    StudentExtraCharge charges = feePlanRepo.GetStudentExtraCharges(id, branchId);
                    int classSectionId = 0;
                    int issueChallanCount = 0;
                    int classId = (int)charges.ClassId;
                    int sectionId = (int)charges.SectionId;
                    string currentMonth = charges.ForMonth;
                    if (classId > 0 && sectionId > 0)
                    {
                        classSectionId = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                    }
                    if (classSectionId > 0)
                        issueChallanCount = feePlanRepo.GetIssueChallanCount(classSectionId, currentMonth, branchId);
                    else
                        issueChallanCount = feePlanRepo.GetIssuedChallanCount(currentMonth, branchId);

                    if (issueChallanCount > 0)
                    {
                        errorCode = 501;
                    }
                    else
                    {
                        feePlanRepo.DeleteStudentEtraCharges(id, branchId);
                        errorCode = 11;
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 2420;
            }
            return RedirectToAction("Index");
        }

        [HttpGet]

        public void SetSrearch()
        {
            Session[ConstHelper.STUDENT_EXTRA_CHARGES_SEARCH_FLAG] = true;
        }

        private void SetSearchParams(string currentMonth, string ClassId, string SectionId, string RollNo, string FeeHeadId, int Amount)
        {
            if (ClassId.Length == 0)
                ClassId = "0";
            if (string.IsNullOrEmpty(SectionId) == true)
                SectionId = "0";
            if (RollNo.Length == 0)
                RollNo = "-1";
            Session[ConstHelper.STUDENT_EXTRA_CHARGES_FOR_MONTH] = currentMonth;
            Session[ConstHelper.STUDENT_EXTRA_CHARGES_CLASS_ID] = ClassId;
            Session[ConstHelper.STUDENT_EXTRA_CHARGES_SECTION_ID] = SectionId;
            Session[ConstHelper.STUDENT_EXTRA_CHARGES_ROLL_NO] = RollNo;
            Session[ConstHelper.STUDENT_EXTRA_CHARGES_FEE_HEAD_ID] = FeeHeadId;
            Session[ConstHelper.STUDENT_EXTRA_CHARGES_AMOUNT] = Amount;

            int classId = int.Parse(string.IsNullOrEmpty(ClassId) == true ? "0" : ClassId);
            int sectionId = int.Parse(string.IsNullOrEmpty(SectionId) == true ? "0" : SectionId);
            Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
            Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;

        }
    }
}
