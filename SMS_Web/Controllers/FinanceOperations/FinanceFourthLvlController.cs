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
    public class FinanceFourthLvlController : Controller
    {
        private IFinanceAccountRepository accountRepo;
        
        private static int errorCode = 0;
        static int typeAccountId;
        //
        // GET: /Class/

        public FinanceFourthLvlController()
        {
            
            accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FA_FINANCE_SUB_SUB_ACCOUNT) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            FinanceFourthLvlAccount account = new FinanceFourthLvlAccount();

            try
            {
                int accountId = 0;
                if (typeAccountId > 0)
                {
                    accountId = typeAccountId;
                    typeAccountId = 0;
                }

                account = accountRepo.GetFinanceFourthLvlAccountById(id);
                if (accountId == 0)
                    accountId = account == null ? 0 : account.ThirdLvlAccountId;
                ViewBag.ThirdLvlAccountId = new SelectList(SessionHelper.FinanceThirdLvlAccountList(Session.SessionID), "Id", "AccountName", accountId);

                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                ViewData["fourthLvlAccounts"] = SessionHelper.FinanceFourthLvlAccountList(Session.SessionID);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                setConfigurationViewBag();
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

        public ActionResult Create(FinanceFourthLvlAccount fourthLvlAccount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                fourthLvlAccount.ThirdLvlAccountId = fourthLvlAccount.FinanceThirdLvlAccount.Id;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = accountRepo.GetFinanceFourthLvlAccountByName(fourthLvlAccount.AccountName, branchId);
                if (ModelState.IsValid && classObj == null && !string.IsNullOrEmpty(fourthLvlAccount.AccountName))
                {
                    fourthLvlAccount.FinanceThirdLvlAccount = null;
                    fourthLvlAccount.BranchId = branchId;
                    fourthLvlAccount.Branch = null;

                    int returnCode = accountRepo.AddFinanceFourthLvlAccount(fourthLvlAccount);
                    if (returnCode == -1)
                        errorCode = 420;
                    else
                        errorCode = 2;
                    SessionHelper.InvalidateFinanceFourthLvlCache = false;
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
        public ActionResult Edit(FinanceFourthLvlAccount fourthLvlAccount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                fourthLvlAccount.ThirdLvlAccountId = fourthLvlAccount.FinanceThirdLvlAccount.Id;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = accountRepo.GetFinanceFourthLvlAccountByNameAndId(fourthLvlAccount.AccountName, fourthLvlAccount.Id, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    fourthLvlAccount.BranchId = branchId;
                    fourthLvlAccount.Branch = null;

                    fourthLvlAccount.FinanceThirdLvlAccount = null;
                    accountRepo.UpdateFinanceFourthLvlAccount(fourthLvlAccount);
                    errorCode = 2;
                    SessionHelper.InvalidateFinanceFourthLvlCache = false;
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

        public ActionResult SubSubAccountRequest(int typeId)
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
                FinanceFourthLvlAccount account = accountRepo.GetFinanceFourthLvlAccountById(id);
                if (account == null)
                {
                    return HttpNotFound();
                }

                int fifthLvlCount = accountRepo.GetFinanceFifthLvlCount(id);

                if (fifthLvlCount == 0)
                {
                    accountRepo.DeleteFinanceFourthLvlAccount(account);
                    errorCode = 4;
                    SessionHelper.InvalidateFinanceFourthLvlCache = false;
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


        public ActionResult CreateBankAccount()
        {
            try
            {
                Session[ConstHelper.BANK_FIRST_LVL] = ConstHelper.CAT_BANK_FIRST_LVL;
                Session[ConstHelper.BANK_SECOND_LVL] = ConstHelper.CAT_BANK_SECOND_LVL;
                Session[ConstHelper.BANK_THIRD_LVL] = ConstHelper.CAT_BANK_THIRD_LVL; 
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 3;
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        public ActionResult CreateCashAccount()
        {
            try
            {
                Session[ConstHelper.CASH_FIRST_LVL] = ConstHelper.CAT_CASH_FIRST_LVL;
                Session[ConstHelper.CASH_SECOND_LVL] = ConstHelper.CAT_CASH_SECOND_LVL;
                Session[ConstHelper.CASH_THIRD_LVL] = ConstHelper.CAT_CASH_THIRD_LVL; 
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 3;
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        private void setConfigurationViewBag()
        {
            if (Session[ConstHelper.BANK_FIRST_LVL] != null)
                ViewData["FirstLvl"] = (int)Session[ConstHelper.BANK_FIRST_LVL];
            if (Session[ConstHelper.BANK_SECOND_LVL] != null)
                ViewData["SecondLvl"] = (int)Session[ConstHelper.BANK_SECOND_LVL];
            if (Session[ConstHelper.BANK_THIRD_LVL] != null)
                ViewData["ThirdLvl"] = (int)Session[ConstHelper.BANK_THIRD_LVL];

            if (Session[ConstHelper.CASH_FIRST_LVL] != null)
                ViewData["FirstLvl"] = (int)Session[ConstHelper.CASH_FIRST_LVL];
            if (Session[ConstHelper.CASH_SECOND_LVL] != null)
                ViewData["SecondLvl"] = (int)Session[ConstHelper.CASH_SECOND_LVL];
            if (Session[ConstHelper.CASH_THIRD_LVL] != null)
                ViewData["ThirdLvl"] = (int)Session[ConstHelper.CASH_THIRD_LVL];

            Session[ConstHelper.BANK_FIRST_LVL] = null;
            Session[ConstHelper.BANK_SECOND_LVL] = null;
            Session[ConstHelper.BANK_THIRD_LVL] = null;

            Session[ConstHelper.CASH_FIRST_LVL] = null;
            Session[ConstHelper.CASH_SECOND_LVL] = null;
            Session[ConstHelper.CASH_THIRD_LVL] = null; 
        }

    }
}