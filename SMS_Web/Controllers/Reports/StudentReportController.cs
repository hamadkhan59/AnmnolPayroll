using Common;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Logger;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Helpers;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace SMS_Web.Controllers.Reports
{
    public class StudentReportController : Controller
    {
        private ISecurityRepository secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
        SMS_DAL.Reports.DAL_Student_Reports std = new SMS_DAL.Reports.DAL_Student_Reports();
        //
        // GET: /StudentReport/
        static int errorCode = 0;

        public ActionResult ViewReport(int id=0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SA_REPORTS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            ViewBag.AdmissionType = new SelectList(SessionHelper.AdmissionTypeList, "Id", "AdmissionType1");
            ViewBag.MonthId = new SelectList(SessionHelper.MonthList, "Id", "Month1");
            ViewBag.ClassSectionId = new SelectList(SessionHelper.ClassSectionList(Session.SessionID), "ClassSectionId", "ClassSectionId");
            ViewBag.GenderCode = new SelectList(SessionHelper.GenderList, "Id", "Gender1");
            ViewBag.AttendanceStatusCode = new SelectList(SessionHelper.AttendanceStatusList, "Id", "CodeName");
            ViewBag.DirecotryOption = new SelectList(SessionHelper.DirectoryViewOptionList(), "Id", "ViewOption");
            ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
            ViewData["student"] = null;

            ViewBag.ReligionCode = new SelectList(SessionHelper.RelegionList, "Id", "Name");
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
                //model.toDate = model.toDate.AddDays(1);
                if (model == null)
                {
                    return RedirectToAction("/ViewReport");
                }
                if (model.reportId == 1)
                {

                    ds = std.GetStudentAdmissionData(model.fromDate, model.toDate, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 2)
                {
                    ds = std.GetStudentLeavingData(model.fromDate, model.toDate, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 3)
                {
                    ds = std.GetYearlyStudentAdmissionData(model.year, model.classId, model.sectionId, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 4)
                {
                    ds = std.GetYearlyStudentAdmissionData(model.year, model.classId, model.sectionId, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 5)
                {
                    ds = std.GetStudentAdmissionData(model.fromDate, model.toDate, model.classId, model.sectionId, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 6)
                {
                    ds = std.GetStudentLeavingData(model.fromDate, model.toDate, model.classId, model.sectionId, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 59 || model.reportId == 60)
                {
                    bool flag = false;
                    if (model.reportId == 60)
                        flag = true;
                    ds = std.GetDailyAttendanceSheet(model.fromDate, flag);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 61)
                {
                    ds = std.GetStudentStrengthData(branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 7 || model.reportId == 10 || model.reportId == 9 || model.reportId == 76)
                {
                    ds = std.GetStudentAttandanceData(model.classId, model.sectionId, model.rollNo, model.Name, model.toDate, model.fromDate, branchId, model.attendanceStatusId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 62)
                {
                    string forMonth = GetForMonth(model);
                    ds = std.GetStudentAttendanceSheet(model.classId, model.sectionId, model.genderId, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 64)
                {
                    ds = std.GetStudentTerminatedReport(model.classId, model.sectionId, branchId);
                    return showReport(ds, model, exportType);
                }
                else if (model.reportId == 67)
                {
                    ds = std.GetStudentSmsHistoryReport(model.classId, model.sectionId, model.rollNo, model.Name, model.toDate, model.fromDate, branchId);
                    return showReport(ds, model, exportType);
                }
                else
                {
                    if (model.directoryOptionId == 1)
                        ds = std.GetStudentContactDirectory(model.classId, model.sectionId, branchId);
                    else
                        ds = std.GetStudentContactDirectoryAdmissionNoWise(model.classId, model.sectionId, branchId);
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

        public FileStreamResult showReportAsExcel(DataSet ds,ReportModel model)
        {
            ReportDocument rd = createReport(ds,model);

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

        public ReportDocument createReport(DataSet ds , ReportModel model)
        {
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            int branchId = (int)Session["branchId"];
            SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);
            ReportDocument rd = new ReportDocument();
            
            if (model.reportId == 1)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "Students Admission Report.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StudentDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 2)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "Students Leaving Report.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StudentDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 3)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "Yearly Class Wise Students Admission Report.rpt"));
                AddParametersToReport(rd, model.month);
                rd.Database.Tables["StudentDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonth(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 4)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "Yearly Students Admission Report.rpt"));
                AddParametersToReport(rd, model.month);
                rd.Database.Tables["StudentDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonth(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 5)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "Class Wise Students Admission Report.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                //rd.SetDataSource(ds.Tables[0]);
                rd.Database.Tables["StudentDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 6)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "Class Wise Students Leaving Report.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StudentDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 61)
            {
                //rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "StudentStrength.rpt"));
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "StudentStrengthReport.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StudentDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
			else if (model.reportId == 59 || model.reportId == 60)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "Daily Attendance Report.rpt"));
                //AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["DataTable1"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable2"].SetDataSource(AddImage());

                rd.SetParameterValue("ToDate", model.fromDate);
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 62)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "AttendanceMonthlyReport.rpt"));
                rd.Database.Tables["StudentDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("ForMonth", GetForMonth(model));
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 64)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "Students Terminated Report.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StudentDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 7)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "Student Attandance Report.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StudentDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 9)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "Class Attendance Report.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StudentDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 10)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "ClassAttendanceSummaryReport.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StudentDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else if (model.reportId == 67)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "Student SMS History Report.rpt"));
                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StudentDataTable"].SetDataSource(ds.Tables[0]);
                rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

                rd.SetParameterValue("FromDate", model.fromDate.ToString());
                rd.SetParameterValue("ToDate", model.toDate.ToString());
                rd.SetParameterValue("CampusName", config.CampusName);
                rd.SetParameterValue("SchoolName", config.SchoolName);

                return rd;
            }
            else
            {
                if (model.directoryOptionId == 1)
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "Students Contact Directory.rpt"));
                else
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/Students"), "Students Contact Directory AdmissionNo Wise.rpt"));

                AddParametersToReport(rd, model.fromDate, model.toDate);
                rd.Database.Tables["StudentDataTable"].SetDataSource(ds.Tables[0]);
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
