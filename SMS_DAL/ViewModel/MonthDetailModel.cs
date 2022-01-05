using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class MonthDetailModel
    {
        public string Month { get; set; }
        public int TotalAmount { get; set; }
        public int TotalReceived { get; set; }
        public int TotalPending { get; set; }

    }
}
