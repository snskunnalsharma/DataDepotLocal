using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.UI.CustomHelpers;
using KSPL.Indus.MSTS.UI.Filters;
using Microsoft.AspNet.Identity;
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
    public class StockRequestController : Controller
    {
        // GET: Admin/StockRequest
        public ActionResult Index()
        {
            //var data = new StockRequestDAL().GetStockRequest().ToList();

            return View();
        }

        public ActionResult GetData()
        {

            var data = new StockRequestDAL().GetStockRequest();

            var checkAdmin = Session["CurrentUserIsAdmin"].ToString();

            if (checkAdmin == "0")
            {
                var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var abcd = new ProjectMasterDAL().GetUserProjectList(userId);


                if (abcd == null || abcd.Count == 0)
                {
                    if (data != null && data.Count() > 0)
                    {
                        data = data.Take(0).ToList();
                    }
                }
                else
                {
                    if (data != null && data.Count() > 0)
                    {
                        data = data.Where(o => abcd.Contains(o.ProjectId)).ToList();
                    }
                }

            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ApproveReject(long requestId, int actionId, string comment)
        {
            bool success = false;
            string returnMessage = "";
            try
            {
                long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                success = new StockRequestDAL().ApproveRejectRecord(currentUserId, requestId, actionId, comment, ref returnMessage);
            }
            catch (Exception ex)
            {
                returnMessage = "Error in Approve/Reject record";
                throw ex;
            }

            return Json(new { success, returnMessage }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetDetailData(long requestId, bool status)
        {
            string strStatus = status == true ? "Edit" : "View";
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("StockRequest", strStatus);

            if (objPermissionCheck)
            {
                bool success = false;
                string returnMessage = "";
                StockRequestDetailVM StockRequestDetailVM = new StockRequestDetailVM();
                try
                {
                    StockRequestDetailVM.StockRequestModel = new StockRequestDAL().GetStockRequest().Where(a => a.RequestId == requestId).FirstOrDefault();
                    StockRequestDetailVM.StockRequestDetailModelList = new StockRequestDAL().GetStockDetailRequest(requestId);

                    return PartialView("_StockRequestDetail", StockRequestDetailVM);

                }
                catch (Exception ex)
                {
                    returnMessage = "Error in Approve/Reject record";
                    //throw;
                }

                return Json(new { success, returnMessage }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}