using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cliver.ProductOffice.Models;
using System.Configuration;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class SqlController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string sql, string database)
        {
            try
            {
                ViewBag.Sql = sql;
                ViewBag.Database = database;
                int n = -1;
                switch (database)
                {
                    case "CliverCrawlerHost":

                        Cliver.CrawlerHost.DbApi db_api = new CrawlerHost.DbApi();
                        n = db_api.Connection.Get(sql).Execute();

                        break;
                    case "ProductOffice":

                        Cliver.FhrCrawlerHost.Db2Api db2_api = new FhrCrawlerHost.Db2Api();
                        n = db2_api.Connection.Get(sql).Execute();

                        break;
                    default:
                        throw new Exception("Unknown databse: " + database);
                }
                Messages.Add("Database: '" + database + "' Affected: " + n);
                if (Request.IsAjaxRequest())
                    return Content(null);
                return View();
            }
            catch (Exception e)
            {
                for (; e.InnerException != null; e = e.InnerException) ;
                Errors.Add("Exception in database '" + database + "' : <br>" + e.Message);
            }
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }
    }
}