using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.SmsRepository
{
    public class QueryResult
    {
        public int StaffId { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Designation { get; set; }
        public int Latein { get; set; }
        public int Halfdays { get; set; }
        public int EarlyOut { get; set; }
        public int Presents { get; set; }
        public int Absents { get; set; }
        public int Salary { get; set; }
        public int allownces { get; set; }
        public int DesignationId { get; set; }
    }
}
