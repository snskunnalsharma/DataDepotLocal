using GarronT.CMS.Model.BO;
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
    public class CityController : Controller
    {
        // GET: Admin/City
        public ActionResult Index()
        {
            return View();
        }

      
        public ActionResult GetCityList()
        {
             var list = new List<CityModel>();
             CityModel obj1 = new CityModel();
             obj1.CountryName = "US";
             obj1.StateName = "New York";
             obj1.CityName = "New York City";
             list.Add(obj1);

             CityModel obj2 = new CityModel();
             obj2.CountryName = "US";
             obj2.StateName = "California";
             obj2.CityName = "Los Angeles";
             list.Add(obj2);

             CityModel obj3 = new CityModel();
             obj3.CountryName = "US";
             obj3.StateName = "Illinois";
             obj3.CityName = "Chicago";
             list.Add(obj3);

            return Json(list, JsonRequestBehavior.AllowGet);

        }
    }
}