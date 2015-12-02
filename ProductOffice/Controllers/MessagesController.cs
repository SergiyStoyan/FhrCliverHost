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
    public class MessagesController : Controller
    {
        private DbApi db = DbApi.Create();

        List<object> SourceSelect
        {
            get
            {
                var ss = (from r in db.Messages group r by r.Source into s select new { Value = s.Key, Name = s.Key }).ToList<object>();
                ss.Insert(0, new { Value = "", Name = "-- ALL --" });
                return ss;
            }
        }

        List<object> MarkSelect
        {
            get
            {
                var ss = (from v in (Cliver.CrawlerHost.DbApi.MessageMark[])Enum.GetValues(typeof(Cliver.CrawlerHost.DbApi.MessageMark)) select new { Name = v.ToString(), Value = (int)v }).ToList<object>();
                //cs.Insert(0, new { Value = "", Name = "-- no mark --" });
                return ss;
            }
        }

        List<object> TypeSelect
        {
            get
            {
                var ts = (from v in (Cliver.Bot.Log.MessageType[])Enum.GetValues(typeof(Cliver.Bot.Log.MessageType)) select new { Name = v.ToString(), Value = (int)v }).ToList<object>();
                ts.Insert(0, new { Name = "-- ALL --", Value = "" });
                return ts;
            }
        }

        public ActionResult Index()
        {
            ViewBag.Source = new SelectList(SourceSelect, "Value", "Name");
            ViewBag.TypeId = new SelectList(TypeSelect, "Value", "Name");
            return View();
        }

        public ActionResult TableJson([ModelBinder(typeof(DataTables.AspNet.Mvc5.ModelBinder))] DataTables.AspNet.Core.IDataTablesRequest request)
        {
            JqueryDataTable.Field[] fields = new JqueryDataTable.Field[] {                 
                new JqueryDataTable.Field("Id", false, -1), 
                new JqueryDataTable.Field("Type"),  
                new JqueryDataTable.Field("Source", true),                                 
                new JqueryDataTable.Field("Time"),
                new JqueryDataTable.Field("Value", true),
                new JqueryDataTable.Field("Details", true)                                        
            };
            JsonResult jr = JqueryDataTable.Index(request, db.Connection, "FROM Messages", fields);
            foreach (var r in ((dynamic)jr.Data).Data)
            {
                if (!String.IsNullOrEmpty(Convert.ToString(r[1])))
                    r[1] = ((Cliver.Bot.Log.MessageType)r[1]).ToString();
                if (!String.IsNullOrEmpty(Convert.ToString(r[4])))
                    r[4] = PrepareField.Trim(r[4], 100);
                if (!String.IsNullOrEmpty(Convert.ToString(r[5])))
                    r[5] = PrepareField.Trim(r[5], 50);
            }
            return jr;
        }                                                               

        public ActionResult Details(int id)
        {         
            Cliver.CrawlerHost.Models.Message message = db.Messages.Where(r => r.Id == id).FirstOrDefault();
            if (message == null)
            {                                                                                 
                return HttpNotFound();
            }
            ViewBag.Mark = new SelectList(MarkSelect, "Value", "Name");
            if (Request.IsAjaxRequest())
                return PartialView(message);
            return View(message);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
