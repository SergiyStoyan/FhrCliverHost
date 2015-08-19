using System;
using System.Collections.Generic;
//using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cliver.ProductOffice.Models;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class PriceComparisonController : Controller
    {
        private ProductOfficeEntities db = new ProductOfficeEntities();

        List<object> CompanySelect
        {
            get
            {
                var cs = (from r in db.Companies select new { Value = r.Id, Name = r.Name }).ToList<object>();
                return cs;
            }
        }

        // GET: Products
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TableJson([ModelBinder(typeof(DataTables.AspNet.Mvc5.ModelBinder))] DataTables.AspNet.Core.IDataTablesRequest request)
        {
            //JqueryDataTable.Field[] fields = new JqueryDataTable.Field[] { 
            //    new JqueryDataTable.Field("Id"),
            //    new JqueryDataTable.Field("LinkCategory", false, "b.Name"),
            //    new JqueryDataTable.Field("CreateTime"),
            //    new JqueryDataTable.Field("Name", true),
            //    new JqueryDataTable.Field("Sku", true),
            //    new JqueryDataTable.Field("Url")
            //};
            //return JqueryDataTable.Index(request, db.Database, "FROM Products a INNER JOIN LinkCategories b ON a.IdentityCategoryId=b.Id", fields);
            
            JqueryDataTable.Field[] fields = new JqueryDataTable.Field[] { 
                new JqueryDataTable.Field("Id"),
                new JqueryDataTable.Field("Category"),
                new JqueryDataTable.Field("UpdateTime"),
                new JqueryDataTable.Field("Name", true),
                new JqueryDataTable.Field("Sku", true),
                new JqueryDataTable.Field("Url")
            };
            return JqueryDataTable.Index(request, db.Database.Connection, "FROM Products", fields);
        }

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
