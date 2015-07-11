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
using System.Threading.Tasks;

using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;

namespace r3mus.Controllers
{
    [Authorize(Roles = "Admin, Director, CEO")]
    public class WebsiteAdminController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;

        public WebsiteAdminController()
        {
            db = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager) { AllowOnlyAlphanumericUserNames = false };
        }

        //
        // GET: /WebsiteAdmin/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TestSlack()
        {
            RecruitmentController.SendMessage("Slack Test From Website");

            return RedirectToAction("ViewUsers");
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
                                                                                WebsiteRoles = string.Join(", ", user.Roles.Select(role => role.RoleId).ToList()),
                                                                                Avatar = user.Avatar
            }));
            var resortModels = userModels.Where(model => model.Titles.Contains("CEO")).ToList();
            userModels.Where(model => (model.Titles != string.Empty && !model.Titles.Contains("CEO"))).OrderBy(model => model.Titles).OrderBy(model => model.MemberSince).ToList().ForEach(model => resortModels.Add(model));
            userModels.Where(model => model.Titles == string.Empty).OrderBy(model => model.MemberSince).ToList().ForEach(model => resortModels.Add(model));

            return View(resortModels);
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

        [OverrideAuthorization]
        public ActionResult ViewProfile(string id = "")
        {
            ApplicationUser currentUser;

            if(TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            if (id == string.Empty)
            {
                try
                {
                    id = User.Identity.GetUserId();
                    if ((id == null) || (id == string.Empty))
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            TempData.Clear();
            TempData.Add("UserId", id);

            currentUser = db.Users.Where(user => user.Id == id).FirstOrDefault();
            currentUser.LoadApiKeys();
            currentUser.Titles = db.Titles.Where(title => title.UserId == id).ToList();

            var roles = db.Roles.Select(role => role.Name).ToList();
                                        
            if(currentUser.Titles.Count == 0)
            {
                currentUser.Titles.Add(new Title(){ UserId = currentUser.Id, TitleName = "Corp Member"});
            }

            var userProfile = new UserProfileViewModel() { 
                Id = currentUser.Id,
                UserName = currentUser.UserName, EmailAddress = currentUser.EmailAddress, MemberSince = Convert.ToDateTime(currentUser.MemberSince), 
                MemberType = currentUser.MemberType, Avatar = currentUser.Avatar, Titles = string.Join(", ", currentUser.Titles.Select(t => t.TitleName).ToList()),
                WebsiteRoles = string.Join(", ", currentUser.Roles.Select(role => role.RoleId).ToList()),
                ApiKeys = db.ApiInfoes.Where(api => api.User.Id == currentUser.Id).ToList(),
                UserRoles = userManager.GetRoles(currentUser.Id).ToList(),
                AvailableRoles = roles
            };

            return View(userProfile);
        }

        [Authorize(Roles = "CEO, Admin, Director")]
        public ActionResult AssignRoles(FormCollection form)
        {
            string userId = form["userId"].ToString();
            ApplicationUser currentUser = db.Users.Where(user => user.Id == userId).FirstOrDefault();

            var roles = db.Roles.Select(role => role.Name).ToList();
            var userRoles = userManager.GetRoles(currentUser.Id).ToList();

            var account = new AccountController();

            foreach (var role in roles)
            {
                if ((form[role] != null) && !userRoles.Contains(role))
                {
                    account.UserManager.AddToRole(currentUser.Id, role);
                }
            }

            foreach (var role in userRoles)
            {
                if ((form[role] == null))
                {
                    account.UserManager.RemoveFromRole(currentUser.Id, role);
                }
            }
            
            return RedirectToAction("ViewProfile", new { id = currentUser.Id });
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

        public ActionResult DeleteApi(int id, string userId)
        {
            db.ApiInfoes.Where(api => api.Id == id).ToList().ForEach(api => db.Entry(api).State = EntityState.Deleted);
            db.SaveChanges();

            return RedirectToAction("ViewProfile", new { id = userId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Director, CEO")]
        public async Task<ActionResult> ResetPassword(string userId = "")
        {
            if (userId != string.Empty)
            {
                var provider = new DpapiDataProtectionProvider("Website");
                userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("Website")) { TokenLifespan = TimeSpan.FromHours(4) };

                var user = userManager.FindById(userId);
                var token = await userManager.GeneratePasswordResetTokenAsync(userId);

                var result = await userManager.ResetPasswordAsync(userId, token, string.Concat("R3MUSUser_", user.UserName.Substring(0, user.UserName.IndexOf(" "))));

                if(result.Succeeded)
                {
                    TempData.Add("Message", string.Format("Password reset confirmed: new password is {0}", string.Concat("R3MUSUser_", user.UserName.Substring(0, user.UserName.IndexOf(" ")))));
                }
                else
                {
                    TempData.Add("Message", string.Format("Password reset failed: {0}", result.Errors.ToList()[0]));
                }

                await userManager.UpdateAsync(user);
            }

            return RedirectToAction("ViewProfile", new { id = userId });
        }

        public ActionResult EditApi(int id)
        {
            var apiInfo = db.ApiInfoes.Where(api => api.Id == id).FirstOrDefault();
            var userId = TempData["UserId"].ToString();

            TempData.Clear();
            TempData.Add("UserId", userId);
            ViewBag.UserId = userId;

            return View(apiInfo);
        }

        [HttpPost]
        public ActionResult EditApi(ApiInfo apiInfo)
        {
            var userId = TempData["UserId"].ToString();
            if(ModelState.IsValid)
            {
                db.Entry(apiInfo).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("ViewProfile", new { id = userId });
        }

        public ActionResult CreateApi(string id)
        {
            var apiInfo = new ApiInfo();
            //ViewBag.UserId = id;
            TempData.Clear();
            TempData.Add("UserId", id);
            return View(apiInfo);
        }

        [HttpPost]
        public ActionResult CreateApi(ApiInfo apiInfo)
        {
            string userId = TempData["UserId"].ToString();

            apiInfo.User = db.Users.Where(user => user.Id == userId).FirstOrDefault();

            db.ApiInfoes.Add(apiInfo);
            db.SaveChanges();            

            return RedirectToAction("ViewProfile", new { id = userId });
        }
	}
}