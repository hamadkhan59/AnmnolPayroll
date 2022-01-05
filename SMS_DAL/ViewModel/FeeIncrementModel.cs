using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class FeeIncrementModel
    {
        public int IncrementId { get; set; }
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string RollNO { get; set; }
        public string FatherName { get; set; }
        public string Revoked { get; set; }
        public int Amount { get; set; }
        public int Percentage { get; set; }
        public string HeadName { get; set; }
        public string Description { get; set; }
        public DateTime IncrementDate { get; set; }

    }
}
