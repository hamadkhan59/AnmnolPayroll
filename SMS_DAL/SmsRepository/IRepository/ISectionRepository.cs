using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface ISectionRepository : IDisposable
    {
        int AddSection(Section section);
        Section GetSectionById(int sectionId);
        Section GetSectionByName(string sectionName, int branchId);
        Section GetSectionByNameAndId(string sectionName, int sectionId, int branchId);
        void UpdateSection(Section section);
        void DeleteSection(Section section);
        List<Section> GetAllSections();
    }
}
