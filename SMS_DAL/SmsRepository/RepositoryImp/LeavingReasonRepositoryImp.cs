using SMS_DAL.SmsRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class LeavingReasonRepositoryImp : ILeavingReasonRepository
    {
        private SC_WEBEntities2 dbContext1;

        public LeavingReasonRepositoryImp(SC_WEBEntities2 context)   
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

        public List<LeavingReason> GetAllLeavingReasons()
        {
            return dbContext.LeavingReasons.ToList();    
        }

        public List<LeavingStatu> GetAllLeavingStatus()
        {
            return dbContext.LeavingStatus.ToList();
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
