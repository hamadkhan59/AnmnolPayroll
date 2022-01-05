using Logger;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using SMS_Web.Controllers.SecurityAssurance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SMS_Web.Helpers
{
    public class FinanceHelper
    {
        private static IFinanceAccountRepository accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
        //
        // GET: /Class/

        public static string AddEntryDebitDetail(int entryId, List<EntryDetail> debitEntryInfo, string entryType, int branchId)
        {
            string debitDescription = GetEntryTypeDescription(entryType, 1);
            string creditDescription = GetEntryTypeDescription(entryType, 2);
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);

                DeleteEntryDebitDetail(entryId);
                foreach (EntryDetail entry in debitEntryInfo)
                {
                    var tempAccount = accountRepo.GetFinanceFifthLvlAccountByName(entry.Account, branchId);
                    string description = GetEntryDetailDescription(entry, debitEntryInfo, entryType);
                    if (entry.DbAmount > 0)
                    {
                        JournalEntryDebitDetail entryDetail = new JournalEntryDebitDetail();
                        entryDetail.EntryId = entryId;
                        entryDetail.FifthLvlAccountId = tempAccount.Id;
                        entryDetail.Amount = entry.DbAmount;
                        entryDetail.Description = description;
                        accountRepo.AddJurnalEntryDebitDetail(entryDetail);
                        UpdateDebitAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount);
                        UpdateDebitFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);
                        debitDescription += GetEntryDescription(entry);
                    }
                    else
                    {
                        JournalEntryCreditDetail entryDetail = new JournalEntryCreditDetail();
                        entryDetail.EntryId = entryId;
                        entryDetail.FifthLvlAccountId = tempAccount.Id;
                        entryDetail.Amount = entry.CrAmount;
                        entryDetail.Description = description;
                        accountRepo.AddJurnalEntryCreditDetail(entryDetail);
                        UpdateCreditAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount, entryId);
                        UpdateCreditFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);
                        creditDescription += GetEntryDescription(entry);
                    }
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return debitDescription + creditDescription;
        }

        public static string AddPettyCashEntryDebitDetail(int entryId, List<EntryDetail> debitEntryInfo, string entryType, int branchId)
        {
            string debitDescription = GetEntryTypeDescription(entryType, 1);
            string creditDescription = GetEntryTypeDescription(entryType, 2);
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);

                DeleteEntryDebitDetail(entryId);
                foreach (EntryDetail entry in debitEntryInfo)
                {
                    var tempAccount = accountRepo.GetPettyCashFinanceFifthLvlAccountByName(entry.Account, branchId);
                    string description = GetEntryDetailDescription(entry, debitEntryInfo, entryType);
                    if (entry.DbAmount > 0)
                    {
                        JournalEntryDebitDetail entryDetail = new JournalEntryDebitDetail();
                        entryDetail.EntryId = entryId;
                        entryDetail.FifthLvlAccountId = tempAccount.Id;
                        entryDetail.Amount = entry.DbAmount;
                        entryDetail.Description = description;
                        accountRepo.AddJurnalEntryDebitDetail(entryDetail);
                        UpdateDebitAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount);
                        UpdateDebitFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);
                        debitDescription += GetEntryDescription(entry);
                    }
                    else
                    {
                        JournalEntryCreditDetail entryDetail = new JournalEntryCreditDetail();
                        entryDetail.EntryId = entryId;
                        entryDetail.FifthLvlAccountId = tempAccount.Id;
                        entryDetail.Amount = entry.CrAmount;
                        entryDetail.Description = description;
                        accountRepo.AddJurnalEntryCreditDetail(entryDetail);
                        UpdateCreditAccountBalance((int)tempAccount.FourthLvlAccountId, (int)entryDetail.Amount, entryId);
                        UpdateCreditFifthAccountBalance((int)entryDetail.FifthLvlAccountId, (int)entryDetail.Amount);
                        creditDescription += GetEntryDescription(entry);
                    }
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return debitDescription + creditDescription;
        }

        private static string GetEntryDetailDescription(EntryDetail entry, List<EntryDetail> debitEntryInfo, string entryType)
        {
            string description = entry.Description;
            if (string.IsNullOrEmpty(description))
            {
                if (entry.DbAmount > 0)
                {
                    description = GetEntryTypeDetailDescription(entryType, 1);
                    description = string.Format(description, entry.DbAmount, entry.Account, GetEntryDescription(debitEntryInfo, 2));
                }
                else
                {
                    description = GetEntryTypeDetailDescription(entryType, 2);
                    description = string.Format(description, entry.CrAmount, entry.Account, GetEntryDescription(debitEntryInfo, 1));
                }
            }

            return description;
        }

        private static string GetEntryDescription(List<EntryDetail> debitEntryInfo, int isDebit)
        {
            string description = "";
            List<EntryDetail> tempList = null;
            if (isDebit == 1) // if detail is debit, isDebit == 1
            {
                tempList = debitEntryInfo.Where(x => x.DbAmount > 0).ToList();
            }
            else // if detail is credit, isDebit == 2
            {
                tempList = debitEntryInfo.Where(x => x.CrAmount > 0).ToList();
            }

            if (tempList != null && tempList.Count > 0)
            {
                if (isDebit == 1) // if detail is debit, isDebit == 1
                {
                    foreach (var entry in tempList)
                    {
                        description += "(" + entry.Account + ", " + entry.DbAmount + ") ";
                    }
                }
                else // if detail is credit, isDebit == 2
                {
                    foreach (var entry in tempList)
                    {
                        description += "(" + entry.Account + ", " + entry.CrAmount + ") ";
                    }
                }
            }
            return description;
        }

        private static string GetEntryDescription(EntryDetail entry)
        {
            string description = "";
            if (entry.DbAmount > 0)
            {
                description = "(" + entry.Account + ", " + entry.DbAmount + ") ";
            }
            else
            {
                description = "(" + entry.Account + ", " + entry.CrAmount + ") ";

            }
            return description;
        }

        private static string GetEntryTypeDescription(string entryType, int isDebit)
        {
            string description = "";
            if (isDebit == 1) // if detail is debit, isDebit == 1
            {
                if (entryType.Equals(ConstHelper.ET_JE) || entryType.Equals(ConstHelper.ET_BPV) || entryType.Equals(ConstHelper.ET_CPV))
                    description = "Payment of ";
                else if (entryType.Equals(ConstHelper.ET_BRV) || entryType.Equals(ConstHelper.ET_CRV))
                    description = "Recevied Payment in ";
            }
            else // if detail is credit, isDebit == 2
            {
                if (entryType.Equals(ConstHelper.ET_JE) || entryType.Equals(ConstHelper.ET_BPV) || entryType.Equals(ConstHelper.ET_CPV))
                    description = " is Paid From : ";
                else if (entryType.Equals(ConstHelper.ET_BRV) || entryType.Equals(ConstHelper.ET_CRV))
                    description = " From : ";
            }
            return description;
        }

        private static string GetEntryTypeDetailDescription(string entryType, int isDebit)
        {
            string description = "";

            if (isDebit == 1) // if detail is debit, isDebit == 1
            {
                if (entryType.Equals(ConstHelper.ET_JE) || entryType.Equals(ConstHelper.ET_BPV) || entryType.Equals(ConstHelper.ET_CPV))
                    description = "Payment of ({0}) is Paid For : {1} From : {2}";
                else if (entryType.Equals(ConstHelper.ET_BRV) || entryType.Equals(ConstHelper.ET_CRV))
                    description = "Payment of ({0}) is Received For : {1} From : {2}";
            }
            else // if detail is credit, isDebit == 2
            {
                if (entryType.Equals(ConstHelper.ET_JE) || entryType.Equals(ConstHelper.ET_BPV) || entryType.Equals(ConstHelper.ET_CPV))
                    description = "Payment of ({0}) is Paid From : {1} For : {2}";
                else if (entryType.Equals(ConstHelper.ET_BRV) || entryType.Equals(ConstHelper.ET_CRV))
                    description = "Payment of ({0}) is Received From : {1} Against : {2}";
            }

            return description;
        }

        public static void DeleteEntryDebitDetail(int entryId)
        {
            var debitEntryList = accountRepo.GetJounralEntryDebitDetail(entryId);
            foreach (JournalEntryDebitDetail detail in debitEntryList)
            {
                ResetDebitFifthAccountBalance((int)detail.FifthLvlAccountId, (int) detail.Amount);
                accountRepo.DeleteJounralEntryDebitDetail(detail);
            }
            var creditEntryList = accountRepo.GetJounralEntryCreditDetail(entryId);
            foreach (JournalEntryCreditDetail detail in creditEntryList)
            {
                ResetCreditFifthAccountBalance((int)detail.FifthLvlAccountId, (int)detail.Amount);
                accountRepo.DeleteJounralEntryCreditDetail(detail);
            }
        }

        public static void UpdateDebitAccountBalance(int financeAccountId, int entryAmount)
        {
            //FinanceFourthLvlAccount financeAccount = accountRepo.GetFinanceFourthLvlAccountById(financeAccountId);
            //int accountBalance = (int)(financeAccount.Value == null ? 0 : financeAccount.Value);
            //if (IsFinancePaybleAccount(financeAccount) == true || IsFinanceCapitalAccount(financeAccount) == true)
            //{
            //    accountBalance = accountBalance - entryAmount;
            //}
            //else
            //    accountBalance = accountBalance + entryAmount;
            //financeAccount.Value = accountBalance;
            //accountRepo.UpdateFinanceFourthLvlAccount(financeAccount);
        }

        public static void UpdateCreditAccountBalance(int financeAccountId, int entryAmount, int journalEntryId)
        {
            //FinanceFourthLvlAccount financeAccount = accountRepo.GetFinanceFourthLvlAccountById(financeAccountId);
            //int accountBalance = (int)(financeAccount.Value == null ? 0 : financeAccount.Value);
            //if (IsFinancePaybleAccount(financeAccount) == true || IsFinanceLiableAccount(financeAccount) == true
            //    || IsFinanceCapitalAccount(financeAccount) == true)
            //{
            //    accountBalance = accountBalance + entryAmount;
            //    CreateJournalVoucher(journalEntryId);
            //}
            //else
            //    accountBalance = accountBalance - entryAmount;
            //financeAccount.Value = accountBalance;
            //accountRepo.UpdateFinanceFourthLvlAccount(financeAccount);
        }

        public static void CreateJournalVoucher(int journalEntryId)
        {
            JournalVoucher jv = new JournalVoucher();
            jv.EntryId = journalEntryId;
            jv.IsCleared = 0;
            jv.CreatedOn = DateTime.Now;
        }
        public static void UpdateDebitFifthAccountBalance(int financeAccountId, int entryAmount)
        {
            //FinanceFifthLvlAccount financeAccount = accountRepo.GetFinanceFifthLvlAccountById(financeAccountId);
            //int accountBalance = (int)(financeAccount.Value == null ? 0 : financeAccount.Value);
            //if (IsFifthPaybleAccount(financeAccount) == true || IsFifthCapitalAccount(financeAccount) == true)
            //{
            //    accountBalance = accountBalance - entryAmount;
            //}
            //else
            //    accountBalance = accountBalance + entryAmount;
            //financeAccount.Value = accountBalance;
            //accountRepo.UpdateFinanceFifthLvlAccount(financeAccount);
        }

        public static void UpdateCreditFifthAccountBalance(int financeAccountId, int entryAmount)
        {
            //FinanceFifthLvlAccount financeAccount = accountRepo.GetFinanceFifthLvlAccountById(financeAccountId);
            //int accountBalance = (int)(financeAccount.Value == null ? 0 : financeAccount.Value);
            //if (IsFifthLiableAccount(financeAccount) == true || IsFifthPaybleAccount(financeAccount) == true || IsFifthCapitalAccount(financeAccount) == true)
            //{
            //    accountBalance = accountBalance + entryAmount;
            //}
            //else
            //    accountBalance = accountBalance - entryAmount;
            //financeAccount.Value = accountBalance;
            //accountRepo.UpdateFinanceFifthLvlAccount(financeAccount);
        }

        public static int ResetDebitAccountBalance(int financeAccountId, int entryAmount)
        {
            int errorCode = 0;
            FinanceFourthLvlAccount financeAccount = accountRepo.GetFinanceFourthLvlAccountById(financeAccountId);
            int accountBalance = (int)(financeAccount.Value == null ? 0 : financeAccount.Value);
            if (IsFinancePaybleAccount(financeAccount) == true || IsFinanceCapitalAccount(financeAccount) == true)
            {
                accountBalance = accountBalance + entryAmount;
            }
            else
                accountBalance = accountBalance - entryAmount;
            if (accountBalance > 0)
            {
                financeAccount.Value = accountBalance;
                accountRepo.UpdateFinanceFourthLvlAccount(financeAccount);
            }
            else
                errorCode = 420;
            return errorCode;
        }

        public static int ResetCreditAccountBalance(int financeAccountId, int entryAmount)
        {
            int errorCode = 0;
            FinanceFourthLvlAccount financeAccount = accountRepo.GetFinanceFourthLvlAccountById(financeAccountId);
            int accountBalance = (int)(financeAccount.Value == null ? 0 : financeAccount.Value);
            if (IsFinancePaybleAccount(financeAccount) == true || IsFinanceLiableAccount(financeAccount) == true
                || IsFinanceCapitalAccount(financeAccount) == true)
            {
                accountBalance = accountBalance - entryAmount;
            }
            else
                accountBalance = accountBalance + entryAmount;

            if (accountBalance > 0)
            {
                financeAccount.Value = accountBalance;
                accountRepo.UpdateFinanceFourthLvlAccount(financeAccount);
            }
            else
                errorCode = 420;
            return errorCode;
        }

        public static int ResetDebitFifthAccountBalance(int financeAccountId, int entryAmount)
        {
            int errorCode = 0;
            FinanceFifthLvlAccount financeAccount = accountRepo.GetFinanceFifthLvlAccountById(financeAccountId);
            int accountBalance = (int)(financeAccount.Value == null ? 0 : financeAccount.Value);
            if (IsFifthPaybleAccount(financeAccount) == true || IsFifthCapitalAccount(financeAccount) == true)
            {
                accountBalance = accountBalance + entryAmount;
            }
            else
                accountBalance = accountBalance - entryAmount;

            //if (accountBalance > 0)
            //{
                financeAccount.Value = accountBalance;
                accountRepo.UpdateFinanceFifthLvlAccount(financeAccount);
            //}
            //else
            //    errorCode = 420;
            return errorCode;
        }

        public static int ResetCreditFifthAccountBalance(int financeAccountId, int entryAmount)
        {
            int errorCode = 0;
            FinanceFifthLvlAccount financeAccount = accountRepo.GetFinanceFifthLvlAccountById(financeAccountId);
            int accountBalance = (int)(financeAccount.Value == null ? 0 : financeAccount.Value);
            if (IsFifthLiableAccount(financeAccount) == true || IsFifthPaybleAccount(financeAccount) == true || IsFifthCapitalAccount(financeAccount) == true)
            {
                accountBalance = accountBalance - entryAmount;
            }
            else
                accountBalance = accountBalance + entryAmount;

            //if (accountBalance > 0)
            //{
                financeAccount.Value = accountBalance;
                accountRepo.UpdateFinanceFifthLvlAccount(financeAccount);
            //}
            //else
            //    errorCode = 420;
            return errorCode;
        }

        private static bool IsFinancePaybleAccount(FinanceFourthLvlAccount financeAccount)
        {
            bool flag = false;
            var thirdLvlaccount = accountRepo.GetFinanceThirdLvlAccountById(financeAccount.ThirdLvlAccountId);
            var secondLvlaccount = accountRepo.GetFinanceSeccondLvlAccountById(thirdLvlaccount.SeccondLvlAccountId);
            var firstLvlaccount = accountRepo.GetFinanceFirstLvlAccountById(secondLvlaccount.FirstLvlAccountId);
            string firstAccount = firstLvlaccount.AccountName;
            string seccondAccount = secondLvlaccount.AccountName;
            string thirdAccount = thirdLvlaccount.AccountName;
            string fourthAccount = financeAccount.AccountName;

            if (firstAccount.ToLower().Contains("payable") || seccondAccount.ToLower().Contains("payable")
                || thirdAccount.ToLower().Contains("payable") || fourthAccount.ToLower().Contains("payable"))
            {
                flag = true;
            }
            return flag;
        }

        private static bool IsFinanceLiableAccount(FinanceFourthLvlAccount financeAccount)
        {
            bool flag = false;
            var thirdLvlaccount = accountRepo.GetFinanceThirdLvlAccountById(financeAccount.ThirdLvlAccountId);
            var secondLvlaccount = accountRepo.GetFinanceSeccondLvlAccountById(thirdLvlaccount.SeccondLvlAccountId);
            var firstLvlaccount = accountRepo.GetFinanceFirstLvlAccountById(secondLvlaccount.FirstLvlAccountId);
            string firstAccount = firstLvlaccount.AccountName;
            string seccondAccount = secondLvlaccount.AccountName;
            string thirdAccount = thirdLvlaccount.AccountName;
            string fourthAccount = financeAccount.AccountName;

            if (firstAccount.ToLower().Contains("liability") || seccondAccount.ToLower().Contains("liability")
                || thirdAccount.ToLower().Contains("liability") || fourthAccount.ToLower().Contains("liability"))
            {
                flag = true;
            }

            return flag;
        }

        private static bool IsFinanceCapitalAccount(FinanceFourthLvlAccount financeAccount)
        {
            bool flag = false;
            var thirdLvlaccount = accountRepo.GetFinanceThirdLvlAccountById(financeAccount.ThirdLvlAccountId);
            var secondLvlaccount = accountRepo.GetFinanceSeccondLvlAccountById(thirdLvlaccount.SeccondLvlAccountId);
            var firstLvlaccount = accountRepo.GetFinanceFirstLvlAccountById(secondLvlaccount.FirstLvlAccountId);
            string firstAccount = firstLvlaccount.AccountName;
            string seccondAccount = secondLvlaccount.AccountName;
            string thirdAccount = thirdLvlaccount.AccountName;
            string fourthAccount = financeAccount.AccountName;

            if (firstAccount.ToLower().Contains("capital") || seccondAccount.ToLower().Contains("capital")
                || thirdAccount.ToLower().Contains("capital") || fourthAccount.ToLower().Contains("capital"))
            {
                flag = true;
            }

            return flag;
        }

        private static bool IsFifthPaybleAccount(FinanceFifthLvlAccount financeAccount)
        {
            bool flag = false;
            var fourthLvlaccount = accountRepo.GetFinanceFourthLvlAccountById(financeAccount.FourthLvlAccountId);
            var thirdLvlaccount = accountRepo.GetFinanceThirdLvlAccountById(fourthLvlaccount.ThirdLvlAccountId);
            var secondLvlaccount = accountRepo.GetFinanceSeccondLvlAccountById(thirdLvlaccount.SeccondLvlAccountId);
            var firstLvlaccount = accountRepo.GetFinanceFirstLvlAccountById(secondLvlaccount.FirstLvlAccountId);
            string firstAccount = firstLvlaccount.AccountName;
            string seccondAccount = secondLvlaccount.AccountName;
            string thirdAccount = thirdLvlaccount.AccountName;
            string fourthAccount = fourthLvlaccount.AccountName;
            string fifthAccount = financeAccount.AccountName;

            if (firstAccount.ToLower().Contains("payable") || seccondAccount.ToLower().Contains("payable")
                || thirdAccount.ToLower().Contains("payable") || fourthAccount.ToLower().Contains("payable")
                || fifthAccount.ToLower().Contains("payable"))
            {
                flag = true;
            }
            return flag;
        }

        private static bool IsFifthLiableAccount(FinanceFifthLvlAccount financeAccount)
        {
            bool flag = false;
            var fourthLvlaccount = accountRepo.GetFinanceFourthLvlAccountById(financeAccount.FourthLvlAccountId);
            var thirdLvlaccount = accountRepo.GetFinanceThirdLvlAccountById(fourthLvlaccount.ThirdLvlAccountId);
            var secondLvlaccount = accountRepo.GetFinanceSeccondLvlAccountById(thirdLvlaccount.SeccondLvlAccountId);
            var firstLvlaccount = accountRepo.GetFinanceFirstLvlAccountById(secondLvlaccount.FirstLvlAccountId);
            string firstAccount = firstLvlaccount.AccountName;
            string seccondAccount = secondLvlaccount.AccountName;
            string thirdAccount = thirdLvlaccount.AccountName;
            string fourthAccount = fourthLvlaccount.AccountName;
            string fifthAccount = financeAccount.AccountName;

            if (firstAccount.ToLower().Contains("liability") || seccondAccount.ToLower().Contains("liability")
                || thirdAccount.ToLower().Contains("liability") || fourthAccount.ToLower().Contains("liability")
                || fifthAccount.ToLower().Contains("liability"))
            {
                flag = true;
            }
            return flag;
        }

        private static bool IsFifthCapitalAccount(FinanceFifthLvlAccount financeAccount)
        {
            bool flag = false;
            var fourthLvlaccount = accountRepo.GetFinanceFourthLvlAccountById(financeAccount.FourthLvlAccountId);
            var thirdLvlaccount = accountRepo.GetFinanceThirdLvlAccountById(fourthLvlaccount.ThirdLvlAccountId);
            var secondLvlaccount = accountRepo.GetFinanceSeccondLvlAccountById(thirdLvlaccount.SeccondLvlAccountId);
            var firstLvlaccount = accountRepo.GetFinanceFirstLvlAccountById(secondLvlaccount.FirstLvlAccountId);
            string firstAccount = firstLvlaccount.AccountName;
            string seccondAccount = secondLvlaccount.AccountName;
            string thirdAccount = thirdLvlaccount.AccountName;
            string fourthAccount = fourthLvlaccount.AccountName;
            string fifthAccount = financeAccount.AccountName;

            if (firstAccount.ToLower().Contains("capital") || seccondAccount.ToLower().Contains("capital")
                || thirdAccount.ToLower().Contains("capital") || fourthAccount.ToLower().Contains("capital")
                || fifthAccount.ToLower().Contains("capital"))
            {
                flag = true;
            }
            return flag;
        }
    }
}