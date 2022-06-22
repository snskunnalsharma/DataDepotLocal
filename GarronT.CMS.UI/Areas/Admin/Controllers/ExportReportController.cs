using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using KSPL.Indus.MSTS.UI.Filters;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using GarronT.CMS.UI.CustomHelpers;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class ExportReportController : Controller
    {
        //
        // GET: /Admin/ExportReport/
        public static List<ExportReport_CompletedRecords> tempObjList = new List<ExportReport_CompletedRecords>();
        public ActionResult Index()
        {
            long currentLoggedInUserID = GetLoggedInUserID();
            TempData["currentLoggedInUserID"] = currentLoggedInUserID;
            return View();
        }


        //******fetch data dropdownlist******
        public JsonResult GetPoject(long status)
        {
            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                string proStatus = "";
                if (status == 2)
                {
                    proStatus = "Complete";
                }
                else
                {
                    proStatus = "Active";
                }
                var data = exportReportDAL.GetProjectName(proStatus);

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
        }

        public long GetLoggedInUserID()
        {
            var vr = User.Identity.Name;
            CompassEntities AIBCARMEntities = new CompassEntities();
            var userid = AIBCARMEntities.tblUsers.Where(x => x.UserName == vr).Select(x => x.UserID).FirstOrDefault();
            return userid;
        }

        public JsonResult GetFormType()
        {
            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                var data = exportReportDAL.GetFormTypeList();
                return base.Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFormName(long projectId, long formTypeId)
        {
            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                List<tblFormMaster> objList1 = exportReportDAL.GetActiveFormName(projectId, formTypeId);
                return Json(objList1, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFormTypeFields(long projectId, long formTypeId)
        {
            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var data = exportReportDAL.GetFieldsOnFormType(formTypeId, projectId, int.Parse(currentUserId.ToString()));
                return base.Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public class arraysourcefield
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        public class arrayDestinationfield
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        public class arrayReportLablefield
        {
            public string value { get; set; }
            public string text { get; set; }
        }



        /// <summary>
        /// Saves source fields & destination fields for the form of the project.
        /// </summary>
        /// <param name="arrFieldDetail"></param>
        /// <param name="arrFieldDetail1"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        public ActionResult SaveFields(List<arraysourcefield> arrFieldDetail, List<arrayDestinationfield> arrFieldDetail1, List<arrayReportLablefield> arrFieldDetail2, long formId)
        {

            try
            {
                using (ExportReportDAL exportReportDAL = new ExportReportDAL())
                {
                    DateTime currentDate = new CommonFunctions().ServerDate();


                    List<TblExportReport> ReportList = new List<TblExportReport>();
                    long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    var currentFieldsInfo = exportReportDAL.GetFormFields(formId, int.Parse(currentUserId.ToString()));

                    if (arrFieldDetail != null && arrFieldDetail.Count > 0)
                    {
                        foreach (var item in arrFieldDetail)
                        {
                            var currentField = currentFieldsInfo.Where(o => o.FieldName == item.text).FirstOrDefault();

                            TblExportReport exportreport = new TblExportReport()
                            {
                                FormId = formId,
                                FieldId = currentField.IsExcelMapFieldFlag == 1 ? currentField.TableId : currentField.FieldId,
                                FieldName = item.text,
                                Flag = 0,
                                SortOrder = 0,
                                Active = 1,
                                CreatedOn = currentDate
                            };

                            ReportList.Add(exportreport);

                        }
                    }
                    int i = 1;
                    var lableCnt = arrFieldDetail2.Count;
                    foreach (var item in arrFieldDetail1)
                    {
                        var currentField = currentFieldsInfo.Where(o => o.FieldName == item.text).FirstOrDefault();
                        var reportFieldName = "";
                        if (lableCnt == 0)
                        {
                            reportFieldName = item.text;
                        }
                        else
                        {
                            reportFieldName = getReportLableName(item.value, arrFieldDetail2);
                        }
                        TblExportReport exportreport1 = new TblExportReport()
                        {
                            FormId = formId,
                            //FieldId = Convert.ToInt32(item.value),
                            FieldId = currentField.IsExcelMapFieldFlag == 1 ? currentField.TableId : currentField.FieldId,
                            FieldName = item.text,
                            ReportFieldName = reportFieldName,
                            Flag = 1,
                            SortOrder = i,
                            Active = 1,
                            CreatedOn = currentDate,
                            ModifiedOn = currentDate
                        };
                        ReportList.Add(exportreport1);
                        i++;
                    }
                    bool success = exportReportDAL.SaveData(ReportList);
                    return Json(new { success = success }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public string getReportLableName(string fieldId, List<arrayReportLablefield> arrLable)
        {
            string lableName = "";
            foreach (var item in arrLable)
            {
                if (item.value.Equals(fieldId))
                {
                    lableName = item.text;
                }
            }
            return lableName;
        }

        public ActionResult SaveFieldsNew(List<arraysourcefield> arrFieldDetail, List<arrayDestinationfield> arrFieldDetail1, List<arrayReportLablefield> arrFieldDetail2, long formTypeId, long projectId)
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("ExportReport", "Add");

            var strMessage = "";
            if (!objPermissionCheck)
            {
                strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
            }
            else
            {
                long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                try
                {
                    using (ExportReportDAL exportReportDAL = new ExportReportDAL())
                    {
                        DateTime currentDate = new CommonFunctions().ServerDate();
                        List<TblExportReport> ReportList = new List<TblExportReport>();

                        if (arrFieldDetail != null && arrFieldDetail.Count > 0)
                        {

                            ReportList.AddRange(arrFieldDetail.Select(o => new TblExportReport()
                            {
                                FKProjectId = projectId,
                                FKFormTypeId = formTypeId,
                                FieldName = o.text,
                                ReportFieldName = o.text,
                                Flag = 0,
                                SortOrder = 0,
                                Active = 1,
                                CreatedOn = currentDate,
                                CreatedBy = int.Parse(currentUserId.ToString())
                            }).ToList());

                        }
                        int i = 1;
                        var lableCnt = arrFieldDetail2 != null ? arrFieldDetail2.Count :0;
                        //foreach (var item in arrFieldDetail1)
                        //{
                        /*ReportList.AddRange(arrFieldDetail1.Select(o => new TblExportReport()
                        {
                            FKProjectId = projectId,
                            FKFormTypeId = formTypeId,
                            FieldName = o.text,
                            ReportFieldName = lableCnt > 0 ? arrFieldDetail2.Where(m => m.value == o.value).FirstOrDefault().text != null ? arrFieldDetail2.Where(m => m.value == o.value).FirstOrDefault().text : o.text : o.text,
                            Flag = 1,
                            SortOrder = i++,
                            Active = 1,
                            CreatedOn = currentDate,
                            CreatedBy = int.Parse(currentUserId.ToString())

                        }).ToList());*/

                        foreach (var item in arrFieldDetail1)
                        {
                            var reportFieldName = "";
                            if (lableCnt == 0)
                            {
                                reportFieldName = item.text;
                            }
                            else
                            {
                                reportFieldName = getReportLableName(item.value, arrFieldDetail2);
                                if (reportFieldName.Equals(""))
                                {
                                    reportFieldName = item.text;
                                }
                            }
                            TblExportReport exportreport1 = new TblExportReport()
                            {
                                FKProjectId = projectId,
                                FKFormTypeId = formTypeId,
                                FieldName = item.text,
                                ReportFieldName = reportFieldName,
                                Flag = 1,
                                SortOrder = i++,
                                Active = 1,
                                CreatedOn = currentDate,
                                CreatedBy = int.Parse(currentUserId.ToString())
                            };
                            ReportList.Add(exportreport1);
                            i++;
                        }

                        objPermissionCheck = exportReportDAL.SaveDataNew(ReportList, int.Parse(currentUserId.ToString()));

                    }
                }
                catch (Exception ex)
                {
                    objPermissionCheck = false;
                    strMessage = ex.Message;
                }

            }
            return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);




        }


        /// <summary>
        /// Resturns the form fields for the form & also the excel mapped fields for the project
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        public ActionResult GetFormField(long formId)
        {

            long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                List<ExportReportBO> obj = exportReportDAL.GetFormFields(formId, int.Parse(currentUserId.ToString()));
                return Json(new { objList = obj }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="visitStatus"></param>
        /// <param name="latlongRequired"></param>
        /// <param name="mediaRequired"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult createExcel(string fromDate, string toDate, long formId, int visitStatus, int latlongRequired, int mediaRequired)
        {
            try
            {
                ExportReportDAL exportReportDAL = new ExportReportDAL();

                var checkIfFieldSavedBeforExport = exportReportDAL.checkFieldSavedForExport(formId);

                if (!checkIfFieldSavedBeforExport)
                {
                    return Json(new { status = false, ErrorMessage = "Please save the report fields first & then try...!" }, JsonRequestBehavior.AllowGet);
                }

                DateTime _fromDate = Convert.ToDateTime(fromDate);
                DateTime _toDate = Convert.ToDateTime(toDate);
                tblProject objProject;
                string FormName;
                List<string> columnNameList1;


                if (visitStatus == 5)
                {
                    #region completed
                    DataTable dt = getCompletedRecordDataTable(formId, _fromDate, _toDate, latlongRequired, mediaRequired, out objProject, out FormName, out columnNameList1);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // string json = ConvertDataTabletoString(dt);
                        TempData["ExportDataList"] = dt;
                        return Json(new
                        {
                            status = true,
                            //objColumnName = columnNameList1.ToArray(),
                            //dt = json,
                            //ProDetails =
                            //    new
                            //    {
                            //        ProjectName = objProject.ProjectName,
                            //        ProjectStatus = objProject.ProjectStatus,
                            //        FromDate = Convert.ToString(objProject.FromDate.Value.ToString("MMM-dd-yyyy")),
                            //        ToDate = Convert.ToString(objProject.ToDate.Value.ToString("MMM-dd-yyyy")),
                            //        Utilitytype = objProject.Utilitytype,
                            //        FormName = FormName
                            //    }
                        },
                            JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, ErrorMessage = "No data avilable for selected date range." }, JsonRequestBehavior.AllowGet);
                    }
                    #endregion
                }
                else if (visitStatus == 1)
                {
                    #region Not Allocated
                    DataTable dt = getNotAllocatedRecordDataTable(formId, out FormName, out objProject, out columnNameList1);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        //string json = ConvertDataTabletoString(dt);
                        TempData["ExportDataList"] = dt;
                        return Json(new
                        {
                            status = true,
                            //objColumnName = columnNameList1.ToArray(),
                            //dt = json,
                            //ProDetails =
                            //    new
                            //    {
                            //        ProjectName = objProject.ProjectName,
                            //        ProjectStatus = objProject.ProjectStatus,
                            //        FromDate = Convert.ToString(objProject.FromDate.Value.ToString("MMM-dd-yyyy")),
                            //        ToDate = Convert.ToString(objProject.ToDate.Value.ToString("MMM-dd-yyyy")),
                            //        Utilitytype = objProject.Utilitytype,
                            //        FormName = FormName
                            //    }
                        },
                            JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, ErrorMessage = "No data avilable for selected date range." }, JsonRequestBehavior.AllowGet);
                    }
                    #endregion
                }
                else if (visitStatus == 2)
                {
                    #region Pending
                    DataTable dt = getPendingRecordDataTable(formId, _fromDate, _toDate, out FormName, out objProject, out columnNameList1);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        //string json = ConvertDataTabletoString(dt);
                        TempData["ExportDataList"] = dt;
                        return Json(new
                        {
                            status = true,
                            //objColumnName = columnNameList1.ToArray(),
                            //dt = json,
                            //ProDetails =
                            //    new
                            //    {
                            //        ProjectName = objProject.ProjectName,
                            //        ProjectStatus = objProject.ProjectStatus,
                            //        FromDate = Convert.ToString(objProject.FromDate.Value.ToString("MMM-dd-yyyy")),
                            //        ToDate = Convert.ToString(objProject.ToDate.Value.ToString("MMM-dd-yyyy")),
                            //        Utilitytype = objProject.Utilitytype,
                            //        FormName = FormName
                            //    }
                        },
                            JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, ErrorMessage = "No data avilable for selected date range." }, JsonRequestBehavior.AllowGet);
                    }
                    #endregion
                }
                else if (visitStatus == 3 || visitStatus == 4)
                {
                    #region Skip & RTU
                    // string FormName;
                    bool isSkiporRtu = visitStatus == 3 ? true : false;
                    DataTable dt = getSkipRtuRecordDataTable(formId, isSkiporRtu, _fromDate, _toDate, latlongRequired, out FormName, out objProject, out columnNameList1);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        //string json = ConvertDataTabletoString(dt);
                        TempData["ExportDataList"] = dt;
                        return Json(new
                        {
                            status = true,

                            //objColumnName = columnNameList1.ToArray(),
                            //dt = json,
                            //ProDetails =
                            //    new
                            //    {
                            //        ProjectName = objProject.ProjectName,
                            //        ProjectStatus = objProject.ProjectStatus,
                            //        FromDate = Convert.ToString(objProject.FromDate.Value.ToString("MMM-dd-yyyy")),
                            //        ToDate = Convert.ToString(objProject.ToDate.Value.ToString("MMM-dd-yyyy")),
                            //        Utilitytype = objProject.Utilitytype,
                            //        FormName = FormName
                            //    }
                        },
                            JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, ErrorMessage = "No data avilable for selected date range." }, JsonRequestBehavior.AllowGet);
                    }
                    #endregion
                }
                else
                {
                    #region All
                    DataTable dt = getAllRecordDataTable(formId, _fromDate, _toDate, latlongRequired, mediaRequired, out  objProject, out  FormName, out columnNameList1);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // string json = ConvertDataTabletoString(dt);

                        TempData["ExportDataList"] = dt;
                        return Json(new
                        {
                            status = true,
                            //objColumnName = columnNameList1.ToArray(),
                            //dt = json,
                            //ProDetails =
                            //    new
                            //    {
                            //        ProjectName = objProject.ProjectName,
                            //        ProjectStatus = objProject.ProjectStatus,
                            //        FromDate = Convert.ToString(objProject.FromDate.Value.ToString("MMM-dd-yyyy")),
                            //        ToDate = Convert.ToString(objProject.ToDate.Value.ToString("MMM-dd-yyyy")),
                            //        Utilitytype = objProject.Utilitytype,
                            //        FormName = FormName
                            //    }
                        },
                            JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, ErrorMessage = "No data avilable for selected date range." }, JsonRequestBehavior.AllowGet);
                    }

                    #endregion
                }

            }
            catch (Exception ex)
            {
                return Json(new { status = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public ActionResult createExcelNew(string fromDate, string toDate, long formTypeId, long projectId, string[] visitStatus, int latlongRequired, int mediaRequired)
        {
            try
            {
                ExportReportDAL exportReportDAL = new ExportReportDAL();
                long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var checkIfFieldSavedBeforExport = exportReportDAL.checkFieldSavedForExportFormType(formTypeId, projectId, int.Parse(currentUserId.ToString()));

                if (!checkIfFieldSavedBeforExport)
                {
                    return Json(new { status = false, ErrorMessage = "Please save the report fields first & then try...!" }, JsonRequestBehavior.AllowGet);
                }

                DateTime _fromDate = Convert.ToDateTime(fromDate);
                DateTime _toDate = Convert.ToDateTime(toDate);

                List<string> columnNameList1;



                #region All

                if (formTypeId == 2)
                {
                    //DataTable dt = AllRecordDataTable(formTypeId, projectId, visitStatus, _fromDate, _toDate, latlongRequired, mediaRequired, out columnNameList1);
                    DataTable dt = AllRecordDataTable(formTypeId, projectId, visitStatus, _fromDate, _toDate, latlongRequired, mediaRequired, out columnNameList1, currentUserId);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        TempData["ExportDataList"] = dt;
                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, ErrorMessage = "No data avilable for selected date range." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (formTypeId == 1)
                {
                    //DataTable dt = AllAuditRecordDataTable(formTypeId, projectId, visitStatus, _fromDate, _toDate, latlongRequired, mediaRequired, out columnNameList1);
                    DataTable dt = AllAuditRecordDataTable(formTypeId, projectId, visitStatus, _fromDate, _toDate, latlongRequired, mediaRequired, out columnNameList1, currentUserId);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        TempData["ExportDataList"] = dt;
                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, ErrorMessage = "No data available for selected date range." }, JsonRequestBehavior.AllowGet);
                    }


                }
                else
                {
                    return Json(new { status = false, ErrorMessage = "No data available for selected date range." }, JsonRequestBehavior.AllowGet);
                }


                #endregion


            }
            catch (Exception ex)
            {
                return Json(new { status = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region complete


        /// <summary>
        /// 
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="latlongRequired"></param>
        /// <param name="mediaRequired"></param>
        /// <param name="objProject"></param>
        /// <param name="FormName"></param>
        /// <param name="columnNameList1"></param>
        /// <returns></returns>
        public DataTable getCompletedRecordDataTable(long formId, DateTime fromDate, DateTime toDate, int latlongRequired, int mediaRequired,
            out tblProject objProject, out string FormName, out List<string> columnNameList1)
        {
            string _webUrl = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();
            List<STP_ExportReportData_Completed_Result> objList = new List<STP_ExportReportData_Completed_Result>();
            List<string> selectedColumnNameList = new List<string>();

            selectedColumnNameList.Add("ATTENDANT NAME");
            selectedColumnNameList.Add("VISIT ATTENDED DATETIME");
            selectedColumnNameList.Add("VISIT STATUS");


            columnNameList1 = new List<string>();

            columnNameList1.Add("ATTENDANT NAME");
            columnNameList1.Add("VISIT ATTENDED DATETIME");
            columnNameList1.Add("VISIT STATUS");


            DataTable dt = new DataTable();
            tblFormMaster objForm;
            List<STP_ExportReport_Service_Result> objServiceListForproject = null;
            List<STP_ExportReportData_Attachments_Result> objAttachmentListForForm = null;
            List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();
            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                objList = exportReportDAL.getReportData(formId, fromDate, toDate, out objForm, out objProject);
                DataTable ExcelUploadedData = exportReportDAL.getExcelUploadDataForCompleteInstallations(formId, fromDate, toDate, out objForm, out objProject);
                selectedColumnNameList = exportReportDAL.getColumnNameList(formId);

                ProjectAllFields = exportReportDAL.GetAllFieldsForProject(formId);
                objServiceListForproject = exportReportDAL.getListOfServices(formId);

                ///if media required
                if (mediaRequired == 2)
                {
                    objAttachmentListForForm = exportReportDAL.getAttachmentListForForm(formId);
                }
            }

            FormName = objForm.FormName;

            List<STP_ExportReport_Service_Result> objRegulorServiceList = new List<STP_ExportReport_Service_Result>();
            List<STP_ExportReport_Service_Result> objAdditionalServiceList = new List<STP_ExportReport_Service_Result>();

            //   List<tblUploadedData> objProjectDataList = new List<tblUploadedData>();
            DataTable dtProjectExcelData = new DataTable();
            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                var objListActiveServices = obj.GetActiveServiceRecords(objProject.ProjectId);
                var objListInActiveServices = obj.GetInActiveServiceRecords(objProject.ProjectId);
                //objProjectDataList = obj.GetActiveProjectData(objProject.ProjectId);
                dtProjectExcelData = new ExportReportDAL().GetDynamicActiveProjectData(formId, fromDate, toDate);
                foreach (var item in objServiceListForproject)
                {
                    var a = objListActiveServices.Where(o => o.ServiceName.Trim() == item.Service.Trim()).FirstOrDefault();
                    if (a == null)
                    {
                        objAdditionalServiceList.Add(item);
                    }
                    else
                    {
                        objRegulorServiceList.Add(item);
                    }
                }
            }



            if (selectedColumnNameList != null && selectedColumnNameList.Count > 0 && objList != null && objList.Count > 0)
            {

                dt.Columns.Add("ATTENDANT NAME", typeof(string));
                dt.Columns.Add("VISIT START DATE", typeof(string));
                dt.Columns.Add("VISIT END DATE", typeof(string));
                dt.Columns.Add("VISIT ATTENDED DATETIME", typeof(string));
                dt.Columns.Add("VISIT STATUS", typeof(string));
                //dt.Columns.Add("CYCLE", typeof(string));
                //dt.Columns.Add("ROUTE", typeof(string));

                //dt.Columns.Add("TblProjectFieldData_Id", typeof(string));


                //columnNameList1.Add("TblProjectFieldData_Id");

                //Create columns
                foreach (var item in selectedColumnNameList)
                {
                    #region columns
                    if (item.ToLower() == "SERVICES".ToLower() || item.ToLower() == "ADDITIONAL SERVICES".ToLower())
                    {
                        if (item.ToLower() == "ADDITIONAL SERVICES".ToLower())
                        {
                            #region Addition service
                            if (objAdditionalServiceList.Count > 0)
                            {
                                foreach (var item1 in objAdditionalServiceList)
                                {
                                    DataColumn dc = new DataColumn("Additional Service - " + item1.Service);
                                    dc.DataType = typeof(string);
                                    dt.Columns.Add(dc);
                                    columnNameList1.Add("Additional Service - " + item1.Service);

                                    ///if  media required for report
                                    if (mediaRequired == 2)
                                    {
                                        DataColumn dc1 = new DataColumn(item1.Service + " Attachments");
                                        dc1.DataType = typeof(string);
                                        dt.Columns.Add(dc1);
                                        columnNameList1.Add(item1.Service + " Attachments");
                                    }
                                }

                                ///if latlong required for the report
                                if (latlongRequired == 2)
                                {
                                    DataColumn dc2 = new DataColumn(item + " Lat-Lon");
                                    dc2.DataType = typeof(string);
                                    dt.Columns.Add(dc2);
                                    columnNameList1.Add(item + " Lat-Lon");
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region service
                            if (objRegulorServiceList.Count > 0)
                            {
                                foreach (var item1 in objRegulorServiceList)
                                {
                                    DataColumn dc = new DataColumn("Service - " + item1.Service);
                                    dc.DataType = typeof(string);
                                    dt.Columns.Add(dc);
                                    columnNameList1.Add("Service - " + item1.Service);

                                    ///if service media required
                                    if (mediaRequired == 2)
                                    {
                                        DataColumn dc1 = new DataColumn(item1.Service + " Attachments");
                                        dc1.DataType = typeof(string);
                                        dt.Columns.Add(dc1);
                                        columnNameList1.Add(item1.Service + " Attachments");
                                    }
                                }

                                //if lat long required for the service
                                if (latlongRequired == 2)
                                {
                                    DataColumn dc2 = new DataColumn(item + " Lat-Lon");
                                    dc2.DataType = typeof(string);
                                    dt.Columns.Add(dc2);
                                    columnNameList1.Add(item + " Lat-Lon");
                                }

                            }
                            #endregion
                        }
                    }
                    else
                    {


                        DataColumn dc = new DataColumn(item);
                        dc.DataType = typeof(string);
                        dt.Columns.Add(dc);
                        columnNameList1.Add(item);

                        #region lat long required
                        ///if lat long required for the service

                        if (latlongRequired == 2)
                        {
                            DataColumn dc1 = new DataColumn(item + " Lat-Lon");
                            dc1.DataType = typeof(string);
                            dt.Columns.Add(dc1);
                            columnNameList1.Add(item + " Lat-Lon");
                        }

                        #endregion
                    }

                    #endregion
                }

                var distinctId = objList.Select(o => o.TblProjectFieldData_Id).Distinct().ToList();

                foreach (var item in distinctId)
                {
                    #region data
                    DataRow dr = dt.NewRow();

                    var _data1 = objList.Where(o => o.TblProjectFieldData_Id == item).FirstOrDefault();
                    dr["ATTENDANT NAME"] = Convert.ToString(_data1.Installer_Name);
                    dr["VISIT START DATE"] = Convert.ToString(_data1.FromDate.Value.Date);
                    dr["VISIT END DATE"] = Convert.ToString(_data1.ToDate.Value.Date);
                    dr["VISIT ATTENDED DATETIME"] = Convert.ToString(_data1.visitdatetime);
                    dr["VISIT STATUS"] = "COMPLETED";

                    #region
                    foreach (var item1 in selectedColumnNameList)
                    {
                        if (item1.ToLower() == "SERVICES".ToLower() || item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                        {

                            if (item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                            {
                                #region Additional Service

                                if (objAdditionalServiceList.Count > 0)
                                {

                                    var _data = objList.Where(o => o.TblProjectFieldData_Id == item && o.InitialFieldName == item1).FirstOrDefault();
                                    string[] _attendedServices = !string.IsNullOrEmpty(_data.FieldValue) ? _data.FieldValue.Split('|').ToArray() : null;
                                    if (_attendedServices != null)
                                    {

                                        foreach (var stritem in _attendedServices)
                                        {
                                            var _serviceDetails = objAdditionalServiceList.Where(o => o.Id.ToString() == stritem).FirstOrDefault();
                                            if (_serviceDetails != null)
                                            {
                                                string Path = "File Links- ";
                                                if (objAttachmentListForForm != null && objAttachmentListForForm.Count > 0)
                                                {
                                                    var _attachments = objAttachmentListForForm.Where(o => o.TblProjectFieldData_Id == item && o.tblService_Id == _serviceDetails.Id).ToList();

                                                    foreach (var item3 in _attachments)
                                                    {
                                                        string _filePath = item3.strFilePath.Replace("..\\..", "").ToString();
                                                        _filePath = _filePath.Replace("~\\", "").ToString();
                                                        Path = Path + "\r\n" + _webUrl + _filePath;
                                                    }
                                                }

                                                dr["Additional Service - " + _serviceDetails.Service] = "Done";

                                                if (mediaRequired == 2)
                                                {
                                                    dr[_serviceDetails.Service + " Attachments"] = Path;
                                                }
                                            }
                                        }

                                        if (latlongRequired == 2 && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLatitude)) && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLongitude)))
                                        {
                                            dr[item1 + " Lat-Lon"] = Convert.ToString(_data.FieldLatitude) + "," + Convert.ToString(_data.FieldLongitude);

                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region Service
                                var _data = objList.Where(o => o.TblProjectFieldData_Id == item && o.InitialFieldName == item1).FirstOrDefault();
                                string[] _attendedServices = !string.IsNullOrEmpty(_data.FieldValue) ? _data.FieldValue.Split('|').ToArray() : null;
                                if (_attendedServices != null)
                                {

                                    foreach (var stritem in _attendedServices)
                                    {
                                        var _serviceDetails = objServiceListForproject.Where(o => o.Id.ToString() == stritem).FirstOrDefault();
                                        if (_serviceDetails != null)
                                        {

                                            string Path = "";
                                            if (objAttachmentListForForm != null && objAttachmentListForForm.Count > 0)
                                            {
                                                var _attachments = objAttachmentListForForm.Where(o => o.TblProjectFieldData_Id == item && o.tblService_Id == _serviceDetails.Id).ToList();
                                                foreach (var item3 in _attachments)
                                                {
                                                    string _filePath = item3.strFilePath.Replace("..\\..\\", "").ToString();
                                                    _filePath = _filePath.Replace("~\\", "").ToString();
                                                    Path = Path + _webUrl + _filePath.Replace("\\", "/").ToString() + "\r\n ";
                                                }
                                            }

                                            dr["Service - " + _serviceDetails.Service] = "Done";
                                            if (mediaRequired == 2)
                                            {
                                                dr[_serviceDetails.Service + " Attachments"] = Path;
                                            }
                                        }
                                    }
                                    if (latlongRequired == 2 && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLatitude)) && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLongitude)))
                                    {
                                        dr[item1 + " Lat-Lon"] = Convert.ToString(_data.FieldLatitude) + "," + Convert.ToString(_data.FieldLongitude);
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            var _data = objList.Where(o => o.TblProjectFieldData_Id == item && o.InitialFieldName == item1).FirstOrDefault();
                            if (_data != null)
                            {
                                //field belongs to form field
                                if (Convert.ToString(_data.FieldValue) == "true" || Convert.ToString(_data.FieldValue) == "false")
                                {
                                    dr[item1] = Convert.ToString(_data.FieldValue) == "true" ? "YES" : "NO";
                                }
                                else
                                {
                                    dr[item1] = Convert.ToString(_data.FieldValue);
                                }

                                if (latlongRequired == 2 && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLatitude)) && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLongitude)))
                                {
                                    dr[item1 + " Lat-Lon"] = Convert.ToString(_data.FieldLatitude) + "," + Convert.ToString(_data.FieldLongitude);
                                }
                            }
                            else
                            {
                                // o.Field<int?>("MobileResponse") != 1
                                var UploadDataRow = dtProjectExcelData.AsEnumerable().Where(o => o.Field<long>("Id") == _data1.ID).FirstOrDefault();

                                var _columnName = ProjectAllFields.Where(o => o.FieldName.ToLower() == item1.Trim().ToLower()
                                    && o.FKUploadedExcelDataDynamicColumn != null && o.isMappedToExcel == 1).FirstOrDefault();

                                if (_columnName != null && UploadDataRow != null)
                                {
                                    dr[item1] = Convert.ToString(UploadDataRow[_columnName.FKUploadedExcelDataDynamicColumn]);
                                }

                            }
                        }
                    }
                    #endregion

                    dt.Rows.Add(dr);
                    #endregion
                }
            }
            foreach (DataRow item in dt.Rows)
            {
                foreach (DataColumn item1 in dt.Columns)
                {
                    if (item[item1] == DBNull.Value)
                    {
                        item[item1] = "-";
                    }
                }
            }
            return dt;
        }


        public DataTable CompleteRecordDataTable(long formTypeId, long projectId, DateTime fromDate, DateTime toDate, int latlongRequired, int mediaRequired,
           out List<string> columnNameList1,long currentUserId)
        {
            string _webUrl = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();
            List<ExportReport_CompletedRecords> objList = new List<ExportReport_CompletedRecords>();
            List<string> ExportColumnNameList = new List<string>();

            ExportColumnNameList.Add("ATTENDANT NAME");
            ExportColumnNameList.Add("VISIT ATTENDED DATETIME");
            ExportColumnNameList.Add("VISIT STATUS");


            columnNameList1 = new List<string>();

            columnNameList1.Add("ATTENDANT NAME");
            columnNameList1.Add("VISIT ATTENDED DATETIME");
            columnNameList1.Add("VISIT STATUS");


            DataTable dt = new DataTable();

            List<PROC_ExportReport_Service_Result> objServiceListForproject = null;
            List<PROC_ExportReport_AttachmentData_Result> objAttachmentListForForm = null;
            List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();
            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {

                objList = exportReportDAL.GetCompletedRecords(formTypeId, projectId, fromDate, toDate);
                tempObjList = new List<ExportReport_CompletedRecords>(); // Define this list as Static 
                tempObjList = objList;
                //TempData["tempObjList"] = objList;  //<== Store data for TNMP CSV Export regarding
                // Remove line
                //DataTable ExcelUploadedData = exportReportDAL.GetExcelDataForCompleteRecords(formTypeId, projectId, fromDate, toDate);
                //long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                ExportColumnNameList = exportReportDAL.GetExportColumnNameList(formTypeId, projectId, int.Parse(currentUserId.ToString()));
                ProjectAllFields = exportReportDAL.GetAllFieldsOfProject(projectId);

                objServiceListForproject = exportReportDAL.GetListOfServices(formTypeId, projectId);

                ///if media required
                if (mediaRequired == 2)
                {
                    objAttachmentListForForm = exportReportDAL.GetAttachmentList(formTypeId, projectId);
                }
            }



            List<PROC_ExportReport_Service_Result> objRegulorServiceList = new List<PROC_ExportReport_Service_Result>();
            List<PROC_ExportReport_Service_Result> objAdditionalServiceList = new List<PROC_ExportReport_Service_Result>();


            DataTable dtProjectExcelData = new DataTable();
            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                var objListActiveServices = obj.GetActiveServiceRecords(projectId);
                var objListInActiveServices = obj.GetInActiveServiceRecords(projectId);


                dtProjectExcelData = new ExportReportDAL().GetProjectData(formTypeId, projectId, fromDate, toDate);

                foreach (var item in objServiceListForproject)
                {
                    var a = objListActiveServices.Where(o => o.ServiceName.Trim() == item.Service.Trim()).FirstOrDefault();
                    if (a == null)
                    {
                        objAdditionalServiceList.Add(item);
                    }
                    else
                    {
                        objRegulorServiceList.Add(item);
                    }
                }
            }



            if (ExportColumnNameList != null && ExportColumnNameList.Count > 0 && objList != null && objList.Count > 0)
            {

                dt.Columns.Add("ATTENDANT NAME", typeof(string));

                dt.Columns.Add("VISIT ATTENDED DATETIME", typeof(string));
                dt.Columns.Add("VISIT STATUS", typeof(string));


                //Create columns
                foreach (var item in ExportColumnNameList)
                {
                    #region columns
                    if (item.ToLower() == "SERVICES".ToLower() || item.ToLower() == "ADDITIONAL SERVICES".ToLower())
                    {
                        if (item.ToLower() == "ADDITIONAL SERVICES".ToLower())
                        {
                            #region Addition service
                            if (objAdditionalServiceList.Count > 0)
                            {
                                foreach (var item1 in objAdditionalServiceList)
                                {
                                    DataColumn dc = new DataColumn("Additional Service - " + item1.Service);
                                    dc.DataType = typeof(string);
                                    dt.Columns.Add(dc);
                                    columnNameList1.Add("Additional Service - " + item1.Service);

                                    ///if  media required for report
                                    if (mediaRequired == 2)
                                    {
                                        DataColumn dc1 = new DataColumn(item1.Service + " Attachments");
                                        dc1.DataType = typeof(string);
                                        dt.Columns.Add(dc1);
                                        columnNameList1.Add(item1.Service + " Attachments");
                                    }
                                }

                                ///if latlong required for the report
                                if (latlongRequired == 2)
                                {
                                    DataColumn dc2 = new DataColumn(item + " Lat-Lon");
                                    dc2.DataType = typeof(string);
                                    dt.Columns.Add(dc2);
                                    columnNameList1.Add(item + " Lat-Lon");
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region service
                            if (objRegulorServiceList.Count > 0)
                            {
                                foreach (var item1 in objRegulorServiceList)
                                {
                                    DataColumn dc = new DataColumn("Service - " + item1.Service);
                                    dc.DataType = typeof(string);
                                    dt.Columns.Add(dc);
                                    columnNameList1.Add("Service - " + item1.Service);

                                    ///if service media required
                                    if (mediaRequired == 2)
                                    {
                                        DataColumn dc1 = new DataColumn(item1.Service + " Attachments");
                                        dc1.DataType = typeof(string);
                                        dt.Columns.Add(dc1);
                                        columnNameList1.Add(item1.Service + " Attachments");
                                    }
                                }

                                //if lat long required for the service
                                if (latlongRequired == 2)
                                {
                                    DataColumn dc2 = new DataColumn(item + " Lat-Lon");
                                    dc2.DataType = typeof(string);
                                    dt.Columns.Add(dc2);
                                    columnNameList1.Add(item + " Lat-Lon");
                                }

                            }
                            #endregion
                        }
                    }
                    else
                    {


                        DataColumn dc = new DataColumn(item);
                        dc.DataType = typeof(string);
                        dt.Columns.Add(dc);
                        columnNameList1.Add(item);

                        #region lat long required
                        ///if lat long required for the service

                        if (latlongRequired == 2)
                        {
                            DataColumn dc1 = new DataColumn(item + " Lat-Lon");
                            dc1.DataType = typeof(string);
                            dt.Columns.Add(dc1);
                            columnNameList1.Add(item + " Lat-Lon");
                        }

                        #endregion
                    }

                    #endregion
                }

                dt.Columns.Add("Form Name", typeof(string));
                if (projectId == 10075) // This Condition applied garland Project as per client
                {
                    dt.Columns.Add("Meter Box Start Time", typeof(string));
                    dt.Columns.Add("Meter Box End Time", typeof(string));
                    dt.Columns.Add("Meter Box Time", typeof(string));
                }

                var distinctId = objList.Select(o => o.TblProjectFieldData_Id).Distinct().ToList();

                foreach (var item in distinctId)
                {
                    #region data
                    DataRow dr = dt.NewRow();

                    var _data1 = objList.Where(o => o.TblProjectFieldData_Id == item).FirstOrDefault();
                    dr["ATTENDANT NAME"] = Convert.ToString(_data1.Installer_Name);
                    dr["VISIT ATTENDED DATETIME"] = Convert.ToString(_data1.visitdatetime);
                    dr["VISIT STATUS"] = "COMPLETED";
                    dr["Form Name"] = Convert.ToString(_data1.FormName);

                    TimeSpan timeDiff = new TimeSpan();
                    DateTime start = new DateTime();
                    DateTime end = new DateTime();
                    if (projectId == 10075) // This Condition applied garland Project as per client
                    {
                        var tempMeterBoxData = objList.Where(o => o.TblProjectFieldData_Id == item).ToList();
                        string beforeTime = string.Empty;
                        string afterTime = string.Empty;
                        foreach (var objItem in tempMeterBoxData)
                        {
                            if (objItem.InitialFieldName.ToLower() == "METER BOX BEFORE PHOTO".ToLower())
                            {
                                beforeTime = objItem.CapturedDateTime.ToString();
                            }
                            if (objItem.InitialFieldName.ToLower() == "METER BOX AFTER PHOTO".ToLower())
                            {
                                afterTime = objItem.CapturedDateTime.ToString();
                            }
                        }
                        start = new DateTime();
                        end = new DateTime();
                        if (beforeTime != null && beforeTime != "" && afterTime != null && afterTime != "")
                        {
                            start = Convert.ToDateTime(beforeTime);
                            end = Convert.ToDateTime(afterTime);
                            timeDiff = TimeSpan.Parse(end.ToString("hh:mm:ss")) - TimeSpan.Parse(start.ToString("hh:mm:ss"));
                        }

                        dr["Meter Box Start Time"] = start.ToString("MMM-dd-yyyy hh:mm tt");
                        dr["Meter Box End Time"] = end.ToString("MMM-dd-yyyy hh:mm tt");
                        dr["Meter Box Time"] = timeDiff;
                    }


                    #region
                    foreach (var item1 in ExportColumnNameList)
                    {
                        if (item1.ToLower() == "SERVICES".ToLower() || item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                        {

                            if (item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                            {
                                #region Additional Service

                                if (objAdditionalServiceList.Count > 0)
                                {

                                    var _data = objList.Where(o => o.TblProjectFieldData_Id == item && o.InitialFieldName == item1).FirstOrDefault();
                                    string[] _attendedServices = !string.IsNullOrEmpty(_data.FieldValue) ? _data.FieldValue.Split('|').ToArray() : null;
                                    if (_attendedServices != null)
                                    {

                                        foreach (var stritem in _attendedServices)
                                        {
                                            var _serviceDetails = objAdditionalServiceList.Where(o => o.Id.ToString() == stritem).FirstOrDefault();
                                            if (_serviceDetails != null)
                                            {
                                                string Path = "File Links- ";
                                                if (objAttachmentListForForm != null && objAttachmentListForForm.Count > 0)
                                                {
                                                    var _attachments = objAttachmentListForForm.Where(o => o.TblProjectFieldData_Id == item && o.tblService_Id == _serviceDetails.Id).ToList();

                                                    foreach (var item3 in _attachments)
                                                    {
                                                        string _filePath = item3.strFilePath.Replace("..\\..", "").ToString();
                                                        _filePath = _filePath.Replace("~\\", "").ToString();
                                                        Path = Path + "\r\n" + _webUrl + _filePath;
                                                    }
                                                }

                                                dr["Additional Service - " + _serviceDetails.Service] = "Done";

                                                if (mediaRequired == 2)
                                                {
                                                    dr[_serviceDetails.Service + " Attachments"] = Path;
                                                }
                                            }
                                        }

                                        if (latlongRequired == 2 && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLatitude)) && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLongitude)))
                                        {
                                            dr[item1 + " Lat-Lon"] = Convert.ToString(_data.FieldLatitude) + "," + Convert.ToString(_data.FieldLongitude);

                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region Service
                                var _data = objList.Where(o => o.TblProjectFieldData_Id == item && o.InitialFieldName == item1).FirstOrDefault();
                                if (_data != null)
                                {
                                    string[] _attendedServices = !string.IsNullOrEmpty(_data.FieldValue) ? _data.FieldValue.Split('|').ToArray() : null;
                                    if (_attendedServices != null)
                                    {

                                        foreach (var stritem in _attendedServices)
                                        {
                                            var _serviceDetails = objServiceListForproject.Where(o => o.Id.ToString() == stritem).FirstOrDefault();
                                            if (_serviceDetails != null)
                                            {

                                                string Path = "";
                                                if (objAttachmentListForForm != null && objAttachmentListForForm.Count > 0)
                                                {
                                                    var _attachments = objAttachmentListForForm.Where(o => o.TblProjectFieldData_Id == item && o.tblService_Id == _serviceDetails.Id).ToList();
                                                    foreach (var item3 in _attachments)
                                                    {
                                                        string _filePath = item3.strFilePath.Replace("..\\..\\", "").ToString();
                                                        _filePath = _filePath.Replace("~\\", "").ToString();
                                                        Path = Path + _webUrl + _filePath.Replace("\\", "/").ToString() + "\r\n ";
                                                    }
                                                }

                                                dr["Service - " + _serviceDetails.Service] = "Done";
                                                if (mediaRequired == 2)
                                                {
                                                    dr[_serviceDetails.Service + " Attachments"] = Path;
                                                }
                                            }
                                        }
                                        if (latlongRequired == 2 && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLatitude)) && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLongitude)))
                                        {
                                            dr[item1 + " Lat-Lon"] = Convert.ToString(_data.FieldLatitude) + "," + Convert.ToString(_data.FieldLongitude);
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            var _data = objList.Where(o => o.TblProjectFieldData_Id == item && o.InitialFieldName == item1).FirstOrDefault();
                            if (_data != null)
                            {
                                //field belongs to form field
                                if (Convert.ToString(_data.FieldValue) == "true" || Convert.ToString(_data.FieldValue) == "false")
                                {
                                    dr[item1] = Convert.ToString(_data.FieldValue) == "true" ? "YES" : "NO";
                                }
                                else
                                {
                                    string Path = "";
                                    if (objAttachmentListForForm != null && objAttachmentListForForm.Count > 0)
                                    {
                                        var _attachments = objAttachmentListForForm.Where(o => o.Id == _data.ProjectFieldDataMasterID).ToList();

                                        foreach (var item3 in _attachments)
                                        {
                                            string _filePath = item3.strFilePath.Replace("..\\..\\", "").ToString();

                                            _filePath = _filePath.Replace("~\\", "").ToString();
                                            Path = Path + _webUrl + _filePath.Replace("\\", "/").ToString() + "\r\n ";
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(Path) && string.IsNullOrEmpty(Convert.ToString(_data.FieldValue)))
                                    {
                                        dr[item1] = Path;
                                    }
                                    else
                                    {
                                        dr[item1] = Convert.ToString(_data.FieldValue);
                                    }


                                }

                                if (latlongRequired == 2 && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLatitude)) && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLongitude)))
                                {
                                    dr[item1 + " Lat-Lon"] = Convert.ToString(_data.FieldLatitude) + "," + Convert.ToString(_data.FieldLongitude);
                                }
                            }
                            else
                            {
                                // o.Field<int?>("MobileResponse") != 1
                                var UploadDataRow = dtProjectExcelData.AsEnumerable().Where(o => o.Field<long>("Id") == _data1.ID).FirstOrDefault();

                                var _columnName = ProjectAllFields.Where(o => o.FieldName.ToLower() == item1.Trim().ToLower()
                                    && o.FKUploadedExcelDataDynamicColumn != null && o.isMappedToExcel == 1).FirstOrDefault();

                                if (_columnName != null && UploadDataRow != null)
                                {
                                    dr[item1] = Convert.ToString(UploadDataRow[_columnName.FKUploadedExcelDataDynamicColumn]);
                                }

                            }
                        }
                    }
                    #endregion

                    dt.Rows.Add(dr);
                    #endregion
                }
            }


            foreach (DataRow item in dt.Rows)
            {
                foreach (DataColumn item1 in dt.Columns)
                {
                    if (item[item1] == DBNull.Value)
                    {
                        item[item1] = "-";
                    }
                }
            }
             
            return dt;

        }


        public DataTable CompleteRecordDataTable_Audit(long formTypeId, long projectId, DateTime fromDate, DateTime toDate, int latlongRequired, int mediaRequired,
          out List<string> columnNameList1)
        {
            string _webUrl = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();
            List<ExportAuditDataComplete> objList = new List<ExportAuditDataComplete>();
            List<string> ExportColumnNameList = new List<string>();

            ExportColumnNameList.Add("ATTENDANT NAME");
            ExportColumnNameList.Add("VISIT ATTENDED DATETIME");
            ExportColumnNameList.Add("VISIT STATUS");


            columnNameList1 = new List<string>();

            columnNameList1.Add("ATTENDANT NAME");
            columnNameList1.Add("VISIT ATTENDED DATETIME");
            columnNameList1.Add("VISIT STATUS");


            DataTable dt = new DataTable();

            List<PROC_ExportReport_Service_Result> objServiceListForproject = null;
            List<PROC_ExportReport_AttachmentData_Audit_Result> objAttachmentListForForm = null;
            List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();
            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {

                objList = exportReportDAL.GetAuditCompletedRecords(formTypeId, projectId, fromDate, toDate);

                DataTable ExcelUploadedData = exportReportDAL.GetExcelDataForCompleteRecords(formTypeId, projectId, fromDate, toDate);
                long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                ExportColumnNameList = exportReportDAL.GetExportColumnNameList(formTypeId, projectId, int.Parse(currentUserId.ToString()));

                ProjectAllFields = exportReportDAL.GetAllFieldsOfProject(projectId);

                objServiceListForproject = exportReportDAL.GetListOfServices(formTypeId, projectId);

                ///if media required
                if (mediaRequired == 2)
                {
                    objAttachmentListForForm = exportReportDAL.GetAuditAttachmentList(formTypeId, projectId);
                }
            }



            List<PROC_ExportReport_Service_Result> objRegulorServiceList = new List<PROC_ExportReport_Service_Result>();
            List<PROC_ExportReport_Service_Result> objAdditionalServiceList = new List<PROC_ExportReport_Service_Result>();


            DataTable dtProjectExcelData = new DataTable();
            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                var objListActiveServices = obj.GetActiveServiceRecords(projectId);
                var objListInActiveServices = obj.GetInActiveServiceRecords(projectId);


                dtProjectExcelData = new ExportReportDAL().GetProjectData(formTypeId, projectId, fromDate, toDate);

                foreach (var item in objServiceListForproject)
                {
                    var a = objListActiveServices.Where(o => o.ServiceName.Trim() == item.Service.Trim()).FirstOrDefault();
                    if (a == null)
                    {
                        objAdditionalServiceList.Add(item);
                    }
                    else
                    {
                        objRegulorServiceList.Add(item);
                    }
                }
            }



            if (ExportColumnNameList != null && ExportColumnNameList.Count > 0 && objList != null && objList.Count > 0)
            {

                dt.Columns.Add("ATTENDANT NAME", typeof(string));
                dt.Columns.Add("VISIT ATTENDED DATETIME", typeof(string));
                dt.Columns.Add("VISIT STATUS", typeof(string));


                //Create columns
                foreach (var item in ExportColumnNameList)
                {
                    #region columns
                    if (item.ToLower() == "SERVICES".ToLower() || item.ToLower() == "ADDITIONAL SERVICES".ToLower())
                    {
                        if (item.ToLower() == "ADDITIONAL SERVICES".ToLower())
                        {
                            #region Addition service
                            if (objAdditionalServiceList.Count > 0)
                            {
                                foreach (var item1 in objAdditionalServiceList)
                                {
                                    DataColumn dc = new DataColumn("Additional Service - " + item1.Service);
                                    dc.DataType = typeof(string);
                                    dt.Columns.Add(dc);
                                    columnNameList1.Add("Additional Service - " + item1.Service);

                                    ///if  media required for report
                                    if (mediaRequired == 2)
                                    {
                                        DataColumn dc1 = new DataColumn(item1.Service + " Attachments");
                                        dc1.DataType = typeof(string);
                                        dt.Columns.Add(dc1);
                                        columnNameList1.Add(item1.Service + " Attachments");
                                    }
                                }

                                ///if latlong required for the report
                                if (latlongRequired == 2)
                                {
                                    DataColumn dc2 = new DataColumn(item + " Lat-Lon");
                                    dc2.DataType = typeof(string);
                                    dt.Columns.Add(dc2);
                                    columnNameList1.Add(item + " Lat-Lon");
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region service
                            if (objRegulorServiceList.Count > 0)
                            {
                                foreach (var item1 in objRegulorServiceList)
                                {
                                    DataColumn dc = new DataColumn("Service - " + item1.Service);
                                    dc.DataType = typeof(string);
                                    dt.Columns.Add(dc);
                                    columnNameList1.Add("Service - " + item1.Service);

                                    ///if service media required
                                    if (mediaRequired == 2)
                                    {
                                        DataColumn dc1 = new DataColumn(item1.Service + " Attachments");
                                        dc1.DataType = typeof(string);
                                        dt.Columns.Add(dc1);
                                        columnNameList1.Add(item1.Service + " Attachments");
                                    }
                                }

                                //if lat long required for the service
                                if (latlongRequired == 2)
                                {
                                    DataColumn dc2 = new DataColumn(item + " Lat-Lon");
                                    dc2.DataType = typeof(string);
                                    dt.Columns.Add(dc2);
                                    columnNameList1.Add(item + " Lat-Lon");
                                }

                            }
                            #endregion
                        }
                    }
                    else
                    {


                        DataColumn dc = new DataColumn(item);
                        dc.DataType = typeof(string);
                        dt.Columns.Add(dc);
                        columnNameList1.Add(item);

                        #region lat long required
                        ///if lat long required for the service

                        if (latlongRequired == 2)
                        {
                            DataColumn dc1 = new DataColumn(item + " Lat-Lon");
                            dc1.DataType = typeof(string);
                            dt.Columns.Add(dc1);
                            columnNameList1.Add(item + " Lat-Lon");
                        }

                        #endregion
                    }

                    #endregion
                }

                var distinctId = objList.Select(o => o.FK_ProjectAuditData).Distinct().ToList();

                foreach (var item in distinctId)
                {
                    #region data
                    DataRow dr = dt.NewRow();

                    var _data1 = objList.Where(o => o.FK_ProjectAuditData == item).FirstOrDefault();
                    dr["ATTENDANT NAME"] = Convert.ToString(_data1.Auditor_Name);
                    dr["VISIT ATTENDED DATETIME"] = Convert.ToString(_data1.visitdatetime);
                    dr["VISIT STATUS"] = "COMPLETED";

                    #region
                    foreach (var item1 in ExportColumnNameList)
                    {
                        if (item1.ToLower() == "SERVICES".ToLower() || item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                        {

                            if (item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                            {
                                #region Additional Service

                                if (objAdditionalServiceList.Count > 0)
                                {

                                    var _data = objList.Where(o => o.FK_ProjectAuditData == item && o.InitialFieldName == item1).FirstOrDefault();
                                    string[] _attendedServices = !string.IsNullOrEmpty(_data.FieldValue) ? _data.FieldValue.Split('|').ToArray() : null;
                                    if (_attendedServices != null)
                                    {

                                        foreach (var stritem in _attendedServices)
                                        {
                                            var _serviceDetails = objAdditionalServiceList.Where(o => o.Id.ToString() == stritem).FirstOrDefault();
                                            if (_serviceDetails != null)
                                            {
                                                string Path = "File Links- ";
                                                if (objAttachmentListForForm != null && objAttachmentListForForm.Count > 0)
                                                {
                                                    var _attachments = objAttachmentListForForm.Where(o => o.FK_ProjectAuditData == item && o.Fk_tblService_Id == _serviceDetails.Id).ToList();

                                                    foreach (var item3 in _attachments)
                                                    {
                                                        string _filePath = item3.StrFilePath.Replace("..\\..", "").ToString();
                                                        _filePath = _filePath.Replace("~\\", "").ToString();
                                                        Path = Path + "\r\n" + _webUrl + _filePath;
                                                    }
                                                }

                                                dr["Additional Service - " + _serviceDetails.Service] = "Done";

                                                if (mediaRequired == 2)
                                                {
                                                    dr[_serviceDetails.Service + " Attachments"] = Path;
                                                }
                                            }
                                        }

                                        if (latlongRequired == 2 && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLatitude)) && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLongitude)))
                                        {
                                            dr[item1 + " Lat-Lon"] = Convert.ToString(_data.FieldLatitude) + "," + Convert.ToString(_data.FieldLongitude);

                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region Service
                                var _data = objList.Where(o => o.FK_ProjectAuditData == item && o.InitialFieldName == item1).FirstOrDefault();
                                string[] _attendedServices = !string.IsNullOrEmpty(_data.FieldValue) ? _data.FieldValue.Split('|').ToArray() : null;
                                if (_attendedServices != null)
                                {

                                    foreach (var stritem in _attendedServices)
                                    {
                                        var _serviceDetails = objServiceListForproject.Where(o => o.Id.ToString() == stritem).FirstOrDefault();
                                        if (_serviceDetails != null)
                                        {

                                            string Path = "";
                                            if (objAttachmentListForForm != null && objAttachmentListForForm.Count > 0)
                                            {
                                                var _attachments = objAttachmentListForForm.Where(o => o.FK_ProjectAuditData == item && o.Fk_tblService_Id == _serviceDetails.Id).ToList();
                                                foreach (var item3 in _attachments)
                                                {
                                                    string _filePath = item3.StrFilePath.Replace("..\\..\\", "").ToString();
                                                    _filePath = _filePath.Replace("~\\", "").ToString();
                                                    Path = Path + _webUrl + _filePath.Replace("\\", "/").ToString() + "\r\n ";
                                                }
                                            }

                                            dr["Service - " + _serviceDetails.Service] = "Done";
                                            if (mediaRequired == 2)
                                            {
                                                dr[_serviceDetails.Service + " Attachments"] = Path;
                                            }
                                        }
                                    }
                                    if (latlongRequired == 2 && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLatitude)) && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLongitude)))
                                    {
                                        dr[item1 + " Lat-Lon"] = Convert.ToString(_data.FieldLatitude) + "," + Convert.ToString(_data.FieldLongitude);
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            var _data = objList.Where(o => o.FK_ProjectAuditData == item && o.InitialFieldName == item1).FirstOrDefault();
                            if (_data != null)
                            {
                                //field belongs to form field
                                if (Convert.ToString(_data.FieldValue) == "true" || Convert.ToString(_data.FieldValue) == "false")
                                {
                                    dr[item1] = Convert.ToString(_data.FieldValue) == "true" ? "YES" : "NO";
                                }
                                else
                                {
                                    dr[item1] = Convert.ToString(_data.FieldValue);
                                }

                                if (latlongRequired == 2 && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLatitude)) && !string.IsNullOrEmpty(Convert.ToString(_data.FieldLongitude)))
                                {
                                    dr[item1 + " Lat-Lon"] = Convert.ToString(_data.FieldLatitude) + "," + Convert.ToString(_data.FieldLongitude);
                                }



                                string Path = "";
                                if (objAttachmentListForForm != null && objAttachmentListForForm.Count > 0)
                                {
                                    var _attachments = objAttachmentListForForm.Where(o => o.Id == _data.ProjectFieldDataMasterID).ToList();

                                    foreach (var item3 in _attachments)
                                    {
                                        string _filePath = item3.StrFilePath.Replace("..\\..\\", "").ToString();
                                        _filePath = _filePath.Replace("~\\", "").ToString();
                                        Path = Path + _webUrl + _filePath.Replace("\\", "/").ToString() + "\r\n ";
                                    }
                                }
                                if (!string.IsNullOrEmpty(Path) && string.IsNullOrEmpty(Convert.ToString(_data.FieldValue)))
                                {
                                    dr[item1] = Path;
                                }
                                else
                                {
                                    dr[item1] = Convert.ToString(_data.FieldValue);
                                }

                            }
                            else
                            {
                                // o.Field<int?>("MobileResponse") != 1
                                var UploadDataRow = dtProjectExcelData.AsEnumerable().Where(o => o.Field<long>("Id") == _data1.ID).FirstOrDefault();

                                var _columnName = ProjectAllFields.Where(o => o.FieldName.ToLower() == item1.Trim().ToLower()
                                    && o.FKUploadedExcelDataDynamicColumn != null && o.isMappedToExcel == 1).FirstOrDefault();

                                if (_columnName != null && UploadDataRow != null)
                                {
                                    dr[item1] = Convert.ToString(UploadDataRow[_columnName.FKUploadedExcelDataDynamicColumn]);
                                }

                            }
                        }
                    }
                    #endregion

                    dt.Rows.Add(dr);
                    #endregion
                }
            }


            foreach (DataRow item in dt.Rows)
            {
                foreach (DataColumn item1 in dt.Columns)
                {
                    if (item[item1] == DBNull.Value)
                    {
                        item[item1] = "-";
                    }
                }
            }

            return dt;

        }

        #endregion

        #region Skip & RTU


        public DataTable getSkipRtuRecordDataTable(long formId, bool isSkiporRtu, DateTime fromDate, DateTime toDate, int latlongRequired, out string FormName,
      out tblProject objProject, out List<string> columnNameList1)
        {
            string _webUrl = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();
            //List<STP_ExportReportData_SkipRtu_Result> objList = new List<STP_ExportReportData_SkipRtu_Result>();

            DataTable dt = new DataTable();
            objProject = new tblProject();

            columnNameList1 = new List<string>();
            dt.Columns.Add("ATTENDANT NAME", typeof(string));
            dt.Columns.Add("VISIT START DATE", typeof(string));
            dt.Columns.Add("VISIT END DATE", typeof(string));
            dt.Columns.Add("VISIT ATTENDED DATETIME", typeof(string));
            dt.Columns.Add("VISIT STATUS", typeof(string));
            dt.Columns.Add("REASON", typeof(string));
            dt.Columns.Add("COMMENT", typeof(string));



            columnNameList1.Add("ATTENDANT NAME");
            columnNameList1.Add("VISIT START DATE");
            columnNameList1.Add("VISIT END DATE");
            columnNameList1.Add("VISIT ATTENDED DATETIME");
            columnNameList1.Add("VISIT STATUS");
            columnNameList1.Add("REASON");
            columnNameList1.Add("COMMENT");

            List<STP_ExportReport_Service_Result> objServiceListForproject = null;

            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                objServiceListForproject = exportReportDAL.getListOfServices(formId);
            }
            List<STP_ExportReport_Service_Result> objRegulorServiceList = new List<STP_ExportReport_Service_Result>();
            List<STP_ExportReport_Service_Result> objAdditionalServiceList = new List<STP_ExportReport_Service_Result>();

            // DataTable dtProjectExcelData = new DataTable();
            //dtProjectExcelData = new ExportReportDAL().GetDynamicActiveProjectData(formId, fromDate, toDate);





            DataTable dtSkipRtu = new DataTable();
            List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();
            List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields1 = new List<PROC_WebService_GetFieldInfoForProject_Result>();
            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                dtSkipRtu = exportReportDAL.getSKipRTUReportData(formId, fromDate, toDate, out FormName, out objProject);



                List<string> obj = new List<string>();
                long proId = objProject.ProjectId;

                GetServiceInfo(proId, objServiceListForproject, out objRegulorServiceList, out objAdditionalServiceList);

                ProjectAllFields = exportReportDAL.GetAllFieldsForProject(formId).Where(o => o.isMappedToExcel == 1 && o.FKUploadedExcelDataDynamicColumn != null).ToList();

                var columnNameList = exportReportDAL.getColumnNameList(formId);



                foreach (var item in ProjectAllFields)
                {
                    var checkFormColumns = columnNameList.Where(o => o == item.FieldName).FirstOrDefault();
                    if (checkFormColumns != null)
                    {
                        ProjectAllFields1.Add(item);
                    }
                }

                // to  set the sort order of columns
                foreach (var item1 in columnNameList)
                {
                    if (!dt.Columns.Contains(item1))
                    {
                        if (item1.ToLower() == "SERVICES".ToLower() || item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                        {
                            if (item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                            {
                                #region Addition service
                                if (objAdditionalServiceList.Count > 0)
                                {
                                    foreach (var item2 in objAdditionalServiceList)
                                    {
                                        DataColumn dc = new DataColumn("Additional Service - " + item2.Service);
                                        dc.DataType = typeof(string);
                                        dt.Columns.Add(dc);
                                        columnNameList1.Add("Additional Service - " + item2.Service);

                                        ///if  media required for report
                                        //if (mediaRequired == 2)
                                        //{
                                        //    DataColumn dc1 = new DataColumn(item2.Service + " Attachments");
                                        //    dc1.DataType = typeof(string);
                                        //    dt.Columns.Add(dc1);
                                        //    columnNameList1.Add(item2.Service + " Attachments");
                                        //}
                                    }

                                    ///if latlong required for the report
                                    if (latlongRequired == 2)
                                    {
                                        DataColumn dc2 = new DataColumn(item1 + " Lat-Lon");
                                        dc2.DataType = typeof(string);
                                        dt.Columns.Add(dc2);
                                        columnNameList1.Add(item1 + " Lat-Lon");
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region service
                                if (objRegulorServiceList.Count > 0)
                                {
                                    foreach (var item2 in objRegulorServiceList)
                                    {
                                        DataColumn dc = new DataColumn("Service - " + item2.Service);
                                        dc.DataType = typeof(string);
                                        dt.Columns.Add(dc);
                                        columnNameList1.Add("Service - " + item2.Service);

                                        ///if service media required
                                        //if (mediaRequired == 2)
                                        //{
                                        //    DataColumn dc1 = new DataColumn(item2.Service + " Attachments");
                                        //    dc1.DataType = typeof(string);
                                        //    dt.Columns.Add(dc1);
                                        //    columnNameList1.Add(item2.Service + " Attachments");
                                        //}
                                    }

                                    //if lat long required for the service
                                    if (latlongRequired == 2)
                                    {
                                        DataColumn dc2 = new DataColumn(item1 + " Lat-Lon");
                                        dc2.DataType = typeof(string);
                                        dt.Columns.Add(dc2);
                                        columnNameList1.Add(item1 + " Lat-Lon");
                                    }

                                }
                                #endregion
                            }
                        }
                        else
                        {
                            dt.Columns.Add(item1, typeof(string));
                            columnNameList1.Add(item1);
                        }
                    }
                }


                if (dtSkipRtu != null && dtSkipRtu.Rows.Count > 0)
                {

                    int countNoOfRecords = dtSkipRtu.AsEnumerable().Where(o => o.Field<string>("CategoryType") == (isSkiporRtu == true ? "SKIP" : "RTU")).Count();

                    if (countNoOfRecords > 0)
                    {
                        dtSkipRtu = dtSkipRtu.AsEnumerable().Where(o => o.Field<string>("CategoryType") == (isSkiporRtu == true ? "SKIP" : "RTU")).CopyToDataTable();

                        foreach (DataRow item in dtSkipRtu.Rows)
                        {
                            #region
                            DataRow dr = dt.NewRow();

                            dr["ATTENDANT NAME"] = Convert.ToString(item["Installer Name"]);
                            dr["VISIT START DATE"] = DateTime.Parse(Convert.ToString(item["FromDate"])).Date.ToString();
                            dr["VISIT END DATE"] = DateTime.Parse(Convert.ToString(item["ToDate"])).Date.ToString();
                            dr["VISIT ATTENDED DATETIME"] = Convert.ToString(item["visitdatetime"]);
                            dr["VISIT STATUS"] = isSkiporRtu == true ? "SKIP" : "RTU";
                            dr["REASON"] = Convert.ToString(item["SkipReason"]);
                            dr["COMMENT"] = Convert.ToString(item["SkipComment"]);


                            foreach (var item1 in ProjectAllFields1)
                            {
                                dr[item1.FieldName] = Convert.ToString(item[item1.FKUploadedExcelDataDynamicColumn]);
                            }

                            dt.Rows.Add(dr);
                            #endregion
                        }
                    }
                }
            }
            foreach (DataRow item in dt.Rows)
            {
                foreach (DataColumn item1 in dt.Columns)
                {
                    if (item[item1] == DBNull.Value)
                    {
                        item[item1] = "-";
                    }
                }
            }
            return dt;
        }



        public DataTable GetSkipRtuDataTable(long formTypeId, long projectId, bool isSkiporRtu, DateTime fromDate, DateTime toDate, int latlongRequired, out List<string> columnNameList1)
        {
            string _webUrl = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();
            //List<STP_ExportReportData_SkipRtu_Result> objList = new List<STP_ExportReportData_SkipRtu_Result>();

            DataTable dt = new DataTable();


            columnNameList1 = new List<string>();
            columnNameList1.Add("ATTENDANT NAME");
            columnNameList1.Add("VISIT ATTENDED DATETIME");
            columnNameList1.Add("VISIT STATUS");
            columnNameList1.Add("REASON");
            columnNameList1.Add("COMMENT");


            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {


                DataTable dtSkipRtu = exportReportDAL.GetSKipRTURecordData(formTypeId, projectId, fromDate, toDate);

                if (dtSkipRtu != null && dtSkipRtu.Rows.Count > 0)
                {

                    dt.Columns.Add("ATTENDANT NAME", typeof(string));
                    dt.Columns.Add("VISIT ATTENDED DATETIME", typeof(string));
                    dt.Columns.Add("VISIT STATUS", typeof(string));
                    dt.Columns.Add("REASON", typeof(string));
                    dt.Columns.Add("COMMENT", typeof(string));





                    List<PROC_ExportReport_Service_Result> objServiceListForproject = exportReportDAL.GetListOfServices(formTypeId, projectId);

                    List<PROC_ExportReport_Service_Result> objRegulorServiceList = new List<PROC_ExportReport_Service_Result>();
                    List<PROC_ExportReport_Service_Result> objAdditionalServiceList = new List<PROC_ExportReport_Service_Result>();

                    // DataTable dtProjectExcelData = new DataTable();
                    //dtProjectExcelData = new ExportReportDAL().GetDynamicActiveProjectData(formId, fromDate, toDate);



                    List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();
                    List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields1 = new List<PROC_WebService_GetFieldInfoForProject_Result>();





                    List<string> obj = new List<string>();


                    GetServiceInfoNew(projectId, objServiceListForproject, out objRegulorServiceList, out objAdditionalServiceList);

                    ProjectAllFields = exportReportDAL.GetAllFieldsOfProject(projectId).Where(o => o.isMappedToExcel == 1 && o.FKUploadedExcelDataDynamicColumn != null).ToList();
                    long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    var ExportColumnNameList = exportReportDAL.GetExportColumnNameList(formTypeId, projectId, int.Parse(currentUserId.ToString()));



                    foreach (var item in ProjectAllFields)
                    {
                        var checkFormColumns = ExportColumnNameList.Where(o => o == item.FieldName).FirstOrDefault();
                        if (checkFormColumns != null)
                        {
                            ProjectAllFields1.Add(item);
                        }
                    }

                    // to  set the sort order of columns
                    foreach (var item1 in ExportColumnNameList)
                    {
                        if (!dt.Columns.Contains(item1))
                        {
                            if (item1.ToLower() == "SERVICES".ToLower() || item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                            {
                                if (item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                                {
                                    #region Addition service
                                    if (objAdditionalServiceList.Count > 0)
                                    {
                                        foreach (var item2 in objAdditionalServiceList)
                                        {
                                            DataColumn dc = new DataColumn("Additional Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Additional Service - " + item2.Service);

                                            ///if  media required for report
                                            //if (mediaRequired == 2)
                                            //{
                                            //    DataColumn dc1 = new DataColumn(item2.Service + " Attachments");
                                            //    dc1.DataType = typeof(string);
                                            //    dt.Columns.Add(dc1);
                                            //    columnNameList1.Add(item2.Service + " Attachments");
                                            //}
                                        }

                                        ///if latlong required for the report
                                        if (latlongRequired == 2)
                                        {
                                            DataColumn dc2 = new DataColumn(item1 + " Lat-Lon");
                                            dc2.DataType = typeof(string);
                                            dt.Columns.Add(dc2);
                                            columnNameList1.Add(item1 + " Lat-Lon");
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region service
                                    if (objRegulorServiceList.Count > 0)
                                    {
                                        foreach (var item2 in objRegulorServiceList)
                                        {
                                            DataColumn dc = new DataColumn("Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Service - " + item2.Service);

                                            ///if service media required
                                            //if (mediaRequired == 2)
                                            //{
                                            //    DataColumn dc1 = new DataColumn(item2.Service + " Attachments");
                                            //    dc1.DataType = typeof(string);
                                            //    dt.Columns.Add(dc1);
                                            //    columnNameList1.Add(item2.Service + " Attachments");
                                            //}
                                        }

                                        //if lat long required for the service
                                        if (latlongRequired == 2)
                                        {
                                            DataColumn dc2 = new DataColumn(item1 + " Lat-Lon");
                                            dc2.DataType = typeof(string);
                                            dt.Columns.Add(dc2);
                                            columnNameList1.Add(item1 + " Lat-Lon");
                                        }

                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                dt.Columns.Add(item1, typeof(string));
                                columnNameList1.Add(item1);
                            }
                        }
                    }


                    if (dtSkipRtu != null && dtSkipRtu.Rows.Count > 0)
                    {

                        int countNoOfRecords = dtSkipRtu.AsEnumerable().Where(o => o.Field<string>("CategoryType") == (isSkiporRtu == true ? "SKIP" : "RTU")).Count();

                        if (countNoOfRecords > 0)
                        {
                            dtSkipRtu = dtSkipRtu.AsEnumerable().Where(o => o.Field<string>("CategoryType") == (isSkiporRtu == true ? "SKIP" : "RTU")).CopyToDataTable();

                            foreach (DataRow item in dtSkipRtu.Rows)
                            {
                                #region
                                DataRow dr = dt.NewRow();

                                dr["ATTENDANT NAME"] = Convert.ToString(item["Installer Name"]);
                                //dr["VISIT START DATE"] = DateTime.Parse(Convert.ToString(item["FromDate"])).Date.ToString();
                                // dr["VISIT END DATE"] = DateTime.Parse(Convert.ToString(item["ToDate"])).Date.ToString();
                                dr["VISIT ATTENDED DATETIME"] = Convert.ToString(item["visitdatetime"]);
                                dr["VISIT STATUS"] = isSkiporRtu == true ? "SKIP" : "RTU";
                                dr["REASON"] = Convert.ToString(item["SkipReason"]);
                                dr["COMMENT"] = Convert.ToString(item["SkipComment"]);


                                foreach (var item1 in ProjectAllFields1)
                                {
                                    dr[item1.FieldName] = Convert.ToString(item[item1.FKUploadedExcelDataDynamicColumn]);
                                }

                                dt.Rows.Add(dr);
                                #endregion
                            }
                        }
                    }
                }


            }


            foreach (DataRow item in dt.Rows)
            {
                foreach (DataColumn item1 in dt.Columns)
                {
                    if (item[item1] == DBNull.Value)
                    {
                        item[item1] = "-";
                    }
                }
            }
            return dt;
        }


        public DataTable GetAuditSkipRtuDataTable(long formTypeId, long projectId, bool isSkiporRtu, DateTime fromDate, DateTime toDate, int latlongRequired, out List<string> columnNameList1)
        {
            string _webUrl = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();

            DataTable dt = new DataTable();


            columnNameList1 = new List<string>();
            columnNameList1.Add("ATTENDANT NAME");
            columnNameList1.Add("VISIT ATTENDED DATETIME");
            columnNameList1.Add("VISIT STATUS");
            columnNameList1.Add("REASON");
            columnNameList1.Add("COMMENT");


            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {


                DataTable dtSkipRtu = exportReportDAL.GetSKipRTURecordData(formTypeId, projectId, fromDate, toDate);

                if (dtSkipRtu != null && dtSkipRtu.Rows.Count > 0)
                {

                    dt.Columns.Add("ATTENDANT NAME", typeof(string));
                    dt.Columns.Add("VISIT ATTENDED DATETIME", typeof(string));
                    dt.Columns.Add("VISIT STATUS", typeof(string));
                    dt.Columns.Add("REASON", typeof(string));
                    dt.Columns.Add("COMMENT", typeof(string));





                    List<PROC_ExportReport_Service_Result> objServiceListForproject = exportReportDAL.GetListOfServices(formTypeId, projectId);

                    List<PROC_ExportReport_Service_Result> objRegulorServiceList = new List<PROC_ExportReport_Service_Result>();
                    List<PROC_ExportReport_Service_Result> objAdditionalServiceList = new List<PROC_ExportReport_Service_Result>();

                    List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();
                    List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields1 = new List<PROC_WebService_GetFieldInfoForProject_Result>();





                    List<string> obj = new List<string>();


                    GetServiceInfoNew(projectId, objServiceListForproject, out objRegulorServiceList, out objAdditionalServiceList);

                    ProjectAllFields = exportReportDAL.GetAllFieldsOfProject(projectId).Where(o => o.isMappedToExcel == 1 && o.FKUploadedExcelDataDynamicColumn != null).ToList();
                    long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    var ExportColumnNameList = exportReportDAL.GetExportColumnNameList(formTypeId, projectId, int.Parse(currentUserId.ToString()));



                    foreach (var item in ProjectAllFields)
                    {
                        var checkFormColumns = ExportColumnNameList.Where(o => o == item.FieldName).FirstOrDefault();
                        if (checkFormColumns != null)
                        {
                            ProjectAllFields1.Add(item);
                        }
                    }

                    var checkInstallerNameColumns = ExportColumnNameList.Where(o => o == "INSTALLER NAME").FirstOrDefault();
                    var checkInstallerVisitColumns = ExportColumnNameList.Where(o => o == "VISIT DATETIME").FirstOrDefault();

                    // to  set the sort order of columns
                    foreach (var item1 in ExportColumnNameList)
                    {
                        if (!dt.Columns.Contains(item1))
                        {
                            if (item1.ToLower() == "SERVICES".ToLower() || item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                            {
                                if (item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                                {
                                    #region Addition service
                                    if (objAdditionalServiceList.Count > 0)
                                    {
                                        foreach (var item2 in objAdditionalServiceList)
                                        {
                                            DataColumn dc = new DataColumn("Additional Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Additional Service - " + item2.Service);


                                        }

                                        ///if latlong required for the report
                                        if (latlongRequired == 2)
                                        {
                                            DataColumn dc2 = new DataColumn(item1 + " Lat-Lon");
                                            dc2.DataType = typeof(string);
                                            dt.Columns.Add(dc2);
                                            columnNameList1.Add(item1 + " Lat-Lon");
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region service
                                    if (objRegulorServiceList.Count > 0)
                                    {
                                        foreach (var item2 in objRegulorServiceList)
                                        {
                                            DataColumn dc = new DataColumn("Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Service - " + item2.Service);


                                        }

                                        //if lat long required for the service
                                        if (latlongRequired == 2)
                                        {
                                            DataColumn dc2 = new DataColumn(item1 + " Lat-Lon");
                                            dc2.DataType = typeof(string);
                                            dt.Columns.Add(dc2);
                                            columnNameList1.Add(item1 + " Lat-Lon");
                                        }

                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                dt.Columns.Add(item1, typeof(string));
                                columnNameList1.Add(item1);
                            }
                        }
                    }


                    if (dtSkipRtu != null && dtSkipRtu.Rows.Count > 0)
                    {

                        int countNoOfRecords = dtSkipRtu.AsEnumerable().Where(o => o.Field<string>("CategoryType") == (isSkiporRtu == true ? "SKIP" : "RTU")).Count();

                        if (countNoOfRecords > 0)
                        {
                            dtSkipRtu = dtSkipRtu.AsEnumerable().Where(o => o.Field<string>("CategoryType") == (isSkiporRtu == true ? "SKIP" : "RTU")).CopyToDataTable();

                            foreach (DataRow item in dtSkipRtu.Rows)
                            {
                                #region
                                DataRow dr = dt.NewRow();

                                dr["ATTENDANT NAME"] = Convert.ToString(item["Auditor Name"]);
                                dr["VISIT ATTENDED DATETIME"] = Convert.ToString(item["VisitDateTime"]);
                                dr["VISIT STATUS"] = isSkiporRtu == true ? "SKIP" : "RTU";
                                dr["REASON"] = Convert.ToString(item["SkipReason"]);
                                dr["COMMENT"] = Convert.ToString(item["SkipComment"]);


                                foreach (var item1 in ProjectAllFields1)
                                {
                                    dr[item1.FieldName] = Convert.ToString(item[item1.FKUploadedExcelDataDynamicColumn]);
                                }

                                if (checkInstallerNameColumns != null)
                                {
                                    dr[checkInstallerNameColumns] = Convert.ToString(item[checkInstallerNameColumns]);
                                }
                                if (checkInstallerVisitColumns != null)
                                {
                                    dr[checkInstallerVisitColumns] = Convert.ToString(item["VisitDate"]);
                                }
                                dt.Rows.Add(dr);
                                #endregion
                            }
                        }
                    }
                }


            }


            foreach (DataRow item in dt.Rows)
            {
                foreach (DataColumn item1 in dt.Columns)
                {
                    if (item[item1] == DBNull.Value)
                    {
                        item[item1] = "-";
                    }
                }
            }
            return dt;
        }

        #endregion

        #region Pending


        public DataTable getPendingRecordDataTable(long formId, DateTime fromDate, DateTime toDate, out string FormName,
      out tblProject objProject, out List<string> columnNameList1)
        {
            string _webUrl = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();


            DataTable dt = new DataTable();
            objProject = new tblProject();

            columnNameList1 = new List<string>();
            columnNameList1.Add("ATTENDANT NAME");
            columnNameList1.Add("VISIT START DATE");
            columnNameList1.Add("VISIT END DATE");
            columnNameList1.Add("VISIT ATTENDED DATETIME");
            columnNameList1.Add("VISIT STATUS");
            List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();
            List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields1 = new List<PROC_WebService_GetFieldInfoForProject_Result>();

            List<STP_ExportReport_Service_Result> objServiceListForproject = null;

            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                objServiceListForproject = exportReportDAL.getListOfServices(formId);
            }
            List<STP_ExportReport_Service_Result> objRegulorServiceList = new List<STP_ExportReport_Service_Result>();
            List<STP_ExportReport_Service_Result> objAdditionalServiceList = new List<STP_ExportReport_Service_Result>();

            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {

                DataTable dtPendingData = exportReportDAL.getPendingData(formId, fromDate, toDate, out FormName, out objProject);

                if (dtPendingData != null && dtPendingData.Rows.Count > 0)
                {
                    dt.Columns.Add("ATTENDANT NAME", typeof(string));
                    dt.Columns.Add("VISIT START DATE", typeof(string));
                    dt.Columns.Add("VISIT END DATE", typeof(string));
                    dt.Columns.Add("VISIT ATTENDED DATETIME", typeof(string));
                    dt.Columns.Add("VISIT STATUS", typeof(string));

                    GetServiceInfo(objProject.ProjectId, objServiceListForproject, out objRegulorServiceList, out objAdditionalServiceList);

                    ProjectAllFields = exportReportDAL.GetAllFieldsForProject(formId).Where(o => o.isMappedToExcel == 1 && o.FKUploadedExcelDataDynamicColumn != null).ToList();

                    var columnNameList = exportReportDAL.getColumnNameList(formId);

                    foreach (var item in ProjectAllFields)
                    {
                        var checkFormColumns = columnNameList.Where(o => o == item.FieldName).FirstOrDefault();
                        if (checkFormColumns != null)
                        {
                            ProjectAllFields1.Add(item);
                        }
                    }

                    // to  set the sort order of columns
                    foreach (var item1 in columnNameList)
                    {
                        if (!dt.Columns.Contains(item1))
                        {
                            if (item1.ToLower() == "SERVICES".ToLower() || item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                            {
                                if (item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                                {
                                    #region Addition service
                                    if (objAdditionalServiceList.Count > 0)
                                    {
                                        foreach (var item2 in objAdditionalServiceList)
                                        {
                                            DataColumn dc = new DataColumn("Additional Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Additional Service - " + item2.Service);

                                            ///if  media required for report
                                            //if (mediaRequired == 2)
                                            //{
                                            //    DataColumn dc1 = new DataColumn(item2.Service + " Attachments");
                                            //    dc1.DataType = typeof(string);
                                            //    dt.Columns.Add(dc1);
                                            //    columnNameList1.Add(item2.Service + " Attachments");
                                            //}
                                        }

                                        ///if latlong required for the report
                                        //if (latlongRequired == 2)
                                        //{
                                        //    DataColumn dc2 = new DataColumn(item1 + " Lat-Lon");
                                        //    dc2.DataType = typeof(string);
                                        //    dt.Columns.Add(dc2);
                                        //    columnNameList1.Add(item1 + " Lat-Lon");
                                        //}
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region service
                                    if (objRegulorServiceList.Count > 0)
                                    {
                                        foreach (var item2 in objRegulorServiceList)
                                        {
                                            DataColumn dc = new DataColumn("Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Service - " + item2.Service);

                                            ///if service media required
                                            //if (mediaRequired == 2)
                                            //{
                                            //    DataColumn dc1 = new DataColumn(item2.Service + " Attachments");
                                            //    dc1.DataType = typeof(string);
                                            //    dt.Columns.Add(dc1);
                                            //    columnNameList1.Add(item2.Service + " Attachments");
                                            //}
                                        }

                                        //if lat long required for the service
                                        //if (latlongRequired == 2)
                                        //{
                                        //    DataColumn dc2 = new DataColumn(item1 + " Lat-Lon");
                                        //    dc2.DataType = typeof(string);
                                        //    dt.Columns.Add(dc2);
                                        //    columnNameList1.Add(item1 + " Lat-Lon");
                                        //}

                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                dt.Columns.Add(item1, typeof(string));
                                columnNameList1.Add(item1);
                            }
                        }
                    }



                    foreach (DataRow item in dtPendingData.Rows)
                    {
                        #region
                        DataRow dr = dt.NewRow();
                        dr["VISIT START DATE"] = Convert.ToString(item["FromDate"]);
                        dr["VISIT END DATE"] = Convert.ToString(item["ToDate"]);
                        dr["VISIT ATTENDED DATETIME"] = "-";
                        dr["ATTENDANT NAME"] = Convert.ToString(item["Installer Name"]);
                        dr["VISIT STATUS"] = "PENDING";


                        foreach (var item1 in ProjectAllFields1)
                        {
                            dr[item1.FieldName] = Convert.ToString(item[item1.FKUploadedExcelDataDynamicColumn]);
                        }



                        dt.Rows.Add(dr);
                        #endregion
                    }
                }
            }
            foreach (DataRow item in dt.Rows)
            {
                foreach (DataColumn item1 in dt.Columns)
                {
                    if (item[item1] == DBNull.Value)
                    {
                        item[item1] = "-";
                    }
                }
            }
            return dt;
        }



        public DataTable PendingRecordDataTable(long formTypeId, long projectId, DateTime fromDate, DateTime toDate, out List<string> columnNameList1)
        {
            string _webUrl = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();


            DataTable dt = new DataTable();

            columnNameList1 = new List<string>();
            columnNameList1.Add("ATTENDANT NAME");
            columnNameList1.Add("VISIT ATTENDED DATETIME");
            columnNameList1.Add("VISIT STATUS");

            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                DataTable dtPendingData = exportReportDAL.PendingDataFormType(formTypeId, projectId, fromDate, toDate);

                List<PROC_WebService_GetFieldInfoForProject_Result> ProjectFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();
                List<PROC_WebService_GetFieldInfoForProject_Result> ProjectExcelFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();

                List<PROC_ExportReport_Service_Result> objServiceListForproject = exportReportDAL.GetListOfServices(formTypeId, projectId);

                List<PROC_ExportReport_Service_Result> objRegulorServiceList = new List<PROC_ExportReport_Service_Result>();
                List<PROC_ExportReport_Service_Result> objAdditionalServiceList = new List<PROC_ExportReport_Service_Result>();

                if (dtPendingData != null && dtPendingData.Rows.Count > 0)
                {
                    dt.Columns.Add("ATTENDANT NAME", typeof(string));
                    dt.Columns.Add("VISIT ATTENDED DATETIME", typeof(string));
                    dt.Columns.Add("VISIT STATUS", typeof(string));

                    GetServiceInfoNew(projectId, objServiceListForproject, out objRegulorServiceList, out objAdditionalServiceList);

                    ProjectFields = exportReportDAL.GetAllFieldsOfProject(projectId).Where(o => o.isMappedToExcel == 1 && o.FKUploadedExcelDataDynamicColumn != null).ToList();
                    long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    var ExportColumnNameList = exportReportDAL.GetExportColumnNameList(formTypeId, projectId, int.Parse(currentUserId.ToString()));

                    foreach (var item in ProjectFields)
                    {
                        var checkFormColumns = ExportColumnNameList.Where(o => o == item.FieldName).FirstOrDefault();
                        if (checkFormColumns != null)
                        {
                            ProjectExcelFields.Add(item);
                        }
                    }

                    // to  set the sort order of columns
                    foreach (var item1 in ExportColumnNameList)
                    {
                        if (!dt.Columns.Contains(item1))
                        {
                            if (item1.ToLower() == "SERVICES".ToLower() || item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                            {
                                if (item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                                {
                                    #region Addition service
                                    if (objAdditionalServiceList.Count > 0)
                                    {
                                        foreach (var item2 in objAdditionalServiceList)
                                        {
                                            DataColumn dc = new DataColumn("Additional Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Additional Service - " + item2.Service);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region service
                                    if (objRegulorServiceList.Count > 0)
                                    {
                                        foreach (var item2 in objRegulorServiceList)
                                        {
                                            DataColumn dc = new DataColumn("Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Service - " + item2.Service);
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                dt.Columns.Add(item1, typeof(string));
                                columnNameList1.Add(item1);
                            }
                        }
                    }



                    #region
                    foreach (DataRow item in dtPendingData.Rows)
                    {

                        DataRow dr = dt.NewRow();
                        dr["VISIT ATTENDED DATETIME"] = "-";
                        dr["ATTENDANT NAME"] = Convert.ToString(item["Installer Name"]);
                        dr["VISIT STATUS"] = "PENDING";


                        foreach (var item1 in ProjectExcelFields)
                        {
                            dr[item1.FieldName] = Convert.ToString(item[item1.FKUploadedExcelDataDynamicColumn]);
                        }

                        dt.Rows.Add(dr);

                    }
                    #endregion
                }
            }




            foreach (DataRow item in dt.Rows)
            {
                foreach (DataColumn item1 in dt.Columns)
                {
                    if (item[item1] == DBNull.Value)
                    {
                        item[item1] = "-";
                    }
                }
            }
            return dt;
        }



        public DataTable PendingAuditRecordDataTable(long formTypeId, long projectId, DateTime fromDate, DateTime toDate, out List<string> columnNameList1)
        {
            string _webUrl = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();


            DataTable dt = new DataTable();

            columnNameList1 = new List<string>();
            columnNameList1.Add("ATTENDANT NAME");
            columnNameList1.Add("VISIT ATTENDED DATETIME");
            columnNameList1.Add("VISIT STATUS");

            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                DataTable dtPendingData = exportReportDAL.PendingDataFormType(formTypeId, projectId, fromDate, toDate);

                List<PROC_WebService_GetFieldInfoForProject_Result> ProjectFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();
                List<PROC_WebService_GetFieldInfoForProject_Result> ProjectExcelFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();

                List<PROC_ExportReport_Service_Result> objServiceListForproject = exportReportDAL.GetListOfServices(formTypeId, projectId);

                List<PROC_ExportReport_Service_Result> objRegulorServiceList = new List<PROC_ExportReport_Service_Result>();
                List<PROC_ExportReport_Service_Result> objAdditionalServiceList = new List<PROC_ExportReport_Service_Result>();

                if (dtPendingData != null && dtPendingData.Rows.Count > 0)
                {
                    dt.Columns.Add("ATTENDANT NAME", typeof(string));
                    dt.Columns.Add("VISIT ATTENDED DATETIME", typeof(string));
                    dt.Columns.Add("VISIT STATUS", typeof(string));

                    GetServiceInfoNew(projectId, objServiceListForproject, out objRegulorServiceList, out objAdditionalServiceList);

                    ProjectFields = exportReportDAL.GetAllFieldsOfProject(projectId).Where(o => o.isMappedToExcel == 1 && o.FKUploadedExcelDataDynamicColumn != null).ToList();
                    long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    var ExportColumnNameList = exportReportDAL.GetExportColumnNameList(formTypeId, projectId, int.Parse(currentUserId.ToString()));

                    foreach (var item in ProjectFields)
                    {
                        var checkFormColumns = ExportColumnNameList.Where(o => o == item.FieldName).FirstOrDefault();
                        if (checkFormColumns != null)
                        {
                            ProjectExcelFields.Add(item);
                        }
                    }

                    var checkInstallerNameColumns = ExportColumnNameList.Where(o => o == "INSTALLER NAME").FirstOrDefault();
                    var checkInstallerVisitColumns = ExportColumnNameList.Where(o => o == "VISIT DATETIME").FirstOrDefault();


                    // to  set the sort order of columns
                    foreach (var item1 in ExportColumnNameList)
                    {
                        if (!dt.Columns.Contains(item1))
                        {
                            if (item1.ToLower() == "SERVICES".ToLower() || item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                            {
                                if (item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                                {
                                    #region Addition service
                                    if (objAdditionalServiceList.Count > 0)
                                    {
                                        foreach (var item2 in objAdditionalServiceList)
                                        {
                                            DataColumn dc = new DataColumn("Additional Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Additional Service - " + item2.Service);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region service
                                    if (objRegulorServiceList.Count > 0)
                                    {
                                        foreach (var item2 in objRegulorServiceList)
                                        {
                                            DataColumn dc = new DataColumn("Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Service - " + item2.Service);
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                dt.Columns.Add(item1, typeof(string));
                                columnNameList1.Add(item1);
                            }
                        }
                    }



                    #region
                    foreach (DataRow item in dtPendingData.Rows)
                    {

                        DataRow dr = dt.NewRow();
                        dr["VISIT ATTENDED DATETIME"] = "-";
                        dr["ATTENDANT NAME"] = Convert.ToString(item["Auditor Name"]);
                        dr["VISIT STATUS"] = "PENDING";


                        foreach (var item1 in ProjectExcelFields)
                        {
                            dr[item1.FieldName] = Convert.ToString(item[item1.FKUploadedExcelDataDynamicColumn]);
                        }

                        if (checkInstallerNameColumns != null)
                        {
                            dr[checkInstallerNameColumns] = Convert.ToString(item[checkInstallerNameColumns]);
                        }
                        if (checkInstallerVisitColumns != null)
                        {
                            dr[checkInstallerVisitColumns] = Convert.ToString(item["VisitDate"]);
                        }

                        dt.Rows.Add(dr);

                    }
                    #endregion
                }
            }




            foreach (DataRow item in dt.Rows)
            {
                foreach (DataColumn item1 in dt.Columns)
                {
                    if (item[item1] == DBNull.Value)
                    {
                        item[item1] = "-";
                    }
                }
            }
            return dt;
        }


        #endregion

        #region Not Allocated


        public DataTable getNotAllocatedRecordDataTable(long formId, out string FormName,
      out tblProject objProject, out List<string> columnNameList1)
        {
            string _webUrl = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();

            DataTable dt = new DataTable();
            objProject = new tblProject();

            columnNameList1 = new List<string>();
            columnNameList1.Add("ATTENDANT NAME");
            columnNameList1.Add("VISIT START DATE");
            columnNameList1.Add("VISIT END DATE");
            columnNameList1.Add("VISIT ATTENDED DATETIME");
            columnNameList1.Add("VISIT STATUS");

            dt.Columns.Add("ATTENDANT NAME", typeof(string));
            dt.Columns.Add("VISIT START DATE", typeof(string));
            dt.Columns.Add("VISIT END DATE", typeof(string));
            dt.Columns.Add("VISIT ATTENDED DATETIME", typeof(string));
            dt.Columns.Add("VISIT STATUS", typeof(string));


            List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();
            List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields1 = new List<PROC_WebService_GetFieldInfoForProject_Result>();

            List<STP_ExportReport_Service_Result> objServiceListForproject = null;

            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                objServiceListForproject = exportReportDAL.getListOfServices(formId);
            }
            List<STP_ExportReport_Service_Result> objRegulorServiceList = new List<STP_ExportReport_Service_Result>();
            List<STP_ExportReport_Service_Result> objAdditionalServiceList = new List<STP_ExportReport_Service_Result>();

            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {

                ProjectAllFields = exportReportDAL.GetAllFieldsForProject(formId).Where(o => o.isMappedToExcel == 1 && o.FKUploadedExcelDataDynamicColumn != null).ToList();

                GetServiceInfo(objProject.ProjectId, objServiceListForproject, out objRegulorServiceList, out objAdditionalServiceList);



                var columnNameList = exportReportDAL.getColumnNameList(formId);



                foreach (var item in ProjectAllFields)
                {
                    var checkFormColumns = columnNameList.Where(o => o == item.FieldName).FirstOrDefault();
                    if (checkFormColumns != null)
                    {
                        ProjectAllFields1.Add(item);
                    }
                }

                // to  set the sort order of columns
                foreach (var item1 in columnNameList)
                {
                    if (!dt.Columns.Contains(item1))
                    {

                        if (item1.ToLower() == "SERVICES".ToLower() || item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                        {
                            if (item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                            {
                                #region Addition service
                                if (objAdditionalServiceList.Count > 0)
                                {
                                    foreach (var item2 in objAdditionalServiceList)
                                    {
                                        DataColumn dc = new DataColumn("Additional Service - " + item2.Service);
                                        dc.DataType = typeof(string);
                                        dt.Columns.Add(dc);
                                        columnNameList1.Add("Additional Service - " + item2.Service);

                                        ///if  media required for report
                                        //if (mediaRequired == 2)
                                        //{
                                        //    DataColumn dc1 = new DataColumn(item2.Service + " Attachments");
                                        //    dc1.DataType = typeof(string);
                                        //    dt.Columns.Add(dc1);
                                        //    columnNameList1.Add(item2.Service + " Attachments");
                                        //}
                                    }

                                    ///if latlong required for the report
                                    //if (latlongRequired == 2)
                                    //{
                                    //    DataColumn dc2 = new DataColumn(item1 + " Lat-Lon");
                                    //    dc2.DataType = typeof(string);
                                    //    dt.Columns.Add(dc2);
                                    //    columnNameList1.Add(item1 + " Lat-Lon");
                                    //}
                                }
                                #endregion
                            }
                            else
                            {
                                #region service
                                if (objRegulorServiceList.Count > 0)
                                {
                                    foreach (var item2 in objRegulorServiceList)
                                    {
                                        DataColumn dc = new DataColumn("Service - " + item2.Service);
                                        dc.DataType = typeof(string);
                                        dt.Columns.Add(dc);
                                        columnNameList1.Add("Service - " + item2.Service);

                                        ///if service media required
                                        //if (mediaRequired == 2)
                                        //{
                                        //    DataColumn dc1 = new DataColumn(item2.Service + " Attachments");
                                        //    dc1.DataType = typeof(string);
                                        //    dt.Columns.Add(dc1);
                                        //    columnNameList1.Add(item2.Service + " Attachments");
                                        //}
                                    }

                                    //if lat long required for the service
                                    //if (latlongRequired == 2)
                                    //{
                                    //    DataColumn dc2 = new DataColumn(item1 + " Lat-Lon");
                                    //    dc2.DataType = typeof(string);
                                    //    dt.Columns.Add(dc2);
                                    //    columnNameList1.Add(item1 + " Lat-Lon");
                                    //}

                                }
                                #endregion
                            }
                        }
                        else
                        {
                            dt.Columns.Add(item1, typeof(string));
                            columnNameList1.Add(item1);
                        }
                    }
                }



                DataTable dtNotAllocatedData = exportReportDAL.getNotAllocatedData(formId, out FormName, out objProject);

                foreach (DataRow item in dtNotAllocatedData.Rows)
                {
                    #region
                    DataRow dr = dt.NewRow();

                    dr["VISIT STATUS"] = "NOT ALLOCATED";
                    dr["VISIT START DATE"] = "-";
                    dr["VISIT END DATE"] = "-";

                    foreach (var item1 in ProjectAllFields1)
                    {
                        dr[item1.FieldName] = Convert.ToString(item[item1.FKUploadedExcelDataDynamicColumn]);
                    }


                    dt.Rows.Add(dr);
                    #endregion
                }
            }


            foreach (DataRow item in dt.Rows)
            {
                foreach (DataColumn item1 in dt.Columns)
                {
                    if (item[item1] == DBNull.Value)
                    {
                        item[item1] = "-";
                    }
                }
            }
            return dt;
        }


        public DataTable NotAllocatedRecordDataTable(long formTypeId, long projectId, out List<string> columnNameList1)
        {
            string _webUrl = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();

            DataTable dt = new DataTable();


            columnNameList1 = new List<string>();
            columnNameList1.Add("ATTENDANT NAME");
            columnNameList1.Add("VISIT ATTENDED DATETIME");
            columnNameList1.Add("VISIT STATUS");

            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                DataTable dtNotAllocatedData = exportReportDAL.NotAllocatedData_FormType(projectId);

                if (dtNotAllocatedData != null && dtNotAllocatedData.Rows.Count > 0)
                {

                    dt.Columns.Add("ATTENDANT NAME", typeof(string));
                    dt.Columns.Add("VISIT ATTENDED DATETIME", typeof(string));
                    dt.Columns.Add("VISIT STATUS", typeof(string));


                    List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();
                    List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields1 = new List<PROC_WebService_GetFieldInfoForProject_Result>();

                    List<PROC_ExportReport_Service_Result> objServiceListForproject = exportReportDAL.GetListOfServices(formTypeId, projectId);


                    List<PROC_ExportReport_Service_Result> objRegulorServiceList = new List<PROC_ExportReport_Service_Result>();
                    List<PROC_ExportReport_Service_Result> objAdditionalServiceList = new List<PROC_ExportReport_Service_Result>();



                    GetServiceInfoNew(projectId, objServiceListForproject, out objRegulorServiceList, out objAdditionalServiceList);

                    ProjectAllFields = exportReportDAL.GetAllFieldsOfProject(projectId).
                        Where(o => o.isMappedToExcel == 1 && o.FKUploadedExcelDataDynamicColumn != null).ToList();

                    long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    var ExportColumnNameList = exportReportDAL.GetExportColumnNameList(formTypeId, projectId, int.Parse(currentUserId.ToString()));


                    foreach (var item in ProjectAllFields)
                    {
                        var checkFormColumns = ExportColumnNameList.Where(o => o == item.FieldName).FirstOrDefault();
                        if (checkFormColumns != null)
                        {
                            ProjectAllFields1.Add(item);
                        }
                    }

                    // to  set the sort order of columns
                    foreach (var item1 in ExportColumnNameList)
                    {
                        if (!dt.Columns.Contains(item1))
                        {

                            if (item1.ToLower() == "SERVICES".ToLower() || item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                            {
                                if (item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                                {
                                    #region Addition service
                                    if (objAdditionalServiceList.Count > 0)
                                    {
                                        foreach (var item2 in objAdditionalServiceList)
                                        {
                                            DataColumn dc = new DataColumn("Additional Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Additional Service - " + item2.Service);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region service
                                    if (objRegulorServiceList.Count > 0)
                                    {
                                        foreach (var item2 in objRegulorServiceList)
                                        {
                                            DataColumn dc = new DataColumn("Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Service - " + item2.Service);
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                dt.Columns.Add(item1, typeof(string));
                                columnNameList1.Add(item1);
                            }
                        }
                    }

                    #region

                    foreach (DataRow item in dtNotAllocatedData.Rows)
                    {

                        DataRow dr = dt.NewRow();

                        dr["VISIT STATUS"] = "NOT ALLOCATED";
                        // dr["VISIT START DATE"] = "-";
                        // dr["VISIT END DATE"] = "-";

                        foreach (var item1 in ProjectAllFields1)
                        {
                            dr[item1.FieldName] = Convert.ToString(item[item1.FKUploadedExcelDataDynamicColumn]);
                        }


                        dt.Rows.Add(dr);

                    }
                    #endregion
                }
            }


            foreach (DataRow item in dt.Rows)
            {
                foreach (DataColumn item1 in dt.Columns)
                {
                    if (item[item1] == DBNull.Value)
                    {
                        item[item1] = "-";
                    }
                }
            }
            return dt;
        }


        public DataTable NotAllocatedAuditRecordDataTable(long formTypeId, long projectId, out List<string> columnNameList1)
        {
            string _webUrl = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();

            DataTable dt = new DataTable();


            columnNameList1 = new List<string>();
            columnNameList1.Add("ATTENDANT NAME");
            columnNameList1.Add("VISIT ATTENDED DATETIME");
            columnNameList1.Add("VISIT STATUS");

            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                DataTable dtNotAllocatedData = exportReportDAL.NotAllocatedAuditData_FormType(projectId);

                if (dtNotAllocatedData != null && dtNotAllocatedData.Rows.Count > 0)
                {

                    dt.Columns.Add("ATTENDANT NAME", typeof(string));
                    dt.Columns.Add("VISIT ATTENDED DATETIME", typeof(string));
                    dt.Columns.Add("VISIT STATUS", typeof(string));


                    List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();
                    List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields1 = new List<PROC_WebService_GetFieldInfoForProject_Result>();

                    List<PROC_ExportReport_Service_Result> objServiceListForproject = exportReportDAL.GetListOfServices(formTypeId, projectId);


                    List<PROC_ExportReport_Service_Result> objRegulorServiceList = new List<PROC_ExportReport_Service_Result>();
                    List<PROC_ExportReport_Service_Result> objAdditionalServiceList = new List<PROC_ExportReport_Service_Result>();



                    GetServiceInfoNew(projectId, objServiceListForproject, out objRegulorServiceList, out objAdditionalServiceList);

                    ProjectAllFields = exportReportDAL.GetAllFieldsOfProject(projectId).
                        Where(o => o.isMappedToExcel == 1 && o.FKUploadedExcelDataDynamicColumn != null).ToList();

                    long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    var ExportColumnNameList = exportReportDAL.GetExportColumnNameList(formTypeId, projectId, int.Parse(currentUserId.ToString()));

                    foreach (var item in ProjectAllFields)
                    {
                        var checkFormColumns = ExportColumnNameList.Where(o => o == item.FieldName).FirstOrDefault();
                        if (checkFormColumns != null)
                        {
                            ProjectAllFields1.Add(item);
                        }
                    }

                    var checkInstallerNameColumns = ExportColumnNameList.Where(o => o == "INSTALLER NAME").FirstOrDefault();
                    var checkInstallerVisitColumns = ExportColumnNameList.Where(o => o == "VISIT DATETIME").FirstOrDefault();

                    // to  set the sort order of columns
                    foreach (var item1 in ExportColumnNameList)
                    {
                        if (!dt.Columns.Contains(item1))
                        {

                            if (item1.ToLower() == "SERVICES".ToLower() || item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                            {
                                if (item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                                {
                                    #region Addition service
                                    if (objAdditionalServiceList.Count > 0)
                                    {
                                        foreach (var item2 in objAdditionalServiceList)
                                        {
                                            DataColumn dc = new DataColumn("Additional Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Additional Service - " + item2.Service);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region service
                                    if (objRegulorServiceList.Count > 0)
                                    {
                                        foreach (var item2 in objRegulorServiceList)
                                        {
                                            DataColumn dc = new DataColumn("Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Service - " + item2.Service);
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                dt.Columns.Add(item1, typeof(string));
                                columnNameList1.Add(item1);
                            }
                        }
                    }

                    #region

                    foreach (DataRow item in dtNotAllocatedData.Rows)
                    {

                        DataRow dr = dt.NewRow();

                        dr["VISIT STATUS"] = "NOT ALLOCATED";
                        // dr["VISIT START DATE"] = "-";
                        // dr["VISIT END DATE"] = "-";

                        foreach (var item1 in ProjectAllFields1)
                        {
                            dr[item1.FieldName] = Convert.ToString(item[item1.FKUploadedExcelDataDynamicColumn]);
                        }

                        if (checkInstallerNameColumns != null)
                        {
                            dr[checkInstallerNameColumns] = Convert.ToString(item[checkInstallerNameColumns]);
                        }
                        if (checkInstallerVisitColumns != null)
                        {
                            dr[checkInstallerVisitColumns] = Convert.ToString(item[checkInstallerVisitColumns]);
                        }

                        dt.Rows.Add(dr);

                    }
                    #endregion
                }
            }


            foreach (DataRow item in dt.Rows)
            {
                foreach (DataColumn item1 in dt.Columns)
                {
                    if (item[item1] == DBNull.Value)
                    {
                        item[item1] = "-";
                    }
                }
            }
            return dt;
        }
        #endregion

        #region All


        /// <summary>
        /// 
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="latlongRequired"></param>
        /// <param name="mediaRequired"></param>
        /// <param name="objProject"></param>
        /// <param name="FormName"></param>
        /// <param name="columnNameList1"></param>
        /// <returns></returns>
        public DataTable getAllRecordDataTable(long formId, DateTime fromDate, DateTime toDate, int latlongRequired, int mediaRequired,
            out tblProject objProject, out string FormName, out List<string> columnNameList1)
        {
            DataTable dt = new DataTable();

            #region Complete



            dt = getCompletedRecordDataTable(formId, fromDate, toDate, latlongRequired, mediaRequired, out objProject, out FormName, out columnNameList1);

            SetCompleteRecordColumns(formId, ref dt, ref columnNameList1);

            #endregion


            #region SKip RTU




            List<string> skipColumns;
            List<string> RtuColumns;
            bool isSkiporRtu = true;
            DataTable dt1 = getSkipRtuRecordDataTable(formId, isSkiporRtu, fromDate, toDate, latlongRequired, out FormName, out objProject, out skipColumns);
            if (dt1 != null && dt1.Rows.Count > 0)
            {

                foreach (DataColumn dc in dt1.Columns)
                {
                    if (!dt.Columns.Contains(dc.ColumnName))
                    {
                        dt.Columns.Add(dc.ColumnName, typeof(string));
                        columnNameList1.Add(dc.ColumnName);
                    }
                }


                foreach (DataRow drdt1 in dt1.Rows)
                {
                    DataRow dr = dt.NewRow();
                    foreach (DataColumn dc in dt1.Columns)
                    {
                        dr[dc.ColumnName] = Convert.ToString(drdt1[dc]);
                    }
                    dt.Rows.Add(dr);
                }
            }
            isSkiporRtu = false;
            DataTable dt2 = getSkipRtuRecordDataTable(formId, isSkiporRtu, fromDate, toDate, latlongRequired, out FormName, out objProject, out RtuColumns);

            if (dt2 != null && dt2.Rows.Count > 0)
            {

                foreach (DataColumn dc in dt2.Columns)
                {
                    if (!dt.Columns.Contains(dc.ColumnName))
                    {
                        dt.Columns.Add(dc.ColumnName, typeof(string));
                        columnNameList1.Add(dc.ColumnName);
                    }
                }

                foreach (DataRow drdt1 in dt2.Rows)
                {
                    DataRow dr = dt.NewRow();
                    foreach (DataColumn dc in dt2.Columns)
                    {
                        dr[dc.ColumnName] = Convert.ToString(drdt1[dc]);
                    }
                    dt.Rows.Add(dr);
                }
            }




            #endregion


            #region Pending

            List<string> PendingColumns;
            DataTable dt3 = getPendingRecordDataTable(formId, fromDate, toDate, out FormName, out objProject, out PendingColumns);



            if (dt3 != null && dt3.Rows.Count > 0)
            {
                foreach (DataColumn dc in dt3.Columns)
                {
                    if (!dt.Columns.Contains(dc.ColumnName))
                    {
                        dt.Columns.Add(dc.ColumnName, typeof(string));
                        columnNameList1.Add(dc.ColumnName);
                    }
                }
                foreach (DataRow drdt1 in dt3.Rows)
                {
                    DataRow dr = dt.NewRow();
                    foreach (DataColumn dc in dt3.Columns)
                    {
                        dr[dc.ColumnName] = Convert.ToString(drdt1[dc]);
                    }
                    dt.Rows.Add(dr);
                }
            }


            #endregion


            #region Not Allocated

            List<string> NotAllocatedColumns;
            DataTable dt4 = getNotAllocatedRecordDataTable(formId, out FormName, out objProject, out NotAllocatedColumns);


            if (dt4 != null && dt4.Rows.Count > 0)
            {
                foreach (DataColumn dc in dt4.Columns)
                {
                    if (!dt.Columns.Contains(dc.ColumnName))
                    {
                        dt.Columns.Add(dc.ColumnName, typeof(string));
                        columnNameList1.Add(dc.ColumnName);
                    }
                }
                foreach (DataRow drdt1 in dt4.Rows)
                {
                    DataRow dr = dt.NewRow();
                    foreach (DataColumn dc in dt4.Columns)
                    {
                        dr[dc.ColumnName] = Convert.ToString(drdt1[dc]);
                    }
                    dt.Rows.Add(dr);
                }
            }


            #endregion


            foreach (DataRow item in dt.Rows)
            {
                foreach (DataColumn item1 in dt.Columns)
                {
                    if (item[item1] == DBNull.Value)
                    {
                        item[item1] = "-";
                    }
                }
            }
            return dt;
        }



        public DataTable AllRecordDataTable(long formTypeId, long projectId, string[] visitStatus, DateTime fromDate, DateTime toDate,
            int latlongRequired, int mediaRequired, out List<string> columnNameList1,long currentUserId)
        {
            TempData["tempProjectId"] = projectId;
            DataTable dt = new DataTable();
            columnNameList1 = new List<string>();
            DataTable ExportReportLableList = new DataTable();
            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                //long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                ExportReportLableList = exportReportDAL.GetExportColumnReportLableList(formTypeId, projectId, int.Parse(currentUserId.ToString()));
            }

            if (visitStatus.Contains("Completed"))
            {
                #region Complete

                dt = CompleteRecordDataTable(formTypeId, projectId, fromDate, toDate, latlongRequired, mediaRequired, out columnNameList1, currentUserId);

                #endregion
            }
            SetCompleteRecordColumnsFormType(formTypeId, projectId, ref dt, ref columnNameList1, currentUserId);


            List<string> skipColumns = new List<string>();
            List<string> RtuColumns = new List<string>();
            bool isSkiporRtu = true;
            if (visitStatus.Contains("Skip"))
            {
                #region skip

                DataTable dt1 = GetSkipRtuDataTable(formTypeId, projectId, isSkiporRtu, fromDate, toDate, latlongRequired, out skipColumns);
                if (dt1 != null && dt1.Rows.Count > 0)
                {


                    foreach (DataColumn dc in dt1.Columns)
                    {
                        if (!dt.Columns.Contains(dc.ColumnName))
                        {
                            dt.Columns.Add(dc.ColumnName, typeof(string));
                            columnNameList1.Add(dc.ColumnName);
                        }
                    }


                    foreach (DataRow drdt1 in dt1.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        foreach (DataColumn dc in dt1.Columns)
                        {
                            dr[dc.ColumnName] = Convert.ToString(drdt1[dc]);
                        }
                        dt.Rows.Add(dr);
                    }
                }


                #endregion
            }


            isSkiporRtu = false;


            if (visitStatus.Contains("RTU"))
            {
                #region rtu
                DataTable dt2 = GetSkipRtuDataTable(formTypeId, projectId, isSkiporRtu, fromDate, toDate, latlongRequired, out RtuColumns);

                if (dt2 != null && dt2.Rows.Count > 0)
                {

                    foreach (DataColumn dc in dt2.Columns)
                    {
                        if (!dt.Columns.Contains(dc.ColumnName))
                        {
                            dt.Columns.Add(dc.ColumnName, typeof(string));
                            columnNameList1.Add(dc.ColumnName);
                        }
                    }

                    foreach (DataRow drdt1 in dt2.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        foreach (DataColumn dc in dt2.Columns)
                        {
                            dr[dc.ColumnName] = Convert.ToString(drdt1[dc]);
                        }
                        dt.Rows.Add(dr);
                    }
                }
                #endregion
            }


            if (visitStatus.Contains("Pending"))
            {
                #region Pending
                List<string> PendingColumns;
                DataTable dt3 = PendingRecordDataTable(formTypeId, projectId, fromDate, toDate, out PendingColumns);



                if (dt3 != null && dt3.Rows.Count > 0)
                {
                    foreach (DataColumn dc in dt3.Columns)
                    {
                        if (!dt.Columns.Contains(dc.ColumnName))
                        {
                            dt.Columns.Add(dc.ColumnName, typeof(string));
                            columnNameList1.Add(dc.ColumnName);
                        }
                    }
                    foreach (DataRow drdt1 in dt3.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        foreach (DataColumn dc in dt3.Columns)
                        {
                            dr[dc.ColumnName] = Convert.ToString(drdt1[dc]);
                        }
                        dt.Rows.Add(dr);
                    }
                }
                #endregion
            }


            if (visitStatus.Contains("Not Allocated"))
            {
                #region Not Allocated

                List<string> NotAllocatedColumns;
                DataTable dt4 = NotAllocatedRecordDataTable(formTypeId, projectId, out NotAllocatedColumns);


                if (dt4 != null && dt4.Rows.Count > 0)
                {
                    foreach (DataColumn dc in dt4.Columns)
                    {
                        if (!dt.Columns.Contains(dc.ColumnName))
                        {
                            dt.Columns.Add(dc.ColumnName, typeof(string));
                            columnNameList1.Add(dc.ColumnName);
                        }
                    }
                    foreach (DataRow drdt1 in dt4.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        foreach (DataColumn dc in dt4.Columns)
                        {
                            dr[dc.ColumnName] = Convert.ToString(drdt1[dc]);
                        }
                        dt.Rows.Add(dr);
                    }
                }


                #endregion
            }

            CompassEntities objCE = new CompassEntities();
            var objUtilitytype = objCE.tblProjects.Where(o => o.ProjectId == projectId).Select(o => o.Utilitytype).FirstOrDefault();
            if (objUtilitytype == "Electric")
            {
                foreach (DataRow item in dt.Rows)
                {
                    foreach (DataColumn item1 in dt.Columns)
                    {
                        if (item[item1] == DBNull.Value || item[item1].ToString() == "-")
                        {
                            item[item1] = "";
                        }
                    }
                }
            }
            else
            {
                foreach (DataRow item in dt.Rows)
                {
                    foreach (DataColumn item1 in dt.Columns)
                    {
                        if (item[item1] == DBNull.Value)
                        {
                            item[item1] = "-";
                        }
                    }
                }
            }

            int cnt = 0;
            foreach (DataColumn column in dt.Columns)
            {
                string colname = column.ColumnName;
                for (int i = 0; i < ExportReportLableList.Rows.Count; i++)
                {
                    var labelName = ExportReportLableList.Rows[i]["ReportFieldName"].ToString();
                    var fieldName = ExportReportLableList.Rows[i]["FieldName"].ToString();
                    if (colname.Equals(fieldName))
                    {
                        dt.Columns[cnt].ColumnName = labelName;
                    }
                }
                cnt++;
            }

            return dt;
        }



        public DataTable AllAuditRecordDataTable(long formTypeId, long projectId, string[] visitStatus, DateTime fromDate, DateTime toDate,
            int latlongRequired, int mediaRequired, out List<string> columnNameList1, long currentUserId)
        {
            DataTable dt = new DataTable();
            columnNameList1 = new List<string>();
            if (visitStatus.Contains("Completed"))
            {
                #region Complete

                dt = CompleteRecordDataTable_Audit(formTypeId, projectId, fromDate, toDate, latlongRequired, mediaRequired, out columnNameList1);

                #endregion
            }

            SetCompleteRecordColumnsFormType(formTypeId, projectId, ref dt, ref columnNameList1, currentUserId);


            List<string> skipColumns = new List<string>();
            List<string> RtuColumns = new List<string>();
            bool isSkiporRtu = true;

            if (visitStatus.Contains("Skip"))
            {
                #region SKip



                DataTable dt1 = GetAuditSkipRtuDataTable(formTypeId, projectId, isSkiporRtu, fromDate, toDate, latlongRequired, out skipColumns);
                if (dt1 != null && dt1.Rows.Count > 0)
                {

                    foreach (DataColumn dc in dt1.Columns)
                    {
                        if (!dt.Columns.Contains(dc.ColumnName))
                        {
                            dt.Columns.Add(dc.ColumnName, typeof(string));
                            columnNameList1.Add(dc.ColumnName);
                        }
                    }


                    foreach (DataRow drdt1 in dt1.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        foreach (DataColumn dc in dt1.Columns)
                        {
                            dr[dc.ColumnName] = Convert.ToString(drdt1[dc]);
                        }
                        dt.Rows.Add(dr);
                    }
                }





                #endregion
            }

            isSkiporRtu = false;


            if (visitStatus.Contains("RTU"))
            {
                #region RTU

                DataTable dt2 = GetSkipRtuDataTable(formTypeId, projectId, isSkiporRtu, fromDate, toDate, latlongRequired, out RtuColumns);

                if (dt2 != null && dt2.Rows.Count > 0)
                {

                    foreach (DataColumn dc in dt2.Columns)
                    {
                        if (!dt.Columns.Contains(dc.ColumnName))
                        {
                            dt.Columns.Add(dc.ColumnName, typeof(string));
                            columnNameList1.Add(dc.ColumnName);
                        }
                    }

                    foreach (DataRow drdt1 in dt2.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        foreach (DataColumn dc in dt2.Columns)
                        {
                            dr[dc.ColumnName] = Convert.ToString(drdt1[dc]);
                        }
                        dt.Rows.Add(dr);
                    }
                }




                #endregion
            }


            if (visitStatus.Contains("Pending"))
            {
                #region Pending

                List<string> PendingColumns;
                DataTable dt3 = PendingAuditRecordDataTable(formTypeId, projectId, fromDate, toDate, out PendingColumns);



                if (dt3 != null && dt3.Rows.Count > 0)
                {
                    foreach (DataColumn dc in dt3.Columns)
                    {
                        if (!dt.Columns.Contains(dc.ColumnName))
                        {
                            dt.Columns.Add(dc.ColumnName, typeof(string));
                            columnNameList1.Add(dc.ColumnName);
                        }
                    }
                    foreach (DataRow drdt1 in dt3.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        foreach (DataColumn dc in dt3.Columns)
                        {
                            dr[dc.ColumnName] = Convert.ToString(drdt1[dc]);
                        }
                        dt.Rows.Add(dr);
                    }
                }


                #endregion
            }


            if (visitStatus.Contains("Not Allocated"))
            {
                #region Not Allocated

                List<string> NotAllocatedColumns;
                DataTable dt4 = NotAllocatedAuditRecordDataTable(formTypeId, projectId, out NotAllocatedColumns);


                if (dt4 != null && dt4.Rows.Count > 0)
                {
                    foreach (DataColumn dc in dt4.Columns)
                    {
                        if (!dt.Columns.Contains(dc.ColumnName))
                        {
                            dt.Columns.Add(dc.ColumnName, typeof(string));
                            columnNameList1.Add(dc.ColumnName);
                        }
                    }
                    foreach (DataRow drdt1 in dt4.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        foreach (DataColumn dc in dt4.Columns)
                        {
                            dr[dc.ColumnName] = Convert.ToString(drdt1[dc]);
                        }
                        dt.Rows.Add(dr);
                    }
                }


                #endregion
            }

            foreach (DataRow item in dt.Rows)
            {
                foreach (DataColumn item1 in dt.Columns)
                {
                    if (item[item1] == DBNull.Value)
                    {
                        item[item1] = "-";
                    }
                }
            }
            return dt;
        }

        #endregion

        public string ConvertDataTabletoString(System.Data.DataTable dt)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (System.Data.DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }

        public void SetCompleteRecordColumns(long formId, ref DataTable dt, ref List<string> columnNameList1)
        {
            if (!dt.Columns.Contains("ATTENDANT NAME"))
            {
                dt.Columns.Add("ATTENDANT NAME", typeof(string));

                if (!columnNameList1.Contains("ATTENDANT NAME"))
                {
                    columnNameList1.Add("ATTENDANT NAME");
                }
            }
            //if (!dt.Columns.Contains("VISIT START DATE"))
            //{
            //    dt.Columns.Add("VISIT START DATE", typeof(string));
            //    //  columnNameList1.Add("VISIT START DATE");
            //}
            //if (!dt.Columns.Contains("VISIT END DATE"))
            //{
            //    dt.Columns.Add("VISIT END DATE", typeof(string));
            //    //columnNameList1.Add("VISIT END DATE");
            //}

            if (!dt.Columns.Contains("VISIT ATTENDED DATETIME"))
            {
                dt.Columns.Add("VISIT ATTENDED DATETIME", typeof(string));
                if (!columnNameList1.Contains("VISIT ATTENDED DATETIME"))
                {
                    columnNameList1.Add("VISIT ATTENDED DATETIME");
                }
            }
            if (!dt.Columns.Contains("VISIT STATUS"))
            {
                dt.Columns.Add("VISIT STATUS", typeof(string));
                if (!columnNameList1.Contains("VISIT STATUS"))
                {
                    columnNameList1.Add("VISIT STATUS");
                }
            }

            if (!dt.Columns.Contains("REASON"))
            {
                dt.Columns.Add("REASON", typeof(string));
                columnNameList1.Add("REASON");
            }

            if (!dt.Columns.Contains("COMMENT"))
            {
                dt.Columns.Add("COMMENT", typeof(string));
                columnNameList1.Add("COMMENT");
            }


            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                List<ExportReportBO> obj = exportReportDAL.GetFormFields(formId, int.Parse(currentUserId.ToString())).Where(o => o.IsExcelMapFieldFlag == 1 && o.Flag == 1).ToList();
                if (obj != null && obj.Count > 0)
                {
                    foreach (var item in obj)
                    {
                        if (!dt.Columns.Contains(item.FieldName))
                        {
                            dt.Columns.Add(item.FieldName, typeof(string));
                            columnNameList1.Add(item.FieldName);
                        }
                    }
                }
            }

        }

        public void SetCompleteRecordColumnsFormType(long formTypeId, long projectId, ref DataTable dt, ref List<string> columnNameList1, long currentUserId)
        {
            if (!dt.Columns.Contains("ATTENDANT NAME"))
            {
                dt.Columns.Add("ATTENDANT NAME", typeof(string));
                if (!columnNameList1.Contains("ATTENDANT NAME"))
                {
                    columnNameList1.Add("ATTENDANT NAME");
                }
            }

            if (!dt.Columns.Contains("VISIT ATTENDED DATETIME"))
            {
                dt.Columns.Add("VISIT ATTENDED DATETIME", typeof(string));
                if (!columnNameList1.Contains("VISIT ATTENDED DATETIME"))
                {
                    columnNameList1.Add("VISIT ATTENDED DATETIME");
                }
            }
            if (!dt.Columns.Contains("VISIT STATUS"))
            {
                dt.Columns.Add("VISIT STATUS", typeof(string));
                if (!columnNameList1.Contains("VISIT STATUS"))
                {
                    columnNameList1.Add("VISIT STATUS");
                }
            }

            if (!dt.Columns.Contains("REASON"))
            {
                dt.Columns.Add("REASON", typeof(string));
                columnNameList1.Add("REASON");
            }

            if (!dt.Columns.Contains("COMMENT"))
            {
                dt.Columns.Add("COMMENT", typeof(string));
                columnNameList1.Add("COMMENT");
            }


            using (ExportReportDAL exportReportDAL = new ExportReportDAL())
            {
                //List<ExportReportBO> obj = exportReportDAL.GetFormFields(formId).Where(o => o.IsExcelMapFieldFlag == 1 && o.Flag == 1).ToList();
                //if (obj != null && obj.Count > 0)
                //{
                //    foreach (var item in obj)
                //    {
                //        if (!dt.Columns.Contains(item.FieldName))
                //        {
                //            dt.Columns.Add(item.FieldName, typeof(string));
                //            columnNameList1.Add(item.FieldName);
                //        }
                //    }
                //}


                List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields = new List<PROC_WebService_GetFieldInfoForProject_Result>();
                List<PROC_WebService_GetFieldInfoForProject_Result> ProjectAllFields1 = new List<PROC_WebService_GetFieldInfoForProject_Result>();

                List<PROC_ExportReport_Service_Result> objServiceListForproject = exportReportDAL.GetListOfServices(formTypeId, projectId);


                List<PROC_ExportReport_Service_Result> objRegulorServiceList = new List<PROC_ExportReport_Service_Result>();
                List<PROC_ExportReport_Service_Result> objAdditionalServiceList = new List<PROC_ExportReport_Service_Result>();



                GetServiceInfoNew(projectId, objServiceListForproject, out objRegulorServiceList, out objAdditionalServiceList);

                ProjectAllFields = exportReportDAL.GetAllFieldsOfProject(projectId).
                    Where(o => o.isMappedToExcel == 1 && o.FKUploadedExcelDataDynamicColumn != null).ToList();

                //long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var ExportColumnNameList = exportReportDAL.GetExportColumnNameList(formTypeId, projectId, int.Parse(currentUserId.ToString()));

                foreach (var item in ProjectAllFields)
                {
                    var checkFormColumns = ExportColumnNameList.Where(o => o == item.FieldName).FirstOrDefault();
                    if (checkFormColumns != null)
                    {
                        ProjectAllFields1.Add(item);
                    }
                }

                // to  set the sort order of columns
                foreach (var item1 in ExportColumnNameList)
                {
                    if (!dt.Columns.Contains(item1))
                    {

                        if (item1.ToLower() == "SERVICES".ToLower() || item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                        {
                            if (item1.ToLower() == "ADDITIONAL SERVICES".ToLower())
                            {
                                #region Addition service
                                if (objAdditionalServiceList.Count > 0)
                                {
                                    foreach (var item2 in objAdditionalServiceList)
                                    {
                                        if (!dt.Columns.Contains("Additional Service - " + item2.Service))
                                        {
                                            DataColumn dc = new DataColumn("Additional Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Additional Service - " + item2.Service);
                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region service
                                if (objRegulorServiceList.Count > 0)
                                {
                                    foreach (var item2 in objRegulorServiceList)
                                    {
                                        if (!dt.Columns.Contains("Service - " + item2.Service))
                                        {
                                            DataColumn dc = new DataColumn("Service - " + item2.Service);
                                            dc.DataType = typeof(string);
                                            dt.Columns.Add(dc);
                                            columnNameList1.Add("Service - " + item2.Service);
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            dt.Columns.Add(item1, typeof(string));
                            columnNameList1.Add(item1);
                        }
                    }
                }
            }

        }

        public ContentResult DownloadExcelExport()
        {

            ProjectMasterDAL projectMasterDAL = new ProjectMasterDAL();
            DataTable data = TempData["ExportDataList"] as DataTable;

            CompassEntities objCE = new CompassEntities();
            long projectId =long.Parse(TempData["tempProjectId"].ToString());
            var objUtilitytype = objCE.tblProjects.Where(o => o.ProjectId == projectId).Select(o => o.Utilitytype).FirstOrDefault();
            if (objUtilitytype == "Electric")
            {
                DownloadCSVExport(data, projectId);
                return null;
            }
            
            #region Export to excel
            GridView gv = new GridView();
            gv.DataSource = data;// data.OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();
            gv.DataBind();
            Response.Clear();
            Response.BufferOutput = true;
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AppendHeader("content-disposition", "attachment;filename=ExportReport.xlsx");
            System.Data.DataTable dt = data;//new CommonFunctions().ToDataTable(data.ToList());

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
            return null;
            //return RedirectToAction("Index");
            // return View("Index");
        }

        public void DownloadCSVExport(DataTable dt,long projectId)
        {
            try
            {
                Response.Clear();
                Response.Buffer = true;
                if (projectId == 10085)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=meter_exchange_CMS_" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
                }
                else if (projectId == 10087)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=meter_exchange_TNMP_" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
                }
                else
                {
                    Response.AddHeader("content-disposition", "attachment;filename=meter_exchange_CMS_" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
                }
                Response.Charset = "";
                Response.ContentType = "application/text";
                StringBuilder sb = new StringBuilder();
                //DataTable dt = Session["TillReportData"] as DataTable;
                // Hide/Remove columns from csv.
                //dt.Columns.RemoveAt(1); // Removes the column at the specified index
                dt = datatableColumnChnages(dt);     // Remove column not requied for TNMP project ==> Electric project
                //dt.Columns.Remove("ATTENDANT NAME"); // Removes DataColumn object that has the specified name
                //dt.Columns.Remove("VISIT ATTENDED DATETIME");
                //dt.Columns.Remove("VISIT STATUS");
                int totalCount = dt.Columns.Count;
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    if ((totalCount - 1) == k)
                    {
                        sb.Append(dt.Columns[k].ColumnName);    // Remove last comma(,) in row
                    }
                    else
                    {
                        sb.Append(dt.Columns[k].ColumnName + ',');
                    }
                }
                sb.Append("\r\n");
                totalCount = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //for (int k = 0; k < dt.Columns.Count; k++)
                    totalCount = dt.Columns.Count;
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        if ((totalCount - 1) == k)
                        {
                            sb.Append(dt.Rows[i][k].ToString());    // Remove last comma(,) in row
                        }
                        else
                        {
                            //sb.Append(dt.Rows[i][k].ToString().Replace(",", ";") + ',');
                            sb.Append(dt.Rows[i][k].ToString() + ',');
                        }
                        
                    }
                    sb.Append("\r\n");
                }
                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                
            }
            
        }

        public DataTable datatableColumnChnages(DataTable dt)
        {
            // Dtatable Column chnages applied for Electricty Project
            //CompassEntities objComEn = new CompassEntities();
            try
            {
                //List<ExportReport_CompletedRecords> tempObjList = new List<ExportReport_CompletedRecords>();
                //tempObjList = TempData["tempObjList"] as List<ExportReport_CompletedRecords>; // Define this list as Static 

                if (dt.Columns.Contains("ATTENDANT NAME"))
                {
                    dt.Columns["ATTENDANT NAME"].ColumnName = "v_installer"; // Column Name chnage
                }
                if (dt.Columns.Contains("VISIT ATTENDED DATETIME"))
                {
                    dt.Columns["VISIT ATTENDED DATETIME"].ColumnName = "v_completion_date"; // Column Name chnage
                }
                /*if (dt.Columns.Contains("VISIT STATUS"))
                {
                    dt.Columns.Remove("VISIT STATUS");  // Remove Column
                }*/
                if (dt.Columns.Contains("Form Name"))
                {
                    dt.Columns.Remove("Form Name");
                }
                if (dt.Columns.Contains("VISIT START DATE"))
                {
                    dt.Columns.Remove("VISIT START DATE");
                }
                if (dt.Columns.Contains("VISIT END DATE"))
                {
                    dt.Columns.Remove("VISIT END DATE");
                }
                if (dt.Columns.Contains("REASON"))
                {
                    dt.Columns.Remove("REASON");
                }
                if (dt.Columns.Contains("COMMENT"))
                {
                    dt.Columns.Remove("COMMENT");
                }
                dt.Columns.Add("V_NEW_LATITUDE");
                dt.AcceptChanges();
                dt.Columns.Add("V_NEW_LONGITUDE");
                dt.AcceptChanges();
                try
                {
                    for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        if (dt.Rows[k]["VISIT STATUS"].ToString().Equals("COMPLETED"))  // Complet date time formate chnage
                        {
                            DateTime obj = Convert.ToDateTime(dt.Rows[k]["v_completion_date"].ToString());
                            string CompletDate = obj.ToString("MMddyyyy HH:mm");
                            dt.Rows[k]["v_completion_date"] = CompletDate;
                        }
                        else
                        {
                            dt.Rows[k]["v_completion_date"] = "";
                        }

                        if (dt.Columns.Contains("V_CYCLE"))
                        {
                            string objCycle = dt.Rows[k]["V_CYCLE"].ToString();
                            string[] arrCycle = objCycle.Split(' ');   // Office text remove
                            dt.Rows[k]["V_CYCLE"] = (arrCycle[1].ToString()).Trim();
                        }

                        if (dt.Columns.Contains("UNIT"))
                        {
                            string objUnit = dt.Rows[k]["UNIT"].ToString();
                            string objStreet = dt.Rows[k]["STREET"].ToString();
                            if (!string.IsNullOrEmpty(objUnit))
                            {
                                int i = objUnit.Length + 1; // 1 Add because remove white space(In Steet and Unit merge time)
                                objStreet = objStreet.Remove(objStreet.Length - i);
                                dt.Rows[k]["STREET"] = $@"""{objStreet}""";     // Add Double qoute with string
                                dt.Rows[k]["UNIT"] = $@"""{objUnit}""";
                            }
                            else
                            {
                                dt.Rows[k]["STREET"] = $@"""{objStreet}""";     // otherwise add "" quote both field
                                dt.Rows[k]["UNIT"] = $@"""{objUnit}""";
                            }
                        }
                        /*if (dt.Columns.Contains("STREET"))
                        {
                            string objStreet = dt.Rows[k]["STREET"].ToString();
                            dt.Rows[k]["STREET"] = $@"""{objStreet}""";     // Add Double qoute with string
                        }
                        if (dt.Columns.Contains("UNIT"))
                        {
                            string objUNIT = dt.Rows[k]["UNIT"].ToString();
                            dt.Rows[k]["UNIT"] = $@"""{objUNIT}""";
                        }*/

                        if (dt.Columns.Contains("c_v_prem_line4"))
                        {
                            string objc_v_prem_line4 = dt.Rows[k]["c_v_prem_line4"].ToString();
                            dt.Rows[k]["c_v_prem_line4"] = $@"""{objc_v_prem_line4}""";
                        }
                        if (dt.Columns.Contains("CITY"))
                        {
                            string objCITY = dt.Rows[k]["CITY"].ToString();
                            dt.Rows[k]["CITY"] = $@"""{objCITY}""";
                        }

                        if (dt.Columns.Contains("V_NEW_SERIAL_NUM"))
                        {
                            string objNewMeterNum = dt.Rows[k]["V_NEW_SERIAL_NUM"].ToString();
                            if (!string.IsNullOrEmpty(objNewMeterNum))
                            {
                                string result = objNewMeterNum.Substring(0, 3);
                                if (result.ToUpper().Equals("1NF"))
                                {
                                    string finalSeNo = objNewMeterNum.Replace(result, "");  // Remove frist 3 cher
                                                                                            //string finalSeNo2nd = objNewMeterNum.Remove(0, 3);
                                    dt.Rows[k]["V_NEW_SERIAL_NUM"] = finalSeNo;
                                }
                            }
                        }

                        var dis = tempObjList.Where(o => o.FieldValue == dt.Rows[k]["ACCOUNT"].ToString()).FirstOrDefault();
                        var objLatLong = tempObjList.Where(o => o.TblProjectFieldData_Id == dis.TblProjectFieldData_Id && o.InitialFieldName == "LOCATION/GPS").FirstOrDefault();
                        if (!string.IsNullOrEmpty(objLatLong.FieldLatitude) && !string.IsNullOrEmpty(objLatLong.FieldLongitude))
                        {
                            dt.Rows[k]["V_NEW_LATITUDE"] = objLatLong.FieldLatitude;
                            dt.Rows[k]["V_NEW_LONGITUDE"] = objLatLong.FieldLongitude;
                        }
                        else
                        {
                            var objWorkCompletLatLong = tempObjList.Where(o => o.TblProjectFieldData_Id == dis.TblProjectFieldData_Id && o.InitialFieldName == "WORK COMPLETED").FirstOrDefault();
                            dt.Rows[k]["V_NEW_LATITUDE"] = objWorkCompletLatLong.FieldLatitude;
                            dt.Rows[k]["V_NEW_LONGITUDE"] = objWorkCompletLatLong.FieldLongitude;
                        }
                        
                        /*if (dt.Columns.Contains("V_NEW_LATITUDE") && dt.Columns.Contains("V_NEW_LONGITUDE") && dt.Columns.Contains("ACCOUNT"))
                        {
                            string objAccount = dt.Rows[k]["ACCOUNT"].ToString();
                            if (!string.IsNullOrEmpty(objAccount))
                            {
                                var obj = objComEn.tblProjectFieldDataMasters.Where(o => o.FieldValue == objAccount && o.Active == 1).FirstOrDefault();
                                var objLatLong = objComEn.tblProjectFieldDataMasters.Where(o => o.TblProjectFieldData_Id == obj.TblProjectFieldData_Id && o.Active == 1 && o.InitialFieldName == "LOCATION/GPS").FirstOrDefault();
                                if (!string.IsNullOrEmpty(objLatLong.FieldLatitude) && !string.IsNullOrEmpty(objLatLong.FieldLongitude))
                                {
                                    dt.Rows[k]["V_NEW_LATITUDE"] = objLatLong.FieldLatitude;
                                    dt.Rows[k]["V_NEW_LONGITUDE"] = objLatLong.FieldLongitude;
                                }
                                else
                                {
                                    var objWorkCompletLatLong = objComEn.tblProjectFieldDataMasters.Where(o => o.TblProjectFieldData_Id == obj.TblProjectFieldData_Id && o.Active == 1 && o.InitialFieldName == "WORK COMPLETED").FirstOrDefault();
                                    dt.Rows[k]["V_NEW_LATITUDE"] = objWorkCompletLatLong.FieldLatitude;
                                    dt.Rows[k]["V_NEW_LONGITUDE"] = objWorkCompletLatLong.FieldLongitude;
                                }
                            }
                        }*/
                        if (dt.Columns.Contains("V_RTU_DATA"))
                        {
                            string objRtuData = dt.Rows[k]["V_RTU_DATA"].ToString();    // Remove new line in text
                            dt.Rows[k]["V_RTU_DATA"] = objRtuData.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                        }
                        if (dt.Columns.Contains("V_COMMENTS"))
                        {
                            string objComment = dt.Rows[k]["V_COMMENTS"].ToString();    // Remove new line in text
                            dt.Rows[k]["V_COMMENTS"] = objComment.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                        }
                    }
                }
                catch(Exception ex)
                {
                    string strError = ex.Message;
                }

                if (dt.Columns.Contains("VISIT STATUS"))
                {
                    dt.Columns.Remove("VISIT STATUS");  // Remove Column below regin "COMPLETED" find status in if contion in upper
                }

                dt.Columns["V_NEW_LATITUDE"].SetOrdinal(27);
                dt.AcceptChanges();
                dt.Columns["V_NEW_LONGITUDE"].SetOrdinal(28);
                dt.AcceptChanges();
                dt.Columns["v_completion_date"].SetOrdinal(28);     // Change dt column index
                dt.AcceptChanges();
                dt.Columns["v_installer"].SetOrdinal(28);
                dt.AcceptChanges();

                return dt;
            }
            catch (Exception ex)
            {
                return dt;
            }
        }

        public void GetServiceInfo(long projectId, List<STP_ExportReport_Service_Result> objServiceListForproject, out List<STP_ExportReport_Service_Result> objRegulorServiceList, out List<STP_ExportReport_Service_Result> objAdditionalServiceList)
        {

            objAdditionalServiceList = new List<STP_ExportReport_Service_Result>();
            objRegulorServiceList = new List<STP_ExportReport_Service_Result>();

            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                var objListActiveServices = obj.GetActiveServiceRecords(projectId);
                var objListInActiveServices = obj.GetInActiveServiceRecords(projectId);


                foreach (var item in objServiceListForproject)
                {
                    var a = objListActiveServices.Where(o => o.ServiceName.Trim() == item.Service.Trim()).FirstOrDefault();
                    if (a == null)
                    {
                        objAdditionalServiceList.Add(item);
                    }
                    else
                    {
                        objRegulorServiceList.Add(item);
                    }
                }
            }
        }

        public void GetServiceInfoNew(long projectId, List<PROC_ExportReport_Service_Result> objServiceListForproject, out List<PROC_ExportReport_Service_Result> objRegulorServiceList, out List<PROC_ExportReport_Service_Result> objAdditionalServiceList)
        {

            objAdditionalServiceList = new List<PROC_ExportReport_Service_Result>();
            objRegulorServiceList = new List<PROC_ExportReport_Service_Result>();

            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                var objListActiveServices = obj.GetActiveServiceRecords(projectId);
                var objListInActiveServices = obj.GetInActiveServiceRecords(projectId);


                foreach (var item in objServiceListForproject)
                {
                    var a = objListActiveServices.Where(o => o.ServiceName.Trim() == item.Service.Trim()).FirstOrDefault();
                    if (a == null)
                    {
                        objAdditionalServiceList.Add(item);
                    }
                    else
                    {
                        objRegulorServiceList.Add(item);
                    }
                }
            }
        }


    }
}