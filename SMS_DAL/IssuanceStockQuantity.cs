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
    
    public partial class IssuanceStockQuantity
    {
        public int Id { get; set; }
        public Nullable<int> ItemIssuanceDetailId { get; set; }
        public Nullable<int> ItemReturnDetailId { get; set; }
        public Nullable<int> ItemPurchaseDetailId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    }
}
