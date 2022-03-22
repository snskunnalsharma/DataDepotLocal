using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using KSPL.Indus.MSTS.UI.Filters;
using OfficeOpenXml;
//using OfficeOpenXml;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Device.Location;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNet.Identity;
using GarronT.CMS.UI.CustomHelpers;
using GarronT.CMS.Model;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class InstallerAllocationController : Controller
    {
        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(clsMail));
        #endregion
        public class Address
        {
            public int id
            {
                get;
                set;
            }
        }
        public ActionResult Index()
        {
            return base.View();
        }
        public ActionResult Index1()
        {
            return base.View();
        }
        public ActionResult GetPoject()
        {
            ProjectMasterDAL projectMasterDAL = new ProjectMasterDAL();
            IEnumerable<ProjectModel> data = (from a in projectMasterDAL.GetProjectName()
                                              orderby a.ProjectName
                                              select a).Distinct<ProjectModel>();
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

            return base.Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getRowPartial()
        {
            return base.PartialView("Partial_InstallerAllocation");
        }

        public JsonResult GetAddress(List<string> Root, List<string> Cycle, List<string> Address, long ProjectID, string installerId, string fromDate, string toDate, string includeSkip)
        {
            try
            {
                List<AddressMaster> installerAddress = new List<AddressMaster>();
                List<AddressMaster> list = new List<AddressMaster>();

                list = new InstallerDAL().GetDynamicAllAddress(Root, Cycle, ProjectID);
                var userId = long.Parse(installerId);
                if (!string.IsNullOrEmpty(installerId)) 
                {
                    // update excludeskip status for user
                    CompassEntities db = new CompassEntities();
                    var UserObj = db.tblUsers.Where(n => n.UserID == userId).FirstOrDefault();
                    if (includeSkip != null && includeSkip.Equals("1"))
                    {
                        UserObj.ExcludeSkip = 0;
                    }
                    else
                    {
                        UserObj.ExcludeSkip = 1;
                    }
                    db.SaveChanges();
                    // get existing mapped records.
                    installerAddress = new InstallerDAL().GetDynamicInstallerExistingAddress(ProjectID, Root, Cycle, fromDate, toDate, long.Parse(installerId), includeSkip);

                    if (list != null && list.Count > 0 && Address != null && Address.Count > 0)
                    {
                        var tempData = list.Where(o => Address.Contains(o.ID.ToString())).Select(o => new AddressMaster
                        {
                            Latitude = o.Latitude,
                            Longitude = o.Longitude,
                            ID = o.ID,
                            Street = o.Street,
                            Cycle = o.Cycle,
                            Route = o.Route
                        }).ToList();

                        foreach (var item in tempData)
                        {
                            var checkPresent = installerAddress.Where(o => o.ID == item.ID).Count();
                            if (checkPresent == 0)
                            {
                                installerAddress.Add(item);
                            }
                        }
                    }

                }
                if ((list == null || list.Count == 0) && (installerAddress != null || installerAddress.Count > 0))
                {
                    list = installerAddress;
                }
                else
                {
                    if (installerAddress != null)
                    {
                        list.AddRange(installerAddress);
                        list = list.OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();
                    }
                }


                JsonResult jsonResult = base.Json(new
                {
                    AllAddress = list,
                    InstallerAddress = installerAddress
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = new int?(2147483647);
                return jsonResult;
            }
            catch (Exception ex)
            {

                // throw;
                log.Info("InstallerAllocation  GetAddress ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));


                return base.Json(new { success = false, message = "Server error." }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult getCurrentInstallerList(long projectId)
        {
            List<UsersSmallModel> obj1 = new List<UsersSmallModel>();
            if (projectId > 0)
            {
                using (UserRoleDAL obj = new UserRoleDAL())
                {
                    obj1 = obj.GeInstallerListCurrentProject(projectId).OrderBy(a => a.UserName).ToList();
                }
            }

            return Json(obj1, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetFreshCycles(long projectID)
        {
            List<string> cyclelist = new List<string>();

            //cyclelist = new InstallerDAL().GetFreshCycleByProjectID(projectID);
            cyclelist = new InstallerDAL().GetDynamicFreshCycleByProjectID(projectID);

            return base.Json(cyclelist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExistingCycles(long projectID, long installerId, string fromDate, string toDate)
        {
            List<string> cyclelist1 = new List<string>();
            cyclelist1 = new InstallerDAL().GetDynamicFreshCycleByProjectID(projectID);

            var cyclelist2 = new InstallerDAL().GetDynamicExistingMappedCycle(projectID, installerId, DateTime.Parse(fromDate), DateTime.Parse(toDate));

            List<CycleMaster> OrignalCycleList = new List<CycleMaster>();
            List<CycleMaster> cyclelist = new List<CycleMaster>();
            foreach (var item in cyclelist1)
            {
                OrignalCycleList.Add(new CycleMaster { CycleId = item, CycleDisabled = false });
            }


            foreach (var item in cyclelist2)
            {
                var ObjCycle = OrignalCycleList.Where(o => o.CycleId == item.Cycle).FirstOrDefault();
                if (ObjCycle == null)
                {
                    OrignalCycleList.Add(new CycleMaster { CycleId = item.Cycle, CycleDisabled = item.Disabled == 0 ? false : true });
                }
                else
                {

                    if (ObjCycle.CycleDisabled == false)
                    {
                        ObjCycle.CycleDisabled = item.Disabled == 0 ? false : true;
                    }
                }
            }

            foreach (var item in cyclelist2)
            {
                var ObjCycle = cyclelist.Where(o => o.CycleId == item.Cycle).FirstOrDefault();
                if (ObjCycle == null)
                {
                    cyclelist.Add(new CycleMaster { CycleId = item.Cycle, CycleDisabled = item.Disabled == 0 ? false : true });
                }
                else
                {

                    if (ObjCycle.CycleDisabled == false)
                    {
                        ObjCycle.CycleDisabled = item.Disabled == 0 ? false : true;
                    }
                }
            }

            cyclelist = cyclelist.OrderBy(o => o.CycleId).Distinct().ToList();
            OrignalCycleList = OrignalCycleList.OrderBy(o => o.CycleId).Distinct().ToList();
            return base.Json(new { OrignalCycleList = OrignalCycleList, CycleList = cyclelist }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllRouteByCycle(long ProjectId, string[] Cycle, string[] Route, string InstallerId)
        {
            List<string> routeList = new InstallerDAL().GetAllRouteByCycle(Cycle, ProjectId);
            var InstallerRouteList = new List<RouteMaster>();

            foreach (var item in Route)
            {
                InstallerRouteList.Add(new RouteMaster { RouteId = item, RouteDisabled = false });
            }
            return base.Json(new { Route = routeList, selectedRoute = InstallerRouteList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRootByCycle(long ProjectId, string[] Cycle, string InstallerId, string strDateTime, string strtoTime)
        {


            List<string> routeList = new InstallerDAL().GetDynamicFreshRootByCycle(Cycle, ProjectId);

            List<PROC_ExistingRoute_Result> ObjExistingList = new InstallerDAL().GetDynamicExistingRootByCycle(Cycle, ProjectId, long.Parse(InstallerId), DateTime.Parse(strDateTime), DateTime.Parse(strtoTime));


            if (routeList == null)
            {
                routeList = new List<string>();
            }

            foreach (var item in ObjExistingList)
            {
                var checkRoutePresent = routeList.Where(o => o == item.Route).FirstOrDefault();
                if (checkRoutePresent == null)
                {
                    routeList.Add(item.Route);
                }
            }

            var InstallerRouteList = new List<RouteMaster>();

            foreach (var item in ObjExistingList)
            {
                InstallerRouteList.Add(new RouteMaster { RouteId = item.Route, RouteDisabled = item.Disabled == 1 ? true : false });
            }

            return base.Json(new { Route = routeList, selectedRoute = InstallerRouteList }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetLat_by_Root(List<long> Address)
        {
            InstallerDAL installerDAL = new InstallerDAL();
            List<tblUploadedDataBO> lat_Lag = installerDAL.GetLat_LagNew(Address).OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();
            var jsonResult = Json(lat_Lag, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }


        public ActionResult GetInstallerList()
        {
            List<UsersModel> installerList = new InstallerDAL().GetInstallerList();
            return base.Json(installerList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveInstallerMap(InstallerMap InstallerMap)
        {
            try
            {
                var objPermissionCheck = false;
                if (!string.IsNullOrEmpty(InstallerMap.OldFromDate))
                {
                    objPermissionCheck = UserPermissionCheck.checkUserStatus("InstallerAllocation", "Edit");
                }
                else
                {
                    objPermissionCheck = UserPermissionCheck.checkUserStatus("InstallerAllocation", "Add");
                }


                if (objPermissionCheck)
                {
                    string returnMessage = "";
                    InstallerDAL installerDAL = new InstallerDAL();
                    bool Flag = false;
                    List<string> DeallocatedItem = null;


                    bool success = installerDAL.create(InstallerMap, out returnMessage, out DeallocatedItem, out Flag);
                    

                    #region Added code by vishal To send mail to installer
                    if (success)
                    {
                        List<String> mailto = new List<String>();
                        CompassEntities db = new CompassEntities();
                        var UserEmailId = db.tblUsers.Where(n => n.UserID == InstallerMap.InstallerID).FirstOrDefault();
                        //
                        string Subject = installerDAL.GetSubject();

                        log.Info("Installer Allocation success-" + DateTime.Now.ToString());

                        List<String> CCEmailTo = new List<String>();
                        CCEmailTo = installerDAL.CCEmailTo();
                        Task.Factory.StartNew(() =>
                        {
                            mailto.Add(UserEmailId.Email);
                            log.Info("Calling Mail send method at-" + DateTime.Now.ToString());
                            new GarronT.CMS.Model.clsMail().SendMail(mailto, CCEmailTo, installerDAL.SendMalil(InstallerMap, DeallocatedItem, Flag), Subject);
                            log.Info("end of Mail send method at-" + DateTime.Now.ToString());
                        });

                    }
                    #endregion


                    //Added by AniketJ on 23-Sep-2016 to change date of existing records
                    if (!string.IsNullOrEmpty(InstallerMap.OldFromDate) && !string.IsNullOrEmpty(InstallerMap.OldToDate))
                    {
                        if ((Convert.ToDateTime(InstallerMap.FromDate) != Convert.ToDateTime(InstallerMap.OldFromDate)) || (Convert.ToDateTime(InstallerMap.ToDate) != Convert.ToDateTime(InstallerMap.OldToDate)))
                        {
                            installerDAL.changeDateofExistingRecords(InstallerMap);
                        }
                    }

                    return base.Json(new { success, returnMessage }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                    return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in save method at-" + DateTime.Now.ToString());
                log.Error("Error in save method at-" + ex.Message);
                log.Error("Error in save method at-" + ex.InnerException.Message);
                return base.Json(new { success = false, ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }



        public ActionResult CheckAddressExistby_ProjectInstallerDate(long ProjectID, long InstallerID, string fromDate, string toDate)
        {

            using (InstallerDAL objDAL = new InstallerDAL())
            {
                string todaysDate = DateTime.Parse(new CommonFunctions().ServerDate().ToString()).ToString("MMM-dd-yyyy");
                DateTime startDate = Convert.ToDateTime(fromDate);
                DateTime endDate = Convert.ToDateTime(toDate);
                var lat_by_ProjectInstaller = objDAL.CheckAddressExistby_ProjectInstallerDate(ProjectID, InstallerID, startDate, endDate);
                CompassEntities db = new CompassEntities();
                var ExcludeSkip = db.tblUsers.Where(n => n.UserID == InstallerID).FirstOrDefault().ExcludeSkip;
                //foreach (var item in lat_by_ProjectInstaller)
                //{
                //    item.strFormDate = item.FromDate.Value.ToString("MMM-dd-yyyy");
                //    item.strToDate = item.ToDate.Value.ToString("MMM-dd-yyyy");
                //}

                return base.Json(new { lat_by_ProjectInstaller, todaysDate, ExcludeSkip }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ExportInstallerAllocation(List<long> Address, long ProjectID, long InstallerID, string InstallerName, string FromDate, string ToDate)
        {
            bool status = true;
            string Message = "";
            //List<projectStatus> Get_lat_lagnew = new InstallerDAL().ProjectStatus(Address, ProjectID, InstallerID, InstallerName, FromDate, ToDate, out Message, ref status);
            DateTime fromDate = DateTime.Parse(FromDate);
            DateTime toDate = DateTime.Parse(ToDate);
            List<InstallerDataStatus> objInstallerDataStatusList = new InstallerDAL().GetDynamicAllRecordDataTable(fromDate, toDate, InstallerID, ProjectID);

            TempData["ExportDataList"] = objInstallerDataStatusList;
            TempData.Keep("ExportDataList");
            return base.Json(new { status, Message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadExcelInstaller()
        {

            ProjectMasterDAL projectMasterDAL = new ProjectMasterDAL();
            List<InstallerDataStatus> data = TempData["ExportDataList"] as List<InstallerDataStatus>;


            #region Export to excel
            GridView gv = new GridView();
            gv.DataSource = data.OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();
            gv.DataBind();
            Response.Clear();
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=InstallerAllocationReport.xlsx");
            System.Data.DataTable dt = new CommonFunctions().ToDataTable(data.ToList());

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



        /// <summary>
        /// Created by Aniket on 25-Aug-2016
        /// Get Installer List
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="InstallerID"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetInstallerListForAddressAssign(long projectId, long InstallerID)
        {
            List<UsersSmallModel> installerList = new List<UsersSmallModel>();
            if (projectId > 0 && InstallerID > 0)
            {
                using (UserRoleDAL obj = new UserRoleDAL())
                {
                    installerList = obj.GeInstallerListCurrentProject(projectId).Where(a => a.UserId != InstallerID).OrderBy(a => a.UserName).ToList();
                }
            }

            return Json(installerList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created by Aniket on 25-Aug-2016
        /// Get Address List
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="InstallerID"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAddressListToAssignInstaller(long projectId, long InstallerID, string Status, string fromDate, string toDate)
        {
            try
            {
                // get existing mapped records.
                List<AddressMaster> installerAddress = new InstallerDAL().GetDynamicInstallerAddress(projectId, fromDate, toDate, InstallerID, Status);

                return Json(installerAddress, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                log.Info("InstallerAllocation Change Installer Get address ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));


                return base.Json(new { success = false, message = "Server error." }, JsonRequestBehavior.AllowGet);
            }

        }


        /// <summary>
        /// Created by Aniket on 25-Aug-2016
        /// Assign Addresses from one installer to another Installer
        /// </summary>
        /// <param name="InstallerMap"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AssignAddressToInstaller(AddressInstallerMap InstallerMap)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("InstallerAllocation", "Edit");



            if (objPermissionCheck)
            {
                string returnMessage = "";
                InstallerDAL installerDAL = new InstallerDAL();
                bool success = installerDAL.AssignDynamicAddressToInstaller(InstallerMap, out returnMessage);

                return base.Json(new { success, returnMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult CheckLatLongCalculatedStatus(long projectId)
        {
            tblProject data = new tblProject();
            int? status = 0;
            bool success = false;
            string utilitytype = "";
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
                    utilitytype = data.Utilitytype;
                }
            }
            catch (Exception)
            {
                success = false;
            }


            //return data;


            return Json(new { success = success, status = status, utilitytype = utilitytype }, JsonRequestBehavior.AllowGet);
        }

    }



    public class NameAndNumber
    {
        public NameAndNumber(string s)
        {
            OriginalString = s;
            Match match = Regex.Match(s, @"^(.*?)(\d*)$");
            Name = match.Groups[1].Value;
            int number;
            int.TryParse(match.Groups[2].Value, out number);
            Number = number; //will get default value when blank
        }

        public string OriginalString { get; private set; }
        public string Name { get; private set; }
        public int Number { get; private set; }
    }






}