using r3mus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Security;

namespace r3mus.Repository
{
    public partial class SQLForumRepository
    {
        ApplicationDbContext forumDB = new ApplicationDbContext();
        List<string> UserRoles;

        public SQLForumRepository()
        {
            string userId = HttpContext.Current.User.Identity.GetUserId();
            UserRoles = forumDB.Roles.Where(role => role.Users.Select(user => user.UserId).Contains(userId)).Select(role => role.Name).ToList();
            //UserRoles = forumDB.Roles.Where(role => HttpContext.Current.User.IsInRole(role.Name)).Select(role => role.Name).ToList();
        }

        private bool UserAuthorisedForForum(int ForumId)
        {
            if(UserRoles.Contains("Admin") || UserRoles.Contains("CEO") || UserRoles.Contains("Director") || (forumDB.Forums.FirstOrDefault(f => f.Id == ForumId).MinimumRole.Equals("Guest")))
            {
                //return true;
                return ((!forumDB.Forums.FirstOrDefault(f => f.Id == ForumId).Deleted));
            }
            else
            {
                return ((UserRoles.Contains(forumDB.Forums.FirstOrDefault(f => f.Id == ForumId).MinimumRole) && (!forumDB.Forums.FirstOrDefault(f => f.Id == ForumId).Deleted)));
            }
        }

        public IQueryable<Forum> GetAllForums()
        {
            //return forumDB.Forums.Where(forum => UserAuthorisedForForum(forum.Id));
            var forums = new List<Forum>();

            forumDB.Forums.ToList().ForEach(forum => { 
                if((UserAuthorisedForForum(forum.Id)) && (!forum.Deleted))
                {
                    forums.Add(forum);
                }
            });
            return forums.AsQueryable();
        }

        public Forum GetForumByID(int ForumID)
        {
            if(UserAuthorisedForForum(ForumID))
            {
                var forum = forumDB.Forums.FirstOrDefault(f => f.Id == ForumID);
                return forum;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<Thread> GetAllThreads()
        {
            //return forumDB.Threads.Where(thread => UserAuthorisedForForum(thread.ForumID));
            var threads = new List<Thread>();

            forumDB.Threads.ToList().ForEach(thread =>
            {
                if (UserAuthorisedForForum(thread.ForumID))
                {
                    threads.Add(thread);
                }
            });
            return threads.AsQueryable();
        }

        public IQueryable<Thread> GetThreadsByForum(int ForumID)
        {
            //return (from thread in forumDB.Threads
            //        where thread.ForumID == ForumID
            //        select thread);
            //return forumDB.Threads.Where(thread => ((thread.ForumID == ForumID) && (UserAuthorisedForForum(thread.ForumID))));

            //var threads = forumDB.Threads.Where(thread => ((thread.ForumID == ForumID) && (UserAuthorisedForForum(thread.ForumID))));

            var threads = new List<Thread>();

            forumDB.Threads.ToList().ForEach(thread =>
            {
                if ((thread.ForumID == ForumID) && (UserAuthorisedForForum(thread.ForumID)))
                {
                    threads.Add(thread);
                }
            });

            return threads.AsQueryable();
        }

        public Thread GetThreadByID(int ThreadID)
        {
            //return forumDB.Threads.Single(t => t.Id == ThreadID);
            var thread = forumDB.Threads.Single(t => t.Id == ThreadID);

            if(UserAuthorisedForForum(thread.ForumID))
            {
                return thread;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<Post> GetAllPosts()
        {
            //return forumDB.Posts.Where(post => UserAuthorisedForForum(post.Thread.ForumID));

            var posts = new List<Post>();

            forumDB.Posts.ToList().ForEach(post =>
            {
                if (UserAuthorisedForForum(post.Thread.ForumID))
                {
                    posts.Add(post);
                }
            });
            return posts.AsQueryable();
        }

        public IQueryable<Post> GetPostsByThread(int ThreadID)
        {
            //return from post in forumDB.Posts
            //       where post.Id == ThreadID
            //       select post;

            //var posts = forumDB.Posts.Where(post => ((post.ThreadId == ThreadID) && (UserAuthorisedForForum(post.Thread.ForumID))));

            var posts = new List<Post>();

            forumDB.Posts.ToList().ForEach(post =>
            {
                if ((post.ThreadId == ThreadID) && (UserAuthorisedForForum(post.Thread.ForumID)))
                {
                    posts.Add(post);
                }
            });

            return posts.AsQueryable();
        }
        
        public Post GetPostByID(int PostID)
        {
            return forumDB.Posts.Single(p => p.Id == PostID);

            var post = forumDB.Posts.Single(p => p.Id == PostID);

            if(UserAuthorisedForForum(post.Thread.ForumID))
            {
                return post;
            }
            else
            {
                return null;
            }
        }

        public ApplicationUser GetUserByID(string UserID)
        {
            return forumDB.Users.Single(u => u.Id == UserID);
        }

        public void AddForum(Forum forum)
        {
            forumDB.Forums.Add(forum);
            forumDB.SaveChanges();
        }

        public void UpdateForum(Forum forum)
        {
            var tmpForum = forumDB.Forums.Single(f => f.Id == forum.Id);
            tmpForum.Title = forum.Title;
            forumDB.SaveChanges();
        }

        public void DeleteForum(Forum forum)
        {
            //Must enable Cascade Delete for threads and posts
            forumDB.Forums.Remove(forum);
            forumDB.SaveChanges();
        }

        public void AddThread(Thread thread)
        {
            thread.CreatorId = HttpContext.Current.User.Identity.GetUserId();
            forumDB.Threads.Add(thread);
            forumDB.SaveChanges();
        }

        public void UpdateThread(Thread thread)
        {
            var tmpThread = forumDB.Threads.Single(t => t.Id == thread.Id);
            tmpThread.Title = thread.Title;
            forumDB.SaveChanges();
        }

        public void DeleteThread(Thread thread)
        {
            forumDB.Threads.Remove(thread);    //Must enable Cascade Delete for posts
            forumDB.SaveChanges();
        }

        public void AddPost(Post post)
        {
            post.UserId = HttpContext.Current.User.Identity.GetUserId();
            post.PostedAt = DateTime.Now;
            forumDB.Posts.Add(post);
            forumDB.SaveChanges();
        }

        public void UpdatePost(Post post)
        {
            var tmpPost = forumDB.Posts.Single(p => p.Id == post.Id);
            tmpPost.Body = post.Body;
            tmpPost.Title = post.Title;
            forumDB.SaveChanges();
        }

        public void DeletePost(Post post)
        {
            forumDB.Posts.Remove(post);
            forumDB.SaveChanges();
        }

        public void Dispose()
        {
            if (forumDB != null)
            {
                forumDB.Dispose();
            }
        }
    }
}