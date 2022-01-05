using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface ILeavingReasonRepository : IDisposable
    {
        List<LeavingReason> GetAllLeavingReasons();
        List<LeavingStatu> GetAllLeavingStatus();
    }
}
