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
    
    public partial class TransportStop
    {
        public TransportStop()
        {
            this.TransportDriverStops = new HashSet<TransportDriverStop>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> Rent { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    
        public virtual ICollection<TransportDriverStop> TransportDriverStops { get; set; }
    }
}
