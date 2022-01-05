using System;
using System.Collections.Generic;
using System.Collections;
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
using Logger;
using System.Reflection;

namespace SMS.Modules.BuildPdf.StaffSheets
{
    public class StaffCertificate : BasicPdf
    {
        static int BranchId = 0;
        public PdfDocument CreatePdf(IList name, IList fatherName, IList designantion, IList joinDate, IList leavingDate, IList staffId, int branchId)
        {
            PdfDocument pdfdoc = new PdfDocument();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                BranchId = branchId;
                pdfdoc.Info.Title = "First Pdf";

                for (int i = 0; i < name.Count; i++)
                {
                    CreatePdf(name[i].ToString(), fatherName[i].ToString(), designantion[i].ToString(), DateTime.Parse(joinDate[i].ToString()), DateTime.Parse(leavingDate[i].ToString()), Convert.ToInt32(staffId[i].ToString()), pdfdoc);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            //string pdfFilename = @"ExperienceCertificate.pdf";
            //if (pdfdoc.PageCount > 0)
            //    pdfdoc.Save(pdfFilename);
            return pdfdoc;
        }

        private void CreatePdf(string name, string fatherName, string designantion, DateTime joinDate, DateTime leavingDate, int staffId, PdfDocument pdfdoc)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                PdfPage page = pdfdoc.AddPage();
                page.Height = 560;
                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 150, 130, 310, 30);
                grph.DrawRectangle(XPens.RoyalBlue, 147, 127, 316, 36);
                grph.DrawString("Experience Certificate", font, XBrushes.White, new XRect(0, 130, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Ref", font, XBrushes.RoyalBlue, new XRect(50, 220, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(staffId.ToString(), font, XBrushes.Black, new XRect(150, 215, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 90, 230, 255, 230);

                string date = DateTime.Now.DayOfWeek.ToString() + ", " + DateTime.Now.Day + " " + DateTime.Now.ToString("MMM") + ", " + DateTime.Now.Year;
                font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawString("Date", font, XBrushes.RoyalBlue, new XRect(320, 220, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(date, font, XBrushes.Black, new XRect(390, 215, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 360, 230, 555, 230);

                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString("It is herebycertify that Mr/Mrs,", font, XBrushes.RoyalBlue, new XRect(90, 270, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(name, font, XBrushes.Black, new XRect(275, 265, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 260, 280, 555, 280);

                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString("S/O, D/O", font, XBrushes.RoyalBlue, new XRect(50, 300, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(fatherName, font, XBrushes.Black, new XRect(130, 295, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 110, 310, 360, 310);

                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString(" has been serving in this institution", font, XBrushes.RoyalBlue, new XRect(370, 300, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString("Since", font, XBrushes.RoyalBlue, new XRect(50, 330, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                date = joinDate.DayOfWeek.ToString() + ", " + joinDate.Day + " " + joinDate.ToString("MMM") + ", " + joinDate.Year;
                grph.DrawString(date, font, XBrushes.Black, new XRect(130, 325, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 100, 340, 330, 340);

                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString("to", font, XBrushes.RoyalBlue, new XRect(340, 330, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                date = leavingDate.DayOfWeek.ToString() + ", " + leavingDate.Day + " " + leavingDate.ToString("MMM") + ", " + leavingDate.Year;
                grph.DrawString(date, font, XBrushes.Black, new XRect(370, 325, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 360, 340, 555, 340);

                font = new XFont("Verdana", 10, XFontStyle.Regular);

                string matan = "During his/her stay here, he/she has been observed a dadicated, devoted and desciplined teacher. ";
                grph.DrawString(matan, font, XBrushes.RoyalBlue, new XRect(50, 360, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                matan = "He/She has never failed to solve the problems faced by the students time to time. He/she often";
                grph.DrawString(matan, font, XBrushes.RoyalBlue, new XRect(50, 380, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                matan = "entertains the student with good moral lessons. On seeing his/here keen interest in educational";
                grph.DrawString(matan, font, XBrushes.RoyalBlue, new XRect(50, 400, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                matan = "activities, I am compelled to say that he/she adopted the profession willingly. I wish him/her";
                grph.DrawString(matan, font, XBrushes.RoyalBlue, new XRect(50, 420, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                matan = "to succed in the future life";
                grph.DrawString(matan, font, XBrushes.RoyalBlue, new XRect(50, 440, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                matan = "Principal Signature __________________________________";
                grph.DrawString(matan, font, XBrushes.Black, new XRect(250, 500, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                base.DesignBorder(grph, page, 550);
                base.DesignSchoolHeader(grph, page, BranchId);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            //Process.Start(pdfFilename);
        }


    }
}
