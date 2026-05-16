using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GarronT.CMS.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            RouteTable.Routes.RouteExistingFiles = false;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("{*x}", new { x = @".*\.asmx(/.*)?" });
           // routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");
            //routes.RouteExistingFiles = true;
            
            routes.MapRoute(
             name: "FormList",
             url: "FormManagement",
             defaults: new { controller = "FormManagement", action = "Index", id = UrlParameter.Optional }
             );

            routes.MapRoute(
            name: "FormAdd",
            url: "FormManagement/Add",
            defaults: new { controller = "FormManagement", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "FormEdit",
            url: "FormManagement/Edit/{id}",
            defaults: new { controller = "FormManagement", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
           name: "Formview",
           url: "FormManagement/View/{id}",
           defaults: new { controller = "FormManagement", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
           name: "FormClone",
           url: "FormManagement/Clone",
           defaults: new { controller = "FormManagement", action = "Index", id = UrlParameter.Optional }
           );



            routes.MapRoute(
             name: "Activation",
             url: "{controller}/{action}/{id}",
             defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
         ).DataTokens = new RouteValueDictionary(new { area = "Global" });

            routes.MapRoute(
              name: "Default",
              url: "{controller}/{action}/{id}",
              defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
          );
            // AreaRegistration.RegisterAllAreas();



            // routes.MapRoute(
            //      name: "AngularCatchAllRoute",
            //      url: "FormManagement/{*.}",
            //      defaults: new { controller = "FormManagement", action = "Index", id = UrlParameter.Optional }
            //);

            // routes.AppendTrailingSlash = true;


        }
    }
}
