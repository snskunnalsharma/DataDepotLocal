using System.Web.Http;

namespace GarronT.CMS.UI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enable attribute routing
            config.MapHttpAttributeRoutes();

            // Route with action name
            config.Routes.MapHttpRoute(
                name: "DefaultApiWithAction",
                routeTemplate: "Api/{controller}/{action}"
            );

            // Default REST-style route
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}