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
    public class StoreReportController : Controller
    {
        SMS_DAL.Reports.DAL_Store_Reports storeDs = new SMS_DAL.Reports.DAL_Store_Reports();
        IStoreRepository storeRepo = new StoreRepositoryImp(new SC_WEBEntities2());
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

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.STM_REPORTS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            ViewBag.ItemNames = SessionHelper.ItemNamesList();
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
                if (model == null)
                {
                    return RedirectToAction("/ViewReport");
                }
                if (model.reportId == 59 || model.reportId == 63)
                {
                    ds = storeDs.GetItemPurchaseData(model.orderId, model.itemId, model.fromDate, model.toDate);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 60 || model.reportId == 64)
                {
                    ds = storeDs.GetItemIssuanceData(model.orderId, model.itemId, model.fromDate, model.toDate);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 61 || model.reportId == 65)
                {
                    ds = storeDs.GetItemReturnData(model.orderId, model.itemId, model.fromDate, model.toDate);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 62)
                {
                    ds = storeDs.GetCurrentStockData();
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 66)
                {
                    ds = storeDs.GetItemData();
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 67)
                {
                    ds = storeDs.GetVendorData();
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 68)
                {
                    ds = storeDs.GetVendorData(model.itemId);
                    return showReport(ds, model, exportType);
                }
                else
                {
                    ds = storeDs.GetItemPurchaseData(model.orderId, model.itemId, model.fromDate, model.toDate);
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

            if (model.reportId == 59 || model.reportId == 63)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Store"), "ItemPurchaseReport.rpt"));
                if(model.reportId == 63)
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/Store"), "ItemPurchaseReportByName.rpt"));

                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StoreDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);
                return rd;
            }
            else if (model.reportId == 60 || model.reportId == 64)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Store"), "ItemIssuanceReport.rpt"));
                if (model.reportId == 64)
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/Store"), "ItemIssuanceReportByName.rpt"));

                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StoreDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);
                return rd;
            }
            else if (model.reportId == 61 || model.reportId == 65)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Store"), "ItemReturnReport.rpt"));
                if (model.reportId == 65)
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/Store"), "ItemReturnReportByName.rpt"));

                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StoreDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);
                return rd;
            }
            else if (model.reportId == 62)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Store"), "ItemStockReport.rpt"));

                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StoreDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);
                return rd;
            }
            else if (model.reportId == 66)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Store"), "ItemsReport.rpt"));

                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StoreDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);
                return rd;
            }
            else if (model.reportId == 67 || model.reportId == 68)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Store"), "VendorsReport.rpt"));

                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StoreDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);
                return rd;
            }
            else
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Store"), "ItemPurchaseReport.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StaffDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);
                return rd;
            }

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
