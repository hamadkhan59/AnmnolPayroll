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
    
    public partial class FinanceThirdLvlAccount
    {
        public FinanceThirdLvlAccount()
        {
            this.FinanceFourthLvlAccounts = new HashSet<FinanceFourthLvlAccount>();
            this.JournalEntries2 = new HashSet<JournalEntry>();
            this.JournalEntries11 = new HashSet<JournalEntry>();
        }
    
        public int Id { get; set; }
        public int SeccondLvlAccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountDescription { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> Value { get; set; }
        public Nullable<int> BranchId { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual ICollection<FinanceFourthLvlAccount> FinanceFourthLvlAccounts { get; set; }
        public virtual FinanceSeccondLvlAccount FinanceSeccondLvlAccount { get; set; }
        public virtual ICollection<JournalEntry> JournalEntries2 { get; set; }
        public virtual ICollection<JournalEntry> JournalEntries11 { get; set; }
    }
}
