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
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS_Web.Helpers.PdfHelper
{
    public class AlNusratChallanPdf : BasicPdf
    {
        int fine;
        IFeePlanRepository feePlanRepo;
        IStudentRepository studentRepo;
        ISecurityRepository securityRepo;
        IFinanceAccountRepository financeRepo;
        public AlNusratChallanPdf()
        {
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());
            financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
            securityRepo = new SecurityRepositoryImp(new SC_WEBEntities2()); ;
        }

        public PdfDocument CreatePdf(string className, string secName, IList SrNo, IList rollNo, IList stdName, IList challanId, IList issueChallanId, IList studId, string financeAccountType, string financeAccountId, IList fineList, string currentMonth)
        {
            PdfDocument pdfdoc = new PdfDocument();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                pdfdoc.Info.Title = "First Pdf";
                fine = (int)feePlanRepo.GetDefinedFine();
                for (int i = 0; i < rollNo.Count; i++)
                {
                    int chId = Convert.ToInt32(challanId[i].ToString());
                    int ischId = Convert.ToInt32(issueChallanId[i].ToString());
                    int stdId = Convert.ToInt32(studId[i].ToString());
                    int studentFine = Convert.ToInt32(fineList[i].ToString());
                    var student = studentRepo.GetStudentById(stdId);
                    CreatePdf(className, secName, SrNo[i].ToString(), rollNo[i].ToString(), stdName[i].ToString(), chId, ischId, stdId, pdfdoc, financeAccountType, financeAccountId, currentMonth, student, studentFine);
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
        private void CreatePdf(string className, string secName, string srNo, string rollNo, string stdName, int challanId, int issueChallanId, int studId, PdfDocument pdfdoc, string financeAccountType, string financeAccountId, string currentMonth, Student student, int studentFine)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                PdfPage page = pdfdoc.AddPage();
                page.Orientation = PageOrientation.Landscape;

                XGraphics grph = XGraphics.FromPdfPage(page);

                IssuedChallan issueChallan = feePlanRepo.GetIssueChallanByChalanId(issueChallanId);
                IssuedChallan prevIssueChallan = feePlanRepo.GetPreviousIssuedChallan((int)issueChallan.ChallanToStdId, (int)issueChallan.Id);

                List<ChallanDetailViewModel> challanDetail = feePlanRepo.GetChallDetailByChallanId(challanId);
                FeeBalance feeBalance = feePlanRepo.GetFeeBalanceByStudentId(studId);

                var arrearDetail = feePlanRepo.GetStudentArrearDetail(studId);
                var lastMonthFee = feePlanRepo.GetLastMonthsUnPaidFee((int)issueChallan.ChallanToStdId, issueChallanId);
                var extraChargesDetail = feePlanRepo.GetStudentExtraChargesByStudent(studId, currentMonth);

                List<IssueChalanDetail> detailList = feePlanRepo.GetIssueChallanDetail((int)issueChallan.Id).OrderBy(x => x.Type).ToList();
                List<IssueChalanDetail> CurrentExtras = detailList.Where(x => x.Type == 3).ToList();

                string challanNo = issueChallanId.ToString().PadLeft(15, '0');

                SchoolConfig schoolConfig = securityRepo.GetSchoolConfigByBranchId((int)student.BranchId);
                for (int part = 0; part < 3; part++)
                {
                    XFont font = new XFont("Verdana", 10, XFontStyle.Bold);
                    if (schoolConfig.SchoolName.Length > 30)
                        font = new XFont("Verdana", 9, XFontStyle.Bold);
                    else if (schoolConfig.SchoolName.Length > 40)
                        font = new XFont("Verdana", 8, XFontStyle.Bold);
                    else if (schoolConfig.SchoolName.Length > 50)
                        font = new XFont("Verdana", 7, XFontStyle.Bold);
                    grph.DrawString(schoolConfig.SchoolName.ToUpper(), font, XBrushes.Black, new XRect(20 + part * 260, 20, 230, 10), XStringFormats.TopCenter);
                    font = new XFont("Verdana", 10, XFontStyle.Regular);
                    grph.DrawString(schoolConfig.CampusName.ToUpper(), font, XBrushes.Black, new XRect(20 + part * 260, 35, 230, 10), XStringFormats.TopCenter);

                    font = new XFont("Verdana", 7, XFontStyle.Regular);
                    grph.DrawString("PH : +92-321-6580672", font, XBrushes.Black, new XRect(15 + part * 260, 50, 230, 10), XStringFormats.TopLeft);
                    grph.DrawString("EMAIL : alnusratschool@gmail.com", font, XBrushes.Black, new XRect(20 + part * 260 + 100, 50, 230, 10), XStringFormats.TopLeft);
                    font = new XFont("Verdana", 10, XFontStyle.Regular);
                    if (part == 0)
                        grph.DrawString("School Copy", font, XBrushes.Black, new XRect(20 + part * 260, 60, 230, 10), XStringFormats.TopCenter);
                    else if (part == 1)
                        grph.DrawString("Parents Copy", font, XBrushes.Black, new XRect(20 + part * 260, 60, 230, 10), XStringFormats.TopCenter);
                    else if (part == 2)
                        grph.DrawString("Branch Copy", font, XBrushes.Black, new XRect(20 + part * 260, 60, 230, 10), XStringFormats.TopCenter);

                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    //var accountList = feePlanRepo.GetFeePaidAccounts(int.Parse(financeAccountType), int.Parse(financeAccountId));
                    //string accountType = "Bank";
                    //if (financeAccountType != "1")
                    //    accountType = "Cash";
                    //var accountList = feePlanRepo.GetFeePaidAccounts(int.Parse(financeAccountType), int.Parse(financeAccountId));
                    //var accountList = financeRepo.GetFinanceAccountsFifthLvl(accountType, (int)student.BranchId);

                    //for(int k = 0; k < accountList.Count && k < 3 ; k++)
                    //{
                    //    grph.DrawString(accountList[k].AccountName, font, XBrushes.Black, new XRect(20 + part * 260, 60 + (k * 13), 230, 10), XStringFormats.TopCenter);
                    //}

                    //if (student.StdImage != null)
                    //{
                    //    TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                    //    Bitmap stdImag = (Bitmap)tc.ConvertFrom((byte[])student.StdImage);
                    //    Bitmap image = new Bitmap(stdImag, new Size(50, 50));
                    //    grph.DrawImage(image, 20 + part * 260 + 195, 40);
                    //}

                    string month = "", issuedDate = "", dueDate = "";
                    if (issueChallan != null)
                    {
                        issuedDate = issueChallan.IssueDate.Value.Date.ToString().Split(' ')[0];
                        dueDate = issueChallan.DueDate.Value.Date.ToString().Split(' ')[0];
                        month = issueChallan.ForMonth;
                    }

                    grph.DrawString("Fee Bill No : " + challanNo, font, XBrushes.Black, new XRect(10 + part * 260, 80, 230, 10), XStringFormats.TopLeft);
                    grph.DrawString("Due Date : " + dueDate, font, XBrushes.Black, new XRect(20 + part * 260 + 130, 80, 230, 10), XStringFormats.TopLeft);
                    grph.DrawRectangle(XPens.Black, 145 + part * 260, 75, 115, 15);
                    grph.DrawLine(XPens.Black, 10 + part * 260, 90, 260 + part * 260, 90);

                    grph.DrawString("Bill Date: " + issuedDate, font, XBrushes.Black, new XRect(10 + part * 260, 95, 230, 10), XStringFormats.TopLeft);
                    grph.DrawString("Admission No: " + student.AdmissionNo, font, XBrushes.Black, new XRect(20 + part * 260 + 130, 95, 230, 10), XStringFormats.TopLeft);
                    grph.DrawLine(XPens.Black, 10 + part * 260, 105, 260 + part * 260, 105);

                    grph.DrawString("Student Name : " + student.Name, font, XBrushes.Black, new XRect(10 + part * 260, 110, 230, 10), XStringFormats.TopLeft);
                    grph.DrawLine(XPens.Black, 10 + part * 260, 120, 260 + part * 260, 120);

                    grph.DrawString("Father Name : " + student.FatherName, font, XBrushes.Black, new XRect(10 + part * 260, 125, 230, 10), XStringFormats.TopLeft);
                    grph.DrawLine(XPens.Black, 10 + part * 260, 135, 260 + part * 260, 135);

                    grph.DrawString("Class : " + className, font, XBrushes.Black, new XRect(10 + part * 260, 140, 230, 10), XStringFormats.TopLeft);
                    grph.DrawLine(XPens.Black, 10 + part * 260, 150, 260 + part * 260, 150);

                    grph.DrawString("Section : " + secName, font, XBrushes.Black, new XRect(10 + part * 260, 155, 230, 10), XStringFormats.TopLeft);
                    grph.DrawLine(XPens.Black, 10 + part * 260, 165, 260 + part * 260, 165);
                    grph.DrawString("Remarks : ", font, XBrushes.Black, new XRect(10 + part * 260, 167, 230, 10), XStringFormats.TopLeft);


                    font = new XFont("Verdana", 8, XFontStyle.Bold);
                    grph.DrawString("Description", font, XBrushes.Black, new XRect(20 + part * 260, 198, 155, 10), XStringFormats.TopLeft);
                    grph.DrawString("Amount in Rupees", font, XBrushes.Black, new XRect(160 + part * 260, 198, 80, 10), XStringFormats.TopLeft);

                    int i = 1, total = 0;
                    int start = 198 + i * 15;
                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    if (challanDetail != null)
                    {
                        foreach (ChallanDetailViewModel detail in challanDetail)
                        {
                            if (detail.Amount > 0)
                            {
                                total += Convert.ToInt32(detail.Amount);
                                grph.DrawString(detail.Name, font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                                grph.DrawString(detail.Amount.ToString(), font, XBrushes.Black, new XRect(160 + part * 260, 198 + i * 15, 110, 10), XStringFormats.TopLeft);
                                i++;
                            }
                        }
                    }

                    if (prevIssueChallan == null)
                    {
                        if (extraChargesDetail != null && extraChargesDetail.Count > 0)
                        {
                            font = new XFont("Verdana", 8, XFontStyle.Bold);
                            //grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                            grph.DrawString("Extra Charges", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                            font = new XFont("Verdana", 8, XFontStyle.Regular);

                            i++;
                            foreach (StudentExtraChargesDetail detail in extraChargesDetail)
                            {
                                if (detail.HeadAmount > 0)
                                {
                                    total += Convert.ToInt32(detail.HeadAmount);
                                    grph.DrawString(detail.FeeHead.Name, font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                                    grph.DrawString(detail.HeadAmount.ToString(), font, XBrushes.Black, new XRect(160 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                                    i++;
                                }
                            }
                        }
                    }
                    else
                    {
                        int extraCount = CurrentExtras.Where(x => (int)x.TotalAmount - (int)(x.PayAmount == null ? 0 : x.PayAmount) - (int)(x.Discount == null ? 0 : x.Discount) > 0).Count();

                        if (extraCount > 0)
                        {
                            font = new XFont("Verdana", 8, XFontStyle.Bold);
                            //grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                            grph.DrawString("Extra Charges", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                            font = new XFont("Verdana", 8, XFontStyle.Regular);

                            i++;
                            foreach (var detail in CurrentExtras)
                            {
                                int pendingAmount = (int)detail.TotalAmount - (int)(detail.PayAmount == null ? 0 : detail.PayAmount) - (int)(detail.Discount == null ? 0 : detail.Discount);
                                if (pendingAmount > 0)
                                {
                                    total += Convert.ToInt32(pendingAmount);
                                    grph.DrawString(detail.FeeHead.Name, font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                                    grph.DrawString(pendingAmount.ToString(), font, XBrushes.Black, new XRect(160 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                                    i++;
                                }
                            }
                        }
                    }

                    arrearDetail = arrearDetail.Where(x => x.ArrearAmount > 0).ToList();

                    if (arrearDetail != null && arrearDetail.Count > 0 || lastMonthFee != null && lastMonthFee.Count > 0)
                    {
                        font = new XFont("Verdana", 8, XFontStyle.Bold);
                        //grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                        grph.DrawString("Arrears", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                        font = new XFont("Verdana", 8, XFontStyle.Regular);

                        i++;

                        if (prevIssueChallan == null)
                        {
                            if (arrearDetail != null && arrearDetail.Count > 0)
                            {
                                foreach (FeeArrearViewModel detail in arrearDetail)
                                {
                                    if (detail.ArrearAmount > 0)
                                    {
                                        total += Convert.ToInt32(detail.ArrearAmount);
                                        grph.DrawString(detail.HeadName, font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                                        grph.DrawString(detail.ArrearAmount.ToString(), font, XBrushes.Black, new XRect(160 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                                        i++;
                                    }
                                }
                            }
                        }

                        if (lastMonthFee != null && lastMonthFee.Count > 0)
                        {
                            foreach (FeeArrearViewModel detail in lastMonthFee)
                            {
                                if (detail.ArrearAmount > 0)
                                {
                                    total += Convert.ToInt32(detail.ArrearAmount);
                                    grph.DrawString(detail.HeadName, font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                                    grph.DrawString(detail.ArrearAmount.ToString(), font, XBrushes.Black, new XRect(160 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                                    i++;
                                }
                            }
                        }
                    }

                    int balance = 0, advance = 0;
                    if (feeBalance != null)
                    {
                        balance = (int)feeBalance.Balance;
                        advance = (int)feeBalance.Advance;
                    }


                    //var studentAnnualCharges = feePlanRepo.GetAnnualChargesByStudentIdAndMonth(studId, currentMonth);
                    //if (studentAnnualCharges != null && studentAnnualCharges.Count > 0)
                    //{
                    //    var annualCharges = studentAnnualCharges[0];
                    //    if (annualCharges != null && annualCharges.Charges > 0)
                    //    {

                    //        grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                    //        grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                    //        grph.DrawString("Annual Charges", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    //        grph.DrawString(annualCharges.Charges.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                    //        i++;
                    //        total += (int)annualCharges.Charges;
                    //    }
                    //}
                    //grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                    //grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                    //total += balance;
                    //grph.DrawString("Balance", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    //grph.DrawString(balance.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                    //i++;

                    total -= advance;
                    grph.DrawString("Advance", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    grph.DrawString(advance.ToString(), font, XBrushes.Black, new XRect(160 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                    i++;

                    grph.DrawString("Fine", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    grph.DrawString(studentFine.ToString(), font, XBrushes.Black, new XRect(160 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                    i++;
                    int end = 198 + i * 15 + 10;

                    grph.DrawLine(XPens.Black, 150 + part * 260, start, 150 + part * 260, end);
                    font = new XFont("Verdana", 8, XFontStyle.Bold);
                    grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 135, 15);
                    grph.DrawRectangle(XPens.Black, 150 + part * 260, 195 + i * 15, 105, 15);
                    grph.DrawString("Total", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    total += studentFine;
                    grph.DrawString(total.ToString(), font, XBrushes.Black, new XRect(160 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);

                    i++;
                    if (part == 0)
                    {
                        font = new XFont("Verdana", 10, XFontStyle.Bold);
                        grph.DrawString("Fee Concession", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    }

                    i++;
                    i++;
                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    grph.DrawString("Total Payable by Due Date", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    font = new XFont("Verdana", 10, XFontStyle.Regular);
                    grph.DrawString("Rs", font, XBrushes.Black, new XRect(170 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);

                    i = i + 4;
                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    grph.DrawString("Reciever Signature", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    grph.DrawString("Stamp", font, XBrushes.Black, new XRect(210 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    i++;
                    grph.DrawLine(XPens.Black, 15 + part * 260, 198 + i * 15, 100 + part * 260, 198 + i * 15);
                    grph.DrawLine(XPens.Black, 180 + part * 260, 198 + i * 15, 260 + part * 260, 198 + i * 15);
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
