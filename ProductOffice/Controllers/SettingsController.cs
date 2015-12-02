using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cliver.CrawlerHost.Models;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;


namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        //private DbApi db = DbApi.Create();
        //List<object> ObjectSelect
        //{
        //    get
        //    {
        //        var os = db.Crawlers.ToList().Select(r => new { Value = r.Id, Name = r.Id + (string.IsNullOrWhiteSpace(r.Comment) ? "" : " [" + r.Comment + "]") }).ToList<object>();
        //        var os2 = db.Services.ToList().Select(r => new { Value = r.Id, Name = r.Id + (string.IsNullOrWhiteSpace(r.Comment) ? "" : " [" + r.Comment + "]") }).ToList<object>();
        //        os.AddRange(os2);
        //        return os;
        //    }
        //}

        public ActionResult Index()
        {
            ViewBag.Database = "CliverCrawlerHost";
            return View();
        }

        public ActionResult TableJson([ModelBinder(typeof(DataTables.AspNet.Mvc5.ModelBinder))] DataTables.AspNet.Core.IDataTablesRequest request)
        {
            string database_ = request.Columns.ToList()[1].Search.Value;
            if (database_ == null)
                return Json(new { });
            Match m = Regex.Match(database_, @"Database\s*=\s*(.+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (!m.Success)
                return Json(new { });
            string database = m.Groups[1].Value;
            Session["Database"] = database;
            Cliver.Bot.DbConnection dbc = GetDbc(database);
            JqueryDataTable.Field[] fields = new JqueryDataTable.Field[] {                 
                new JqueryDataTable.Field("Scope", true), 
                new JqueryDataTable.Field("[Key]", true),                               
                new JqueryDataTable.Field("SetTime", false),
            };
            return JqueryDataTable.Index(request, dbc, "FROM Settings", fields);
        }

        Cliver.Bot.DbConnection GetDbc(string database)
        {
            switch (database)
            {
                case "CliverCrawlerHost":
                    return Bot.DbConnection.Create(Cliver.CrawlerHost.Models.DbApi.GetConnectionString());
                case "ProductOffice":
                    return Cliver.Bot.DbConnection.Create(Cliver.Fhr.ProductOffice.Models.DbApi.GetProviderConnectionString());
                default:
                    throw new Exception("Unknown databse: " + database);
            }
        }

        public ActionResult Edit(string database, string scope, string key)
        {
            if (database == null)
                database = (string)Session["Database"];
            if (database == null || scope == null || key == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Bot.DbSettings dss = new Bot.DbSettings(GetDbc(database));
            ViewBag.Scope = scope;
            ViewBag.Key = key;
            dynamic value = dss.Get<dynamic>(scope, key);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.Value = serializer.Serialize(value);
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string database, string scope, string key, string value)
        {
            if (database == null)
                database = (string)Session["Database"];
            if (database == null || scope == null || key == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Bot.DbSettings dss = new Bot.DbSettings(GetDbc(database));
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> ss = serializer.Deserialize<Dictionary<string, object>>(value);
            dss.Save(scope, key, ss);
            if (Request.IsAjaxRequest())
                return Content(null);
            return RedirectToAction("Index");
        }

        public ActionResult Details(string database, string scope, string key)
        {
            if (database == null)
                database = (string)Session["Database"];
            if (database == null || scope == null || key == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Bot.DbSettings dss = new Bot.DbSettings(GetDbc(database));
            ViewBag.Scope = scope;
            ViewBag.Key = key;
            dynamic value = dss.Get<dynamic>(scope, key);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.Value = serializer.Serialize(value);
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }

        public ActionResult Delete(string database, string scope, string key)
        {
            if (database == null)
                database = (string)Session["Database"];
            if (database == null || scope == null || key == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Bot.DbSettings dss = new Bot.DbSettings(GetDbc(database));
            ViewBag.Scope = scope;
            ViewBag.Key = key;
            dynamic value = dss.Get<dynamic>(scope, key);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.Value = serializer.Serialize(value);
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string database, string scope, string key)
        {
            if (database == null)
                database = (string)Session["Database"];
            if (database == null || scope == null || key == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Bot.DbSettings dss = new Bot.DbSettings(GetDbc(database));
            dss.Delete(scope, "%");
            if (Request.IsAjaxRequest())
                return Content(null);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            //   db.Dispose();
            base.Dispose(disposing);
        }
    }
}