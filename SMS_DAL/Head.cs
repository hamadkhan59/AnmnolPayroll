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
    
    public partial class Head
    {
        public int HeadId { get; set; }
        public string HeadName { get; set; }
        public string HeadDescription { get; set; }
        public string IsDeleted { get; set; }
        public Nullable<int> BranchId { get; set; }
    
        public virtual Branch Branch { get; set; }
    }
}