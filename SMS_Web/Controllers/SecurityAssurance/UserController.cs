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
using SMS_Web.Filters;
using SMS_Web.Helpers;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.SecurityAssurance
{
    public class UserController : Controller
    {
        //private SC_WEBEntities2 db = SessionHelper.dbContext;

        
        private ISecurityRepository secRepo;
        private IStaffRepository staffRepo;
        private static int errorCode = 0, pErrorCode = 0;
        //
        // GET: /Class/

        public UserController()
        {
            staffRepo = new StaffRepositoryImp(new SC_WEBEntities2());; 
            secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());;
        }

        //
        // GET: /User/

        public ActionResult Index(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }
            if (UserPermissionController.CheckUserPermission(Session.SessionID, ConstHelper.SECA_Users) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            User user = new SMS_DAL.User();

            try
            {
                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                ViewData["branch"] = SessionHelper.BranchList;
                ViewData["users"] = secRepo.GetAllUsers(branchId);
                ViewData["Error"] = errorCode;
                errorCode = 0;
                user = secRepo.GetUserById(id);

                var generalStaff = new Staff();
                generalStaff.StaffId = 0;
                generalStaff.Name = "General User";
                var staffList = new List<Staff>();
                staffList.Add(generalStaff);
                staffList.AddRange(SessionHelper.StaffList(Session.SessionID));

                if (id > 0)
                {
                    //ViewBag.StaffId = new SelectList(staffRepo.GetAllStaff(), "StaffId", "Name", user.StaffId);
                    ViewBag.BranchId = new SelectList(SessionHelper.BranchList, "ID", "BRANCH_NAME", user.BranchId);
                    ViewBag.GroupId = new SelectList(secRepo.GetAllUserGroups(branchId), "Id", "Name", user.UserGroupId);
                    ViewBag.StaffId = new SelectList(staffList, "StaffId", "Name", user.StaffId);
                }
                else
                {
                    //ViewBag.StaffId = new SelectList(staffRepo.GetAllStaff(), "StaffId", "Name");
                    ViewBag.BranchId = new SelectList(SessionHelper.BranchList, "ID", "BRANCH_NAME");
                    ViewBag.GroupId = new SelectList(secRepo.GetAllUserGroups(branchId), "Id", "Name");
                    ViewBag.StaffId = new SelectList(staffList, "StaffId", "Name");
                }
                ViewBag.UserGroupId = new SelectList(secRepo.GetAllUserGroups(branchId), "Id", "Name");
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View(user);
        }

        public ActionResult ChangePassword()
        {
            try
            {
                int userId = UserPermissionController.GetUserId(Session.SessionID);
                ViewData["UserId"] = userId;
                ViewData["Error"] = pErrorCode;
                pErrorCode = 0;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]

        public ActionResult Create(User user)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = secRepo.GetUserByName(user.UserName, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    user.BranchId = user.Branch.ID;
                    user.UserGroupId = user.UserGroup.Id;
                    user.Staff = null;
                    user.UserGroup = null;
                    user.Branch = null;
                    user.StaffId = user.StaffId == 0 ? null : user.StaffId; 
                    int returnCode = secRepo.AddUser(user);
                    SessionHelper.InvalidateUserCache = false;
                    if (returnCode == -1)
                        errorCode = 420;
                    else
                        errorCode = 2;
                }
                else if (classObj != null)
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

        //
        // POST: /User/Edit/5

        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = secRepo.GetUserByNameAndId(user.UserName, user.Id, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    user.BranchId = user.Branch.ID;
                    user.Branch = null;
                    user.StaffId = user.StaffId == 0 ? null : user.StaffId;
                    user.UserGroupId = user.UserGroup.Id;
                    user.UserGroup = null;

                    secRepo.UpdateUser(user);
                    SessionHelper.InvalidateUserCache = false;
                    errorCode = 2;
                }
                else if (classObj != null)
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

        [HttpPost]
        [ActionName("ChangePassword")]
        [OnAction(ButtonName = "SaveChangePassword")]
        [ValidateAntiForgeryToken]
        public ActionResult SaveChangePassword(int UserId, string CurrentPassword, string NewPassword)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                User user = secRepo.GetUserById(UserId);
                if (user.Password == CurrentPassword)
                {
                    user.Password = NewPassword;
                    secRepo.UpdateUser(user);
                    pErrorCode = 2;
                    LogWriter.WriteLog("User password is changed");
                }
                else
                {
                    LogWriter.WriteLog("Current password is invalid");
                    pErrorCode = 20;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return RedirectToAction("ChangePassword");
        }

        //
        // GET: /User/Delete/5
        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                User user = secRepo.GetUserById(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                int userCount = secRepo.GetSessionUserCount(id);
                SessionHelper.InvalidateUserCache = false;

                if (userCount == 0)
                {
                    secRepo.DeleteUser(user);
                    errorCode = 4;
                }
                else
                    errorCode = 40;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 3;
            }
            return RedirectToAction("Index", new { id = 0 });
        }

    }
}