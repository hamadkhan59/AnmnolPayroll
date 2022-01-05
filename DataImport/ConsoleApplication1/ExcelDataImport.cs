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
    public class ExcelDataImport
    {
        static string FilePath = "";
        private static IClassRepository classRepo;
        private static IFinanceAccountRepository financeRepo;
        private static ISectionRepository secRepo;
        private static IClassSectionRepository classSectionRepo;
        private static IFeePlanRepository feeRepo;
        private static IStudentRepository studentRepo;
        private static List<int> DeleteChallanList;
        private const string CAT_FEE_RECEIVABLE = "Fee Receivables";
        private const int CAT_RECEIVABLES = 1023;
        private static int BRANCH_ID = 1023;
        private static string IS_CHALLAN_CREATED = "";
        private static string SCHOOL_NAME = "";
        private static string DUMMY_MOBILE = "";

        public static void InitializeExcelImport(string filePath)
        {
            FilePath = filePath;
            ExcelHelper.InitializeExcelHelper(FilePath);
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            classSectionRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            feeRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());
            DeleteChallanList = new List<int>();
            string branchId = ConfigurationManager.AppSettings["branchId"];
            IS_CHALLAN_CREATED = ConfigurationManager.AppSettings["isChallanCreated"];
            SCHOOL_NAME = ConfigurationManager.AppSettings["SchoolName"];
            DUMMY_MOBILE = ConfigurationManager.AppSettings["DummyMobile"];
            BRANCH_ID = int.Parse(branchId);
        }

        public static int ImportClassData()
        {
            string columnsValue = ConfigurationManager.AppSettings["class-columns"];
            string[] columnsList = columnsValue.Split('|');
            int rowCount = ExcelHelper.RowCount();
            string className = "", classDescription = "";
            int isActive = int.Parse(ConfigurationManager.AppSettings["isActive"]);

            Console.WriteLine("=======================================================");
            Console.WriteLine("             Class data Import Started");
            Console.WriteLine("=======================================================");

            for (int i = 2; i <= rowCount; i++)
            {
                for (int j = 0; j < columnsList.Count(); j++)
                {
                    if (columnsList[j] == "NAME")
                    {
                        className = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "DESCRIPTION")
                    {
                        classDescription = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "IS_ACTIVE")
                    {
                        isActive = 0;
                        string tempValue = ExcelHelper.GetCellValue(i, j + 1);
                        if (tempValue != null && tempValue.Length > 0)
                        {
                            isActive = int.Parse(tempValue);
                        }
                    }
                }


                Class clas = new Class();
                clas.Name = className;
                clas.Description = classDescription;
                clas.IsActive = (isActive > 0 ? true : false);
                clas.BranchId = BRANCH_ID;
                clas.IsFinanceAccountOpen = true;
                var classObj = classRepo.GetClassByName(clas.Name, BRANCH_ID);
                if (classObj == null)
                {
                    createFinanceAccount(clas.Name, BRANCH_ID);
                    int returnCode = classRepo.AddClass(clas);
                }
                else
                {
                    classObj.Name = className;
                    classObj.Description = classDescription;
                    classObj.IsActive = (isActive > 0 ? true : false);
                    classObj.BranchId = clas.BranchId;
                    classRepo.UpdateClass(classObj);
                }
            }

            Console.WriteLine("Class data import is finished");
            Console.WriteLine("Class data import count : " + (rowCount - 1).ToString());
            //Console.WriteLine("_______________________________________________________");

            ExcelHelper.CloseExcelSheets();
            return rowCount - 1;
        }

        public static int ImportSectionData()
        {
            string columnsValue = ConfigurationManager.AppSettings["section-columns"];
            string[] columnsList = columnsValue.Split('|');
            int rowCount = ExcelHelper.RowCount();
            string sectionName = "", sectionDescription = "";
            int isActive = int.Parse(ConfigurationManager.AppSettings["isActive"]);

            Console.WriteLine("=======================================================");
            Console.WriteLine("            Section data Import Started");
            Console.WriteLine("=======================================================");

            for (int i = 2; i <= rowCount; i++)
            {
                for (int j = 0; j < columnsList.Count(); j++)
                {
                    if (columnsList[j] == "NAME")
                    {
                        sectionName = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "DESCRIPTION")
                    {
                        sectionDescription = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "IS_ACTIVE")
                    {
                        isActive = 0;
                        string tempValue = ExcelHelper.GetCellValue(i, j + 1);
                        if (tempValue != null && tempValue.Length > 0)
                        {
                            isActive = int.Parse(tempValue);
                        }
                    }
                }

                Section section = new Section();
                section.Name = sectionName;
                section.Description = sectionDescription;
                section.IsActive = (isActive > 0 ? true : false);
                section.BranchId = BRANCH_ID;
                var secObj = secRepo.GetSectionByName(section.Name, BRANCH_ID);
                if (secObj != null)
                {
                    secObj.Name = section.Name;
                    secObj.Description = section.Description;
                    secObj.Description = section.Description;
                    secObj.BranchId = section.BranchId;
                    secRepo.UpdateSection(secObj);
                }
                else
                {
                    int returnStatus = secRepo.AddSection(section);
                }
            }

            Console.WriteLine("Section data import is finished");
            Console.WriteLine("Section data import count : " + (rowCount - 1).ToString());
            //Console.WriteLine("_______________________________________________________");

            ExcelHelper.CloseExcelSheets();
            return rowCount - 1;
        }

        private static int createFinanceAccount(string className, int branchId)
        {
            try
            {
                var financeObj = financeRepo.GetFinanceThirdLvlAccountByName(className, branchId);
                if (financeObj == null)
                {
                    FinanceThirdLvlAccount ftla = new FinanceThirdLvlAccount();
                    ftla.AccountName = className;
                    ftla.AccountDescription = "This account is created for class " + className + " fee collection";
                    ftla.CreatedOn = DateTime.Now;
                    ftla.SeccondLvlAccountId = 28;
                    ftla.BranchId = branchId;
                    financeRepo.AddFinanceThirdLvlAccount(ftla);
                    return ftla.Id;
                }
            }
            catch (Exception exc)
            {
            }
            return 0;
        }

        public static int ImportClassSectionData()
        {
            string columnsValue = ConfigurationManager.AppSettings["class-section-columns"];
            string[] columnsList = columnsValue.Split('|');
            int rowCount = ExcelHelper.RowCount();
            string className = "", sectionName = "";

            Console.WriteLine("=======================================================");
            Console.WriteLine("         Class Section data Import Started");
            Console.WriteLine("=======================================================");

            for (int i = 2; i <= rowCount; i++)
            {
                for (int j = 0; j < columnsList.Count(); j++)
                {
                    if (columnsList[j] == "CLASS_NAME")
                    {
                        className = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "SECTION_NAME")
                    {
                        sectionName = ExcelHelper.GetCellValue(i, j + 1);
                    }
                }

                ClassSection classSec = new ClassSection();
                if (className.Length > 0)
                {
                    classSec.ClassId = classRepo.GetClassByName(className, BRANCH_ID).Id;
                    classSec.SectionId = secRepo.GetSectionByName(sectionName, BRANCH_ID).Id;
                    classSec.IsFinanceAccountOpen = true;
                    var tempObj = classSectionRepo.GetClassSectionByClassAndSectionId((int)classSec.ClassId, (int)classSec.SectionId);
                    if (tempObj == null)
                    {
                        int returnStatus = classSectionRepo.AddClassSection(classSec);
                    }
                    CreateFinanceAccount((int)classSec.ClassId, (int)classSec.SectionId, BRANCH_ID);
                }
            }

            Console.WriteLine("Class Section data import is finished");
            Console.WriteLine("Class Section data import count : " + (rowCount - 1).ToString());
            //Console.WriteLine("_______________________________________________________");

            ExcelHelper.CloseExcelSheets();
            return rowCount - 1;
        }

        private static int CreateFinanceAccount(int classId, int sectionId, int branchId)
        {
            string className = classRepo.GetClassById(classId).Name;
            string sectionName = secRepo.GetSectionById(sectionId).Name;
            string fourthLvlAccountName = className + ", Section : " + sectionName;
            FinanceFourthLvlAccount accounts = new FinanceFourthLvlAccount();
            var thirdLvlObj = financeRepo.GetFinanceThirdLvlAccountByName(className, branchId);
            if (thirdLvlObj == null)
                return -59;
            else
                accounts.ThirdLvlAccountId = thirdLvlObj.Id;

            var fourthLvlAccountTemp = financeRepo.GetFinanceFourthLvlAccountByName(fourthLvlAccountName, branchId);

            if (fourthLvlAccountTemp == null)
            {
                accounts.AccountName = fourthLvlAccountName;
                accounts.AccountDescription = "This account is created for section " + sectionName + " fee collection";
                accounts.CreatedOn = DateTime.Now;
                accounts.Value = 0;// int.Parse(this.txtValue.Text.ToString());
                accounts.BranchId = branchId;
                financeRepo.AddFinanceFourthLvlAccount(accounts);
                List<FeeHead> feeHeadList = feeRepo.GetAllFeeHeads();
                foreach (FeeHead head in feeHeadList)
                {
                    FinanceFifthLvlAccount accountsFifthLvl = new FinanceFifthLvlAccount();
                    var fifthLvlObj = financeRepo.GetFinanceFifthLvlAccountByName(head.Name, branchId);

                    if (fifthLvlObj == null || fifthLvlObj.FourthLvlAccountId != accounts.Id)
                    {
                        accountsFifthLvl.AccountName = head.Name;
                        accountsFifthLvl.AccountDescription = "This account is created for fee head " + head.Name + " fee collection for " + accounts.AccountName;
                        accountsFifthLvl.CreatedOn = DateTime.Now;
                        accountsFifthLvl.Value = 0;
                        accountsFifthLvl.Count = 0;
                        accountsFifthLvl.FourthLvlAccountId = accounts.Id;
                        accountsFifthLvl.BranchId = branchId;
                        financeRepo.AddFinanceFifthLvlAccount(accountsFifthLvl);
                    }
                }
                return accounts.Id;
            }
            else
            {
                List<FeeHead> feeHeadList = feeRepo.GetAllFeeHeads();
                foreach (FeeHead head in feeHeadList)
                {
                    FinanceFifthLvlAccount accountsFifthLvl = new FinanceFifthLvlAccount();
                    var fifthLvlObj = financeRepo.GetFinanceFifthLvlAccount(head.Name, fourthLvlAccountTemp.Id);

                    if (fifthLvlObj == null || fifthLvlObj.FourthLvlAccountId != accounts.Id)
                    {
                        accountsFifthLvl.AccountName = head.Name;
                        accountsFifthLvl.AccountDescription = "This account is created for fee head " + head.Name + " fee collection for " + accounts.AccountName;
                        accountsFifthLvl.CreatedOn = DateTime.Now;
                        accountsFifthLvl.Value = 0;
                        accountsFifthLvl.Count = 0;
                        accountsFifthLvl.FourthLvlAccountId = fourthLvlAccountTemp.Id;
                        accountsFifthLvl.BranchId = branchId;
                        financeRepo.AddFinanceFifthLvlAccount(accountsFifthLvl);
                    }
                }
            }
            return fourthLvlAccountTemp.Id;
        }


        public static int ImportStudentData()
        {
            string columnsValue = ConfigurationManager.AppSettings["student-columns"];
            string[] columnsList = columnsValue.Split('|');
            string feeHeads = ConfigurationManager.AppSettings["fee-challan-columns"];
            string[] feeHeadsList = feeHeads.Split('|');

            string arrearHeads = ConfigurationManager.AppSettings["arrears-columns"];
            arrearHeads = arrearHeads.Replace("(", "");
            arrearHeads = arrearHeads.Replace(")", "");
            string[] arrearHeadsList = arrearHeads.Split('|');

            AddFeeHeads();

            int rowCount = ExcelHelper.RowCount();
            //int rowCount = 24;

            Console.WriteLine("=======================================================");
            Console.WriteLine("            Student data Import Started");
            Console.WriteLine("=======================================================");

            int actualCount = 0;
            for (int i = 500; i < rowCount; i++)
            {
                int classId = 0, sectionId = 0;
                Student student = new Student();
                student.IsPromoted = false;
                for (int j = 0; j < columnsList.Count(); j++)
                {
                    if (columnsList[j] == "ROLL_NO")
                    {
                        student.RollNumber = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "NAME")
                    {
                        student.Name = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "FATHER_NAME")
                    {
                        student.FatherName = ExcelHelper.GetCellValue(i, j + 1);
                        if (string.IsNullOrEmpty(student.FatherName))
                        {
                            student.FatherName = student.Name;
                        }
                    }
                    else if (columnsList[j] == "CLASS")
                    {
                        classId = classRepo.GetClassByName(ExcelHelper.GetCellValue(i, j + 1), BRANCH_ID).Id;
                    }
                    else if (columnsList[j] == "SECTION")
                    {
                        sectionId = secRepo.GetSectionByName(ExcelHelper.GetCellValue(i, j + 1), BRANCH_ID).Id;
                    }
                    else if (columnsList[j] == "DATE_OF_BIRTH")
                    {
                        var dateValue = ExcelHelper.GetCellValue(i, j + 1);
                        if (dateValue != null && dateValue.Length > 0)
                        {
                            if (dateValue.Contains("/"))
                            {
                                student.DateOfBirth = GetDate(dateValue);
                            }
                            else
                            {
                                double date = double.Parse(ExcelHelper.GetCellValue(i, j + 1));
                                student.DateOfBirth = DateTime.FromOADate(date);
                            }
                            //student.DateOfBirth = GetDate(dateValue);
                            //student.DateOfBirth =  DateTime.Parse();
                        }
                        else
                        {
                            student.DateOfBirth = DateTime.Now;
                        }
                    }
                    else if (columnsList[j] == "ADMISSION_DATE")
                    {
                        var dateValue = ExcelHelper.GetCellValue(i, j + 1);
                        if (dateValue != null && dateValue.Length > 0)
                        {
                            if (dateValue.Contains("/"))
                            {
                                student.AdmissionDate = GetDate(dateValue);
                            }
                            else
                            {
                                double date = double.Parse(ExcelHelper.GetCellValue(i, j + 1));
                                student.AdmissionDate = DateTime.FromOADate(date);
                            }
                            student.LeavingDate = student.AdmissionDate;
                            //student.AdmissionDate = DateTime.Parse(ExcelHelper.GetCellValue(i, j + 1));
                        }
                        else
                        {
                            student.DateOfBirth = DateTime.Now;
                        }
                    }
                    else if (columnsList[j] == "CONTACT_1")
                    {
                        student.Contact_1 = ExcelHelper.GetCellValue(i, j + 1);
                        student.Contact_1 = student.Contact_1.Trim();
                        if (string.IsNullOrEmpty(student.Contact_1))
                        {
                            student.Contact_1 = DUMMY_MOBILE;
                        }
                    }
                    else if (columnsList[j] == "CONTACT_2")
                    {
                        student.Contact_2 = ExcelHelper.GetCellValue(i, j + 1);
                        student.Contact_2 = student.Contact_2.Trim();
                        if (string.IsNullOrEmpty(student.Contact_2))
                        {
                            student.Contact_2 = DUMMY_MOBILE;
                        }
                    }
                    else if (columnsList[j] == "CURRENT_ADDRESS")
                    {
                        student.CurrentAddress = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "RELIGION")
                    {
                        student.ReligionCode = studentRepo.GetAllReligion().Where(x => x.Name == ExcelHelper.GetCellValue(i, j + 1)).FirstOrDefault().Id;
                    }
                    else if (columnsList[j] == "GENDER")
                    {
                        student.GenderCode = studentRepo.GetAllGenders().Where(x => x.Gender1 == ExcelHelper.GetCellValue(i, j + 1)).FirstOrDefault().Id;
                    }
                    else if (columnsList[j] == "NATIONALITY")
                    {
                        student.Nationality = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "FATHER_CNIC")
                    {
                        student.FatherCNIC = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "MOTHER_NAME")
                    {
                        student.MotherName = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "MOTHER_CONTACT")
                    {
                        student.MotherContact1 = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "MOTHER_CNIC")
                    {
                        student.MotherCnic = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "BFORMNO")
                    {
                        student.BFormNo = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "ADMISSION_NO")
                    {
                        student.AdmissionNo = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    //else if (columnsList[j] == "SESSION_YEAR")
                    //{
                    //    student.SessionYear = ExcelHelper.GetCellValue(i, j + 1);
                    //    if (student.SessionYear == null || student.SessionYear.Length == 0)
                    //        student.SessionYear = "2018";
                    //    student.SessionId = staffMang.GetSessionIdByName(student.SessionYear);
                    //}
                    
                }
                if (student.AdmissionDate == null)
                    student.AdmissionDate = DateTime.Now;
                student.SrNo = "SC-" + DateTime.Now.Year.ToString() + "-" + "1159" + studentRepo.GetMaxSrNo();
                student.AdmissionNo = string.IsNullOrEmpty(student.AdmissionNo) == true ? studentRepo.GetMaxAdmissionNo() : student.AdmissionNo;

                student.ClassSectionId = classSectionRepo.GetClassSectionId(classId, sectionId);
                student.FatherCNIC = ConfigurationManager.AppSettings["fatherCnic"].ToString();
                student.CurrentAddress = string.IsNullOrEmpty(student.CurrentAddress) ? SCHOOL_NAME : student.CurrentAddress;
                //student.CurrentAddress = ConfigurationManager.AppSettings["studentAddress"].ToString();
                student.AdmissionType = int.Parse(ConfigurationManager.AppSettings["admissionType"]);
                student.SessionId = int.Parse(ConfigurationManager.AppSettings["sessionId"]);
                student.LeavingStatus = int.Parse(ConfigurationManager.AppSettings["leavingStatus"]);
                student.BranchId = BRANCH_ID;
                student.Contact_1 = string.IsNullOrEmpty(student.Contact_1) ? DUMMY_MOBILE : student.Contact_1;
                //student.FatherCNIC = student.AdmissionNo = "";
                Student temp = studentRepo.GetStudentByRollNoAndClassSectionId(student.RollNumber, (int)student.ClassSectionId);
                if (temp != null)
                {
                    temp.AdmissionType = student.AdmissionType;
                    temp.AdmissionDate = student.AdmissionDate;
                    temp.SrNo = student.SrNo;
                    temp.ClassSectionId = student.ClassSectionId;
                    temp.SessionId = student.SessionId;
                    temp.RollNumber = student.RollNumber;
                    temp.Name = student.Name;
                    temp.FatherName = student.FatherName;
                    temp.MotherName = student.MotherName;
                    temp.CurrentAddress = student.CurrentAddress;
                    temp.FatherCNIC = student.FatherCNIC;
                    temp.MotherCnic = student.MotherCnic;
                    temp.Contact_1 = student.Contact_1;
                    temp.MotherContact1 = student.MotherContact1;
                    temp.DateOfBirth = student.DateOfBirth;
                    temp.BFormNo = student.BFormNo;
                    temp.ReligionCode = student.ReligionCode;
                    temp.GenderCode = student.GenderCode;
                    temp.Nationality = student.Nationality;
                    temp.LeavingStatu = student.LeavingStatu;
                    temp.BranchId = student.BranchId;

                    studentRepo.UpdateStudent(temp);
                }
                else
                {
                    studentRepo.AddStudent(student);
                    OpenFinanceAccount(student);
                }


                // add challan detail
                if (IS_CHALLAN_CREATED == "1")
                {
                    if (feeHeadsList != null && feeHeadsList.Count() > 0)
                    {
                        int hCount = 0;
                        List<UserChallanModel> modelList = new List<UserChallanModel>();
                        for (int j = columnsList.Count(); j < columnsList.Count() + feeHeadsList.Count(); j++)
                        {
                            UserChallanModel model = new UserChallanModel();
                            model.FeeHead = feeHeadsList[hCount];
                            string amountString = ExcelHelper.GetCellValue(i, j + 1);
                            if (amountString.Length > 0)
                                model.Amount = int.Parse(ExcelHelper.GetCellValue(i, j + 1));
                            else
                                model.Amount = 0;
                            modelList.Add(model);
                            hCount++;
                        }

                        //add remaingin fee heads to challan
                        string feeHeadTemp = ConfigurationManager.AppSettings["fee-heads"];
                        string[] feeHeadsTempList = feeHeadTemp.Split('|');

                        foreach (var head in feeHeadsTempList)
                        {
                            if (!feeHeadsList.Contains(head))
                            {
                                UserChallanModel model = new UserChallanModel();
                                model.FeeHead = head;
                                model.Amount = 0;
                                modelList.Add(model);
                            }
                        }
                        AddStudentChallan(student.id == 0 ? temp.id : student.id, modelList, classId);
                    }

                    //add fee arrears detail 
                    if (arrearHeadsList != null && arrearHeadsList.Count() > 0)
                    {
                        int startIndex = columnsList.Count() + feeHeadsList.Count();
                        int endINdex = columnsList.Count() + feeHeadsList.Count() + arrearHeadsList.Count();
                        int aCount = 0;
                        FeeBalance balance = feeRepo.GetFeeBalanceByStudentId(student.id);
                        for (int j = startIndex; j < endINdex; j++)
                        {
                            FeeArrearsDetail detail = new FeeArrearsDetail();
                            detail.FeeBalanceId = balance.Id;
                            detail.FeeHeadId = feeRepo.GetFeeHeadByName(arrearHeadsList[aCount], BRANCH_ID).Id;
                            string amountString = ExcelHelper.GetCellValue(i, j + 1);
                            if (amountString.Length > 0)
                                detail.HeadAmount = int.Parse(ExcelHelper.GetCellValue(i, j + 1));
                            else
                                detail.HeadAmount = 0;
                            detail.CreatedOn = DateTime.Now;
                            detail.UpdatedOn = DateTime.Now;
                            feeRepo.SaveFeeArrearDetail(detail);
                            aCount++;
                        }

                        //add remaingin fee arrear heads to Students
                        string feeHeadTemp = ConfigurationManager.AppSettings["fee-heads"];
                        string[] feeHeadsTempList = feeHeadTemp.Split('|');

                        foreach (var head in feeHeadsTempList)
                        {
                            if (!arrearHeadsList.Contains(head))
                            {
                                FeeArrearsDetail detail = new FeeArrearsDetail();
                                detail.FeeBalanceId = balance.Id;
                                detail.FeeHeadId = feeRepo.GetFeeHeadByName(head, BRANCH_ID).Id;
                                detail.HeadAmount = 0;
                                detail.CreatedOn = DateTime.Now;
                                detail.UpdatedOn = DateTime.Now;
                                feeRepo.SaveFeeArrearDetail(detail);
                            }
                        }
                    }

                }


                actualCount++;


                if (actualCount % 25 == 0)
                {
                    Console.WriteLine("Student Data Import Count : " + actualCount.ToString());
                }
            }

            Console.WriteLine("Student data import count : " + (rowCount - 1).ToString());
            Console.WriteLine("Student data import is finished");
            Console.WriteLine("_______________________________________________________");
            DeleteExtraChallan();
            ExcelHelper.CloseExcelSheets();
            return rowCount - 1;
        }

        private static DateTime GetDate(string dateString)
        {
            string[] dateData = dateString.Split('/');
            DateTime date = new DateTime(int.Parse(dateData[2]), int.Parse(dateData[1]), int.Parse(dateData[0]) );
            return date;
        }

        public static int UpdateStudentCurrentAddress()
        {
            string columnsValue = ConfigurationManager.AppSettings["student-columns"];
            string[] columnsList = columnsValue.Split('|');
            string feeHeads = ConfigurationManager.AppSettings["fee-challan-columns"];
            string[] feeHeadsList = feeHeads.Split('|');

            int rowCount = ExcelHelper.RowCount();
            //int rowCount = 24;

            Console.WriteLine("=======================================================");
            Console.WriteLine("            Student data Import Started");
            Console.WriteLine("=======================================================");

            int actualCount = 0;
            for (int i = 365; i <= rowCount; i++)
            {
                int classId = 0, sectionId = 0;
                Student student = new Student();
                for (int j = 0; j < columnsList.Count(); j++)
                {
                    if (columnsList[j] == "ROLL_NO")
                    {
                        student.RollNumber = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "NAME")
                    {
                        student.Name = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "FATHER_NAME")
                    {
                        student.FatherName = ExcelHelper.GetCellValue(i, j + 1);
                        if (string.IsNullOrEmpty(student.FatherName))
                        {
                            student.FatherName = student.Name;
                        }
                    }
                    else if (columnsList[j] == "CLASS")
                    {
                        classId = classRepo.GetClassByName(ExcelHelper.GetCellValue(i, j + 1), BRANCH_ID).Id;
                    }
                    else if (columnsList[j] == "SECTION")
                    {
                        sectionId = secRepo.GetSectionByName(ExcelHelper.GetCellValue(i, j + 1), BRANCH_ID).Id;
                    }
                    else if (columnsList[j] == "DATE_OF_BIRTH")
                    {
                        var dateValue = ExcelHelper.GetCellValue(i, j + 1);
                        if (dateValue != null && dateValue.Length > 0)
                        {
                            double date = double.Parse(ExcelHelper.GetCellValue(i, j + 1));
                            student.DateOfBirth = DateTime.FromOADate(date);
                            //student.DateOfBirth =  DateTime.Parse();
                        }
                        else
                        {
                            student.DateOfBirth = DateTime.Now;
                        }
                    }
                    else if (columnsList[j] == "ADMISSION_DATE")
                    {
                        var dateValue = ExcelHelper.GetCellValue(i, j + 1);
                        if (dateValue != null && dateValue.Length > 0)
                        {
                            double date = double.Parse(ExcelHelper.GetCellValue(i, j + 1));
                            student.AdmissionDate = DateTime.FromOADate(date);
                            student.LeavingDate = student.AdmissionDate;
                            //student.AdmissionDate = DateTime.Parse(ExcelHelper.GetCellValue(i, j + 1));
                        }
                        else
                        {
                            student.DateOfBirth = DateTime.Now;
                        }
                    }
                    else if (columnsList[j] == "CONTACT_1")
                    {
                        student.Contact_1 = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "CURRENT_ADDRESS")
                    {
                        student.CurrentAddress = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "RELIGION")
                    {
                        student.ReligionCode = studentRepo.GetAllReligion().Where(x => x.Name == ExcelHelper.GetCellValue(i, j + 1)).FirstOrDefault().Id;
                    }
                    else if (columnsList[j] == "GENDER")
                    {
                        student.GenderCode = studentRepo.GetAllGenders().Where(x => x.Gender1 == ExcelHelper.GetCellValue(i, j + 1)).FirstOrDefault().Id;
                    }
                    else if (columnsList[j] == "NATIONALITY")
                    {
                        student.Nationality = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "FATHER_CNIC")
                    {
                        student.FatherCNIC = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "MOTHER_NAME")
                    {
                        student.MotherName = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "MOTHER_CONTACT")
                    {
                        student.MotherContact1 = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "MOTHER_CNIC")
                    {
                        student.MotherCnic = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "BFORMNO")
                    {
                        student.BFormNo = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    else if (columnsList[j] == "ADMISSION_NO")
                    {
                        student.AdmissionNo = ExcelHelper.GetCellValue(i, j + 1);
                    }
                    //else if (columnsList[j] == "SESSION_YEAR")
                    //{
                    //    student.SessionYear = ExcelHelper.GetCellValue(i, j + 1);
                    //    if (student.SessionYear == null || student.SessionYear.Length == 0)
                    //        student.SessionYear = "2018";
                    //    student.SessionId = staffMang.GetSessionIdByName(student.SessionYear);
                    //}

                }
                if (student.AdmissionDate == null)
                    student.AdmissionDate = DateTime.Now;
                student.SrNo = "SC-" + DateTime.Now.Year.ToString() + "-" + "1159" + studentRepo.GetMaxSrNo();
                student.ClassSectionId = classSectionRepo.GetClassSectionId(classId, sectionId);
                student.FatherCNIC = ConfigurationManager.AppSettings["fatherCnic"].ToString();
                //student.CurrentAddress = ConfigurationManager.AppSettings["studentAddress"].ToString();
                student.AdmissionType = int.Parse(ConfigurationManager.AppSettings["admissionType"]);
                student.SessionId = int.Parse(ConfigurationManager.AppSettings["sessionId"]);
                student.LeavingStatus = int.Parse(ConfigurationManager.AppSettings["leavingStatus"]);
                student.BranchId = BRANCH_ID;
                //student.FatherCNIC = student.AdmissionNo = "";
                Student temp = studentRepo.GetStudentByRollNoAndClassSectionId(student.RollNumber, (int)student.ClassSectionId);
                if (temp != null)
                {
                    temp.CurrentAddress = student.CurrentAddress;
                    if (string.IsNullOrEmpty(temp.CurrentAddress))
                    {
                        temp.CurrentAddress = SCHOOL_NAME;
                    }
                    studentRepo.UpdateStudent(temp);
                }
                else
                {
                    studentRepo.AddStudent(student);
                    OpenFinanceAccount(student);
                }
                actualCount++;

                if (actualCount % 25 == 0)
                {
                    Console.WriteLine("Student Data Import Count : " + actualCount.ToString());
                }
            }

            Console.WriteLine("Student data import count : " + (rowCount - 1).ToString());
            Console.WriteLine("Student data import is finished");
            Console.WriteLine("_______________________________________________________");
            //DeleteExtraChallan();
            ExcelHelper.CloseExcelSheets();
            return rowCount - 1;
        }

        private static void DeleteExtraChallan()
        {
            if(DeleteChallanList != null && DeleteChallanList.Count > 0)
            {
                foreach (int challanId in DeleteChallanList)
                {
                    Challan chalan = feeRepo.GetChallanById(challanId);
                    feeRepo.DeleteChallan(chalan);
                }
            }
        }
        private static void AddStudentChallan(int StudentId, List<UserChallanModel> challanDetail, int ClassId)
        {
            var stdList = feeRepo.GetStudentChallanDetailByStudentId(StudentId);
            int ChallanId = IsNewChallan(ClassId, challanDetail, StudentId);
            if (stdList == null)
            {
                ChallanStudentDetail detail = new ChallanStudentDetail();
                detail.StdId = StudentId;
                detail.ChallanId = ChallanId;
                detail.CreatedOn = DateTime.Now;
                feeRepo.AddStudentChallanDetail(detail);
            }
            else
            {
                if (ChallanId != stdList.ChallanId)
                {
                    if (DeleteChallanList.Count == 0 || DeleteChallanList.Contains(stdList.ChallanId) == false)
                            DeleteChallanList.Add(stdList.ChallanId);
                }
                var detail = stdList;
                detail.ChallanId = ChallanId;
                feeRepo.UpdateStudentChallanDetail(detail);
            }
        }

        private static int IsNewChallan(int ClassId, List<UserChallanModel> ChallanDetail, int studentId)
        {
            int challanId = 0;
            bool newChallannFLag = true;
            int newAmount = ChallanDetail.Sum(x => x.Amount);

            var oldChallanList = feeRepo.GetAllChallanByClassId(ClassId);

            if (oldChallanList != null && oldChallanList.Count > 0)
            {
                foreach (Challan chalan in oldChallanList)
                {
                    var tempDetail = feeRepo.GetChallDetailByChallanId(chalan.Id);
                    int oldAmount = tempDetail.Sum(x => x.Amount);
                    if (oldAmount == newAmount)
                    {
                        var updateList = ChallanDetail.Where(x => x.Amount > 0).ToList();

                        if (updateList != null && updateList.Count > 0)
                        {
                            int unMatchedCount = 0;
                            foreach (UserChallanModel model in updateList)
                            {
                                var tempObj = tempDetail.Where(x => x.Name == model.FeeHead.Trim()).FirstOrDefault();
                                if (tempObj != null && tempObj.Amount != model.Amount)
                                {
                                    unMatchedCount++;
                                    break;
                                }
                            }
                            if (unMatchedCount == 0)
                            {
                                newChallannFLag = false;
                                challanId = chalan.Id;
                                break;
                            }
                        }
                    }
                }
            }

            if (newChallannFLag)
            {

                Challan newChallan = new Challan();
                newChallan.BranchId = BRANCH_ID;
                newChallan.ClassId = ClassId;
                string className = classRepo.GetClassById(ClassId).Name;
                newChallan.Name = className + " Class Challan (" + newAmount + ")";
                var challanList = feeRepo.GetAllChallan().Where(x => x.Name.Contains(newChallan.Name)).ToList();
                if (challanList != null && challanList.Count > 0)
                    newChallan.Name = newChallan.Name + challanList.Count;
                newChallan.Description = "Customise Challan for Class " + className + "Amount (" + newAmount + ")";
                newChallan.CreatedOn = DateTime.Now;
                //newChallan.SystemGenerated = true;
                newChallan.IsDefault = false;
                feeRepo.AddChallan(newChallan);

                var headList = feeRepo.GetAllFeeHeads();
                foreach (FeeHead head in headList)
                {
                    UserChallanModel temp = ChallanDetail.Where(x => x.FeeHead == head.Name).FirstOrDefault();
                    if (temp != null)
                    {
                        ChallanFeeHeadDetail detail = new ChallanFeeHeadDetail();
                        detail.HeadId = head.Id;
                        detail.ChallanId = newChallan.Id;
                        detail.Amount = ChallanDetail.Where(x => x.FeeHead == head.Name).FirstOrDefault().Amount;
                        feeRepo.AddChallanDetail(detail);
                    }
                }

                challanId = newChallan.Id;
            }
            return challanId;
        }

        private static void AddFeeHeads()
        {
            string feeHeads = ConfigurationManager.AppSettings["fee-heads"];
            string[] feeHeadsList = feeHeads.Split('|');
            foreach(string head in feeHeadsList)
            {
                FeeHead temp = feeRepo.GetFeeHeadByName(head, BRANCH_ID);
                if (temp == null)
                {
                    FeeHead feeHead = new FeeHead();
                    feeHead.BranchId = BRANCH_ID;
                    feeHead.Amount = 1000;
                    feeHead.Description = "Fee Head created autotmatically";
                    feeHead.Name = head;
                    feeRepo.AddFeeHead(feeHead);
                    CreateFinnaceAccount(head, BRANCH_ID);
                }
            }
        }

        public static void AddFinanceeadsAccounts()
        {
            string feeHeads = ConfigurationManager.AppSettings["fee-challan-columns"];
            string[] feeHeadsList = feeHeads.Split('|');

            foreach (string head in feeHeadsList)
            {
                CreateFinnaceAccount(head, BRANCH_ID);
            }
        }

        private static void CreateFinnaceAccount(string headName, int branchId)
        {
            FinanceFifthLvlAccount advanceAccount = new FinanceFifthLvlAccount();
            advanceAccount.AccountName = headName;
            advanceAccount.AccountDescription = "Fee Heads Receivable Account";
            advanceAccount.CreatedOn = DateTime.Now;
            advanceAccount.Value = 0;
            advanceAccount.Count = 0;
            advanceAccount.BranchId = branchId;
            advanceAccount.FourthLvlAccountId = GetFourthLvlConfigurationAccount((int)advanceAccount.BranchId, CAT_FEE_RECEIVABLE, CAT_RECEIVABLES);
            financeRepo.AddFinanceFifthLvlAccount(advanceAccount);

            List<ClassSection> list = classSectionRepo.GetAllClassSections();

            foreach (ClassSection sec in list)
            {
                string fourthLvlAccountName = sec.Class.Name + ", Section : " + sec.Section.Name;
                var fourthLvlAccountTemp = financeRepo.GetFinanceFourthLvlAccountByName(fourthLvlAccountName, branchId);

                if (fourthLvlAccountTemp != null)
                {
                    FinanceFifthLvlAccount accountsFifthLvl = new FinanceFifthLvlAccount();
                    var fifthLvlObj = financeRepo.GetFinanceFifthLvlAccount(headName, fourthLvlAccountTemp.Id);

                    if (fifthLvlObj == null)
                    {
                        accountsFifthLvl.AccountName = headName;
                        accountsFifthLvl.AccountDescription = "This account is created for fee head " + headName + " fee collection for " + fourthLvlAccountName;
                        accountsFifthLvl.CreatedOn = DateTime.Now;
                        accountsFifthLvl.Value = 0;
                        accountsFifthLvl.Count = 0;
                        accountsFifthLvl.BranchId = branchId;
                        accountsFifthLvl.FourthLvlAccountId = fourthLvlAccountTemp.Id;
                        financeRepo.AddFinanceFifthLvlAccount(accountsFifthLvl);
                    }
                }
            }

        }

        private static void OpenFinanceAccount(Student student)
        {
            FinanceFifthLvlAccount accounts = new FinanceFifthLvlAccount();

            accounts.AccountName = student.id.ToString().PadLeft(6, '0') + "-" + student.Name;
            accounts.AccountDescription = "Fee Recivables Account for : " + accounts.AccountName;
            accounts.CreatedOn = DateTime.Now;
            accounts.Value = 0;
            accounts.Count = 0;
            accounts.BranchId = BRANCH_ID;
            accounts.FourthLvlAccountId = GetFourthLvlConfigurationAccount((int)accounts.BranchId, CAT_FEE_RECEIVABLE, CAT_RECEIVABLES);
            financeRepo.AddFinanceFifthLvlAccount(accounts);
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