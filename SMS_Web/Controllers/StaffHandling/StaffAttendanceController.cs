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
using System.Globalization;
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.StaffHandling
{
    public class StaffAttendanceController : Controller
    {

        private static int errorCode = -1;
        //
        // GET: /StaffAttendance/

        IStaffRepository staffRepo;
        public StaffAttendanceController()
        {

            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2()); ;
        }

        public ActionResult Index()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_STAFF_ATTENDANCE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<StaffAttandanceModel> list = new List<StaffAttandanceModel>();

            try
            {
                ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryListDD(Session.SessionID), "Id", "CatagoryName");
                ViewData["Error"] = errorCode;
                errorCode = 0;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (Session[ConstHelper.SALARY_ATTENDANCE_SEARCH_FLAG] != null && (bool)Session[ConstHelper.SALARY_ATTENDANCE_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.SALARY_ATTENDANCE_SEARCH_FLAG] = false;
                    list = searchAttendance(branchId, Session.SessionID);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return View(list);
        }

        public ActionResult AttendanceRequest()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.AD_STAFF_ATTENDANCE_DASHBOARD) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<StaffAttandanceModel> list = new List<StaffAttandanceModel>();

            try
            {
                ViewBag.StaffId = new SelectList(SessionHelper.StaffList(Session.SessionID), "StaffId", "Name");

                if (Session[ConstHelper.STAFF_ATTENDANCE_REQUEST] != null)
                {
                    list = (List<StaffAttandanceModel>)Session[ConstHelper.STAFF_ATTENDANCE_REQUEST];
                    Session[ConstHelper.STAFF_ATTENDANCE_REQUEST] = null;
                }

                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(list);
        }

        public ActionResult RequestList()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.AD_STAFF_ATTENDANCE_DASHBOARD) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<StaffAttendanceRequestModel> list = new List<StaffAttendanceRequestModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ViewBag.StaffId = new SelectList(SessionHelper.StaffList(Session.SessionID), "StaffId", "Name");

                if (Session[ConstHelper.STAFF_ATTENDANCE_REQUEST_LIST] != null)
                {
                    list = (List<StaffAttendanceRequestModel>)Session[ConstHelper.STAFF_ATTENDANCE_REQUEST_LIST];
                    Session[ConstHelper.STAFF_ATTENDANCE_REQUEST_LIST] = null;
                }
                else
                {
                    list = staffRepo.GetStaffAttendanceRequests(DateTime.Now, DateTime.Now, 0);
                }
                ViewData["Error"] = errorCode;
                errorCode = 0;
                LogWriter.WriteLog("Request List Count : " + (list == null ? 0 : list.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(list);
        }

        public ActionResult RequestDetail(int Id)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            List<StaffAttendanceRequestModel> list = new List<StaffAttendanceRequestModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                list = staffRepo.GetStaffAttendanceRequests(Id);
                LogWriter.WriteLog("Request Detail List Count : " + (list == null ? 0 : list.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(list);
        }

        public ActionResult Dashboard()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.AD_STAFF_ATTENDANCE_DASHBOARD) == false)
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
                return Json(staffRepo.GetAttendanceStatsByDate(branchId, fromDate, toDate, view), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }

        public JsonResult GetAttendanceStatsByMonth(string from = null, string to = null)
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
                Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_FROM_DATE] = fromDate;
                Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_TO_DATE] = toDate;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return Json(staffRepo.GetAttendanceStatsByMonth(branchId, fromDate, toDate), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }


        [HttpPost]
        [ActionName("AttendanceRequest")]
        [OnAction(ButtonName = "AttendanceRequest1")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchAttendanceForRequest(DateTime FromDate, DateTime ToDate, int StaffId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                List<StaffAttandanceModel> list = staffRepo.SearchStaffAttendnaceModel(FromDate, ToDate, StaffId);

                int days = (int)(ToDate - FromDate).TotalDays;
                if (list == null || list.Count == 0)
                {
                    DateTime tempDate = FromDate;
                    for (int i = 0; i <= days; i++)
                    {
                        if (tempDate.DayOfWeek != DayOfWeek.Sunday)
                        {
                            StaffAttandance tempObj = new StaffAttandance();
                            tempObj.StaffId = StaffId;
                            tempObj.Date = tempDate;
                            tempObj.Time = "09:00";
                            tempObj.Status = 2;
                            staffRepo.AddStaffAttendnace(tempObj);
                        }
                        tempDate = tempDate.AddDays(1);
                    }
                }
                else
                {
                    DateTime tempDate = FromDate;
                    for (int i = 0; i <= days; i++)
                    {
                        var dateItem = list.Where(x => x.Date.Value.Date == tempDate.Date).FirstOrDefault();
                        if (tempDate.DayOfWeek != DayOfWeek.Sunday && dateItem == null)
                        {
                            StaffAttandance tempObj = new StaffAttandance();
                            tempObj.StaffId = StaffId;
                            tempObj.Date = tempDate;
                            tempObj.Time = "09:00";
                            tempObj.Status = 2;
                            staffRepo.AddStaffAttendnace(tempObj);
                        }
                        tempDate = tempDate.AddDays(1);
                    }
                }

                list = staffRepo.SearchStaffAttendnaceModel(FromDate, ToDate, StaffId);
                Session[ConstHelper.STAFF_ATTENDANCE_REQUEST] = list;
                LogWriter.WriteLog("Search Attendance For Request List Count : " + (list == null ? 0 : list.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("AttendanceRequest");
        }

        [HttpPost]
        [ActionName("RequestList")]
        [OnAction(ButtonName = "AttendanceRequestList")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchAttendanceRequestList(DateTime FromDate, DateTime ToDate, int StaffId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                List<StaffAttendanceRequestModel> list = staffRepo.GetStaffAttendanceRequests(FromDate, ToDate, StaffId);
                Session[ConstHelper.STAFF_ATTENDANCE_REQUEST_LIST] = list;
                LogWriter.WriteLog("Search Attendance Request List Count : " + (list == null ? 0 : list.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("RequestList");
        }

        [HttpPost]
        [ActionName("AttendanceRequest")]
        [OnAction(ButtonName = "SubmitAttendanceRequest")]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitAttendanceRequest(List<StaffAttandance> staffAttandance, string[] startTimes, int[] StatusId, string[] endTimes, string [] Comments)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (staffAttandance != null && staffAttandance.Count > 0)
                {
                    StaffAttendanceRequest request = new StaffAttendanceRequest();
                    request.StatusId = ConstHelper.ATTENDANCE_REQUEST_PENDING;
                    request.StaffId = (int) staffAttandance[0].StaffId;
                    request.CreatedOn = DateTime.Now;
                    //request.UserId = UserPermissionController.GetUserId(Session.SessionID);
                    staffRepo.AddStaffAttendanceRequest(request);

                    int count = 0;
                    foreach (var attendance in staffAttandance)
                    {
                        var tempAttendance = staffRepo.GetStaffAttendanceById(attendance.Id);
                        if (StatusId[count] == 1)
                        {
                            StaffAttendanceRequestDetail detail = new StaffAttendanceRequestDetail();
                            detail.AttendanceId = attendance.Id;
                            detail.StaffAttendanceRequestId = request.Id;
                            detail.CreatedOn = DateTime.Now;
                            detail.InTime = startTimes[count];
                            detail.OutTime = endTimes[count];
                            detail.StatusId = StatusId[count];
                            detail.Comments = Comments[count];
                            detail.AttendanceInTime = tempAttendance.Time;
                            detail.AttendanceOutTime = tempAttendance.OutTime;
                            detail.AttendanceStatusId = tempAttendance.Status;
                            staffRepo.AddStaffAttendanceRequestDetail(detail);
                        }
                        else
                        {
                            //staffRepo.DeleteStaffAttendance(tempAttendance);
                        }
                        count++;
                    }
                }
                errorCode = 2;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("AttendanceRequest", new { id = 0 });
        }

        //
        // GET: /StaffAttendance/Details/5

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Mark")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchAttendanceDate(int? CatagoryId, int? DesignationId, DateTime AttandanceDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                //if (AttandanceDate.DayOfWeek == DayOfWeek.Sunday)
                //{
                //    errorCode = 300;
                //    return RedirectToAction("Index", new { id = 0 });
                //}
                //else
                //{
                    Session[ConstHelper.SALARY_ATTENDANCE_SEARCH_CATEGORY_ID] = Session[ConstHelper.SALARY_ATTENDANCE_SEARCH_DESIGNATION_ID] = 0;
                    if (CatagoryId != null && CatagoryId > 0)
                        Session[ConstHelper.SALARY_ATTENDANCE_SEARCH_CATEGORY_ID] = (int)CatagoryId;
                    if (DesignationId != null && DesignationId > 0)
                        Session[ConstHelper.SALARY_ATTENDANCE_SEARCH_DESIGNATION_ID] = (int)DesignationId;
                    Session[ConstHelper.SALARY_ATTENDANCE_MARK_DATE] = AttandanceDate;
                    Session[ConstHelper.SALARY_ATTENDANCE_SEARCH_FLAG] = true;
                //}
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        //
        // POST: /StaffAttendance/Edit/5

        private List<StaffAttandanceModel> searchAttendance(int branchId, string browserDetail)
        {
            List<StaffAttandanceModel> list = null;

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int searchCatagoryId = (int)Session[ConstHelper.SALARY_ATTENDANCE_SEARCH_CATEGORY_ID];
                int searchDesignationId = (int)Session[ConstHelper.SALARY_ATTENDANCE_SEARCH_DESIGNATION_ID];
                DateTime markDate = (DateTime)Session[ConstHelper.SALARY_ATTENDANCE_MARK_DATE];
                
                list = staffRepo.SearchStaffAttendnaceModel(markDate, searchCatagoryId, searchDesignationId, branchId);

                List<Staff> staffList = SessionHelper.StaffList(browserDetail);
                foreach (Staff staff in staffList)
                {
                    if (list.Where(x => x.StaffId == staff.StaffId).FirstOrDefault() == null)
                    {
                        StaffAttandance tempObj = new StaffAttandance();
                        tempObj.StaffId = staff.StaffId;
                        tempObj.Date = markDate;
                        tempObj.Time = "09:00";
                        tempObj.Status = 2;
                        staffRepo.AddStaffAttendnace(tempObj);
                        SmsInfoProxy.sendSmsStaffAttendanceEvent(tempObj);
                    }
                }

                list = staffRepo.SearchStaffAttendnaceModel(markDate, searchCatagoryId, searchDesignationId, branchId);
                LogWriter.WriteLog("Search Attendance List Count : " + (list == null ? 0 : list.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return list;
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Submit")]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitAttendance(List<StaffAttandance> staffAttandance, string[] startTimes, int[] StatusId, string[] endTimes)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int count = 0;
                foreach (var tempObj in staffAttandance)
                {
                    tempObj.Staff = null;
                    tempObj.Time = startTimes[count];
                    tempObj.OutTime = endTimes[count];
                    tempObj.Status = StatusId[count];
                    tempObj.CreatedOn = DateTime.Now;
                    var attendance = staffRepo.SearchStaffDailyAttendance((DateTime)tempObj.Date, (int)tempObj.StaffId);

                    if (attendance.Time != tempObj.Time)
                    {
                        StaffAttendanceDetail detail = staffRepo.GetFirstStaffAttendanceDetailByAttId(tempObj.Id);
                        detail.TimeIn = tempObj.Time;
                        staffRepo.UpdateStaffAttendanceDetail(detail);
                    }

                    if (attendance.OutTime != tempObj.OutTime)
                    {
                        StaffAttendanceDetail detail = staffRepo.GetTopStaffAttendanceDetailByAttId(tempObj.Id);
                        detail.TimeOut = tempObj.OutTime;
                        staffRepo.UpdateStaffAttendanceDetail(detail);
                    }

                    staffRepo.UpdateStaffAttendance(tempObj);

                    SmsInfoProxy.sendSmsStaffAttendanceEvent(tempObj);
                    count++;
                }
                errorCode = 2;
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



        public ActionResult AttendanceSheet(int statusId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_STAFF_ATTENDANCE_SHEET) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            List<SMS_DAL.ViewModel.StaffAttandanceModel> list = new List<SMS_DAL.ViewModel.StaffAttandanceModel>();

            try
            {
                ViewData["Error"] = errorCode;
                ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryListDD(Session.SessionID), "Id", "CatagoryName");
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                //if (Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_FLAG] != null && (bool)Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_FLAG] = false;
                    list = createAttendanceSheet(branchId, statusId);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(list);
        }

        private List<StaffAttandanceModel> createAttendanceSheet(int branchId, int statusId = 0)
        {
            List<StaffAttandanceModel> list = new List<StaffAttandanceModel>();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int searchCatagoryId = Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_CATEGORY_ID] != null ? (int)Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_CATEGORY_ID] : 0;
                int searchDesignationId = Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_DESIGNATION_ID] != null ? (int)Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_DESIGNATION_ID] : 0;
                DateTime fromDate = Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_FROM_DATE] != null ? (DateTime)Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_FROM_DATE] : DateTime.Now;
                DateTime toDate = Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_TO_DATE] != null ? (DateTime)Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_TO_DATE] : DateTime.Now;
                int staffId = Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_STAFF_ID] != null ? (int)Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_STAFF_ID] : 0;

                list = staffRepo.SearchStaffAttendnaceModel(fromDate, toDate, searchCatagoryId, searchDesignationId, staffId, branchId, statusId);
                //sheetFlag = false;
                LogWriter.WriteLog("Search Attendance Sheet List Count : " + (list == null ? 0 : list.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return list;
        }

        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "CreateSheet")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAttendanceSheet(DateTime FromDate, DateTime ToDate, int? staffId, int? CatagoryId, int? DesignationId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                if (staffId == null)
                    staffId = 0;

                if (CatagoryId == null)
                    CatagoryId = 0;
                if (DesignationId == null)
                    DesignationId = 0;

                Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_STAFF_ID] = (int)staffId;
                Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_FROM_DATE] = FromDate;
                Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_TO_DATE] = ToDate;
                Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_CATEGORY_ID] = (int)CatagoryId;
                Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_DESIGNATION_ID] = (int)DesignationId;
                Session[ConstHelper.STAFF_ATTENDANCE_SHEET_SEARCH_FLAG] = true;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("AttendanceSheet");
        }


    }
}