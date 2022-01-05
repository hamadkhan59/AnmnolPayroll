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
    public class BankAccountController : Controller
    {
        private static int errorCode = 0;
        //
        // GET: /BankAccount/

        
        IFeePlanRepository feePlanRepo;
        IFinanceAccountRepository financeRepo;

        public BankAccountController()
        {

            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());;
            financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index(int Id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_BANK_ACCOUNT) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            BankAccount bank = new BankAccount();

            try
            {
                bank = feePlanRepo.GetBankAccountById(Id);
                int financeId = 0;
                if (Id > 0)
                {
                    financeId = (int)bank.FinanceAccountId;
                    errorCode = 0;
                }
                ViewData["Operation"] = Id;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.BankFourthLvlAccountId = new SelectList(financeRepo.GetFinanceAccounts("Bank", branchId), "Id", "AccountName", financeId);
                ViewBag.BankFourthLvlAccountId1 = new SelectList(financeRepo.GetFinanceAccounts("Bank", branchId), "Id", "AccountName");
                ViewData["BankAccounts"] = SessionHelper.BankAccountList(Session.SessionID);
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

        //
        // POST: /BankAccount/Create

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]BankAccount bank)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var tempObj = feePlanRepo.GetBankAccountByAccountNo(bank.AccountNo, branchId);
                if (ModelState.IsValid && tempObj == null)
                {
                    LogWriter.WriteLog("Model state is valid, creating bank account");
                    //bank.FinanceAccountId = bank.FinanceFourthLvlAccount.Id;
                    //bank.FinanceFourthLvlAccount = null;

                    bank.BranchId = branchId;
                    //bank.Branch = null;

                    feePlanRepo.AddBankAccount(bank);
                    CreateFinanceBankAccount(bank);
                    errorCode = 2;
                    SessionHelper.InvalidateBankAccountCache = false;
                    SessionHelper.InvalidateFeeAccountDetailCache = false;
                    SessionHelper.InvalidateFeeFinanceAccountCache = false;
                }
                else if (tempObj != null)
                    errorCode = 11;
                else
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["BankAccounts"] = SessionHelper.BankAccountList(Session.SessionID);
                    ViewBag.BankFourthLvlAccountId = new SelectList(financeRepo.GetFinanceAccounts("Bank", branchId), "Id", "AccountName");
                    ViewBag.BankFourthLvlAccountId1 = new SelectList(financeRepo.GetFinanceAccounts("Bank", branchId), "Id", "AccountName");
                    return View(bank);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", new { id = 0 });

        }

        private void CreateFinanceBankAccount(BankAccount account)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                FinanceFifthLvlAccount ffa = new FinanceFifthLvlAccount();
                ffa.AccountName = account.BankName + ", " + account.AccountNo;
                ffa.FourthLvlAccountId = (int)account.FinanceAccountId;
                ffa.CreatedOn = DateTime.Now;
                ffa.BranchId = account.BranchId;
                ffa.Value = 0;
                ffa.AccountDescription = "This bank account is created by system for finance operations";
                financeRepo.AddFinanceFifthLvlAccount(ffa);

                SessionHelper.InvalidateFinanceFifthLvlCache = false;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private void UpdateFinanceAccount(BankAccount account, string oldName, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                FinanceFifthLvlAccount accounts = financeRepo.GetFinanceFifthLvlAccountByName(oldName, branchId);
                accounts.AccountName = account.BankName + ", " + account.AccountNo;
                //accounts.AccountDescription = "Fee Recivables Account for : " + accounts.AccountName;
                financeRepo.UpdateFinanceFifthLvlAccount(accounts);
                SessionHelper.InvalidateFinanceFifthLvlCache = false;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        //
        // GET: /BankAccount/Edit/5

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BankAccount bankAccount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var tempObj = feePlanRepo.GetBankAccountByAccountNoAndId(bankAccount.AccountNo, bankAccount.Id, branchId);
                var oldInfo = feePlanRepo.GetBankAccountById(bankAccount.Id);
                if (ModelState.IsValid)
                {
                    LogWriter.WriteLog("Model state is valid, updating bank account");
                    //bankAccount.FinanceAccountId = bankAccount.FinanceFourthLvlAccount.Id;
                    //bankAccount.FinanceFourthLvlAccount = null;
                    bankAccount.BranchId = branchId;
                    //bankAccount.Branch = null;

                    string oldName = oldInfo.BankName + ", " + oldInfo.AccountNo;
                    string newName = bankAccount.BankName + ", " + bankAccount.AccountNo;
                    if (oldName != newName)
                    {
                        UpdateFinanceAccount(bankAccount, oldName, branchId);
                    }

                    feePlanRepo.UpdateBankAccount(bankAccount);
                    errorCode = 2;
                    SessionHelper.InvalidateBankAccountCache = false;
                    SessionHelper.InvalidateBankAccountCache = false;
                    SessionHelper.InvalidateFeeAccountDetailCache = false;
                    SessionHelper.InvalidateFeeFinanceAccountCache = false;
                }
                else if (tempObj != null)
                    errorCode = 11;
                else
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["Operation"] = bankAccount.Id;
                    ViewData["BankAccounts"] = SessionHelper.BankAccountList(Session.SessionID);
                    ViewBag.BankFourthLvlAccountId = new SelectList(financeRepo.GetFinanceAccounts("Bank", branchId), "Id", "AccountName");
                    ViewBag.BankFourthLvlAccountId1 = new SelectList(financeRepo.GetFinanceAccounts("Bank", branchId), "Id", "AccountName");
                    return View(bankAccount);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", new { id = 0 });

        }

        //
        // GET: /BankAccount/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            BankAccount bank = feePlanRepo.GetBankAccountById(id);
            string oldName = bank.BankName + ", " + bank.AccountNo;
            FinanceFifthLvlAccount accounts = financeRepo.GetFinanceFifthLvlAccountByName(oldName, (int)bank.BranchId);

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (bank == null)
                {
                    return HttpNotFound();
                }
                int entriesCount = financeRepo.GetEntriesCount(accounts.Id);
                int challanCount = financeRepo.GetIssueChallanCount(accounts.Id);
                FinanceFifthLvlAccount account = financeRepo.GetFinanceFifthLvlAccountById(accounts.Id);

                if (entriesCount == 0 && challanCount == 0)
                {
                    financeRepo.DeleteFinanceFifthLvlAccount(account);
                    feePlanRepo.DeleteBankAccount(bank);
                    errorCode = 4;
                    SessionHelper.InvalidateBankAccountCache = false;
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