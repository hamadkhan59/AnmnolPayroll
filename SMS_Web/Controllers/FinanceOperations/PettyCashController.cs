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
using SMS_DAL.ViewModel;
using SMS_Web.Filters;
using SMS_Web.Helpers;
using System.Globalization;
using Logger;
using System.Reflection;
using SMS_DAL.Reports;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;

namespace SMS_Web.Controllers.FinanceOperations
{
    public class PettyCashController : Controller
    {

        private IFinanceAccountRepository accountRepo;
        private ISecurityRepository secRepo;
        
        private static int errorCode = 0;
        //
        // GET: /Class/

        public PettyCashController()
        {
            
            accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());;
            secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());;
        }


        //
        // GET: /JournalEntry/

        public ActionResult Index()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FA_FINANCE_JOURNAL_ENTRY) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewData["fourthLvlAccounts"] = SessionHelper.FinanceFourthLvlAccountList(Session.SessionID);
                ViewData["fifthLvlAccounts"] = SessionHelper.FinanceFifthLvlAccountListWitoutReceipts(Session.SessionID);
                ViewBag.FifthLvlAccountId = new SelectList(SessionHelper.FinanceFifthLvlAccountListWitoutReceipts(Session.SessionID), "Id", "AccountName");
                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(new List<JournalEntry>());
        }

        public ActionResult Dashboard()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.AD_FINANCE_DASHBAORD) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.FirstLvlAccountId = new SelectList(SessionHelper.FinanceFirstLvlAccountListByNames(), "Id", "AccountName");
                ViewBag.SeccondLvlAccountId = new SelectList(SessionHelper.FinanceSeccondLvlAccountListByName(), "Id", "AccountName");
                ViewBag.ThirdLvlAccountId = new SelectList(SessionHelper.FinanceThirdLvlAccountListByName(Session.SessionID), "Id", "AccountName");
                ViewBag.FourthLvlAccountId = new SelectList(SessionHelper.FinanceFourthLvlAccountListByName(Session.SessionID), "Id", "AccountName");
                ViewBag.FifthLvlAccountId = new SelectList(SessionHelper.FinanceFifthLvlAccountListByName(Session.SessionID), "Id", "AccountName");
                ViewBag.FinanceLevelId = new SelectList(SessionHelper.GetFinanceLevelList(), "Id", "LevelDescription");
                ViewBag.FinanceModeId = new SelectList(accountRepo.GetAllFinanceModes(), "ID", "MODE_NAME");
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            //ViewData["seccondLvlAccounts"] = SessionHelper.FinanceSeccondLvlAccountList;
            //ViewData["thirdLvlAccounts"] = SessionHelper.FinanceThirdLvlAccountList(Session.SessionID);
            //ViewData["fourthLvlAccounts"] = SessionHelper.FinanceFourthLvlAccountList(Session.SessionID);
            //ViewData["fifthLvlAccounts"] = SessionHelper.FinanceFifthLvlAccountList(Session.SessionID);

            return View();
        }

        public JsonResult GetFinanceStats(string from = null, string to = null, string view = "month", int firstLvlId = 0, int secondLvlId = 0, int thirdLvlId = 0, int fourthLvlId = 0, int fifthLvlId = 0)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DateTime fromDate = DateTime.Now.AddDays(DateTime.Now.Day * -1 + 1);
                if (from != null && from.Length > 24)
                    fromDate = DateTime.ParseExact(from.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                DateTime toDate = DateTime.Now;
                if (to != null && to.Length > 24)
                {
                    toDate = DateTime.ParseExact(to.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return Json(accountRepo.GetFinanceStats(branchId, fromDate, toDate, view, firstLvlId, secondLvlId, thirdLvlId, fourthLvlId, fifthLvlId), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }

        public JsonResult GetRevenueExpenseStats(string from = null, string to = null, string view = "month")
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DateTime fromDate = DateTime.Now.AddDays(DateTime.Now.Day * -1 + 1);
                if (from != null && from.Length > 24)
                    fromDate = DateTime.ParseExact(from.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                DateTime toDate = DateTime.Now;
                if (to != null && to.Length > 24)
                {
                    toDate = DateTime.ParseExact(to.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return Json(accountRepo.GetRevenueExpenseStats(branchId, fromDate, toDate, view), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }


        public ActionResult BRVoucher()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FA_BANK_RECEIPT_VOUCHER) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["fourthLvlAccounts"] = SessionHelper.FinanceFourthLvlAccountList(Session.SessionID);
                ViewData["fifthLvlAccounts"] = SessionHelper.FinanceFifthLvlAccountListWitoutReceipts(Session.SessionID);
                ViewBag.FifthLvlAccountId = new SelectList(accountRepo.GetFinanceAccountsFifthLvl("Bank", branchId), "Id", "AccountName");
                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(new List<JournalEntry>());
        }

        public ActionResult CRVoucher()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.PC_PETTY_RECEIPTS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["fourthLvlAccounts"] = SessionHelper.PettyCashFourthLvlAccounts(Session.SessionID);
                ViewData["fifthLvlAccounts"] = SessionHelper.PettyCashFifthLvlAccount(Session.SessionID);
                ViewBag.FifthLvlAccountId = new SelectList(SessionHelper.AnmolPettyCashFifthLvlAccount(Session.SessionID), "Id", "AccountName");
                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(new List<JournalEntry>());
        }

        public ActionResult BPVoucher()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FA_BANK_PAYMENT_VOUCHER) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["fourthLvlAccounts"] = SessionHelper.FinanceFourthLvlAccountList(Session.SessionID);
                ViewData["fifthLvlAccounts"] = SessionHelper.FinanceFifthLvlAccountListWitoutReceipts(Session.SessionID);
                ViewBag.FifthLvlAccountId = new SelectList(accountRepo.GetFinanceAccountsFifthLvl("Bank", branchId), "Id", "AccountName");
                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(new List<JournalEntry>());
        }

        public ActionResult CPVoucher()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.PC_PETTY_PAYMENTS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["fourthLvlAccounts"] = SessionHelper.PettyCashFourthLvlAccounts(Session.SessionID);
                ViewData["fifthLvlAccounts"] = SessionHelper.PettyCashFifthLvlAccount(Session.SessionID);
                ViewBag.FifthLvlAccountId = new SelectList(SessionHelper.AnmolPettyCashFifthLvlAccount(Session.SessionID), "Id", "AccountName");
                //ViewBag.FourthLvlAccountId = new SelectList(accountRepo.GetFinanceAccounts("Cash"), "Id", "AccountName");
                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(new List<JournalEntry>());
        }

        public ActionResult Details(int id)
        {
            JournalEntryModel entry = new JournalEntryModel();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                entry = accountRepo.GetJournalEntryModel(id);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(entry);
        }

        public ActionResult CreatePdf(int id)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DAL_Finance_Reports fiananceDS = new DAL_Finance_Reports();
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                DataSet ds = fiananceDS.GetPettyCashEntryData(id, branchId);
                return showReportAsPdf(ds, branchId, "PettyCashJournalEntryReport");
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return null;
        }

        public FileStreamResult showReportAsPdf(DataSet ds, int branchId, string reportName)
        {
            ReportDocument rd = createReport(ds, branchId, reportName);

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            string fileName = reportName + ".pdf";
            var contentLength = stream.Length;
            Response.AppendHeader("Content-Length", contentLength.ToString());
            Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName);

            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf");

        }

        public ReportDocument createReport(DataSet ds, int branchId, string reportName)
        {
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            ReportDocument rd = new ReportDocument();
            SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);

            rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), reportName + ".rpt"));
            rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
            rd.Database.Tables["DataTable1"].SetDataSource(AddImage(config));

            rd.SetParameterValue("CampusName", config.CampusName);
            rd.SetParameterValue("SchoolName", config.SchoolName);

            return rd;
        }

        private DataTable AddImage(SchoolConfig config)
        {
            DataTable tbl = new DataTable();
            tbl.Rows.Add();

            DataColumn colByteArray = new DataColumn("ReportImage");
            colByteArray.DataType = System.Type.GetType("System.Byte[]");
            tbl.Columns.Add(colByteArray);
            tbl.Rows[0]["ReportImage"] = config.SchoolLogo;
            return tbl;
        }

        public ActionResult AdjustVoucher(int id)
        {
            JournalEntry entry = new JournalEntry();
            try
            {
                ViewData["fourthLvlAccounts"] = SessionHelper.FinanceFourthLvlAccountList(Session.SessionID);
                ViewData["fifthLvlAccounts"] = SessionHelper.FinanceFifthLvlAccountList(Session.SessionID);
                ViewBag.FifthLvlAccountId = new SelectList(SessionHelper.FinanceFifthLvlAccountListWitoutReceipts(Session.SessionID), "Id", "AccountName");
                ViewData["Error"] = errorCode;
                errorCode = 0;
                entry = accountRepo.GetJournalEntry(id);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(entry);
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string ChequeNo, DateTime EntryDate, int EntryAmount, string CreditDescription)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                //CreateJournalEntry(ChequeNo, EntryDate, CreditFourthLvl, AccountBalance, EntryAmount, CreditDescription, "JE", CreditFifthLvl);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                CreateJournalEntry(ChequeNo, EntryDate, EntryAmount, CreditDescription, ConstHelper.ET_JE, branchId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("BPVoucher")]
        [OnAction(ButtonName = "CreateBankPaymentVoucher")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBankPaymentVoucher(string ChequeNo, DateTime EntryDate, int EntryAmount, string CreditDescription)
        {
            try
            {

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                //FinanceFourthLvlAccount account = accountRepo.GetFinanceFourthLvlAccountById(int.Parse(CreditFourthLvl));
                //CreditFourthLvl = account.AccountName;
                //CreateJournalEntry(ChequeNo, EntryDate, CreditFourthLvl, AccountBalance, EntryAmount, CreditDescription, "BPV", CreditFifthLvl);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                CreateJournalEntry(ChequeNo, EntryDate, EntryAmount, CreditDescription, ConstHelper.ET_BPV, branchId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("BPVoucher");
        }

        [HttpPost]
        [ActionName("CPVoucher")]
        [OnAction(ButtonName = "CreateCashPaymentVoucher")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCashPaymentVoucher(string ChequeNo, DateTime EntryDate, int EntryAmount, string CreditDescription)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                //FinanceFourthLvlAccount account = accountRepo.GetFinanceFourthLvlAccountById(int.Parse(CreditFourthLvl));
                //CreditFourthLvl = account.AccountName;
                //CreateJournalEntry(ChequeNo, EntryDate, CreditFourthLvl, AccountBalance, EntryAmount, CreditDescription, "CPV", CreditFifthLvl);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                CreateJournalEntry(ChequeNo, EntryDate, EntryAmount, CreditDescription, ConstHelper.ET_CPV, branchId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("CPVoucher");
        }

        [HttpPost]
        [ActionName("BRVoucher")]
        [OnAction(ButtonName = "CreateBankReceiptVoucher")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBankReceiptVoucher(string ChequeNo, DateTime EntryDate, int EntryAmount, string CreditDescription)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                //CreateJournalEntry(ChequeNo, EntryDate, CreditFourthLvl, AccountBalance, EntryAmount, CreditDescription, "BRV", CreditFifthLvl);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                CreateJournalEntry(ChequeNo, EntryDate, EntryAmount, CreditDescription, ConstHelper.ET_BRV, branchId);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("BRVoucher");
        }

        [HttpPost]
        [ActionName("CRVoucher")]
        [OnAction(ButtonName = "CreateCashReceiptVoucher")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCashReceiptVoucher(string ChequeNo, DateTime EntryDate, int EntryAmount, string CreditDescription)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                //CreateJournalEntry(ChequeNo, EntryDate, CreditFourthLvl, AccountBalance, EntryAmount, CreditDescription, "CRV", CreditFifthLvl);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                CreateJournalEntry(ChequeNo, EntryDate, EntryAmount, CreditDescription, ConstHelper.ET_CRV, branchId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("CRVoucher");
        }

        [HttpPost]
        [ActionName("AdjustVoucher")]
        [OnAction(ButtonName = "CreateVoucherAdjustment")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVoucherAdjustment(string CreditFourthLvl, int EntryId, int? CreditFifthLvl = 0)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                JournalVoucher voucher = accountRepo.GetJournalVoucher(EntryId);
                if (voucher.IsCleared == 1)
                {
                    errorCode = 300;
                }
                else
                {
                    int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                    JournalEntry oldENtry = accountRepo.GetJournalEntry(EntryId);
                    JournalEntry newEntry = new JournalEntry();
                    FinanceFourthLvlAccount account = accountRepo.GetFinanceFourthLvlAccountByName(CreditFourthLvl, branchId);

                    newEntry.CreditFirstLvlId = account.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId;
                    newEntry.CreditSeccondLvlId = account.FinanceThirdLvlAccount.SeccondLvlAccountId;
                    newEntry.CreditThirdLvlId = account.ThirdLvlAccountId;
                    newEntry.CreditFourthLvlId = account.Id;
                    if (CreditFifthLvl > 0)
                    {
                        FinanceFifthLvlAccount fifthAccount = new FinanceFifthLvlAccount();
                        newEntry.CreditFifthLvlId = fifthAccount.Id;
                    }

                    newEntry.DebitFirstLvlId = oldENtry.CreditFirstLvlId;
                    newEntry.DebitSeccondLvlId = oldENtry.CreditSeccondLvlId;
                    newEntry.DebitThirdLvlId = oldENtry.CreditThirdLvlId;
                    newEntry.DebitFourthLvlId = oldENtry.CreditFourthLvlId;
                    newEntry.CreditAmount = oldENtry.CreditAmount;
                    newEntry.DebitAmount = oldENtry.DebitAmount;
                    newEntry.CreditDescription = "Debit to clear Journal Voucher to Account : " + CreditFourthLvl;
                    newEntry.DebitDescription = "Credit to clear journal voucher from Account : " + accountRepo.GetFinanceFourthLvlAccountByName(CreditFourthLvl, branchId).AccountName;
                    newEntry.CreatedOn = DateTime.Now;

                    newEntry.BranchId = branchId;
                    newEntry.Branch = null;

                    accountRepo.AddJurnalEntry(newEntry);

                    FinanceHelper.UpdateCreditAccountBalance((int)newEntry.CreditFourthLvlId, (int)newEntry.CreditAmount, (int)newEntry.EntryId);
                    FinanceHelper.UpdateDebitAccountBalance((int)newEntry.DebitFourthLvlId, (int)newEntry.DebitAmount);
                    voucher.IsCleared = 1;
                    accountRepo.UpdateJournalVoucher(voucher);
                    errorCode = 10;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            //CreateJournalEntry(ChequeNo, EntryDate, CreditFourthLvl, AccountBalance, EntryAmount, CreditDescription, "JE", CreditFifthLvl);
            return RedirectToAction("AdjustVoucher");
        }

        private void CreateJournalEntry(string ChequeNo, DateTime EntryDate, int EntryAmount, string CreditDescription, string EntryType, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                JournalEntry je = new JournalEntry();
                je.CreditAmount = EntryAmount;
                je.DebitAmount = EntryAmount;
                je.CreditDescription = CreditDescription;
                je.CreatedOn = EntryDate;
                je.ChequeNo = ChequeNo;
                je.EntryType = EntryType;
                je.IsPettyCash = true;
                je.BranchId = branchId;
                je.Branch = null;

                accountRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("Journal Entry is saved");
                LogWriter.WriteLog("Saving Journal Entry Detail");

                List<EntryDetail> debitEntryInfo = (List<EntryDetail>)Session[ConstHelper.ENTRY_DEBIT_DETAIL];

                string description = FinanceHelper.AddPettyCashEntryDebitDetail(je.EntryId, debitEntryInfo, EntryType, branchId);
                je.CreditDescription = description;
                accountRepo.UpdateJurnalEntry(je);
                errorCode = 10;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
        }

        private void CreateJournalEntry(string ChequeNo, DateTime EntryDate, string CreditFourthLvl, int AccountBalance, int EntryAmount, string CreditDescription, string EntryType, int branchId, int? CreditFifthLvl = 0)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                JournalEntry je = new JournalEntry();
                je.CreditAmount = EntryAmount;
                je.DebitAmount = EntryAmount;
                je.CreditDescription = CreditDescription;
                je.CreatedOn = EntryDate;
                je.ChequeNo = ChequeNo;
                je.EntryType = EntryType;

                if (CreditFifthLvl > 0)
                {
                    FinanceFifthLvlAccount account = accountRepo.GetFinanceFifthLvlAccountById((int)CreditFifthLvl);
                    je.CreditFirstLvlId = account.FinanceFourthLvlAccount.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId;
                    je.CreditSeccondLvlId = account.FinanceFourthLvlAccount.FinanceThirdLvlAccount.SeccondLvlAccountId;
                    je.CreditThirdLvlId = account.FinanceFourthLvlAccount.ThirdLvlAccountId;
                    je.CreditFourthLvlId = account.FourthLvlAccountId;
                    je.CreditFifthLvlId = account.Id;
                }
                else
                {
                    FinanceFourthLvlAccount account = accountRepo.GetFinanceFourthLvlAccountByName(CreditFourthLvl, branchId);
                    je.CreditFirstLvlId = account.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId;
                    je.CreditSeccondLvlId = account.FinanceThirdLvlAccount.SeccondLvlAccountId;
                    je.CreditThirdLvlId = account.ThirdLvlAccountId;
                    je.CreditFourthLvlId = account.Id;
                    je.CreditFifthLvlId = 0;
                }

                List<EntryDetail> debitEntryInfo = (List<EntryDetail>)Session[ConstHelper.ENTRY_DEBIT_DETAIL];
                string debitFourthLvlAccount = debitEntryInfo[0].Account;
                string debitFifthLvlAccount = debitEntryInfo[0].SubAccount;
                FinanceFourthLvlAccount debitAccount = accountRepo.GetFinanceFourthLvlAccountByName(debitFourthLvlAccount, branchId);
                je.DebitFirstLvlId = debitAccount.FinanceThirdLvlAccount.FinanceSeccondLvlAccount.FirstLvlAccountId;
                je.DebitSeccondLvlId = debitAccount.FinanceThirdLvlAccount.SeccondLvlAccountId;
                je.DebitThirdLvlId = debitAccount.ThirdLvlAccountId;
                je.DebitFourthLvlId = debitAccount.Id;
                //je.debiut = 0;

                je.BranchId = branchId;
                je.Branch = null;

                accountRepo.AddJurnalEntry(je);

                FinanceHelper.UpdateCreditAccountBalance((int)je.CreditFourthLvlId, (int)je.CreditAmount, (int) je.EntryId);
                if (string.IsNullOrEmpty(debitFifthLvlAccount) == false)
                {
                    FinanceHelper.AddEntryDebitDetail(je.EntryId, debitEntryInfo, "", branchId);
                }
                else
                {
                    FinanceHelper.UpdateDebitAccountBalance((int)je.DebitFourthLvlId, (int)je.DebitAmount);
                }
                errorCode = 10;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
        }


        public ActionResult JournalInquiry()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.PC_PETTY_INQUIRY) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.FirstLvlAccountId = new SelectList(SessionHelper.FinanceFirstLvlAccountList, "Id", "AccountName");
                ViewBag.SeccondLvlAccountId = new SelectList(SessionHelper.FinanceThirdLvlAccountList(Session.SessionID), "Id", "AccountName");
                ViewBag.ThirdLvlAccountId = new SelectList(SessionHelper.FinanceThirdLvlAccountList(Session.SessionID), "Id", "AccountName");
                ViewBag.FourthLvlAccountId = new SelectList(SessionHelper.PettyCashFourthLvlAccounts(Session.SessionID), "Id", "AccountName");
                ViewBag.FifthLvlAccountId = new SelectList(SessionHelper.PettyCashFifthLvlAccount(Session.SessionID), "Id", "AccountName");
                ViewBag.FinanceModeId = new SelectList(accountRepo.GetAllFinanceModes(), "ID", "MODE_NAME");

                ViewData["seccondLvlAccounts"] = SessionHelper.FinanceSeccondLvlAccountList;
                ViewData["thirdLvlAccounts"] = SessionHelper.FinanceThirdLvlAccountList(Session.SessionID);
                ViewData["fourthLvlAccounts"] = SessionHelper.PettyCashFourthLvlAccounts(Session.SessionID);
                ViewData["fifthLvlAccounts"] = SessionHelper.PettyCashFifthLvlAccount(Session.SessionID);

                ViewData["Error"] = errorCode;
                ViewData["entryList"] = Session["entryList"];
                Session["entryList"] = null;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(new List<JournalEntry>());
        }

        public ActionResult JournalVoucher()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FA_ADJUST_PAYABLE_VOUCHER) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.FirstLvlAccountId = new SelectList(SessionHelper.FinanceFirstLvlAccountList, "Id", "AccountName");
                ViewBag.SeccondLvlAccountId = new SelectList(SessionHelper.FinanceThirdLvlAccountList(Session.SessionID), "Id", "AccountName");
                ViewBag.ThirdLvlAccountId = new SelectList(SessionHelper.FinanceThirdLvlAccountList(Session.SessionID), "Id", "AccountName");
                ViewBag.FourthLvlAccountId = new SelectList(SessionHelper.FinanceFourthLvlAccountList(Session.SessionID), "Id", "AccountName");
                ViewBag.FifthLvlAccountId = new SelectList(SessionHelper.FinanceFifthLvlAccountList(Session.SessionID), "Id", "AccountName");

                ViewData["seccondLvlAccounts"] = SessionHelper.FinanceSeccondLvlAccountList;
                ViewData["thirdLvlAccounts"] = SessionHelper.FinanceThirdLvlAccountList(Session.SessionID);
                ViewData["fourthLvlAccounts"] = SessionHelper.FinanceFourthLvlAccountList(Session.SessionID);
                ViewData["fifthLvlAccounts"] = SessionHelper.FinanceFifthLvlAccountList(Session.SessionID);

                ViewData["Error"] = errorCode;
                ViewData["entryList"] = Session["voucherList"];
                Session["voucherList"] = null;
                errorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(new List<JournalEntry>());
        }


        [HttpPost]
        [ActionName("JournalInquiry")]
        [OnAction(ButtonName = "SearchEntry")]
        [ValidateAntiForgeryToken]

        public ActionResult SearchJournalInquiries(DateTime FromDate, DateTime ToDate, int FinanceFirstLvlAccount = 0 , int FinanceSeccondLvlAccount = 0,
            int FinanceThirdLvlAccount = 0, int FinanceFourthLvlAccount = 0, int FinanceFifthLvlAccount = 0, int FinanceMode = 0)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                ToDate = SetToDate(ToDate);
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var entryList = accountRepo.SearchPettyCashJournalEntries(FromDate, ToDate, FinanceFirstLvlAccount, FinanceSeccondLvlAccount,
                    FinanceThirdLvlAccount, FinanceFourthLvlAccount, FinanceFifthLvlAccount, FinanceMode, branchId);
                Session["entryList"] = entryList;
                LogWriter.WriteLog("Search Entry Count : " + (entryList == null ? 0 : entryList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("JournalInquiry");
        }

        private DateTime SetToDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23,59,59);
        }

        [HttpPost]
        [ActionName("JournalVoucher")]
        [OnAction(ButtonName = "SearchVoucher")]
        [ValidateAntiForgeryToken]

        public ActionResult SearchJournalVouchers(DateTime FromDate, DateTime ToDate, int FinanceFirstLvlAccount = 0, int FinanceSeccondLvlAccount = 0,
            int FinanceThirdLvlAccount = 0, int FinanceFourthLvlAccount = 0, int FinanceFifthLvlAccount = 0, int JvNo = 0)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var voucherList = accountRepo.SearchJournalVoucher(FromDate, ToDate, FinanceFirstLvlAccount, FinanceSeccondLvlAccount,
                    FinanceThirdLvlAccount, FinanceFourthLvlAccount, FinanceFifthLvlAccount, JvNo, branchId);
                Session["voucherList"] = voucherList;
                LogWriter.WriteLog("Search Entry Count : " + (voucherList == null ? 0 : voucherList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("JournalVoucher");
        }


        [HttpPost]
        public void SaveDebitEntry(List<EntryDetail> debitEntryInfo)
        {
            Session[ConstHelper.ENTRY_DEBIT_DETAIL] = debitEntryInfo;
        }

        [HttpPost]
        public JsonResult GetVoucherId()
        {
            int voucherId = accountRepo.GetMaxVouxherId();
            string voucherNo = voucherId.ToString().PadLeft(6, '0');
            return Json(voucherNo);
        }

       
        public ActionResult Delete(int id = 0)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                JournalEntry journalentry = accountRepo.GetJournalEntry(id);
                FinanceHelper.DeleteEntryDebitDetail(journalentry.EntryId);
                accountRepo.DeleteJournalEntry(journalentry);
                
                errorCode = 510;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }

            return RedirectToAction("JournalInquiry");
        }

    }
}