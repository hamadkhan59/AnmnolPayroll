using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using System.Data;
using SMS_DAL.ViewModel;
using Common;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using CrystalDecisions.Shared;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.Reports
{
    public class StaffReportController : Controller
    {
        SMS_DAL.Reports.DAL_Staff_Reports staffDs = new SMS_DAL.Reports.DAL_Staff_Reports();
        IStaffRepository staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
        private ISecurityRepository secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
        SC_WEBEntities2 db = new SC_WEBEntities2();
        static int errorCode = 0;

        //
        // GET: /StudentReport/

        public ActionResult ViewReport(int id = 0)
        {

            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SH_STAFF_REPORTS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            ViewBag.Designations = new SelectList(SessionHelper.DesignationList(Session.SessionID), "Id", "Name");
            ViewBag.Catagories = new SelectList(SessionHelper.DesignationCatagoryList(Session.SessionID), "Id", "CatagoryName");

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
                model.toDate = model.toDate.AddDays(1);
                model.designationId = model.designationId == 0 ? -1 : model.designationId;
                model.categoryId = model.categoryId == 0 ? -1 : model.categoryId;
                if (model == null)
                {
                    return RedirectToAction("/ViewReport");
                }
                if (model.reportId == 33)
                {
                    ds = staffDs.GetNewStaffData(model.categoryId, model.designationId, model.fromDate, model.toDate, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 34)
                {
                    ds = staffDs.GetLeavingStaffData(model.categoryId, model.designationId, model.fromDate, model.toDate, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 35)
                {
                    ds = staffDs.GetStaffMonthlySalaryData(GetDBForMonth(model), model.categoryId, model.designationId, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 36)
                {
                    ds = staffDs.GetStaffMonthlySalaryData(GetDBForMonth(model), model.categoryId, model.designationId, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 37)
                {
                    ds = staffDs.GeStaffContactDirectoryData(branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 38)
                {
                    ds = staffDs.GetStaffMonthlySalaryData(GetDBForMonth(model), model.categoryId, model.designationId, branchId, model.staffId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 40)
                {
                    model.toDate = model.toDate.AddDays(-1);
                    //ds = staffDs.GetStaffattandanceData(model.categoryId, model.designationId, model.staffId, model.fromDate, model.toDate, branchId);
                    ds = staffDs.GetStaffattandanceReport(model.fromDate, model.toDate);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 48)
                {
                    model.toDate = model.toDate.AddDays(-1);
                    //ds = staffDs.GetStaffattandanceData(model.categoryId, model.designationId, model.staffId, model.fromDate, model.toDate, branchId);
                    ds = staffDs.GetStaffattandanceReportForStaff(model.fromDate, model.toDate);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 49)
                {
                    model.toDate = model.toDate.AddDays(-1);
                    //ds = staffDs.GetStaffattandanceData(model.categoryId, model.designationId, model.staffId, model.fromDate, model.toDate, branchId);
                    ds = staffDs.GetStaffattandanceReportMonthly(model.fromDate, model.toDate, model.staffId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 68)
                {
                    ds = staffDs.GetStaffSmsHistoryData(model.categoryId, model.designationId, model.staffId, model.fromDate, model.toDate, branchId);
                    return showReport(ds, model, exportType);
                }
                else
                {
                    ds = staffDs.GetStaffYearlySalaryData(GetDBForMonth(model), model.categoryId, model.designationId, branchId);
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

        private string GetDBForMonth(ReportModel model)
        {
            string forMonth = "";
            if (model.month != null && model.month.Trim() != "All" && model.month.Trim() != "-1")
            {
                string months = db.Months.Where(x => x.Month1 == model.month.Trim()).FirstOrDefault().Month1.ToString();
                forMonth += months;
            }
            if (model.year != null && model.year.Trim() != "All" && model.year.Trim() != "-1")
            {
                if (forMonth.Length > 0)
                    forMonth = forMonth + "-";
                forMonth = forMonth + model.year.Trim();
            }
            return forMonth;
        }

        private string GetDBForMonth1(ReportModel model)
        {
            string forMonth = "";
            if (model.month != null && model.month.Trim() != "All" && model.month.Trim() != "-1")
            {
                string months = db.Months.Where(x => x.Month1 == model.month.Trim()).FirstOrDefault().Id.ToString();
                forMonth += months;
            }
            if (model.year != null && model.year.Trim() != "All" && model.year.Trim() != "-1")
            {
                if (forMonth.Length > 0)
                    forMonth = "-" + forMonth;
                forMonth = model.year.Trim() + forMonth;
            }
            return forMonth;
        }

        private string GetForMonth(ReportModel model)
        {
            string forMonth = "";
            if (model.month != null && model.month.Trim() != "All" && model.month.Trim() != "-1")
                forMonth += model.month.Trim();
            if (model.year != null && model.year.Trim() != "All" && model.year.Trim() != "-1")
            {
                if (forMonth.Length > 0)
                    forMonth += "-";
                forMonth += model.year.Trim();
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

        public ReportDocument createReport(DataSet ds, ReportModel model)
        {
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            ReportDocument rd = new ReportDocument();
            int branchId = (int)Session["branchId"];
            SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);

            if (model.reportId == 33)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "Staff Joining Report.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StaffDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 34)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "Staff Leaving Report.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StaffDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 35)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "Staff Monthly Salary Report.rpt"));
                rd.Database.Tables["StaffDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonth(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 36)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "Month Wise Yearly Salary Report.rpt"));
                rd.Database.Tables["StaffDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonth(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 37)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "Staff Contact Directory.rpt"));
                rd.Database.Tables["StaffDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 38)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "Staff Wise Yearly Salary Report.rpt"));
                rd.Database.Tables["StaffDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonth(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 39)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "Staff Detailed Yearly Salary Report.rpt"));
                rd.Database.Tables["StaffDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonth(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 40)
            {
                //rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "StaffAtendanceReport.rpt"));
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "StaffAttendanceDetailReport.rpt"));
                rd.Database.Tables["StaffDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 48)
            {
                //rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "StaffAtendanceReport.rpt"));
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "StaffAttendanceDetailReportForStaff.rpt"));
                rd.Database.Tables["StaffDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 49)
            {
                //rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "StaffAtendanceReport.rpt"));
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "StaffAttendanceDetailReportMonthly.rpt"));
                rd.Database.Tables["StaffDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 68)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "Staff SMS History Report.rpt"));
                rd.Database.Tables["StaffDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Staffs"), "Students Contact Directory.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.SetDataSource(ds.Tables[0]);

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
