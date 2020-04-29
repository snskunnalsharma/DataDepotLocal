using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using KSPL.Indus.MSTS.UI.Filters;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class EmployeeCardController : Controller
    {
        // GET: Admin/EmployeeCard
        //public ActionResult Index()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult UploadFile()
        //{
        //    EmployeeCardDAL _objEmployeeDAL = new EmployeeCardDAL();
        //    bool value = false;
        //    string result = "We are unable to process your request.Please try again.";
        //    try
        //    {
        //        //string fileName = Request.Headers["X-File-Name"];
        //        //string fileType = Request.Headers["X-File-Type"];
        //        // int fileSize = Convert.ToInt32(Request.Headers["X-File-Size"]);
        //        //long StateID = Convert.ToInt64(Request.Headers["X-File-State"]);
        //        //long CityID = Convert.ToInt64(Request.Headers["X-File-City"]);
        //        //File's content is available in Request.InputStream property
        //        var currentDatetime = new CommonFunctions().ServerDate().ToString("Mddyyhhmmss");
        //        string fileName = Request.Headers["X-File-Name"];
        //        string fileType = Request.Headers["X-File-Type"];
        //        int fileSize = int.Parse(Request.Headers["X-File-Size"]);
        //        long StateID = Convert.ToInt64(Request.Headers["stateid"]);
        //        long CityID = Convert.ToInt64(Request.Headers["cityid"]);
        //        System.IO.Stream fileContent = Request.InputStream;

        //        var Filepath = "~/Uploads/UploadCityLogo/" ;//+ currentDatetime;
        //        bool exists = System.IO.Directory.Exists(Server.MapPath(Filepath));
        //        if (!exists)
        //            System.IO.Directory.CreateDirectory(Server.MapPath(Filepath));
        //        Filepath = Filepath + "/" + fileName;
        //        System.IO.FileStream fileStream = System.IO.File.Create(Server.MapPath("~/Uploads/UploadCityLogo/") + fileName);



        //       // System.IO.FileStream fileStream = System.IO.File.Create(Server.MapPath("~/Uploads/UploadCityLogo/")+ fileName);
        //        fileContent.Seek(0, System.IO.SeekOrigin.Begin);
        //        fileContent.CopyTo(fileStream);
        //        fileStream.Dispose();
        //        var path = Server.MapPath(Filepath);
        //        long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
        //        int loginId = (int)currentUserId;
        //        value = _objEmployeeDAL.InsertIntoTblStateCityLogo(path, StateID, CityID, loginId, fileName);
        //        if (value == true)
        //        {
        //            result = "File uploaded successfully.";
        //        }             


        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    //return Json(new { msg = value, returnMessage = result }, JsonRequestBehavior.AllowGet);
        //    return View();
        //}
      
        //public ActionResult GetStateList()
        //{
        //    List<StateVM> objList1 = new List<StateVM>();
        //    using (EmployeeCardDAL obj = new EmployeeCardDAL())
        //    {

        //        objList1 = obj.GetActiveStateRecords();
        //    }

        //    return Json(objList1, JsonRequestBehavior.AllowGet);
        //}
       
        //public ActionResult GetCityList(long stateId)
        //{
        //    List<CityVM> objList1 = new List<CityVM>();
        //    //objList1=
        //    using (EmployeeCardDAL obj = new EmployeeCardDAL())
        //    {
        //        objList1 = obj.GetActiveCityRecords(stateId);
        //    }

        //    return Json(objList1, JsonRequestBehavior.AllowGet);
        //}

    }

   
}