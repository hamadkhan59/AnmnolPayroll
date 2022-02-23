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
    
    public partial class ItemPurchase
    {
        public ItemPurchase()
        {
            this.ItemPurchaseDetails = new HashSet<ItemPurchaseDetail>();
        }
    
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    
        public virtual Vendor Vendor { get; set; }
        public virtual ICollection<ItemPurchaseDetail> ItemPurchaseDetails { get; set; }
    }
}
