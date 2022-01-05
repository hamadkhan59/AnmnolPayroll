using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class ChallanDetailViewModel
    {
        public int Id { get; set; }
        public int HeadId { get; set; }
        public int ChallanId { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public int Type { get; set; }
        public int StandardAmount { get; set; }
    }
}
