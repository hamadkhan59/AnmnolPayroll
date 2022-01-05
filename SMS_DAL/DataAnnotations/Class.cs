using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL
{
    [MetadataType(typeof(ClassMetadata))]
    public partial class Class
    {
        public class ClassMetadata
        {
            [Required(ErrorMessage = "Class Name is required")]
            public String Name { get; set; }
        }
    }
}
