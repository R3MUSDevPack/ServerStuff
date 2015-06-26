using r3mus.Models;
using r3mus.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace r3mus.Controllers
{
    public class PostController : Controller
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
        // GET: /Post/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ViewBag.Message = "All Posts";
            var posts = repository.GetAllPosts();
            return View(posts);
        }

        //
        // GET: /Post/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.Message = "Post detail";
            var post = repository.GetPostByID(id);
            return View(post);
        }

        //
        // GET: /Post/Create

        public ActionResult Create(int? id)
        {
            ViewBag.Threads = repository.GetAllThreads();
            Post post = new Post();
            if (id.HasValue) post.ThreadId = id.Value;
            return View(post);
        } 

        //
        // POST: /Post/Create
        [HttpPost]
        public ActionResult Create(Post post)
        {
            try
            {
                // TODO: Add insert logic here
                repository.AddPost(post);
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Threads = repository.GetAllThreads();
                return View(post);
            }
        }
        
        //
        // GET: /Post/Edit/5
        public ActionResult Edit(int id)
        {
            var post = repository.GetPostByID(id);
            return View(post);
        }

        //
        // POST: /Post/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Post post)
        {
            // TODO: Add update logic here
            try
            {
                repository.UpdatePost(post);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(post);
            }
        }

        //
        // GET: /Post/Delete/5
        public ActionResult Delete(int id)
        {
            var post = repository.GetPostByID(id);
            return View(post);
        }

        //
        // POST: /Post/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var post = repository.GetPostByID(id);
            try
            {
                repository.DeletePost(post);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}