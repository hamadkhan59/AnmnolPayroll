using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class PermissionModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int PermissionId { get; set; }
        public string GroupName { get; set; }
        public string ModuleName { get; set; }
        public string SubModuleName { get; set; }
        public bool Granted { get; set; }

    }
}
