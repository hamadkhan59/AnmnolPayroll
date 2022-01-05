using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Context = System.Web.HttpContext;

namespace Common
{
    public class SessionManager
    {
        #region Report session handling
        public static void SetReportSession(ReportModel model)
        {
            Context.Current.Session["REPORT_MODEL"] = model;
        }
        public static ReportModel GetReportSession()
        {
            return Context.Current.Session["REPORT_MODEL"] as ReportModel;
        }
        public static void RemoveReportSession()
        {
            Context.Current.Session.Remove("REPORT_MODEL");
        }
        #endregion
    }
}
