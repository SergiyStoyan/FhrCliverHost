using System;
using System.Collections.Generic;
//using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cliver.FhrApi.ProductOffice.Models;
using Cliver.FhrApi.ProductOffice;
using System.Text.RegularExpressions;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private DbApi db = new DbApi();

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
                //new JqueryDataTable.Field("Url", false, 1, "CASE MainProductId<0 THEN Id ELSE MainProductId"),
            };
            
            string from_sql;
            string expand_groups_ = request.Columns.ToList()[1].Search.Value;
            if (expand_groups_ != null && Regex.IsMatch(expand_groups_, @"ExpandGroups\s*=\s*true", RegexOptions.Singleline | RegexOptions.IgnoreCase))
                from_sql = "FROM Products";
            else
                from_sql = @"FROM Products INNER JOIN 
(SELECT MIN(Id) AS Gid FROM Products WHERE MainProductId>=0 AND MainProductId NOT IN (SELECT Id AS Gid FROM Products WHERE MainProductId=Id) GROUP BY MainProductId
UNION
SELECT Id AS Gid FROM Products WHERE MainProductId=Id
UNION
SELECT Id AS Gid FROM Products WHERE MainProductId<0
) a ON a.Gid=Products.Id";

            return JqueryDataTable.Index(request, db.Database.Connection, from_sql, fields);
        }

        public ActionResult SaveGroup(
            [Bind(Prefix = "product_ids[]")]string[] product_ids_,
            int? main_product_id
            )
        {
            int[] product_ids;
            if (product_ids_ != null)
                product_ids = (from x in product_ids_ where !string.IsNullOrWhiteSpace(x) select int.Parse(x)).ToArray();
            else
                product_ids = new int[] { };
            if (main_product_id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "main_product_id == null");
            if (!product_ids.Contains((int)main_product_id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "main_product_id is not within product_ids");
            if (null == db.Products.Where(p => p.Id == main_product_id).FirstOrDefault())
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Product Id=" + main_product_id + " does not exist");
            db.Products.Where(p => p.MainProductId == main_product_id).ToList().ForEach((p) => { p.MainProductId = -1; });
            if (product_ids.Length > 1)
                db.Products.Where(p => product_ids.Contains(p.Id)).ToList().ForEach((p) => { p.MainProductId = (int)main_product_id; });
            try
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                string m = "";
                foreach (var validationErrors in e.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                        m += validationError.PropertyName + ": " + validationError.ErrorMessage + "\r\n";
                }
                return PartialView(m);
            }
            return Content(null);
        }

        //public ActionResult AddProduct2Group(
        //    int? product_id,
        //    int? main_product_id
        //    )
        //{
        //    if (product_id == null)
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "product_id == null");
        //    if (main_product_id == null)
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "main_product_id == null");
        //    if (product_id == main_product_id)
        //        db.Products.Where(p => p.MainProductId == main_product_id).ToList().ForEach((p) => { p.MainProductId = -1; });
        //    if (null == db.Products.Where(p => p.Id == main_product_id).FirstOrDefault())
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Product Id=" + main_product_id + " does not exist");
        //    if (product_ids.Length > 1)
        //        db.Products.Where(p => product_ids.Contains(p.Id)).ToList().ForEach((p) => { p.MainProductId = (int)main_product_id; });
        //    try
        //    {
        //        db.Configuration.ValidateOnSaveEnabled = false;
        //        db.SaveChanges();
        //    }
        //    catch (System.Data.Entity.Validation.DbEntityValidationException e)
        //    {
        //        string m = "";
        //        foreach (var validationErrors in e.EntityValidationErrors)
        //        {
        //            foreach (var validationError in validationErrors.ValidationErrors)
        //                m += validationError.PropertyName + ": " + validationError.ErrorMessage + "\r\n";
        //        }
        //        return PartialView(m);
        //    }
        //    return Content(null);
        //}

        public ActionResult GetGroup(int? product_id)
        {
            if (product_id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "product_id == null");
            int? main_product_id = db.Products.Where(p => p.Id == product_id).Select(p => p.MainProductId < 0 ? p.Id : p.MainProductId).FirstOrDefault();
            if (main_product_id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Product Id:" + product_id + "  does not exist.");
            List<Product> ps = db.Products.Where(p => p.MainProductId == main_product_id || p.Id == main_product_id).ToList();
            List<object[]> rs = new List<object[]>();
            foreach(Product p in ps)
            {
                object[] r = { p.Id, p.UpdateTime, p.ImageUrls, p.Name, p.Category, p.Sku, p.Url };
                if (p.MainProductId < 0 || p.MainProductId == p.Id)
                    rs.Insert(0, r);
                else
                    rs.Add(r);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteGroup(int? main_product_id)
        {
            if (main_product_id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "main_product_id == null");
            db.Products.Where(p => p.MainProductId == main_product_id).ToList().ForEach((p) => { p.MainProductId = -1; });
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();
            return Content(null);
        }

        public ActionResult Ungroup([Bind(Prefix = "product_ids[]")]string[] product_ids_)
        {    
            if (product_ids_ == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "product_ids_ == null");
            int[] product_ids = (from x in product_ids_ where !string.IsNullOrWhiteSpace(x) select int.Parse(x)).ToArray();
            db.Products.Where(p => product_ids.Contains(p.Id)).ToList().ForEach((p) => { p.MainProductId = -1; });
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();
            return Content(null);
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
            DataApi.Products.Delete(db, id);
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
