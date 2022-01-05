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
using System.Configuration;
using SMS_DAL.ViewModel;
using Logger;
using System.Reflection;

namespace SMS_Web.Controllers.SecurityAssurance
{
    public static class UserPermissionController
    {
        private static ISecurityRepository secRepo = new SecurityRepositoryImp(new SC_WEBEntities2());
        //private static int errorCode = 0;
        ////
        //// GET: /Class/

        //public static UserPermissionController()
        //{
        //    secRepo = new SecurityRepositoryImp(SessionHelper.dbContext);
        //}

        public static SessionModel GetSessionModel(string browserDetail)
        {
            SessionModel model = null;
            try
            {
                if (browserDetail != null && browserDetail.Length > 0)
                    SessionHelper.session.TryGetValue(browserDetail, out model);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return model;
        }

        public static int GetUserId(string browserDetail)
        {
            SessionModel model = null;
            int UserId = 0;
            try
            {
                if (browserDetail != null && browserDetail.Length > 0)
                    SessionHelper.session.TryGetValue(browserDetail, out model);
                if (model != null)
                    UserId = (int)model.SESSION_USER.USER_ID;
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return (int)UserId;
        }

        public static bool CheckUserPermission(string browserDetail, string permissionName)
        {
            bool flag = false;

            try
            {
                SessionModel model = GetSessionModel(browserDetail);
                if (model != null && model.USER_MODULE_PERMISSIONS != null)
                {
                    string permissions = model.USER_SUB_MODULE_PERMISSIONS;
                    if (permissions.Contains(permissionName))
                        flag = true;
                }
                if(flag == false)
                    LogWriter.WriteLog("Permission is not found for the user");
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return flag;
        }

        public static int CheckAdminLogin(string browserDetail)
        {
            SessionModel model = GetSessionModel(browserDetail);
            if (model.USER_NAME == "hamad")
            {
                return 1;
            }

            return 1;
        }
        public static int GetLoginBranchId(string browserDetail)
        {
            int branchId = 0;
            try
            {
                SessionModel model = GetSessionModel(browserDetail);
                branchId = model.BRANCH_ID;
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return branchId;
        }

        public static void SetLoginBranchId(string browserDetail, int branchId)
        {
            try
            {
                SessionModel model = null;
                if (browserDetail != null && browserDetail.Length > 0)
                    SessionHelper.session.TryGetValue(browserDetail, out model);
                model.BRANCH_ID = branchId;
                SessionHelper.session.Remove(browserDetail);
                SessionHelper.session.Add(browserDetail, model);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
        }

        public static bool isMoscoUser(string browserDetail)
        {
            bool flag = false;
            try
            {
                SessionModel model = null;
                if (browserDetail != null && browserDetail.Length > 0)
                    SessionHelper.session.TryGetValue(browserDetail, out model);
                if (model.USER_NAME.Equals("mosco"))
                    flag = true;
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return flag;
        }

        public static bool CheckUserLoginStatus(string browserDetail)
        {
            bool flag = false;
            try
            {
                SessionModel model = GetSessionModel(browserDetail);
                if (model != null)
                {
                    //SessionUser sessionUser = (SessionUser)System.Web.HttpContext.Current.Session[ConstHelper.SESSION_USER];
                    SessionUser sessionUser = model.SESSION_USER;
                    DateTime loginDate = (DateTime)sessionUser.LOGIN_TIME;
                    //int sessionTimeOut = int.Parse(ConfigurationManager.AppSettings["SessionTimeOut"]);
                    if ((DateTime.Now - loginDate).Minutes < 30 && sessionUser.LOGIN_STATUS == 1)
                    {
                        sessionUser.LOGIN_TIME = DateTime.Now;
                        flag = true;
                    }
                    else
                    {
                        if (sessionUser.LOGIN_STATUS == 1)
                        {
                            sessionUser.LOGIN_STATUS = 0;
                            sessionUser.LOGOUT_TIME = DateTime.Now;
                            sessionUser.User = null;
                            secRepo.UpdateSessionUser(sessionUser);
                            LogWriter.WriteLog("User is logged out, as session is timed out");
                            LogoutUser(browserDetail);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return flag;
        }

        public static int AuthenticateBranch(string userName, string password)
        {
            int loginStatus = 0;
            try
            {
                Branch loginBranch = secRepo.AuthenticateBracnh(userName, password);
                if (loginBranch != null)
                {
                    loginStatus = 1;
                    System.Web.HttpContext.Current.Session[ConstHelper.BRANCH_ID] = loginBranch.ID;
                }
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }

            return loginStatus;
        }

        public static int AuthenticateUser(string userName, string password, string browserDetail)
        {
            int loginStatus = 0;
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, -1);
                //int branchId = GetLoginBranchId(Session.SessionID);
                //User loginUser = secRepo.AuthenticateUser(userName, password, branchId);
                LogWriter.WriteLog("Authenticating the user username : " + userName);
                User loginUser = secRepo.AuthenticateUser(userName, password);
                if (loginUser != null)
                {
                    LogWriter.WriteLog("User is authenticating successfully");
                    LogWriter.WriteLog("Starting user session.......");
                    SessionModel model = new SessionModel();
                    SessionUser sessionUser = new SessionUser();
                    sessionUser.LOGIN_STATUS = 1;
                    sessionUser.USER_ID = loginUser.Id;
                    sessionUser.User = loginUser;
                    sessionUser.LOGIN_TIME = DateTime.Now;
                    sessionUser.LOGOUT_TIME = DateTime.Now;

                    secRepo.AddSessionUser(sessionUser);
                    //System.Web.HttpContext.Current.Session[ConstHelper.SESSION_USER] = sessionUser;
                    model.SESSION_USER = sessionUser;

                    LogWriter.WriteLog("Getting user permissions.......");
                    List<PermissionModel> permissionList = secRepo.GetPermissionsByGroup((int)loginUser.UserGroupId);
                    string userSubModulePermissions = "|";
                    string userModulePermissions = "|";

                    foreach (PermissionModel perm in permissionList)
                    {
                        if (perm.Granted == true)
                        {
                            userSubModulePermissions += perm.SubModuleName + "|";
                            if (userModulePermissions.Contains(perm.ModuleName) == false)
                            {
                                userModulePermissions += perm.ModuleName + "|";
                            }
                        }
                    }

                    //System.Web.HttpContext.Current.Session[ConstHelper.BRANCH_ID] = loginUser.BranchId;
                    //System.Web.HttpContext.Current.Session[ConstHelper.USER_MODULE_PERMISSIONS] = userModulePermissions;
                    //System.Web.HttpContext.Current.Session[ConstHelper.USER_SUB_MODULE_PERMISSIONS] = userSubModulePermissions;
                    //System.Web.HttpContext.Current.Session[ConstHelper.USER_NAME] = loginUser.UserName;

                    model.BRANCH_ID = (int)loginUser.BranchId;
                    model.STAFF_ID = loginUser.StaffId;
                    model.USER_MODULE_PERMISSIONS = userModulePermissions;
                    model.USER_SUB_MODULE_PERMISSIONS = userSubModulePermissions;
                    model.USER_NAME = loginUser.UserName;

                    if (SessionHelper.userList.IndexOf(model.USER_NAME) >= 0)
                    {
                        SessionHelper.userList.Remove(model.USER_NAME);
                    }
                    LogWriter.WriteLog("Adding user session.......");
                    SessionModel tempModel = null;
                    SessionHelper.session.TryGetValue(browserDetail, out tempModel);
                    if (tempModel != null)
                    {
                        SessionHelper.session.Remove(browserDetail);
                    }
                    SessionHelper.userList.Add(model.USER_NAME);
                    SessionHelper.session.Add(browserDetail, model);
                    loginStatus = 1;
                }
                else
                {
                    LogWriter.WriteLog("User is not authenticated");
                }
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return loginStatus;
        }

        public static int LogoutUser(string browserDetail)
        {
            int logOutStatus = 0;
            try
            {
                LogWriter.WriteProcStartLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, GetUserId(browserDetail));
                SessionModel model = GetSessionModel(browserDetail);

                if (model != null)
                {
                    SessionUser sessionUser = model.SESSION_USER;
                    sessionUser.LOGIN_STATUS = 0;
                    sessionUser.LOGOUT_TIME = DateTime.Now;
                    sessionUser.User = null;
                    secRepo.UpdateSessionUser(sessionUser);

                    SessionHelper.session.Remove(browserDetail);
                    SessionHelper.userList.Remove(model.USER_NAME);
                    //System.Web.HttpContext.Current.Session[ConstHelper.SESSION_USER] = null;
                    //System.Web.HttpContext.Current.Session[ConstHelper.BRANCH_ID] = null;
                    //System.Web.HttpContext.Current.Session[ConstHelper.USER_MODULE_PERMISSIONS] = null;
                    //System.Web.HttpContext.Current.Session[ConstHelper.USER_SUB_MODULE_PERMISSIONS] = null;
                    //System.Web.HttpContext.Current.Session[ConstHelper.USER_NAME] = null;
                    logOutStatus = 1;
                }
                else
                    logOutStatus = 1;
                LogWriter.WriteProcEndLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return logOutStatus;
        }
    }
}
