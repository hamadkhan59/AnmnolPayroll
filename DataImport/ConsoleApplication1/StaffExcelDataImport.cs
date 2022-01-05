using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;

namespace ConsoleApplication1
{
    public class StaffExcelDataImport
    {
        static string FilePath = "";
        private static IStaffRepository staffRepo;
        private static IFinanceAccountRepository financeRepo;
        private static int BRANCH_ID = 1;
        private static int DESIGNATION_ID = 1;
        private static int SALARY = 0;
        private static int DUTY_HOURS = 0;
        private static string CNIC = "";
        private static string FATHER_CNIC = "";
        private static string STAFF_NAME = "";
        private static string FATHER_NAME = "";
        private static string ADDRESS = "";
        private static string CONTACT_NO = "";

        public const string CAT_FEE_RECEIVABLE = "Fee Receivables";
        public const string CAT_TEACHING_STAFF = "Teaching Staff";
        public const string CAT_NON_TEACHING_STAFF = "Non Teaching Staff";
        public const string CAT_ACCEDAMIC_STAFF = "Academic Staff Salaries";
        public const string CAT_NON_ACCEDAMIC_STAFF = "Non Academic Staff Salaries";
        public const int CAT_STAFF_SALARIES = 33;
        public const int CAT_STAFF_ADVANCES = 10;
        public const int CAT_RECEIVABLES = 1023;
        public const int CAT_BANK_FIRST_LVL = 7;
        public const int CAT_BANK_SECOND_LVL = 13;
        public const int CAT_BANK_THIRD_LVL = 13;

        public static void InitializeExcelImport(string filePath)
        {
            FilePath = filePath;
            ExcelHelper.InitializeExcelHelper(FilePath);
            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2()); ;
            financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
            ADDRESS = ConfigurationManager.AppSettings["staffAddress"];
            FATHER_CNIC = ConfigurationManager.AppSettings["staffFatherCnic"];
            FATHER_NAME = ConfigurationManager.AppSettings["staffFatherName"];
            CONTACT_NO = ConfigurationManager.AppSettings["staffDummyMobile"];
            CNIC = ConfigurationManager.AppSettings["staffCNIC"];
        }

        public static int ImportStaffData()
        {
            string columnsValue = ConfigurationManager.AppSettings["staff-columns"];
            string[] columnsList = columnsValue.Split('|');
            int rowCount = ExcelHelper.RowCount();

            Console.WriteLine("=======================================================");
            Console.WriteLine("             Staff data Import Started");
            Console.WriteLine("=======================================================");

            try
            {
                for (int i = 2; i <= rowCount; i++)
                {
                    Staff staff = new Staff();
                    staff.FatherCNIC = FATHER_CNIC;
                    staff.FatherName = FATHER_NAME;
                    staff.Address = ADDRESS;
                    staff.CurrentAddress = ADDRESS;
                    staff.DateOfBirth = DateTime.Now;
                    staff.JoinDate = DateTime.Now;

                    for (int j = 0; j < columnsList.Count(); j++)
                    {
                        if (columnsList[j] == "StaffName")
                        {
                            STAFF_NAME = ExcelHelper.GetCellValue(i, j + 1);
                            staff.Name = STAFF_NAME;
                        }
                        else if (columnsList[j] == "Salary")
                        {
                            string salary = ExcelHelper.GetCellValue(i, j + 1);
                            salary = salary.Trim();
                            salary = salary.Replace(",", "");
                            SALARY = string.IsNullOrEmpty(salary) ? 0 : int.Parse(salary);
                            staff.Salary = SALARY;
                        }
                        else if (columnsList[j] == "DutyHours")
                        {
                            string dutyHours = ExcelHelper.GetCellValue(i, j + 1);
                            dutyHours = dutyHours.Trim();
                            DUTY_HOURS = string.IsNullOrEmpty(dutyHours) ? 0 : int.Parse(dutyHours);
                            staff.DutyHours = DUTY_HOURS;
                        }
                        else if (columnsList[j] == "CNIC")
                        {
                            string cnic = ExcelHelper.GetCellValue(i, j + 1);
                            cnic = cnic.Trim();
                            cnic = (string.IsNullOrEmpty(cnic) || cnic == "-") ? CNIC : cnic;

                            staff.CNIC = cnic;
                        }
                        else if (columnsList[j] == "ContactNo")
                        {
                            string contactNo = ExcelHelper.GetCellValue(i, j + 1);
                            contactNo = contactNo.Trim();
                            contactNo = (string.IsNullOrEmpty(contactNo) || contactNo == "-") ? CONTACT_NO : contactNo;
                            string contact1 = "", contact2 = "";
                            if (contactNo.Contains("/"))
                            {
                                contact1 = contactNo.Split('/')[0];
                                contact2 = contactNo.Split('/')[1];
                            }
                            else
                            {
                                contact1 = contactNo;
                            }
                            staff.PhoneNumber = contact1;
                            staff.PhoneNumber1 = contact2;
                        }
                    }

                    staff.DesignationId = DESIGNATION_ID;
                    staff.BranchId = BRANCH_ID;

                    staffRepo.AddStaff(staff);
                    createFinanceAccount(staff);

                    if (i % 20 == 0)
                    {
                        Console.WriteLine("Staff data import Count : " + i);
                    }
                }

                Console.WriteLine("Staff data import is finished");
                Console.WriteLine("Staff data import count : " + (rowCount - 1).ToString());
                //Console.WriteLine("_______________________________________________________");

                ExcelHelper.CloseExcelSheets();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error in staff import");
                Console.WriteLine("Exception message : " + exc.Message) ;
                Console.WriteLine("Exception stack trace : " + exc.StackTrace) ;
                Console.ReadLine();
            }
            return rowCount - 1;
        }


