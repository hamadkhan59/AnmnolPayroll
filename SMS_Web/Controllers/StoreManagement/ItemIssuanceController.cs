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

namespace SMS_Web.Controllers.StoreManagement
{
    public class ItemIssuanceController : Controller
    {
        //private cs db = new cs();

        private IStoreRepository storeRepo;
        private static int errorCode = 0;
        //
        // GET: /Class/

        public ItemIssuanceController()
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
                    var itemIssuance = storeRepo.GetItemIssuanceById(id);
                    ViewData["OrderId"] = itemIssuance.OrderId;
                    ViewData["OrderDate"] = itemIssuance.IssuanceDate;
                    ViewData["itemIssuance"] = storeRepo.GetAllItemIssuanceDetailModelByItemIssuanceId(itemIssuance.Id);
                }
                else
                {
                    ViewData["OrderId"] = storeRepo.GetIssuanceOrderId();
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
        public ActionResult SaveItemIssuance(List<ItemIssuanceDetailModel> itemIssuanceList, DateTime IssuanceDate, int OrderId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var itemIssuance = storeRepo.GetItemIssuanceByOrderId(OrderId);
                if (itemIssuance == null)
                {
                    AddNewItemIssuance(itemIssuanceList, IssuanceDate, OrderId);
                }
                else
                {
                    UpdateItemIssuance(itemIssuanceList, IssuanceDate, itemIssuance);
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

        public void AddNewItemIssuance(List<ItemIssuanceDetailModel> itemIssuanceList, DateTime IssuanceDate, int OrderId)
        {
            try
            {
                ItemIssuance issuance = new ItemIssuance();
                issuance.OrderId = OrderId;
                issuance.IssuanceDate = IssuanceDate;
                issuance.CreatedOn = DateTime.Now;
                storeRepo.AddItemIssuance(issuance);

                foreach (var model in itemIssuanceList)
                {
                    var itemName = model.ItemName.Trim();
                    var unitName = model.UnitName.Trim();
                    var item = SessionHelper.ItemList().Where(x => x.ItemName == itemName).FirstOrDefault();
                    var unit = SessionHelper.UnitList().Where(x => x.Name == unitName).FirstOrDefault();
                    ItemIssuanceDetail detail = new ItemIssuanceDetail();
                    detail.ItemIssuanceId = issuance.Id;
                    detail.Quantity = model.Quantity;
                    detail.Rate = model.Rate;
                    detail.Total = model.Total;
                    detail.ItemId = item.Id;
                    detail.CreatedOn = DateTime.Now;
                    detail.UnitId = unit.Id;

                    storeRepo.AddItemIssuanceDetail(detail);
                    ItemQuantitySettlement((int)detail.ItemId, detail.Id, (decimal)detail.Quantity);
                }

                issuance.Amount = (int)itemIssuanceList.Sum(x => x.Total ?? 0);
                storeRepo.UpdateItemIssuance(issuance);
                errorCode = 2;
            }
            catch (Exception exc)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
        }

        public void UpdateItemIssuance(List<ItemIssuanceDetailModel> itemIssuanceList, DateTime IssuanceDate, ItemIssuance issuance)
        {
            try
            {
                List<ItemIssuanceDetail> exisitngIssuanceDetail = storeRepo.GetAllItemIssuanceDetailByItemIssuanceId(issuance.Id);

                foreach (var model in exisitngIssuanceDetail)
                {
                    ItemQuantityReverseSettlement(model.Id);
                    storeRepo.DeleteItemIssuanceDetail(model);
                }

                foreach (var model in itemIssuanceList)
                {
                    var itemName = model.ItemName.Trim();
                    var unitName = model.UnitName.Trim();
                    var item = SessionHelper.ItemList().Where(x => x.ItemName == itemName).FirstOrDefault();
                    var unit = SessionHelper.UnitList().Where(x => x.Name == unitName).FirstOrDefault();
                    ItemIssuanceDetail detail = new ItemIssuanceDetail();
                    detail.ItemIssuanceId = issuance.Id;
                    detail.Quantity = model.Quantity;
                    detail.Rate = model.Rate;
                    detail.Total = model.Total;
                    detail.ItemId = item.Id;
                    detail.CreatedOn = DateTime.Now;
                    detail.UnitId = unit.Id;

                    storeRepo.AddItemIssuanceDetail(detail);
                    ItemQuantitySettlement((int)detail.ItemId, detail.Id, (decimal)detail.Quantity);
                }

                issuance.UpdatedOn = DateTime.Now;
                issuance.IssuanceDate = IssuanceDate;
                issuance.Amount = (int)itemIssuanceList.Sum(x => x.Total ?? 0);
                storeRepo.UpdateItemIssuance(issuance);
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
                List<ItemIssuanceModel> issuanceList = new List<ItemIssuanceModel>();
                if (Session[ConstHelper.STM_ITEM_ISSUANCE_LIST] != null)
                {
                    issuanceList = (List<ItemIssuanceModel>)Session[ConstHelper.STM_ITEM_ISSUANCE_LIST];
                    Session[ConstHelper.STM_ITEM_ISSUANCE_LIST] = null;
                    ViewData["itemIssuance"] = issuanceList;
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
        public ActionResult SearchItemIssuance(DateTime FromDate, DateTime ToDate, int OrderId = 0, int ItemId = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var issuanceList = storeRepo.SearchItemIssuanc(FromDate, ToDate, OrderId, ItemId);
                Session[ConstHelper.STM_ITEM_ISSUANCE_LIST] = issuanceList;
            }
            catch (Exception exc)
            {
                errorCode = 420;
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return RedirectToAction("Search");
        }

        public void ItemQuantitySettlement(int itemId, int itemIssuanceId, decimal quantity)
        {
            var purchaseListWithQuantity = storeRepo.GetItemPurchaseWithQuantity(itemId);
            foreach (var purchase in purchaseListWithQuantity)
            {
                IssuanceStockQuantity stock = new IssuanceStockQuantity();
                var purchaseDetail = storeRepo.GetItemPurchaseDetailById(purchase.Id);
                var availableQuanatity = (decimal) (purchaseDetail.Quantity - (purchaseDetail.IssuanceQuantity ?? 0));
                decimal adjustableQuanatity = availableQuanatity - quantity >= 0 ? quantity : availableQuanatity;
                purchaseDetail.IssuanceQuantity = (purchaseDetail.IssuanceQuantity ?? 0) + adjustableQuanatity;
                storeRepo.UpdateItemPurchaseDetail(purchaseDetail);

                stock.ItemIssuanceDetailId = itemIssuanceId;
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
            var issuanceQuanatityList = storeRepo.GetIssuanceQuanatityList(itemIssuanceId);
            foreach (var issuance in issuanceQuanatityList)
            {
                var itemPurchaseDetail = storeRepo.GetItemPurchaseDetailById((int)issuance.ItemPurchaseDetailId);
                itemPurchaseDetail.IssuanceQuantity = itemPurchaseDetail.IssuanceQuantity - issuance.Quantity;
                storeRepo.UpdateItemPurchaseDetail(itemPurchaseDetail);

                storeRepo.DeleteIssuanceStockQuantity(issuance);
            }
        }
    }
}