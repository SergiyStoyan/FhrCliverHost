using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cliver.ProductOffice.Models;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class ImportMapsController : Controller
    {
        private ProductOfficeEntities db = new ProductOfficeEntities();

        // GET: ImportMaps
        public ActionResult Index()
        {
            var importMaps = db.ImportMaps.Include(i => i.Company);
            return View(importMaps.ToList());
        }

        // GET: ImportMaps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImportMap importMap = db.ImportMaps.Find(id);
            if (importMap == null)
            {
                return HttpNotFound();
            }
            if(Request.IsAjaxRequest())
                return PartialView(importMap);
            return View(importMap);
        }

        // GET: ImportMaps/Create
        public ActionResult Create()
        {
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name");
            ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "Name");
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }

        // POST: ImportMaps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CompanyId,Name,SkipFirstRow,C_CompanyProductIdI,C_NameI,C_SkuI,C_PriceI,C_DescriptionI,C_CategoryI")] ImportMap importMap)
        {
            if (ModelState.IsValid)
            {
                db.ImportMaps.Add(importMap);
                db.SaveChanges();
                if (Request.IsAjaxRequest())
                    return Content(null);
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", importMap.CompanyId);
            ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "Name", importMap.CurrencyId);
            if (Request.IsAjaxRequest())
                return PartialView(importMap);
            return View(importMap);
        }

        // GET: ImportMaps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImportMap importMap = db.ImportMaps.Find(id);
            if (importMap == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", importMap.CompanyId);
            ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "Name", importMap.CurrencyId);
            if (Request.IsAjaxRequest())
                return PartialView(importMap);
            return View(importMap);
        }

        // POST: ImportMaps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ImportMap importMap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(importMap).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                if (Request.IsAjaxRequest())
                    return Content(null);
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", importMap.CompanyId);
            ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "Name", importMap.CurrencyId);
            if (Request.IsAjaxRequest())
                return PartialView(importMap);
            return View(importMap);
        }

        // GET: ImportMaps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImportMap importMap = db.ImportMaps.Find(id);
            if (importMap == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView(importMap);
            return View(importMap);
        }

        // POST: ImportMaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ImportMap importMap = db.ImportMaps.Find(id);
            db.ImportMaps.Remove(importMap);
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
