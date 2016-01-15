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
    [HandleActionError]
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
                throw new Exception("No database specified");
            Match m = Regex.Match(database_, @"Database\s*=\s*(.+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (!m.Success)
                throw new Exception("No database found: " + database_);
            string database = m.Groups[1].Value;
            Session["Database"] = database;
            Cliver.Bot.DbConnection dbc = GetDbc(database);
            if (null == dbc.Get("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE 'Settings'").GetSingleValue())
                throw new DatatableException("No table 'Settings' found in " + database_);

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
            ViewBag.Scope = scope;
            ViewBag.Key = key;
            ViewBag.Value = get_value_as_string(database, scope, key);
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }

        string get_value_as_string(string database, string scope, string key)
        {
            dynamic value = Cliver.Bot.DbSettings.Get<dynamic>(GetDbc(database), scope, key);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string v = serializer.Serialize(value);
            return Regex.Replace(v, @"""(?'Key'.*?)""\s*\:\s*(?'Value'\d+|"".*?""|\[.*?\])\s*(,|})", "$0\r\n", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string database, string scope, string key, string value)
        {
            if (database == null)
                database = (string)Session["Database"];
            if (database == null || scope == null || key == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> ss = serializer.Deserialize<Dictionary<string, object>>(value);
            Cliver.Bot.DbSettings.Save(GetDbc(database), scope, key, ss);
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
            ViewBag.Scope = scope;
            ViewBag.Key = key;
            ViewBag.Value = get_value_as_string(database, scope, key);
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
            ViewBag.Scope = scope;
            ViewBag.Key = key;
            ViewBag.Value = get_value_as_string(database, scope, key);
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
            Cliver.Bot.DbSettings.Delete(GetDbc(database), scope, key);
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