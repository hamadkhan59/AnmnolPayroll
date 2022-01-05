using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IClassRepository : IDisposable
    {
        int AddClass(Class clas);
        Class GetClassByName(string className, int branchId);
        Class GetClassByNameAndId(string className, int classId, int branchId);
        void UpdateClass(Class clas);
        Class GetClassById(int classId);
        void DeleteClass(Class clas);
        List<Class> GetAllClasses();

        List<Class> GetAllClassesData();
    }
}
