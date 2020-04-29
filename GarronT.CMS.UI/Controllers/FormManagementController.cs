using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KSPL.Indus.MSTS.UI.Filters;
using GarronT.CMS.UI.CustomHelpers;

namespace GarronT.CMS.UI.Controllers
{
    [Authorize]
    [SessionExpire]
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [RBAC]
    public class FormManagementController : Controller
    {
        //
        // GET: /Form/FormManagement/

        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public ActionResult GetProjectList(bool Activeproject)
        {
            List<ProjectModel> objList1 = new List<ProjectModel>();
            if (Activeproject == false)
            {
                using (ProjectMasterDAL obj = new ProjectMasterDAL())
                {
                    objList1 = obj.GetAllProjectName().ToList();
                }
            }
            else
            {
                using (ProjectMasterDAL obj = new ProjectMasterDAL())
                {
                    objList1 = obj.GetProjectName().ToList();
                }
            }


            var checkAdmin = Session["CurrentUserIsAdmin"].ToString();

            if (checkAdmin == "0")
            {
                var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var abcd = new ProjectMasterDAL().GetUserProjectList(userId);


                if (abcd == null || abcd.Count == 0)
                {
                    if (objList1 != null && objList1.Count() > 0)
                    {
                        objList1 = objList1.Take(0).ToList();
                    }
                }
                else
                {
                    if (objList1 != null && objList1.Count() > 0)
                    {
                        objList1 = objList1.Where(o => abcd.Contains(o.ProjectId)).ToList();
                    }
                }

            }
            return Json(objList1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataTypeMaster()
        {
            List<FieldDataTypeMasterModel> objList1 = new List<FieldDataTypeMasterModel>();
            using (FormDAL obj = new FormDAL())
            {
                objList1 = obj.GetFieldDataType();
            }

            return Json(objList1, JsonRequestBehavior.AllowGet);
        }




        public ActionResult GetDynamicDivData(long ProjectId, string isAuditor)
        {

            List<SP_GetFieldInfoForProject_Result> objData = new List<SP_GetFieldInfoForProject_Result>();
            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                if (isAuditor.ToLower() == "false")
                {
                    objData = obj.GetFieldInfoForProject(ProjectId).Where(a => a.FieldName != "INSTALLER NAME" && a.FieldName != "VISIT DATETIME").ToList();
                }
                else
                {

                    /*Audit form data setup */
                    objData = obj.GetFieldInfoForProject(ProjectId).Where(a => a.FieldName != "ADDITIONAL SERVICES" && a.FieldName != "SERVICES").ToList(); ;
                    foreach (var item in objData)
                    {
                        if (item.FieldName == "INSTALLER NAME" || item.FieldName == "VISIT DATETIME" || item.FieldName == "NEW METER NUMBER" || item.FieldName == "NEW METER RADIO")
                        {
                            item.isMappedToExcel = 1;
                        }
                    }

                }


            }

            return Json(objData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Returns form list
        /// </summary>
        /// <param name="Status">1=> active , 0=> Deactive </param>
        /// <returns></returns>
        public ActionResult GetFormList(bool Status)
        {

            List<FormDOL> objList1 = new List<FormDOL>();
            using (FormDAL obj = new FormDAL())
            {
                objList1 = obj.GetFormList(Status);
            }


            var checkAdmin = Session["CurrentUserIsAdmin"].ToString();

            if (checkAdmin == "0")
            {
                var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var abcd = new ProjectMasterDAL().GetUserProjectList(userId);


                if (abcd == null || abcd.Count == 0)
                {
                    if (objList1 != null && objList1.Count() > 0)
                    {
                        objList1 = objList1.Take(0).ToList();
                    }
                }
                else
                {
                    if (objList1 != null && objList1.Count() > 0)
                    {
                        objList1 = objList1.Where(o => abcd.Contains(o.ProjectId)).ToList();
                    }
                }
            }


            return Json(objList1, JsonRequestBehavior.AllowGet);
        }



        public ActionResult saveFormFields(List<FormManagementViewModel> formdata, FormMasterFields masterFields, List<ListOptionViewModel> optionList)
        {
            string returnMessage = "Please add sections";
            bool success = false;
            long formid = 0;
            //string loggeduserid = System.Web.HttpContext.Current.User.Identity.GetUserId();
            int loggeduserid = 1;

            CompassEntities CompassEntities = new CompassEntities();
            var recordfound = (from m in CompassEntities.TblProjectFieldDatas
                               where m.FormId == masterFields.FormId
                               select m).FirstOrDefault();
            var formused = false;
            if (recordfound != null)
            {
                formused = true;
            }
            if (formused == false && recordfound == null)
            {

                if (formdata != null)
                {
                    if (formdata[0].SectionFields == null)
                    {
                        returnMessage = "Please Select Fields";
                    }
                    else
                    {
                        success = new FormDAL().SaveUpdateMasterFields(masterFields, formdata, optionList, Convert.ToInt16(loggeduserid), out returnMessage, ref formid);
                    }
                }
            }
            else
            {
                success = false;
                returnMessage = "Form in use";
            }

            return Json(new { success, returnMessage, formid }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getRecordsforEdit(long RecordID, long ProjectId, string recordfrom)
        {
            string returnMessage = "Record not Found";
            bool success = false;


            List<SP_GetFieldInfoForProject_Result> objData = new List<SP_GetFieldInfoForProject_Result>();
            if (recordfrom == "Clone" && ProjectId != 0)
            {
                using (ProjectMasterDAL obj = new ProjectMasterDAL())
                {
                    objData = obj.GetFieldInfoForProject(ProjectId);

                }
            }


            List<FormManagementViewModel> formdata = new List<FormManagementViewModel>();

            List<ListOptionViewModel> Optiondata = new List<ListOptionViewModel>();

            FormMasterFields masterFields = new FormMasterFields();

            using (FormDAL obj = new FormDAL())
            {
                masterFields = obj.GetFormmasterFields(RecordID);
                if (masterFields != null)
                {
                    success = true;
                    if (recordfrom == "Clone" && ProjectId != 0)
                    {
                        formdata = obj.GetFormSectionFieldsClone(masterFields.FormId, objData);


                    }
                    else
                    {
                        formdata = obj.GetFormSectionFields(masterFields.FormId);
                    }

                    Optiondata = obj.GetFormOptions(masterFields.FormId);
                }

            }
            CompassEntities CompassEntities = new CompassEntities();
            var recordfound = (from m in CompassEntities.TblProjectFieldDatas
                               where m.FormId == RecordID
                               select m).FirstOrDefault();
            var formused = false;
            if (recordfound != null)
            {
                formused = true;
            }


            return Json(new { success, returnMessage, masterFields, Optiondata, formdata, formused }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeactivateRecords(long DeactivateID)
        {
            string returnMessage = "";
            bool success = false;

            using (FormDAL obj = new FormDAL())
            {
                success = obj.DeactivateRecord(DeactivateID, out returnMessage);
            }

            return Json(new { success, returnMessage }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult ActivateRecords(long ActivateID)
        {
            string returnMessage = "";
            bool success = false;

            using (FormDAL obj = new FormDAL())
            {
                success = obj.ActivateRecord(ActivateID, out returnMessage);
            }

            return Json(new { success, returnMessage }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetFormsbyProjectID(long ProjectID)
        {

            List<FormMasterFields> formdata = new List<FormMasterFields>();

            using (FormDAL obj = new FormDAL())
            {
                formdata = obj.GetFormsbyProjectID(ProjectID);
            }


            return Json(formdata, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CheckFormUsed(long? FormID)
        {
            DateTime currentdate = new CommonFunctions().ServerDate();
            bool recordexist = false;
            bool projectActive = true;
            bool projectDateexpire = false;
            if (FormID != null)
            {
                using (CompassEntities CompassEntities = new CompassEntities())
                {
                    var recordfound = (from m in CompassEntities.TblProjectFieldDatas
                                       where m.FormId == FormID
                                       select m).FirstOrDefault();

                    //AddedBy AniketJ on 10-Jan-2016
                    //To dontallow user to edit record when form used
                    var recordfoundAydit = (from m in CompassEntities.ProjectAuditDatas
                                            where m.FormId == FormID
                                            select m).FirstOrDefault();

                    if (recordfound != null || recordfoundAydit != null)
                    {
                        recordexist = true;
                    }

                    var projectfound = (from m in CompassEntities.tblFormMasters
                                        join p in CompassEntities.tblProjects on m.ProjectId equals p.ProjectId
                                        where m.FormId == FormID
                                        select p).FirstOrDefault();
                    if (projectfound.Active == 0)
                    {
                        projectActive = false;
                    }
                    if (projectfound.ToDate > currentdate)
                    {
                        projectDateexpire = true;
                    }


                }

            }
            return Json(new { recordexist, projectActive, projectDateexpire }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult CheckUserPermission(string straction)
        {
            var strMessage = "";
            if (straction == "ADD" || straction == "CLONE")
            {
                var objPermissionCheck = UserPermissionCheck.checkUserStatus("FormManagement", "Add");

                if (objPermissionCheck == false)
                {
                    strMessage = UserPermissionCheck.PerMessage;
                }
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
            else if (straction == "EDIT")
            {
                var objPermissionCheck = UserPermissionCheck.checkUserStatus("FormManagement", "Edit");

                if (objPermissionCheck == false)
                {
                    strMessage = UserPermissionCheck.PerMessage;
                }
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
            else if (straction == "VIEW")
            {
                var objPermissionCheck = UserPermissionCheck.checkUserStatus("FormManagement", "View");

                if (objPermissionCheck == false)
                {
                    strMessage = UserPermissionCheck.PerMessage;
                }
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }


        }

    }


}