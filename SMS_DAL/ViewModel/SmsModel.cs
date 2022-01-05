using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class SmsModel
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string EventDetail { get; set; }
        public bool SmsFlag { get; set; }
        public string SmsFlagDescription { get; set; }
        public string TemplateName { get; set; }
        public string TemplateText { get; set; }
    }
}
