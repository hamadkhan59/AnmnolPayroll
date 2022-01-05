using SMS_DAL.SmsRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class SubjectRepositoryImp : ISubjectRepository
    {
        private SC_WEBEntities2 dbContext1;

        public SubjectRepositoryImp(SC_WEBEntities2 context)   
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

        public int AddSubject(Subject Subject)
        {
            int result = -1;
            if (Subject != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Subjects.Add(Subject);
                dbContext.SaveChanges();
                result = Subject.Id;
            }

            return result;
        }

        public Subject GetSubjectById(int SubjectId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Subjects.Where(x => x.Id == SubjectId).FirstOrDefault();
        }

        public Subject GetSubjectByName(string SubjectName, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Subjects.Where(x => x.Name == SubjectName && x.BranchId == branchId).FirstOrDefault();
        }

        public Subject GetSubjectByNameAndId(string SubjectName, int SubjectId, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Subjects.Where(x => x.Name == SubjectName && x.Id != SubjectId && x.BranchId == branchId).FirstOrDefault();
        }

        public List<Subject> GetAllSubjectes()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Subjects.ToList();    
        }

        public void UpdateSubject(Subject Subject)
        {
            if (Subject != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(Subject).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteSubject(Subject Subject)
        {
            if (Subject != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Subjects.Remove(Subject);
                dbContext.SaveChanges();
            }
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
