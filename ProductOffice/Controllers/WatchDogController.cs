using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cliver.CrawlerHost.Models;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class WatchDogController : Controller
    {
        private DbApi db = DbApi.Create();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TableJson([ModelBinder(typeof(DataTables.AspNet.Mvc5.ModelBinder))] DataTables.AspNet.Core.IDataTablesRequest request)
        {
            try
            {
                string table = Cliver.CrawlerHostWatchDog.WatchDog.GetReportsTempTable(db.Connection);
                JqueryDataTable.Field[] fields = new JqueryDataTable.Field[] {                 
                new JqueryDataTable.Field("Source", true), 
                new JqueryDataTable.Field("SourceType", true),  
                new JqueryDataTable.Field("MessageType", true), 
                new JqueryDataTable.Field("Value", true),
               // new JqueryDataTable.Field("Details", true)                                        
            };
            JsonResult jr = JqueryDataTable.Index(request, db.Connection, "FROM " + table, fields);
            return jr;
            }
            catch (Exception e)
            {
                Errors.Add(e.Message);
            }
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
