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
    public class StudentTranscriptResult : BasicPdf
    {
        string teacherRemarks;
        static int BranchId = 0;

        public PdfDocument CreatePdf(StudentModel student, string className, string secName, string remarks, DataSet resultDatatableList, PdfDocument pdfdoc, int branchId, List<ActivityMarksViewModel> activitiesList, DateTime IssuedDate, int Position)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                //PdfDocument pdfdoc = new PdfDocument();
                BranchId = branchId;
                teacherRemarks = remarks;
                int examNameCount = 0;

                DataSet ds = resultDatatableList;
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];
                    PdfPage page = pdfdoc.AddPage();
                    page.Height = 750;
                    XGraphics grph = XGraphics.FromPdfPage(page);
                    XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                    grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 28, 110, 537, 30);
                    grph.DrawRectangle(XPens.RoyalBlue, 25, 107, 543, 36);
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
                    grph.DrawRectangle(XPens.RoyalBlue, 460, 180, 100, 15);
                    grph.DrawRectangle(XPens.RoyalBlue, 460, 195, 100, 15);
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

                    //grph.DrawString("Exam", font, XBrushes.RoyalBlue, new XRect(385, 200, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    //font = new XFont("Verdana", 6, XFontStyle.Regular);

                    //string examName = "Student Term Result";



                    //while (!dt.Columns[0].ColumnName.Equals("SubjectName"))
                    //{
                    //    dt.Columns.Remove(dt.Columns[0].ColumnName);
                    //}

                    //grph.DrawString(examName.ToUpper(), font, XBrushes.Black, new XRect(427, 195, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    //grph.DrawString("Final Result", font, XBrushes.Black, new XRect(427, 195, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    //grph.DrawLine(XPens.Black, 414, 210, 550, 210);

                    Populate_Result(page, pdfdoc, dt, grph, activitiesList, Position);
                    base.DesignBorder(grph, page, (int)page.Height);
                    base.DesignSchoolHeader(grph, page, BranchId);
                    examNameCount++;
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

        public void CreatePdf(string rollno, string studName, string className, string secName, string examName, string remarks, DataTable dt, List<ActivityMarksViewModel> activitiesList, int Position)
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

                Populate_Result(page, pdfdoc, dt, grph, activitiesList, Position);

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

        private List<string> GetExamNames(DataTable dt)
        {
            string examName = dt.Rows[1][1].ToString();
            List<string> examNames = new List<string>();
            examNames.Add(examName);
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (!dt.Rows[i][1].ToString().Equals(examName))
                {
                    examName = dt.Rows[i][1].ToString();
                    if (!examName.Equals("Summer Vacation Work"))
                        examNames.Add(examName);
                }
            }

            return examNames;
        }

        private int GetSubjectCount(DataTable dt)
        {
            string examName = dt.Rows[1][1].ToString();
            List<string> examNames = new List<string>();
            examNames.Add(examName);
            int count = 1;
            for (count = 1; count < dt.Rows.Count; count++)
            {
                if (!dt.Rows[count][1].ToString().Equals(examName))
                {
                    break;
                }

            }

            return count;
        }

        private void Populate_Result(PdfPage page, PdfDocument doc, DataTable dt, XGraphics grph, List<ActivityMarksViewModel> activitiesList, int Position)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                if (dt.Columns.Contains("TotalMarks"))
                    dt.Columns.Remove("TotalMarks");
                string examName = dt.Rows[0][1].ToString();
                List<string> examList = GetExamNames(dt);
                int examCount = examList.Count;
                int subjectCount = GetSubjectCount(dt);
                grph.DrawRectangle(XPens.RoyalBlue, 50, 220, 150, 20);
                int cellWidth = 260 / examList.Count;
                int ik = 0;
                for (ik = 0; ik < examList.Count; ik++)
                {
                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth), 220, cellWidth, 20);
                }
                grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth), 220, 50, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth) + 50, 220, 50, 20);


                grph.DrawRectangle(XPens.RoyalBlue, 52, 222, 146, 16);
                for (ik = 0; ik < examList.Count; ik++)
                {
                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth) + 2, 222, cellWidth - 4, 16);
                }
                grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth) + 2, 220 + 2, 46, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth) + 50 + 2, 220 + 2, 46, 16);

                XFont font = new XFont("Verdana", 9, XFontStyle.Bold);


                grph.DrawString("Subject Name", font, XBrushes.Black, new XRect(60, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                for (ik = 0; ik < examList.Count; ik++)
                {
                    if (examList[ik].Length > 20)
                        font = new XFont("Verdana", 7, XFontStyle.Bold);
                    else
                        font = new XFont("Verdana", 9, XFontStyle.Bold);

                    grph.DrawString(examList[ik].ToString(), font, XBrushes.Black, new XRect(200 + (ik * cellWidth) + 10, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                font = new XFont("Verdana", 9, XFontStyle.Bold);

                grph.DrawString("Total", font, XBrushes.Black, new XRect(200 + (ik * cellWidth) + 10, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 8, XFontStyle.Bold);
                grph.DrawString("Obtained", font, XBrushes.Black, new XRect(200 + (ik * cellWidth) + 55, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                font = new XFont("Verdana", 9, XFontStyle.Regular);
                int tblIndex = 0;
                decimal[] obtained = new decimal[subjectCount];
                int[] Total = new int[subjectCount];
                for (int j = 0; j < subjectCount; j++)
                {
                    obtained[j] = Total[j] = 0;
                }

                int svTotal = int.Parse(dt.Rows[tblIndex][2].ToString());
                decimal svObtained = decimal.Parse(dt.Rows[tblIndex][7].ToString());
                for (ik = 0; ik < examList.Count; ik++)
                {
                    if (ik == 1)
                    {
                        if (examList[ik].Equals("Summer Vacation Work"))
                        {
                            string marks = dt.Rows[tblIndex][2].ToString();
                            string obtainedM = dt.Rows[tblIndex][7].ToString();
                            svTotal = int.Parse(marks);
                            svObtained = decimal.Parse(obtainedM);

                            for (int p = 0; p < subjectCount + 1; p++)
                            {

                                if (p == subjectCount)
                                {
                                    grph.DrawRectangle(XPens.RoyalBlue, 50, 240 + (p * 20), 150, 20);
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth), 240 + (p * 20), cellWidth / 2, 20);
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth) + cellWidth / 2, 240 + (p * 20), cellWidth / 2, 20);

                                    XFont font1 = new XFont("Verdana", 9, XFontStyle.Bold);
                                    grph.DrawString("Total", font1, XBrushes.Blue, new XRect(60, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    grph.DrawString(marks, font1, XBrushes.Blue, new XRect(200 + (ik * cellWidth) + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    grph.DrawString(obtainedM, font1, XBrushes.Blue, new XRect(200 + (ik * cellWidth) + cellWidth / 2 + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                                }
                                else
                                {

                                    grph.DrawRectangle(XPens.RoyalBlue, 50, 240 + (p * 20), 150, 20);
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth), 240 + (p * 20), cellWidth / 2, 20);
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth) + cellWidth / 2, 240 + (p * 20), cellWidth / 2, 20);
                                    grph.DrawString(dt.Rows[tblIndex][7].ToString(), font, XBrushes.Black, new XRect(60, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    grph.DrawString("", font, XBrushes.Black, new XRect(200 + (ik * cellWidth) + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    grph.DrawString("", font, XBrushes.Black, new XRect(200 + (ik * cellWidth) + cellWidth / 2 + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                                    //obt += int.Parse(dt.Rows[tblIndex][7].ToString());
                                    //total += int.Parse(dt.Rows[tblIndex][2].ToString());

                                    //Total[p] = Total[p] + int.Parse(dt.Rows[tblIndex][2].ToString());
                                    //obtained[p] = obtained[p] + int.Parse(dt.Rows[tblIndex][7].ToString());
                                    tblIndex++;
                                }
                            }
                        }
                        else
                        {
                            decimal obt = 0; int total = 0;
                            for (int p = 0; p < subjectCount + 1; p++)
                            {

                                if (p == subjectCount)
                                {
                                    grph.DrawRectangle(XPens.RoyalBlue, 50, 240 + (p * 20), 150, 20);
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth), 240 + (p * 20), cellWidth / 2, 20);
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth) + cellWidth / 2, 240 + (p * 20), cellWidth / 2, 20);

                                    XFont font1 = new XFont("Verdana", 9, XFontStyle.Bold);
                                    grph.DrawString("Total", font1, XBrushes.Blue, new XRect(60, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    grph.DrawString(total.ToString(), font1, XBrushes.Blue, new XRect(200 + (ik * cellWidth) + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    grph.DrawString(obt.ToString(), font1, XBrushes.Blue, new XRect(200 + (ik * cellWidth) + cellWidth / 2 + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                                }
                                else
                                {

                                    grph.DrawRectangle(XPens.RoyalBlue, 50, 240 + (p * 20), 150, 20);
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth), 240 + (p * 20), cellWidth / 2, 20);
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth) + cellWidth / 2, 240 + (p * 20), cellWidth / 2, 20);
                                    grph.DrawString(dt.Rows[tblIndex][6].ToString(), font, XBrushes.Black, new XRect(60, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    grph.DrawString(dt.Rows[tblIndex][2].ToString(), font, XBrushes.Black, new XRect(200 + (ik * cellWidth) + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    grph.DrawString(dt.Rows[tblIndex][7].ToString(), font, XBrushes.Black, new XRect(200 + (ik * cellWidth) + cellWidth / 2 + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                                    obt += decimal.Parse(dt.Rows[tblIndex][7].ToString());
                                    total += int.Parse(dt.Rows[tblIndex][2].ToString());

                                    Total[p] = Total[p] + int.Parse(dt.Rows[tblIndex][2].ToString());
                                    obtained[p] = obtained[p] + decimal.Parse(dt.Rows[tblIndex][7].ToString());
                                    tblIndex++;
                                }
                            }
                        }
                    }
                    else
                    {
                        decimal obt = 0; int total = 0;
                        if (examList[ik].Equals("Summer Vacation Work"))
                        {
                            string marks = dt.Rows[tblIndex][2].ToString();
                            string obtainedM = dt.Rows[tblIndex][7].ToString();
                            svTotal = int.Parse(marks);
                            svObtained = decimal.Parse(obtainedM);
                            for (int p = 0; p < subjectCount + 1; p++)
                            {

                                if (p == subjectCount)
                                {
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth), 240 + (p * 20), cellWidth / 2, 20);
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth) + cellWidth / 2, 240 + (p * 20), cellWidth / 2, 20);

                                    XFont font1 = new XFont("Verdana", 9, XFontStyle.Bold);
                                    grph.DrawString(marks.ToString(), font1, XBrushes.Blue, new XRect(200 + (ik * cellWidth) + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    grph.DrawString(obtainedM.ToString(), font1, XBrushes.Blue, new XRect(200 + (ik * cellWidth) + cellWidth / 2 + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                                }
                                else
                                {
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth), 240 + (p * 20), cellWidth / 2, 20);
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth) + cellWidth / 2, 240 + (p * 20), cellWidth / 2, 20);
                                    grph.DrawString("", font, XBrushes.Black, new XRect(200 + (ik * cellWidth) + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    grph.DrawString("", font, XBrushes.Black, new XRect(200 + (ik * cellWidth) + cellWidth / 2 + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                                    //grph.DrawString(marks, font, XBrushes.Black, new XRect(220 + (ik * cellWidth) + 10, 240 + (p * 20), page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    //obt += int.Parse(dt.Rows[tblIndex][7].ToString());
                                    //total += int.Parse(dt.Rows[tblIndex][2].ToString());

                                    //Total[p] = Total[p] + int.Parse(dt.Rows[tblIndex][2].ToString());
                                    //obtained[p] = obtained[p] + int.Parse(dt.Rows[tblIndex][7].ToString());
                                    tblIndex++;
                                }
                            }
                        }
                        else
                        {
                            for (int p = 0; p < subjectCount + 1; p++)
                            {
                                if (p == subjectCount)
                                {
                                    grph.DrawRectangle(XPens.RoyalBlue, 50, 240 + (p * 20), 150, 20);
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth), 240 + (p * 20), cellWidth / 2, 20);
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth) + cellWidth / 2, 240 + (p * 20), cellWidth / 2, 20);

                                    XFont font1 = new XFont("Verdana", 9, XFontStyle.Bold);
                                    grph.DrawString("Total", font1, XBrushes.Blue, new XRect(60, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    grph.DrawString(total.ToString(), font1, XBrushes.Blue, new XRect(200 + (ik * cellWidth) + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    grph.DrawString(obt.ToString(), font1, XBrushes.Blue, new XRect(200 + (ik * cellWidth) + cellWidth / 2 + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                                }
                                else
                                {
                                    grph.DrawRectangle(XPens.RoyalBlue, 50, 240 + (p * 20), 150, 20);
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth), 240 + (p * 20), cellWidth / 2, 20);
                                    grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth) + cellWidth / 2, 240 + (p * 20), cellWidth / 2, 20);
                                    grph.DrawString(dt.Rows[tblIndex][6].ToString(), font, XBrushes.Black, new XRect(60, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    grph.DrawString(dt.Rows[tblIndex][2].ToString(), font, XBrushes.Black, new XRect(200 + (ik * cellWidth) + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                                    grph.DrawString(dt.Rows[tblIndex][7].ToString(), font, XBrushes.Black, new XRect(200 + (ik * cellWidth) + cellWidth / 2 + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                                    //grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth), 240 + (p * 20), cellWidth, 20);
                                    //grph.DrawString(dt.Rows[tblIndex][7].ToString(), font, XBrushes.Black, new XRect(220 + (ik * cellWidth) + 10, 240 + (p * 20), page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                                    obt += decimal.Parse(dt.Rows[tblIndex][7].ToString());
                                    total += int.Parse(dt.Rows[tblIndex][2].ToString());

                                    Total[p] = Total[p] + int.Parse(dt.Rows[tblIndex][2].ToString());
                                    obtained[p] = obtained[p] + decimal.Parse(dt.Rows[tblIndex][7].ToString());
                                    tblIndex++;
                                }
                            }
                        }
                    }
                }

                decimal sum1 = 0, sum2 = 0;
                int pk = 0;

                for (int p = 0; p < subjectCount + 1; p++)
                {
                    if (p == subjectCount)
                    {
                        XFont font1 = new XFont("Verdana", 9, XFontStyle.Bold);
                        grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth) + 50, 240 + (p * 20), 50, 20);
                        grph.DrawString(sum1.ToString(), font1, XBrushes.Black, new XRect(200 + (ik * cellWidth) + 60, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    }
                    else
                    {
                        grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth) + 50, 240 + (p * 20), 50, 20);
                        grph.DrawString(obtained[p].ToString(), font, XBrushes.Black, new XRect(200 + (ik * cellWidth) + 60, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        sum1 += obtained[p];
                        pk++;
                    }
                }

                for (int p = 0; p < subjectCount + 1; p++)
                {
                    if (p == subjectCount)
                    {
                        XFont font1 = new XFont("Verdana", 9, XFontStyle.Bold);
                        grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth), 240 + (p * 20), 50, 20);
                        grph.DrawString(sum2.ToString(), font1, XBrushes.Blue, new XRect(205 + (ik * cellWidth) + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    }
                    else
                    {
                        XFont font1 = new XFont("Verdana", 9, XFontStyle.Bold);
                        grph.DrawRectangle(XPens.RoyalBlue, 200 + (ik * cellWidth), 240 + (p * 20), 50, 20);
                        grph.DrawString(Total[p].ToString(), font1, XBrushes.Blue, new XRect(205 + (ik * cellWidth) + 10, 240 + (p * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        sum2 += Total[p];
                    }
                }

                pk = pk + 2;
                teacherRemarks = SessionHelper.GetRemarks(sum1, sum2, 33);
                string grade = SessionHelper.GetGrade(sum1, sum2, 33);
                decimal percentage = (sum1 * 100) / sum2;

                XFont font2 = new XFont("Verdana", 10, XFontStyle.Bold);
                int gt = pk;
                int activityTotal = 0, activityObt = 0;
                if (activitiesList != null && activitiesList.Count > 0)
                {
                    grph.DrawRectangle(XPens.RoyalBlue, 50, 240 + (pk * 20), 280, 20);
                    grph.DrawString("Activities", font2, XBrushes.Blue, new XRect(60, 240 + (pk * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    foreach (var actitiy in activitiesList)
                    {
                        pk++;
                        grph.DrawRectangle(XPens.RoyalBlue, 50, 240 + (pk * 20), 140, 20);
                        grph.DrawRectangle(XPens.RoyalBlue, 190, 240 + (pk * 20), 70, 20);
                        grph.DrawRectangle(XPens.RoyalBlue, 260, 240 + (pk * 20), 70, 20);
                        grph.DrawString(actitiy.ActitivtyName, font, XBrushes.Black, new XRect(60, 240 + (pk * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString(actitiy.TotalMarks.ToString(), font, XBrushes.Black, new XRect(200, 240 + (pk * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        grph.DrawString(actitiy.ObtMarks.ToString(), font, XBrushes.Black, new XRect(270, 240 + (pk * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        activityTotal += (int)actitiy.TotalMarks;
                        activityObt += (int)actitiy.ObtMarks;
                    }
                }

                grph.DrawRectangle(XPens.RoyalBlue, 330, 240 + (gt * 20), 230, 20);
                grph.DrawString("Grand Total", font2, XBrushes.Blue, new XRect(340, 240 + (gt * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                gt++;

                if (pk > gt)
                {
                    grph.DrawRectangle(XPens.RoyalBlue, 330, 240 + (gt * 20), 120, ((pk - gt) + 1) * 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 450, 240 + (gt * 20), 110, ((pk - gt) + 1) * 20);
                }
                else
                {
                    grph.DrawRectangle(XPens.RoyalBlue, 330, 240 + (gt * 20), 120, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 450, 240 + (gt * 20), 110, 20);
                }

                int additionalHeight = ((pk - gt) + 1) / 2;
                additionalHeight += gt;
                grph.DrawString((activityTotal + sum2).ToString(), font2, XBrushes.Black, new XRect(340, 240 + (additionalHeight * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString((activityObt + sum1).ToString(), font2, XBrushes.Black, new XRect(455, 240 + (additionalHeight * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                decimal totalObtMarks = activityTotal + sum2;
                decimal totalSubMarks = activityObt + sum1;

                //grph.DrawString(svTotal.ToString(), font, XBrushes.Black, new XRect(360, 240 + (pk * 20), page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString(svObtained.ToString(), font, XBrushes.Black, new XRect(460, 240 + (pk * 20), page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //pk++;

                //grph.DrawRectangle(XPens.RoyalBlue, 50, 240 + (pk * 20), 300, 20);
                //grph.DrawRectangle(XPens.RoyalBlue, 350, 240 + (pk * 20), 100, 20);
                //grph.DrawRectangle(XPens.RoyalBlue, 450, 240 + (pk * 20), 100, 20);
                ////grph.DrawRectangle(XPens.RoyalBlue, 450, 240 + (pk * 20), 100, 20);
                //grph.DrawString("Grand Total", font2, XBrushes.Blue, new XRect(60, 240 + (pk * 20), page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString((svTotal + sum2).ToString(), font, XBrushes.Black, new XRect(360, 240 + (pk * 20), page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString((svObtained + sum1).ToString(), font, XBrushes.Black, new XRect(460, 240 + (pk * 20), page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                pk = pk + 3;

                font = new XFont("Verdana", 10, XFontStyle.Regular);
                font2 = new XFont("Verdana", 10, XFontStyle.Bold);


                if (SysConfig.GetSystemParam(SysConfig.EC_REMARKS_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, 50, 240 + (pk * 20), 125, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 175, 240 + (pk * 20), 385, 20);

                    grph.DrawString("Teacher Remarks", font2, XBrushes.Blue, new XRect(60, 240 + (pk * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(teacherRemarks, font, XBrushes.Black, new XRect(185, 240 + (pk * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    pk++;
                }


                int tempPos = 50;
                if (SysConfig.GetSystemParam(SysConfig.EC_GRADE_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (pk * 20), 100, 20);
                    grph.DrawString("Grade", font2, XBrushes.Blue, new XRect(tempPos + 10, 240 + (pk * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 100;
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (pk * 20), 560 - tempPos, 20);
                    grph.DrawString(grade, font, XBrushes.Black, new XRect(tempPos + 10, 240 + (pk * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 70;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_PERCENTAGE_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (pk * 20), 100, 20);
                    grph.DrawString("Percentage", font2, XBrushes.Blue, new XRect(tempPos + 10, 240 + (pk * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 100;
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (pk * 20), 560 - tempPos, 20);
                    grph.DrawString(Math.Round(((decimal)totalObtMarks / (decimal)totalSubMarks) * 100m, 2).ToString() + " %", font, XBrushes.Black, new XRect(tempPos + 10, 240 + (pk * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 70;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_POSITION_FLAG).ParamValue == "Yes")
                {
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (pk * 20), 100, 20);
                    grph.DrawString("Position", font2, XBrushes.Blue, new XRect(tempPos + 10, 240 + (pk * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 100;
                    grph.DrawRectangle(XPens.RoyalBlue, tempPos, 240 + (pk * 20), 560 - tempPos, 20);
                    grph.DrawString(Position + "", font, XBrushes.Black, new XRect(tempPos + 10, 240 + (pk * 20) + 3, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                
                pk = pk + 3;
                font = new XFont("Verdana", 7, XFontStyle.Regular);

                tempPos = 50;
                if (SysConfig.GetSystemParam(SysConfig.EC_PRICIPLE_SIGN_FLAG).ParamValue == "Yes")
                {
                    base.DrawPrincipalSignature(grph, page, 60, 240 + (pk + 1) * 20 + 15, BranchId);
                    grph.DrawLine(XPens.Black, tempPos, 240 + (pk + 4) * 20 + 15, tempPos + 150, 240 + (pk + 4) * 20 + 15);
                    grph.DrawString("Principal's Signature", font, XBrushes.RoyalBlue, new XRect(tempPos, 240 + (pk + 5) * 20, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 150;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_TEACHER_SIGN_FLAG).ParamValue == "Yes")
                {
                    if (tempPos > 50)
                        tempPos += 30;
                    grph.DrawLine(XPens.Black, tempPos, 240 + (pk + 4) * 20 + 15, tempPos + 150, 240 + (pk + 4) * 20 + 15);
                    grph.DrawString("Teacher's Signature", font, XBrushes.RoyalBlue, new XRect(tempPos, 240 + (pk + 5) * 20, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    tempPos += 150;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_PARENT_SIGN_FLAG).ParamValue == "Yes")
                {
                    if (tempPos > 50)
                        tempPos += 30;
                    grph.DrawLine(XPens.Black, tempPos, 240 + (pk + 4) * 20 + 15, tempPos + 140, 240 + (pk + 4) * 20 + 15);
                    grph.DrawString("Parent's Signature", font, XBrushes.RoyalBlue, new XRect(tempPos, 240 + (pk + 5) * 20, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                pk += 1;
                if (SysConfig.GetSystemParam(SysConfig.EC_PASSING_PER_NOTE_FLAG).ParamValue == "Yes")
                {
                    grph.DrawString(SysConfig.GetSystemParam(SysConfig.EC_PASSING_PER_NOTE).ParamValue, font, XBrushes.Black, new XRect(50, 240 + (pk + 5) * 20 + 15, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private string GetRemarks(int obtained, int totalMarks, int passPercentage)
        {
            int obtPercentage = (obtained * 100) / totalMarks;
            string grade = null;
            if (obtPercentage >= 85)
                grade = "Excellent!";
            else if (obtPercentage >= 70)
                grade = "Very Good!";
            else if (obtPercentage >= 55)
                grade = "Good";
            else
                grade = "Satisfactory!";

            return grade;
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
