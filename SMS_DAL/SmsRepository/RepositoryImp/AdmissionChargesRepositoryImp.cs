using SMS_DAL.SmsRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class AdmissionChargesRepositoryImp : IAdmissionChargesRepository
    {
        private SC_WEBEntities2 dbContext1;

        public AdmissionChargesRepositoryImp(SC_WEBEntities2 context)   
        {
            dbContext1 = context;  
        }

        SC_WEBEntities2 dbContext
        {
            get
            {
                if (dbContext1 == null || this.disposed == true)
                    dbContext1 = new SC_WEBEntities2();
                this.disposed = false;
                return dbContext1;
            }
        }

        #region ADMISSION_CHARGES
        public List<StudentAdmissionCharge> GetStudentAdmissionChargesByStudentId(int studentId)
        {
            return dbContext.StudentAdmissionCharges.Where(x => x.StudentId == studentId).ToList();
        }

        public int GetStudentAdmissionChargesSumByStudentId(int studentId)
        {
            return (int)dbContext.StudentAdmissionCharges.Where(x => x.StudentId == studentId).Sum(x => x.Amount);
        }

        public List<SACPaidCatalog> GetPaidStudentAdmissionChargesByStudentId(int studentId)
        {
            return dbContext.SACPaidCatalogs.Where(x => x.StudentId == studentId).ToList();
        }

        public void UpdatePaidAdmissionCharges(SACPaidCatalog paidCharges)
        {
            dbContext.Entry(paidCharges).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public int AddPaidAdmissionCharges(SACPaidCatalog paidCharges)
        {
            int result = -1;
            if (paidCharges != null)
            {
                dbContext.SACPaidCatalogs.Add(paidCharges);
                dbContext.SaveChanges();
                result = paidCharges.Id;
            }

            return result;
        }

        #endregion

        #region FEE_INCREMENT_REGION

        public List<FeeIncrementHistory> GetAllFeeIncrementHistories(int branchId)
        {
            return dbContext.FeeIncrementHistories.Where(x => x.BranchId == branchId).ToList();
        }

        public int AddFeeIncrementHistory(FeeIncrementHistory incrementHistory)
        {
            int result = -1;
            if (incrementHistory != null)
            {
                dbContext.FeeIncrementHistories.Add(incrementHistory);
                dbContext.SaveChanges();
                result = incrementHistory.IncrementId;
            }

            return result;
        }

        public FeeIncrementHistory GetFeeIncrementHistoryById(int incrmentId)
        {
            return dbContext.FeeIncrementHistories.Find(incrmentId);
        }

        public void DeleteFeeIncrementHistory(FeeIncrementHistory feeincrementhistory)
        {
            dbContext.FeeIncrementHistories.Remove(feeincrementhistory);
            dbContext.SaveChanges();
        }

        public void MakeFeeIncrementHistory(string sqlQuery)
        {
            dbContext.Database.SqlQuery<List<string>>(sqlQuery).FirstOrDefault();
        }
        #endregion


        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }  
    }
}
