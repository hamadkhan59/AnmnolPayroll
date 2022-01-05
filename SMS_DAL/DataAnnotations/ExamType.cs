using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL
{
    [MetadataType(typeof(ExamTypeMetadata))]
    public partial class ExamType
    {
        public class ExamTypeMetadata
        {
            [Required(ErrorMessage = "Name is required")]
            public String Name { get; set; }
            [Required(ErrorMessage = "Percentage is required")]
            [Range(1, 100, ErrorMessage = "Percentage must be between 1 and 100")]
            public String Percent_Of_Total { get; set; }
        }
    }
}
