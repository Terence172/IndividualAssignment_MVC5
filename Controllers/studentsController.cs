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
    public class studentsController : Controller
    {
        private db_individualTestEntities db = new db_individualTestEntities();

        // GET: students
        public ActionResult Index()
        {
            var students = db.students.Include(s => s.lecturer).Include(s => s.lecturer1).Include(s => s.lecturer2).Include(s => s.lecturer3).Include(s => s.programme).Include(s => s.user);
            return View(students.ToList());
        }

        // GET: students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            student student = db.students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: students/Create
        public ActionResult Create()
        {
            ViewBag.stu_evaluator1 = new SelectList(db.lecturers, "lect_id", "lect_faculty");
            ViewBag.stu_evaluator2 = new SelectList(db.lecturers, "lect_id", "lect_faculty");
            ViewBag.stu_sup_ID = new SelectList(db.lecturers, "lect_id", "lect_faculty");
            ViewBag.stu_pref_sup_ID = new SelectList(db.lecturers, "lect_id", "lect_faculty");
            ViewBag.prog_id = new SelectList(db.programmes, "prog_id", "prog_name");
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_firstname");
            return View();
        }

        // POST: students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "stu_id,stu_faculty,stu_school,prog_id,stu_sup_ID,stu_pref_sup_ID,stu_status_done_sup,stu_sup_agreement,user_id,stu_evaluator1,stu_evaluator2")] student student)
        {
            if (ModelState.IsValid)
            {
                db.students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.stu_evaluator1 = new SelectList(db.lecturers, "lect_id", "lect_faculty", student.stu_evaluator1);
            ViewBag.stu_evaluator2 = new SelectList(db.lecturers, "lect_id", "lect_faculty", student.stu_evaluator2);
            ViewBag.stu_sup_ID = new SelectList(db.lecturers, "lect_id", "lect_faculty", student.stu_sup_ID);
            ViewBag.stu_pref_sup_ID = new SelectList(db.lecturers, "lect_id", "lect_faculty", student.stu_pref_sup_ID);
            ViewBag.prog_id = new SelectList(db.programmes, "prog_id", "prog_name", student.prog_id);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", student.user_id);
            return View(student);
        }

        // GET: students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            student student = db.students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.stu_evaluator1 = new SelectList(db.lecturers, "lect_id", "lect_faculty", student.stu_evaluator1);
            ViewBag.stu_evaluator2 = new SelectList(db.lecturers, "lect_id", "lect_faculty", student.stu_evaluator2);
            ViewBag.stu_sup_ID = new SelectList(db.lecturers, "lect_id", "lect_faculty", student.stu_sup_ID);
            ViewBag.stu_pref_sup_ID = new SelectList(db.lecturers, "lect_id", "lect_faculty", student.stu_pref_sup_ID);
            ViewBag.prog_id = new SelectList(db.programmes, "prog_id", "prog_name", student.prog_id);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", student.user_id);
            return View(student);
        }

        // POST: students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "stu_id,stu_faculty,stu_school,prog_id,stu_sup_ID,stu_pref_sup_ID,stu_status_done_sup,stu_sup_agreement,user_id,stu_evaluator1,stu_evaluator2")] student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.stu_evaluator1 = new SelectList(db.lecturers, "lect_id", "lect_faculty", student.stu_evaluator1);
            ViewBag.stu_evaluator2 = new SelectList(db.lecturers, "lect_id", "lect_faculty", student.stu_evaluator2);
            ViewBag.stu_sup_ID = new SelectList(db.lecturers, "lect_id", "lect_faculty", student.stu_sup_ID);
            ViewBag.stu_pref_sup_ID = new SelectList(db.lecturers, "lect_id", "lect_faculty", student.stu_pref_sup_ID);
            ViewBag.prog_id = new SelectList(db.programmes, "prog_id", "prog_name", student.prog_id);
            ViewBag.user_id = new SelectList(db.users, "user_id", "user_name", student.user_id);
            return View(student);
        }

        // GET: students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            student student = db.students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            student student = db.students.Find(id);
            db.students.Remove(student);
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
