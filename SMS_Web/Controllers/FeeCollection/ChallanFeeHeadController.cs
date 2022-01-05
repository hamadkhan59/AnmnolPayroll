using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS_DAL;

namespace SMS_Web.Controllers.FeeCollection
{
    public class ChallanFeeHeadController : Controller
    {
        private SC_WEBEntities2 db = new SC_WEBEntities2();

        private static int errorCode = 0;

        //
        // GET: /ChallanFeeHead/

        public ActionResult Index(int id = 0)
        {
            if (id > 0)
                errorCode = 0;
            ViewData["Operation"] = id;
            ViewData["challanFeeHead"] = db.ChallanToFeeHeads.ToList();
            ViewData["Error"] = errorCode;
            Challan head = db.Challans.Find(id);
            return View(head);
        }

        //
        // GET: /ChallanFeeHead/Details/5

        public ActionResult Details(int id = 0)
        {
            ChallanToFeeHead challantofeehead = db.ChallanToFeeHeads.Find(id);
            if (challantofeehead == null)
            {
                return HttpNotFound();
            }
            return View(challantofeehead);
        }

        //
        // GET: /ChallanFeeHead/Create

        public ActionResult Create()
        {
            ViewBag.ChallanId = new SelectList(db.Challans, "Id", "Name");
            ViewBag.HeadId = new SelectList(db.FeeHeads, "Id", "Name");
            return View();
        }

        //
        // POST: /ChallanFeeHead/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChallanToFeeHead challantofeehead)
        {
            if (ModelState.IsValid)
            {
                db.ChallanToFeeHeads.Add(challantofeehead);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ChallanId = new SelectList(db.Challans, "Id", "Name", challantofeehead.ChallanId);
            ViewBag.HeadId = new SelectList(db.FeeHeads, "Id", "Name", challantofeehead.HeadId);
            return View(challantofeehead);
        }

        //
        // GET: /ChallanFeeHead/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ChallanToFeeHead challantofeehead = db.ChallanToFeeHeads.Find(id);
            if (challantofeehead == null)
            {
                return HttpNotFound();
            }
            ViewBag.ChallanId = new SelectList(db.Challans, "Id", "Name", challantofeehead.ChallanId);
            ViewBag.HeadId = new SelectList(db.FeeHeads, "Id", "Name", challantofeehead.HeadId);
            return View(challantofeehead);
        }

        //
        // POST: /ChallanFeeHead/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChallanToFeeHead challantofeehead)
        {
            if (ModelState.IsValid)
            {
                db.Entry(challantofeehead).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ChallanId = new SelectList(db.Challans, "Id", "Name", challantofeehead.ChallanId);
            ViewBag.HeadId = new SelectList(db.FeeHeads, "Id", "Name", challantofeehead.HeadId);
            return View(challantofeehead);
        }

        //
        // GET: /ChallanFeeHead/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ChallanToFeeHead challantofeehead = db.ChallanToFeeHeads.Find(id);
            if (challantofeehead == null)
            {
                return HttpNotFound();
            }
            return View(challantofeehead);
        }

        //
        // POST: /ChallanFeeHead/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChallanToFeeHead challantofeehead = db.ChallanToFeeHeads.Find(id);
            db.ChallanToFeeHeads.Remove(challantofeehead);
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