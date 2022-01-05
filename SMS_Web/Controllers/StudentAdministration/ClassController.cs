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

namespace SMS_Web.Controllers.StudentAdministration
{
    public class ClassController : Controller
    {
        //private cs db = new cs();

        private IClassRepository classRepo;
        private IClassSectionRepository classSecRepo;
        private IFinanceAccountRepository financeRepo;
        private static int errorCode = 0;
        //
        // GET: /Class/

        public ClassController()
        {
            classRepo = new ClassRepositoryImp(new SC_WEBEntities2());
            classSecRepo = new ClassSectionRepositoryImp(new SC_WEBEntities2());
            financeRepo = new FinanceAccountRepositoryImp(new SC_WEBEntities2());;
        }

        //[OutputCache(Duration = 300, VaryByParam = "none")]
        public ActionResult Index( int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            if (UserPermissionController.CheckUserPermission(Session.SessionID,ConstHelper.SA_CLASSES) == false)
            {
                return RedirectToAction("Index", "NoPermission");
            }
            Class clas = new Class();
            try
            {
                if (id > 0)
                    errorCode = 0;
                ViewData["Operation"] = id;
                ViewData["class"] = SessionHelper.ClassList(Session.SessionID);
                ViewData["Error"] = errorCode;
                errorCode = 0;

                clas = classRepo.GetClassById(id);
            }
            catch (Exception exc)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(exc);
            }
            return View(clas);
        }

       
        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Create")]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Exclude = "Id")]Class clas)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = classRepo.GetClassByName(clas.Name, branchId);
                clas.IsFinanceAccountOpen = true;
                clas.BranchId = branchId;
                clas.Branch = null;
                //if (clas.IsFinanceAccountOpen)
                createFinanceAccount(clas.Name, branchId);
                if (ModelState.IsValid && classObj == null)
                {
                    int returnCode = classRepo.AddClass(clas);
                    if (returnCode == -1)
                        errorCode = 420;
                    else
                        errorCode = 2;
                    SessionHelper.InvalidateClassCache = false;
                    SessionHelper.InvalidateClassSectionCache = false;
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

        private int createFinanceAccount(string className, int branchId)
        {
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                var financeObj = financeRepo.GetFinanceThirdLvlAccountByName(className, branchId);
                    FinanceThirdLvlAccount ftla = new FinanceThirdLvlAccount();
                if (financeObj == null)
                {
                    ftla.AccountName = className;
                    ftla.AccountDescription = "This account is created for class " + className + " fee collection";
                    ftla.CreatedOn = DateTime.Now;
                    ftla.SeccondLvlAccountId = 28;
                    ftla.BranchId = branchId;
                    financeRepo.AddFinanceThirdLvlAccount(ftla);
                    SessionHelper.InvalidateFinanceThirdLvlCache = false;
                }
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return ftla.Id;
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return 0;
        }
        
        [HttpPost]
        [ActionName("Index")]
        [OnAction(ButtonName = "Update")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Class clas)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }


            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                int branchId = UserPermissionController.GetLoginBranchId(Session.SessionID);
                var classObj = classRepo.GetClassByNameAndId(clas.Name, clas.Id, branchId);
                clas.BranchId = branchId;
                clas.Branch = null;
                clas.IsFinanceAccountOpen = true;
                //if (clas.IsFinanceAccountOpen)
                //    createFinanceAccount(clas.Name);
                if (ModelState.IsValid && classObj == null)
                {
                    classRepo.UpdateClass(clas);
                    errorCode = 2;
                    SessionHelper.InvalidateClassCache = false;
                    SessionHelper.InvalidateClassSectionCache = false;
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

        public ActionResult Delete(int id = 0)
        {
            if (UserPermissionController.CheckUserLoginStatus(Session.SessionID) == false)
            {
                return RedirectToAction("Index", "Login");
            }

            Class classsection = classRepo.GetClassById(id);
            try
            {
                LogWriter.WriteProcStartLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, UserPermissionController.GetUserId(Session.SessionID));
                if (classsection == null)
                {
                    return HttpNotFound();
                }
                int classCount = classSecRepo.GetClassCount(id);
                if (classCount == 0)
                {
                    classRepo.DeleteClass(classsection);
                    SessionHelper.InvalidateClassCache = false;
                    SessionHelper.InvalidateClassSectionCache = false;
                    errorCode = 4;
                }
                else
                    errorCode = 40;
                LogWriter.WriteProcEndLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex) { 
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
                errorCode = 3;
            }
            return RedirectToAction("Index", new { id = 0 });
        }


    }
}