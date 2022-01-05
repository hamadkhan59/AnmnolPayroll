using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL
{
    [MetadataType(typeof(SubjectMetadata))]
    public partial class Subject
    {
        public class SubjectMetadata
        {
            [Required(ErrorMessage = "Name is required")]
            public String Name { get; set; }
        }
    }
}
