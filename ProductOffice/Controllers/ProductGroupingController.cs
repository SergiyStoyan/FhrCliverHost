using System;
using System.Collections.Generic;
//using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cliver.ProductOffice.Models;
using System.Text.RegularExpressions;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class ProductGroupingController : Controller
    {
        private ProductOfficeEntities db = new ProductOfficeEntities();
        
        List<object> CompanySelect
        {
            get
            {
                var cs = (from r in db.Companies select new { Value = r.Id, Name = r.Name }).ToList<object>();
                cs.Insert(0, new { Value = -1, Name = "-- ALL --" });
                return cs;
            }
        }

        // GET: Products
        public ActionResult Index()
        {
            ViewBag.CompanyId = new SelectList(CompanySelect, "Value", "Name");
            var products = db.Products.Include(p => p.Company);
            return View(products);
        }

        public ActionResult TableJson([ModelBinder(typeof(DataTables.AspNet.Mvc5.ModelBinder))] DataTables.AspNet.Core.IDataTablesRequest request)
        {
            JqueryDataTable.Field[] fields = new JqueryDataTable.Field[] { 
                new JqueryDataTable.Field("Id"),
                new JqueryDataTable.Field("UpdateTime"),
                new JqueryDataTable.Field("ImageUrls", false),
                new JqueryDataTable.Field("Name", true),
                new JqueryDataTable.Field("Category"),
                new JqueryDataTable.Field("Sku", true),
                new JqueryDataTable.Field("Url"),
            };

            string from_sql;
            string explode_groups_info = request.Columns.ToList()[1].Search.Value;
            if (explode_groups_info!=null && Regex.IsMatch(explode_groups_info, @"ExplodeGroups\s*=\s*true", RegexOptions.Singleline| RegexOptions.IgnoreCase))
                from_sql = "FROM Products";
            else
                from_sql = "FROM Products WHERE MainProductId<0";

            JsonResult jr = JqueryDataTable.Index(request, db.Database.Connection, from_sql, fields);
            //foreach (var r in ((dynamic)jr.Data).Data)
            //{
            //    string s = Convert.ToString(r[2]);
            //    if (s != null)
            //        r[2] = "<img src='" + Regex.Replace(s, @"[\r\n].*", "", RegexOptions.Singleline) + "'/>";
            //    s = Convert.ToString(r[6]);
            //    if (s != null)
            //        r[6] = "<a href='" + s + "' target='_blank'>Site</a>";
            //}
            return jr;
        }

        public ActionResult GroupProducts(int[] product_ids, int? main_product_id)
        {
            if (product_ids == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "product_ids == null");
            if (main_product_id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "main_product_id == null");
            if (null == db.Products.Where(p => p.Id == main_product_id).FirstOrDefault())
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Product Id=" + main_product_id + " does not exist");
            product_ids = product_ids.Where(i => i != main_product_id).ToArray();
            db.Products.Where(p => product_ids.Contains(p.Id)).ToList().ForEach((p) => { p.MainProductId = (int)main_product_id; });
            db.SaveChanges();
            return PartialView(null);
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView(product);
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name");
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                if (Request.IsAjaxRequest())
                    return Content(null);
                return RedirectToAction("Index");
            }

            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", product.CompanyId);
            if (Request.IsAjaxRequest())
                return PartialView(product);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", product.CompanyId);
            if (Request.IsAjaxRequest())
                return PartialView(product);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                if (Request.IsAjaxRequest())
                    return Content(null);
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "Id", "Name", product.CompanyId);
            if (Request.IsAjaxRequest())
                return PartialView(product);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView(product);
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
