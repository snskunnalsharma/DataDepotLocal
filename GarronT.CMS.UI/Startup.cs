using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GarronT.CMS.UI.Startup))]
namespace GarronT.CMS.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
