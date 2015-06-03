using r3mus.Models;
using r3mus.Repository;
using r3mus.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace r3mus.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        SQLForumRepository repository = new SQLForumRepository();
        ApplicationDbContext db = new ApplicationDbContext();

        //
        // GET: /Forum/
        public ActionResult Index()
        {
            ViewBag.Message = "Forums";
            var forums = repository.GetAllForums();
            return View(forums);
        }

        [Authorize(Roles = "Admin, CEO, Director")]
        // GET: Forum/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.Message = "Forum detail";
            var forum = repository.GetForumByID(id);
            //ViewBag.Threads = repository.GetThreadsByForum(id).ToList();
            return View(forum);
        }

        public ActionResult ShowForum(int id)
        {
            var forum = repository.GetForumByID(id);
            var threads = repository.GetThreadsByForum(id).Where(thread => thread != null).ToList();
            threads.ForEach(thread => thread.Posts = repository.GetPostsByThread(thread.Id).Where(post => post != null).ToList());

            var forumVM = new ForumViewModel()
            {
                Forum = forum,
                RoleList = new SelectList(db.Roles.Select(role => role.Name).ToList<string>(), repository.GetForumByID(id).MinimumRole),
                CreatorName = db.Users.Where(user => user.Id == forum.CreatorId).FirstOrDefault().UserName,
                Threads = threads
            };

            ViewBag.Message = forum.Title;

            return View(forumVM);
        }

        [Authorize(Roles = "Admin, CEO, Director")]
        // GET: Forum/Create
        public ActionResult Create()
        {
            var forumVM = new ForumViewModel() { Forum = new Forum(), RoleList = new SelectList(db.Roles.Select(role => role.Name).ToList<string>(), "Corp Member") };
            return View(forumVM);
        }

        [Authorize(Roles = "Admin, CEO, Director")]
        // POST: Forum/Create
        [HttpPost]
        public ActionResult Create(ForumViewModel forumVM)
        {
            try
            {
                forumVM.Forum.CreatorId = User.Identity.GetUserId();
                forumVM.Forum.Created = DateTime.Now;
                forumVM.Forum.Deleted = false;
                repository.AddForum(forumVM.Forum);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(forumVM);
            }

        }

        [Authorize(Roles = "Admin, CEO, Director")]
        // GET: Forum/Edit/5
        public ActionResult Edit(int id)
        {
            var forum = repository.GetForumByID(id);

            var forumVM = new ForumViewModel() { 
                Forum = forum, 
                RoleList = new SelectList(db.Roles.Select(role => role.Name).ToList<string>(), repository.GetForumByID(id).MinimumRole), 
                CreatorName = db.Users.Where(user => user.Id == forum.CreatorId).FirstOrDefault().UserName };
            return View(forumVM);
        }

        [Authorize(Roles = "Admin, CEO, Director")]
        // POST: Forum/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Forum forum)
        {
            try
            {
                repository.UpdateForum(forum);
                return RedirectToAction("AdminIndex");
            }
            catch
            {
                return View(forum);
            }

        }

        [Authorize(Roles = "Admin, CEO, Director")]
        // GET: Forum/Delete/5
        public ActionResult Delete(int id)
        {
            var forum = repository.GetForumByID(id);
            return View(forum);
        }

        [Authorize(Roles = "Admin, CEO, Director")]
        // POST: Forum/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var forum = repository.GetForumByID(id);
            try
            {
                //repository.DeleteForum(forum);
                forum.Deleted = true;
                repository.UpdateForum(forum);
                return RedirectToAction("AdminIndex");
            }
            catch
            {
                return View();
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (repository != null)
            {
                repository.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = "Admin, CEO, Director")]
        public ActionResult AdminIndex()
        {
            ViewBag.Message = "Forum Administration";
            var forums = repository.GetAllForums().ToList();
            var forumVMs = new List<ForumViewModel>();

            forums.ForEach(forum => forumVMs.Add(new ForumViewModel() { Forum = forum, CreatorName = db.Users.Where(user => user.Id == forum.CreatorId).FirstOrDefault().UserName }));

            return View(forumVMs);
        }
	}
}