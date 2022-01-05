using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class SessionModel
    {
        public int BRANCH_ID { get; set; }
        public int? STAFF_ID { get; set; }
        public string USER_MODULE_PERMISSIONS { get; set; }
        public string USER_SUB_MODULE_PERMISSIONS { get; set; }
        public string USER_NAME { get; set; }
        public SessionUser SESSION_USER { get; set; }
    }
}
