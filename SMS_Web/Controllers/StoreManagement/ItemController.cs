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
using System.Net.Http;
using System.Net;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json;

namespace SMS_Web.Controllers.StoreManagement
{
    public class ItemController : Controller
    {
        //private cs db = new cs();

        private IStoreRepository storeRepo;
        private static int errorCode = 0;
        //
        // GET: /Class/

        public ItemController()
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

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.STM_ITEMS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            Item item = new Item();
            try
            {
                if (id > 0)
                {
                    errorCode = 0;
                    item = storeRepo.GetItemById(id);
                }
                ViewData["Operation"] = id;
                ViewData["item"] = SessionHelper.ItemList();
                ViewBag.UnitId = new SelectList(SessionHelper.UnitList(), "Id", "Name", item.UnitId ?? 0);
                ViewBag.VendorNames = SessionHelper.VendorNameList();
                ViewData["Error"] = errorCode;
                errorCode = 0;

            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return View(item);
        }

        public ActionResult ItemVendor(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.STM_ITEMS) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            Item item = new Item();
            try
            {
                item = storeRepo.GetItemById(id);
                ViewBag.VendorNames = SessionHelper.VendorNameList();
                ViewBag.UnitId = new SelectList(SessionHelper.UnitList(), "Id", "Name", item.UnitId ?? 0);
                ViewData["itemVendor"] = storeRepo.GetItemVendorsByItemId(id);
                ViewData["Error"] = errorCode;
                errorCode = 0;

            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return View(item);
        }


        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Exclude = "Id")]Item item)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var itemObj = storeRepo.GetItemByName(item.ItemName);
                if (ModelState.IsValid && itemObj == null)
                {
                    item.UnitId = item.ItemUnit.Id;
                    item.ItemUnit = null;
                    item.CreatedOn = DateTime.Now;
                    int returnCode = storeRepo.AddItems(item);
                    if (returnCode == -1)
                        errorCode = 420;
                    else
                        errorCode = 2;
                    SessionHelper.InvalidateItemCache = false;
                }
                else if (itemObj != null)
                    errorCode = 11;
                else
                    errorCode = 1;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 420;
            }
            return RedirectToAction("Index");
                
        }

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Item item)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }


            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var storeObj = storeRepo.GetItemByNameAndId(item.ItemName, item.Id);
                
                if (ModelState.IsValid && storeObj == null)
                {
                    item.UnitId = item.ItemUnit.Id;
                    item.ItemUnit = null;
                    storeRepo.UpdateItem(item);
                    errorCode = 2;
                    SessionHelper.InvalidateItemCache = false;
                }
                else if (storeObj != null)
                    errorCode = 11;
                else
                    errorCode = 1;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            Item item = storeRepo.GetItemById(id);
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (item == null)
                {
                    return HttpNotFound();
                }

                storeRepo.DeleteItem(item);
                SessionHelper.InvalidateItemCache = false;
                errorCode = 4;
                
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex) { 
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 3;
            }
            return RedirectToAction("Index", new { id = 0 });
        }

        [HttpGet]
        [WebMethod()]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveItemIvendor(int itemId, int vendorId)
        {
            int result = 10;
            try
            {
                ItemVendor vendor = new ItemVendor();
                vendor.ItemId = itemId;
                vendor.VendorId = vendorId;

                storeRepo.AddItemVendor(vendor);
            }
            catch (Exception exc)
            {
                result = 420;
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpGet]
        [WebMethod()]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteItemIvendor(int id)
        {
            int result = 10;
            try
            {
                ItemVendor vendor = storeRepo.GetItemVendorById(id);
                storeRepo.DeleteItemVendor(vendor);
            }
            catch (Exception exc)
            {
                result = 420;
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}
