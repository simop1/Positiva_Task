using Positiva_Task.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Positiva_Task.DataAccess;

namespace Positiva_Task.Controllers
{




	public class HomeController : Controller
    {
		private UsersContext db = new UsersContext();

		public ActionResult Index()
		{
			var firstName = db.Users.FirstOrDefault().FirstName;
			ViewBag.FirstName = firstName;
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
		public ActionResult EditOrAddUser(int UserID)
		{
			UserModel model = new UserModel();
			model.UserID = UserID;

			if(UserID > 0)
			{
				var userForEdit = db.Users.Find(UserID);

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
					User newUser = new User() {
						FirstName = model.FirstName,
						LastName = model.LastName,
						UserName = model.UserName,
						Email = model.Email,
						UserPassword = model.Password,
						DateOfBirth = model.DateOfBirth
					};

					db.Users.Add(newUser);
					db.SaveChanges();
					return Json(new { Error = false }, JsonRequestBehavior.AllowGet);
				}
			}

			return RedirectToAction("UserManagement");
			//return Json(new { Error = true }, JsonRequestBehavior.AllowGet);
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