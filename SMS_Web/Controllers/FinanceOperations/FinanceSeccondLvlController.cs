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
    public class FinanceSeccondLvlController : Controller
    {
        private IFinanceAccountRepository accountRepo;
        
        private static int errorCode = 0;
        static int typeAccountId;
        //
        // GET: /Class/

        public FinanceSeccondLvlController()
        {
            
            accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());;
        }

        //
        // GET: /FinanceSeccondLvl/

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FA_FINANCE_MAIN_ACCOUNT) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            FinanceSeccondLvlAccount account = new FinanceSeccondLvlAccount();
            try
            {
                int accountId = 0;
                if (typeAccountId > 0)
                {
                    accountId = typeAccountId;
                    typeAccountId = 0;
                }

                account = accountRepo.GetFinanceSeccondLvlAccountById(id);
                if (accountId == 0)
                    accountId = account == null ? 0 : account.FirstLvlAccountId;
                ViewBag.FirstLvlAccountId = new SelectList(SessionHelper.FinanceFirstLvlAccountList, "Id", "AccountName", accountId);

                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                ViewData["seccondLvlAccounts"] = SessionHelper.FinanceSeccondLvlAccountList;
                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(account);
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]

        public ActionResult Create(FinanceSeccondLvlAccount seccondLvlAccount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                seccondLvlAccount.FirstLvlAccountId = seccondLvlAccount.FinanceFirstLvlAccount.Id;
                var classObj = accountRepo.GetFinanceSeccondLvlAccountByName(seccondLvlAccount.AccountName);
                if (ModelState.IsValid && classObj == null && !string.IsNullOrEmpty(seccondLvlAccount.AccountName))
                {
                    seccondLvlAccount.FinanceFirstLvlAccount = null;
                    int returnCode = accountRepo.AddFinanceSeccondLvlAccount(seccondLvlAccount);
                    if (returnCode == -1)
                        errorCode = 420;
                    else
                        errorCode = 2;
                    SessionHelper.InvalidateFinanceSeccondLvlCache = false;
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
        public ActionResult Edit(FinanceSeccondLvlAccount seccondLvlAccount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                seccondLvlAccount.FirstLvlAccountId = seccondLvlAccount.FinanceFirstLvlAccount.Id;
                var classObj = accountRepo.GetFinanceSeccondLvlAccountByNameAndId(seccondLvlAccount.AccountName, seccondLvlAccount.Id);
                if (ModelState.IsValid && classObj == null)
                {
                    seccondLvlAccount.FinanceFirstLvlAccount = null;
                    accountRepo.UpdateFinanceSeccondLvlAccount(seccondLvlAccount);
                    errorCode = 2;
                    SessionHelper.InvalidateFinanceSeccondLvlCache = false;
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

        public ActionResult MainAccountRequest(int typeId)
        {
            typeAccountId = typeId;
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
                FinanceSeccondLvlAccount account = accountRepo.GetFinanceSeccondLvlAccountById(id);
                if (account == null)
                {
                    return HttpNotFound();
                }

                int thirdLvlCount = accountRepo.GetFinanceThirdLvlCount(id);

                if (thirdLvlCount == 0)
                {
                    accountRepo.DeleteFinanceSeccondLvlAccount(account);
                    errorCode = 4;
                    SessionHelper.InvalidateFinanceSeccondLvlCache = false;
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