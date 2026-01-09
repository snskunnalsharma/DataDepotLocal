using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using KSPL.Indus.MSTS.UI.Filters;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;


namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class DuplicateReportController : Controller
    {
        //
        // GET: /Admin/DuplicateReport/
        public ActionResult Index()
        {
            return View();
        }




        /// <summary>
        /// 2016/09/22
        /// Bharat Magdum
        /// Method to return the project list based on project status
        /// </summary>
        /// <param name="status">Active or Complete</param>
        /// <returns>Project list</returns>
        public JsonResult GetPoject(long status)
        {
            using (DuplicateReportDAL objDAL = new DuplicateReportDAL())
            {
                string proStatus = "";

                proStatus = status == 2 ? "Complete" : "Active";




                var checkAdmin = Session["CurrentUserIsAdmin"].ToString();


                var data = objDAL.GetProjectName(proStatus);

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

                data = data.OrderBy(o => o.ProjectName).ToList();
                return base.Json(data, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 2016/09/22
        /// Bharat Magdum
        /// Returns installer list for project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns>installer list</returns>
        [HttpGet]
        public JsonResult GetCurrentInstallerList(long projectId)
        {
            List<UsersSmallModel> obj1 = new List<UsersSmallModel>();
            if (projectId > 0)
            {
                using (DuplicateReportDAL obj = new DuplicateReportDAL())
                {
                    //fetch installer list from database
                    obj1 = obj.GeInstallerListCurrentProject(projectId).OrderBy(a => a.UserName).ToList();
                }
            }

            return Json(obj1, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 2016/09/22
        /// Bharat Magdum
        /// Returns the duplicate records for given project & installer
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="installerId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDuplicateRecords(long projectId, long installerId, int searchFieldId)
        {

            List<DuplicateReportBO> objDuplicateList = new List<DuplicateReportBO>();
            using (DuplicateReportDAL obj = new DuplicateReportDAL())
            {
                //fetch duplicate list from database
                objDuplicateList = obj.GetDuplicateRecords(projectId, installerId, searchFieldId);
            }
            return Json(new { varData = objDuplicateList }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 2016/09/22
        /// Bharat Magdum
        /// Returns the excel having data of duplicate records for given project & installer
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="installerId"></param>
        /// <returns></returns>
        public ActionResult ExportDuplicateRecordsExcel(long projectId, long installerId, int searchFieldId)
        {

            List<DuplicateReportBO> objDuplicateList = new List<DuplicateReportBO>();
            using (DuplicateReportDAL obj = new DuplicateReportDAL())
            {
                //fetch duplicate list from database
                objDuplicateList = obj.GetDuplicateRecords(projectId, installerId, searchFieldId);
            }


            #region Export to excel
            GridView gv = new GridView();
            var obj1 = objDuplicateList.OrderBy(a => a.Street).ThenBy(a => a.Account).Select(o =>
                new
                {
                    Account = o.Account,
                    Customer = o.Customer,
                    Street = o.Street,
                    Installer = o.FirstName,
                    VisitDate = o.visitdatetime,
                    OldMeterNo = o.OldMeterNo,
                    OldMeterRadioNo = o.OldMeterRadioNo,
                    NEWMETERNUMBER = o.NEWMETERNUMBER,
                    NEWMETERRADIO = o.NEWMETERRADIO
                }).ToList();
            gv.DataSource = obj1;
            gv.DataBind();
            Response.Clear();
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=DuplicateReport.xlsx");
            System.Data.DataTable dt = new CommonFunctions().ToDataTable(obj1.ToList());
            dt.TableName = "DuplicateReport";
            // gv.DataSource as System.Data.DataTable;
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet wsDt = pck.Workbook.Worksheets.Add("Sheet1");
                wsDt.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                wsDt.Cells[wsDt.Dimension.Address].AutoFitColumns();

                Response.BinaryWrite(pck.GetAsByteArray());
            }

            Response.Flush();
            Response.End();
            #endregion

            return RedirectToAction("Index");
        }

    }
}