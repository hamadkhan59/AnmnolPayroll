using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class SalarySheetViewModel
    {
        public int StaffId { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Designation { get; set; }
        public int LateIN { get; set; }
        public int EarlyOut { get; set; }
        public int HalfDays { get; set; }
        public int Absents { get; set; }
        public int Presents { get; set; }
        public int BasicSalary { get; set; }
        public int Allownces { get; set; }
        public int Deduction { get; set; }
        public int GrossSalary { get; set; }
    }
}
