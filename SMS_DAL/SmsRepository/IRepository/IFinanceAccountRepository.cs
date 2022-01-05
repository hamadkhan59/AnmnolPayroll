using SMS_DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SMS_DAL.ViewModel.FinanceDashboardViewModel;

namespace SMS_DAL.SmsRepository.IRepository
{
    public interface IFinanceAccountRepository : IDisposable
    {
        FinanceDashboardViewModel GetFinanceStats(int branchId, DateTime from, DateTime to, string view = "month", int firstLvlId = 0, int secondLvlId = 0, int thirdLvlId = 0, int fourthLvlId = 0, int fifthLvlId = 0);
        FinanceDashboardViewModel GetRevenueExpenseStats(int branchId, DateTime from, DateTime to, string view = "month");
        int AddFinanceFirstLvlAccount(FinanceFirstLvlAccount firstLvlAccount);
        FinanceFirstLvlAccount GetFinanceFirstLvlAccountById(int firstLvlAccountId);
        FinanceFirstLvlAccount GetFinanceFirstLvlAccountByName(string firstLvlAccountName);
        FinanceFirstLvlAccount GetFinanceFirstLvlAccountByNameAndId(string firstLvlAccountName, int firstLvlAccountId);
        List<FinanceFirstLvlAccount> GetAllFinanceFirstLvlAccounts();
        void UpdateFinanceFirstLvlAccount(FinanceFirstLvlAccount firstLvlAccount);
        void DeleteFinanceFirstLvlAccount(FinanceFirstLvlAccount clas);

        int AddFinanceSeccondLvlAccount(FinanceSeccondLvlAccount seccondLvlAccount);
        FinanceSeccondLvlAccount GetFinanceSeccondLvlAccountById(int seccondLvlAccountId);
        FinanceSeccondLvlAccount GetFinanceSeccondLvlAccountByName(string seccondLvlAccountName);
        FinanceSeccondLvlAccount GetFinanceSeccondLvlAccountByNameAndId(string seccondLvlAccountName, int seccondLvlAccountId);
        List<FinanceSeccondLvlAccount> GetAllFinanceSeccondLvlAccounts();
        List<FinanceSeccondLvlAccountModel> GetAllFinanceSeccondLvlAccountsModel();
        void UpdateFinanceSeccondLvlAccount(FinanceSeccondLvlAccount firstLvlAccount);
        void DeleteFinanceSeccondLvlAccount(FinanceSeccondLvlAccount clas);

        int AddFinanceThirdLvlAccount(FinanceThirdLvlAccount thirdLvlAccount);
        FinanceThirdLvlAccount GetFinanceThirdLvlAccountById(int thirdLvlAccountId);
        FinanceThirdLvlAccount GetFinanceThirdLvlAccountByName(string thirdLvlAccountName, int branchId);
        FinanceThirdLvlAccount GetFinanceThirdLvlAccountByNameAndId(string thirdLvlAccountName, int thirdLvlAccountId, int branchId);
        List<FinanceThirdLvlAccount> GetAllFinanceThirdLvlAccounts();
        List<FinanceThirdLvlAccountModel> GetAllFinanceThirdLvlAccountsModel();
        void UpdateFinanceThirdLvlAccount(FinanceThirdLvlAccount thirdLvlAccount);
        void DeleteFinanceThirdLvlAccount(FinanceThirdLvlAccount thirdLvlAccount);

        int AddFinanceFourthLvlAccount(FinanceFourthLvlAccount fourthLvlAccount);
        FinanceFourthLvlAccount GetFinanceFourthLvlAccountById(int fourthLvlAccountId);
        FinanceFourthLvlAccount GetFinanceFourthLvlAccountByName(string fourthLvlAccountName, int branchId);
        FinanceFourthLvlAccount GetFinanceFourthLvlAccountByNameAndId(string fourthLvlAccountName, int fourthLvlAccountId, int branchId);
        List<FinanceFourthLvlAccount> GetAllFinanceFourthLvlAccounts();
        List<FinanceFourthLvlAccountModel> GetAllFinanceFourthLvlAccountsModel();
        void UpdateFinanceFourthLvlAccount(FinanceFourthLvlAccount fourthLvlAccount);
        void DeleteFinanceFourthLvlAccount(FinanceFourthLvlAccount fourthLvlAccount);

