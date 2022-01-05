using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SMS_DAL;
using System.Drawing;
using System.ComponentModel;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using Logger;
using System.Reflection;

namespace SMS_Web.Helpers.PdfHelper
{
    public class AttandanceSheetPdf
    {
        private ISecurityRepository secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
        static int BranchId = 0;
        public PdfDocument CreatePdf(string className, string secName, string subjName, string examName, string date, string[] rollNumber, string[] studentName, int branchId)
        {
            PdfDocument pdfdoc = new PdfDocument();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                pdfdoc.Info.Title = "First Pdf";
                PdfPage page = pdfdoc.AddPage();
                BranchId = branchId;
                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, XBrushes.RoyalBlue, 150, 130, 310, 30);
                grph.DrawRectangle(XPens.RoyalBlue, 147, 127, 316, 36);
                grph.DrawString("Exam Attendance Sheet", font, XBrushes.White, new XRect(0, 130, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                font = new XFont("Verdana", 10, XFontStyle.Regular);
                grph.DrawString("Subject", font, XBrushes.RoyalBlue, new XRect(50, 180, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(subjName.ToUpper(), font, XBrushes.Black, new XRect(110, 175, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 90, 190, 295, 190);

                grph.DrawString("Exam", font, XBrushes.RoyalBlue, new XRect(300, 180, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(examName.ToUpper(), font, XBrushes.Black, new XRect(348, 175, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 328, 190, 550, 190);

                grph.DrawString("Class", font, XBrushes.RoyalBlue, new XRect(50, 200, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(className.ToUpper(), font, XBrushes.Black, new XRect(97, 195, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 77, 210, 205, 210);

                grph.DrawString("Section", font, XBrushes.RoyalBlue, new XRect(210, 200, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(secName.ToUpper(), font, XBrushes.Black, new XRect(287, 195, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 247, 210, 350, 210);

                grph.DrawString("Date", font, XBrushes.RoyalBlue, new XRect(355, 200, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString(date.ToUpper(), font, XBrushes.Black, new XRect(387, 195, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawLine(XPens.Black, 380, 210, 550, 210);

                Populate_Result(page, pdfdoc, rollNumber, studentName, grph);

                DesignBorder(grph, page, (int)page.Height);
                DesignSchoolHeader(grph, page);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return pdfdoc;
        }

        private void DesignBorder(XGraphics grph, PdfPage page, int height)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                for (int i = 10; i < 15; i++)
                {
                    grph.DrawRectangle(XPens.RoyalBlue, i, i, page.Width - i - i, height - i - i);
                }


                grph.DrawRectangle(XPens.RoyalBlue, 6, 6, 15, 15);
                grph.DrawLine(XPens.RoyalBlue, 21, 21, 21, 25);
                grph.DrawLine(XPens.RoyalBlue, 21, 21, 25, 21);
                grph.DrawLine(XPens.RoyalBlue, 21, 25, 6, 25);
                grph.DrawLine(XPens.RoyalBlue, 25, 21, 25, 6);

                grph.DrawRectangle(XPens.RoyalBlue, page.Width - 20, height - 20, 15, 15);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 20, height - 20, page.Width - 20, height - 24);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 20, height - 20, page.Width - 24, height - 20);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 20, height - 24, page.Width - 5, height - 24);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 24, height - 20, page.Width - 24, height - 5);

                grph.DrawRectangle(XPens.RoyalBlue, page.Width - 20, 6, 15, 15);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 20, 21, page.Width - 24, 21);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 20, 21, page.Width - 20, 25);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 24, 21, page.Width - 24, 6);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 20, 25, page.Width - 5, 25);

                grph.DrawRectangle(XPens.RoyalBlue, 6, height - 20, 15, 15);
                grph.DrawLine(XPens.RoyalBlue, 21, height - 20, 25, height - 20);
                grph.DrawLine(XPens.RoyalBlue, 21, height - 20, 21, height - 25);
                grph.DrawLine(XPens.RoyalBlue, 25, height - 20, 25, height - 5);
                grph.DrawLine(XPens.RoyalBlue, 21, height - 25, 6, height - 25);

                grph.DrawLine(XPens.RoyalBlue, 6, 25, 6, height - 25);
                grph.DrawLine(XPens.RoyalBlue, 25, 6, page.Width - 24, 6);
                grph.DrawLine(XPens.RoyalBlue, page.Width - 5, 25, page.Width - 5, height - 24);
                grph.DrawLine(XPens.RoyalBlue, 25, height - 5, page.Width - 24, height - 5);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private void DesignSchoolHeader(XGraphics grph, PdfPage page)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                SchoolConfig config = secRepo.GetSchoolConfigByBranchId(BranchId);

                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                grph.DrawImage((Bitmap)tc.ConvertFrom((byte[])config.SchoolLogo), 25, 25);

                XFont font = new XFont("Verdana", 25, XFontStyle.Bold);
                if (config.SchoolName.Length > 30)
                    font = new XFont("Verdana", 20, XFontStyle.Bold);
                else if (config.SchoolName.Length > 40)
                    font = new XFont("Verdana", 18, XFontStyle.Bold);
                else if (config.SchoolName.Length > 50)
                    font = new XFont("Verdana", 15, XFontStyle.Bold);
                grph.DrawString(config.SchoolName, font, XBrushes.Black, new XRect(0, 30, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                font = new XFont("Verdana", 10, XFontStyle.Bold);
                if (config.SchoolName.Length > 55)
                    font = new XFont("Verdana", 9, XFontStyle.Bold);
                grph.DrawString(config.CampusName, font, XBrushes.Black, new XRect(0, 70, page.Width.Point, page.Height.Point), XStringFormats.TopCenter);

                for (int i = 0; i < 5; i++)
                {
                    grph.DrawLine(XPens.RoyalBlue, 10, 90 + i, page.Width - 10, 90 + i);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }

        private void Populate_Result(PdfPage page, PdfDocument doc, string [] rollnumber, string [] studentName, XGraphics grph)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                grph.DrawRectangle(XPens.RoyalBlue, 50, 220, 100, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 150, 220, 200, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 350, 220, 100, 20);
                grph.DrawRectangle(XPens.RoyalBlue, 450, 220, 100, 20);

                grph.DrawRectangle(XPens.RoyalBlue, 52, 222, 96, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 152, 222, 196, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 352, 222, 96, 16);
                grph.DrawRectangle(XPens.RoyalBlue, 452, 222, 96, 16);

                XFont font = new XFont("Verdana", 10, XFontStyle.Regular);

                grph.DrawString("Roll No", font, XBrushes.Black, new XRect(70, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Name", font, XBrushes.Black, new XRect(170, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Signature", font, XBrushes.Black, new XRect(370, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                grph.DrawString("Sheet No", font, XBrushes.Black, new XRect(470, 225, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                int rowPos = 0;
                int position = 220;
                for (int i = 0; i < studentName.Count(); i++)
                {
                    int newPosition = position + (rowPos + 1) * 20;

                    if (newPosition >= page.Height - 100)
                    {
                        rowPos = 0;
                        position = 100;
                        page = doc.AddPage();
                        grph = XGraphics.FromPdfPage(page);
                        newPosition = position + (rowPos + 1) * 20;
                    }
                    grph.DrawRectangle(XPens.RoyalBlue, 50, newPosition, 100, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 150, newPosition, 200, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 350, newPosition, 100, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 450, newPosition, 100, 20);

                    newPosition += 5;
                    grph.DrawString(rollnumber[i], font, XBrushes.Black, new XRect(60, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString(studentName[i], font, XBrushes.Black, new XRect(160, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString("", font, XBrushes.Black, new XRect(360, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    grph.DrawString("", font, XBrushes.Black, new XRect(460, newPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    rowPos++;
                }
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
