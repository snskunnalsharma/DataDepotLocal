using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KSPL.Indus.MSTS.UI.Filters
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {


            // check  sessions here
            //if (HttpContext.Current.Session["Username"] == null)
            //{
            //    filterContext.Result = new RedirectResult("~/Global/Login");
            //    return;
            //}


            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var sessions = filterContext.HttpContext.Session;
                if (sessions["Username"] != null)
                {
                    return;
                }
                else
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            status = "401"
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };

                    //xhr status code 401 to redirect
                    filterContext.HttpContext.Response.StatusCode = 401;

                    return;
                }
            }

            var session = filterContext.HttpContext.Session;
            if (session["Username"] != null)
                return;

            //Redirect to login page.
            //var redirectTarget = new RouteValueDictionary { { "action", "Index" }, { "controller", "Login" } };
            //filterContext.Result = new RedirectToRouteResult(redirectTarget);
            filterContext.Result = new RedirectResult("~/Global/Login");


            base.OnActionExecuting(filterContext);
        }
    }


}