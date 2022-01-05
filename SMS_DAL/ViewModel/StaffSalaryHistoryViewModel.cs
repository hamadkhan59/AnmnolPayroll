using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class StaffSalaryHistoryViewModel
    {
        public int StaffId { get; set; }
        public int IncrementId { get; set; }
        public DateTime IncrementDate { get; set; }
        public int BasicSalaryIncrement { get; set; }
        public int AllownceIncrement { get; set; }
        public int Increment { get; set; }
        public string IncrementName { get; set; }
        public string AllownceName { get; set; }
        public bool IsApplied { get; set; }

    }
}
