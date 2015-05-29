using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace r3mus.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if ((User == null) || (User.Identity.IsAuthenticated == false))
            {
                if(TempData["Message"] != null)
                {
                    ViewBag.Message = TempData["Message"].ToString();
                }

                ViewBag.NewsTitle = "Raised By Wolves lives!";
                ViewBag.NewsDate = new DateTime(2015, 04, 17);
                ViewBag.NewsArticle = "Yeah, we're here. Suck it up!";

                string AboutUs = System.IO.File.ReadAllText(Server.MapPath(@"~/App_Data/AboutUs.txt"));
                if (AboutUs.Length > 196)
                {
                    ViewBag.AboutUs = string.Concat(AboutUs.Substring(0, 196), "...");
                }
                else
                {
                    ViewBag.AboutUs = AboutUs;
                }

                return View();
            }
            else
            {
                return RedirectToAction("Index", "LoggedInHome");
            }
        }

        public ActionResult About()
        {
            string[] AboutUs = System.IO.File.ReadAllText(Server.MapPath(@"~/App_Data/AboutUs.txt")).Split(new string[]{Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            ViewBag.Messages = AboutUs;
            //ViewBag.Message = System.IO.File.ReadAllText(Server.MapPath(@"~/App_Data/AboutUs.txt"));

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "You can contact us with any questions by the following methods:";

            return View();
        }
    }
}