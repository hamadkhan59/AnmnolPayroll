using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL
{
    [MetadataType(typeof(StudentMetadata))]
    public partial class Student
    {
        public class StudentMetadata
        {
            [Required(ErrorMessage = "Name is required")]
            public String Name { get; set; }

            [Required(ErrorMessage = "Father Name is required")]
            public String FatherName { get; set; }

            [Required(ErrorMessage = "Father CNIC is required")]
            public String FatherCNIC { get; set; }


            [Required(ErrorMessage = "Current Address is required")]
            public String CurrentAddress { get; set; }

            [Required(ErrorMessage = "Mobile No is required")]
            public String Contact_1 { get; set; }

        }
    }
}
