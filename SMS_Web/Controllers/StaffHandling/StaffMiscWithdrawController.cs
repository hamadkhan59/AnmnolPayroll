using Logger;
using SMS_DAL;
using SMS_DAL.Reports;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Filters;
using SMS_Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SMS_Web.Controllers.StaffHandling
{
    public class StaffMiscWithdrawController : Controller
    {
        //
        // GET: /StaffAdvances/
        private static int errorCode = 0;

        IStaffRepository staffRepo;
        private IFinanceAccountRepository financeRepo;
        IStudentRepository studentRepo;
        public StaffMiscWithdrawController()
        {
            
            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());;
            financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2()); ;
        }


        public ActionResult NewMiscWithdraw(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ADVANCE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            StaffMiscWithdraw miscWithDraw = new StaffMiscWithdraw();

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (id > 0)
                {
                    Staff staff = staffRepo.GetStaffById(id);
                    miscWithDraw.Staff = staff;
                    var withdrawList = staffRepo.GetStaffMiscWithdrawByStaffId(id);
                    if (withdrawList != null && withdrawList.Count > 0)
                        withdrawList = withdrawList.Take(10).OrderByDescending(x => x.Id).ToList();
                    ViewData["staffAdvances"] = withdrawList;
                }
                ViewData["Error"] = errorCode;
                ViewData["branchId"] = branchId;
                errorCode = 0;

                ViewBagData();
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(miscWithDraw);
        }

        public ActionResult InstantWithdraws()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_STAFF_INSTANT_WITHDRAW) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }


            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                
                ViewData["Error"] = errorCode;
                ViewData["branchId"] = branchId;
                errorCode = 0;
                ViewBag.StaffNames = SessionHelper.GetStaffNames(Session.SessionID);

                ViewBagData();
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View();
        }

        public ActionResult EditMiscWithdraw(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ADVANCE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            StaffMiscWithdraw miscWithdraw = new StaffMiscWithdraw();

            try
            {
                if (id > 0)
                {
                    miscWithdraw = staffRepo.GetStaffMiscWithdrawsById(id);
                    var withdrawList = staffRepo.GetStaffMiscWithdrawByStaffId((int)miscWithdraw.StaffId);
                    if (withdrawList != null && withdrawList.Count > 0)
                        withdrawList = withdrawList.Take(10).OrderByDescending(x => x.Id).ToList();
                    ViewData["staffAdvances"] = withdrawList;
                }
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["Error"] = errorCode;
                ViewData["branchId"] = branchId;

                errorCode = 0;
                ViewBagData();
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(miscWithdraw);
        }

        private void ViewBagData()
        {
            ViewBag.MonthId = new SelectList(SessionHelper.MonthList, "Id", "Month1");
            ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
            ViewBag.AccountTypeId = new SelectList(SessionHelper.AccountTypeList, "Id", "TypeName");
        }

        private void CreateJournalEntry(int StaffId, int financeAccountId, int amount, int branchId, string ChequeNO)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating the Staff Misc withdrawl journal entry for the staff Id : " + StaffId);
                var staff = staffRepo.GetStaffById(StaffId);
                var tempAccount = financeRepo.GetFinanceFifthLvlAccountById(financeAccountId);
                JournalEntry je = new JournalEntry();
                je.CreditAmount = amount;
                je.DebitAmount = je.CreditAmount;
                je.CreditDescription = "Staff Misc withdraw is paid to : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
                je.CreatedOn = DateTime.Now;
                je.ChequeNo = "";
                je.EntryType = "JE";
                je.DebitDescription = je.CreditDescription;
                je.BranchId = branchId;
                financeRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("Entry is saved succesfully");

                LogWriter.WriteLog("Adding entry credit detail");
                JournalEntryCreditDetail entryDetail = new JournalEntryCreditDetail();
                entryDetail.EntryId = je.EntryId;
                entryDetail.FifthLvlAccountId = tempAccount.Id;
                entryDetail.Amount = je.CreditAmount;
                entryDetail.Description = "Staff Misc withdraw is paid to : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
                financeRepo.AddJurnalEntryCreditDetail(entryDetail);

                LogWriter.WriteLog("Adding entry debit detail");
                var tempAccount1 = financeRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Misc Withdraw Account", branchId);
                //var tempAccount1 = financeRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                JournalEntryDebitDetail entryDetail1 = new JournalEntryDebitDetail();
                entryDetail1.EntryId = je.EntryId;
                entryDetail1.FifthLvlAccountId = tempAccount1.Id;
                entryDetail1.Amount = je.CreditAmount;
                entryDetail1.Description = "Staff Misc withdraw is paid to : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
                financeRepo.AddJurnalEntryDebitDetail(entryDetail1);

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private void CreateRverseJournalEntry(int StaffId, int financeAccountId, int amount, int branchId, string ChequeNO)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating the staff Misc withdrawl reversal journal entry for the staff Id : " + StaffId);
                var staff = staffRepo.GetStaffById(StaffId);
                var tempAccount1 = financeRepo.GetFinanceFifthLvlAccountById(financeAccountId);
                var tempAccount = financeRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Misc Withdraw Account", branchId);
                JournalEntry je = new JournalEntry();
                je.CreditAmount = amount;
                je.DebitAmount = je.CreditAmount;
                je.CreditDescription = "Staff Misc withdrawl is reversed for : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
                je.CreatedOn = DateTime.Now;
                je.ChequeNo = "";
                je.EntryType = "JE";
                je.DebitDescription = je.CreditDescription;
                je.BranchId = branchId;
                financeRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("Entry is saved succesfully");

                LogWriter.WriteLog("Adding entry credit detail");
                JournalEntryCreditDetail entryDetail = new JournalEntryCreditDetail();
                entryDetail.EntryId = je.EntryId;
                entryDetail.FifthLvlAccountId = tempAccount.Id;
                entryDetail.Amount = je.CreditAmount;
                entryDetail.Description = "Staff Misc withdrawl is reversed for : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
                financeRepo.AddJurnalEntryCreditDetail(entryDetail);
                FinanceHelper.UpdateCreditAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount, je.EntryId);
                FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);

                //var tempAccount1 = financeRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                LogWriter.WriteLog("Adding entry debit detail");
                JournalEntryDebitDetail entryDetail1 = new JournalEntryDebitDetail();
                entryDetail1.EntryId = je.EntryId;
                entryDetail1.FifthLvlAccountId = tempAccount1.Id;
                entryDetail1.Amount = je.CreditAmount;
                entryDetail1.Description = "Staff Misc withdrawl is reversed for : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
                financeRepo.AddJurnalEntryDebitDetail(entryDetail1);
                FinanceHelper.UpdateDebitAccountBalance((int)tempAccount1.FourthLvlAccountId, (int)entryDetail1.Amount);
                FinanceHelper.UpdateDebitFifthAccountBalance((int)entryDetail1.FifthLvlAccountId, (int)entryDetail1.Amount);

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        [HttpPost]
        [ActionName("InstantWithdraws")]
        [OnAction(ButtonName = "CreateInstantWithDraw")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateInstantWithDraw(int StaffId, int AdvanceAmount, string Remarks, int FinanceAccountId, int YearId, int MonthId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ADVANCE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            errorCode = SaveStaffMiscWithdraw(StaffId, AdvanceAmount, Remarks, FinanceAccountId, "", YearId, MonthId);
            return RedirectToAction("InstantWithdraws");
        }

        [HttpPost]
        [ActionName("NewMiscWithdraw")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMiscWithdraw(int StaffId, int AdvanceAmount, string Remarks, int FinanceAccountId, string ChequeNO, int YearId, int MonthId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ADVANCE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            errorCode = SaveStaffMiscWithdraw(StaffId, AdvanceAmount, Remarks, FinanceAccountId, ChequeNO, YearId, MonthId);
            return RedirectToAction("NewMiscWithdraw");
        }

        private int SaveStaffMiscWithdraw(int StaffId, int AdvanceAmount, string Remarks, int FinanceAccountId, string ChequeNO, int YearId, int MonthId)
        {
            int code = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                StaffMiscWithdraw miscWithdraw = new StaffMiscWithdraw();
                string currentMonth = SessionHelper.GetMonthName(MonthId) + "-" + (2016 + YearId - 1);
                miscWithdraw.StaffId = StaffId;
                miscWithdraw.WithdrawAmount = AdvanceAmount;
                miscWithdraw.Remarks = Remarks;
                miscWithdraw.CreatedOn = DateTime.Now;
                miscWithdraw.ForMonth = currentMonth;

                staffRepo.AddStaffMiscWithdraw(miscWithdraw);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                CreateJournalEntry(StaffId, FinanceAccountId, AdvanceAmount, branchId, ChequeNO);
                (new DAL_Staff_Reports()).ResetStaffApprovalData(StaffId);

                code = 2;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                code = 420;
            }
            return code;
        }

        [HttpPost]
        [ActionName("EditMiscWithdraw")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult SaveEditMiscWithdraw(int Id, int StaffId, int AdvanceAmount, string Remarks, int FinanceAccountId, string ChequeNO, int YearId, int MonthId)
        {
            try
            {
                if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ADVANCE) == false)
                {
                    return RedirectToAction("Index", "NoPermission");
                }
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));

                int currentAmount = 0;
                StaffMiscWithdraw miscWithdraw = staffRepo.GetStaffMiscWithdrawsById(Id);
                string currentMonth = SessionHelper.GetMonthName(MonthId) + "-" + (2016 + YearId - 1);
                currentAmount = AdvanceAmount - (int)miscWithdraw.WithdrawAmount;
                miscWithdraw.Remarks = Remarks;
                miscWithdraw.ForMonth = currentMonth;
                miscWithdraw.WithdrawAmount = AdvanceAmount;
                staffRepo.UpdateStaffMiscWithdraw(miscWithdraw);

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if(currentAmount > 0)
                    CreateJournalEntry(StaffId, FinanceAccountId, currentAmount, branchId, ChequeNO);
                else
                    CreateRverseJournalEntry(StaffId, FinanceAccountId, -1 * currentAmount, branchId, ChequeNO);

                errorCode = 2;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("EditMiscWithdraw");
        }



        public ActionResult SearchMiscWithdraw()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_SEARCH_ADVANCE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryListDD(Session.SessionID), "Id", "CatagoryName");
                ViewData["Error"] = errorCode;
                errorCode = 0;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (Session[ConstHelper.SALARY_SHEET_SEARCH_FLAG] != null && (bool)Session[ConstHelper.SALARY_SHEET_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.SALARY_SHEET_SEARCH_FLAG] = false;
                    return View(SearchStaffMiscWithdraw(branchId));
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View("");
        }


        [HttpPost]
        //[ActionName("Index")]
        //[OnAction(ButtonName = "CreateSheet")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchStaffMiscWithdraws(int? CatagoryId, int? DesignationId, int? StaffId, DateTime FromDate, DateTime ToDate)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_STAFF_SEARCH_WITHDRAW) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

           
            try
            {
                Session[ConstHelper.SALARY_SHEET_SEARCH_CATEGORY_ID] = Session[ConstHelper.SALARY_SHEET_SEARCH_DESIGNATION_ID] = Session[ConstHelper.SALARY_SHEET_SEARCH_STAFF_ID] = 0;
                if (CatagoryId != null)
                    Session[ConstHelper.SALARY_SHEET_SEARCH_CATEGORY_ID] = (int)CatagoryId;
                if (DesignationId != null)
                    Session[ConstHelper.SALARY_SHEET_SEARCH_DESIGNATION_ID] = (int)DesignationId;
                if (StaffId != null)
                    Session[ConstHelper.SALARY_SHEET_SEARCH_STAFF_ID] = (int)StaffId;
                Session[ConstHelper.SALARY_SHEET_SEARCH_FROM_DATE] = FromDate;
                Session[ConstHelper.SALARY_SHEET_SEARCH_TO_DATE] = ToDate;
                Session[ConstHelper.SALARY_SHEET_SEARCH_FLAG] = true;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("SearchMiscWithdraw");
        }

        public List<StaffMiscWithdrawModel> SearchStaffMiscWithdraw(int branchId)
        {
            List<StaffMiscWithdrawModel> list = new List<StaffMiscWithdrawModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int searchSheetCatagoryId = 0;
                if (Session[ConstHelper.STAFF_SALARY_SEARCH_CATEGORY_ID] != null)
                    searchSheetCatagoryId = (int)Session[ConstHelper.STAFF_SALARY_SEARCH_CATEGORY_ID];
                int searchSheetDesginationId = 0;
                if (Session[ConstHelper.STAFF_SALARY_SEARCH_DESIGNATION_ID] != null)
                    searchSheetCatagoryId = (int)Session[ConstHelper.STAFF_SALARY_SEARCH_CATEGORY_ID];
                int searchSheetStaffId = 0;
                if (Session[ConstHelper.SALARY_SHEET_SEARCH_STAFF_ID] != null)
                    searchSheetStaffId = (int)Session[ConstHelper.SALARY_SHEET_SEARCH_STAFF_ID];

                DateTime searchFromDate = (DateTime)Session[ConstHelper.SALARY_SHEET_SEARCH_FROM_DATE];
                DateTime searchToDate = (DateTime)Session[ConstHelper.SALARY_SHEET_SEARCH_TO_DATE];
                list = staffRepo.SearchMiscWithdraws(searchSheetCatagoryId, searchSheetDesginationId, searchSheetStaffId, searchFromDate, searchToDate, branchId);

                LogWriter.WriteLog("Search Staff Advance List Count : " + (list == null ? 0 : list.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return list;
        }
    }
}
