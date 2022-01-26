using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;
using SMS_Web.Filters;
using SMS_DAL.SmsRepository.IRepository;
using SMS_DAL.SmsRepository.RepositoryImp;
using SMS_Web.Controllers.SecurityAssurance;
using SMS_Web.Helpers;
using Logger;
using System.Reflection;
using SMS_DAL.ViewModel;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace SMS_Web.Controllers.StoreManagement
{
    public class ItemPurchaseController : Controller
    {
        //private cs db = new cs();

        private IStoreRepository storeRepo;
        SMS_DAL.Reports.DAL_Store_Reports storeDs = new SMS_DAL.Reports.DAL_Store_Reports();
        private ISecurityRepository secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
        private static int errorCode = 0;
        //
        // GET: /Class/

        public ItemPurchaseController()
        {
            storeRepo = new StoreRepositoryImp(new SC_WEBEntities2());
        }

        //[OutputCache(Duration = 300, VaryByParam = "none")]
        public ActionResult Index( int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.STM_ITEM_PURCHASE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            try
            {
                if (id > 0)
                {
                    var itemPurchase = storeRepo.GetItemPurchaseById(id);
                    ViewData["OrderId"] = itemPurchase.OrderId;
                    ViewData["OrderDate"] = itemPurchase.PurchaseDate;
                    ViewData["itemPurchase"] = storeRepo.GetAllItemPurchaseDetailModelByItemPurchaseId(itemPurchase.Id);
                }
                else
                {
                    ViewData["OrderId"] = storeRepo.GetPurchaseOrderId();
                }
                ViewBag.ItemNames = SessionHelper.ItemNamesList();
                ViewBag.UnitId = new SelectList(SessionHelper.UnitList(), "Id", "Name");
                ViewData["Error"] = errorCode;
                errorCode = 0;
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveItemPurchase(List<ItemPurchaseDetailModel> itemPurchaseList, DateTime PurchaseDate, int OrderId, int Print)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var itemPurchase = storeRepo.GetItemPurchaseByOrderId(OrderId);
                if (Print == 1)
                {
                    return PrintItemPurchase(OrderId);
                }
                else
                {
                    if (itemPurchase == null)
                    {
                        AddNewItemPurchase(itemPurchaseList, PurchaseDate, OrderId);
                    }
                    else
                    {
                        UpdateItemPurchase(itemPurchaseList, PurchaseDate, itemPurchase);
                    }
                    if (Print == 2)
                    {
                        return PrintItemPurchase(OrderId);
                    }
                }
            }
            catch (Exception exc)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        public void AddNewItemPurchase(List<ItemPurchaseDetailModel> itemPurchaseList, DateTime PurchaseDate, int OrderId)
        {
            try
            {
                ItemPurchase purchase = new ItemPurchase();
                purchase.OrderId = OrderId;
                purchase.PurchaseDate = PurchaseDate;
                purchase.CreatedOn = DateTime.Now;
                storeRepo.AddItemPurchase(purchase);

                foreach (var model in itemPurchaseList)
                {
                    var itemName = model.ItemName.Trim();
                    var item = SessionHelper.ItemList().Where(x => x.ItemName == itemName).FirstOrDefault();
                    ItemPurchaseDetail detail = new ItemPurchaseDetail();
                    detail.ItemPurchaseId = purchase.Id;
                    detail.Quantity = model.Quantity;
                    detail.Rate = model.Rate;
                    detail.Total = model.Total;
                    detail.ItemId = item.Id;
                    detail.CreatedOn = DateTime.Now;

                    storeRepo.AddItemPurchaseDetail(detail);
                }

                purchase.Amount = (int) itemPurchaseList.Sum(x => x.Total);
                storeRepo.UpdateItemPurchase(purchase);
                errorCode = 2;
            }
            catch (Exception exc)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
        }

        public void UpdateItemPurchase(List<ItemPurchaseDetailModel> itemPurchaseList, DateTime PurchaseDate, ItemPurchase purchase)
        {
            try
            {
                List<ItemPurchaseDetail> exisitngPurchaseDetail = storeRepo.GetAllItemPurchaseDetailByItemPurchaseId(purchase.Id);

                foreach (var model in exisitngPurchaseDetail)
                {
                    storeRepo.DeleteItemPurchaseDetail(model);
                }

                foreach (var model in itemPurchaseList)
                {
                    var itemName = model.ItemName.Trim();
                    var item = SessionHelper.ItemList().Where(x => x.ItemName == itemName).FirstOrDefault();
                    ItemPurchaseDetail detail = new ItemPurchaseDetail();
                    detail.ItemPurchaseId = purchase.Id;
                    detail.Quantity = model.Quantity;
                    detail.Rate = model.Rate;
                    detail.Total = model.Total;
                    detail.ItemId = item.Id;
                    detail.CreatedOn = DateTime.Now;

                    storeRepo.AddItemPurchaseDetail(detail);
                }

                purchase.UpdatedOn = DateTime.Now;
                purchase.PurchaseDate = PurchaseDate;
                purchase.Amount = (int)itemPurchaseList.Sum(x => x.Total);
                storeRepo.UpdateItemPurchase(purchase);
                errorCode = 2;
            }
            catch (Exception exc)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
        }

        public ActionResult Search()
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.STM_ITEM_PURCHASE) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }

            try
            {
                List<ItemPurchaseModel> purchaseList = new List<ItemPurchaseModel>();
                if (Session[ConstHelper.STM_ITEM_PURCHASE_LIST] != null)
                {
                    purchaseList = (List<ItemPurchaseModel>)Session[ConstHelper.STM_ITEM_PURCHASE_LIST];
                    Session[ConstHelper.STM_ITEM_PURCHASE_LIST] = null;
                    ViewData["itemPurchase"] = purchaseList;
                }
                ViewData["Error"] = errorCode;
                errorCode = 0;
                ViewBag.ItemNames = SessionHelper.ItemNamesList();
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchItemPurchase(DateTime FromDate, DateTime ToDate, int OrderId = 0, int ItemId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var purchaseList = storeRepo.SearchItemPurchase(FromDate, ToDate, OrderId, ItemId);
                Session[ConstHelper.STM_ITEM_PURCHASE_LIST] = purchaseList;
            }
            catch (Exception exc)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return RedirectToAction("Search");
        }

        public ActionResult CreatePdf(int id = 0)
        {
            var itemPurchase = storeRepo.GetItemPurchaseById(id);
            return PrintItemPurchase(itemPurchase.OrderId);
        }
        public FileStreamResult PrintItemPurchase(int orderId)
        {
            DataSet ds = storeDs.GetItemPurchase(orderId);

            return showReportAsPdf(ds);
        }

        public FileStreamResult showReportAsPdf(DataSet ds)
        {
            ReportDocument rd = createReport(ds);

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            string fileName = "ItemReportOrder.pdf";
            var contentLength = stream.Length;
            Response.AppendHeader("Content-Length", contentLength.ToString());
            Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName);

            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf");

        }

        public ReportDocument createReport(DataSet ds)
        {
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            ReportDocument rd = new ReportDocument();
            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);

            rd.Load(Path.Combine(Server.MapPath("~/Reports/Store"), "ItemPurchaseOrder.rpt"));

            rd.Database.Tables["StoreDataTable"].SetDataSource(ds.Tables[0]);
            rd.Database.Tables["DataTable1"].SetDataSource(AddImage());

            rd.SetParameterValue("CampusName", config.CampusName);
            rd.SetParameterValue("SchoolName", config.SchoolName);
            return rd;
        }

        private DataTable AddImage()
        {
            DataTable tbl = new DataTable();
            tbl.Rows.Add();

            int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
            SchoolConfig config = secRepo.GetSchoolConfigByBranchId(branchId);
            DataColumn colByteArray = new DataColumn("ReportImage");
            colByteArray.DataType = System.Type.GetType("System.Byte[]");
            tbl.Columns.Add(colByteArray);
            tbl.Rows[0]["ReportImage"] = config.SchoolLogo;

            return tbl;
        }
    }
}