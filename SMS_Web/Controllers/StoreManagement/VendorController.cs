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

namespace SMS_Web.Controllers.StoreManagement
{
    public class VendorController : Controller
    {
        //private cs db = new cs();

        private IStoreRepository storeRepo;
        private static int errorCode = 0;
        //
        // GET: /Class/

        public VendorController()
        {
            storeRepo = new StoreRepositoryImp(new SC_WEBEntities2());
        }

        //[OutputCache(Duration = 300, VaryByParam = "none")]
        public ActionResult Index( int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.STM_VEDNORS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            Vendor vendor = new Vendor();
            try
            {
                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                ViewData["vendor"] = SessionHelper.VendorList();
                ViewData["Error"] = errorCode;
                errorCode = 0;

                vendor = storeRepo.GetVendorById(id);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return View(vendor);
        }

       
        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Exclude = "Id")]Vendor vendor)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var vendorObj = storeRepo.GetVendorByName(vendor.Name);
                if (ModelState.IsValid && vendorObj == null)
                {
                    vendor.CreatedOn = DateTime.Now;
                    int returnCode = storeRepo.AddVendor(vendor);
                    if (returnCode == -1)
                        errorCode = 420;
                    else
                        errorCode = 2;
                    SessionHelper.InvalidateVendorCache = false;
                }
                else if (vendorObj != null)
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

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Vendor vendor)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }


            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var storeObj = storeRepo.GetVendorByNameAndId(vendor.Name, vendor.Id);
                
                if (ModelState.IsValid && storeObj == null)
                {
                    storeRepo.UpdateVendor(vendor);
                    errorCode = 2;
                    SessionHelper.InvalidateVendorCache = false;
                }
                else if (storeObj != null)
                    errorCode = 11;
                else
                    errorCode = 1;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            Vendor vendor = storeRepo.GetVendorById(id);
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (vendor == null)
                {
                    return HttpNotFound();
                }

                storeRepo.DeleteVendor(vendor);
                SessionHelper.InvalidateVendorCache = false;
                errorCode = 4;
                
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex) { 
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 3;
            }
            return RedirectToAction("Index", new { id = 0 });
        }


    }
}