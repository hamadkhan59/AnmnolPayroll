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
using SMS_Web.Filters;
using SMS_Web.Helpers;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.FinanceOperations
{
    public class FinanceThirdLvlController : Controller
    {
        private IFinanceAccountRepository accountRepo;
        
        private static int errorCode = 0;
        static int typeAccountId;
        //
        // GET: /Class/

        public FinanceThirdLvlController()
        {
            
            accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());;
        }


        //
        // GET: /FInanceThirdLvl/

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FA_FINANCE_SUB_ACCOUNT) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            FinanceThirdLvlAccount account = new FinanceThirdLvlAccount();
            try
            {
                int accountId = 0;
                if (typeAccountId > 0)
                {
                    accountId = typeAccountId;
                    typeAccountId = 0;
                }

                account = accountRepo.GetFinanceThirdLvlAccountById(id);
                if (accountId == 0)
                    accountId = account == null ? 0 : account.SeccondLvlAccountId;

                ViewBag.SeccondLvlAccountId = new SelectList(SessionHelper.FinanceSeccondLvlAccountList, "Id", "AccountName", accountId);

                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                ViewData["thirdLvlAccounts"] = SessionHelper.FinanceThirdLvlAccountList(Session.SessionID);
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

        public ActionResult Create(FinanceThirdLvlAccount thirdLvlAccount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                thirdLvlAccount.SeccondLvlAccountId = thirdLvlAccount.FinanceSeccondLvlAccount.Id;
                var classObj = accountRepo.GetFinanceThirdLvlAccountByName(thirdLvlAccount.AccountName, branchId);
                if (ModelState.IsValid && classObj == null && !string.IsNullOrEmpty(thirdLvlAccount.AccountName))
                {
                    thirdLvlAccount.FinanceSeccondLvlAccount = null;
                    thirdLvlAccount.BranchId = branchId;
                    thirdLvlAccount.Branch = null;
                    int returnCode = accountRepo.AddFinanceThirdLvlAccount(thirdLvlAccount);
                    if (returnCode == -1)
                        errorCode = 420;
                    else
                        errorCode = 2;
                    SessionHelper.InvalidateFinanceThirdLvlCache = false;
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
        public ActionResult Edit(FinanceThirdLvlAccount thirdLvlAccount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                thirdLvlAccount.SeccondLvlAccountId = thirdLvlAccount.FinanceSeccondLvlAccount.Id;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = accountRepo.GetFinanceThirdLvlAccountByNameAndId(thirdLvlAccount.AccountName, thirdLvlAccount.Id, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    thirdLvlAccount.BranchId = branchId;
                    thirdLvlAccount.Branch = null;

                    thirdLvlAccount.FinanceSeccondLvlAccount = null;
                    accountRepo.UpdateFinanceThirdLvlAccount(thirdLvlAccount);
                    errorCode = 2;
                    SessionHelper.InvalidateFinanceThirdLvlCache = false;
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

        public ActionResult SubAccountRequest(int typeId)
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
                FinanceThirdLvlAccount account = accountRepo.GetFinanceThirdLvlAccountById(id);
                if (account == null)
                {
                    return HttpNotFound();
                }

                int fourthLvlCount = accountRepo.GetFinanceFourthLvlCount(id);
                if (fourthLvlCount == 0)
                {
                    accountRepo.DeleteFinanceThirdLvlAccount(account);
                    errorCode = 4;
                    SessionHelper.InvalidateFinanceThirdLvlCache = false;
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