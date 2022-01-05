using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL
{
    [MetadataType(typeof(RegisterCourseMetadata))]
    public partial class RegisterCourse
    {
        public class RegisterCourseMetadata
        {
            [Required(ErrorMessage = "Result Order is required")]
            public int ResultOrder { get; set; }
        }
    }
}
