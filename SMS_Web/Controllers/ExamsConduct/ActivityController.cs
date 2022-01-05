using Logger;
using PdfSharp.Pdf;
using SMS.Modules.BuildPdf;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Filters;
using SMS_Web.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SMS_Web.Controllers.ExamsConduct
{
    public class ActivityController : Controller
    {
        //
        // GET: /Activity/

        private static int errorCode = 0;
        IExamRepository examRepo;
        IClassSectionRepository classSecRepo;
        IStudentRepository studentRepo;
        IFeePlanRepository feePlanRepo;

        public ActivityController()
        {
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            examRepo = new ExamRepositoryImp(new SC_WEBEntities2());
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
        }

        public ActionResult Index(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_ACTIVITY) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            Activity activity = new Activity();

            try
            {
                if (Id > 0)
                    errorCode = 0;
                ViewData["Operation"] = Id;
                ViewData["Error"] = errorCode;
                errorCode = 0;
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewData["activities"] = SessionHelper.ActivityList(Session.SessionID);

                activity = examRepo.GetActivityById(Id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(activity);
        }

        public ActionResult Marks(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_ACTIVITY_MARKS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<ActivityMarksViewModel> marksDetail = new List<ActivityMarksViewModel>();

            try
            {
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewBag.ActivityId = new SelectList(SessionHelper.ActivityList(Session.SessionID), "Id", "ActivityName");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                ViewData["activities"] = SessionHelper.ActivityList(Session.SessionID);

                ActivityMark marks = new ActivityMark();

                if (Session["detailList"] != null)
                {
                    marksDetail = (List<ActivityMarksViewModel>)Session["detailList"];
                    Session["detailList"] = null;
                }

                if (Session["marks"] != null)
                {
                    marks = (ActivityMark)Session["marks"];
                    ViewData["PassPercentage"] = marks.PassPercentage;
                    ViewData["TotalMarks"] = marks.TotalMarks;
                    ViewData["marksId"] = marks.Id;
                    Session["marks"] = null;
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
            return View(marksDetail);
        }

        public ActionResult DailyTest(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_DAILY_TESTS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<DailyTestViewModel> testDetail = new List<DailyTestViewModel>();
            try
            {
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewBag.SubjectId = new SelectList(SessionHelper.SubjectList(Session.SessionID), "Id", "Name");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                ViewData["classSubject"] = SessionHelper.ClassSubjectList(Session.SessionID);

                DailyTest dailyTest = new DailyTest();

                if (Session["detailList"] != null)
                {
                    testDetail = (List<DailyTestViewModel>)Session["detailList"];
                    Session["detailList"] = null;
                }

                if (Session["dailyTest"] != null)
                {
                    dailyTest = (DailyTest)Session["dailyTest"];
                    ViewData["PassPercentage"] = dailyTest.PassPercentage;
                    ViewData["TotalMarks"] = dailyTest.TotalMarks;
                    ViewData["dailyTestId"] = dailyTest.Id;
                    Session["dailyTest"] = null;
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
            return View(testDetail);
        }

        public ActionResult DailyTestSearch(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.EC_DAILY_TESTS_SEARCH) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<DailyTestViewModel> testDetail = new List<DailyTestViewModel>();

            try
            {
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewBag.SubjectId = new SelectList(SessionHelper.SubjectList(Session.SessionID), "Id", "Name");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                ViewData["classSubject"] = SessionHelper.ClassSubjectList(Session.SessionID);


                if (Session["detailListSearch"] != null)
                {
                    testDetail = (List<DailyTestViewModel>)Session["detailListSearch"];
                    Session["detailListSearch"] = null;
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
            return View(testDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveActivityMarksDetail(int[] marksDetailId, decimal[] ObtMarks, decimal totalMarks, int marksId, int IsPrint, int IsDelete, int passPercentage = 0)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (IsDelete == 1)
                {
                    DeleteActivityMarks(marksId);
                    return RedirectToAction("Marks", new { id = -59 });
                }
                if (IsPrint == 1)
                {
                    return PrintMarksSheet(marksId);
                }
                int i = 0;
                foreach (int detailId in marksDetailId)
                {
                    ActivityMarksDetail detail = examRepo.GetActivityMarksDetailById(detailId);
                    detail.ObtMarks = (int)ObtMarks[i];
                    examRepo.UpdateActivityMarksDetail(detail);
                    i++;
                }

                ActivityMark marks = examRepo.GetActivityMarksById(marksId);
                marks.PassPercentage = passPercentage;
                marks.TotalMarks = (int)totalMarks;
                examRepo.UpdateActivityMarks(marks);

                errorCode = 2;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 1420;
            }
            return RedirectToAction("Marks", new { id = -59 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDailyTestDetail(int[] testDetailId, decimal[] ObtMarks, decimal totalMarks, int dailyTestId, int IsPrint, int IsDelete, int passPercentage = 0)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (IsDelete == 1)
                {
                    DeleteDailyTest(dailyTestId);
                    return RedirectToAction("DailyTest", new { id = -59 });
                }
                if (IsPrint == 1)
                {
                    return PrintDailyTestSheet(dailyTestId);
                }
                int i = 0;
                foreach (int detailId in testDetailId)
                {
                    DailyTestsDetail detail = examRepo.GetDailyTestsDetailById(detailId);
                    detail.ObtMarks = (int)ObtMarks[i];
                    examRepo.UpdateDailyTestDetail(detail);
                    i++;
                }

                DailyTest marks = examRepo.GetDailyTestById(dailyTestId);
                marks.PassPercentage = passPercentage;
                marks.TotalMarks = (int)totalMarks;
                examRepo.UpdateDailyTest(marks);

                errorCode = 2;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 1420;
            }
            return RedirectToAction("DailyTest", new { id = -59 });
        }

        private void DeleteActivityMarks(int marksId)
        { 
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                errorCode = 200;
                List<ActivityMarksDetail> detailList = examRepo.GetActivityMarksDetailByActivityMarksId(marksId);
                foreach (var detail in detailList)
                {
                    examRepo.DeleteActivityMarksDetail(detail);
                }

                ActivityMark marks = examRepo.GetActivityMarksById(marksId);
                examRepo.DeleteActivityMarks(marks);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 520;
            }
        }

        private void DeleteDailyTest(int dailyTestId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                errorCode = 200;
                List<DailyTestsDetail> detailList = examRepo.GetDailyTestsDetailByDailyTestId(dailyTestId);
                foreach (var detail in detailList)
                {
                    examRepo.DeleteDailyTestsDetail(detail);
                }

                DailyTest marks = examRepo.GetDailyTestById(dailyTestId);
                examRepo.DeleteDailyTest(marks);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 520;
            }
        }

        private ActionResult PrintMarksSheet(int marksId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ActivitySheetPdf pdf = new ActivitySheetPdf();
                LogWriter.WriteLog("creating the pdf");
                PdfDocument document = pdf.CreatePdf(marksId);

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

        private ActionResult PrintDailyTestSheet(int dailyTestId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DailyTestSheetPdf pdf = new DailyTestSheetPdf();
                LogWriter.WriteLog("creating the pdf");
                PdfDocument document = pdf.CreatePdf(dailyTestId);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchActivityMarks(string ClassId, string SectionId, string YearId, string ActivityId)
        {
            try
            {
                

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                errorCode = 100;
                int classId = int.Parse(ClassId);
                int sectionId = int.Parse(SectionId);
                int classSectionId = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                int activityId = int.Parse(ActivityId);

                Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
                Session[ConstHelper.GLOBAL_YEAR_ID] = int.Parse(YearId);
                Session[ConstHelper.GLOBAL_MARKS_ID] = activityId;

                ActivityMark activityMarks = examRepo.GetActivityMarksByClassSectionAndActivtyId(classSectionId, activityId);
                List<ActivityMarksViewModel> detailList = new List<ActivityMarksViewModel>();
                if (activityMarks != null)
                {
                    detailList = examRepo.GetActivityMarksModelByActivityMarksId((int)activityMarks.Id);
                    if (detailList == null || detailList.Count == 0)
                    {
                        AddActivityMarksDetail(classSectionId, activityMarks.Id);
                        detailList = examRepo.GetActivityMarksModelByActivityMarksId((int)activityMarks.Id);
                    }
                }
                else
                {
                    activityMarks = AddNewActivityMarks(classSectionId, activityId);
                    AddActivityMarksDetail(classSectionId, activityMarks.Id);
                    detailList = examRepo.GetActivityMarksModelByActivityMarksId((int)activityMarks.Id);
                }

                Session["detailList"] = detailList;
                Session["marks"] = activityMarks;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Marks", new { id = -59 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchDailyTest(string ClassId, string SectionId, string SubjectId, DateTime TestDate)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                errorCode = 100;
                int classId = int.Parse(ClassId);
                int sectionId = int.Parse(SectionId);
                int classSectionId = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                int subjectId = int.Parse(SubjectId);

                Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
                Session[ConstHelper.GLOBAL_SUBJECT_ID] = subjectId;
                Session[ConstHelper.GLOBAL_TEST_DATE] = TestDate;

                DailyTest dailyTest = examRepo.GetDailyTestByClassSectionAndSubjectId(classSectionId, subjectId, TestDate);
                List<DailyTestViewModel> detailList = new List<DailyTestViewModel>();
                if (dailyTest != null)
                {
                    detailList = examRepo.GetDailyTestsModelByDailyTestId((int)dailyTest.Id);
                    if (detailList == null || detailList.Count == 0)
                    {
                        AddDailyTestDetail(classSectionId, dailyTest.Id);
                        detailList = examRepo.GetDailyTestsModelByDailyTestId((int)dailyTest.Id);
                    }
                }
                else
                {
                    dailyTest = AddNewDailyTest(classSectionId, subjectId, TestDate);
                    AddDailyTestDetail(classSectionId, dailyTest.Id);
                    detailList = examRepo.GetDailyTestsModelByDailyTestId((int)dailyTest.Id);
                }

                Session["detailList"] = detailList;
                Session["dailyTest"] = dailyTest;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("DailyTest", new { id = -59 });
        }


        public ActionResult GetDailyTest(int DailyTestId)
        {
            try
            {
                errorCode = 100;

                DailyTest dailyTest = examRepo.GetDailyTestById(DailyTestId);

                var clsecModel = classSecRepo.GetClassSectionsModelById((int)dailyTest.ClassSectionId);
                Session[ConstHelper.GLOBAL_CLASS_ID] = clsecModel.ClassId;
                Session[ConstHelper.GLOBAL_SECTION_ID] = clsecModel.SectionId;
                Session[ConstHelper.GLOBAL_SUBJECT_ID] = dailyTest.SubjectId;
                Session[ConstHelper.GLOBAL_TEST_DATE] = dailyTest.TestDate;

                List<DailyTestViewModel> detailList = new List<DailyTestViewModel>();
                detailList = examRepo.GetDailyTestsModelByDailyTestId((int)dailyTest.Id);
                
                Session["detailList"] = detailList;
                Session["dailyTest"] = dailyTest;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("DailyTest", new { id = -59 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchDailyTestHistory(string ClassId, string SectionId, string SubjectId, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                errorCode = 100;
                int classId = int.Parse(string.IsNullOrEmpty(ClassId) == true ? "0" : ClassId);
                int sectionId = int.Parse(string.IsNullOrEmpty(ClassId) == true ? "0" : ClassId);
                int subjectId = int.Parse(string.IsNullOrEmpty(SubjectId) == true ? "0" : SubjectId);

                Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
                Session[ConstHelper.GLOBAL_SUBJECT_ID] = subjectId;
                Session[ConstHelper.GLOBAL_TEST_FROM_DATE] = FromDate;
                Session[ConstHelper.GLOBAL_TEST_TO_DATE] = ToDate;

                List<DailyTestViewModel> detailList = examRepo.SearchDailyTest(classId, sectionId, subjectId, FromDate, ToDate);

                Session["detailListSearch"] = detailList;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("DailyTestSearch", new { id = -59 });
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

            if (Session[ConstHelper.GLOBAL_MARKS_ID] != null)
            {
                ViewData["GlobalMarksId"] = (int)Session[ConstHelper.GLOBAL_MARKS_ID];
                Session[ConstHelper.GLOBAL_MARKS_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_SUBJECT_ID] != null)
            {
                ViewData["GlobalSujectId"] = (int)Session[ConstHelper.GLOBAL_SUBJECT_ID];
                Session[ConstHelper.GLOBAL_SUBJECT_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_TEST_DATE] != null)
            {
                ViewData["GlobalTestDate"] = (DateTime)Session[ConstHelper.GLOBAL_TEST_DATE];
                Session[ConstHelper.GLOBAL_TEST_DATE] = null;
            }

            if (Session[ConstHelper.GLOBAL_TEST_FROM_DATE] != null)
            {
                ViewData["GlobalTestfromDate"] = (DateTime)Session[ConstHelper.GLOBAL_TEST_FROM_DATE];
                Session[ConstHelper.GLOBAL_TEST_FROM_DATE] = null;
            }

            if (Session[ConstHelper.GLOBAL_TEST_TO_DATE] != null)
            {
                ViewData["GlobalTestToDate"] = (DateTime)Session[ConstHelper.GLOBAL_TEST_TO_DATE];
                Session[ConstHelper.GLOBAL_TEST_TO_DATE] = null;
            }

        }

        private ActivityMark AddNewActivityMarks(int classSectionId, int activityId)
        {
            ActivityMark mark = new ActivityMark();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                mark.ClassSectionId = classSectionId;
                mark.ActivityId = activityId;
                mark.TotalMarks = 0;
                mark.PassPercentage = 0;
                mark.CreatedOn = DateTime.Now;
                examRepo.AddActivityMarks(mark);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return mark;
        }

        private DailyTest AddNewDailyTest(int classSectionId, int subjectId, DateTime testDate)
        {
            DailyTest mark = new DailyTest();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                mark.ClassSectionId = classSectionId;
                mark.SubjectId = subjectId;
                mark.TestDate = testDate;
                mark.TotalMarks = 0;
                mark.PassPercentage = 0;
                mark.CreatedOn = DateTime.Now;
                examRepo.AddDailyTest(mark);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return mark;
        }

        private void AddActivityMarksDetail(int classSectionId, int activityMarksId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var studentList = studentRepo.GetStudentByClassSectionId(classSectionId);
                if (studentList != null && studentList.Count > 0)
                {
                    foreach (var student in studentList)
                    {
                        ActivityMarksDetail detail = new ActivityMarksDetail();
                        detail.StudentId = student.Id;
                        detail.ObtMarks = 0;
                        detail.ActivityMarksId = activityMarksId;
                        detail.CreatedOn = DateTime.Now;

                        examRepo.AddActivityMarksDetail(detail);
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private void AddDailyTestDetail(int classSectionId, int dailyTestId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var studentList = studentRepo.GetStudentByClassSectionId(classSectionId);
                if (studentList != null && studentList.Count > 0)
                {
                    foreach (var student in studentList)
                    {
                        DailyTestsDetail detail = new DailyTestsDetail();
                        detail.StudentId = student.Id;
                        detail.ObtMarks = 0;
                        detail.DailyTestId = dailyTestId;
                        detail.CreatedOn = DateTime.Now;

                        examRepo.AddDailyTestDetail(detail);
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }


        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]Activity activity)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            bool errorFlag = false;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (ModelState.IsValid)
                {
                    if (examRepo.GetActivityByName(activity.ActivityName) == null)
                    {
                        activity.BranchId = branchId;
                        activity.Branch = null;

                        examRepo.Addactivity(activity);
                        errorCode = 2;
                        SessionHelper.InvalidateActivityCache = false;
                    }
                    else
                    {
                        errorCode = 11;
                        errorFlag = true;
                    }
                }
                else
                {
                    errorFlag = true;
                }
                if (errorFlag)
                {
                    ViewData["Error"] = errorCode;
                    ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                    ViewData["activities"] = SessionHelper.ActivityList(Session.SessionID);
                    return View(activity);
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


        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Activity activity)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            bool errorFlag = false;
            try
            {
                //int termPerecentage = (int)db.ExamTerms.Find(examterm.Id).Percentage;
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (ModelState.IsValid)
                {
                    if (examRepo.GetActivityByNameAndId(activity.ActivityName, activity.Id) == null)
                    {
                        activity.BranchId = branchId;
                        activity.Branch = null;

                        examRepo.UpdateActivity(activity);
                        errorCode = 2;
                        SessionHelper.InvalidateActivityCache = false;
                    }
                    else
                    {
                        errorCode = 11;
                        errorFlag = true;
                    }
                }
                else
                {
                    errorFlag = true;
                }
                if (errorFlag)
                {
                    ViewData["Error"] = errorCode;
                    ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                    ViewData["activities"] = SessionHelper.ActivityList(Session.SessionID);
                    ViewData["Operation"] = activity.Id;
                    return View(activity);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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

        //
        // GET: /ExamTerm/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            Activity activity = new Activity();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                activity = examRepo.GetActivityById(id);
                if (activity == null)
                {
                    return HttpNotFound();
                }

                int activityCount = examRepo.GetActivityCount(id);
                if (activityCount == 0)
                {
                    examRepo.DeleteActivity(activity);
                    errorCode = 4;
                    SessionHelper.InvalidateActivityCache = false;
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
            return RedirectToAction("Index", new { id = -59 });
        }


    }
}
