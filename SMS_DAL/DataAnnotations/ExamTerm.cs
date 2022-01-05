using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL
{
    [MetadataType(typeof(ExamTermMetadata))]
    public partial class ExamTerm
    {
        public class ExamTermMetadata
        {
            [Required(ErrorMessage = "Term Name is required")]
            public String TermName { get; set; }
            [Required(ErrorMessage = "Percentage is required")]
            [Range(1, 100, ErrorMessage = "Percentage must be between 1 and 100")]
            public String Percentage { get; set; }
        }
    }
}
