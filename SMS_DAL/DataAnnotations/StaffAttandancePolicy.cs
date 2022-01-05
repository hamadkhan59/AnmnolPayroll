using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL
{
    [MetadataType(typeof(StaffAttandancePolicyMetadata))]
    public partial class StaffAttandancePolicy
    {
        public class StaffAttandancePolicyMetadata
        {
            [Required(ErrorMessage = "LeaveInYear is required")]
            [Range(0, 30, ErrorMessage = "LeaveInYear must be between 0 and 30")]
            public String LeaveInYear { get; set; }
            [Required(ErrorMessage = "LeaveInMonth is required")]
            [Range(0, 5, ErrorMessage = "LeaveInMonth must be between 0 and 5")]
            public String LeaveInMonth { get; set; }

            [Required(ErrorMessage = "LateInCount is required")]
            [Range(1, 5, ErrorMessage = "LateInCount must be between 1 and 5")]
            public String LateInCount { get; set; }

           
        }
    }
}
