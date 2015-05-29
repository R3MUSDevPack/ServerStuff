using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using r3mus.Models;
using r3mus.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using r3mus.Filters;

namespace r3mus.Controllers
{
    [Authorize]
    public class RecruitmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RecruitmentStatEntities ent = new RecruitmentStatEntities();
        private ApplicantEntities appent = new ApplicantEntities();

        //protected UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        //
        // GET: /Recruitment/
        public ActionResult Index()
        {
            var statsVM = new RecruitmentStatsViewModel();
            statsVM.Submitters = ent.LastWeeksSubmissionStats.ToList();
            statsVM.Mailers = ent.LastWeeksMailStats.ToList();
            statsVM.MailsToSend = db.RecruitmentMailees.Where(m => (m.MailerId == null) && (m.CorpId_AtLastCheck >= 1000000) && (m.CorpId_AtLastCheck <= 1000200)).Count();
            statsVM.ApplicationsToProcess = appent.ApplicantLists.Where(a => (a.Status != ApplicationReviewViewModel.ApplicationStatus.Accepted.ToString()) && (a.Status != ApplicationReviewViewModel.ApplicationStatus.Rejected.ToString())).Count();

            return View(statsVM);
        }

        public ActionResult GetNames()
        {
            var mailees = db.RecruitmentMailees.Where(m => (m.MailerId == null) && (m.CorpId_AtLastCheck >= 1000000) && (m.CorpId_AtLastCheck <= 1000200)).Take(20);

            List<RecruitmentMailee> NPCMailees = new List<RecruitmentMailee>();

            foreach (RecruitmentMailee mailee in mailees)
            {
                //if (mailee.IsInNPCCorp())
                //{
                    NPCMailees.Add(mailee);
                //}
                //if (NPCMailees.Count == 20)
                //{
                //    break;
                //}
            }

            NPCMailees.ToList().ForEach(m => m.MailerId = User.Identity.GetUserId());
            db.SaveChanges();

            ViewBag.Mailees = string.Join(",", NPCMailees.Select(m => m.Name).ToArray());

            return View();
        }

        [HttpPost]
        //[ActionName("GetNames")]
        [r3mus.Filters.ApiKeyAttribute.MultipleButton(Name = "GetNames", Argument = "sent")]
        public ActionResult UpdateNamesAfterMailing(FormCollection form)
        {
            string names = string.Concat("'", form["mailees"].ToString().Replace(",", "','"), "'");

            var mailees = (from RecruitmentMailee in db.RecruitmentMailees
                           where names.Contains(RecruitmentMailee.Name)
                           select RecruitmentMailee);
            mailees.ToList().ForEach(m => m.Mailed = DateTime.Now);
            db.SaveChanges();            

            return RedirectToAction("Index");
        }

