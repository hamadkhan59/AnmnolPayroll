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

namespace SMS_Web.Controllers.FeeCollection
{
    public class CashAccountController : Controller
    {
        //
        // GET: /CashAccount/

        private static int errorCode = 0;
       
        IFeePlanRepository feePlanRepo;
        IFinanceAccountRepository financeRepo;

        public CashAccountController()
        {

            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());;
            financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2()); ;
        }


        public ActionResult Index(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_CASH_ACCOUNT) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            FinanceFifthLvlAccount bank = new FinanceFifthLvlAccount();

            try
            {
                bank = financeRepo.GetFinanceFifthLvlAccountById(Id);
                //var bank = feePlanRepo.GetBankAccountById(Id);
                int financeId = 0;
                if (Id > 0)
                {
                    financeId = (int)bank.FourthLvlAccountId;
                    errorCode = 0;
                }
                ViewData["Operation"] = Id;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.CashFourthLvlAccountId = new SelectList(financeRepo.GetFinanceAccounts("Cash", branchId), "Id", "AccountName", financeId);
                ViewBag.CashFourthLvlAccountId1 = new SelectList(financeRepo.GetFinanceAccounts("Cash", branchId), "Id", "AccountName");
                ViewData["CashAccounts"] = SessionHelper.FeeCashAccountList(Session.SessionID);
                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(bank);
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
                var classObj = financeRepo.GetFinanceFifthLvlAccountByName(fifthLvlAccount.AccountName, branchId);
                if (ModelState.IsValid && classObj == null && !string.IsNullOrEmpty(fifthLvlAccount.AccountName))
                {
                    LogWriter.WriteLog("Model state is valid, creating cash account");
                    fifthLvlAccount.FinanceFourthLvlAccount = null;
                    fifthLvlAccount.BranchId = branchId;
                    fifthLvlAccount.Branch = null;

                    int returnCode = financeRepo.AddFinanceFifthLvlAccount(fifthLvlAccount);
                    if (returnCode == -1)
                        errorCode = 420;
                    else
                        errorCode = 2;
                    SessionHelper.InvalidateBankAccountCache = false;
                    SessionHelper.InvalidateFeeAccountDetailCache = false;
                    SessionHelper.InvalidateFeeFinanceAccountCache = false;
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
                var classObj = financeRepo.GetFinanceFifthLvlAccountByNameAndId(fifthLvlAccount.AccountName, fifthLvlAccount.Id, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    LogWriter.WriteLog("Model state is valid, updating cash account");
                    fifthLvlAccount.FinanceFourthLvlAccount = null;
                    fifthLvlAccount.BranchId = branchId;
                    fifthLvlAccount.Branch = null;

                    financeRepo.UpdateFinanceFifthLvlAccount(fifthLvlAccount);
                    errorCode = 2;
                    SessionHelper.InvalidateBankAccountCache = false;
                    SessionHelper.InvalidateFeeAccountDetailCache = false;
                    SessionHelper.InvalidateFeeFinanceAccountCache = false;
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

            FinanceFifthLvlAccount account = financeRepo.GetFinanceFifthLvlAccountById(id);
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (account == null)
                {
                    return HttpNotFound();
                }
                int entriesCount = financeRepo.GetEntriesCount(account.Id);
                int challanCount = financeRepo.GetIssueChallanCount(account.Id);
                if (entriesCount == 0 && challanCount == 0)
                {
                    financeRepo.DeleteFinanceFifthLvlAccount(account);
                    errorCode = 4;
                    SessionHelper.InvalidateBankAccountCache = false;
                    SessionHelper.InvalidateFeeAccountDetailCache = false;
                    SessionHelper.InvalidateFeeFinanceAccountCache = false;
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
