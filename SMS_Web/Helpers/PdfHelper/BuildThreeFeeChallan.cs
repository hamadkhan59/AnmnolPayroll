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
    public class BuildThreeFeeChallan : BasicPdf
    {
        int fine;
        IFeePlanRepository feePlanRepo;
        IStudentRepository studentRepo;
        ISecurityRepository securityRepo;
        IFinanceAccountRepository financeRepo;
        public BuildThreeFeeChallan()
        {
            securityRepo = new SecurityRepositoryImp(new SC_WEBEntities2()); ;
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2()); ;
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());;
            financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());;
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
                grph.DrawRectangle(XPens.Black, 15, 15, 245, 575);
                grph.DrawRectangle(XPens.Black, 275, 15, 245, 575);
                grph.DrawRectangle(XPens.Black, 535, 15, 245, 575);

                SchoolConfig schoolConfig = securityRepo.GetSchoolConfigByBranchId((int)student.BranchId);
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

                for (int part = 0; part < 3; part++)
                {
                    XFont font = new XFont("Verdana", 10, XFontStyle.Bold);
                    if (schoolConfig.SchoolName.Length > 30)
                        font = new XFont("Verdana", 9, XFontStyle.Bold);
                    else if (schoolConfig.SchoolName.Length > 40)
                        font = new XFont("Verdana", 8, XFontStyle.Bold);
                    else if (schoolConfig.SchoolName.Length > 50)
                        font = new XFont("Verdana", 7, XFontStyle.Bold);
                    grph.DrawString(feePlanRepo.GetSchoolConfig().SchoolName, font, XBrushes.Black, new XRect(20 + part * 260, 20, 230, 10), XStringFormats.TopCenter);
                    grph.DrawLine(XPens.Black, 15 + part * 260, 35, 260 + part * 260, 35);
                    font = new XFont("Verdana", 10, XFontStyle.Regular);
                    if (part == 0)
                        grph.DrawString("Bank's Copy", font, XBrushes.Black, new XRect(20 + part * 260, 40, 230, 10), XStringFormats.TopCenter);
                    else if (part == 2)
                        grph.DrawString("Student's Copy", font, XBrushes.Black, new XRect(20 + part * 260, 40, 230, 10), XStringFormats.TopCenter);
                    else if (part == 1)
                        grph.DrawString("School's Copy", font, XBrushes.Black, new XRect(20 + part * 260, 40, 230, 10), XStringFormats.TopCenter);

                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    //var accountList = feePlanRepo.GetFeePaidAccounts(int.Parse(financeAccountType), int.Parse(financeAccountId));
                    string accountType = "Bank";
                    if (financeAccountType != "1")
                        accountType = "Cash";
                    //var accountList = feePlanRepo.GetFeePaidAccounts(int.Parse(financeAccountType), int.Parse(financeAccountId));
                    var accountList = financeRepo.GetFinanceAccountsFifthLvl(accountType, (int)student.BranchId);
                    if (!financeAccountId.Equals("0"))
                        accountList = accountList.Where(x => x.Id == int.Parse(financeAccountId)).ToList();

                    for (int k = 0; k < accountList.Count && k < 3; k++)
                    {
                        grph.DrawString(accountList[k].AccountName, font, XBrushes.Black, new XRect(20 + part * 260, 60 + (k * 13), 230, 10), XStringFormats.TopCenter);
                    }

                    if (schoolConfig.SchoolLogo != null)
                    {
                        TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                        Bitmap schImag = (Bitmap)tc.ConvertFrom((byte[])schoolConfig.SchoolLogo);
                        Bitmap schImage = new Bitmap(schImag, new Size(50, 50));
                        grph.DrawImage(schImage, 20 + part * 260, 40);
                    }

                    if (student.StdImage != null)
                    {
                        TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                        Bitmap stdImag = (Bitmap)tc.ConvertFrom((byte[])student.StdImage);
                        Bitmap image = new Bitmap(stdImag, new Size(50, 50));
                        grph.DrawImage(image, 20 + part * 260 + 195, 40);
                    }


                    grph.DrawLine(XPens.Black, 15 + part * 260, 100, 260 + part * 260, 100);
                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    grph.DrawString("Challan No: " + challanNo, font, XBrushes.Black, new XRect(20 + part * 260, 105, 230, 10), XStringFormats.TopCenter);

                    string month = "", issuedDate = "", dueDate = "";
                    if (issueChallan != null)
                    {
                        issuedDate = issueChallan.IssueDate.Value.Date.ToString().Split(' ')[0];
                        dueDate = issueChallan.DueDate.Value.Date.ToString().Split(' ')[0];
                        month = issueChallan.ForMonth;
                    }

                    grph.DrawString("Fee Of Month : " + month, font, XBrushes.Black, new XRect(20 + part * 260, 120, 230, 10), XStringFormats.TopCenter);
                    grph.DrawString("Issue Date : " + issuedDate + "  Due Date: " + dueDate, font, XBrushes.Black, new XRect(20 + part * 260, 135, 230, 10), XStringFormats.TopCenter);
                    //if(accountNo != null && accountNo.Length>0)
                    //    grph.DrawString("Account No : " + accountNo, font, XBrushes.Black, new XRect(20 + part * 260, 135, 230, 10), XStringFormats.TopCenter);

                    //font = new XFont("Verdana", 7, XFontStyle.Bold);
                    font = new XFont("Arial", 8, XFontStyle.Bold);
                    string name = srNo + ", " + stdName + ", " + rollNo;
                    if (name.Length > 50)
                        font = new XFont("Arial", 6, XFontStyle.Bold);
                    if (name.Length > 40)
                        font = new XFont("Arial", 7, XFontStyle.Bold);

                    grph.DrawString(srNo + ", " + stdName + ", " + rollNo, font, XBrushes.Black, new XRect(20 + part * 260, 150, 230, 10), XStringFormats.TopCenter);
                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    grph.DrawString("Class : " + className + "  " + "Section : " + secName, font, XBrushes.Black, new XRect(20 + part * 260, 165, 230, 10), XStringFormats.TopCenter);

                    grph.DrawRectangle(XPens.Black, 15 + part * 260, 195, 160, 15);
                    grph.DrawRectangle(XPens.Black, 175 + part * 260, 195, 85, 15);
                    font = new XFont("Verdana", 8, XFontStyle.Bold);
                    grph.DrawString("Fee Head", font, XBrushes.Black, new XRect(20 + part * 260, 198, 155, 10), XStringFormats.TopLeft);
                    grph.DrawString("Amount", font, XBrushes.Black, new XRect(180 + part * 260, 198, 80, 10), XStringFormats.TopLeft);

                    int i = 1, total = 0;
                    font = new XFont("Verdana", 8, XFontStyle.Regular);
                    if (challanDetail != null)
                    {
                        foreach (ChallanDetailViewModel detail in challanDetail)
                        {
                            if (detail.Amount > 0)
                            {
                                grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                                grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                                total += Convert.ToInt32(detail.Amount);
                                grph.DrawString(detail.Name, font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                                grph.DrawString(detail.Amount.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                                i++;
                            }
                        }
                    }



                    if (prevIssueChallan == null)
                    {
                        if (extraChargesDetail != null && extraChargesDetail.Count > 0)
                        {
                            font = new XFont("Verdana", 8, XFontStyle.Bold);
                            grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 245, 15);
                            //grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                            grph.DrawString("Extra Charges", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                            font = new XFont("Verdana", 8, XFontStyle.Regular);

                            i++;
                            foreach (StudentExtraChargesDetail detail in extraChargesDetail)
                            {
                                if (detail.HeadAmount > 0)
                                {
                                    grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                                    grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                                    total += Convert.ToInt32(detail.HeadAmount);
                                    grph.DrawString(detail.FeeHead.Name, font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                                    grph.DrawString(detail.HeadAmount.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
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
                                    grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                                    grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                                    total += Convert.ToInt32(pendingAmount);
                                    grph.DrawString(detail.FeeHead.Name, font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                                    grph.DrawString(pendingAmount.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                                    i++;
                                }
                            }
                        }
                    }

                    arrearDetail = arrearDetail.Where(x => x.ArrearAmount > 0).ToList();

                    if ((arrearDetail != null && arrearDetail.Count > 0) || (lastMonthFee != null && lastMonthFee.Count > 0))
                    {
                        font = new XFont("Verdana", 8, XFontStyle.Bold);
                        grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 245, 15);
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
                                        grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                                        grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                                        total += Convert.ToInt32(detail.ArrearAmount);
                                        grph.DrawString(detail.HeadName, font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                                        grph.DrawString(detail.ArrearAmount.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
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
                                    grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                                    grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                                    total += Convert.ToInt32(detail.ArrearAmount);
                                    grph.DrawString(detail.HeadName, font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                                    grph.DrawString(detail.ArrearAmount.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
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


                    var studentAnnualCharges = feePlanRepo.GetAnnualChargesByStudentIdAndMonth(studId, currentMonth);
                    if (studentAnnualCharges != null && studentAnnualCharges.Count > 0)
                    {
                        var annualCharges = studentAnnualCharges[0];
                        if (annualCharges != null && annualCharges.Charges > 0)
                        {

                            grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                            grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                            grph.DrawString("Annual Charges", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                            grph.DrawString(annualCharges.Charges.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                            i++;
                            total += (int)annualCharges.Charges;
                        }
                    }
                    //grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                    //grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                    //total += balance;
                    //grph.DrawString("Balance", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    //grph.DrawString(balance.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                    //i++;

                    grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                    grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                    total -= advance;
                    grph.DrawString("Advance", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    grph.DrawString(advance.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                    i++;

                    grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                    grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                    grph.DrawString("Fine", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    grph.DrawString(studentFine.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);
                    i++;

                    font = new XFont("Verdana", 8, XFontStyle.Bold);
                    grph.DrawRectangle(XPens.Black, 15 + part * 260, 195 + i * 15, 160, 15);
                    grph.DrawRectangle(XPens.Black, 175 + part * 260, 195 + i * 15, 85, 15);
                    grph.DrawString("Total", font, XBrushes.Black, new XRect(20 + part * 260, 198 + i * 15, 155, 10), XStringFormats.TopLeft);
                    total += studentFine;
                    grph.DrawString(total.ToString(), font, XBrushes.Black, new XRect(180 + part * 260, 198 + i * 15, 80, 10), XStringFormats.TopLeft);

                    XFont cashierFont = new XFont("Verdana", 8, XFontStyle.Regular);
                    int cashierTextHeight = 520;

                    font = new XFont("Verdana", 6, XFontStyle.Regular);
                    Branch schoolBranch = securityRepo.GetBranchById((int)student.BranchId);

                    string campusEmail = schoolBranch.EMAIL;
                    //campusEmail = "gcgs.cantt@gmail.com";
                    if (campusEmail != null && campusEmail.Length > 5)
                    {
                        XFont camFont = new XFont("Verdana", 9, XFontStyle.Bold);
                        if (campusEmail.Length > 30)
                            camFont = new XFont("Verdana", 8, XFontStyle.Bold);
                        else if (campusEmail.Length > 40)
                            camFont = new XFont("Verdana", 7, XFontStyle.Bold);
                        else if (campusEmail.Length > 50)
                            camFont = new XFont("Verdana", 6, XFontStyle.Bold);

                        grph.DrawString(campusEmail, camFont, XBrushes.Black, new XRect(25 + part * 260, 535, 230, 10), XStringFormats.TopLeft);
                    }
                    else
                        cashierTextHeight += 15;

                    string campusPhoneNumber = schoolBranch.PHONE_NUMBER;
                    //campusPhoneNumber = "MOB - 0309-3333154, UAN 0304-(111-4949)";
                    if (campusPhoneNumber != null && campusPhoneNumber.Length > 5)
                    {
                        XFont camPFont = new XFont("Verdana", 9, XFontStyle.Bold);
                        if (campusPhoneNumber.Length > 30)
                            camPFont = new XFont("Verdana", 8, XFontStyle.Bold);
                        else if (campusPhoneNumber.Length > 40)
                            camPFont = new XFont("Verdana", 7, XFontStyle.Bold);
                        else if (campusPhoneNumber.Length > 50)
                            camPFont = new XFont("Verdana", 6, XFontStyle.Bold);

                        grph.DrawString(campusPhoneNumber, camPFont, XBrushes.Black, new XRect(25 + part * 260, 550, 230, 10), XStringFormats.TopLeft);
                    }
                    else
                        cashierTextHeight += 15;

                    grph.DrawString("S.No_______________ Officer CASHIER", cashierFont, XBrushes.Black, new XRect(25 + part * 260, cashierTextHeight, 230, 10), XStringFormats.TopLeft);

                    //grph.DrawLine(XPens.Black, startPoint - 5 + part * 260, 200 + i * 13 + height, startPoint + 240 + part * 260, 200 + i * 13 + height);
                    font = new XFont("Verdana", 6, XFontStyle.Regular);
                    if (fine > 0)
                    {
                        grph.DrawString("Note : Fine of Rs." + fine.ToString() + " is charged per day on late fee submission", font, XBrushes.Black, new XRect(25 + part * 260, 570, 230, 10), XStringFormats.TopLeft);
                    }
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
