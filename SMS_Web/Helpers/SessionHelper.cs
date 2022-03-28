using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_DAL.ViewModel;
using SMS_Web.Controllers.SecurityAssurance;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SMS_Web.Helpers
{
    public static class SessionHelper
    {
        public static Dictionary<string, SessionModel> session = new Dictionary<string, SessionModel>();
        public static List<string> userList = new List<string>();
        public static SC_WEBEntities2 dbContext1 = new SC_WEBEntities2();
        public static SC_WEBEntities2 dbContext {
            get {
                if (dbContext1 == null)
                    dbContext1 = new SC_WEBEntities2();
                return dbContext1;
            }
        }

        public static IClassRepository classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
        public static IClassSectionRepository classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
        public static ISectionRepository secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
        public static IAttendanceRepository attendanceRepo = new AttendanceRepositoryImp(new SC_WEBEntities2());
        public static IStudentRepository studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());
        public static IFinanceAccountRepository financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
        public static IFeePlanRepository feeRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
        public static IClassSubjectRepository classSubjRepo = new ClassSubjectRepositoryImp(new SC_WEBEntities2());
        public static ISubjectRepository subjectRepo = new SubjectRepositoryImp(new SC_WEBEntities2());
        public static ILeavingReasonRepository leavingRepo = new LeavingReasonRepositoryImp(new SC_WEBEntities2());
        public static IPreviousHitoryRepository historyRepo = new PreviousHistoryRepositoryImp(new SC_WEBEntities2());
        public static IStaffRepository staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
        public static IExamRepository examRepo = new ExamRepositoryImp(new SC_WEBEntities2());
        public static IFeePlanRepository feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
        public static IFinanceAccountRepository accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
        public static IAdmissionChargesRepository acRepo = new AdmissionChargesRepositoryImp(new SC_WEBEntities2());
        public static ISecurityRepository securityRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
        public static ITransportRepository treansRepo = new TransportRepositoryImp(new SC_WEBEntities2());
        public static IStoreRepository storeRepo = new StoreRepositoryImp(new SC_WEBEntities2());
        static SMS_DAL.Reports.DAL_Store_Reports storeDs = new SMS_DAL.Reports.DAL_Store_Reports();


        //public static bool IsCacheLoaded = false;
        //public static bool InvalidateClassCache = true;
        //public static bool InvalidateSectionCache = true;
        //public static bool InvalidateClassSectionCache = true;
        //public static bool InvalidateSubjectCache = true;
        //public static bool InvalidateClassSubjectCache = true;
        //public static bool InvalidateSessionCache = true;
        //public static bool InvalidateStaffCache = true;
        //public static bool InvalidateExamTermCache = true;
        //public static bool InvalidateExamTypeCache = true;
        //public static bool InvalidateChallanCache = true;
        //public static bool InvalidateFeeHeadCache = true;
        //public static bool InvalidateFeeAccountDetailCache = true;
        //public static bool InvalidateFeeFinanceAccountCache = true;
        //public static bool InvalidateBankAccountCache = true;
        //public static bool InvalidateFinanceFirstLvlCache = true;
        //public static bool InvalidateFinanceSeccondLvlCache = true;
        //public static bool InvalidateFinanceThirdLvlCache = true;
        //public static bool InvalidateFinanceFourthLvlCache = true;
        //public static bool InvalidateFinanceFifthLvlCache = true;
        //public static bool InvalidateDesignationCache = true;
        //public static bool InvalidateDesignationCategoryCache = true;
        //public static bool InvalidateAttendancePolicyCache = true;
        //public static bool InvalidateAllownceCache = true;
        //public static bool InvalidateBranchCache = true;
        //public static bool InvalidateUserCache = true;
        //public static bool InvalidateStudentNameCache = true;
        //public static bool InvalidateFathernameCache = true;
        //public static bool InvalidateTransportCache = true;
        //public static bool InvalidateTransportDriverCache = true;

        public static bool IsCacheLoaded = false;
        public static bool InvalidateClassCache = false;
        public static bool InvalidateGradeCache = false;
        public static bool InvalidateSystemConfigCache = false;
        public static bool InvalidateRemarksCache = false;
        public static bool InvalidateSectionCache = false;
        public static bool InvalidateClassSectionCache = false;
        public static bool InvalidateSubjectCache = false;
        public static bool InvalidateClassSubjectCache = false;
        public static bool InvalidateSessionCache = false;
        public static bool InvalidateStaffCache = false;
        public static bool InvalidateExamTermCache = false;
        public static bool InvalidateActivityCache = false;
        public static bool InvalidateExamTypeCache = false;
        public static bool InvalidateChallanCache = false;
        public static bool InvalidateFeeHeadCache = false;
        public static bool InvalidateFeeAccountDetailCache = false;
        public static bool InvalidateFeeFinanceAccountCache = false;
        public static bool InvalidateBankAccountCache = false;
        public static bool InvalidateFinanceFirstLvlCache = false;
        public static bool InvalidateFinanceSeccondLvlCache = false;
        public static bool InvalidateFinanceThirdLvlCache = false;
        public static bool InvalidateFinanceFourthLvlCache = false;
        public static bool InvalidateFinanceFifthLvlCache = false;
        public static bool InvalidateDesignationCache = false;
        public static bool InvalidateDesignationCategoryCache = false;
        public static bool InvalidateAttendancePolicyCache = false;
        public static bool InvalidateAllownceCache = false;
        public static bool InvalidateBranchCache = false;
        public static bool InvalidateUserCache = false;
        public static bool InvalidateStudentNameCache = false;
        public static bool InvalidateFathernameCache = false;
        public static bool InvalidateTransportCache = false;
        public static bool InvalidateTransportDriverCache = false;
        public static bool InvalidateStaffHolidayCache = false;
        public static bool InvalidateVendorCache = false;
        public static bool InvalidateIssuerCache = false;
        public static bool InvalidateItemCache = false;

        private static List<Class> _classList;
        private static List<Branch> _branchList;
        private static List<Section> _sectionList;
        private static List<ClassSectionModel> _classSectionList;
        private static List<Subject> _subjectList;
        private static List<RegisterCourseModel> _classSubjectList;
        private static List<AdmissionType> _admissionTypeList;
        private static List<Gender> _genderList;
        private static List<StudentAttendanceStatu> _attendanceStatusList;
        private static List<Relegion> _religionList;
        private static List<Session> _sessionList;
        private static List<LeavingReason> _reasonList;
        private static List<LeavingStatu> _statusList;
        private static List<Staff> _staffList;
        private static List<TestStatu> _testStatusList;
        private static List<Year> _yearList;
        private static List<ResultType> _resultTypeList;
        private static List<Month> _monthList;
        private static List<ExamTerm> _examTermList;
        private static List<Activity> _activityList;
        private static List<ExamType> _examTypeList;
        private static List<Challan> _challanList;
        private static List<FeeHead> _feeHeadList;
        private static List<AccountType> _accountTypeList;
        private static List<FinanceFourthLvlAccount> _feeAccountDetailList;
        private static List<FinanceFifthLvlAccountModel> _feeFinanceAccountList;
        private static List<BankAccount> _bankAccountList;
        private static List<FinanceFirstLvlAccount> _firstLvlAccountList;
        private static List<FinanceSeccondLvlAccountModel> _seccondLvlAccountList;
        private static List<FinanceThirdLvlAccountModel> _thirdLvlAccountList;
        private static List<FinanceFourthLvlAccountModel> _fourthLvlAccountList;
        private static List<FinanceFifthLvlAccountModel> _fifthLvlAccountList;
        private static List<FinanceFifthLvlAccountModel> _fifthLvlAccountListWithoutReceipts;
        private static List<Designation> _designationList;
        private static List<DesignationCatagory> _designationCategoryList;
        private static List<StaffAttandancePolicyModel> _staffAttandancePolicyList;
        private static List<AllowLeave> _allowLeaveList;
        private static List<LateInCount> _lateInCountList;
        private static List<Allownce> _allownceList;
        private static List<MeritalStatu> _meritalStatusList;
        private static List<StaffType> _staffTypeList;
        private static List<User> _userList;
        private static List<SmsEvent> _smsEventList;
        private static List<string> _studentFatherList;
        private static List<string> _studentNameList;

        public static List<SmsMessage> _smsMessageList;
        public static List<SmsParam> _smsParamsList;
        public static List<SmsVender> _smsVendersList;
        public static List<SmsEventParam> _smsEventParamList;

        private static List<TransportStop> _transportStopList;
        private static List<TransportDriver> _transportDriverList;
        private static List<StudentPerChallan> _studentPerChallans;
        private static List<GradesConfig> _gradeConfig;
        private static List<RemarksConfig> _remarksConfig;
        private static List<FinanceAccountLevel> _financeLevel;
        private static List<DirectoryViewOption> _direcotryViewOption;
        private static List<SystemConfig> _systemConfig;
        private static List<StaffHoliday> _staffHolidayList;
        private static List<Vendor> _vendorList;
        private static List<Issuer> _issuerList;
        private static List<ItemUnit> _unitList;
        private static List<ItemModel> _itemList;

        public static void BuildCache()
        {
            //_classList = classRepo.GetAllClasses();
            //_sectionList = secRepo.GetAllSections();
            //_classSectionList = classSecRepo.GetAllClassSections();
           // _subjectList = subjectRepo.GetAllSubjectes();
           // _classSubjectList = classSubjRepo.GetAllRegisterCourses();
            _admissionTypeList = studentRepo.GetAllAdmissionTypes();
            _genderList = studentRepo.GetAllGenders();
            _attendanceStatusList = studentRepo.GetAllAttendanceStatus();
            _religionList = studentRepo.GetAllReligion();
            _sessionList = studentRepo.GetAllSessions();
            _reasonList = leavingRepo.GetAllLeavingReasons();
            _statusList = leavingRepo.GetAllLeavingStatus();
            //_staffList = staffRepo.GetAllStaff();
            _testStatusList = studentRepo.GetAllTestStatus();
            _yearList = feePlanRepo.GetAllYears();
            _resultTypeList = feePlanRepo.GetAllResultTypes(); 
             _monthList = feePlanRepo.GetAllMonths();
            _studentPerChallans = feePlanRepo.GetAllStudentPerChallans();
            //_examTermList = examRepo.GetAllExamTerm();
            //_examTypeList = examRepo.GetAllExamTypes();
            //_challanList = feePlanRepo.GetAllChallan();
            //_feeHeadList = feePlanRepo.GetAllFeeHeads();
            _accountTypeList = feePlanRepo.GetAllAccountTypes();
            //_feeAccountDetailList = null;
            //_feeFinanceAccountList = null;
            //_bankAccountList = feePlanRepo.GetAllBankAccount();
            //_firstLvlAccountList = accountRepo.GetAllFinanceFirstLvlAccounts();
            //_seccondLvlAccountList = accountRepo.GetAllFinanceSeccondLvlAccounts();
            //_thirdLvlAccountList = accountRepo.GetAllFinanceThirdLvlAccounts();
            //_fourthLvlAccountList = accountRepo.GetAllFinanceFourthLvlAccounts();
            //_fifthLvlAccountList = accountRepo.GetAllFinanceFifthLvlAccounts();
           // _designationList = staffRepo.GetAllDesignations();
            //_designationCategoryList = staffRepo.GetAllDesignationCategories();
            //_staffAttandancePolicyList = staffRepo.GetAllStaffAttandancePolicies();
            _allowLeaveList = staffRepo.GetAllAllowLeaves();
            _lateInCountList = staffRepo.GetAllLateInCOunts();
           // _allownceList = staffRepo.GetAllAllownces();
            _meritalStatusList = staffRepo.GetAllMeritalStatuses();
            _staffTypeList = staffRepo.GetAllStaffTypes();
            _gradeConfig = examRepo.GetAllGrade();
            _remarksConfig = examRepo.GetAllRemarks();
            _staffTypeList = staffRepo.GetAllStaffTypes();
            //_branchList = securityRepo.GetAllBranches();
            //_userList = securityRepo.GetAllUsers();
            //_studentFatherList = studentRepo.GetFatherNameList();
            //_studentNameList = studentRepo.GetStudentNameList();
            _smsMessageList = feePlanRepo.GetAllMessages();
            _smsEventList = feePlanRepo.GetAllSMSEvents();
            _smsParamsList = feePlanRepo.GetSmsParams();
            _smsVendersList = feePlanRepo.GetAllSmsVenders();
            _smsEventParamList = feePlanRepo.GetAllSmsEventParam();
            _direcotryViewOption = dbContext.DirectoryViewOptions.ToList();
        }


        public static List<Class> ClassList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_classList == null || _classList.Count == 0 || InvalidateClassCache == false)
            {
                classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
                _classList = classRepo.GetAllClasses();
                InvalidateClassCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
                var tempList = _classList.Where(x => x.BranchId == branchId).ToList();
            //}

                return tempList; 
        }

        public static List<Vendor> VendorList()
        {
            if (_vendorList == null || _vendorList.Count == 0 || InvalidateVendorCache == false)
            {
                storeRepo = new StoreRepositoryImp(new SC_WEBEntities2());
                _vendorList = storeRepo.GetAllVendors();
                InvalidateVendorCache = true;
            }

            return _vendorList;
        }

        public static List<Issuer> IssuerList()
        {
            if (_issuerList == null || _issuerList.Count == 0 || InvalidateVendorCache == false)
            {
                storeRepo = new StoreRepositoryImp(new SC_WEBEntities2());
                _issuerList = storeRepo.GetAllIssuers();
                InvalidateIssuerCache = true;
            }

            return _issuerList;
        }

        public static List<ItemModel> ItemList()
        {
            var ds = storeDs.GetItemList();

            if (_itemList == null)
                _itemList = new List<ItemModel>();
            else
                _itemList.Clear();

            if (ds.Tables != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ItemModel model = new ItemModel();
                    model.Id = int.Parse(row["Id"].ToString());
                    model.ItemName = row["ItemName"].ToString();
                    model.UnitName = row["Unit"].ToString();
                    model.UnitId = int.Parse(row["UnitId"].ToString());
                    model.Qty = double.Parse(row["Quantity"].ToString());
                    _itemList.Add(model);
                }
            }

            return _itemList;
        }

        public static List<string> ItemNamesList()
        {
            ItemList();
            List<string> names = new List<string>();

            foreach (var item in _itemList)
            {
                names.Add(item.Id.ToString().PadLeft(4, '0') +" | "+ item.ItemName + " | " + item.UnitName + " | " + item.Qty);
            }

            return names;
        }

        public static List<string> VendorNameList()
        {
            VendorList();
            List<string> names = new List<string>();

            foreach (var item in _vendorList)
            {
                names.Add(item.Id + " | " + item.Name + " | " + item.PhoneNo + " | " + item.Email + " | " + item.CompanyName);
            }

            return names;
        }

        public static List<string> IssuerNameList()
        {
            IssuerList();
            List<string> names = new List<string>();

            foreach (var item in _issuerList)
            {
                names.Add(item.Id + " | " + item.Name + " | " + item.PhoneNo + " | " + item.Email + " | " + item.CompanyName);
            }

            return names;
        }

        public static List<ItemUnit> UnitList()
        {
            if (_unitList == null || _unitList.Count == 0)
            {
                storeRepo = new StoreRepositoryImp(new SC_WEBEntities2());
                _unitList = storeRepo.GetAllItemUnits();
            }

            return _unitList;
        }


        public static List<GradesConfig> GradesConfigList()
        {
            if (InvalidateGradeCache == false)
            {
                examRepo = new ExamRepositoryImp(new SC_WEBEntities2());
                _gradeConfig = examRepo.GetAllGrade();
                InvalidateGradeCache = true;
            }

            return _gradeConfig;
        }

        public static List<SystemConfig> SystemConfigList()
        {
            if (InvalidateSystemConfigCache == false || (_systemConfig == null || _systemConfig.Count == 0))
            {
                using (SC_WEBEntities2 context = new SC_WEBEntities2())
                {
                    _systemConfig = context.SystemConfigs.ToList();
                }
                InvalidateSystemConfigCache = true;
            }

            return _systemConfig;
        }

        public static List<RemarksConfig> RemarksConfigList()
        {
            if (InvalidateRemarksCache == false)
            {
                examRepo = new ExamRepositoryImp(new SC_WEBEntities2());
                _remarksConfig = examRepo.GetAllRemarks();
                InvalidateRemarksCache = true;
            }

            return _remarksConfig;
        }

        public static List<FinanceAccountLevel> GetFinanceLevelList()
        {
            if (_financeLevel == null || _financeLevel.Count == 0)
            {
                SC_WEBEntities2 context = new SC_WEBEntities2();
                _financeLevel = context.FinanceAccountLevels.ToList();
            }

            return _financeLevel;
        }

        public static bool IsRemarksOverlapped(RemarksConfig config)
        {
            bool flag = false;

            foreach (var item in RemarksConfigList())
            {
                if (config.Id == 0 || item.Id != config.Id)
                {
                    if (config.MinRange >= item.MinRange && config.MinRange <= item.MaxRange
                    || config.MaxRange >= item.MinRange && config.MaxRange <= item.MaxRange)
                    {
                        flag = true;
                        break;
                    }
                }
            }

            return flag;
        }

        public static List<DirectoryViewOption> DirectoryViewOptionList()
        {
            return _direcotryViewOption;
        }

        public static bool IsGradesOverlapped(GradesConfig config)
        {
            bool flag = false;

            foreach (var item in GradesConfigList())
            {
                if (config.Id == 0 || item.Id != config.Id)
                {
                    if (config.MinRange >= item.MinRange && config.MinRange <= item.MaxRange
                        || config.MaxRange >= item.MinRange && config.MaxRange <= item.MaxRange)
                    {
                        flag = true;
                        break;
                    }
                }
            }

            return flag;
        }

        public static int GetFourthLvlConfigurationAccount(int BranchId, string AccountName, int thirdLvlAccountId)
        {
            int accountId = 0;

            var tempAccount = accountRepo.GetFinanceFourthLvlAccountByName(AccountName, BranchId);
            if (tempAccount == null)
            {
                tempAccount = new FinanceFourthLvlAccount();
                tempAccount.AccountName = AccountName;
                tempAccount.AccountDescription = "Confoguration account is created for " + AccountName;
                tempAccount.BranchId = BranchId;
                tempAccount.ThirdLvlAccountId = thirdLvlAccountId;
                tempAccount.CreatedOn = DateTime.Now;
                accountRepo.AddFinanceFourthLvlAccount(tempAccount);
            }

            accountId = tempAccount.Id;

            return accountId;
        }

        public static List<Branch> BranchList
        {
            get
            {
                if (_branchList == null || _branchList.Count == 0 || InvalidateBranchCache == false)
                {
                    securityRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
                    _branchList = securityRepo.GetAllBranches();
                    InvalidateBranchCache = true;
                }
                return _branchList;
            }
        }

        public static List<Class> ActiveClassList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);

            if (_classList == null || _classList.Count == 0 || InvalidateClassCache == false)
            {
                classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
                _classList = classRepo.GetAllClasses();
                InvalidateClassCache = true;
            }
            _classList = _classList.Where(x => x.IsActive == true).ToList();

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
                var tempList = _classList.Where(x => x.BranchId == branchId).ToList();
            //}
                return tempList;
        }

        public static List<ClassDD> ActiveClassListDD(string browserDetail)
        {
            var ddList = ActiveClassList(browserDetail);
            var query = from dd in ddList
                        select new ClassDD
                        {
                            Id = dd.Id,
                            Name = dd.Name
                        };
            return query.ToList();
        }

        public static List<RegisterCourseModel> AllClassSubjectList()
        {
            if (_classSubjectList == null || _classSubjectList.Count == 0 || InvalidateClassSubjectCache == false)
            {
                classSubjRepo = new ClassSubjectRepositoryImp(new SC_WEBEntities2());
                _classSubjectList = classSubjRepo.GetAllRegisterCoursesModel();
                InvalidateClassSubjectCache = true;
            }

            return _classSubjectList;
        }

        public static List<RegisterCourseModel> ClassSubjectList(int ClassId, int SectionId)
        {
            var tempList = AllClassSubjectList().Where(x => x.ClassId == ClassId && x.SectionId == SectionId).ToList();
            return tempList;
        }


        public static List<Section> SectionList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_sectionList == null || _sectionList.Count == 0 || InvalidateSectionCache == false)
            {
                secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
                _sectionList = secRepo.GetAllSections();
                InvalidateSectionCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _sectionList.Where(x => x.BranchId == branchId).ToList();
            //}

            return tempList;
        }

        public static List<ClassSectionModel> ClassSectionList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_classSectionList == null || _classSectionList.Count == 0 || InvalidateClassSectionCache == false)
            {
                classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
                _classSectionList = classSecRepo.GetAllClassSectionsModel();
                InvalidateClassSectionCache = true;
            }
            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _classSectionList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }


        public static List<ClassSectionModel> ClassAllSectionList()
        {
            if (_classSectionList == null || _classSectionList.Count == 0 || InvalidateClassSectionCache == false)
            {
                classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
                _classSectionList = classSecRepo.GetAllClassSectionsModel();
                InvalidateClassSectionCache = true;
            }
            return _classSectionList;
        }


        public static List<SectionDD> SectionListDD(int ClassId)
        {
            var ddList = ClassAllSectionList();
            ddList = ddList.Where(x => x.ClassId == ClassId).ToList();
            var query = from dd in ddList
                        select new SectionDD
                        {
                            Id = dd.SectionId,
                            Name = dd.SectionName
                        };
            return query.ToList();
        }

        public static List<Designation> DesignationAllList()
        {
            if (_designationList == null || _designationList.Count == 0 || InvalidateDesignationCache == false)
            {
                staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
                _designationList = staffRepo.GetAllDesignations();
                InvalidateDesignationCache = true;
            }

            return _designationList;
        }

        public static List<Designation> DesignationDD(int CategoryId)
        {
            var ddList = DesignationAllList();
            ddList = ddList.Where(x => x.CatagoryId == CategoryId).ToList();
           
            return ddList;
        }

        public static string GetGrade(decimal obtained, decimal totalMarks, decimal passPercentage)
        {
            var gradeList = GradesConfigList();
            decimal Totalpercent = 100;
            decimal percentage = (obtained / totalMarks) * Totalpercent;
            percentage = FlorPercntage(percentage);

            string obtGrade = "";

            if (gradeList != null && gradeList.Count() > 0)
            {
                foreach (var grade in gradeList)
                {
                    if (percentage >= (decimal)grade.MinRange && percentage <= (decimal)grade.MaxRange)
                    {
                        obtGrade = grade.Grade;
                    }

                }
            }

            return obtGrade;
        }

        public static string GetRemarks(decimal obtained, decimal totalMarks, decimal passPercentage)
        {
            var remarksList = RemarksConfigList();
            decimal Totalpercent = 100;
            decimal percentage = (obtained / totalMarks) * Totalpercent;
            string obtGrade = "";
            percentage = FlorPercntage(percentage);

            if (remarksList != null && remarksList.Count() > 0)
            {
                foreach (var remarks in remarksList)
                {
                    if (percentage >= (decimal)remarks.MinRange && percentage <= (decimal)remarks.MaxRange)
                    {
                        obtGrade = remarks.Remarks;
                    }
                }
            }

            return obtGrade;
        }

        public static string GetRemarks(decimal obtained, decimal totalMarks)
        {
            var remarksList = RemarksConfigList();
            decimal Totalpercent = 100;
            decimal percentage = (obtained / totalMarks) * Totalpercent;
            string obtRemarks = "";
            percentage = FlorPercntage(percentage);

            if (remarksList != null && remarksList.Count() > 0)
            {
                foreach (var remarks in remarksList)
                {
                    if (percentage >= (decimal)remarks.MinRange && percentage <= (decimal)remarks.MaxRange)
                    {
                        obtRemarks = remarks.Remarks;
                    }
                }
            }

            return obtRemarks;
        }

        public static decimal FlorPercntage(decimal percentage)
        {
            decimal obtaindPercent = percentage;
            int florValue = (int)obtaindPercent;
            obtaindPercent = obtaindPercent - (decimal)florValue;
            if (obtaindPercent >= 0.5m)
                percentage = (decimal)(florValue + 1);
            else
                percentage = (decimal)florValue;

            return percentage;
        }


        public static List<Subject> SubjectList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_subjectList == null || _subjectList.Count == 0 || InvalidateSubjectCache == false)
            {
                subjectRepo = new SubjectRepositoryImp(new SC_WEBEntities2());
                _subjectList = subjectRepo.GetAllSubjectes();
                InvalidateSubjectCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _subjectList.Where(x => x.BranchId == branchId).ToList();
            //}

            return tempList;
        }

        public static List<RegisterCourseModel> ClassSubjectList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_classSubjectList == null || _classSubjectList.Count == 0 || InvalidateClassSubjectCache == false)
            {
                classSubjRepo = new ClassSubjectRepositoryImp(new SC_WEBEntities2());
                _classSubjectList = classSubjRepo.GetAllRegisterCoursesModel();
                InvalidateClassSubjectCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _classSubjectList.Where(x => x.BranchId == branchId).ToList();
            //}

            return tempList;
        }

        public static List<SmsMessage> SmsMessageList
        {
            get
            {
                if (_smsMessageList == null || _smsMessageList.Count == 0 || InvalidateBranchCache == false)
                {
                    feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
                    _smsMessageList = feePlanRepo.GetAllMessages();
                    InvalidateBranchCache = true;
                }
                return _smsMessageList;
            }
        }

        public static List<SmsMessage> RefreshSmsMessageList
        {
            get
            {
                feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
                _smsMessageList = feePlanRepo.GetAllMessages();
                InvalidateBranchCache = true;
                return _smsMessageList;
            }
        }

        public static List<SmsEvent> SmsEventList
        {
            get
            {
                if (_smsEventList == null || _smsEventList.Count == 0 || InvalidateBranchCache == false)
                {
                    feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
                    _smsEventList = feePlanRepo.GetAllSMSEvents();
                    InvalidateBranchCache = true;
                }
                return _smsEventList;
            }
        }

        public static List<SmsEvent> RefreshSmsEventList
        { 
            get
            {
                feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
                _smsEventList = feePlanRepo.GetAllSMSEvents();
                InvalidateBranchCache = true;
                return _smsEventList;
            }
        }

        public static List<SmsParam> SmsParamsList
        {
            get
            {
                if (_smsParamsList == null || _smsParamsList.Count == 0)
                {
                    feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
                    _smsParamsList = feePlanRepo.GetSmsParams();
                }
                return _smsParamsList;
            }
        }

        public static List<SmsVender> SmsVenderList
        {
            get
            {
                if (_smsVendersList == null || _smsVendersList.Count == 0)
                {
                    feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
                    _smsVendersList = feePlanRepo.GetAllSmsVenders();
                }
                return _smsVendersList;
            }
        }//_smsEventParamList

        public static List<SmsEventParam> SmsEventParamList
        {
            get
            {
                if (_smsEventParamList == null || _smsEventParamList.Count == 0)
                {
                    feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
                    _smsEventParamList = feePlanRepo.GetAllSmsEventParam();
                }
                return _smsEventParamList;
            }
        }

        public static List<string> StudentNameList(int branchId)
        {
            string tempBranchId = "," + branchId;
            if (_studentNameList == null || _studentNameList.Count == 0 || InvalidateStudentNameCache == false)
            {
                _studentNameList = studentRepo.GetStudentNameList();
                //for (int i = 0; i < 5; i++)
                //{
                //    _studentNameList = _studentNameList.Concat(_studentNameList).ToList();
                //}
                    InvalidateStudentNameCache = true;
            }

            return _studentNameList.Where(x => x.EndsWith(tempBranchId)).ToList();
        }

        public static List<string> FatherNameList()
        {
            if (_studentFatherList == null || _studentFatherList.Count == 0 || InvalidateFathernameCache == false)
            {
                _studentFatherList = studentRepo.GetFatherNameList();
                InvalidateFathernameCache = true;
            }

            return _studentFatherList;
        }

        public static List<AdmissionType> AdmissionTypeList
        {
            get
            {
                return _admissionTypeList;
            }
        }

        public static List<Gender> GenderList
        {
            get
            {
                return _genderList;
            }
        }

        public static List<StudentAttendanceStatu> AttendanceStatusList
        {
            get
            {
                return _attendanceStatusList;
            }
        }

        public static List<Relegion> RelegionList
        {
            get
            {
                return _religionList;
            }
        }

        public static List<Session> SessionList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_sessionList == null || _sessionList.Count == 0 || InvalidateSessionCache == false)
            {
                studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());
                _sessionList = studentRepo.GetAllSessions();
                InvalidateSessionCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _sessionList.Where(x => x.BranchId == branchId).ToList();
            //}

            return tempList;

        }

        public static List<SessionDD> SessionListDD(string browserDetail)
        {
            var ddList = SessionList(browserDetail);
            var query = from dd in ddList
                        select new SessionDD
                        {
                            Id = dd.Id,
                            Name = dd.Name
                        };
            return query.ToList();
        }

        public static List<LeavingReason> LeavingReasonList
        {
            get
            {
                return _reasonList;
            }
        }

        public static List<LeavingStatu> LeavingStatusList
        {
            get
            {
                return _statusList;
            }
        }

        public static List<string> GetStaffNames(string browserDetail)
        {
            var staffList = StaffList(browserDetail);

            return staffList.Select(x => x.StaffId.ToString().PadLeft(4, '0') + " | " + x.Name + " | " + x.PhoneNumber).ToList();
        }
        public static List<Staff> StaffList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_staffList == null || _staffList.Count == 0 || InvalidateStaffCache == false)
            {
                staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
                _staffList = staffRepo.GetAllStaff();
                _staffList = _staffList.Select(x => new Staff { StaffId = x.StaffId, Name = x.Name, PhoneNumber = x.PhoneNumber, BranchId = x.BranchId, DesignationId = x.DesignationId, IsLeft = x.IsLeft }).Where(x => x.IsLeft == null || x.IsLeft == false).ToList();
                InvalidateStaffCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _staffList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }

        public static List<StaffDD> StaffListDD(string browserDetail)
        {
            var ddList = StaffList(browserDetail);
            var query = from dd in ddList
                        select new StaffDD
                        {
                            StaffId = dd.StaffId,
                            Name = dd.Name
                        };
            return query.ToList();
        }

        public static List<Staff> TeacherList(string browserDetail, string designation)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_staffList == null || _staffList.Count == 0 || InvalidateStaffCache == false)
            {
                staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
                _staffList = staffRepo.GetAllStaff();
                _staffList = _staffList.Select(x => new Staff { StaffId = x.StaffId, Name = x.Name, BranchId = x.BranchId, DesignationId = x.DesignationId }).Where(x => x.IsLeft == null || x.IsLeft == false).ToList();
                InvalidateStaffCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _staffList.Where(x => x.BranchId == branchId).ToList();
            List<Staff> stList = new List<Staff>();
            //}
            var designList = dbContext.Designations.Where(x => x.Name.ToLower().Contains(designation)).ToList();
            foreach (var desing in designList)
            {
                stList.AddRange(tempList.Where(x => x.DesignationId == desing.Id).ToList());
            }
            //return tempList.Where(x => x.Designation.Name.ToLower().Contains(designation)).ToList();
            return stList;
        }

        public static List<StaffDD> TeacherListDD(string browserDetail, string designation)
        {
            var stList = TeacherList(browserDetail, designation);
            var query = from dd in stList
                        select new StaffDD
                        {
                            StaffId = dd.StaffId,
                            Name = dd.Name
                        };
            return query.ToList();
        }

        public static List<TestStatu> TestStatusList
        {
            get
            {
                return _testStatusList;
            }
        }

        public static List<Year> YearList
        {
            get
            {
                return _yearList;
            }
        }

        public static List<ResultType> ResultTypeList
        {
            get
            {
                return _resultTypeList;
            }
        }

        public static List<ExamType> ExamTypeList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_examTypeList == null || _examTypeList.Count == 0 || InvalidateExamTypeCache == false)
            {
                examRepo = new ExamRepositoryImp(new SC_WEBEntities2());
                _examTypeList = examRepo.GetAllExamTypes();
                InvalidateExamTypeCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _examTypeList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }

        public static List<ExamType> ExamTypeList(int branchId)
        {
            if (_examTypeList == null || _examTypeList.Count == 0 || InvalidateExamTypeCache == false)
            {
                examRepo = new ExamRepositoryImp(new SC_WEBEntities2());
                _examTypeList = examRepo.GetAllExamTypes();
                InvalidateExamTypeCache = true;
            }

            var tempList = _examTypeList.Where(x => x.BranchId == branchId).ToList();
            return tempList;
        }

        public static List<ExamTerm> ExamTermList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_examTermList == null || _examTermList.Count == 0 || InvalidateExamTermCache == false)
            {
                examRepo = new ExamRepositoryImp(new SC_WEBEntities2());
                _examTermList = examRepo.GetAllExamTerm();
                InvalidateExamTermCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
                var tempList = _examTermList.Where(x => x.BranchId == branchId).ToList();
            //}
                return tempList;
        }

        public static List<ExamTerm> ExamTermList(int branchId)
        {
            if (_examTermList == null || _examTermList.Count == 0 || InvalidateExamTermCache == false)
            {
                examRepo = new ExamRepositoryImp(new SC_WEBEntities2());
                _examTermList = examRepo.GetAllExamTerm();
                InvalidateExamTermCache = true;
            }

            var tempList = _examTermList.Where(x => x.BranchId == branchId).ToList();
            return tempList;
        }

        public static List<Activity> ActivityList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_activityList == null || _activityList.Count == 0 || InvalidateActivityCache == false)
            {
                examRepo = new ExamRepositoryImp(new SC_WEBEntities2());
                _activityList = examRepo.GetAllActivity();
                InvalidateExamTermCache = true;
            }

            var tempList = _activityList.Where(x => x.BranchId == branchId).ToList();
            return tempList;
        }

        public static List<Challan> ChallanList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_challanList == null || _challanList.Count == 0 || InvalidateChallanCache == false)
            {
                feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
                _challanList = feePlanRepo.GetAllChallan();
                InvalidateChallanCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _challanList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }

        public static List<Challan> ChallanList(int branchId)
        {
            if (_challanList == null || _challanList.Count == 0 || InvalidateChallanCache == false)
            {
                feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
                _challanList = feePlanRepo.GetAllChallan();
                InvalidateChallanCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _challanList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }

        public static List<FeeHead> FeeHeadList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_feeHeadList == null || _feeHeadList.Count == 0 || InvalidateFeeHeadCache == false)
            {
                feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
                _feeHeadList = feePlanRepo.GetAllFeeHeads();
                InvalidateFeeHeadCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _feeHeadList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }

        public static List<FeeHeadDD> FeeHeadListDD(string browserDetail)
        {
            var heads = FeeHeadList(browserDetail);
            var query = from dd in heads
                        select new FeeHeadDD
                        {
                            Id = dd.Id,
                            Name = dd.Name
                        };
            return query.ToList();
        }

        public static List<Month> MonthList
        {
            get
            {
                return _monthList;
            }
        }

        public static List<StudentPerChallan> StudentPerChallanList
        {
            get
            {
                return _studentPerChallans;
            }
        }

        public static string GetMonthName(int monthId)
        {
            return MonthList.Where(x => x.Id == monthId).FirstOrDefault().Month1;
        }

        public static int GetMonthID(string monthName)
        {
            return MonthList.Where(x => x.Month1 == monthName).FirstOrDefault().Id;
        }
        public static int GetYearID(string yearName)
        {
            return YearList.Where(x => x.Year1 == yearName).FirstOrDefault().Id;
        }

        public static List<AccountType> AccountTypeList
        {
            get
            {
                //return _accountTypeList.OrderByDescending(x => x.ID).ToList();
                //return _accountTypeList.OrderBy(x => x.ID).ToList();
                return _accountTypeList.Where(x => x.ID == 2).OrderBy(x => x.ID).ToList();
            }
        }
        public static List<FinanceFourthLvlAccount> FeeAccountDetailList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_feeAccountDetailList == null || _feeAccountDetailList.Count == 0 || InvalidateFeeAccountDetailCache == false)
            {
                feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
                _feeAccountDetailList = feePlanRepo.GetFeeAccountDetails(branchId);
                InvalidateFeeAccountDetailCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _feeAccountDetailList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }

        public static List<FinanceFifthLvlAccountModel> FeeFinanceAccountList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_feeFinanceAccountList == null || _feeFinanceAccountList.Count == 0 || InvalidateFeeFinanceAccountCache == false)
            {
                financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
                _feeFinanceAccountList = financeRepo.GetFeeFinanceAccountsModel(branchId);
                InvalidateFeeFinanceAccountCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _feeFinanceAccountList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }

        public static List<FinanceFifthLvlAccountModel> FeeFinanceAccountList(int branchId)
        {
            if (_feeFinanceAccountList == null || _feeFinanceAccountList.Count == 0 || InvalidateFeeFinanceAccountCache == false)
            {
                financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
                _feeFinanceAccountList = financeRepo.GetFeeFinanceAccountsModel(branchId);
                InvalidateFeeFinanceAccountCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _feeFinanceAccountList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }


        public static List<FinanceFifthLvlAccountModel> FeeCashAccountList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_feeFinanceAccountList == null || _feeFinanceAccountList.Count == 0 || InvalidateFeeFinanceAccountCache == false)
            {
                financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
                _feeFinanceAccountList = financeRepo.GetFinanceAccountsFifthLvlModel("Cash");
                InvalidateFeeFinanceAccountCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _feeFinanceAccountList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }

        public static List<BankAccount> BankAccountList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_bankAccountList == null || _bankAccountList.Count == 0 || InvalidateBankAccountCache == false)
            {
                feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
                _bankAccountList = feePlanRepo.GetAllBankAccount();
                InvalidateBankAccountCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _bankAccountList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }

        public static List<FinanceFirstLvlAccount> FinanceFirstLvlAccountList
        {
            get
            {
                if (_firstLvlAccountList == null || _firstLvlAccountList.Count == 0 || InvalidateFinanceFirstLvlCache == false)
                {
                    accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
                    _firstLvlAccountList = accountRepo.GetAllFinanceFirstLvlAccounts();
                    InvalidateFinanceFirstLvlCache = true;
                }
                return _firstLvlAccountList;
            }
        }

        public static List<FinanceFirstLvlAccount> FinanceFirstLvlAccountListByNames()
        {
            return FinanceFirstLvlAccountList.OrderBy(x => x.AccountName).ToList();
        }

        public static List<FinanceSeccondLvlAccountModel> FinanceSeccondLvlAccountList
        {
            get
            {
                if (_seccondLvlAccountList == null || _seccondLvlAccountList.Count == 0 || InvalidateFinanceSeccondLvlCache == false)
                {
                    accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
                    _seccondLvlAccountList = accountRepo.GetAllFinanceSeccondLvlAccountsModel();
                    InvalidateFinanceSeccondLvlCache = true;
                }
                return _seccondLvlAccountList;
            }
        }

        public static List<FinanceSeccondLvlAccountModel> FinanceSeccondLvlAccountListByName()
        {
            return FinanceSeccondLvlAccountList.OrderBy(x => x.AccountName).ToList();
        }

        public static List<FinanceThirdLvlAccountModel> FinanceThirdLvlAccountList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_thirdLvlAccountList == null || _thirdLvlAccountList.Count == 0 || InvalidateFinanceThirdLvlCache == false)
            {
                accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
                _thirdLvlAccountList = accountRepo.GetAllFinanceThirdLvlAccountsModel();
                InvalidateFinanceThirdLvlCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            //var tempList = _thirdLvlAccountList.Where(x => x.BranchId == branchId).ToList();
            //}
            return _thirdLvlAccountList;
        }

        public static List<FinanceThirdLvlAccountModel> FinanceThirdLvlAccountListByName(string browserDetail)
        {
            return FinanceThirdLvlAccountList(browserDetail).OrderBy(x => x.AccountName).ToList();
        }

        public static List<FinanceFourthLvlAccountModel> FinanceFourthLvlAccountList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_fourthLvlAccountList == null || _fourthLvlAccountList.Count == 0 || InvalidateFinanceFourthLvlCache == false)
            {
                accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
                _fourthLvlAccountList = accountRepo.GetAllFinanceFourthLvlAccountsModel();
                InvalidateFinanceFourthLvlCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _fourthLvlAccountList.Where(x => x.BranchId == branchId).ToList();
            tempList = tempList.Where(x => x.IsPettyCashAccount == false).ToList();
            //}
            return tempList;
        }

        public static List<FinanceFourthLvlAccountModel> PettyCashFourthLvlAccounts(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_fourthLvlAccountList == null || _fourthLvlAccountList.Count == 0 || InvalidateFinanceFourthLvlCache == false)
            {
                accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
                _fourthLvlAccountList = accountRepo.GetAllFinanceFourthLvlAccountsModel();
                InvalidateFinanceFourthLvlCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _fourthLvlAccountList.Where(x => x.BranchId == branchId).ToList();
            //}
            tempList = tempList.Where(x => x.IsPettyCashAccount == true).ToList();
            return tempList;
        }

        public static List<FinanceFourthLvlAccountModel> FinanceFourthLvlAccountListByName(string browserDetail)
        {
            return FinanceFourthLvlAccountList(browserDetail).OrderBy(x => x.AccountName).ToList();
        }

        public static List<FinanceFourthLvlAccountModel> FourthLvlCapitalAccountList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_fourthLvlAccountList == null || _fourthLvlAccountList.Count == 0)
            {
                accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
                _fourthLvlAccountList = accountRepo.GetAllFinanceFourthLvlAccountsModel();
            }

            var tempList = _fourthLvlAccountList.Where(x => x.BranchId == branchId && x.ThirdLvlAccountId == ConstHelper.CAPITAL_ACCOUNT_ID).ToList();
            return tempList;
        }

        public static List<FinanceFifthLvlAccountModel> FinanceFifthLvlAccountList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_fifthLvlAccountList == null || _fifthLvlAccountList.Count == 0 || InvalidateFinanceFifthLvlCache == false)
            {
                accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
                _fifthLvlAccountList = accountRepo.GetAllFinanceFifthLvlAccountsModel();
                FinanceFifthLvlAccountListWitoutReceipts(browserDetail);
                InvalidateFinanceFifthLvlCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _fifthLvlAccountList.Where(x => x.BranchId == branchId).ToList();
            tempList = tempList.Where(x => x.IsPettyCashAccount == false).ToList();
            //}
            return tempList;
        }

        public static List<FinanceFifthLvlAccountModel> PettyCashFifthLvlAccount(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_fifthLvlAccountList == null || _fifthLvlAccountList.Count == 0 || InvalidateFinanceFifthLvlCache == false)
            {
                accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
                _fifthLvlAccountList = accountRepo.GetAllFinanceFifthLvlAccountsModel();
                FinanceFifthLvlAccountListWitoutReceipts(browserDetail);
                InvalidateFinanceFifthLvlCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _fifthLvlAccountList.Where(x => x.BranchId == branchId).ToList();
            tempList = tempList.Where(x => x.IsPettyCashAccount == true).ToList();
            //}
            return tempList;
        }

        public static List<FinanceFifthLvlAccountModel> AnmolPettyCashFifthLvlAccount(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_fifthLvlAccountList == null || _fifthLvlAccountList.Count == 0 || InvalidateFinanceFifthLvlCache == false)
            {
                accountRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
                _fifthLvlAccountList = accountRepo.GetAllFinanceFifthLvlAccountsModel();
                FinanceFifthLvlAccountListWitoutReceipts(browserDetail);
                InvalidateFinanceFifthLvlCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _fifthLvlAccountList.Where(x => x.BranchId == branchId).ToList();
            tempList = tempList.Where(x => x.IsPettyCashAccount == true).ToList();
            tempList = tempList.Where(x => x.FinanceFourthLvlAccountAccountName == "Anmol Accounts").ToList();
            //}
            return tempList;
        }

        public static List<FinanceFifthLvlAccountModel> FinanceFifthLvlAccountListByName(string browserDetail)
        {
            return FinanceFifthLvlAccountList(browserDetail).OrderBy(x => x.AccountName).ToList();
        }

        public static List<FinanceFifthLvlAccountModel> FifthCapitalLvlAccountList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            
                SC_WEBEntities2 contect = new SC_WEBEntities2();
                var query = from detail in contect.FinanceFifthLvlAccounts
                            join subsub in contect.FinanceFourthLvlAccounts on detail.FourthLvlAccountId equals subsub.Id
                            join sub in contect.FinanceThirdLvlAccounts on subsub.ThirdLvlAccountId equals sub.Id
                            join main in contect.FinanceSeccondLvlAccounts on sub.SeccondLvlAccountId equals main.Id
                            join top in contect.FinanceFirstLvlAccounts on main.FirstLvlAccountId equals top.Id
                            where subsub.ThirdLvlAccountId == ConstHelper.CAPITAL_ACCOUNT_ID
                            && detail.BranchId == branchId
                            select new FinanceFifthLvlAccountModel
                            {
                                AccountDescription = detail.AccountDescription, 
                                AccountName = detail.AccountName,
                                BranchId = detail.BranchId,
                                FinanceFourthLvlAccountAccountName = subsub.AccountName,
                                FirstLvlAccountId = top.Id,
                                FourthLvlAccountId = subsub.Id,
                                Id = detail.Id,
                                SeccondLvlAccountId = main.Id,
                                ThirdLvlAccountId = sub.Id,
                                Value = (int)(detail.Value == null ? 0 : detail.Value)

                            };
            var tempList = query.ToList();
            return tempList;
        }

        public static List<FinanceFifthLvlAccountModel> FinanceFifthLvlAccountListWitoutReceipts(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_fifthLvlAccountListWithoutReceipts == null || _fifthLvlAccountListWithoutReceipts.Count == 0 || InvalidateFinanceFifthLvlCache == false)
            {
                SC_WEBEntities2 contect = new SC_WEBEntities2();

                var query = from fifth in contect.FinanceFifthLvlAccounts
                            join fourth in contect.FinanceFourthLvlAccounts on fifth.FourthLvlAccountId equals fourth.Id
                            join third in contect.FinanceThirdLvlAccounts on fourth.ThirdLvlAccountId equals third.Id
                            join second in contect.FinanceSeccondLvlAccounts on third.SeccondLvlAccountId equals second.Id
                            where second.Id != 28
                            select new FinanceFifthLvlAccountModel
                            {
                                AccountDescription = fifth.AccountDescription,
                                AccountName = fifth.AccountName,
                                FourthLvlAccountId = fourth.Id,
                                BranchId = fourth.BranchId,
                                FirstLvlAccountId = second.FirstLvlAccountId,
                                Id = fifth.Id,
                                SeccondLvlAccountId = second.Id,
                                ThirdLvlAccountId = third.Id
                            };

                //var query = from detail in contect.FinanceFifthLvlAccounts
                //            join subsub in contect.FinanceFourthLvlAccounts on detail.FourthLvlAccountId equals subsub.Id
                //            join sub in contect.FinanceThirdLvlAccounts on subsub.ThirdLvlAccountId equals sub.Id
                //            where sub.SeccondLvlAccountId != 28
                //            select detail;
                _fifthLvlAccountListWithoutReceipts = query.ToList();
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _fifthLvlAccountListWithoutReceipts.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }

        

        public static List<Designation> DesignationList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_designationList == null || _designationList.Count == 0 || InvalidateDesignationCache == false)
            {
                staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
                _designationList = staffRepo.GetAllDesignations();
                InvalidateDesignationCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _designationList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }

        public static List<StaffHoliday> StaffHolidayList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_staffHolidayList == null || _staffHolidayList.Count == 0 || InvalidateStaffHolidayCache == false)
            {
                staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
                _staffHolidayList = staffRepo.GetAllStaffHoliday();
                InvalidateStaffHolidayCache = true;
            }

            var tempList = _staffHolidayList.Where(x => x.BranchId == branchId).ToList();
            return tempList;
        }

        public static List<DesignationCatagory> DesignationCatagoryList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_designationCategoryList == null || _designationCategoryList.Count == 0 || InvalidateDesignationCategoryCache == false)
            {
                staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
                _designationCategoryList = staffRepo.GetAllDesignationCategories();
                InvalidateDesignationCategoryCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
                var tempList = _designationCategoryList.Where(x => x.BranchId == branchId).ToList();
            //}
                return tempList;
        }

        public static List<DesignationCatagoryDD> DesignationCatagoryListDD(string browserDetail)
        {
            var ddList = DesignationCatagoryList(browserDetail);
            var query = from dd in ddList
                        select new DesignationCatagoryDD
                        {
                            Id = dd.Id,
                            CatagoryName = dd.CatagoryName
                        };
            return query.ToList();
        }

        public static List<StaffAttandancePolicyModel> StaffAttandancePolicyList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_staffAttandancePolicyList == null || _staffAttandancePolicyList.Count == 0 || InvalidateAttendancePolicyCache == false)
            {
                staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
                _staffAttandancePolicyList = staffRepo.GetAllStaffAttandancePoliciesModel();
                InvalidateAttendancePolicyCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _staffAttandancePolicyList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }

        public static List<AllowLeave> AllowLeaveList
        {
            get
            {
                return _allowLeaveList;
            }
        }

        public static List<LateInCount> LateInCountList
        {
            get
            {
                return _lateInCountList;
            }
        }

        public static List<TransportStop> transportStopList
        {
            get
            {
                if (_transportStopList == null || _transportStopList.Count == 0 || InvalidateTransportCache == false)
                {
                    treansRepo = new TransportRepositoryImp(new SC_WEBEntities2());
                    _transportStopList = treansRepo.GetAllTransportStops();
                    InvalidateBranchCache = true;
                }
                return _transportStopList;
            }
        }

        public static List<TransportDriver> transportDriverList
        {
            get
            {
                if (_transportDriverList == null || _transportDriverList.Count == 0 || InvalidateTransportDriverCache == false)
                {
                    treansRepo = new TransportRepositoryImp(new SC_WEBEntities2());
                    _transportDriverList = treansRepo.GetAllTransportDrivers();
                    InvalidateBranchCache = true;
                }
                return _transportDriverList;
            }
        }

        public static List<Allownce> AllownceList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_allownceList == null || _allownceList.Count == 0 || InvalidateAllownceCache == false)
            {
                staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());
                _allownceList = staffRepo.GetAllAllownces();
                InvalidateAllownceCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _allownceList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }

        public static List<MeritalStatu> MeritalStatusList
        {
            get
            {
                return _meritalStatusList;
            }
        }

        public static List<StaffType> StaffTypeList
        {
            get
            {
                return _staffTypeList;
            }
        }

        public static List<User> UserList(string browserDetail)
        {
            int branchId = UserPermissionController.GetLoginBranchId(browserDetail);
            if (_userList == null || _userList.Count == 0 || InvalidateUserCache == false)
            {
                securityRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
                _userList = securityRepo.GetAllUsers();
                InvalidateUserCache = true;
            }

            //if (branchId != ConstHelper.LOGIN_BRNACH_ID)
            //{
            var tempList = _userList.Where(x => x.BranchId == branchId).ToList();
            //}
            return tempList;
        }

        public static List<ExamResultViewModel> GetClassPosition(List<ExamResultViewModel> classResult)
        {
            decimal MaximumMarks = 0;

            foreach (var std in classResult)
            {
                decimal tempMax = classResult.Where(x => x.StudentId == std.StudentId).Sum(x => x.Obtained);
                decimal tempTotalMax = classResult.Where(x => x.StudentId == std.StudentId).Sum(x => x.ActualMarks);
                decimal actualPercent = Math.Round((tempMax * 100m) / tempTotalMax, 2);
                std.actualPercentage = actualPercent.ToString();
                if (tempMax > MaximumMarks)
                    MaximumMarks = tempMax;
                std.TotalObtained = tempMax;
            }

            List<decimal> positionList = new List<decimal>();
            foreach (var result in classResult)
            {
                //decimal stdObtained = classResult.Where(x => x.StudentId == std.Id).Sum(x => x.Obtained);
                result.Maximum = MaximumMarks;
                decimal prcc = (result.TotalObtained * 100) / result.Maximum;
                result.Percentage = Math.Round((result.TotalObtained * 100) / result.Maximum, 2);
                if (positionList.Count == 0)
                    positionList.Add(result.Percentage);
                else
                {
                    if (positionList.IndexOf(result.Percentage) == -1)
                        positionList.Add(result.Percentage);
                }
            }

            positionList = positionList.OrderByDescending(i => i).ToList();

            foreach (var std in classResult)
            {
                std.Position = positionList.IndexOf(std.Percentage) + 1;
            }

            return classResult;
        }


        public static int getCardFontSize(string text)
        {
            int fontSize = 20;
            if (text.Length >= 40)
                fontSize = 16;
            if (text.Length >= 25)
                fontSize = 18;
            return fontSize;
        }

        public static DataSet GetClassTablePosition(DataSet dataSet)
        {
            foreach (DataTable table in dataSet.Tables)
            {
                List<decimal> positionList = new List<decimal>();
                foreach (DataRow row in table.Rows)
                {
                    decimal percentage = 0;
                    var total = row["Total"].ToString();
                    if (!string.IsNullOrEmpty(total) && total.Length > 0)
                    {
                        percentage = decimal.Parse(total);
                        positionList.Add(percentage);
                    }
                    else
                        break;
                }

                positionList = positionList.OrderByDescending(i => i).ToList();

                foreach (DataRow row in table.Rows)
                {
                    decimal percentage = 0;
                    var total = row["Total"].ToString();
                    if (!string.IsNullOrEmpty(total) && total.Length > 0)
                    {
                        percentage = decimal.Parse(total);
                        row["Position"] = positionList.IndexOf(percentage) + 1;
                    }
                    else
                        break;
                }
            }
            return dataSet;
        }

        public static double ConvertToInchesQty(int unitId, double quantity)
        {
            double resultQty = 0;

            if (unitId == ConstHelper.METER_UNIT)
            {
                resultQty = quantity * ConstHelper.METER_TO_INCHES;
            }
            else if (unitId == ConstHelper.FOOT_UNIT)
            {
                resultQty = quantity * ConstHelper.FOOT_TO_INCHES;
            }
            else
            {
                resultQty = quantity;
            }
            return Math.Round(resultQty, 2);
        }

        public static double ConvertFromInchesQty(int unitId, double quantity)
        {
            double resultQty = 0;

            if (unitId == ConstHelper.METER_UNIT)
            {
                resultQty = quantity * ConstHelper.INCHES_TO_METER;
            }
            else if (unitId == ConstHelper.FOOT_UNIT)
            {
                resultQty = quantity * ConstHelper.INCHES_TO_FOOT;
            }
            else
            {
                resultQty = quantity;
            }

            return Math.Round(resultQty, 2);
        }


    }
}