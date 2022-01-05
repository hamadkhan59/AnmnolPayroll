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
using SMS_DAL.ViewModel;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using PdfSharp.Drawing.Layout;
using Logger;
using System.Reflection;

namespace SMS_Web.Helpers.PdfHelper
{
    public class DateSheetClassWisePdf : BasicPdf
    {
        private SC_WEBEntities2 db = SessionHelper.dbContext;

        public PdfDocument createDateSheet(int[] subjectIds, string className, string sectionName, string examName, DateTime[] ExamDate, string[] startTimes, string[] endTimes, string[] Center, string[] SubjectName, PdfDocument pdfdoc, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                //PdfDocument pdfdoc = new PdfDocument();
                PdfPage page = pdfdoc.AddPage();
                page.Height = 550;
                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, 40, 100, 520, 20);
                grph.DrawString("Date Sheet of Exam : " + examName, font, XBrushes.RoyalBlue, new XRect(50, 105, 500, page.Height.Point), XStringFormats.TopCenter);

                if (string.IsNullOrEmpty(sectionName))
                {
                    grph.DrawRectangle(XPens.RoyalBlue, 40, 130, 520, 20);
                    grph.DrawString("Class : " + className, font, XBrushes.RoyalBlue, new XRect(50, 135, 500, page.Height.Point), XStringFormats.TopCenter);

                }
                else
                {
                    grph.DrawRectangle(XPens.RoyalBlue, 40, 130, 260, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 300, 130, 260, 20);

                    grph.DrawString("Class : " + className, font, XBrushes.Black, new XRect(70, 135, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    grph.DrawString("Section : " + sectionName, font, XBrushes.Black, new XRect(330, 135, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                }


                int count = 0;
                for (int i = 0; i <= subjectIds.Count(); i++)
                {
                    if (i == 0)
                    {
                        grph.DrawRectangle(XPens.RoyalBlue, 40, 190 + count, 180, 25);
                        grph.DrawRectangle(XPens.RoyalBlue, 40 + 180, 190 + count, 120, 25);
                        grph.DrawRectangle(XPens.RoyalBlue, 40 + 180 + 120, 190 + count, 105, 25);
                        grph.DrawRectangle(XPens.RoyalBlue, 40 + 180 + 120 + 105, 190 + count, 115, 25);
                        count += 25;
                    }
                    else
                    {
                        grph.DrawRectangle(XPens.RoyalBlue, 40, 190 + count, 180, 20);
                        grph.DrawRectangle(XPens.RoyalBlue, 40 + 180, 190 + count, 120, 20);
                        grph.DrawRectangle(XPens.RoyalBlue, 40 + 180 + 120, 190 + count, 105, 20);
                        grph.DrawRectangle(XPens.RoyalBlue, 40 + 180 + 120 + 105, 190 + count, 115, 20);
                        count += 20;
                    }
                }

                font = new XFont("Verdana", 11, XFontStyle.Bold);

                grph.DrawString("Subject Name", font, XBrushes.RoyalBlue, new XRect(45, 193, 180, 20), XStringFormats.TopLeft);
                grph.DrawString("Exam Date", font, XBrushes.RoyalBlue, new XRect(45 + 180, 193, 120, 20), XStringFormats.TopLeft);
                grph.DrawString("Exam Time", font, XBrushes.RoyalBlue, new XRect(45 + 180 + 120, 193, 105, 20), XStringFormats.TopLeft);
                grph.DrawString("Exam Center", font, XBrushes.RoyalBlue, new XRect(45 + 180 + 120 + 105, 193, 115, 20), XStringFormats.TopLeft);

                count = 20;
                font = new XFont("Verdana", 9, XFontStyle.Regular);
                for (int i = 0; i < subjectIds.Count(); i++)
                {
                    grph.DrawString(SubjectName[i], font, XBrushes.Black, new XRect(45, 198 + count, 180, 20), XStringFormats.TopLeft);
                    string sDate = ExamDate[i].ToString();
                    string[] date = sDate.Split(' ');
                    grph.DrawString(date[0], font, XBrushes.Black, new XRect(45 + 180, 198 + count, 120, 20), XStringFormats.TopLeft);
                    grph.DrawString(startTimes[i] + "-" + endTimes[i], font, XBrushes.Black, new XRect(45 + 180 + 120, 198 + count, 105, 20), XStringFormats.TopLeft);
                    grph.DrawString(Center[i], font, XBrushes.Black, new XRect(45 + 180 + 120 + 105, 198 + count, 115, 20), XStringFormats.TopLeft);
                    count += 20;
                }

                //grph.DrawString("Note:", font, XBrushes.RoyalBlue, new XRect(50, page.Height - 110, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //font = new XFont("Verdana", 8, XFontStyle.Regular);
                //grph.DrawString("> Absent Student will be considered as Fail", font, XBrushes.Black, new XRect(60, page.Height - 100, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString("> Late comers will not be permitted to sit in Examination Hall", font, XBrushes.Black, new XRect(60, page.Height - 90, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                //grph.DrawString("Contact : 03216580672", font, XBrushes.Black, new XRect(50, page.Height - 60, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                DateSheetConfig config = db.DateSheetConfigs.Where(x => x.BranchId == branchId).FirstOrDefault();

                if (config != null && config.Notes != null)
                {
                    grph.DrawString("Note:", font, XBrushes.RoyalBlue, new XRect(50, page.Height - 110, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    font = new XFont("Verdana", 8, XFontStyle.Regular);

                    XTextFormatter tf = new XTextFormatter(grph);
                    tf.DrawString(config.Notes, font, XBrushes.Black, new XRect(60, page.Height - 100, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    //grph.DrawString(config.Notes, font, XBrushes.Black, new XRect(60, page.Height - 100, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                if (config != null && config.ContactNo != null)
                {
                    grph.DrawString("Contact : " + config.ContactNo, font, XBrushes.Black, new XRect(50, page.Height - 60, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                //XImage image = XImage.FromFile(@"C:\signatures\alnusrat.png");
                //grph.DrawImage(image, 435, page.Height - 110);
                base.DrawPrincipalSignature(grph, page, 435, (int)page.Height - 110, branchId);

                grph.DrawLine(XPens.Black, 435, page.Height - 50, page.Width - 40, page.Height - 50);
                font = new XFont("Verdana", 7, XFontStyle.Regular);
                grph.DrawString("Principal's Signature", font, XBrushes.RoyalBlue, new XRect(435, page.Height - 45, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                DesignSchoolHeader(grph, page, branchId);
                //DesignBoreder(grph, page, 520);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return pdfdoc;
        }

        public PdfDocument createDateSheet(int[] subjectIds, string className, string sectionName, string examName, DateTime[] ExamDate, string[] startTimes, string[] endTimes, string[] Center, string[] SubjectName, PdfDocument pdfdoc, int branchId, List<int> classSubjectsIds)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                //PdfDocument pdfdoc = new PdfDocument();
                PdfPage page = pdfdoc.AddPage();
                page.Height = 550;
                XGraphics grph = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Verdana", 10, XFontStyle.Bold);
                grph.DrawRectangle(XPens.RoyalBlue, 40, 100, 520, 20);
                grph.DrawString("Date Sheet of Exam : " + examName, font, XBrushes.RoyalBlue, new XRect(50, 105, 500, page.Height.Point), XStringFormats.TopCenter);

                if (string.IsNullOrEmpty(sectionName))
                {
                    grph.DrawRectangle(XPens.RoyalBlue, 40, 130, 520, 20);
                    grph.DrawString("Class : " + className, font, XBrushes.RoyalBlue, new XRect(50, 135, 500, page.Height.Point), XStringFormats.TopCenter);

                }
                else
                {
                    grph.DrawRectangle(XPens.RoyalBlue, 40, 130, 260, 20);
                    grph.DrawRectangle(XPens.RoyalBlue, 300, 130, 260, 20);

                    grph.DrawString("Class : " + className, font, XBrushes.Black, new XRect(70, 135, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    grph.DrawString("Section : " + sectionName, font, XBrushes.Black, new XRect(330, 135, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                }


                int count = 0;
                for (int i = 0; i <= subjectIds.Count(); i++)
                {
                    if (i == subjectIds.Count() || classSubjectsIds.Contains(subjectIds[i]) == true)
                    {
                        if (i == 0)
                        {
                            grph.DrawRectangle(XPens.RoyalBlue, 40, 190 + count, 180, 25);
                            grph.DrawRectangle(XPens.RoyalBlue, 40 + 180, 190 + count, 120, 25);
                            grph.DrawRectangle(XPens.RoyalBlue, 40 + 180 + 120, 190 + count, 105, 25);
                            grph.DrawRectangle(XPens.RoyalBlue, 40 + 180 + 120 + 105, 190 + count, 115, 25);
                            count += 25;
                        }
                        else
                        {
                            grph.DrawRectangle(XPens.RoyalBlue, 40, 190 + count, 180, 20);
                            grph.DrawRectangle(XPens.RoyalBlue, 40 + 180, 190 + count, 120, 20);
                            grph.DrawRectangle(XPens.RoyalBlue, 40 + 180 + 120, 190 + count, 105, 20);
                            grph.DrawRectangle(XPens.RoyalBlue, 40 + 180 + 120 + 105, 190 + count, 115, 20);
                            count += 20;
                        }
                    }
                }

                font = new XFont("Verdana", 11, XFontStyle.Bold);

                grph.DrawString("Subject Name", font, XBrushes.RoyalBlue, new XRect(45, 193, 180, 20), XStringFormats.TopLeft);
                grph.DrawString("Exam Date", font, XBrushes.RoyalBlue, new XRect(45 + 180, 193, 120, 20), XStringFormats.TopLeft);
                grph.DrawString("Exam Time", font, XBrushes.RoyalBlue, new XRect(45 + 180 + 120, 193, 105, 20), XStringFormats.TopLeft);
                grph.DrawString("Exam Center", font, XBrushes.RoyalBlue, new XRect(45 + 180 + 120 + 105, 193, 115, 20), XStringFormats.TopLeft);

                count = 20;
                font = new XFont("Verdana", 9, XFontStyle.Regular);
                for (int i = 0; i < subjectIds.Count(); i++)
                {
                    if (classSubjectsIds.Contains(subjectIds[i]) == true)
                    {
                        grph.DrawString(SubjectName[i], font, XBrushes.Black, new XRect(45, 198 + count, 180, 20), XStringFormats.TopLeft);
                        string sDate = ExamDate[i].ToString();
                        string[] date = sDate.Split(' ');
                        grph.DrawString(date[0], font, XBrushes.Black, new XRect(45 + 180, 198 + count, 120, 20), XStringFormats.TopLeft);
                        grph.DrawString(startTimes[i] + "-" + endTimes[i], font, XBrushes.Black, new XRect(45 + 180 + 120, 198 + count, 105, 20), XStringFormats.TopLeft);
                        grph.DrawString(Center[i], font, XBrushes.Black, new XRect(45 + 180 + 120 + 105, 198 + count, 115, 20), XStringFormats.TopLeft);
                        count += 20;
                    }
                }

                DateSheetConfig config = db.DateSheetConfigs.Where(x => x.BranchId == branchId).FirstOrDefault();

                if (config != null && config.Notes != null)
                {
                    grph.DrawString("Note:", font, XBrushes.RoyalBlue, new XRect(50, page.Height - 110, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    XTextFormatter tf = new XTextFormatter(grph);
                    tf.DrawString(config.Notes, font, XBrushes.Black, new XRect(60, page.Height - 100, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                    //grph.DrawString(config.Notes, font, XBrushes.Black, new XRect(60, page.Height - 100, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }

                if (config != null && config.ContactNo != null)
                {
                    grph.DrawString("Contact : " + config.ContactNo, font, XBrushes.Black, new XRect(50, page.Height - 60, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                }
                //XImage image = XImage.FromFile(@"C:\signatures\alnusrat.png");
                //grph.DrawImage(image, 435, page.Height - 110);
                base.DrawPrincipalSignature(grph, page, 435, (int)page.Height - 110, branchId);
                grph.DrawLine(XPens.Black, 435, page.Height - 50, page.Width - 40, page.Height - 50);
                font = new XFont("Verdana", 7, XFontStyle.Regular);
                grph.DrawString("Principal's Signature", font, XBrushes.RoyalBlue, new XRect(435, page.Height - 45, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

                DesignSchoolHeader(grph, page, branchId);
                //DesignBoreder(grph, page, 520);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return pdfdoc;
        }

        private void DesignBoreder(XGraphics grph, PdfPage page, int height)
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

        private void DesignSchoolHeader(XGraphics grph, PdfPage page, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                ISecurityRepository secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
                SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                grph.DrawImage((Bitmap)tc.ConvertFrom((byte[])config.SchoolLogo), 25, 25);

                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);

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
    }
}