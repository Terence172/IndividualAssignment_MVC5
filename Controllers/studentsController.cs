using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using IndividualAssignment_MVC5.Models;

namespace IndividualAssignment_MVC5.Controllers
{
    public class studentsController : Controller
    {
        private db_individualTestEntities db = new db_individualTestEntities();

        // GET: lecturer
        public ActionResult Index()
        {
            return View(db.users.ToList());
        }

        // GET: lecturer
        public ActionResult Index2()
        {
            return View(db.users.ToList());
        }

        // GET: lecturer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: lecturer/Create
        public ActionResult Create()
        {
            var lecturerList = db.lecturers
               .Select(l => new SelectListItem
               {
                   Value = l.lect_id.ToString(),
                   Text = l.user.user_firstname + " " + l.user.user_lastname
               })
               .ToList();

            ViewBag.stu_sup_ID = new SelectList(lecturerList, "Value", "Text");
            ViewBag.prog_id = new SelectList(db.programmes, "prog_id", "prog_name");


            return View();
        }

        // POST: lecturer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "user_id,user_name,user_email,user_password,user_firstname,user_lastname")] user user, [Bind(Include = "stu_id,stu_faculty,stu_school,prog_id,stu_sup_ID,user_id")] student student)
        {
            if (ModelState.IsValid)
            {
                // Set the default user_type
                user.user_type = "Student";


                // Call the SendCredentials method of EmailController to send the credentials
                EmailController emailController = new EmailController();
                emailController.SendCredentialsStudent(user.user_name, user.user_password, user.user_email, user.user_firstname, user.user_lastname);


                // Encrypt the password
                string encryptedPassword = Encrypt(user.user_password);

                // Set the encrypted password as the user's password
                user.user_password = encryptedPassword;


                // Save the user record
                db.users.Add(user);
                db.SaveChanges();

                // Retrieve the generated user_id
                int userId = user.user_id;

                // Assign the user_id to the lecturer
                student.user_id = userId;
                student.stu_status_done_sup = 1;

                // Save the investor record
                db.students.Add(student);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.stu_sup_ID = new SelectList(db.lecturers, "lect_id", "lect_faculty", student.stu_sup_ID);
            ViewBag.prog_id = new SelectList(db.programmes, "prog_id", "prog_name", student.prog_id);

            return View(user);
        }



        // GET: Supervisor/Create
        public ActionResult Supervisor(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Retrieve the generated user_id
            int userId = user.user_id;

            // Retrieve the existing investor record based on the user_id
            student existingStudent = db.students.SingleOrDefault(i => i.user_id == user.user_id);

            var lecturerList = db.lecturers
               .Select(l => new SelectListItem
               {
                   Value = l.lect_id.ToString(),
                   Text = l.user.user_firstname + " " + l.user.user_lastname + " (" + l.user.user_name + ")"
               })
               .ToList();

            ViewBag.stu_sup_ID = new SelectList(lecturerList, "Value", "Text");

            return View(user);
        }



        // POST: lecturer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Supervisor([Bind(Include = "user_id")] user user, [Bind(Include = "stu_id,stu_sup_ID")] student student)
        {
            if (ModelState.IsValid)
            {


                // Retrieve the generated user_id
                int userId = user.user_id;

                // Assign the user_id to the investor
                student.user_id = userId;

                // Retrieve the existing investor record based on the user_id
                student existingStudent = db.students.SingleOrDefault(i => i.user_id == user.user_id);

                // Update the investor record
                if (existingStudent != null)
                {
                    // Update the investor record
                    existingStudent.stu_sup_ID = student.stu_sup_ID;
                    existingStudent.stu_status_done_sup = 1;

                    db.Entry(existingStudent).State = EntityState.Modified;
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(user);
        }







        // GET: Evaluater/Create
        public ActionResult Evaluator(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Retrieve the generated user_id
            int userId = user.user_id;

            // Retrieve the existing investor record based on the user_id
            student existingStudent = db.students.SingleOrDefault(i => i.user_id == user.user_id);

            var lecturerList = db.lecturers
                .Where(l => l.lect_id != existingStudent.lecturer2.lect_id)
               .Select(l => new SelectListItem
               {
                   Value = l.lect_id.ToString(),
                   Text = l.user.user_firstname + " " + l.user.user_lastname + " (" + l.user.user_name + ")"
               })
               .ToList();

            ViewBag.stu_evaluator1 = new SelectList(lecturerList, "Value", "Text");
            ViewBag.stu_evaluator2 = new SelectList(lecturerList, "Value", "Text");


            return View(user);
        }

        // POST: lecturer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Evaluator([Bind(Include = "user_id")] user user, [Bind(Include = "stu_id,stu_evaluator1,stu_evaluator2")] student student)
        {
            if (ModelState.IsValid)
            {
                

                // Retrieve the generated user_id
                int userId = user.user_id;

                // Assign the user_id to the investor
                student.user_id = userId;

                // Retrieve the existing investor record based on the user_id
                student existingStudent = db.students.SingleOrDefault(i => i.user_id == user.user_id);

                // Update the investor record
                if (existingStudent != null)
                {
                    // Update the investor record
                    existingStudent.stu_evaluator1 = student.stu_evaluator1;
                    existingStudent.stu_evaluator2 = student.stu_evaluator2;

                    db.Entry(existingStudent).State = EntityState.Modified;
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(user);
        }





        public static string Encrypt(string plainText)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            string encryptedPassword = Convert.ToBase64String(plainBytes);
            return encryptedPassword;
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            string decryptedPassword = Encoding.UTF8.GetString(encryptedBytes);
            return decryptedPassword;
        }

        // GET: lecturer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            user user = db.users.Include(u => u.students).SingleOrDefault(u => u.user_id == id);

            if (user == null)
            {
                return HttpNotFound();
            }

            string decryptedPassword = Decrypt(user.user_password);
            user.user_password = decryptedPassword;

            // Ensure there is at least one student associated with the user
            if (user.students.Count == 0)
            {
                // Create a new student and associate it with the user
                student student = new student();
                user.students.Add(student);
            }

            ViewBag.prog_id = new SelectList(db.programmes, "prog_id", "prog_name");

            return View(user);
        }

        // POST: lecturer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "user_id,user_name,user_email,user_password,user_firstname,user_lastname")] user user, [Bind(Include = "stu_id,stu_faculty,stu_school,prog_id,user_id")] student student)
        {
            if (ModelState.IsValid)
            {
                // Set the default user_type
                user.user_type = "Student";

                // Encrypt the password
                string encryptedPassword = Encrypt(user.user_password);

                // Set the encrypted password as the user's password
                user.user_password = encryptedPassword;

                // Update the user record
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                // Retrieve the generated user_id
                int userId = user.user_id;

                // Assign the user_id to the student
                student.user_id = userId;

                // Retrieve the existing student record based on the user_id
                student existingStudent = db.students.SingleOrDefault(i => i.user_id == user.user_id);

                // Update the student record
                if (existingStudent != null)
                {
                    // Update the student record
                    existingStudent.stu_faculty = student.stu_faculty;
                    existingStudent.stu_school = student.stu_school;
                    existingStudent.prog_id = student.prog_id;

                    db.Entry(existingStudent).State = EntityState.Modified;
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: lecturer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            user user = db.users.Include(u => u.lecturers).SingleOrDefault(u => u.user_id == id);

            if (user == null)
            {
                return HttpNotFound();
            }

            // Ensure there is at least one investor associated with the user
            if (user.students.Count == 0)
            {
                // Create a new student and associate it with the user
                student student = new student();
                user.students.Add(student);
            }

            return View(user);
        }





        // POST: lecturer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            user user = db.users.Find(id);

            // Retrieve the existing investor record based on the user_id
            student student = db.students.SingleOrDefault(i => i.user_id == user.user_id);

            db.students.Remove(student);

            db.users.Remove(user);

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
