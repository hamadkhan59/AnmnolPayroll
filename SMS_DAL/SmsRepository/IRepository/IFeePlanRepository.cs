﻿using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IFeePlanRepository : IDisposable
    {
        int AddFeeHead(FeeHead head);
        FeeHead GetFeeHeadByName(string name, int branchId);
        FeeHead GetFeeHeadByNameAndId(string name, int headId, int branchId);
        void UpdateFeeHead(FeeHead head);
        FeeHead GetFeeHeadById(int headId);
        void DeleteFeeHead(FeeHead head);
        List<FeeHead> GetAllFeeHeads(int branchId);
        List<FeeHead> GetAllFeeHeads();
		List<FeeModel> GetFeeStatus(int id, DateTime fromDate, DateTime curDate);
        List<IssuedChallanViewModel> SearchChallanByStatus(int branchId, int classId, int sectionId, int feeStatus, DateTime fromDate, DateTime to);
        int AddChallan(Challan chalan);
        Challan GetChallanByName(string name, int branchId);
        Challan GetChallanByNameAndId(string name, int challanId, int branchId);
        void UpdateChallan(Challan chalan);
        Challan GetChallanById(int chalanId);
        void DeleteChallan(Challan chalan);
        List<Challan> GetAllChallan();
        List<Challan> GetAllChallanByClassId(int classId);
        int AddChallanDetail(ChallanFeeHeadDetail chalanDetail);
        List<ChallanDetailViewModel> GetChallDetailByClassIdId(int classId);
        List<ChallanDetailViewModel> GetChallDetailByChallanId(int challanId);
        ChallanFeeHeadDetail GetChallDetailById(int Id);
        void UpdateChallanDetail(ChallanFeeHeadDetail chalanDetail);

        void SaveAdmissionCharges(StudentAdmissionCharge charges);
        List<StudentAdmissionCharge> GetStudentAdmissionChargesByStudentId(int id);
        StudentAdmissionCharge GetStudentAdmissionChargesById(int id);
        List<StudentAdmissionCharge> GetPositiveStudentAdmissionChargesByStudentId(int id);
        void UpdateStudentAdmissionCharges(StudentAdmissionCharge charges);

        int AddBankAccount(BankAccount bankAccount);
        BankAccount GetBankAccountByAccountNo(string bankAccount, int branchId);
        BankAccount GetBankAccountByAccountNoAndId(string bankAccount, int accountId, int branchId);
        void UpdateBankAccount(BankAccount bankAccount);
        List<BankAccount> GetAllBankAccount();
        BankAccount GetBankAccountById(int id);
        void DeleteBankAccount(BankAccount bankAccount);

        int AddArrearHistory(ArreartHistory history);
        void DeleteArrearHistory(ArreartHistory history);
        void UpdateArrearHistory(ArreartHistory history);
        ArreartHistory GetArrearHistoryById(int Id);
        List<ArreartHistory> SearchArrearHistory(int FeeBalanceId = 0, int FeeHeadId = 0);
        int AddStudentExtrachargesHistory(ExtraChargesHistory history);
        void DeleteExtraChargesHistory(ExtraChargesHistory history);
        void UpdateExtraChargesHistory(ExtraChargesHistory history);
        ExtraChargesHistory GetExtraChargesHistoryById(int Id);
        List<ExtraChargesHistory> SearchExtraChargesHistory(int StudentId = 0, int FeeHeadId = 0);
        List<ExtraChargesHistory> SearchExtraChargesHistoryForMonth(int StudentId, string currentMonth);
        int AddPaymentHistory(PaymentHistory history);
        int getPaymentHistorySum(int feeBalanceId, DateTime fromDate, DateTime toDate);
        void DeletePaymentHistory(PaymentHistory history);
        void UpdatePaymentHistory(PaymentHistory history);
        PaymentHistory GetPaymentHistoryById(int Id);
        List<PaymentHistory> SearchPaymentHistory(int PaymentType = 0, int IssueChallanId = 0, int FeeBalanceId = 0, int FeeHeadId = 0);

        List<AccountType> GetAllAccountTypes();
        List<FinanceFourthLvlAccount> GetFeeAccountDetails(int branchId);
        List<Month> GetAllMonths();
        List<StudentPerChallan> GetAllStudentPerChallans();
        List<Year> GetAllYears();
        List<ResultType> GetAllResultTypes();
        int GetDefinedFine();
        void UpdateFine(Fine fine);
        //Fine GetFine();
        SchoolConfig GetSchoolConfig();
        int AddFeeBalance(FeeBalance balance);
        void UpdateFeeBalance(FeeBalance balance);
        List<IssuedChallanViewModel> SearchClassPaidChallan(int classId, string rollNumber, string name, string fatherName, string currentMonth, string fatherCnic, int branchId, string AdmissionNo, string ContactNo);

        FeeSubmissionDetails GetMonthlyFeeStats(int branchId, DateTime from, DateTime to, string view = "month");
        FeeSubmissionDetails GetFeeDetailsByStatus(int branchId, DateTime from, DateTime to);
        List<FeeByClassSection> GetFeeDetailsByClassSection(int branchId, int feeStatus, DateTime from, DateTime to, int sectionId = 0);	
		List<IssuedChallanViewModel> SearchPaidChallan(int classSectionId, string rollNumber, string name, string fatherName, string currentMonth, string fatherCnic, int branchId, string AdmissionNo, string ContactNo);
        List<IssuedChallanViewModel> SearchIssueChallan(int classSectionId, string rollNumber, string name, string fatherName, string currentMonth, string fatherCnic, int branchId, string AdmissionNo, string ContactNo);
        List<IssuedChallanViewModel> SearchClassIssueChallan(int classId, string rollNumber, string name, string fatherName, string currentMonth, string fatherCnic, int branchId, string AdmissionNo, string ContactNo);
        List<IssuedChallanViewModel> SearchFeeArrears(int classSectionId, string rollNumber, string name, string fatherName, string fatherCnic);
        List<IssuedChallanViewModel> SearchFeeArrearsByAdmissionNo(string AdmissionNo);
        List<IssuedChallanViewModel> SearchIssueChallanByAdmissionNo(string AdmissionNo);
        int GetChallanDiscount(int issueChallanId, int FeeHeadId);
        List<FeeArrearViewModel> GetStudentArrearDetail(int studentId);
        List<FeeArrearViewModel> GetLastMonthUnPaidFee(int studentChallanId, int challanId);
        List<FeeArrearViewModel> GetLastMonthsUnPaidFee(int studentChallanId, int challanId);
        IssuedChallan GetPreviousIssuedChallan(int challanStudentId, int issueChallanId);
        string GetPreviousIssuedMonth(int issueChallanId);
        string GetPreviousMonth(string month);
        List<IssuedChallanDetailModel> GetPendingChallanDetailModel(int challanStudentId, string forMonth);
        void SaveStudentEtraCharges(StudentExtraCharge extraCharges);
        int GetIssuedChallanCount(string forMonth, int branchId);
        int GetIssueChallanCount(int classSectionId, string currentMonth, int branchId);
        void DeleteStudentEtraCharges(int extraChargesId, int branchId);
        StudentExtraCharge GetStudentExtraCharges(int extraChargesId, int branchId);
        List<StudentExtraChargesViewModel> SearchStudentExtraCharges(string forMonth, int classId, int sectionId, string rollNo, int feeHeadId, int Amount);
        List<StudentExtraChargesDetail> GetStudentExtraChargesByStudent(int studentId);
        List<StudentExtraChargesDetail> GetStudentExtraChargesByStudent(int studentId, string forMonth);
        void SaveFeeArrearDetail(FeeArrearsDetail detail);
        void UpdateFeeArrearDetail(FeeArrearsDetail detail);
        FeeArrearsDetail GetFeeArrearDetail(int feeBalanceId, int headId);
        FeeBalance GetFeeBalanceByStudentId(int studentId);
        FeeBalance GetFeeBalanceById(int feeBalanceId);
        List<AnnualCharge> GetAnnualChargesByStudentIdAndMonth(int studentId, string currentMonth);
        List<StudentModel> SearchStudentForChallan(int classSectionId, string rollNumber, string name, string fatherName, string fatherCnic);
        List<StudentModel> SearchClassStudentForChallan(int classId, string rollNumber, string name, string fatherName, string fatherCnic);
        List<StudentModel> SearchStudentForChallan(string rollNumber, string name, string fatherName, string fatherCnic, int branchId);
        List<StudentModel> SearchNewStudentForChallan(int classSectionId, string rollNumber, string name, string fatherName, string fatherCnic, string forMonth);
        List<StudentModel> SearchClassNewStudentForChallan(int classId, string rollNumber, string name, string fatherName, string fatherCnic, string forMonth);
        List<StudentModel> SearchNewStudentForChallan(string rollNumber, string name, string fatherName, string fatherCnic);
        ChallanStudentDetail GetStudentChallanDetailByStudentId(int studentId);
        void UpdateStudentChallanDetail(ChallanStudentDetail chalanStudentDetail);
        int AddStudentChallanDetail(ChallanStudentDetail chalanStudentDetail);
        int GetChallanAmountByChallanId(int challanId);
        int AddIssueChallan(IssuedChallan issueChallan);
        ChallanStudentDetail GetStudentChallanDetailById(int Id);
        void SaveFeeDiscount(int[] Discount, int[] DetailId);
        MonthDetailModel GetLastMonthSummary();
        AnnualCharge GetAnnualChargesByStudentId(int studentId);
        void DeleteAnnualCharges(AnnualCharge charges);
        IssuedChallan GetIssueChallanByIdAndMonth(int id, string month);
        void DeleteIssueChallan(IssuedChallan challan);
        void DeleteIssueChallanDetail(IssueChalanDetail challanDetail);
        int AddAnnualCharges(AnnualCharge charges);
        void UpdateIssueChallan(IssuedChallan issueChallan);
        void UpdateAnnualCharges(AnnualCharge charges);
        IssuedChallan GetIssueChallanById(int id);
        IssuedChallan GetIssueChallanByChalanId(int id);
        IssuedChallan GetIssueChallanByChalanStudentId(int id);
        IssuedChallan GetPaidChallanByChalanStudentId(int id);
        List<FinanceFourthLvlAccount> GetFeePaidAccounts(int accountType, int financeAccountId);
        List<IssuedChallanViewModel> GetIssueChallanByChallanNo(int chalanNo);
        List<IssuedChallanViewModel> GetPaidChallanByChallanNo(int chalanNo);

        List<IssueChalanDetail> GetIssueChallanDetail(int chalanId);
        void saveIssuedChallanDetail(IssueChalanDetail detail);
        IssueChalanDetail GetIssuedChallanDetail(int chalanId, int feeHeadId, int type);
        IssueChalanDetail GetIssuedChallanDetailById(int issueChalanDetailId);
        void UpdateIssueChalanDetail(IssueChalanDetail detail);
        void UpdateIssueChalanPaidAmount(int issueChallanId, int payToType, int PayedTo);
        IssuedChallanViewModel SearchFastPaidChallan(string rollNumber, string name, string fatherName, string fatherCnic, int branchId, string AdmissionNo, int chalanNo);
        List<IssuedChallanViewModel> GetStudentSixMonthPaymentDetail(int studentChallanId, int issueChallanId);
        IssueChallanConfig GetFine(int branchId);
        void UpdateFineValue(IssueChallanConfig fine);
        void AddFineValue(IssueChallanConfig fine);

        int AddSmsMessage(SmsMessage branch);
        SmsMessage GetMessageById(int messageId);
        List<SmsMessage> GetAllMessages();
        void UpdateSmsMessage(SmsMessage bracnh);
        void DeleteSmsMessage(SmsMessage bracnh);

        int AddSmsHistory(SmsHistory branch);
        void ProceedCurrentMonthAttendance(int Status, DateTime AttandanceDate);
        SmsHistory GetSmsHistoryById(int messageId);
        List<SmsHistory> GetSmsHistoryForAttendace(int staffId, int stdId, DateTime sentDate, DateTime nextDate, int attendanceStatus);
        List<SmsHistory> GetAllSmsHistory();
        void UpdateSmsHistory(SmsHistory bracnh);
        void DeleteSmsHistory(SmsHistory bracnh);

        //int AddSmsVender(SmsVender SmsVender);
        //SmsVender GetSmsVenderById(int SmsVenderId);
        //List<SmsVender> GetAllSmsVender();
        //void UpdateSmsVender(SmsVender SmsVender);
        //void DeleteSmsVender(SmsVender SmsVender);

        SmsVender GetSmsVenderByBranchId(int branchId);
        List<SmsVender> GetAllSmsVenders();
        List<SmsEventParam> GetAllSmsEventParam();

        List<ArreartHistory> GetLastMonthArrears(int issuedChalanId, int FeeBalanceId);
        void setArrearDiscount(int feeHeadId, int discount, int studentId);
        List<IssueChalanDetail> GetLastMonthUnpaidDetail(int issuedChalanId);
        List<SmsModel> GetSMSEvents(int BranchId);
        List<SmsEvent> GetAllSMSEvents();
        SmsEvent GetSmsEventById(int eventId);
        SmsEvent GetSmsEventByName(string smsEventName);
        List<SmsParam> GetSmsParams();
        void UpdateSmsEvent(SmsEvent smsEvent);
        List<IssuedChallanViewModel> SearchIssueChallanForSms(int classSectionId, string rollNumber, string name, string fatherName, string currentMonth, string fatherCnic, int branchId, string AdmissionNo);
        void AddFeeIncrementHistoryDtail(FeeIncrementHistoryDetail detail);
        FeeIncrementHistoryDetail GetLastFeeIncrementHistoryDetail(int studentId);
        FeeIncrementHistoryDetail GetFeeIncrementHistoryDetail(int detailId);
        void UpdateFeeIncreentHistoryDetail(FeeIncrementHistoryDetail detail);
        List<FeeIncrementModel> GetFeeIncrementHistoryDetailList(int studentId);

        int GetHeadChallanDetailCount(int FeeHeadId);
        int GetChallanDetailCount(int ChallanId);
        FeeHead GetAdmissionChargesHead();
        List<IssuedChallanDetailModel> GetMonthlyWaveOffFeeDetail(int studentId);
    }
}
