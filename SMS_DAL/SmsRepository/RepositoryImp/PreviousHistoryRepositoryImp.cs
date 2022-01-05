using SMS_DAL.SmsRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    
    public class PreviousHistoryRepositoryImp : IPreviousHitoryRepository
    {
        private SC_WEBEntities2 dbContext1;

        public PreviousHistoryRepositoryImp(SC_WEBEntities2 context)   
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

        public int AddPreviousHistory(PreviousStudyHistory history)
        {
            int result = -1;

            if (history != null)
            {
                dbContext.PreviousStudyHistories.Add(history);
                dbContext.SaveChanges();
            }
            return result;
        }

        public void DeletePreviousHistory(int studentId)
        {
            List<PreviousStudyHistory> historyList = dbContext.PreviousStudyHistories.Where(x => x.StudentId == studentId).ToList();
            if (historyList != null && historyList.Count > 0)
            {
                foreach (PreviousStudyHistory history in historyList)
                {
                    dbContext.PreviousStudyHistories.Remove(history);
                    dbContext.SaveChanges();
                }
            }
        }

        public List<PreviousStudyHistory> GetAllPreviousHitoryByStudentId(int studentId)
        {
            List<PreviousStudyHistory> historyList = dbContext.PreviousStudyHistories.Where(x => x.StudentId == studentId).ToList();
            return historyList;
        }

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
