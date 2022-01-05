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
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.FeeCollection
{
    public class FeeHeadController : Controller
    {
        private static int errorCode = 0;
        
        private IFinanceAccountRepository accountRepo;
        private IClassSectionRepository classSectionRepo;
        IFeePlanRepository feePlanRepo;

        //
        // GET: /FeeHead/

        public FeeHeadController()
        {
            
            accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());; 
            classSectionRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_FEE_HEAD) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            FeeHead head = new FeeHead();

            try
            {
                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                ViewData["feeHeads"] = SessionHelper.FeeHeadList(Session.SessionID);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                head = feePlanRepo.GetFeeHeadById(id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(head);
        }

       

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")]FeeHead feehead)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = feePlanRepo.GetFeeHeadByName(feehead.Name, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    LogWriter.WriteLog("Model state is valid, creating fee head");
                    feehead.BranchId = branchId;
                    feehead.Branch = null;

                    feePlanRepo.AddFeeHead(feehead);
                    errorCode = 2;
                    CreateFinnaceAccount(feehead.Name, branchId);
                    SessionHelper.InvalidateFeeHeadCache = false;
                }
                else if (classObj != null)
                    errorCode = 11;
                else
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["feeHeads"] = SessionHelper.FeeHeadList(Session.SessionID);
                    return View(feehead);
                }
                
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }

            return RedirectToAction("Index", new { id = 0});
        }

        private void CreateFinnaceAccount(string headName, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating finance account : " + headName);
                FinanceFifthLvlAccount advanceAccount = new FinanceFifthLvlAccount();
                advanceAccount.AccountName = headName;
                advanceAccount.AccountDescription = "Fee Heads Receivable Account";
                advanceAccount.CreatedOn = DateTime.Now;
                advanceAccount.Value = 0;
                advanceAccount.Count = 0;
                advanceAccount.BranchId = branchId;
                advanceAccount.FourthLvlAccountId = SessionHelper.GetFourthLvlConfigurationAccount(branchId, ConstHelper.CAT_FEE_RECEIVABLE, ConstHelper.CAT_RECEIVABLES);
                accountRepo.AddFinanceFifthLvlAccount(advanceAccount);
                SessionHelper.InvalidateFinanceFifthLvlCache = false;

                var list = classSectionRepo.GetAllClassSectionsModel();

                LogWriter.WriteLog("Adding fee head account in all classes");

                foreach (ClassSectionModel sec in list)
                {
                    string fourthLvlAccountName = sec.ClassName + ", Section : " + sec.SectionName;
                    var fourthLvlAccountTemp = accountRepo.GetFinanceFourthLvlAccountByName(fourthLvlAccountName, branchId);

                    if (fourthLvlAccountTemp != null)
                    {
                        LogWriter.WriteLog("Creating finance account under class section : " + fourthLvlAccountName);
                        FinanceFifthLvlAccount accountsFifthLvl = new FinanceFifthLvlAccount();
                        var fifthLvlObj = accountRepo.GetFinanceFifthLvlAccount(headName, fourthLvlAccountTemp.Id);

                        if (fifthLvlObj == null)
                        {
                            accountsFifthLvl.AccountName = headName;
                            accountsFifthLvl.AccountDescription = "This account is created for fee head " + headName + " fee collection for " + fourthLvlAccountName;
                            accountsFifthLvl.CreatedOn = DateTime.Now;
                            accountsFifthLvl.Value = 0;
                            accountsFifthLvl.Count = 0;
                            accountsFifthLvl.BranchId = branchId;
                            accountsFifthLvl.FourthLvlAccountId = fourthLvlAccountTemp.Id;
                            accountRepo.AddFinanceFifthLvlAccount(accountsFifthLvl);
                            SessionHelper.InvalidateFinanceFifthLvlCache = false;
                        }
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

        }

        
        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FeeHead feehead)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = feePlanRepo.GetFeeHeadByNameAndId(feehead.Name, feehead.Id, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    LogWriter.WriteLog("Model state is valid, updating fee head");
                    feehead.BranchId = branchId;
                    feehead.Branch = null;

                    feePlanRepo.UpdateFeeHead(feehead);
                    errorCode = 2;
                    SessionHelper.InvalidateFeeHeadCache = false;
                }
                else if (classObj != null)
                    errorCode = 11;
                else
                {
                    LogWriter.WriteLog("Model state is invalid");
                    ViewData["Operation"] = feehead.Id;
                    ViewData["feeHeads"] = SessionHelper.FeeHeadList(Session.SessionID);
                    return View(feehead);
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
        // GET: /FeeHead/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                FeeHead feehead = feePlanRepo.GetFeeHeadById(id);
                if (feehead == null)
                {
                    return HttpNotFound();
                }

                int headCount = feePlanRepo.GetHeadChallanDetailCount(id);

                if (headCount == 0)
                {
                    feePlanRepo.DeleteFeeHead(feehead);
                    errorCode = 4;
                    SessionHelper.InvalidateFeeHeadCache = false;
                }
                else
                    errorCode = 40;
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