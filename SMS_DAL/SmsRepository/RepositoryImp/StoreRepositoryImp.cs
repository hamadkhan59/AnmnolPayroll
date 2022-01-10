using SMS_DAL.SmsRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.RepositoryImp
{
    public class StoreRepositoryImp : IStoreRepository
    {
        private SC_WEBEntities2 dbContext1;

        public StoreRepositoryImp(SC_WEBEntities2 context)   
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

        public int AddVendor(Vendor vendor)
        {
            int result = -1;
            if (vendor != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Vendors.Add(vendor);
                dbContext.SaveChanges();
                result = vendor.Id;
            }

            return result;
        }

        public Vendor GetVendorByName(string vendorName)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Vendors.Where(x => x.Name == vendorName).FirstOrDefault();
        }

        public Vendor GetVendorByNameAndId(string vendorName, int id)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Vendors.Where(x => x.Name == vendorName && x.Id != id).FirstOrDefault();
        }

        public void UpdateVendor(Vendor vendor)
        {
            if (vendor != null)
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Entry(vendor).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public Vendor GetVendorById(int vendorId)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Vendors.Where(x => x.Id == vendorId).FirstOrDefault();
        }

        public void DeleteVendor(Vendor vendor)
        {
            if (vendor != null)
            {
                dbContext.Vendors.Remove(vendor);
                dbContext.SaveChanges();
            }
        }

        public List<Vendor> GetAllVendors()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Vendors.ToList();
        }

        public List<Vendor> GetAllVendorsData()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            return dbContext.Vendors.Select(c => new Vendor { Id = c.Id, Name = c.Name }).ToList();
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
