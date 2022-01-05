using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IPreviousHitoryRepository : IDisposable 
    {
        int AddPreviousHistory(PreviousStudyHistory history);

        void DeletePreviousHistory(int studentId);

        List<PreviousStudyHistory> GetAllPreviousHitoryByStudentId(int studentId);
    }
}
