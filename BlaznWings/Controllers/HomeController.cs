using BlaznWings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlaznWings.Controllers
{
	public class HomeController : Controller
	{
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public ActionResult AdminPortal()
		{
			return View();
		}

		[HttpPost]
		public ActionResult AdminPortal(AdminUser user)
		{
			AdminDB db = new AdminDB();

			AdminUser checkUser = db.Admins.Where(v => v.Username.Equals(user.Username)).SingleOrDefault();

			if(checkUser == null)
			{
				ViewBag.UserMessage = "That username does not exist.";

				return View();
			}
			else
			{
				if(user.Password.Equals(checkUser.Password))
				{


					return RedirectToAction("AdminLanding", "Home");
				}
				else
				{
					ViewBag.PasswordMessage = "That password is incorrect";

					return View();
				}
			}
		}

		[HttpGet]
		public ActionResult AdminLanding()
		{
			return View();
		}

	}
}