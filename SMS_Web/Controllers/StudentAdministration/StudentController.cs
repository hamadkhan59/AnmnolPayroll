using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_Web.Filters;
using System.Drawing;
using System.IO;
using SMS_Web.Helpers;
using System.IO.Compression;
using PdfSharp.Pdf;
using SMS_Web.Helpers.PdfHelper;
using SMS_DAL.ViewModel;
using SMS.Modules.BuildPdf.FeeSheets;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.SmsRepository.IRepository;
using SMS_Web.Controllers.SecurityAssurance;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json;
using System.Globalization;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.StudentAdministration
{
    public class StudentController : Controller
    {
        private IStudentRepository studentRepo;
        private IClassSectionRepository classSecRepo;
        private IPreviousHitoryRepository historyRepo;
        private IClassRepository classRepo;
        private ISectionRepository secRepo;
        private ILeavingReasonRepository leavingRepo;
        private IFeePlanRepository feePlanRepo;
        private IFinanceAccountRepository financeRepo;

        static byte[] studentImage;
        static bool searchFlag = false;
        static int admissionChargesStudentId = 0;
        static int errorCode = 0;
        static string studentInquiryNumber = "";
        static bool IsPrint = false;
        //static bool IsInquirySearch = false;

        //static List<Student> studentList = null;
        //
        // GET: /Student/

        public StudentController()
        {
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2()); ;
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            historyRepo = SessionHelper.historyRepo;
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            leavingRepo = new LeavingReasonRepositoryImp(new SC_WEBEntities2());
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
            financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
        }

        public ActionResult Index(int id = 0)
        {

            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_ADMISSION) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            Student students = null;
            try
            {
                if (studentInquiryNumber.Length == 0)
                {
                    if (id > 0)
                        students = studentRepo.GetStudentById(id); //db.Students.Where(x => x.id == id).Include(s => s.AdmissionType1).Include(s => s.ClassSection).Include(s => s.Gender).Include(s => s.Relegion).FirstOrDefault();
                }
                else
                {
                    List<StudentInquiry> inquiryList = studentRepo.SearchStudentInquiryByInquiryNo(studentInquiryNumber, branchId);
                    if (inquiryList != null && inquiryList.Count > 0)
                    {
                        StudentInquiry inquiry = inquiryList[0];
                        if (inquiry.TestStatu.StatusName == "PASS")
                            students = getStudent(inquiry);
                        else
                            errorCode = 1421;
                    }
                    else
                    {
                        errorCode = 1422;
                    }
                    studentInquiryNumber = "";
                }

                //if (id > 0 && admissionChargesStudentId == 0 && regularFeeStudentId == 0)
                //    errorCode = 0;
                ViewData["Error"] = errorCode;
                errorCode = 0;
                admissionChargesStudentId = 0;

                ViewBag.DriverId = new SelectList(SessionHelper.transportDriverList, "Id", "DriverName", students == null ? 0 : students.DriverId);

                if (searchFlag)
                    ViewData["student"] = SearchStudent();
                else
                    ViewData["student"] = null;

                searchFlag = false;
                ViewData["Operation"] = id;
                ViewData["nameList"] = SessionHelper.StudentNameList(branchId);

                if (students == null)
                {
                    ViewBag.AdmissionType = new SelectList(SessionHelper.AdmissionTypeList, "Id", "AdmissionType1");
                    ViewBag.GenderCode = new SelectList(SessionHelper.GenderList, "Id", "Gender1");

                    ViewBag.ReligionCode = new SelectList(SessionHelper.RelegionList, "Id", "Name");
                    ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                    ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                    ViewBag.SessionId = new SelectList(SessionHelper.SessionListDD(Session.SessionID), "Id", "Name");
                }
                else
                {
                    var classSection = classSecRepo.GetClassSectionById((int)students.ClassSectionId);
                    studentImage = students.StdImage;
                    ViewBag.AdmissionType = new SelectList(SessionHelper.AdmissionTypeList, "Id", "AdmissionType1", students.AdmissionType);
                    ViewBag.GenderCode = new SelectList(SessionHelper.GenderList, "Id", "Gender1", students.GenderCode);
                    ViewBag.ReligionCode = new SelectList(SessionHelper.RelegionList, "Id", "Name", students.ReligionCode);

                    ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name", classSection.SectionId);
                    ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name", classSection.ClassId);
                    ViewBag.SessionId = new SelectList(SessionHelper.SessionListDD(Session.SessionID), "Id", "Name", students.SessionId);
                    Session["OldName"] = students.Name;
                    Session["ClassId"] = classSection.ClassId;
                    Session["SectionId"] = classSection.SectionId;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchByAdmissionNo(string admissionNo)
        {
            var temp = studentRepo.GetStudentBySrNo(admissionNo);
            if (temp != null)
            {
                return RedirectToAction("Index", new { id = temp.id });
            }
            else
            {
                return RedirectToAction("Index", new { id = 0 });
            }
        }

        public ActionResult Search()
        {

            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_ADMISSION) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            Student students = null;
            try
            {
                if (searchFlag)
                    ViewData["student"] = SearchStudent();
                else
                    ViewData["student"] = null;

                searchFlag = false;

                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                voidSetSearchVeriables();
                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(students);
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


        public ActionResult LoadInquiry(string inquiryNumber)
        {
            studentInquiryNumber = inquiryNumber;
            return RedirectToAction("Index", new { id = 0 });
        }

        private int checkValidClassSection(int ? ClassId, int ? SectionId)
        {
            int errorCode = 0;

            if (ClassId == null || SectionId == null)
            {
                errorCode = 111;
                if (ClassId != null && SectionId == null)
                    errorCode = 112;
            }
            return errorCode;
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]Student student, int ? ClassId, int SectionId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                errorCode = 0;
                errorCode = checkValidClassSection(ClassId, SectionId);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (ModelState.IsValid && errorCode == 0)
                {
                    LogWriter.WriteLog("Student Model is valid, creating new student");
                    if (!string.IsNullOrEmpty(student.AdmissionNo))
                    {
                        LogWriter.WriteLog("Checking if the student exist with the admission no : " + student.AdmissionNo);
                        var temp = studentRepo.GetStudentByAdmissionNo(student.AdmissionNo, branchId);
                        if (temp != null)
                        {
                            errorCode = 500;
                            LogWriter.WriteLog("Student already exist with the admission no : " + student.AdmissionNo);
                        }
                    }
                    if (errorCode == 0)
                    {
                        var maxSrNo = studentRepo.GetMaxSrNo();
                        student.StdImage = studentImage;
                        student.RollNumber = studentRepo.GetMaxRollNo((int)ClassId, (int)SectionId);
                        studentImage = null;
                        if (student.Session == null)
                        {
                            errorCode = 520;
                            return RedirectToAction("Index", new { id = 0 });
                        }
                        student.SessionId = student.Session.Id;
                        student.GenderCode = student.Gender.Id;
                        student.Gender = null;
                        student.ReligionCode = student.Relegion.Id;
                        student.Relegion = null;
                        student.Session = null;
                        student.ClassSectionId = classSecRepo.GetClassSectionId((int)ClassId, (int)SectionId);
                        student.ClassSection = null;
                        student.SrNo = maxSrNo;
                        student.LeavingStatus = 1;
                        student.LeavingStatu = null;
                        student.IsPromoted = false;
                        student.AdmissionNo = string.IsNullOrEmpty(student.AdmissionNo) == true ? studentRepo.GetMaxAdmissionNo() : student.AdmissionNo;
                        student.BranchId = branchId;
                        //student.Branch = null;

                        int returnStatus = studentRepo.AddStudent(student);
                        LogWriter.WriteLog("Student is created succesfully : " + student.AdmissionNo);
                        SmsInfoProxy.sendSmsStudentAdmissionEvent(student);

                        LogWriter.WriteLog("Creating Student Finance account : " + student.AdmissionNo);
                        createFinanceAccount(student);
                        SessionHelper.InvalidateStudentNameCache = false;
                        SessionHelper.InvalidateFathernameCache = false;
                        if (returnStatus == -1)
                            errorCode = 420;
                        else
                        {
                            errorCode = 2;
                            SavePreviousEducationHistory(returnStatus);
                            SaveStudentDocuments(returnStatus);
                        }
                    }
                }
                else
                {
                    LogWriter.WriteLog("Student Model is invalid, returning object");
                    if (errorCode == 0)
                    {
                        ViewBag.DriverId = new SelectList(SessionHelper.transportDriverList, "Id", "DriverName");
                        ViewBag.AdmissionType = new SelectList(SessionHelper.AdmissionTypeList, "Id", "AdmissionType1");
                        //ViewBag.ClassSectionId = new SelectList(SessionHelper.ClassSectionList(Session.SessionID), "ClassSectionId", "ClassSectionId");
                        ViewBag.GenderCode = new SelectList(SessionHelper.GenderList, "Id", "Gender1");
                        ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                        ViewData["nameList"] = SessionHelper.StudentNameList(branchId);
                        ViewBag.ReligionCode = new SelectList(SessionHelper.RelegionList, "Id", "Name");
                        ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                        ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                        ViewBag.SessionId = new SelectList(SessionHelper.SessionList(Session.SessionID), "Id", "Name");
                        return View(student);
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
            //return RedirectToAction("Index", new { id = (student.id == null ? 0 : student.id) });
            return RedirectToAction("Index", new { id = 0 });
        }


        private void createFinanceAccount(Student student)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                FinanceFifthLvlAccount accounts = new FinanceFifthLvlAccount();

                accounts.AccountName = student.id.ToString().PadLeft(6, '0') + "-" + student.Name;
                accounts.AccountDescription = "Fee Recivables Account for : " + accounts.AccountName;
                accounts.CreatedOn = DateTime.Now;
                accounts.Value = 0;
                accounts.Count = 0;
                accounts.FourthLvlAccountId = SessionHelper.GetFourthLvlConfigurationAccount((int)student.BranchId, ConstHelper.CAT_FEE_RECEIVABLE, ConstHelper.CAT_RECEIVABLES);
                accounts.BranchId = student.BranchId;
                financeRepo.AddFinanceFifthLvlAccount(accounts);
                SessionHelper.InvalidateFinanceFifthLvlCache = false;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
}

        private void UpdateFinanceAccount(Student student, string oldName, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                string oldAccountName = student.id.ToString().PadLeft(6, '0') + "-" + oldName;
                FinanceFifthLvlAccount accounts = financeRepo.GetFinanceFifthLvlAccountByName(oldAccountName, branchId);
                accounts.AccountName = student.id.ToString().PadLeft(6, '0') + "-" + student.Name;
                accounts.AccountDescription = "Fee Recivables Account for : " + accounts.AccountName;
                financeRepo.UpdateFinanceFifthLvlAccount(accounts);
                SessionHelper.InvalidateFinanceFifthLvlCache = false;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private void SaveStudentDocuments(int studentId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                studentRepo.DeleteStudentDocs(studentId);
                List<byte[]> stdDocs = (List<byte[]>)Session["StudentDocs"];
                LogWriter.WriteLog("Student documents count : " + (stdDocs == null ? 0 : stdDocs.Count));
                if (stdDocs != null)
                {
                    for (int i = 0; i < 11; i++)
                    {
                        if (stdDocs[i] != null)
                        {
                            StudentDocument doc = new StudentDocument();
                            doc.DocumentType = i + 1;
                            doc.StudentId = studentId;
                            doc.DocumentImage = stdDocs[i];
                            studentRepo.AddStudentDocs(doc);
                        }
                    }
                    Session["StudentDocs"] = null;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private void SavePreviousEducationHistory(int studentId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                historyRepo.DeletePreviousHistory(studentId);
                List<PreviousStudyHistory> historyList = (List<PreviousStudyHistory>)Session[ConstHelper.SESSION_EDUCATION_HISTORY];
                LogWriter.WriteLog("Student education history count : " + (historyList == null ? 0 : historyList.Count));
                if (historyList != null && historyList.Count > 0)
                {
                    foreach (PreviousStudyHistory history in historyList)
                    {
                        history.StudentId = studentId;
                        historyRepo.AddPreviousHistory(history);
                    }
                }
                Session[ConstHelper.SESSION_EDUCATION_HISTORY] = null;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        public ActionResult StudentReleived(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_STUDENT_LEAVING) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                if (id != 1)
                    errorCode = -1;
                ViewData["Error"] = errorCode;
                errorCode = -1;

                //ViewBag.ClassSectionId = new SelectList(SessionHelper.ClassSectionList(Session.SessionID), "ClassSectionId", "ClassSectionId");
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.ReasonId = new SelectList(SessionHelper.LeavingReasonList, "ReasonId", "LeavingReason1");
                ViewBag.StatusId = new SelectList(SessionHelper.LeavingStatusList, "Id", "StatusName", 2);

                voidSetSearchVeriables();
                if (Session[ConstHelper.RELEIVED_SEARCH_FLAG] != null && (bool)Session[ConstHelper.RELEIVED_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.RELEIVED_SEARCH_FLAG] = false;
                    return View(SearchStudent());
                }
                else
                {
                    //var students = db.Students.Where(x => x.id == id).Include(s => s.AdmissionType1).Include(s => s.ClassSection).Include(s => s.Gender).Include(s => s.Relegion);
                    return View(new List<StudentModel>());
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return View(new List<StudentModel>());
            }
        }


        public ActionResult DischargedStudents(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            //if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_DISCHARGED_STUDENTS) == false)
            //{
            //    return RedirectToAction("Index", "NoPermission");
            //}
            try
            {
                
                ViewData["Error"] = errorCode;
                errorCode = -1;

                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.ReasonId = new SelectList(SessionHelper.LeavingReasonList, "ReasonId", "LeavingReason1");
                ViewBag.StatusId = new SelectList(SessionHelper.LeavingStatusList, "Id", "StatusName", 2);

                voidSetSearchVeriables();
                if (Session[ConstHelper.RELEIVED_SEARCH_FLAG] != null && (bool)Session[ConstHelper.RELEIVED_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.RELEIVED_SEARCH_FLAG] = false;
                    return View(SearchDischargedStudent());
                }
                else
                {
                    //var students = db.Students.Where(x => x.id == id).Include(s => s.AdmissionType1).Include(s => s.ClassSection).Include(s => s.Gender).Include(s => s.Relegion);
                    return View(new List<StudentModel>());
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return View(new List<StudentModel>());
            }
        }

        public ActionResult Certificates(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_CERTIFICATES) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                
                ViewData["Error"] = errorCode;
                errorCode = -1;

                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");

                voidSetSearchVeriables();

                if (Session[ConstHelper.CERTIFICATES_SEARCH_FLAG] != null && (bool)Session[ConstHelper.CERTIFICATES_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.CERTIFICATES_SEARCH_FLAG] = false;
                    return View(SearchStudent().Where(x => x.LeavingStatusCode == 1).ToList());
                }
                else
                {
                    //var students = db.Students.Where(x => x.id == id).Include(s => s.AdmissionType1).Include(s => s.ClassSection).Include(s => s.Gender).Include(s => s.Relegion);
                    return View(new List<StudentModel>());
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return View(new List<StudentModel>());
            }
        }
        public ActionResult GenerateCards(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_CARDS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                
                ViewData["Error"] = errorCode;
                errorCode = -1;

                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                voidSetSearchVeriables();

                if (Session[ConstHelper.CARDS_SEARCH_FLAG] != null && (bool)Session[ConstHelper.CARDS_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.CARDS_SEARCH_FLAG] = false;
                    return View(SearchStudent().Where(x => x.LeavingStatusCode == 1).ToList());
                }
                else
                {
                    //var students = db.Students.Where(x => x.id == id).Include(s => s.AdmissionType1).Include(s => s.ClassSection).Include(s => s.Gender).Include(s => s.Relegion);
                    return View(new List<StudentModel>());
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return View(new List<StudentModel>());
            }
        }

        //[HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "Search")]
        //[ValidateAntiForgeryToken]
        //public ActionResult SearchStudent(Student student)
        //{
        //    if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }

        //    StudentSearchParameters(student);

        //    searchFlag = true;
        //    return RedirectToAction("Index", new { id = 0 });
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchStudent(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo, string FatherContact = "")
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                StudentSearchParameters(ClassId, SectionId, RollNo, Name, FatherName, FatherCnic, AdmissionNo, FatherContact);
                searchFlag = true;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Search");
        }

        private void StudentSearchParameters(Student student)
        {
            int classId = 0, sectionId = 0;
            if (student.ClassSection.ClassId != null)
                classId = (int)student.ClassSection.ClassId;
            if (student.ClassSection.SectionId != null)
                sectionId = (int)student.ClassSection.SectionId;

            Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
            Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
            Session[ConstHelper.STUDENT_ROLL_NO] = student.RollNumber;
            Session[ConstHelper.STUDENT_NAME] = student.Name;
            Session[ConstHelper.STUDENT_FATHER_NAME] = student.FatherName;
            Session[ConstHelper.STUDENT_FATHER_CNIC] = student.FatherCNIC;
            Session[ConstHelper.CLASS_ID] = classId;
            if (classId > 0 && sectionId > 0)
                Session[ConstHelper.STUDENT_CLASS_SECTION_ID] = classSecRepo.GetClassSectionId(classId, sectionId);
            else
                Session[ConstHelper.STUDENT_CLASS_SECTION_ID] = 0;
        }



        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "Search")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchStudentReleived(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo, string FatherContact)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                SearchStudentParams(ClassId, SectionId, RollNo, Name, FatherName, FatherCnic, AdmissionNo, FatherContact);
                Session[ConstHelper.RELEIVED_SEARCH_FLAG] = true;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("StudentReleived", new { id = -59 });

        }

        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "Search")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchDischargedStudent(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo, string FatherContact)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                SearchStudentParams(ClassId, SectionId, RollNo, Name, FatherName, FatherCnic, AdmissionNo, FatherContact);
                Session[ConstHelper.RELEIVED_SEARCH_FLAG] = true;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("DischargedStudents", new { id = -59 });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchStudentCards(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo, string FatherContact)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                SearchStudentParams(ClassId, SectionId, RollNo, Name, FatherName, FatherCnic, AdmissionNo, FatherContact);
                //voidSetSearchVeriables();
                Session[ConstHelper.CARDS_SEARCH_FLAG] = true;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("GenerateCards", new { id = -59 });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchCertificates(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo, string FatherContact)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                SearchStudentParams(ClassId, SectionId, RollNo, Name, FatherName, FatherCnic, AdmissionNo, FatherContact);
                //voidSetSearchVeriables();
                Session[ConstHelper.CERTIFICATES_SEARCH_FLAG] = true;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Certificates", new { id = -59 });

        }


        public void SearchStudentParams(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string admissionNo, string FatherContact)
        {
            int classId = int.Parse(string.IsNullOrEmpty(ClassId) == true ? "0" : ClassId);
            int sectionId = int.Parse(string.IsNullOrEmpty(SectionId) == true ? "0" : SectionId);
            Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
            Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;

            Session[ConstHelper.STUDENT_ROLL_NO] = RollNo;
            Session[ConstHelper.STUDENT_NAME] = Name;
            Session[ConstHelper.STUDENT_FATHER_NAME] = FatherName;
            Session[ConstHelper.STUDENT_FATHER_CNIC] = FatherCnic;
            Session[ConstHelper.STUDENT_ADMISSION_NO] = admissionNo;
            Session[ConstHelper.STUDENT_CONTACT_NO] = FatherContact;
            Session[ConstHelper.CLASS_ID] = classId;
            if (classId > 0 && sectionId > 0)
                if (classId > 0 && sectionId > 0)
                    Session[ConstHelper.STUDENT_CLASS_SECTION_ID] = classSecRepo.GetClassSectionId(classId, sectionId);
                else
                    Session[ConstHelper.STUDENT_CLASS_SECTION_ID] = 0;

        }


        private void StudentSearchParameters(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo, string FatherContact)
        {
            int classId = 0, sectionId = 0;
            if (!string.IsNullOrEmpty(ClassId))
                classId = int.Parse(ClassId);
            if (!string.IsNullOrEmpty(SectionId))
                sectionId = int.Parse(SectionId);

            Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
            Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
            Session[ConstHelper.STUDENT_ROLL_NO] = RollNo;
            Session[ConstHelper.STUDENT_NAME] = Name;
            Session[ConstHelper.STUDENT_FATHER_NAME] = FatherName;
            Session[ConstHelper.STUDENT_FATHER_CNIC] = FatherCnic;
            Session[ConstHelper.STUDENT_ADMISSION_NO] = AdmissionNo;
            Session[ConstHelper.STUDENT_CONTACT_NO] = FatherContact;
            Session[ConstHelper.CLASS_ID] = classId;
            if (classId > 0 && sectionId > 0)
                Session[ConstHelper.STUDENT_CLASS_SECTION_ID] = classSecRepo.GetClassSectionId(classId, sectionId);
            else
                Session[ConstHelper.STUDENT_CLASS_SECTION_ID] = 0;
        }

        private List<StudentModel> SearchDischargedStudent()
        {
            List<StudentModel> studentList = null;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int classSectionId = 0;
                if (Session[ConstHelper.STUDENT_CLASS_SECTION_ID] != null)
                    classSectionId = (int)Session[ConstHelper.STUDENT_CLASS_SECTION_ID];
                int classId = (int)Session[ConstHelper.CLASS_ID];
                string rollNo = (string)Session[ConstHelper.STUDENT_ROLL_NO];
                string name = (string)Session[ConstHelper.STUDENT_NAME];
                string fatherName = (string)Session[ConstHelper.STUDENT_FATHER_NAME];
                string fatherCnic = (string)Session[ConstHelper.STUDENT_FATHER_CNIC];
                string admissionNo = (string)Session[ConstHelper.STUDENT_ADMISSION_NO];
                string contact1 = (string)Session[ConstHelper.STUDENT_CONTACT_NO];

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (classSectionId > 0)
                    studentList = studentRepo.SearchDischargedStudents(rollNo, name, fatherName, classSectionId, fatherCnic, branchId, admissionNo, contact1);
                else if (classId > 0)
                    studentList = studentRepo.SearchDischargedClassStudents(rollNo, name, fatherName, classId, fatherCnic, branchId, admissionNo, contact1);
                else
                    studentList = studentRepo.SearchDischargedStudents(rollNo, name, fatherName, fatherCnic, branchId, admissionNo, contact1);

                LogWriter.WriteLog("Total Student(s) found : " + (studentList == null ? 0 : studentList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return studentList;
        }

        private List<StudentModel> SearchStudent()
        {
            List<StudentModel> studentList = null;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int classSectionId = 0;
                if (Session[ConstHelper.STUDENT_CLASS_SECTION_ID] != null)
                    classSectionId = (int)Session[ConstHelper.STUDENT_CLASS_SECTION_ID];
                int classId = (int)Session[ConstHelper.CLASS_ID];
                string rollNo = (string)Session[ConstHelper.STUDENT_ROLL_NO];
                string name = (string)Session[ConstHelper.STUDENT_NAME];
                string fatherName = (string)Session[ConstHelper.STUDENT_FATHER_NAME];
                string fatherCnic = (string)Session[ConstHelper.STUDENT_FATHER_CNIC];
                string admissionNo = (string)Session[ConstHelper.STUDENT_ADMISSION_NO];
                string contact1 = (string)Session[ConstHelper.STUDENT_CONTACT_NO];

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (classSectionId > 0)
                    studentList = studentRepo.SearchStudents(rollNo, name, fatherName, classSectionId, fatherCnic, branchId, admissionNo, contact1);
                else if (classId > 0)
                    studentList = studentRepo.SearchClassStudents(rollNo, name, fatherName, classId, fatherCnic, branchId, admissionNo, contact1);
                else
                    studentList = studentRepo.SearchStudents(rollNo, name, fatherName, fatherCnic, branchId, admissionNo, contact1);

                LogWriter.WriteLog("Total Student(s) found : " + (studentList == null ? 0 : studentList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return studentList;
        }

        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "SubmitLeaving")]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitReleivedEntry(int[] studentIds, DateTime? LeavingDate, string ReasonId, int StatusId, string LeavingRemarks, bool DuesStatus = false)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Total Student(s) found : " + (studentIds == null ? 0 : studentIds.Count()));
                if (studentIds != null && studentIds.Length > 0)
                {
                    if (LeavingDate == null)
                    {
                        RemoveReleivedEntry(studentIds);
                    }
                    else
                    {
                        try
                        {
                            foreach (var studentId in studentIds)
                            {
                                var student = studentRepo.GetStudentById(studentId);
                                student.LeavingDate = LeavingDate;
                                student.ReasonId = int.Parse(ReasonId);
                                student.ClearDues = DuesStatus;
                                student.LeavingStatus = StatusId;
                                student.LeavingRemarks = LeavingRemarks;
                                studentRepo.UpdateStudent(student);
                                SmsInfoProxy.sendSmsStudentLeavingEvent(student);
                            }
                            errorCode = 0;
                        }
                        catch (Exception ex)
                        {
                            LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                            LogWriter.WriteExceptionLog(ex);
                            errorCode = 420;
                        }
                    }
                }
                else
                {
                    errorCode = 420;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("StudentReleived", new { id = 1 });

        }

        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "SubmitLeaving")]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitDischargedStudents(int[] studentIds, DateTime? LeavingDate, string ReasonId, int StatusId, string LeavingRemarks, bool DuesStatus = false)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Total Student(s) found : " + (studentIds == null ? 0 : studentIds.Count()));
                if (studentIds != null && studentIds.Length > 0)
                {
                    try
                    {
                        foreach (var studentId in studentIds)
                        {
                            var student = studentRepo.GetStudentById(studentId);
                            student.LeavingDate = null;
                            student.ReasonId = null;
                            student.LeavingStatus = 1;
                            student.LeavingRemarks = "";
                            student.ClearDues = false;

                            studentRepo.UpdateStudent(student);
                        }
                        errorCode = 0;
                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        LogWriter.WriteExceptionLog(ex);
                        errorCode = 420;
                    }
                }
                else
                {
                    errorCode = 420;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("DischargedStudents", new { id = 1 });

        }

        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "SubmitLeaving")]
        [ValidateAntiForgeryToken]
        public FileResult CertificatesGenerationRequest(string certificateValue, int[] studentIds, string Activities = "", string Remarks = "")
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Total Student(s) found : " + (studentIds == null ? 0 : studentIds.Count()));
                if (studentIds != null && studentIds.Length > 0)
                {
                    StudentHelper helper = new StudentHelper();
                    PdfDocument document = null;
                    StudentPdf pdf = new StudentPdf();
                    if (certificateValue == "1")
                    {
                        document = pdf.CreateCharahcterCertificatePdf(studentIds);
                    }
                    else
                    {
                        document = pdf.CreateSchoolLeavingCertificatePdf(studentIds, Activities, Remarks);
                    }

                    MemoryStream stream = new MemoryStream();
                    document.Save(stream, false);
                    stream.Seek(0, SeekOrigin.Begin);
                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return File(stream, "application/pdf");
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return null;
        }

        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "SubmitLeaving")]
        [ValidateAntiForgeryToken]
        public FileResult CardGenerationRequest(int[] studentIds)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Total Student(s) found : " + (studentIds == null ? 0 : studentIds.Count()));
                if (studentIds != null && studentIds.Length > 0)
                {
                    StudentHelper helper = new StudentHelper();
                    Image[] imageArray = new Image[studentIds.Count()];
                    int imgCount = 0;
                    foreach (var studentId in studentIds)
                    {
                        if (studentId != 0)
                        {
                            var student = studentRepo.GetStudentById(studentId);
                            imageArray[imgCount++] = helper.CreateCard(student);
                        }
                    }

                    using (var compressedFileStream = new MemoryStream())
                    {
                        //Create an archive and store the stream in memory.
                        using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update, false))
                        {
                            for (int i = 0; i < studentIds.Count(); i++)
                            {
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    imageArray[i].Save(stream, imageArray[i].RawFormat);
                                    stream.Seek(0, SeekOrigin.Begin);

                                    var zipEntry = zipArchive.CreateEntry("Student" + (i + 1) + ".png");
                                    using (var zipEntryStream = zipEntry.Open())
                                    {
                                        stream.CopyTo(zipEntryStream);
                                    }
                                }
                            }
                        }

                        LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = "CardsGenrated-" + DateTime.Now.ToString() + ".zip" };
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return null;
        }

        public void RemoveReleivedEntry(int[] studentIds)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                foreach (var studentId in studentIds)
                {
                    var student = studentRepo.GetStudentById(studentId);
                    student.LeavingDate = null;
                    student.ReasonId = null;
                    student.LeavingStatus = 1;
                    student.LeavingRemarks = "";
                    student.ClearDues = false;

                    studentRepo.UpdateStudent(student);
                }
                errorCode = 10;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }

        }

        [HttpPost]
        public void UploadStudentDocs()
        {
             List<byte[]> imageList = new List<byte[]>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                for (int i = 1; i <= 11; i++)
                {
                    if (System.Web.HttpContext.Current.Request.Files["UploadedImage" + i] != null)
                    {
                        var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadedImage" + i];
                        using (var binaryReader = new BinaryReader(httpPostedFile.InputStream))
                        {
                            byte[] doc = binaryReader.ReadBytes(httpPostedFile.ContentLength);
                            imageList.Add(doc);
                        }
                    }
                    else
                    {
                        imageList.Add(null);
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            Session["StudentDocs"] = imageList;
        }

        [HttpPost]
        public void UploadImage()
        {
            var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadedImage"];
            using (var binaryReader = new BinaryReader(httpPostedFile.InputStream))
            {
                studentImage = binaryReader.ReadBytes(httpPostedFile.ContentLength);
            }
        }

        [HttpPost]
        public void UploadPreviousHistory(List<PreviousStudyHistory> historyList)
        {
            Session[ConstHelper.SESSION_EDUCATION_HISTORY] = historyList;
        }


        [HttpGet]
        [WebMethod()]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetRollNumber(int classId, int sectionId)
        {
            var result = studentRepo.GetMaxRollNo(classId, sectionId);
            return JsonConvert.SerializeObject(result);
            //var classSection = db.ClassSections.Where(x => x.ClassId == classId && x.SectionId == sectionId).FirstOrDefault();
            //var maxRollNo = 0;
            //if (classSection != null)
            //{
            //    if (db.Students.Where(x => x.ClassSectionId == classSection.ClassSectionId).FirstOrDefault() != null)
            //        maxRollNo = db.Students.Where(x => x.ClassSectionId == classSection.ClassSectionId).ToList().Count;
            //}

            //return maxRollNo + 1;
        }

        [HttpGet]
        [WebMethod()]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetVanStrength(int driverId)
        {
            var result = studentRepo.GetVanStrength(driverId);
            return JsonConvert.SerializeObject(result);
        }

        //
        // POST: /Student/Edit/5

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student student, int ? ClassId, int ? SectionId)
        {

            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                errorCode = 0;
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (ModelState.IsValid)
                {
                    LogWriter.WriteLog("Student Model is valid, updating student information");
                    if (!string.IsNullOrEmpty(student.AdmissionNo))
                    {
                        var temp = studentRepo.GetStudentByAdmissionNo(student.AdmissionNo, branchId);
                        if (temp != null && temp.id != student.id)
                        {
                            errorCode = 500;
                            LogWriter.WriteLog("Student already exist with the admission no : " + student.AdmissionNo);
                        }
                    }
                    if (errorCode == 0)
                    {
                        student.StdImage = studentImage;
                        studentImage = null;
                        student.SessionId = student.Session.Id;
                        student.GenderCode = student.Gender.Id;
                        student.Gender = null;
                        student.ReligionCode = student.Relegion.Id;
                        student.Relegion = null;
                        student.Session = null;
                        student.ClassSectionId = classSecRepo.GetClassSectionId((int)ClassId, (int)SectionId);
                        //db.Students.Add(student);
                        student.ClassSection = null;
                        //student.LeavingStatus = 1;// student.LeavingStatu.Id;
                        student.LeavingStatu = null;

                        SessionHelper.InvalidateStudentNameCache = false;
                        SessionHelper.InvalidateFathernameCache = false;
                        student.BranchId = branchId;
                        //student.Branch = null;

                        student.AdmissionNo = string.IsNullOrEmpty(student.AdmissionNo) == true ? studentRepo.GetMaxAdmissionNo() : student.AdmissionNo;
                        student.IsPromoted = false;
                        studentRepo.UpdateStudent(student);
                        errorCode = 2;
                        SavePreviousEducationHistory(student.id);
                        SaveStudentDocuments(student.id);

                        string oldName = (string)Session["OldName"];
                        Session["OldName"] = null;

                        if (student.Name != oldName)
                        {
                            UpdateFinanceAccount(student, oldName, branchId);
                        }
                    }
                }
                else
                {
                    ViewBag.AdmissionType = new SelectList(SessionHelper.AdmissionTypeList, "Id", "AdmissionType1", student.AdmissionType);
                    //ViewBag.ClassSectionId = new SelectList(SessionHelper.ClassSectionList(Session.SessionID), "ClassSectionId", "ClassSectionId", student.ClassSectionId);
                    ViewBag.GenderCode = new SelectList(SessionHelper.GenderList, "Id", "Gender1", student.GenderCode);
                    ViewBag.DriverId = new SelectList(SessionHelper.transportDriverList, "Id", "DriverName");
                    ViewData["nameList"] = SessionHelper.StudentNameList(branchId);

                    ViewData["Operation"] = student.id;
                    ViewBag.ReligionCode = new SelectList(SessionHelper.RelegionList, "Id", "Name", student.ReligionCode);
                    ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name", student.ClassSection.ClassId);
                    ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name", student.ClassSection.SectionId);
                    ViewBag.SessionId = new SelectList(SessionHelper.SessionList(Session.SessionID), "Id", "Name", student.SessionId);

                    return View(student);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "AdmissionCharges")]
        [ValidateAntiForgeryToken]
        public ActionResult StudentAdmissionChargePlan(Student student)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            return RedirectToAction("AdmissionCharges", new { id = student.id });
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "RegularFee")]
        [ValidateAntiForgeryToken]
        public ActionResult StudentRegularFeePlan(int StudentId, int ClassId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                Session["regularFeeStudentId"] = StudentId;
                Session["regularFeeClassId"] = ClassId;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("RegularFeePlan", new { id = StudentId });
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Print")]
        [ValidateAntiForgeryToken]
        public FileResult Print(Student student)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (student.id == 0)
                {
                    var maxSrNo = studentRepo.GetMaxSrNo();
                    student.StdImage = studentImage;
                    student.ClassSectionId = classSecRepo.GetClassSectionId((int)student.ClassSection.ClassId, (int)student.ClassSection.SectionId);
                    //student.ClassSectionId = db.ClassSections.Where(x => x.ClassId == student.ClassSection.ClassId && x.SectionId == student.ClassSection.SectionId).FirstOrDefault().ClassSectionId;
                    student.ClassSection = null;
                    student.SrNo = maxSrNo;
                    studentRepo.AddStudent(student);
                }
                student = studentRepo.GetStudentById(student.id);
                StudentForm form = new StudentForm();
                PdfDocument document = form.CreatePdf(student);

                MemoryStream stream = new MemoryStream();
                document.Save(stream, false);
                stream.Seek(0, SeekOrigin.Begin);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }

        //
        // GET: /Student/Delete/5

        public FileStreamResult GetStudentDoc(int studentId, int imageId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                byte[] image = studentRepo.GetStudentImageDoc(studentId, imageId);
                if (image != null)
                {
                    var contentLength = image.Length;
                    Response.AppendHeader("Content-Length", contentLength.ToString());
                    Response.AppendHeader("Content-Disposition", "inline; filename=studentDoc.jpg");

                    Stream stream = new MemoryStream(image);
                    stream.Seek(0, SeekOrigin.Begin);
                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return File(stream, "image/jpeg");
                }
                else
                {
                    LogWriter.WriteLog("No image found");
                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            Student student = studentRepo.GetStudentById(id);
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (student != null)
                {
                    studentRepo.DeleteStudent(student);
                    errorCode = 4;
                }
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

        public ActionResult RegularFeePlan(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int regularFeeClassId = Session["regularFeeClassId"] == null ? 0 : (int)Session["regularFeeClassId"];
                ViewData["challan"] = feePlanRepo.GetChallDetailByClassIdId(regularFeeClassId);
                ViewData["TransportStops"] = SessionHelper.transportStopList;
                if (id > 0)
                {
                    int regularFeeStudentId = id;
                    var studentDetail = feePlanRepo.GetStudentChallanDetailByStudentId(regularFeeStudentId);
                    ViewData["vanFee"] = studentRepo.GetStudentById(regularFeeStudentId).VanFee;
                    //var studentDetail = db.ChallanStudentDetails.Where(c => c.StdId == regularFeeStudentId).ToList();
                    var classChallan = feePlanRepo.GetAllChallanByClassId(regularFeeClassId);
                    if (classChallan == null || classChallan.Count == 0)
                    {
                        ViewData["Error"] = 200;
                        ViewData["DefaultClass"] = regularFeeClassId;
                    }
                    if (studentDetail != null)
                    {
                        LogWriter.WriteLog("Student challan detail is found");
                        var feedetail = studentDetail;
                        var challanDetail = feePlanRepo.GetChallDetailByChallanId(feedetail.ChallanId);
                        //var challanDetail = db.ChallanFeeHeadDetails.Where(c => c.ChallanId == feedetail.ChallanId).ToList();
                        if (challanDetail.Count > 0)
                        {
                            ViewBag.Challans = new SelectList(classChallan, "Id", "Name", studentDetail.ChallanId);
                            LogWriter.WriteLog("Challan detail is found");
                            LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                            return View(challanDetail);
                        }
                        else
                        {
                            LogWriter.WriteLog("No challan detail is found");
                        }
                    }
                    else
                    {
                        LogWriter.WriteLog("No student challan detail is found");
                    }
                    ViewBag.Challans = new SelectList(classChallan, "Id", "Name", 0);
                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return View("");
                }
                else
                {
                    LogWriter.WriteLog("No student is selected to assign fee plan");
                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    ViewBag.Challans = new SelectList(feePlanRepo.GetAllChallanByClassId(regularFeeClassId), "Id", "Name", 0);
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

        public ActionResult AdmissionCharges(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ViewData["Error"] = errorCode;
                errorCode = 0;
                if (id > 0)
                {
                    var admissionDetail = feePlanRepo.GetStudentAdmissionChargesByStudentId(id);
                    //var admissionDetail = db.StudentAdmissionCharges.Where(c => c.StudentId == id).ToList();
                    if (admissionDetail.Count == 0)
                    {
                        var feaHeadsList = SessionHelper.FeeHeadList(Session.SessionID);
                        foreach (FeeHead head in feaHeadsList)
                        {
                            StudentAdmissionCharge detail = new StudentAdmissionCharge();
                            detail.StudentId = id;
                            detail.HeadId = head.Id;
                            //detail.FeeHead = head;
                            detail.Amount = 0;
                            feePlanRepo.SaveAdmissionCharges(detail);
                        }
                        admissionDetail = feePlanRepo.GetStudentAdmissionChargesByStudentId(id);
                        //admissionDetail = db.StudentAdmissionCharges.Where(c => c.StudentId == id).ToList();
                    }
                    List<ChallanDetailViewModel> vmList = new List<ChallanDetailViewModel>();
                    foreach (StudentAdmissionCharge detail in admissionDetail)
                    {
                        ChallanDetailViewModel vm = new ChallanDetailViewModel();
                        vm.Id = detail.Id;
                        vm.Amount = (int)detail.Amount;
                        vm.StandardAmount = (int)detail.FeeHead.Amount;
                        vm.Name = detail.FeeHead.Name;
                        vmList.Add(vm);
                    }
                    admissionChargesStudentId = id;
                    return View(vmList);

                }
                else
                {
                    return View("");
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
        public ActionResult SaveAdmissionCharges(int[] ChargesIds, int[] Indexes, string[] PaidAmount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (admissionChargesStudentId > 0)
                {
                    for (int i = 0; i < ChargesIds.Count(); i++)
                    {
                        int PaidIndex = Indexes[i];
                        int paidAmount = int.Parse(PaidAmount[PaidIndex]);
                        int chargesId = ChargesIds[i];
                        if (chargesId > 0)
                        {
                            //StudentAdmissionCharge detail = db.StudentAdmissionCharges.Find(chargesId);
                            StudentAdmissionCharge detail = feePlanRepo.GetStudentAdmissionChargesById(chargesId);
                            detail.Amount = paidAmount;
                            feePlanRepo.UpdateStudentAdmissionCharges(detail);
                        }
                    }
                    errorCode = 230;
                    if (IsPrint)
                    {
                        IsPrint = false;
                        //var chargesList = db.StudentAdmissionCharges.Where(x => x.StudentId == admissionChargesStudentId && x.Amount > 0).ToList();
                        var chargesList = feePlanRepo.GetPositiveStudentAdmissionChargesByStudentId(admissionChargesStudentId);
                        if (chargesList.Count == 0)
                        {
                            errorCode = 5;
                            return RedirectToAction("AdmissionCharges", new { id = admissionChargesStudentId });
                        }
                        else
                            return PrintStudentAdmissionChallan();
                    }
                }
                else
                {
                    if (IsPrint)
                    {
                        errorCode = 5;
                        return RedirectToAction("AdmissionCharges", new { id = admissionChargesStudentId });
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 1420;
            }
            //return Redirect(Request.UrlReferrer.ToString());
            return RedirectToAction("Index", new { id = admissionChargesStudentId });
        }

        private FileResult PrintStudentAdmissionChallan()
        {
            Student student = studentRepo.GetStudentById(admissionChargesStudentId);
            AdmissionChargesPdf pdf = new AdmissionChargesPdf();
            PdfDocument document = pdf.CreatePdf(student);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf");

            //using (MemoryStream stream = new MemoryStream())
            //{
            //    document.Save(stream, false);

            //    stream.Seek(0, SeekOrigin.Begin);
            //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = student.RollNumber + "-" + student.Name + "-" + "Admission Charges" + "-" + DateTime.Now.ToString() + ".pdf" };
            //}

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRegularFeePlan(int ChallanId, string StopName, List<UserChallanModel> ChallanDetail)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            ChallanDetail.RemoveAt(ChallanDetail.Count - 1);
            int regularFeeStudentId = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (Session["regularFeeStudentId"] != null)
                    regularFeeStudentId = (int)Session["regularFeeStudentId"];
                if (regularFeeStudentId > 0)
                {
                    //var stdList = db.ChallanStudentDetails.Where(x => x.StdId == regularFeeStudentId).ToList();
                    var stdList = feePlanRepo.GetStudentChallanDetailByStudentId(regularFeeStudentId);
                    var admissionHead = feePlanRepo.GetAdmissionChargesHead();
                    int admissionAmount = ChallanDetail.Where(x => x.FeeHead == admissionHead.Name).FirstOrDefault().Amount;
                    ChallanDetail.Where(x => x.FeeHead == admissionHead.Name).ToList().ForEach(x => x.Amount = 0);

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
                    errorCode = 1230;

                    var vanFee = ChallanDetail.Where(x => x.FeeHead.ToUpper() == "VAN FEE").FirstOrDefault();
                    if (vanFee != null)
                    {
                        var student = studentRepo.GetStudentById(regularFeeStudentId);
                        student.StopName = StopName;
                        student.VanFee = vanFee.Amount;
                        studentRepo.UpdateStudent(student);
                    }

                    if (admissionAmount > 0)
                    {
                        var arrearDetail = feePlanRepo.GetStudentArrearDetail(regularFeeStudentId);
                        int feeBalanceId = (int) (arrearDetail[0].FeeBalanceId == null ? 0 : arrearDetail[0].FeeBalanceId);

                        if (feeBalanceId == 0)
                        {
                            FeeBalance feeBalance = new FeeBalance();
                            feeBalance.StudentId = regularFeeStudentId;
                            feeBalance.Balance = feeBalance.Advance = 0;
                            feePlanRepo.AddFeeBalance(feeBalance);
                            feeBalanceId = feeBalance.Id;
                        }


                        foreach (var arrear in arrearDetail)
                        {
                            FeeArrearsDetail detail = new FeeArrearsDetail();
                            detail.FeeHeadId = arrear.FeeHeadId;
                            detail.FeeBalanceId = feeBalanceId;
                            detail.HeadAmount = 0;
                            if(arrear.FeeHeadId == admissionHead.Id)
                                detail.HeadAmount = admissionAmount;

                            var obj = feePlanRepo.GetFeeArrearDetail(feeBalanceId, (int)arrear.FeeHeadId);
                            if (detail.HeadAmount > 0)
                                AddArrearHistroy(detail, 0, (int)detail.HeadAmount, 0);

                            if (obj == null)
                            {
                                feePlanRepo.SaveFeeArrearDetail(detail);
                            }
                            else
                            {
                                detail.ID = obj.ID;
                                feePlanRepo.UpdateFeeArrearDetail(detail);
                            }
                        }

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
            //return Redirect(Request.UrlReferrer.ToString());
            return RedirectToAction("Index", new { id = regularFeeStudentId });
        }

        private void AddArrearHistroy(FeeArrearsDetail detail, int discount, int amount, int arrearAmount)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (discount > 0 || amount > 0)
                {
                    ArreartHistory history = new ArreartHistory();
                    history.FeeBalanceId = detail.FeeBalanceId;
                    history.FeeHeadId = detail.FeeHeadId;
                    history.CreatedOn = DateTime.Now;
                    if (arrearAmount == 0)
                    {
                        history.Description = "New Arrear is added to student with Amount : " + amount.ToString();
                    }
                    else
                    {
                        if (amount > 0)
                        {
                            history.PayAmount = detail.HeadAmount;
                            history.Discount = 0;
                            history.Description = "Amount : " + amount + " is added to student arrear";
                        }
                        if (discount > 0)
                        {
                            history.PayAmount = 0;
                            history.Discount = discount;
                            history.Description = "Discount : " + discount + " is given to student arrear";
                        }
                    }

                    history.IsAddedInChallan = true;
                    feePlanRepo.AddArrearHistory(history);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
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


        [HttpPost]
        public JsonResult GetStudentPreviousStudyHistory(int studentId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var historyList = historyRepo.GetAllPreviousHitoryByStudentId(studentId);
                List<string[]> historyListJson = new List<string[]>();
                LogWriter.WriteLog("Student History Count : " + (historyList == null ? 0 : historyList.Count));
                foreach (PreviousStudyHistory history in historyList)
                {
                    string[] tempObj = new string[4];
                    tempObj[0] = history.CampusName;
                    tempObj[1] = history.WithdrawlClass;
                    tempObj[2] = history.StudyDuration;
                    tempObj[3] = history.id.ToString();
                    historyListJson.Add(tempObj);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return Json(historyListJson);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }

        Student getStudent(StudentInquiry inquiry)
        {
            Student std = new Student();
            std.Name = inquiry.Name;
            std.FatherName = inquiry.FatherName;
            std.DateOfBirth = inquiry.DateOfBirth;
            std.AdmissionDate = inquiry.AdmissionDate;
            std.Contact_1 = inquiry.Contact_1;
            std.CurrentAddress = inquiry.CurrentAddress;
            std.ReligionCode = inquiry.ReligionCode;
            std.GenderCode = inquiry.GenderCode;
            std.ClassSection = new ClassSection();
            std.ClassSection.ClassId = inquiry.ClassId;
            std.FatherOccupation = inquiry.FatherOccupation;
            std.Nationality = inquiry.Nationality;
            std.Cast = inquiry.Cast;
            std.FatherCNIC = inquiry.FatherCNIC;
            std.Contact_2 = inquiry.Contact_2;
            std.PermanentAddress = inquiry.PermanentAddress;
            std.SessionId = inquiry.SessionId;
            std.MotherName = inquiry.MotherName;
            std.MotherContact1 = inquiry.MotherContact1;
            std.MotherContact2 = inquiry.MotherContact2;
            std.Email = inquiry.Email;
            std.StdImage = inquiry.StdImage;
            std.BFormNo = inquiry.BFormNo;
            std.MotherCnic = inquiry.MotherCnic;
            std.MotherOccupation = inquiry.MotherOccupation;
            std.GuardianName = inquiry.GuardianName;
            std.GuardinCnic = inquiry.GuardinCnic;
            std.GuardinContact = inquiry.GuardinContact;
            return std;
        }

        [HttpGet]
        public void setPrint()
        {
            IsPrint = true;
        }


        public ActionResult DashBoard()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.AD_ADMISSION_DASHBOARD) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            return View();
        }

        public JsonResult GetAdmissionLineStats(string from = null, string to = null, string view = "month")
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DateTime fromDate = DateTime.Now.AddDays(DateTime.Now.Day * -1 + 1);
                if (from != null && from.Length > 24)
                    fromDate = DateTime.ParseExact(from.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                DateTime toDate = DateTime.Now;
                if (to != null && to.Length > 24)
                {
                    toDate = DateTime.ParseExact(to.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var admissionStats = studentRepo.GetAdmissionLineStats(branchId, fromDate, toDate, view);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return Json(admissionStats, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }

        public JsonResult GetClassAdmissionStats(string from = null, string to = null)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DateTime fromDate = DateTime.Now.AddDays(DateTime.Now.Day * -1 + 1);
                if (from != null && from.Length > 24)
                    fromDate = DateTime.ParseExact(from.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                DateTime toDate = DateTime.Now;
                if (to != null && to.Length > 24)
                {
                    toDate = DateTime.ParseExact(to.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var admissionStats = studentRepo.GetClassAdmissionStats(branchId, fromDate, toDate);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return Json(admissionStats, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }
    }
}