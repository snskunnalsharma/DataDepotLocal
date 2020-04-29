using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.UI.CustomHelpers;
using KSPL.Indus.MSTS.UI.Filters;
using log4net;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class UploadCityLogoController : Controller
    {

        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(CMS_MobileService));
        #endregion


        // GET: Admin/UploadCityLogo
        public ActionResult Index()
        {
            return View();
        }
        // Get and bind records todata table
        public ActionResult GetUploadedImageData()
        {
            List<UploadedImageData> dataList = new List<UploadedImageData>();
            try
            {

                dataList = new UploadCityLogoDAL().GetUploadedImagRecords();

            }
            catch (Exception ex)
            {
                // throw ex;
                return Json(dataList, JsonRequestBehavior.AllowGet);
            }

            return Json(dataList, JsonRequestBehavior.AllowGet);

        }



        /// <summary>
        /// Create record
        /// </summary>
        /// <param name="cityLogoId"></param>
        /// <returns></returns>
        public ActionResult CreateRecord()
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("UploadCityLogo", "Add");
            var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";

            return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Edit Operation
        /// </summary>
        /// <param name="cityLogoId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetLogoImageRecord(long cityLogoId)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("UploadCityLogo", "Edit");
            var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
            if (objPermissionCheck == true)
            {

                try
                {
                    var Obj = new UploadCityLogoDAL().GetData(cityLogoId);
                    if (!string.IsNullOrEmpty(Obj.strImagePath))
                    {
                        string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
                        Obj.strImagePath = _webUrl + "/Uploads/UploadCityLogo/" + Obj.ImageName + "/" + Obj.strImagePath; ;
                    }
                    else
                    {
                        Obj.strImagePath = "https://www.placehold.it/200x150/EFEFEF/AAAAAA&amp;text=no+image";
                    }

                    return Json(new { status = true, Obj = Obj }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, returnMessage = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }




        /// <summary>
        /// Deactivate operation 
        /// </summary>
        /// <param name="cityLogoId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeactivateCityLogo(long cityLogoId)
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("UploadCityLogo", "Delete");
            var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
            if (objPermissionCheck == true)
            {

                bool result = false;
                try
                {
                    long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    result = new UploadCityLogoDAL().DeactiveRecord(cityLogoId, currentUserId);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadFile()
        {
            UploadCityLogoDAL _objEmployeeDAL = new UploadCityLogoDAL();
            bool value = false;
            bool checkName = false;
            string result = "We are unable to process your request. Please try again.";
            string imagePath = "";
            try
            {
                long editCityLogoId = Convert.ToInt64(Request.Headers["editCityLogoId"]);

                if (editCityLogoId != 0)
                {
                    string fileName = Request.Headers["X-File-Name"];
                    string fileType = Request.Headers["X-File-Type"];
                    int fileSize = int.Parse(Request.Headers["X-File-Size"]);
                    long StateID = Convert.ToInt64(Request.Headers["stateid"]);
                    long CityID = Convert.ToInt64(Request.Headers["cityid"]);
                    long customerId = Convert.ToInt64(Request.Headers["customerId"]);

                    string imageName1 = Request.Headers["imageName"];

                    var Filepath = "~/Uploads/UploadCityLogo/" + imageName1;
                    if (System.IO.File.Exists(Server.MapPath(Filepath)))
                    { System.IO.File.Delete(Filepath); }

                    //Create again
                    bool IsExist = System.IO.Directory.Exists(Server.MapPath(Filepath));
                    if (!IsExist)
                        System.IO.Directory.CreateDirectory(Server.MapPath(Filepath));

                    System.IO.FileStream fileStream = System.IO.File.Create(Server.MapPath(Filepath + "/" + fileName));
                    System.IO.Stream fileContent = Request.InputStream;
                    fileContent.Seek(0, System.IO.SeekOrigin.Begin);
                    fileContent.CopyTo(fileStream);
                    fileStream.Dispose();
                    log.Info("Company Logo Path at - " + DateTime.Now.ToString() + " - " + (Server.MapPath(Filepath + "/" + fileName).ToString()));

                    int loginId = (int)new UserDetails().GetUserId(User.Identity.GetUserId());

                    value = _objEmployeeDAL.InsertIntoTblStateCityLogo(Filepath, StateID, CityID, customerId, loginId, fileName, imageName1, editCityLogoId);
                    string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
                    imagePath = (Filepath + "/" + fileName).Replace("~/", _webUrl);
                    result = value == true ? "File uploaded successfully." : "File is not  uploaded successfully.";
                }
                else
                {
                    string imageName = Request.Headers["imageName"];
                    long StateID = Convert.ToInt64(Request.Headers["stateid"]);
                    long CityID = Convert.ToInt64(Request.Headers["cityid"]);
                    long customerId = Convert.ToInt64(Request.Headers["customerId"]);


                    checkName = new UploadCityLogoDAL().CheckRecord(CityID, customerId);

                    if (checkName == false)
                    {
                        result = "Save failed. Logo image for the customer & city already present.";
                        return Json(new { success = checkName, Data1 = result }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string fileName = Request.Headers["X-File-Name"];
                        string fileType = Request.Headers["X-File-Type"];
                        int fileSize = int.Parse(Request.Headers["X-File-Size"]);



                        var Filepath = "~/Uploads/UploadCityLogo/" + imageName;
                        bool exists = System.IO.Directory.Exists(Server.MapPath(Filepath));
                        if (!exists)
                            System.IO.Directory.CreateDirectory(Server.MapPath(Filepath));

                        System.IO.FileStream fileStream = System.IO.File.Create(Server.MapPath(Filepath + "/" + fileName));
                        System.IO.Stream fileContent = Request.InputStream;
                        fileContent.Seek(0, System.IO.SeekOrigin.Begin);
                        fileContent.CopyTo(fileStream);
                        fileStream.Dispose();
                        log.Info("Company Logo Path at - " + DateTime.Now.ToString() + " - " + (Server.MapPath(Filepath + "/" + fileName).ToString()));

                        int loginId = (int)new UserDetails().GetUserId(User.Identity.GetUserId());

                        value = _objEmployeeDAL.InsertIntoTblStateCityLogo(Filepath, StateID, CityID, customerId, loginId, fileName, imageName, 0);
                        string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
                        imagePath = (Filepath + "/" + fileName).Replace("~/", _webUrl);
                        result = value == true ? "File uploaded successfully." : "File is not  uploaded successfully.";

                    }
                }

                return Json(new { success = value, Data1 = result, ImagePath = imagePath }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, Data1 = ex.Message }, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult GetStateList()
        {

            using (UploadCityLogoDAL obj = new UploadCityLogoDAL())
            {
                var objList1 = obj.GetActiveStateRecords();

                return Json(objList1, JsonRequestBehavior.AllowGet);
            }

        }




        public ActionResult GetCityList(long stateId)
        {

            using (UploadCityLogoDAL obj = new UploadCityLogoDAL())
            {
                var objList1 = obj.GetActiveCityRecords(stateId);

                return Json(objList1, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetCustomerList()
        {

            using (UploadCityLogoDAL obj = new UploadCityLogoDAL())
            {
                var objList1 = obj.GetActiveCustomerRecords();

                return Json(objList1, JsonRequestBehavior.AllowGet);
            }

        }


    }
}