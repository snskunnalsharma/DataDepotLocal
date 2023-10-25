using AutoMapper;
using GarronT.CMS.Model.BO;
using GoogleMaps.LocationServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;


namespace GarronT.CMS.Model.DAL
{
    public class ExportReportDAL : IDisposable
    {

        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }



        public List<ProjectModel> GetProjectName(string proStatus)
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<ProjectModel> data = CompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1 && a.ProjectStatus == proStatus).Select(o =>
                                       new ProjectModel
                                       {
                                           ProjectId = o.ProjectId,
                                           ProjectCityState = o.ProjectCityName
                                       }).OrderBy(a => a.ProjectCityState).ToList();
            return data;
        }


        /// <summary>
        /// get list of all form 
        /// </summary>
        /// <param name="ActiveStatus"></param>
        /// <returns></returns>
        /// 

        public List<FormTypeModel> GetFormTypeList()
        {
            CompassEntities CompassEntities = new CompassEntities();
            var data = CompassEntities.tblFormTypes.AsNoTracking().Where(o => o.Active == 1 && o.ID <= 2).AsEnumerable().
                Select(o => new FormTypeModel
                {
                    ID = o.ID,
                    FormType = o.FormType,
                }).OrderBy(o => o.FormType).ToList();

            return data;
        }

        public List<STP_ExportReport_Service_Result> getListOfServices(long formId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var data = CompassEntities.STP_ExportReport_Service(formId).ToList();

            return data;
        }


        public List<PROC_ExportReport_Service_Result> GetListOfServices(long formTypeId, long projectId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var data = CompassEntities.PROC_ExportReport_Service(formTypeId, projectId).ToList();

            return data;
        }


        public List<STP_ExportReportData_Attachments_Result> getAttachmentListForForm(long formId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var data = CompassEntities.STP_ExportReportData_Attachments(formId).ToList();

            return data;
        }

        public List<PROC_ExportReport_AttachmentData_Result> GetAttachmentList(long formTypeId, long projectId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var data = CompassEntities.PROC_ExportReport_AttachmentData(formTypeId, projectId).ToList();

            return data;
        }

        public List<PROC_ExportReport_AttachmentData_Audit_Result> GetAuditAttachmentList(long formTypeId, long projectId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var data = CompassEntities.PROC_ExportReport_AttachmentData_Audit(formTypeId, projectId).ToList();

            return data;
        }

        public List<tblFormMaster> GetActiveFormName(long projectId, long formTypeId)
        {
            var list = new List<tblFormMaster>();
            CompassEntities CompassEntities = new CompassEntities();

            list = CompassEntities.tblFormMasters.Where(o => o.ProjectId == projectId && o.FormTypeID == formTypeId).ToList();
            return list;
        }

        public List<FormTypeModel> GetFormType()
        {
            CompassEntities CompassEntities = new CompassEntities();
            var data = CompassEntities.tblFormTypes.Select(o => new FormTypeModel
                {
                    ID = o.ID,
                    FormType = o.FormType,
                }).ToList();

            return data;

        }



        public bool SaveData(List<TblExportReport> sourceFields)
        {
            bool isCreated = false;
            try
            {
                using (CompassEntities objCompassEntities = new CompassEntities())
                {
                    if (sourceFields != null && sourceFields.Count > 0)
                    {
                        var _val = sourceFields[0].FormId;
                        var _data = objCompassEntities.TblExportReports.Where(o => o.FormId == _val && o.Active == 1).ToList();

                        foreach (var item in _data)
                        {
                            item.SortOrder = 0;
                            item.ModifiedOn = sourceFields[0].CreatedOn;
                            item.Active = 0;
                            objCompassEntities.SaveChanges();
                        }


                        foreach (var item in sourceFields)
                        {
                            var result = objCompassEntities.TblExportReports.Where(o => o.FormId == item.FormId && o.FieldName == item.FieldName).FirstOrDefault();
                            if (result == null)
                            {
                                result = new TblExportReport();
                                result.Active = 1;
                                result.CreatedOn = item.CreatedOn;
                                result.FieldId = item.FieldId;
                                result.IsExcelMapFieldFlag = item.IsExcelMapFieldFlag;
                                result.FieldName = item.FieldName;
                                result.ReportFieldName = item.FieldName;
                                result.FormId = item.FormId;
                                result.Flag = (item.Flag == 1 ? 1 : 0);
                                result.SortOrder = (item.Flag == 1 ? item.SortOrder : 0);
                                objCompassEntities.TblExportReports.Add(result);
                                objCompassEntities.SaveChanges();
                            }
                            else
                            {
                                result.FieldName = item.FieldName;
                                result.ReportFieldName = item.ReportFieldName;
                                result.Flag = (item.Flag == 1 ? 1 : 0);
                                result.SortOrder = (item.Flag == 1 ? item.SortOrder : 0);
                                result.ModifiedOn = item.CreatedOn;
                                result.Active = 1;
                                objCompassEntities.SaveChanges();
                            }

                        }

                    }
                }
                isCreated = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isCreated;
        }

        public bool SaveDataNew(List<TblExportReport> sourceFields, int userId)
        {
            bool isCreated = false;
            try
            {
                using (CompassEntities objCompassEntities = new CompassEntities())
                {
                    if (sourceFields != null && sourceFields.Count > 0)
                    {
                        var _val = sourceFields[0].FKFormTypeId;
                        var _val1 = sourceFields[0].FKProjectId;
                        var _data = objCompassEntities.TblExportReports.Where(o => o.FKFormTypeId == _val && o.FKProjectId == _val1 && o.Active == 1 &&
                            o.CreatedBy==userId).ToList();

                        foreach (var item in _data)
                        {
                            item.SortOrder = 0;
                            item.ModifiedOn = sourceFields[0].CreatedOn;
                            item.ModifiedBy = userId;
                            item.Active = 0;
                            objCompassEntities.SaveChanges();
                        }


                        foreach (var item in sourceFields)
                        {
                            var result = objCompassEntities.TblExportReports.Where(o => o.FKFormTypeId == _val 
                                && o.FKProjectId == _val1 && o.FieldName == item.FieldName   && o.CreatedBy==userId
                                ).FirstOrDefault();
                            if (result == null)
                            {
                                result = new TblExportReport();
                                result.FKProjectId = item.FKProjectId;
                                result.FKFormTypeId = item.FKFormTypeId;
                                result.Active = 1;
                                result.CreatedOn = item.CreatedOn;
                                result.CreatedBy = userId;
                                result.FieldId = item.FieldId;
                                result.IsExcelMapFieldFlag = item.IsExcelMapFieldFlag;
                                result.FieldName = item.FieldName;
                                result.ReportFieldName = item.ReportFieldName;
                                //result.FormId = item.FormId;
                                result.Flag = (item.Flag == 1 ? 1 : 0);
                                result.SortOrder = (item.Flag == 1 ? item.SortOrder : 0);

                                objCompassEntities.TblExportReports.Add(result);
                                objCompassEntities.SaveChanges();
                            }
                            else
                            {
                                result.FieldName = item.FieldName;
                                result.ReportFieldName = item.ReportFieldName;
                                result.Flag = (item.Flag == 1 ? 1 : 0);
                                result.SortOrder = (item.Flag == 1 ? item.SortOrder : 0);
                                result.ModifiedOn = item.CreatedOn;
                                result.ModifiedBy = userId;
                                result.Active = 1;
                                objCompassEntities.SaveChanges();
                            }

                        }

                    }
                }
                isCreated = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isCreated;
        }
        public class arrayReportLablefield
        {
            public string value { get; set; }
            public string text { get; set; }
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
        public bool checkFieldSavedForExport(long formId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var list = objCompassEntities.TblExportReports.Where(o => o.FormId == formId && o.Active == 1).ToList();

            if (list != null && list.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkFieldSavedForExportFormType(long formTypeId, long projectId, int UserId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var list = objCompassEntities.TblExportReports.AsNoTracking().Where(o => o.FKProjectId == projectId && o.FKFormTypeId==formTypeId && o.Active == 1
                && o.CreatedBy == UserId).FirstOrDefault();

            if (list != null )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<ExportReportBO> GetFormFields(long formId, int UserId)
        {

            CompassEntities CompassEntities = new CompassEntities();
            var tblForm = CompassEntities.tblFormMasters.Where(o => o.FormId == formId).FirstOrDefault();

       
            var resultdata = CompassEntities.SP_GetFieldInfoForProject(tblForm.ProjectId).Where(o => o.isMappedToExcel == 1).ToList();

            // get form fields for the given formid
            var data = CompassEntities.STP_ExportReport(formId).ToList();

            //remove the fields from excel mapped column list which are present in the form.
            foreach (var item in data)
            {
                var a = resultdata.Where(o => o.FieldName == item.FieldName).FirstOrDefault();
                resultdata.Remove(a);
            }

            //get current form selected fields for the export report.
            var list = CompassEntities.TblExportReports.Where(o => o.FormId == formId && o.Active == 1 && o.CreatedBy == UserId).ToList().OrderBy(o => o.SortOrder).ToList();

            int maxSortOrder = 0;

            if (list == null || list.Count() == 0)
            {
                // if no data present in the database it means no export report done for the form

                //select all fields for the form . 
                //Flag =1 then that field must be in destination section.
                //IsExcelMapFieldFlag==1 then that field belongs to excel mapping & not from form field.
                //FieldId= if form field then id of that field & TableId=if it is excel mapp field then it is tabelId column
                var resultList = data.Select(o => new ExportReportBO { FieldId = o.FieldId, TableId = 0, FieldName = o.FieldName, Flag = 0, SortOrder = 0, IsExcelMapFieldFlag = 0 }).ToList();

                //also get rest data from excel column mapping 
                foreach (var item in resultdata)
                {
                    resultList.Add(new ExportReportBO { FieldId = 0, TableId = item.tabelId, FieldName = item.FieldName, Flag = 0, SortOrder = 0, IsExcelMapFieldFlag = 1 });
                }

                resultList = resultList.OrderBy(o => o.FieldName).ToList();
                int i = 1;
                foreach (var item in resultList)
                {
                    item.Id = i;
                    i++;
                }

                return resultList;
            }
            else
            {
                // if data present in the database it means export report done for the form


                maxSortOrder = int.Parse(list.Select(o => o.SortOrder).Max().ToString());


                //select all fields for the form . 
                //Flag =1 then that field must be in destination section.
                //IsExcelMapFieldFlag==1 then that field belongs to excel mapping & not from form field.
                //FieldId= if form field then id of that field & TableId=if it is excel mapp field then it is tabelId column
                var resultList = data.Select(o => new ExportReportBO { FieldId = o.FieldId, TableId = 0, FieldName = o.FieldName, Flag = 0, SortOrder = 0, IsExcelMapFieldFlag = 0 }).ToList();

                //also get rest data from excel column mapping.
                foreach (var item in resultdata)
                {
                    resultList.Add(new ExportReportBO { FieldId = 0, TableId = item.tabelId, FieldName = item.FieldName, Flag = 0, SortOrder = 0, IsExcelMapFieldFlag = 1 });
                }

                // if fields is in destination field then set flag =1 and it's set order 
                foreach (var item in list)
                {
                    var a = resultList.Where(o => o.FieldName == item.FieldName).FirstOrDefault();
                    if (a != null)
                    {
                        if (item.Flag == 1)
                        {
                            a.Flag = 1;
                            a.SortOrder = int.Parse(item.SortOrder.ToString());
                        }
                    }
                }

                resultList = resultList.OrderBy(o => o.SortOrder).ToList();

                int i = 1;
                foreach (var item in resultList)
                {
                    item.Id = i;
                    i++;
                }

                return resultList;
            }

        }


        public List<STP_ExportReportData_Completed_Result> getReportData(long formId, DateTime fromDate, DateTime toDate, out tblFormMaster objform, out tblProject objProject)
        {
            // CompassEntities objCompassEntities = new CompassEntities();
            // var data = objCompassEntities.STP_ExportReportData_Completed(formId, fromDate, toDate).ToList();
            using (CompassEntities dbcontext = new CompassEntities())
            {
                List<STP_ExportReportData_Completed_Result> objList = new List<STP_ExportReportData_Completed_Result>();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "PROC_ExportReport_CompleteRecords";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@FormId", formId);
                cmd.Parameters.Add("@FromDate", fromDate);
                cmd.Parameters.Add("@Todate", toDate);

                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objList.Add(new STP_ExportReportData_Completed_Result
                    {

                        CapturedDateTime = DateTime.Parse(reader["CapturedDateTime"].ToString()),
                        Cycle = reader["Cycle"].ToString(),
                        FieldLatitude = reader["FieldLatitude"].ToString(),
                        FieldLongitude = reader["FieldLongitude"].ToString(),
                        FieldValue = reader["FieldValue"].ToString(),
                        FromDate = DateTime.Parse(reader["FromDate"].ToString()),
                        ID = long.Parse(reader["Id"].ToString()),
                        InitialFieldName = reader["InitialFieldName"].ToString(),
                        Installer_Name = reader["Installer Name"].ToString(),
                        Route = reader["Route"].ToString(),
                        TblProjectFieldData_Id = long.Parse(reader["TblProjectFieldData_Id"].ToString()),
                        ToDate = DateTime.Parse(reader["ToDate"].ToString()),
                        visitdatetime = DateTime.Parse(reader["visitdatetime"].ToString()),
    

                    });
                 
                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();


                var re = dbcontext.tblFormMasters.Where(o => o.FormId == formId).FirstOrDefault();
                objform = re;
                objProject = dbcontext.tblProjects.Where(o => o.ProjectId == re.ProjectId).FirstOrDefault();
                return objList;
            }
        }






        public List<ExportReport_CompletedRecords> GetCompletedRecords(long formTypeId, long projectId, DateTime fromDate, DateTime toDate)
        {
           
            using (CompassEntities dbcontext = new CompassEntities())
            {
                List<ExportReport_CompletedRecords> objList = new List<ExportReport_CompletedRecords>();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                string checkTime = fromDate.ToString("hh:mm:ss");
                if (checkTime == "12:00:00")            // Request comes from web browser ==> User side Reponse
                {
                    fromDate = Convert.ToDateTime(fromDate.ToString("yyyy-MM-dd 00:00:00"));
                    toDate = Convert.ToDateTime(toDate.ToString("yyyy-MM-dd 23:59:00"));        // Cover whole day for date range
                }
                else
                {
                    // This convertion not a need because same data pass in object
                    // Because developer understatnding
                    fromDate = Convert.ToDateTime(fromDate.ToString("yyyy-MM-dd HH:mm"));   // This request comes from Export report utility
                    toDate = Convert.ToDateTime(toDate.ToString("yyyy-MM-dd HH:mm"));       
                }

                cmd.CommandText = "PROC_ExportReport_CompleteRecords_FormType";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@FormTypeId", formTypeId);
                cmd.Parameters.Add("@projectId", projectId);
                cmd.Parameters.Add("@FromDate", fromDate);
                cmd.Parameters.Add("@Todate", toDate);

                sqlConnection1.Open();

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    objList.Add(new ExportReport_CompletedRecords
                    {

                        CapturedDateTime = DateTime.Parse(reader["CapturedDateTime"].ToString()),
                        Cycle = reader["Cycle"].ToString(),
                        FieldLatitude = reader["FieldLatitude"].ToString(),
                        FieldLongitude = reader["FieldLongitude"].ToString(),
                        FieldValue = reader["FieldValue"].ToString(),
                        FromDate = DateTime.Parse(reader["FromDate"].ToString()),
                        ID = long.Parse(reader["Id"].ToString()),
                        InitialFieldName = reader["InitialFieldName"].ToString(),
                        Installer_Name = reader["Installer Name"].ToString(),
                        Route = reader["Route"].ToString(),
                        TblProjectFieldData_Id = long.Parse(reader["TblProjectFieldData_Id"].ToString()),
                        ProjectFieldDataMasterID = long.Parse(reader["ProjectFieldDataMasterID"].ToString()),
                        ToDate = DateTime.Parse(reader["ToDate"].ToString()),
                        visitdatetime = DateTime.Parse(reader["visitdatetime"].ToString()),
                        FormName = reader["FormName"].ToString()

                    });

                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();


               
                return objList;
            }
        }


        public List<ExportAuditDataComplete> GetAuditCompletedRecords(long formTypeId, long projectId, DateTime fromDate, DateTime toDate)
        {

            using (CompassEntities dbcontext = new CompassEntities())
            {
                List<ExportAuditDataComplete> objList = new List<ExportAuditDataComplete>();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "PROC_ExportReport_CompleteRecords_FormType";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@FormTypeId", formTypeId);
                cmd.Parameters.Add("@projectId", projectId);
                cmd.Parameters.Add("@FromDate", fromDate);
                cmd.Parameters.Add("@Todate", toDate);

                sqlConnection1.Open();

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    objList.Add(new ExportAuditDataComplete
                    {

                        CapturedDateTime = DateTime.Parse(reader["CapturedDateTime"].ToString()),
                        Cycle = reader["Cycle"].ToString(),
                        FieldLatitude = reader["FieldLatitude"].ToString(),
                        FieldLongitude = reader["FieldLongitude"].ToString(),
                        FieldValue = reader["FieldValue"].ToString(),
                        FromDate = DateTime.Parse(reader["FromDate"].ToString()),
                        ID = long.Parse(reader["Id"].ToString()),
                        InitialFieldName = reader["InitialFieldName"].ToString(),
                        Installer_Name = reader["Installer Name"].ToString(),
                        Auditor_Name = reader["Auditor Name"].ToString(),
                        Route = reader["Route"].ToString(),
                        FK_ProjectAuditData = long.Parse(reader["FK_ProjectAuditData"].ToString()),
                        ProjectFieldDataMasterID = long.Parse(reader["ProjectFieldDataMasterID"].ToString()),
                        ToDate = DateTime.Parse(reader["ToDate"].ToString()),
                        visitdatetime = DateTime.Parse(reader["visitdatetime"].ToString())

                    });

                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();



                return objList;
            }
        }




        public DataTable getExcelUploadDataForCompleteInstallations(long formId, DateTime fromDate, DateTime toDate, out tblFormMaster objform, out tblProject objProject)
        {
            // CompassEntities objCompassEntities = new CompassEntities();
            // var data = objCompassEntities.STP_ExportReportData_Completed(formId, fromDate, toDate).ToList();
            using (CompassEntities dbcontext = new CompassEntities())
            {
                DataTable dt = new DataTable();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "PROC_ExportReport_CompleteRecords_ExcelData";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@FormId", formId);
                cmd.Parameters.Add("@FromDate", fromDate);
                cmd.Parameters.Add("@Todate", toDate);

                sqlConnection1.Open();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                sqlConnection1.Close();
                cmd.Dispose();
                sqlConnection1.Dispose();


                var re = dbcontext.tblFormMasters.Where(o => o.FormId == formId).FirstOrDefault();
                objform = re;
                objProject = dbcontext.tblProjects.Where(o => o.ProjectId == re.ProjectId).FirstOrDefault();
                return dt;
            }
        }


        public DataTable GetExcelDataForCompleteRecords(long formTypeId, long projectId, DateTime fromDate, DateTime toDate)
        {
           
            using (CompassEntities dbcontext = new CompassEntities())
            {
                DataTable dt = new DataTable();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "PROC_ExportReport_CompleteRecords_ExcelData_FormType";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@FormTypeId", formTypeId);
                cmd.Parameters.Add("@ProjectId", projectId);
                cmd.Parameters.Add("@FromDate", fromDate);
                cmd.Parameters.Add("@Todate", toDate);

                sqlConnection1.Open();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                sqlConnection1.Close();
                cmd.Dispose();
                sqlConnection1.Dispose();


               
                return dt;
            }
        }

        public DataTable getSKipRTUReportData(long formId, DateTime fromDate, DateTime toDate, out string FormName, out tblProject objProject)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            // var data = objCompassEntities.STP_ExportReportData_SkipRtu(formId, fromDate, toDate).ToList();

            DataTable dt = new DataTable();
            SqlConnection sqlConnection1 = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
            SqlCommand cmd = new SqlCommand();


            cmd.CommandText = "PROC_ExportReport_SkipRtuData";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.Add("@FormId", formId);
            cmd.Parameters.Add("@FromDate", fromDate);
            cmd.Parameters.Add("@Todate", toDate);

            sqlConnection1.Open();

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            sqlConnection1.Close();
            cmd.Dispose();
            sqlConnection1.Dispose();

            var re = objCompassEntities.tblFormMasters.Where(o => o.FormId == formId).FirstOrDefault();
            objProject = objCompassEntities.tblProjects.Where(o => o.ProjectId == re.ProjectId).FirstOrDefault();


            FormName = re.FormName;
            return dt;
        }


        public DataTable GetSKipRTURecordData(long formTypeId, long projectId, DateTime fromDate, DateTime toDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            // var data = objCompassEntities.STP_ExportReportData_SkipRtu(formId, fromDate, toDate).ToList();

            DataTable dt = new DataTable();
            SqlConnection sqlConnection1 = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
            SqlCommand cmd = new SqlCommand();


            cmd.CommandText = "PROC_ExportReport_SkipRtuData_FormType";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.Add("@FormTypeId", formTypeId);
            cmd.Parameters.Add("@projectId", projectId);
            cmd.Parameters.Add("@FromDate", fromDate);
            cmd.Parameters.Add("@Todate", toDate);

            sqlConnection1.Open();

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            sqlConnection1.Close();
            cmd.Dispose();
            sqlConnection1.Dispose();

           
            return dt;
        }

        
        public DataTable getPendingData(long formId, DateTime fromDate, DateTime toDate, out string objformName, out tblProject objProject)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            //var data = objCompassEntities.STP_ExportReportData_Pending(formId, fromDate, toDate).ToList();


            DataTable dt = new DataTable();
            SqlConnection sqlConnection1 = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
            SqlCommand cmd = new SqlCommand();


            cmd.CommandText = "PROC_ExportReport_PendingRecords";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.Add("@FormId", formId);
            cmd.Parameters.Add("@FromDate", fromDate);
            cmd.Parameters.Add("@Todate", toDate);

            sqlConnection1.Open();

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            sqlConnection1.Close();
            cmd.Dispose();
            sqlConnection1.Dispose();

            var re = objCompassEntities.tblFormMasters.Where(o => o.FormId == formId).FirstOrDefault();
            objformName = re.FormName;
            objProject = objCompassEntities.tblProjects.Where(o => o.ProjectId == re.ProjectId).FirstOrDefault();
            return dt;
        }


        public DataTable PendingDataFormType(long formTypeId, long projectId, DateTime fromDate, DateTime toDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();         

            DataTable dt = new DataTable();
            SqlConnection sqlConnection1 = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
            SqlCommand cmd = new SqlCommand();


            cmd.CommandText = "PROC_ExportReport_PendingRecords_FormType";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.Add("@FormTypeId", formTypeId);
            cmd.Parameters.Add("@projectId", projectId);
            cmd.Parameters.Add("@FromDate", fromDate);
            cmd.Parameters.Add("@Todate", toDate);

            sqlConnection1.Open();

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            sqlConnection1.Close();
            cmd.Dispose();
            sqlConnection1.Dispose();

         
            return dt;
        }



        public DataTable getNotAllocatedData(long formId, out string objformName, out tblProject objProject)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            // var data = objCompassEntities.STP_ExportReportData_NotAllocated(formId).ToList();

            using (CompassEntities dbcontext = new CompassEntities())
            {
                List<STP_ExportReportData_NotAllocated_Result> objList = new List<STP_ExportReportData_NotAllocated_Result>();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                cmd.CommandText = "PROC_ExportReport_NotAllocatedRecords";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@FormId", formId);
                sqlConnection1.Open();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                sqlConnection1.Close();

                cmd.Dispose();
                sqlConnection1.Dispose();


                var re = objCompassEntities.tblFormMasters.Where(o => o.FormId == formId).FirstOrDefault();
                objformName = re.FormName;
                objProject = objCompassEntities.tblProjects.Where(o => o.ProjectId == re.ProjectId).FirstOrDefault();
                return dt;
            }

        }


        public DataTable NotAllocatedData_FormType(long projectId,bool status)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            // var data = objCompassEntities.STP_ExportReportData_NotAllocated(formId).ToList();

            using (CompassEntities dbcontext = new CompassEntities())
            {
                string ProcName = string.Empty;
                if (status)
                { ProcName = "PROC_ExportReport_UnAllocatedSkipRecords_FormType"; }
                else
                { ProcName = "PROC_ExportReport_NotAllocatedRecords_FormType"; }

                List<STP_ExportReportData_NotAllocated_Result> objList = new List<STP_ExportReportData_NotAllocated_Result>();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                cmd.CommandText = ProcName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.AddWithValue("@ProjectId", projectId);
                sqlConnection1.Open();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                sqlConnection1.Close();

                cmd.Dispose();
                sqlConnection1.Dispose();
          
                return dt;
            }

        }


        public DataTable NotAllocatedAuditData_FormType(long projectId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            // var data = objCompassEntities.STP_ExportReportData_NotAllocated(formId).ToList();

            using (CompassEntities dbcontext = new CompassEntities())
            {
                List<STP_ExportReportData_NotAllocated_Result> objList = new List<STP_ExportReportData_NotAllocated_Result>();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                cmd.CommandText = "PROC_ExportReport_NotAllocatedRecords_FormType_Audit";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@ProjectId", projectId);
                sqlConnection1.Open();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                sqlConnection1.Close();

                cmd.Dispose();
                sqlConnection1.Dispose();

                return dt;
            }

        }

        public List<string> getColumnNameList(long formId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var data = objCompassEntities.TblExportReports.Where(o => o.Active == 1 && o.FormId == formId && o.Flag == 1).OrderBy(o => o.SortOrder).Select(o => o.FieldName).ToList();
            return data;
        }

        public List<string> GetExportColumnNameList(long formTypeId, long projectId, int UserId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var data = objCompassEntities.TblExportReports.Where(o => o.Active == 1 && o.CreatedBy == UserId  && o.FKFormTypeId == formTypeId
                && o.FKProjectId== projectId  && o.Flag == 1).OrderBy(o => o.SortOrder).Select(o => o.FieldName).ToList();
            return data;
        }

        public DataTable GetExportColumnReportLableList(long formTypeId, long projectId, int UserId)
        {
            List<string> result = new List<string>();
            CompassEntities objCompassEntities = new CompassEntities();
            /*var data = objCompassEntities.TblExportReports.Where(o => o.Active == 1 && o.CreatedBy == UserId  && o.FKFormTypeId == formTypeId
                && o.FKProjectId== projectId  && o.Flag == 1).OrderBy(o => o.SortOrder).Select(o => o.FieldName).ToList();
            return data;*/
            string query = "select FieldName,ReportFieldName from TblExportReport where Active=1 and CreatedBy = " + UserId + " and FKFormTypeId = " + formTypeId + " and FKProjectId = " + projectId + "  and Flag = 1 order by SortOrder";

            SqlConnection sqlConnection1 = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
            sqlConnection1.Open();
            SqlCommand cmd = new SqlCommand(query, sqlConnection1);
            DataTable dt = new DataTable();


            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
                
            }
            
           /* for (int i = 0; i < dt.Rows.Count; i++)
            {
                var labelName = dt.Rows[i]["ReportFieldName"].ToString();
                result.Add(labelName);

            }*/
            
            sqlConnection1.Close();

            cmd.Dispose();
            sqlConnection1.Dispose();

            return dt;
        }

        public List<PROC_WebService_GetFieldInfoForProject_Result> GetAllFieldsForProject(long formId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var projectId = objCompassEntities.tblFormMasters.Where(o => o.FormId == formId).Select(o => o.ProjectId).FirstOrDefault();
            var checkMappedColumnsToExcel = objCompassEntities.PROC_WebService_GetFieldInfoForProject(projectId).ToList();
            return checkMappedColumnsToExcel;
        }

        public List<PROC_WebService_GetFieldInfoForProject_Result> GetAllFieldsOfProject(long projectId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            //var projectId = objCompassEntities.tblFormMasters.Where(o => o.FormId == formId).Select(o => o.ProjectId).FirstOrDefault();
            var checkMappedColumnsToExcel = objCompassEntities.PROC_WebService_GetFieldInfoForProject(projectId).ToList();
            return checkMappedColumnsToExcel;
        }

        public DataTable GetDynamicActiveProjectData(long formId, DateTime startDate, DateTime endDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            //var tblData = objCompassEntities.UploadedExcelDatas.Where(o => o.FKProjectId == projectId && o.Active == 1).ToList();
            DataTable dt = new DataTable();

            using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PROC_ExportReport_CompleteRecords_ExcelData", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@FormId", formId);
                    cmd.Parameters.Add("@FromDate", startDate);
                    cmd.Parameters.Add("@Todate", endDate);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }


        public DataTable GetProjectData(long formTypeId, long projectId, DateTime startDate, DateTime endDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            //var tblData = objCompassEntities.UploadedExcelDatas.Where(o => o.FKProjectId == projectId && o.Active == 1).ToList();
            DataTable dt = new DataTable();

            using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PROC_ExportReport_CompleteRecords_ExcelData_FormType", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProjectId", projectId);
                    cmd.Parameters.Add("@FormTypeId", formTypeId);
                    cmd.Parameters.Add("@FromDate", startDate);
                    cmd.Parameters.Add("@Todate", endDate);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }



        public List<ExportReportBO> GetFieldsOnFormType(long formTypeId, long projectId, int UserId)
        {

            CompassEntities CompassEntities = new CompassEntities();

            //get all fields selected for current project
            var ExcelMapColumnList = CompassEntities.SP_GetFieldInfoForProject(projectId).Where(o => o.isMappedToExcel == 1).ToList();

            // get form fields for the given formid
            var CurrentAllFormFieldList = CompassEntities.PROC_ExportReport_Form_CurrentFields(formTypeId, projectId).ToList();

            //remove the fields from excel mapped column list which are present in the form.

            ExcelMapColumnList.RemoveAll(o => CurrentAllFormFieldList.Exists(x => x == o.FieldName));


            //get current form selected fields for the export report.
            var PresentSavedlist = CompassEntities.TblExportReports.Where(o => o.FKFormTypeId == formTypeId 
                && o.FKProjectId==projectId && o.Active == 1 && o.CreatedBy== UserId).ToList().OrderBy(o => o.SortOrder).ToList();

            int maxSortOrder = 0;

            if (PresentSavedlist == null || PresentSavedlist.Count() == 0)
            {
                // if no data present in the database it means no export report done for the form

                //select all fields for the form type . 
                //Flag =1 then that field must be in destination section.
                //IsExcelMapFieldFlag==1 then that field belongs to excel mapping & not from form field.
                //FieldId= if form field then id of that field & TableId=if it is excel mapp field then it is tabelId column

                List<ExportReportBO> FinalFieldList = new List<ExportReportBO>();

                if (CurrentAllFormFieldList != null && CurrentAllFormFieldList.Count > 0)
                {
                    FinalFieldList.AddRange(CurrentAllFormFieldList.Select(o => new ExportReportBO { FieldName = o, Flag = 0, SortOrder = 0, IsExcelMapFieldFlag = 0 }).ToList());
                }

                if (ExcelMapColumnList != null && ExcelMapColumnList.Count > 0)
                {
                    FinalFieldList.AddRange(ExcelMapColumnList.Select(o => new ExportReportBO { FieldName = o.FieldName, Flag = 0, SortOrder = 0, IsExcelMapFieldFlag = 1 }).ToList());
                }

                FinalFieldList = FinalFieldList.OrderBy(o => o.FieldName).ToList();

                int i = 1;
                foreach (var item in FinalFieldList)
                {
                    item.Id = i;
                    i++;
                }

                return FinalFieldList;
            }
            else
            {
                // if data present in the database it means export report done for the form


                maxSortOrder = int.Parse(PresentSavedlist.Select(o => o.SortOrder).Max().ToString());
                List<ExportReportBO> FinalFieldList = new List<ExportReportBO>();

                //select all fields for the form type . 
                //Flag =1 then that field must be in destination section.
                //IsExcelMapFieldFlag==1 then that field belongs to excel mapping & not from form field.
                //FieldId= if form field then id of that field & TableId=if it is excel mapp field then it is tabelId column

                if (CurrentAllFormFieldList != null && CurrentAllFormFieldList.Count > 0)
                {
                    FinalFieldList.AddRange(CurrentAllFormFieldList.Select(o => new ExportReportBO { FieldName = o, Flag = 0, SortOrder = 0, IsExcelMapFieldFlag = 0 }).ToList());
                }


                //also get rest data from excel column mapping.
                if (ExcelMapColumnList != null && ExcelMapColumnList.Count > 0)
                {
                    FinalFieldList.AddRange(ExcelMapColumnList.Select(o => new ExportReportBO { FieldName = o.FieldName, Flag = 0, SortOrder = 0, IsExcelMapFieldFlag = 1 }).ToList());
                }

                // if fields is in destination field then set flag =1 and it's set order 
                foreach (var item in PresentSavedlist)
                {
                    var a = FinalFieldList.Where(o => o.FieldName == item.FieldName).FirstOrDefault();
                    if (a != null)
                    {
                        if (item.Flag == 1)
                        {
                            a.Flag = 1;
                            a.ReportFieldName = item.ReportFieldName;
                            a.SortOrder = int.Parse(item.SortOrder.ToString());
                        }
                    }
                }

                FinalFieldList = FinalFieldList.OrderBy(o => o.SortOrder).ToList();

                int i = 1;
                foreach (var item in FinalFieldList)
                {
                    item.Id = i;
                    i++;
                }

                return FinalFieldList;
            }

        }
    }
}
