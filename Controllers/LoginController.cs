using IndividualAssignment_MVC5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace IndividualAssignment_MVC5.Controllers
{
    public class LoginController : Controller
    {
        private db_individualTestEntities db = new db_individualTestEntities();
        
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        //forgot password
        public ActionResult ForgotPassword()
        {
            return View();
        }


        // GET: User/Register
        public ActionResult Register()
        {
            var userList = db.users
                .Select(u => new SelectListItem
                {
                    Value = u.user_id.ToString(),
                    Text = u.user_firstname + " " + u.user_lastname
                })
                .ToList();

            ViewBag.stu_pref_sup_ID = new SelectList(db.lecturers, "lect_id", "lect_faculty");
            ViewBag.prog_id = new SelectList(db.programmes, "prog_id", "prog_name");

            return View();
        }


        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "user_id,user_name,user_email,user_password,user_firstname,user_lastname")] user user, [Bind(Include = "stu_id,stu_faculty,stu_school,prog_id,stu_pref_sup_ID,user_id")] student student)
        {
            if (ModelState.IsValid)
            {
                // Set the default user_type
                user.user_type = "Student";

                // Encrypt the password
                string encryptedPassword = Encrypt(user.user_password);

                // Set the encrypted password as the user's password
                user.user_password = encryptedPassword;

                // Save the user record
                db.users.Add(user);
                db.SaveChanges();

                // Retrieve the generated user_id
                int userId = user.user_id;

                // Assign the user_id to the student
                student.user_id = userId;

                // Save the student record
                db.students.Add(student);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.stu_pref_sup_ID = new SelectList(db.lecturers, "lect_id", "lect_faculty", student.stu_pref_sup_ID);
            ViewBag.prog_id = new SelectList(db.programmes, "prog_id", "prog_name", student.prog_id);

            return View(user);
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(user objchk)
        {
            if (ModelState.IsValid)
            {

                string encryptedPassword = Encrypt(objchk.user_password);
                Console.WriteLine("Encrypted Password: " + encryptedPassword);
                var obj = db.users.Where(a => a.user_name.Equals(objchk.user_name) && a.user_password.Equals(encryptedPassword)).FirstOrDefault();

                if (obj != null)
                    {
                        Session["UserID"] = obj.user_id;
                        Session["UserName"] = obj.user_name.ToString();
                        Session["FirstName"] = obj.user_firstname.ToString();
                        Session["LastName"] = obj.user_lastname.ToString();
                        Session["UserType"] = obj.user_type.ToString();

                        if (obj.user_type == "Admin")
                        {
                            return RedirectToAction("DashboardAdmin", "Home");
                        }

                        else if (obj.user_type == "Lecturer")
                        {
                            return RedirectToAction("DashboardLecturer", "Home");
                        }

                        else if (obj.user_type == "Student")
                        {
                            return RedirectToAction("DashboardStudent", "Home");
                        }

                        else if (obj.user_type == "Committee")
                        {
                            return RedirectToAction("DashboardCommittee", "Home");
                        }
                    /* 
                     Might wanna have more redirect for other user types  c4ca4238a0b923820dcc509a6f75849b
                     */
                }

                else
                    {
                        ModelState.AddModelError("", "The Username or Password Incorrect");
                    }
                

            }

            return View();
        }

        public static string Encrypt(string plainText)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            string encryptedPassword = Convert.ToBase64String(plainBytes);
            return encryptedPassword;
        }


        private List<SelectListItem> GetProgramList()
        {
            // Retrieve the program data from the database
            var programs = db.programmes.ToList();

            // Create a list of SelectListItem for the dropdown
            var programList = programs.Select(p => new SelectListItem
            {
                Value = p.prog_id.ToString(),
                Text = p.prog_name
            }).ToList();

            return programList;
        }

        private List<SelectListItem> GetSupervisorList()
        {
            // Retrieve the supervisor data from the database
            var supervisors = db.lecturers.ToList();

            // Create a list of SelectListItem for the dropdown
            var supervisorList = supervisors.Select(s => new SelectListItem
            {
                Value = s.lect_id.ToString(),
                Text = s.user.user_firstname + " " + s.user.user_lastname
            }).ToList();

            return supervisorList;
        }



        [HttpPost]
        public ActionResult ForgotPassword(string user_email)
        {
            //Verify Email ID
            //Generate Reset Password Link
            //Send email

            string message = "";


            var account = db.users.Where(u => u.user_email == user_email).FirstOrDefault();

            if (account != null)
            {
                //send email to reset password
                //generate unique identification number
                string resetCode = Guid.NewGuid().ToString();
                SendEmail(account.user_email, resetCode, "ResetPassword");//method
                account.u_forgotpwdCode = resetCode;

                //avid confirm password not match issue
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                message = "Reset password link has been sent to your email address";
            }
            else
            {
                message = "Account not found";
            }

            ViewBag.Message = message;
            return View();
        }


        [NonAction]
        public void SendEmail(string emailID, string activationCode, string emailFor = "ResetPassword")
        {
            var verifyUrl = "/Login/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("akmalrentalsalphazero@gmail.com", "Akmal Rentals");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "otubezrpgcnrvxnl"; // Replace with actual password

            string subject = "";
            string body = "";

            string logoImageUrl = "https://i.imgur.com/j3xnJzi.png";
            string backgroundImageUrl = "https://i.imgur.com/4Q9bX9h.png";

            if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = $@"
                    
            <html lang='en' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:v='urn:schemas-microsoft-com:vml'>
                        <head>
                            <title></title>
                            <meta charset='utf-8'/>
                            <meta content='width=device-width, initial-scale=1.0' name='viewport'/>
                            <!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></xml><![endif]-->
                            <style>
                                * {{
                                    box-sizing: border-box;
                                }}

                                body {{
                                    margin: 0;
                                    padding: 0;
                                }}

                                a[x-apple-data-detectors] {{
                                    color: inherit !important;
                                    text-decoration: inherit !important;
                                }}

                                #MessageViewBody a {{
                                    color: inherit;
                                    text-decoration: none;
                                }}

                                p {{
                                    line-height: inherit
                                }}

                                @media (max-width:690px) {{
                                    .icons-inner {{
                                        text-align: center;
                                    }}

                                    .icons-inner td {{
                                        margin: 0 auto;
                                    }}

                                    .row-content {{
                                        width: 100% !important;
                                    }}

                                    .stack .column {{
                                        width: 100%;
                                        display: block;
                                    }}
                                }}
                            </style>
                        </head>
                        <body style='background: url({backgroundImageUrl}) center / cover; margin: 0; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none;'>
                            <table border='0' cellpadding='0' cellspacing='0' class='nl-container' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background: url({backgroundImageUrl}) center / cover;' width='100%'>
                                <tbody>
                                    <tr>
                                        <td>
                                            <table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-1' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>
                                                <tbody>
                                                    <tr>
                                                        <td>
                                                            <table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #ffffff; color: #000000; width: 670px;' width='670'>
                                                                <tbody>
                                                                    <tr>
                                                                        <td class='column' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px none #000000; border-bottom: 0px none #000000; background-color: transparent;' valign='top'>
                                                                            <table align='center' border='0' cellpadding='0' cellspacing='0' class='icons-inner' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>
                                                                                <tbody>
                                                                                    <tr>
                                                                                        <td align='middle' style='padding-bottom: 10px;' valign='middle'>
                                                                                            <img alt='Logo' class='icon' height='32' src='{logoImageUrl}' style='outline: none; text-decoration: none; -ms-interpolation-mode: bicubic; height: auto; border: none; display: block;' title='Logo' width='null'/>
                                                                                        </td>
                                                                                    </tr>
                                                                                </tbody>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-2' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>
                                                <tbody>
                                                    <tr>
                                                        <td>
                                                            <table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #ffffff; color: #000000; width: 670px;' width='670'>
                                                                <tbody>
                                                                    <tr>
                                                                        <td class='column' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px none #000000; border-bottom: 0px none #000000; background-color: transparent;' valign='top'>
                                                                            <table align='center' border='0' cellpadding='0' cellspacing='0' class='icons-inner' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; margin: 0 auto;' width='100%'>
                                                                                <tbody>
                                                                                    <tr>
                                                                                        <td style='padding: 20px; text-align: center;'>
                                                                                            <h3 style='text-align: center;'>Hello, We received a request to change your password</h3>
                                                                                            <p style='text-align: center;'>Click on the link below, to continue to reset your password</p>
                                                                                            
                                                                                            <p style='text-align: center;'>Link: {link}</p>
                                                                                            <br>
                                                                                            <p style='text-align: center;'>Follow us: https://www.facebook.com/AkmalBilikSewa</p>

                                                                                        </td>
                                                                                    </tr>
                                                                                </tbody>
                                                                            </table>

                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table align='center' border='0' cellpadding='0' cellspacing='0' class='row row-3' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>
                                                <tbody>
                                                    <tr>
                                                        <td>
                                                            <table align='center' border='0' cellpadding='0' cellspacing='0' class='row-content stack' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #ffffff; color: #000000; width: 670px;' width='670'>
                                                                <tbody>
                                                                    <tr>
                                                                        <td class='column' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px none #000000; background-color: transparent;' valign='top'>
                                                                            <table align='center' border='0' cellpadding='0' cellspacing='0' class='icons-inner' role='presentation' style='mso-table-lspace: 0pt; mso-table-rspace: 0pt;' width='100%'>
                                                                                <tbody>
                                                                                    <tr>
                                                                                        <td style='padding: 20px;'>
                                                                                            <p style='margin-bottom: 0;'>Regards,</p>
                                                                                            <p style='margin-top: 0;'>Akmal Rentals</p>
                                                                                        </td>
                                                                                    </tr>
                                                                                </tbody>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </body>
                    </html>";
            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }



        public ActionResult ResetPassword(string id)
        {
            //verify the reset password link
            //find account associated with this link
            //redirect user to reset password page


            var user = db.users.Where(u => u.u_forgotpwdCode == id).FirstOrDefault();
            if (user != null)
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {

                var user = db.users.Where(u => u.u_forgotpwdCode == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    var tempPass = model.NewPassword;

                    //hash
                    user.user_password = Encrypt(tempPass);

                    //user can update one time with each user password code
                    user.u_forgotpwdCode = "";
                    //avoid confirm password not match issue
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    TempData["success"] = "New password updated successfully";
                }
                else
                {
                    return HttpNotFound();
                }

            }
            else
            {
                TempData["fail"] = "Something invalid";
            }
            return View(model);
        }



        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }


    }
}