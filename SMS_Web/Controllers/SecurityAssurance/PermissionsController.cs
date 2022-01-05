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
using SMS_Web.Helpers;
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.SecurityAssurance
{
    public class PermissionsController : Controller
    {

        private ISecurityRepository secRepo;

        private SC_WEBEntities2 db = SessionHelper.dbContext;

        private static int errorCode = 0;
        //
        // GET: /Permissions/

        public PermissionsController()
        {
            secRepo = new SecurityRepositoryImp(new SC_WEBEntities2()); ;

        }

        public ActionResult Index(int id = 0)
        {
            //if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            //if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SECA_Permissions) == false)
            //{
            //    return RedirectToAction("Index", "NoPermission");
            //}
            List<PermissionModel> list = new List<PermissionModel>();
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                int groupId = (int)SMS_Web.Controllers.SecurityAssurance.UserPermissionController.GetSessionModel(Session.SessionID).SESSION_USER.User.UserGroupId;
                ViewBag.GroupId = new SelectList(secRepo.GetAllUserGroups(branchId).Where(x => x.Id != groupId).ToList(), "Id", "Name");

                list = secRepo.GetPermissionsByGroup(id);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(list);
        }

        public ActionResult GetGroupPermission(int Id = 0)
        {
            return RedirectToAction("Index", new { id = Id });
        }

        //
        // GET: /Permissions/Details/5

        public ActionResult Details(int id = 0)
        {
            Permission permission = db.Permissions.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            return View(permission);
        }

        [HttpPost]
        public ActionResult SavePermissions(List<PermissionModel> permissionsList)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                foreach (PermissionModel permission in permissionsList)
                {
                    if (permission != null)
                    {
                        var sessionObject = secRepo.GetPermissionById(permission.Id);
                        //if (sessionObject == null)
                        //{
                        //    permission.CreatedOn = DateTime.Now;
                        //    secRepo.AddPermission(permission);
                        //}
                        //else
                        //{
                        sessionObject.IsGranted = permission.Granted;
                        sessionObject.UpdatedOn = DateTime.Now;
                        secRepo.UpdateGroupPermission(sessionObject);
                        //}
                    }
                }

                errorCode = 10;
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

        //[HttpPost]
        //public JsonResult GetPermissionsByGroup(int GroupId)
        //{
        //    var permissions= secRepo.GetPermissionsByGroup(GroupId);
        //    List<string[]> permissionsList = new List<string[]>();
        //    foreach (Permission perm in permissions)
        //    {
        //        string[] tempObj = new string[3];
        //        tempObj[0] = perm.SubModuleName;
        //        tempObj[1] = perm.Granted.ToString();
        //        tempObj[2] = perm.GroupId.ToString();
        //        permissionsList.Add(tempObj);
        //    }
        //    return Json(permissionsList);
        //}

        //
        // GET: /Permissions/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Permissions/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Permission permission)
        {
            if (ModelState.IsValid)
            {
                db.Permissions.Add(permission);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(permission);
        }

        //
        // GET: /Permissions/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Permission permission = db.Permissions.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            return View(permission);
        }

        //
        // POST: /Permissions/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Permission permission)
        {
            if (ModelState.IsValid)
            {
                db.Entry(permission).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(permission);
        }

        //
        // GET: /Permissions/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Permission permission = db.Permissions.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            return View(permission);
        }

        //
        // POST: /Permissions/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Permission permission = db.Permissions.Find(id);
            db.Permissions.Remove(permission);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}