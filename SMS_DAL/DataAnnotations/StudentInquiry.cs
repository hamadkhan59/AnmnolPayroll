using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL
{
    [MetadataType(typeof(StudentInquiryMetadata))]
    public partial class StudentInquiry
    {
        public class StudentInquiryMetadata
        {
            [Required(ErrorMessage = "Inquiry Name is required")]
            public String Name { get; set; }

            [Required(ErrorMessage = "Father Name is required")]
            public String FatherName { get; set; }

            [Required(ErrorMessage = "Father CNIC is required")]
            public String FatherCNIC { get; set; }

            [Required(ErrorMessage = "Current Address is required")]
            public String CurrentAddress { get; set; }

        }
    }
}
