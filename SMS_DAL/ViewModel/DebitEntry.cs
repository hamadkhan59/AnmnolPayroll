using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class EntryDetail
    {
        public string Account { get; set; }
        public string SubAccount { get; set; }
        public int CrAmount { get; set; }
        public int DbAmount { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
    }
}
