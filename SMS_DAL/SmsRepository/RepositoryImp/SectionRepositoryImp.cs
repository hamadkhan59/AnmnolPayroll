using SMS_DAL.SmsRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class SectionRepositoryImp : ISectionRepository
    {
        private SC_WEBEntities2 dbContext1;

        public SectionRepositoryImp(SC_WEBEntities2 context)   
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

        public int AddSection(Section section)
        {
            int result = -1;
            if (section != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Sections.Add(section);
                dbContext.SaveChanges();
                result = section.Id;
            }

            return result;
        }

        public Section GetSectionById(int sectionId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Sections.Where(x => x.Id == sectionId).FirstOrDefault();
        }

        public Section GetSectionByName(string sectionName, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Sections.Where(x => x.Name == sectionName).FirstOrDefault();
        }

        public Section GetSectionByNameAndId(string sectionName, int sectionId, int branchId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Sections.Where(x => x.Name == sectionName && x.Id != sectionId && x.BranchId == branchId).FirstOrDefault();
        }

        public void UpdateSection(Section section)
        {
            if (section != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(section).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteSection(Section section)
        {
            if (section != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Sections.Remove(section);
                dbContext.SaveChanges();
            }
        }

        public List<Section> GetAllSections()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Sections.ToList();
            //return dbContext.Sections.Select(c => new Section { Name = c.Name, Description = c.Description, IsActive = c.IsActive, Id = c.Id }).ToList();
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
