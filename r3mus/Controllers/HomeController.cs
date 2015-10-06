using JKON.EveApi.Corporation.Models;
using JKON.EveWho;
using r3mus.Models;
using r3mus.ViewModels;
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
                var latestNewsItem = new ApplicationDbContext().LatestNewsItem.FirstOrDefault();

                if(TempData["Message"] != null)
                {
                    ViewBag.Message = TempData["Message"].ToString();
                }

                //ViewBag.NewsTitle = "Raised By Wolves lives!";
                //ViewBag.NewsDate = new DateTime(2015, 04, 17);
                //ViewBag.NewsArticle = "Yeah, we're here. Suck it up!";

                ViewBag.NewsTitle = latestNewsItem.Topic;
                ViewBag.NewsDate = latestNewsItem.Date;

                if (latestNewsItem.Post.Length > 196)
                {
                    ViewBag.NewsArticle = string.Concat(latestNewsItem.Post.Substring(0, 196), "...");
                }
                else
                {
                    ViewBag.NewsArticle = latestNewsItem.Post;
                }

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

        public ActionResult LatestNews()
        {
            var latestNewsItem = new ApplicationDbContext().LatestNewsItem.Where(newsItem => newsItem.Category == "External News").FirstOrDefault();
            return View(latestNewsItem);
        }

        [OutputCache(Duration = 3600)]
        public ActionResult Wolves()
        {
            var members = Api.GetCorpMembers(Convert.ToInt64(Properties.Settings.Default.CorpAPI), Properties.Settings.Default.VCode);
            var resortMembers = members.Where(member => member.Title.Contains("CEO")).ToList();
            members.Where(member => member.Title.Contains("Director") && !member.Title.Contains("CEO")).OrderBy(member => member.Title).OrderBy(member => member.MemberSince).ToList().ForEach(member => resortMembers.Add(member));
            members.Where(member => (member.Title != string.Empty && (!member.Title.Contains("CEO") && !member.Title.Contains("Director")))).OrderBy(member => member.Title).OrderBy(member => member.MemberSince).ToList().ForEach(member => resortMembers.Add(member));
            members.Where(member => member.Title == string.Empty).OrderBy(member => member.MemberSince).ToList().ForEach(member => resortMembers.Add(member));

            return View(new CorpMemberViewModel() { CorpMembers = resortMembers });
        }
    }
}