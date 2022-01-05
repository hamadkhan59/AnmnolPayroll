using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class FinanceDashboardViewModel
    {
        public List<string> Months { get; set; }
        public List<DateTime> Days { get; set; }
        public List<decimal> CreditCash { get; set; }
        public List<decimal> CreditCheque { get; set; }
        public List<decimal> DebitCash { get; set; }
        public List<decimal> DebitCheque { get; set; }
        public List<decimal> Expenses { get; set; }
        public List<decimal> Revenue { get; set; }

        public FinanceDashboardViewModel()
        {
            Months = new List<string>();
            Days = new List<DateTime>();
            CreditCash = new List<decimal>();
            CreditCheque = new List<decimal>();
            DebitCash = new List<decimal>();
            DebitCheque = new List<decimal>();
            Expenses = new List<decimal>();
            Revenue = new List<decimal>();
        }
    }

    public class JournalEntryModel
    {
        public int EntryId { get; set; }
        public Nullable<int> DebitAmount { get; set; }
        public Nullable<int> CreditAmount { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreditDescription { get; set; }
        public string DebitDescription { get; set; }
        public string IsDeleted { get; set; }
        public string ChequeNo { get; set; }
        public string EntryType { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string LongDescription { get; set; }
        public Nullable<bool> SystemGenerated { get; set; }

        public virtual ICollection<JournalEntryDetailModel> JournalEntryCreditDetails { get; set; }
        public virtual ICollection<JournalEntryDetailModel> JournalEntryDebitDetails { get; set; }
    }

    public class JournalEntryDetailModel
    {
        public int ID { get; set; }
        public Nullable<int> EntryId { get; set; }
        public Nullable<int> FifthLvlAccountId { get; set; }
        public Nullable<int> FourthLvlAccountId { get; set; }
        public Nullable<int> ThirdLvlAccountId { get; set; }
        public Nullable<int> SeccondLvlAccountId { get; set; }
        public Nullable<int> FirstLvlAccountId { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<int> Count { get; set; }
        public string Description { get; set; }
        public string FifthLvlLvlAccountName { get; set; }
        public string FourthLvlLvlAccountName { get; set; }
        public string ThirdLvlLvlAccountName { get; set; }
        public string SeccondLvlLvlAccountName { get; set; }
        public string FirstLvlLvlAccountName { get; set; }
    }
}
