using Positiva_Task.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Positiva_Task.DataAccess;
using System.Security.Cryptography;
using System.Text;

namespace Positiva_Task.Controllers
{
    public class HomeController : Controller
    {
        private UsersContext db = new UsersContext();
        [HttpGet]
        public ActionResult Login()
        {
            LoginModel model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (model == null) return Json(new { Error = true });

            var user = db.Users.FirstOrDefault(x => x.UserName == model.UserName || x.Email == model.UserName);
            
            if(user == null) return Json(new { Error = true });

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

            return RedirectToAction("Index", "Home", user);
        }

        public ActionResult Index(User loggedUser)
		{
			ViewBag.FirstName = loggedUser.FirstName;
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
					DateOfBirth = user.DateOfBirth
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

				return PartialView("EditOrAddUser", model);
			}

			return PartialView("EditOrAddUser", model);
		}

		[HttpPost]
		public ActionResult SaveUser(UserModel model, int userID)
		{
			if (ModelState.IsValid)
			{
				if (model == null) return Json(new { Error = true }, JsonRequestBehavior.AllowGet);

				if (userID == -1)
				{
                    string hash = "";
                    using (MD5 md5Hash = MD5.Create())
                    {
                        hash = GetMd5Hash(md5Hash, model.Password);
                    }

                    if(hash == "") return Json(new { Error = false }, JsonRequestBehavior.AllowGet);

                    User newUser = new User() {
						FirstName = model.FirstName,
						LastName = model.LastName,
						UserName = model.UserName,
						Email = model.Email,
						UserPassword = hash,
						DateOfBirth = model.DateOfBirth
					};

                    db.Users.Add(newUser);
					db.SaveChanges();
					return Json(new { Error = false }, JsonRequestBehavior.AllowGet);
				}
			}

			return RedirectToAction("UserManagement", "Home");
			//return Json(new { Error = true }, JsonRequestBehavior.AllowGet);
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
				}

				return RedirectToAction("UserManagement");
			}

			return RedirectToAction("UserManagement");
		}

	}
}