using System;
using System.Collections.Generic;
//using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cliver.FhrApi.ProductOffice.Models;
using System.Linq.Dynamic;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class LinkCategoriesController : Controller
    {
        private DbApi db = FhrApi.ProductOffice.Models.DbApi.Create();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TableJson([ModelBinder(typeof(DataTables.AspNet.Mvc5.ModelBinder))] DataTables.AspNet.Core.IDataTablesRequest request)
        {
            JqueryDataTable.Field[] fields = new JqueryDataTable.Field[] { 
                new JqueryDataTable.Field("Id"),
                new JqueryDataTable.Field("ParentId"),
                new JqueryDataTable.Field("Name", true) 
            };
            return JqueryDataTable.Index(request, db.Database.Connection, "FROM LinkCategories", fields);
        }

        public ActionResult Details(int id = 0)
        {
            LinkCategory category = db.LinkCategories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        public ActionResult Create()
        {
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LinkCategory category)
        {
            if (ModelState.IsValid)
            {
                db.LinkCategories.Add(category);
                db.SaveChanges();
                if (Request.IsAjaxRequest())
                    return Content(null);
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
                return PartialView(category);
            return View(category);
        }

        public ActionResult Edit(int id = 0)
        {
            LinkCategory category = db.LinkCategories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView(category);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LinkCategory category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                if (Request.IsAjaxRequest())
                    return Content(null);
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
                return PartialView(category);
            return View(category);
        }

        public ActionResult Delete(int id = 0)
        {
            LinkCategory category = db.LinkCategories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView(category);
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LinkCategory category = db.LinkCategories.Find(id);
            db.LinkCategories.Remove(category);
            db.SaveChanges();
            if (Request.IsAjaxRequest())
                return Content(null);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}