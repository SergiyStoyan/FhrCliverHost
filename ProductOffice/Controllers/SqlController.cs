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
        Cliver.CrawlerHost.DbApi db_api = new CrawlerHost.DbApi();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string sql)
        {
            ViewBag.Sql = sql;
            try
            {
                int n = db_api.Connection.Get(sql).Execute();
                Messages.Add("Affected: " + n);
                if (Request.IsAjaxRequest())
                    return Content(null);
                return View();
            }
            catch(Exception e)
            {
                for (; e.InnerException != null; e = e.InnerException) ;
                Errors.Add("Exception: <br>" + e.Message);
            }
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }
    }
}