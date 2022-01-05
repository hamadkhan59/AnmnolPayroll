using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SMS_Web.Helpers.PdfHelper;
using SMS_DAL;
using SMS_Web.Helpers;
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS.Modules.BuildPdf
{
    public class StudentResultPdf : BasicPdf
    {
        private SC_WEBEntities2 db = SessionHelper.dbContext;

        string teacherRemarks;
        static int BranchId = 0;
        Nullable<int> m_percentOfExam;

        public PdfDocument CreatePdfOfAll(List<ExamResultViewModel> examdetail, int examTypeId, PdfDocument pdfdoc, int branchId, int examClassSectionId, DateTime IssuedDate, int[] Position, int[] StudentId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                if (examdetail != null && examdetail.Count > 0)
                {
                    ExamType etype = db.ExamTypes.Find(examTypeId);
                    string[] CourseName = new string[examdetail.Count];
                    string[] TotalMarks = new string[examdetail.Count];
                    string[] ObtMarks = new string[examdetail.Count];
                    string[] Grade = new string[examdetail.Count];
                    int i = 0;
                    int totalMarks = 0;
                    decimal obtainedMarks = 0;
                    BranchId = branchId;
                    ClassSection clsec = db.ClassSections.Find(examClassSectionId);
                    foreach (var exam in examdetail)
                    {
                        CourseName[i] = exam.CourseName;
                        TotalMarks[i] = exam.totalMarks;
                        ObtMarks[i] = Math.Round(decimal.Parse(exam.ObtMarks), 2).ToString();
                        Grade[i] = exam.Grade;
                        totalMarks += int.Parse(exam.totalMarks);
                        obtainedMarks += decimal.Parse(exam.ObtMarks);
                        i++;
                    }
                    Student student = db.Students.Find((int)examdetail[0].StudentId);
                    string remarks = SessionHelper.GetRemarks((decimal)obtainedMarks, (decimal)totalMarks);
                    CreatePdfParent(student, etype, remarks, CourseName, TotalMarks, ObtMarks, Grade, false, etype.Percent_Of_Total, pdfdoc, clsec.Class.Name, clsec.Section.Name, IssuedDate, Position, StudentId);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return pdfdoc;
        }

        public PdfDocument CreatePdfParent(Student student, ExamType examType, string remarks, string[] CourseName, string[] TotalMarks, string[] ObtMarks, string[] Grade, bool isAll, Nullable<int> percentOfExam, PdfDocument pdfdoc, string className, string sectionName, DateTime IssuedDate, int[] Position, int[] StudentId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                m_percentOfExam = percentOfExam;
                teacherRemarks = remarks;
                pdfdoc.Info.Title = "First Pdf";
                PdfPage page = pdfdoc.AddPage();
                //page.Height = 750;

                //Student student = db.Students.Find(studentId);
                //ExamType examType = db.ExamTypes.Find(examTypeId);
                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);

                grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 28, 110, 537, 30);
                grph.DrawRectangle(XPens.RoyalBlue, 25, 107, 543, 36);
                grph.DrawString("First Term Result Card", font, XBrushes.White, new XRect(0, 110, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                grph.DrawRectangle(XPens.RoyalBlue, 50, 165, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 130, 165, 130, 15);
                //grph.DrawRectangle(XPens.RoyalBlue, 260, 165, 40, 15);
                //grph.DrawRectangle(XPens.RoyalBlue, 300, 165, 100, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 50, 180, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 50, 195, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 130, 180, 50, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 130, 195, 50, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 180, 180, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 180, 195, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 260, 180, 140, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 260, 195, 140, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 400, 180, 60, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 400, 195, 60, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 460, 180, 90, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 460, 195, 90, 15);
                font = new XFont("Verdana", 8, XFontStyle.Regular);
                grph.DrawString("Date", font, XBrushes.RoyalBlue, new XRect(57, 167, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Roll No", font, XBrushes.RoyalBlue, new XRect(57, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Admission No", font, XBrushes.RoyalBlue, new XRect(57, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Name", font, XBrushes.RoyalBlue, new XRect(185, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Father Name", font, XBrushes.RoyalBlue, new XRect(185, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Class", font, XBrushes.RoyalBlue, new XRect(405, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Section", font, XBrushes.RoyalBlue, new XRect(405, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString(student.RollNumber.ToUpper(), font, XBrushes.Black, new XRect(140, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(student.AdmissionNo.ToUpper(), font, XBrushes.Black, new XRect(140, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(student.Name.ToUpper(), font, XBrushes.Black, new XRect(265, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(student.FatherName.ToUpper(), font, XBrushes.Black, new XRect(265, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(className.ToUpper(), font, XBrushes.Black, new XRect(465, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(sectionName.ToUpper(), font, XBrushes.Black, new XRect(465, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                string currDate = IssuedDate.Day.ToString().PadLeft(2, '0') + "-" + IssuedDate.Month.ToString().PadLeft(2, '0') + "-" + IssuedDate.Year.ToString();
                grph.DrawString(currDate, font, XBrushes.Black, new XRect(140, 167, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                Populate_Result1(page, pdfdoc, CourseName, TotalMarks, ObtMarks, Grade, grph, Position, StudentId, student);

                base.DesignBorder(grph, page, (int)page.Height);
                base.DesignSchoolHeader(grph, page, BranchId);

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return pdfdoc;
            //Process.Start(pdfFilename);
        }

        private void Populate_Result1(PdfPage page, PdfDocument doc, string[] CourseName, string[] TotalMarks, string[] ObtMarks, string[] Grade, XGraphics grph, int[] Position, int[] StudentId, Student student)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                grph.DrawRectangle(XPens.RoyalBlue, 50, 220, 200, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 250, 220, 125, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 375, 220, 100, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 475, 220, 75, 20);

                grph.DrawRectangle(XPens.RoyalBlue, 52, 222, 196, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 252, 222, 121, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 377, 222, 96, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 477, 222, 71, 16);

                XFont font = new XFont("Verdana", 10, XFontStyle.Bold);

                grph.DrawString("Subject Name", font, XBrushes.Black, new XRect(70, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Total Marks", font, XBrushes.Black, new XRect(270, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Obt Marks", font, XBrushes.Black, new XRect(390, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Grade", font, XBrushes.Black, new XRect(495, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                font = new XFont("Verdana", 10, XFontStyle.Regular);
                int rowPos = 0, newPosition = 0;
                decimal totalSubMarks = 0, totalObtMarks = 0;
                int position = 220;
                for (int i = 0; i < CourseName.Count(); i++)
                {
                    newPosition = position + (rowPos + 1) * 20;

                    if (newPosition >= page.Height - 100)
                    {
                        rowPos = 0;
                        position = 100;
                        page = doc.AddPage();
                        grph = XGraphics.FromPdfPage(page);
                        newPosition = position + (rowPos + 1) * 20;
                    }
                    grph.DrawRectangle(XPens.RoyalBlue, 50, newPosition, 200, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 250, newPosition, 125, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 375, newPosition, 100, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 475, newPosition, 75, 20);

                    newPosition += 5;
                    grph.DrawString(CourseName[i], font, XBrushes.Black, new XRect(60, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(TotalMarks[i], font, XBrushes.Black, new XRect(290, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    totalSubMarks += decimal.Parse(TotalMarks[i]);
                    grph.DrawString(ObtMarks[i], font, XBrushes.Black, new XRect(415, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    totalObtMarks += decimal.Parse(ObtMarks[i]);
                    grph.DrawString(Grade[i], font, XBrushes.Black, new XRect(505, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    rowPos++;
                }
                position++;
                newPosition = position + (rowPos + 1) * 20;
                grph.DrawRectangle(XPens.RoyalBlue, 50, newPosition, 200, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 250, newPosition, 125, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 375, newPosition, 100, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 475, newPosition, 75, 20);
                newPosition += 5;
                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Total", font, XBrushes.Black, new XRect(60, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Math.Round(totalSubMarks, 2).ToString(), font, XBrushes.Black, new XRect(290, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Math.Round(totalObtMarks, 2).ToString(), font, XBrushes.Black, new XRect(415, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                string totalGrade = SessionHelper.GetGrade(int.Parse(Math.Round(totalObtMarks, 0).ToString()), int.Parse(Math.Round(totalSubMarks, 0).ToString()), int.Parse(m_percentOfExam.ToString()));
                grph.DrawString(totalGrade, font, XBrushes.Black, new XRect(505, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                rowPos++;
                rowPos++;

                XFont font2 = new XFont("Verdana", 10, XFontStyle.Bold);
                font = new XFont("Verdana", 10, XFontStyle.Regular);

                if (SysConfig.GetSystemParam(SysConfig.EC_REMARKS_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, 50, 240 + (rowPos * 20), 125, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 175, 240 + (rowPos * 20), 375, 20);

                    grph.DrawString("Teacher Remarks", font2, XBrushes.Blue, new XRect(60, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    teacherRemarks = SessionHelper.GetRemarks((int)totalObtMarks, (int)totalSubMarks);
                    grph.DrawString(teacherRemarks, font, XBrushes.Black, new XRect(185, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    rowPos++;
                }


                int tempPos = 50;
                if (SysConfig.GetSystemParam(SysConfig.EC_GRADE_FLAG).ParamValue == "Yes")
                {
                    string grade = SessionHelper.GetGrade((int)totalObtMarks, (int)totalSubMarks, 50);
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 100, 20);
                    grph.DrawString("Grade", font2, XBrushes.Blue, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 100;
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 550 - tempPos, 20);
                    grph.DrawString(grade, font, XBrushes.Black, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 70;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_PERCENTAGE_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 100, 20);
                    grph.DrawString("Percentage", font2, XBrushes.Blue, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 100;
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 550 - tempPos, 20);
                    grph.DrawString(Math.Round(((decimal)totalObtMarks / (decimal)totalSubMarks) * 100m, 2).ToString() + " %", font, XBrushes.Black, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 70;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_POSITION_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 100, 20);
                    grph.DrawString("Position", font2, XBrushes.Blue, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 100;
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 550 - tempPos, 20);
                    int studentIndex = StudentId.ToList().IndexOf((int)student.id);
                    grph.DrawString(Position[studentIndex].ToString() + "", font, XBrushes.Black, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                rowPos += 5;
                tempPos = 50;
                font = new XFont("Verdana", 7, XFontStyle.Regular);
                if (SysConfig.GetSystemParam(SysConfig.EC_PRICIPLE_SIGN_FLAG).ParamValue == "Yes")
                {
                    base.DrawPrincipalSignature(grph, page, 60, position + (rowPos + 1) * 20 + 15, BranchId);
                    grph.DrawLine(XPens.Black, tempPos, position + (rowPos + 4) * 20 + 15, tempPos + 150, position + (rowPos + 4) * 20 + 15);
                    grph.DrawString("Principal's Signature", font, XBrushes.RoyalBlue, new XRect(tempPos, position + (rowPos + 5) * 20, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 150;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_TEACHER_SIGN_FLAG).ParamValue == "Yes")
                {
                    if (tempPos > 50)
                        tempPos += 30;
                    grph.DrawLine(XPens.Black, tempPos, position + (rowPos + 4) * 20 + 15, tempPos + 150, position + (rowPos + 4) * 20 + 15);
                    grph.DrawString("Teacher's Signature", font, XBrushes.RoyalBlue, new XRect(tempPos, position + (rowPos + 5) * 20, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 150;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_PARENT_SIGN_FLAG).ParamValue == "Yes")
                {
                    if (tempPos > 50)
                        tempPos += 30;
                    grph.DrawLine(XPens.Black, tempPos, position + (rowPos + 4) * 20 + 15, tempPos + 140, position + (rowPos + 4) * 20 + 15);
                    grph.DrawString("Parent's Signature", font, XBrushes.RoyalBlue, new XRect(tempPos, position + (rowPos + 5) * 20, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                rowPos += 1;
                if (SysConfig.GetSystemParam(SysConfig.EC_PASSING_PER_NOTE).ParamValue == "Yes")
                {
                    grph.DrawString("Note : Passing percentage is 40", font, XBrushes.Black, new XRect(50, position + (rowPos + 5) * 20 + 15, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            //grph.DrawString("Note : Passing percentage is 40", font, XBrushes.Black, new XRect(50, page.Height - 150, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
            //grph.DrawLine(XPens.Black, 100, position + (rowPos + 2) * 20 + 10, 550, position + (rowPos + 2) * 20 + 10);
        }

        public PdfDocument CreatePdf(Student student, ExamType examType, string remarks, string[] CourseName, string[] TotalMarks, string[] ObtMarks, string[] Grade, bool isAll, Nullable<int> percentOfExam, DateTime IssuedDate, int position, string className, string sectionName)
        {
            PdfDocument pdfdoc = new PdfDocument();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                m_percentOfExam = percentOfExam;
                teacherRemarks = remarks;
                pdfdoc.Info.Title = "First Pdf";
                PdfPage page = pdfdoc.AddPage();
                //page.Height = 750;

                //Student student = db.Students.Find(studentId);
                //ExamType examType = db.ExamTypes.Find(examTypeId);
                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);

                grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 28, 110, 537, 30);
                grph.DrawRectangle(XPens.RoyalBlue, 25, 107, 543, 36);
                grph.DrawString(examType.Name + " Report", font, XBrushes.White, new XRect(0, 110, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                grph.DrawRectangle(XPens.RoyalBlue, 50, 165, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 130, 165, 130, 15);
                //grph.DrawRectangle(XPens.RoyalBlue, 260, 165, 40, 15);
                //grph.DrawRectangle(XPens.RoyalBlue, 300, 165, 100, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 50, 180, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 50, 195, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 130, 180, 50, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 130, 195, 50, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 180, 180, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 180, 195, 80, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 260, 180, 140, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 260, 195, 140, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 400, 180, 60, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 400, 195, 60, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 460, 180, 90, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 460, 195, 90, 15);
                font = new XFont("Verdana", 8, XFontStyle.Regular);
                grph.DrawString("Date", font, XBrushes.RoyalBlue, new XRect(57, 167, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Roll No", font, XBrushes.RoyalBlue, new XRect(57, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Admission No", font, XBrushes.RoyalBlue, new XRect(57, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Name", font, XBrushes.RoyalBlue, new XRect(185, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Father Name", font, XBrushes.RoyalBlue, new XRect(185, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Class", font, XBrushes.RoyalBlue, new XRect(405, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Section", font, XBrushes.RoyalBlue, new XRect(405, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString(student.RollNumber.ToUpper(), font, XBrushes.Black, new XRect(140, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(student.AdmissionNo.ToUpper(), font, XBrushes.Black, new XRect(140, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(student.Name.ToUpper(), font, XBrushes.Black, new XRect(265, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(student.FatherName.ToUpper(), font, XBrushes.Black, new XRect(265, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(className.ToUpper(), font, XBrushes.Black, new XRect(465, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(sectionName.ToUpper(), font, XBrushes.Black, new XRect(465, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                string currDate = IssuedDate.Day.ToString().PadLeft(2, '0') + "-" + IssuedDate.Month.ToString().PadLeft(2, '0') + "-" + IssuedDate.Year.ToString();
                grph.DrawString(currDate, font, XBrushes.Black, new XRect(140, 167, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                Populate_Result(page, pdfdoc, CourseName, TotalMarks, ObtMarks, Grade, grph, position);

                base.DesignBorder(grph, page, (int)page.Height);
                base.DesignSchoolHeader(grph, page, (int)student.BranchId);

                BranchId = (int)student.BranchId;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return pdfdoc;
            //Process.Start(pdfFilename);
        }

        private void Populate_Result(PdfPage page, PdfDocument doc, string[] CourseName, string[] TotalMarks, string[] ObtMarks, string[] Grade, XGraphics grph, int studentPosition)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                grph.DrawRectangle(XPens.RoyalBlue, 50, 220, 200, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 250, 220, 125, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 375, 220, 100, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 475, 220, 75, 20);

                grph.DrawRectangle(XPens.RoyalBlue, 52, 222, 196, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 252, 222, 121, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 377, 222, 96, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 477, 222, 71, 16);

                XFont font = new XFont("Verdana", 10, XFontStyle.Bold);

                grph.DrawString("Subject Name", font, XBrushes.Black, new XRect(70, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Total Marks", font, XBrushes.Black, new XRect(270, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Obt Marks", font, XBrushes.Black, new XRect(390, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Grade", font, XBrushes.Black, new XRect(495, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                font = new XFont("Verdana", 10, XFontStyle.Regular);
                int rowPos = 0, newPosition = 0;
                double totalSubMarks = 0, totalObtMarks = 0;
                int position = 220;
                for (int i = 0; i < CourseName.Count(); i++)
                {
                    newPosition = position + (rowPos + 1) * 20;

                    if (newPosition >= page.Height - 100)
                    {
                        rowPos = 0;
                        position = 100;
                        page = doc.AddPage();
                        grph = XGraphics.FromPdfPage(page);
                        newPosition = position + (rowPos + 1) * 20;
                    }
                    grph.DrawRectangle(XPens.RoyalBlue, 50, newPosition, 200, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 250, newPosition, 125, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 375, newPosition, 100, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 475, newPosition, 75, 20);

                    newPosition += 5;
                    grph.DrawString(CourseName[i], font, XBrushes.Black, new XRect(60, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(TotalMarks[i], font, XBrushes.Black, new XRect(290, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    totalSubMarks += double.Parse(TotalMarks[i]);
                    grph.DrawString(ObtMarks[i], font, XBrushes.Black, new XRect(415, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    totalObtMarks += double.Parse(ObtMarks[i]);
                    grph.DrawString(Grade[i], font, XBrushes.Black, new XRect(505, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    rowPos++;
                }
                position++;
                newPosition = position + (rowPos + 1) * 20;
                grph.DrawRectangle(XPens.RoyalBlue, 50, newPosition, 200, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 250, newPosition, 125, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 375, newPosition, 100, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 475, newPosition, 75, 20);
                newPosition += 5;
                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Total", font, XBrushes.Black, new XRect(60, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Math.Round(totalSubMarks, 2).ToString(), font, XBrushes.Black, new XRect(290, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Math.Round(totalObtMarks, 2).ToString(), font, XBrushes.Black, new XRect(415, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                string totalGrade = SessionHelper.GetGrade(int.Parse(Math.Round(totalObtMarks, 0).ToString()), int.Parse(Math.Round(totalSubMarks, 0).ToString()), int.Parse(m_percentOfExam.ToString()));
                grph.DrawString(totalGrade, font, XBrushes.Black, new XRect(505, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                rowPos++;
                rowPos++;

                //pk = pk + 2;

                font = new XFont("Verdana", 10, XFontStyle.Regular);
                XFont font2 = new XFont("Verdana", 10, XFontStyle.Bold);
                

                if (SysConfig.GetSystemParam(SysConfig.EC_REMARKS_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, 50, 240 + (rowPos * 20), 125, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 175, 240 + (rowPos * 20), 375, 20);

                    grph.DrawString("Teacher Remarks", font2, XBrushes.Blue, new XRect(60, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    teacherRemarks = SessionHelper.GetRemarks((int)totalObtMarks, (int)totalSubMarks);
                    grph.DrawString(teacherRemarks, font, XBrushes.Black, new XRect(185, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    rowPos++;
                }


                int tempPos = 50;
                if (SysConfig.GetSystemParam(SysConfig.EC_GRADE_FLAG).ParamValue == "Yes")
                {
                    string grade = SessionHelper.GetGrade((int)totalObtMarks, (int)totalSubMarks, 50);
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 100, 20);
                    grph.DrawString("Grade", font2, XBrushes.Blue, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 100;
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 550-tempPos, 20);
                    grph.DrawString(grade, font, XBrushes.Black, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 70;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_PERCENTAGE_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 100, 20);
                    grph.DrawString("Percentage", font2, XBrushes.Blue, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 100;
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 550 - tempPos, 20);
                    grph.DrawString(Math.Round(((decimal)totalObtMarks / (decimal)totalSubMarks) * 100m, 2).ToString() + " %", font, XBrushes.Black, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 70;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_POSITION_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 100, 20);
                    grph.DrawString("Position", font2, XBrushes.Blue, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 100;
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 550 - tempPos, 20);
                    grph.DrawString(studentPosition + "", font, XBrushes.Black, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                rowPos += 5;
                font = new XFont("Verdana", 7, XFontStyle.Regular);

                tempPos = 50;
                if (SysConfig.GetSystemParam(SysConfig.EC_PRICIPLE_SIGN_FLAG).ParamValue == "Yes")
                {
                    base.DrawPrincipalSignature(grph, page, 60, position + (rowPos + 1) * 20 + 15, BranchId);
                    grph.DrawLine(XPens.Black, tempPos, position + (rowPos + 4) * 20 + 15, tempPos + 150, position + (rowPos + 4) * 20 + 15);
                    grph.DrawString("Principal's Signature", font, XBrushes.RoyalBlue, new XRect(tempPos, position + (rowPos + 5) * 20, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 150;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_TEACHER_SIGN_FLAG).ParamValue == "Yes")
                {
                    if (tempPos > 50)
                        tempPos += 30;
                    grph.DrawLine(XPens.Black, tempPos, position + (rowPos + 4) * 20 + 15, tempPos + 150, position + (rowPos + 4) * 20 + 15);
                    grph.DrawString("Teacher's Signature", font, XBrushes.RoyalBlue, new XRect(tempPos, position + (rowPos + 5) * 20, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 150;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_PARENT_SIGN_FLAG).ParamValue == "Yes")
                {
                    if (tempPos > 50)
                        tempPos += 30;
                    grph.DrawLine(XPens.Black, tempPos, position + (rowPos + 4) * 20 + 15, tempPos + 140, position + (rowPos + 4) * 20 + 15);
                    grph.DrawString("Parent's Signature", font, XBrushes.RoyalBlue, new XRect(tempPos, position + (rowPos + 5) * 20, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                rowPos += 1;
                if (SysConfig.GetSystemParam(SysConfig.EC_PASSING_PER_NOTE_FLAG).ParamValue == "Yes")
                {
                    grph.DrawString(SysConfig.GetSystemParam(SysConfig.EC_PASSING_PER_NOTE).ParamValue, font, XBrushes.Black, new XRect(50, position + (rowPos + 5) * 20 + 15, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                //grph.DrawLine(XPens.Black, 100, position + (rowPos + 2) * 20 + 10, 550, position + (rowPos + 2) * 20 + 10);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private string GetGrade(int obtained, int totalMarks, int passPercentage)
        {
            int obtPercentage = (obtained * 100) / totalMarks;
            string grade = null;
            if (obtPercentage >= 95)
                grade = "A+";
            else if (obtPercentage >= 90)
                grade = "A";
            else if (obtPercentage >= 85)
                grade = "A-";
            else if (obtPercentage >= 80)
                grade = "B+";
            else if (obtPercentage >= 75)
                grade = "B";
            else if (obtPercentage >= 70)
                grade = "B-";
            else if (obtPercentage >= 65)
                grade = "C+";
            else
                grade = "C";

            return grade;
        }
        //public string GetGrade(int obtained, int totalMarks, int passPercentage)
        //{
        //    int obtPercentage = (obtained * 100) / totalMarks;
        //    string grade = null;
        //    if (obtPercentage >= 85)
        //        grade = "A+";
        //    else if (obtPercentage >= 80)
        //        grade = "A";
        //    else if (obtPercentage >= 75)
        //        grade = "B+";
        //    else if (obtPercentage >= 70)
        //        grade = "B";
        //    else if (obtPercentage >= 65)
        //        grade = "C+";
        //    else if (obtPercentage >= 60)
        //        grade = "C";
        //    else if (obtPercentage < passPercentage)
        //        grade = "F";
        //    else
        //        grade = "D";

        //    return grade;
        //}
    }
}
