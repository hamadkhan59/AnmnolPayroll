using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
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
using PdfSharp.Drawing.Layout;
using Logger;
using System.Reflection;

namespace SMS.Modules.BuildPdf
{
    public class ClassGrandResultPdf : BasicPdf
    {
        private SC_WEBEntities2 db = SessionHelper.dbContext;
        static int BranchId = 0;

        PdfDocument pdfdoc;
        bool sessionResult = false;

        public PdfDocument CreatePdf(string className, string sectionName, IList<string> subjList, List<DataSet> examResultList, int branchId, DateTime IssuedDate)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                sessionResult = true;
                BranchId = branchId;
                pdfdoc = new PdfDocument();

                string examName = "";

                for (int i = 0; i < examResultList.Count; i++)
                {
                    DataSet ds = examResultList[i];
                    for (int j = 0; j < ds.Tables.Count; j++)
                    {
                        DataTable dt = ds.Tables[j];
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

                        while (!dt.Columns[0].ColumnName.Equals("RollNo"))
                        {
                            dt.Columns.Remove(dt.Columns[0].ColumnName);
                        }
                        CreatePdf(className, sectionName, examName, subjList, dt, IssuedDate);

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

        public void CreatePdf(string className, string secName, string examName, IList<string> subjList, DataTable dt, DateTime IssuedDate)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                if (examName.ToLower().Equals("session"))
                    examName = "Complete " + examName + " Result";
                if (!sessionResult)
                    pdfdoc = new PdfDocument();
                pdfdoc.Info.Title = "First Pdf";
                PdfPage page = pdfdoc.AddPage();

                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 28, 130, 552, 30);
                grph.DrawRectangle(XPens.RoyalBlue, 25, 127, 558, 36);
                if (examName.Equals(""))
                    examName = "Complete Session Result";
                grph.DrawString(examName, font, XBrushes.White, new XRect(0, 130, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                font = new XFont("Verdana", 8, XFontStyle.Regular);
                grph.DrawRectangle(XPens.RoyalBlue, 25, 175, 105, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 130, 175, 180, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 25, 190, 105, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 130, 190, 180, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 310, 190, 100, 15);
                grph.DrawRectangle(XPens.RoyalBlue, 410, 190, 173, 15);


                grph.DrawString("Date", font, XBrushes.RoyalBlue, new XRect(35, 177, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                string currDate = IssuedDate.Day.ToString().PadLeft(2, '0') + "-" + IssuedDate.Month.ToString().PadLeft(2, '0') + "-" + IssuedDate.Year.ToString();
                grph.DrawString(currDate, font, XBrushes.Black, new XRect(140, 177, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                grph.DrawString("Class", font, XBrushes.RoyalBlue, new XRect(35, 192, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(className.ToUpper(), font, XBrushes.Black, new XRect(140, 192, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 77, 190, 355, 190);

                grph.DrawString("Section", font, XBrushes.RoyalBlue, new XRect(320, 192, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(secName.ToUpper(), font, XBrushes.Black, new XRect(420, 192, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawLine(XPens.Black, 397, 190, 550, 190);
                Populate_Result(page, pdfdoc, dt, grph, subjList);

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

        private void Populate_Result(PdfPage page, PdfDocument doc, DataTable dt, XGraphics grph, IList<string> subjList)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                grph.DrawRectangle(XPens.RoyalBlue, 25, 220, 35, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 60, 220, 110, 20);
                int count = subjList.Count + 4;

                if (SysConfig.GetSystemParam(SysConfig.EC_GRADE_FLAG).ParamValue == "No")
                {
                    dt.Columns.Remove("Grade");
                    count--;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_POSITION_FLAG).ParamValue == "No")
                {
                    dt.Columns.Remove("Position");
                    count--;
                }

                if (SysConfig.GetSystemParam(SysConfig.EC_PERCENTAGE_FLAG).ParamValue == "No")
                {
                    dt.Columns.Remove("Percentage");
                    count--;
                }

                int length = 430 / count;
                for (int i = 0; i < count; i++)
                {
                    if (i == count - 1)
                        grph.DrawRectangle(XPens.RoyalBlue, 170 + (i * length), 220, length - 10, 20);
                    else
                        grph.DrawRectangle(XPens.RoyalBlue, 170 + (i * length), 220, length, 20);
                }

                grph.DrawRectangle(XPens.RoyalBlue, 27, 222, 31, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 62, 222, 106, 16);
                for (int i = 0; i < count; i++)
                {
                    if (i == count - 1)
                        grph.DrawRectangle(XPens.RoyalBlue, 172 + (i * length), 222, length - 10 - 4, 16);
                    else
                        grph.DrawRectangle(XPens.RoyalBlue, 172 + (i * length), 222, length - 4, 16);
                }

                XFont font = new XFont("Verdana", 6, XFontStyle.Bold);

                grph.DrawString("Roll No", font, XBrushes.Black, new XRect(30, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Name", font, XBrushes.Black, new XRect(70, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                string[] subj = new string[count - 2];

                int j = 0;
                foreach (string name in subjList)
                {
                    var name1 = String.Join(Environment.NewLine,
                    name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    XTextFormatter tf = new XTextFormatter(grph);

                    if (name.Length > 6)
                        tf.DrawString(name, font, XBrushes.Black, new XRect(175 + (j * length), 222, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    else
                        tf.DrawString(name, font, XBrushes.Black, new XRect(180 + (j * length), 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    j++;
                }

                grph.DrawString("Total", font, XBrushes.Black, new XRect(178 + (j * length), 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                int temp = 0;
                if (SysConfig.GetSystemParam(SysConfig.EC_GRADE_FLAG).ParamValue == "Yes")
                {
                    temp++;
                    grph.DrawString("Grade", font, XBrushes.Black, new XRect(180 + ((j + temp) * length) - 5, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                if (SysConfig.GetSystemParam(SysConfig.EC_PERCENTAGE_FLAG).ParamValue == "Yes")
                {
                    temp++;
                    grph.DrawString("Percent", font, XBrushes.Black, new XRect(180 + ((j + temp) * length) - 5, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                if (SysConfig.GetSystemParam(SysConfig.EC_POSITION_FLAG).ParamValue == "Yes")
                {
                    temp++;
                    grph.DrawString("Pos", font, XBrushes.Black, new XRect(180 + ((j + temp) * length) - 5, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                font = new XFont("Verdana", 6, XFontStyle.Regular);

                int rowPos = 0;
                int position = 220;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i >= dt.Rows.Count - 5)
                        font = new XFont("Verdana", 6, XFontStyle.Bold);

                    int newPosition = position + (rowPos + 1) * 20;
                    if (newPosition >= page.Height - 100)
                    {
                        rowPos = 0;
                        position = 100;
                        page = doc.AddPage();
                        grph = XGraphics.FromPdfPage(page);
                        newPosition = position + (rowPos + 1) * 20;
                        base.DesignBorder(grph, page, (int)page.Height);
                        base.DesignSchoolHeader(grph, page, BranchId);
                    }

                    grph.DrawRectangle(XPens.RoyalBlue, 25, newPosition, 35, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 60, newPosition, 110, 20);
                    for (int jk = 0; jk < count; jk++)
                    {
                        if (jk == count - 1)
                            grph.DrawRectangle(XPens.RoyalBlue, 170 + (jk * length), newPosition, length - 10, 20);
                        else
                            grph.DrawRectangle(XPens.RoyalBlue, 170 + (jk * length), newPosition, length, 20);
                    }

                    newPosition += 5;
                    DataRow row = dt.Rows[i];
                    grph.DrawString(row[0].ToString(), font, XBrushes.Black, new XRect(30, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(row[1].ToString(), font, XBrushes.Black, new XRect(70, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    for (int kl = 0; kl < count; kl++)
                    {
                        grph.DrawString(row[kl + 2].ToString(), font, XBrushes.Black, new XRect(175 + (kl * length), newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    }
                    rowPos++;
                }

                rowPos += 5;
                grph.DrawString("Prepared By ___________________________________________", font, XBrushes.Black, new XRect(50, position + (rowPos + 2) * 20, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }




    }
}
