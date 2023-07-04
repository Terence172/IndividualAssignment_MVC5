using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IndividualAssignment_MVC5.Models;

namespace IndividualAssignment_MVC5.Controllers
{
    public class comemsController : Controller
    {
        private db_individualTestEntities db = new db_individualTestEntities();

        // GET: comems
        public ActionResult Index()
        {
            var comems = db.comems.Include(c => c.lecturer).Include(c => c.programme);
            return View(comems.ToList());
        }

        // GET: comems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comem comem = db.comems.Find(id);
            if (comem == null)
            {
                return HttpNotFound();
            }
            return View(comem);
        }

        // GET: comems/Create
        public ActionResult Create()
        {
            var existingMembers = db.comems.Select(c => c.lect_id).ToList();

            var lecturers = db.lecturers
                .Include(l => l.user)
                .Where(l => !existingMembers.Contains(l.lect_id))
                .ToList();

            var lecturerList = lecturers.Select(l => new SelectListItem
            {
                Value = l.lect_id.ToString(),
                Text = l.user.user_firstname + " " + l.user.user_lastname + "  - Username: " + l.user.user_name
            }).ToList();

            ViewBag.lect_id = lecturerList;
            ViewBag.prog_id = new SelectList(db.programmes, "prog_id", "prog_name");

            return View();
        }


        // POST: comems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "comem_id,prog_id,lect_id")] comem comem)
        {
            if (ModelState.IsValid)
            {
                db.comems.Add(comem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.lect_id = new SelectList(db.lecturers, "lect_id", "lect_faculty", comem.lect_id);
            ViewBag.prog_id = new SelectList(db.programmes, "prog_id", "prog_name", comem.prog_id);
            return View(comem);
        }



        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            comem comem = db.comems.Find(id);
            if (comem == null)
            {
                return HttpNotFound();
            }

            var existingMembers = db.comems.Where(c => c.comem_id != comem.comem_id).Select(c => c.lect_id).ToList();

            var lecturerList = db.lecturers
                .ToList()
                .Where(l => !existingMembers.Contains(l.lect_id))
                .Select(l => new SelectListItem
                {
                    Value = l.lect_id.ToString(),
                    Text = $"{l.user.user_firstname} {l.user.user_lastname} - Username: {l.user.user_name}"
                })
                .ToList();


            ViewBag.lect_id = new SelectList(lecturerList, "Value", "Text", comem.lect_id);
            ViewBag.prog_id = new SelectList(db.programmes, "prog_id", "prog_name", comem.prog_id);

            return View(comem);
        }

        // POST: comems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "comem_id,prog_id,lect_id")] comem comem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var existingMembers = db.comems.Where(c => c.comem_id != comem.comem_id).Select(c => c.lect_id).ToList();

            var lecturerList = db.lecturers
                .ToList()
                .Where(l => !existingMembers.Contains(l.lect_id))
                .Select(l => new SelectListItem
                {
                    Value = l.lect_id.ToString(),
                    Text = $"{l.user.user_firstname} {l.user.user_lastname} - Username: {l.user.user_name}"
                })
                .ToList();


            ViewBag.lect_id = new SelectList(lecturerList, "Value", "Text", comem.lect_id);
            ViewBag.prog_id = new SelectList(db.programmes, "prog_id", "prog_name", comem.prog_id);

            return View(comem);
        }





        // GET: comems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comem comem = db.comems.Find(id);
            if (comem == null)
            {
                return HttpNotFound();
            }
            return View(comem);
        }

        // POST: comems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            comem comem = db.comems.Find(id);
            db.comems.Remove(comem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
