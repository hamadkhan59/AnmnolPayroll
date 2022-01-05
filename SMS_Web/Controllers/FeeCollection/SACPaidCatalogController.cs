using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_DAL.ViewModel;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.FeeCollection
{
    public class SACPaidCatalogController : Controller
    {
        static int errorCode = 0;
        //
        // GET: /SACPaidCatalog/

        
        IClassSectionRepository classSecRepo;
        IClassRepository classRepo;
        ISectionRepository secRepo;
        IStudentRepository studentRepo;
        IAdmissionChargesRepository acRepo;

        public SACPaidCatalogController()
        {
            
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            secRepo = new SectionRepositoryImp(new SC_WEBEntities2());
            studentRepo = new StudentRepositoryImp(new SC_WEBEntities2());;
            acRepo = new AdmissionChargesRepositoryImp(new SC_WEBEntities2());;
        }

        public ActionResult Index()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_ADMISSION_CHARGES) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                ViewBag.ClassSectionId = new SelectList(SessionHelper.ClassSectionList(Session.SessionID), "ClassSectionId", "ClassSectionId");
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                ViewBag.SectionId = new SelectList(SessionHelper.SectionList(Session.SessionID), "Id", "Name");
                ViewData["classSection"] = SessionHelper.ClassSectionList(Session.SessionID);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                voidSetSearchVeriables();
                if ((Session[ConstHelper.SEARCH_ADMISSION_CHARGES_FLAG] != null && (bool)Session[ConstHelper.SEARCH_ADMISSION_CHARGES_FLAG] == true) || errorCode == 5 || errorCode == 6 || errorCode == 7 || errorCode == 8)
                {
                    Session[ConstHelper.SEARCH_ADMISSION_CHARGES_FLAG] = false;
                    return View(SearchStudentAdmissionCharges());
                }
                else
                {
                    return View("");
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return View("");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchStudentAdmissionCharges(DateTime FromDate, DateTime ToDate, string ClassId, string SectionId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            Session[ConstHelper.SEARCH_ADMISSION_CHARGES_FROM_DATE] = FromDate;
            Session[ConstHelper.SEARCH_ADMISSION_CHARGES_TO_DATE] = ToDate;
            Session[ConstHelper.SEARCH_ADMISSION_CHARGES_CLASS_ID] = int.Parse(ClassId);
            Session[ConstHelper.SEARCH_ADMISSION_CHARGES_SECTION_ID] = int.Parse(SectionId);

            Session[ConstHelper.GLOBAL_FROM_DATE] = FromDate;
            Session[ConstHelper.GLOBAL_TO_DATE] = ToDate;
            Session[ConstHelper.GLOBAL_CLASS_ID] = int.Parse(ClassId);
            Session[ConstHelper.GLOBAL_SECTION_ID] = int.Parse(SectionId);

            Session[ConstHelper.SEARCH_ADMISSION_CHARGES_FLAG] = true;
            errorCode = 0;
            return RedirectToAction("Index", new { id = -59 });

        }

        private List<SACPaidCatalogViewModel> SearchStudentAdmissionCharges()
        {
            List<SACPaidCatalogViewModel> sacList = new List<SACPaidCatalogViewModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DateTime FromDate = (DateTime)Session[ConstHelper.SEARCH_ADMISSION_CHARGES_FROM_DATE];
                DateTime ToDate = (DateTime)Session[ConstHelper.SEARCH_ADMISSION_CHARGES_TO_DATE];
                int ClassId = (int)Session[ConstHelper.SEARCH_ADMISSION_CHARGES_CLASS_ID];
                int SectionId = (int)Session[ConstHelper.SEARCH_ADMISSION_CHARGES_SECTION_ID];

                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var studentList = studentRepo.GetStudentByAdmissionDate(FromDate, ToDate, ClassId, SectionId, branchId);
                studentList = studentList.Where(x => x.LeavingStatus == "ACTIVE").ToList();

                LogWriter.WriteLog("Searched Student List Count : " + (studentList == null ? 0 : studentList.Count));
                if (studentList != null && studentList.Count > 0)
                {
                    foreach (StudentModel std in studentList)
                    {
                        SACPaidCatalogViewModel model = new SACPaidCatalogViewModel();
                        model.Id = std.Id;
                        model.RollNo = std.RollNumber;
                        model.Name = std.Name;
                        int admissionChargesAmount = 0;
                        var chargesList = acRepo.GetStudentAdmissionChargesByStudentId(std.Id);
                        if (chargesList != null && chargesList.Count > 0)
                            admissionChargesAmount = (int)chargesList.Sum(x => x.Amount);
                        //admissionChargesAmount = acRepo.GetStudentAdmissionChargesSumByStudentId(std.Id);
                        var paidCatalog = acRepo.GetPaidStudentAdmissionChargesByStudentId(std.Id);
                        if (admissionChargesAmount > 0)
                            model.AdmissionCharges = admissionChargesAmount;
                        else
                            model.AdmissionCharges = 0;
                        if (paidCatalog != null && paidCatalog.Count > 0)
                        {
                            model.Paid = "Yes";
                            model.ChargesPaid = (int)paidCatalog[0].Amount;
                            model.PaidDate = paidCatalog[0].PaidDate.ToString();
                        }
                        else
                        {
                            model.Paid = "No";
                            model.ChargesPaid = 0;
                        }
                        sacList.Add(model);
                    }
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return sacList;
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

            if (Session[ConstHelper.GLOBAL_FROM_DATE] != null)
            {
                ViewData["GlobalFromDate"] = (DateTime)Session[ConstHelper.GLOBAL_FROM_DATE];
                Session[ConstHelper.GLOBAL_FROM_DATE] = null;
            }

            if (Session[ConstHelper.GLOBAL_TO_DATE] != null)
            {
                ViewData["GlobalToDate"] = (DateTime)Session[ConstHelper.GLOBAL_TO_DATE];
                Session[ConstHelper.GLOBAL_TO_DATE] = null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveStudentAdmissionCharges(int[] StudentIds, int[] Indexes, int[] PaidCharges)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                for (int i = 0; i < Indexes.Length; i++)
                {
                    //if (Indexes[i] > 0)
                    //{
                        int studentId = StudentIds[i];
                        int indexValue = Indexes[i];
                        int charges = PaidCharges[indexValue];
                        var paidList = acRepo.GetPaidStudentAdmissionChargesByStudentId(studentId);
                        if (paidList != null && paidList.Count > 0)
                        {
                            var admissionCHarges = paidList[0];
                            admissionCHarges.Amount = charges;
                            admissionCHarges.PaidDate = DateTime.Now;
                            acRepo.UpdatePaidAdmissionCharges(admissionCHarges);
                        }
                        else
                        {
                            SACPaidCatalog sac = new SACPaidCatalog();
                            sac.StudentId = studentId;
                            sac.Amount = charges;
                            sac.PaidDate = DateTime.Now;
                            acRepo.AddPaidAdmissionCharges(sac);
                        }

                    //}
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
        
    }
}