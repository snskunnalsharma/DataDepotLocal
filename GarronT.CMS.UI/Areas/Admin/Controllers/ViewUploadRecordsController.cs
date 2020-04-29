using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.Model.BO;
using System.Configuration;
using KSPL.Indus.MSTS.UI.Filters;
using Microsoft.AspNet.Identity;
using GarronT.CMS.Model;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class ViewUploadRecordsController : Controller
    {
        // GET: Admin/ViewUploadRecords
        public ActionResult Index()
        {

            var records = new InventoryDAL().GetInwardHeader();

            var checkAdmin = Session["CurrentUserIsAdmin"].ToString();

            if (checkAdmin == "0")
            {
                var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var abcd = new ProjectMasterDAL().GetUserProjectList(userId);


                if (abcd == null || abcd.Count == 0)
                {
                    if (records != null && records.Count() > 0)
                    {
                        records = records.Take(0).ToList();
                    }
                }
                else
                {
                    if (records != null && records.Count() > 0)
                    {
                        records = records.Where(o => abcd.Contains(o.ProjectId)).ToList();
                    }
                }

            }
            return View(records);
        }

        public ActionResult ViewUploadRecords(long Id)
        {
            ViewUploadInventryVM viewUploadInventryVM = new ViewUploadInventryVM();
            try
            {
                var records = new InventoryDAL().GetInwardHeader(Id);

                viewUploadInventryVM.ReceiveInvoiceSavedRecordList = new List<ReceiveInvoiceSavedRecords>();
                viewUploadInventryVM.RequestId = records.RequestId;
                viewUploadInventryVM.uploadType = records.uploadType;
                viewUploadInventryVM.UserName = records.UserName;
                viewUploadInventryVM.DeviceDateTime = records.DeviceDateTime;

                viewUploadInventryVM.ProjectName = records.ProjectName;
                viewUploadInventryVM.WarehouseName = records.WarehouseName;


                string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);


                viewUploadInventryVM.ReceiveInvoiceSavedRecordList = new InventoryDAL().GetUploadedRecords(Id);

                foreach (var item in viewUploadInventryVM.ReceiveInvoiceSavedRecordList)
                {
                    string strFilePath = item.BarCodeFilePath;
                    if (!string.IsNullOrEmpty(strFilePath))
                    {
                        if (strFilePath.Contains("..\\..\\"))
                        {
                            strFilePath = "" + strFilePath.Replace("..\\..\\", _webUrl);
                        }

                        if (strFilePath.Contains("~"))
                        {
                            strFilePath = "" + strFilePath.Replace("~\\", _webUrl);
                        }

                        strFilePath = "" + strFilePath.Replace('\\', '/');
                        item.BarCodeFilePath = strFilePath.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return View(viewUploadInventryVM);
        }
    }
}