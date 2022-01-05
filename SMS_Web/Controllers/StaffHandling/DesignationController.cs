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
    public class DesignationController : Controller
    {
        private static int errorCode = 0;

        
        IStaffRepository staffRepo;

        public DesignationController()
        {
            
            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());;
        }

        //
        // GET: /Designation/

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_DESIGNATION) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            Designation catagory = new Designation();

            try
            {
                if (id > 0)
                    errorCode = 0;
                var designations = SessionHelper.DesignationList(Session.SessionID);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                ViewData["designations"] = designations;
                ViewBag.CatagoryId = new SelectList(SessionHelper.DesignationCatagoryList(Session.SessionID), "Id", "CatagoryName");
                catagory = staffRepo.GetDesignationById(id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(catagory);
        }

        //
        // POST: /Designation/Create

        [HttpPost]
        [ActionName("Index")]
        [SMS_Web.Filters.OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]Designation designation)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = staffRepo.GetDesignationByNameAndId((string)designation.Name, (int)designation.CatagoryId, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    LogWriter.WriteLog("Model state is valid, creating the designation");
                    designation.BranchId = branchId;
                    designation.Branch = null;

                    staffRepo.AddDesignation(designation);
                    errorCode = 2;
                    SessionHelper.InvalidateDesignationCache = false;
                }
                else if (classObj != null)
                    errorCode = 11;
                else
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["designations"] = SessionHelper.DesignationList(Session.SessionID);
                    ViewBag.CatagoryId = new SelectList(SessionHelper.DesignationCatagoryList(Session.SessionID), "Id", "CatagoryName");
                    return View(designation);
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
                Designation designation = staffRepo.GetDesignationById(id);
                if (designation == null)
                {
                    return HttpNotFound();
                }
                int staffCount = staffRepo.GetStaffCount(id);

                if (staffCount == 0)
                {
                    staffRepo.DeleteDesignation(designation);
                    errorCode = 4;
                    SessionHelper.InvalidateDesignationCache = false;
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
            return RedirectToAction("Index", new { id = 0 });
        }

        
    }
}