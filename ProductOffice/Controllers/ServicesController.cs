using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cliver.ProductOffice.Models;

namespace Cliver.ProductOffice.Controllers
{
    [Authorize]
    public class ServicesController : Controller
    {
        private CrawlerHostDataContext chdc = new CrawlerHostDataContext(Cliver.CrawlerHost.DbApi.ConnectionString);
   
        List<object> StateSelect
        {
            get            
            {
                var ss = (from v in (Cliver.CrawlerHost.Service.State[])Enum.GetValues(typeof(Cliver.CrawlerHost.Service.State)) select new { Name = v.ToString(), Value = (int)v }).ToList<object>();                
                return ss;
            }
        }
           
        List<object> CommandSelect
        {
            get            
            {
                var cs = (from v in (Cliver.CrawlerHost.Service.Command[])Enum.GetValues(typeof(Cliver.CrawlerHost.Service.Command)) select new { Name = v.ToString(), Value = (int)v }).ToList<object>();
                //cs.Insert(0, new { Id = "", Name = "-- no service --" });
                return cs;
            }
        }

        public ActionResult Index()
        {
            //return View(chdc.Services.ToList());
            return View();
        }
        
        public ActionResult TableJson([ModelBinder(typeof(DataTables.AspNet.Mvc5.ModelBinder))] DataTables.AspNet.Core.IDataTablesRequest request)
        {
            JqueryDataTable.Field[] fields = new JqueryDataTable.Field[] {                  
                new JqueryDataTable.Field("Id", false, 1), 
                new JqueryDataTable.Field("Service", true, 0, "Id"), 
                new JqueryDataTable.Field("State"),
                new JqueryDataTable.Field("Command"),
                new JqueryDataTable.Field("_LastSessionState"),
                new JqueryDataTable.Field("_NextStartTime"),
                new JqueryDataTable.Field("_LastStartTime"),
                new JqueryDataTable.Field("_LastEndTime"),
                new JqueryDataTable.Field("RunTimeSpan")
            };
            JsonResult jr = JqueryDataTable.Index(request, chdc.Connection, "FROM Services", fields);
            foreach (var r in ((dynamic)jr.Data).Data)
            {
                if (!String.IsNullOrEmpty(Convert.ToString(r[2])))
                    r[2] = ((Cliver.CrawlerHost.Service.State)Convert.ToInt32(r[2])).ToString();
                if (!String.IsNullOrEmpty(Convert.ToString(r[3])))
                    r[3] = ((Cliver.CrawlerHost.Service.Command)Convert.ToInt32(r[3])).ToString();
                if (!String.IsNullOrEmpty(Convert.ToString(r[4])))
                    r[4] = ((Cliver.CrawlerHost.Service.SessionState)Convert.ToInt32(r[4])).ToString();
            }
            return jr;
        }  

        public ActionResult Details(string id)
        {
            Service service = chdc.Services.Where(r => r.Id == id).FirstOrDefault();
            if (service == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView(service);
            return View(service);
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
        public ActionResult Create(Service service)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (service._NextStartTime < DateTime.Now)
                        service._NextStartTime = DateTime.Now;
                    chdc.Services.InsertOnSubmit(service);
                    chdc.SubmitChanges();
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
                return PartialView(service);
            return View(service);
        }

        public ActionResult Edit(string id)
        {
            Service service = chdc.Services.Where(r => r.Id == id).FirstOrDefault();
            if (service == null)
            {
                return HttpNotFound();
            }
            ViewBag.State = new SelectList(StateSelect, "Value", "Name", service.State);
            ViewBag.Command = new SelectList(CommandSelect, "Value", "Name", service.Command);
            if (Request.IsAjaxRequest())
                return PartialView(service);
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Service service)
        {
            if (ModelState.IsValid)
            {
                Service s = chdc.Services.Where(r => r.Id == service.Id).First();
                //s.Id = service.Id;
                s.AdminEmails = service.AdminEmails;
                s.Command = service.Command;
                s.Comment = service.Comment;
                s.RunTimeout = service.RunTimeout;
                s.RunTimeSpan = service.RunTimeSpan;
                s.RestartDelayIfBroken = service.RestartDelayIfBroken;
                s.ExeFolder = service.ExeFolder;
                s.State = service.State;
                chdc.SubmitChanges();
                if (Request.IsAjaxRequest())
                    return Content(null);
                return RedirectToAction("Index");
            }
            ViewBag.State = new SelectList(StateSelect, "Value", "Name", service.State);
            ViewBag.Command = new SelectList(CommandSelect, "Value", "Name", service.Command);
            if (Request.IsAjaxRequest())
                return PartialView(service);
            return View(service);
        }

        public ActionResult Delete(string id)
        {
            Service service = chdc.Services.Where(r => r.Id == id).FirstOrDefault();
            if (service == null)
            {
                return HttpNotFound();
            }
            if (Request.IsAjaxRequest())
                return PartialView(service);
            return View(service);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Service service = chdc.Services.Where(r => r.Id == id).FirstOrDefault();
            chdc.Services.DeleteOnSubmit(service);
            chdc.SubmitChanges();
            if (Request.IsAjaxRequest())
                return Content(null);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            chdc.Dispose();
            base.Dispose(disposing);
        }
    }
}
