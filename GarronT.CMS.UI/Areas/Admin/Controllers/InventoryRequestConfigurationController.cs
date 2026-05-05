using KSPL.Indus.MSTS.UI.Filters;
using System.Web.Mvc;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class InventoryRequestConfigurationController : Controller
    {
        public ActionResult Index()
        {
            return HttpNotFound();
        }
    }
}

