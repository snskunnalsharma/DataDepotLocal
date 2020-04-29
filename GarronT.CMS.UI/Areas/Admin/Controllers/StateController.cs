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
    public class StateController : Controller
    {
        // GET: Admin/State
        public ActionResult Index()
        {
            return View();
        }

        List<StateModel> list = new List<StateModel>();

        public ActionResult GetStateList()
        {

            StateModel obj1 = new StateModel();
            obj1.StateID = 1;
            obj1.CountryID = 1;
            obj1.CountryName = "US";
            obj1.StateName = "Alabama";
            list.Add(obj1);

            StateModel obj2 = new StateModel();
            obj2.StateID = 2;
            obj2.CountryID = 1;
            obj2.CountryName = "US";
            obj2.StateName = "Arizona";
            list.Add(obj2);

            StateModel obj3 = new StateModel();
            obj3.StateID = 3;
            obj3.CountryID = 1;
            obj3.CountryName = "US";
            obj3.StateName = "Mississippi"; 
            list.Add(obj3);

            StateModel obj4 = new StateModel();
            obj4.StateID = 4;
            obj4.CountryID = 1;
            obj4.CountryName = "US";
            obj4.StateName = "New York";
            list.Add(obj4);
            StateModel obj5 = new StateModel();
            obj5.StateID = 5;
            obj5.CountryID = 1;
            obj5.CountryName = "US";
            obj5.StateName = "Texas";
            list.Add(obj5);
            StateModel obj6 = new StateModel();
            obj6.StateID = 6;
            obj6.CountryID = 1;
            obj6.CountryName = "US";
            obj6.StateName = "Washington";
            list.Add(obj6);

            TempData["StateMasterList"] = list;
            return Json(list, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Edit(long editId)
        {
            list = (List<StateModel>)TempData["StateMasterList"];
            StateModel obj1 = list.Where(o => o.StateID == editId).FirstOrDefault();
            TempData.Keep("StateMasterList");
            return Json(obj1, JsonRequestBehavior.AllowGet);

        }
    }
}