using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class SACPaidCatalogViewModel
    {
        public int Id { get; set; }
        public string Paid { get; set; }
        public string RollNo { get; set; }
        public string Name { get; set; }
        public int AdmissionCharges { get; set; }
        public int ChargesPaid { get; set; }
        public string PaidDate { get; set; }
    }
}