        private static void createFinanceAccount(Staff staff)
        {
            try
            {
                int advanceAccountTeaching = GetFourthLvlConfigurationAccount((int)staff.BranchId, CAT_TEACHING_STAFF, CAT_STAFF_ADVANCES);
                int advanceAccountNonTeaching = GetFourthLvlConfigurationAccount((int)staff.BranchId, CAT_NON_TEACHING_STAFF, CAT_STAFF_ADVANCES);
                int salaryAccountAccedamic = GetFourthLvlConfigurationAccount((int)staff.BranchId, CAT_ACCEDAMIC_STAFF, CAT_STAFF_SALARIES);
                int salaryAccountNonAccedamic = GetFourthLvlConfigurationAccount((int)staff.BranchId, CAT_NON_ACCEDAMIC_STAFF, CAT_STAFF_SALARIES);

                FinanceFifthLvlAccount accounts = new FinanceFifthLvlAccount();

                accounts.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name;
                accounts.AccountDescription = "Finanace Salary Account For : " + accounts.AccountName;
                accounts.CreatedOn = DateTime.Now;
                accounts.Value = 0;
                accounts.Count = 0;
                accounts.BranchId = staff.BranchId;
                accounts.FourthLvlAccountId = salaryAccountAccedamic;
                if (staff.TypeId == 2)
                    accounts.FourthLvlAccountId = salaryAccountNonAccedamic;

                financeRepo.AddFinanceFifthLvlAccount(accounts);

                FinanceFifthLvlAccount advanceAccount = new FinanceFifthLvlAccount();

                advanceAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Advance Account";
                advanceAccount.AccountDescription = "Advance Account For : " + accounts.AccountName;
                advanceAccount.CreatedOn = DateTime.Now;
                advanceAccount.Value = 0;
                advanceAccount.Count = 0;
                advanceAccount.BranchId = staff.BranchId;
                advanceAccount.FourthLvlAccountId = advanceAccountTeaching;
                if (staff.TypeId == 2)
                    advanceAccount.FourthLvlAccountId = advanceAccountNonTeaching;

                financeRepo.AddFinanceFifthLvlAccount(advanceAccount);

                FinanceFifthLvlAccount deductionAccount = new FinanceFifthLvlAccount();

                deductionAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Deduction Account";
                deductionAccount.AccountDescription = "Deduction Account For : " + accounts.AccountName;
                deductionAccount.CreatedOn = DateTime.Now;
                deductionAccount.Value = 0;
                deductionAccount.Count = 0;
                deductionAccount.BranchId = staff.BranchId;
                deductionAccount.FourthLvlAccountId = salaryAccountAccedamic;
                if (staff.TypeId == 2)
                    deductionAccount.FourthLvlAccountId = salaryAccountNonAccedamic;

                financeRepo.AddFinanceFifthLvlAccount(deductionAccount);

                FinanceFifthLvlAccount bonusAccount = new FinanceFifthLvlAccount();

                bonusAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Bonus Account";
                bonusAccount.AccountDescription = "Bonus Account For : " + accounts.AccountName;
                bonusAccount.CreatedOn = DateTime.Now;
                bonusAccount.Value = 0;
                bonusAccount.Count = 0;
                bonusAccount.BranchId = staff.BranchId;
                bonusAccount.FourthLvlAccountId = salaryAccountAccedamic;
                if (staff.TypeId == 2)
                    bonusAccount.FourthLvlAccountId = salaryAccountNonAccedamic;

                financeRepo.AddFinanceFifthLvlAccount(bonusAccount);

                FinanceFifthLvlAccount miscWithdrawAccount = new FinanceFifthLvlAccount();

                miscWithdrawAccount.AccountName = staff.StaffId.ToString().PadLeft(6, '0') + "-" + staff.Name + " Misc Withdraw Account";
                miscWithdrawAccount.AccountDescription = "Bonus Account For : " + accounts.AccountName;
                miscWithdrawAccount.CreatedOn = DateTime.Now;
                miscWithdrawAccount.Value = 0;
                miscWithdrawAccount.Count = 0;
                miscWithdrawAccount.BranchId = staff.BranchId;
                miscWithdrawAccount.FourthLvlAccountId = salaryAccountAccedamic;
                if (staff.TypeId == 2)
                    miscWithdrawAccount.FourthLvlAccountId = salaryAccountNonAccedamic;

                financeRepo.AddFinanceFifthLvlAccount(miscWithdrawAccount);
            }
            catch (Exception ex)
            {
            }
        }

        public static int GetFourthLvlConfigurationAccount(int BranchId, string AccountName, int thirdLvlAccountId)
        {
            int accountId = 0;

            var tempAccount = financeRepo.GetFinanceFourthLvlAccountByName(AccountName, BranchId);
            if (tempAccount == null)
            {
                tempAccount = new FinanceFourthLvlAccount();
                tempAccount.AccountName = AccountName;
                tempAccount.AccountDescription = "Confoguration account is created for " + AccountName;
                tempAccount.BranchId = BranchId;
                tempAccount.ThirdLvlAccountId = thirdLvlAccountId;
                tempAccount.CreatedOn = DateTime.Now;
                financeRepo.AddFinanceFourthLvlAccount(tempAccount);
            }

            accountId = tempAccount.Id;

            return accountId;
        }

    }
}