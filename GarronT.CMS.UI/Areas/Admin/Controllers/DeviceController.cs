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
    public class DeviceController : Controller
    {
        // GET: Admin/Device
        public ActionResult Index()
        {
            return View();
        }

        List<DeviceMasterModel> list = new List<DeviceMasterModel>();

        public ActionResult GetDeviceList()
        {

            DeviceMasterModel obj1 = new DeviceMasterModel();
            obj1.DeviceId = 1;
            obj1.Device = "ALV-HH01";
            list.Add(obj1);

            DeviceMasterModel obj2 = new DeviceMasterModel();
            obj2.DeviceId = 2;
            obj2.Device = "ALV-HH02";
            list.Add(obj2);

            DeviceMasterModel obj3 = new DeviceMasterModel();
            obj3.DeviceId = 3;
            obj3.Device = "ALV-HH03";
            list.Add(obj3);

            DeviceMasterModel obj4 = new DeviceMasterModel();
            obj4.DeviceId = 4;
            obj4.Device = "ALV-HH04";
            list.Add(obj4);
            DeviceMasterModel obj5 = new DeviceMasterModel();
            obj5.DeviceId = 5;
            obj5.Device = "ALV-HH05";
            list.Add(obj5);
            DeviceMasterModel obj6 = new DeviceMasterModel();
            obj6.DeviceId = 6;
            obj6.Device = "06";
            list.Add(obj6);

            TempData["DeviceMasterList"] = list;
            return Json(list, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Edit(long editId)
        {
            list = (List<DeviceMasterModel>)TempData["DeviceMasterList"];
            DeviceMasterModel obj1 = list.Where(o => o.DeviceId == editId).FirstOrDefault();
            TempData.Keep("DeviceMasterList");
            return Json(obj1, JsonRequestBehavior.AllowGet);

        }
    }
}