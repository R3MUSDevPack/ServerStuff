using r3mus.Models;
using r3mus.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace r3mus.Controllers
{
    [Authorize(Roles = "Admin, Director, CEO")]
    public class WebsiteAdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //
        // GET: /WebsiteAdmin/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewUsers(r3mus.Models.ApplicationUser.IDType memberType = r3mus.Models.ApplicationUser.IDType.Corporation)
        {
            var users = db.Users.Where(user => user.MemberType == memberType.ToString()).ToList<ApplicationUser>();
            var userModels = new List<UserProfileViewModel>();

            users.ForEach(user => userModels.Add(new UserProfileViewModel() { 
                                                                                Id = user.Id, 
                                                                                UserName = user.UserName, 
                                                                                EmailAddress = user.EmailAddress,
                                                                                MemberSince = Convert.ToDateTime(user.MemberSince),
                                                                                MemberType = user.MemberType,
                                                                                Titles = string.Join(", ", db.Titles.Where(title => title.UserId == user.Id).Select(title => title.TitleName).ToList()),
                                                                                WebsiteRoles = string.Join(", ", user.Roles.Select(role => role.Role.Name).ToList()),
                                                                                Avatar = user.Avatar
            }));

            return View(userModels);
        }

        //public ActionResult ViewUsers(r3mus.Models.ApplicationUser.IDType memberType = r3mus.Models.ApplicationUser.IDType.Corporation)
        //{
        //    var users = db.Users;
        //    var userModels = new List<UserProfileViewModel>();

        //    foreach (ApplicationUser user in users)
        //    {
        //        if (user.MemberType == memberType.ToString())
        //        {
        //            UserProfileViewModel newModel = new UserProfileViewModel();
        //            newModel.Id = user.Id;
        //            newModel.UserName = user.UserName;
        //            newModel.EmailAddress = user.EmailAddress;
        //            newModel.MemberSince = Convert.ToDateTime(user.MemberSince);
        //            newModel.MemberType = user.MemberType;
        //            newModel.Avatar = user.Avatar;

        //            foreach (Title title in user.Titles)
        //            {
        //                newModel.Titles = string.Concat(newModel.Titles, title.TitleName, ", ");
        //            }
        //            if (newModel.Titles.Length > 0)
        //            {
        //                newModel.Titles = newModel.Titles.Trim().Substring(0, (newModel.Titles.Trim().Length - 1));
        //            }

        //            foreach (IdentityUserRole role in user.Roles)
        //            {
        //                newModel.WebsiteRoles = string.Concat(newModel.WebsiteRoles, role.Role.Name, ", ");
        //            }
        //            if (newModel.WebsiteRoles.Length > 0)
        //            {
        //                newModel.WebsiteRoles = newModel.WebsiteRoles.Trim().Substring(0, (newModel.WebsiteRoles.Trim().Length - 1));
        //            }
        //            userModels.Add(newModel);
        //        }
        //    }

        //    return View(userModels);
        //}

        [Authorize]
        public ActionResult ViewProfile(string id = "")
        {
            ApplicationUser currentUser;

            if (id == string.Empty)
            {
                id = User.Identity.GetUserId();
            }

            currentUser = db.Users.Where(user => user.Id == id).FirstOrDefault();
            currentUser.LoadApiKeys();
            currentUser.Titles = db.Titles.Where(title => title.UserId == id).ToList();
                
            if(currentUser.Titles.Count == 0)
            {
                currentUser.Titles.Add(new Title(){ UserId = currentUser.Id, TitleName = "Corp Member"});
            }

            var userProfile = new UserProfileViewModel() { 
                Id = currentUser.Id,
                UserName = currentUser.UserName, EmailAddress = currentUser.EmailAddress, MemberSince = Convert.ToDateTime(currentUser.MemberSince), 
                MemberType = currentUser.MemberType, Avatar = currentUser.Avatar, Titles = string.Join(", ", currentUser.Titles.Select(t => t.TitleName).ToList()),
                WebsiteRoles = string.Join(", ", currentUser.Roles.Select(role => role.Role.Name).ToList())
            };

            return View(userProfile);
        }

        public ActionResult UpdateApiDetails(string originator, string id = "")
        {
            ApplicationUser currentUser = new ApplicationUser();

            if (id == string.Empty)
            {
                id = User.Identity.GetUserId();
            }

            currentUser = db.Users.Where(user => user.Id == id).FirstOrDefault();
            var cUsrTitles = currentUser.Titles;

            currentUser.GetDetails(true);

            db.Titles.Where(dbTitle => dbTitle.UserId == id).ToList().Where(dbTitle => cUsrTitles.Any(cUsrTitle => dbTitle.TitleName == cUsrTitle.TitleName)).ToList().ForEach(dbTitle => db.Entry(dbTitle).State = EntityState.Unchanged);
            db.Titles.Where(dbTitle => dbTitle.UserId == id).ToList().Where(dbTitle => !cUsrTitles.Any(cUsrTitle => dbTitle.TitleName == cUsrTitle.TitleName)).ToList().ForEach(dbTitle => db.Entry(dbTitle).State = EntityState.Deleted);

            db.SaveChanges();

            return RedirectToAction(originator, new { id = id });
        }
	}
}