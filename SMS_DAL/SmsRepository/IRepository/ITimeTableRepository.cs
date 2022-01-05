using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface ITimeTableRepository : IDisposable
    {
        #region TimeTable
        ScheduleViewModel GetViewModelByBranch(int branchId);
        List<TimeTable> GetByBranch(int id);
        //List<TimeTable> GetCategoriesByBranch(int brachId);
        bool CreateOrUpdate(int brachId, List<TimeTable> objs);
        bool SaveChanges(int branchId, List<LectureSlot> LectureSlots);
        bool Delete(int id);
        #endregion

        
    }
}
