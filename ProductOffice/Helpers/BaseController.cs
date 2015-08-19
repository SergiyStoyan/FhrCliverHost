using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.Entity;
using Cliver.ProductOffice.Models;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Data.Common;
using System.Collections.Specialized;

namespace Cliver.ProductOffice
{
    public class BaseController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            if (Request.IsAjaxRequest())
            {
                filterContext.ExceptionHandled = true;
                Exception e = filterContext.Exception;
                filterContext.Result = Content(Cliver.Bot.Log.GetExceptionMessage(e));
            }
        }
    }
}