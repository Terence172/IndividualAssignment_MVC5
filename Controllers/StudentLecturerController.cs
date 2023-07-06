using IndividualAssignment_MVC5.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Data.Entity.Validation;

namespace IndividualAssignment_MVC5.Controllers
{
    public class StudentLecturerController : Controller
    {
        private db_individualTestEntities db = new db_individualTestEntities();



        // GET: StudentLecturer
        public ActionResult Index()
        {
            int id = int.Parse(Session["UserID"].ToString());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            user user = db.users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            // Retrieve the existing student record based on the user_id
            student existingStudent = db.students.SingleOrDefault(i => i.user_id == user.user_id);

            var lecturerList = db.lecturers
               .Select(l => new SelectListItem
               {
                   Value = l.lect_id.ToString(),
                   Text = l.user.user_firstname + " " + l.user.user_lastname + " (" + l.user.user_name + ")"
               })
               .ToList();

            // Set the selected value in the dropdown list
            var selectedSupervisor = existingStudent?.stu_pref_sup_ID; // Get the selected supervisor ID
            ViewBag.stu_pref_sup_ID = new SelectList(lecturerList, "Value", "Text", selectedSupervisor);

            return View(user);
        }


        public ActionResult GetPdfForm()
        {
            int id = int.Parse(Session["UserID"].ToString());


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            user user = db.users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            // Retrieve the existing student record based on the user_id
            student existingStudent = db.students.SingleOrDefault(i => i.user_id == user.user_id);

            string pdfFilePath = Server.MapPath("~/Content/assets/uploads/" + existingStudent.stu_sup_agreement);
            byte[] fileBytes = System.IO.File.ReadAllBytes(pdfFilePath);
            return File(fileBytes, "application/pdf");
        }

        public ActionResult GetPdfProposal()
        {
            int id = int.Parse(Session["UserID"].ToString());


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            user user = db.users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            // Retrieve the existing student record based on the user_id
            student existingStudent = db.students.SingleOrDefault(i => i.user_id == user.user_id);

            string pdfFilePath = Server.MapPath("~/Content/assets/uploads/" + existingStudent.proposals.First().prop_file);
            byte[] fileBytes = System.IO.File.ReadAllBytes(pdfFilePath);
            return File(fileBytes, "application/pdf");
        }



        public ActionResult GetPdfFormLecturer(int? id)
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

            // Retrieve the existing student record based on the user_id
            student existingStudent = db.students.SingleOrDefault(i => i.user_id == user.user_id);

            string pdfFilePath = Server.MapPath("~/Content/assets/uploads/" + existingStudent.stu_sup_agreement);
            byte[] fileBytes = System.IO.File.ReadAllBytes(pdfFilePath);
            return File(fileBytes, "application/pdf");
        }

        // POST: lecturer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePreferredSupervisor([Bind(Include = "user_id")] user user, [Bind(Include = "stu_id,stu_pref_sup_ID")] student student)
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
                    existingStudent.stu_pref_sup_ID = student.stu_pref_sup_ID;
                    db.Entry(existingStudent).State = EntityState.Modified;
                    db.SaveChanges();
                }
                

                return RedirectToAction("Index");
            }

            return View(user);
        }

        

        // POST: StudentLecturer/UploadConsentForm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadConsentForm(user user, HttpPostedFileBase stu_sup_agreement)
        {
            if (ModelState.IsValid && stu_sup_agreement != null && stu_sup_agreement.ContentLength > 0)
            {
                // Retrieve the existing student record based on the user_id
                student existingStudent = db.students.SingleOrDefault(i => i.user_id == user.user_id);

                // Delete the old consent form if it exists
                if (existingStudent != null && !string.IsNullOrEmpty(existingStudent.stu_sup_agreement))
                {
                    string oldFilePath = Path.Combine(Server.MapPath("~/Content/assets/uploads"), existingStudent.stu_sup_agreement);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save the uploaded consent form
                string fileName = Path.GetFileName(stu_sup_agreement.FileName);
                string filePath = Path.Combine(Server.MapPath("~/Content/assets/uploads"), fileName);
                stu_sup_agreement.SaveAs(filePath);

                // Update the student's consent form file name
                if (existingStudent != null)
                {
                    existingStudent.stu_sup_agreement = fileName;
                    db.Entry(existingStudent).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if(Session["UserType"].ToString() == "Lecturer")
                {
                    return RedirectToAction("LecturerIndex", "StudentLecturer");
                }
                else
                {
                    return RedirectToAction("Index", "StudentLecturer");
                }
               
            }

            return View(user);
        }


        // POST: StudentLecturer/UploadProposal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadProposal([Bind(Include = "stu_id")] student student, [Bind(Include = "prop_id,prop_title,prop_type,prop_file")] proposal proposal, HttpPostedFileBase prop_file)
        {
            if (ModelState.IsValid)
            {
                
                    // Retrieve the generated user_id
                    int stuId = student.stu_id;
                    proposal.stu_id = stuId;
                    proposal.prop_status = "Submitted";

                    // Save the uploaded consent form
                    string fileName = Path.GetFileName(prop_file.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/assets/uploads"), fileName);
                    prop_file.SaveAs(filePath);

                    proposal.prop_file = fileName;

                
                    // Save the user record
                    db.proposals.Add(proposal);
                    db.SaveChanges();

                    return RedirectToAction("ProposalStudent", "StudentLecturer");
                

            }

            return View(student);
        }




        public ActionResult LecturerIndex()
        {
            
                int id = int.Parse(Session["UserID"].ToString());

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                user lecturer = db.users.Find(id);

                if (lecturer == null || lecturer.user_type != "Lecturer")
                {
                    return HttpNotFound();
                }

                // Retrieve the lecturer's supervised students
                lecturer existingLecturer = db.lecturers.Include(l => l.students).SingleOrDefault(l => l.user_id == id);

                if (existingLecturer == null)
                {
                    return HttpNotFound();
                }

                return View(existingLecturer);
            

        }


        public ActionResult ProposalStudent()
        {
            int id = int.Parse(Session["UserID"].ToString());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            user user = db.users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            // Retrieve the existing student record based on the user_id
            student existingStudent = db.students.SingleOrDefault(i => i.user_id == user.user_id);

            return View(existingStudent);
        }




        // GET: lecturer/Details/5
        public ActionResult DetailsStudent(int? id)
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


        // GET: lecturer/Details/5
        public ActionResult ConsentForm(int? id)
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

    }
}