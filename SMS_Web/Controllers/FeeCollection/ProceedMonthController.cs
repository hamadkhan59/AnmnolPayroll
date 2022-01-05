using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_DAL.ViewModel;
using System.Globalization;
using System.Collections;
using SMS_Web.Helpers.PdfHelper;
using System.IO;
using PdfSharp.Pdf;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.FeeCollection
{
    public class ProceedMonthController : Controller
    {
        //
        // GET: /ProceedMonth/

        IFeePlanRepository feePlanRepo;
        IFinanceAccountRepository financeRepo;
        IClassSectionRepository classSecRepo;
        IClassRepository classRepo;
        ISectionRepository secRepo;
        IAttendanceRepository attRepo;
        private IFinanceAccountRepository accountRepo;
        IStudentRepository studentRepo;
        static int errorCode = 0;

        public ProceedMonthController()
        {
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());;
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());;
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());;
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2()); ;
            attRepo = new AttendanceRepositoryImp(new SC_WEBEntities2()); ;
        }

        public ActionResult Index()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_PROCEED_MONTH) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            MonthDetailModel model = new MonthDetailModel();

            try
            {
                ViewBag.MonthId = new SelectList(SessionHelper.MonthList, "Id", "Month1");
                ViewBag.YearId = new SelectList(SessionHelper.YearList, "Id", "Year1");
                ViewData["Error"] = errorCode;
                errorCode = 0;
                model = feePlanRepo.GetLastMonthSummary();
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(model);
        }


        public ActionResult ProceedMonth(int MonthId, int YearId)
        {
            try
            {

                if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
                {
                    return RedirectToAction("Index", "Login");
                }

                if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_PROCEED_MONTH) == false)
                {
                    return RedirectToAction("Index", "NoPermission");
                }

                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);

                if (IsFineConfigDefined(branchId) == false)
                {
                    errorCode = 515;
                    return RedirectToAction("Index", new { id = -59 });
                }

                List<StudentModel> studentList = null;
                List<IssuedChallanViewModel> studentChalanList = new List<IssuedChallanViewModel>();

                //int monthId = (int)Session[ConstHelper.SEARCH_CHALLAN_MONTH_ID];
                //int yearId = (int)Session[ConstHelper.SEARCH_CHALLAN_YEAR_ID];
                string currentMonth = SessionHelper.GetMonthName(MonthId) + "-" + (2016 + YearId - 1);

                studentChalanList = feePlanRepo.SearchIssueChallan(0, "", "", "", currentMonth, "", branchId, "", "");
                LogWriter.WriteLog("Student Challan List Count : " + (studentChalanList == null ? 0 : studentChalanList.Count));
                if (studentChalanList == null || studentChalanList.Count == 0)
                {
                    studentList = feePlanRepo.SearchStudentForChallan("", "", "", "", branchId);
                    studentList = studentList.Where(x => x.LeavingStatusCode == 1).ToList();

                    foreach (var student in studentList)
                    {
                        IssuedChallanViewModel icvm = new IssuedChallanViewModel();
                        ChallanStudentDetail detail = feePlanRepo.GetStudentChallanDetailByStudentId(student.Id);

                        if (detail != null)
                        {
                            IssuedChallan issueChalan = new IssuedChallan();
                            issueChalan.ChallanToStdId = detail.Id;
                            issueChalan.ChalanAmount = feePlanRepo.GetChallanAmountByChallanId(detail.ChallanId);
                            //issueChalan.ChalanAmount += feePlanRepo.GetStudentArrearDetail(student.Id).Sum(x => x.ArrearAmount);
                            issueChalan.ChalanAmount += feePlanRepo.GetStudentExtraChargesByStudent(student.Id, currentMonth).Sum(x => x.HeadAmount);
                            issueChalan.PayedTo = 0;
                            issueChalan.Fine = GetStudentTotalFine(student.Id, MonthId, (2016 + YearId - 1), branchId, DateTime.Parse(issueChalan.DueDate.ToString()));
                            //issueChalan.Fine = 0;
                            issueChalan.Amount = 0;
                            issueChalan.PaidFlag = 0;
                            issueChalan.IssuedFlag = 1;
                            issueChalan.ForMonth = currentMonth;
                            issueChalan.PaidDate = issueChalan.IssueDate = DateTime.Now;
                            issueChalan.DueDate = DateTime.Now.AddDays(10);
                            issueChalan.BranchId = branchId;
                            feePlanRepo.AddIssueChallan(issueChalan);
                            LogWriter.WriteLog("Challan Saved Succesfully Id : " + issueChalan.Id);

                            int balanceId = 0;
                            FeeBalance balance = feePlanRepo.GetFeeBalanceByStudentId(student.Id);
                            if (balance != null)
                                balanceId = balance.Id;
                            var studentChalan = feePlanRepo.GetStudentChallanDetailById((int)issueChalan.Id);
                            var studentModel = studentRepo.GetStudentById(studentChalan.StdId);
                            //SaveIssueChallanDetail((int)issueChalan.Id, DateTime.Now, balanceId, 0, 0, detail.ChallanId, student.Id, currentMonth);
                            LogWriter.WriteLog("Saving Challan Detail");
                            SaveIssueChallanDetail((int)issueChalan.Id, DateTime.Now, DateTime.Now, balanceId, 0, 0, studentChalan.ChallanId, studentChalan.StdId, currentMonth);
                            LogWriter.WriteLog("Creating Journal Entry for the Challan");
                            CreateJournalEntry(issueChalan.Id.ToString(), DateTime.Now, (int)issueChalan.ChalanAmount, "", "JE", studentModel, currentMonth, branchId);
                        }
                    }
                    errorCode = 2;
                    LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                }
                else
                {
                    errorCode = 10;
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index");
        }

        private int CreateJournalEntry(string ChequeNo, DateTime EntryDate, int EntryAmount, string CreditDescription, string EntryType, Student student, string forMonth, int branchId)
        {
            int errorCode = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Creating Journal Entry for the IssueChallanNo : " + ChequeNo);

                string studenInfo = " (" + student.RollNumber + ", " + student.Name + ", " + student.FatherName + ")";
                List<IssueChalanDetail> detailList = feePlanRepo.GetIssueChallanDetail(int.Parse(ChequeNo));
                JournalEntry je = new JournalEntry();
                je.CreditAmount = detailList.Sum(x => x.TotalAmount);
                je.DebitAmount = je.CreditAmount;
                je.CreditDescription = "Fee Amount is added to Student Receivables account, Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                je.CreatedOn = EntryDate;
                je.ChequeNo = ChequeNo;
                je.EntryType = EntryType;
                je.DebitDescription = je.CreditDescription;
                je.BranchId = branchId;
                accountRepo.AddJurnalEntry(je);
                LogWriter.WriteLog("Journal Entry is Saved Succesfully");

                LogWriter.WriteLog("Journal Entry Adding Debit Entry Detail");
                int receivableAccountid = SessionHelper.GetFourthLvlConfigurationAccount(branchId, ConstHelper.CAT_FEE_RECEIVABLE, ConstHelper.CAT_RECEIVABLES);
                foreach (IssueChalanDetail detail in detailList)
                {
                    if (detail.TotalAmount > 0)
                    {
                        var tempAccount1 = accountRepo.GetFinanceFifthLvlAccount(detail.FeeHead.Name, receivableAccountid);
                        JournalEntryDebitDetail entryDetail1 = new JournalEntryDebitDetail();
                        entryDetail1.EntryId = je.EntryId;
                        entryDetail1.FifthLvlAccountId = tempAccount1.Id;
                        entryDetail1.Amount = detail.TotalAmount;
                        if (detail.Type == 1)
                            entryDetail1.Description = "Fee Challan Issued, For Head : " + detail.FeeHead.Name + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                        else if (detail.Type == 2)
                            entryDetail1.Description = "Fee Challan Issued, For Arrear Head : " + detail.FeeHead.Name + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                        else
                            entryDetail1.Description = "Fee Challan Issued, For Extra Charges Head : " + detail.FeeHead.Name + ", Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;

                        accountRepo.AddJurnalEntryDebitDetail(entryDetail1);
                        FinanceHelper.UpdateDebitAccountBalance((int)tempAccount1.FourthLvlAccountId, (int)entryDetail1.Amount);
                        FinanceHelper.UpdateDebitFifthAccountBalance((int)entryDetail1.FifthLvlAccountId, (int)entryDetail1.Amount);
                    }
                }


                LogWriter.WriteLog("Journal Entry Adding Credit Entry Detail");
                var tempAccount = accountRepo.GetFinanceFifthLvlAccount(student.id.ToString().PadLeft(6, '0') + "-" + student.Name, receivableAccountid);
                JournalEntryCreditDetail entryDetail = new JournalEntryCreditDetail();
                entryDetail.EntryId = je.EntryId;
                entryDetail.FifthLvlAccountId = tempAccount.Id;
                entryDetail.Amount = je.CreditAmount;
                entryDetail.Description = "Fee Amount is added to Student Receivables account, Against Challan No : " + ChequeNo + ", For Month : " + forMonth + studenInfo;
                accountRepo.AddJurnalEntryCreditDetail(entryDetail);
                FinanceHelper.UpdateCreditAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount, je.EntryId);
                FinanceHelper.UpdateCreditFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);

                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return errorCode;
        }
        

        public void SaveIssueChallanDetail(int issuedChallanId, DateTime paidDate, DateTime previousDate, int FeeBalanceId, int payToType, int PayedTo, int challanId = 0, int studentChallanId = 0, string month = "", int fullPayment = 0, bool isPaid = false)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Saving Issue Challan Detail with ChallanId : " + issuedChallanId);
                List<IssueChalanDetail> detailList = (List<IssueChalanDetail>)Session[ConstHelper.ISSUED_CHALLAN_DETAIL_LIST];

                LogWriter.WriteLog("Full Payment Status : " + fullPayment);
                if (fullPayment == 1)
                {
                    int historyCount = 0;
                    List<IssueChalanDetail> paidList = feePlanRepo.GetIssueChallanDetail(issuedChallanId);
                    LogWriter.WriteLog("Full Payment Paid List Count : " + (paidList == null ? 0 : paidList.Count));
                    LogWriter.WriteLog("Full Payment Adding Issue Challan Detail");
                    foreach (IssueChalanDetail detail in paidList)
                    {
                        PaymentHistory history = new PaymentHistory();
                        history.PaymentType = detail.Type;
                        history.IssueChallanId = issuedChallanId;
                        history.FeeHeadId = detail.FeeHeadId;
                        history.PayAmount = (detail.TotalAmount == null ? 0 : detail.TotalAmount) - (detail.PayAmount == null ? 0 : detail.PayAmount) - (detail.Discount == null ? 0 : detail.Discount);
                        history.CreatedOn = DateTime.Now;
                        history.FeeBalanceId = FeeBalanceId;
                        history.Description = "Amount : " + detail.TotalAmount + " is paid for Student Challan for month (" + month + ") in month (" + month + ")";
                        history.PaidDate = paidDate;
                        history.PayedTo = PayedTo;
                        history.PayedToType = payToType;
                        history.ForMonth = month;
                        history.FeeBalanceId = FeeBalanceId;
                        if (history.PayAmount != 0)
                        {
                            feePlanRepo.AddPaymentHistory(history);
                            historyCount++;
                        }

                        detail.PayAmount = (detail.TotalAmount == null ? 0 : detail.TotalAmount) - (detail.Discount == null ? 0 : detail.Discount);
                        detail.UpdateOn = DateTime.Now;

                        feePlanRepo.UpdateIssueChalanDetail(detail);

                        int arrearAmount = (int)paidList.Where(x => x.FeeHeadId == detail.FeeHeadId && x.Type == 2).Sum(x => (x.TotalAmount - (x.PayAmount == null ? 0 : x.PayAmount) - (x.Discount == null ? 0 : x.Discount)));
                        //int payAmount = (int)detailList.Where(x => x.FeeHeadId == detail.FeeHeadId).Sum(x => x.PayAmount);

                        if (arrearAmount > 0)
                        {
                            FeeArrearsDetail arrear = feePlanRepo.GetFeeArrearDetail(FeeBalanceId, (int)detail.FeeHeadId);
                            if (arrear == null)
                            {
                                arrear = new FeeArrearsDetail();
                                arrear.HeadAmount = arrearAmount;
                                arrear.FeeBalanceId = FeeBalanceId;
                                arrear.FeeHeadId = detail.FeeHeadId;
                                arrear.CreatedOn = DateTime.Now;
                                arrear.UpdatedOn = DateTime.Now;
                                feePlanRepo.SaveFeeArrearDetail(arrear);
                            }
                            else
                            {
                                arrear.HeadAmount = arrear.HeadAmount - arrearAmount;
                                feePlanRepo.UpdateFeeArrearDetail(arrear);
                            }
                        }
                    }

                    LogWriter.WriteLog("Full Payment Updating Issue Challan Amount");
                    feePlanRepo.UpdateIssueChalanPaidAmount(issuedChallanId, payToType, PayedTo);

                    if (historyCount == 0)
                    {
                        var paymentHistory = feePlanRepo.SearchPaymentHistory(0, issuedChallanId, 0, 0);
                        foreach (var history in paymentHistory)
                        {
                            if (history.PaidDate.Value.Date == previousDate.Date)
                            {
                                history.PaidDate = paidDate;
                                feePlanRepo.UpdatePaymentHistory(history);
                            }
                        }
                    }

                }
                else
                {

                    LogWriter.WriteLog("Issue Challan Detail List Count : " + (detailList == null ? 0 : detailList.Count));
                    LogWriter.WriteLog("Issue Challan Adding Issue Challan Detail");
                    if (detailList == null)
                    {
                        var challanDetail = feePlanRepo.GetChallDetailByChallanId(challanId);
                        foreach (ChallanDetailViewModel detail in challanDetail)
                        {
                            if (detail.Amount > 0)
                            {
                                IssueChalanDetail chDetail = new IssueChalanDetail();
                                chDetail.PayAmount = 0;
                                chDetail.IssueChallanId = issuedChallanId;
                                chDetail.FeeHeadId = detail.HeadId;
                                chDetail.TotalAmount = detail.Amount;
                                chDetail.CreatedOn = DateTime.Now;
                                chDetail.UpdateOn = paidDate;
                                chDetail.Discount = 0;
                                chDetail.Type = 1;
                                feePlanRepo.saveIssuedChallanDetail(chDetail);
                            }
                        }

                        LogWriter.WriteLog("Issue Challan Adding Student Arrears");
                        var studentChallan = feePlanRepo.GetStudentChallanDetailById(issuedChallanId);
                        var tempChallan = feePlanRepo.GetPaidChallanByChalanStudentId(studentChallan.Id);
                        if (tempChallan == null)
                        {
                            var arrearDetail = feePlanRepo.GetStudentArrearDetail(studentChallanId).Where(x => x.ArrearAmount > 0).ToList();
                            if (arrearDetail != null && arrearDetail.Count > 0)
                            {
                                foreach (FeeArrearViewModel detail in arrearDetail)
                                {
                                    IssueChalanDetail chDetail = new IssueChalanDetail();
                                    chDetail.PayAmount = 0;
                                    chDetail.IssueChallanId = issuedChallanId;
                                    chDetail.FeeHeadId = detail.FeeHeadId;
                                    chDetail.TotalAmount = detail.ArrearAmount;
                                    chDetail.CreatedOn = DateTime.Now;
                                    chDetail.UpdateOn = paidDate;
                                    chDetail.Type = 2;
                                    feePlanRepo.saveIssuedChallanDetail(chDetail);
                                }
                            }
                        }
                        else
                        {
                            var lastmonthPaidDetail = feePlanRepo.GetLastMonthUnpaidDetail(issuedChallanId);
                            if (lastmonthPaidDetail != null && lastmonthPaidDetail.Count > 0)
                            {
                            }
                            else
                            {
                                List<int> addedList = new List<int>();
                                var arrearHistory = feePlanRepo.GetLastMonthArrears(issuedChallanId, FeeBalanceId);
                                if (arrearHistory != null && arrearHistory.Count > 0)
                                {
                                    foreach (ArreartHistory history in arrearHistory)
                                    {
                                        if (addedList.Count == 0 || addedList.Contains((int)history.FeeHeadId) == false)
                                        {
                                            int payAmount = (int)arrearHistory.Where(x => x.FeeHeadId == history.FeeHeadId).Sum(x => x.PayAmount);
                                            int discount = (int)arrearHistory.Where(x => x.FeeHeadId == history.FeeHeadId).Sum(x => x.Discount == null ? 0 : x.Discount);
                                            int arrearAmount = payAmount - discount;
                                            if (arrearAmount > 0)
                                            {
                                                IssueChalanDetail chDetail = new IssueChalanDetail();
                                                chDetail.PayAmount = 0;
                                                chDetail.IssueChallanId = issuedChallanId;
                                                chDetail.FeeHeadId = history.FeeHeadId;
                                                chDetail.TotalAmount = arrearAmount;
                                                chDetail.CreatedOn = DateTime.Now;
                                                chDetail.UpdateOn = paidDate;
                                                chDetail.Type = 2;
                                                feePlanRepo.saveIssuedChallanDetail(chDetail);
                                                addedList.Add((int)history.FeeHeadId);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        LogWriter.WriteLog("Issue Challan Adding Extra Charges");
                        var extraChargesDetail = feePlanRepo.GetStudentExtraChargesByStudent(studentChallanId, month).Where(x => x.HeadAmount > 0).ToList();
                        if (extraChargesDetail != null && extraChargesDetail.Count > 0)
                        {
                            foreach (StudentExtraChargesDetail detail in extraChargesDetail)
                            {
                                IssueChalanDetail chDetail = new IssueChalanDetail();
                                chDetail.PayAmount = 0;
                                chDetail.IssueChallanId = issuedChallanId;
                                chDetail.FeeHeadId = detail.HeadId;
                                chDetail.TotalAmount = detail.HeadAmount;
                                chDetail.CreatedOn = DateTime.Now;
                                chDetail.UpdateOn = paidDate;
                                chDetail.Type = 3;
                                feePlanRepo.saveIssuedChallanDetail(chDetail);
                            }
                        }
                    }
                    else if (detailList != null && detailList.Count > 0)
                    {
                        LogWriter.WriteLog("Issue Challan Adding Payment History");
                        foreach (IssueChalanDetail detail in detailList)
                        {
                            var temp = feePlanRepo.GetIssuedChallanDetail(issuedChallanId, (int)detail.FeeHeadId, (int)detail.Type);
                            PaymentHistory history = new PaymentHistory();
                            history.PaymentType = detail.Type;
                            history.IssueChallanId = issuedChallanId;
                            history.FeeHeadId = detail.FeeHeadId;
                            history.PayAmount = (detail.PayAmount == null ? 0 : detail.PayAmount);
                            history.CreatedOn = DateTime.Now;
                            history.FeeBalanceId = FeeBalanceId;
                            history.Description = "Amount : " + detail.PayAmount + " is paid for Student Challan for month (" + month + ") in month (" + month + ")";

                            if (temp == null)
                            {
                                detail.IssueChallanId = issuedChallanId;
                                feePlanRepo.saveIssuedChallanDetail(detail);
                            }
                            else
                            {
                                if (temp.PayAmount != detail.PayAmount)
                                {
                                    temp.PayAmount = (detail.PayAmount == null ? 0 : detail.PayAmount);
                                    temp.UpdateOn = paidDate;
                                    feePlanRepo.UpdateIssueChalanDetail(temp);
                                }
                            }

                            if (isPaid)
                            {
                                int arrearAmount = (int)detailList.Where(x => x.FeeHeadId == detail.FeeHeadId).Sum(x => (x.TotalAmount - x.PayAmount - (x.Discount == null ? 0 : x.Discount)));
                                //int payAmount = (int)detailList.Where(x => x.FeeHeadId == detail.FeeHeadId).Sum(x => x.PayAmount);
                                if (arrearAmount > 0)
                                {
                                    FeeArrearsDetail arrear = feePlanRepo.GetFeeArrearDetail(FeeBalanceId, (int)detail.FeeHeadId);
                                    if (arrear == null)
                                    {
                                        arrear = new FeeArrearsDetail();
                                        arrear.HeadAmount = arrearAmount;
                                        arrear.FeeBalanceId = FeeBalanceId;
                                        arrear.FeeHeadId = detail.FeeHeadId;
                                        arrear.CreatedOn = DateTime.Now;
                                        arrear.UpdatedOn = DateTime.Now;
                                        feePlanRepo.SaveFeeArrearDetail(arrear);
                                    }
                                    else
                                    {
                                        arrear.HeadAmount = arrearAmount;
                                        feePlanRepo.UpdateFeeArrearDetail(arrear);
                                    }
                                }
                            }

                            history.PaidDate = paidDate;
                            history.PayedTo = PayedTo;
                            history.PayedToType = payToType;
                            history.ForMonth = month;
                            history.FeeBalanceId = FeeBalanceId;

                            if (detail.PayAmount > 0 && history.Description != null)
                                feePlanRepo.AddPaymentHistory(history);
                        }
                    }

                    LogWriter.WriteLog("Issue Challan Updating Issue Chalan Amount");
                    IssuedChallan tempIssueChallan = feePlanRepo.GetIssueChallanByChalanId(issuedChallanId);
                    tempIssueChallan.Amount = tempIssueChallan.IssueChalanDetails.Sum(x => x.TotalAmount);
                    tempIssueChallan.ChalanAmount = (int)tempIssueChallan.Amount;
                    feePlanRepo.UpdateIssueChallan(tempIssueChallan);
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
        }


        private bool IsFineConfigDefined(int branchId)
        {
            IssueChallanConfig config = feePlanRepo.GetFine(branchId);
            if (config == null)
                return false;
            return true;
        }

        private int GetStudentTotalFine(int id, int month, int year, int branchId, DateTime challanDueDate)
        {
            int actualFine = 0;

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                LogWriter.WriteLog("Calculating the Student Fine, studentId : " + id);
                IssueChallanConfig isueChalConfg = feePlanRepo.GetFine(branchId);
                //DateTime challanDueDate = DateTime.Parse(issueChalan.DueDate.ToString());
                DateTime currentDay = DateTime.Now;
                String diff2 = (currentDay - challanDueDate).TotalDays.ToString();
                //float daysDiffr = float.Parse(diff2, System.Globalization.CultureInfo.InvariantCulture);
                //int FineDays = int.Parse(daysDiffr, System.Globalization.CultureInfo.InvariantCulture);
                string dayExceed = diff2.ToString().Split('.')[0];
                int FineDays = int.Parse(dayExceed);



                if (FineDays != null && FineDays > 0)
                {
                    if (isueChalConfg.Fine != null && isueChalConfg.Fine > 0)
                    {
                        actualFine = int.Parse(isueChalConfg.Fine.ToString());
                    }
                    else if (isueChalConfg.FinePerDay != null && isueChalConfg.FinePerDay > 0)
                    {
                        actualFine = int.Parse(isueChalConfg.FinePerDay.ToString()) * FineDays;
                    }
                }

                DateTime firstDay = new DateTime(year, month, 1);
                DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);
                int nOfAbsents = attRepo.GetStudentAbsents(id, firstDay, lastDay);
                if (nOfAbsents != null && nOfAbsents > 0 && isueChalConfg.AttendanceDays != null && isueChalConfg.AttendanceDays > 0)
                {
                    actualFine = actualFine + (nOfAbsents * int.Parse(isueChalConfg.AttendanceDays.ToString()));
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return actualFine;
        }



    }
}
