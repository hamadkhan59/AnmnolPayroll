using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IStoreRepository : IDisposable
    {
        int AddVendor(Vendor vendor);

        Vendor GetVendorByName(string vendorName);
        Vendor GetVendorByNameAndId(string vendorName, int id);
        void UpdateVendor(Vendor vendor);
        Vendor GetVendorById(int vendorId);
        void DeleteVendor(Vendor vendor);
        List<Vendor> GetAllVendors();
        List<Vendor> GetAllVendorsData();
    }
}
