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
    
    public partial class TransportDriverStop
    {
        public int Id { get; set; }
        public Nullable<int> DriverId { get; set; }
        public Nullable<int> StopId { get; set; }
    
        public virtual TransportStop TransportStop { get; set; }
    }
}