        int AddFinanceFifthLvlAccount(FinanceFifthLvlAccount fifthLvlAccount);
        FinanceFifthLvlAccount GetFinanceFifthLvlAccountById(int fifthLvlAccountId);
        FinanceFifthLvlAccount GetFinanceFifthLvlAccount(string fifthLvlAccountName, int fourthLvlId);
        FinanceFifthLvlAccount GetFinanceFifthLvlAccountByName(string fifthLvlAccountName, int branchId);
        FinanceFifthLvlAccount GetPettyCashFinanceFifthLvlAccountByName(string fifthLvlAccountName, int branchId);
        FinanceFifthLvlAccount GetFinanceFifthLvlAccountByNameAndId(string fifthLvlAccountName, int fifthLvlAccountId, int branchId);
        FinanceFifthLvlAccount GetPettCashFinanceFifthLvlAccountByNameAndId(string fifthLvlAccountName, int fifthLvlAccountId, int branchId);
        List<FinanceFifthLvlAccount> GetAllFinanceFifthLvlAccounts();
        List<FinanceFifthLvlAccountModel> GetAllFinanceFifthLvlAccountsModel();
        void UpdateFinanceFifthLvlAccount(FinanceFifthLvlAccount fifthLvlAccount);
        void DeleteFinanceFifthLvlAccount(FinanceFifthLvlAccount fifthLvlAccount);
        int GetEntriesCount(int FinanceAccountId);
        int GetIssueChallanCount(int FinanceAccountId);
        int GetFinanceSeccondLvlCount(int FinanceAccountId);
        int GetFinanceThirdLvlCount(int FinanceAccountId);
        int GetFinanceFourthLvlCount(int FinanceAccountId);
        int GetFinanceFifthLvlCount(int FinanceAccountId);

        int AddJurnalEntry(JournalEntry journalEntry);
        int UpdateJurnalEntry(JournalEntry journalEntry);

        int AddJurnalEntryDebitDetail(JournalEntryDebitDetail journalEntryDebitDetail);
        IList<JournalEntryDebitDetail> GetJounralEntryDebitDetail(int entryId);
        void DeleteJounralEntryDebitDetail(JournalEntryDebitDetail journalEntryDebitDetail);
        int AddJurnalEntryCreditDetail(JournalEntryCreditDetail journalEntryCreditDetail);
        IList<JournalEntryCreditDetail> GetJounralEntryCreditDetail(int entryId);
        void DeleteJounralEntryCreditDetail(JournalEntryCreditDetail journalEntryCreditDetail);
        void DeleteJournalEntry(JournalEntry entry);
        List<FinanceMode> GetAllFinanceModes();
        List<FinanceFifthLvlAccount> GetFeeFinanceAccounts(int branchId);
        List<FinanceFifthLvlAccountModel> GetFeeFinanceAccountsModel(int branchId);
        IList<JournalEntry> SearchJournalEntries(DateTime FromDate, DateTime ToDate, int FinanceFirstLvlAccount, int FinanceSeccondLvlAccount,
            int FinanceThirdLvlAccount, int FinanceFourthLvlAccount, int FinanceFifthLvlAccount, int FinanceMode, int branchId);

        IList<JournalEntry> SearchPettyCashJournalEntries(DateTime FromDate, DateTime ToDate, int FinanceFirstLvlAccount,
            int FinanceSeccondLvlAccount, int FinanceThirdLvlAccount, int FinanceFourthLvlAccount, int FinanceFifthLvlAccount, int FinanceMode, int branchId);
        IList<JournalEntry> SearchJournalVoucher(DateTime FromDate, DateTime ToDate, int FinanceFirstLvlAccount,
            int FinanceSeccondLvlAccount, int FinanceThirdLvlAccount, int FinanceFourthLvlAccount, int FinanceFifthLvlAccount, int JvNo, int branchId);

        int GetMaxVouxherId();

        JournalEntry GetJournalEntry(int entryId);
        JournalEntryModel GetJournalEntryModel(int entryId);
        List<FinanceFourthLvlAccount> GetFinanceAccounts(string accountName, int branchId);
        List<FinanceFifthLvlAccount> GetFinanceAccountsFifthLvl(string accountName, int branchId, int level = 3);
        List<FinanceFifthLvlAccount> GetPettyCashFinanceAccountsFifthLvl(string accountName, int branchId, int level = 3);
        List<FinanceFifthLvlAccount> GetFinanceAccountsFifthLvl(string accountName);
        List<FinanceFifthLvlAccountModel> GetFinanceAccountsFifthLvlModel(string accountName);
        void CreateJournalVoucher(JournalVoucher voucher);
        void UpdateJournalVoucher(JournalVoucher voucher);
        JournalVoucher GetJournalVoucher(int entryId);
    }
}
