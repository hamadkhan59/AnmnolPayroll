using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS_Web.Helpers
{
    public class FinanceLevel
    {
        public string name { get; set; }
        public int id { get; set; }
    }
    public class FinanceLevelList
    {
        public List<FinanceLevel> list { get; set; }
    }
}