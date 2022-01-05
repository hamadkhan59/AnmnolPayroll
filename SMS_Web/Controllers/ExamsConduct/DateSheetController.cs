using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_Web.Helpers.PdfHelper;
using PdfSharp.Pdf;
using System.IO;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_DAL.ViewModel;
using SMS.Modules.BuildPdf;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.ExamsConduct
{
    public class DateSheetController : Controller
    {
        static int errorCode = 0, sheetErrorCode = 0;
        //
        // GET: /DateSheet/

        
        IClassSectionRepository classSecRepo;
        IClassRepository classRepo;
        ISectionRepository secRepo;
        IStudentRepository studentRepo;
        IExamRepository examRepo;
        IFeePlanRepository feePlanRepo;
        IClassSubjectRepository clasSubjRepo;
        ISubjectRepository subjRepo;
        public DateSheetController()
        {
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());;
            examRepo = new ExamRepositoryImp(new SC_WEBEntities2());;
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());;
            clasSubjRepo = new ClassSubjectRepositoryImp(new SC_WEBEntities2());;
            subjRepo = new SubjectRepositoryImp(new SC_WEBEntities2());;

        }
        public ActionResult Index(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_SECTION_WISE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewData["branchId"] = branchId;

                if (Session[ConstHelper.SEARCH_DATE_SHEET_FLAG] != null && (bool)Session[ConstHelper.SEARCH_DATE_SHEET_FLAG] == true)
                {
                    Session[ConstHelper.SEARCH_DATE_SHEET_FLAG] = false;
                    ViewData["dateSheet"] = SearchDateSheet();
                }
                ViewData["Error"] = errorCode;
                errorCode = 0;
                voidSetSearchVeriables();
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View("");
        }

        public ActionResult ClassDateSheet(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_CLASS_WISE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewData["branchId"] = branchId;

                if (Session[ConstHelper.SEARCH_DATE_SHEET_FLAG] != null && (bool)Session[ConstHelper.SEARCH_DATE_SHEET_FLAG] == true)
                {
                    Session[ConstHelper.SEARCH_DATE_SHEET_FLAG] = false;
                    ViewData["dateSheet"] = SearchClassDateSheet();
                }
                ViewData["Error"] = errorCode;
                errorCode = 0;
                voidSetSearchVeriables();
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
            if (Session[ConstHelper.GLOBAL_DATE_SHEET_CLASS_ID] != null)
            {
                ViewData["GlobalClassId"] = (int)Session[ConstHelper.GLOBAL_DATE_SHEET_CLASS_ID];
                Session[ConstHelper.GLOBAL_DATE_SHEET_CLASS_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_DATE_SHEET_SECTION_ID] != null)
            {
                ViewData["GlobalSectionId"] = (int)Session[ConstHelper.GLOBAL_DATE_SHEET_SECTION_ID];
                Session[ConstHelper.GLOBAL_DATE_SHEET_SECTION_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_DATE_SHEET_YEAR_ID] != null)
            {
                ViewData["GlobalYearId"] = (int)Session[ConstHelper.GLOBAL_DATE_SHEET_YEAR_ID];
                Session[ConstHelper.GLOBAL_DATE_SHEET_YEAR_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_DATE_SHEET_TERM_ID] != null)
            {
                ViewData["GlobalTermId"] = (int)Session[ConstHelper.GLOBAL_DATE_SHEET_TERM_ID];
                Session[ConstHelper.GLOBAL_DATE_SHEET_TERM_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_DATE_SHEET_EXAM_ID] != null)
            {
                ViewData["GLobalExamId"] = (int)Session[ConstHelper.GLOBAL_DATE_SHEET_EXAM_ID];
                Session[ConstHelper.GLOBAL_DATE_SHEET_EXAM_ID] = null;
            }

        }

        //
        // GET: /DateSheet/Details/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchDateSheet(string ClassId, string SectionId, string Year, string ExamId, string TermId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Session[ConstHelper.SEARCH_DATE_SHEET_FLAG] = true;
                Session[ConstHelper.SEARCH_DATE_SHEET_CLASS_ID] = int.Parse(ClassId);
                Session[ConstHelper.SEARCH_DATE_SHEET_SECTION_ID] = int.Parse(string.IsNullOrEmpty(SectionId) == true ? "0" : SectionId);
                Session[ConstHelper.SEARCH_DATE_SHEET_EXAM_ID] = int.Parse(ExamId);

                Session[ConstHelper.GLOBAL_DATE_SHEET_CLASS_ID] = int.Parse(ClassId);
                Session[ConstHelper.GLOBAL_DATE_SHEET_SECTION_ID] = int.Parse(SectionId);
                Session[ConstHelper.GLOBAL_DATE_SHEET_YEAR_ID] = int.Parse(Year);
                Session[ConstHelper.GLOBAL_DATE_SHEET_EXAM_ID] = int.Parse(ExamId);
                Session[ConstHelper.GLOBAL_DATE_SHEET_TERM_ID] = int.Parse(TermId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", new { id = -59 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchClassDateSheet(string ClassId, string Year, string ExamId, string TermId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Session[ConstHelper.SEARCH_DATE_SHEET_FLAG] = true;
                Session[ConstHelper.SEARCH_DATE_SHEET_CLASS_ID] = int.Parse(ClassId);
                Session[ConstHelper.SEARCH_DATE_SHEET_EXAM_ID] = int.Parse(ExamId);

                Session[ConstHelper.GLOBAL_DATE_SHEET_CLASS_ID] = int.Parse(ClassId);
                Session[ConstHelper.GLOBAL_DATE_SHEET_YEAR_ID] = int.Parse(Year);
                Session[ConstHelper.GLOBAL_DATE_SHEET_EXAM_ID] = int.Parse(ExamId);
                Session[ConstHelper.GLOBAL_DATE_SHEET_TERM_ID] = int.Parse(TermId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("ClassDateSheet", new { id = -59 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAttendanceSheet(string ClassId, string SectionId, string Year, string ExamId, string subjectId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_FLAG] = true;
                Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_CLASS_ID] = int.Parse(ClassId);
                Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_SECTION_ID] = int.Parse(SectionId);
                Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_EXAM_ID] = int.Parse(ExamId);
                Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_SUBJECT_ID] = int.Parse(subjectId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("AttendanceSheet", new { id = -59 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAttendanceSheetPdf(string[] RollNumber, string[] Name)
        {
            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int classId = (int)Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_CLASS_ID];
                int sectionId = (int)Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_SECTION_ID];
                int examId = (int)Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_EXAM_ID];
                int subjectId = (int)Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_SUBJECT_ID];

                string examName = examRepo.GetExamTypeById(examId).Name;
                string className = classRepo.GetClassById(classId).Name;
                string sectionName = secRepo.GetSectionById(sectionId).Name;
                string subjectName = subjRepo.GetSubjectById(subjectId).Name;

                var dateSheet = examRepo.GetDateSheetByClassAndExam(classId, sectionId, examId, subjectId);

                if (dateSheet != null)
                {
                    string examDate = dateSheet.ExamDate.ToString().Split(' ')[0];
                    AttandanceSheetPdf pdf = new AttandanceSheetPdf();
                    PdfDocument document = new PdfDocument();
                    LogWriter.WriteLog("creating the pdf");
                    document = pdf.CreatePdf(className, sectionName, subjectName, examName, examDate, RollNumber, Name, branchId);

                    MemoryStream stream = new MemoryStream();
                    document.Save(stream, false);
                    stream.Seek(0, SeekOrigin.Begin);
                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return File(stream, "application/pdf");

                    //using (MemoryStream stream = new MemoryStream())
                    //{
                    //    document.Save(stream, false);

                    //    stream.Seek(0, SeekOrigin.Begin);
                    //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = "Attendance Sheet : " + examName + "_" + className + "_" + sectionName + "_" + DateTime.Now.ToString() + ".pdf" };
                    //}
                }
                else
                {
                    sheetErrorCode = 450;
                    LogWriter.WriteLog("No date sheet is found to create pdf");
                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return RedirectToAction("AttendanceSheet", new { id = -59 });
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return RedirectToAction("AttendanceSheet", new { id = -59 });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAwardSheet(string ClassId, string SectionId, string Year, string ExamId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_EXAM_AWARD_SHEET) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                int classId = int.Parse(ClassId);
                int sectionId = int.Parse(SectionId);
                var clasSection = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId);
                int clasSectionId = clasSection.ClassSectionId;
                int examTypeId = int.Parse(ExamId);

                var courseList = clasSubjRepo.GetRegisterCourseByClassSectionId(clasSectionId);

                if (courseList != null && courseList.Count > 0)
                {
                    AwardsList pdf = new AwardsList();
                    LogWriter.WriteLog("creating the pdf");
                    PdfDocument document = pdf.CreatePdf(examTypeId, clasSectionId, branchId);

                    MemoryStream stream = new MemoryStream();
                    document.Save(stream, false);
                    stream.Seek(0, SeekOrigin.Begin);
                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return File(stream, "application/pdf");

                }
                else
                {
                    sheetErrorCode = 510;
                    LogWriter.WriteLog("No course list is found to create pdf");
                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return RedirectToAction("AwardSheet", new { id = -59 });

                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return RedirectToAction("AwardSheet", new { id = -59 });
            }


            //using (MemoryStream stream = new MemoryStream())
            //{
            //    document.Save(stream, false);

            //    stream.Seek(0, SeekOrigin.Begin);
            //    return File(stream, "application/pdf");
            //    //return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = "Awards Sheet : " + clasSection.Class.Name + "_" + clasSection.Section.Name + "_" + DateTime.Now.ToString() + ".pdf" };
            //}

        }

        public ActionResult AttendanceSheet(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_EXAM_ATTENDANCE_SHEET) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewBag.ExamId = new SelectList(SessionHelper.ExamTypeList(Session.SessionID), "Id", "Name");
                ViewBag.TermId = new SelectList(SessionHelper.ExamTermList(Session.SessionID), "Id", "TermName");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                ViewData["examType"] = SessionHelper.ExamTypeList(Session.SessionID);
                ViewData["examTerm"] = SessionHelper.ExamTermList(Session.SessionID);
                ViewData["classSubject"] = SessionHelper.ClassSubjectList(Session.SessionID);
                ViewBag.SubjectId = new SelectList(SessionHelper.SubjectList(Session.SessionID), "Id", "Name");
                if (Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_FLAG] != null && (bool)Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_FLAG] == true)
                {
                    Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_FLAG] = false;
                    ViewData["ateendanceSheet"] = CreateAttendanceSheet();
                }
                ViewData["SheetError"] = sheetErrorCode;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View("");
        }

        public ActionResult AwardSheet(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_EXAM_AWARD_SHEET) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewBag.ExamId = new SelectList(SessionHelper.ExamTypeList(Session.SessionID), "Id", "Name");
                ViewBag.TermId = new SelectList(SessionHelper.ExamTermList(Session.SessionID), "Id", "TermName");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                ViewData["examType"] = SessionHelper.ExamTypeList(Session.SessionID);
                ViewData["examTerm"] = SessionHelper.ExamTermList(Session.SessionID);
                ViewData["classSubject"] = SessionHelper.ClassSubjectList(Session.SessionID);
                ViewBag.SubjectId = new SelectList(SessionHelper.SubjectList(Session.SessionID), "Id", "Name");
                ViewData["SheetError"] = sheetErrorCode;
                sheetErrorCode = 0;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDateSheet(int[] subjectIds, int[] classIds, int[] sectionIds, int[] examIds, DateTime[] ExamDate, string[] startTimes, string[] endTimes, string[] Center, string[] SubjectName, int IsPrint)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (IsPrint != 0)
                {
                    if (IsPrint == 1)
                        return PrintStudentDateSheet(subjectIds, classIds, sectionIds, examIds, ExamDate, startTimes, endTimes, Center, SubjectName);
                    if (IsPrint == 2)
                        return PrintNoticeBoardDateSheet(subjectIds, classIds, sectionIds, examIds, ExamDate, startTimes, endTimes, Center, SubjectName);
                    if (IsPrint == 3)
                    {
                        DeleteDateSheet(subjectIds, classIds, sectionIds, examIds, ExamDate, startTimes, endTimes, Center, SubjectName);
                        return RedirectToAction("Index", new { id = -59 });
                    }
                }
                else
                {
                    try
                    {
                        for (int i = 0; i < subjectIds.Count(); i++)
                        {
                            int subjectId = subjectIds[i];
                            int classId = classIds[i];
                            int sectionId = sectionIds[i];
                            int examId = examIds[i];
                            DateSheet ds = examRepo.GetDateSheetByClassAndExam(classId, sectionId, examId, subjectId);
                            ds.ExamDate = ExamDate[i];
                            ds.ExamTime = startTimes[i] + "-" + endTimes[i];
                            ds.Center = (Center == null || Center[i] == null || Center[i].Length == 0) ? "" : Center[i];
                            ds.CreatedOn = DateTime.Now;
                            examRepo.UpdateDateSheet(ds);
                        }
                        errorCode = 2;
                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        LogWriter.WriteExceptionLog(ex);
                        errorCode = 420;
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
            return RedirectToAction("Index", new { id = -59 });
        }

        public void DeleteDateSheet(int[] subjectIds, int[] classIds, int[] sectionIds, int[] examIds, DateTime[] ExamDate, string[] startTimes, string[] endTimes, string[] Center, string[] SubjectName)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                for (int i = 0; i < subjectIds.Count(); i++)
                {
                    int subjectId = subjectIds[i];
                    int classId = classIds[i];
                    int sectionId = sectionIds[i];
                    int examId = examIds[i];
                    DateSheet ds = examRepo.GetDateSheetByClassAndExam(classId, sectionId, examId, subjectId);
                    examRepo.DeleteDateSheet(ds);
                }
                errorCode = 20;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 1420;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveClassDateSheet(int[] subjectIds, int[] classIds, int[] sectionIds, int[] examIds, DateTime[] ExamDate, string[] startTimes, string[] endTimes, string[] Center, string[] SubjectName, int IsPrint)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (IsPrint != 0)
                {
                    if (IsPrint == 1)
                        return PrintClassStudentDateSheet(subjectIds, classIds, sectionIds, examIds, ExamDate, startTimes, endTimes, Center, SubjectName);
                    if (IsPrint == 2)
                        return PrintClassWiseDateSheet(subjectIds, classIds, sectionIds, examIds, ExamDate, startTimes, endTimes, Center, SubjectName);
                    if (IsPrint == 3)
                        return PrintSectionWiseDateSheet(subjectIds, classIds, sectionIds, examIds, ExamDate, startTimes, endTimes, Center, SubjectName);
                    if (IsPrint == 4)
                    {
                        DeleteClassWiseDateSheet(subjectIds, classIds, sectionIds, examIds, ExamDate, startTimes, endTimes, Center, SubjectName);
                        return RedirectToAction("ClassDateSheet", new { id = -59 });
                    }
                }
                else
                {
                    try
                    {
                        for (int i = 0; i < subjectIds.Count(); i++)
                        {
                            int subjectId = subjectIds[i];
                            int classId = classIds[i];
                            int examId = examIds[i];
                            DateSheet ds = examRepo.GetSingleDateSheetBySubjectAndExam(classId, examId, subjectId);
                            ds.ExamDate = ExamDate[i];
                            ds.ExamTime = startTimes[i] + "-" + endTimes[i];
                            ds.Center = (Center == null || Center[i] == null || Center[i].Length == 0) ? "" : Center[i];
                            ds.CreatedOn = DateTime.Now;
                            examRepo.UpdateDateSheet(ds);
                        }
                        errorCode = 2;
                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        LogWriter.WriteExceptionLog(ex);
                        errorCode = 420;
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
            return RedirectToAction("ClassDateSheet", new { id = -59 });
        }

        public void DeleteClassWiseDateSheet(int[] subjectIds, int[] classIds, int[] sectionIds, int[] examIds, DateTime[] ExamDate, string[] startTimes, string[] endTimes, string[] Center, string[] SubjectName)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                for (int i = 0; i < subjectIds.Count(); i++)
                {
                    int subjectId = subjectIds[i];
                    int classId = classIds[i];
                    int examId = examIds[i];
                    DateSheet ds = examRepo.GetSingleDateSheetBySubjectAndExam(classId, examId, subjectId);
                    examRepo.DeleteDateSheet(ds);
                }
                errorCode = 20;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 1420;
            }
        }
        private ActionResult PrintStudentDateSheet(int[] subjectIds, int[] classIds, int[] sectionIds, int[] examIds, DateTime[] ExamDate, string[] startTimes, string[] endTimes, string[] Center, string[] SubjectName)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                string examName = examRepo.GetExamTypeById(examIds[0]).Name;
                string className = classRepo.GetClassById(classIds[0]).Name;
                string sectionName = secRepo.GetSectionById(sectionIds[0]).Name;

                int classId = (int)Session[ConstHelper.SEARCH_DATE_SHEET_CLASS_ID];
                int sectionId = (int)Session[ConstHelper.SEARCH_DATE_SHEET_SECTION_ID];
                int classSectionId = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                var studentList = studentRepo.GetStudentByClassSectionId(classSectionId);

                DateSheetPdf pdf = new DateSheetPdf();
                PdfDocument document = new PdfDocument();
                if (studentList == null || studentList.Count == 0)
                {
                    errorCode = 210;
                    LogWriter.WriteLog("No student is found to print datesheet");
                    return null;
                }
                else
                {
                    foreach (var student in studentList)
                    {
                        document = pdf.createDateSheet(subjectIds, className, sectionName, examName, ExamDate, startTimes, endTimes, Center, SubjectName, document, student);
                    }

                    MemoryStream stream = new MemoryStream();
                    document.Save(stream, false);

                    stream.Seek(0, SeekOrigin.Begin);
                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return File(stream, "application/pdf");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }


        private ActionResult PrintNoticeBoardDateSheet(int[] subjectIds, int[] classIds, int[] sectionIds, int[] examIds, DateTime[] ExamDate, string[] startTimes, string[] endTimes, string[] Center, string[] SubjectName)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                string examName = examRepo.GetExamTypeById(examIds[0]).Name;
                string className = classRepo.GetClassById(classIds[0]).Name;
                string sectionName = secRepo.GetSectionById(sectionIds[0]).Name;

                PdfDocument document = new PdfDocument();
                DateSheetClassWisePdf pdf = new DateSheetClassWisePdf();
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                document = pdf.createDateSheet(subjectIds, className, sectionName, examName, ExamDate, startTimes, endTimes, Center, SubjectName, document, branchId);

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

        private ActionResult PrintClassStudentDateSheet(int[] subjectIds, int[] classIds, int[] sectionIds, int[] examIds, DateTime[] ExamDate, string[] startTimes, string[] endTimes, string[] Center, string[] SubjectName)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                string examName = examRepo.GetExamTypeById(examIds[0]).Name;
                string className = classRepo.GetClassById(classIds[0]).Name;
                string sectionName = "";

                int classId = (int)Session[ConstHelper.SEARCH_DATE_SHEET_CLASS_ID];
                var studentList = studentRepo.GetStudentByClassId(classId);

                DateSheetPdf pdf = new DateSheetPdf();
                PdfDocument document = new PdfDocument();
                if (studentList == null || studentList.Count == 0)
                {
                    LogWriter.WriteLog("No student is found to print datesheet");
                    errorCode = 210;
                    return null;
                }
                else
                {
                    string tempSection = studentList[0].SectionName;
                    var classSubjectsIds = clasSubjRepo.GetSubjectListByClassAndSectionName(className, tempSection);
                    foreach (var student in studentList)
                    {
                        sectionName = student.SectionName;
                        if (tempSection != sectionName)
                        {
                            tempSection = sectionName;
                            classSubjectsIds = clasSubjRepo.GetSubjectListByClassAndSectionName(className, tempSection);
                        }
                        document = pdf.createDateSheet(subjectIds, className, sectionName, examName, ExamDate, startTimes, endTimes, Center, SubjectName, document, student, classSubjectsIds);
                    }

                    MemoryStream stream = new MemoryStream();
                    document.Save(stream, false);

                    stream.Seek(0, SeekOrigin.Begin);
                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return File(stream, "application/pdf");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }


        private ActionResult PrintClassWiseDateSheet(int[] subjectIds, int[] classIds, int[] sectionIds, int[] examIds, DateTime[] ExamDate, string[] startTimes, string[] endTimes, string[] Center, string[] SubjectName)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                string examName = examRepo.GetExamTypeById(examIds[0]).Name;
                string className = classRepo.GetClassById(classIds[0]).Name;
                int classId = (int)Session[ConstHelper.SEARCH_DATE_SHEET_CLASS_ID];

                DateSheetClassWisePdf pdf = new DateSheetClassWisePdf();
                PdfDocument document = new PdfDocument();

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                document = pdf.createDateSheet(subjectIds, className, "", examName, ExamDate, startTimes, endTimes, Center, SubjectName, document, branchId);

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

        private ActionResult PrintSectionWiseDateSheet(int[] subjectIds, int[] classIds, int[] sectionIds, int[] examIds, DateTime[] ExamDate, string[] startTimes, string[] endTimes, string[] Center, string[] SubjectName)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                string examName = examRepo.GetExamTypeById(examIds[0]).Name;
                string className = classRepo.GetClassById(classIds[0]).Name;
                int classId = (int)Session[ConstHelper.SEARCH_DATE_SHEET_CLASS_ID];

                DateSheetClassWisePdf pdf = new DateSheetClassWisePdf();
                PdfDocument document = new PdfDocument();

                var clasSecList = classSecRepo.GetSectionsByClassId(classId);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);

                string tempSection = clasSecList[0].Name;
                var classSubjectsIds = clasSubjRepo.GetSubjectListByClassAndSectionName(className, tempSection);

                foreach (var section in clasSecList)
                {
                    if (tempSection != section.Name)
                    {
                        tempSection = section.Name;
                        classSubjectsIds = clasSubjRepo.GetSubjectListByClassAndSectionName(className, tempSection);
                    }
                    document = pdf.createDateSheet(subjectIds, className, section.Name, examName, ExamDate, startTimes, endTimes, Center, SubjectName, document, branchId, classSubjectsIds);
                }
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

        //private FileResult PrintDateSheet(int[] subjectIds, int[] classIds, int[] sectionIds, int[] examIds, DateTime[] ExamDate, string[] startTimes, string[] endTimes, string[] Center, string[] SubjectName)
        //{
        //    int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
        //    string examName = examRepo.GetExamTypeById(examIds[0]).Name;
        //    string className = classRepo.GetClassById(classIds[0]).Name;
        //    string sectionName = secRepo.GetSectionById(sectionIds[0]).Name;

        //    DateSheetPdf pdf = new DateSheetPdf();
        //    PdfDocument document = pdf.createDateSheet(subjectIds, className, sectionName, examName, ExamDate, startTimes, endTimes, Center, SubjectName, branchId);

        //    MemoryStream stream = new MemoryStream();
        //    document.Save(stream, false);
        //    stream.Seek(0, SeekOrigin.Begin);
        //    return File(stream, "application/pdf");

        //    //using (MemoryStream stream = new MemoryStream())
        //    //{
        //    //    document.Save(stream, false);

        //    //    stream.Seek(0, SeekOrigin.Begin);
        //    //    return new FileContentResult(stream.ToArray(), "application/pdf") { FileDownloadName = "Date Sheet : " + examName + "-" + className + "-" + sectionName + "-" + DateTime.Now + "-" + DateTime.Now.ToString() + ".pdf" };
        //    //}  
        //}

        private IList<StudentModel> CreateAttendanceSheet()
        {
            
            //Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_EXAM_ID] = int.Parse(ExamId);
            //Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_SUBJECT_ID] = int.Parse(subjectId);

            IList<StudentModel> studentList = null;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int classId = (int)Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_CLASS_ID];
                int sectionId = (int)Session[ConstHelper.SEARCH_ATTENDANCE_SHEET_SECTION_ID];
                int classSectionId = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                studentList = studentRepo.GetStudentByClassSectionId(classSectionId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                sheetErrorCode = 420;
            }
            return studentList;
        }

        private IList<DateSheetModel> SearchClassDateSheet()
        {
            
            //IList<DateSheet> dateSheetList = examRepo.GetDateSheetByClassAndExam(classId, sectionId, examId);
            IList<DateSheetModel> dateSheetList = new List<DateSheetModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int classId = (int)Session[ConstHelper.SEARCH_DATE_SHEET_CLASS_ID];
                int examId = (int)Session[ConstHelper.SEARCH_DATE_SHEET_EXAM_ID];

                dateSheetList = examRepo.GetClassDateSheetByExamModel(classId, examId);
                if (dateSheetList == null || dateSheetList.Count == 0)
                {
                    IList<SubjectModel> subjectList = clasSubjRepo.GetSubjectListByClass(classId);
                    foreach (SubjectModel model in subjectList)
                    {
                        DateSheet ds = new DateSheet();
                        ds.ClassId = classId;
                        ds.SectionId = 0;
                        ds.ExamId = examId;
                        ds.SubjectId = model.SubjectId;
                        examRepo.AddDateSheet(ds);
                        //dateSheetList.Add(ds);
                    }

                    dateSheetList = examRepo.GetClassDateSheetByExamModel(classId, examId);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return dateSheetList;
        }

        private IList<DateSheetModel> SearchDateSheet()
        {
            IList<DateSheetModel> dateSheetList = new List<DateSheetModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int classId = (int)Session[ConstHelper.SEARCH_DATE_SHEET_CLASS_ID];
                int sectionId = (int)Session[ConstHelper.SEARCH_DATE_SHEET_SECTION_ID];
                int examId = (int)Session[ConstHelper.SEARCH_DATE_SHEET_EXAM_ID];

                dateSheetList = examRepo.GetDateSheetByClassAndExamModel(classId, sectionId, examId);
                if (dateSheetList == null || dateSheetList.Count == 0)
                {
                    int classSectionId = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                    IList<RegisterCourse> registerCourseList = clasSubjRepo.GetRegisterCourseByClassSectionId(classSectionId);
                    foreach (RegisterCourse rc in registerCourseList)
                    {
                        DateSheet ds = new DateSheet();
                        ds.ClassId = classId;
                        ds.SectionId = sectionId;
                        ds.ExamId = examId;
                        ds.SubjectId = rc.SubjectId;
                        examRepo.AddDateSheet(ds);
                        //dateSheetList.Add(ds);
                    }

                    dateSheetList = examRepo.GetDateSheetByClassAndExamModel(classId, sectionId, examId);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            
            return dateSheetList;
        }

    }
}