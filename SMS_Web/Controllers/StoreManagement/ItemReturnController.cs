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
    public class ItemReturnController : Controller
    {
        //private cs db = new cs();

        private IStoreRepository storeRepo;
        SMS_DAL.Reports.DAL_Store_Reports storeDs = new SMS_DAL.Reports.DAL_Store_Reports();
        private ISecurityRepository secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
        private static int errorCode = 0;
        //
        // GET: /Class/

        public ItemReturnController()
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

            //if (UserPermissionController.CheckUserPermission(Session.SessionID,ConstHelper.SA_CLASSES) == false)
            //{
            //    return RedirectToAction("Index", "NoPermission");
            //}
            try
            {
                if (id > 0)
                {
                    var itemReturn = storeRepo.GetItemReturnById(id);
                    ViewData["OrderId"] = itemReturn.OrderId;
                    ViewData["OrderDate"] = itemReturn.ReturnDate;
                    ViewData["itemReturn"] = storeRepo.GetAllItemReturnDetailModelByItemReturnId(itemReturn.Id);
                }
                else
                {
                    ViewData["OrderId"] = storeRepo.GetReturnOrderId();
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
        public ActionResult SaveItemReturn(List<ItemReturnDetailModel> itemReturnList, DateTime ReturnDate, int OrderId, int Print)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var itemReturn = storeRepo.GetItemReturnByOrderId(OrderId);
                if (Print == 1)
                {
                    return PrintItemReturn(OrderId);
                }
                else
                {
                    if (itemReturn == null)
                    {
                        AddNewItemIssuance(itemReturnList, ReturnDate, OrderId);
                    }
                    else
                    {
                        UpdateItemReturn(itemReturnList, ReturnDate, itemReturn);
                    }
                    if (Print == 2)
                    {
                        return PrintItemReturn(OrderId);
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

        public void AddNewItemIssuance(List<ItemReturnDetailModel> itemReturnList, DateTime ReturnDate, int OrderId)
        {
            try
            {
                ItemReturn ireturn = new ItemReturn();
                ireturn.OrderId = OrderId;
                ireturn.ReturnDate = ReturnDate;
                ireturn.CreatedOn = DateTime.Now;
                storeRepo.AddItemReturn(ireturn);

                foreach (var model in itemReturnList)
                {
                    var itemName = model.ItemName.Trim();
                    var unitName = model.UnitName.Trim();
                    var item = SessionHelper.ItemList().Where(x => x.ItemName == itemName).FirstOrDefault();
                    var unit = SessionHelper.UnitList().Where(x => x.Name == unitName).FirstOrDefault();
                    ItemReturnDetail detail = new ItemReturnDetail();
                    detail.ItemReturnId = ireturn.Id;
                    detail.Quantity = model.Quantity;
                    detail.Rate = model.Rate;
                    detail.Total = model.Total;
                    detail.ItemId = item.Id;
                    detail.CreatedOn = DateTime.Now;
                    detail.UnitId = unit.Id;

                    storeRepo.AddItemReturnDetail(detail);
                    ItemQuantitySettlement((int)detail.ItemId, detail.Id, (decimal)detail.Quantity);
                }

                ireturn.Amount = (int)itemReturnList.Sum(x => x.Total ?? 0);
                storeRepo.UpdateItemReturn(ireturn);
                errorCode = 2;
            }
            catch (Exception exc)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
        }

        public void UpdateItemReturn(List<ItemReturnDetailModel> itemReturnList, DateTime ReturnDate, ItemReturn itemReturn)
        {
            try
            {
                List<ItemReturnDetail> exisitngReturnDetail = storeRepo.GetAllItemReturnDetailByItemReturnId(itemReturn.Id);

                foreach (var model in exisitngReturnDetail)
                {
                    ItemQuantityReverseSettlement(model.Id);
                    storeRepo.DeleteItemReturnDetail(model);
                }

                foreach (var model in itemReturnList)
                {
                    var itemName = model.ItemName.Trim();
                    var unitName = model.UnitName.Trim();
                    var item = SessionHelper.ItemList().Where(x => x.ItemName == itemName).FirstOrDefault();
                    var unit = SessionHelper.UnitList().Where(x => x.Name == unitName).FirstOrDefault();
                    ItemReturnDetail detail = new ItemReturnDetail();
                    detail.ItemReturnId = itemReturn.Id;
                    detail.Quantity = model.Quantity;
                    detail.Rate = model.Rate;
                    detail.Total = model.Total;
                    detail.ItemId = item.Id;
                    detail.CreatedOn = DateTime.Now;
                    detail.UnitId = unit.Id;

                    storeRepo.AddItemReturnDetail(detail);
                    ItemQuantitySettlement((int)detail.ItemId, detail.Id, (decimal)detail.Quantity);
                }

                itemReturn.UpdatedOn = DateTime.Now;
                itemReturn.ReturnDate = ReturnDate;
                itemReturn.Amount = (int)itemReturnList.Sum(x => x.Total ?? 0);
                storeRepo.UpdateItemReturn(itemReturn);
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

            //if (UserPermissionController.CheckUserPermission(Session.SessionID,ConstHelper.SA_CLASSES) == false)
            //{
            //    return RedirectToAction("Index", "NoPermission");
            //}
            try
            {
                List<ItemReturnModel> returnList = new List<ItemReturnModel>();
                if (Session[ConstHelper.STM_ITEM_RETURN_LIST] != null)
                {
                    returnList = (List<ItemReturnModel>)Session[ConstHelper.STM_ITEM_RETURN_LIST];
                    Session[ConstHelper.STM_ITEM_RETURN_LIST] = null;
                    ViewData["itemRetrn"] = returnList;
                }
                ViewBag.ItemNames = SessionHelper.ItemNamesList();
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
        public ActionResult SearchItemReturn(DateTime FromDate, DateTime ToDate, int OrderId = 0, int ItemId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var returnList = storeRepo.SearchItemReturn(FromDate, ToDate, OrderId, ItemId);
                Session[ConstHelper.STM_ITEM_RETURN_LIST] = returnList;
            }
            catch (Exception exc)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return RedirectToAction("Search");
        }

        public void ItemQuantitySettlement(int itemId, int itemReturnId, decimal quantity)
        {
            List<ItemPurchaseDetailModel> purchaseListWithQuantity = storeRepo.GetItemPurchaseWithZeroQuantity(itemId);
            if (purchaseListWithQuantity == null || purchaseListWithQuantity.Count == 0)
            {
                purchaseListWithQuantity = storeRepo.GetItemPurchaseWithQuantity(itemId);
            }

            foreach (var purchase in purchaseListWithQuantity)
            {
                IssuanceStockQuantity stock = new IssuanceStockQuantity();
                var purchaseDetail = storeRepo.GetItemPurchaseDetailById(purchase.Id);
                var availableQuanatity = (decimal)purchaseDetail.IssuanceQuantity;
                decimal adjustableQuanatity = availableQuanatity - quantity >= 0 ? quantity : availableQuanatity;
                purchaseDetail.IssuanceQuantity = (purchaseDetail.IssuanceQuantity ?? 0) - adjustableQuanatity;
                storeRepo.UpdateItemPurchaseDetail(purchaseDetail);

                stock.ItemReturnDetailId = itemReturnId;
                stock.ItemPurchaseDetailId = purchaseDetail.Id;
                stock.Quantity = adjustableQuanatity;
                stock.CreatedOn = DateTime.Now;
                storeRepo.AddIssuanceStockQuantity(stock);

                quantity = quantity - adjustableQuanatity;
                if (quantity == 0)
                    break;
            }
        }

        public void ItemQuantityReverseSettlement(int itemIssuanceId)
        {
            var issuanceQuanatityList = storeRepo.GetIssuanceReturnQuanatityList(itemIssuanceId);
            foreach (var issuance in issuanceQuanatityList)
            {
                var itemPurchaseDetail = storeRepo.GetItemPurchaseDetailById((int)issuance.ItemPurchaseDetailId);
                itemPurchaseDetail.IssuanceQuantity = itemPurchaseDetail.IssuanceQuantity + issuance.Quantity;
                storeRepo.UpdateItemPurchaseDetail(itemPurchaseDetail);

                storeRepo.DeleteIssuanceStockQuantity(issuance);
            }
        }

        public ActionResult CreatePdf(int id = 0)
        {
            var itemReturn = storeRepo.GetItemReturnById(id);
            return PrintItemReturn(itemReturn.OrderId);
        }
        public FileStreamResult PrintItemReturn(int orderId)
        {
            DataSet ds = storeDs.GetItemReturn(orderId);

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

            rd.Load(Path.Combine(Server.MapPath("~/Reports/Store"), "ItemReturnOrder.rpt"));

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