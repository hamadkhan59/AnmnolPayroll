using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class FeeArrearViewModel
    {
        public int? IssueChallanDetailId { get; set; }
        public int? FeeHeadId { get; set; }
        public int? FeeBalanceId { get; set; }
        public string HeadName { get; set; }
        public int? ArrearAmount { get; set; }
    }
}
