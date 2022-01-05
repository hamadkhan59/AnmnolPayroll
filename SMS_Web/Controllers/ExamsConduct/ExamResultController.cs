using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_DAL.ViewModel;
using PdfSharp.Pdf;
using SMS.Modules.BuildPdf;
using System.IO;
using System.IO.Compression;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using System.Configuration;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.ExamsConduct
{
    public class ExamResultController : Controller
    {
        //private SC_WEBEntities2 db = SessionHelper.dbContext;
        static int errorCode = 0, classErrorCode = 0, grandErrorCode;
        static int examTotalMarks = 0, examPassPercentage = 0;
        static int sheetSubjectId = 0;
        static List<int> totalExamMarksList = null;
        static List<int> examPassPercentageList = null;
        static List<string> courseNameList = null;
        //
        // GET: /ExamResult/


        IClassSectionRepository classSecRepo;
        IClassRepository classRepo;
        ISectionRepository secRepo;
        IStudentRepository studentRepo;
        IExamRepository examRepo;
        IFeePlanRepository feePlanRepo;
        IClassSubjectRepository clasSubjRepo;
        ISubjectRepository subjRepo;
        public ExamResultController()
        {

            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2()); ;
            examRepo = new ExamRepositoryImp(new SC_WEBEntities2()); ;
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2()); ;
            clasSubjRepo = new ClassSubjectRepositoryImp(new SC_WEBEntities2()); ;
            subjRepo = new SubjectRepositoryImp(new SC_WEBEntities2()); ;
        }

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_MARKS_SHEET) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                voidSetSearchVeriables();

                if (Session[ConstHelper.SEARCH_EXAM_RESULT_FLAG] != null && (bool)Session[ConstHelper.SEARCH_EXAM_RESULT_FLAG] == true)
                {
                    Session[ConstHelper.SEARCH_EXAM_RESULT_FLAG] = false;
                    ViewData["examSheet"] = SearchMarksSheet();
                }
                ViewData["Error"] = errorCode;
                ViewData["branchId"] = branchId;
                errorCode = 0;
                ViewData["totalMarks"] = examTotalMarks;
                ViewData["passPerecentage"] = examPassPercentage;
                examTotalMarks = examPassPercentage = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View("");
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

            if (Session[ConstHelper.GLOBAL_YEAR_ID] != null)
            {
                ViewData["GlobalYearId"] = (int)Session[ConstHelper.GLOBAL_YEAR_ID];
                Session[ConstHelper.GLOBAL_YEAR_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_SUBJECT_ID] != null)
            {
                ViewData["GlobalSujectId"] = (int)Session[ConstHelper.GLOBAL_SUBJECT_ID];
                Session[ConstHelper.GLOBAL_SUBJECT_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_TERM_ID] != null)
            {
                ViewData["GlobalTermId"] = (int)Session[ConstHelper.GLOBAL_TERM_ID];
                Session[ConstHelper.GLOBAL_TERM_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_EXAM_TYPE_ID] != null)
            {
                ViewData["GLobalExamTypeId"] = (int)Session[ConstHelper.GLOBAL_EXAM_TYPE_ID];
                Session[ConstHelper.GLOBAL_EXAM_TYPE_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_RESULT_TYPE_ID] != null)
            {
                ViewData["GLobalResultTypeId"] = (int)Session[ConstHelper.GLOBAL_RESULT_TYPE_ID];
                Session[ConstHelper.GLOBAL_RESULT_TYPE_ID] = null;
            }
        }


        public ActionResult Promote()
        {
            List<StudentModel> list = new List<StudentModel>();
            try
            {
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                ViewData["Error"] = classErrorCode;
                classErrorCode = 0;
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

        public ActionResult PromotedStudents()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_PROMOTED_STUDENT) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<StudentModel> list = new List<StudentModel>();
            try
            {
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                ViewData["Error"] = classErrorCode;
                classErrorCode = 0;
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchPromoteStudent(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo, string FatherContact)
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
                    classSectionId = classSecRepo.GetClassSectionId(classId, sectionId);

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);

                List<StudentModel> list = new List<StudentModel>();
                if (classSectionId > 0)
                    list = studentRepo.SearchStudents(RollNo, Name, FatherName, classSectionId, FatherCnic, branchId, AdmissionNo, FatherContact).Where(x => x.IsPromoted == false).ToList();
                else if (classId > 0)
                    list = studentRepo.SearchClassStudents(RollNo, Name, FatherName, classId, FatherCnic, branchId, AdmissionNo, FatherContact).Where(x => x.IsPromoted == false).ToList();
                else
                    list = studentRepo.SearchStudents(RollNo, Name, FatherName, FatherCnic, branchId, AdmissionNo, FatherContact).Where(x => x.IsPromoted == false).ToList();

                LogWriter.WriteLog("Search Record Count : " + (list == null ? 0 : list.Count));
                Session[ConstHelper.EXAM_PROMOTE_STUDENTSEARCH] = list;
                Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Promote");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchPromotedStudent(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo, string FatherContact)
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
                    classSectionId = classSecRepo.GetClassSectionId(classId, sectionId);

                List<StudentModel> list = new List<StudentModel>();
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (classSectionId > 0)
                    list = studentRepo.SearchStudents(RollNo, Name, FatherName, classSectionId, FatherCnic, branchId, AdmissionNo, FatherContact).Where(x => x.IsPromoted == true).ToList();
                else if (classId > 0)
                    list = studentRepo.SearchClassStudents(RollNo, Name, FatherName, classId, FatherCnic, branchId, AdmissionNo, FatherContact).Where(x => x.IsPromoted == true).ToList();
                else
                    list = studentRepo.SearchStudents(RollNo, Name, FatherName, FatherCnic, branchId, AdmissionNo, FatherContact).Where(x => x.IsPromoted == true).ToList();

                LogWriter.WriteLog("Search Record Count : " + (list == null ? 0 : list.Count));
                Session[ConstHelper.EXAM_PROMOTE_STUDENTSEARCH] = list;
                Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("PromotedStudents");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePromoteStudent(int[] studentIds, int PromoteClassId, int PromoteSectionId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int clasSectionId = classSecRepo.GetClassSectionId(PromoteClassId, PromoteSectionId);
                if (clasSectionId > 0)
                {
                    foreach (int id in studentIds)
                    {
                        var student = studentRepo.GetStudentById(id);
                        student.ClassSectionId = clasSectionId;
                        student.IsPromoted = true;
                        studentRepo.UpdateStudent(student);
                    }
                }
                classErrorCode = 100;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                classErrorCode = 420;
            }
            return RedirectToAction("Promote");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePromotedStudent(int[] studentIds, int PromoteClassId, int PromoteSectionId, int IsDemoted = 0)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int clasSectionId = classSecRepo.GetClassSectionId(PromoteClassId, PromoteSectionId);
                if (clasSectionId > 0)
                {
                    foreach (int id in studentIds)
                    {
                        var student = studentRepo.GetStudentById(id);
                        student.ClassSectionId = clasSectionId;
                        if (IsDemoted == 1)
                        {
                            student.IsPromoted = false;
                        }
                        studentRepo.UpdateStudent(student);
                    }
                }
                classErrorCode = 100;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                classErrorCode = 420;
            }
            return RedirectToAction("PromotedStudents");
        }

        public ActionResult ClassResult(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_CLASS_RESULT) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<ExamResultViewModel> ervmList = null;

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewData["branchId"] = branchId;


                examTotalMarks = examPassPercentage = 0;
                if (Session[ConstHelper.SEARCH_CLASS_SHEET_FLAG] != null && (bool)Session[ConstHelper.SEARCH_CLASS_SHEET_FLAG] == true)
                {
                    Session[ConstHelper.SEARCH_CLASS_SHEET_FLAG] = false;
                    ervmList = SearchClassSheet();
                }

                int studentCount = ervmList == null ? 0 : ervmList.Count;
                int courseCount = courseNameList == null ? 0 : courseNameList.Count;
                if (courseCount > 0 && studentCount % courseCount != 0)
                    classErrorCode = 111;

                ViewData["totalMarksList"] = totalExamMarksList;
                ViewData["passPerecentageList"] = examPassPercentageList;
                ViewData["courseNameList"] = courseNameList;
                ViewData["Error"] = classErrorCode;
                GetExamSysConfig();
                voidSetSearchVeriables();

                classErrorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                classErrorCode = 420;
            }

            if ((ervmList != null && ervmList.Count > 0) && ((int)ViewData["Error"]) != 111)
                return View(ervmList);
            else
                return View("");
        }

        private void GetExamSysConfig()
        {
            ViewData[SysConfig.EC_GRADE_FLAG] = SysConfig.GetSystemParam(SysConfig.EC_GRADE_FLAG).ParamValue;
            ViewData[SysConfig.EC_POSITION_FLAG] = SysConfig.GetSystemParam(SysConfig.EC_POSITION_FLAG).ParamValue;
            ViewData[SysConfig.EC_PERCENTAGE_FLAG] = SysConfig.GetSystemParam(SysConfig.EC_PERCENTAGE_FLAG).ParamValue;
            ViewData[SysConfig.EC_REMARKS_FLAG] = SysConfig.GetSystemParam(SysConfig.EC_REMARKS_FLAG).ParamValue;
            ViewData[SysConfig.EC_TEACHER_SIGN_FLAG] = SysConfig.GetSystemParam(SysConfig.EC_TEACHER_SIGN_FLAG).ParamValue;
            ViewData[SysConfig.EC_PARENT_SIGN_FLAG] = SysConfig.GetSystemParam(SysConfig.EC_PARENT_SIGN_FLAG).ParamValue;
            ViewData[SysConfig.EC_PRICIPLE_SIGN_FLAG] = SysConfig.GetSystemParam(SysConfig.EC_PRICIPLE_SIGN_FLAG).ParamValue;
        }
        public ActionResult GrandResult(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_GRAND_RESULT) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<List<string>> resultDataset = null;

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewBag.ResultTypeId = new SelectList(SessionHelper.ResultTypeList, "Id", "TypeDescription");
                ViewData["Operation"] = 0;
                ViewData["branchId"] = branchId;
                examTotalMarks = examPassPercentage = 0;

                if (Session[ConstHelper.SEARCH_GRAND_SHEET_FLAG] != null && (bool)Session[ConstHelper.SEARCH_GRAND_SHEET_FLAG] == true)
                {
                    Session[ConstHelper.SEARCH_GRAND_SHEET_FLAG] = false;
                    resultDataset = SearchGrandResult(branchId);
                }
                ViewData["Error"] = grandErrorCode;
                grandErrorCode = 0;
                ViewData["totalMarksList"] = totalExamMarksList;
                ViewData["passPerecentageList"] = examPassPercentageList;
                ViewData["courseNameList"] = courseNameList;
                GetExamSysConfig();
                voidSetSearchVeriables();
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                classErrorCode = 420;
            }

            if (resultDataset != null && resultDataset.Count > 0)
                return View(resultDataset);
            else
                return View("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchMarksSheet(string ClassId, string SectionId, string Year, string ExamTypeId, string SubjectId, string TermId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (string.IsNullOrEmpty(ClassId) == true || string.IsNullOrEmpty(SectionId) == true || string.IsNullOrEmpty(Year) == true
                    || string.IsNullOrEmpty(ExamTypeId) == true || string.IsNullOrEmpty(SubjectId) == true)
                {
                    errorCode = 1420;
                }
                else
                {
                    Session[ConstHelper.SEARCH_EXAM_RESULT_FLAG] = true;
                    int classId = int.Parse(ClassId);
                    int sectionId = int.Parse(SectionId);
                    Session[ConstHelper.SEARCH_EXAM_RESULT_CLASS_SECTION_ID] = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                    Session[ConstHelper.SEARCH_EXAM_RESULT_EXAM_ID] = int.Parse(ExamTypeId);
                    Session[ConstHelper.SEARCH_EXAM_RESULT_SUBJECT_ID] = int.Parse(SubjectId);

                    Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                    Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
                    Session[ConstHelper.GLOBAL_YEAR_ID] = int.Parse(Year);
                    Session[ConstHelper.GLOBAL_EXAM_TYPE_ID] = int.Parse(ExamTypeId);
                    Session[ConstHelper.GLOBAL_SUBJECT_ID] = int.Parse(SubjectId);
                    Session[ConstHelper.GLOBAL_TERM_ID] = int.Parse(TermId);

                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                classErrorCode = 420;
            }

            return RedirectToAction("Index", new { id = -59 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchClassSheet(string ClassId, string SectionId, string Year, string ExamTypeId, string TermId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Session[ConstHelper.SEARCH_CLASS_SHEET_FLAG] = true;
                int classId = int.Parse(ClassId);
                int sectionId = int.Parse(SectionId);
                Session[ConstHelper.SEARCH_CLASS_SHEET_CLASS_SECTION_ID] = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                Session[ConstHelper.SEARCH_CLASS_SHEET_EXAM_ID] = int.Parse(ExamTypeId);

                Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
                Session[ConstHelper.GLOBAL_YEAR_ID] = int.Parse(Year);
                Session[ConstHelper.GLOBAL_EXAM_TYPE_ID] = int.Parse(ExamTypeId);
                Session[ConstHelper.GLOBAL_TERM_ID] = int.Parse(TermId);

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("ClassResult", new { id = -59 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchGrandResult(string ClassId, string SectionId, string Year, string ExamTypeId, string TermId, string RollNo, int ResultTypeId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Session[ConstHelper.SEARCH_GRAND_SHEET_FLAG] = true;
                int classId = int.Parse(ClassId);
                int sectionId = int.Parse(SectionId);
                Session[ConstHelper.SEARCH_GRAND_SHEET_CLASS_SECTION_ID] = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                if (!RollNo.Equals(""))
                    Session[ConstHelper.SEARCH_GRAND_STUDENT_ID] = studentRepo.GetStudentByRollNoAndClassSectionId(RollNo, (int)Session[ConstHelper.SEARCH_GRAND_SHEET_CLASS_SECTION_ID]).id;
                else
                    Session[ConstHelper.SEARCH_GRAND_STUDENT_ID] = -1;
                if (ExamTypeId != null && ExamTypeId.Length > 0 && !ExamTypeId.Equals("0"))
                    Session[ConstHelper.SEARCH_GRAND_SHEET_EXAM_ID] = int.Parse(ExamTypeId);
                else
                    Session[ConstHelper.SEARCH_GRAND_SHEET_EXAM_ID] = -1;
                if (TermId != null && TermId.Length > 0 && !TermId.Equals("0"))
                    Session[ConstHelper.SEARCH_GRAND_SHEET_TERM_ID] = int.Parse(TermId);
                else
                    Session[ConstHelper.SEARCH_GRAND_SHEET_TERM_ID] = -1;
                Session[ConstHelper.SEARCH_GRAND_SHEET_YEAR] = Year;
                Session[ConstHelper.SEARCH_GRAND_RESULT_TYPE] = ResultTypeId;

                Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
                Session[ConstHelper.GLOBAL_YEAR_ID] = int.Parse(Year);
                Session[ConstHelper.GLOBAL_EXAM_TYPE_ID] = int.Parse(ExamTypeId);
                Session[ConstHelper.GLOBAL_TERM_ID] = int.Parse(TermId);
                Session[ConstHelper.GLOBAL_RESULT_TYPE_ID] = ResultTypeId;

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("GrandResult", new { id = -59 });
        }

        public static IEnumerable<IEnumerable<T>> ToChunks<T>(IEnumerable<T> enumerable,
                                                      int chunkSize)
        {
            int itemsReturned = 0;
            var list = enumerable.ToList(); // Prevent multiple execution of IEnumerable.
            int count = list.Count;
            while (itemsReturned < count)
            {
                int currentChunkSize = Math.Min(chunkSize, count - itemsReturned);
                yield return list.GetRange(itemsReturned, currentChunkSize);
                itemsReturned += currentChunkSize;
            }
        }

        private List<List<string>> SearchGrandResult(int branchId)
        {
            List<List<string>> result = new List<List<string>>();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int grandClassSectionId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_CLASS_SECTION_ID];
                int grandTermId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_TERM_ID];
                int grandExamTypeId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_EXAM_ID];
                int grandStudentId = (int)Session[ConstHelper.SEARCH_GRAND_STUDENT_ID];
                int resultTypeId = (int)Session[ConstHelper.SEARCH_GRAND_RESULT_TYPE];
                string grandYear = (string)Session[ConstHelper.SEARCH_GRAND_SHEET_YEAR];


                if (resultTypeId == 1)
                {
                    LogWriter.WriteLog("Compiling Accumulative Grand result");
                    result = GetAccumulativeSessionResult(grandClassSectionId, grandTermId, grandExamTypeId, grandStudentId, branchId, grandYear);
                }
                else
                {
                    LogWriter.WriteLog("Compiling Scalar Grand result");
                    result = GetSessionResult(grandClassSectionId, grandTermId, grandExamTypeId, grandStudentId, branchId, grandYear);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return result;
        }


        List<List<string>> GetAccumulativeSessionResult(int grandClassSectionId, int grandTermId, int grandExamTypeId, int grandStudentId, int branchId, string grandYear)
        {
            var resultds = new DataSet();
            List<List<string>> result = new List<List<string>>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[spSessionResultAccumulative]
		                                                        @ClassSectionId = " + grandClassSectionId + ","
                                                                        + "@termId = " + grandTermId + ","
                                                                        + "@ExamTypeId = " + grandExamTypeId + ","
                                                                        + "@studentId = " + grandStudentId + ","
                                                                        + "@year = '" + grandYear + "',"
                                                                        + "@BranchId = " + branchId;

                List<Exam> examList = new List<Exam>();
                resultds = examRepo.BuildStudentGrandList(spQuery);
                LogWriter.WriteLog("Accumulative Session Result Dataset Count : " + (resultds == null || resultds.Tables == null ? 0 : resultds.Tables.Count));

                if (resultds != null && resultds.Tables != null && resultds.Tables.Count > 0)
                {
                    for (int i = 0; i < resultds.Tables.Count; i++)
                    {
                        DataTable tb = resultds.Tables[i];

                        int chunkSize = GetTableChunkSize(tb);
                        if (examList == null || examList.Count == 0)
                        {
                            for (int chucnkCount = 0; chucnkCount * chunkSize < tb.Rows.Count; chucnkCount++)
                            {
                                int examTypeId = int.Parse(tb.Rows[chucnkCount * chunkSize]["ExamTypeId"].ToString());
                                examList.AddRange(examRepo.GetExamByExamType(examTypeId, grandClassSectionId).OrderBy(x => x.CourseId));
                            }
                        }
                        DataSet ds = convertAccumulativeTabletoDataset(tb, chunkSize, examList);
                        LogWriter.WriteLog("Accumulative Session Result Table Count : " + (ds == null || ds.Tables == null ? 0 : ds.Tables.Count));
                        List<string> tableList = new List<string>();
                        for (int k = 0; k < ds.Tables.Count; k++)
                        {
                            tableList.Add(ConvertDataTableToHTML(ds.Tables[k], grandClassSectionId, grandTermId, grandExamTypeId, grandYear, 1));
                        }
                        result.Add(tableList);
                    }
                }
                grandErrorCode = 0;
                ViewData["Operation"] = 1;
                Session["GrandResultPosition"] = resultds;
                LogWriter.WriteLog("Accumulative Session Compiled Result Count : " + (result == null ? 0 : result.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                grandErrorCode = 420;
            }
            //} while (reader.IsClosed == false && reader.NextResult() != null);
            return result;
        }

        List<List<string>> GetSessionResult(int grandClassSectionId, int grandTermId, int grandExamTypeId, int grandStudentId, int branchId, string grandYear)
        {
            var resultds = new DataSet();
            List<List<string>> result = new List<List<string>>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[spSessionResult]
		                                                        @ClassSectionId = " + grandClassSectionId + ","
                                                                        + "@termId = " + grandTermId + ","
                                                                        + "@ExamTypeId = " + grandExamTypeId + ","
                                                                        + "@studentId = " + grandStudentId + ","
                                                                        + "@year = '" + grandYear + "',"
                                                                        + "@BranchId = " + branchId;

                List<Exam> examList = new List<Exam>();
                resultds = examRepo.BuildStudentGrandList(spQuery);
                LogWriter.WriteLog("Session Scalar Result Dataset Count : " + (resultds == null || resultds.Tables == null ? 0 : resultds.Tables.Count));
                for (int i = 0; i < resultds.Tables.Count; i++)
                {
                    DataTable tb = resultds.Tables[i];

                    int chunkSize = GetTableChunkSize(tb);
                    if (examList == null || examList.Count == 0)
                    {
                        for (int chucnkCount = 0; chucnkCount * chunkSize < tb.Rows.Count; chucnkCount++)
                        {
                            int examTypeId = int.Parse(tb.Rows[chucnkCount * chunkSize]["ExamTypeId"].ToString());
                            examList.AddRange(examRepo.GetExamByExamType(examTypeId, grandClassSectionId).OrderBy(x => x.CourseId));
                        }
                    }
                    DataSet ds = convertTabletoDataset(tb, chunkSize, examList);
                    LogWriter.WriteLog("Session Scalar Result Table Count : " + (ds == null || ds.Tables == null ? 0 : ds.Tables.Count));
                    List<string> tableList = new List<string>();
                    for (int k = 0; k < ds.Tables.Count; k++)
                    {
                        tableList.Add(ConvertDataTableToHTML(ds.Tables[k], grandClassSectionId, grandTermId, grandExamTypeId, grandYear, 2));
                    }
                    result.Add(tableList);
                }
                LogWriter.WriteLog("Session Scalar Compiled Result Count " + (result == null ? 0 : result.Count));
                grandErrorCode = 0;
                ViewData["Operation"] = 1;
                Session["GrandResultPosition"] = resultds;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                grandErrorCode = 420;
            }
            //} while (reader.IsClosed == false && reader.NextResult() != null);
            return result;
        }
        
        private DataSet convertStudentTabletoDataset(DataTable dt, int chunkSize, List<Exam> examList)
        {

            DataSet ds = new DataSet();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int tblCount = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string tableName = "StudentResultTable" + dt.Rows[i][1];
                    if (ds.Tables == null || ds.Tables.Count == 0)
                    {
                        ds.Tables.Add(dt.Copy());
                        ds.Tables[tblCount].TableName = tableName;
                        ds.Tables[tblCount].Rows.Clear();
                        tblCount++;
                    }
                    else if (ds.Tables.Contains(tableName) == false)
                    {
                        ds.Tables.Add(dt.Copy());
                        ds.Tables[tblCount].TableName = tableName;
                        ds.Tables[tblCount].Rows.Clear();
                        tblCount++;
                    }
                }
                var startRollNO = dt.Rows[0][4].ToString();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string tableName = "StudentResultTable" + dt.Rows[i][1];
                    ds.Tables[tableName].ImportRow(dt.Rows[i]);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return ds;
        }

        private DataSet convertAccumulativeStudentTabletoDataset(DataTable dt, int chunkSize, List<Exam> examList)
        {

            DataSet ds = new DataSet();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int tblCount = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string tableName = "StudentResultTable" + dt.Rows[i][1];
                    if (ds.Tables == null || ds.Tables.Count == 0)
                    {
                        ds.Tables.Add(dt.Copy());
                        ds.Tables[tblCount].TableName = tableName;
                        ds.Tables[tblCount].Rows.Clear();
                        tblCount++;
                    }
                    else if (ds.Tables.Contains(tableName) == false)
                    {
                        ds.Tables.Add(dt.Copy());
                        ds.Tables[tblCount].TableName = tableName;
                        ds.Tables[tblCount].Rows.Clear();
                        tblCount++;
                    }
                }
                var startRollNO = dt.Rows[0][4].ToString();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string tableName = "StudentResultTable" + dt.Rows[i][1];
                    ds.Tables[tableName].ImportRow(dt.Rows[i]);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return ds;
        }

        private DataSet convertTabletoDataset(DataTable dt, int chunkSize, List<Exam> examList)
        {
            DataSet ds = new DataSet();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                dt.Columns.Add("Total");
                dt.Columns.Add("Grade");
                dt.Columns.Add("Percentage");
                dt.Columns.Add("Position");

                for (int i = 0; i < dt.Rows.Count / chunkSize; i++)
                {
                    ds.Tables.Add(dt.Copy());
                    ds.Tables[i].TableName = "StudentResultTable" + i;
                    ds.Tables[i].Rows.Clear();
                }
                var startRollNO = dt.Rows[0][4].ToString();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    decimal studentTotal = 0;
                    //string startSubject = examList[0].Subject.Name;
                    string startSubject = subjRepo.GetSubjectById((int)examList[0].CourseId).Name;
                    int subjectCount = 0;

                    foreach (Exam ex in examList)
                    {
                        string currSubject = subjRepo.GetSubjectById((int)ex.CourseId).Name;
                        if (currSubject == startSubject && subjectCount > 0)
                            break;
                        decimal marksTemp = 0;
                        if (!dt.Rows[i][currSubject].ToString().Equals(""))
                            marksTemp = Math.Round(decimal.Parse(dt.Rows[i][currSubject].ToString()), 2);
                        studentTotal += marksTemp;
                        subjectCount++;
                    }
                    dt.Rows[i]["Total"] = studentTotal.ToString();
                    ds.Tables[i / chunkSize].ImportRow(dt.Rows[i]);

                }

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable table = ds.Tables[i];
                    DataRow totalMarksRow = table.NewRow();
                    //DataRow passPercentageRow = table.NewRow();
                    DataRow totalPassRow = table.NewRow();
                    DataRow totalFailROw = table.NewRow();
                    DataRow resultPercentageRow = table.NewRow();
                    DataRow seperatorRow = table.NewRow();


                    totalMarksRow["StudentName"] = "Total Marks";
                    //passPercentageRow["StudentName"] = "Pass Percentage";
                    totalPassRow["StudentName"] = "Pass Count";
                    totalFailROw["StudentName"] = "Fail Count";
                    resultPercentageRow["StudentName"] = "Result Percentage";
                    int totalMarks = 0;
                    int examId = 0, termId = 0;

                    if (table.Columns.Contains("ExamTypeName"))
                    {
                        examId = int.Parse(table.Rows[0]["ExamTypeId"].ToString());
                    }
                    else if (table.Columns.Contains("TermName"))
                    {
                        termId = int.Parse(table.Rows[0]["ExamTermId"].ToString());
                    }

                    int termCount = 0, sessionCount = 0;
                    int startSubjectId = (int)examList[0].CourseId;
                    foreach (Exam ex in examList)
                    {
                        int passCount = 0, failCount = 0;
                        var examtype = examRepo.GetExamTypeById((int)ex.ExamTypeId);
                        var examterm = examRepo.GetExamTermById((int)examtype.TermId);
                        string currSubject = subjRepo.GetSubjectById((int)ex.CourseId).Name;
                        if (examId > 0)
                        {
                            if (ex.ExamTypeId == examId)
                            {
                                totalMarksRow[currSubject] = ex.TotalMarks;
                                //passPercentageRow[ex.Subject.Name] = ex.PassPercentage;
                                totalMarks += (int)ex.TotalMarks;

                                for (int m = 0; m < table.Rows.Count; m++)
                                {
                                    string grade = SessionHelper.GetGrade(decimal.Parse(table.Rows[m][currSubject].ToString()), (int)ex.TotalMarks, (int)ex.PassPercentage);
                                    if (grade.Equals("F"))
                                        failCount++;
                                    else
                                        passCount++;
                                }
                                totalPassRow[currSubject] = passCount;
                                totalFailROw[currSubject] = failCount;
                                if ((passCount - failCount) > 0)
                                    resultPercentageRow[currSubject] = ((passCount) * 100) / (passCount + failCount);
                                else
                                    resultPercentageRow[currSubject] = 0;
                            }
                        }
                        else if (termId > 0)
                        {
                            if (ex.CourseId == startSubjectId && termCount > 0)
                                break;
                            if (examterm.Id == termId)
                            {
                                termCount++;
                                int termTotalMarks = (int)examList.Where(x => x.ExamType.ExamTerm.Id == termId && x.CourseId == ex.CourseId).Sum(x => x.TotalMarks);
                                totalMarksRow[currSubject] = termTotalMarks;
                                //passPercentageRow[ex.Subject.Name] = ex.PassPercentage;
                                totalMarks += termTotalMarks;

                                for (int m = 0; m < table.Rows.Count; m++)
                                {
                                    string grade = SessionHelper.GetGrade(decimal.Parse(table.Rows[m][currSubject].ToString()), termTotalMarks, (int)ex.PassPercentage);
                                    if (grade.Equals("F"))
                                        failCount++;
                                    else
                                        passCount++;
                                }
                                totalPassRow[currSubject] = passCount;
                                totalFailROw[currSubject] = failCount;
                                if ((passCount - failCount) > 0)
                                    resultPercentageRow[currSubject] = ((passCount) * 100) / (passCount + failCount);
                                else
                                    resultPercentageRow[currSubject] = 0;
                            }
                        }
                        else
                        {
                            if (ex.CourseId == startSubjectId && sessionCount > 0)
                                break;
                            sessionCount++;
                            int sessionTotalMarks = (int)examList.Where(x => x.CourseId == ex.CourseId).Sum(x => x.TotalMarks);
                            totalMarksRow[currSubject] = sessionTotalMarks;
                            //passPercentageRow[ex.Subject.Name] = ex.PassPercentage;
                            totalMarks += sessionTotalMarks;
                            for (int m = 0; m < table.Rows.Count; m++)
                            {
                                string grade = SessionHelper.GetGrade(decimal.Parse(table.Rows[m][currSubject].ToString()), sessionTotalMarks, (int)ex.PassPercentage);
                                if (grade.Equals("F"))
                                    failCount++;
                                else
                                    passCount++;
                            }
                            totalPassRow[currSubject] = passCount;
                            totalFailROw[currSubject] = failCount;
                            if ((passCount - failCount) > 0)
                                resultPercentageRow[currSubject] = ((passCount - failCount) * 100) / (passCount + failCount);
                            else
                                resultPercentageRow[currSubject] = 0;
                        }
                    }
                    totalMarksRow["Total"] = totalMarks;
                    table.Rows.Add(seperatorRow);
                    table.Rows.Add(totalMarksRow);
                    //table.Rows.Add(passPercentageRow);
                    table.Rows.Add(totalPassRow);
                    table.Rows.Add(totalFailROw);
                    table.Rows.Add(resultPercentageRow);

                    for (int cInt = 0; cInt < table.Rows.Count; cInt++)
                    {
                        if (!table.Rows[cInt]["Total"].ToString().Equals(""))
                        {
                            string grade = SessionHelper.GetGrade(decimal.Parse(table.Rows[cInt]["Total"].ToString()), totalMarks, 50);
                            decimal percentage = decimal.Parse(table.Rows[cInt]["Total"].ToString()) * 100 / totalMarks;
                            table.Rows[cInt]["Grade"] = grade;
                            table.Rows[cInt]["Percentage"] = Math.Round(percentage, 2) + "%";
                            table.Rows[cInt]["Position"] = 0;
                            dt.Rows[cInt]["Grade"] = grade;
                            dt.Rows[cInt]["Percentage"] = Math.Round(percentage, 2) + "%";
                            dt.Rows[cInt]["Position"] = 0;
                        }
                        else
                            break;

                    }


                }

                ds = SessionHelper.GetClassTablePosition(ds);
                //copy position to actual table
                for (int cInt = 0; cInt < ds.Tables[0].Rows.Count; cInt++)
                {
                    if (!ds.Tables[0].Rows[cInt]["Total"].ToString().Equals(""))
                    {
                        dt.Rows[cInt]["Position"] = ds.Tables[0].Rows[cInt]["Position"];
                    }
                    else
                        break;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return ds;
        }

        private DataSet convertAccumulativeTabletoDataset(DataTable dt, int chunkSize, List<Exam> examList)
        {

            DataSet ds = new DataSet();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                dt.Columns.Add("Total");
                dt.Columns.Add("Grade");
                dt.Columns.Add("Percentage");
                dt.Columns.Add("Position");

                for (int i = 0; i < dt.Rows.Count / chunkSize; i++)
                {
                    ds.Tables.Add(dt.Copy());
                    ds.Tables[i].TableName = "StudentResultTable" + i;
                    ds.Tables[i].Rows.Clear();
                }
                var startRollNO = dt.Rows[0][4].ToString();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    decimal studentTotal = 0;
                    //string startSubject = examList[0].Subject.Name;
                    string startSubject = subjRepo.GetSubjectById((int)examList[0].CourseId).Name;
                    int subjectCount = 0;
                    foreach (Exam ex in examList)
                    {
                        string currentSubj = subjRepo.GetSubjectById((int)ex.CourseId).Name;
                        if (currentSubj == startSubject && subjectCount > 0)
                            break;
                        decimal marksTemp = 0;
                        if (!dt.Rows[i][currentSubj].ToString().Equals(""))
                            marksTemp = Math.Round(decimal.Parse(dt.Rows[i][currentSubj].ToString()), 2);
                        studentTotal += marksTemp;
                        subjectCount++;
                    }
                    dt.Rows[i]["Total"] = studentTotal.ToString();
                    ds.Tables[i / chunkSize].ImportRow(dt.Rows[i]);

                }

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable table = ds.Tables[i];
                    DataRow totalMarksRow = table.NewRow();
                    //DataRow passPercentageRow = table.NewRow();
                    DataRow totalPassRow = table.NewRow();
                    DataRow totalFailROw = table.NewRow();
                    DataRow resultPercentageRow = table.NewRow();
                    DataRow seperatorRow = table.NewRow();


                    totalMarksRow["StudentName"] = "Total Marks";
                    //passPercentageRow["StudentName"] = "Pass Percentage";
                    totalPassRow["StudentName"] = "Pass Count";
                    totalFailROw["StudentName"] = "Fail Count";
                    resultPercentageRow["StudentName"] = "Result Percentage";
                    int totalMarks = 0;
                    int examId = 0, termId = 0;

                    if (table.Columns.Contains("ExamTypeName"))
                    {
                        examId = int.Parse(table.Rows[0]["ExamTypeId"].ToString());
                    }
                    else if (table.Columns.Contains("TermName"))
                    {
                        termId = int.Parse(table.Rows[0]["ExamTermId"].ToString());
                    }

                    int termCount = 0, sessionCount = 0;
                    //int startSubjectId = examList[0].Subject.Id;
                    int startSubjectId = (int)examList[0].CourseId;
                    foreach (Exam ex in examList)
                    {
                        int passCount = 0, failCount = 0;
                        var examtype = examRepo.GetExamTypeById((int)ex.ExamTypeId);
                        var examterm = examRepo.GetExamTermById((int)examtype.TermId);
                        string currentSubj = subjRepo.GetSubjectById((int)ex.CourseId).Name;
                        if (examId > 0)
                        {
                            if (ex.ExamTypeId == examId)
                            {
                                totalMarksRow[currentSubj] = examtype.Percent_Of_Total;
                                //passPercentageRow[ex.Subject.Name] = ex.PassPercentage;
                                totalMarks += (int)ex.ExamType.Percent_Of_Total;

                                for (int m = 0; m < table.Rows.Count; m++)
                                {
                                    string grade = SessionHelper.GetGrade(decimal.Parse(table.Rows[m][currentSubj].ToString()), (int)examtype.Percent_Of_Total, (int)ex.PassPercentage);
                                    if (grade.Equals("F"))
                                        failCount++;
                                    else
                                        passCount++;
                                }
                                totalPassRow[currentSubj] = passCount;
                                totalFailROw[currentSubj] = failCount;
                                if ((passCount - failCount) > 0)
                                    resultPercentageRow[currentSubj] = ((passCount) * 100) / (passCount + failCount);
                                else
                                    resultPercentageRow[currentSubj] = 0;
                            }
                        }
                        else if (termId > 0)
                        {
                            if (ex.CourseId == startSubjectId && termCount > 0)
                                break;

                            if (examterm.Id == termId)
                            {
                                termCount++;
                                totalMarksRow[currentSubj] = examterm.Percentage;
                                //passPercentageRow[ex.Subject.Name] = ex.PassPercentage;
                                totalMarks += (int)ex.ExamType.ExamTerm.Percentage;

                                for (int m = 0; m < table.Rows.Count; m++)
                                {
                                    string grade = SessionHelper.GetGrade(decimal.Parse(table.Rows[m][currentSubj].ToString()), (int)examterm.Percentage, (int)ex.PassPercentage);
                                    if (grade.Equals("F"))
                                        failCount++;
                                    else
                                        passCount++;
                                }
                                totalPassRow[currentSubj] = passCount;
                                totalFailROw[currentSubj] = failCount;
                                if ((passCount - failCount) > 0)
                                    resultPercentageRow[currentSubj] = ((passCount) * 100) / (passCount + failCount);
                                else
                                    resultPercentageRow[currentSubj] = 0;
                            }
                        }
                        else
                        {
                            if (ex.CourseId == startSubjectId && sessionCount > 0)
                                break;
                            sessionCount++;
                            totalMarksRow[currentSubj] = "100";
                            //passPercentageRow[ex.Subject.Name] = ex.PassPercentage;
                            totalMarks += 100;
                            for (int m = 0; m < table.Rows.Count; m++)
                            {
                                string grade = SessionHelper.GetGrade(decimal.Parse(table.Rows[m][currentSubj].ToString()), 100, (int)ex.PassPercentage);
                                if (grade.Equals("F"))
                                    failCount++;
                                else
                                    passCount++;
                            }
                            totalPassRow[currentSubj] = passCount;
                            totalFailROw[currentSubj] = failCount;
                            if ((passCount - failCount) > 0)
                                resultPercentageRow[currentSubj] = ((passCount - failCount) * 100) / (passCount + failCount);
                            else
                                resultPercentageRow[currentSubj] = 0;
                        }
                    }
                    totalMarksRow["Total"] = totalMarks;
                    table.Rows.Add(seperatorRow);
                    table.Rows.Add(totalMarksRow);
                    //table.Rows.Add(passPercentageRow);
                    table.Rows.Add(totalPassRow);
                    table.Rows.Add(totalFailROw);
                    table.Rows.Add(resultPercentageRow);

                    for (int cInt = 0; cInt < table.Rows.Count; cInt++)
                    {
                        if (!table.Rows[cInt]["Total"].ToString().Equals(""))
                        {
                            string grade = SessionHelper.GetGrade(decimal.Parse(table.Rows[cInt]["Total"].ToString()), totalMarks, 50);
                            decimal percentage = decimal.Parse(table.Rows[cInt]["Total"].ToString()) * 100 / totalMarks;
                            table.Rows[cInt]["Grade"] = grade;
                            table.Rows[cInt]["Percentage"] = Math.Round(percentage, 2) + "%";
                            table.Rows[cInt]["Position"] = 0;
                            dt.Rows[cInt]["Grade"] = grade;
                            dt.Rows[cInt]["Percentage"] = Math.Round(percentage, 2) + "%";
                            dt.Rows[cInt]["Position"] = 0;
                        }
                        else
                            break;

                    }
                }
                ds = SessionHelper.GetClassTablePosition(ds);

                //copy position to actual table
                for (int cInt = 0; cInt < ds.Tables[0].Rows.Count; cInt++)
                {
                    if (!ds.Tables[0].Rows[cInt]["Total"].ToString().Equals(""))
                    {
                        dt.Rows[cInt]["Position"] = ds.Tables[0].Rows[cInt]["Position"];
                    }
                    else
                        break;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return ds;
        }

        private int GetStudentTableChunkSize(DataTable dt)
        {
            int count = 1;
            var startSubject = dt.Rows[0]["SubjectName"].ToString();
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["SubjectName"].ToString().Equals(startSubject))
                    break;
                count++;
            }
            return count;
        }

        private int GetTableChunkSize(DataTable dt)
        {
            int count = 1;
            var startRollNO = dt.Rows[0]["RollNo"].ToString();
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["RollNo"].ToString().Equals(startRollNO))
                    break;
                count++;
            }
            return count;
        }

        private static string ConvertDataTableToHTML(DataTable dt, int grandClassSectionId, int grandTermId, int grandExamTypeId, string grandYear, int resultTypeId)
        {
            string appInstance = ConfigurationManager.AppSettings["appInstance"];
            if (appInstance.Length > 0)
                appInstance = "/" + appInstance;
            string html = "<div class=" + "x_panel" + "> <div class=" + "sc_panel_header" + ">";

            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                if (dt.Columns.Contains("ExamTypeName"))
                {
                    html += "<label class=" + "sc_panel_label" + ">Exam Result : " + dt.Rows[0]["ExamTypeName"].ToString() + "</label> ";
                }
                else if (dt.Columns.Contains("TermName"))
                {
                    html += "<label class=" + "sc_panel_label" + ">Term Result : " + dt.Rows[0]["TermName"].ToString() + "</label>";
                }
                else
                {
                    html += "<label class=" + "sc_panel_label" + ">Complete Session Result</label> ";
                }

                //html += "<ul class=" + "\"nav navbar-right panel_toolbox\"" + "><li><a class=" + "collapse-link" + "><i class=" + "\"fa fa-chevron-up\"" + "></i></a></li><li><a class=" + "close-link" + "><i class=" + "\"fa fa-close\"" + "></i></a></li></ul>";
                html += "<div class=" + "clearfix" + "></div></div><div class=" + "x_content" + ">";


                while (!dt.Columns[0].ColumnName.Equals("StudentId"))
                {
                    dt.Columns.Remove(dt.Columns[0].ColumnName);
                }

                html += "<div class=" + "form-group" + "> <div class=" + "table-responsive" + "> <table class=" + "\"table table-striped table-bordered\"" + ">";
                //add header row
                html += " <thead> <tr> ";
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string columnName = dt.Columns[i].ColumnName;
                    if (columnName.Equals("StudentId"))
                        columnName = "";
                    else if (columnName.Equals("RollNo"))
                        columnName = "Roll No";
                    else if (columnName.Equals("StudentName"))
                        columnName = "Student Name";
                    if (columnName.Length > 0)
                        html += "<th>" + columnName + "</th>";
                }
                html += "</tr> </thead>";
                html += "<tbody style=" + "background-color:white;color:#2A3F54" + ">";
                //add rows
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    html += "<tr>";
                    for (int j = 1; j < dt.Columns.Count; j++)
                    {
                        //if (dt.Columns[j].ColumnName.Equals("StudentId") && !dt.Rows[i]["StudentName"].ToString().Equals("Total Marks") && !dt.Rows[i]["StudentName"].ToString().Equals("") &&
                        //    !dt.Rows[i]["StudentName"].ToString().Equals("Pass Percentage") && !dt.Rows[i]["StudentName"].ToString().Equals("Pass Count") &&
                        //    !dt.Rows[i]["StudentName"].ToString().Equals("Fail Count") && !dt.Rows[i]["StudentName"].ToString().Equals("Result Percentage"))
                        //    html += "<td>" + "<a href=" + "/ExamResult/StudentGrandResult/" + dt.Rows[i][j].ToString() + "?termId=" + grandTermId + "&&examTypeId=" + grandExamTypeId + "&&classSectionId=" + grandClassSectionId + "&&year=" + grandYear + " style=" + "color:blue;" + ">View</a>" + "</td>";
                        //else
                        //    html += "<td>" + dt.Rows[i][j].ToString() + "</td>";

                        if (j == 2 && !dt.Rows[i]["StudentName"].ToString().Equals("Total Marks") && !dt.Rows[i]["StudentName"].ToString().Equals("") &&
                            !dt.Rows[i]["StudentName"].ToString().Equals("Pass Percentage") && !dt.Rows[i]["StudentName"].ToString().Equals("Pass Count") &&
                            !dt.Rows[i]["StudentName"].ToString().Equals("Fail Count") && !dt.Rows[i]["StudentName"].ToString().Equals("Result Percentage"))
                            html += "<td>" + "<a href=" + appInstance + "/ExamResult/StudentGrandResult/" + dt.Rows[i][0].ToString() + "?termId=" + grandTermId + "&&examTypeId=" + grandExamTypeId + "&&classSectionId=" + grandClassSectionId + "&&year=" + grandYear + "&&resultTypeId=" + resultTypeId + " style=" + "color:blue;" + ", title=" + "View_Student_Result" + ">" + dt.Rows[i]["StudentName"].ToString() + "</a>" + "</td>";
                        else
                            html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                    }
                    html += "</tr>";
                }
                html += "</tbody> </table></div></div></div></div>";
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return html;
        }

        private static string ConvertStudentDataTableToHTML(DataTable dt)
        {
            string html = "<div class=" + "x_panel" + "> <div class=" + "sc_panel_header" + ">";

            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                if (dt.Columns.Contains("ExamTypeName"))
                {
                    html += "<label class=" + "sc_panel_label" + ">Exam Result : " + dt.Rows[0]["ExamTypeName"].ToString() + "</label> ";
                }
                else if (dt.Columns.Contains("TermName"))
                {
                    html += "<label class=" + "sc_panel_label" + ">Term Result : " + dt.Rows[0]["TermName"].ToString() + "</label>";
                }
                else
                {
                    html += "<label class=" + "sc_panel_label" + ">Complete Session Result</label> ";
                }

                //html += "<ul class=" + "\"nav navbar-right panel_toolbox\"" + "><li><a class=" + "collapse-link" + "><i class=" + "\"fa fa-chevron-up\"" + "></i></a></li><li><a class=" + "close-link" + "><i class=" + "\"fa fa-close\"" + "></i></a></li></ul>";
                //html += "<div class=" + "clearfix" + "></div></div>";
                html += "<div class=" + "clearfix" + "></div></div><div class=" + "x_content" + " style=" + "display: block;" + ">";


                while (!dt.Columns[0].ColumnName.Equals("SubjectName"))
                {
                    dt.Columns.Remove(dt.Columns[0].ColumnName);
                }

                html += "<div class=" + "form-group row" + "> <div class=" + "table-responsive" + "> <table class=" + "\"table table-striped table-bordered\"" + ">";
                //add header row
                html += "<thead <tr>";
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string columnName = dt.Columns[i].ColumnName;
                    if (columnName.Equals("SubjectName"))
                        columnName = "Subject Name";
                    else if (columnName.Equals("ObtainedMarks"))
                        columnName = "Obtained Marks";
                    else if (columnName.Equals("Total"))
                        columnName = "Total Marks";

                    html += "<th>" + columnName + "</th>";
                }
                html += "</tr> </thead>";
                html += "<tbody style=" + "background-color:white;color:#2A3F54" + ">";
                //add rows
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    html += "<tr>";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                    }
                    html += "</tr>";
                }
                html += "</tbody> </table></div></div></div></div>";
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return html;
        }

        private ActionResult PrintMarksSheet(int examTypeId, int classSectionId, int courseId, DateTime IssuedDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ExamType examType = examRepo.GetExamTypeById(examTypeId);
                Subject subject = subjRepo.GetSubjectById(courseId);
                ClassSection classSection = classSecRepo.GetClassSectionById(classSectionId);

                SubjectSheet pdf = new SubjectSheet();
                PdfDocument document = pdf.CreatePdf(examTypeId, classSectionId, courseId, branchId, IssuedDate);

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

        private int DeleteMarksSheet(int examTypeId, int classSectionId, int courseId)
        {
            int errorCode = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Exam exam = examRepo.GetExamByExamType(examTypeId, classSectionId, courseId);

                errorCode = examRepo.DeleteExamResult(exam.Id);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 520;
            }
            return errorCode;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMarksSheet(int[] examResultIds, int[] studentIds, float[] ObtMarks, int[] CourseIds, int[] ExamTypeIds, int totalMarks, int IsPrint, int IsDelete, DateTime IssuedDate, int passPercentage = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            bool temp = false;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int studentId = studentIds[0];
                int classSectionId = (int)studentRepo.GetStudentById(studentId).ClassSectionId;
                int examTypeId = ExamTypeIds[0];
                int courseId = CourseIds[0];
                if (IsPrint == 1)
                {
                    temp = true;
                    return PrintMarksSheet(examTypeId, classSectionId, courseId, IssuedDate);
                }

                if (IsDelete == 1)
                {
                    errorCode = DeleteMarksSheet(examTypeId, classSectionId, courseId);
                    return RedirectToAction("Index", new { id = -59 });
                }

                Exam exam = examRepo.GetExamByExamType(examTypeId, classSectionId, courseId);
                if (exam == null)
                {
                    exam = new Exam();
                    exam.ClassSectionId = classSectionId;
                    exam.CourseId = CourseIds[0];
                    exam.ExamTypeId = ExamTypeIds[0];
                    exam.TotalMarks = totalMarks;
                    exam.PassPercentage = passPercentage;
                    examRepo.AddExam(exam);
                }
                else
                {
                    exam.TotalMarks = totalMarks;
                    exam.PassPercentage = passPercentage;
                    examRepo.UpdateExam(exam);
                }

                int count = 0;
                foreach (int id in studentIds)
                {
                    int examResultId = examResultIds[count];
                    ExamResult er = examRepo.GetExamResultById(examResultId);
                    if (er == null)
                    {
                        er = new ExamResult();
                        er.ExamId = exam.Id;
                        er.StudentId = id;
                        er.ObtainedMarks = Convert.ToDecimal(ObtMarks[count]);
                        er.CreatedOn = DateTime.Now;
                        examRepo.AddExamResult(er);
                    }
                    else
                    {
                        er.ObtainedMarks = Convert.ToDecimal(ObtMarks[count]);
                        examRepo.UpdateExamResult(er);
                    }
                    count++;
                }
                errorCode = 2;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                if (temp)
                {
                    errorCode = 2420;
                }
                else
                {
                    errorCode = 420;
                }
            }
            return RedirectToAction("Index", new { id = -59 });
        }

        private List<ExamResultViewModel> SearchClassSheet()
        {
            List<ExamResultViewModel> examResultViewModelList = new List<ExamResultViewModel>();
            IList<ExamResultViewModel> examResultList = null;
            totalExamMarksList = new List<int>();
            examPassPercentageList = new List<int>();
            courseNameList = new List<string>();

            int classExamTypeId = (int)Session[ConstHelper.SEARCH_CLASS_SHEET_EXAM_ID];
            int examClassSectionId = (int)Session[ConstHelper.SEARCH_EXAM_RESULT_CLASS_SECTION_ID];
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                List<Exam> examList = examRepo.GetExamByExamType(classExamTypeId, examClassSectionId);
                if (examList != null && examList.Count > 0)
                {
                    foreach (Exam exam in examList)
                    {
                        examTotalMarks = (int)exam.TotalMarks;
                        examPassPercentage = (int)exam.PassPercentage;
                        totalExamMarksList.Add(examTotalMarks);
                        examPassPercentageList.Add(examPassPercentage);
                        var subject = subjRepo.GetSubjectById((int)exam.CourseId);
                        courseNameList.Add(subject.Name);

                        examResultList = examRepo.GetExamResultModelByExamId(exam.Id);
                        examResultViewModelList.AddRange(examResultList.Where(x => x.LeavingStatus == 1).ToList());
                        
                    }
                }
                
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                classErrorCode = 420;
            }
            examResultViewModelList = SessionHelper.GetClassPosition(examResultViewModelList);
            return examResultViewModelList;
        }

        private IList<ExamResultViewModel> SearchMarksSheet()
        {
            int classSectionId = (int)Session[ConstHelper.SEARCH_EXAM_RESULT_CLASS_SECTION_ID];
            int examTypeId = (int)Session[ConstHelper.SEARCH_EXAM_RESULT_EXAM_ID];
            int sheetSubjectId = (int)Session[ConstHelper.SEARCH_EXAM_RESULT_SUBJECT_ID];

            List<ExamResultViewModel> examResultViewModelList = new List<ExamResultViewModel>();
            IList<ExamResultViewModel> examResultList = null;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Exam exam = examRepo.GetExamByExamType(examTypeId, classSectionId, sheetSubjectId);
                if (exam != null)
                {
                    examTotalMarks = (int)exam.TotalMarks;
                    examPassPercentage = (int)exam.PassPercentage;
                    examRepo.GetNewStudentForExam(exam.Id);
                    examResultList = examRepo.GetExamResultModelByExamId(exam.Id);
                    examResultViewModelList = examResultList.Where(x => x.LeavingStatus == 1).ToList();
                    
                }
                else
                {
                    IList<StudentModel> studentList = studentRepo.GetStudentByClassSectionId(classSectionId);
                    foreach (StudentModel st in studentList)
                    {
                        ExamResultViewModel ervm = new ExamResultViewModel();
                        ervm.Name = st.Name;
                        ervm.Id = 0;
                        ervm.CourseId = sheetSubjectId;
                        ervm.AdmissionNo = st.AdmissionNo;
                        ervm.ExamTypeId = examTypeId;
                        ervm.StudentId = (int)st.Id;
                        ervm.RollNumber = st.RollNumber;
                        examResultViewModelList.Add(ervm);
                    }
                    //Exam newExam = new Exam();
                    //exam.ExamTypeId = examTypeId;
                    //exam.CourseId = sheetSubjectId;
                    //exam.ClassSectionId = classSectionId;

                }
                errorCode = 0;
                LogWriter.WriteLog("Search Record Count : " + (examResultViewModelList == null ? 0 : examResultViewModelList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return examResultViewModelList;
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateClassResultPdf(string teacherRemarks, string[] CourseName, string[] TotalMarks, string[] ObtMarks, string[] Grade)
        //{
        //    if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }

        //    int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
        //    ClassResultPdf pdf = new ClassResultPdf();
        //    int classExamTypeId = (int)Session[ConstHelper.SEARCH_CLASS_SHEET_EXAM_ID];
        //    int examClassSectionId = (int)Session[ConstHelper.SEARCH_EXAM_RESULT_CLASS_SECTION_ID];
        //    PdfDocument document = pdf.CreatePdf(classExamTypeId, examClassSectionId, branchId);
        //    ClassSection clsec = classSecRepo.GetClassSectionById(examClassSectionId);

        //    MemoryStream stream = new MemoryStream();
        //    document.Save(stream, false);
        //    stream.Seek(0, SeekOrigin.Begin);
        //    return File(stream, "application/pdf");

        //    //using (MemoryStream stream = new MemoryStream())
        //    //{
        //    //    document.Save(stream, false);

        //    //    stream.Seek(0, SeekOrigin.Begin);
        //    //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = "Class Sheet : " + clsec.Class.Name + "_" + clsec.Section.Name + "_" + DateTime.Now.ToString() + ".pdf" };
        //    //}  
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateClassResultPdf(string teacherRemarks, string[] CourseName, string[] TotalMarks, string[] ObtMarks, string[] Grade, int[] Position, int[] StudentId, int IsAllStudent, DateTime IssuedDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                PdfDocument document = new PdfDocument();
                int classExamTypeId = (int)Session[ConstHelper.SEARCH_CLASS_SHEET_EXAM_ID];
                int examClassSectionId = (int)Session[ConstHelper.SEARCH_EXAM_RESULT_CLASS_SECTION_ID];
                if (IsAllStudent == 0)
                {
                    ClassResultPdf pdf = new ClassResultPdf();
                    document = pdf.CreatePdf(classExamTypeId, examClassSectionId, branchId, IssuedDate, Position, StudentId);
                    ClassSection clsec = classSecRepo.GetClassSectionById(examClassSectionId);
                }
                else
                {
                    List<Exam> examList = examRepo.GetExamByExamTypeId(classExamTypeId, examClassSectionId);
                    //var studentList = studentRepo.GetStudentByClassSectionId(examClassSectionId);
                    var studentList = examRepo.GetExamResultModelByExamId(examList[0].Id);
                    StudentResultPdf resultPdf = new StudentResultPdf();
                    foreach (var student in studentList)
                    {
                        List<ExamResultViewModel> examResultViewModelList = new List<ExamResultViewModel>();

                        foreach (Exam exam in examList)
                        {
                            ExamResultViewModel er = examRepo.GetExamResultModelByExamAndStudentId(exam.Id, student.StudentId);
                            if (er != null)
                            {
                                er.Grade = SessionHelper.GetGrade((int)er.Obtained, (int)exam.TotalMarks, (int)exam.PassPercentage);
                                examResultViewModelList.Add(er);
                            }
                        }
                        if (IsAllStudent == 1)
                        {
                            document = resultPdf.CreatePdfOfAll(examResultViewModelList, classExamTypeId, document, branchId, examClassSectionId, IssuedDate, Position, StudentId);
                        }
                        if (IsAllStudent == 2)
                        {
                            SmsInfoProxy.sendTermResultToAllStudentEvent(examResultViewModelList, classExamTypeId, IssuedDate, ConstHelper.SMS_EVENT_NAME_EXAM_ALL_STUDENT_TERM_RESULT);
                        }
                    }

                }

                if (IsAllStudent == 1 || IsAllStudent == 0)
                {
                    MemoryStream stream = new MemoryStream();
                    document.Save(stream, false);
                    stream.Seek(0, SeekOrigin.Begin);
                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return File(stream, "application/pdf");
                }
                else
                {
                    classErrorCode = 20;
                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return RedirectToAction("ClassResult", new { id = -59 });
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return RedirectToAction("ClassResult", new { id = -59 });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStudentResultPdf(string teacherRemarks, string[] CourseName, string[] TotalMarks, string[] ObtMarks, string[] Grade, DateTime IssuedDate, int StudentPositon)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int studentResultId = (int)Session[ConstHelper.STUDENT_RESULT_STUDENT_ID];
                int studentExamTypeId = (int)Session[ConstHelper.STUDENT_RESULT_EXAM_ID];
                int classSectionId = (int)Session[ConstHelper.SEARCH_EXAM_RESULT_CLASS_SECTION_ID];
                var clsecMOdel = classSecRepo.GetClassSectionsModelById(classSectionId);
                StudentResultPdf pdf = new StudentResultPdf();
                Student st = studentRepo.GetStudentById(studentResultId);
                ExamType er = examRepo.GetExamTypeById(studentExamTypeId);
                PdfDocument document = pdf.CreatePdf(st, er, teacherRemarks, CourseName, TotalMarks, ObtMarks, Grade, false, er.Percent_Of_Total, IssuedDate, StudentPositon, clsecMOdel.ClassName, clsecMOdel.SectionName);

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
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    document.Save(stream, false);

            //    stream.Seek(0, SeekOrigin.Begin);
            //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = "Student Sheet : " + st.RollNumber + "_" + st.Name + "_" + er.Name + "_" + st.ClassSection.Class.Name + "_" + st.ClassSection.Section.Name + "_" + DateTime.Now.ToString() + ".pdf" };
            //}  
        }

        public ActionResult SendStudentSmsExamResult(string teacherRemarks, DateTime IssuedDate)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int studentResultId = (int)Session[ConstHelper.STUDENT_RESULT_STUDENT_ID];
                int studentExamTypeId = (int)Session[ConstHelper.STUDENT_RESULT_EXAM_ID];

                StudentResultPdf pdf = new StudentResultPdf();
                Student st = studentRepo.GetStudentById(studentResultId);
                ExamType errr = examRepo.GetExamTypeById(studentExamTypeId);


                int classExamTypeId = studentExamTypeId;
                int examClassSectionId = (int)st.ClassSectionId;
                List<Exam> examList = examRepo.GetExamByExamTypeId(classExamTypeId, examClassSectionId);

                List<ExamResultViewModel> examResultViewModelList = new List<ExamResultViewModel>();


                foreach (Exam exam in examList)
                {
                    ExamResult er = examRepo.GetExamResultByExamAndStudentId(exam.Id, st.id);
                    if (er != null)
                    {
                        ExamResultViewModel ervm = new ExamResultViewModel();
                        ervm.Name = er.Student.Name;
                        ervm.FatherName = er.Student.FatherName;
                        ervm.Id = (int)er.id;
                        ervm.StudentId = (int)er.StudentId;
                        ervm.CourseId = sheetSubjectId;
                        ervm.RollNumber = er.Student.RollNumber;
                        ervm.ObtMarks = er.ObtainedMarks.ToString();
                        ervm.totalMarks = exam.TotalMarks.ToString();
                        ervm.CourseName = subjRepo.GetSubjectById((int)exam.CourseId).Name;
                        ervm.Grade = SessionHelper.GetGrade((int)er.ObtainedMarks, (int)exam.TotalMarks, (int)exam.PassPercentage);
                        examResultViewModelList.Add(ervm);
                    }
                }


                StudentResultPdf resultPdf = new StudentResultPdf();
                if (examResultViewModelList != null && examResultViewModelList.Count > 0)
                {
                    SmsInfoProxy.sendTermResultToAllStudentEvent(examResultViewModelList, classExamTypeId, IssuedDate, ConstHelper.SMS_EVENT_NAME_EXAM_STUDENT_TERM_RESULT);
                }
                classErrorCode = 20;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                classErrorCode = 1420;
            }
            return RedirectToAction("ClassResult", new { id = -59 });
        }

        public ActionResult StudentResult(int id, int examTypeId, int position)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            List<ExamResultViewModel> examResultViewModelList = new List<ExamResultViewModel>();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Session[ConstHelper.STUDENT_RESULT_STUDENT_ID] = id;
                Session[ConstHelper.STUDENT_RESULT_EXAM_ID] = examTypeId;
                var studentObj = studentRepo.GetStudentById(id);

                int examClassSectionId = (int)Session[ConstHelper.SEARCH_EXAM_RESULT_CLASS_SECTION_ID];
                List<Exam> examList = examRepo.GetExamByExamTypeId(examTypeId, (int)examClassSectionId);
                ViewData["student"] = studentObj;
                ViewData["classModel"] = classSecRepo.GetClassSectionsModelById((int)examClassSectionId);
                ViewData["examType"] = examRepo.GetExamTypeById(examTypeId);
                ViewData["position"] = position;

                foreach (Exam exam in examList)
                {
                    ExamResult er = examRepo.GetExamResultByExamAndStudentId(exam.Id, id);
                    if (er != null)
                    {
                        ExamResultViewModel ervm = new ExamResultViewModel();
                        ervm.Name = er.Student.Name;
                        ervm.FatherName = er.Student.FatherName;
                        ervm.Id = (int)er.id;
                        ervm.StudentId = (int)er.StudentId;
                        ervm.CourseId = sheetSubjectId;
                        ervm.RollNumber = er.Student.RollNumber;
                        ervm.ObtMarks = Math.Round((decimal)er.ObtainedMarks, 2).ToString();
                        ervm.totalMarks = exam.TotalMarks.ToString();
                        ervm.CourseName = subjRepo.GetSubjectById((int)exam.CourseId).Name;
                        ervm.Grade = SessionHelper.GetGrade((int)er.ObtainedMarks, (int)exam.TotalMarks, (int)exam.PassPercentage);
                        examResultViewModelList.Add(ervm);
                    }
                }

                GetExamSysConfig();
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(examResultViewModelList);
        }

        public ActionResult CreatePdfOfGrandResult(DateTime IssuedDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            ResultModel model = new ResultModel();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                int grandClassSectionId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_CLASS_SECTION_ID];
                int grandTermId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_TERM_ID];
                int grandExamTypeId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_EXAM_ID];
                int grandStudentId = (int)Session[ConstHelper.SEARCH_GRAND_STUDENT_ID];
                string grandYear = (string)Session[ConstHelper.SEARCH_GRAND_SHEET_YEAR];
                int resultTypeId = (int)Session[ConstHelper.SEARCH_GRAND_RESULT_TYPE];


                if (resultTypeId == 2)
                {
                    model = GetSessionResult(grandClassSectionId, grandTermId, grandExamTypeId, grandStudentId, grandYear, branchId);
                }
                else
                {
                    model = GetSessionResultAccumulative(grandClassSectionId, grandTermId, grandExamTypeId, grandStudentId, grandYear, branchId);
                }

                List<string> subjectList = new List<string>();
                ClassGrandResultPdf pdf = new ClassGrandResultPdf();
                //string startSubject = model.examList[0].Subject.Name;
                string startSubject = subjRepo.GetSubjectById((int)model.examList[0].CourseId).Name;
                int subjCount = 0;
                foreach (Exam ex in model.examList)
                {
                    string currSubject = subjRepo.GetSubjectById((int)ex.CourseId).Name;
                    if (currSubject == startSubject && subjCount > 0)
                        break;
                    subjCount++;
                    subjectList.Add(currSubject);
                }
                ClassSectionModel clsec = classSecRepo.GetClassSectionsModelById(grandClassSectionId);
                string className = clsec.ClassName;
                string sectionName = clsec.SectionName;
                subjectList = subjectList.OrderBy(x => x).Distinct().ToList();

                PdfDocument document = pdf.CreatePdf(className, sectionName, subjectList, model.DatasetList, branchId, IssuedDate);

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

        private ResultModel GetSessionResult(int grandClassSectionId, int grandTermId, int grandExamTypeId, int grandStudentId, string grandYear, int branchId)
        {
            ResultModel model = new ResultModel();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DataSet resultds = new DataSet();
                var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[spSessionResult]
		                                                        @ClassSectionId = " + grandClassSectionId + ","
                                                                            + "@termId = " + grandTermId + ","
                                                                            + "@ExamTypeId = " + grandExamTypeId + ","
                                                                            + "@studentId = " + grandStudentId + ","
                                                                            + "@year = '" + grandYear + "',"
                                                                            + "@BranchId = " + branchId;
                resultds = examRepo.BuildStudentGrandList(spQuery);
                LogWriter.WriteLog("Result Model Session Result Dataset Count : " + (resultds == null || resultds.Tables == null ? 0 : resultds.Tables.Count));

                List<Exam> examList = new List<Exam>();
                List<DataSet> dsList = new List<DataSet>();

                for (int i = 0; i < resultds.Tables.Count; i++)
                {
                    DataTable tb = resultds.Tables[i];

                    int chunkSize = GetTableChunkSize(tb);
                    if (examList == null || examList.Count == 0)
                    {
                        for (int chucnkCount = 0; chucnkCount * chunkSize < tb.Rows.Count; chucnkCount++)
                        {
                            int examTypeId = int.Parse(tb.Rows[chucnkCount * chunkSize]["ExamTypeId"].ToString());
                            examList.AddRange(examRepo.GetExamByExamType(examTypeId, grandClassSectionId).OrderBy(x => x.CourseId));
                        }
                    }
                    DataSet ds = convertTabletoDataset(tb, chunkSize, examList);
                    dsList.Add(ds);
                }

                model.DatasetList = dsList;
                model.examList = examList;
                LogWriter.WriteLog("Result Model Session Result Table Count : " + (dsList == null ? 0 : dsList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return model;
        }

        private ResultModel GetSessionResultAccumulative(int grandClassSectionId, int grandTermId, int grandExamTypeId, int grandStudentId, string grandYear, int branchId)
        {
            ResultModel model = new ResultModel();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DataSet resultds = new DataSet();
                var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[spSessionResultAccumulative]
		                                                        @ClassSectionId = " + grandClassSectionId + ","
                                                                            + "@termId = " + grandTermId + ","
                                                                            + "@ExamTypeId = " + grandExamTypeId + ","
                                                                            + "@studentId = " + grandStudentId + ","
                                                                            + "@year = '" + grandYear + "',"
                                                                            + "@BranchId = " + branchId;
                resultds = examRepo.BuildStudentGrandList(spQuery);
                LogWriter.WriteLog("Result Model Session Accumulative Result Dataset Count : " + (resultds == null || resultds.Tables == null ? 0 : resultds.Tables.Count));

                List<Exam> examList = new List<Exam>();
                List<DataSet> dsList = new List<DataSet>();

                for (int i = 0; i < resultds.Tables.Count; i++)
                {
                    DataTable tb = resultds.Tables[i];

                    int chunkSize = GetTableChunkSize(tb);
                    if (examList == null || examList.Count == 0)
                    {
                        for (int chucnkCount = 0; chucnkCount * chunkSize < tb.Rows.Count; chucnkCount++)
                        {
                            int examTypeId = int.Parse(tb.Rows[chucnkCount * chunkSize]["ExamTypeId"].ToString());
                            examList.AddRange(examRepo.GetExamByExamType(examTypeId, grandClassSectionId).OrderBy(x => x.CourseId));
                        }
                    }
                    DataSet ds = convertTabletoDataset(tb, chunkSize, examList);
                    dsList.Add(ds);
                }

                model.DatasetList = dsList;
                model.examList = examList;
                LogWriter.WriteLog("Result Model Session Accumulative Result Table Count : " + (dsList == null ? 0 : dsList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return model;
        }

        public ActionResult StudentGrandResult(int id, int termId, int examTypeId, int classSectionId, string year, int resultTypeId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            List<string> result = new List<string>();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Session[ConstHelper.STUDENT_CLASS_SECTION_ID] = classSectionId;
                Session[ConstHelper.STUDENT_RESULT_STUDENT_ID] = id;
                Session[ConstHelper.STUDENT_RESULT_YEAR] = year;
                Session[ConstHelper.STUDENT_RESULT_TERM_ID] = termId;
                Session[ConstHelper.STUDENT_RESULT_EXAM_ID] = examTypeId;
                var student = studentRepo.GetStudentById(id);
                ViewData["student"] = student;
                ViewData["classModel"] = classSecRepo.GetClassSectionsModelById((int)classSectionId);
                //ViewData["examType"] = db.ExamTypes.Find(examTypeId);

                if (resultTypeId == 1)
                {
                    LogWriter.WriteLog("Compiling Accumulative Student Grand result");
                    result = GetAccumulativeStudentResut(classSectionId, id, year, termId, examTypeId);
                }
                else
                {
                    LogWriter.WriteLog("Compiling Scalar Student Grand result");
                    result = GetStudentResut(classSectionId, id, year, termId, examTypeId);
                }

                GetExamSysConfig();
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(result);
        }

        List<string> GetAccumulativeStudentResut(int classSectionId, int id, string year, int termId, int examTypeId)
        {
            var resultds = new DataSet();
            List<string> result = new List<string>();
            try
            {

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[spStudentResultAccumulative]
		                                                        @ClassSectionId = " + classSectionId + ","
                                                                        + "@termId = " + termId + ","
                                                                        + "@ExamTypeId = " + examTypeId + ","
                                                                        + "@studentId = " + id + ","
                                                                        + "@year = '" + year + "'";

                resultds = examRepo.BuildStudentGrandList(spQuery);
                LogWriter.WriteLog("Accumulative Student Result Dataset Count : " + (resultds == null || resultds.Tables == null ? 0 : resultds.Tables.Count));
                List<Exam> examList = null;

                for (int i = 0; i < resultds.Tables.Count; i++)
                {
                    DataTable tb = resultds.Tables[i];
                    if (examList == null)
                    {
                        int examTypeIdTemp = int.Parse(tb.Rows[0]["ExamTypeId"].ToString());
                        examList = examRepo.GetExamByExamType(examTypeIdTemp, classSectionId);
                    }
                    int chunkSIze = GetStudentTableChunkSize(tb);
                    DataSet ds = convertAccumulativeStudentTabletoDataset(tb, chunkSIze, examList);
                    for (int k = 0; k < ds.Tables.Count; k++)
                    {
                        DataTable table = AddAccumulativeGradeAndTotalToTable(ds.Tables[k]);
                        if (table.Columns.Contains("TotalMarks"))
                            table.Columns.Remove("TotalMarks");
                        result.Add(ConvertStudentDataTableToHTML(table));
                    }
                }
                ViewData["Error"] = 0;
                LogWriter.WriteLog("Accumulative Student Result Table Count : " + (result == null ? 0 : result.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                ViewData["Error"] = 420;
            }
            return result;
        }

        List<DataSet> GetAccumulativePDFStudentResut(int classSectionId, int id, string year, int termId, int examTypeId)
        {
            var resultds = new DataSet();
            List<DataSet> dsList = new List<DataSet>();
            try
            {

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[spStudentResultAccumulative]
		                                                        @ClassSectionId = " + classSectionId + ","
                                                                        + "@termId = " + termId + ","
                                                                        + "@ExamTypeId = " + examTypeId + ","
                                                                        + "@studentId = " + id + ","
                                                                        + "@year = '" + year + "'";

                resultds = examRepo.BuildStudentGrandList(spQuery);
                LogWriter.WriteLog("Accumulative Pdf Student Result Dataset Count : " + (resultds == null || resultds.Tables == null ? 0 : resultds.Tables.Count));
                List<Exam> examList = null;

                for (int i = 0; i < resultds.Tables.Count; i++)
                {
                    DataTable tb = resultds.Tables[i];
                    if (examList == null)
                    {
                        int examTypeIdTemp = int.Parse(tb.Rows[0]["ExamTypeId"].ToString());
                        examList = examRepo.GetExamByExamType(examTypeIdTemp, classSectionId);
                    }
                    int chunkSIze = GetStudentTableChunkSize(tb);
                    DataSet ds = convertAccumulativeStudentTabletoDataset(tb, chunkSIze, examList);
                    for (int k = 0; k < ds.Tables.Count; k++)
                    {
                        DataTable table = AddAccumulativeGradeAndTotalToTable(ds.Tables[k]);
                        //table.Rows.RemoveAt(table.Rows.Count - 1);
                    }
                    dsList.Add(ds);
                }
                ViewData["Error"] = 0;
                LogWriter.WriteLog("Accumulative Pdf Student Result Table Count : " + (dsList == null ? 0 : dsList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                ViewData["Error"] = 420;
            }
            return dsList;
        }

        List<string> GetStudentResut(int classSectionId, int id, string year, int termId, int examTypeId)
        {
            var resultds = new DataSet();
            List<string> result = new List<string>();
            try
            {

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[spStudentResult]
		                                                        @ClassSectionId = " + classSectionId + ","
                                                                        + "@termId = " + termId + ","
                                                                        + "@ExamTypeId = " + examTypeId + ","
                                                                        + "@studentId = " + id + ","
                                                                        + "@year = '" + year + "'";

                resultds = examRepo.BuildStudentGrandList(spQuery);
                LogWriter.WriteLog("The Student Result Dataset Count : " + (resultds == null || resultds.Tables == null ? 0 : resultds.Tables.Count));
                List<Exam> examList = null;

                for (int i = 0; i < resultds.Tables.Count; i++)
                {
                    DataTable tb = resultds.Tables[i];
                    if (examList == null)
                    {
                        int examTypeIdTemp = int.Parse(tb.Rows[0]["ExamTypeId"].ToString());
                        examList = examRepo.GetExamByExamType(examTypeIdTemp, classSectionId);
                    }
                    int chunkSIze = GetStudentTableChunkSize(tb);
                    DataSet ds = convertStudentTabletoDataset(tb, chunkSIze, examList);
                    for (int k = 0; k < ds.Tables.Count; k++)
                    {
                        DataTable table = AddGradeAndTotalToTable(ds.Tables[k]);
                        result.Add(ConvertStudentDataTableToHTML(table));
                    }
                }
                ViewData["Error"] = 0;
                LogWriter.WriteLog("The Student Result Table Count : " + (result == null ? 0 : result.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                ViewData["Error"] = 420;
            }
            return result;
        }

        List<DataSet> GetPDFStudentResut(int classSectionId, int id, string year, int termId, int examTypeId)
        {
            var resultds = new DataSet();
            List<DataSet> dsList = new List<DataSet>();
            try
            {

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[spStudentResult]
		                                                        @ClassSectionId = " + classSectionId + ","
                                                                        + "@termId = " + termId + ","
                                                                        + "@ExamTypeId = " + examTypeId + ","
                                                                        + "@studentId = " + id + ","
                                                                        + "@year = '" + year + "'";

                resultds = examRepo.BuildStudentGrandList(spQuery);
                LogWriter.WriteLog("The Student Pdf Result Dataset Count : " + (resultds == null || resultds.Tables == null ? 0 : resultds.Tables.Count));
                List<Exam> examList = null;

                for (int i = 0; i < resultds.Tables.Count; i++)
                {
                    DataTable tb = resultds.Tables[i];
                    if (examList == null)
                    {
                        int examTypeIdTemp = int.Parse(tb.Rows[0]["ExamTypeId"].ToString());
                        examList = examRepo.GetExamByExamType(examTypeIdTemp, classSectionId);
                    }
                    int chunkSIze = GetStudentTableChunkSize(tb);
                    DataSet ds = convertStudentTabletoDataset(tb, chunkSIze, examList);
                    for (int k = 0; k < ds.Tables.Count; k++)
                    {
                        DataTable table = AddGradeAndTotalToTable(ds.Tables[k]);
                        //table.Rows.RemoveAt(table.Rows.Count - 1);
                    }
                    dsList.Add(ds);
                }
                ViewData["Error"] = 0;
                LogWriter.WriteLog("The Student Pdf Result Table Count : " + (dsList == null ? 0 : dsList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                ViewData["Error"] = 420;
            }
            return dsList;
        }

        public ActionResult CreatePdfStudentGrandResult(string teacherRemarks, int AutoRemarks, DateTime IssuedDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int studentClassSectionId = (int)Session[ConstHelper.STUDENT_CLASS_SECTION_ID];
                int studentGrandResultId = (int)Session[ConstHelper.STUDENT_RESULT_STUDENT_ID];
                string studentYear = (string)Session[ConstHelper.STUDENT_RESULT_YEAR];
                int studentTermId = (int)Session[ConstHelper.STUDENT_RESULT_TERM_ID];
                int studentGrandExamTypeId = (int)Session[ConstHelper.STUDENT_RESULT_EXAM_ID];
                int resultTypeId = (int)Session[ConstHelper.SEARCH_GRAND_RESULT_TYPE];

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                StudentModel st = studentRepo.GetStudentModelByStudentId(studentGrandResultId);
                ClassSectionModel clsec = classSecRepo.GetClassSectionsModelById(studentClassSectionId);
                string className = clsec.ClassName;
                string sectionName = clsec.SectionName;

                List<DataSet> dsList = new List<DataSet>();
                if (resultTypeId == 1)
                {
                    LogWriter.WriteLog("Compiling Accumulative Student Pdf Grand result");
                    dsList = GetAccumulativePDFStudentResut(studentClassSectionId, studentGrandResultId, studentYear, studentTermId, studentGrandExamTypeId);
                }
                else
                {
                    LogWriter.WriteLog("Compiling Scalar Student Pdf Grand result");
                    dsList = GetPDFStudentResut(studentClassSectionId, studentGrandResultId, studentYear, studentTermId, studentGrandExamTypeId);
                }

                StudentGrandResultPdf pdf = new StudentGrandResultPdf();
                PdfDocument document = pdf.CreatePdf(st, className, sectionName, teacherRemarks, dsList, branchId, AutoRemarks, IssuedDate);

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

        public ActionResult CreatePdfStudentDetailTranscript(string teacherRemarks, int AutoRemarks, DateTime IssuedDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int studentClassSectionId = (int)Session[ConstHelper.STUDENT_CLASS_SECTION_ID];
                int studentGrandResultId = (int)Session[ConstHelper.STUDENT_RESULT_STUDENT_ID];
                string studentYear = (string)Session[ConstHelper.STUDENT_RESULT_YEAR];
                int studentTermId = (int)Session[ConstHelper.STUDENT_RESULT_TERM_ID];
                int studentGrandExamTypeId = (int)Session[ConstHelper.STUDENT_RESULT_EXAM_ID];
                int resultTypeId = (int)Session[ConstHelper.SEARCH_GRAND_RESULT_TYPE];

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                StudentModel st = studentRepo.GetStudentModelByStudentId(studentGrandResultId);
                ClassSectionModel clsec = classSecRepo.GetClassSectionsModelById(studentClassSectionId);
                string className = clsec.ClassName;
                string sectionName = clsec.SectionName;

                List<DataSet> dsList = new List<DataSet>();
                if (resultTypeId == 1)
                {
                    LogWriter.WriteLog("Compiling Accumulative Student Detail Transcript Pdf Grand result");
                    dsList = GetAccumulativePDFStudentResut(studentClassSectionId, studentGrandResultId, studentYear, studentTermId, studentGrandExamTypeId);
                }
                else
                {
                    LogWriter.WriteLog("Compiling Scalar Student Detail Transcript Pdf Grand result");
                    dsList = GetPDFStudentResut(studentClassSectionId, studentGrandResultId, studentYear, studentTermId, studentGrandExamTypeId);
                }

                List<ActivityMarksViewModel> activitiesList = examRepo.GetActivityMarksModelByStudentId(st.Id);
                PdfDocument document = new PdfDocument(); //pdf.CreatePdf(st.RollNumber, st.Name, className, sectionName, teacherRemarks, dsList, branchId, AutoRemarks);
                DetailStudentTranscript pdf = new DetailStudentTranscript();
                document = pdf.CreatePdf(st, className, sectionName, "", dsList, branchId, AutoRemarks, document, activitiesList, IssuedDate);


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

        public ActionResult SendSmsStudentDetailTranscript(string teacherRemarks, int AutoRemarks, DateTime IssuedDate)
        {
            try
            {
                if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                {
                    return RedirectToAction("Index", "Login");
                }

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int studentClassSectionId = (int)Session[ConstHelper.STUDENT_CLASS_SECTION_ID];
                int studentGrandResultId = (int)Session[ConstHelper.STUDENT_RESULT_STUDENT_ID];
                string studentYear = (string)Session[ConstHelper.STUDENT_RESULT_YEAR];
                int studentTermId = (int)Session[ConstHelper.STUDENT_RESULT_TERM_ID];
                int studentGrandExamTypeId = (int)Session[ConstHelper.STUDENT_RESULT_EXAM_ID];
                int resultTypeId = (int)Session[ConstHelper.SEARCH_GRAND_RESULT_TYPE];

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                StudentModel st = studentRepo.GetStudentModelByStudentId(studentGrandResultId);
                ClassSectionModel clsec = classSecRepo.GetClassSectionsModelById(studentClassSectionId);
                string className = clsec.ClassName;
                string sectionName = clsec.SectionName;

                List<DataSet> dsList = new List<DataSet>();
                if (resultTypeId == 1)
                {
                    LogWriter.WriteLog("Compiling Accumulative Student Detail Transcript Sms Grand result");
                    dsList = GetAccumulativePDFStudentResut(studentClassSectionId, studentGrandResultId, studentYear, studentTermId, studentGrandExamTypeId);
                }
                else
                {
                    LogWriter.WriteLog("Compiling Scalar Student Detail Transcript Sms Grand result");
                    dsList = GetPDFStudentResut(studentClassSectionId, studentGrandResultId, studentYear, studentTermId, studentGrandExamTypeId);
                }

                List<ActivityMarksViewModel> activitiesList = examRepo.GetActivityMarksModelByStudentId(st.Id);
                if (dsList != null && dsList.Count > 0)
                {
                    SmsInfoProxy.sendSessionResultToAllStudentEvent(st, dsList, AutoRemarks, "", IssuedDate, ConstHelper.SMS_EVENT_NAME_EXAM_STUDENT_SESSION_RESULT);
                }
                grandErrorCode = 20;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                grandErrorCode = 1420;
            }
            return RedirectToAction("GrandResult", new { id = -59 });
        }

        public ActionResult CreatePdfStudentTranscript(string teacherRemarks, int AutoRemarks, DateTime IssuedDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int studentClassSectionId = (int)Session[ConstHelper.STUDENT_CLASS_SECTION_ID];
                int studentGrandResultId = (int)Session[ConstHelper.STUDENT_RESULT_STUDENT_ID];
                string studentYear = (string)Session[ConstHelper.STUDENT_RESULT_YEAR];
                int studentTermId = (int)Session[ConstHelper.STUDENT_RESULT_TERM_ID];
                int studentGrandExamTypeId = (int)Session[ConstHelper.STUDENT_RESULT_EXAM_ID];
                int resultTypeId = (int)Session[ConstHelper.SEARCH_GRAND_RESULT_TYPE];

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                StudentModel st = studentRepo.GetStudentModelByStudentId(studentGrandResultId);
                ClassSectionModel clsec = classSecRepo.GetClassSectionsModelById(studentClassSectionId);
                string className = clsec.ClassName;
                string sectionName = clsec.SectionName;

                DataSet resultds = new DataSet();
                var spQuery = "";
                if (resultTypeId == 2)
                {
                    spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[spTranscriptResult]
		                                                        @ClassSectionId = " + studentClassSectionId + ","
                                                                        + "@termId = " + studentTermId + ","
                                                                        + "@ExamTypeId = " + studentGrandExamTypeId + ","
                                                                        + "@studentId = " + st.Id + ","
                                                                        + "@year = '" + studentYear + "'";
                }
                else
                {
                    spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[spTranscriptResultAccumulative]
		                                                        @ClassSectionId = " + studentClassSectionId + ","
                                                                        + "@termId = " + studentTermId + ","
                                                                        + "@ExamTypeId = " + studentGrandExamTypeId + ","
                                                                        + "@studentId = " + st.Id + ","
                                                                        + "@year = '" + studentYear + "'";
                }

                resultds = examRepo.BuildStudentGrandList(spQuery);
                LogWriter.WriteLog("Pdf Student Transcript Result Dataset Count : " + (resultds == null || resultds.Tables == null ? 0 : resultds.Tables.Count));
                List<ActivityMarksViewModel> activitiesList = examRepo.GetActivityMarksModelByStudentId(st.Id);
                PdfDocument document = new PdfDocument();
                StudentTranscriptResult pdf = new StudentTranscriptResult();
                DataSet ds = (DataSet)Session["GrandResultPosition"];
                var rows = ds.Tables[ds.Tables.Count - 1].AsEnumerable().Where(dr => dr.Field<int>("StudentId") == st.Id);
                int position = int.Parse(rows.ElementAt(0)["Position"].ToString());
                document = pdf.CreatePdf(st, className, sectionName, "", resultds, document, branchId, activitiesList, IssuedDate, position);

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

        public ActionResult CreatePdfOfAllStudents(int AutoRemarks, DateTime IssuedDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                int grandClassSectionId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_CLASS_SECTION_ID];
                int grandTermId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_TERM_ID];
                int grandExamTypeId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_EXAM_ID];
                int grandStudentId = (int)Session[ConstHelper.SEARCH_GRAND_STUDENT_ID];
                string grandYear = (string)Session[ConstHelper.SEARCH_GRAND_SHEET_YEAR];
                int resultTypeId = (int)Session[ConstHelper.SEARCH_GRAND_RESULT_TYPE];

                var studentList = studentRepo.GetStudentByClassSectionIdAndExamYear(grandClassSectionId, int.Parse(grandYear));
                PdfDocument[] docList = new PdfDocument[studentList.Count];

                ClassSectionModel clsec = classSecRepo.GetClassSectionsModelById(grandClassSectionId);
                string className = clsec.ClassName;
                string sectionName = clsec.SectionName;

                int studentCOunt = 0;
                foreach (StudentModel st in studentList)
                {
                    List<DataSet> dsList = new List<DataSet>();
                    if (resultTypeId == 1)
                    {
                        LogWriter.WriteLog("Compiling Accumulative All Students Pdf Grand result");
                        dsList = GetAccumulativePDFStudentResut(grandClassSectionId, st.Id, grandYear, grandTermId, grandExamTypeId);
                    }
                    else
                    {
                        LogWriter.WriteLog("Compiling Scalar All Students Pdf Grand result");
                        dsList = GetPDFStudentResut(grandClassSectionId, st.Id, grandYear, grandTermId, grandExamTypeId);
                    }

                    if (dsList.Count > 0)
                    {
                        StudentGrandResultPdf pdf = new StudentGrandResultPdf();
                        docList[studentCOunt] = pdf.CreatePdf(st, className, sectionName, "", dsList, branchId, AutoRemarks, IssuedDate);
                        studentCOunt++;
                    }
                }

                using (var compressedFileStream = new MemoryStream())
                {
                    //Create an archive and store the stream in memory.
                    using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update, false))
                    {
                        for (int i = 0; i < studentCOunt; i++)
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                docList[i].Save(stream, false);
                                //stream.Position = 0;
                                stream.Seek(0, SeekOrigin.Begin);

                                var zipEntry = zipArchive.CreateEntry(studentList[i].RollNumber + "-" + studentList[i].Name + ".pdf");

                                using (var zipEntryStream = zipEntry.Open())
                                {
                                    stream.CopyTo(zipEntryStream);
                                }
                            }
                        }
                    }

                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = "StudentGrandResult_" + className + "_" + sectionName + DateTime.Now.ToString() + ".zip" };
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }

		public ActionResult SendSmsToAllStudents(int AutoRemarks, DateTime IssuedDate)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                {
                    return RedirectToAction("Index", "Login");
                }

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                int grandClassSectionId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_CLASS_SECTION_ID];
                int grandTermId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_TERM_ID];
                int grandExamTypeId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_EXAM_ID];
                int grandStudentId = (int)Session[ConstHelper.SEARCH_GRAND_STUDENT_ID];
                string grandYear = (string)Session[ConstHelper.SEARCH_GRAND_SHEET_YEAR];
                int resultTypeId = (int)Session[ConstHelper.SEARCH_GRAND_RESULT_TYPE];

                var studentList = studentRepo.GetStudentByClassSectionIdAndExamYear(grandClassSectionId, int.Parse(grandYear));
                PdfDocument[] docList = new PdfDocument[studentList.Count];

                ClassSectionModel clsec = classSecRepo.GetClassSectionsModelById(grandClassSectionId);
                string className = clsec.ClassName;
                string sectionName = clsec.SectionName;

                int studentCOunt = 0;
                foreach (StudentModel st in studentList)
                {
                    List<DataSet> dsList = new List<DataSet>();
                    if (resultTypeId == 1)
                    {
                        dsList = GetAccumulativePDFStudentResut(grandClassSectionId, st.Id, grandYear, grandTermId, grandExamTypeId);
                    }
                    else
                    {
                        dsList = GetPDFStudentResut(grandClassSectionId, st.Id, grandYear, grandTermId, grandExamTypeId);
                    }

                    List<ActivityMarksViewModel> activitiesList = examRepo.GetActivityMarksModelByStudentId(st.Id);
                    if (dsList != null && dsList.Count > 0)
                    {
                        SmsInfoProxy.sendSessionResultToAllStudentEvent(st, dsList, AutoRemarks, "", IssuedDate, ConstHelper.SMS_EVENT_NAME_EXAM_ALL_STUDENT_SESSION_RESULT);
                    }
                    studentCOunt++;
                }
                grandErrorCode = 20;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                grandErrorCode = 1420;
            }
            return RedirectToAction("GrandResult", new { id = -59 });
        }

        public ActionResult CreateDetailTranscriptOfAllStudents(int AutoRemarks, DateTime IssuedDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                int grandClassSectionId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_CLASS_SECTION_ID];
                int grandTermId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_TERM_ID];
                int grandExamTypeId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_EXAM_ID];
                int grandStudentId = (int)Session[ConstHelper.SEARCH_GRAND_STUDENT_ID];
                string grandYear = (string)Session[ConstHelper.SEARCH_GRAND_SHEET_YEAR];
                int resultTypeId = (int)Session[ConstHelper.SEARCH_GRAND_RESULT_TYPE];

                var studentList = studentRepo.GetStudentByClassSectionIdAndExamYear(grandClassSectionId, int.Parse(grandYear));
                PdfDocument[] docList = new PdfDocument[studentList.Count];

                ClassSectionModel clsec = classSecRepo.GetClassSectionsModelById(grandClassSectionId);
                string className = clsec.ClassName;
                string sectionName = clsec.SectionName;

                PdfDocument doc = new PdfDocument();
                int studentCOunt = 0;
                foreach (StudentModel st in studentList)
                {
                    //if (studentCOunt > 0)
                    //    break;
                    List<DataSet> dsList = new List<DataSet>();
                    if (resultTypeId == 1)
                    {
                        LogWriter.WriteLog("Compiling Accumulative Detail Transcript Of All Students");
                        dsList = GetAccumulativePDFStudentResut(grandClassSectionId, st.Id, grandYear, grandTermId, grandExamTypeId);
                    }
                    else
                    {
                        LogWriter.WriteLog("Compiling Scalar Detail Transcript Of All Students");
                        dsList = GetPDFStudentResut(grandClassSectionId, st.Id, grandYear, grandTermId, grandExamTypeId);
                    }
                    List<ActivityMarksViewModel> activitiesList = examRepo.GetActivityMarksModelByStudentId(st.Id);
                    if (dsList.Count > 0)
                    {
                        DetailStudentTranscript pdf = new DetailStudentTranscript();
                        doc = pdf.CreatePdf(st, className, sectionName, "", dsList, branchId, AutoRemarks, doc, activitiesList, IssuedDate);
                    }
                    studentCOunt++;
                }

                MemoryStream stream = new MemoryStream();
                doc.Save(stream, false);
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




        public ActionResult CreateTranscriptOfAllStudents(DateTime IssuedDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                int grandClassSectionId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_CLASS_SECTION_ID];
                int grandTermId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_TERM_ID];
                int grandExamTypeId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_EXAM_ID];
                int grandStudentId = (int)Session[ConstHelper.SEARCH_GRAND_STUDENT_ID];
                string grandYear = (string)Session[ConstHelper.SEARCH_GRAND_SHEET_YEAR];
                int resultTypeId = (int)Session[ConstHelper.SEARCH_GRAND_RESULT_TYPE];

                var studentList = studentRepo.GetStudentByClassSectionIdAndExamYear(grandClassSectionId, int.Parse(grandYear));
                PdfDocument[] docList = new PdfDocument[studentList.Count];

                ClassSectionModel clsec = classSecRepo.GetClassSectionsModelById(grandClassSectionId);
                string className = clsec.ClassName;
                string sectionName = clsec.SectionName;

                int studentCOunt = 0;
                PdfDocument doc = new PdfDocument();
                foreach (StudentModel st in studentList)
                {
                    DataSet resultds = new DataSet();
                    var spQuery = "";
                    if (resultTypeId == 2)
                    {
                        spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[spTranscriptResult]
		                                                        @ClassSectionId = " + grandClassSectionId + ","
                                                                            + "@termId = " + grandTermId + ","
                                                                            + "@ExamTypeId = " + grandExamTypeId + ","
                                                                            + "@studentId = " + st.Id + ","
                                                                            + "@year = '" + grandYear + "'";
                    }
                    else
                    {
                        spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[spTranscriptResultAccumulative]
		                                                        @ClassSectionId = " + grandClassSectionId + ","
                                                                            + "@termId = " + grandTermId + ","
                                                                            + "@ExamTypeId = " + grandExamTypeId + ","
                                                                            + "@studentId = " + st.Id + ","
                                                                            + "@year = '" + grandYear + "'";
                    }

                    resultds = examRepo.BuildStudentGrandList(spQuery);
                    LogWriter.WriteLog("Transcript Of All Students Result Table Count : " + (resultds == null || resultds.Tables == null ? 0 : resultds.Tables.Count));
                    List<ActivityMarksViewModel> activitiesList = examRepo.GetActivityMarksModelByStudentId(st.Id);

                    StudentTranscriptResult pdf = new StudentTranscriptResult();
                    DataSet ds = (DataSet)Session["GrandResultPosition"];
                    var rows = ds.Tables[ds.Tables.Count - 1].AsEnumerable().Where(dr => dr.Field<int>("StudentId") == st.Id);
                    int position = int.Parse(rows.ElementAt(0)["Position"].ToString());
                    //docList[studentCOunt] = pdf.CreatePdf(st.RollNumber, st.Name, className, sectionName, "", resultds);
                    doc = pdf.CreatePdf(st, className, sectionName, "", resultds, doc, branchId, activitiesList, IssuedDate, position);
                    studentCOunt++;
                }

                MemoryStream stream = new MemoryStream();
                doc.Save(stream, false);
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

        //        public ActionResult CreateTranscriptOfAllStudents()
        //        {
        //            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
        //            {
        //                return RedirectToAction("Index", "Login");
        //            }

        //            int grandClassSectionId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_CLASS_SECTION_ID];
        //            int grandTermId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_TERM_ID];
        //            int grandExamTypeId = (int)Session[ConstHelper.SEARCH_GRAND_SHEET_EXAM_ID];
        //            int grandStudentId = (int)Session[ConstHelper.SEARCH_GRAND_STUDENT_ID];
        //            string grandYear = (string)Session[ConstHelper.SEARCH_GRAND_SHEET_YEAR];

        //            var studentList = studentRepo.GetStudentByClassSectionId(grandClassSectionId);
        //            PdfDocument[] docList = new PdfDocument[studentList.Count];

        //            ClassSection clsec = classSecRepo.GetClassSectionById(grandClassSectionId);
        //            string className = clsec.Class.Name;
        //            string sectionName = clsec.Section.Name;

        //            int studentCOunt = 0;
        //            foreach (StudentModel st in studentList)
        //            {
        //                DataSet resultds = new DataSet();
        //                var spQuery = @"DECLARE	@return_value int
        //                                                        EXEC	@return_value = [dbo].[sp_Get_Student_Result]
        //		                                                        @ClassSectionId = " + grandClassSectionId + ","
        //                                                                        + "@termId = " + grandTermId + ","
        //                                                                        + "@ExamTypeId = " + grandExamTypeId + ","
        //                                                                        + "@studentId = " + st.Id + ","
        //                                                                        + "@year = '" + grandYear + "'";
        //                resultds = examRepo.BuildStudentGrandList(spQuery);


        //                StudentTranscriptResult pdf = new StudentTranscriptResult();
        //                docList[studentCOunt] = pdf.CreatePdf(st.RollNumber, st.Name, className, sectionName, "", resultds);
        //                studentCOunt++;
        //            }

        //            using (var compressedFileStream = new MemoryStream())
        //            {
        //                //Create an archive and store the stream in memory.
        //                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update, false))
        //                {
        //                    for (int i = 0; i < studentCOunt; i++)
        //                    {
        //                        using (MemoryStream stream = new MemoryStream())
        //                        {
        //                            docList[i].Save(stream, false);
        //                            //stream.Position = 0;
        //                            stream.Seek(0, SeekOrigin.Begin);

        //                            var zipEntry = zipArchive.CreateEntry(studentList[i].RollNumber + "-" + studentList[i].Name + ".pdf");

        //                            using (var zipEntryStream = zipEntry.Open())
        //                            {
        //                                stream.CopyTo(zipEntryStream);
        //                            }
        //                        }
        //                    }
        //                }

        //                return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = "StudentGrandResult_" + className + "_" + sectionName + DateTime.Now.ToString() + ".zip" };
        //            }
        //        }

        private string[] GetPositionAndPercentage(string columnName, DataTable dt)
        {
            string[] result = new string[2];

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DataSet ds = (DataSet)Session["GrandResultPosition"];
                int studentId = int.Parse(dt.Rows[0]["StudentId"].ToString());
                if (dt.Columns.Contains("ExamTypeName"))
                {
                    int termId = int.Parse(dt.Rows[0]["TermId"].ToString());
                    int examTypeId = int.Parse(dt.Rows[0]["ExamTypeId"].ToString());
                    foreach (DataTable tbl in ds.Tables)
                    {
                        if (tbl.Columns.Contains("ExamTypeName"))
                        {
                            int grandTermId = int.Parse(tbl.Rows[0]["TermId"].ToString());
                            int grandExamTypeId = int.Parse(tbl.Rows[0]["ExamTypeId"].ToString());
                            if (termId == grandTermId && examTypeId == grandExamTypeId)
                            {
                                var row = tbl.AsEnumerable().Where(dr => dr.Field<int>("StudentId") == studentId);
                                result[0] = row.ElementAt(0)["Position"].ToString();
                                result[1] = row.ElementAt(0)["Percentage"].ToString();
                                break;
                            }
                        }
                    }
                }
                else if (dt.Columns.Contains("TermName"))
                {
                    int termId = int.Parse(dt.Rows[0]["ExamTermId"].ToString());
                    foreach (DataTable tbl in ds.Tables)
                    {
                        if (tbl.Columns.Contains("TermName"))
                        {
                            int grandTermId = int.Parse(tbl.Rows[0]["ExamTermId"].ToString());
                            if (termId == grandTermId)
                            {
                                var row = tbl.AsEnumerable().Where(dr => dr.Field<int>("StudentId") == studentId);
                                result[0] = row.ElementAt(0)["Position"].ToString();
                                result[1] = row.ElementAt(0)["Percentage"].ToString();
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (DataTable tbl in ds.Tables)
                    {
                        if (!tbl.Columns.Contains("TermName") && !tbl.Columns.Contains("ExamTypeName"))
                        {
                            var row = tbl.AsEnumerable().Where(dr => dr.Field<int>("StudentId") == studentId);
                            result[0] = row.ElementAt(0)["Position"].ToString();
                            result[1] = row.ElementAt(0)["Percentage"].ToString();
                        }
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return result;
        }

        private DataTable AddGradeAndTotalToTable(DataTable dt)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                dt.Columns.Add("Total");
                dt.Columns.Add("Grade");

                decimal obtMarksSum = 0;
                int totalMarksSum = 0;
                string[] postionPercnt = new string[2];

                if (dt.Columns.Contains("ExamTypeName"))
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string grade = SessionHelper.GetGrade(decimal.Parse(dt.Rows[i]["ObtainedMarks"].ToString()), int.Parse(dt.Rows[i]["ExamPercentage"].ToString()), 50);
                        dt.Rows[i]["Grade"] = grade;
                        dt.Rows[i]["Total"] = int.Parse(dt.Rows[i]["ExamPercentage"].ToString());
                        obtMarksSum += decimal.Parse(dt.Rows[i]["ObtainedMarks"].ToString());
                        totalMarksSum += int.Parse(dt.Rows[i]["ExamPercentage"].ToString());
                    }
                    postionPercnt = GetPositionAndPercentage("ExamTypeName", dt);
                }
                else if (dt.Columns.Contains("TermName"))
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string grade = SessionHelper.GetGrade(decimal.Parse(dt.Rows[i]["ObtainedMarks"].ToString()), int.Parse(dt.Rows[i]["TermPercentage"].ToString()), 50);
                        dt.Rows[i]["Grade"] = grade;
                        dt.Rows[i]["Total"] = int.Parse(dt.Rows[i]["TermPercentage"].ToString());
                        obtMarksSum += decimal.Parse(dt.Rows[i]["ObtainedMarks"].ToString());
                        totalMarksSum += int.Parse(dt.Rows[i]["TermPercentage"].ToString());
                    }
                    postionPercnt = GetPositionAndPercentage("TermName", dt);
                }
                else
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string grade = SessionHelper.GetGrade(decimal.Parse(dt.Rows[i]["ObtainedMarks"].ToString()), int.Parse(dt.Rows[i]["TotalMarks"].ToString()), 50);
                        dt.Rows[i]["Grade"] = grade;
                        dt.Rows[i]["Total"] = int.Parse(dt.Rows[i]["TotalMarks"].ToString());
                        obtMarksSum += decimal.Parse(dt.Rows[i]["ObtainedMarks"].ToString());
                        totalMarksSum += int.Parse(dt.Rows[i]["TotalMarks"].ToString());
                    }
                    postionPercnt = GetPositionAndPercentage("", dt);
                }
                dt.Rows.Add();
                var row = dt.Rows.Add();
                row["SubjectName"] = "Total";
                row["ObtainedMarks"] = obtMarksSum.ToString();
                row["Total"] = totalMarksSum.ToString();
                row["Grade"] = SessionHelper.GetGrade(obtMarksSum, totalMarksSum, 50);

                var row1 = dt.Rows.Add();
                row1["SubjectName"] = "Percentage";
                row1["ObtainedMarks"] = decimal.Parse(postionPercnt[1].Replace("%", ""));
                row1["Total"] = "Position";
                row1["Grade"] = postionPercnt[0];
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return dt;
        }

        private DataTable AddAccumulativeGradeAndTotalToTable(DataTable dt)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                dt.Columns.Add("Total");
                dt.Columns.Add("Grade");
                decimal obtMarksSum = 0;
                int totalMarksSum = 0;
                string[] postionPercnt = new string[2];

                if (dt.Columns.Contains("ExamTypeName"))
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string grade = SessionHelper.GetGrade(decimal.Parse(dt.Rows[i]["ObtainedMarks"].ToString()), int.Parse(dt.Rows[i]["ExamPercentage"].ToString()), 50);
                        dt.Rows[i]["Grade"] = grade;
                        dt.Rows[i]["Total"] = int.Parse(dt.Rows[i]["ExamPercentage"].ToString());
                        obtMarksSum += decimal.Parse(dt.Rows[i]["ObtainedMarks"].ToString());
                        totalMarksSum += int.Parse(dt.Rows[i]["ExamPercentage"].ToString());
                    }
                    postionPercnt = GetPositionAndPercentage("ExamTypeName", dt);
                }
                else if (dt.Columns.Contains("TermName"))
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string grade = SessionHelper.GetGrade(decimal.Parse(dt.Rows[i]["ObtainedMarks"].ToString()), int.Parse(dt.Rows[i]["TermPercentage"].ToString()), 50);
                        dt.Rows[i]["Grade"] = grade;
                        dt.Rows[i]["Total"] = int.Parse(dt.Rows[i]["TermPercentage"].ToString());
                        obtMarksSum += decimal.Parse(dt.Rows[i]["ObtainedMarks"].ToString());
                        totalMarksSum += int.Parse(dt.Rows[i]["TermPercentage"].ToString());
                    }
                    postionPercnt = GetPositionAndPercentage("TermName", dt);
                }
                else
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string grade = SessionHelper.GetGrade(decimal.Parse(dt.Rows[i]["ObtainedMarks"].ToString()), 100, 50);
                        dt.Rows[i]["Grade"] = grade;
                        dt.Rows[i]["Total"] = 100;
                        obtMarksSum += decimal.Parse(dt.Rows[i]["ObtainedMarks"].ToString());
                        totalMarksSum += 100;
                    }
                    postionPercnt = GetPositionAndPercentage("", dt);
                }
                if (dt.Columns.Contains("TotalMarks"))
                    dt.Rows.Add(null, null, null, null, null, totalMarksSum, null);
                else
                    dt.Rows.Add();

                DataRow row = null;
                DataRow row1 = null;
                if (dt.Columns.Contains("TotalMarks"))
                    row = dt.Rows.Add(null, null, null, null, null, totalMarksSum, null);
                else
                    row = dt.Rows.Add();

                row["SubjectName"] = "Total";
                row["ObtainedMarks"] = obtMarksSum.ToString();
                row["Total"] = totalMarksSum.ToString();
                //if(dt.Columns.Contains("TotalMarks"))
                //    row["TotalMarks"] = totalMarksSum.ToString();
                row["Grade"] = SessionHelper.GetGrade(obtMarksSum, totalMarksSum, 50);

                if (dt.Columns.Contains("TotalMarks"))
                    row1 = dt.Rows.Add(null, null, null, null, null, totalMarksSum, null);
                else
                    row1 = dt.Rows.Add();

                row1["SubjectName"] = "Percentage";
                row1["ObtainedMarks"] = decimal.Parse(postionPercnt[1].Replace("%", ""));
                row1["Total"] = "Position";
                row1["Grade"] = postionPercnt[0];
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return dt;
        }

        [HttpGet]
        public void setExamDetail(int totalMarks, int passPerecentage)
        {
            examTotalMarks = totalMarks;
            examPassPercentage = passPerecentage;
        }

    }
}