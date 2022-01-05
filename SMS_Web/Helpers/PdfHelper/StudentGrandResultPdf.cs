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
using SMS_Web.Helpers;
using SMS_DAL;
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS.Modules.BuildPdf
{
    public class StudentGrandResultPdf : BasicPdf
    {
        string teacherRemarks;
        static int BranchId = 0;

        public PdfDocument CreatePdf(StudentModel student, string className, string secName, string remarks, List<DataSet> resultDatatableList, int branchId, int AutoRemarks, DateTime issueDate)
        {
            PdfDocument pdfdoc = new PdfDocument();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                teacherRemarks = remarks;
                BranchId = branchId;
                int examNameCount = 0;
                for (int m = 0; m < resultDatatableList.Count; m++)
                {
                    DataSet ds = resultDatatableList[m];
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        DataTable dt = ds.Tables[i];
                        PdfPage page = pdfdoc.AddPage();
                        //page.Height = 750;
                        XGraphics grph = XGraphics.FromPdfPage(page);
                        XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                        grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 28, 110, 537, 30);
                        grph.DrawRectangle(XPens.RoyalBlue, 25, 107, 543, 36);
                        grph.DrawString("Student Exam Report", font, XBrushes.White, new XRect(0, 110, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                        font = new XFont("Verdana", 10, XFontStyle.Regular);
                        grph.DrawRectangle(XPens.RoyalBlue, 50, 165, 80, 15);
                        grph.DrawRectangle(XPens.RoyalBlue, 130, 165, 130, 15);
                        grph.DrawRectangle(XPens.RoyalBlue, 260, 165, 40, 15);
                        grph.DrawRectangle(XPens.RoyalBlue, 300, 165, 100, 15);
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
                        grph.DrawString("Exam", font, XBrushes.RoyalBlue, new XRect(57, 167, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString("Date", font, XBrushes.RoyalBlue, new XRect(265, 167, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString("Roll No", font, XBrushes.RoyalBlue, new XRect(57, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString("Admission No", font, XBrushes.RoyalBlue, new XRect(57, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString("Name", font, XBrushes.RoyalBlue, new XRect(185, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString("Father Name", font, XBrushes.RoyalBlue, new XRect(185, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString("Class", font, XBrushes.RoyalBlue, new XRect(405, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString("Section", font, XBrushes.RoyalBlue, new XRect(405, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                        font = new XFont("Verdana", 8, XFontStyle.Regular);
                        grph.DrawString(student.RollNumber.ToUpper(), font, XBrushes.Black, new XRect(140, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString(student.AdmissionNo.ToUpper(), font, XBrushes.Black, new XRect(140, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString(student.Name.ToUpper(), font, XBrushes.Black, new XRect(265, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString(student.FatherName.ToUpper(), font, XBrushes.Black, new XRect(265, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString(className, font, XBrushes.Black, new XRect(465, 182, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString(secName, font, XBrushes.Black, new XRect(465, 197, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        string currDate = issueDate.Day.ToString().PadLeft(2, '0') + "-" + issueDate.Month.ToString().PadLeft(2, '0') + "-" + issueDate.Year.ToString();
                        grph.DrawString(currDate, font, XBrushes.Black, new XRect(305, 167, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                        font = new XFont("Verdana", 8, XFontStyle.Regular);
                        string examName = "";

                        if (dt.Columns.Contains("ExamTypeName"))
                        {
                            examName = dt.Rows[0]["ExamTypeName"].ToString();
                        }
                        else if (dt.Columns.Contains("TermName"))
                        {
                            examName = dt.Rows[0]["TermName"].ToString();
                        }
                        else
                        {
                            examName = "Complete Session Result";
                        }

                        while (!dt.Columns[0].ColumnName.Equals("SubjectName"))
                        {
                            dt.Columns.Remove(dt.Columns[0].ColumnName);
                        }

                        grph.DrawString(examName.ToUpper(), font, XBrushes.Black, new XRect(140, 167, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                        Populate_Result(page, pdfdoc, dt, grph, AutoRemarks);
                        base.DesignBorder(grph, page, (int)page.Height);
                        base.DesignSchoolHeader(grph, page, BranchId);
                        examNameCount++;
                    }
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

        public void CreatePdf(string rollno, string studName, string className, string secName, string examName, string remarks, DataTable dt, int AutoRemarks)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                teacherRemarks = remarks;
                PdfDocument pdfdoc = new PdfDocument();
                pdfdoc.Info.Title = "First Pdf";
                PdfPage page = pdfdoc.AddPage();

                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 150, 130, 310, 30);
                grph.DrawRectangle(XPens.RoyalBlue, 147, 127, 316, 36);
                grph.DrawString(examName + " Report", font, XBrushes.White, new XRect(0, 130, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString("Student Name", font, XBrushes.RoyalBlue, new XRect(50, 180, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(studName.ToUpper(), font, XBrushes.Black, new XRect(135, 175, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 122, 190, 355, 190);

                grph.DrawString("Roll Number", font, XBrushes.RoyalBlue, new XRect(360, 180, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(rollno.ToUpper(), font, XBrushes.Black, new XRect(435, 175, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 422, 190, 550, 190);

                grph.DrawString("Class", font, XBrushes.RoyalBlue, new XRect(50, 200, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(className.ToUpper(), font, XBrushes.Black, new XRect(97, 195, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 77, 210, 225, 210);

                grph.DrawString("Section", font, XBrushes.RoyalBlue, new XRect(230, 200, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(secName.ToUpper(), font, XBrushes.Black, new XRect(287, 195, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 267, 210, 380, 210);

                grph.DrawString("Exam", font, XBrushes.RoyalBlue, new XRect(385, 200, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(examName.ToUpper(), font, XBrushes.Black, new XRect(427, 195, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 414, 210, 550, 210);

                page.Height = Populate_Result(page, pdfdoc, dt, grph, 0);

                base.DesignBorder(grph, page, (int)page.Height);
                base.DesignSchoolHeader(grph, page, BranchId);

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private int Populate_Result(PdfPage page, PdfDocument doc, DataTable dt, XGraphics grph, int AutoRemarks)
        {
            int height = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                if (dt.Columns.Contains("TotalMarks"))
                    dt.Columns.Remove("TotalMarks");

                if (SysConfig.GetSystemParam(SysConfig.EC_GRADE_FLAG).ParamValue == "No")
                {
                    dt.Columns.Remove("Grade");
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_POSITION_FLAG).ParamValue == "No")
                {
                    dt.Columns.Remove("Position");
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_PERCENTAGE_FLAG).ParamValue == "No")
                {
                    dt.Columns.Remove("Percentage");
                }

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
                grph.DrawString("Obt Marks", font, XBrushes.Black, new XRect(270, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Total Marks", font, XBrushes.Black, new XRect(390, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Grade", font, XBrushes.Black, new XRect(495, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                font = new XFont("Verdana", 10, XFontStyle.Regular);

                double m_totalSubMarks = 0, m_totalObtMarks = 0;
                int rowPos = 0;
                int position = 220;
                for (int i = 0; i < dt.Rows.Count - 3; i++)
                {
                    DataRow row = dt.Rows[i];

                    int newPosition = position + (rowPos + 1) * 20;

                    if (newPosition >= page.Height - 100)
                    {
                        rowPos = 0;
                        position = 100;
                        page = doc.AddPage();
                        page.Height = 600;
                        grph = XGraphics.FromPdfPage(page);
                        base.DesignBorder(grph, page, (int)page.Height);
                        base.DesignSchoolHeader(grph, page, BranchId);
                        newPosition = position + (rowPos + 1) * 20;
                    }
                    grph.DrawRectangle(XPens.RoyalBlue, 50, newPosition, 200, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 250, newPosition, 125, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 375, newPosition, 100, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 475, newPosition, 75, 20);

                    newPosition += 5;

                    grph.DrawString(row[0].ToString(), font, XBrushes.Black, new XRect(60, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(row[1].ToString(), font, XBrushes.Black, new XRect(290, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    m_totalObtMarks += double.Parse(row[1].ToString());
                    grph.DrawString(row[2].ToString(), font, XBrushes.Black, new XRect(415, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    m_totalSubMarks += double.Parse(row[2].ToString());
                    grph.DrawString(row[3].ToString(), font, XBrushes.Black, new XRect(505, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    rowPos++;
                }
                int newTotalPosition = position + (rowPos + 1) * 20;
                grph.DrawRectangle(XPens.RoyalBlue, 50, newTotalPosition, 200, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 250, newTotalPosition, 125, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 375, newTotalPosition, 100, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 475, newTotalPosition, 75, 20);
                newTotalPosition += 5;

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Total", font, XBrushes.Black, new XRect(60, newTotalPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Math.Round(m_totalObtMarks, 2).ToString(), font, XBrushes.Black, new XRect(290, newTotalPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Math.Round(m_totalSubMarks, 2).ToString(), font, XBrushes.Black, new XRect(415, newTotalPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                string totalGrade = SessionHelper.GetGrade(int.Parse(Math.Round(m_totalObtMarks, 0).ToString()), int.Parse(Math.Round(m_totalSubMarks, 0).ToString()), 33);
                if (AutoRemarks == 1)
                    teacherRemarks = SessionHelper.GetRemarks(int.Parse(Math.Round(m_totalObtMarks, 0).ToString()), int.Parse(Math.Round(m_totalSubMarks, 0).ToString()), 33);
                grph.DrawString(totalGrade, font, XBrushes.Black, new XRect(505, newTotalPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                
                rowPos += 2;
                newTotalPosition = position + (rowPos + 1) * 20;

                XFont font2 = new XFont("Verdana", 10, XFontStyle.Bold);
                if (SysConfig.GetSystemParam(SysConfig.EC_REMARKS_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, 50, newTotalPosition, 125, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 175, newTotalPosition, 375, 20);

                    grph.DrawString("Teacher Remarks", font2, XBrushes.Black, new XRect(60, position + (rowPos + 1) * 20 + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    font = new XFont("Verdana", 10, XFontStyle.Regular);
                    grph.DrawString(teacherRemarks, font, XBrushes.Black, new XRect(210, position + (rowPos + 1) * 20 + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    rowPos++;
                }
                
                int tempPos = 50;
                if (SysConfig.GetSystemParam(SysConfig.EC_GRADE_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 100, 20);
                    grph.DrawString("Grade", font2, XBrushes.Blue, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 100;
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 550 - tempPos, 20);
                    grph.DrawString(dt.Rows[dt.Rows.Count - 2][3].ToString(), font, XBrushes.Black, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 70;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_PERCENTAGE_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 100, 20);
                    grph.DrawString("Percentage", font2, XBrushes.Blue, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 100;
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 550 - tempPos, 20);
                    grph.DrawString(dt.Rows[dt.Rows.Count - 1][1].ToString()+"%", font, XBrushes.Black, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 70;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_POSITION_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 100, 20);
                    grph.DrawString("Position", font2, XBrushes.Blue, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 100;
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (rowPos * 20), 550 - tempPos, 20);
                    grph.DrawString(dt.Rows[dt.Rows.Count - 1][3].ToString(), font, XBrushes.Black, new XRect(tempPos + 10, 240 + (rowPos * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                int tempRowPos = rowPos;
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
                if (SysConfig.GetSystemParam(SysConfig.EC_PASSING_PER_NOTE_FLAG).ParamValue == "Yes")
                {
                    grph.DrawString(SysConfig.GetSystemParam(SysConfig.EC_PASSING_PER_NOTE).ParamValue, font, XBrushes.Black, new XRect(50, position + (rowPos + 5) * 20 + 15, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }

            return height;
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
    }
}
