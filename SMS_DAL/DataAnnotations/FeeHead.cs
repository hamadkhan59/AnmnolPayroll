using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL
{
    [MetadataType(typeof(FeeHeadMetadata))]
    public partial class FeeHead
    {
        public class FeeHeadMetadata
        {
            [Required(ErrorMessage = "Name is required")]
            public String Name { get; set; }

            [Required(ErrorMessage = "Amount is required")]
            [Range(1, int.MaxValue, ErrorMessage = "Amount must be positive")]
            public int Amount { get; set; }
        }
    }
}
