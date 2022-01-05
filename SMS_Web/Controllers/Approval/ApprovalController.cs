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

namespace SMS_Web.Controllers.Approval
{
    public class ApprovalController : Controller
    {
        //
        // GET: /Approval/

        private static int errorCode = -1;
        IStaffRepository staffRepo;
        IStudentRepository studentRepo;
        IClassSectionRepository classSecRepo;
        IAttendanceRepository attendanceRepo;
        public ApprovalController()
        {

            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2()); ;
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2()); ;
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            attendanceRepo = new AttendanceRepositoryImp(new SC_WEBEntities2()); ;
        }

        public ActionResult ApproveStudentAttendance()
        {
            ViewBag.StaffId = new SelectList(SessionHelper.StaffList(Session.SessionID), "StaffId", "Name");
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SECA_Branch) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<AttendanceRequestModel> list = new List<AttendanceRequestModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
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
                LogWriter.WriteLog("Record Count : " + (list == null ? 0 : list.Count()));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(list);
        }

        public ActionResult ApproveStaffAttendance()
        {
            ViewBag.StaffId = new SelectList(SessionHelper.StaffList(Session.SessionID), "StaffId", "Name");
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.AP_STAFF_ATTENDANCE_REQ) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<StaffAttendanceRequestModel> list = new List<StaffAttendanceRequestModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (Session[ConstHelper.STAFF_ATTENDANCE_APPROVE_REQUEST_LIST] != null)
                {
                    list = (List<StaffAttendanceRequestModel>)Session[ConstHelper.STAFF_ATTENDANCE_APPROVE_REQUEST_LIST];
                    Session[ConstHelper.STAFF_ATTENDANCE_APPROVE_REQUEST_LIST] = null;
                }
                else
                {
                    list = staffRepo.GetStaffAttendanceRequests(DateTime.Now, DateTime.Now, 0);
                }
                ViewData["Error"] = errorCode;
                errorCode = 0;
                LogWriter.WriteLog("Record Count : " + (list == null ? 0 : list.Count()));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(list);
        }

        [HttpPost]
        [ActionName("ApproveStaffAttendance")]
        [OnAction(ButtonName = "ApproveRequestList")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchAttendanceRequestList(DateTime FromDate, DateTime ToDate, int StaffId = 0)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                {
                    return RedirectToAction("Index", "Login");
                }

                List<StaffAttendanceRequestModel> list = staffRepo.GetStaffAttendanceRequests(FromDate, ToDate, StaffId);
                Session[ConstHelper.STAFF_ATTENDANCE_APPROVE_REQUEST_LIST] = list;
                LogWriter.WriteLog("Record Count : " + (list == null ? 0 : list.Count()));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 422;
            }
            return RedirectToAction("ApproveStaffAttendance");
        }

        public ActionResult ApprovalRequestDetail(int Id)
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
                LogWriter.WriteLog("Record Count : " + (list == null ? 0 : list.Count()));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(list);
        }

        public ActionResult StudentApprovalRequestDetail(int Id)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            List<AttendanceRequestModel> list = new List<AttendanceRequestModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                list = attendanceRepo.GetAttendanceRequests(Id);
                LogWriter.WriteLog("Record Count : " + (list == null ? 0 : list.Count()));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
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
        public ActionResult ApproveStaffAttendanceRequest(int Action, int RequestId, string Remarks)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                bool resetFlag = false;
                StaffAttendanceRequest request = staffRepo.GetStaffAttendanceRequest(RequestId);
                if (request.StatusId == ConstHelper.ATTENDANCE_REQUEST_APPROVED && Action == ConstHelper.ATTENDANCE_REQUEST_REJECTED)
                    resetFlag = true;

                request.StatusId = Action;
                request.Remarks = Remarks;
                request.UserId = UserPermissionController.GetUserId(Session.SessionID);
                staffRepo.UpdateStaffAttendanceRequest(request);

                if (Action == ConstHelper.ATTENDANCE_REQUEST_APPROVED)
                {
                    LogWriter.WriteLog("Approve the staff attendance request");
                    var list = staffRepo.GetStaffAttendanceRequests(RequestId);
                    foreach (var detail in list)
                    {
                        StaffAttandance attendance = staffRepo.GetStaffAttendanceById(detail.AttendanceId);
                        attendance.Status = detail.StatusId;
                        attendance.Time = detail.TimeIn;
                        attendance.OutTime = detail.TimeOut;

                        staffRepo.UpdateStaffAttendance(attendance);
                    }
                }

                if (resetFlag)
                {
                    LogWriter.WriteLog("Reset the staff attendance request");
                    var list = staffRepo.GetStaffAttendanceRequests(RequestId);
                    foreach (var detail in list)
                    {
                        StaffAttandance attendance = staffRepo.GetStaffAttendanceById(detail.AttendanceId);
                        attendance.Status = detail.AttendanceStatusId;
                        attendance.Time = detail.AttendanceTimeIn;
                        attendance.OutTime = detail.AttendanceTimeOut;

                        staffRepo.UpdateStaffAttendance(attendance);
                    }
                }

                errorCode = 20;
                if (Action != ConstHelper.ATTENDANCE_REQUEST_APPROVED)
                    errorCode = 21;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
                if (Action != ConstHelper.ATTENDANCE_REQUEST_APPROVED)
                    errorCode = 421;
            }
            return RedirectToAction("ApproveStaffAttendance");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveStudentAttendanceRequest(int Action, int RequestId, string Remarks)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                bool resetFlag = false;
                AttendanceRequest request = attendanceRepo.GetAttendanceRequest(RequestId);
                if (request.StatusId == ConstHelper.ATTENDANCE_REQUEST_APPROVED && Action == ConstHelper.ATTENDANCE_REQUEST_REJECTED)
                    resetFlag = true;

                request.StatusId = Action;
                request.Remarks = Remarks;
                request.UserId = UserPermissionController.GetUserId(Session.SessionID);
                attendanceRepo.UpdateAttendanceRequest(request);

                if (Action == ConstHelper.ATTENDANCE_REQUEST_APPROVED)
                {
                    LogWriter.WriteLog("Approve the student attendance request");
                    var list = attendanceRepo.GetAttendanceRequests(RequestId);
                    foreach (var detail in list)
                    {
                        Attandance attendance = attendanceRepo.GetAttandanceById(detail.AttendanceId);
                        attendance.StatusId = detail.StatusId;
                        attendanceRepo.UpdateAttendance(attendance);
                    }
                }

                if (resetFlag)
                {
                    LogWriter.WriteLog("Reset the student attendance request");
                    var list = attendanceRepo.GetAttendanceRequests(RequestId);
                    foreach (var detail in list)
                    {
                        Attandance attendance = attendanceRepo.GetAttandanceById(detail.AttendanceId);
                        attendance.StatusId = detail.AttendanceStatusId;
                        attendanceRepo.UpdateAttendance(attendance);
                    }
                }

                errorCode = 20;
                if (Action != ConstHelper.ATTENDANCE_REQUEST_APPROVED)
                    errorCode = 21;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
                if (Action != ConstHelper.ATTENDANCE_REQUEST_APPROVED)
                    errorCode = 421;
            }
            return RedirectToAction("ApproveStudentAttendance");
        }
    }
}
