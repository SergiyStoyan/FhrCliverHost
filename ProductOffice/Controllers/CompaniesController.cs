using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Cliver.Fhr.ProductOffice.Models;
using Cliver.CrawlerHost.Models;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class CompaniesController : Controller
    {
        private Cliver.CrawlerHost.Models.DbApi chdb = Cliver.CrawlerHost.Models.DbApi.Create();
        private Cliver.Fhr.ProductOffice.Models.DbApi db = Fhr.ProductOffice.Models.DbApi.Create();
        
        List<object> CrawlerSelect
        {
            get            
            {
                var cs = chdb.Crawlers.ToList().Select(r => new { Value = r.Id, Name = r.Id + " (" + r.Site + ")" + (string.IsNullOrWhiteSpace(r.Comment) ? "" : " [" + r.Comment + "]") }).ToList<object>();
                cs.Insert(0, new { Value = "", Name = "-- no crawler --" });
                return cs;
            }
        }

        //
        // GET: /Company/

        public ActionResult Index()
        {
            //return View(db.Companies.ToList());
            return View();
        }

        public ActionResult TableJson([ModelBinder(typeof(DataTables.AspNet.Mvc5.ModelBinder))] DataTables.AspNet.Core.IDataTablesRequest request)
        {
            JqueryDataTable.Field[] fields = new JqueryDataTable.Field[] {                 
                new JqueryDataTable.Field("Id", false, 0), 
                new JqueryDataTable.Field("Name", true, 1),  
                new JqueryDataTable.Field("Url", true),                                 
                new JqueryDataTable.Field("Comment", true),
                new JqueryDataTable.Field("CrawlerId", true)                                      
            };
            JsonResult jr = JqueryDataTable.Index(request, db.Database.Connection, "FROM Companies", fields);
            return jr;
        }   

        public ActionResult Details(int id = 0)
        {
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView(company);
            return View(company);
        }

        public ActionResult Create()
        {
            ViewBag.CrawlerId = new SelectList(CrawlerSelect, "Value", "Name");
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Company company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrWhiteSpace(company.CrawlerId))
                        company.CrawlerId = null;
                    db.Companies.Add(company);
                    db.SaveChanges();
                    if (Request.IsAjaxRequest())
                        return Content(null);
                    return RedirectToAction("Index");
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                var g = e.EntityValidationErrors;
            }
            if (Request.IsAjaxRequest())
                return PartialView(company);
            return View(company);
        }

        public ActionResult Edit(int id = 0)
        {
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            ViewBag.CrawlerId = new SelectList(CrawlerSelect, "Value", "Name", company.CrawlerId);
            if (Request.IsAjaxRequest())
                return PartialView(company);         
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Company company)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(company.CrawlerId))
                    company.CrawlerId = null;
                db.Entry(company).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                if (Request.IsAjaxRequest())
                    return Content(null);
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
                return PartialView(company);
            return View(company);
        }

        public ActionResult Delete(int id = 0)
        {
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView(company);            
            return View(company);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Companies.Find(id);
            if (db.ImportMaps.Where(m => m.CompanyId == id).Count() > 0)
            {
                Errors.Add("Please remove Import Maps beloning to this compnay before deleting it.");
                return PartialView("_Notifications");
            }
            db.Companies.Remove(company);
            foreach(var p in db.Products.Where(p => p.CompanyId == id))
               Cliver.Fhr.ProductOffice.DataApi.Product.Delete(db, p.Id);
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