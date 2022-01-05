using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_Web.Filters;
using System.Data.Objects;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_DAL.ViewModel;
using System.Globalization;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.StudentAdministration
{
    public class AttendanceController : Controller
    {
        private static int errorCode = -1;
        IClassRepository classRepo;
        IClassSectionRepository classSecRepo;
        ISectionRepository secRepo;
        IAttendanceRepository attendanceRepo;
        IStudentRepository studentRepo;
        ISecurityRepository securityRepo;
        IFeePlanRepository feePlanRepo;
        //
        // GET: /Attendance/

        public AttendanceController()
        {
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            attendanceRepo = new AttendanceRepositoryImp(new SC_WEBEntities2()); ;
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2()); ;
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2()); ;
            securityRepo = new SecurityRepositoryImp(new SC_WEBEntities2()); ;
        }

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_ATTENDANCE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            List<AttendanceModel> model = new List<AttendanceModel>();

            try
            {
                ViewData["Error"] = errorCode;
                errorCode = -1;

                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                if (Session[ConstHelper.ATTENDANCE_SEARCH_FLAG] != null && (bool)Session[ConstHelper.ATTENDANCE_SEARCH_FLAG] == true)
                    model = searchAttendance();

                voidSetSearchVeriables();
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return View(model);

        }

        public ActionResult Dashboard()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.AD_ATTENDANCE_DASHBOARD) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            return View();
        }

        public JsonResult GetAttendanceStatsByDate(string from = null, string to = null, string view = "day")
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DateTime fromDate = DateTime.Now.AddDays(DateTime.Now.Day * -1);
                if (from != null && from.Length > 24)
                    fromDate = DateTime.ParseExact(from.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                DateTime toDate = DateTime.Now;
                if (to != null && to.Length > 24)
                {
                    toDate = DateTime.ParseExact(to.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return Json(attendanceRepo.GetAttendanceStatsByDate(branchId, fromDate, toDate, view), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
                return null;
            }
        }

        public JsonResult GetAttendanceStatsByMonth(string from = null, string to = null, int? statusId=null, int? classId=null)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DateTime fromDate = DateTime.Now.AddDays(DateTime.Now.Day * -1);
                if (from != null && from.Length > 24)
                    fromDate = DateTime.ParseExact(from.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                DateTime toDate = DateTime.Now;
                if (to != null && to.Length > 24)
                {
                    toDate = DateTime.ParseExact(to.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                Session[ConstHelper.ATTENDANCE_SHEET_FROM_DATE] = fromDate;
                Session[ConstHelper.ATTENDANCE_SHEET_TO_DATE] = toDate;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return Json(attendanceRepo.GetAttendanceStatsByMonth(branchId, fromDate, toDate, statusId, classId), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
                return null;
            }
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

            if (Session[ConstHelper.ATTENDANCE_MARK_DATE] != null)
            {
                ViewData["AttendnaceDate"] = (DateTime)Session[ConstHelper.ATTENDANCE_MARK_DATE];
                Session[ConstHelper.ATTENDANCE_MARK_DATE] = null;
            }

            if (Session[ConstHelper.ATTENDANCE_SHEET_FROM_DATE] != null)
            {
                ViewData["AttendnaceFromDate"] = (DateTime)Session[ConstHelper.ATTENDANCE_SHEET_FROM_DATE];
                Session[ConstHelper.ATTENDANCE_SHEET_FROM_DATE] = null;
            }

            if (Session[ConstHelper.ATTENDANCE_SHEET_TO_DATE] != null)
            {
                ViewData["AttendnaceToDate"] = (DateTime)Session[ConstHelper.ATTENDANCE_SHEET_TO_DATE];
                Session[ConstHelper.ATTENDANCE_SHEET_TO_DATE] = null;
            }
        }


        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Mark")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchAttendanceDate(string ClassId, string SectionId, DateTime AttandanceDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (AttandanceDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    errorCode = 300;
                    LogWriter.WriteLog("Cannot Mark attendance as the selected day is sunday");
                }
                else
                {
                    int classId = 0, sectionId = 0;
                    classId = string.IsNullOrEmpty(ClassId) == true ? 0 : int.Parse(ClassId);
                    sectionId = string.IsNullOrEmpty(SectionId) == true ? 0 : int.Parse(SectionId);


                    Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                    Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
                    if (classId == 0 || sectionId == 0)
                        Session[ConstHelper.ATTENDANCE_CLASS_SECTION_ID] = 0;
                    else
                        Session[ConstHelper.ATTENDANCE_CLASS_SECTION_ID] = classSecRepo.GetClassSectionId(classId, sectionId);

                    Session[ConstHelper.ATTENDANCE_MARK_DATE] = AttandanceDate;
                    Session[ConstHelper.ATTENDANCE_SEARCH_FLAG] = true;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Submit")]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitAttendance(List<Attandance> attandance, int[] StatusId, int[] StudentId, DateTime[] Attendances)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int count = 0;

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                SchoolConfig schoolConfig = securityRepo.GetSchoolConfigByBranchId(branchId);
                bool smsflag = schoolConfig.SchoolName.ToLower().Contains("nusrat") ? true : false;
                foreach (var tempObj in attandance)
                {
                    //tempObj.StudentID = studentRepo.GetStudentId(tempObj.Student.RollNumber, tempObj.Student.Name, tempObj.Student.FatherName);
                    tempObj.StudentID = StudentId[count];
                    tempObj.AttandanceDate = Attendances[count];
                    //tempObj.Student = null;
                    tempObj.StatusId = StatusId[count];
                    tempObj.CreatedOn = DateTime.Now;
                    attendanceRepo.UpdateAttendance(tempObj);
                    SmsInfoProxy.sendSmsStudentAttendanceEvent((int)tempObj.StudentID, tempObj.StatusId, Attendances[count]);

                    if (tempObj.StatusId == 2 && smsflag)
                    {
                        //string message = "Dear Parents, Your child {detail} is absent from school today, Date : " + DateTime.Now;
                        string message = "Muhtram {parentdetail}! Apko itlaa di jati hai k {detail} aj school se ghair hazir hai, Date : " + DateTime.Now;
                        //SmsHelper.SendHajanaSMSToStudent((int)tempObj.StudentID, message);
                    }
                    count++;
                }
                errorCode = 100;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
                errorCode = 420;
            }
            return RedirectToAction("Index", new { id = 0 });
        }
        
        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "CreateSheet")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAttendanceSheet(string ClassId, string SectionId, DateTime FromDate, DateTime ToDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int classId = int.Parse(ClassId);
                int sectionId = int.Parse(SectionId);

                Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;

                Session[ConstHelper.ATTENDANCE_SHEET_CLASS_SECTION_ID] = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                Session[ConstHelper.ATTENDANCE_SHEET_FROM_DATE] = FromDate;
                Session[ConstHelper.ATTENDANCE_SHEET_TO_DATE] = ToDate;
                Session[ConstHelper.ATTENDANCE_SHEET_SEARCH_FLAG] = true;
                errorCode = 0;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
                errorCode = 420;
            }
            return RedirectToAction("AttendanceSheet");
        }

        public ActionResult CreateFilteredAttendanceSheet(int statusId, int classId, int sectionId, string from, string to)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DateTime fromDate = DateTime.Now.AddDays(DateTime.Now.Day * -1);
                if (from != null && from.Length > 24)
                    fromDate = DateTime.ParseExact(from.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                DateTime toDate = DateTime.Now;
                if (to != null && to.Length > 24)
                {
                    toDate = DateTime.ParseExact(to.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }

                Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
                Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;

                Session[ConstHelper.ATTENDANCE_SHEET_CLASS_SECTION_ID] = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
                Session[ConstHelper.ATTENDANCE_SHEET_FROM_DATE] = fromDate;
                Session[ConstHelper.ATTENDANCE_SHEET_TO_DATE] = toDate;
                Session[ConstHelper.ATTENDANCE_SHEET_SEARCH_FLAG] = true;
                Session["AttendanceStatusId"] = statusId;
                errorCode = 0;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
                errorCode = 420;
            }
            return RedirectToAction("AttendanceSheet");
        }

        private List<AttendanceModel> searchAttendance()
        {
            List<AttendanceModel> list = null;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int classSectionId = (int)Session[ConstHelper.ATTENDANCE_CLASS_SECTION_ID];
                DateTime markDate = (DateTime)Session[ConstHelper.ATTENDANCE_MARK_DATE];
                list = attendanceRepo.GetAttendanceByDate(classSectionId, markDate);

                List<StudentModel> studentList = studentRepo.GetStudentByClassSectionId(classSectionId);
                foreach (StudentModel std in studentList)
                {
                    if (list.Where(x => x.StudentId == std.Id).FirstOrDefault() == null)
                    {
                        Attandance tempObj = new Attandance();
                        tempObj.StudentID = std.Id;
                        tempObj.AttandanceDate = markDate;
                        tempObj.StatusId = 1;
                        attendanceRepo.AddAttendance(tempObj);
                    }
                }

                list = attendanceRepo.GetAttendanceByDate(classSectionId, markDate);
                Session[ConstHelper.ATTENDANCE_SEARCH_FLAG] = false;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }

            return list;
        }

        private List<AttendanceModel> createAttendanceSheet(int statusId = 0)
        {
            List<AttendanceModel> list = null;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (Session[ConstHelper.ATTENDANCE_SHEET_SEARCH_FLAG] != null && (bool)Session[ConstHelper.ATTENDANCE_SHEET_SEARCH_FLAG] == true)
                {
                    int classSectionId = Session[ConstHelper.ATTENDANCE_SHEET_CLASS_SECTION_ID] != null ? (int)Session[ConstHelper.ATTENDANCE_SHEET_CLASS_SECTION_ID] : 0;
                    DateTime fromDate = Session[ConstHelper.ATTENDANCE_SHEET_FROM_DATE] != null ? (DateTime)Session[ConstHelper.ATTENDANCE_SHEET_FROM_DATE] : DateTime.Now;
                    DateTime toDate = Session[ConstHelper.ATTENDANCE_SHEET_TO_DATE] != null ? (DateTime)Session[ConstHelper.ATTENDANCE_SHEET_TO_DATE] : DateTime.Now;

                    list = attendanceRepo.GetAttendanceSheetByDate(classSectionId, fromDate, toDate, statusId);
                }
                Session[ConstHelper.ATTENDANCE_SHEET_SEARCH_FLAG] = false;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return list;
        }

        public ActionResult AttendanceSheet(int statusId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_ATTENDANCE_SHEET) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                if (statusId <= 0 && Session["AttendanceStatusId"] != null)
                {
                    statusId = Int32.Parse(Session["AttendanceStatusId"].ToString());
                }

                ViewData["Error"] = errorCode;
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                //if (Session[ConstHelper.ATTENDANCE_SHEET_SEARCH_FLAG] != null && (bool)Session[ConstHelper.ATTENDANCE_SHEET_SEARCH_FLAG] == true)
                Session[ConstHelper.ATTENDANCE_SHEET_SEARCH_FLAG] = true;
                List<AttendanceModel> model = createAttendanceSheet(statusId);
                voidSetSearchVeriables();
                return View(model);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
                return View(new List<AttendanceModel>());
            }
        }

		public ActionResult ProceedMonth()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_PROCEED_ATTENDANCE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            ViewData["Error"] = errorCode;
            errorCode = 0;
            return View("");
        }

        [HttpPost]
        [ActionName("ProceedMonth")]
        [OnAction(ButtonName = "ProceedAttendnace")]
        [ValidateAntiForgeryToken]
        public ActionResult ProceedCurrentMonthAttendance(int Status, DateTime AttandanceDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (AttandanceDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    LogWriter.WriteLog("Cannot proceed attendance as the selected day is sunday");
                    errorCode = 300;
                }
                else
                {
                    feePlanRepo.ProceedCurrentMonthAttendance(Status, AttandanceDate);
                    errorCode = 100;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return RedirectToAction("ProceedMonth");
        }



        public ActionResult StudentAttendanceRequest(int StudentId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.AD_STAFF_ATTENDANCE_DASHBOARD) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<AttendanceModel> list = new List<AttendanceModel>();
            if (Session[ConstHelper.ATTENDANCE_REQUEST_LIST] != null)
            {
                list = (List<AttendanceModel>)Session[ConstHelper.ATTENDANCE_REQUEST_LIST];
                Session[ConstHelper.ATTENDANCE_REQUEST_LIST] = null;
            }
            
            var student = studentRepo.GetStudentById(StudentId);
            ViewData["AdmissionNo"] = student.AdmissionNo;
            ViewData["RollNumber"] = student.RollNumber;
            ViewData["Name"] = student.Name;
            ViewData["FatherName"] = student.FatherName;

            ViewData["Error"] = errorCode;
            ViewData["StudentId"] = StudentId;
            errorCode = 0;
            return View(list);
        }

        [HttpPost]
        [ActionName("StudentAttendanceRequest")]
        [OnAction(ButtonName = "StdAttendanceRequest")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchAttendanceForRequest(DateTime FromDate, DateTime ToDate, int StudentId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                List<AttendanceModel> list = attendanceRepo.GetStudentAttendanceByDate(StudentId, FromDate, ToDate);

                int days = (int)(ToDate - FromDate).TotalDays;
                if (list == null || list.Count == 0)
                {
                    DateTime tempDate = FromDate;
                    for (int i = 0; i <= days; i++)
                    {
                        if (tempDate.DayOfWeek != DayOfWeek.Sunday)
                        {
                            Attandance tempObj = new Attandance();
                            tempObj.StudentID = StudentId;
                            tempObj.AttandanceDate = tempDate;
                            tempObj.StatusId = 2;
                            attendanceRepo.AddAttendance(tempObj);
                        }
                        tempDate = tempDate.AddDays(1);
                    }
                }
                else
                {
                    DateTime tempDate = FromDate;
                    for (int i = 0; i <= days; i++)
                    {
                        var dateItem = list.Where(x => x.AttendanceDate.Date == tempDate.Date).FirstOrDefault();
                        if (tempDate.DayOfWeek != DayOfWeek.Sunday && dateItem == null)
                        {
                            Attandance tempObj = new Attandance();
                            tempObj.StudentID = StudentId;
                            tempObj.AttandanceDate = tempDate;
                            tempObj.StatusId = 2;
                            attendanceRepo.AddAttendance(tempObj);
                        }
                        tempDate = tempDate.AddDays(1);
                    }
                }

                list = attendanceRepo.GetStudentAttendanceByDate(StudentId, FromDate, ToDate);
                Session[ConstHelper.ATTENDANCE_REQUEST_LIST] = list;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }

            return RedirectToAction("StudentAttendanceRequest", new { StudentId = StudentId });
        }

        [HttpPost]
        [ActionName("StudentAttendanceRequest")]
        [OnAction(ButtonName = "SubmitStdAttendanceRequest")]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitStdAttendanceRequest(List<Attandance> attandance, int[] StatusId, int[] StudentId, DateTime[] Attendances, string [] Comments)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (attandance != null && attandance.Count > 0)
                {
                    AttendanceRequest request = new AttendanceRequest();
                    request.StatusId = ConstHelper.ATTENDANCE_REQUEST_PENDING;
                    request.StudentId = (int)attandance[0].StudentID;
                    request.CreatedOn = DateTime.Now;
                    //request.UserId = UserPermissionController.GetUserId(Session.SessionID);
                    attendanceRepo.AddAttendanceRequest(request);

                    int count = 0;
                    foreach (var att in attandance)
                    {
                        var tempAttendance = attendanceRepo.GetAttandanceById(att.id);
                        if (StatusId[count] == 1)
                        {
                            AttendanceRequestDetail detail = new AttendanceRequestDetail();
                            detail.AttendanceId = att.id;
                            detail.AttendanceRequesId = request.Id;
                            detail.CreatedOn = DateTime.Now;
                            detail.StatusId = StatusId[count];
                            detail.Comments = Comments[count];
                            detail.AttendanceStatusId = tempAttendance.StatusId;
                            attendanceRepo.AddAttendanceRequestDetail(detail);
                        }
                        else
                        {
                            //attendanceRepo.DeleteAttendance(tempAttendance);
                        }
                        count++;
                    }
                }

                errorCode = 100;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
                errorCode = 420;
            }
            return RedirectToAction("StudentRequestList", new { id = 0 });
        }

        public ActionResult StudentRequestList()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.AD_STAFF_ATTENDANCE_DASHBOARD) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<AttendanceRequestModel> list = new List<AttendanceRequestModel>();
            if (Session[ConstHelper.ATTENDANCE_REQUEST_LIST] != null)
            {
                list = (List<AttendanceRequestModel>)Session[ConstHelper.ATTENDANCE_REQUEST_LIST];
                Session[ConstHelper.ATTENDANCE_REQUEST_LIST] = null;
            }
            else
            {
                list = attendanceRepo.GetAttendanceRequests(DateTime.Now, DateTime.Now, 0);
            }
            ViewData["Error"] = errorCode;
            errorCode = 0;
            return View(list);
        }

        [HttpPost]
        [ActionName("StudentRequestList")]
        [OnAction(ButtonName = "AttendanceRequestList")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchAttendanceRequestList(DateTime FromDate, DateTime ToDate, int StudentId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                List<AttendanceRequestModel> list = attendanceRepo.GetAttendanceRequests(FromDate, ToDate, StudentId);
                Session[ConstHelper.ATTENDANCE_REQUEST_LIST] = list;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return RedirectToAction("StudentRequestList");
        }

        public ActionResult StudentRequestDetail(int Id)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            List<AttendanceRequestModel> list = attendanceRepo.GetAttendanceRequests(Id);
            return View(list);
        }
    }
}