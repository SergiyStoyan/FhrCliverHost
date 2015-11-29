using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cliver.CrawlerHost.Models;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class CrawlersController : Controller
    {
        private DbApi db = new DbApi();
   
        List<object> StateSelect
        {
            get            
            {
                var ss = (from v in (Cliver.CrawlerHost.Crawler.State[])Enum.GetValues(typeof(Cliver.CrawlerHost.Crawler.State)) select new { Name = v.ToString(), Value = (int)v }).ToList<object>();
                //cs.Insert(0, new { Id = "", Name = "-- no crawler --" });
                return ss;
            }
        }
           
        List<object> CommandSelect
        {
            get            
            {
                var cs = (from v in (Cliver.CrawlerHost.Crawler.Command[])Enum.GetValues(typeof(Cliver.CrawlerHost.Crawler.Command)) select new { Name = v.ToString(), Value = (int)v }).ToList<object>();
                //cs.Insert(0, new { Id = "", Name = "-- no crawler --" });
                return cs;
            }
        }

        public ActionResult Index()
        {
            //return View(db.Crawlers.ToList());
            return View();
        }
        
        public ActionResult TableJson([ModelBinder(typeof(DataTables.AspNet.Mvc5.ModelBinder))] DataTables.AspNet.Core.IDataTablesRequest request)
        {
            JqueryDataTable.Field[] fields = new JqueryDataTable.Field[] {                  
                new JqueryDataTable.Field("Id", false, 1), 
                new JqueryDataTable.Field("Crawler", true, 0, "Id"), 
                new JqueryDataTable.Field("State"),
                new JqueryDataTable.Field("Command"),
                new JqueryDataTable.Field("_SessionStartTime"),
                new JqueryDataTable.Field("_LastSessionState"),
                new JqueryDataTable.Field("_NextStartTime"),
                new JqueryDataTable.Field("_LastStartTime"),
                new JqueryDataTable.Field("_LastEndTime"),
                new JqueryDataTable.Field("RunTimeSpan")
            };
            JsonResult jr = JqueryDataTable.Index(request, db.Connection, "FROM Crawlers", fields);
            foreach (var r in ((dynamic)jr.Data).Data)
            {
                if (!String.IsNullOrEmpty(Convert.ToString(r[2])))
                    r[2] = ((Cliver.CrawlerHost.Crawler.State)Convert.ToInt32(r[2])).ToString();
                if (!String.IsNullOrEmpty(Convert.ToString(r[3])))
                    r[3] = ((Cliver.CrawlerHost.Crawler.Command)Convert.ToInt32(r[3])).ToString();
                if (!String.IsNullOrEmpty(Convert.ToString(r[5])))
                    r[5] = ((Cliver.CrawlerHost.Crawler.SessionState)Convert.ToInt32(r[5])).ToString();
            }
            return jr;
        }      

        public ActionResult Details(string id)
        {
            Crawler crawler = db.Crawlers.Where(r => r.Id == id).FirstOrDefault();
            if (crawler == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView(crawler);
            return View(crawler);
        }

        public ActionResult Create()
        {
            ViewBag.State = new SelectList(StateSelect, "Value", "Name");
            ViewBag.Command = new SelectList(CommandSelect, "Value", "Name");
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Crawler crawler)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Crawlers.InsertOnSubmit(crawler);
                    db.SubmitChanges();
                    if (Request.IsAjaxRequest())
                        return Content(null);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                var g = ((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors;
            }
            if (Request.IsAjaxRequest())
                return PartialView(crawler);
            return View(crawler);
        }

        public ActionResult Edit(string id)
        {
            Crawler crawler = db.Crawlers.Where(r => r.Id == id).FirstOrDefault();
            if (crawler == null)
            {
                return HttpNotFound();
            }
            ViewBag.State = new SelectList(StateSelect, "Value", "Name", crawler.State);
            ViewBag.Command = new SelectList(CommandSelect, "Value", "Name", crawler.Command);
            if (Request.IsAjaxRequest())
                return PartialView(crawler);
            return View(crawler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Crawler crawler)
        {
            if (ModelState.IsValid)
            {
                Crawler c = db.Crawlers.Where(r => r.Id == crawler.Id).First();
                c.AdminEmails = crawler.AdminEmails;
                c.Command = crawler.Command;
                c.Comment = crawler.Comment;
                c.CrawlProductTimeout = crawler.CrawlProductTimeout;
                c.RestartDelayIfBroken = crawler.RestartDelayIfBroken;
                c.RunTimeSpan = crawler.RunTimeSpan;
                c.Site = crawler.Site;
                c.State = crawler.State;
                c.YieldProductTimeout = crawler.YieldProductTimeout;                
                db.SubmitChanges();
                if (Request.IsAjaxRequest())
                    return Content(null);
                return RedirectToAction("Index");
            }
            ViewBag.State = new SelectList(StateSelect, "Value", "Name", crawler.State);
            ViewBag.Command = new SelectList(CommandSelect, "Value", "Name", crawler.Command);
            if (Request.IsAjaxRequest())
                return PartialView(crawler);
            return View(crawler);
        }

        public ActionResult Delete(string id)
        {
            Crawler crawler = db.Crawlers.Where(r => r.Id == id).FirstOrDefault();
            if (crawler == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView(crawler);
            return View(crawler);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Crawler crawler = db.Crawlers.Where(r => r.Id == id).FirstOrDefault();
            db.Crawlers.DeleteOnSubmit(crawler);
            db.SubmitChanges();
            if (Request.IsAjaxRequest())
                return Content(null);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
