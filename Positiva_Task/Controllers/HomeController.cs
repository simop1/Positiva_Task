using Positiva_Task.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Positiva_Task.DataAccess;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using System.Net.Mail;

namespace Positiva_Task.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private UsersContext db = new UsersContext();

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var user = db.Users.FirstOrDefault(x => x.Email == email);
            if(user == null)
                return Json(new { Error = true, Message = "Email not registered." }, JsonRequestBehavior.AllowGet);

            var link = "<a href='@Url.Action(\"ResetPassword\", \"Home\")' class='elements'>Click hear</a>";
            var body = "<span>" + link + " to reset your password.</span>";
            SendEmail(email, "Password reset", body);

            return RedirectToAction("Login", "Home");
        }

        private void SendEmail(string email, string subject, string body)
        {
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("noreply@test.com", "password");
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("noreply@test.com");
            msg.To.Add(new MailAddress(email));

            msg.Subject = subject;
            msg.IsBodyHtml = true;
            msg.Body = body;

            client.Send(msg);
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            ResetPasswordModel model = new ResetPasswordModel();
            model.Email = "test";
            return View(model);
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var user = db.Users.FirstOrDefault(x => x.Email == model.Email);
            string hash = "";
            using (MD5 md5Hash = MD5.Create())
            {
                hash = GetMd5Hash(md5Hash, model.Password);
            }

            user.UserPassword = hash;
            db.SaveChanges();

            return RedirectToAction("Login", "Home");
            //return Json(new { Message = "Password has been changed." });
        }

        [HttpPost]
        public ActionResult LogOut()
        {
            //FormsAuthentication.SignOut();
            Session.Abandon(); 
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            LoginModel model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (model == null) return Json(new { Error = true }, JsonRequestBehavior.AllowGet);

            var user = db.Users.FirstOrDefault(x => x.UserName == model.UserName || x.Email == model.UserName);
            
            if(user == null) return Json(new { Error = true }, JsonRequestBehavior.AllowGet);

            string inputHash = "";
            bool correctPassword;
            using (MD5 md5Hash = MD5.Create())
            {
                inputHash = GetMd5Hash(md5Hash, model.Password);

                if (VerifyMd5Hash(md5Hash, inputHash, user.UserPassword))
                {
                    correctPassword = true;
                }
                else
                {
                    correctPassword = false;
                }
            }

            if (!correctPassword) return Json(new { Error = true });

            Session["UserName"] = user.UserName;
            Session["Email"] = user.Email;
            Session["Role"] = user.Role.HasValue ? user.Role.Value : 0;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
		{
			ViewBag.FirstName = Session["UserName"];
			return View();
        }

		public ActionResult UserManagement()
        {
            List<UserModel> model = new List<UserModel>();
            var users = db.Users.ToList();

			foreach (var user in users)
			{
				model.Add(new UserModel
				{
					UserID = user.UserID,
					FirstName = user.FirstName,
					LastName = user.LastName,
					UserName = user.UserName,
					Email = user.Email,
					Password = user.UserPassword,
					DateOfBirth = user.DateOfBirth,
					Role = user.Role.HasValue ? user.Role.Value : 0
				});
			}

			ViewBag.Message = "User management";
            return View(model);
        }

        [HttpPost]
		public ActionResult EditOrAddUser(int userID)
		{
			UserModel model = new UserModel();
			model.UserID = userID;

			if(userID > 0)
			{
				var userForEdit = db.Users.Find(userID);

				model.UserID = userForEdit.UserID;
				model.FirstName = userForEdit.FirstName;
				model.LastName = userForEdit.LastName;
				model.UserName = userForEdit.UserName;
				model.Email = userForEdit.Email;
				model.DateOfBirth = userForEdit.DateOfBirth;
				model.Role = userForEdit.Role.HasValue ? userForEdit.Role.Value : 0;

				return PartialView("EditOrAddUser", model);
			}

			return PartialView("EditOrAddUser", model);
		}

		[HttpPost]
		public ActionResult SaveUser(UserModel model, int userID)
		{
			if (model == null) return Json(new { Error = true }, JsonRequestBehavior.AllowGet);

			if (userID == -1)
			{
				string hash = "";
				using (MD5 md5Hash = MD5.Create())
				{
					hash = GetMd5Hash(md5Hash, model.Password);
				}

				if (hash == "") return Json(new { Error = true, Message = "Error in adding user" }, JsonRequestBehavior.AllowGet);

				//check if username and email are in use
				var userNameTaken = db.Users.FirstOrDefault(x => x.UserName == model.UserName) != null;
				if (userNameTaken) return Json(new { Error = true, Message = "User name is taken" }, JsonRequestBehavior.AllowGet);

				var emailTaken = db.Users.FirstOrDefault(x => x.Email == model.Email) != null;
				if (emailTaken) return Json(new { Error = true, Message = "Email is in use" }, JsonRequestBehavior.AllowGet);

				User newUser = new User()
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					UserName = model.UserName,
					Email = model.Email,
					UserPassword = hash,
					DateOfBirth = model.DateOfBirth,
					Role = model.Role
				};

				db.Users.Add(newUser);
				db.SaveChanges();
				return Json(new { Error = false, Message = "User created" }, JsonRequestBehavior.AllowGet);
			}
			else if(userID > 0)
			{
				string hash = "";
				using (MD5 md5Hash = MD5.Create())
				{
					hash = GetMd5Hash(md5Hash, model.Password);
				}

				if (hash == "") return Json(new { Error = true, Message = "Error in updating user data" }, JsonRequestBehavior.AllowGet);

				var user = db.Users.Find(userID);
				user.FirstName = model.FirstName;
				user.LastName = model.LastName;
				user.UserName = model.UserName;
				user.Email = model.Email;
				user.UserPassword = hash;
				user.DateOfBirth = model.DateOfBirth;
				user.Role = model.Role.HasValue ? model.Role.Value : 0;
				db.SaveChanges();
				return Json(new { Error = false, Message = "User data updated" }, JsonRequestBehavior.AllowGet);
			}
			else
				return Json(new { Error = true, Message = "Error has happened" }, JsonRequestBehavior.AllowGet);
		}

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        static bool VerifyMd5Hash(MD5 md5Hash, string inputHash, string dbHash)
        {
            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(inputHash, dbHash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
		public ActionResult DeleteUser(int userID)
		{
			if(userID > 0)
			{
				var user = db.Users.Find(userID);
				if(user != null)
				{
					db.Users.Remove(user);
					db.SaveChanges();
				}
				return Json(new { Error = false }, JsonRequestBehavior.AllowGet);
			}
			else
			{
				return Json(new { Error = true }, JsonRequestBehavior.AllowGet);
			}

		}

		[HttpGet]
		public ActionResult InfoForAdmins()
		{
			ViewBag.isAdmin = (int)Session["Role"] == 1; ;
			return View();
		}

	}
}