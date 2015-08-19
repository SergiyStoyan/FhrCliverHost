using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cliver.ProductOffice.Models;
using Cliver.ProductOffice.Filters;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using System.Transactions;
using DotNetOpenAuth.AspNet;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class UsersController : Controller
    {
        private UsersContext db = new UsersContext();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.UserProfiles.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = db.UserProfiles.Find(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView(userProfile);
            return View(userProfile);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
       }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, propertyValues: new { Active = model.Active });
                    if (Request.IsAjaxRequest())
                        return Content(null);
                    return RedirectToAction("Index");
                }
                catch (MembershipCreateUserException e)
                {
                    Errors.Add(Errors.Expand(e));
                }
            }

            if (Request.IsAjaxRequest())
                return PartialView(model);
            return View(model);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = db.UserProfiles.Find(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView(userProfile);
            return View(userProfile);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                //if (userProfile.UserId == WebSecurity.CurrentUserId && !userProfile.Active)
                //{
                //    Errors.Add("It is impossible to set own account inactive.");
                //    if (Request.IsAjaxRequest())
                //        return PartialView(userProfile);
                //    return View(userProfile);
                //}
                //if (0 < db.UserProfiles.Where(r => r.UserName.ToLower() == userProfile.UserName.ToLower() && r.UserId != userProfile.UserId).Count())
                //{
                //    Errors.Add("Such a name exists already. Please choose another one.");
                //    if (Request.IsAjaxRequest())
                //        return PartialView(userProfile);
                //    return View(userProfile);
                //}
                db.Entry(userProfile).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                if (Request.IsAjaxRequest())
                    return Content(null);
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
                return PartialView(userProfile);
            return View(userProfile);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserProfile userProfile = db.UserProfiles.Find(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }
            if (userProfile.UserId == WebSecurity.CurrentUserId)
            {
                Errors.Add("It is impossible to delete own account...");
                if (Request.IsAjaxRequest())
                    return PartialView(userProfile);
                return View(userProfile);
            }
            if (Request.IsAjaxRequest())
                return PartialView(userProfile);
            return View(userProfile);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserProfile userProfile = db.UserProfiles.Find(id);
            if (userProfile.UserId == WebSecurity.CurrentUserId)
            {
                Errors.Add("It is impossible to delete own account...");
                if (Request.IsAjaxRequest())
                    return Content(null);
                RedirectToAction("Index");
            }
            db.UserProfiles.Remove(userProfile);
            db.SaveChanges();
            if (Request.IsAjaxRequest())
                return Content(null);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
