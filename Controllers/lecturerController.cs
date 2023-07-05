using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IndividualAssignment_MVC5.Models;

using System.Security.Cryptography;
using System.Text;

namespace IndividualAssignment_MVC5.Controllers
{
    public class lecturerController : Controller
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
            return View();
        }

        // POST: lecturer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "user_id,user_name,user_email,user_password,user_firstname,user_lastname")] user user, [Bind(Include = "lect_id,lect_faculty")] lecturer lecturer)
        {
            if (ModelState.IsValid)
            {
                // Set the default user_type
                user.user_type = "Lecturer";


                // Call the SendCredentials method of EmailController to send the credentials
                EmailController emailController = new EmailController();
                emailController.SendCredentials(user.user_name, user.user_password, user.user_email, lecturer.lect_faculty, user.user_firstname, user.user_lastname);


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
                lecturer.user_id = userId;

                // Save the investor record
                db.lecturers.Add(lecturer);
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

            user user = db.users.Include(u => u.lecturers).SingleOrDefault(u => u.user_id == id);

            if (user == null)
            {
                return HttpNotFound();
            }

            string decryptedPassword = Decrypt(user.user_password);
            user.user_password = decryptedPassword;

            // Ensure there is at least one investor associated with the user
            if (user.lecturers.Count == 0)
            {
                // Create a new investor and associate it with the user
                lecturer investor = new lecturer();
                user.lecturers.Add(investor);
            }

            return View(user);
        }

        // POST: lecturer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "user_id,user_name,user_email,user_password,user_firstname,user_lastname")] user user, [Bind(Include = "lect_id,lect_faculty")] lecturer lecturer)
        {
            if (ModelState.IsValid)
            {
                // Set the default user_type
                user.user_type = "Lecturer";

                // Encrypt the password
                string encryptedPassword = Encrypt(user.user_password);

                // Set the encrypted password as the user's password
                user.user_password = encryptedPassword;

                // Update the user record
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                // Retrieve the generated user_id
                int userId = user.user_id;

                // Assign the user_id to the investor
                lecturer.user_id = userId;

                // Retrieve the existing investor record based on the user_id
                lecturer existingInvestor = db.lecturers.SingleOrDefault(i => i.user_id == user.user_id);

                // Update the investor record
                if (existingInvestor != null)
                {
                    // Update the investor record
                    existingInvestor.lect_faculty = lecturer.lect_faculty;

                    db.Entry(existingInvestor).State = EntityState.Modified;
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
            if (user.lecturers.Count == 0)
            {
                // Create a new investor and associate it with the user
                lecturer investor = new lecturer();
                user.lecturers.Add(investor);
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
            lecturer existingInvestor = db.lecturers.SingleOrDefault(i => i.user_id == user.user_id);
            db.lecturers.Remove(existingInvestor);
            db.users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }





        // GET: lecturer/Domain/5
        public ActionResult Domain(int? id)
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
            if (user.lecturers.Count == 0)
            {
                // Create a new investor and associate it with the user
                lecturer investor = new lecturer();
                user.lecturers.Add(investor);
            }

            return View(user);
        }



        // POST: lecturer/Domain/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Domain([Bind(Include = "user_id")] user user, [Bind(Include = "lect_id,lect_domain")] lecturer lecturer)
        {
            if (ModelState.IsValid)
            {

                // Retrieve the generated user_id
                int userId = user.user_id;

                // Assign the user_id to the investor
                lecturer.user_id = userId;

                // Retrieve the existing investor record based on the user_id
                lecturer existingInvestor = db.lecturers.SingleOrDefault(i => i.user_id == user.user_id);

                // Update the investor record
                if (existingInvestor != null)
                {
                    // Update the investor record
                    existingInvestor.lect_domain = lecturer.lect_domain;

                    db.Entry(existingInvestor).State = EntityState.Modified;
                }
                db.SaveChanges();

                return RedirectToAction("Index2");
            }

            return View(user);
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
