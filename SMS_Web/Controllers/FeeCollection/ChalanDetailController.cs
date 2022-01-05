using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_Web.Filters;
using SMS_DAL.ViewModel;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Helpers;
using SMS_Web.Controllers.SecurityAssurance;
using System.Globalization;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.FeeCollection
{
    public class ChalanDetailController : Controller
    {
        private static int errorCode = 0;
        //
        // GET: /ChalanDetail/

        
        IFeePlanRepository feePlanRepo;
        IClassRepository classRepo;
        //
        // GET: /FeeHead/

        public ChalanDetailController()
        {
            
            feePlanRepo = new FeePlanRepositoryImp(new SC_WEBEntities2());; 
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
        }


        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.FO_CHALLAN_DETAIL) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                if (id > 0)
                    errorCode = 0;
                ViewData["Error"] = errorCode;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["branchId"] = branchId;
                ViewBag.ClassId = new SelectList(SessionHelper.ActiveClassListDD(Session.SessionID), "Id", "Name");
                if ((Session[ConstHelper.SEARCH_CHALLAN_FLAG] != null && (bool)Session[ConstHelper.SEARCH_CHALLAN_FLAG] == true) || errorCode == 5 || errorCode == 6 || errorCode == 7 || errorCode == 8)
                {
                    Session[ConstHelper.SEARCH_CHALLAN_FLAG] = false;
                    return View(GetChallanDetail());
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
            //return View(challanDetail.ToList());
        }

        public JsonResult GetFeeDetailsByStatus(string from = null, string to = null)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DateTime fromDate = DateTime.Now.AddDays(DateTime.Now.Day * -1 + 1);
                if (from != null && from.Length > 24)
                    fromDate = DateTime.ParseExact(from.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                DateTime toDate = DateTime.Now;
                if (to != null && to.Length > 24)
                {
                    toDate = DateTime.ParseExact(to.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return Json(feePlanRepo.GetFeeDetailsByStatus(branchId, fromDate, toDate), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }

        public JsonResult GetFeeDetailsByClassSection(string from = null, string to = null, int status = 0, int classId = 0)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DateTime fromDate = DateTime.Now.AddDays(DateTime.Now.Day * -1 + 1);
                if (from != null && from.Length > 24)
                    fromDate = DateTime.ParseExact(from.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                DateTime toDate = DateTime.Now;
                if (to != null && to.Length > 24)
                {
                    toDate = DateTime.ParseExact(to.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                string currentMonth = SessionHelper.GetMonthName(DateTime.Now.Month) + "-" + (DateTime.Now.ToString("yyyy"));
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return Json(feePlanRepo.GetFeeDetailsByClassSection(branchId, status, fromDate, toDate, classId), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                return null;
            }
        }

        public ActionResult CreateFilteredChalanSheet(int classId, int sectionId, int feeStatus, string from, string to)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                DateTime fromDate = DateTime.Now.AddDays(DateTime.Now.Day * -1);
                if (from != null && from.Length > 24)
                    fromDate = DateTime.ParseExact(from.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                DateTime toDate = DateTime.Now;
                if (to != null && to.Length > 24)
                {
                    toDate = DateTime.ParseExact(to.Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var chalans = feePlanRepo.SearchChallanByStatus(branchId, classId, sectionId, feeStatus, fromDate, toDate);
                if (chalans.First() != null)
                {
                    ViewData["ClassName"] = chalans.First().Class;
                    ViewData["SectionName"] = chalans.First().Section;
                    ViewData["FeeStatus"] = feeStatus == 1 ? "Pending" : feeStatus == 2 ? "Unpaid" : "Paid";
                    ViewData["FromDate"] = fromDate.ToString("dd/MM/yyyy");
                    ViewData["ToDate"] = toDate.ToString("dd/MM/yyyy");
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return View("ChalanStatusSheet", chalans);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return View("ChalanStatusSheet", new List<IssuedChallanViewModel>());
        }

        //
        // GET: /ChalanDetail/Details/5

        //public ActionResult Details(int id = 0)
        //{
        //    ChallanFeeHeadDetail challanfeeheaddetail = db.ChallanFeeHeadDetails.Find(id);
        //    if (challanfeeheaddetail == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(challanfeeheaddetail);
        //}

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetChallanDetail(string ChallanId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            Session[ConstHelper.SEARCH_CHALLAN_ID] = int.Parse(ChallanId);
            Session[ConstHelper.SEARCH_CHALLAN_FLAG] = true;
            errorCode = 0;
            return RedirectToAction("Index", new { id = -59 });

        }

        private List<ChallanDetailViewModel> GetChallanDetail()
        {
            List<ChallanDetailViewModel> challanDetail = new List<ChallanDetailViewModel>();

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int searchChallanId = (int)Session[ConstHelper.SEARCH_CHALLAN_ID];
                challanDetail = feePlanRepo.GetChallDetailByChallanId(searchChallanId);
                if (challanDetail == null || challanDetail.Count == 0)
                {
                    var feaHeadsList = SessionHelper.FeeHeadList(Session.SessionID);
                    foreach (FeeHead head in feaHeadsList)
                    {
                        ChallanFeeHeadDetail detail = new ChallanFeeHeadDetail();
                        detail.ChallanId = searchChallanId;
                        detail.HeadId = head.Id;
                        detail.Amount = 0;
                        feePlanRepo.AddChallanDetail(detail);
                    }
                    challanDetail = feePlanRepo.GetChallDetailByChallanId(searchChallanId);
                }
                Session[ConstHelper.SEARCH_CHALLAN_ID] = 0;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return (List<ChallanDetailViewModel>)challanDetail;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveChallanDetail(int[] ChalanIds, int[] Indexes, string[] PaidAmount)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                for (int i = 0; i < ChalanIds.Count(); i++)
                {
                    int PaidIndex = Indexes[i];
                    int paidAmount = int.Parse(PaidAmount[PaidIndex]);
                    int chalanId = ChalanIds[i];
                    ChallanFeeHeadDetail detail = feePlanRepo.GetChallDetailById(chalanId);
                    detail.Amount = paidAmount;
                    feePlanRepo.UpdateChallanDetail(detail);
                }
                errorCode = 2;
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