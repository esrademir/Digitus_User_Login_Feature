using Digitus_User_Login_Feature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Digitus_User_Login_Feature.Controllers
{
    public class LoginController : Controller
    {
        UserLoginModel db = new UserLoginModel();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Signin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signin(string userName, string password)
        {
            int count = db.Users.Count(x => x.UserName == userName && x.Password == password);
            if (count > 0)
            {
                User user = db.Users.Where(x => x.UserName == userName && x.Password == password).FirstOrDefault();
                Session["user"] = user;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.message = "Kullanıcı Bulunamadı.";
            }

            return View();
        }

        [HttpPost]
        public ActionResult SignUp(string userName, string password, string password2, string mail, string fullName)
        {

            User newUser = new User();
            UserActivationTime activationTime = new UserActivationTime();
            activationTime.date = DateTime.Now;
            activationTime.User = newUser;
            db.UserActivationTimes.Add(activationTime);

            if (password == password2)
            {
                try
                {
                    newUser.UserName = userName;
                    newUser.Password = password;
                    newUser.Mail = mail;
                    newUser.NameSurname = fullName;
                    newUser.Status = true;
                    newUser.Authority = Enums.AuthorityEnum.user;
                    newUser.AuthorityApproval = false;
                    newUser.RegistrationDate = DateTime.Now;

                    SmtpClient smtp = new SmtpClient();
                    smtp.UseDefaultCredentials = true;
                    string SendingAccount = "******";
                    string sendingPassword = "******";
                    smtp.Credentials = new NetworkCredential(SendingAccount, sendingPassword);
                    smtp.Port = 587;
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;

                    MailMessage VerificationCode = new MailMessage();
                    VerificationCode.IsBodyHtml = true;
                    VerificationCode.From = new MailAddress(SendingAccount);
                    VerificationCode.To.Add(mail);
                    VerificationCode.Subject = "Doğrulama Kodu";
                    newUser.VerificationCode = CreateVerificationCode();
                    VerificationCode.Body = CreateMailText(newUser, newUser.VerificationCode);

                    smtp.Send(VerificationCode);
                    db.Users.Add(newUser);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewBag.message = "Bir hata oluştu. Tekrar deneyiniz. " + e.Message;
                    return RedirectToAction("Signin", "Login");
                }
                return RedirectToAction("VerificationMailControl", "Login", new { id = newUser.ID });
            }

            ViewBag.message = "Lütfen şifre bilgilerinizi doğru girdiğinize emin olunuz.";
            return RedirectToAction("Signin", "Login");
        }

        public string CreateMailText(User user, string code)
        {
            StringBuilder mailText = new StringBuilder();
            mailText.Append("Sevgili " + user.NameSurname);
            mailText.Append(Environment.NewLine);
            mailText.Append("Sitemize başarılı bir şekilde kayıt işlemini tamamlayabilmen için aşağıdaki doğrulama kodunu kullanman gerekiyor.");
            mailText.Append(Environment.NewLine);
            mailText.Append(Environment.NewLine);
            mailText.Append("DOĞRULAMA KODU: " + code);
            mailText.Append(Environment.NewLine);
            mailText.Append(Environment.NewLine);
            mailText.Append("İyi günler dileriz...");

            return mailText.ToString();
        }

        public string CreateVerificationCode()
        {
            string code = "";
            Random rnd = new Random();
            string letters = "ABCDEFGHIJKLMNOPRSTUVYZ0123456789";
            for (int i = 0; i < 5; i++)
            {
                code += letters[rnd.Next(0, 33)];
            }

            return code;
        }

        [HttpGet]
        public ActionResult VerificationMailControl(int? id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult VerificationMailControl(string verifiyCode, int? id)
        {
            ViewBag.message = "";
            try
            {
                User newUser = db.Users.Find(id);
                if (newUser != null)
                {
                    if (newUser.VerificationCode == verifiyCode)
                    {
                        newUser.AuthorityApproval = true;
                        Session["user"] = newUser;

                        UserActivationTime ac_time = db.UserActivationTimes.Where(x => x.User.ID == newUser.ID).FirstOrDefault();
                        ac_time.time = DateTime.Now - ac_time.date;

                        db.SaveChanges();

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.message = "Kod eşleşmesi sağlanamadı.";
                    }
                }
                else
                {
                    ViewBag.message = "Kullanıcı bulunamadı.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.message = "Doğrulama kodları eşlenirken bir hata oluştu. Lütfen tekrar deneyiniz." + ex.Message;
            }

            return View();
        }

        public ActionResult SignOut()
        {
            Session["user"] = null;
            return RedirectToAction("Signin", "Login");
        }
    }
}