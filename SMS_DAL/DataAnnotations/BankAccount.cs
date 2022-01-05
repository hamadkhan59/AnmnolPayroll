using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL
{
    [MetadataType(typeof(BankAccountMetadata))]
    public partial class BankAccount
    {
        public class BankAccountMetadata
        {
            [Required(ErrorMessage = "Bank Name is required")]
            public String BankName { get; set; }
            [Required(ErrorMessage = "Account No is required")]
            public String AccountNo { get; set; }
            [Required(ErrorMessage = "Account Title is required")]
            public String AccountTitle { get; set; }
        }
    }
}
