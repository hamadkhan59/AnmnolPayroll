using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_DAL.ViewModel
{
    public class IssuedChallanDetailModel
    {
        public int IssueChallanDetailId { get; set; }
        public int IssueChallanId { get; set; }
        public int FeeHeadId { get; set; }
        public string ForMonth { get; set; }
        public string HeadName { get; set; }
        public string HeadDetail { get; set; }
        public int Discount { get; set; }
        public int TotalAmount { get; set; }
        public int PaidAmount { get; set; }
        public int PendingAmount { get; set; }
    }

    public class MonthlyWaveOffModel
    {
        public StudentModel Student { get; set; }
        public virtual List<IssuedChallanDetailModel> ChallanDetail { get; set; }

    }

    public partial class FinanceFourthLvlAccountModel
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public int ThirdLvlAccountId { get; set; }
        public int SeccondLvlAccountId { get; set; }
        public int FirstLvlAccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountDescription { get; set; }
        public bool IsPettyCashAccount { get; set; }
        public Nullable<int> BranchId { get; set; }
    }

    public partial class FinanceFifthLvlAccountModel
    {
        public int Id { get; set; }
        public int FourthLvlAccountId { get; set; }
        public int ThirdLvlAccountId { get; set; }
        public int SeccondLvlAccountId { get; set; }
        public int Value { get; set; }
        public int FirstLvlAccountId { get; set; }
        public string AccountName { get; set; }
        public string FinanceFourthLvlAccountAccountName { get; set; }
        public string AccountDescription { get; set; }
        public bool IsPettyCashAccount { get; set; }
        public Nullable<int> BranchId { get; set; }
    }

    public partial class FinanceThirdLvlAccountModel
    {
        public int Id { get; set; }
        public int SeccondLvlAccountId { get; set; }
        public int FirstLvlAccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountDescription { get; set; }
        public Nullable<int> BranchId { get; set; }
    }

    public partial class FinanceSeccondLvlAccountModel
    {
        public int Id { get; set; }
        public int FirstLvlAccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountDescription { get; set; }
        public Nullable<int> BranchId { get; set; }
    }
}
