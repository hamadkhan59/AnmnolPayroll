using SMS_DAL.SmsRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class ClassRepositoryImp : IClassRepository
    {
        private SC_WEBEntities2 dbContext1;

        public ClassRepositoryImp(SC_WEBEntities2 context)   
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

        public int AddClass(Class clas)
        {
            int result = -1;
            if (clas != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Classes.Add(clas);
                dbContext.SaveChanges();
                result = clas.Id;
            }

            return result;
        }

        public Class GetClassById(int classId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Classes.Where(x => x.Id == classId).FirstOrDefault();
        }

        public Class GetClassByName(string className, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Classes.Where(x => x.Name == className && x.BranchId == branchId).FirstOrDefault();
        }

        public Class GetClassByNameAndId(string className, int classId, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Classes.Where(x => x.Name == className && x.Id != classId && x.BranchId == branchId).FirstOrDefault();
        }

        public List<Class> GetAllClasses()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Classes.ToList();
            //return (List<Class>)dbContext.Classes.Select(c => new
            //{
            //    Id = c.Id,
            //    Name = c.Name,
            //    IsFinanceAccountOpen = c.IsFinanceAccountOpen,
            //    IsActive = c.IsActive,
            //    Description = c.Description}).ToList();    
        }

        public List<Class> GetAllClassesData()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Classes.Select(c => new Class { Id = c.Id, Name = c.Name }).ToList();
        }

        public void UpdateClass(Class clas)
        {
            if (clas != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(clas).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteClass(Class clas)
        {
            if (clas != null)
            {
                dbContext.Classes.Remove(clas);
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
