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
    
    public partial class GroupPermission
    {
        public int Id { get; set; }
        public Nullable<int> GroupId { get; set; }
        public Nullable<int> PermissionId { get; set; }
        public Nullable<bool> IsGranted { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    
        public virtual Permission Permission { get; set; }
        public virtual UserGroup UserGroup { get; set; }
    }
}