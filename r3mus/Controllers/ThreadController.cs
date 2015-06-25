using r3mus.Models;
using r3mus.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace r3mus.Controllers
{
    public class ThreadController : Controller
    {
        SQLForumRepository repository = new SQLForumRepository();
        ApplicationDbContext db = new ApplicationDbContext();

        protected override void Dispose(bool disposing)
        {
            if (repository != null)
            {
                repository.Dispose();
            }
            base.Dispose(disposing);
        }
        //
        // GET: /Thread/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ViewBag.Message = "All Threads";
            var threads = repository.GetAllThreads();
            return View(threads);
        }

        //
        // GET: /Thread/Details/5

        public ActionResult Details(int id)
        {
            ViewBag.Message = "Thread detail";
            var thread = repository.GetThreadByID(id);
            ViewBag.Posts = repository.GetPostsByThread(id).ToList();
            return View(thread);
        }

        //
        // GET: /Thread/Create

        public ActionResult Create(int forumId)
        {
            ViewBag.Forums = repository.GetAllForums();
            Thread thread = new Thread();
            thread.ForumID = forumId;
            thread.Posts = new List<Post>();
            thread.Posts.Add(new Post());
            return View(thread);
        }

        //
        // POST: /Thread/Create

        [HttpPost]
        public ActionResult Create(Thread thread)
        {
            try
            {
                thread.User = db.Users.Where(user => user.Id == User.Identity.GetUserId()).FirstOrDefault();
                thread.CreatorId = User.Identity.GetUserId();
                thread.Created = DateTime.Now;
                thread.Posts.FirstOrDefault().User = thread.User;
                thread.Posts.FirstOrDefault().PostedAt = thread.Created;

                repository.AddThread(thread);
                repository.AddPost(thread.Posts.FirstOrDefault());

                return RedirectToAction("Index");
            }
            catch
            {
                return View(thread);
            }
        }

        //
        // GET: /Thread/Edit/5
        public ActionResult Edit(int id)
        {
            var thread = repository.GetThreadByID(id);
            return View(thread);
        }

        //
        // POST: /Thread/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Thread thread)
        {
            // TODO: Add update logic here
            try
            {
                repository.UpdateThread(thread);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(thread);
            }
        }

        //
        // GET: /Thread/Delete/5
        [Authorize(Roles = "Admin, CEO, Director")]
        public ActionResult Delete(int id)
        {
            var thread = repository.GetThreadByID(id);
            return View(thread);
        }

        //
        // POST: /Thread/Delete/5
        [Authorize(Roles = "Admin, CEO, Director")]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var thread = repository.GetThreadByID(id);
            try
            {
                repository.DeleteThread(thread);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
	}
}