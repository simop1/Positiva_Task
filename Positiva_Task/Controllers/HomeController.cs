using Positiva_Task.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Positiva_Task.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserManagement()
        {
            ViewBag.Message = "User management";

            return View();
        }

		[HttpPost]
		public ActionResult EditOrAddUser(int UserID)
		{
			UserModel model = new UserModel();
			model.UserID = UserID;
			return PartialView("EditOrAddUser", model);
		}

		[HttpPost]
		public ActionResult SaveUser(UserModel model)
		{
			if (ModelState.IsValid)
			{
				return Json(new { success = true});
			}

			return Json(false);
		}
	}
}