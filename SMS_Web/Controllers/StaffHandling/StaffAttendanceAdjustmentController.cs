using System;
using System.Collections.Generic;
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
using System.Web.Services;
using System.Web.Script.Services;

namespace SMS_Web.Controllers.StaffHandling
{
    public class StaffAttendanceAdjustmentController : Controller
    {
        //
        // GET: /StaffAttendanceAdjustment/
        static int errorCode = 0;
        IStaffRepository staffRepo;
        public StaffAttendanceAdjustmentController()
        {

            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2()); 
        }

        public ActionResult Index(int id = 0)
        {
            List<StaffAttendanceDetailModel> list = new List<StaffAttendanceDetailModel>();

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.StaffNames = SessionHelper.GetStaffNames(Session.SessionID);
                if (Session[ConstHelper.ATTENDANCE_DETAIL_LIST] != null)
                {
                    list = (List<StaffAttendanceDetailModel>)Session[ConstHelper.ATTENDANCE_DETAIL_LIST];
                    Session[ConstHelper.ATTENDANCE_DETAIL_LIST] = null;
                }
                if (Session[ConstHelper.ATTENDANCE_ID] != null)
                {
                    ViewData["AttendanceId"] = (int) Session[ConstHelper.ATTENDANCE_ID];
                    Session[ConstHelper.ATTENDANCE_ID] = null;
                }
                else
                {
                    ViewData["AttendanceId"] = 0;
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

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "LoadAttendanceDetail")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchStaffAttendanceDetail(DateTime AttandanceDate, int StaffId = 0)
        {
            if (AttandanceDate > DateTime.Now)
            {
                errorCode = 120;
                return RedirectToAction("Index");
            }
            else
            {
                var attendance = staffRepo.SearchStaffDailyAttendance(AttandanceDate, StaffId);
                if (attendance == null)
                {
                    attendance = new StaffAttandance();
                    attendance.StaffId = StaffId;
                    attendance.Time = "08:00";
                    attendance.OutTime = "20:00";
                    attendance.Date = AttandanceDate;
                    attendance.CreatedOn = DateTime.Now;
                    attendance.Status = 2;
                    staffRepo.AddStaffAttendnace(attendance);

                    StaffAttendanceDetail detail = new StaffAttendanceDetail();
                    detail.AttendanceId = attendance.Id;
                    detail.TimeIn = "08:00";
                    detail.TimeOut = "20:00";
                    staffRepo.AddStaffAttendnaceDetail(detail);   
                }
                var list = staffRepo.GetStaffAttendnaceDetailByStaffId(AttandanceDate, StaffId);
                Session[ConstHelper.ATTENDANCE_DETAIL_LIST] = list;
                Session[ConstHelper.ATTENDANCE_ID] = attendance.Id;

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [WebMethod()]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteAttendanceDetail(int attendnaceDetailId)
        {
            var detail = staffRepo.GetStaffAttendanceDetailById(attendnaceDetailId);
            staffRepo.DeleteStaffAttendanceDetail(detail);
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "saveStaffAttendanceDetail")]
        [ValidateAntiForgeryToken]
        public ActionResult SaveStaffAttendanceDetail(int AttendanceId, int StatusId, int [] DetailIds, string [] startTimes, string [] endTimes)
        {
            try
            {
                if (AttendanceId > 0)
                {
                    if (DetailIds != null && DetailIds.Count() > 0)
                    {
                        for (int i = 0; i < DetailIds.Count(); i++)
                        {
                            int detailId = DetailIds[i];
                            if (detailId > 0)
                            {
                                var attendanceDetal = staffRepo.GetStaffAttendanceDetailById(detailId);
                                attendanceDetal.TimeIn = startTimes[i];
                                attendanceDetal.TimeOut = endTimes[i];
                                staffRepo.UpdateStaffAttendanceDetail(attendanceDetal);
                            }
                            else
                            {
                                StaffAttendanceDetail detail = new StaffAttendanceDetail();
                                detail.AttendanceId = AttendanceId;
                                detail.TimeIn = startTimes[i];
                                detail.TimeOut = endTimes[i];
                                staffRepo.AddStaffAttendnaceDetail(detail);
                            }
                        }

                        var attendance = staffRepo.GetStaffAttendanceById(AttendanceId);
                        attendance.Status = StatusId;
                        staffRepo.UpdateStaffAttendance(attendance);
                        errorCode = 100;
                    }
                    else
                    {
                        errorCode = 12;
                    }
                    
                }
                else
                {
                    errorCode = 10;
                }
            }
            catch(Exception ex)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index");
        }
    }
}
