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
    public class ItemPurchaseController : Controller
    {
        //private cs db = new cs();

        private IStoreRepository storeRepo;
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

            //if (UserPermissionController.CheckUserPermission(Session.SessionID,ConstHelper.SA_CLASSES) == false)
            //{
            //    return RedirectToAction("Index", "NoPermission");
            //}
            try
            {
                ViewData["OrderId"] = storeRepo.GetPurchaseOrderId();
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
        public ActionResult SaveItemPurchase(List<ItemPurchaseDetailModel> itemPurchaseList, DateTime PurchaseDate, int OrderId)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var itemPurchase = storeRepo.GetItemPurchaseByOrderId(OrderId);
                if (itemPurchase == null)
                {
                    AddNewItemPurchase(itemPurchaseList, PurchaseDate, OrderId);
                }
                else
                {
                    UpdateItemPurchase(itemPurchaseList, PurchaseDate, itemPurchase);
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
                    var itemName = model.ItemName;
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
                    var itemName = model.ItemName;
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
    }
}