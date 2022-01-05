using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL
{
    [MetadataType(typeof(DesignationCatagoryMetadata))]
    public partial class DesignationCatagory
    {
        public class DesignationCatagoryMetadata
        {
            [Required(ErrorMessage = "Catagory Name is required")]
            public String CatagoryName { get; set; }
        }
    }
}
