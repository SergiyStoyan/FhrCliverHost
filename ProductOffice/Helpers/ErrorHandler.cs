using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.Entity;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Data.Common;
using System.Collections.Specialized;

namespace Cliver.ProductOffice
{
    public class DatatableException : Exception
    {
        public DatatableException(string message) : base(message) { }
    }

    public class HandleActionError : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Controller is System.Web.Mvc.Controller)
            {
                Controller c = (Controller)context.Controller;
                if (context.Exception is DatatableException)
                {
                    context.HttpContext.Response.Write(context.Exception.Message);
                    context.HttpContext.Response.Flush();
                    context.ExceptionHandled = true;
                }
                //                if (c.Request.IsAjaxRequest())
                //                {
                //                    context.ExceptionHandled = true;
                //                    Exception e = context.Exception;
                //                    for (; e.InnerException != null; e = e.InnerException) ;
                //                    string message = "Exception: <br>" + e.Message;
                //#if DEBUG
                //                    message += "<br><hr>Module:<br>" + e.TargetSite.Module + "<br><br>Stack:<br>" + e.StackTrace;
                //#else
                //#endif
                //                    Errors.Add(message);
                //                    context.Result = new ViewResult { ViewName = "_Notifications" };
                //                    c.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest; 
                //                }
            }
        }
    }
}