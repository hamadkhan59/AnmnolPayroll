using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMSApi.Models
{
    public class FeeModel
    {
        public string paidMonth { get; set; }
        public decimal? paidAmount { get; set; }
        public DateTime? date { get; set; }
        public int stdId { get; set; }

    }
}
