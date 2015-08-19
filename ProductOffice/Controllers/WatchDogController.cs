using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cliver.ProductOffice.Models;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class WatchDogController : Controller
    {
        private CrawlerHostDataContext chdc = new CrawlerHostDataContext(Cliver.CrawlerHost.DbApi.ConnectionString);

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TableJson([ModelBinder(typeof(DataTables.AspNet.Mvc5.ModelBinder))] DataTables.AspNet.Core.IDataTablesRequest request)
        {
            try
            {
                string table = Cliver.CrawlerHostWatchDog.WatchDog.GetReportsTempTable(chdc.Connection);
                JqueryDataTable.Field[] fields = new JqueryDataTable.Field[] {                 
                new JqueryDataTable.Field("Source", true), 
                new JqueryDataTable.Field("SourceType", true),  
                new JqueryDataTable.Field("MessageType", true), 
                new JqueryDataTable.Field("Value", true),
               // new JqueryDataTable.Field("Details", true)                                        
            };
            JsonResult jr = JqueryDataTable.Index(request, chdc.Connection, "FROM " + table, fields);
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
            chdc.Dispose();
            base.Dispose(disposing);
        }
    }
}
