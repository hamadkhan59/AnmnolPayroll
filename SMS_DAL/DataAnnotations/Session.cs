using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL
{
    [MetadataType(typeof(SessionMetadata))]
    public partial class Session
    {
        public class SessionMetadata
        {
            //[Required(ErrorMessage = "Name is required")]
            //public String Name { get; set; }
        }
    }
}
