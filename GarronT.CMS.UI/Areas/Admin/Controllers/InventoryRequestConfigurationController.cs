using KSPL.Indus.MSTS.UI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class InventoryRequestConfigurationController : Controller
    {
        //
        // GET: /Admin/InventoryRequestConfiguration/
        public ActionResult Index()
        {
            return View();
        }
	}
}