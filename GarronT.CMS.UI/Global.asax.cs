using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GarronT.CMS.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = false;
            HtmlHelper.ClientValidationEnabled = true;
            HtmlHelper.UnobtrusiveJavaScriptEnabled = true;
            string l4net = Server.MapPath("~/log4Net.config");
            log4net.Config.XmlConfigurator.Configure();

            //var roleStore = new RoleStore<IdentityRole>();
            //var roleManager = new RoleManager<IdentityRole>(roleStore);
            //if (!roleManager.RoleExists("Super Admin"))
            //{
            //    var roleresult = roleManager.Create(new IdentityRole("Super Admin"));
            //}
        }


        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }

        protected void Application_EndRequest()
        {
            var context = new HttpContextWrapper(Context);
            if (context.Request.IsAjaxRequest() && context.Response.StatusCode == 401)
            {
                Context.Response.Clear();
                Context.Response.Write("**custom error message**");
                Context.Response.StatusCode = 306;
            }



        }

        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(MvcApplication));
        #endregion


        protected void Application_Error(object sender, EventArgs e)
        {

            try
            {

                Exception exception = Server.GetLastError();

                Response.Clear();

                HttpException httpException = exception as HttpException;

                if (httpException != null)
                {
                    log.Info("ERROR  ex.Message - " + (httpException.Message != null ? httpException.Message : ""));
                    log.Info("ERROR  ex.Message - " + (httpException.InnerException != null ? httpException.InnerException.ToString() : ""));
                    string action;

                    switch (httpException.GetHttpCode())
                    {
                        case 404:
                            // page not found
                            action = "HttpError404";
                            break;
                        case 500:
                            // server error
                            action = "HttpError500";
                            break;
                        default:
                            action = "General";
                            break;
                    }

                    // clear error on server
                    Server.ClearError();

                    Response.Redirect(String.Format("~/Error/Index"));
                }
            }
            catch
            {

                // throw;
            }


        }




    }

    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
    }
}
