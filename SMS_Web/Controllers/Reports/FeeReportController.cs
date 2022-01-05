using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data;
using SMS_DAL.ViewModel;
using Common;
using System.IO;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.Reports
{
    public class FeeReportController : Controller
    {
        private SC_WEBEntities2 db = SessionHelper.dbContext;
        SMS_DAL.Reports.DAL_Fee_Reports fee = new SMS_DAL.Reports.DAL_Fee_Reports();
        IFeePlanRepository feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
        private ISecurityRepository secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
        static int errorCode = 0;
        public ActionResult Index()
        {

            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.AD_FEE_DASHBOARD) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            return View();
        }
        //
        // GET: /FeeReport/

        public ActionResult ViewReport(int id = 0)
        {

            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_FEE_REPORTS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            ViewBag.ClassSectionId = new SelectList(SessionHelper.ClassSectionList(Session.SessionID), "ClassSectionId", "ClassSectionId");
            ViewBag.MonthId = new SelectList(SessionHelper.MonthList, "Id", "Month1");
            ViewBag.FeeHeadId = new SelectList(SessionHelper.FeeHeadList(Session.SessionID), "Id", "Name");
            ViewBag.GenderCode = new SelectList(SessionHelper.GenderList, "Id", "Gender1");
            ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);

            ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
            ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
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
                //if(model.fromDate == model.toDate)
                //    model.toDate = model.toDate.AddDays(1);
                model.rollNo = model.rollNo.Length == 0 ? "0" : model.rollNo;
                if (model == null)
                {
                    return RedirectToAction("/ViewReport");
                }
                if (model.reportId == 11)
                {
                    ds = fee.GetFeeCollectionReportData(model.fromDate, model.toDate, model.classId, model.sectionId, model.rollNo, model.paid, branchId);
                    return showReport(ds, model, exportType);
                }
                if (model.reportId == 65 || model.reportId == 66)
                {
                    ds = fee.GetFeeCollectionDetailReportData(model.fromDate, model.toDate, model.classId, model.sectionId, model.rollNo, model.feeHeadId, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 12 || model.reportId == 13)
                {
                    //verify
                    ds = fee.GetAdmissionChargesReportData(model.fromDate, model.toDate, model.classId, model.sectionId, model.rollNo, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 14)
                {
                    //verify
                    ds = fee.GetFeeCollectionReportData(model.fromDate, model.toDate, model.classId, model.sectionId, model.rollNo, model.paid, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 14)
                {
                    //verify
                    ds = fee.GetBadDebtorsReportData(branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 15)
                {
                    ds = fee.GetAnnualChargesReportData(model.classId, model.sectionId, GetForMonth(model), branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 16)
                {
                    ds = fee.GetAdvanceStudentsReportData(model.fromDate, model.toDate, model.classId, model.sectionId, model.rollNo, model.paid, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 17 || model.reportId == 18 || model.reportId == 28)
                {
                    ds = fee.GetMonthlyCollectionReportData(model.classId, model.sectionId, model.rollNo, model.paid, GetForMonth(model), model.Name, model.fatherName, branchId, model.fromDate, model.toDate);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 21)
                {
                    //check with 16
                    ds = fee.GetMonthlyCollectionSummaryData(model.classId, model.sectionId, model.rollNo, model.paid, GetForMonth(model), model.Name, model.fatherName, branchId, model.fromDate, model.toDate);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 26)
                {
                    model.paid = 0;
                    ds = fee.GetFeeDefaulterReportData(model.fromDate, model.toDate, model.classId, model.sectionId, model.rollNo, model.paid, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 19)
                {
                    //check with 16
                    ds = fee.GetClassWiseMonthlyCollectionSummaryData(model.classId, model.sectionId, model.rollNo, model.paid, GetForMonth(model), model.Name, model.fatherName, branchId, model.fromDate, model.toDate);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 20)
                {
                    ds = fee.GetIssueChallanReportData(model.fromDate, model.toDate, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 22 || model.reportId == 23)
                {
                    //check with 20
                    //not taking years?
                    ds = fee.GetYearlyCollectionReportData(model.classId, model.sectionId, GetForMonth(model), branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 32)
                {
                    //ds = fee.GetFeeHeadsReport(branchId);
                    string forMonth = GetForMonth(model);
                    ds = fee.GetFeeDetailReport(branchId, forMonth);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 24)
                {
                    ds = fee.GetBadDebtorsReportData(branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 25)
                {
                    ds = fee.GetFeeBreakUpReport(branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 27)
                {
                    ds = fee.GetFeeBalanceReportData(model.classId, model.sectionId, model.rollNo, model.Name, model.fatherName, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 29)
                {
                    model.paid = 0;
                    ds = fee.GetFeeDefaulterTerminatedReportData(model.fromDate, model.toDate, model.classId, model.sectionId, model.rollNo, model.paid, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 30 || model.reportId == 63)
                {
                    model.paid = 0;
                    ds = fee.GetUnpaidStudentsFeeHeadWise(model.classId, model.sectionId, model.rollNo, model.paid, GetForMonth(model), model.Name,
                        model.fatherName, model.feeHeadId, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 31)
                {
                    model.paid = 0;
                    if (model.year == "-1")
                        model.year = "";
                    ds = fee.GeProspectusAndRegisterationCollection(model.classId, model.Name, model.fatherName, model.year, model.inquiryNumber, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 57)
                {
                    ds = fee.GetFeePaymentHistoryReportData(model.fromDate, model.toDate, model.classId, model.sectionId, model.rollNo, model.feeHeadId, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 58)
                {
                    ds = fee.GetPaymentDetailedReportData(model.fromDate, model.toDate, model.classId, model.sectionId, model.rollNo, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 59)
                {
                    string forMonth = GetForMonth(model);
                    ds = fee.GetBillingStrength(forMonth);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 60)
                {
                    string forMonth = GetForMonth(model);
                    ds = fee.GetClassWiseFeeDetailReport(branchId, forMonth);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 69)
                {
                    string forMonth = GetForMonth(model);
                    ds = fee.GetStudentDiscountData(model.fromDate, model.toDate, model.classId, model.sectionId, forMonth, branchId);
                    return showReport(ds, model, exportType);
                }
                else
                {
                    ds = fee.GetFeeBreakUpReport(branchId);
                    return showReport(ds, model, exportType);
                }
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
            if (model.month.Trim() != "All" && model.month.Trim() != "-1")
                forMonth += model.month;
            forMonth = forMonth.Trim();
            if (model.year.Trim() != "All" && model.year.Trim() != "-1")
            {
                if (forMonth.Length > 0)
                    forMonth += "-";
                forMonth += model.year;
            }
            return forMonth;
        }

        private string GetForMonthForReport(ReportModel model)
        {
            string forMonth = "";
            if (model.month.Trim() != "All" && model.month.Trim() != "-1")
                forMonth += model.month;
            forMonth = forMonth.Trim();
            if (model.year.Trim() != "All" && model.year.Trim() != "-1")
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

        //private void AddImage(DataSet ds)
        //{
        //    if (ds.Tables[0].Rows.Count == 0)
        //    {
        //        ds.Tables[0].Rows.Add();
        //    }

        //    int branchId = (int) Session["branchId"];
        //    SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);
        //    DataColumn colByteArray = new DataColumn("IMAGE");
        //    colByteArray.DataType = System.Type.GetType("System.Byte[]");
        //    ds.Tables[0].Columns.Add(colByteArray);
        //    ds.Tables[0].Rows[0]["IMAGE"] = config.SchoolLogo;
        //}

        public FileStreamResult showReportAsExcel(DataSet ds, ReportModel model)
        {
            ReportDocument rd = createReport(ds, model);
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);

            var contentLength = stream.Length;
            Response.AppendHeader("Content-Length", contentLength.ToString());
            Response.AppendHeader("Content-Disposition", "inline; filename=" + model.reportName + ".xls");

            stream.Seek(0, SeekOrigin.Begin);
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
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);

            var contentLength = stream.Length;
            Response.AppendHeader("Content-Length", contentLength.ToString());
            Response.AppendHeader("Content-Disposition", "inline; filename=" + model.reportName + ".doc");

            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/msword");

        }

        public ReportDocument createReport(DataSet ds, ReportModel model)
        {
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            int branchId = (int)Session["branchId"];
            SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);

            ReportDocument rd = new ReportDocument();
            if (model.reportId == 11)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Fee Collection Report.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            if (model.reportId == 65)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "FeeCollectionDetailReport.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            if (model.reportId == 66)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "ClassWiseFeeCollectionDetailReport.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 12)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Admission Charges Report.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 13)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Admission Charges Detail Report.rpt"));
                //AddParametersToReport(rd, model.month);
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 14)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Class Wise Fee Collection Report.rpt"));
                //AddParametersToReport(rd, model.month);
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 15)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Annual Charges Report.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", model.month);
                //rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 16)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Advance Students Report.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 17)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Monthly Collection Report.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonthForReport(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 18)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Class Wise Monthly Collection Report.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonthForReport(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 19)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Class Wise Monthly Collection Summary.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonthForReport(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 20)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "IssueChalanReport.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 21)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Monthly Collection Summary.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonthForReport(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 22)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Yearly Collection Summary.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonthForReport(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 23)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Yearly Detail Collection Summary.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonth(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 24)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "BadDebtersReport.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 25)
            {
                //rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Fee BreakUp Report.rpt"));
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Fee BreakUp Report.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 26)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Unpaid Students Report.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 27)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Fee Balance Report.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 28)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Parents Fee Statement.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonthForReport(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 29)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Fee Dafaulter Terminated Students Report.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 30)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Unpaid Students Report Fee Head Wise.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonthForReport(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 63)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Unpaid Students Report Student Wise.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonthForReport(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 31)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Prospectus And Registeration Fee Report.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonthForReport(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 32)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "FeeDetailReport.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 57)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "PaymentHistoryDetailReport.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 58)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "PaymentDetailReport.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 59)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "BillingStrength.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                string forMonth = GetForMonthForReport(model);

                rd.SetParameterValue("ForMonth", forMonth);
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 60)
            {
                //rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Fee BreakUp Report.rpt"));
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "ClassWiseFeeDetailReport.rpt"));
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 69)
            {
                //rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "Fee BreakUp Report.rpt"));
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Fee"), "FeeDiscountReport.rpt"));
                rd.SetDataSource(ds.Tables[0]);

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Student"), "Students Contact Directory.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["FeeCollectionDataTable"].SetDataSource(ds.Tables[0]);
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
