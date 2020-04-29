using KSPL.Indus.MSTS.UI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GarronT.CMS.UI.Areas.Installer.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [Authorize]
     [SessionExpire]
    public class InstallerDashboardController : Controller
    {
        //
        // GET: /Installer/InstallerDashboard/
        public ActionResult Index()
        {
            return View();
        }
	}
}