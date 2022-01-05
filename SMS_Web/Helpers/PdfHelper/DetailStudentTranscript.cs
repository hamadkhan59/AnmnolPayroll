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
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS.Modules.BuildPdf
{
    public class DetailStudentTranscript : BasicPdf
    {
        string teacherRemarks;
        static int BranchId = 0;
        int rectX = 40, rectY = 220;
        int strX = 48, strY = 223;
        int drawX = 0, drawY = 0;
        int displayCount = 0;
        int pageCount = 1;
        double m_totalSubMarks = 0, m_totalObtMarks = 0;
        public PdfDocument CreatePdf(StudentModel student, string className, string secName, string remarks, List<DataSet> resultDatatableList, int branchId, int AutoRemarks, PdfDocument pdfdoc, List<ActivityMarksViewModel> activitiesList, DateTime IssuedDate)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                teacherRemarks = remarks;
                BranchId = branchId;
                PdfPage page = pdfdoc.AddPage();
                page.Height = 650;
                int examNameCount = 0;

                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 28, 110, 537, 30);
                grph.DrawRectangle(XPens.RoyalBlue, 25, 107, 543, 36);
                //grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 150, 110, 310, 30);
                //grph.DrawRectangle(XPens.RoyalBlue, 147, 107, 316, 36);
                grph.DrawString("Student Exam Report", font, XBrushes.White, new XRect(0, 110, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawRectangle(XPens.RoyalBlue, 50, 165, 40, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 90, 165, 90, 15);
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
                grph.DrawString("Date", font, XBrushes.RoyalBlue, new XRect(57, 167, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
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
                string currDate = IssuedDate.Day.ToString().PadLeft(2, '0') + "-" + IssuedDate.Month.ToString().PadLeft(2, '0') + "-" + IssuedDate.Year.ToString();
                grph.DrawString(currDate, font, XBrushes.Black, new XRect(95, 167, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                for (int m = 0; m < resultDatatableList.Count; m++)
                {
                    DataSet ds = resultDatatableList[m];
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        DataTable dt = ds.Tables[i];
                        if (displayCount == 4)
                        {
                            base.DesignBorder(grph, page, (int)page.Height);
                            base.DesignSchoolHeader(grph, page, BranchId);
                            page = pdfdoc.AddPage();
                            page.Height = 650;
                            grph = XGraphics.FromPdfPage(page);
                            pageCount++;
                            base.DesignBorder(grph, page, (int)page.Height);
                            base.DesignSchoolHeader(grph, page, BranchId);
                        }
                        //grph.DrawString("Exam", font, XBrushes.RoyalBlue, new XRect(385, 200, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                        font = new XFont("Verdana", 6, XFontStyle.Regular);

                        string examName = "";

                        if (dt.Columns.Contains("ExamTypeName"))
                        {
                            examName = "EXAM : " + dt.Rows[0]["ExamTypeName"].ToString().ToUpper();
                        }
                        else if (dt.Columns.Contains("TermName"))
                        {
                            examName = "TERM : " + dt.Rows[0]["TermName"].ToString().ToUpper();
                        }
                        else
                        {
                            examName = "SESSION RESULT";
                        }

                        while (!dt.Columns[0].ColumnName.Equals("SubjectName"))
                        {
                            dt.Columns.Remove(dt.Columns[0].ColumnName);
                        }

                        //grph.DrawString(examName.ToUpper(), font, XBrushes.Black, new XRect(427, 195, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        //grph.DrawString("Final Result", font, XBrushes.Black, new XRect(427, 195, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        //grph.DrawLine(XPens.Black, 414, 210, 550, 210);

                        Populate_Result(page, pdfdoc, dt, grph, AutoRemarks, examName);

                        examNameCount++;
                        displayCount++;
                    }
                }


                if (displayCount < 4)
                {
                    base.DesignBorder(grph, page, (int)page.Height);
                    base.DesignSchoolHeader(grph, page, BranchId);
                }
                else
                {
                    base.DesignBorder(grph, page, (int)page.Height);
                    base.DesignSchoolHeader(grph, page, BranchId);
                    if (pageCount == 1)
                    {
                        page = pdfdoc.AddPage();
                        page.Height = 650;
                        grph = XGraphics.FromPdfPage(page);
                        base.DesignBorder(grph, page, (int)page.Height);
                        base.DesignSchoolHeader(grph, page, BranchId);
                    }
                }

                Populate_Activities(page, pdfdoc, grph, activitiesList);
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

                Populate_Result(page, pdfdoc, dt, grph, 0, "");

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

        private void Populate_Result(PdfPage page, PdfDocument doc, DataTable dt, XGraphics grph, int AutoRemarks, string examName)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                if (dt.Columns.Contains("TotalMarks"))
                    dt.Columns.Remove("TotalMarks");
                rectX = rectX + drawX;
                rectY = rectY + drawY;
                strX += drawX;
                strY += drawY;
                XFont font = new XFont("Verdana", 8, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, rectX, rectY, 255, 15);
                grph.DrawString(examName, font, XBrushes.Black, new XRect(strX + 50, strY, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                rectY += 15;
                strY += 15;
                grph.DrawRectangle(XPens.RoyalBlue, rectX, rectY, 100, 15);
                grph.DrawRectangle(XPens.RoyalBlue, rectX + 100, rectY, 60, 15);
                grph.DrawRectangle(XPens.RoyalBlue, rectX + 100 + 60, rectY, 60, 15);
                grph.DrawRectangle(XPens.RoyalBlue, rectX + 100 + 60 + 60, rectY, 35, 15);

                //grph.DrawRectangle(XPens.RoyalBlue, rectX + 2, rectY + 2, 96, 11);
                //grph.DrawRectangle(XPens.RoyalBlue, rectX + 2 + 100, rectY +2 , 56, 11);
                //grph.DrawRectangle(XPens.RoyalBlue, rectX + 2 + 100 + 60, rectY + 2, 56, 11);
                //grph.DrawRectangle(XPens.RoyalBlue, rectX + 2 + 100 + 60 + 60, rectY + 2, 31, 11);

                grph.DrawString("Subject Name", font, XBrushes.Black, new XRect(strX, strY, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Obt Marks", font, XBrushes.Black, new XRect(strX + 100, strY, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Total Marks", font, XBrushes.Black, new XRect(strX + 100 + 56, strY, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Grade", font, XBrushes.Black, new XRect(strX + 100 + 60 + 56, strY, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                font = new XFont("Verdana", 8, XFontStyle.Regular);

                m_totalSubMarks = 0; m_totalObtMarks = 0;
                int rowPos = 0;
                int position = rectY;
                for (int i = 0; i < dt.Rows.Count - 3; i++)
                {
                    int newPosition = position + (rowPos + 1) * 15;

                    //if (newPosition >= page.Height - 100)
                    //{
                    //    rowPos = 0;
                    //    position = 100;
                    //    page = doc.AddPage();
                    //    page.Height = 600;
                    //    grph = XGraphics.FromPdfPage(page);
                    //    base.DesignBorder(grph, page, (int)page.Height);
                    //    base.DesignSchoolHeader(grph, page, BranchId);
                    //    newPosition = position + (rowPos + 1) * 20;
                    //}
                    grph.DrawRectangle(XPens.RoyalBlue, rectX, newPosition, 100, 15);
                    grph.DrawRectangle(XPens.RoyalBlue, rectX + 100, newPosition, 60, 15);
                    grph.DrawRectangle(XPens.RoyalBlue, rectX + 100 + 60, newPosition, 60, 15);
                    grph.DrawRectangle(XPens.RoyalBlue, rectX + 100 + 60 + 60, newPosition, 35, 15);

                    newPosition += 5;
                    DataRow row = dt.Rows[i];
                    grph.DrawString(row[0].ToString(), font, XBrushes.Black, new XRect(strX, newPosition - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(row[1].ToString(), font, XBrushes.Black, new XRect(strX + 100, newPosition - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    m_totalObtMarks += double.Parse(row[1].ToString());
                    grph.DrawString(row[2].ToString(), font, XBrushes.Black, new XRect(strX + 100 + 60, newPosition - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    m_totalSubMarks += double.Parse(row[2].ToString());
                    grph.DrawString(row[3].ToString(), font, XBrushes.Black, new XRect(strX + 100 + 60 + 60, newPosition - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    rowPos++;
                }
                int newTotalPosition = position + (rowPos + 1) * 15;
                grph.DrawRectangle(XPens.RoyalBlue, rectX, newTotalPosition, 100, 15);
                grph.DrawRectangle(XPens.RoyalBlue, rectX + 100, newTotalPosition, 60, 15);
                grph.DrawRectangle(XPens.RoyalBlue, rectX + 100 + 60, newTotalPosition, 60, 15);
                grph.DrawRectangle(XPens.RoyalBlue, rectX + 100 + 60 + 60, newTotalPosition, 35, 15);
                newTotalPosition += 5;

                font = new XFont("Verdana", 8, XFontStyle.Bold);
                grph.DrawString("Total", font, XBrushes.Black, new XRect(strX, newTotalPosition - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Math.Round(m_totalObtMarks, 2).ToString(), font, XBrushes.Black, new XRect(strX + 100, newTotalPosition - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Math.Round(m_totalSubMarks, 2).ToString(), font, XBrushes.Black, new XRect(strX + 100 + 60, newTotalPosition - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                string totalGrade = SessionHelper.GetGrade(int.Parse(Math.Round(m_totalObtMarks, 0).ToString()), int.Parse(Math.Round(m_totalSubMarks, 0).ToString()), 33);
                if (AutoRemarks == 1)
                    teacherRemarks = SessionHelper.GetRemarks(int.Parse(Math.Round(m_totalObtMarks, 0).ToString()), int.Parse(Math.Round(m_totalSubMarks, 0).ToString()), 33);
                grph.DrawString(totalGrade, font, XBrushes.Black, new XRect(strX + 100 + 60 + 60, newTotalPosition - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                rowPos++;

                newTotalPosition = position + (rowPos + 1) * 15;
                int tempRectX = rectX;
                int tempStrX = strX;
                if (SysConfig.GetSystemParam(SysConfig.EC_PERCENTAGE_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, rectX, newTotalPosition, 100, 15);
                    grph.DrawString(dt.Rows[dt.Rows.Count - 1][0].ToString(), font, XBrushes.Black, new XRect(strX, newTotalPosition + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    rectX += 100;
                    strX += 100;

                    grph.DrawRectangle(XPens.RoyalBlue, rectX, newTotalPosition, 155, 15);
                    grph.DrawString(dt.Rows[dt.Rows.Count - 1][1].ToString() + "%", font, XBrushes.Black, new XRect(strX, newTotalPosition + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    rectX += 60;
                    strX += 60;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_POSITION_FLAG).ParamValue == "Yes")
                {
                    if (tempRectX == rectX)
                    {
                        grph.DrawRectangle(XPens.RoyalBlue, rectX, newTotalPosition, 100, 15);
                        grph.DrawString(dt.Rows[dt.Rows.Count - 1][2].ToString(), font, XBrushes.Black, new XRect(strX, newTotalPosition + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        rectX += 100;
                        strX += 100;
                        grph.DrawRectangle(XPens.RoyalBlue, rectX, newTotalPosition, 155, 15);
                        grph.DrawString(dt.Rows[dt.Rows.Count - 1][3].ToString(), font, XBrushes.Black, new XRect(strX, newTotalPosition + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    }
                    else
                    {
                        grph.DrawRectangle(XPens.RoyalBlue, rectX, newTotalPosition, 60, 15);
                        grph.DrawString(dt.Rows[dt.Rows.Count - 1][2].ToString(), font, XBrushes.Black, new XRect(strX, newTotalPosition + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        rectX += 60;
                        strX += 60;
                        grph.DrawRectangle(XPens.RoyalBlue, rectX, newTotalPosition, 35, 15);
                        grph.DrawString(dt.Rows[dt.Rows.Count - 1][3].ToString(), font, XBrushes.Black, new XRect(strX, newTotalPosition + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    }

                }

                newTotalPosition += 5;

                if (SysConfig.GetSystemParam(SysConfig.EC_REMARKS_FLAG).ParamValue == "Yes")
                {
                    rowPos++;
                    newTotalPosition = position + (rowPos + 1) * 15;
                    grph.DrawRectangle(XPens.RoyalBlue, tempRectX, newTotalPosition, 100, 15);
                    grph.DrawRectangle(XPens.RoyalBlue, tempRectX + 100, newTotalPosition, 155, 15);

                    grph.DrawString("Teacher Remarks", font, XBrushes.Black, new XRect(tempStrX, newTotalPosition + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    grph.DrawString(teacherRemarks, font, XBrushes.Black, new XRect(tempStrX + 100, newTotalPosition + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                drawX = drawX == 0 ? 270 : 0;
                if (drawX == 0)
                {
                    drawY = drawY == 0 ? newTotalPosition - rectY + 45 : 0;
                }
                if (displayCount >= 3)
                {
                    rectX = 40; rectY = 120;
                    strX = 48; strY = 123;
                }
                else
                {
                    rectX = 40; rectY = 220;
                    strX = 48; strY = 223;
                }
                if (newTotalPosition >= page.Height - 50)
                {
                    page.Height = page.Height + (newTotalPosition - (page.Height - 50));
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }


        private void Populate_Activities(PdfPage page, PdfDocument doc, XGraphics grph, List<ActivityMarksViewModel> activitiesList)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                rectX = rectX + drawX;
                rectY = rectY + drawY;
                strX += drawX;
                strY += drawY;
                int rowPos = 0;
                int position = rectY;
                XFont font = new XFont("Verdana", 8, XFontStyle.Bold);
                if (activitiesList != null && activitiesList.Count > 0)
                {
                    grph.DrawRectangle(XPens.RoyalBlue, rectX, rectY, 255, 15);
                    grph.DrawString("ACTIVITIES", font, XBrushes.Black, new XRect(strX + 50, strY, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    rectY += 15;
                    strY += 15;
                    grph.DrawRectangle(XPens.RoyalBlue, rectX, rectY, 100, 15);
                    grph.DrawRectangle(XPens.RoyalBlue, rectX + 100, rectY, 60, 15);
                    grph.DrawRectangle(XPens.RoyalBlue, rectX + 100 + 60, rectY, 60, 15);
                    grph.DrawRectangle(XPens.RoyalBlue, rectX + 100 + 60 + 60, rectY, 35, 15);

                    //grph.DrawRectangle(XPens.RoyalBlue, rectX + 2, rectY + 2, 96, 11);
                    //grph.DrawRectangle(XPens.RoyalBlue, rectX + 2 + 100, rectY +2 , 56, 11);
                    //grph.DrawRectangle(XPens.RoyalBlue, rectX + 2 + 100 + 60, rectY + 2, 56, 11);
                    //grph.DrawRectangle(XPens.RoyalBlue, rectX + 2 + 100 + 60 + 60, rectY + 2, 31, 11);

                    grph.DrawString("Activity Name", font, XBrushes.Black, new XRect(strX, strY, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString("Obt Marks", font, XBrushes.Black, new XRect(strX + 100, strY, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString("Total Marks", font, XBrushes.Black, new XRect(strX + 100 + 56, strY, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString("Grade", font, XBrushes.Black, new XRect(strX + 100 + 60 + 56, strY, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    rowPos++;

                    if (activitiesList != null && activitiesList.Count > 0)
                    {
                        for (int i = 0; i < activitiesList.Count; i++)
                        {
                            int newPosition = position + (rowPos + 1) * 15;

                            grph.DrawRectangle(XPens.RoyalBlue, rectX, newPosition, 100, 15);
                            grph.DrawRectangle(XPens.RoyalBlue, rectX + 100, newPosition, 60, 15);
                            grph.DrawRectangle(XPens.RoyalBlue, rectX + 100 + 60, newPosition, 60, 15);
                            grph.DrawRectangle(XPens.RoyalBlue, rectX + 100 + 60 + 60, newPosition, 35, 15);

                            newPosition += 5;
                            grph.DrawString(activitiesList[i].ActitivtyName.ToString(), font, XBrushes.Black, new XRect(strX, newPosition - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                            grph.DrawString(activitiesList[i].ObtMarks.ToString(), font, XBrushes.Black, new XRect(strX + 100, newPosition - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                            m_totalObtMarks += double.Parse(activitiesList[i].ObtMarks.ToString());
                            grph.DrawString(activitiesList[i].TotalMarks.ToString(), font, XBrushes.Black, new XRect(strX + 100 + 60, newPosition - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                            m_totalSubMarks += double.Parse(activitiesList[i].TotalMarks.ToString());
                            string grade = SessionHelper.GetGrade((int)activitiesList[i].ObtMarks, (int)activitiesList[i].TotalMarks, 50);
                            grph.DrawString(grade, font, XBrushes.Black, new XRect(strX + 100 + 60 + 60, newPosition - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                            rowPos++;
                        }
                    }
                }
                int newTotalPosition = position + (rowPos + 1) * 15;
                grph.DrawRectangle(XPens.RoyalBlue, rectX, newTotalPosition, 255, 15);
                font = new XFont("Verdana", 8, XFontStyle.Bold);
                grph.DrawString("Grand Total", font, XBrushes.Black, new XRect(strX, newTotalPosition + 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                rowPos++;
                newTotalPosition = position + (rowPos + 1) * 15;

                grph.DrawRectangle(XPens.RoyalBlue, rectX, newTotalPosition, 100, 15);
                grph.DrawRectangle(XPens.RoyalBlue, rectX + 100, newTotalPosition, 155, 15);
                font = new XFont("Verdana", 8, XFontStyle.Bold);

                int tempNewPos = newTotalPosition + 5;

                grph.DrawString(Math.Round(m_totalObtMarks, 2).ToString(), font, XBrushes.Black, new XRect(strX, tempNewPos - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(Math.Round(m_totalSubMarks, 2).ToString(), font, XBrushes.Black, new XRect(strX + 100, tempNewPos - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);


                int tempRextX = rectX + 100;
                int tempStrX = strX + 100;

                if (SysConfig.GetSystemParam(SysConfig.EC_PERCENTAGE_FLAG).ParamValue == "Yes")
                {
                    tempRextX += 60;
                    tempStrX += 60;
                    grph.DrawRectangle(XPens.RoyalBlue, tempRextX, newTotalPosition, 95, 15);
                    double percentage = (m_totalObtMarks * 100) / m_totalSubMarks;
                    tempNewPos = newTotalPosition + 5;
                    grph.DrawString(Math.Round(percentage, 2).ToString() + "%", font, XBrushes.Black, new XRect(tempStrX, tempNewPos - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                }

                if (SysConfig.GetSystemParam(SysConfig.EC_GRADE_FLAG).ParamValue == "Yes")
                {
                    tempRextX += 60;
                    tempStrX += 60;

                    if(tempRextX == rectX + 160 )
                        grph.DrawRectangle(XPens.RoyalBlue, tempRextX, newTotalPosition, 95, 15);
                    else
                        grph.DrawRectangle(XPens.RoyalBlue, tempRextX, newTotalPosition, 35, 15);

                    string totalGrade = SessionHelper.GetGrade(int.Parse(Math.Round(m_totalObtMarks, 0).ToString()), int.Parse(Math.Round(m_totalSubMarks, 0).ToString()), 33);
                    tempNewPos = newTotalPosition + 5;
                    grph.DrawString(totalGrade, font, XBrushes.Black, new XRect(tempStrX, tempNewPos - 2, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                drawX = drawX == 0 ? 270 : 0;
                if (drawX == 0)
                {
                    drawY = drawY == 0 ? newTotalPosition - rectY + 45 : 0;
                }
                if (displayCount >= 3)
                {
                    rectX = 40; rectY = 120;
                    strX = 48; strY = 123;
                }
                else
                {
                    rectX = 40; rectY = 220;
                    strX = 48; strY = 223;
                }
                if (newTotalPosition >= page.Height - 50)
                {
                    page.Height = page.Height + (newTotalPosition - (page.Height - 50));
                }
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
    }
}
