using IndividualAssignment_MVC5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IndividualAssignment_MVC5.Controllers
{
    public class HomeController : Controller
    {
        private db_individualTestEntities db = new db_individualTestEntities();
        
        public ActionResult Index()
        {
            return View();
        }

        //Dashboard blah blah
        public ActionResult DashboardAdmin()
        {
            if (Session["UserID"] == null)
            {
                ViewBag.user = db.users.Count();
                return RedirectToAction("Index", "Login");
            }

            if (Session["UserType"].ToString() == "Lecturer")
            {
                return RedirectToAction("DashboardLecturer", "Home");
            }

            if (Session["UserType"].ToString() == "Student")
            {
                return RedirectToAction("DashboardStudent", "Home");
            }


            //ViewBag.tenantcount = db.tenants.Count();
            //ViewBag.landlords = db.landlords.Count();
            //ViewBag.room = db.rooms.Count();
            //ViewBag.availableRoom = db.rooms.Count(r => r.room_status == "Available");
            //ViewBag.bookedRoom = db.rooms.Count(r => r.room_status == "Booked");


            //var availableRooms = db.rooms.Where(r => r.room_status == "Available").ToList();

            // Pass the available rooms to the view
            return View();
        }

        public ActionResult DashboardLecturer()
        {
            if (Session["UserID"] == null)
            {
                ViewBag.user = db.users.Count();
                return RedirectToAction("Index", "Login");
            }

            if (Session["UserType"].ToString() == "Student")
            {
                return RedirectToAction("DashboardStudent", "Home");
            }

            if (Session["UserType"].ToString() == "Admin")
            {
                return RedirectToAction("DashboardAdmin", "Home");
            }

            return View();
        }

        public ActionResult DashboardStudent()
        {
            if (Session["UserID"] == null)
            {
                ViewBag.user = db.users.Count();
                return RedirectToAction("Index", "Login");
            }

            if (Session["UserType"].ToString() == "Lecturer")
            {
                return RedirectToAction("DashboardLecturer", "Home");
            }

            if (Session["UserType"].ToString() == "Admin")
            {
                return RedirectToAction("DashboardAdmin", "Home");
            }

            return View();
        }

    }
}