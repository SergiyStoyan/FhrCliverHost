using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
                Cliver.Bot.DbConnection dbc;
                switch (database)
                {
                    case "CliverCrawlerHost":

                        dbc = Bot.DbConnection.Create(Cliver.CrawlerHost.Models.DbApi.ConnectionString);
                        n = dbc.Get(sql).Execute();

                        break;
                    case "ProductOffice":

                        dbc = Cliver.Bot.DbConnection.Create(Cliver.FhrApi.ProductOffice.Models.DbApi.GetProviderConnectionString());
                        n = dbc.Get(sql).Execute();

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