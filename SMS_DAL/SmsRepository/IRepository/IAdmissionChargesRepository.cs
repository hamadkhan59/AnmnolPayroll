using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IAdmissionChargesRepository : IDisposable
    {
        List<StudentAdmissionCharge> GetStudentAdmissionChargesByStudentId(int studentId);
        int GetStudentAdmissionChargesSumByStudentId(int studentId);
        List<SACPaidCatalog> GetPaidStudentAdmissionChargesByStudentId(int studentId);
        void UpdatePaidAdmissionCharges(SACPaidCatalog paidCharges);
        int AddPaidAdmissionCharges(SACPaidCatalog paidCharges);
        List<FeeIncrementHistory> GetAllFeeIncrementHistories(int branchId);
        int AddFeeIncrementHistory(FeeIncrementHistory incrementHistory);
        FeeIncrementHistory GetFeeIncrementHistoryById(int incrmentId);
        void DeleteFeeIncrementHistory(FeeIncrementHistory feeincrementhistory);
        void MakeFeeIncrementHistory(string sqlQuery);
    }
}
