using CrystalDecisions.CrystalReports.Engine;
using SMS_DAL.Reports;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS_Web.Controllers.ExamsConduct
{
    public class ExamReportsController : Controller
    {
        //
        // GET: /ExamReports/

        public ActionResult Index()
        {
            return View();
        }


        public FileStreamResult GetTeacherAnalaysisReportChart()
        {
            DAL_Exam_Reports report = new DAL_Exam_Reports();
            DataSet reportSet = report.GetFeeCollectionReportDataGraphical();

            ReportDocument rd = null;

            rd = createReport(reportSet, "TeacherAnalysisChart.rpt");

            Stream stream = GetPdf(rd);
            return File(stream, "application/pdf");
        }

        public FileStreamResult GetAnalaysisReport()
        {
            DAL_Exam_Reports report = new DAL_Exam_Reports();
            DataSet reportSet = report.GetFeeCollectionReportData();

            ReportDocument rd = null;

            rd = createReport(reportSet, "TeacherAnalysis.rpt");

            Stream stream = GetPdf(rd);
            return File(stream, "application/pdf");
        }

        public FileStreamResult SubjectWiseTeacherPerformanceChart()
        {
            DAL_Exam_Reports report = new DAL_Exam_Reports();
            DataSet reportSet = report.SubjectWiseTeacherAnalysisData();

            ReportDocument rd = null;

            rd = createReport(reportSet, "SubjectAnalysisTeacherWiseChart.rpt");

            Stream stream = GetPdf(rd);
            return File(stream, "application/pdf");
        }

        public FileStreamResult SubjectWiseTeacherPerformance()
        {
            DAL_Exam_Reports report = new DAL_Exam_Reports();
            DataSet reportSet = report.SubjectWiseTeacherAnalysisData();

            ReportDocument rd = null;

            rd = createReport(reportSet, "SubjectWiseTeacherAnalysisSheet.rpt");

            Stream stream = GetPdf(rd);
            return File(stream, "application/pdf");
        }

        private ReportDocument createReport(DataSet ds, string reportName)
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/Exam"), reportName));
            rd.SetDataSource(ds.Tables[0]);
            rd.SetParameterValue("CampusName", "");
            rd.SetParameterValue("SchoolName", "");

            return rd;
        }

        private Stream GetPdf(ReportDocument rd)
        {
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            var contentLength = stream.Length;
            Response.AppendHeader("Content-Length", contentLength.ToString());
            Response.AppendHeader("Content-Disposition", "inline; filename=Analysis.pdf");

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

    }
}
