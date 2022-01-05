using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Helpers;
using System.Diagnostics;
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.FeeCollection
{
    public class FeeIncrementController : Controller
    {
        private static int errorCode = 0;
        //
        // GET: /FeeIncrement/

        
        IFeePlanRepository feePlanRepo;
        IClassSectionRepository classSecRepo;
        IClassRepository classRepo;
        ISectionRepository secRepo;
        IAdmissionChargesRepository acRepo;
        private IStudentRepository studentRepo;

        public FeeIncrementController()
        {
            
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());;
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            acRepo = new AdmissionChargesRepositoryImp(new SC_WEBEntities2());; 
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2()); ;
        }

        public ActionResult Index()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_FEE_INCREMENT) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            List<StudentChallanViewModel> model = new List<StudentChallanViewModel>();
            try
            {
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewBag.FeeHeadId = new SelectList(SessionHelper.FeeHeadListDD(Session.SessionID), "Id", "Name");
                ViewData["Error"] = errorCode;
                errorCode = 0;
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                if (Session[ConstHelper.CARDS_SEARCH_FLAG] != null)
                {
                    model = SearchStudent();
                }
                voidSetSearchVeriables();
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(model);
        }

        

        private void voidSetSearchVeriables()
        {
            if (Session[ConstHelper.GLOBAL_CLASS_ID] != null)
            {
                ViewData["GlobalClassId"] = (int)Session[ConstHelper.GLOBAL_CLASS_ID];
                Session[ConstHelper.GLOBAL_CLASS_ID] = null;
            }

            if (Session[ConstHelper.GLOBAL_SECTION_ID] != null)
            {
                ViewData["GlobalSectionId"] = (int)Session[ConstHelper.GLOBAL_SECTION_ID];
                Session[ConstHelper.GLOBAL_SECTION_ID] = null;
            }
        }

        //
        // POST: /FeeIncrement/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveFeeIncrementHistory(int [] StudentIds, int FeeHeadId, string Amount, string Percentage, int IsRevoke)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (IsRevoke == 1)
                {
                    LogWriter.WriteLog("Revoking the last increment");
                    errorCode = RevokeLastIncrement(StudentIds);
                    return RedirectToAction("Index", new { id = -59 });
                }
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                int amount = int.Parse(string.IsNullOrEmpty(Amount) == true ? "0" : Amount);
                int percentage = int.Parse(string.IsNullOrEmpty(Percentage) == true ? "0" : Percentage);

                LogWriter.WriteLog("Increment student count : " + (StudentIds == null ? 0 : StudentIds.Count()));

                foreach (int Id in StudentIds)
                {
                    var studentChalan = feePlanRepo.GetStudentChallanDetailByStudentId(Id);
                    if (studentChalan != null)
                    {
                        var challanDetail = feePlanRepo.GetChallDetailByChallanId(studentChalan.ChallanId);

                        int grossAmount = 0;
                        List<UserChallanModel> newchallan = new List<UserChallanModel>();
                        foreach (var challan in challanDetail)
                        {
                            UserChallanModel model = new UserChallanModel();
                            model.FeeHead = challan.Name;
                            model.Amount = challan.Amount;
                            if (challan.HeadId == FeeHeadId)
                            {
                                if (amount != 0)
                                {
                                    grossAmount = amount;
                                }
                                else
                                {
                                    grossAmount = (challan.Amount * percentage) / 100;
                                }
                                model.Amount = model.Amount + grossAmount;
                            }

                            newchallan.Add(model);
                        }

                        LogWriter.WriteLog("Getting student challan accoding to increment in fee");
                        int ChallanId = IsNewChallan(studentChalan.ChallanId, newchallan, Id, branchId);
                        var detail = studentChalan;
                        detail.ChallanId = ChallanId;
                        feePlanRepo.UpdateStudentChallanDetail(detail);

                        LogWriter.WriteLog("Adding fee increment history");
                        FeeIncrementHistoryDetail historyDetail = new FeeIncrementHistoryDetail();
                        historyDetail.StudentId = Id;
                        historyDetail.GrossAmount = grossAmount;
                        historyDetail.Amount = amount;
                        historyDetail.Percentage = percentage;
                        historyDetail.IsRevoked = false;
                        historyDetail.FeeHeadId = FeeHeadId;
                        historyDetail.CreatedOn = DateTime.Now;
                        historyDetail.IsRevoked = false;
                        historyDetail.Description = GetDescription(amount, percentage, FeeHeadId);
                        feePlanRepo.AddFeeIncrementHistoryDtail(historyDetail);
                    }
                }
                errorCode = 4;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index", new { id = -59 });
        }

        private int RevokeLastIncrement(int[] StudentIds)
        {
            int tempErrorCode = 0;
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);

                LogWriter.WriteLog("Revoke Increment student count : " + (StudentIds == null ? 0 : StudentIds.Count()));
                foreach (int Id in StudentIds)
                {
                    var tempDetail = feePlanRepo.GetLastFeeIncrementHistoryDetail(Id);
                    if (tempDetail != null)
                    {
                        var studentChalan = feePlanRepo.GetStudentChallanDetailByStudentId(Id);
                        var challanDetail = feePlanRepo.GetChallDetailByChallanId(studentChalan.ChallanId);

                        int grossAmount = 0;
                        List<UserChallanModel> newchallan = new List<UserChallanModel>();
                        foreach (var challan in challanDetail)
                        {
                            UserChallanModel model = new UserChallanModel();
                            model.FeeHead = challan.Name;
                            model.Amount = challan.Amount;
                            if (challan.HeadId == tempDetail.FeeHeadId)
                            {
                                model.Amount = model.Amount - (int)tempDetail.GrossAmount;
                            }

                            newchallan.Add(model);
                        }

                        LogWriter.WriteLog("Getting student challan after revoking increment in fee");
                        int ChallanId = IsNewChallan(studentChalan.ChallanId, newchallan, Id, branchId);
                        var detail = studentChalan;
                        detail.ChallanId = ChallanId;
                        feePlanRepo.UpdateStudentChallanDetail(detail);

                        LogWriter.WriteLog("Adding fee increment revoke history");
                        FeeIncrementHistoryDetail historyDetail = new FeeIncrementHistoryDetail();
                        historyDetail.StudentId = Id;
                        historyDetail.GrossAmount = grossAmount;
                        historyDetail.Amount = (int)tempDetail.Amount;
                        historyDetail.Percentage = (int)tempDetail.Percentage;
                        historyDetail.IsRevoked = false;
                        historyDetail.FeeHeadId = tempDetail.FeeHeadId;
                        historyDetail.CreatedOn = DateTime.Now;
                        historyDetail.IsRevoked = true;
                        historyDetail.Description = GetRevokDescription((int)tempDetail.Amount, (int)tempDetail.Percentage, (int)tempDetail.FeeHeadId);
                        feePlanRepo.AddFeeIncrementHistoryDtail(historyDetail);

                        tempDetail.IsRevoked = true;
                        feePlanRepo.UpdateFeeIncreentHistoryDetail(tempDetail);
                    }
                }
                tempErrorCode = 41;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                tempErrorCode = 421;
            }

            return tempErrorCode;
        }

        public ActionResult Delete(int id)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var tempDetail = feePlanRepo.GetFeeIncrementHistoryDetail(id);
                LogWriter.WriteLog("Deleting Increment ("+tempDetail.Id+") for the student : ("+tempDetail.StudentId+")");
                if (tempDetail != null)
                {
                    var studentChalan = feePlanRepo.GetStudentChallanDetailByStudentId((int)tempDetail.StudentId);
                    var challanDetail = feePlanRepo.GetChallDetailByChallanId(studentChalan.ChallanId);

                    int grossAmount = 0;
                    List<UserChallanModel> newchallan = new List<UserChallanModel>();
                    foreach (var challan in challanDetail)
                    {
                        UserChallanModel model = new UserChallanModel();
                        model.FeeHead = challan.Name;
                        model.Amount = challan.Amount;
                        if (challan.HeadId == tempDetail.FeeHeadId)
                        {
                            model.Amount = model.Amount - (int)tempDetail.GrossAmount;
                        }

                        newchallan.Add(model);
                    }

                    LogWriter.WriteLog("Getting student challan after deleting the increment");
                    int ChallanId = IsNewChallan(studentChalan.ChallanId, newchallan, (int)tempDetail.StudentId, branchId);
                    var detail = studentChalan;
                    detail.ChallanId = ChallanId;
                    feePlanRepo.UpdateStudentChallanDetail(detail);

                    LogWriter.WriteLog("Adding fee increment delete history");
                    FeeIncrementHistoryDetail historyDetail = new FeeIncrementHistoryDetail();
                    historyDetail.StudentId = (int)tempDetail.StudentId;
                    historyDetail.GrossAmount = grossAmount;
                    historyDetail.Amount = (int)tempDetail.Amount;
                    historyDetail.Percentage = (int)tempDetail.Percentage;
                    historyDetail.IsRevoked = false;
                    historyDetail.FeeHeadId = tempDetail.FeeHeadId;
                    historyDetail.CreatedOn = DateTime.Now;
                    historyDetail.IsRevoked = true;
                    historyDetail.Description = GetRevokDescription((int)tempDetail.Amount, (int)tempDetail.Percentage, (int)tempDetail.FeeHeadId);
                    feePlanRepo.AddFeeIncrementHistoryDtail(historyDetail);

                    tempDetail.IsRevoked = true;
                    feePlanRepo.UpdateFeeIncreentHistoryDetail(tempDetail);
                    errorCode = 41;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 421;
            }

            return RedirectToAction("Index", new { id = -59 });
        }

        string GetDescription(int amount, int percentage, int feeHeadId)
        {
            FeeHead head = feePlanRepo.GetFeeHeadById(feeHeadId);
            string description = "";
            if (amount != 0)
            {
                if (amount > 0)
                    description = "Increment of Rs." + amount + " is applied in Fee Head : " + head.Name;
                else
                    description = "Decrement of Rs." + amount + " is applied in Fee Head : " + head.Name;
            }
            else
            {
                if (percentage > 0)
                    description = "Percentage Increment of " + percentage + "% is applied in Fee Head : " + head.Name;
                else
                    description = "Percentage Decrement of " + percentage + "% is applied in Fee Head : " + head.Name;
            }

            return description;
        }

        string GetRevokDescription(int amount, int percentage, int feeHeadId)
        {
            FeeHead head = feePlanRepo.GetFeeHeadById(feeHeadId);
            string description = "";
            if (amount != 0)
            {
                if (amount > 0)
                    description = "Increment of Rs." + amount + " is revoked in Fee Head : " + head.Name;
                else
                    description = "Decrement of Rs." + amount + " is revoked in Fee Head : " + head.Name;
            }
            else
            {
                if (percentage > 0)
                    description = "Percentage Increment of " + percentage + "% is revoked in Fee Head : " + head.Name;
                else
                    description = "Percentage Decrement of " + percentage + "% is revoked in Fee Head : " + head.Name;
            }

            return description;
        }


        public ActionResult FeeIncrementHistoryDetail(int id)
        {
            List<FeeIncrementModel> model = new List<FeeIncrementModel>();
            Session[ConstHelper.SEARCH_CHALLAN_NAME] = feePlanRepo.GetFeeIncrementHistoryDetailList(id);
            if (Session[ConstHelper.SEARCH_CHALLAN_NAME] != null)
            {
                model = (List<FeeIncrementModel>)Session[ConstHelper.SEARCH_CHALLAN_NAME];
            }

            return View(model);
        }

        

        private int IsNewChallan(int ChallanId, List<UserChallanModel> ChallanDetail, int studentId, int branchId)
        {
            int challanId = 0;

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                bool newChallannFLag = true;
                var oldChallan = feePlanRepo.GetChallanById(ChallanId);
                int newAmount = ChallanDetail.Sum(x => x.Amount);
                LogWriter.WriteLog("Check if the challan is found in the class with the amount : " + newAmount);

                var oldChallanList = feePlanRepo.GetAllChallanByClassId((int)oldChallan.ClassId);

                foreach (Challan chalan in oldChallanList)
                {
                    var tempDetail = feePlanRepo.GetChallDetailByChallanId(chalan.Id);
                    int oldAmount = tempDetail.Sum(x => x.Amount);
                    if (oldAmount == newAmount)
                    {
                        var updateList = ChallanDetail.Where(x => x.Amount > 0).ToList();

                        if (updateList != null && updateList.Count > 0)
                        {
                            int unMatchedCount = 0;
                            foreach (UserChallanModel model in updateList)
                            {
                                var tempObj = tempDetail.Where(x => x.Name == model.FeeHead).FirstOrDefault();
                                if (tempObj.Amount != model.Amount)
                                {
                                    unMatchedCount++;
                                    break;
                                }
                            }
                            if (unMatchedCount == 0)
                            {
                                LogWriter.WriteLog("Challan is found with the amount");
                                newChallannFLag = false;
                                challanId = chalan.Id;
                                break;
                            }
                        }
                    }
                }

                if (newChallannFLag)
                {
                    LogWriter.WriteLog("No challan is found with the amount");
                    LogWriter.WriteLog("Creating new challan with the amount");
                    Challan newChallan = new Challan();
                    newChallan.BranchId = oldChallan.BranchId;
                    newChallan.ClassId = oldChallan.ClassId;
                    string className = classRepo.GetClassById((int)oldChallan.ClassId).Name;
                    newChallan.Name = className + " Class Challan (" + newAmount + ")";
                    var challanList = feePlanRepo.GetAllChallan().Where(x => x.Name.Contains(newChallan.Name)).ToList();
                    if (challanList != null && challanList.Count > 0)
                        newChallan.Name = newChallan.Name + challanList.Count;
                    newChallan.Description = "Customise Challan for Class " + className + "Amount (" + newAmount + ")";
                    newChallan.CreatedOn = DateTime.Now;
                    newChallan.SystemGenerated = true;
                    newChallan.IsDefault = false;
                    feePlanRepo.AddChallan(newChallan);
                    SessionHelper.InvalidateChallanCache = false;

                    LogWriter.WriteLog("Adding fee heads in the challan");
                    var headList = feePlanRepo.GetAllFeeHeads(branchId);
                    foreach (FeeHead head in headList)
                    {
                        ChallanFeeHeadDetail detail = new ChallanFeeHeadDetail();
                        detail.HeadId = head.Id;
                        detail.ChallanId = newChallan.Id;
                        var tempData = ChallanDetail.Where(x => x.FeeHead == head.Name).FirstOrDefault();
                        detail.Amount = 0;
                        if (tempData != null)
                            detail.Amount = tempData.Amount;
                        feePlanRepo.AddChallanDetail(detail);
                    }

                    challanId = newChallan.Id;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return challanId;
        }

        //
        // POST: /FeeIncrement/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string ClassId, string SectionId, string FeeHeadId, string Amount, string Percentage, string Description)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if ((Amount.Length == 0 || int.Parse(Amount) <= 0) && (Percentage.Length == 0 || int.Parse(Percentage) <= 0))
                {
                    errorCode = 10;
                }
                else if (Amount.Length > 0 && Percentage.Length > 0)
                {
                    errorCode = 11;
                }
                else
                {
                    string className = "All", sectionName = "All", feeHeadName = "";
                    if (ClassId.Length > 0 && ClassId != "0")
                    {
                        int tempClassId = int.Parse(ClassId);
                        className = classRepo.GetClassById(tempClassId).Name;
                    }
                    else
                        ClassId = "-1";
                    if (!string.IsNullOrEmpty(SectionId) && SectionId != "0")
                    {
                        int tempSectionId = int.Parse(SectionId);
                        sectionName = secRepo.GetSectionById(tempSectionId).Name;
                    }
                    else
                        SectionId = "-1";

                    if (FeeHeadId.Length > 0 && FeeHeadId != "0")
                    {
                        int tempFeeHeadId = int.Parse(FeeHeadId);
                        feeHeadName = feePlanRepo.GetFeeHeadById(tempFeeHeadId).Name;
                    }

                    Amount = Amount.Length == 0 ? "0" : Amount;
                    Percentage = Percentage.Length == 0 ? "0" : Percentage;
                    LogWriter.WriteLog(string.Format("Saving fee increment with the (amount : {0}, feeHeadName : {1}, ClassId : {2}, SectionId : {3}) : ",
                        Amount, feeHeadName, ClassId, SectionId));
                    var spQuery = @"DECLARE	@return_value int
                                                        EXEC	@return_value = [dbo].[sp_Increment_Fee]
		                                                        @ClassId = " + ClassId + ","
                                                                + "@SectionId = " + SectionId + ","
                                                                + "@IncrementAmount = " + Amount + ","
                                                                + "@feeHead = '" + feeHeadName + "',"
                                                                + "@PercentageIncrement = " + Percentage + ","
                                                                + "@BranchId = " + branchId;

                    acRepo.MakeFeeIncrementHistory(spQuery);

                    LogWriter.WriteLog("Adding fee increment create history");
                    FeeIncrementHistory history = new FeeIncrementHistory();
                    history.ClassId = className;
                    history.SectionId = sectionName;
                    history.FeeHead = feeHeadName;
                    history.BranchId = branchId;
                    history.IncrementAmount = int.Parse(Amount);
                    history.PercentageIncrement = int.Parse(Percentage);
                    history.Description = Description;
                    history.IncrementDate = DateTime.Now;
                    acRepo.AddFeeIncrementHistory(history);
                    errorCode = 2;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index", new { id = -59 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchStudents(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string chalanNo, string FatherCnic, string AdmissionNo, string FatherContact, int MinFee = 0, int MaxFee = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            SearchStudentParams(ClassId, SectionId, RollNo, Name, FatherName, FatherCnic, AdmissionNo, FatherContact, MinFee, MaxFee);
            //voidSetSearchVeriables();
            Session[ConstHelper.CARDS_SEARCH_FLAG] = true;
            return RedirectToAction("Index", new { id = -59 });

        }

        public void SearchStudentParams(string ClassId, string SectionId, string RollNo, string Name, string FatherName, string FatherCnic, string AdmissionNo, string FatherContact, int MinFee, int MaxFee)
        {
            int classId = int.Parse(string.IsNullOrEmpty(ClassId) == true ? "0" : ClassId);
            int sectionId = int.Parse(string.IsNullOrEmpty(SectionId) == true ? "0" : SectionId);
            Session[ConstHelper.SEARCH_CHALLAN_ROLL_NO] = RollNo;
            Session[ConstHelper.SEARCH_CHALLAN_CLASS_ID] = classId;
            Session[ConstHelper.SEARCH_CHALLAN_NAME] = Name;
            Session[ConstHelper.SEARCH_CHALLAN_FATHER_NAME] = FatherName;
            Session[ConstHelper.SEARCH_CHALLAN_FATHER_CNIC] = FatherCnic;
            Session[ConstHelper.SEARCH_CHALLAN_ADMISSION_NO] = AdmissionNo;
            Session[ConstHelper.SEARCH_CHALLAN_CONTACT_NO] = FatherContact;
            Session[ConstHelper.GLOBAL_CLASS_ID] = classId;
            Session[ConstHelper.GLOBAL_SECTION_ID] = sectionId;
            Session[ConstHelper.GLOBAL_MIN_FEE] = MinFee;
            Session[ConstHelper.GLOBAL_MAX_FEE] = MaxFee;
            if (classId > 0 && sectionId > 0)
                Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID] = classSecRepo.GetClassSectionByClassAndSectionId(classId, sectionId).ClassSectionId;
            else
                Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID] = 0;
        }

        private List<StudentChallanViewModel> SearchStudent()
        {
            List<StudentModel> studentList = null;
            List<StudentChallanViewModel> studentChalanList = new List<StudentChallanViewModel>();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));

                Session[ConstHelper.CARDS_SEARCH_FLAG] = null;
                string rollNumber = (string)Session[ConstHelper.SEARCH_CHALLAN_ROLL_NO];
                string name = (string)Session[ConstHelper.SEARCH_CHALLAN_NAME];
                string fatherName = (string)Session[ConstHelper.SEARCH_CHALLAN_FATHER_NAME];
                string fatherCnic = (string)Session[ConstHelper.SEARCH_CHALLAN_FATHER_CNIC];
                string admissionNo = (string)Session[ConstHelper.SEARCH_CHALLAN_ADMISSION_NO];
                string contactNo = (string)Session[ConstHelper.SEARCH_CHALLAN_CONTACT_NO];
                int classSectionId = (int)Session[ConstHelper.SEARCH_CHALLAN_CLASS_SCETION_ID];
                int classId = (int)Session[ConstHelper.SEARCH_CHALLAN_CLASS_ID];
                int minFee = (int)Session[ConstHelper.GLOBAL_MIN_FEE];
                int maxFee = (int)Session[ConstHelper.GLOBAL_MAX_FEE];


                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                if (classSectionId > 0)
                    studentList = studentRepo.SearchStudents(rollNumber, name, fatherName, classSectionId, fatherCnic, branchId, admissionNo, contactNo);
                else if (classId > 0)
                    studentList = studentRepo.SearchClassStudents(rollNumber, name, fatherName, classId, fatherCnic, branchId, admissionNo, contactNo);
                else
                    studentList = studentRepo.SearchStudents(rollNumber, name, fatherName, fatherCnic, branchId, admissionNo, contactNo);

                studentList = studentList.Where(x => x.LeavingStatus == "ACTIVE").ToList();

                foreach (var student in studentList)
                {
                    StudentChallanViewModel scvm = new StudentChallanViewModel();
                    scvm.Id = student.Id;
                    scvm.Name = student.Name;
                    scvm.FatherName = student.FatherName;
                    scvm.RollNumber = student.RollNumber;
                    var studentChalan = feePlanRepo.GetStudentChallanDetailByStudentId(student.Id);
                    if (studentChalan != null)
                    {
                        scvm.Chalan = studentChalan.Challan.Name;
                        //scvm.Chalan = feePlanRepo.GetChallanById(studentChalan.ChallanId).Name;
                        scvm.Amount = "" + feePlanRepo.GetChallanAmountByChallanId(studentChalan.ChallanId);
                        //scvm.Amount = "" + feePlanRepo.GetChallanAmountByChallanId(studentChalan.ChallanId);
                    }
                    else
                    {
                        scvm.Chalan = "";
                        scvm.Amount = "";
                    }

                    var historyDetail = feePlanRepo.GetLastFeeIncrementHistoryDetail(student.Id);
                    if (historyDetail != null)
                    {
                        if (historyDetail.Amount != 0)
                            scvm.IncrementAmount = historyDetail.Amount.ToString();
                        else
                            scvm.IncrementAmount = historyDetail.Percentage + "%";
                        scvm.IncrmentDate = historyDetail.CreatedOn.ToString().Split(' ')[0];
                        scvm.HeadName = feePlanRepo.GetFeeHeadById((int)historyDetail.FeeHeadId).Name;
                    }
                    else
                    {
                        scvm.IncrementAmount = "";
                        scvm.IncrmentDate = "";
                        scvm.HeadName = "";
                    }

                    studentChalanList.Add(scvm);
                }

                if (minFee > 0)
                {
                    studentChalanList = studentChalanList.Where(x => int.Parse(string.IsNullOrEmpty(x.Amount) ? "0" : x.Amount) >= minFee).ToList();
                }
                if (maxFee > 0)
                {
                    studentChalanList = studentChalanList.Where(x => int.Parse(string.IsNullOrEmpty(x.Amount) ? "0" : x.Amount) <= maxFee).ToList();
                }

                LogWriter.WriteLog("Search student count : " + (studentChalanList == null ? 0 : studentChalanList.Count));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return studentChalanList;
        }
        //
        // GET: /FeeIncrement/Delete/5

        //public ActionResult Delete(int id = 0)
        //{
        //    if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }

        //    int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
        //    FeeIncrementHistory feeincrementhistory = acRepo.GetFeeIncrementHistoryById(id);
        //    try
        //    {
        //        if (feeincrementhistory == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        int ClassId = -1, SectionId = -1, Amount = 0, Percentage = 0;
        //        if (feeincrementhistory.ClassId != "All")
        //        {
        //            ClassId = classRepo.GetClassByName(feeincrementhistory.ClassId, branchId).Id;
        //        }
        //        if (feeincrementhistory.SectionId != "All")
        //        {
        //            SectionId = secRepo.GetSectionByName(feeincrementhistory.SectionId, branchId).Id;
        //        }
        //        if (feeincrementhistory.IncrementAmount > 0)
        //        {
        //            Amount = -1 * (int)feeincrementhistory.IncrementAmount;
        //        }
        //        if (feeincrementhistory.PercentageIncrement > 0)
        //        {
        //            Percentage = -1 * (int)feeincrementhistory.PercentageIncrement;
        //        }
        //        var spQuery = @"DECLARE	@return_value int
        //                                                EXEC	@return_value = [dbo].[sp_Increment_Fee]
		      //                                                  @ClassId = " + ClassId + ","
        //                                                        + "@SectionId = " + SectionId + ","
        //                                                        + "@IncrementAmount = " + Amount + ","
        //                                                        + "@feeHead = '" + feeincrementhistory.FeeHead + "',"
        //                                                        + "@PercentageIncrement = " + Percentage + ","
        //                                                        + "@BranchId = " + branchId;

        //        SessionHelper.dbContext.Database.SqlQuery<List<string>>(spQuery).FirstOrDefault();
        //        acRepo.DeleteFeeIncrementHistory(feeincrementhistory);
        //        errorCode = 41;
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", "Unable to Delete Fee Increment :" + ex.Message);
        //        errorCode = 421;
        //    }
        //    return RedirectToAction("Index", new { id = 0 });
        //}

    }
}