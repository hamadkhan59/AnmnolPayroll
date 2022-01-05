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
using SMS.Modules.BuildPdf;
using System.Globalization;
using SMS_Web.Helpers.PdfHelper;
using SMS_DAL;
using SMS_Web.Helpers;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using Logger;
using System.Reflection;

namespace SMS.Modules.BuildPdf.FeeSheets
{
    public class AdmissionChargesPdf : BasicPdf
    {
        //private SC_WEBEntities2 db = SessionHelper.dbContext;

        private IStudentRepository studentRepo;
        private IFeePlanRepository feePlanRepo;
        private SC_WEBEntities2 db = SessionHelper.dbContext;

        public AdmissionChargesPdf()
        {
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2()); ;
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2()); ;
        }

        public PdfDocument CreatePdf(Student stdudent)
        {
            PdfDocument pdfdoc = new PdfDocument();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                pdfdoc.Info.Title = "First Pdf";
                CreatePdf(stdudent, pdfdoc, "", "");
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return pdfdoc;
        }
        private void CreatePdf(Student student, PdfDocument pdfdoc, string accountNo, string bankName)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                PdfPage page = pdfdoc.AddPage();
                page.Orientation = PageOrientation.Landscape;

                XGraphics grph = XGraphics.FromPdfPage(page);
                grph.DrawRectangle(XPens.Black, 15, 15, 245, 575);
                grph.DrawRectangle(XPens.Black, 275, 15, 245, 575);
                grph.DrawRectangle(XPens.Black, 535, 15, 245, 575);

                var admissionCharges = feePlanRepo.GetPositiveStudentAdmissionChargesByStudentId(student.id);
                //IList issueChallanList = feeMang.GetIssueChallanInfo(issueChallanId);
                //IList challanList = feeMang.GetChallanInfo(challanId);
                //IList feeBalance = feeMang.GetBalanceAndAdvance(studId);
                //string challanNo = GetChallanNo(issueChallanId);
                string SchoolName = db.SchoolConfigs.Find(1).SchoolName;
                for (int part = 0; part < 3; part++)
                {
                    XFont font = new XFont("Verdana", 10, XFontStyle.Bold);
                    grph.DrawString(SchoolName, font, XBrushes.Black, new XRect(20 + part * 260, 20, 230, 10), XStringFormats.TopCenter);
                    grph.DrawLine(XPens.Black, 15 + part * 260, 35, 260 + part * 260, 35);
                    font = new XFont("Verdana", 10, XFontStyle.Regular);
                    if (part == 0)
                        grph.DrawString("Student's Copy (Admission Charges)", font, XBrushes.Black, new XRect(20 + part * 260, 45, 230, 10), XStringFormats.TopCenter);
                    else if (part == 1)
                        grph.DrawString("Bank's Copy (Admission Charges)", font, XBrushes.Black, new XRect(20 + part * 260, 45, 230, 10), XStringFormats.TopCenter);
                    else
                        grph.DrawString("School's Copy (Admission Charges)", font, XBrushes.Black, new XRect(20 + part * 260, 45, 230, 10), XStringFormats.TopCenter);

                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    grph.DrawString(bankName, font, XBrushes.Black, new XRect(20 + part * 260, 65, 230, 10), XStringFormats.TopCenter);

                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    //grph.DrawString("Challan No: " + challanNo, font, XBrushes.Black, new XRect(20 + part * 260, 80, 230, 10), XStringFormats.TopCenter);

                    //string month = "", issuedDate = "", dueDate = "";
                    //foreach( object [] obj in issueChallanList )
                    //{
                    //    issuedDate = obj.ElementAt(0).ToString().Split(' ')[0];
                    //    dueDate = obj.ElementAt(1).ToString().Split(' ')[0];
                    //    month = obj.ElementAt(2).ToString();
                    //}

                    grph.DrawString("Admission Month : " + DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture), font, XBrushes.Black, new XRect(20 + part * 260, 100, 230, 10), XStringFormats.TopCenter);
                    grph.DrawString("Issue Date : " + DateTime.Now.ToString("dd/MM/yyyy") + "  Due Date: " + DateTime.Now.Date.AddDays(10).ToString("dd/MM/yyyy"), font, XBrushes.Black, new XRect(20 + part * 260, 115, 230, 10), XStringFormats.TopCenter);
                    if (accountNo.Length > 0)
                        grph.DrawString("Account No : " + accountNo, font, XBrushes.Black, new XRect(20 + part * 260, 135, 230, 10), XStringFormats.TopCenter);

                    font = new XFont("Verdana", 8, XFontStyle.Bold);
                    grph.DrawString(student.SrNo + ", " + student.Name + ", " + student.RollNumber, font, XBrushes.Black, new XRect(20 + part * 260, 150, 230, 10), XStringFormats.TopCenter);
                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    grph.DrawString("Class : " + student.ClassSection.Class.Name + "  " + "Section : " + student.ClassSection.Section.Name, font, XBrushes.Black, new XRect(20 + part * 260, 165, 230, 10), XStringFormats.TopCenter);

                    grph.DrawRectangle(XPens.Black, 15 + part * 260, 195, 160, 15);
                    grph.DrawRectangle(XPens.Black, 175 + part * 260, 195, 85, 15);
                    font = new XFont("Verdana", 8, XFontStyle.Bold);
                    grph.DrawString("Fee Head", font, XBrushes.Black, new XRect(20 + part * 260, 198, 155, 10), XStringFormats.TopLeft);
                    grph.DrawString("Amount", font, XBrushes.Black, new XRect(180 + part * 260, 198, 80, 10), XStringFormats.TopLeft);

                    int i = 1, total = 0;
                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    foreach (StudentAdmissionCharge charges in admissionCharges)
                    {
                        if (charges.Amount > 0)
                        {
                            grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                            grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                            total += Convert.ToInt32(charges.Amount);
                            grph.DrawString(charges.FeeHead.Name.ToString(), font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                            grph.DrawString(string.Format("{0:#,##0}", double.Parse(charges.Amount.ToString())), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                            i++;
                        }
                    }
                    //int balance = 0, advance = 0;
                    //foreach( object [] objbl in feeBalance )
                    //{
                    //    balance = Convert.ToInt32(objbl.ElementAt(0).ToString());
                    //    advance = Convert.ToInt32(objbl.ElementAt(1).ToString());
                    //}
                    //grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                    //grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                    //total += balance;
                    //grph.DrawString("Balance", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    //grph.DrawString(balance.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                    //i++;

                    //grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                    //grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                    //total -= advance;
                    //grph.DrawString("Advance", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    //grph.DrawString(advance.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                    //i++;

                    //grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                    //grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                    //total -= advance;
                    //grph.DrawString("Fine", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    ////grph.DrawString(advance.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                    //i++;

                    font = new XFont("Verdana", 8, XFontStyle.Bold);
                    grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                    grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                    grph.DrawString("Total", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    grph.DrawString(string.Format("{0:#,##0}", double.Parse(total.ToString())), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);

                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    grph.DrawString("S.No_______________ Officer CASHIER", font, XBrushes.Black, new XRect(25 + part * 260, 550, 230, 10), XStringFormats.TopLeft);

                    //font = new XFont("Verdana", 6, XFontStyle.Regular);
                    //grph.DrawString("Fine of Rs." + fine.ToString() + " is charged per day on late fee submission", font, XBrushes.Black, new XRect(25 + part * 260, 570, 230, 10), XStringFormats.TopLeft);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            //Process.Start(pdfFilename);
        }
        private string GetChallanNo(int challanId)
        {
            string challnNo = "";
            for (int i = 0; i < 15 - challanId.ToString().Length; i++)
            {
                challnNo += "0";
            }
            challnNo += challanId.ToString();
            return challnNo;
        }
    }
}
