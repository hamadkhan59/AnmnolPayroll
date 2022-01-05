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
    public class StaffAdvancesController : Controller
    {
        //
        // GET: /StaffAdvances/
        private static int errorCode = 0;

        IStaffRepository staffRepo;
        private IFinanceAccountRepository financeRepo;
        public StaffAdvancesController()
        {
            
            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());;
            financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2()); ;
        }


        public ActionResult Index()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ADVANCE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryListDD(Session.SessionID), "Id", "CatagoryName");

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (Session[ConstHelper.STAFF_SEARCH_FLAG] != null && (bool)Session[ConstHelper.STAFF_SEARCH_FLAG] == true)
                {
                    Session[ConstHelper.STAFF_SEARCH_FLAG] = false;
                    ViewData["staff"] = SearchStaff(branchId);
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View("");
        }


        public ActionResult NewAdvance(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ADVANCE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            StaffAdvance advance = new StaffAdvance();

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (id > 0)
                {
                    Staff staff = staffRepo.GetStaffById(id);
                    advance.Staff = staff;
                    var advanceList = staffRepo.GetSTaffAdvancesByStaffId(id);
                    if (advanceList != null && advanceList.Count > 0)
                        advanceList = advanceList.Take(10).OrderByDescending(x => x.Id).ToList();
                    ViewData["staffAdvances"] = advanceList;
                }
                ViewData["Error"] = errorCode;
                ViewData["branchId"] = branchId;
                errorCode = 0;

                ViewBag.AccountTypeId = new SelectList(SessionHelper.AccountTypeList, "Id", "TypeName");
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(advance);
        }

        public ActionResult EditAdvance(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ADVANCE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            StaffAdvance advance = new StaffAdvance();

            try
            {
                if (id > 0)
                {
                    advance = staffRepo.GetStaffAdvancesById(id);
                    var advanceList = staffRepo.GetSTaffAdvancesByStaffId(id);
                    if (advanceList != null && advanceList.Count > 0)
                        advanceList = advanceList.Take(10).OrderByDescending(x => x.Id).ToList();
                    ViewData["staffAdvances"] = advanceList;
                }
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["branchId"] = branchId;
                ViewData["Error"] = errorCode;
                errorCode = 0;

                ViewBag.AccountTypeId = new SelectList(SessionHelper.AccountTypeList, "Id", "TypeName");
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(advance);
        }

        private void CreateJournalEntry(int StaffId, int financeAccountId, int amount, int branchId, string ChequeNO)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating the staff advance journal entry for the staff Id : " + StaffId);
                var staff = staffRepo.GetStaffById(StaffId);
                var tempAccount = financeRepo.GetFinanceFifthLvlAccountById(financeAccountId);
                JournalEntry je = new JournalEntry();
                je.CreditAmount = amount;
                je.DebitAmount = je.CreditAmount;
                je.CreditDescription = "Staff advance is paid to : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
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
                entryDetail.Description = "Staff advance is paid to : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
                financeRepo.AddJurnalEntryCreditDetail(entryDetail);
                FinanceHelper.UpdateCreditAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount, je.EntryId);
                FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);

                LogWriter.WriteLog("Adding entry debit detail");
                var tempAccount1 = financeRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Advance Account", branchId);
                //var tempAccount1 = financeRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                JournalEntryDebitDetail entryDetail1 = new JournalEntryDebitDetail();
                entryDetail1.EntryId = je.EntryId;
                entryDetail1.FifthLvlAccountId = tempAccount1.Id;
                entryDetail1.Amount = je.CreditAmount;
                entryDetail1.Description = "Staff advance is paid to : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
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

        private void CreateRverseJournalEntry(int StaffId, int financeAccountId, int amount, int branchId, string ChequeNO)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating the staff advance reversal journal entry for the staff Id : " + StaffId);
                var staff = staffRepo.GetStaffById(StaffId);
                var tempAccount1 = financeRepo.GetFinanceFifthLvlAccountById(financeAccountId);
                var tempAccount = financeRepo.GetFinanceFifthLvlAccountByName(staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Advance Account", branchId);
                JournalEntry je = new JournalEntry();
                je.CreditAmount = amount;
                je.DebitAmount = je.CreditAmount;
                je.CreditDescription = "Staff advance is reversed for : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
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
                entryDetail.Description = "Staff advance is reversed for : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
                financeRepo.AddJurnalEntryCreditDetail(entryDetail);
                FinanceHelper.UpdateCreditAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount, je.EntryId);
                FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);

                //var tempAccount1 = financeRepo.GetFinanceFifthLvlAccountById(int.Parse(finanaceAccount));
                LogWriter.WriteLog("Adding entry debit detail");
                JournalEntryDebitDetail entryDetail1 = new JournalEntryDebitDetail();
                entryDetail1.EntryId = je.EntryId;
                entryDetail1.FifthLvlAccountId = tempAccount1.Id;
                entryDetail1.Amount = je.CreditAmount;
                entryDetail1.Description = "Staff advance is reversed for : " + staff.Name + ", From Finanace Account : " + tempAccount.AccountName;
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
        [ActionName("NewAdvance")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAdvance(int StaffId, int AdvanceAmount, string Remarks, int FinanceAccountId, string ChequeNO, int InstallmentAmount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_ADVANCE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                StaffAdvance advance = new StaffAdvance();
                advance.StaffId = StaffId;
                advance.AdvanceAmount = AdvanceAmount;
                advance.Remarks = Remarks;
                advance.CreatedOn = DateTime.Now;

                staffRepo.AddStaffAdvance(advance);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                CreateJournalEntry(StaffId, FinanceAccountId, AdvanceAmount, branchId, ChequeNO);

                Staff staf = staffRepo.GetStaffById(StaffId);
                staf.Advance = (staf.Advance == null ? 0 : staf.Advance) + AdvanceAmount;
                staf.AdvanceInstallment = InstallmentAmount;
                staffRepo.UpdateStaff(staf);
                (new DAL_Staff_Reports()).ResetStaffApprovalData(StaffId);

                errorCode = 2;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("NewAdvance");
        }

        [HttpPost]
        [ActionName("EditAdvance")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult SaveEditAdvance(int Id, int StaffId, int AdvanceAmount, string Remarks, int FinanceAccountId, string ChequeNO, int InstallmentAmount)
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
                StaffAdvance advance = staffRepo.GetStaffAdvancesById(Id);
                currentAmount = AdvanceAmount - (int)advance.AdvanceAmount;
                advance.Remarks = Remarks;
                advance.AdvanceAmount = AdvanceAmount;
                staffRepo.UpdateStaffAdvance(advance);

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if(currentAmount > 0)
                    CreateJournalEntry(StaffId, FinanceAccountId, currentAmount, branchId, ChequeNO);
                else
                    CreateRverseJournalEntry(StaffId, FinanceAccountId, -1 * currentAmount, branchId, ChequeNO);

                Staff staf = staffRepo.GetStaffById(StaffId);
                staf.Advance = (staf.Advance == null ? 0 : staf.Advance) + currentAmount;
                staf.AdvanceInstallment = InstallmentAmount;
                staffRepo.UpdateStaff(staf);
                errorCode = 2;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("EditAdvance");
        }


        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Search")]
        [ValidateAntiForgeryToken]
        public ActionResult SearchStaff(int? CatagoryId = 0, int? DesignationId = 0, string StaffId = "", string Name = "", string FatherName = "")
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                Session[ConstHelper.STAFF_SEARCH_CATEGORY_ID] = 0;
                Session[ConstHelper.STAFF_SEARCH_DESIGNATION_ID] = 0;
                Session[ConstHelper.STAFF_SEARCH_STAFF_ID] = 0;
                Session[ConstHelper.STAFF_SEARCH_STAFF_NAME] = "";
                Session[ConstHelper.STAFF_SEARCH_STAFF_FATHER_NAME] = "";
                Session[ConstHelper.STAFF_SEARCH_FLAG] = true;

                if (CatagoryId != null)
                    Session[ConstHelper.STAFF_SEARCH_CATEGORY_ID] = (int)CatagoryId;
                //if (staff.DesignationId != null)
                Session[ConstHelper.STAFF_SEARCH_DESIGNATION_ID] = DesignationId;
                if (StaffId.Length > 0)
                    Session[ConstHelper.STAFF_SEARCH_STAFF_ID] = int.Parse(StaffId);
                //if (staff.Name != null)
                Session[ConstHelper.STAFF_SEARCH_STAFF_NAME] = Name;
                //if (staff.FatherName != null)
                Session[ConstHelper.STAFF_SEARCH_STAFF_FATHER_NAME] = FatherName;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        private List<StaffModel> SearchStaff(int branchId)
        {
            List<StaffModel> staffList = new List<StaffModel>();
            
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int searchCatagoryId = (int)Session[ConstHelper.STAFF_SEARCH_CATEGORY_ID];
                int searchDesignationId = (int)Session[ConstHelper.STAFF_SEARCH_DESIGNATION_ID];
                int searchStaffId = (int)Session[ConstHelper.STAFF_SEARCH_STAFF_ID];
                string searchStaffName = (string)Session[ConstHelper.STAFF_SEARCH_STAFF_NAME];
                string searchStaffFatherName = (string)Session[ConstHelper.STAFF_SEARCH_STAFF_FATHER_NAME];
                staffList = staffRepo.SearchStaffModel(searchCatagoryId, searchDesignationId, searchStaffId, searchStaffName, searchStaffFatherName, branchId);
                LogWriter.WriteLog("Search Staff List Count : " + (staffList == null ? 0 : staffList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 1420;
            }
            return staffList;
        }

        public ActionResult SearchAdvances()
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
                    return View(SearchStaffAdvances(branchId));
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
        public ActionResult SearchStaffAdvances(int? CatagoryId, int? DesignationId, int? StaffId, DateTime FromDate, DateTime ToDate)
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
            return RedirectToAction("SearchAdvances");
        }

        public List<StaffAdvanceModel> SearchStaffAdvances(int branchId)
        {
            List<StaffAdvanceModel> list = new List<StaffAdvanceModel>();
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
                if (Session[ConstHelper.STAFF_SALARY_SEARCH_STAFF_ID] != null)
                    searchSheetCatagoryId = (int)Session[ConstHelper.STAFF_SALARY_SEARCH_CATEGORY_ID];

                DateTime searchFromDate = (DateTime)Session[ConstHelper.SALARY_SHEET_SEARCH_FROM_DATE];
                DateTime searchToDate = (DateTime)Session[ConstHelper.SALARY_SHEET_SEARCH_TO_DATE];
                list = staffRepo.SearchAdvance(searchSheetCatagoryId, searchSheetDesginationId, searchSheetStaffId, searchFromDate, searchToDate, branchId);

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
