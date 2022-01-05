using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class StudentExtraChargesViewModel
    {
        public int Id { get; set; }
        public string HeadName { get; set; }
        public int ? HeadAmount { get; set; }
        public string Criteria { get; set; }
        public string Descroption { get; set; }
        public string ForMonth { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
