using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using System.Data;
using SMS_DAL.ViewModel;
using Common;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.Reports
{
    public class FinanceReportController : Controller
    {
        SMS_DAL.Reports.DAL_Finance_Reports fiananceDS = new SMS_DAL.Reports.DAL_Finance_Reports();
        private IFinanceAccountRepository accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
        private ISecurityRepository secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
        static int errorCode = 0;


        //
        // GET: /StudentReport/

        public ActionResult ViewReport(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FA_FINANCE_REPORTS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            ViewBag.FirstLvlAccountId = new SelectList(SessionHelper.FinanceFirstLvlAccountList, "Id", "AccountName");
            ViewBag.SeccondLvlAccountId = new SelectList(SessionHelper.FinanceThirdLvlAccountList(Session.SessionID), "Id", "AccountName");
            ViewBag.ThirdLvlAccountId = new SelectList(SessionHelper.FinanceThirdLvlAccountList(Session.SessionID), "Id", "AccountName");
            ViewBag.FourthLvlAccountId = new SelectList(SessionHelper.FinanceFourthLvlAccountList(Session.SessionID), "Id", "AccountName");
            ViewBag.FifthLvlAccountId = new SelectList(SessionHelper.FinanceFifthLvlAccountList(Session.SessionID), "Id", "AccountName");

            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            ViewBag.BankFourthLvlAccountId = new SelectList(accountRepo.GetFinanceAccountsFifthLvl("Bank", branchId), "Id", "AccountName");
            ViewBag.CashFourthLvlAccountId = new SelectList(accountRepo.GetFinanceAccountsFifthLvl("Cash", branchId), "Id", "AccountName");
            ViewData["seccondLvlAccounts"] = SessionHelper.FinanceSeccondLvlAccountList;
            ViewData["thirdLvlAccounts"] = SessionHelper.FinanceThirdLvlAccountList(Session.SessionID);
            ViewData["fourthLvlAccounts"] = SessionHelper.FinanceFourthLvlAccountList(Session.SessionID);
            ViewData["fifthLvlAccounts"] = SessionHelper.FinanceFifthLvlAccountListWitoutReceipts(Session.SessionID);

            ViewData["Error"] = errorCode;
            errorCode = 0;
            return View();
        }

        public ActionResult ExportExcel()
        {
            return Export(1);
        }
        public ActionResult ExportWord()
        {
            return Export(2);
        }
        public ActionResult ExportPdf()
        {
            return Export(3);
        }

        public ActionResult Export(int exportType)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                Session["branchId"] = branchId;
                DataSet ds;
                ReportModel model = new ReportModel();
                model = SessionManager.GetReportSession();
                //model.toDate = model.toDate.AddDays(1);
                if (model == null)
                {
                    return RedirectToAction("/ViewReport");
                }
                if (model.reportId == 41 || model.reportId == 45)
                {
                    ds = fiananceDS.GetFinanaceDetailSummary(model.isActiveAccounts, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 42)
                {
                    ds = fiananceDS.GetJournalEntryReportData(model.firstLevel, model.secondLevel, model.thirdLevel, model.fourthLevel, model.fifthLevel, model.mode, model.fromDate, model.toDate.AddDays(1), model.entryId, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 43)
                {
                    ds = fiananceDS.GetJVReportData(model.firstLevel, model.secondLevel, model.thirdLevel, model.fourthLevel, model.fifthLevel, model.mode, model.fromDate, model.toDate, model.entryId, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 44)
                {
                    ds = fiananceDS.GetFinanaceLiabilityBookSummary(branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 46)
                {
                    ds = fiananceDS.GetFeeCollectionReportData(model.fromDate, model.toDate, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 47)
                {
                    ds = fiananceDS.GetGeneralLedgerBookData(model.fromDate, model.toDate, branchId, model.ledgerAccount);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 48)
                {
                    ds = fiananceDS.GetBankBookData(model.bankFifthLevel, model.fromDate, model.toDate, model.BankName, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 49)
                {
                    ds = fiananceDS.GetCashBookData(model.fromDate, model.toDate, model.cashFifthLevel, model.CashName, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 50)
                {
                    ds = fiananceDS.GetAccountQuantityBookData(branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 51)
                {
                    ds = fiananceDS.GetFeeCollectionJournalReport(model.fromDate, model.toDate, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 52 || model.reportId == 56)
                {
                    ds = fiananceDS.GetIncomeStatement(model.fromDate, model.toDate, model.financeLevelId, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 53)
                {
                    ds = fiananceDS.GetProfitData(model.fromDate, model.toDate, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 54)
                {
                    ds = fiananceDS.GetCapitalWithDrawlReportData(model.fromDate, model.toDate, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 55)
                {
                    ds = fiananceDS.GetCapitalInvestmentReportData(model.fromDate, model.toDate, branchId);
                    return showReport(ds, model, exportType);
                }
                return RedirectToAction("/ViewReport");
            }
            catch (Exception ex)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return RedirectToAction("/ViewReport");
            }
        }

        private string GetForMonth(ReportModel model)
        {
            string forMonth = "";
            if (model.month != null && model.month.Trim() != "All" && model.month.Trim() != "-1")
                forMonth += model.month;
            if (model.year != null && model.year.Trim() != "All" && model.year.Trim() != "-1")
            {
                if (forMonth.Length > 0)
                    forMonth += "-";
                forMonth += model.year;
            }
            if (forMonth.Length == 0)
                forMonth = "All";
            return forMonth;
        }

        public FileStreamResult showReport(DataSet ds, ReportModel model, int exportType)
        {
            //AddImage(ds);
            if (exportType == 1)
            {
                return showReportAsExcel(ds, model);
            }
            else if (exportType == 2)
            {
                return showReportAsWord(ds, model);
            }
            else
            {
                return showReportAsPdf(ds, model);
            }
        }

        //private void AddImage(DataSet ds)
        //{
        //    if (ds.Tables[0].Rows.Count == 0)
        //    {
        //        ds.Tables[0].Rows.Add();
        //    }

        //    int branchId = (int)Session["branchId"];
        //    SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);
        //    DataColumn colByteArray = new DataColumn("IMAGE");
        //    colByteArray.DataType = System.Type.GetType("System.Byte[]");
        //    ds.Tables[0].Columns.Add(colByteArray);
        //    ds.Tables[0].Rows[0]["IMAGE"] = config.SchoolLogo;
        //}

        public FileStreamResult showReportAsExcel(DataSet ds, ReportModel model)
        {
            ReportDocument rd = createReport(ds, model);

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);

            var contentLength = stream.Length;
            Response.AppendHeader("Content-Length", contentLength.ToString());
            Response.AppendHeader("Content-Disposition", "inline; filename=" + model.reportName + ".xls");

            stream.Seek(0, SeekOrigin.Begin);
            //return File(stream, "application/pdf", model.reportName + ".pdf");
            //return File(stream, "application/pdf");
            return File(stream, "application/msword");

        }

        public FileStreamResult showReportAsPdf(DataSet ds, ReportModel model)
        {
            ReportDocument rd = createReport(ds, model);

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            string fileName = model.reportName + ".pdf";
            var contentLength = stream.Length;
            Response.AppendHeader("Content-Length", contentLength.ToString());
            Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName);

            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf");

        }

        public FileStreamResult showReportAsWord(DataSet ds, ReportModel model)
        {
            ReportDocument rd = createReport(ds, model);

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);

            var contentLength = stream.Length;
            Response.AppendHeader("Content-Length", contentLength.ToString());
            Response.AppendHeader("Content-Disposition", "inline; filename=" + model.reportName + ".doc");

            stream.Seek(0, SeekOrigin.Begin);
            //return File(stream, "application/pdf", model.reportName + ".pdf");
            //return File(stream, "application/pdf");
            return File(stream, "application/msword");

        }

        private string GetBalanceSheetReportName(int level)
        {
            string reportName = "BalanceSheetFifthLvl.rpt";
            if (level == 1)
                reportName = "BalanceSheetFirstLvl.rpt";
            else if (level == 2)
                reportName = "BalanceSheetSeccondLvl.rpt";
            else if (level == 3)
                reportName = "BalanceSheetThirdLvl.rpt";
            else if (level == 4)
                reportName = "BalanceSheetFourthLvl.rpt";

            return reportName;
        }

        private string GetTrailBalanceReportName(int level)
        {
            string reportName = "TrailBalanceReport5thLvl.rpt";
            if (level == 1)
                reportName = "TrailBalanceReport1stLvl.rpt";
            else if (level == 2)
                reportName = "TrailBalanceReport2ndLvl.rpt";
            else if (level == 3)
                reportName = "TrailBalanceReport3rdLvl.rpt";
            else if (level == 4)
                reportName = "TrailBalanceReport4thLvl.rpt";

            return reportName;
        }

        private string GetIncomeStatementReportName(int level)
        {
            string reportName = "IncomeStatementReport5th.rpt";
            if (level == 1)
                reportName = "IncomeStatementReport1st.rpt";
            else if (level == 2)
                reportName = "IncomeStatementReport2nd.rpt";
            else if (level == 3)
                reportName = "IncomeStatementReport3rd.rpt";
            else if (level == 4)
                reportName = "IncomeStatementReport4th.rpt";

            return reportName;
        }

        private string GetChartOfAccountsReportName(int level)
        {
            string reportName = "ChartOfAccountsFifthLvl.rpt";
            if (level == 1)
                reportName = "ChartOfAccountsFirstLvl.rpt";
            else if (level == 2)
                reportName = "ChartOfAccountsSeccondLvl.rpt";
            else if (level == 3)
                reportName = "ChartOfAccountsThirdLvl.rpt";
            else if (level == 4)
                reportName = "ChartOfAccountsFifthLvl.rpt";

            return reportName;
        }

        private DataTable AddImage()
        {
            DataTable tbl = new DataTable();
            tbl.Rows.Add();

            int branchId = (int)Session["branchId"];
            SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);
            DataColumn colByteArray = new DataColumn("ReportImage");
            colByteArray.DataType = System.Type.GetType("System.Byte[]");
            tbl.Columns.Add(colByteArray);
            tbl.Rows[0]["ReportImage"] = config.SchoolLogo;

            return tbl;
        }

        public ReportDocument createReport(DataSet ds, ReportModel model)
        {
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            ReportDocument rd = new ReportDocument();
            int branchId = (int)Session["branchId"];
            SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);

            if (model.reportId == 41)
            {
                string reportName = GetBalanceSheetReportName(model.financeLevelId);
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), reportName));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 42)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), "Journal Entries Report.rpt"));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 43)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), "Payables JV Report.rpt"));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 44)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), "PayablesReport.rpt"));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 45)
            {
                string reportName = GetChartOfAccountsReportName(model.financeLevelId);
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), reportName));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 46)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), "FeeCollectionReport.rpt"));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 47)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), "GeneralLedgerReport.rpt"));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 48)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), "Bank Book Report.rpt"));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 49)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), "Cash Book Report.rpt"));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 50)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), "Account Count Book.rpt"));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 51)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), "FeeCollectionLedger.rpt"));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 52)
            {
                string reportName = GetTrailBalanceReportName(model.financeLevelId);
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), reportName));
                ds.Tables[0].TableName = "DataTable1";
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 53)
            {
                string reportname = "CapitalProfitReport.rpt";
                if (model.includeCashWithdrawal)
                    reportname = "CapitalProfitReportIncludedWithDrawl.rpt";

                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), reportname));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 54)
            {
                string reportname = "CapitalWithdrawlReport.rpt";
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), reportname));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 55)
            {
                string reportname = "CapitalInvestmentReport.rpt";
                if (model.isCashWithdrawalDetailReport)
                    reportname = "CapitalInvestmentDetailedReport.rpt";

                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), reportname));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else
            {
                string reportName = GetIncomeStatementReportName(model.financeLevelId);
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Finance"), reportName));
                rd.Database.Tables["DataTableF"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }

        }


        public void AddParametersToReport(ReportDocument rpt, string forMonth)
        {
            ParameterFieldDefinitions crParameterFieldDefinitions;
            ParameterFieldDefinition crParameterFieldDefinition;
            crParameterFieldDefinitions = rpt.DataDefinition.ParameterFields;

            ParameterValues crParameterValues = new ParameterValues();
            ParameterDiscreteValue crParameterDiscreteValue = new ParameterDiscreteValue();

            crParameterDiscreteValue.Value = forMonth;
            crParameterFieldDefinition = crParameterFieldDefinitions["ForMonth"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;

            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);
        }


        public void AddParametersToReport(ReportDocument rpt, DateTime fromDate, DateTime toDate)
        {
            ParameterFieldDefinitions crParameterFieldDefinitions;
            ParameterFieldDefinition crParameterFieldDefinition;
            crParameterFieldDefinitions = rpt.DataDefinition.ParameterFields;

            ParameterValues crParameterValues = new ParameterValues();
            ParameterDiscreteValue crParameterDiscreteValue = new ParameterDiscreteValue();

            crParameterDiscreteValue.Value = fromDate;
            crParameterFieldDefinition = crParameterFieldDefinitions["FromDate"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;

            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

            ParameterFieldDefinitions crParameterFieldDefinitions1;
            ParameterFieldDefinition crParameterFieldDefinition1;
            crParameterFieldDefinitions1 = rpt.DataDefinition.ParameterFields;

            ParameterValues crParameterValues1 = new ParameterValues();
            ParameterDiscreteValue crParameterDiscreteValue1 = new ParameterDiscreteValue();

            crParameterDiscreteValue1.Value = toDate;
            crParameterFieldDefinition1 = crParameterFieldDefinitions1["ToDate"];
            crParameterValues1 = crParameterFieldDefinition1.CurrentValues;


            crParameterValues1.Clear();
            crParameterValues1.Add(crParameterDiscreteValue1);
            crParameterFieldDefinition1.ApplyCurrentValues(crParameterValues1);
        }
    }
}
