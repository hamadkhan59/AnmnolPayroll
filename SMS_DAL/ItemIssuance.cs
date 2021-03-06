//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SMS_DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class ItemIssuance
    {
        public ItemIssuance()
        {
            this.ItemIssuanceDetails = new HashSet<ItemIssuanceDetail>();
        }
    
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Nullable<int> IssuerId { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<System.DateTime> IssuanceDate { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    
        public virtual Issuer Issuer { get; set; }
        public virtual ICollection<ItemIssuanceDetail> ItemIssuanceDetails { get; set; }
    }
}
