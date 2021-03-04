using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using OfficeOpenXml;
using GarronT.CMS.Model;
using System.Collections;
using Microsoft.AspNet.Identity;
using GarronT.CMS.UI.Models;
using KSPL.Indus.MSTS.UI.Filters;
using GarronT.CMS.UI.WebAPI;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using log4net;
using System.Web.Script.Serialization;
using GarronT.CMS.UI.CustomHelpers;


namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class ProjectController : Controller
    {
        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(CMS_MobileService));
        #endregion


        // GET: Admin/Project
        public ActionResult Index()
        {
            DateTime dt = DateTime.Now;
            //decimal a = decimal.Parse("33.38944800");
            //decimal b = decimal.Parse("-96.78510600");
            //MobileDAL objMobile = new MobileDAL();
            //var result = objMobile.addressByLatLon(19, 2, a, b, dt, dt);


            // MobileDAL objMobile = new MobileDAL();
            //var result = objMobile.GetTodaysMapSectionVistByUserIdNew(19, dt, dt);
            //var result = objMobile.GetTodaysMenuSectionVistByUserId(19, dt, dt);


            //MobileDAL objMobile = new MobileDAL();
            //var result = objMobile.getFormListDataNew(19, 2, 122);

            //MobileDAL objMobile = new MobileDAL();
            //var result = objMobile.getFormListDataNew(14, 12, 3464);

            TempData["ProjectId"] = null;
            return View();
        }

        public ActionResult getActiveOrDeactiveProjectList(bool ActiveStatus)
        {
            ProjectMasterDAL obj = new ProjectMasterDAL();

            List<ProjectModel> objList = obj.GetProjectList(ActiveStatus);

            var checkAdmin = Session["CurrentUserIsAdmin"].ToString();

            if (checkAdmin == "0")
            {
                var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var abcd = new ProjectMasterDAL().GetUserProjectList(userId);


                if (abcd == null || abcd.Count == 0)
                {
                    if (objList != null && objList.Count() > 0)
                    {
                        objList = objList.Take(0).ToList();
                    }
                }
                else
                {
                    if (objList != null && objList.Count() > 0)
                    {
                        objList = objList.Where(o => abcd.Contains(o.ProjectId)).ToList();
                    }
                }

            }


            return Json(objList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Project", "Add");

            if (objPermissionCheck)
            {
                ViewBag.Form = "Create";
                return View();
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpGet]
        public ActionResult Edit(long ProjectId)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Project", "Edit");

            if (objPermissionCheck)
            {
                TempData["ProjectId"] = null;
                TempData["ProjectId"] = ProjectId.ToString();
                TempData.Keep("ProjectId");
                return View();
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult CloneProject(long CloneId)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Project", "Add");

            if (objPermissionCheck)
            {
                TempData["CloneId"] = null;
                TempData["CloneId"] = CloneId.ToString();
                TempData.Keep("CloneId");
                return RedirectToAction("Create");
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult getRowPartial()
        {
            return PartialView("_SingleText");
        }


        #region tab 1 methods


        /// <summary>
        /// gets project details for the current project
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProjectDetails()
        {
            ProjectModel obj1 = new ProjectModel();
            if (TempData["ProjectId"] != null)
            {
                long ProjectId = long.Parse(TempData["ProjectId"].ToString());
                TempData.Keep("ProjectId");
                using (ProjectMasterDAL obj = new ProjectMasterDAL())
                {
                    obj1 = obj.GetProjectDetail(ProjectId);
                    if (obj1 == null)
                    {
                        obj1 = new ProjectModel();
                        obj1.ProjectId = 0;
                    }
                }
            }
            else if (TempData["CloneId"] != null)
            {
                long ProjectId = long.Parse(TempData["CloneId"].ToString());
                TempData.Keep("CloneId");
                using (ProjectMasterDAL obj = new ProjectMasterDAL())
                {
                    obj1 = obj.GetProjectDetail(ProjectId);
                    if (obj1 == null)
                    {
                        obj1 = new ProjectModel();
                        obj1.ProjectId = 0;
                    }
                    else
                    {
                        obj1.CloneId = ProjectId;
                        obj1.ProjectName = "Clone " + obj1.ProjectName;
                    }
                }
            }
            else
            {
                using (ProjectMasterDAL obj = new ProjectMasterDAL())
                {
                    obj1.ProjectId = 0;
                    obj1.objStateList = obj.GetActiveStateRecords();
                    obj1.objCityList = new List<CityModel>();
                    obj1.objClientModelList = obj.GetActiveClientRecords();
                    obj1.objContactModelDDLList = new List<ContactModelDDL>();
                    obj1.objMobileWarehouseBOList = new List<MobileWarehouseBO>();//obj.GetWarehouseList();


                    obj1.objMeterMakeModelList = obj.getMeterMakeList();
                    obj1.objMeterSizeList = obj.getMetersizeList();
                    obj1.objMeterTypeList = obj.getMeterTypeList();
                    obj1.objServiceList = obj.getServiceList();
                    obj1.objUsersSmallModelList = obj.GetUserList();

                }
            }
            return Json(obj1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWarehouseList()
        {
            List<MobileWarehouseBO> objList1 = new List<MobileWarehouseBO>();
            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                objList1 = obj.GetWarehouseList();
            }

            return Json(objList1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCityList(long stateId, string cityName)
        {
            List<CityModel> objList1 = new List<CityModel>();

            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                objList1 = obj.GetActiveCityRecords(stateId, "");
            }

            return Json(objList1, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetContactList(long ClientId)
        {
            List<ContactModelDDL> objList1 = new List<ContactModelDDL>();

            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                objList1 = obj.GetActiveContactRecords(ClientId);
            }

            return Json(objList1, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveTab1(ProjectModel objProject)
        {
            try
            {
                int currentUserId = (int)new UserDetails().GetUserId(User.Identity.GetUserId());
                string returnMessage = "";
                bool success = false;


                objProject.CreatedBy = currentUserId;
                objProject.CreatedOn = new CommonFunctions().ServerDate();
                objProject.Active = 1;

                long proId = 0;
                success = new ProjectMasterDAL().CreateOrUpdate(objProject, out proId, out returnMessage);

                return Json(new { success = success, returnMessage = returnMessage, proId = proId }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return View();
            }
        }


        #endregion

        #region tab 2 methods



        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="objMeterSizeList"></param>
        /// <param name="objMeterTypeList"></param>
        /// <param name="objServiceList"></param>
        /// <param name="objInstallerList"></param>
        /// <returns></returns>
        public ActionResult SaveTab2Data(long projectId, string[] objMeterSizeList, string[] objMeterTypeList, string[] objServiceList, string[] objInstallerList, string[] objMakeList)
        {
            List<long> objList1 = new List<long>();
            string result;
            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {

                int currentUserId = (int)new UserDetails().GetUserId(User.Identity.GetUserId());


                bool a = obj.CreateOrUpdateMasterField(1, projectId, currentUserId, objMeterSizeList, out result);

                bool b = obj.CreateOrUpdateMasterField(2, projectId, currentUserId, objMeterTypeList, out result);

                bool c = obj.CreateOrUpdateMasterField(3, projectId, currentUserId, objServiceList, out result);

                bool e = obj.CreateOrUpdateMasterField(4, projectId, currentUserId, objMakeList, out result);

                bool d = new UserRoleDAL().CreateOrUpdateMasterField(projectId, currentUserId, objInstallerList, out result);
                return Json(new { success = a, returnMessage = result }, JsonRequestBehavior.AllowGet);

            }

        }

        #endregion

        #region tab 3 Actions

        /// <summary>
        /// Tab 3- gets service list for the current project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns>service list</returns>
        public JsonResult GetTab3Data(long projectId)
        {
            ServiceModel obj1 = new ServiceModel();
            IEnumerable<ServiceSmallModel> objList1 = new List<ServiceSmallModel>();
            IEnumerable<ServiceNotSelectedModel> objList2 = new List<ServiceNotSelectedModel>();
            //List<ProductServiceBo> objList3 = new List<ProductServiceBo>();

            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                objList1 = obj.GetActiveServiceRecords(projectId);
                objList2 = obj.GetInActiveServiceRecords(projectId);
                // objList3 = obj.GetProductServiceList(projectId);

                obj1.objList1 = objList1.ToList();
                obj1.objList2 = objList2.ToList();
                //obj1.objList3 = objList3;
                return Json(obj1, JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult SaveTab3Data(string Objdata)
        {
            IEnumerable<ServiceSmallModel> objList1 = new List<ServiceSmallModel>();
            //public bool SaveTab3Records(long projectId, List<ServiceSmallModel> objList, int UserId)

            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            Tab3BO objTab3BO = new Tab3BO();
            int userId = (int)new UserDetails().GetUserId(User.Identity.GetUserId());
            try
            {
                objTab3BO = s.Deserialize<Tab3BO>(Objdata);

                using (ProjectMasterDAL obj = new ProjectMasterDAL())
                {
                    bool a1 = obj.SaveTab3Records(objTab3BO, userId);
                    return Json(new { success = a1 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }


        }


        [HttpGet]
        public JsonResult GetPictureList(long ServiceId)
        {
            try
            {
                List<string> tempdata = TempData["ObjCurrentProject"] as List<string>;
                ProjectMasterDAL obj = new ProjectMasterDAL();
                List<string> pictureList = obj.GetPictureListData(ServiceId);
                if (pictureList != null && pictureList.Count > 0)
                {
                    string s = "<div class='form-group'> <div class='col-lg-12'> <ul class='task-list list-group list-group_new'>";

                    foreach (var item in pictureList)
                    {
                        s = s + "<li class='list-group-item'> " + item + " </li>";
                    }
                    s = s + "";
                    return Json(new { success = true, pictureData = s }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true, pictureData = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, pictureData = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        #endregion

        #region tab 4 Actions

        public ActionResult GetTab4Data(long ProjectId)
        {
            List<tblStandardFieldMaster> objList = new List<tblStandardFieldMaster>();
            List<tblStandardFieldMaster> objList2 = new List<tblStandardFieldMaster>();
            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                ///get source fields
                objList = obj.GetActiveStandardFields();
                ///get destination fields
                objList2 = obj.GetSelectedStandardFields(ProjectId);
            }

            foreach (var item in objList2)
            {
                var a = objList.Where(o => o.StandardFieldId == item.StandardFieldId).FirstOrDefault();
                objList.Remove(a);
            }

            string sourceFields = "";
            string destinationFields = "";
            int i = 1;

            foreach (var item in objList)
            {
                sourceFields = sourceFields + "<li>" + item.StandardFieldName + "</li>";
                i++;
            }

            foreach (var item in objList2)
            {
                destinationFields = destinationFields + "<li>" + item.StandardFieldName + "</li>";
                i++;
            }
            return Json(new { SourcceDiv = sourceFields, DestinationDiv = destinationFields }, JsonRequestBehavior.AllowGet);
        }





        public ActionResult SaveTab4Data(long projectId, string[] objStandardFieldList)
        {
            List<long> objList1 = new List<long>();
            string result;
            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                bool a = obj.CreateOrUpdateStandardFields(projectId, 1, objStandardFieldList, out result);

                return Json(new { success = a, returnMessage = result }, JsonRequestBehavior.AllowGet);

            }

        }


        #endregion

        #region tab 5 Actions

        /// <summary>
        /// tab 5 - gets dustom field single record
        /// </summary>
        /// <param name="customFieldId"></param>
        /// <returns></returns>
        public ActionResult GetCustomFieldRecord(long customFieldId)
        {
            ProjectMasterDAL obj = new ProjectMasterDAL();

            CustomFieldModel obj1 = obj.GetCustomFieldRecord(customFieldId);
            return Json(obj1, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Tab 5 - Deactivates the custom field
        /// </summary>
        /// <param name="customFieldId"></param>
        /// <returns>true if successful</returns>
        public ActionResult DeactivateCustomFieldRecord(long customFieldId)
        {
            ProjectMasterDAL obj = new ProjectMasterDAL();
            string result;
            int userId = 1;
            bool a = obj.DeactivateCustomFieldRecord(customFieldId, userId, out result);
            return Json(new { success = a, returnMessage = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Tab 5 - Activates the deactivated record of custom field
        /// </summary>
        /// <param name="customFieldId"></param>
        /// <returns>true if successful</returns>
        public ActionResult ActivateCustomFieldRecord(long customFieldId)
        {
            ProjectMasterDAL obj = new ProjectMasterDAL();
            string result;
            int userId = 1;
            bool a = obj.ActivateCustomFieldRecord(customFieldId, userId, out result);
            return Json(new { success = a, returnMessage = result }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// tab 5 - Save the custom field data
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="CustomFieldId"></param>
        /// <param name="CustomFieldName"></param>
        /// <param name="status">1=> Insert 2=>Update </param>
        /// <returns>true=> </returns>
        public ActionResult SaveTab5Data(long projectId, long CustomFieldId, string CustomFieldName, int status)
        {

            string result;
            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                TblProjectCustomeFieldReletion obj1 = new TblProjectCustomeFieldReletion();
                if (status == 1)
                {
                    obj1.CreatedBy = 1;
                    obj1.FieldName = CustomFieldName;
                    obj1.ProjectID = projectId;
                }
                else if (status == 2)
                {
                    obj1.ID = CustomFieldId;
                    obj1.FieldName = CustomFieldName;
                    obj1.ModifiedBy = 1;
                }



                bool a = obj.C_U_D_CustomFields(obj1, status, out result);
                return Json(new { success = a, returnMessage = result }, JsonRequestBehavior.AllowGet);

            }

        }


        /// <summary>
        /// tab 5 - get custom fields for the current project
        /// </summary>
        /// <param name="ActiveStatus"></param>
        /// <param name="projectId"></param>
        /// <returns>list of customfield</returns>
        public ActionResult getTab5Data(bool ActiveStatus, long projectId)
        {
            ProjectMasterDAL obj = new ProjectMasterDAL();

            List<CustomFieldModel> objList = obj.GetCustomFieldRecordList(ActiveStatus, projectId);
            return Json(objList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region tab 6 Action

        public ActionResult SaveTab6Data(long ProjectID, string[] ExcelField, string[] FieldID)
        {

            try
            {
                DataTable dt = new DataTable();
                List<ExcelSheetBO> objobjExcelSheetBOList = new List<ExcelSheetBO>();
                if ((ExcelField != null || ExcelField.Length != 0)
                    && (FieldID != null || FieldID.Length != 0)
                    && ExcelField.Length == FieldID.Length)
                {



                    dt = (DataTable)TempData["ExcelData"];
                    if (dt == null)
                    {
                        return Json(new { success = false, returnMessage = "Submit failed. Please upload excel file first." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        List<VW_GETTab6DropdownData> objData = TempData["FieldMapingView"] as List<VW_GETTab6DropdownData>;
                        int i = 0;
                        foreach (var item in FieldID)
                        {
                            long m = long.Parse(item);

                            var b = objData.Where(x => x.Id == m).Select(o => o).FirstOrDefault();
                            ExcelSheetBO objExcelSheetBO = new ExcelSheetBO();

                            objExcelSheetBO.FieldID = b != null ? b.tabelId : 0;
                            objExcelSheetBO.FieldType = b != null ? b.FieldType : 0;


                            objExcelSheetBO.FieldName = ExcelField[i];

                            objobjExcelSheetBOList.Add(objExcelSheetBO);

                            i++;
                        }

                        TblProjectExcelMapping objTblProjectExcelMapping = new TblProjectExcelMapping();

                        string returnMessage = "";

                        bool success = false;

                        string MSg = "";
                        int UserId = CommonFunctions.GetLoggedUserID(User.Identity.Name);
                        objTblProjectExcelMapping.CreatedBy = UserId;
                        objTblProjectExcelMapping.Active = 1;

                        DataTable dt1;
                        string fileName = "";
                        bool isLatlongCalucalted = false;


                        success = new ProjectMasterDAL().CreateOrUpdateExcelMappingData(objTblProjectExcelMapping, ProjectID,
                            objobjExcelSheetBOList, UserId, dt, out returnMessage, out dt1, out isLatlongCalucalted);

                        List<String> mailto = new List<String>();
                        mailto.Add("yogendra.rajpoot@snssystem.com");
                        mailto.Add("deepak.yadav@snssystem.com");
                        //new GarronT.CMS.Model.clsMail().SendMail(mailto, null, "Excel file uploaded successfully. <br /> Project Id : " + ProjectID + "<br /><br />Thanks and Regards,<br />Team CMS", "Excel file uploaded");

                        /// calculate the latitude
                        Task.Factory.StartNew(() =>
                        {
                            //new CalculateLatLongController().CalculateNullLatLongbyProjectID(ProjectID); // Old method
                            new CalculateLatLongController().GetLatLongForProject(ProjectID);
                        });

                        MSg = returnMessage;
                        //return Json(new { success = success, returnMessage = "Record Submitted Successfully. \n Processing in Background", filePath = fileName }, JsonRequestBehavior.AllowGet);
                        return Json(new { success = success, returnMessage = "File Uploaded Successfully Processing in Background", filePath = fileName }, JsonRequestBehavior.AllowGet);
                        
                    }

                }
                else
                {
                    return Json(new { success = false, returnMessage = "Submit failed. Please upload excel file first." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, returnMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public JsonResult GetOldExcelData(int ProjectID)
        {
            string returnMessage = "";
            List<ExcelSheetBO> objobjExcelSheetBOList = new List<ExcelSheetBO>();
            try
            {
                List<ExcelSheetBO> objExcelMapping = new ProjectMasterDAL().GetOldExcelMappingdata(ProjectID, out returnMessage);

                List<VW_GETTab6DropdownData> objData = new List<VW_GETTab6DropdownData>();
                using (ProjectMasterDAL obj = new ProjectMasterDAL())
                {
                    objData = obj.GetViewData(ProjectID);
                    TempData["FieldMapingView"] = objData;
                    TempData.Keep("FieldMapingView");
                }
                string s = "";
                if (objExcelMapping != null && objData.Count > 0)
                {

                    s = "<div class='form-group' style='padding:2px;'>" +
                       "<div class='col-lg-2 col-sm-2'></div> <div class='col-lg-4 col-sm-4' style='white-space: nowrap; font-weight: bold;'>" +
                         "Excel File Fields</div>" +
                           "<div class='col-lg-4 col-sm-4' style='white-space:nowrap; font-weight: bold;'>" +
                       "Data</div><div class='col-lg-2 col-sm-2'></div> </div>";
                    int i = 1;

                    foreach (var item in objExcelMapping)
                    {
                        s = s + "<div class='form-group' style='padding:2px;'><div class='col-lg-2 col-sm-2'></div> <div class='col-lg-4 col-sm-4'>" +
                             item.FieldName + "</div><div class='col-lg-4 col-sm-4' id='opt'>" +
                             "<select class='dropDownFoo' id='_DFoo" + i.ToString() + "'> <option value='0'>-- Select --</option>";

                        foreach (var item1 in objData)
                        {
                            string checkMatch = item.FieldID == item1.tabelId && item.FieldType == item1.FieldType ? "selected" : "";
                            s = s + "<option value='" + item1.Id + "' " + checkMatch + ">" + item1.FieldName + "</option>";
                        }
                        s = s + "</select> </div>  <div class='col-lg-2 col-sm-2'></div></div> ";
                        i++;
                    }
                }

                return Json(new { success = true, excelData = s }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, excelData = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="OldRecords"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetOldParitalViewData(List<ExcelSheetBO> OldRecords, int ProjectID)
        {

            try
            {

                List<string> tempdata = new List<string>();
                List<BindDropDownListData> objBindDropDownListDatalist = new List<BindDropDownListData>();
                BindDropDownListData objBindDropDownListData = new BindDropDownListData();
                List<ProjectStandardFieldReletionBO> objProjectStandardFieldReletionBOList = new List<ProjectStandardFieldReletionBO>();
                foreach (var item in OldRecords)
                {
                    ProjectStandardFieldReletionBO objProjectStandardFieldReletionBO = new ProjectStandardFieldReletionBO();


                    if (item.FieldName != null)
                    {

                        tempdata.Add(item.FieldName.ToString());
                        objBindDropDownListData.headerlist = tempdata;
                    }
                    if (item.FieldType != null)
                    {
                        objProjectStandardFieldReletionBO.StandarFiledName = item.FieldType;
                    }
                    else
                    {
                        //objProjectStandardFieldReletionBO.StandarFiledName = null;
                    }
                    if (item.FieldID != null)
                    {
                        objProjectStandardFieldReletionBO.filedid = Convert.ToInt32(item.FieldID);
                    }
                    else
                    {
                        objProjectStandardFieldReletionBO.filedid = 0;
                    }

                    objProjectStandardFieldReletionBOList.Add(objProjectStandardFieldReletionBO);

                    //objBindDropDownListData.objStandardFieldReletion = objProjectStandardFieldReletionBOList;
                    using (ProjectMasterDAL obj = new ProjectMasterDAL())
                    {

                        objBindDropDownListData.objData = obj.GetViewData(ProjectID);

                    }

                }
                objBindDropDownListDatalist.Add(objBindDropDownListData);




                // return PartialView("_FileUploadFieldMapping", tempdata);
                //  return PartialView("_FileUploadFieldMapping", dt);
                return PartialView("_FileUploadFieldMapping", objBindDropDownListDatalist);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getFileUploadPartial()
        {
            try
            {
                log.Info("getFileUploadPartial Started at - " + DateTime.Now.ToString());

                string fileName = Request.Headers["X-File-Name"];
                string fileType = Request.Headers["X-File-Type"];
                int fileSize = Convert.ToInt32(Request.Headers["X-File-Size"]);
                //File's content is available in Request.InputStream property
                System.IO.Stream fileContent = Request.InputStream;
                //Creating a FileStream to save file's content
                System.IO.FileStream fileStream = System.IO.File.Create(Server.MapPath("~/UploadData/") + fileName);


                // 
                List<string> List1 = new List<string>();
                fileContent.Seek(0, System.IO.SeekOrigin.Begin);
                //Copying file's content to FileStream
                fileContent.CopyTo(fileStream);

                fileStream.Dispose();
                ValidateExcel1(Convert.ToString(fileStream.Name), out List1);


                if (List1.Count < 5)
                {
                    TempData["ExcelData"] = null;
                    return Json(new { success = false, excelData1 = "Excel file not hold minimum columns. At least 5 columns needed (STREET, ACCOUNT, Old Meter Number, CYCLE, ROUTE )" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //List<string> sample = new List<string>();
                    //sample.Add("CYCLE".ToLower()); sample.Add("ROUTE".ToLower()); sample.Add("ACCOUNT".ToLower());
                    //sample.Add("STREET".ToLower()); sample.Add("OLD METER NUMBER".ToLower()); sample.Add("OLD METER SIZE".ToLower());
                    //sample.Add("OLD METER TYPE".ToLower()); sample.Add("OLD METER RADIO NUMBER".ToLower()); sample.Add("OLD METER LAST READING".ToLower());
                    //bool st = false;
                    //foreach (var item in sample)
                    //{
                    //    var match = List1.Where(o => o.ToLower().Trim() == item).FirstOrDefault();
                    //    if (match == null)
                    //    {
                    //        st = true;
                    //        break;
                    //    }
                    //}
                    //if (st)
                    //{
                    //    TempData["ExcelData"] = null;
                    //    return Json(new { success = false, excelData1 = "Excel Column Not Match With Sample File Provided" }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    TempData["ObjCurrentProject"] = List1;
                    TempData.Keep("ObjCurrentProject");
                    return Json(new { success = true, excelData1 = "File uploaded successfully." }, JsonRequestBehavior.AllowGet);
                    //}
                }
            }
            catch (Exception ex)
            {
                log.Info("Error at - " + DateTime.Now.ToString() + " -" + ex.Message);
                TempData["ExcelData"] = null;
                return Json(new { success = false, excelData1 = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FileUploadFieldMapping(long ProjectID)
        {
            try
            {
                List<string> tempdata = TempData["ObjCurrentProject"] as List<string>;

                List<VW_GETTab6DropdownData> objData = new List<VW_GETTab6DropdownData>();
                using (ProjectMasterDAL obj = new ProjectMasterDAL())
                {
                    objData = obj.GetViewData(ProjectID);
                    TempData["FieldMapingView"] = objData;
                    TempData.Keep("FieldMapingView");
                }


                string s = "<div class='form-group' style='padding:2px;'>" +
                    " <div class='col-lg-2 col-sm-2'></div><div class='col-lg-4 col-sm-4' style='white-space: nowrap; font-weight: bold;'>" +
                      "Excel File Fields</div>" +
                        "<div class='col-lg-4 col-sm-4' style='white-space:nowrap; font-weight: bold;'>" +
                    "Data</div><div class='col-lg-2 col-sm-2'></div> </div>";
                int i = 1;

                foreach (var item in tempdata)
                {
                    s = s + "<div class='form-group' style='padding:2px;'><div class='col-lg-2 col-sm-2'></div> <div class='col-lg-4 col-sm-4'>" +
                         item + "</div><div class='col-lg-4 col-sm-4' id='opt'>" +
                         "<select class='dropDownFoo' id='_DFoo" + i.ToString() + "'> <option value='0'>-- Select --</option>";

                    foreach (var item1 in objData)
                    {
                        string checkMatch = item1.FieldName.ToLower().Trim() == item.ToLower().Trim() ? "selected" : "";
                        s = s + "<option value='" + item1.Id + "' " + checkMatch + ">" + item1.FieldName + "</option>";
                    }
                    s = s + "</select> </div> <div class='col-lg-2 col-sm-2'></div> </div> ";
                    i++;
                }
                return Json(new { success = true, excelData = s }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, excelData = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="newFilePath"></param>
        /// <param name="List1"></param>
        /// <returns></returns>
        private List<string> ValidateExcel1(string newFilePath, out List<string> List1)
        {
            List<string> objList1 = new List<string>();
            try
            {

                #region getExcel data
                System.Data.DataTable dt = new System.Data.DataTable();
                List1 = null;
                FileInfo existingFile = new FileInfo(newFilePath);
                using (ExcelPackage xlPackage = new ExcelPackage(existingFile))
                {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];
                    dt = new DataTable();
                    //adding columns  
                    int iCol = 1;
                    int iRow = 1;
                    bool CanRead = true;
                    while (CanRead)
                    {
                        if (worksheet.Cells[iRow, iCol].Value != null)
                        {
                            if (worksheet.Cells[iRow, iCol].Value != DBNull.Value && !string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[iRow, iCol].Value)))
                            {
                                dt.Columns.Add(Convert.ToString(worksheet.Cells[iRow, iCol].Value));
                                objList1.Add(Convert.ToString(worksheet.Cells[iRow, iCol].Value));
                                iCol++;

                            }
                            else
                            {
                                CanRead = false;
                            }
                        }
                        else
                        {
                            CanRead = false;
                        }

                    }
                    //adding rows  
                    iRow = 2;
                    bool canRowRead = true;
                    while (canRowRead)
                    {



                        DataRow dr = dt.NewRow();
                        bool rowVal = true;
                        int colCount = 1;
                        while (colCount <= iCol)
                        {
                            if (worksheet.Cells[iRow, colCount].Value != DBNull.Value && !string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[iRow, colCount].Value)))
                            {
                                try
                                {
                                    dr[colCount - 1] = worksheet.Cells[iRow, colCount].Value;
                                }
                                catch
                                {
                                }
                                rowVal = false;
                            }
                            colCount++;
                        }



                        if (rowVal)
                        {
                            canRowRead = false;
                        }
                        else
                        {
                            dt.Rows.Add(dr);
                            iRow++;
                        }

                    }

                    TempData["ExcelData"] = dt;
                    TempData.Keep("ExcelData");
                }
                #endregion

                return List1 = objList1;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region Index page Actions
        /// <summary>
        /// Index page method
        /// </summary>
        /// <param name="ProjectId">Project id</param>
        /// <param name="status">True=>Activate, False=>Deactivate</param>
        /// <returns></returns>
        public ActionResult A_Or_D_Project(long ProjectId, bool status)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Project", "Delete");

            if (objPermissionCheck)
            {
                ProjectMasterDAL obj = new ProjectMasterDAL();
                string result;
                int currentUserId = (int)new UserDetails().GetUserId(User.Identity.GetUserId());
                bool a = obj.A_Or_D_Project(ProjectId, currentUserId, status, out result);
                return Json(new { success = a, returnMessage = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        public JsonResult CheckLatLongCalculatedStatus(long projectId)
        {
            tblProject data = new tblProject();
            int? status = 0;
            bool success = false;
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                data = (from b in CompassEntities.tblProjects
                        where b.Active == 1 && b.ProjectId == projectId
                        select b).FirstOrDefault();
                if (data != null)
                {
                    status = data.IsLatLongCalculated;
                    success = true;
                }
            }
            catch (Exception)
            {
                success = false;
            }


            //return data;


            return Json(new { success = success, status = status }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult setProjectStatus(long ProjectID)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Project", "Edit");

            if (objPermissionCheck)
            {
                bool s = false;
                using (ProjectMasterDAL obj = new ProjectMasterDAL())
                {
                    s = obj.GetProjectStatusData(ProjectID);
                }

                return Json(new { success = true, returnMessage = "Status Changed Successfully." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult GetUploadedDatabyProjectID(long ProjectId)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Project", "Edit");

            if (objPermissionCheck)
            {

                TempData["UploadedDataProjectID"] = ProjectId;
                TempData.Keep("UploadedDataProjectID");
                return Json(ProjectId, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult EditUploadedData()
        {
            if (TempData["UploadedDataProjectID"] != null)
            {
                // uploadedDataList = TempData["UploadedDataforEdit"] as List<tblUploadedData>;
                TempData.Keep("UploadedDataProjectID");
                return View();
            }
            else
            {
                return RedirectToAction("Create", "Project");
            }

        }


        /// <summary>
        /// Returns  excel uploaded data of the project & the project name.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetActiveUploadedData()
        {
            List<ProjectEditModel> uploadedDataList = new List<ProjectEditModel>();

            string projectName = string.Empty;

            bool LatLongButtonShow = false;

            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                long ProjectId = Convert.ToInt64(TempData["UploadedDataProjectID"]);

                TempData.Keep("UploadedDataProjectID");

                //get current project name.
                projectName = obj.GetProjectName().Where(o => o.ProjectId == ProjectId).Select(o => o.ProjectCityState).FirstOrDefault();

                //get all excel uploaded records.
                uploadedDataList = obj.GetUploadedDatabyProjectID(ProjectId).OrderByDescending(o => o.Latitude).ThenByDescending(o => o.Longitude).ToList();

                LatLongButtonShow = obj.GetLatLongCalculation(ProjectId);
            }

            var jsonResult = Json(new { projectName, uploadedDataList, LatLongButtonShow }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult GetRecordsbyUploadedID(long uploadedID)
        {
            ProjectEditModel uploadedData = new ProjectEditModel();

            try
            {
                using (ProjectMasterDAL obj = new ProjectMasterDAL())
                {
                    uploadedData = obj.GetUploadedDatabyUploadID(uploadedID);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, excelData = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(uploadedData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveRecord(ProjectEditModel uploadedData, bool StreetChanged)
        {
            bool success = false;
            string returnmessege = "";

            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                success = obj.UpdateUploadData(uploadedData, StreetChanged, out returnmessege);
                return Json(new { success = success, returnMessage = returnmessege }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CalculatePendingLatLon()
        {
            try
            {
                long ProjectId = Convert.ToInt64(TempData["UploadedDataProjectID"]);
                TempData.Keep("UploadedDataProjectID");

                Task.Factory.StartNew(() =>
                {
                    //new CalculateLatLongController().CalculatPendingLatLongbyProjectID(ProjectId);
                    new CalculateLatLongController().CalPendingLatLongForProject(ProjectId);
                });

                return Json(new { success = true, resultMessage = "Pending Lat Lon Calculation Started Successfully...!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = true, resultMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetProjectList(long ProjectID)
        {
            ProjectMasterDAL obj1 = new ProjectMasterDAL();
            try
            {
                List<ProductServiceBo> poductList = new List<ProductServiceBo>();


                poductList = obj1.GetProductServiceList(ProjectID);

                return Json(new { success = true, poductList = poductList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = true, resultMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveServiceProduct(long ProjectID, long serviceId, long productid, int quantity, int projectServiceId)
        {
            ProjectMasterDAL obj1 = new ProjectMasterDAL();
            try
            {
                List<ProductServiceBo> poductList = new List<ProductServiceBo>();


                bool success = obj1.SaveProductServiceList(ProjectID, serviceId, productid, quantity, projectServiceId);
                if (success)
                {
                    poductList = obj1.GetProductServiceListbyProjectId(ProjectID, serviceId);
                }

                return PartialView("_serviceProductList", poductList);

                // return Json(new { success = true, poductList = poductList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = true, resultMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetServiceProduct(long ProjectID, long serviceId)
        {
            ProjectMasterDAL obj1 = new ProjectMasterDAL();
            List<ProductServiceBo> poductList = new List<ProductServiceBo>();
            try
            {
                poductList = obj1.GetProductServiceListbyProjectId(ProjectID, serviceId);
                //return PartialView("_serviceProductList", poductList);
                //var productList = PartialView("_serviceProductList", poductList);

                return PartialView("_serviceProductList", poductList);
            }
            catch (Exception ex)
            {

                return Json(new { success = true, resultMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult DeleteServiceProduct(long ProjectID, long projectServiceId, long ServiceId)
        {
            ProjectMasterDAL obj1 = new ProjectMasterDAL();
            List<ProductServiceBo> poductList = new List<ProductServiceBo>();
            try
            {
                bool success = obj1.DeleteProductService(projectServiceId);
                if (success)
                {
                    poductList = obj1.GetProductServiceListbyProjectId(ProjectID, ServiceId);
                }

                return PartialView("_serviceProductList", poductList);


            }
            catch (Exception ex)
            {

                return Json(new { success = true, resultMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }



    }
}