using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;

namespace SMS_Web.Controllers.StaffHandling
{
    public class StaffAttendancePolicyController : Controller
    {
        private SC_WEBEntities2 db = new SC_WEBEntities2();
        private static int errorCode = 0;

        //
        // GET: /StaffAttendancePolicy/

        public ActionResult Index(int id = 0)
        {
            if (id > 0)
                errorCode = 0;
            ViewData["Operation"] = id;
            ViewData["attendancePolicy"] = db.StaffAttandancePolicies.ToList();
            ViewData["Error"] = errorCode;
            errorCode = 0;
            ViewBag.CatagoryId = new SelectList(db.DesignationCatagories, "Id", "CatagoryName");
            ViewData["catagory"] = db.DesignationCatagories.ToList();
            ViewData["designation"] = db.Designations.ToList();

            StaffAttandancePolicy catagory = db.StaffAttandancePolicies.Find(id);
            return View(catagory);
        }

        //
        // GET: /StaffAttendancePolicy/Details/5

        public ActionResult Details(int id = 0)
        {
            StaffAttandancePolicy staffattandancepolicy = db.StaffAttandancePolicies.Find(id);
            if (staffattandancepolicy == null)
            {
                return HttpNotFound();
            }
            return View(staffattandancepolicy);
        }

        //
        // GET: /StaffAttendancePolicy/Create

        public ActionResult Create()
        {
            ViewBag.DesignationId = new SelectList(db.Designations, "Id", "Name");
            return View();
        }

        //
        // POST: /StaffAttendancePolicy/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StaffAttandancePolicy staffattandancepolicy)
        {
            if (ModelState.IsValid)
            {
                db.StaffAttandancePolicies.Add(staffattandancepolicy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DesignationId = new SelectList(db.Designations, "Id", "Name", staffattandancepolicy.DesignationId);
            return View(staffattandancepolicy);
        }

        //
        // GET: /StaffAttendancePolicy/Edit/5

        public ActionResult Edit(int id = 0)
        {
            StaffAttandancePolicy staffattandancepolicy = db.StaffAttandancePolicies.Find(id);
            if (staffattandancepolicy == null)
            {
                return HttpNotFound();
            }
            ViewBag.DesignationId = new SelectList(db.Designations, "Id", "Name", staffattandancepolicy.DesignationId);
            return View(staffattandancepolicy);
        }

        //
        // POST: /StaffAttendancePolicy/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StaffAttandancePolicy staffattandancepolicy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(staffattandancepolicy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DesignationId = new SelectList(db.Designations, "Id", "Name", staffattandancepolicy.DesignationId);
            return View(staffattandancepolicy);
        }

        //
        // GET: /StaffAttendancePolicy/Delete/5

        public ActionResult Delete(int id = 0)
        {
            StaffAttandancePolicy staffattandancepolicy = db.StaffAttandancePolicies.Find(id);
            if (staffattandancepolicy == null)
            {
                return HttpNotFound();
            }
            return View(staffattandancepolicy);
        }

        //
        // POST: /StaffAttendancePolicy/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StaffAttandancePolicy staffattandancepolicy = db.StaffAttandancePolicies.Find(id);
            db.StaffAttandancePolicies.Remove(staffattandancepolicy);
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