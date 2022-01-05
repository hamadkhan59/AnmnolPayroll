using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_Web.Filters;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Helpers;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.StaffHandling
{
    public class AtteandancePolicyController : Controller
    {
        private static int errorCode = 0;
        //
        // GET: /AtteandancePolicy/


        IStaffRepository staffRepo;

        public AtteandancePolicyController()
        {
            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2()); ;
        }

        public ActionResult Index(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_STAFF_ATTENDANCE_POLICY) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                if (Id > 0)
                    errorCode = 0;
                ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryListDD(Session.SessionID), "Id", "CatagoryName");
                ViewData["policies"] = SessionHelper.StaffAttandancePolicyList(Session.SessionID);
                ViewData["Error"] = errorCode;
                ViewData["Operation"] = Id;
                errorCode = 0;
                if (Id > 0)
                {
                    var policy = staffRepo.GetStaffAttandancePolicyById(Id);
                    var design = staffRepo.GetDesignationById((int)policy.DesignationId);
                    //allowLeaveSelectedValue = (int)policy.LateInCountId;
                    //lateInSelectedValue = (int)policy.AllowLeaveId;
                    ViewBag.Designations = new SelectList(SessionHelper.DesignationList(Session.SessionID), "Id", "Name", policy.DesignationId);
                    ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryList(Session.SessionID), "Id", "CatagoryName", design.CatagoryId);
                    //ViewBag.AllowedLeaves = new SelectList(SessionHelper.AllowLeaveList, "Id", "Count", allowLeaveSelectedValue);
                    //ViewBag.LateInsCount = new SelectList(SessionHelper.LateInCountList, "Id", "Count", lateInSelectedValue);
                    return View(policy);
                }
                ViewBag.AllowedLeaves = new SelectList(SessionHelper.AllowLeaveList, "Id", "Count");
                ViewBag.LateInsCount = new SelectList(SessionHelper.LateInCountList, "Id", "Count");
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View("");
        }

        //
        // GET: /AtteandancePolicy/Create

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]StaffAttandancePolicy staffAttendancePolicy, string LateInTime, string HalfDayTime, string EarlyOutTime)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var policyObj = staffRepo.GetStaffAttandancePolicyByDesignationId((int)staffAttendancePolicy.Designation.Id, branchId);
                if (policyObj == null)
                {
                    staffAttendancePolicy.DesignationId = staffAttendancePolicy.Designation.Id;
                    staffAttendancePolicy.Designation = null;
                    staffAttendancePolicy.LateInTime = LateInTime;
                    staffAttendancePolicy.HalfDayTime = HalfDayTime;
                    staffAttendancePolicy.EarlyOutTime = EarlyOutTime;
                    staffAttendancePolicy.BranchId = branchId;
                    staffAttendancePolicy.Branch = null;

                    staffRepo.AddStaffAttandancePolicy(staffAttendancePolicy);
                    SessionHelper.InvalidateAttendancePolicyCache = false;
                    errorCode = 2;
                }
                else if (policyObj != null)
                    errorCode = 11;
                else
                    errorCode = 1;
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

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                StaffAttandancePolicy policy = staffRepo.GetStaffAttandancePolicyById(id);
                if (policy != null)
                {
                    staffRepo.DeleteStaffAttandancePolicy(policy);
                    errorCode = 4;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to Delete Staff Attendance Policy :" + ex.Message);
                errorCode = 3;
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StaffAttandancePolicy staffAttendancePolicy, string LateInTime, string HalfDayTime, string EarlyOutTime)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var policyObj = staffRepo.GetStaffAttandancePolicyByDesignationIdAndId((int)staffAttendancePolicy.Designation.Id, staffAttendancePolicy.Id, branchId);
                if (policyObj == null)
                {
                    staffAttendancePolicy.DesignationId = staffAttendancePolicy.Designation.Id;
                    staffAttendancePolicy.Designation = null;
                    staffAttendancePolicy.LateInTime = LateInTime;
                    staffAttendancePolicy.HalfDayTime = HalfDayTime;
                    staffAttendancePolicy.EarlyOutTime = EarlyOutTime;
                    staffAttendancePolicy.BranchId = branchId;
                    staffAttendancePolicy.Branch = null;

                    staffRepo.UpdateStaffAttandancePolicy(staffAttendancePolicy);
                    errorCode = 2;
                    SessionHelper.InvalidateAttendancePolicyCache = false;
                }
                else if (policyObj != null)
                    errorCode = 11;
                else
                    errorCode = 1;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index", new { Id = 0 });

        }

    }
}