        [HttpPost]
        //[ActionName("GetNames")]
        [r3mus.Filters.ApiKeyAttribute.MultipleButton(Name = "GetNames", Argument = "unlock")]
        public ActionResult UnlockNames(FormCollection form)
        {
            string names = string.Concat("'", form["mailees"].ToString().Replace(",", "','"), "'");

            var mailees = (from RecruitmentMailee in db.RecruitmentMailees
                           where names.Contains(RecruitmentMailee.Name)
                           select RecruitmentMailee);
            mailees.ToList().ForEach(m => m.MailerId = null);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult AddNames()
        {
            return View();
        }

        [HttpPost]
        [ActionName("AddNames")]
        public ActionResult AddNamesToDB(FormCollection form)
        {
            string[] names = form["names"].ToString().Split(new string[]{Environment.NewLine}, StringSplitOptions.None).Distinct().ToArray();
            IList<RecruitmentMailee> mailees = new List<RecruitmentMailee>();
            
            foreach (string name in names)
            {
                if (db.RecruitmentMailees.Where(m => m.Name == name).Any() == false)
                {
                    RecruitmentMailee mailee = new RecruitmentMailee() { Name = name, Submitted = DateTime.Now };
                    //mailee.IsInNPCCorp();

                    db.RecruitmentMailees.Add(mailee);
                }
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [OverrideAuthorization]
        public ActionResult Apply()
        {
            var model = new ApplicantViewModel();

            return View(model);
        }

        [OverrideAuthorization]
        [HttpPost]
        [ActionName("Apply")]
        public ActionResult SubmitApplication(ApplicantViewModel model)
        {
            var applicant = new Applicant() { Name = model.UserName, EmailAddress = model.Email, TimeZone = model.TimeZone, Information = model.Information, Age = model.Age, ToonAge = model.ToonAge, Source = model.Source };
            
            ApiInfo api = new ApiInfo() { ApiKey = Convert.ToInt32(model.ApiKey), VerificationCode = model.VerificationCode };
            bool goodAccessMask = api.ValidateAccessMask(ApplicationUser.IDType.Corporation);
            bool goodTZ = (model.TimeZone != "Select a Time Zone");

            if (goodAccessMask && goodTZ)
            {
                applicant.ApiKey = Convert.ToInt32(model.ApiKey);
                applicant.VerificationCode = model.VerificationCode;

                db.Applicants.Add(applicant);
                db.SaveChanges();

                //applicant = db.Applicants.Where(m => m.Name == applicant.Name).Last();
                var application = new Application() { ApplicantId = applicant.Id, Notes = "New application", Status = ApplicationReviewViewModel.ApplicationStatus.Applied.ToString(), DateTimeCreated = DateTime.Now };
                db.Applications.Add(application);
                db.SaveChanges();

                TempData.Clear();
                TempData.Add("Message", "Thank you for your application. A recruiter will contact you shortly.");

                return RedirectToAction("Index", "Home");
            }

            if (!goodAccessMask)
            {
                ViewBag.Message = "The access mask for the API key you have submitted is not correct. Please correct this and try again.";
            }
            else if(!goodTZ)
            {
                ViewBag.Message = "Please select a valid Time Zone.";
            }
            return View(model);
        }

        public ActionResult ShowApplications(bool review = false)
        {
            IQueryable<ApplicantList> apps;
            if (!review)
            {
                apps = appent.ApplicantLists.Where(a => (a.Status != ApplicationReviewViewModel.ApplicationStatus.Accepted.ToString()) && (a.Status != ApplicationReviewViewModel.ApplicationStatus.Rejected.ToString())).OrderBy(a => a.Applied);
                ViewBag.Title = "Applications To Process";
            }
            else
            {
                apps = appent.ApplicantLists.Where(a => (a.Status == ApplicationReviewViewModel.ApplicationStatus.Accepted.ToString()) || (a.Status == ApplicationReviewViewModel.ApplicationStatus.Rejected.ToString())).OrderBy(a => a.Applied);
                ViewBag.Title = "Processed Applications";
            }

            return View(apps);
        }

        public ActionResult ReviewApplication(int? id)
        {
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            if (id == null)
            {
                return RedirectToAction("ShowApplications");
            }
            var newReviewModel = new ApplicationReviewViewModel() { };

            newReviewModel.ApplicationInfo = db.Applications.Where(a => a.ApplicantId == id).ToList<Application>();
            if ((newReviewModel.ApplicationInfo.Last().Status == ApplicationReviewViewModel.ApplicationStatus.Accepted.ToString()) || (newReviewModel.ApplicationInfo.Last().Status == ApplicationReviewViewModel.ApplicationStatus.Rejected.ToString()))
            {
                ViewBag.Complete = true;
            }
            else
            {
                ViewBag.Complete = false;
            }

            newReviewModel.Applicant = db.Applicants.Where(a => a.Id == id).FirstOrDefault();
            if((newReviewModel.Applicant.TimeZone == null) || (newReviewModel.Applicant.TimeZone == string.Empty))
            {
                newReviewModel.Applicant.TimeZone = "Unknown";
            }
            newReviewModel.NewReviewItem = new Application() { ApplicantId = newReviewModel.Applicant.Id, Reviewer = UserManager.FindById(User.Identity.GetUserId()) };

            return View(newReviewModel);
        }

        [HttpPost]
        [ActionName("ReviewApplication")]
        public ActionResult SubmitNewReviewItem(ApplicationReviewViewModel model)
        {            
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            if(ModelState.IsValid)
            {
                model.NewReviewItem.Reviewer = UserManager.FindByName(model.NewReviewItem.Reviewer.UserName);
                //model.NewReviewItem.Applicant = db.Applicants.Where(a => a.Id == model.NewReviewItem.Applicant.Id).FirstOrDefault();
                model.NewReviewItem.Status = model.NewReviewItemStatus.GetDisplayName();
                
                model.NewReviewItem.DateTimeCreated = DateTime.Now;
                
                db.Applications.Add(model.NewReviewItem);
                db.SaveChanges();

                return RedirectToAction("ShowApplications");
            }
            return View(model);
        }
	}
}
