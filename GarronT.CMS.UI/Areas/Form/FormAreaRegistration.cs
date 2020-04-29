using System.Web.Mvc;

namespace GarronT.CMS.UI.Areas.Form
{
    public class FormAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Form";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Form_default",
                "Form/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );



         //   context.MapRoute(
         //      name: "AngularCatchAllRoute",
         //      url: "Form/{*.}",
         //      defaults: new { controller = "FormManagement", action = "Index", id = UrlParameter.Optional }
         //);
        }
    }
}