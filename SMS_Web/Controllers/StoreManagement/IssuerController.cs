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
    public class IssuerController : Controller
    {
        //private cs db = new cs();

        private IStoreRepository storeRepo;
        private static int errorCode = 0;
        //
        // GET: /Class/

        public IssuerController()
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

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.STM_ISSUERS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            Issuer issuer = new Issuer();
            try
            {
                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                ViewData["issuer"] = SessionHelper.IssuerList();
                ViewData["Error"] = errorCode;
                errorCode = 0;

                issuer = storeRepo.GetIssuerById(id);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return View(issuer);
        }

       
        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Exclude = "Id")]Issuer issuer)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var issuerObj = storeRepo.GetIssuerByName(issuer.Name);
                if (ModelState.IsValid && issuerObj == null)
                {
                    issuer.CreatedOn = DateTime.Now;
                    int returnCode = storeRepo.AddIssuer(issuer);
                    if (returnCode == -1)
                        errorCode = 420;
                    else
                        errorCode = 2;
                    SessionHelper.InvalidateIssuerCache = false;
                }
                else if (issuerObj != null)
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
        public ActionResult Edit(Issuer issuer)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }


            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var storeObj = storeRepo.GetIssuerByNameAndId(issuer.Name, issuer.Id);
                
                if (ModelState.IsValid && storeObj == null)
                {
                    storeRepo.UpdateIssuer(issuer);
                    errorCode = 2;
                    SessionHelper.InvalidateIssuerCache = false;
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

            Issuer issuer = storeRepo.GetIssuerById(id);
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (issuer == null)
                {
                    return HttpNotFound();
                }

                storeRepo.DeleteIssuer(issuer);
                SessionHelper.InvalidateIssuerCache = false;
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