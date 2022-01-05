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
    public class DesignationCatagoryController : Controller
    {
        private static int errorCode = 0;

        
        IStaffRepository staffRepo;

        public DesignationCatagoryController()
        {
            
            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());; 
        }

        //
        // GET: /DesignationCatagory/

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_DESIGNATION_CATEGORY) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            DesignationCatagory catagory = new DesignationCatagory();

            try
            {
                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                ViewData["designationCatagory"] = SessionHelper.DesignationCatagoryList(Session.SessionID);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                catagory = staffRepo.GetDesignationCategoryById(id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(catagory);
        }

        //
        // POST: /DesignationCatagory/Create

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "CatagroyId")]DesignationCatagory designationcatagory)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = staffRepo.GetDesignationCategoryByName(designationcatagory.CatagoryName, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    LogWriter.WriteLog("Model state is valid, creating the designation category");
                    designationcatagory.BranchId = branchId;
                    designationcatagory.Branch = null;

                    staffRepo.AddDesignationCategory(designationcatagory);
                    errorCode = 2;
                    SessionHelper.InvalidateDesignationCategoryCache = false;
                }
                else if (classObj != null)
                    errorCode = 11;
                else
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["designationCatagory"] = SessionHelper.DesignationCatagoryList(Session.SessionID);
                    return View(designationcatagory);
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
        // POST: /DesignationCatagory/Edit/5

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DesignationCatagory designationcatagory)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = staffRepo.GetDesignationCategoryByNameAndId(designationcatagory.CatagoryName, designationcatagory.Id, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    LogWriter.WriteLog("Model state is valid, updating the designation category");
                    designationcatagory.BranchId = branchId;
                    designationcatagory.Branch = null;

                    staffRepo.UpdateDesignationCategory(designationcatagory);
                    errorCode = 2;
                    SessionHelper.InvalidateDesignationCategoryCache = false;
                }
                else if (classObj != null)
                    errorCode = 11;
                else
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["designationCatagory"] = SessionHelper.DesignationCatagoryList(Session.SessionID);
                    ViewData["Operation"] = designationcatagory.Id;
                    return View(designationcatagory);
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
        // GET: /DesignationCatagory/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DesignationCatagory catagory = staffRepo.GetDesignationCategoryById(id);
                if (catagory == null)
                {
                    return HttpNotFound();
                }

                int designationCount = staffRepo.GetDesignationCount(id);
                if (designationCount == 0)
                {
                    staffRepo.DeleteDesignationCategory(catagory);
                    errorCode = 4;
                    SessionHelper.InvalidateDesignationCategoryCache = false;
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