using System;
using System.Web;
using AutoMapper;
using System.Linq;
using System.Text;
using System.Reflection;
using GarronT.CMS.Model.BO;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using log4net;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Transactions;

namespace GarronT.CMS.Model.DAL
{
    public class AuditReportDAL
    {
        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportDAL));
        #endregion
        public List<STP_GetReportTable_Result> GetReportsRecords()
        {
            var list = new List<STP_GetReportTable_Result>();
            CompassEntities CompassEntities = new CompassEntities();
            list = CompassEntities.STP_GetReportTable().OrderByDescending(o => o.Dates).ToList();
            return list;
        }

        public List<STP_GetReportTableNew_Result> GetReportsRecordsNew(long ProjectId, DateTime fromDate, DateTime Todate, long InstallerId)
        {
            var list = new List<STP_GetReportTableNew_Result>();
            CompassEntities CompassEntities = new CompassEntities();
            list = CompassEntities.STP_GetReportTableNew(ProjectId, fromDate, Todate, InstallerId, null, null, null).OrderByDescending(o => o.Dates).ToList();
            return list;
        }

        public List<STP_GetReportTableNew_bak_Result> GetReportsRecordsNew_bak(long ProjectId, DateTime? fromDate, DateTime? Todate)
        {

            try
            {
                var list = new List<STP_GetReportTableNew_bak_Result>();
                CompassEntities CompassEntities = new CompassEntities();
                // CompassEntities.Database.CommandTimeout = 180;

                SqlConnection sqlConnection1 = new SqlConnection(CompassEntities.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "PROC_Audit_ReportHome_SearchSP";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.CommandTimeout = 180;
                cmd.Parameters.Add("@ProjectId", ProjectId);
                cmd.Parameters.Add("@fromDate", fromDate);
                cmd.Parameters.Add("@toDate", Todate);

                sqlConnection1.Open();
                DateTime? dt1 = null;
                long? s1 = null;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //Route = reader["Route"].ToString(), Cycle = reader["Cycle"].ToString() 
                    list.Add(new STP_GetReportTableNew_bak_Result
                    {

                        Account = Convert.ToString(reader["Account"]),
                        CategoryType = Convert.ToString(reader["CategoryType"]),
                        Cycle = Convert.ToString(reader["Cycle"]),
                        Dates = Convert.ToString(reader["Dates"]),
                        FirstName = Convert.ToString(reader["FirstName"]),
                        FormId = !string.IsNullOrEmpty(Convert.ToString(reader["FormId"])) ? long.Parse(reader["FormId"].ToString()) : s1,
                        FormName = Convert.ToString(reader["FormName"]),
                        FromDate = !string.IsNullOrEmpty(Convert.ToString(reader["FromDate"])) ? DateTime.Parse(reader["FromDate"].ToString()) : dt1,
                        ID = long.Parse(reader["Id"].ToString()),
                        // InstallerId = !string.IsNullOrEmpty(Convert.ToString(reader["InstallerId"])) ? long.Parse(reader["InstallerId"].ToString()) : s1,
                        AuditorId = !string.IsNullOrEmpty(Convert.ToString(reader["FK_AuditorId"])) ? long.Parse(reader["FK_AuditorId"].ToString()) : s1,

                        //IsDuplicate = bool.Parse(reader["IsDuplicate"].ToString()),
                        Latitude = Convert.ToString(reader["Latitude"]),
                        Longitude = Convert.ToString(reader["Longitude"]),
                        Months = Convert.ToString(reader["Months"]),
                        //NewMeterNo = Convert.ToString(reader["NewMeterNo"]),
                        NewMeterResgisterNo = Convert.ToString(reader["NewMeterResgisterNo"]),
                        //NewMeterSize = Convert.ToString(reader["NewMeterSize"]),

                        OldMeterNo = Convert.ToString(reader["OldMeterNo"]),
                        OldMeterSize = Convert.ToString(reader["OldMeterSize"]),
                        ProjectId = long.Parse(reader["ProjectId"].ToString()),
                        ProjectStatus = Convert.ToString(reader["ProjectStatus"]),
                        Route = Convert.ToString(reader["Route"]),
                        ShortComment = Convert.ToString(reader["ShortComment"]),
                        SkipComment = Convert.ToString(reader["SkipComment"]),
                        SkipReason = Convert.ToString(reader["SkipReason"]),
                        // SkipRtuAttachmentModel = Convert.ToString(reader["SkipRtuAttachmentModel"]),
                        Street = Convert.ToString(reader["Street"]),
                        ToDate = !string.IsNullOrEmpty(Convert.ToString(reader["ToDate"])) ? DateTime.Parse(reader["ToDate"].ToString()) : dt1,

                        OLDMETERFINALREADING = Convert.ToString(reader["OLDMETERFINALREADING"]),
                        NewMeterNo = Convert.ToString(reader["NewMeterNumber1"]),
                        NewMeterSize = Convert.ToString(reader["NEWMETERSIZE1"]),
                        NEWRADIONUMBER = Convert.ToString(reader["NEWRADIONUMBER1"]),
                    });
                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();


                //if (fromDate == null && Todate == null)
                //{
                //    list = CompassEntities.STP_GetReportTableNew_bak(ProjectId, null, null, null, null, null).OrderByDescending(o => o.Dates).ToList();
                //}
                //else
                //{
                //    list = CompassEntities.STP_GetReportTableNew_bak(ProjectId, fromDate.Value.Date, Todate.Value.Date, null, null, null).OrderByDescending(o => o.Dates).ToList();
                //}
                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public DetailsAll detailsOfProjetcs(long userId, long ProjectId, long formId)
        {
            CompassEntities cmp = new CompassEntities();

            var formRecord = cmp.tblFormMasters.Where(o => o.FormId == formId).FirstOrDefault();
            var projectRecord = cmp.tblProjects.Where(o => o.ProjectId == ProjectId).FirstOrDefault();
            var installerRecord = cmp.tblUsers.Where(o => o.UserID == userId).FirstOrDefault();


            DetailsAll prodetails = new DetailsAll
            {
                FromName = formRecord.FormName,
                ProjectName = projectRecord.ProjectName,
                AuditorName = (installerRecord.FirstName + " " + installerRecord.LastName)
            };

            return prodetails;
        }

        public List<ProjectDate> projectDates(int ProjectID)
        {
            CompassEntities cmp = new CompassEntities();



            List<ProjectDate> projectData = (from b in cmp.tblProjects.ToList()
                                             where b.ProjectId == ProjectID
                                             select new ProjectDate
                                             {
                                                 frmStartDate = Convert.ToDateTime(b.FromDate).ToString("dd-MM-yyyy"),
                                                 frmEndDate = Convert.ToDateTime(b.ToDate).ToString("dd-MM-yyyy"),

                                             }).ToList();
            return projectData;
        }

        public List<projectFiledData> projectFiledDataId(long idinitialId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<projectFiledData> projectFiledId = (from b in CompassEntities.TblProjectFieldDatas.AsNoTracking()
                                                     where b.tblUploadedData_Id == idinitialId && b.Active == 1
                                                     select new projectFiledData
                                                     {
                                                         ID = b.ID,
                                                         tblUploadID = b.tblUploadedData_Id
                                                     }).ToList();

            return projectFiledId;
        }

        public List<SectionList> SectionList(long id)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<SectionList> sectionList = (from b in CompassEntities.tblFormSectionRelations.AsNoTracking()
                                             orderby b.SortOrderNumber
                                             where b.FormID == id && b.Active == 1
                                             select new SectionList
                                             {
                                                 SectionId = b.SectionId,
                                                 SectionName = b.SectionName

                                             }).ToList();

            return sectionList;
        }

        public List<ProjectFildDataTypeMaster> ProjectFildDataTypeMasters(long ID)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<ProjectFildDataTypeMaster> FieldDdlList = (from b in CompassEntities.tblProjectFieldDataMasters.AsNoTracking()
                                                            join d in CompassEntities.TblProjectFieldDataAttachments.AsNoTracking()
                                                            on b.ID equals d.tblProjectFieldDataMaster_ID
                                                            where b.InitialFieldId == ID && b.Active == 1 && d.Active == 1
                                                            select new ProjectFildDataTypeMaster
                                                            {
                                                                ID = b.ID,
                                                                InitialFieldId = b.InitialFieldId,
                                                                InitialFieldName = b.InitialFieldName,
                                                                FieldValue = b.FieldValue,
                                                                FieldStatus = b.FieldStatus,
                                                                ProjectAttachmentId = d.ID,
                                                                InitialFieldIdAttach = d.tblProjectFieldDataMaster_ID,
                                                                IsAudioVideoImage = d.IsAudioVideoImage,
                                                                strFilePath = d.strFilePath,
                                                                isservicesData = d.isServiceData,
                                                                tblservices_Id = d.tblService_Id,
                                                                tblservicePic_Id = d.TblServicePicture_ID,
                                                                Active = d.Active
                                                            }).ToList();

            return FieldDdlList;
        }

        public List<ProjectFildDataTypeMaster> ProjectFildDataTypeMaster(long ID, string FieldName)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<ProjectFildDataTypeMaster> FieldDdlList = (from b in CompassEntities.tblProjectFieldDataMasters.AsNoTracking()
                                                            join d in CompassEntities.TblProjectFieldDataAttachments.AsNoTracking()
                                                            on b.ID equals d.tblProjectFieldDataMaster_ID
                                                            where b.InitialFieldName.ToLower() == FieldName.ToLower() && b.InitialFieldId == ID && b.Active == 1
                                                            select new ProjectFildDataTypeMaster
                                                            {
                                                                ID = b.ID,
                                                                InitialFieldId = b.InitialFieldId,
                                                                InitialFieldName = b.InitialFieldName,
                                                                FieldValue = b.FieldValue,
                                                                FieldStatus = b.FieldStatus,
                                                                InitialFieldIdAttach = d.tblProjectFieldDataMaster_ID,
                                                                IsAudioVideoImage = d.IsAudioVideoImage,
                                                                strFilePath = d.strFilePath
                                                            }).ToList();

            return FieldDdlList;
        }

        public List<FieldDDLMaster> FieldDdlList(long id)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<FieldDDLMaster> FieldDdlList = (from b in CompassEntities.tblProjectFieldDDLMasters.AsNoTracking()
                                                 orderby b.SortOrderNumber
                                                 where b.FormSectionFieldID == id && b.Active == 1
                                                 select new FieldDDLMaster
                                                 {
                                                     Id = b.ID,
                                                     FieldText = b.FieldText,
                                                     Fieldvalue = b.FieldValue
                                                 }).ToList();

            return FieldDdlList;
        }

        public List<FieldNames> FieldDetails(long id)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<FieldNames> Field = (from b in CompassEntities.tblFormSectionFieldRelations.AsNoTracking()
                                      join d in CompassEntities.TblFieldDataTypeMasters.AsNoTracking() on b.TblFieldDataTypeMaster_Id equals d.DataTypeId
                                      join c in CompassEntities.tblProjectFieldProperties.AsNoTracking() on b.FieldId equals c.FormSectionFieldID
                                      where b.SectionId == id && b.Active == 1
                                      select new FieldNames
                                      {
                                          FieldId = b.FieldId,
                                          FieldName = b.FieldName,
                                          TblFieldDataTypeId = b.TblFieldDataTypeMaster_Id,
                                          FielddatatypeNames = d.FieldDataTypeName,
                                          FileddataTypedescription = d.FieldDataTypeDescription,
                                          Id = c.ID,
                                          ProjectID = c.ProjectID,
                                          FormSectionFieldID = c.FormSectionFieldID,
                                          FDID = c.FDID,
                                          FieldLabel = c.FieldLabel,
                                          DefaultValue = c.DefaultValue,
                                          DecimalPositions = c.DecimalPositions,
                                          HintText = c.HintText,
                                          EnforceMinMax = Convert.ToBoolean(c.EnforceMinMax),
                                          MaxLengthValue = c.MaxLengthValue,
                                          MinLengthValue = c.MinLengthValue,
                                          IsRequired = c.IsRequired,
                                          IsEnabled = c.IsEnabled,
                                          CaptureGeolocation = c.CaptureGeolocation,
                                          CaptureTimestamp = c.CaptureTimestamp,
                                          HasAlert = c.HasAlert,
                                          Secure = c.Secure,
                                          allowprii = c.allowprii,
                                          KeyboardType = c.KeyboardType,
                                          FormatMask = c.FormatMask,
                                          DisplayMask = c.DisplayMask,
                                          CaptureMode = c.CaptureMode,
                                          PhotoFile = c.PhotoFile,
                                          AllowAnnotation = c.AllowAnnotation,
                                          PhotoQuality = c.PhotoQuality,
                                          MaximumHeight = c.MaximumHeight,
                                          MaximumWidth = c.MaximumWidth,
                                          ExcludeonSync = c.ExcludeonSync,
                                          PhotoLibraryEnabled = c.PhotoLibraryEnabled,
                                          CameraEnabled = c.CameraEnabled,
                                          GPSTagging = c.GPSTagging,
                                          AllowNotApplicable = c.AllowNotApplicable,
                                          RatingMax = c.RatingMax,
                                          RatingLabels = c.RatingLabels,
                                          ShowMultiselect = c.ShowMultiselect,
                                          ShowRatingLabels = c.ShowRatingLabels,
                                          AllowMultiSelection = c.AllowMultiSelection,
                                          NumberOfColumnsForPhones = c.NumberOfColumnsForPhones,
                                          NumberOfColumnsForTablets = c.NumberOfColumnsForTablets,
                                          HasValues = c.HasValues,
                                          FieldFilterkey = c.FieldFilterkey,
                                          AcknowlegementText = c.AcknowlegementText,
                                          AcknowlegementButtonText = c.AcknowlegementButtonText,
                                          AcknowlegementClickedButtonText = c.AcknowlegementClickedButtonText,
                                          HasFile = c.HasFile,
                                          VideoQuality = c.VideoQuality,
                                          MaxrecordabletimeInSeconds = c.MaxrecordabletimeInSeconds,
                                          AudioQuality = c.AudioQuality,
                                          //FieldDatatypeName=c.FieldDataTypeName

                                      }).ToList();

            return Field;
        }

        public List<FieldProperty> GetAllFieldproperty(long FormSectionid)
        {
            CompassEntities CompassEntities = new CompassEntities();
            //List<FromDetails> FromD = CompassEntities.tblFormMasters.Where(x => x.FormId == 2).Select(x => x.FormName).ToList();
            List<FieldProperty> fieldProperty = (from b in CompassEntities.tblProjectFieldProperties.AsNoTracking()
                                                 join d in CompassEntities.tblFormSectionFieldRelations.AsNoTracking() on b.FormSectionFieldID equals d.FieldId
                                                 join c in CompassEntities.TblFieldDataTypeMasters.AsNoTracking() on d.TblFieldDataTypeMaster_Id equals c.DataTypeId
                                                 where b.FormSectionFieldID == FormSectionid && b.Active == 1
                                                 select new FieldProperty
                                                 {
                                                     Id = b.ID,
                                                     ProjectID = b.ProjectID,
                                                     FormSectionFieldID = b.FormSectionFieldID,
                                                     FDID = b.FDID,
                                                     FieldLabel = b.FieldLabel,
                                                     DefaultValue = b.DefaultValue,
                                                     DecimalPositions = b.DecimalPositions,
                                                     HintText = b.HintText,
                                                     EnforceMinMax = b.EnforceMinMax,
                                                     MaxLengthValue = b.MaxLengthValue,
                                                     MinLengthValue = b.MinLengthValue,
                                                     IsRequired = b.IsRequired,
                                                     IsEnabled = b.IsEnabled,
                                                     CaptureGeolocation = b.CaptureGeolocation,
                                                     CaptureTimestamp = b.CaptureTimestamp,
                                                     HasAlert = b.HasAlert,
                                                     Secure = b.Secure,
                                                     allowprii = b.allowprii,
                                                     KeyboardType = b.KeyboardType,
                                                     FormatMask = b.FormatMask,
                                                     DisplayMask = b.DisplayMask,
                                                     CaptureMode = b.CaptureMode,
                                                     PhotoFile = b.PhotoFile,
                                                     AllowAnnotation = b.AllowAnnotation,
                                                     PhotoQuality = b.PhotoQuality,
                                                     MaximumHeight = b.MaximumHeight,
                                                     MaximumWidth = b.MaximumWidth,
                                                     ExcludeonSync = b.ExcludeonSync,
                                                     PhotoLibraryEnabled = b.PhotoLibraryEnabled,
                                                     CameraEnabled = b.CameraEnabled,
                                                     GPSTagging = b.GPSTagging,
                                                     AllowNotApplicable = b.AllowNotApplicable,
                                                     RatingMax = b.RatingMax,
                                                     RatingLabels = b.RatingLabels,
                                                     ShowMultiselect = b.ShowMultiselect,
                                                     ShowRatingLabels = b.ShowRatingLabels,
                                                     AllowMultiSelection = b.AllowMultiSelection,
                                                     NumberOfColumnsForPhones = b.NumberOfColumnsForPhones,
                                                     NumberOfColumnsForTablets = b.NumberOfColumnsForTablets,
                                                     HasValues = b.HasValues,
                                                     FieldFilterkey = b.FieldFilterkey,
                                                     AcknowlegementText = b.AcknowlegementText,
                                                     AcknowlegementButtonText = b.AcknowlegementButtonText,
                                                     AcknowlegementClickedButtonText = b.AcknowlegementClickedButtonText,
                                                     HasFile = b.HasFile,
                                                     VideoQuality = b.VideoQuality,
                                                     MaxrecordabletimeInSeconds = b.MaxrecordabletimeInSeconds,
                                                     AudioQuality = b.AudioQuality,
                                                     FieldDatatypeName = c.FieldDataTypeName

                                                 }).ToList();

            return fieldProperty;
        }
        public List<FromDetails> FormDetails()
        {
            CompassEntities CompassEntities = new CompassEntities();
            //List<FromDetails> FromD = CompassEntities.tblFormMasters.Where(x => x.FormId == 2).Select(x => x.FormName).ToList();
            List<FromDetails> FromD = (from b in CompassEntities.tblFormMasters.AsNoTracking()
                                       where b.FormId == 2
                                       select new FromDetails
                                       {
                                           FormId = b.FormId,
                                           FormName = b.FormName
                                       }).ToList();

            return FromD;
        }

        public DataSet getListReport(long ID, long frmID)
        {



            //member declaration
            DataSet dsReturnData = null;
            dsReturnData = new DataSet();

            //IDbCommand objIDbCommand;
            SqlCommand objSqlCommand;
            //Create sqlCommand object
            objSqlCommand = new SqlCommand();
            objSqlCommand.CommandTimeout = 0;
            SqlConnection objConn = null;
            //  Boolean blnConnection;
            try
            {


                objConn = new SqlConnection(Convert.ToString(ConfigurationManager.ConnectionStrings["DefaultConnection"]));
                // objIDbCommand = objSqlCommand;
                //Check current connection is open or close
                // blnConnection = OpenConnection();
                //Set command objects commandtype equals to stored procedure
                objSqlCommand.CommandType = CommandType.StoredProcedure;
                //Set stored procedure name to the commandtext
                objSqlCommand.CommandText = "STP_GetReprotDataByFormIDAndtblUploadedData_Id";
                //Create adapter object 
                SqlDataAdapter objDataAdapter;
                objDataAdapter = new SqlDataAdapter();

                //Check the connect state
                //if (blnConnection == true)
                //    {
                //Assign connection to command object
                objSqlCommand.Connection = objConn;

                //add parameters to Commamd

                //objSqlCommand.Parameters.AddWithValue("@VALFROMDATE", valFromDate);
                //objSqlCommand.Parameters.AddWithValue("@VALTODATE", valToDate);
                //objSqlCommand.Parameters.AddWithValue("@VALLINEID", valLineId);
                //objSqlCommand.Parameters.AddWithValue("@VALMACHINEID", valMachineid);
                //objSqlCommand.Parameters.AddWithValue("@VALBREAKDOWNTYPE", valBreakdownType);
                // objSqlCommand.Parameters.AddWithValue("@VALLINEIDS", lineids);

                //Sets the stored procedure to select records
                objDataAdapter.SelectCommand = objSqlCommand;
                //Fill dataset
                objDataAdapter.Fill(dsReturnData, "STP_GetReprotDataByFormIDAndtblUploadedData_Id");
                //Dispose adapter object
                objDataAdapter.Dispose();

                //Close connection
                //objSqlCommand.Connection.Close();
                // }
            }
            catch (Exception ex)
            {
                //Throw new exception
                throw ex;
            }
            //Return dataset
            return dsReturnData;

        }


        public List<sectionfiledval> GetReportsRecordsID1(long ID, long frmID)
        {
            List<sectionfiledval> filedVal = new List<sectionfiledval>();
            try
            {
                CompassEntities CompassEntities = new CompassEntities();

                var sectionList = CompassEntities.tblFormSectionRelations.AsNoTracking().Where(o => o.FormID == frmID && o.Active == 1).OrderBy(a => a.SortOrderNumber).ToList();

                long ProjectFieldData = CompassEntities.TblProjectFieldDatas.AsNoTracking().Where(o => o.tblUploadedData_Id == ID && o.FormId == frmID && o.Active == 1).Select(a => a.ID).FirstOrDefault();

                foreach (var item in sectionList)
                {
                    var FormSectionFieldRelationList = CompassEntities.tblFormSectionFieldRelations.AsNoTracking().Where(o => o.SectionId == item.SectionId && o.Active == 1).OrderBy(a => a.FieldOrder).ToList();
                    foreach (var FormSectionFieldRelation in FormSectionFieldRelationList)
                    {
                        sectionfiledval sectionfiledval = new sectionfiledval();

                        var ProjectFieldDataMaster = CompassEntities.tblProjectFieldDataMasters.AsNoTracking().Where(o => o.TblProjectFieldData_Id == ProjectFieldData && o.InitialFieldId == FormSectionFieldRelation.FieldId && o.Active == 1).FirstOrDefault();

                        #region check image found
                        var fielddataattachment = CompassEntities.TblProjectFieldDataAttachments.AsNoTracking().Where(a => a.tblProjectFieldDataMaster_ID == ProjectFieldDataMaster.ID && a.Active == 1).FirstOrDefault();
                        if (fielddataattachment != null)
                        {
                            sectionfiledval.IsAudioVideoImage = fielddataattachment.IsAudioVideoImage;
                            sectionfiledval.strFilePath = fielddataattachment.strFilePath;
                        }
                        else
                        {
                            sectionfiledval.IsAudioVideoImage = 0;
                        }
                        #endregion

                        sectionfiledval.IDS = ProjectFieldDataMaster.ID;
                        sectionfiledval.sectionName = item.SectionName;
                        sectionfiledval.FieldId = FormSectionFieldRelation.FieldId;
                        sectionfiledval.FieldName = FormSectionFieldRelation.FieldName;
                        sectionfiledval.FieldValues = ProjectFieldDataMaster != null ? ProjectFieldDataMaster.FieldValue : "";
                        sectionfiledval.Fieldlatitude = ProjectFieldDataMaster != null ? ProjectFieldDataMaster.FieldLatitude : "";
                        sectionfiledval.Fieldlongitude = ProjectFieldDataMaster != null ? ProjectFieldDataMaster.FieldLongitude : "";

                        filedVal.Add(sectionfiledval);

                    }

                }

                return filedVal;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return filedVal;
            }
        }

        //new 
        public List<sectionfiledval> GetReportsRecordsID(long ID, long frmID)
        {
            List<sectionfiledval> filedVal = new List<sectionfiledval>();
            try
            {

                CompassEntities CompassEntities = new CompassEntities();
                //filedVal = CompassEntities.STP_GetReportPDFFields(ID, frmID).Select(a => new sectionfiledval
                filedVal = CompassEntities.PROC_ReportPDF_Audit_GetReportPDFFields(ID, frmID).Select(a => new sectionfiledval
                {
                    IDS = a.IDS,
                    sectionName = a.sectionName,
                    FieldId = a.FieldId,
                    FieldName = a.FieldName,
                    FieldValues = a.FieldValue,
                    Fieldlatitude = a.FieldLatitude,
                    Fieldlongitude = a.FieldLongitude,
                    strFilePath = a.strFilePath,
                    // IsAudioVideoImage = a.IsAudioVideoImage
                    FieldOrder = a.FieldOrder,
                    SortOrderNumber = a.SortOrderNumber,
                    captureTimeStamp = (a.CapturedDateTime != null ? a.CapturedDateTime.Value.ToString("MMM-dd-yyyy hh:mm tt") : ""),
                    isLaLongCaptureRequired = (a.CaptureGeolocation != null ? Convert.ToBoolean(a.CaptureGeolocation) : false),
                    isTimeStampCaptureRequired = (a.CaptureTimestamp != null ? Convert.ToBoolean(a.CaptureTimestamp) : false)


                }).Distinct().OrderBy(a => a.FieldOrder).ThenBy(a => a.SortOrderNumber).ToList();


            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return filedVal;
        }

        public List<tblUploadedData> GetReportsRecordsIDUpload(long ID)
        {
            // var list = new List<tblUploadedData>();
            CompassEntities CompassEntities = new CompassEntities();
            //  var tblStateList = CompassEntities.tblStateMasters.Include("tblCountryMaster").Where(a => a.Active == 1 && a.CountryID == Id).ToList();
            //list = (from b in CompassEntities.tblClients
            //        join c in CompassEntities.TblCityMasters on b.City equals c.CityId
            //        join d in CompassEntities.tblStateMasters on b.State equals d.StateId
            //        where b.Active == 1
            //        select new ClientModel
            //        {
            //            ClientId = b.ClientId,
            //            ClientName = b.ClientName,
            //            Address1 = b.Address1,
            //            Address2 = b.Address2,
            //            Address3 = b.Address3,
            //            City = b.City,
            //            State = b.State,
            //            CityName = c.CityName,
            //            StateName = d.StateName,
            //            Pincode = b.Pincode,
            //            MobileNumber = b.MobileNumber,
            //            PhoneNumber = b.PhoneNumber
            //        }).ToList();

            //long list1 = Convert.ToInt64(CompassEntities.TblProjectFieldDatas.Where(o => o.tblUploadedData_Id == ID).Select(o => o.ID).FirstOrDefault());
            //var list = CompassEntities.tblProjectFieldDataMasters.Where(o => o.TblProjectFieldData_Id == list1).ToList();
            var list = CompassEntities.tblUploadedDatas.AsNoTracking().Where(o => o.ID == ID).ToList();
            return list;
        }

        string services1;

        // Method used to get the service model along with image paths of this service
        public List<ServicesModel> getServicesWithImages(String serviceID, long? tblProjectFieldDataMaster_ID, bool isAdditionalService)
        {
            List<ServicesModel> listServiceModel = new List<ServicesModel>();
            if (serviceID != "" && serviceID != null)
            {
                string[] ids = serviceID.Split('|');
                for (int i = 0; i <= ids.Length - 1; i++)
                {
                    if (ids[i] != null && ids[i] != "")
                    {
                        CompassEntities CompassEntities = new CompassEntities();
                        long splittedServiceID = Convert.ToInt64(ids[i]);
                        //get service details
                        ServicesModel objService = new ServicesModel();
                        //if (!isAdditionalService)
                        //{
                        objService = new ServiceDAL().GetServiceById(splittedServiceID);
                        //}
                        //else
                        //{
                        //    long masterFieldId = CompassEntities.TblProjectMasterFieldRelations.AsNoTracking().Where(o => o.ID == splittedServiceID).Select(o => o.MasterFieldID).FirstOrDefault();
                        //    objService = new ServiceDAL().GetServiceById(masterFieldId);
                        //}
                        //now get the image paths for services

                        if (objService != null)
                        {


                            List<string> serviceImagepath = new List<string>();
                            List<ServiceAttachmentModel> paths = new List<ServiceAttachmentModel>();

                            paths = (from o in CompassEntities.ProjectAuditFieldDataAttachments.AsNoTracking()
                                     where o.FK_ProjectAuditFieldDataMaster_ID == tblProjectFieldDataMaster_ID
                                     && o.Fk_tblService_Id == splittedServiceID && o.Active == 1
                                     select new ServiceAttachmentModel
                                     {
                                         id = o.ID,
                                         Active = o.Active,
                                         IsAudioVideoImage = o.IsAudioVideoImage,
                                         isServiceData = o.IsServiceData,
                                         strFilePath = o.StrFilePath,
                                         tblProjectFieldDataMaster_ID = o.FK_ProjectAuditFieldDataMaster_ID,
                                         TblServicePicture_ID = o.FK_TblServicePicture_ID
                                     }).OrderBy(a => a.IsAudioVideoImage).ToList();


                            objService.lisServiceAttachment = paths;

                            listServiceModel.Add(objService);
                        }
                    }
                }
            }

            return listServiceModel;

        }


        // Method used to get the service model along with image paths of this service
        public string getLocationImage(long? tblProjectFieldDataMaster_ID)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            string paths = Convert.ToString((from o in objCompassEntities.ProjectAuditFieldDataAttachments.AsNoTracking()
                                             where o.FK_ProjectAuditFieldDataMaster_ID == tblProjectFieldDataMaster_ID
                                             select o.StrFilePath).FirstOrDefault());


            return paths;
        }


        //CreatedBy AniketJ on 13-jan-2016
        public string getLocationImage_Skip(long? uploadid, long? formid)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            string paths = Convert.ToString((from am in objCompassEntities.ProjectAuditDatas
                                             join o in objCompassEntities.ProjectAuditFieldDataAttachments.AsNoTracking()
                                             on am.ID equals o.FK_ProjectAuditFieldDataId
                                             where am.FK_UploadedExcelData_Id == uploadid && am.FormId == formid && o.IsSkipped == true && o.IsAudioVideoImage == 3 && o.StrFilePath.Contains("img.png")
                                             select o.StrFilePath).FirstOrDefault());


            return paths;
        }

        public string getSkipGPSLocation(long? uploadid, long? auditor, long? formid)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var auditdata = objCompassEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == formid
                 && o.FK_UploadedExcelData_Id == uploadid && o.FK_AuditorId == auditor).FirstOrDefault();

            return auditdata.SkipGPSLocation;
        }

        public string services(string id)
        {
            CompassEntities CompassEntities = new CompassEntities();
            //List<servicesname> services=new List<servicesname>();
            if (id != "" && id != null)
            {
                string[] ids = id.Split('|');
                for (int i = 0; i <= ids.Length - 1; i++)
                {
                    if (ids[i] != null && ids[i] != "")
                    {
                        int id1 = Convert.ToInt32(ids[i]);
                        services1 += CompassEntities.tblServices.AsNoTracking().Where(o => o.ID == id1).Select(o => o.Service).FirstOrDefault() + ",";
                    }
                }
            }
            else
            {
                services1 = "-";
            }

            return services1;
        }

        string imagespath;
        public string servicesimeges(long? id)
        {
            if (id != null)
            {
                CompassEntities CompassEntities = new CompassEntities();
                var paths = CompassEntities.TblProjectFieldDataAttachments.AsNoTracking().Where(o => o.tblProjectFieldDataMaster_ID == id).ToList();
                foreach (var item in paths)
                {
                    imagespath += item.strFilePath + "|";
                }
                return imagespath;
            }
            else
            {
                return imagespath;
            }

        }

        public bool reOpenRecords(long uploadId, long installerId, long formId, long reOpenId, string reOpenComment)
        {
            bool result = false;

            // define our transaction scope
            var scope = new TransactionScope(
                // a new transaction will always be created
                TransactionScopeOption.RequiresNew,
                // we will allow volatile data to be read during transaction
                new TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                }
            );
            try
            {

                // use the scope we just defined
                using (scope)
                {
                    CompassEntities objCompassEntities = new CompassEntities();
                    var projectId = objCompassEntities.UploadedExcelDatas.AsNoTracking().Where(o => o.Id == uploadId && o.Active == 1).Select(o => o.FKProjectId).FirstOrDefault();

                    var formObject = objCompassEntities.tblFormMasters.AsNoTracking().Where(o => o.FormId == formId && o.ProjectId == projectId).FirstOrDefault();

                    var objDependantFormList = objCompassEntities.tblFormTypeMaps.Where(o => o.FormTypeId == formObject.FormId && o.Active == 1).Select(o => o).ToList();

                    //if (formObject.FormTypeID=)
                    var objFormType = objCompassEntities.tblFormTypes.AsNoTracking().Where(o => o.Active == 1 && o.ID == formObject.FormTypeID).FirstOrDefault();

                    var installerMap = objCompassEntities.tblInstallerMapDatas.Where(o => o.FK_UploadedExcelData_Id == uploadId && o.Active == 1).FirstOrDefault();
                    installerMap.MobileResponse = null;
                    installerMap.Active = 2;
                    installerMap.ModifiedOn = new CommonFunctions().ServerDate();
                    installerMap.SkipReasonMasterId = reOpenId;
                    installerMap.SkipComment = reOpenComment;
                    objCompassEntities.SaveChanges();

                    //if (objFormType.FormType.ToLower() == "Installation Form".ToLower())
                    //{

                    //}
                    var FormIds = objDependantFormList.Select(o => o.AffectingFormTypeId).ToList();
                    FormIds.Add(formObject.FormId);

                    foreach (var itemFormId in FormIds)
                    {
                        var fieldData = objCompassEntities.TblProjectFieldDatas.Where(o => o.FK_UploadedExcelData_Id == uploadId && o.FormId == itemFormId && o.Active == 1).FirstOrDefault();

                        if (fieldData != null)
                        {

                            var listFieldDataAttachments1 = objCompassEntities.TblProjectFieldDataAttachments.Where(o => o.Active == 1 && o.ProjectFieldDataId == fieldData.ID).ToList();

                            foreach (var item2 in listFieldDataAttachments1)
                            {
                                item2.Active = 2;
                                item2.ModifiedOn = new CommonFunctions().ServerDate();
                                objCompassEntities.SaveChanges();
                            }


                            var listFieldDataMaster = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1 && o.TblProjectFieldData_Id == fieldData.ID).ToList();

                            foreach (var item1 in listFieldDataMaster)
                            {
                                var listFieldDataAttachments = objCompassEntities.TblProjectFieldDataAttachments.Where(o => o.Active == 1 && o.tblProjectFieldDataMaster_ID == item1.ID).ToList();

                                foreach (var item2 in listFieldDataAttachments)
                                {
                                    item2.Active = 2;
                                    item2.ModifiedOn = new CommonFunctions().ServerDate();
                                    objCompassEntities.SaveChanges();
                                }
                                item1.Active = 2;
                                item1.ModifiedOn = new CommonFunctions().ServerDate();
                                objCompassEntities.SaveChanges();
                            }
                            fieldData.InstallerMapId = installerMap.ID;
                            fieldData.SkipComment = reOpenComment;
                            fieldData.SkipId = reOpenId;
                            fieldData.SkippedDatetime = new CommonFunctions().ServerDate();
                            //fieldData.SkippedReason=
                            fieldData.Active = 2;
                            fieldData.ModifiedOn = new CommonFunctions().ServerDate();
                            objCompassEntities.SaveChanges();
                        }
                    }



                    tblInstallerMapData objtblInstallerMapDatas = new tblInstallerMapData();
                    objtblInstallerMapDatas.Active = 1;
                    objtblInstallerMapDatas.CreatedOn = new CommonFunctions().ServerDate();
                    objtblInstallerMapDatas.InstallerId = installerId;
                    objtblInstallerMapDatas.FK_UploadedExcelData_Id = uploadId;
                    if (installerMap.InstallerId == installerId)
                    {
                        objtblInstallerMapDatas.FromDate = installerMap.FromDate;
                        objtblInstallerMapDatas.ToDate = installerMap.ToDate;
                    }
                    else
                    {
                        //var _getInstallerMapList = objCompassEntities.STP_GetExistedUserInstallationbyDate(objtblInstallerMapDatas.CreatedOn, objtblInstallerMapDatas.CreatedOn, installerId, projectId).ToList();
                        var _getInstallerMapList = objCompassEntities.PROC_ReportHome_ReOpen_GetCurrentVisitByDate(objtblInstallerMapDatas.CreatedOn, objtblInstallerMapDatas.CreatedOn, installerId, projectId).ToList();
                        if (_getInstallerMapList == null || _getInstallerMapList.Count == 0)
                        {
                            objtblInstallerMapDatas.FromDate = installerMap.FromDate;
                            objtblInstallerMapDatas.ToDate = installerMap.ToDate;
                        }
                        else
                        {
                            bool _result = false;
                            foreach (var item in _getInstallerMapList)
                            {
                                if ((item.FromDate.Value.Date <= installerMap.FromDate.Date && item.ToDate.Value.Date >= installerMap.FromDate.Date) ||
                                    (item.FromDate.Value.Date <= installerMap.ToDate.Date && item.ToDate.Value.Date >= installerMap.ToDate.Date))
                                {
                                    objtblInstallerMapDatas.FromDate = item.FromDate.Value;
                                    objtblInstallerMapDatas.ToDate = item.ToDate.Value;
                                    _result = true;
                                    break;
                                }
                            }
                            if (!_result)
                            {
                                objtblInstallerMapDatas.FromDate = installerMap.FromDate;
                                objtblInstallerMapDatas.ToDate = installerMap.ToDate;
                            }
                        }
                    }
                    objCompassEntities.tblInstallerMapDatas.Add(objtblInstallerMapDatas);
                    objCompassEntities.SaveChanges();
                    result = true;
                    scope.Complete();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        #region--Createdby AniketJ on 30-5-2016--

        //get project by project status
        public List<ProjectModel> GetProjectNamebyProjectStatus(List<string> ProjectStatusList)
        {

            try
            {

                CompassEntities CompassEntities = new CompassEntities();
                List<ProjectModel> data = (from m in CompassEntities.STP_GetProjectCityState()
                                           join c in CompassEntities.AuditMasters on m.ProjectId equals c.FKProjectId
                                           where m.Active == 1 && ProjectStatusList.Contains(m.ProjectStatus)
                                           select new ProjectModel
                                           {
                                               ProjectId = m.ProjectId,
                                               // ProjectName = o.ProjectName,
                                               ProjectCityState = m.ProjectCityName,
                                               stringFromDate = c.AuditStartDate.ToString(),
                                               stringToDate = c.AuditEndDate.ToString()
                                           }).OrderBy(m => m.ProjectCityState).ToList();
                return data;
                //CompassEntities CompassEntities = new CompassEntities();
                //List<ProjectModel> data = CompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1 && ProjectStatusList.Contains(a.ProjectStatus)).Select(o =>
                //                     new ProjectModel
                //                     {
                //                         ProjectId = o.ProjectId,
                //                         // ProjectName = o.ProjectName,
                //                         ProjectCityState = o.ProjectCityName,
                //                         stringFromDate = o.strFormDate,
                //                         stringToDate = o.strToDate
                //                     }).OrderBy(a => a.ProjectCityState).ToList();
                //return data;
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }




        /// <summary>
        /// Get AuditorList by ProjectID
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns>AuditorList</returns>
        public List<UsersSmallModel> GeAuditorListbyProjectID(long ProjectId)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<UsersSmallModel> AuditorList = (from b in CompassEntities.AuditMasters.AsNoTracking()
                                                     join AM in CompassEntities.AuditorMaps.AsNoTracking() on b.Id equals AM.FK_AuditId
                                                     join um in CompassEntities.tblUsers.AsNoTracking() on AM.FK_UserId equals um.UserID
                                                     where b.FKProjectId == ProjectId //&& b.Active == 1 && AM.Active == 1 && um.Active == 1
                                                     select new UsersSmallModel
                                                     {
                                                         UserId = um.UserID,
                                                         UserName = um.FirstName
                                                     }).OrderBy(a => a.UserName).ToList();


                //var InstallerList1 = CompassEntities.TblProjectInstallers.AsNoTracking().Where(o => o.Active == 0 && o.ProjectID == ProjectId).ToList();
                //if (InstallerList1 != null && InstallerList1.Count > 0)
                //{
                //    foreach (var item in InstallerList1)
                //    {
                //        var checkExist = (from post in CompassEntities.UploadedExcelDatas
                //                          join meta in CompassEntities.TblProjectFieldDatas on post.Id equals meta.FK_UploadedExcelData_Id
                //                          where meta.Active == 1 && post.FKProjectId == ProjectId
                //                          select new { id = meta.ID }).FirstOrDefault();

                //        if (checkExist != null)
                //        {
                //            InstallerList.Add(
                //                new UsersSmallModel
                //                {
                //                    UserId = item.tblUsers_UserID,
                //                    UserName = CompassEntities.tblUsers.Where(a => a.UserID == item.tblUsers_UserID).Select(m => m.FirstName).FirstOrDefault()
                //                }
                //                );
                //        }
                //    }
                //}


                return AuditorList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<ReasonBO> GetReopenReasonList()
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<ReasonBO> ReasonBOList = CompassEntities.tblSkipReasonMasters.Where(o => o.Active == 1 && o.CategoryType == "REOPEN").
                    Select(o => new ReasonBO { SkipId = o.SkipId, SkipReason = o.SkipReason }).ToList();
                return ReasonBOList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> GetCycleByProjectID(long ProjectID)
        {
            CompassEntities dbcontext = new CompassEntities();

            //  List<string> GetCycle = dbcontext.tblUploadedDatas.Where(x => x.ProjectId == ProjectID && x.Active == 1).Select(a => a.Cycle).Distinct().OrderBy(a => a).ToList();
            List<string> GetCycle = new List<string>();



            SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "PROC_AuditReport_DynamicCycle";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.Add("@ProjectId", ProjectID);
            sqlConnection1.Open();

            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                GetCycle.Add(reader["cycle"].ToString());
            }
            // Data is accessible through the DataReader object here.

            sqlConnection1.Close();
            reader.Close();
            reader.Dispose();
            cmd.Dispose();
            sqlConnection1.Dispose();
            return GetCycle;

        }

        public List<string> GetRouteListByCycle_ProjectID(string[] Cycle, long ProjectId)
        {
            CompassEntities dbcontext = new CompassEntities();

            // List<string> GetRoot = dbcontext.tblUploadedDatas.Where(x => Cycle.Contains(x.Cycle) && x.ProjectId == ProjectId && x.Active == 1).Select(a => a.Route).Distinct().OrderBy(a => a).ToList();
            List<PROC_ExistingRoute_Result> objListPROC_ExistingRoute_Result = new List<PROC_ExistingRoute_Result>();
            List<string> GetRoot = new List<string>();
            SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "PROC_AuditReport_DynamicRoute";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            cmd.Parameters.Add("@ProjectId", ProjectId);

            sqlConnection1.Open();

            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                objListPROC_ExistingRoute_Result.Add(new PROC_ExistingRoute_Result { Route = reader["Route"].ToString(), Cycle = reader["Cycle"].ToString() });
            }

            sqlConnection1.Close();
            reader.Close();
            reader.Dispose();
            cmd.Dispose();
            sqlConnection1.Dispose();
            GetRoot = objListPROC_ExistingRoute_Result.Where(x => Cycle.Contains(x.Cycle)).Select(a => a.Route).Distinct().OrderBy(a => a).ToList();

            return GetRoot;

        }

        public List<tblUploadedData> GetAddress(List<string> Route, List<string> Cycle, long projectId, string fromDate, string toDate)
        {

            CompassEntities CompassEntities = new CompassEntities();
            List<tblUploadedData> objaddress = new List<tblUploadedData>();
            List<string> stringaddress = new List<string>();

            DateTime frmdate = Convert.ToDateTime(fromDate);
            DateTime todate = Convert.ToDateTime(toDate);
            if (Route != null)
            {
                //var demo =GetReportsRecordsNew_bak(Projectid, frmdate, todate);
                var list = new List<STP_GetReportTableNew_bak_Result>();
                SqlConnection sqlConnection1 = new SqlConnection(CompassEntities.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "[PROC_AuditReport_GetAllAddress]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@ProjectId", projectId);

                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new STP_GetReportTableNew_bak_Result
                    {
                        Route = reader["Route"].ToString(),
                        Cycle = reader["Cycle"].ToString(),
                        ID = long.Parse(reader["Id"].ToString()),
                        Street = reader["Street"].ToString()
                    });
                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();

                if (Cycle != null)
                {
                    list = list.Where(a => Cycle.Contains(a.Cycle)).ToList();

                }
                if (Route != null)
                {
                    list = list.Where(a => Route.Contains(a.Route)).ToList();
                }


                var objaddress1 = list.Select(o => new tblUploadedData
                {
                    Latitude = o.Latitude,
                    Longitude = o.Longitude,
                    ID = o.ID,
                    Street = o.Street,

                }).ToList();

                objaddress = objaddress1.OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();

                ;

            }

            return objaddress;

        }

        #endregion

        public bool UploadImages(string FinalPath, long ExistingImageId, out string Message, out long AttachmentId, int CreatedBy)
        {
            bool result = false;
            Message = "Unable to Save";
            AttachmentId = 0;
            CompassEntities compassEntities = new CompassEntities();
            try
            {
                var ObjTblProjectFieldDataAttachments = (compassEntities.TblProjectFieldDataAttachments.Where(n => n.ID == ExistingImageId)).FirstOrDefault();

                TblProjectFieldDataAttachment Obj = new TblProjectFieldDataAttachment();
                Obj.IsSkipped = false;
                Obj.tblProjectFieldDataMaster_ID = ObjTblProjectFieldDataAttachments.tblProjectFieldDataMaster_ID;
                Obj.IsAudioVideoImage = ObjTblProjectFieldDataAttachments.IsAudioVideoImage;
                Obj.strFilePath = FinalPath;
                Obj.isServiceData = ObjTblProjectFieldDataAttachments.isServiceData;
                Obj.tblService_Id = ObjTblProjectFieldDataAttachments.tblService_Id;
                Obj.TblServicePicture_ID = ObjTblProjectFieldDataAttachments.TblServicePicture_ID;
                Obj.Active = 1;
                Obj.CreatedBy = CreatedBy;
                Obj.CreatedOn = new CommonFunctions().ServerDate();
                compassEntities.TblProjectFieldDataAttachments.Add(Obj);
                compassEntities.SaveChanges();
                AttachmentId = Obj.ID;
                Message = "Image Upload successfully";
                result = true;
            }
            catch (Exception ex)
            {
                Message += "<br/> " + ex.Message;
            }
            return result;
        }


        /// <summary>
        /// Deactivates old record & Insert new Record.
        /// </summary>
        /// <param name="newFilePath"></param>
        /// <param name="oldAttachId"></param>
        /// <param name="newAttachmentId"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public bool UploadAudio(string newFilePath, long oldAttachId, out long newAttachmentId, int createdBy)
        {
            bool result = false;

            newAttachmentId = 0;

            try
            {
                CompassEntities compassEntities = new CompassEntities();

                var Obj = compassEntities.ProjectAuditFieldDataAttachments.Where(o => o.ID == oldAttachId).FirstOrDefault();
                Obj.Active = 0;
                Obj.ModifiedBy = createdBy;
                Obj.ModifiedOn = new CommonFunctions().ServerDate();
                compassEntities.SaveChanges();


                ProjectAuditFieldDataAttachment objTblProjectFieldDataAttachment = new ProjectAuditFieldDataAttachment();
                objTblProjectFieldDataAttachment.Active = 1;
                objTblProjectFieldDataAttachment.CreatedOn = new CommonFunctions().ServerDate();
                objTblProjectFieldDataAttachment.CreatedBy = createdBy;
                objTblProjectFieldDataAttachment.IsAudioVideoImage = 1;
                objTblProjectFieldDataAttachment.IsServiceData = true;
                objTblProjectFieldDataAttachment.StrFilePath = newFilePath;
                objTblProjectFieldDataAttachment.FK_ProjectAuditFieldDataMaster_ID = Obj.FK_ProjectAuditFieldDataMaster_ID;
                objTblProjectFieldDataAttachment.Fk_tblService_Id = Obj.Fk_tblService_Id;
                objTblProjectFieldDataAttachment.FK_TblServicePicture_ID = Obj.FK_TblServicePicture_ID;
                compassEntities.ProjectAuditFieldDataAttachments.Add(objTblProjectFieldDataAttachment);
                compassEntities.SaveChanges();
                newAttachmentId = objTblProjectFieldDataAttachment.ID;
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }



        /// <summary>
        /// Deactivates old record & Insert new Record.
        /// </summary>
        /// <param name="newFilePath"></param>
        /// <param name="oldAttachId"></param>
        /// <param name="newAttachmentId"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public bool UploadVideo(string newFilePath, long oldAttachId, out long newAttachmentId, int createdBy)
        {
            bool result = false;

            newAttachmentId = 0;

            try
            {
                CompassEntities compassEntities = new CompassEntities();

                var Obj = compassEntities.ProjectAuditFieldDataAttachments.Where(o => o.ID == oldAttachId).FirstOrDefault();
                Obj.Active = 0;
                Obj.ModifiedBy = createdBy;
                Obj.ModifiedOn = new CommonFunctions().ServerDate();
                compassEntities.SaveChanges();


                ProjectAuditFieldDataAttachment objTblProjectFieldDataAttachment = new ProjectAuditFieldDataAttachment();
                objTblProjectFieldDataAttachment.Active = 1;
                objTblProjectFieldDataAttachment.CreatedOn = new CommonFunctions().ServerDate();
                objTblProjectFieldDataAttachment.CreatedBy = createdBy;
                objTblProjectFieldDataAttachment.IsAudioVideoImage = 2;
                objTblProjectFieldDataAttachment.IsServiceData = true;
                objTblProjectFieldDataAttachment.StrFilePath = newFilePath;
                objTblProjectFieldDataAttachment.FK_ProjectAuditFieldDataMaster_ID = Obj.FK_ProjectAuditFieldDataMaster_ID;
                objTblProjectFieldDataAttachment.Fk_tblService_Id = Obj.Fk_tblService_Id;
                objTblProjectFieldDataAttachment.FK_TblServicePicture_ID = Obj.FK_TblServicePicture_ID;
                compassEntities.ProjectAuditFieldDataAttachments.Add(objTblProjectFieldDataAttachment);
                compassEntities.SaveChanges();
                newAttachmentId = objTblProjectFieldDataAttachment.ID;
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        public List<ProjectAuditFieldDataMaster> saveHistory(long formId, long uploadId, long installerid, long userid)
        {
            #region

            CompassEntities ComEntities = new CompassEntities();
            var objFieldData = ComEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == formId && o.FK_UploadedExcelData_Id == uploadId && o.FK_AuditorId == installerid).FirstOrDefault();

            var objFieldDataMaster = ComEntities.ProjectAuditFieldDataMasters.Where(o => o.Active == 1 && o.FK_ProjectAuditData == objFieldData.ID).ToList();

            foreach (var item in objFieldDataMaster)
            {
                ProjectAuditFieldDataMasterHistory obj = new ProjectAuditFieldDataMasterHistory();
                obj.Active = item.Active;
                obj.CapturedDateTime = item.CapturedDateTime;
                obj.Comments = item.Comments;
                obj.CreatedOn = new CommonFunctions().ServerDate();
                obj.CreatedBy = Convert.ToInt32(userid);
                obj.FieldLatitude = item.FieldLatitude;
                obj.FieldLongitude = item.FieldLongitude;
                obj.FieldStatus = item.FieldStatus;
                obj.FieldValue = item.FieldValue;
                obj.InitialFieldId = item.InitialFieldId;
                obj.ID = item.ID;
                obj.InitialFieldName = item.InitialFieldName;
                obj.OldCreatedBy = item.CreatedBy;
                obj.OldCreatedOn = item.CreatedOn;
                obj.FK_ProjectAuditData = item.FK_ProjectAuditData;
                ComEntities.ProjectAuditFieldDataMasterHistories.Add(obj);
                ComEntities.SaveChanges();
            }

            return objFieldDataMaster;

            #endregion
        }


        public bool saveReportEdit(long formId, long uploadId, List<string> fieldName, List<string> fieldValue, List<string> strService, List<string> strAddService)
        {

            CompassEntities ComEntities = new CompassEntities();
            var objFieldData = ComEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == formId && o.FK_UploadedExcelData_Id == uploadId).FirstOrDefault();

            var objFieldDataMaster = ComEntities.ProjectAuditFieldDataMasters.Where(o => o.Active == 1 && o.FK_ProjectAuditData == objFieldData.ID).ToList();

            int i = 0;
            foreach (var item in fieldName)
            {
                var aRecord = objFieldDataMaster.Where(o => o.Active == 1 && o.InitialFieldName == item).FirstOrDefault();
                aRecord.FieldValue = fieldValue[i];
                aRecord.ModifiedOn = new CommonFunctions().ServerDate();
                ComEntities.SaveChanges();
                i++;
            }

            string strServiceId = "";
            string strAddServiceId = "";


            foreach (var item in strService)
            {
                var gRecord = ComEntities.tblServices.Where(o => o.Service == item).FirstOrDefault();
                if (strServiceId.Length > 0)
                {
                    strServiceId = strServiceId + "|";
                }
                strServiceId = strServiceId + gRecord.ID.ToString();
            }

            var aRecord1 = objFieldDataMaster.Where(o => o.Active == 1 && o.InitialFieldName == "SERVICES").FirstOrDefault();
            if (aRecord1 != null)
            {
                aRecord1.FieldValue = strServiceId;
                aRecord1.ModifiedOn = new CommonFunctions().ServerDate();
            }
            ComEntities.SaveChanges();



            foreach (var item in strAddService)
            {
                var gRecord = ComEntities.tblServices.Where(o => o.Service == item).FirstOrDefault();
                if (strAddServiceId.Length > 0)
                {
                    strAddServiceId = strAddServiceId + "|";
                }
                strAddServiceId = strAddServiceId + gRecord.ID.ToString();
            }

            var aRecord2 = objFieldDataMaster.Where(o => o.Active == 1 && o.InitialFieldName == "ADDITIONAL SERVICES").FirstOrDefault();
            if (aRecord2 != null)
            {
                aRecord2.FieldValue = strAddServiceId;
                aRecord2.ModifiedOn = new CommonFunctions().ServerDate();
            }
            ComEntities.SaveChanges();

            return true;
        }



        public List<tblService> getServiceList()
        {
            CompassEntities ComEntities = new CompassEntities();

            var objServicesList = ComEntities.tblServices.ToList();
            return objServicesList;
        }

        public List<ProjectAuditFieldDataAttachment> getAudioVIdeoList(long userId, long formId, long uploadId)
        {
            CompassEntities ComEntities = new CompassEntities();
            List<ProjectAuditFieldDataAttachment> objList = new List<ProjectAuditFieldDataAttachment>();

            var result = ComEntities.ProjectAuditDatas.Where(o => o.FK_AuditorId == userId && o.FormId == formId && o.FK_UploadedExcelData_Id == uploadId && o.Active == 1).FirstOrDefault();

            if (result != null)
            {
                var resData = ComEntities.ProjectAuditFieldDataMasters.Where(o => o.FK_ProjectAuditData == result.ID && o.Active == 1).ToList();

                foreach (var item in resData)
                {
                    var resultData = ComEntities.ProjectAuditFieldDataAttachments.Where(o => o.FK_ProjectAuditFieldDataMaster_ID == item.ID && o.Active == 1).ToList();
                    objList.AddRange(resultData);
                }
            }
            return objList;
        }


        public ProjectAuditFieldDataMaster getFieldValue(long userId, long formId, long uploadId, string _fieldName)
        {
            CompassEntities ComEntities = new CompassEntities();

            var result = ComEntities.ProjectAuditDatas.Where(o => o.FK_AuditorId == userId && o.FormId == formId && o.FK_UploadedExcelData_Id == uploadId && o.Active == 1).FirstOrDefault();

            ProjectAuditFieldDataMaster resData = ComEntities.ProjectAuditFieldDataMasters.
                Where(o => o.InitialFieldName == _fieldName && o.Active == 1 && o.FK_ProjectAuditData == result.ID).FirstOrDefault();

            return resData;
        }


        /// <summary>
        /// Returns id from tblFieldDataMaster table for current field
        /// </summary>
        /// <param name="userId">installer id who visited the location</param>
        /// <param name="formId">form id of the form which filled by installer</param>
        /// <param name="uploadId">record id for which visit completed </param>
        /// <param name="_fieldName">Field name</param>
        /// <returns></returns>
        public long getFieldId(long userId, long formId, long uploadId, string _fieldName)
        {
            CompassEntities ComEntities = new CompassEntities();

            var result = ComEntities.ProjectAuditDatas.Where(o => o.FK_AuditorId == userId && o.FormId == formId && o.FK_UploadedExcelData_Id == uploadId && o.Active == 1).FirstOrDefault();

            var resData = ComEntities.ProjectAuditFieldDataMasters.
                Where(o => o.InitialFieldName == _fieldName && o.Active == 1 && o.FK_ProjectAuditData == result.ID).Select(o => o.ID).FirstOrDefault();

            return resData;
        }



        /// <summary>
        /// returns image list for the custom fields from the attachment table.
        /// </summary>
        /// <param name="fieldMasterId"></param>
        /// <returns></returns>
        public List<ProjectAuditFieldDataAttachment> getFieldImageData(long fieldMasterId)
        {
            CompassEntities ComEntities = new CompassEntities();


            var resData = ComEntities.ProjectAuditFieldDataAttachments.
                Where(o => o.Active == 1 && o.IsAudioVideoImage == 3 && o.FK_ProjectAuditFieldDataMaster_ID == fieldMasterId).ToList();

            return resData;
        }



        /// <summary>
        /// returns image list for the service from the attachment table.
        /// </summary>
        /// <param name="initialFieldId"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public List<ProjectAuditFieldDataAttachment> getServiceFieldImageData(long initialFieldId, long serviceId)
        {
            CompassEntities ComEntities = new CompassEntities();


            var resData = ComEntities.ProjectAuditFieldDataAttachments.
                Where(o => o.Active == 1 && o.IsAudioVideoImage == 3 && o.FK_ProjectAuditFieldDataMaster_ID == initialFieldId
                && o.IsServiceData == true && o.Fk_tblService_Id == serviceId).ToList();
            foreach (var item in resData)
            {
                var a = ComEntities.TblServicePictures.Where(o => o.ID == item.FK_TblServicePicture_ID).FirstOrDefault();
                if (a != null)
                {
                    item.ImageName = ComEntities.tblPictures.Where(o => o.ID == a.tblPicture_PictureId).Select(o => o.Picture).FirstOrDefault();
                }
            }


            return resData;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialFieldId"></param>
        /// <returns></returns>
        public string getServiceFieldImagePath(long initialFieldId)
        {
            CompassEntities ComEntities = new CompassEntities();

            var resData = ComEntities.TblProjectFieldDataAttachments.
            Where(o => o.Active == 1 && o.IsAudioVideoImage == 3 && o.ID == initialFieldId).FirstOrDefault();

            return (resData != null ? resData.strFilePath : "");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialFieldId"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public long saveRotatedImageFilePath(long initialFieldId, string filePath)
        {
            CompassEntities ComEntities = new CompassEntities();

            var resData = ComEntities.TblProjectFieldDataAttachments.
            Where(o => o.Active == 1 && o.IsAudioVideoImage == 3 && o.ID == initialFieldId).FirstOrDefault();
            resData.Active = 0;
            ComEntities.SaveChanges();
            TblProjectFieldDataAttachment objTblProjectFieldDataAttachment = new TblProjectFieldDataAttachment();
            objTblProjectFieldDataAttachment.Active = 1;
            objTblProjectFieldDataAttachment.CreatedOn = new CommonFunctions().ServerDate();
            objTblProjectFieldDataAttachment.IsAudioVideoImage = 3;
            objTblProjectFieldDataAttachment.isServiceData = resData.isServiceData;
            objTblProjectFieldDataAttachment.IsSkipped = resData.IsSkipped;
            objTblProjectFieldDataAttachment.strFilePath = filePath;
            objTblProjectFieldDataAttachment.tblProjectFieldDataMaster_ID = resData.tblProjectFieldDataMaster_ID;
            objTblProjectFieldDataAttachment.tblService_Id = resData.tblService_Id;
            objTblProjectFieldDataAttachment.TblServicePicture_ID = resData.TblServicePicture_ID;

            ComEntities.TblProjectFieldDataAttachments.Add(objTblProjectFieldDataAttachment);
            ComEntities.SaveChanges();

            return objTblProjectFieldDataAttachment.ID;
        }


        // Added By vishal For get audio and video path
        public string getServiceFieldAudioVideoPath(long initialFieldId)
        {
            CompassEntities ComEntities = new CompassEntities();

            var resData = ComEntities.ProjectAuditFieldDataAttachments.
            Where(o => o.Active == 1 && (o.IsAudioVideoImage == 1 || o.IsAudioVideoImage == 2) && o.ID == initialFieldId).FirstOrDefault();

            return (resData != null ? resData.StrFilePath : "");
        }

        public GetAttachmentBo GetAttachmentDetails(long attachmentID)
        {
            GetAttachmentBo List = new GetAttachmentBo();
            using (CompassEntities ComEntities = new CompassEntities())
            {
                List = (from b in ComEntities.ProjectAuditFieldDataAttachments
                        join c in ComEntities.tblServices on b.Fk_tblService_Id equals c.ID
                        where b.ID == attachmentID && b.Active == 1
                        select new GetAttachmentBo
                        {
                            AttachmentID = b.ID,
                            strPath = b.StrFilePath,
                            strServiceName = c.Service
                        }).FirstOrDefault();
            }
            return List;
        }




        #region Added code by vishal to send mail to installer when issue reopen
        public string GetSubject()
        {
            string subject = "";
            CompassEntities dbcontext = new CompassEntities();
            var ObjTblEmailConfigure = (from b in dbcontext.TblAlertMasters
                                        join c in dbcontext.TblEmailConfigures on b.AlertId equals c.AlertTableID
                                        where b.AlertId == 3
                                        select new EmailConfigureBO
                                        {
                                            Subject = c.Subject
                                        }).FirstOrDefault();

            if (ObjTblEmailConfigure != null)
            {
                subject = ObjTblEmailConfigure.Subject;
            }
            else
            {
                subject = null;
            }
            return subject;
        }

        public List<String> CCEmailTo()
        {
            List<String> List = new List<String>();
            CompassEntities db = new CompassEntities();
            var CCEmailId = (from b in db.TblAlertMasters
                             join c in db.TblEmailConfigures on b.AlertId equals c.AlertTableID
                             join d in db.TblEmailConfigureUserDetails on c.ID equals d.TblEmailConfigureID
                             where b.AlertName == "On ReOpen Record"
                             select d.UserID
                             )
                             .ToList();
            if (CCEmailId.Count > 0)
            {
                foreach (var item in CCEmailId)
                {
                    var UserEmailId = db.tblUsers.Where(n => n.UserID == item).FirstOrDefault();
                    if (UserEmailId != null)
                    {
                        List.Add(UserEmailId.Email);
                    }
                }
            }
            return List;
        }

        public string SendMalil(string Comment, long uploadId, long installerId, long formId)
        {
            DateTime dtserverdate = new CommonFunctions().ServerDate();
            StringBuilder sb = new StringBuilder();

            //var Message = "";
            var Message = GetMessage();
            ReOpneMail List = new ReOpneMail();
            List = GetReportData(uploadId, installerId, formId);
            CompassEntities dbcontext = new CompassEntities();
            var InstallerName = (dbcontext.tblUsers.Where(n => n.UserID == installerId)).FirstOrDefault();
            sb.Append("<body  style='padding: 10px 0px 0px 10px;background: #f8f8fb; font-family:Times New Roman; font-size:14px;'>");

            sb.Append("Dear ");
            if (InstallerName != null)
            {
                sb.Append(" " + InstallerName.FirstName + " ,<br/>");
            }
            else
            {
                sb.Append(",<br/>");
            }
            sb.Append("  <p style='font-size:15px;'>Greetings from CMS....!</p>");
            sb.Append("<br/><br/> <p >This is to inform you that a visit is Reopened and allocated to you by Data Depot Team. Please check below details.</p><br/>");
            sb.Append("<div style='background: #e6e1e1; width:100%;'>" + Message + "</div><br/>");

            var ObjInstallerMapDatas = (dbcontext.tblInstallerMapDatas.Where(n => n.InstallerId == installerId && n.tblUploadedData_ID == uploadId)).FirstOrDefault();

            var ProjectModel = (from b in dbcontext.tblFormMasters
                                join c in dbcontext.tblProjects on b.ProjectId equals c.ProjectId
                                join e in dbcontext.tblStateMasters on c.StateId equals e.StateId
                                join f in dbcontext.TblCityMasters on c.CityId equals f.CityId
                                where b.FormId == formId
                                select new ProjectModel
                                {
                                    ProjectName = c.ProjectName,
                                    StateName = e.StateName,
                                    CityName = f.CityName,
                                    Utilitytype = c.Utilitytype
                                }).FirstOrDefault();


            sb.Append("<div style='background: #e6e1e1; width:100%;'><table style='border:none;padding: 0px 0px 0px 10px;'>" +
                            "<tr>" +
                                "<td style='border:none;'><b>Project Name </b></td>" +
                                "<td style='border:none;'><b>:</b></td>" +
                                "<td style='border:none;' colspan='2'>" + ProjectModel.ProjectName.ToUpper() + "</td>" +
                            "</tr>" +

                            "<tr>" +
                                "<td style='border:none;'><b>Utility Type</b></td>" +
                                "<td style='border:none;'><b>:</b></td>" +
                                "<td style='border:none;' colspan='4'>" + ProjectModel.Utilitytype + "</td>" +
                            "</tr>" +

                            "<tr>" +
                                "<td style='border:none;'><b>City:</b></td>" +
                                "<td style='border:none;'><b>:</b></td>" +
                               " <td style='border:none;'>" + ProjectModel.CityName.ToUpper() + " &nbsp;&nbsp;</td>" +

                               " <td style='border:none;'><b>State</b></td>" +
                               " <td style='border:none;'><b>:</b></td>" +
                                "<td style='border:none;'>" + ProjectModel.StateName.ToUpper() + "</td>" +
                           " </tr>" +
                           "<tr>" +
                               " <td style='border:none;'><b>Visit Start Date</b></td>" +
                               " <td style='border:none;'><b>:</b></td>" +
                                "<td style='border:none;'>" + ObjInstallerMapDatas.FromDate.ToString("MMM-dd-yyyy") + "   &nbsp;&nbsp;</td>" +


                                 "<td style='border:none;'><b>Visit End Date</b></td>" +
                               " <td style='border:none;'><b>:</b></td>" +
                                "<td style='border:none;'>" + ObjInstallerMapDatas.ToDate.ToString("MMM-dd-yyyy") + "   </td>" +
                           "</tr>" +
                          "</table></div><br />");


            sb.Append("<p>Reopened & Allocated Visit -</p>");
            sb.Append("<p>Comment from Admin - " + Comment + "</p><br/>");
            sb.Append("<table style='border:none;padding: 0px 0px 0px 10px;'>");
            sb.Append("<thead>" +
                "<tr style='font-size:13px;background-color:#80B2DF;'> " +
                    "<th style='border: 1px solid black;padding: 0px 8px;'>Sr.No</th> " +
                    "<th style='border: 1px solid black;padding: 0px 8px;'>Cycle</th> " +
                    "<th style='border: 1px solid black;padding: 0px 10px;'>Route</th> " +
                    "<th style='border: 1px solid black;padding: 0px 10px;'>Account Number</th> " +
                    "<th style='border: 1px solid black;padding: 0px 20px;'>Customer Name</th> " +
                    "<th style='border: 1px solid black;padding: 0px 34px;'>Street</th> " +
                    "<th style='border: 1px solid black;padding: 0px 10px;'>Old Meter Number</th> " +
                    "<th style='border: 1px solid black;padding: 0px 10px;'>Old Meter Radio Number</th> " +
                "</tr>" +
            "</thead>");


            sb.Append("<tbody><tr style='font-size:13px; background-color: #c1dffa; text-align: center;' >");
            sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:5%;'>1</td>");
            sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:5%;'>" + List.Cycle + "</td>");
            sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:5%;'>" + List.Route + "</td>");
            sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:10%;'>" + List.Account + "</td>");
            sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:25%;'>" + (!string.IsNullOrEmpty(List.CustomerName) ? List.CustomerName : "-") + "</td>");
            sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:30%;'>" + List.Street + "</td>");
            sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:10%;'>" + (!string.IsNullOrEmpty(List.OldMeterNo) ? List.OldMeterNo : "-") + "</td>");
            sb.Append("<td style='border: 1px solid black;font-weight:500; text-align: center; width:10%;'>" + (!string.IsNullOrEmpty(List.OldMeterRadioNo) ? List.OldMeterRadioNo : "-") + "</td>");
            sb.Append("</tr>");

            sb.Append("</tbody>");
            sb.Append("</table><br/>");

            sb.Append("<br/><p>Comment from Admin - " + Comment + "</p><br/>");

            sb.Append("Thank You,<br/>Data Depot Team.<br>" +
              "<p style='text-align:center;'> <a href='https://www.datadepotonline.com/' title='' rel='home'>" +
              "<img src='https://www.datadepotonline.com/images/logo_03_2.png' title='Data Depot' alt='' style='height:100px; width:200px;'>" +
              "</a> </p>" +
              "<p style='text-align:center; font-size:13px;'>You are receiving this notification because you are registered with Data Depot <br/>" +
             " ©" + DateTime.Now.ToString("yyyy") + " Data Depot</p> <br>");
            sb.Append("<p style='text-align:center; color:red; font-size:15px;'>*** This Email is system generated. Please do not reply to this email ID." +
                "For any queries please contact Data Depot Team. ***</p>");

            sb.Append("<p style='text-align:center; color:red; font-size:13px;'>This message (including any attachments) contains confidential information" +
                " intended for a specific individual and purpose and is protected by law." +
            " If you are not the intended recipient, you should delete this message." +
            " Any disclosure, copying or distribution of this message, or the taking of any action based on it, is strictly prohibited.</p>");


            sb.Append("</body>");
            string MailBody = sb.ToString();
            return MailBody;

        }

        public ReOpneMail GetReportData(long uploadId, long installerId, long formId)
        {

            ReOpneMail List = new ReOpneMail();
            using (CompassEntities dbcontext = new CompassEntities())
            {
                var FirstName = (from c in dbcontext.tblUsers where c.UserID == installerId select c.FirstName).FirstOrDefault();
                var FormName = (from d in dbcontext.tblFormMasters where d.FormId == formId select d.FormName).FirstOrDefault();
                var Dates = (from e in dbcontext.TblProjectFieldDatas
                             where e.FK_UploadedExcelData_Id == uploadId
                                 && e.InstallerId == installerId
                             select e.visitdatetime).FirstOrDefault();
                DateTime DT = Convert.ToDateTime(Dates);

                var projectId = (from d in dbcontext.tblFormMasters where d.FormId == formId select d.ProjectId).FirstOrDefault();

                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "PROC_DynamicRow_By_Id";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@ProjectId", projectId);
                cmd.Parameters.Add("@UploadId", uploadId);
                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    List = new ReOpneMail
                    {
                        Street = reader["Street"].ToString(),
                        Cycle = reader["Cycle"].ToString(),
                        Route = reader["Route"].ToString(),
                        Account = reader["Account"].ToString(),
                        FirstName = FirstName,
                        FormName = FormName,
                        Dates = DT,
                        OldMeterNo = reader["OldMeterNo"].ToString(),
                        OldMeterRadioNo = reader["OldMeterRadioNo"].ToString(),
                        CustomerName = reader["Customer"].ToString()
                    };

                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();



            }


            return List;
        }

        public string GetMessage()
        {
            string Message = "";
            CompassEntities dbcontext = new CompassEntities();
            var ObjTblEmailConfigure = (from b in dbcontext.TblAlertMasters
                                        join c in dbcontext.TblEmailConfigures on b.AlertId equals c.AlertTableID
                                        where b.AlertId == 3
                                        select new EmailConfigureBO
                                        {
                                            Message = c.Message
                                        }).FirstOrDefault();

            if (ObjTblEmailConfigure != null)
            {
                Message = ObjTblEmailConfigure.Message;
            }
            else
            {
                Message = null;
            }
            return Message;
        }


        #endregion


        #region Createdby AniketJ on 2-sep-2016--To export SKIP_RTU records

        /// <summary>
        /// Get Report Header by uploaded ID
        /// </summary>
        /// <param name="uploadID"></param>
        /// <returns></returns>
        public ReportHeaderModel GetReportHeaderModel(long uploadID)
        {
            CompassEntities CompassEntities = new CompassEntities();
            ReportHeaderModel reportheader = new ReportHeaderModel();
            try
            {
                //reportheader = CompassEntities.STP_GetReportHeaderFields(uploadID).Select(a => new ReportHeaderModel
                //{
                //    FormName = a.formname,
                //    ProjectName = a.projectname,
                //    ProjectDate = Convert.ToDateTime(a.visiteddate).ToString("MMM-dd-yyyy hh:mm tt"),
                //    InstallerName = a.firstname,

                //}).FirstOrDefault();
                reportheader = CompassEntities.PROC_ReportPDF_Audit_GetReportHeaderFields(uploadID).Select(a => new ReportHeaderModel
                {
                    FormName = a.formname,
                    ProjectName = a.projectname,
                    ProjectDate = Convert.ToDateTime(a.visiteddate).ToString("MMM-dd-yyyy hh:mm tt"),
                    AuditorName = a.firstname,

                }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return reportheader;
        }

        /// <summary>
        /// To Get SKIP/RTU records from list by uploadID
        /// </summary>
        /// <param name="ReportList"></param>
        /// <param name="uploadID"></param>
        /// <returns></returns>
        public ReportSkipRTUModel SKIP_RTU_Records(List<STP_GetReportTableNew_bak_Result> ReportList, long uploadID, long formId)
        {
            ReportSkipRTUModel ReportSkipRTUModel = new ReportSkipRTUModel();
            try
            {
                ReportSkipRTUModel = ReportList.Where(i => i.ID == uploadID && i.FormId == formId).Select(a => new ReportSkipRTUModel
                {
                    Dates = a.Dates,
                    Account = a.Account,
                    Cycle = a.Cycle,
                    Route = a.Route,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    Street = a.Street,
                    FirstName = a.FirstName,
                    FormName = a.FormName,
                    SkipReason = a.SkipReason,
                    CategoryType = a.CategoryType,
                    SkipComment = a.SkipComment,
                    ShortComment = a.ShortComment,
                    ProjectStatus = a.ProjectStatus,
                    FormId = a.FormId,
                    ID = a.ID,
                    InstallerId = a.InstallerId,
                    AuditorId = a.AuditorId
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return ReportSkipRTUModel;
        }

        #endregion



        // Method used to get the skiprtu model along with image paths
        public List<SkipRtuAttachmentModel> getSkipRtuImages(long? _uploadId, long? _installerId, long? _formId)
        {
            long uploadId = long.Parse(_uploadId.ToString());
            long installerId = long.Parse(_installerId.ToString());
            long formId = long.Parse(_formId.ToString());
            CompassEntities dbcontext = new CompassEntities();

            var ID = dbcontext.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == formId
                && o.FK_UploadedExcelData_Id == uploadId && o.FK_AuditorId == installerId).FirstOrDefault();

            List<SkipRtuAttachmentModel> paths = new List<SkipRtuAttachmentModel>();
            if (ID != null)
            {
                paths = dbcontext.ProjectAuditFieldDataAttachments.AsNoTracking()
                         .Where(o => o.FK_ProjectAuditFieldDataId == ID.ID && o.IsSkipped == true &&
                         o.Active == 1).Select(o => new SkipRtuAttachmentModel
                         {
                             id = o.ID,
                             IsAudioVideoImage = o.IsAudioVideoImage,
                             strFilePath = o.StrFilePath
                         }).OrderBy(o => o.IsAudioVideoImage).ToList();
                // var MapPaths = paths.Where(o => o.IsAudioVideoImage == 3 && o.strFilePath.Contains("MAP\\img.png")).ToList();
                paths.RemoveAll(o => o.IsAudioVideoImage == 3 && o.strFilePath.Contains("MAP\\img.png"));
            }
            return paths;

        }


        #region CreatedBy AniketJ on 18-Oct-2016 TO get ProjectFieldProperty

        public IEnumerable<tblProjectFieldProperty> ProjectFieldPropertyList(long formid, long projectid)
        {
            IEnumerable<tblProjectFieldProperty> ProjectFieldPropertyBO;
            CompassEntities dbcontext = new CompassEntities();

            ProjectFieldPropertyBO = (from fm in dbcontext.tblFormMasters
                                      join fsr in dbcontext.tblFormSectionRelations on fm.FormId equals fsr.FormID
                                      join fsfr in dbcontext.tblFormSectionFieldRelations on fsr.SectionId equals fsfr.SectionId
                                      join pfp in dbcontext.tblProjectFieldProperties on fsfr.FieldId equals pfp.FormSectionFieldID
                                      where fsr.Active == 1 && fsfr.Active == 1 && pfp.Active == 1 && fm.FormId == formid && fm.ProjectId == projectid
                                      select pfp).AsEnumerable();



            return ProjectFieldPropertyBO;
        }


        #endregion

        //AddedBy AniketJ on 25-Oct-2016
        //to get picture list
        public List<string> GetPictureListData(long ServiceId)
        {
            List<string> picList = new List<string>();

            CompassEntities CompassEntities = new CompassEntities();
            //var a = CompassEntities.TblProjectMasterFieldRelations.Where(o => o.ID == ServiceId).FirstOrDefault();
            picList = CompassEntities.VW_GetPictures.Where(o => o.tblService_ServiceId == ServiceId).OrderBy(o => o.Picture).Select(o => o.Picture).ToList();

            return picList;
        }



        public void GetReportEditData(long uploadId, long formId, long userId, out ProjectAuditData objProjectAuditData, out List<ProjectAuditFieldDataMaster> objListProjectAuditFieldDataMaster)
        {

            CompassEntities dbcontext = new CompassEntities();

            var fieldData = dbcontext.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == formId
               && o.FK_UploadedExcelData_Id == uploadId && o.FK_AuditorId == userId).FirstOrDefault();
            objProjectAuditData = fieldData;
            objListProjectAuditFieldDataMaster = dbcontext.ProjectAuditFieldDataMasters.Where(o => o.Active == 1 && o.FK_ProjectAuditData == fieldData.ID).ToList();


        }
        //AddedBy AniketJ on 19-Jan-2016
        // Method used to get the skiprtu model along with image paths
        public string getSkipRtuGPSLocation(long? _uploadId, long? _installerId, long? _formId)
        {
            long uploadId = long.Parse(_uploadId.ToString());
            long installerId = long.Parse(_installerId.ToString());
            long formId = long.Parse(_formId.ToString());

            string skipGPSLocation = "";


            CompassEntities dbcontext = new CompassEntities();

            var ID = dbcontext.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == formId
                && o.FK_UploadedExcelData_Id == uploadId && o.FK_AuditorId == installerId).FirstOrDefault();

            skipGPSLocation = Convert.ToString(ID.SkipGPSLocation);
            return skipGPSLocation;

        }



        //unused code
        public List<UsersSmallModel> GeInstallerListbyProjectID(long ProjectId)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<UsersSmallModel> InstallerList = (from b in CompassEntities.TblProjectInstallers.AsNoTracking()
                                                       where b.Active == 1 && b.ProjectID == ProjectId
                                                       select new UsersSmallModel
                                                       {
                                                           UserId = b.tblUsers_UserID,
                                                           UserName = CompassEntities.tblUsers.Where(a => a.UserID == b.tblUsers_UserID).Select(m => m.FirstName).FirstOrDefault()
                                                       }).OrderBy(a => a.UserName).ToList();


                var InstallerList1 = CompassEntities.TblProjectInstallers.AsNoTracking().Where(o => o.Active == 0 && o.ProjectID == ProjectId).ToList();
                if (InstallerList1 != null && InstallerList1.Count > 0)
                {
                    foreach (var item in InstallerList1)
                    {
                        var checkExist = (from post in CompassEntities.UploadedExcelDatas
                                          join meta in CompassEntities.TblProjectFieldDatas on post.Id equals meta.FK_UploadedExcelData_Id
                                          where meta.Active == 1 && post.FKProjectId == ProjectId
                                          select new { id = meta.ID }).FirstOrDefault();

                        if (checkExist != null)
                        {
                            InstallerList.Add(
                                new UsersSmallModel
                                {
                                    UserId = item.tblUsers_UserID,
                                    UserName = CompassEntities.tblUsers.Where(a => a.UserID == item.tblUsers_UserID).Select(m => m.FirstName).FirstOrDefault()
                                }
                                );
                        }
                    }
                }


                return InstallerList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }






        public string getMultipleMapPins(long? uploadid, long? Formid)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            string paths = Convert.ToString((from o in objCompassEntities.ProjectAuditDatas.AsNoTracking()
                                             join ua in objCompassEntities.ProjectAuditFieldDataAttachments.AsNoTracking()
                                             on o.ID equals ua.FK_ProjectAuditFieldDataId
                                             where o.FormId == Formid && o.FK_UploadedExcelData_Id == uploadid && ua.IsSkipped == false //&& o.strFilePath.Contains("MultipleMapPins")
                                             select ua.StrFilePath).FirstOrDefault());


            return paths;
        }
    }
}
