using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Filters;
using SMS_Web.Helpers;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.FinanceOperations
{
    public class FinanceFirstLvlController : Controller
    {

        private IFinanceAccountRepository accountRepo;
        
        private static int errorCode = 0;
        //
        // GET: /Class/

        public FinanceFirstLvlController()
        {
            
            accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());;
        }

        //
        // GET: /FinanceFirstLvl/

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FA_FINANCE_CATEGORY) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            FinanceFirstLvlAccount account = new FinanceFirstLvlAccount();

            try
            {
                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                ViewData["firstLvlccounts"] = SessionHelper.FinanceFirstLvlAccountList;
                ViewData["Error"] = errorCode;
                errorCode = 0;
                account = accountRepo.GetFinanceFirstLvlAccountById(id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(account);
        }

        

        //
        // GET: /FinanceFirstLvl/Details/5

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]

        public ActionResult Create(FinanceFirstLvlAccount firstLvlAccount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var classObj = accountRepo.GetFinanceFirstLvlAccountByName(firstLvlAccount.AccountName);
                if (ModelState.IsValid && classObj == null && !string.IsNullOrEmpty(firstLvlAccount.AccountName))
                {
                    int returnCode = accountRepo.AddFinanceFirstLvlAccount(firstLvlAccount);
                    if (returnCode == -1)
                        errorCode = 420;
                    else
                        errorCode = 2;
                    SessionHelper.InvalidateFinanceFirstLvlCache = false;
                }
                else if (classObj != null)
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
        public ActionResult Edit(FinanceFirstLvlAccount firstLvlAccount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var classObj = accountRepo.GetFinanceFirstLvlAccountByNameAndId(firstLvlAccount.AccountName, firstLvlAccount.Id);
                if (ModelState.IsValid && classObj == null)
                {
                    accountRepo.UpdateFinanceFirstLvlAccount(firstLvlAccount);
                    errorCode = 2;
                    SessionHelper.InvalidateFinanceFirstLvlCache = false;
                }
                else if (classObj != null)
                    errorCode = 11;
                else
                    errorCode = 1;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                errorCode = 420;
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

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                FinanceFirstLvlAccount account = accountRepo.GetFinanceFirstLvlAccountById(id);
                if (account == null)
                {
                    return HttpNotFound();
                }

                int seccondLvlCount = accountRepo.GetFinanceSeccondLvlCount(id);
                if (seccondLvlCount == 0)
                {
                    accountRepo.DeleteFinanceFirstLvlAccount(account);
                    errorCode = 4;
                    SessionHelper.InvalidateFinanceFirstLvlCache = false;
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