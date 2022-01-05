using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Helpers;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using SMS_DAL.Reports;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.StaffHandling
{
    public class StaffHolidayController : Controller
    {
        private static int errorCode = 0;

        
        IStaffRepository staffRepo;

        public StaffHolidayController()
        {
            
            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
        }

        //
        // GET: /Designation/

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_STAFF_HOLIDAYS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            StaffHoliday holiday = new StaffHoliday();
            try
            {
                var staffHolidays = SessionHelper.StaffHolidayList(Session.SessionID);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                ViewData["staffHolidays"] = staffHolidays;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(holiday);
        }

        //
        // POST: /Designation/Create

        [HttpPost]
        [ActionName("Index")]
        [SMS_Web.Filters.OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]StaffHoliday holiday)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var holidayObj = staffRepo.GetStaffHolidayByDate((DateTime)holiday.Date, branchId);
                if (ModelState.IsValid && holidayObj == null)
                {
                    LogWriter.WriteLog("Model state is valid, creating the designation");
                    holiday.BranchId = branchId;
                    holiday.CreatedOn = DateTime.Now;
                    staffRepo.AddStaffHolidays(holiday);
                    errorCode = 2;
                    SessionHelper.InvalidateDesignationCache = false;
                }
                else if (holidayObj != null)
                    errorCode = 11;
                else
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["designations"] = SessionHelper.DesignationList(Session.SessionID);
                    ViewBag.CatagoryId = new SelectList(SessionHelper.DesignationCatagoryList(Session.SessionID), "Id", "CatagoryName");
                    return View(holidayObj);
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

        
        //
        // GET: /Designation/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                StaffHoliday holiday = staffRepo.GetStaffHolidayById(id);
                if (holiday == null)
                {
                    return HttpNotFound();
                }
                staffRepo.DeleteStaffHoliday(holiday);
                SessionHelper.InvalidateStaffHolidayCache = false;
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
    }
}