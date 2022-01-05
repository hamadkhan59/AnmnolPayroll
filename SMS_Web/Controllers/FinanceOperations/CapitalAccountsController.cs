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
    public class CapitalAccountsController : Controller
    {
        private IFinanceAccountRepository accountRepo;
        
        private static int errorCode = 0;
        static int typeAccountId;
        //
        // GET: /Class/

        public CapitalAccountsController()
        {
            
            accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FA_CAPITAL_ACCOUNTS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            FinanceFifthLvlAccount account = new FinanceFifthLvlAccount();

            try
            {
                int accountId = 0;
                if (typeAccountId > 0)
                {
                    accountId = typeAccountId;
                    typeAccountId = 0;
                }

                account = accountRepo.GetFinanceFifthLvlAccountById(id);
                if (accountId == 0)
                    accountId = account == null ? 0 : account.FourthLvlAccountId;
                ViewBag.FourthLvlAccountId = new SelectList(SessionHelper.FourthLvlCapitalAccountList(Session.SessionID), "Id", "AccountName", accountId);
                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                ViewData["fifthLvlAccounts"] = SessionHelper.FifthCapitalLvlAccountList(Session.SessionID);
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

        public ActionResult Create(FinanceFifthLvlAccount fifthLvlAccount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                fifthLvlAccount.FourthLvlAccountId = fifthLvlAccount.FinanceFourthLvlAccount.Id;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = accountRepo.GetFinanceFifthLvlAccountByName(fifthLvlAccount.AccountName, branchId);
                if (ModelState.IsValid && classObj == null && !string.IsNullOrEmpty(fifthLvlAccount.AccountName))
                {
                    fifthLvlAccount.FinanceFourthLvlAccount = null;
                    fifthLvlAccount.BranchId = branchId;
                    fifthLvlAccount.Branch = null;
                    fifthLvlAccount.AccountType = ConstHelper.CAPITAL_ACCOUNT_TYPE;

                    int returnCode = accountRepo.AddFinanceFifthLvlAccount(fifthLvlAccount);
                    if (returnCode == -1)
                        errorCode = 420;
                    else
                        errorCode = 2;
                    SessionHelper.InvalidateFinanceFifthLvlCache = false;
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
        public ActionResult Edit(FinanceFifthLvlAccount fifthLvlAccount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                fifthLvlAccount.FourthLvlAccountId = fifthLvlAccount.FinanceFourthLvlAccount.Id;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = accountRepo.GetFinanceFifthLvlAccountByNameAndId(fifthLvlAccount.AccountName, fifthLvlAccount.Id, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    fifthLvlAccount.FinanceFourthLvlAccount = null;
                    fifthLvlAccount.BranchId = branchId;
                    fifthLvlAccount.Branch = null;
                    fifthLvlAccount.AccountType = ConstHelper.CAPITAL_ACCOUNT_TYPE;

                    accountRepo.UpdateFinanceFifthLvlAccount(fifthLvlAccount);
                    errorCode = 2;
                    SessionHelper.InvalidateFinanceFifthLvlCache = false;
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

        public ActionResult DetailAccountRequest(int typeId)
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

            FinanceFifthLvlAccount account = accountRepo.GetFinanceFifthLvlAccountById(id);
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (account == null)
                {
                    return HttpNotFound();
                }

                int entryCount = accountRepo.GetEntriesCount(id);
                int chalanCount = accountRepo.GetIssueChallanCount(id);

                if (entryCount == 0 && chalanCount == 0)
                {
                    accountRepo.DeleteFinanceFifthLvlAccount(account);
                    errorCode = 4;
                    SessionHelper.InvalidateFinanceFifthLvlCache = false;
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