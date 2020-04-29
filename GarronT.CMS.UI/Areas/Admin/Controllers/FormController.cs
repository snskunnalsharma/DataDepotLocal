using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using KSPL.Indus.MSTS.UI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
     [SessionExpire]
     [Authorize]
     [RBAC]
    public class FormController : Controller
    {
        //
        // GET: /Admin/Form/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getPreviousRecordsPartial()
        {
            return PartialView("_FormHome");
        }

        public ActionResult getFormPartial()
        {
            return PartialView("_AddNewForm");
        }



        public ActionResult get_setField1Partial()
        {
            return PartialView("_setField1");
        }


        public ActionResult GetProjectList()
        {
            List<ProjectModel> objList1 = new List<ProjectModel>();
            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                objList1 = obj.GetProjectName();
            }

            return Json(objList1, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetDynamicDivData(long ProjectId)
        {

            List<VW_GETTab6DropdownData> objData = new List<VW_GETTab6DropdownData>();
            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                objData = obj.GetViewData(ProjectId);
               
            }


            string s = "<div id='fieldDiv1' name='textField' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>ACCOUNT</div>" +
                "<div id='fieldDiv2' name='textField' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>STREET</div>" +
                "<div id='fieldDiv3' name='textField' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>OLD METER NUMBER</div>" +
                "<div id='fieldDiv4' name='textField' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>OLD METER SIZE</div>" +
                "<div id='fieldDiv5' name='textField' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>OLD METER TYPE</div>";
                //"<div id='fieldDiv8' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>OLD METER RADIO NO</div>" +
                //"<div id='fieldDiv9' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>OLD METER READING</div>" +
                //"<div id='fieldDiv10' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>NEW METER NUMBER</div>" +
                //"<div id='fieldDiv11' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>NEW REGISTER NUMBER</div>" +
                //"<div id='fieldDiv12' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>NEW METER RADIO</div>" +
                //"<div id='fieldDiv13' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>NEW METER SIZE</div>" +
                //"<div id='fieldDiv14' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>NEW METER TYPE</div>" +
                //"<div id='fieldDiv18' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>SERVICES</div>" +
                //"<div id='fieldDiv19' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>COMMENTS</div>" +
                //"<div id='fieldDiv20' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>Customer Pin</div>" +
                //"<div id='fieldDiv21' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>Customer Name</div>" +
                //"<div id='fieldDiv22' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>Customer Address</div>" +
                //"<div id='fieldDiv23' class='nextBtn' style='min-width: 80% !important; margin-bottom: 0px; float:none;'>Customer Pin</div>";

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

            return Json(objList1, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Tab 5 - Deactivates the custom field
        /// </summary>
        /// <param name="customFieldId"></param>
        /// <returns>true if successful</returns>
        public ActionResult A_Or_D_FormRecord(bool status, long FormId)
        {
            using (FormDAL obj = new FormDAL())
            {
                string result;
                int userId = 1;
                bool a = obj.A_Or_D_FormRecord(status, FormId, userId, out result);
                return Json(new { success = a, returnMessage = result }, JsonRequestBehavior.AllowGet);
            }
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



    }
}