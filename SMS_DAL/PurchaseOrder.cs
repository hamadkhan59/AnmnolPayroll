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
    
    public partial class PurchaseOrder
    {
        public int OrderId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<int> PaidStatus { get; set; }
        public Nullable<System.DateTime> PaidDate { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
    }
}