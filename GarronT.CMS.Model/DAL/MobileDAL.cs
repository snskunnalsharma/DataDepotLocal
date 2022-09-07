using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace GarronT.CMS.Model.DAL
{
    public class MobileDAL : IDisposable
    {

        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            //if (!disposed)
            //{
            //    if (disposing)
            //    {
            //        //dispose managed ressources

            //    }
            //}
            //dispose unmanaged ressources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<TodaysVisitInfo> GetVistByUserId(long userId, DateTime fromDate, DateTime toDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            List<TodaysVisitInfo> objUploadedData = new List<TodaysVisitInfo>();

            // DateTime currentdate = new CommonFunctions().ServerDate();

            var getAllMapedList = objCompassEntities.STP_GetUserWiseVisits(userId, fromDate, toDate).ToList();

            var proInfo = getAllMapedList.Select(o => o.ProjectId).Distinct().ToList();

            foreach (var item in proInfo)
            {
                if (item != null)
                {
                    TodaysVisitInfo obj1 = new TodaysVisitInfo();
                    obj1.projectId = Convert.ToString(item);
                    obj1.projectName = getAllMapedList.Where(o => o.ProjectId == item).Select(o => o.ProjectName).FirstOrDefault();
                    var getRecords = getAllMapedList.Where(o => o.ProjectId == item).ToList();
                    List<visitInfo> objvisitList = new List<visitInfo>();
                    foreach (var item1 in getRecords)
                    {
                        visitInfo obj2 = new visitInfo();
                        obj2.Id = item1.ID.ToString();
                        obj2.account = item1.Account;
                        obj2.Latitude = Convert.ToString(item1.Latitude);
                        obj2.Longitude = Convert.ToString(item1.Longitude);
                        obj2.street = item1.Street;
                        obj2.isVisited = item1.IsVisisted == 0 ? false : true;
                        objvisitList.Add(obj2);
                    }
                    obj1.visits = objvisitList;
                    objUploadedData.Add(obj1);
                }
            }

            return objUploadedData;
        }

        public List<ProjectMapVisitInfo> GetTodaysMenuSectionVistByUserId(long userId, DateTime fromDate, DateTime toDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            List<ProjectMapVisitInfo> objUploadedData = new List<ProjectMapVisitInfo>();

            // DateTime currentdate = new CommonFunctions().ServerDate();

            // var getAllMapedList = objCompassEntities.STP_GetUserWiseVisits(userId, fromDate, toDate).ToList();
            var getAllMapedList = objCompassEntities.STP_GetUserWiseTodaysVisits(userId, fromDate, toDate).ToList();
            var proInfo = getAllMapedList.Select(o => o.ProjectId).Distinct().ToList();

            foreach (var item in proInfo)
            {
                if (item != null)
                {
                    ProjectMapVisitInfo obj1 = new ProjectMapVisitInfo();
                    obj1.projectId = Convert.ToString(item);
                    obj1.projectName = getAllMapedList.Where(o => o.ProjectId == item).Select(o => o.ProjectName).FirstOrDefault();
                    var getRecords = getAllMapedList.Where(o => o.ProjectId == item).OrderBy(o => o.IsVisited).ToList();
                    List<visitMapInfo> objvisitList = new List<visitMapInfo>();
                    var FormList = objCompassEntities.tblFormMasters.Where(o => o.Active == 1 && o.ProjectId == item).ToList();
                    int formCount = FormList == null ? 0 : FormList.Count;
                    foreach (var item1 in getRecords)
                    {
                        var result = objvisitList.Where(o => o.street.ToLower() == item1.Street.ToLower()).FirstOrDefault();
                        if (result == null)
                        {
                            visitMapInfo obj2 = new visitMapInfo();
                            obj2.latitude = Convert.ToString(item1.Latitude);
                            obj2.longitude = Convert.ToString(item1.Longitude);
                            obj2.street = item1.Street;
                            obj2.isVisited = bool.Parse(item1.IsVisited);




                            var accountList = getRecords.Where(o => o.Street == item1.Street).ToList();


                            int accountCount = accountList == null ? 0 : accountList.Count;
                            int CompleteCount = accountCount * formCount;
                            int visitCount = 0;
                            int skipCount = 0;
                            foreach (var accountItem in accountList)
                            {
                                foreach (var formItem in FormList)
                                {
                                    var a = objCompassEntities.TblProjectFieldDatas.Where(o => o.FormId == formItem.FormId &&
                                        o.tblUploadedData_Id == accountItem.ID && o.IsSkipped != true && o.Active == 1).FirstOrDefault();
                                    if (a != null)
                                    {
                                        var b = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1 && o.InitialFieldName == "WORK COMPLETED"
                                            && o.FieldValue == "true" && o.TblProjectFieldData_Id == a.ID).FirstOrDefault();
                                        if (b != null)
                                        {
                                            visitCount = visitCount + 1;
                                        }
                                    }
                                    else
                                    {
                                        a = objCompassEntities.TblProjectFieldDatas.Where(o => o.FormId == formItem.FormId &&
                                            o.tblUploadedData_Id == accountItem.ID && o.IsSkipped == true && o.Active == 1).FirstOrDefault();
                                        if (a != null)
                                        {
                                            skipCount = skipCount + 1;
                                        }
                                    }
                                }
                            }


                            if (CompleteCount == visitCount)
                            {
                                obj2.colorFlag = 4;
                            }
                            else if (skipCount > 0 && CompleteCount == (visitCount + skipCount))
                            {
                                obj2.colorFlag = 3;
                            }
                            else if (visitCount == 0)
                            {
                                obj2.colorFlag = 1;
                            }
                            else
                            {
                                obj2.colorFlag = 2;
                            }


                            objvisitList.Add(obj2);
                        }
                        else
                        {
                            if (item1.IsVisited == "false")
                            {
                                result.isVisited = false;
                            }
                        }

                    }
                    obj1.visits = objvisitList.OrderBy(o => o.isVisited).ThenBy(o => o.latitude).ThenBy(o => o.longitude).ThenBy(o => o.street).ToList();
                    objUploadedData.Add(obj1);
                }
            }
            return objUploadedData;
        }

        public List<ProjectMapVisitInfo> GetTodaysMapSectionVistByUserId(long userId, DateTime fromDate, DateTime toDate)
        {

            CompassEntities objCompassEntities = new CompassEntities();

            List<ProjectMapVisitInfo> objUploadedData = new List<ProjectMapVisitInfo>();

            // DateTime currentdate = new CommonFunctions().ServerDate();

            //var getAllMapedList = objCompassEntities.STP_GetUserWiseVisits(userId, fromDate, toDate).ToList();
            var getAllMapedList = objCompassEntities.STP_GetUserWiseTodaysVisits(userId, fromDate, toDate).ToList();

            var proInfo = getAllMapedList.Select(o => o.ProjectId).Distinct().ToList();

            foreach (var item in proInfo)
            {

                ProjectMapVisitInfo obj1 = new ProjectMapVisitInfo();
                obj1.projectId = Convert.ToString(item);
                obj1.projectName = getAllMapedList.Where(o => o.ProjectId == item).Select(o => o.ProjectName).FirstOrDefault();
                var getRecords = getAllMapedList.Where(o => o.ProjectId == item).ToList();
                List<visitMapInfo> objvisitList = new List<visitMapInfo>();
                foreach (var item1 in getRecords)
                {
                    var result = objvisitList.Where(o => o.latitude == Convert.ToString(item1.Latitude) && o.longitude == Convert.ToString(item1.Longitude)).FirstOrDefault();

                    if (result == null)
                    {

                        visitMapInfo obj2 = new visitMapInfo();
                        obj2.latitude = Convert.ToString(item1.Latitude);
                        obj2.longitude = Convert.ToString(item1.Longitude);
                        obj2.street = item1.Street;
                        obj2.isVisited = bool.Parse(item1.IsVisited);
                        objvisitList.Add(obj2);
                    }
                    else
                    {

                        string[] checkAddressList = result.street.Split(',');

                        var result1 = checkAddressList.Where(o => o.Trim().ToLower() == item1.Street.Trim().ToLower()).FirstOrDefault();
                        if (string.IsNullOrEmpty(result1))
                        {
                            result.street = result.street + "," + item1.Street;
                        }
                        if (item1.IsVisited == "false")
                        {
                            result.isVisited = false;
                        }
                    }
                }
                obj1.visits = objvisitList.OrderBy(o => o.isVisited).ThenBy(o => o.street).ToList();
                objUploadedData.Add(obj1);
            }
            return objUploadedData;

        }







        public List<FormListAgainstProject> formListAgainstAccount(long userId, long projectId, long initialFieldId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            // DateTime currentdate = new CommonFunctions().ServerDate();


            var obj = objCompassEntities.tblFormMasters.Where(o => o.ProjectId == projectId && o.Active == 1).ToList();
            List<FormListAgainstProject> objList = new List<FormListAgainstProject>();
            var formInfo = obj.Select(o => o.FormId).Distinct().ToList();
            foreach (var item in formInfo)
            {

                //form name
                //formid
                //IsVisisted
                //IsSkiped
                FormListAgainstProject objForm = new FormListAgainstProject();
                objForm.formId = item.ToString();
                objForm.formName = obj.Where(o => o.FormId == item).Select(o => o.FormName).FirstOrDefault();



                var formVisitedStatus = objCompassEntities.TblProjectFieldDatas.Where(o => o.FormId == item &&
                                                   o.InstallerId == userId && o.tblUploadedData_Id == initialFieldId && o.Active == 1).FirstOrDefault();
                if (formVisitedStatus == null)
                {
                    objForm.isVisited = false;
                    objForm.isSkipped = false;
                }
                else if (formVisitedStatus.IsSkipped == true)
                {
                    objForm.isVisited = false;
                    objForm.isSkipped = true;
                }
                else
                {

                    var getData = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.TblProjectFieldData_Id == formVisitedStatus.ID && o.Active == 1 &&
                        o.InitialFieldName == "WORK COMPLETED" && o.FieldValue.ToLower() == "true").FirstOrDefault();

                    if (getData == null)
                    {
                        objForm.isVisited = false;
                        objForm.isSkipped = false;
                    }
                    else
                    {
                        objForm.isVisited = true;
                        objForm.isSkipped = false;
                    }
                }
                objList.Add(objForm);


                //AccountsByStreet

            }

            return objList;
        }





        public ProjectFormBO getFormListDataNew(long userId, long formId, long initialId)
        {
            try
            {


                CompassEntities objCompassEntities = new CompassEntities();
                //DateTime currentdate = new CommonFunctions().ServerDate();
                DateTime fromDate = new DateTime(2016, 01, 01);
                DateTime toDate = new DateTime(2050, 01, 01);
                var obj = objCompassEntities.tblFormMasters.Where(o => o.FormId == formId).FirstOrDefault();
                var proDetails = objCompassEntities.tblProjects.Where(o => o.ProjectId == obj.ProjectId && o.Active == 1).FirstOrDefault();
                var checkMappedColumnsToExcel = objCompassEntities.PROC_WebService_GetFieldInfoForProject(obj.ProjectId).ToList();

                // List<STP_UserVisits_Result> objtblList = objCompassEntities.STP_UserVisits(fromDate, toDate, userId, obj.ProjectId).ToList().Where(o => o.ID == initialId).ToList();
                ProjectFormBO objProjectFormBO = new ProjectFormBO();
                objProjectFormBO.projectId = obj.ProjectId;
                objProjectFormBO.projectName = proDetails.ProjectName;

                List<FormSectionBO> objSectioFieldBOList = new List<FormSectionBO>();

                FormSectionBO oo = new FormSectionBO();
                oo.formId = obj.FormId;
                oo.formName = obj.FormName;



                // long projectId1 = formResult.ProjectId;
                var sectionResult = objCompassEntities.tblFormSectionRelations.Where(o => o.FormID == obj.FormId
                    && o.Active == 1).OrderBy(o => o.SortOrderNumber).ToList();


                List<MobileSectionBO> objMobileSectionBO = new List<MobileSectionBO>();
                foreach (var item2 in sectionResult)
                {
                    MobileSectionBO obj1 = new MobileSectionBO();
                    obj1.sectionId = item2.SectionId;
                    obj1.sectionName = item2.SectionName;
                    obj1.sectionOrder = item2.SortOrderNumber.ToString();
                    List<SectionFieldBO> objSectionFieldBOList = new List<SectionFieldBO>();

                    var fieldResultList = objCompassEntities.tblFormSectionFieldRelations.
                        Where(o => o.SectionId == item2.SectionId && o.Active == 1).OrderBy(o => o.FieldOrder).ToList();

                    foreach (var item3 in fieldResultList)
                    {
                        #region
                        //var au = objCompassEntities.tblProjectFieldProperties.Where(o => o.FormSectionFieldID == item3.FieldId && o.Active == 1).FirstOrDefault();                               
                        var au = objCompassEntities.tblProjectFieldProperties.Where(o => o.FormSectionFieldID == item3.FieldId
                                 && o.Active == 1 && o.HideFieldLabel != true).FirstOrDefault(); // Added HideFieldLabel where condition because HideFieldLabel==True on form filed are hide
                        if (au != null)
                        {
                            SectionFieldBO objSectionFieldBO = new SectionFieldBO();
                            objSectionFieldBO.acknowlegementButtonText = au.AcknowlegementButtonText;
                            objSectionFieldBO.acknowlegementClickedButtonText = au.AcknowlegementClickedButtonText;
                            objSectionFieldBO.acknowlegementText = au.AcknowlegementText;
                            objSectionFieldBO.allowAnnotation = au.AllowAnnotation != null ? bool.Parse(au.AllowAnnotation.ToString()) : false;
                            objSectionFieldBO.allowMultiSelection = au.AllowMultiSelection != null ? bool.Parse(au.AllowMultiSelection.ToString()) : false;
                            objSectionFieldBO.allowNotApplicable = au.AllowNotApplicable != null ? bool.Parse(au.AllowNotApplicable.ToString()) : false;
                            objSectionFieldBO.allowprii = au.allowprii != null ? bool.Parse(au.allowprii.ToString()) : false;
                            objSectionFieldBO.audioQuality = au.AudioQuality;
                            objSectionFieldBO.cameraEnabled = au.CameraEnabled != null ? bool.Parse(au.CameraEnabled.ToString()) : false;
                            objSectionFieldBO.captureGeolocation = au.CaptureGeolocation != null ? bool.Parse(au.CaptureGeolocation.ToString()) : false;
                            objSectionFieldBO.captureMode = au.CaptureMode;
                            objSectionFieldBO.captureTimestamp = au.CaptureTimestamp != null ? bool.Parse(au.CaptureTimestamp.ToString()) : false;
                            objSectionFieldBO.decimalPositions = au.DecimalPositions;
                            objSectionFieldBO.defaultValue = au.DefaultValue;
                            objSectionFieldBO.hideFieldLabel = au.HideFieldLabel != null ? bool.Parse(au.HideFieldLabel.ToString()) : false;
                            objSectionFieldBO.displayMask = au.DisplayMask;
                            objSectionFieldBO.enforceMinMax = au.EnforceMinMax != null ? bool.Parse(au.EnforceMinMax.ToString()) : false;
                            objSectionFieldBO.excludeonSync = au.ExcludeonSync != null ? bool.Parse(au.ExcludeonSync.ToString()) : false;

                            var fieldddID = objCompassEntities.tblFormSectionFieldRelations.
                                Where(o => o.FieldId == au.FormSectionFieldID && o.Active == 1).Select(o => o).FirstOrDefault();
                            var au1 = objCompassEntities.TblFieldDataTypeMasters.Where(o => o.DataTypeId == fieldddID.TblFieldDataTypeMaster_Id
                                && o.Active == 1).FirstOrDefault();

                            objSectionFieldBO.fieldDataTypeName = au1.FieldDataTypeName;
                            objSectionFieldBO.fieldFilterkey = au.FieldFilterkey;
                            objSectionFieldBO.fieldId = fieldddID.FieldId;
                            objSectionFieldBO.fieldLabel = au.FieldLabel;
                            objSectionFieldBO.fieldName = fieldddID.FieldName;
                            objSectionFieldBO.fieldOrder = fieldddID.FieldOrder;

                            objSectionFieldBO.formatMask = au.FormatMask;
                            objSectionFieldBO.gpsTagging = au.GPSTagging != null ? bool.Parse(au.GPSTagging.ToString()) : false;
                            objSectionFieldBO.hasAlert = au.HasAlert != null ? bool.Parse(au.HasAlert.ToString()) : false;
                            objSectionFieldBO.hasFile = au.HasFile != null ? bool.Parse(au.HasFile.ToString()) : false;
                            objSectionFieldBO.hasValues = au.HasValues != null ? bool.Parse(au.HasValues.ToString()) : false;
                            objSectionFieldBO.hintText = au.HintText;
                            objSectionFieldBO.isEnabled = au.IsEnabled != null ? bool.Parse(au.IsEnabled.ToString()) : false;
                            objSectionFieldBO.isRequired = au.IsRequired != null ? bool.Parse(au.IsRequired.ToString()) : false;
                            objSectionFieldBO.keyboardType = au.KeyboardType != null ? bool.Parse(au.KeyboardType.ToString()) : false;

                            objSectionFieldBO.maximumHeight = au.MaximumHeight;
                            objSectionFieldBO.maximumWidth = au.MaximumWidth;
                            objSectionFieldBO.maxLengthValue = !string.IsNullOrEmpty(au.MaxLengthValue) ? int.Parse(au.MaxLengthValue) : 250;
                            objSectionFieldBO.minLengthValue = !string.IsNullOrEmpty(au.MinLengthValue) ? int.Parse(au.MinLengthValue) : 250;
                            objSectionFieldBO.numberOfColumnsForPhones = au.NumberOfColumnsForPhones;
                            objSectionFieldBO.numberOfColumnsForTablets = au.NumberOfColumnsForTablets;
                            objSectionFieldBO.photoFile = au.PhotoFile != null ? bool.Parse(au.PhotoFile.ToString()) : false;
                            objSectionFieldBO.photoLibraryEnabled = au.PhotoLibraryEnabled != null ? bool.Parse(au.PhotoLibraryEnabled.ToString()) : false;
                            objSectionFieldBO.photoQuality = au.PhotoQuality;


                            objSectionFieldBO.ratingLabels = au.RatingLabels != null ? bool.Parse(au.RatingLabels.ToString()) : false;
                            objSectionFieldBO.ratingMax = au.RatingMax != null ? bool.Parse(au.RatingMax.ToString()) : false;
                            objSectionFieldBO.sectionId = fieldddID.SectionId;
                            objSectionFieldBO.secure = au.Secure != null ? bool.Parse(au.Secure.ToString()) : false;
                            objSectionFieldBO.showMultiselect = au.ShowMultiselect != null ? bool.Parse(au.ShowMultiselect.ToString()) : false;
                            objSectionFieldBO.showRatingLabels = au.ShowRatingLabels != null ? bool.Parse(au.ShowRatingLabels.ToString()) : false;
                            objSectionFieldBO.maxrecordabletimeInSeconds = au.MaxrecordabletimeInSeconds;
                            objSectionFieldBO.videoQuality = au.VideoQuality;


                            List<FieldInitialData> objFieldInitialDataList = new List<FieldInitialData>();

                            if (objSectionFieldBO.fieldName.ToLower().Trim() == "NEW METER SIZE".ToLower())
                            {
                                var res = objCompassEntities.TblProjectMasterFieldRelations.Where(o => o.MasterTableName.ToLower() == "tblMeterSize"
                                    && o.ProjectID == objProjectFormBO.projectId && o.Active == 1).ToList();
                                foreach (var itemres in res)
                                {
                                    var getmasterRecord = objCompassEntities.tblMeterSizes.Where(o => o.ID == itemres.MasterFieldID
                                        && o.Active == 1).FirstOrDefault();
                                    FieldInitialData objFieldInitialData = new FieldInitialData();
                                    objFieldInitialData.initialFieldId = getmasterRecord.ID;
                                    objFieldInitialData.initialFieldValue = getmasterRecord.MeterSize;
                                    objFieldInitialDataList.Add(objFieldInitialData);
                                }
                            }
                            else if (objSectionFieldBO.fieldName.ToLower().Trim() == "NEW METER MAKE".ToLower())
                            {
                                var res = objCompassEntities.TblProjectMasterFieldRelations.Where(o => o.MasterTableName.ToLower() == "tblMeterMake"
                                    && o.ProjectID == objProjectFormBO.projectId && o.Active == 1).ToList();
                                foreach (var itemres in res)
                                {
                                    var getmasterRecord = objCompassEntities.tblMeterMakes.Where(o => o.ID == itemres.MasterFieldID
                                         && o.Active == 1).FirstOrDefault();
                                    FieldInitialData objFieldInitialData = new FieldInitialData();
                                    objFieldInitialData.initialFieldId = getmasterRecord.ID;
                                    objFieldInitialData.initialFieldValue = getmasterRecord.Make;
                                    objFieldInitialDataList.Add(objFieldInitialData);
                                }
                            }
                            else if (objSectionFieldBO.fieldName.ToLower().Trim() == "NEW METER TYPE".ToLower())
                            {
                                var res = objCompassEntities.TblProjectMasterFieldRelations.Where(o => o.MasterTableName.ToLower() == "tblMeterType"
                                    && o.ProjectID == objProjectFormBO.projectId && o.Active == 1).ToList();
                                foreach (var itemres in res)
                                {
                                    var getmasterRecord = objCompassEntities.tblMeterTypes.Where(o => o.ID == itemres.MasterFieldID
                                         && o.Active == 1).FirstOrDefault();
                                    FieldInitialData objFieldInitialData = new FieldInitialData();
                                    objFieldInitialData.initialFieldId = getmasterRecord.ID;
                                    objFieldInitialData.initialFieldValue = getmasterRecord.MeterType;
                                    objFieldInitialDataList.Add(objFieldInitialData);
                                }
                            }
                            else if (objSectionFieldBO.fieldName.ToLower().Trim() == "Installer Name".ToLower())
                            {
                                var res = objCompassEntities.AuditDatas.Where(o => o.FK_UploadId == initialId && o.Active == 1).FirstOrDefault();
                                var userbo = objCompassEntities.tblUsers.Where(o => o.UserID == res.Fk_InstallerId).FirstOrDefault();
                                FieldInitialData objFieldInitialData = new FieldInitialData();
                                objFieldInitialData.initialFieldId = objSectionFieldBO.fieldId;
                                objFieldInitialData.initialFieldValue = userbo.FirstName;
                                objFieldInitialDataList.Add(objFieldInitialData);
                            }
                            else if (objSectionFieldBO.fieldName.ToLower().Trim() == "Visit DateTime".ToLower())
                            {
                                var res = objCompassEntities.AuditDatas.Where(o => o.FK_UploadId == initialId && o.Active == 1).FirstOrDefault();

                                FieldInitialData objFieldInitialData = new FieldInitialData();
                                objFieldInitialData.initialFieldId = objSectionFieldBO.fieldId;
                                objFieldInitialData.initialFieldValue = res.VisitDate.ToString();
                                objFieldInitialDataList.Add(objFieldInitialData);

                            }
                            else if (objSectionFieldBO.fieldName.ToLower().Trim() == "SERVICES".ToLower())
                            {
                                var res = objCompassEntities.TblProjectMasterFieldRelations.Where(o => o.MasterTableName.ToLower() == "tblService"
                                    && o.ProjectID == objProjectFormBO.projectId && o.Active == 1 && o.IsAdditionalService == false).ToList();
                                foreach (var itemres in res)
                                {
                                    var getmasterRecord = objCompassEntities.tblServices.Where(o => o.ID == itemres.MasterFieldID && o.Active == 1).FirstOrDefault();
                                    FieldInitialData objFieldInitialData = new FieldInitialData();
                                    objFieldInitialData.initialFieldId = getmasterRecord.ID;
                                    objFieldInitialData.initialFieldValue = getmasterRecord.Service;
                                    objFieldInitialData.isAdditionalService = false;
                                    serviceInitialData objserviceInitialData = new serviceInitialData();
                                    objserviceInitialData.flagCaptureAudio = getmasterRecord.AudioRequired == null ? false : bool.Parse(getmasterRecord.AudioRequired.ToString());
                                    objserviceInitialData.flagCaptureVideo = getmasterRecord.VideoRequired == null ? false : bool.Parse(getmasterRecord.VideoRequired.ToString());

                                    var pictureList = objCompassEntities.TblServicePictures.
                                        Where(o => o.Active == 1 && o.tblService_ServiceId == getmasterRecord.ID).Select(o => o.tblPicture_PictureId).ToList();
                                    List<string> obj1List = new List<string>();
                                    if (pictureList != null)
                                    {
                                        foreach (var item in pictureList)
                                        {
                                            var pictureName = objCompassEntities.tblPictures.Where(o => o.ID == item).Select(o => o.Picture).FirstOrDefault();
                                            obj1List.Add(pictureName);
                                        }
                                    }

                                    objserviceInitialData.imageList = obj1List;
                                    objFieldInitialData.objserviceInitialData = objserviceInitialData;
                                    objFieldInitialDataList.Add(objFieldInitialData);
                                }
                            }
                            else if (objSectionFieldBO.fieldName.ToLower().Trim() == "ADDITIONAL SERVICES".ToLower())
                            {

                                var res1 = objCompassEntities.TblProjectMasterFieldRelations.Where(o => o.MasterTableName.ToLower() == "tblService"
                                    && o.ProjectID == objProjectFormBO.projectId && o.Active == 1 && o.IsAdditionalService == true).ToList();
                                var objAllServices = objCompassEntities.tblServices.Where(o => o.Active == 1).Select(o => o).ToList();

                                foreach (var itemres in res1)
                                {

                                    var getmasterRecord1 = objAllServices.Where(o => o.ID == itemres.MasterFieldID && o.Active == 1).FirstOrDefault();

                                    FieldInitialData objFieldInitialData = new FieldInitialData();
                                    objFieldInitialData.initialFieldId = getmasterRecord1.ID;
                                    objFieldInitialData.initialFieldValue = getmasterRecord1.Service;
                                    objFieldInitialData.isAdditionalService = true;
                                    serviceInitialData objserviceInitialData = new serviceInitialData();
                                    objserviceInitialData.flagCaptureAudio = getmasterRecord1.AudioRequired == null ? false : bool.Parse(getmasterRecord1.AudioRequired.ToString());
                                    objserviceInitialData.flagCaptureVideo = getmasterRecord1.VideoRequired == null ? false : bool.Parse(getmasterRecord1.VideoRequired.ToString());

                                    var pictureList = objCompassEntities.TblServicePictures.
                                        Where(o => o.Active == 1 && o.tblService_ServiceId == itemres.MasterFieldID).Select(o => o.tblPicture_PictureId).ToList();
                                    List<string> obj1List = new List<string>();
                                    if (pictureList != null)
                                    {
                                        foreach (var item in pictureList)
                                        {
                                            var pictureName = objCompassEntities.tblPictures.Where(o => o.ID == item).Select(o => o.Picture).FirstOrDefault();
                                            obj1List.Add(pictureName);
                                        }
                                    }

                                    objserviceInitialData.imageList = obj1List;
                                    objFieldInitialData.objserviceInitialData = objserviceInitialData;
                                    objFieldInitialDataList.Add(objFieldInitialData);

                                }
                            }
                            else
                            {
                                // var checkMappedToExcel = objCompassEntities.SP_GetFieldInfoForProject(obj.ProjectId).ToList();

                                var checkfield = checkMappedColumnsToExcel.Where(o => o.FieldName == item3.FieldName).FirstOrDefault();

                                if (checkfield != null && checkfield.isMappedToExcel == 1 && checkfield.FieldName != "LOCATION/GPS")
                                {

                                    var itemres = objCompassEntities.UploadedExcelDatas.Where(o => o.Id == initialId && o.Active == 1).FirstOrDefault();
                                    FieldInitialData objFieldInitialData = new FieldInitialData();
                                    objFieldInitialData.initialFieldId = itemres.Id;
                                    //src.GetType().GetProperty(propName).GetValue(src, null);
                                    if (checkfield.FieldName.ToLower() == "account")
                                    {
                                        // objFieldInitialData.initialFieldValue = itemres.Account;
                                        var formVisitedStatus = objCompassEntities.TblProjectFieldDatas.Where(o => o.FormId == oo.formId &&
                                            o.InstallerId == userId && o.FK_UploadedExcelData_Id == itemres.Id && o.Active == 1).FirstOrDefault();
                                        if (formVisitedStatus == null)
                                        {
                                            objFieldInitialData.isVisitedForForm = false;
                                            objFieldInitialData.isPreviousSkipped = false;
                                        }
                                        else if (formVisitedStatus.IsSkipped == true)
                                        {
                                            objFieldInitialData.isVisitedForForm = false;
                                            objFieldInitialData.isPreviousSkipped = true;
                                        }
                                        else
                                        {
                                            objFieldInitialData.isVisitedForForm = true;
                                            objFieldInitialData.isPreviousSkipped = false;
                                        }
                                    }

                                    var _columnName = checkMappedColumnsToExcel.Where(o => o.FieldName.ToLower() == checkfield.FieldName.ToLower()).FirstOrDefault();

                                    if (_columnName != null)
                                    {
                                        Type tClass = itemres.GetType();
                                        PropertyInfo[] pClass = tClass.GetProperties();
                                        PropertyInfo currentProperty = pClass.Where(o => o.Name == _columnName.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                        objFieldInitialData.initialFieldValue = Convert.ToString(currentProperty.GetValue(itemres));
                                    }


                                    objFieldInitialDataList.Add(objFieldInitialData);



                                }
                                else
                                {
                                    //var res = objCompassEntities.tblProjectFieldDDLMasters.Where(o => o.FormSectionFieldID == au.ID && o.Active == 1).ToList();
                                    var res = objCompassEntities.tblProjectFieldDDLMasters.Where(o => o.FormSectionFieldID == item3.FieldId && o.Active == 1).ToList();
                                    foreach (var itemres in res)
                                    {
                                        FieldInitialData objFieldInitialData = new FieldInitialData();
                                        objFieldInitialData.initialFieldId = itemres.ID;
                                        objFieldInitialData.initialFieldValue = itemres.FieldValue;
                                        objFieldInitialDataList.Add(objFieldInitialData);
                                    }
                                }


                            }
                            objSectionFieldBO.fieldInitialDataList = objFieldInitialDataList;
                            objSectionFieldBOList.Add(objSectionFieldBO);
                            #endregion
                        }//field end
                    }
                    obj1.FieldList = objSectionFieldBOList;

                    objMobileSectionBO.Add(obj1);


                }//section end

                oo.SectionList = objMobileSectionBO;

                objSectioFieldBOList.Add(oo);

                objProjectFormBO.FormNameList = objSectioFieldBOList;

                return objProjectFormBO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public MobileFormBO getFormFieldData(long userId, long formId)
        {
            MobileFormBO obj = new MobileFormBO();

            // List<MobileSectionBO> objSectionFieldBO = new List<MobileSectionBO>();
            // List<FieldInitialData> objFieldInitialData = new List<FieldInitialData>();
            CompassEntities objCompassEntities = new CompassEntities();

            var formResult = objCompassEntities.tblFormMasters.Where(o => o.FormId == formId && o.Active == 1).FirstOrDefault();
            long projectId = formResult.ProjectId;
            var sectionResult = objCompassEntities.tblFormSectionRelations.Where(o => o.FormID == formId && o.Active == 1).ToList();
            obj.formId = formResult.FormId;
            obj.formName = formResult.FormName;

            List<MobileSectionBO> objMobileSectionBO = new List<MobileSectionBO>();
            foreach (var item in sectionResult)
            {
                MobileSectionBO obj1 = new MobileSectionBO();
                obj1.sectionId = item.SectionId;
                obj1.sectionName = item.SectionName;
                obj1.sectionOrder = item.SortOrderNumber.ToString();
                List<SectionFieldBO> objSectionFieldBOList = new List<SectionFieldBO>();

                var fieldResultList = objCompassEntities.tblFormSectionFieldRelations.Where(o => o.SectionId == item.SectionId && o.Active == 1)
                    .OrderBy(o => o.FieldOrder).ToList();

                foreach (var item1 in fieldResultList)
                {
                    var au = objCompassEntities.tblProjectFieldProperties.Where(o => o.FormSectionFieldID == item1.FieldId).FirstOrDefault();
                    SectionFieldBO objSectionFieldBO = new SectionFieldBO();
                    objSectionFieldBO.acknowlegementButtonText = au.AcknowlegementButtonText;
                    objSectionFieldBO.acknowlegementClickedButtonText = au.AcknowlegementClickedButtonText;
                    objSectionFieldBO.acknowlegementText = au.AcknowlegementText;
                    objSectionFieldBO.allowAnnotation = au.AllowAnnotation != null ? bool.Parse(au.AllowAnnotation.ToString()) : false;
                    objSectionFieldBO.allowMultiSelection = au.AllowMultiSelection != null ? bool.Parse(au.AllowMultiSelection.ToString()) : false;
                    objSectionFieldBO.allowNotApplicable = au.AllowNotApplicable != null ? bool.Parse(au.AllowNotApplicable.ToString()) : false;
                    objSectionFieldBO.allowprii = au.allowprii != null ? bool.Parse(au.allowprii.ToString()) : false;
                    objSectionFieldBO.audioQuality = au.AudioQuality;
                    objSectionFieldBO.cameraEnabled = au.CameraEnabled != null ? bool.Parse(au.CameraEnabled.ToString()) : false;
                    objSectionFieldBO.captureGeolocation = au.CaptureGeolocation != null ? bool.Parse(au.CaptureGeolocation.ToString()) : false;
                    objSectionFieldBO.captureMode = au.CaptureMode;
                    objSectionFieldBO.captureTimestamp = au.CaptureTimestamp != null ? bool.Parse(au.CaptureTimestamp.ToString()) : false;
                    objSectionFieldBO.decimalPositions = au.DecimalPositions;
                    objSectionFieldBO.defaultValue = au.DefaultValue;

                    objSectionFieldBO.displayMask = au.DisplayMask;
                    objSectionFieldBO.enforceMinMax = au.EnforceMinMax != null ? bool.Parse(au.EnforceMinMax.ToString()) : false;
                    objSectionFieldBO.excludeonSync = au.ExcludeonSync != null ? bool.Parse(au.ExcludeonSync.ToString()) : false;

                    var fieldddID = objCompassEntities.tblFormSectionFieldRelations.Where(o => o.FieldId == au.FormSectionFieldID).Select(o => o).FirstOrDefault();
                    var au1 = objCompassEntities.TblFieldDataTypeMasters.Where(o => o.DataTypeId == fieldddID.TblFieldDataTypeMaster_Id).FirstOrDefault();

                    objSectionFieldBO.fieldDataTypeName = au1.FieldDataTypeName;
                    objSectionFieldBO.fieldFilterkey = au.FieldFilterkey;
                    objSectionFieldBO.fieldId = fieldddID.FieldId;
                    objSectionFieldBO.fieldLabel = au.FieldLabel;
                    objSectionFieldBO.fieldName = fieldddID.FieldName;
                    objSectionFieldBO.fieldOrder = fieldddID.FieldOrder;

                    objSectionFieldBO.formatMask = au.FormatMask;
                    objSectionFieldBO.gpsTagging = au.GPSTagging != null ? bool.Parse(au.GPSTagging.ToString()) : false;
                    objSectionFieldBO.hasAlert = au.HasFile != null ? bool.Parse(au.HasAlert.ToString()) : false;
                    objSectionFieldBO.hasFile = au.HasFile != null ? bool.Parse(au.HasFile.ToString()) : false;
                    objSectionFieldBO.hasValues = au.HasValues != null ? bool.Parse(au.HasValues.ToString()) : false;
                    objSectionFieldBO.hintText = au.HintText;
                    objSectionFieldBO.isEnabled = au.IsEnabled != null ? bool.Parse(au.IsEnabled.ToString()) : false;
                    objSectionFieldBO.isRequired = au.IsRequired != null ? bool.Parse(au.IsRequired.ToString()) : false;
                    objSectionFieldBO.keyboardType = au.KeyboardType != null ? bool.Parse(au.KeyboardType.ToString()) : false;

                    objSectionFieldBO.maximumHeight = au.MaximumHeight;
                    objSectionFieldBO.maximumWidth = au.MaximumWidth;
                    objSectionFieldBO.maxLengthValue = !string.IsNullOrEmpty(au.MaxLengthValue) ? int.Parse(au.MaxLengthValue) : 250;
                    objSectionFieldBO.minLengthValue = !string.IsNullOrEmpty(au.MinLengthValue) ? int.Parse(au.MinLengthValue) : 250;
                    objSectionFieldBO.numberOfColumnsForPhones = au.NumberOfColumnsForPhones;
                    objSectionFieldBO.numberOfColumnsForTablets = au.NumberOfColumnsForTablets;
                    objSectionFieldBO.photoFile = au.PhotoFile != null ? bool.Parse(au.PhotoFile.ToString()) : false;
                    objSectionFieldBO.photoLibraryEnabled = au.PhotoLibraryEnabled != null ? bool.Parse(au.PhotoLibraryEnabled.ToString()) : false;
                    objSectionFieldBO.photoQuality = au.PhotoQuality;


                    objSectionFieldBO.ratingLabels = au.RatingLabels != null ? bool.Parse(au.RatingLabels.ToString()) : false;
                    objSectionFieldBO.ratingMax = au.RatingMax != null ? bool.Parse(au.RatingMax.ToString()) : false;
                    objSectionFieldBO.sectionId = fieldddID.SectionId;
                    objSectionFieldBO.secure = au.Secure != null ? bool.Parse(au.Secure.ToString()) : false;
                    objSectionFieldBO.showMultiselect = au.ShowMultiselect != null ? bool.Parse(au.ShowMultiselect.ToString()) : false;
                    objSectionFieldBO.showRatingLabels = au.ShowRatingLabels != null ? bool.Parse(au.ShowRatingLabels.ToString()) : false;
                    objSectionFieldBO.maxrecordabletimeInSeconds = au.MaxrecordabletimeInSeconds;
                    objSectionFieldBO.videoQuality = au.VideoQuality;

                    List<FieldInitialData> objFieldInitialDataList = new List<FieldInitialData>();

                    var res = objCompassEntities.tblProjectFieldDDLMasters.Where(o => o.FormSectionFieldID == au.ID).ToList();
                    foreach (var itemres in res)
                    {
                        FieldInitialData objFieldInitialData = new FieldInitialData();
                        objFieldInitialData.initialFieldId = itemres.ID;
                        objFieldInitialData.initialFieldValue = itemres.FieldValue;
                        objFieldInitialDataList.Add(objFieldInitialData);
                    }
                    objSectionFieldBO.fieldInitialDataList = objFieldInitialDataList;
                    objSectionFieldBOList.Add(objSectionFieldBO);
                }

                obj1.FieldList = objSectionFieldBOList;
                objMobileSectionBO.Add(obj1);
            }

            obj.SectionList = objMobileSectionBO;
            return obj;

        }

        public bool saveMobileData(FormData objForm, string _jsonData, out string responseMessage)
        {

            bool isRecrodSaved = false;
            responseMessage = "";



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
                    using (CompassEntities objCompassEntities = new CompassEntities())
                    {



                        bool saveRecord = false;

                        var objFormDetails = objCompassEntities.tblFormMasters.Where(o => o.FormId == objForm.formId).FirstOrDefault();

                        TblProjectFieldData objTblProjectFieldData = objCompassEntities.TblProjectFieldDatas.
                            Where(o => o.tblUploadedData_Id == objForm.currentAccountData.initialFieldId
                            && o.FormId == objForm.formId && o.Active == 1).FirstOrDefault();


                        if (objTblProjectFieldData != null && (objTblProjectFieldData.IsSkipped == null || objTblProjectFieldData.IsSkipped == false))
                        {
                            ///if record is present & not skipped.
                            var objtblProjectFieldDataMaster = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1
                                && o.InitialFieldName == "WORK COMPLETED" && o.TblProjectFieldData_Id == objTblProjectFieldData.ID).FirstOrDefault();
                            if (objtblProjectFieldDataMaster != null && objtblProjectFieldDataMaster.FieldValue == "true")
                            {
                                responseMessage = "Failed to save the record. It is already submitted.";
                            }
                            else if (objtblProjectFieldDataMaster != null && objtblProjectFieldDataMaster.FieldValue == "false")
                            {
                                objTblProjectFieldData.Active = 0;
                                objCompassEntities.SaveChanges();
                                var objtblProjectFieldDataList = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1
                                                              && o.TblProjectFieldData_Id == objTblProjectFieldData.ID).ToList();
                                foreach (var item in objtblProjectFieldDataList)
                                {
                                    item.Active = 0;
                                    objCompassEntities.SaveChanges();

                                    var objAttachmentList = objCompassEntities.TblProjectFieldDataAttachments.Where(o => o.Active == 1
                                                             && o.tblProjectFieldDataMaster_ID == item.ID).ToList();
                                    foreach (var item1 in objAttachmentList)
                                    {
                                        item.Active = 0;
                                        objCompassEntities.SaveChanges();
                                    }
                                }
                                saveRecord = true;
                            }
                        }
                        else
                        {
                            saveRecord = true;
                        }


                        if (saveRecord)
                        {
                            if (objTblProjectFieldData != null && objTblProjectFieldData.IsSkipped == true)
                            {
                                //
                                objTblProjectFieldData.Active = 0;
                                objTblProjectFieldData.ModifiedOn = new CommonFunctions().ServerDate();
                                objCompassEntities.SaveChanges();
                            }

                            objTblProjectFieldData = new TblProjectFieldData();
                            objTblProjectFieldData.Active = 1;
                            objTblProjectFieldData.CreatedOn = new CommonFunctions().ServerDate();
                            objTblProjectFieldData.CreatedBy = int.Parse(objForm.userId.ToString());
                            objTblProjectFieldData.InstallerId = long.Parse(objForm.userId.ToString());
                            objTblProjectFieldData.FormId = objForm.formId;
                            objTblProjectFieldData.tblUploadedData_Id = objForm.currentAccountData.initialFieldId;
                            objTblProjectFieldData.IsSkipped = objForm.currentAccountData.isAccountSkipped;
                            objTblProjectFieldData.IsReAssign = false;
                            if (!string.IsNullOrEmpty(objForm.currentAccountData.skipeCapturedDateTime))
                            {
                                //DateTime _captureTime = UnixTimeStampToDateTime(Convert.ToDouble(objForm.currentAccountData.skipeCapturedDateTime));
                                string[] subStr = objForm.visitDateTime.Split(' ');
                                string[] strDate = subStr[0].ToString().Split('-');
                                string[] strTimeSplit = subStr[1].Split(':');
                                int _year = int.Parse(strDate[2]);
                                int _Mon = DateTime.ParseExact(strDate[0], "MMM", CultureInfo.CurrentCulture).Month;
                                int _Day = int.Parse(strDate[1]);

                                int _hr = subStr[2] == "PM" ? (strTimeSplit[0] == "12" ? int.Parse(strTimeSplit[0]) : (12 + int.Parse(strTimeSplit[0]))) : int.Parse(strTimeSplit[0]);
                                int _MN = int.Parse(strTimeSplit[1]);

                                DateTime _VisitDatetime = new DateTime(_year, _Mon, _Day, _hr, _MN, 0);
                                objTblProjectFieldData.SkippedDatetime = _VisitDatetime;
                                objTblProjectFieldData.SkippedReason = objForm.currentAccountData.skipeReason;
                                objTblProjectFieldData.SkipComment = objForm.currentAccountData.skipComment;

                                if (objForm.currentAccountData.skipId != null)
                                {
                                    objTblProjectFieldData.SkipId = long.Parse(objForm.currentAccountData.skipId.ToString());
                                }

                            }
                            if (!string.IsNullOrEmpty(objForm.visitDateTime))
                            {
                                string[] subStr = objForm.visitDateTime.Split(' ');
                                string[] strDate = subStr[0].ToString().Split('-');
                                string[] strTimeSplit = subStr[1].Split(':');
                                int _year = int.Parse(strDate[2]);
                                int _Mon = DateTime.ParseExact(strDate[0], "MMM", CultureInfo.CurrentCulture).Month;
                                int _Day = int.Parse(strDate[1]);

                                int _hr = subStr[2] == "PM" ? (strTimeSplit[0] == "12" ? int.Parse(strTimeSplit[0]) : (12 + int.Parse(strTimeSplit[0]))) : int.Parse(strTimeSplit[0]);
                                int _MN = int.Parse(strTimeSplit[1]);

                                DateTime _VisitDatetime = new DateTime(_year, _Mon, _Day, _hr, _MN, 0);
                                objTblProjectFieldData.visitdatetime = _VisitDatetime;
                            }
                            else
                            {
                                // objTblProjectFieldData.VisitDatetime = objTblProjectFieldData.CreatedOn;
                            }
                            //

                            objTblProjectFieldData.JsonDataVal = _jsonData;
                            objCompassEntities.TblProjectFieldDatas.Add(objTblProjectFieldData);
                            objCompassEntities.SaveChanges();


                            if (objForm.currentAccountData.isAccountSkipped == false)
                            {

                                string mapFilePath = string.Empty;
                                long masterFieldId = 0;
                                string tempPath1 = string.Empty;
                                foreach (var item in objForm.currentAccountData.accountFieldDataList)
                                {
                                    tblProjectFieldDataMaster objtblProjectFieldDataMaster = new tblProjectFieldDataMaster();
                                    objtblProjectFieldDataMaster.Active = 1;
                                    objtblProjectFieldDataMaster.Comments = item.commentForServiceField;
                                    objtblProjectFieldDataMaster.CreatedOn = new CommonFunctions().ServerDate();
                                    objtblProjectFieldDataMaster.CreatedBy = int.Parse(objForm.userId.ToString());
                                    objtblProjectFieldDataMaster.FieldLatitude = item.geolocationLatitudeValue;
                                    objtblProjectFieldDataMaster.FieldLongitude = item.geolocationLongitudeValue;
                                    objtblProjectFieldDataMaster.FieldStatus = item.statusForServiceField;
                                    if (!string.IsNullOrEmpty(item.capturedDateTime))
                                    {
                                        // DateTime _captureDateTime = UnixTimeStampToDateTime(Convert.ToDouble(item.capturedDateTime));
                                        string[] subStr = item.capturedDateTime.Split(' ');
                                        string[] strDate = subStr[0].ToString().Split('-');
                                        string[] strTimeSplit = subStr[1].Split(':');
                                        int _year = int.Parse(strDate[2]);
                                        int _Mon = DateTime.ParseExact(strDate[0], "MMM", CultureInfo.CurrentCulture).Month;
                                        int _Day = int.Parse(strDate[1]);

                                        int _hr = subStr[2] == "PM" ? (strTimeSplit[0] == "12" ? int.Parse(strTimeSplit[0]) : (12 + int.Parse(strTimeSplit[0]))) : int.Parse(strTimeSplit[0]);
                                        int _MN = int.Parse(strTimeSplit[1]);

                                        DateTime _captureDateTime = new DateTime(_year, _Mon, _Day, _hr, _MN, 0);
                                        objtblProjectFieldDataMaster.CapturedDateTime = _captureDateTime;

                                    }


                                    objtblProjectFieldDataMaster.FieldValue = item.fieldName == "WORK COMPLETED" ? "false" : item.fieldValue;

                                    if (item.fieldName == "LOCATION/GPS")
                                    {
                                        objtblProjectFieldDataMaster.FieldValue = item.geolocationLatitudeValue + ", " + item.geolocationLongitudeValue;

                                        string latlng = item.geolocationLatitudeValue + "," + item.geolocationLongitudeValue;
                                        string urlPath = "https://maps.googleapis.com/maps/api/staticmap?center=" + latlng +
                                           "&zoom=15&size=200x200&maptype=roadmap&markers=color:green%7C" + latlng + "&sensor=false";

                                        string ServerUploadFolder = @"~\MobileUploadData\";
                                        string ServerUploadFolder1 = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"]);
                                        string tempPath = ServerUploadFolder + objForm.projectId + @"\" + objForm.formId + @"\" +
                                        objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "MAP" + @"\";
                                        mapFilePath = System.Web.HttpContext.Current.Request.MapPath(tempPath);
                                        grantAccess(mapFilePath);

                                        mapFilePath = mapFilePath + @"img.png";
                                        tempPath1 = ServerUploadFolder1 + objForm.projectId + @"\" + objForm.formId + @"\" +
                                        objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "MAP" + @"\img.png";
                                        //String Url to Bytes

                                        byte[] bytes = GetBytesFromUrl(urlPath);


                                        //Bytes Saved to Specified File

                                        WriteBytesToFile(mapFilePath, bytes);
                                        //using (System.Net.WebClient wc = new System.Net.WebClient())
                                        //{

                                        //   // wc.Proxy = new System.Net.WebProxy("https://maps.googleapis.com");
                                        //    wc.DownloadFile(urlPath, mapFilePath);
                                        //}
                                    }
                                    objtblProjectFieldDataMaster.InitialFieldId = item.fieldId;
                                    objtblProjectFieldDataMaster.InitialFieldName = item.fieldName;
                                    objtblProjectFieldDataMaster.TblProjectFieldData_Id = objTblProjectFieldData.ID;
                                    objCompassEntities.tblProjectFieldDataMasters.Add(objtblProjectFieldDataMaster);
                                    objCompassEntities.SaveChanges();
                                    if (item.fieldName == "LOCATION/GPS")
                                    {
                                        masterFieldId = objtblProjectFieldDataMaster.ID;
                                    }
                                }



                                if (masterFieldId != 0 && !string.IsNullOrEmpty(mapFilePath))
                                {
                                    TblProjectFieldDataAttachment obj = new TblProjectFieldDataAttachment();
                                    obj.Active = 1;
                                    obj.CreatedOn = new CommonFunctions().ServerDate();
                                    obj.CreatedBy = 1;
                                    obj.tblProjectFieldDataMaster_ID = masterFieldId;
                                    obj.IsAudioVideoImage = 3;
                                    obj.strFilePath = tempPath1;
                                    obj.isServiceData = false;
                                    objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                                    objCompassEntities.SaveChanges();
                                }
                            }

                            var objUploadDetails = objCompassEntities.tblUploadedDatas.
                                Where(o => o.ID == objForm.currentAccountData.initialFieldId).Select(o => o).FirstOrDefault();
                            objUploadDetails.IsVisisted = true;
                            objUploadDetails.VisitedOn = new CommonFunctions().ServerDate();
                            objUploadDetails.VisitedBy = long.Parse(objForm.userId);
                            objCompassEntities.SaveChanges();
                            isRecrodSaved = true;
                            responseMessage = "Record Saved Successfull.";

                        }

                        scope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isRecrodSaved;

        }

        public static bool grantAccess(string _direcotryPath)
        {
            bool isSucessed = false;
            try
            {
                string folderPath = _direcotryPath;
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                DirectoryInfo dInfo = new DirectoryInfo(folderPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule("everyone", FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
                isSucessed = true;
            }

            catch (Exception ex) { throw ex; }
            return isSucessed;
        }


        public bool updateCompletionFlag(long userId, long formId, long initialId)
        {
            try
            {
                bool a = false;

                CompassEntities objCompassEntities = new CompassEntities();

                var getrecord = objCompassEntities.TblProjectFieldDatas.Where(o => o.Active == 1 && o.FormId == formId && o.tblUploadedData_Id == initialId).FirstOrDefault();

                if (getrecord != null)
                {
                    if (getrecord.IsSkipped == true)
                    {
                        a = true;
                    }
                    else
                    {

                        var getfield = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1 && o.TblProjectFieldData_Id == getrecord.ID
                            && o.InitialFieldName == "WORK COMPLETED").FirstOrDefault();
                        if (getfield != null)
                        {
                            getfield.FieldValue = "true";
                            objCompassEntities.SaveChanges();
                            a = true;

                            // getrecord.VisitDatetime = getfield.CapturedDateTime;
                            objCompassEntities.SaveChanges();

                        }
                    }
                }

                return a;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        static readonly double MaxUnixSeconds = (DateTime.MaxValue - UnixEpoch).TotalSeconds;
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            return unixTimeStamp > MaxUnixSeconds
               ? UnixEpoch.AddMilliseconds(unixTimeStamp)
               : UnixEpoch.AddSeconds(unixTimeStamp);
        }

        //public bool saveMobileData1(FormData objForm, Dictionary<long, string> imageFilePaths, out string responseMessage)
        //{

        //    bool isRecrodSaved = false;
        //    responseMessage = "";


        //    try
        //    {
        //        CompassEntities objCompassEntities = new CompassEntities();

        //        var objFormDetails = objCompassEntities.tblFormMasters.Where(o => o.FormId == objForm.formId).FirstOrDefault();

        //        TblProjectFieldData objTblProjectFieldData = new TblProjectFieldData();

        //        objTblProjectFieldData.Active = 1;
        //        objTblProjectFieldData.CreatedOn = new CommonFunctions().ServerDate();
        //        objTblProjectFieldData.CreatedBy = int.Parse(objForm.userId.ToString());
        //        objTblProjectFieldData.InstallerId = long.Parse(objForm.userId.ToString());
        //        objTblProjectFieldData.FormId = objForm.formId;
        //        objTblProjectFieldData.tblUploadedData_Id = objForm.currentAccountData.initialFieldId;

        //        objCompassEntities.TblProjectFieldDatas.Add(objTblProjectFieldData);
        //        objCompassEntities.SaveChanges();


        //        foreach (var item in objForm.currentAccountData.accountFieldDataList)
        //        {
        //            tblProjectFieldDataMaster objtblProjectFieldDataMaster = new tblProjectFieldDataMaster();
        //            objtblProjectFieldDataMaster.Active = 1;
        //            objtblProjectFieldDataMaster.Comments = item.commentForServiceField;
        //            objtblProjectFieldDataMaster.CreatedOn = new CommonFunctions().ServerDate();
        //            objtblProjectFieldDataMaster.CreatedBy = int.Parse(objForm.userId.ToString());
        //            objtblProjectFieldDataMaster.FieldLatitude = item.geolocationLatitudeValue;
        //            objtblProjectFieldDataMaster.FieldLongitude = item.geolocationLongitudeValue;
        //            objtblProjectFieldDataMaster.FieldStatus = item.statusForServiceField;
        //            objtblProjectFieldDataMaster.FieldValue = item.fieldValue;
        //            objtblProjectFieldDataMaster.InitialFieldId = item.fieldId;
        //            objtblProjectFieldDataMaster.InitialFieldName = item.fieldName;
        //            objtblProjectFieldDataMaster.TblProjectFieldData_Id = objTblProjectFieldData.ID;
        //            objCompassEntities.tblProjectFieldDataMasters.Add(objtblProjectFieldDataMaster);
        //            objCompassEntities.SaveChanges();



        //            foreach (var item1 in item.capturedVideos)
        //            {
        //                TblProjectFieldDataAttachment objTblProjectFieldDataAttachment = new TblProjectFieldDataAttachment();
        //                objTblProjectFieldDataAttachment.Active = 1;
        //                objTblProjectFieldDataAttachment.CreatedBy = int.Parse(objForm.userId.ToString());
        //                objTblProjectFieldDataAttachment.CreatedOn = new CommonFunctions().ServerDate();
        //                objTblProjectFieldDataAttachment.tblProjectFieldDataMaster_ID = objtblProjectFieldDataMaster.ID;
        //                objTblProjectFieldDataAttachment.IsAudioVideoImage = 2;
        //                objTblProjectFieldDataAttachment.IsSkipped = false;
        //                //objTblProjectFieldDataAttachment.strFilePath = getVideoPaths(objFormDetails.ProjectId.ToString(), objFormDetails.FormId.ToString(),
        //                //objForm.currentAccountData.accountNumber, objForm.currentAccountData.initialFieldId.ToString(), item1);

        //                objCompassEntities.TblProjectFieldDataAttachments.Add(objTblProjectFieldDataAttachment);
        //                objCompassEntities.SaveChanges();
        //            }


        //            foreach (var item1 in item.capturedAudios)
        //            {
        //                TblProjectFieldDataAttachment objTblProjectFieldDataAttachment = new TblProjectFieldDataAttachment();
        //                objTblProjectFieldDataAttachment.Active = 1;
        //                objTblProjectFieldDataAttachment.CreatedBy = int.Parse(objForm.userId.ToString());
        //                objTblProjectFieldDataAttachment.CreatedOn = new CommonFunctions().ServerDate();
        //                objTblProjectFieldDataAttachment.tblProjectFieldDataMaster_ID = objtblProjectFieldDataMaster.ID;
        //                objTblProjectFieldDataAttachment.IsAudioVideoImage = 1;
        //                objTblProjectFieldDataAttachment.IsSkipped = false;
        //                //objTblProjectFieldDataAttachment.strFilePath = getAudioPaths(objFormDetails.ProjectId.ToString(), objFormDetails.FormId.ToString(),
        //                //objForm.currentAccountData.accountNumber, objForm.currentAccountData.initialFieldId.ToString(), item1);

        //                objCompassEntities.TblProjectFieldDataAttachments.Add(objTblProjectFieldDataAttachment);
        //                objCompassEntities.SaveChanges();
        //            }


        //            var resImageList = imageFilePaths.Where(o => o.Key == item.fieldId);
        //            foreach (var item1 in resImageList)
        //            {

        //                TblProjectFieldDataAttachment objTblProjectFieldDataAttachment = new TblProjectFieldDataAttachment();
        //                objTblProjectFieldDataAttachment.Active = 1;
        //                objTblProjectFieldDataAttachment.CreatedBy = int.Parse(objForm.userId.ToString());
        //                objTblProjectFieldDataAttachment.CreatedOn = new CommonFunctions().ServerDate();
        //                objTblProjectFieldDataAttachment.tblProjectFieldDataMaster_ID = objtblProjectFieldDataMaster.ID;
        //                objTblProjectFieldDataAttachment.IsAudioVideoImage = 3;
        //                objTblProjectFieldDataAttachment.IsSkipped = false;
        //                objTblProjectFieldDataAttachment.strFilePath = item1.Value;//getImagePaths(objFormDetails.ProjectId.ToString(), objFormDetails.FormId.ToString(),
        //                //objForm.currentAccountData.accountNumber, objForm.currentAccountData.initialFieldId.ToString(), item1);

        //                objCompassEntities.TblProjectFieldDataAttachments.Add(objTblProjectFieldDataAttachment);
        //                objCompassEntities.SaveChanges();
        //            }

        //            isRecrodSaved = true;
        //            responseMessage = "Record Saved Successfull.";
        //        }



        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }


        //    return isRecrodSaved;


        //}

        //public FormData testing()
        //{
        //    FormData obj = new FormData();

        //    CompassEntities objCompassEntities = new CompassEntities();
        //    // a=objCompassEntities

        //    obj.formId = 30;
        //    obj.projectId = 14;
        //    obj.userId = "16";
        //    AccountData obj1 = new AccountData();
        //    obj1.accountNumber = "14-5278-00";
        //    obj1.initialFieldId = 3881;
        //    obj1.isAccountSkipped = false;
        //    obj1.skipeReason = null;

        //    List<fieldData> obj2FieldDataList = new List<fieldData>();

        //    for (int i = 0; i < 10; i++)
        //    {
        //        fieldData obj3 = new fieldData();
        //        obj3.capturedDateTime = "1457437753";
        //        obj3.fieldId = i;
        //        obj3.fieldValue = "10";
        //        obj3.geolocationLatitudeValue = "345345";
        //        obj3.geolocationLongitudeValue = "345345";

        //        obj2FieldDataList.Add(obj3);
        //    }

        //    obj1.accountFieldDataList = obj2FieldDataList;
        //    obj.currentAccountData = obj1;
        //    return obj;
        //}



        static public byte[] GetBytesFromUrl(string url)
        {

            byte[] b = { };
            try
            {


                System.Net.HttpWebRequest myReq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);

                System.Net.WebResponse myResp = myReq.GetResponse();


                Stream stream = myResp.GetResponseStream();


                using (BinaryReader br = new BinaryReader(stream))
                {

                    //i = (int)(stream.Length);

                    b = br.ReadBytes(500000);

                    br.Close();

                }

                myResp.Close();


            }
            catch (Exception ex)
            {


            }
            return b;

        }


        //Writing the bytes in to a Specified File to Save

        static public void WriteBytesToFile(string fileName, byte[] content)
        {
            if (content != null && content.Length != 0)
            {
                FileStream fs = new FileStream(fileName, FileMode.Create);
                BinaryWriter w = new BinaryWriter(fs);
                try
                {
                    w.Write(content);
                }
                finally
                {
                    fs.Close();
                    w.Close();
                }
            }
        }


        /// <summary>
        /// Date 2016-10-06 Devloper Bharat Magdum
        /// method to save information submitted from mobile app.
        /// </summary>
        /// <param name="objForm"></param>
        /// <param name="_jsonData"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public bool saveData(FormDataNew objForm, string _jsonData, out string responseMessage)
        {

            bool isRecrodSaved = false;
            responseMessage = "";



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
                    using (CompassEntities objCompassEntities = new CompassEntities())
                    {
                        bool saveRecord = false;

                        var objFormDetails = objCompassEntities.tblFormMasters.Where(o => o.FormId == objForm.formId && o.Active == 1).FirstOrDefault();

                        TblProjectFieldData objTblProjectFieldData = objCompassEntities.TblProjectFieldDatas.
                            Where(o => o.tblUploadedData_Id == objForm.currentAccountData.initialFieldId
                            && o.FormId == objForm.formId && o.Active == 1).FirstOrDefault();


                        if (objTblProjectFieldData != null && (objTblProjectFieldData.IsSkipped == null || objTblProjectFieldData.IsSkipped == false))
                        {
                            ///if record is present & not skipped.
                            var objtblProjectFieldDataMaster = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1
                                && o.InitialFieldName == "WORK COMPLETED" && o.TblProjectFieldData_Id == objTblProjectFieldData.ID).FirstOrDefault();
                            if (objtblProjectFieldDataMaster != null && objtblProjectFieldDataMaster.FieldValue == "true")
                            {
                                responseMessage = "Failed to save the record. It is already submitted.";
                            }
                            else if (objtblProjectFieldDataMaster != null && objtblProjectFieldDataMaster.FieldValue == "false")
                            {

                                objTblProjectFieldData.Active = 0;
                                objCompassEntities.SaveChanges();


                                var objtblProjectFieldDataList = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1
                                                              && o.TblProjectFieldData_Id == objTblProjectFieldData.ID).ToList();
                                foreach (var item in objtblProjectFieldDataList)
                                {
                                    item.Active = 0;
                                    objCompassEntities.SaveChanges();

                                    var objAttachmentList = objCompassEntities.TblProjectFieldDataAttachments.Where(o => o.Active == 1
                                                             && o.tblProjectFieldDataMaster_ID == item.ID).ToList();
                                    foreach (var item1 in objAttachmentList)
                                    {
                                        item.Active = 0;
                                        objCompassEntities.SaveChanges();
                                    }
                                }
                                saveRecord = true;
                                #region history inserted
                                TblProjectFieldDataHistory objTblProjectFieldDataHistory = new TblProjectFieldDataHistory();
                                objTblProjectFieldDataHistory.Active = 1;
                                objTblProjectFieldDataHistory.CreatedBy = 1;
                                objTblProjectFieldDataHistory.CreatedOn = new CommonFunctions().ServerDate();
                                objTblProjectFieldDataHistory.FormId = objTblProjectFieldData.FormId;
                                objTblProjectFieldDataHistory.InstallerId = objTblProjectFieldData.InstallerId;
                                objTblProjectFieldDataHistory.InstallerMapId = objTblProjectFieldData.InstallerId;
                                objTblProjectFieldDataHistory.IsReAssign = objTblProjectFieldData.IsReAssign;
                                objTblProjectFieldDataHistory.IsSkipped = objTblProjectFieldData.IsSkipped;
                                objTblProjectFieldDataHistory.JsonDataVal = objTblProjectFieldData.JsonDataVal;
                                objTblProjectFieldDataHistory.SkipComment = objTblProjectFieldData.SkipComment;
                                objTblProjectFieldDataHistory.SkipId = objTblProjectFieldData.SkipId;
                                objTblProjectFieldDataHistory.SkippedDatetime = objTblProjectFieldData.SkippedDatetime;
                                objTblProjectFieldDataHistory.SkippedReason = objTblProjectFieldData.SkippedReason;
                                objTblProjectFieldDataHistory.tblUploadedData_Id = objTblProjectFieldData.tblUploadedData_Id;
                                objTblProjectFieldDataHistory.visitdatetime = objTblProjectFieldData.visitdatetime;

                                objCompassEntities.TblProjectFieldDataHistories.Add(objTblProjectFieldDataHistory);
                                objCompassEntities.SaveChanges();
                                #endregion

                                //tblProjectFieldDataMasterHistory
                            }
                        }
                        else
                        {
                            saveRecord = true;
                        }


                        if (saveRecord)
                        {
                            if (objTblProjectFieldData != null && objTblProjectFieldData.IsSkipped == true)
                            {
                                //
                                objTblProjectFieldData.Active = 0;
                                objTblProjectFieldData.ModifiedOn = new CommonFunctions().ServerDate();
                                objCompassEntities.SaveChanges();
                                #region history inserted
                                TblProjectFieldDataHistory objTblProjectFieldDataHistory = new TblProjectFieldDataHistory();
                                objTblProjectFieldDataHistory.Active = 1;
                                objTblProjectFieldDataHistory.CreatedBy = 1;
                                objTblProjectFieldDataHistory.CreatedOn = new CommonFunctions().ServerDate();
                                objTblProjectFieldDataHistory.FormId = objTblProjectFieldData.FormId;
                                objTblProjectFieldDataHistory.InstallerId = objTblProjectFieldData.InstallerId;
                                objTblProjectFieldDataHistory.InstallerMapId = objTblProjectFieldData.InstallerId;
                                objTblProjectFieldDataHistory.IsReAssign = objTblProjectFieldData.IsReAssign;
                                objTblProjectFieldDataHistory.IsSkipped = objTblProjectFieldData.IsSkipped;
                                objTblProjectFieldDataHistory.JsonDataVal = objTblProjectFieldData.JsonDataVal;
                                objTblProjectFieldDataHistory.SkipComment = objTblProjectFieldData.SkipComment;
                                objTblProjectFieldDataHistory.SkipId = objTblProjectFieldData.SkipId;
                                objTblProjectFieldDataHistory.SkippedDatetime = objTblProjectFieldData.SkippedDatetime;
                                objTblProjectFieldDataHistory.SkippedReason = objTblProjectFieldData.SkippedReason;
                                objTblProjectFieldDataHistory.tblUploadedData_Id = objTblProjectFieldData.tblUploadedData_Id;
                                objTblProjectFieldDataHistory.visitdatetime = objTblProjectFieldData.visitdatetime;

                                objCompassEntities.TblProjectFieldDataHistories.Add(objTblProjectFieldDataHistory);
                                objCompassEntities.SaveChanges();
                                #endregion

                            }

                            objTblProjectFieldData = new TblProjectFieldData();
                            objTblProjectFieldData.Active = 1;
                            objTblProjectFieldData.CreatedOn = new CommonFunctions().ServerDate();
                            objTblProjectFieldData.CreatedBy = int.Parse(objForm.userId.ToString());
                            objTblProjectFieldData.InstallerId = long.Parse(objForm.userId.ToString());
                            objTblProjectFieldData.FormId = objForm.formId;
                            objTblProjectFieldData.tblUploadedData_Id = objForm.currentAccountData.initialFieldId;
                            objTblProjectFieldData.IsSkipped = objForm.currentAccountData.isAccountSkipped;
                            objTblProjectFieldData.IsReAssign = false;
                            objTblProjectFieldData.IsMediaPending = objForm.isMediaPending;
                            ;
                            if (!string.IsNullOrEmpty(objForm.currentAccountData.skipeCapturedDateTime))
                            {
                                //DateTime _captureTime = UnixTimeStampToDateTime(Convert.ToDouble(objForm.currentAccountData.skipeCapturedDateTime));
                                string[] subStr = objForm.visitDateTime.Split(' ');
                                string[] strDate = subStr[0].ToString().Split('-');
                                string[] strTimeSplit = subStr[1].Split(':');
                                int _year = int.Parse(strDate[2]);
                                int _Mon = DateTime.ParseExact(strDate[0], "MMM", CultureInfo.CurrentCulture).Month;
                                int _Day = int.Parse(strDate[1]);

                                int _hr = subStr[2] == "PM" ? (strTimeSplit[0] == "12" ? int.Parse(strTimeSplit[0]) : (12 + int.Parse(strTimeSplit[0]))) : int.Parse(strTimeSplit[0]);
                                int _MN = int.Parse(strTimeSplit[1]);

                                DateTime _VisitDatetime = new DateTime(_year, _Mon, _Day, _hr, _MN, 0);
                                objTblProjectFieldData.SkippedDatetime = _VisitDatetime;
                                objTblProjectFieldData.SkippedReason = objForm.currentAccountData.skipeReason;
                                objTblProjectFieldData.SkipComment = objForm.currentAccountData.skipComment;

                                if (objForm.currentAccountData.skipId != null)
                                {
                                    objTblProjectFieldData.SkipId = long.Parse(objForm.currentAccountData.skipId.ToString());
                                }

                            }
                            if (!string.IsNullOrEmpty(objForm.visitDateTime))
                            {
                                string[] subStr = objForm.visitDateTime.Split(' ');
                                string[] strDate = subStr[0].ToString().Split('-');
                                string[] strTimeSplit = subStr[1].Split(':');
                                int _year = int.Parse(strDate[2]);
                                int _Mon = DateTime.ParseExact(strDate[0], "MMM", CultureInfo.CurrentCulture).Month;
                                int _Day = int.Parse(strDate[1]);

                                int _hr = subStr[2] == "PM" ? (strTimeSplit[0] == "12" ? int.Parse(strTimeSplit[0]) : (12 + int.Parse(strTimeSplit[0]))) : int.Parse(strTimeSplit[0]);
                                int _MN = int.Parse(strTimeSplit[1]);

                                DateTime _VisitDatetime = new DateTime(_year, _Mon, _Day, _hr, _MN, 0);
                                objTblProjectFieldData.visitdatetime = _VisitDatetime;
                            }
                            else
                            {
                                // objTblProjectFieldData.VisitDatetime = objTblProjectFieldData.CreatedOn;
                            }


                            objTblProjectFieldData.JsonDataVal = _jsonData;
                            objCompassEntities.TblProjectFieldDatas.Add(objTblProjectFieldData);
                            objCompassEntities.SaveChanges();


                            if (objForm.currentAccountData.isAccountSkipped == false)
                            {

                                string mapFilePath = string.Empty;
                                long masterFieldId = 0;
                                string tempPath1 = string.Empty;
                                foreach (var item in objForm.currentAccountData.accountFieldDataList)
                                {
                                    tblProjectFieldDataMaster objtblProjectFieldDataMaster = new tblProjectFieldDataMaster();
                                    objtblProjectFieldDataMaster.Active = 1;
                                    objtblProjectFieldDataMaster.Comments = item.commentForServiceField;
                                    objtblProjectFieldDataMaster.CreatedOn = new CommonFunctions().ServerDate();
                                    objtblProjectFieldDataMaster.CreatedBy = int.Parse(objForm.userId.ToString());
                                    objtblProjectFieldDataMaster.FieldLatitude = item.geolocationLatitudeValue;
                                    objtblProjectFieldDataMaster.FieldLongitude = item.geolocationLongitudeValue;
                                    objtblProjectFieldDataMaster.FieldStatus = item.statusForServiceField;
                                    if (!string.IsNullOrEmpty(item.capturedDateTime))
                                    {
                                        // DateTime _captureDateTime = UnixTimeStampToDateTime(Convert.ToDouble(item.capturedDateTime));
                                        string[] subStr = item.capturedDateTime.Split(' ');
                                        string[] strDate = subStr[0].ToString().Split('-');
                                        string[] strTimeSplit = subStr[1].Split(':');
                                        int _year = int.Parse(strDate[2]);
                                        int _Mon = DateTime.ParseExact(strDate[0], "MMM", CultureInfo.CurrentCulture).Month;
                                        int _Day = int.Parse(strDate[1]);

                                        int _hr = subStr[2] == "PM" ? (strTimeSplit[0] == "12" ? int.Parse(strTimeSplit[0]) : (12 + int.Parse(strTimeSplit[0]))) : int.Parse(strTimeSplit[0]);
                                        int _MN = int.Parse(strTimeSplit[1]);

                                        DateTime _captureDateTime = new DateTime(_year, _Mon, _Day, _hr, _MN, 0);
                                        objtblProjectFieldDataMaster.CapturedDateTime = _captureDateTime;

                                    }


                                    //  objtblProjectFieldDataMaster.FieldValue = item.fieldName == "WORK COMPLETED" ? "false" : item.fieldValue;
                                    objtblProjectFieldDataMaster.FieldValue = item.fieldValue;

                                    if (item.fieldName == "LOCATION/GPS")
                                    {
                                        objtblProjectFieldDataMaster.FieldValue = item.geolocationLatitudeValue + ", " + item.geolocationLongitudeValue;

                                        string latlng = item.geolocationLatitudeValue + "," + item.geolocationLongitudeValue;
                                        string urlPath = "https://maps.googleapis.com/maps/api/staticmap?center=" + latlng +
                                           "&zoom=15&size=200x200&maptype=roadmap&markers=color:green%7C" + latlng + "&sensor=false";

                                        string ServerUploadFolder = @"~\MobileUploadData\";
                                        string ServerUploadFolder1 = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"]);
                                        string tempPath = ServerUploadFolder + objForm.projectId + @"\" + objForm.formId + @"\" +
                                        objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "MAP" + @"\";
                                        mapFilePath = System.Web.HttpContext.Current.Request.MapPath(tempPath);
                                        grantAccess(mapFilePath);

                                        mapFilePath = mapFilePath + @"img.png";
                                        tempPath1 = ServerUploadFolder1 + objForm.projectId + @"\" + objForm.formId + @"\" +
                                        objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "MAP" + @"\img.png";

                                        //String Url to Bytes
                                        byte[] bytes = GetBytesFromUrl(urlPath);

                                        //Bytes Saved to Specified File
                                        WriteBytesToFile(mapFilePath, bytes);

                                    }
                                    objtblProjectFieldDataMaster.InitialFieldId = item.fieldId;
                                    objtblProjectFieldDataMaster.InitialFieldName = item.fieldName.Trim();
                                    objtblProjectFieldDataMaster.TblProjectFieldData_Id = objTblProjectFieldData.ID;
                                    objCompassEntities.tblProjectFieldDataMasters.Add(objtblProjectFieldDataMaster);
                                    objCompassEntities.SaveChanges();
                                    if (item.fieldName == "LOCATION/GPS")
                                    {
                                        masterFieldId = objtblProjectFieldDataMaster.ID;
                                    }
                                }



                                if (masterFieldId != 0 && !string.IsNullOrEmpty(mapFilePath))
                                {
                                    TblProjectFieldDataAttachment obj = new TblProjectFieldDataAttachment();
                                    obj.Active = 1;
                                    obj.CreatedOn = new CommonFunctions().ServerDate();
                                    obj.CreatedBy = 1;
                                    obj.tblProjectFieldDataMaster_ID = masterFieldId;
                                    obj.IsAudioVideoImage = 3;
                                    obj.strFilePath = tempPath1;
                                    obj.isServiceData = false;
                                    objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                                    objCompassEntities.SaveChanges();
                                }
                            }


                            isRecrodSaved = true;
                            responseMessage = "Record Saved Successfull.";

                        }

                        scope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isRecrodSaved;

        }


        public bool saveDynamicData(FormDataNew objForm, string _jsonData, out string responseMessage, string googleKey)
        {

            bool isRecrodSaved = false;
            responseMessage = "";

            long userId = long.Parse(objForm.userId.ToString());

            try
            {
                // use the scope we just defined

                CompassEntities objCompassEntities1 = new CompassEntities();
                var objFormDetails = objCompassEntities1.tblFormMasters.Where(o => o.FormId == objForm.formId).FirstOrDefault();
                var objProjectDetails = objCompassEntities1.tblProjects.Where(o => o.ProjectId == objFormDetails.ProjectId).FirstOrDefault();
                if (objFormDetails.FormTypeID == 2)
                {

                    TransactionOptions options = new TransactionOptions();
                    options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                    options.Timeout = new TimeSpan(0, 15, 0);


                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, options))
                    {
                        using (CompassEntities objCompassEntities = new CompassEntities())
                        {

                            string newMeterNo = "";
                            string newMeterRadioNo = "";
                            long projectFieldDataId = 0;

                            bool saveRecord = false;

                            #region installation form

                            objFormDetails = objCompassEntities.tblFormMasters.Where(o => o.FormId == objForm.formId).FirstOrDefault();

                            var workCompleteFlag = objCompassEntities.PROC_WebService_CheckRecordStatus_WorkCompleted(objForm.currentAccountData.initialFieldId, objForm.formId).FirstOrDefault();
                            var skipOrRtuFlag = objCompassEntities.PROC_WebService_CheckRecordStatus_SkipOrRtu(objForm.currentAccountData.initialFieldId, objForm.formId).FirstOrDefault();

                            var ServiceProductList = objCompassEntities.PROC_ProjectServiceProductlist(objFormDetails.ProjectId).ToList();

                            if (workCompleteFlag != null || skipOrRtuFlag != null)
                            {
                                if (workCompleteFlag != null)
                                {
                                    responseMessage = "Failed to save the record. It is already submitted.";
                                }
                                else if (skipOrRtuFlag != null)
                                {

                                    long objInstallerId = long.Parse(objForm.userId.ToString());
                                    var oldTblProjectFieldData = objCompassEntities.TblProjectFieldDatas.Where(o => o.FK_UploadedExcelData_Id == objForm.currentAccountData.initialFieldId 
                                                                 && o.InstallerId == objInstallerId
                                                                 && o.SkipId!=null && o.Active == 1).OrderByDescending(o => o.ID).FirstOrDefault();
                                    if (oldTblProjectFieldData != null && objForm.currentAccountData.isAccountSkipped == true)
                                    {
                                        //Already data present in Projectfielddata table only for update do not insert again
                                        //oldTblProjectFieldData.Active = 1;
                                        //oldTblProjectFieldData.CreatedOn = new CommonFunctions().ServerDate();
                                        //oldTblProjectFieldData.CreatedBy = int.Parse(objForm.userId.ToString());
                                        oldTblProjectFieldData.ModifiedOn = new CommonFunctions().ServerDate();
                                        oldTblProjectFieldData.ModifiedBy = int.Parse(objForm.userId.ToString());
                                        oldTblProjectFieldData.InstallerId = long.Parse(objForm.userId.ToString());
                                        oldTblProjectFieldData.FormId = objForm.formId;
                                        oldTblProjectFieldData.FK_UploadedExcelData_Id = objForm.currentAccountData.initialFieldId;
                                        oldTblProjectFieldData.IsSkipped = objForm.currentAccountData.isAccountSkipped;
                                        oldTblProjectFieldData.IsReAssign = false;
                                        oldTblProjectFieldData.IsMediaPending = objForm.isMediaPending;

                                        if (!string.IsNullOrEmpty(objForm.currentAccountData.skipeCapturedDateTime))
                                        {
                                            oldTblProjectFieldData.SkippedDatetime = getDate(objForm.visitDateTime);
                                            oldTblProjectFieldData.SkippedReason = objForm.currentAccountData.skipeReason;
                                            oldTblProjectFieldData.SkipComment = objForm.currentAccountData.skipComment;
                                            oldTblProjectFieldData.SkipGPSLocation = objForm.currentAccountData.skipGPSLocation;
                                            oldTblProjectFieldData.SkipId = objForm.currentAccountData.skipId != null ? long.Parse(objForm.currentAccountData.skipId.ToString()) : 0;
                                        }
                                        if (!string.IsNullOrEmpty(objForm.visitDateTime))
                                        {
                                            oldTblProjectFieldData.visitdatetime = getDate(objForm.visitDateTime); ;
                                        }
                                        oldTblProjectFieldData.JsonDataVal = _jsonData;
                                        objCompassEntities.SaveChanges();
                                        saveRecord = false;

                                        if (objForm.currentAccountData.skipId != null && objForm.currentAccountData.skipId > 0)
                                        {
                                            // Old imges/Audio/Video Inactive save
                                            CompassEntities CE = new CompassEntities();
                                            var oldTblProjectFieldDataAttachment = CE.TblProjectFieldDataAttachments.Where(o => o.ProjectFieldDataId == oldTblProjectFieldData.ID && o.Active == 1).ToList();
                                            foreach (var item in oldTblProjectFieldDataAttachment)
                                            {
                                                item.Active = 0;
                                                item.ModifiedBy = int.Parse(objForm.userId.ToString());
                                                item.ModifiedOn = new CommonFunctions().ServerDate();
                                                CE.SaveChanges();
                                            }
                                            //end hare

                                            //Save New files
                                            string latlng = objForm.currentAccountData.skipGPSLocation;
                                            string urlPath = "https://maps.googleapis.com/maps/api/staticmap?center=" + latlng +
                                               "&zoom=15&size=200x200&maptype=roadmap&markers=color:green%7C" + latlng + "&sensor=false&key=" + googleKey;

                                            string ServerUploadFolder = @"~\MobileUploadData\";
                                            string ServerUploadFolder1 = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"]);
                                            string tempPath = ServerUploadFolder + objForm.projectId + @"\" + objForm.formId + @"\" +
                                            objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "MAP" + @"\";
                                            string mapFilePath = System.Web.HttpContext.Current.Request.MapPath(tempPath);
                                            grantAccess(mapFilePath);

                                            mapFilePath = mapFilePath + @"img.png";
                                            string tempPath1 = ServerUploadFolder1 + objForm.projectId + @"\" + objForm.formId + @"\" +
                                            objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "MAP" + @"\img.png";

                                            //String Url to Bytes
                                            byte[] bytes = GetBytesFromUrl(urlPath);

                                            //Bytes Saved to Specified File
                                            WriteBytesToFile(mapFilePath, bytes);

                                            TblProjectFieldDataAttachment obj = new TblProjectFieldDataAttachment();
                                            obj.Active = 1;
                                            obj.CreatedOn = new CommonFunctions().ServerDate();
                                            obj.CreatedBy = int.Parse(objForm.userId.ToString());
                                            obj.ProjectFieldDataId = oldTblProjectFieldData.ID;
                                            obj.IsAudioVideoImage = 3;
                                            obj.strFilePath = tempPath1;
                                            obj.IsSkipped = true;
                                            obj.isServiceData = false;
                                            objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                                            objCompassEntities.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        long recordId = long.Parse(skipOrRtuFlag.ToString());
                                        TblProjectFieldData objTblProjectFieldData = objCompassEntities.TblProjectFieldDatas.Where(o => o.ID == recordId).FirstOrDefault();
                                        objTblProjectFieldData.Active = 0;
                                        objTblProjectFieldData.ModifiedOn = new CommonFunctions().ServerDate();
                                        objCompassEntities.SaveChanges();

                                        #region history inserted
                                        TblProjectFieldDataHistory objTblProjectFieldDataHistory = new TblProjectFieldDataHistory();
                                        objTblProjectFieldDataHistory.Active = 1;
                                        objTblProjectFieldDataHistory.CreatedBy = int.Parse(objForm.userId.ToString());
                                        objTblProjectFieldDataHistory.CreatedOn = new CommonFunctions().ServerDate();
                                        objTblProjectFieldDataHistory.FormId = objTblProjectFieldData.FormId;
                                        objTblProjectFieldDataHistory.InstallerId = objTblProjectFieldData.InstallerId;
                                        objTblProjectFieldDataHistory.InstallerMapId = objTblProjectFieldData.InstallerId;
                                        objTblProjectFieldDataHistory.IsReAssign = objTblProjectFieldData.IsReAssign;
                                        objTblProjectFieldDataHistory.IsSkipped = objTblProjectFieldData.IsSkipped;
                                        objTblProjectFieldDataHistory.JsonDataVal = objTblProjectFieldData.JsonDataVal;
                                        objTblProjectFieldDataHistory.SkipComment = objTblProjectFieldData.SkipComment;
                                        objTblProjectFieldDataHistory.SkipId = objTblProjectFieldData.SkipId;
                                        objTblProjectFieldDataHistory.SkippedDatetime = objTblProjectFieldData.SkippedDatetime;
                                        objTblProjectFieldDataHistory.SkippedReason = objTblProjectFieldData.SkippedReason;
                                        // objTblProjectFieldDataHistory.tblUploadedData_Id = objTblProjectFieldData.tblUploadedData_Id;
                                        objTblProjectFieldDataHistory.FK_UploadedExcelData_Id = objTblProjectFieldData.FK_UploadedExcelData_Id;
                                        objTblProjectFieldDataHistory.visitdatetime = objTblProjectFieldData.visitdatetime;

                                        objCompassEntities.TblProjectFieldDataHistories.Add(objTblProjectFieldDataHistory);
                                        objCompassEntities.SaveChanges();
                                        #endregion

                                        saveRecord = true;
                                    }
                                }
                            }
                            else
                            {
                                saveRecord = true;
                            }


                            if (saveRecord)
                            {
                                #region

                                TblProjectFieldData objTblProjectFieldData = new TblProjectFieldData();
                                objTblProjectFieldData.Active = 1;
                                objTblProjectFieldData.CreatedOn = new CommonFunctions().ServerDate();
                                objTblProjectFieldData.CreatedBy = int.Parse(objForm.userId.ToString());
                                objTblProjectFieldData.InstallerId = long.Parse(objForm.userId.ToString());
                                objTblProjectFieldData.FormId = objForm.formId;
                                //objTblProjectFieldData.tblUploadedData_Id = objForm.currentAccountData.initialFieldId;
                                objTblProjectFieldData.FK_UploadedExcelData_Id = objForm.currentAccountData.initialFieldId;
                                objTblProjectFieldData.IsSkipped = objForm.currentAccountData.isAccountSkipped;
                                objTblProjectFieldData.IsReAssign = false;

                                objTblProjectFieldData.IsMediaPending = objForm.isMediaPending;

                                if (!string.IsNullOrEmpty(objForm.currentAccountData.skipeCapturedDateTime))
                                {
                                    objTblProjectFieldData.SkippedDatetime = getDate(objForm.visitDateTime);
                                    objTblProjectFieldData.SkippedReason = objForm.currentAccountData.skipeReason;
                                    objTblProjectFieldData.SkipComment = objForm.currentAccountData.skipComment;
                                    objTblProjectFieldData.SkipGPSLocation = objForm.currentAccountData.skipGPSLocation;
                                    objTblProjectFieldData.SkipId = objForm.currentAccountData.skipId != null ? long.Parse(objForm.currentAccountData.skipId.ToString()) : 0;
                                }

                                if (!string.IsNullOrEmpty(objForm.visitDateTime))
                                {
                                    objTblProjectFieldData.visitdatetime = getDate(objForm.visitDateTime); ;
                                }
                                //else
                                //{
                                //    // objTblProjectFieldData.VisitDatetime = objTblProjectFieldData.CreatedOn;
                                //}


                                objTblProjectFieldData.JsonDataVal = _jsonData;
                                objCompassEntities.TblProjectFieldDatas.Add(objTblProjectFieldData);
                                objCompassEntities.SaveChanges();

                                if (objForm.currentAccountData.skipId != null && objForm.currentAccountData.skipId > 0)
                                {

                                    string latlng = objForm.currentAccountData.skipGPSLocation;
                                    string urlPath = "https://maps.googleapis.com/maps/api/staticmap?center=" + latlng +
                                       "&zoom=15&size=200x200&maptype=roadmap&markers=color:green%7C" + latlng + "&sensor=false&key=" + googleKey;

                                    string ServerUploadFolder = @"~\MobileUploadData\";
                                    string ServerUploadFolder1 = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"]);
                                    string tempPath = ServerUploadFolder + objForm.projectId + @"\" + objForm.formId + @"\" +
                                    objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "MAP" + @"\";
                                    string mapFilePath = System.Web.HttpContext.Current.Request.MapPath(tempPath);
                                    grantAccess(mapFilePath);

                                    mapFilePath = mapFilePath + @"img.png";
                                    string tempPath1 = ServerUploadFolder1 + objForm.projectId + @"\" + objForm.formId + @"\" +
                                    objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "MAP" + @"\img.png";

                                    //String Url to Bytes
                                    byte[] bytes = GetBytesFromUrl(urlPath);

                                    //Bytes Saved to Specified File
                                    WriteBytesToFile(mapFilePath, bytes);

                                    TblProjectFieldDataAttachment obj = new TblProjectFieldDataAttachment();
                                    obj.Active = 1;
                                    obj.CreatedOn = new CommonFunctions().ServerDate();
                                    obj.CreatedBy = int.Parse(objForm.userId.ToString());
                                    obj.ProjectFieldDataId = objTblProjectFieldData.ID;
                                    obj.IsAudioVideoImage = 3;
                                    obj.strFilePath = tempPath1;
                                    obj.IsSkipped = true;
                                    obj.isServiceData = false;
                                    objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                                    objCompassEntities.SaveChanges();
                                }


                                if (objForm.currentAccountData.isAccountSkipped == false)
                                {

                                    string mapFilePath = string.Empty;
                                    string CommonMapFilePath = string.Empty;
                                    long masterFieldId = 0;
                                    string tempPath1 = string.Empty;
                                    string CommonTempPath = string.Empty;

                                    var objGeoCaptureList = GetGeoCaptureFields(objForm.formId);

                                    string urlPath1 = "https://maps.googleapis.com/maps/api/staticmap?center=";
                                    if (objGeoCaptureList != null && objGeoCaptureList.Count > 0)
                                    {
                                        #region create gps location tagged image for report.

                                        int i = 1;
                                        foreach (var item in objGeoCaptureList)
                                        {
                                            var a = objForm.currentAccountData.accountFieldDataList.Where(o => o.fieldName == item.FieldName).FirstOrDefault();

                                            if (a != null && !string.IsNullOrEmpty(a.geolocationLatitudeValue) && !string.IsNullOrEmpty(a.geolocationLongitudeValue))
                                            {
                                                item.strLatLong = a.geolocationLatitudeValue + "," + a.geolocationLongitudeValue;

                                                if (i == 1)
                                                {
                                                    urlPath1 = urlPath1 + item.strLatLong + "&zoom=20&size=600x400&maptype=roadmap&";
                                                }

                                                urlPath1 = urlPath1 + "markers=color:blue%7Clabel:" + item.Id.ToString() + "%7C" + item.strLatLong + "&";
                                            }
                                        }


                                        urlPath1 = urlPath1 + "sensor=false&key=" + googleKey;


                                        string ServerUploadFolder = @"~\MobileUploadData\";
                                        string ServerUploadFolder1 = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"]);
                                        string tempPath = ServerUploadFolder + objForm.projectId + @"\" + objForm.formId + @"\" +
                                        objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "CommonMAP" + @"\";
                                        CommonMapFilePath = System.Web.HttpContext.Current.Request.MapPath(tempPath);
                                        grantAccess(CommonMapFilePath);

                                        CommonMapFilePath = CommonMapFilePath + @"img.png";
                                        CommonTempPath = ServerUploadFolder1 + objForm.projectId + @"\" + objForm.formId + @"\" +
                                        objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "CommonMAP" + @"\img.png";

                                        //String Url to Bytes
                                        byte[] bytes = GetBytesFromUrl(urlPath1);

                                        //Bytes Saved to Specified File
                                        WriteBytesToFile(CommonMapFilePath, bytes);

                                        #endregion
                                    }

                                    foreach (var item in objForm.currentAccountData.accountFieldDataList)
                                    {
                                        tblProjectFieldDataMaster objtblProjectFieldDataMaster = new tblProjectFieldDataMaster();
                                        objtblProjectFieldDataMaster.Active = 1;
                                        objtblProjectFieldDataMaster.Comments = item.commentForServiceField;
                                        objtblProjectFieldDataMaster.CreatedOn = new CommonFunctions().ServerDate();
                                        objtblProjectFieldDataMaster.CreatedBy = int.Parse(objForm.userId.ToString());
                                        objtblProjectFieldDataMaster.FieldLatitude = item.geolocationLatitudeValue;
                                        objtblProjectFieldDataMaster.FieldLongitude = item.geolocationLongitudeValue;
                                        objtblProjectFieldDataMaster.FieldStatus = item.statusForServiceField;
                                        if (!string.IsNullOrEmpty(item.capturedDateTime))
                                        {
                                            objtblProjectFieldDataMaster.CapturedDateTime = getDate(item.capturedDateTime);
                                        }


                                        //  objtblProjectFieldDataMaster.FieldValue = item.fieldName == "WORK COMPLETED" ? "false" : item.fieldValue;
                                        objtblProjectFieldDataMaster.FieldValue = item.fieldValue;

                                        if (item.fieldName == "LOCATION/GPS")
                                        {

                                            objtblProjectFieldDataMaster.FieldValue = item.geolocationLatitudeValue + ", " + item.geolocationLongitudeValue;

                                            #region location image
                                            if (!string.IsNullOrEmpty(item.geolocationLatitudeValue) && !string.IsNullOrEmpty(item.geolocationLongitudeValue))
                                            {


                                                string latlng = item.geolocationLatitudeValue + "," + item.geolocationLongitudeValue;
                                                string urlPath = "https://maps.googleapis.com/maps/api/staticmap?center=" + latlng +
                                                   "&zoom=15&size=200x200&maptype=roadmap&markers=color:green%7C" + latlng + "&sensor=false&key=" + googleKey;

                                                string ServerUploadFolder = @"~\MobileUploadData\";
                                                string ServerUploadFolder1 = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"]);
                                                string tempPath = ServerUploadFolder + objForm.projectId + @"\" + objForm.formId + @"\" +
                                                objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "MAP" + @"\";
                                                mapFilePath = System.Web.HttpContext.Current.Request.MapPath(tempPath);
                                                grantAccess(mapFilePath);

                                                mapFilePath = mapFilePath + @"img.png";
                                                tempPath1 = ServerUploadFolder1 + objForm.projectId + @"\" + objForm.formId + @"\" +
                                                objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "MAP" + @"\img.png";

                                                //String Url to Bytes
                                                byte[] bytes = GetBytesFromUrl(urlPath);

                                                //Bytes Saved to Specified File
                                                WriteBytesToFile(mapFilePath, bytes);
                                            }
                                            #endregion

                                        }


                                        objtblProjectFieldDataMaster.InitialFieldId = item.fieldId;
                                        objtblProjectFieldDataMaster.InitialFieldName = item.fieldName.Trim();

                                        if (item.fieldName == "WORK COMPLETED" && string.IsNullOrEmpty(item.fieldValue))
                                        {
                                            item.fieldValue = "true";
                                            objtblProjectFieldDataMaster.FieldValue = "true";
                                        }


                                        objtblProjectFieldDataMaster.TblProjectFieldData_Id = objTblProjectFieldData.ID;
                                        objCompassEntities.tblProjectFieldDataMasters.Add(objtblProjectFieldDataMaster);
                                        objCompassEntities.SaveChanges();

                                        //if (item.fieldName == "SERVICES" && objForm.inventoryDetails != null && objForm.inventoryDetails.Count > 0 && !string.IsNullOrEmpty(item.fieldValue))
                                        if (item.fieldName == "SERVICES" && !string.IsNullOrEmpty(item.fieldValue))
                                        {
                                            if (objProjectDetails.ManageInventory == "Yes")
                                            {
                                                #region
                                                string[] serviceCurrentlist = item.fieldValue.Split('|').ToArray();

                                                foreach (var SID in serviceCurrentlist)
                                                {

                                                    var currentServiceId = long.Parse(SID);


                                                    var currentServiceProductList = ServiceProductList.Where(o => o.FKServiceId == currentServiceId).ToList();

                                                    var serialNumber1 = "";

                                                    if (currentServiceId == 1)
                                                    {
                                                        serialNumber1 = objForm.currentAccountData.accountFieldDataList.Where(o => o.fieldName == "NEW METER NUMBER").Select(o => o.fieldValue).FirstOrDefault();
                                                    }
                                                    else if (currentServiceId == 4 || currentServiceId == 6)
                                                    {
                                                        serialNumber1 = objForm.currentAccountData.accountFieldDataList.Where(o => o.fieldName == "NEW METER RADIO").Select(o => o.fieldValue).FirstOrDefault();
                                                    }


                                                    foreach (var itemProduct in currentServiceProductList)
                                                    {
                                                        var Product = objCompassEntities.ProductMasters.Where(o => o.Id == itemProduct.Id).FirstOrDefault();


                                                        if (Product.SerialNumberRequired == true)
                                                        {
                                                            #region

                                                            var serialNumber = "";

                                                            if (objForm.currentAccountData.inventoryDetails != null && objForm.currentAccountData.inventoryDetails.Count > 0)
                                                            {
                                                                ServiceDataSubmit objNewTable = objForm.currentAccountData.inventoryDetails.
                                                                    Where(o => o.serviceId == itemProduct.FKServiceId).FirstOrDefault();



                                                                if (objNewTable != null && objNewTable.produtDetails != null && objNewTable.produtDetails.Count > 0)
                                                                {
                                                                    serialNumber = objNewTable.produtDetails.Where(o => o.productId == itemProduct.Id).
                                                                        Select(o => o.serialNumber).FirstOrDefault();
                                                                }
                                                                else if (!string.IsNullOrEmpty(serialNumber1))
                                                                {


                                                                    var checkstockdetailProductId = objCompassEntities.StockDetails.Where(o => o.SerialNumber == serialNumber1
                                                                                                    && o.Active == 1 && o.FKProjectId == objForm.projectId).FirstOrDefault();

                                                                    if (checkstockdetailProductId != null && checkstockdetailProductId.FKProductId == itemProduct.Id)
                                                                    {
                                                                        serialNumber = serialNumber1;
                                                                    }
                                                                }

                                                            }
                                                            else if ((objForm.currentAccountData.inventoryDetails == null ||
                                                                objForm.currentAccountData.inventoryDetails.Count == 0) && !string.IsNullOrEmpty(serialNumber1))
                                                            {


                                                                var checkstockdetailProductId = objCompassEntities.StockDetails.Where(o => o.SerialNumber == serialNumber1
                                                                    && o.Active == 1 && o.FKProjectId == objForm.projectId).FirstOrDefault();

                                                                if (checkstockdetailProductId != null && checkstockdetailProductId.FKProductId == itemProduct.Id)
                                                                {
                                                                    serialNumber = serialNumber1;
                                                                }
                                                            }


                                                            if (!string.IsNullOrEmpty(serialNumber))
                                                            {
                                                                tblProjectFieldDataInventoryDetail objInvent = new tblProjectFieldDataInventoryDetail();
                                                                objInvent.Active = 1;
                                                                objInvent.CreatedOn = DateTime.Now;
                                                                objInvent.CreatedBy = long.Parse(objForm.userId.ToString());

                                                                objInvent.FKServiceId = itemProduct.FKServiceId;
                                                                objInvent.FKProductId = itemProduct.Id;
                                                                objInvent.FKtblProjectFieldDataMasterID = objtblProjectFieldDataMaster.ID;
                                                                objInvent.StockVerified = 0;
                                                                objInvent.SerialNumber = serialNumber;

                                                                objInvent.TotalQty = itemProduct.Quantity == null ? 0 : int.Parse(itemProduct.Quantity.ToString());//TRecord.qty;

                                                                objCompassEntities.tblProjectFieldDataInventoryDetails.Add(objInvent);
                                                                objCompassEntities.SaveChanges();

                                                                var OutWardRow = objCompassEntities.OutwardDetails.Where(o => o.AllocatedTo == userId
                                                                 && o.AllStockUtilized == false && o.Active == 1 && o.StockDetail.FKProductId == itemProduct.Id
                                                                 && o.StockDetail.SerialNumber == serialNumber && o.StockDetail.FKProjectId == objForm.projectId).FirstOrDefault();

                                                                if (OutWardRow != null)
                                                                {

                                                                    tblProjectFieldDataInventoryOutwardDetail objOut = new tblProjectFieldDataInventoryOutwardDetail();
                                                                    objOut.Active = 1;
                                                                    objOut.CreatedOn = DateTime.Now;
                                                                    objOut.CreatedBy = long.Parse(objForm.userId.ToString());

                                                                    objOut.FKOutwardDetailsId = OutWardRow.Id;
                                                                    objOut.FKtblProjectFieldDataInventoryDetailsId = objInvent.ID;
                                                                    objOut.TotalQty = 1;
                                                                    objCompassEntities.tblProjectFieldDataInventoryOutwardDetails.Add(objOut);
                                                                    objCompassEntities.SaveChanges();

                                                                    // objInvent.FKOutwardDetailsId = outItem.Id;
                                                                    objInvent.StockVerified = 1;
                                                                    objCompassEntities.SaveChanges();

                                                                    OutWardRow.AllStockUtilized = true;
                                                                    OutWardRow.InstalledQty = 1;
                                                                    OutWardRow.ModifiedOn = DateTime.Now;
                                                                    OutWardRow.ModifiedBy = long.Parse(objForm.userId.ToString());
                                                                    objCompassEntities.SaveChanges();

                                                                    var stockRecord = objCompassEntities.StockDetails.Where(o => o.Id == OutWardRow.FKStockDetailsID).FirstOrDefault();
                                                                    stockRecord.IsAllocated = true;
                                                                    stockRecord.AllStockUtilized = true;
                                                                    stockRecord.BalanceQty = 0;
                                                                    objCompassEntities.SaveChanges();

                                                                }

                                                            }


                                                            #endregion
                                                        }
                                                        else
                                                        {

                                                            #region


                                                            tblProjectFieldDataInventoryDetail objInvent = new tblProjectFieldDataInventoryDetail();
                                                            objInvent.Active = 1;
                                                            objInvent.CreatedOn = DateTime.Now;
                                                            objInvent.CreatedBy = long.Parse(objForm.userId.ToString());

                                                            objInvent.FKServiceId = itemProduct.FKServiceId;
                                                            objInvent.FKProductId = itemProduct.Id;
                                                            objInvent.FKtblProjectFieldDataMasterID = objtblProjectFieldDataMaster.ID;
                                                            objInvent.StockVerified = 0;
                                                            //objInvent.SerialNumber = TRecord.serialNumber;

                                                            objInvent.TotalQty = itemProduct.Quantity == null ? 0 : int.Parse(itemProduct.Quantity.ToString());//TRecord.qty;

                                                            objCompassEntities.tblProjectFieldDataInventoryDetails.Add(objInvent);
                                                            objCompassEntities.SaveChanges();



                                                            var OutWardRow = objCompassEntities.OutwardDetails.Where(o => o.AllocatedTo == userId
                                                           && o.AllStockUtilized == false && o.Active == 1 && o.StockDetail.FKProductId == itemProduct.Id
                                                           && o.StockDetail.FKProjectId == objForm.projectId).ToList();

                                                            if (OutWardRow != null)
                                                            {
                                                                int usedQty = objInvent.TotalQty;

                                                                foreach (var outItem in OutWardRow)
                                                                {
                                                                    int AllocatedQty = outItem.AllocatedQty;
                                                                    int UsedQty = outItem.InstalledQty + outItem.TransferredQty + outItem.ReturnToWarehouseQty + outItem.RemovedQty;

                                                                    int BalanceQty = AllocatedQty - UsedQty;

                                                                    if (usedQty < BalanceQty)
                                                                    {
                                                                        tblProjectFieldDataInventoryOutwardDetail objOut = new tblProjectFieldDataInventoryOutwardDetail();
                                                                        objOut.Active = 1;
                                                                        objOut.CreatedOn = DateTime.Now;
                                                                        objOut.CreatedBy = long.Parse(objForm.userId.ToString());

                                                                        objOut.FKOutwardDetailsId = outItem.Id;
                                                                        objOut.FKtblProjectFieldDataInventoryDetailsId = objInvent.ID;
                                                                        objOut.TotalQty = usedQty;
                                                                        objCompassEntities.tblProjectFieldDataInventoryOutwardDetails.Add(objOut);
                                                                        objCompassEntities.SaveChanges();

                                                                        // objInvent.FKOutwardDetailsId = outItem.Id;
                                                                        objInvent.StockVerified = 1;
                                                                        objCompassEntities.SaveChanges();

                                                                        outItem.InstalledQty = outItem.InstalledQty + usedQty;
                                                                        outItem.ModifiedOn = DateTime.Now;
                                                                        outItem.ModifiedBy = long.Parse(objForm.userId.ToString());
                                                                        objCompassEntities.SaveChanges();
                                                                        break;
                                                                    }
                                                                    else if (usedQty > BalanceQty)
                                                                    {

                                                                        tblProjectFieldDataInventoryOutwardDetail objOut = new tblProjectFieldDataInventoryOutwardDetail();
                                                                        objOut.Active = 1;
                                                                        objOut.CreatedOn = DateTime.Now;
                                                                        objOut.CreatedBy = long.Parse(objForm.userId.ToString());

                                                                        objOut.FKOutwardDetailsId = outItem.Id;
                                                                        objOut.FKtblProjectFieldDataInventoryDetailsId = objInvent.ID;
                                                                        objOut.TotalQty = (usedQty - BalanceQty);
                                                                        objCompassEntities.tblProjectFieldDataInventoryOutwardDetails.Add(objOut);
                                                                        objCompassEntities.SaveChanges();


                                                                        outItem.InstalledQty = outItem.InstalledQty + (usedQty - BalanceQty);
                                                                        outItem.AllStockUtilized = true;
                                                                        outItem.ModifiedOn = DateTime.Now;
                                                                        outItem.ModifiedBy = long.Parse(objForm.userId.ToString());
                                                                        objCompassEntities.SaveChanges();
                                                                        usedQty = usedQty - BalanceQty;
                                                                    }
                                                                    else
                                                                    {

                                                                        tblProjectFieldDataInventoryOutwardDetail objOut = new tblProjectFieldDataInventoryOutwardDetail();
                                                                        objOut.Active = 1;
                                                                        objOut.CreatedOn = DateTime.Now;
                                                                        objOut.CreatedBy = long.Parse(objForm.userId.ToString());

                                                                        objOut.FKOutwardDetailsId = outItem.Id;
                                                                        objOut.FKtblProjectFieldDataInventoryDetailsId = objInvent.ID;
                                                                        objOut.TotalQty = usedQty;
                                                                        objCompassEntities.tblProjectFieldDataInventoryOutwardDetails.Add(objOut);
                                                                        objCompassEntities.SaveChanges();


                                                                        outItem.InstalledQty = outItem.InstalledQty + usedQty;
                                                                        outItem.AllStockUtilized = true;
                                                                        outItem.ModifiedOn = DateTime.Now;
                                                                        outItem.ModifiedBy = long.Parse(objForm.userId.ToString());
                                                                        objCompassEntities.SaveChanges();
                                                                        usedQty = usedQty - BalanceQty;
                                                                    }

                                                                }

                                                            }


                                                            #endregion

                                                        }

                                                    }



                                                }

                                                #endregion
                                            }
                                        }

                                        //  if (item.fieldName == "ADDITIONAL SERVICES" && objForm.inventoryDetails != null && objForm.inventoryDetails.Count > 0 && !string.IsNullOrEmpty(item.fieldValue))
                                        if (item.fieldName == "ADDITIONAL SERVICES" && !string.IsNullOrEmpty(item.fieldValue))
                                        {
                                            if (objProjectDetails.ManageInventory == "Yes")
                                            {
                                                #region
                                                string[] serviceCurrentlist = item.fieldValue.Split('|').ToArray();

                                                foreach (var SID in serviceCurrentlist)
                                                {
                                                    var currentServiceId = long.Parse(SID);


                                                    var currentServiceProductList = ServiceProductList.Where(o => o.FKServiceId == currentServiceId).ToList();


                                                    var serialNumber1 = "";

                                                    if (currentServiceId == 1)
                                                    {
                                                        serialNumber1 = objForm.currentAccountData.accountFieldDataList.Where(o => o.fieldName == "NEW METER NUMBER").Select(o => o.fieldValue).FirstOrDefault();
                                                    }
                                                    else if (currentServiceId == 4 || currentServiceId == 6)
                                                    {
                                                        serialNumber1 = objForm.currentAccountData.accountFieldDataList.Where(o => o.fieldName == "NEW METER RADIO").Select(o => o.fieldValue).FirstOrDefault();
                                                    }




                                                    foreach (var itemProduct in currentServiceProductList)
                                                    {
                                                        var Product = objCompassEntities.ProductMasters.Where(o => o.Id == itemProduct.Id).FirstOrDefault();


                                                        if (Product.SerialNumberRequired == true)
                                                        {
                                                            #region serial number

                                                            var serialNumber = "";

                                                            if (objForm.currentAccountData.inventoryDetails != null && objForm.currentAccountData.inventoryDetails.Count > 0)
                                                            {
                                                                ServiceDataSubmit objNewTable = objForm.currentAccountData.inventoryDetails.
                                                                    Where(o => o.serviceId == itemProduct.FKServiceId).FirstOrDefault();



                                                                if (objNewTable != null && objNewTable.produtDetails != null && objNewTable.produtDetails.Count > 0)
                                                                {
                                                                    serialNumber = objNewTable.produtDetails.Where(o => o.productId == itemProduct.Id).
                                                                        Select(o => o.serialNumber).FirstOrDefault();
                                                                }
                                                                else if (!string.IsNullOrEmpty(serialNumber1))
                                                                {
                                                                    var checkstockdetailProductId = objCompassEntities.StockDetails.
                                                                        Where(o => o.SerialNumber == serialNumber1 && o.Active == 1 && o.FKProjectId == objForm.projectId).FirstOrDefault();

                                                                    if (checkstockdetailProductId != null && checkstockdetailProductId.FKProductId == itemProduct.Id)
                                                                    {
                                                                        serialNumber = serialNumber1;
                                                                    }
                                                                }

                                                            }
                                                            else if ((objForm.currentAccountData.inventoryDetails == null || objForm.currentAccountData.inventoryDetails.Count == 0) && !string.IsNullOrEmpty(serialNumber1))
                                                            {
                                                                var checkstockdetailProductId = objCompassEntities.StockDetails.Where(o => o.SerialNumber == serialNumber1
                                                                    && o.Active == 1 && o.FKProjectId == objForm.projectId).FirstOrDefault();

                                                                if (checkstockdetailProductId != null && checkstockdetailProductId.FKProductId == itemProduct.Id)
                                                                {
                                                                    serialNumber = serialNumber1;
                                                                }
                                                            }


                                                            if (!string.IsNullOrEmpty(serialNumber))
                                                            {

                                                                tblProjectFieldDataInventoryDetail objInvent = new tblProjectFieldDataInventoryDetail();
                                                                objInvent.Active = 1;
                                                                objInvent.CreatedOn = DateTime.Now;
                                                                objInvent.CreatedBy = long.Parse(objForm.userId.ToString());

                                                                objInvent.FKServiceId = itemProduct.FKServiceId;
                                                                objInvent.FKProductId = itemProduct.Id;
                                                                objInvent.FKtblProjectFieldDataMasterID = objtblProjectFieldDataMaster.ID;
                                                                objInvent.StockVerified = 0;
                                                                objInvent.SerialNumber = serialNumber;

                                                                objInvent.TotalQty = itemProduct.Quantity == null ? 0 : int.Parse(itemProduct.Quantity.ToString());//TRecord.qty;

                                                                objCompassEntities.tblProjectFieldDataInventoryDetails.Add(objInvent);
                                                                objCompassEntities.SaveChanges();



                                                                var OutWardRow = objCompassEntities.OutwardDetails.Where(o => o.AllocatedTo == userId
                                                                 && o.AllStockUtilized == false && o.Active == 1 && o.StockDetail.FKProductId == itemProduct.Id
                                                                 && o.StockDetail.SerialNumber == serialNumber
                                                                 && o.StockDetail.FKProjectId == objForm.projectId).FirstOrDefault();

                                                                if (OutWardRow != null)
                                                                {
                                                                    tblProjectFieldDataInventoryOutwardDetail objOut = new tblProjectFieldDataInventoryOutwardDetail();
                                                                    objOut.Active = 1;
                                                                    objOut.CreatedOn = DateTime.Now;
                                                                    objOut.CreatedBy = long.Parse(objForm.userId.ToString());

                                                                    objOut.FKOutwardDetailsId = OutWardRow.Id;
                                                                    objOut.FKtblProjectFieldDataInventoryDetailsId = objInvent.ID;
                                                                    objOut.TotalQty = 1;
                                                                    objCompassEntities.tblProjectFieldDataInventoryOutwardDetails.Add(objOut);
                                                                    objCompassEntities.SaveChanges();

                                                                    // objInvent.FKOutwardDetailsId = outItem.Id;
                                                                    objInvent.StockVerified = 1;
                                                                    objCompassEntities.SaveChanges();

                                                                    OutWardRow.AllStockUtilized = true;
                                                                    OutWardRow.InstalledQty = 1;
                                                                    OutWardRow.ModifiedOn = DateTime.Now;
                                                                    OutWardRow.ModifiedBy = long.Parse(objForm.userId.ToString());
                                                                    objCompassEntities.SaveChanges();

                                                                    var stockRecord = objCompassEntities.StockDetails.Where(o => o.Id == OutWardRow.FKStockDetailsID).FirstOrDefault();
                                                                    stockRecord.IsAllocated = true;
                                                                    stockRecord.BalanceQty = 0;
                                                                    stockRecord.AllStockUtilized = true;
                                                                    objCompassEntities.SaveChanges();


                                                                }

                                                            }

                                                            #endregion
                                                        }
                                                        else
                                                        {
                                                            #region Quantity Product

                                                            tblProjectFieldDataInventoryDetail objInvent = new tblProjectFieldDataInventoryDetail();
                                                            objInvent.Active = 1;
                                                            objInvent.CreatedOn = DateTime.Now;
                                                            objInvent.CreatedBy = long.Parse(objForm.userId.ToString());

                                                            objInvent.FKServiceId = itemProduct.FKServiceId;
                                                            objInvent.FKProductId = itemProduct.Id;
                                                            objInvent.FKtblProjectFieldDataMasterID = objtblProjectFieldDataMaster.ID;
                                                            objInvent.StockVerified = 0;
                                                            //objInvent.SerialNumber = TRecord.serialNumber;

                                                            objInvent.TotalQty = itemProduct.Quantity == null ? 0 : int.Parse(itemProduct.Quantity.ToString());//TRecord.qty;

                                                            objCompassEntities.tblProjectFieldDataInventoryDetails.Add(objInvent);
                                                            objCompassEntities.SaveChanges();

                                                            var OutWardRow = objCompassEntities.OutwardDetails.Where(o => o.AllocatedTo == userId
                                                           && o.AllStockUtilized == false && o.Active == 1 && o.StockDetail.FKProductId == itemProduct.Id
                                                            && o.StockDetail.FKProjectId == objForm.projectId).ToList();

                                                            if (OutWardRow != null)
                                                            {
                                                                int usedQty = objInvent.TotalQty;

                                                                foreach (var outItem in OutWardRow)
                                                                {
                                                                    int AllocatedQty = outItem.AllocatedQty;
                                                                    int UsedQty = outItem.InstalledQty + outItem.TransferredQty + outItem.ReturnToWarehouseQty + outItem.RemovedQty;

                                                                    int BalanceQty = AllocatedQty - UsedQty;

                                                                    if (usedQty < BalanceQty)
                                                                    {
                                                                        tblProjectFieldDataInventoryOutwardDetail objOut = new tblProjectFieldDataInventoryOutwardDetail();
                                                                        objOut.Active = 1;
                                                                        objOut.CreatedOn = DateTime.Now;
                                                                        objOut.CreatedBy = long.Parse(objForm.userId.ToString());

                                                                        objOut.FKOutwardDetailsId = outItem.Id;
                                                                        objOut.FKtblProjectFieldDataInventoryDetailsId = objInvent.ID;
                                                                        objOut.TotalQty = usedQty;
                                                                        objCompassEntities.tblProjectFieldDataInventoryOutwardDetails.Add(objOut);
                                                                        objCompassEntities.SaveChanges();

                                                                        // objInvent.FKOutwardDetailsId = outItem.Id;
                                                                        objInvent.StockVerified = 1;
                                                                        objCompassEntities.SaveChanges();

                                                                        outItem.InstalledQty = outItem.InstalledQty + usedQty;
                                                                        outItem.ModifiedOn = DateTime.Now;
                                                                        outItem.ModifiedBy = long.Parse(objForm.userId.ToString());
                                                                        objCompassEntities.SaveChanges();
                                                                        break;
                                                                    }
                                                                    else if (usedQty > BalanceQty)
                                                                    {

                                                                        tblProjectFieldDataInventoryOutwardDetail objOut = new tblProjectFieldDataInventoryOutwardDetail();
                                                                        objOut.Active = 1;
                                                                        objOut.CreatedOn = DateTime.Now;
                                                                        objOut.CreatedBy = long.Parse(objForm.userId.ToString());

                                                                        objOut.FKOutwardDetailsId = outItem.Id;
                                                                        objOut.FKtblProjectFieldDataInventoryDetailsId = objInvent.ID;
                                                                        objOut.TotalQty = (usedQty - BalanceQty);
                                                                        objCompassEntities.tblProjectFieldDataInventoryOutwardDetails.Add(objOut);
                                                                        objCompassEntities.SaveChanges();


                                                                        outItem.InstalledQty = outItem.InstalledQty + (usedQty - BalanceQty);
                                                                        outItem.AllStockUtilized = true;
                                                                        outItem.ModifiedOn = DateTime.Now;
                                                                        outItem.ModifiedBy = long.Parse(objForm.userId.ToString());
                                                                        objCompassEntities.SaveChanges();
                                                                        usedQty = usedQty - BalanceQty;
                                                                    }
                                                                    else
                                                                    {

                                                                        tblProjectFieldDataInventoryOutwardDetail objOut = new tblProjectFieldDataInventoryOutwardDetail();
                                                                        objOut.Active = 1;
                                                                        objOut.CreatedOn = DateTime.Now;
                                                                        objOut.CreatedBy = long.Parse(objForm.userId.ToString());

                                                                        objOut.FKOutwardDetailsId = outItem.Id;
                                                                        objOut.FKtblProjectFieldDataInventoryDetailsId = objInvent.ID;
                                                                        objOut.TotalQty = usedQty;
                                                                        objCompassEntities.tblProjectFieldDataInventoryOutwardDetails.Add(objOut);
                                                                        objCompassEntities.SaveChanges();


                                                                        outItem.InstalledQty = outItem.InstalledQty + usedQty;
                                                                        outItem.AllStockUtilized = true;
                                                                        outItem.ModifiedOn = DateTime.Now;
                                                                        outItem.ModifiedBy = long.Parse(objForm.userId.ToString());
                                                                        objCompassEntities.SaveChanges();
                                                                        usedQty = usedQty - BalanceQty;
                                                                    }

                                                                }

                                                            }
                                                            #endregion

                                                        }


                                                    }


                                                }

                                                #endregion
                                            }
                                        }

                                        if (item.fieldName == "LOCATION/GPS")
                                        {
                                            masterFieldId = objtblProjectFieldDataMaster.ID;
                                        }

                                        if (item.fieldName == "NEW METER NUMBER" || item.fieldName == "NEW METER RADIO")
                                        {

                                            if (item.fieldName == "NEW METER NUMBER")
                                            {
                                                newMeterNo = item.fieldValue;
                                            }
                                            if (item.fieldName == "NEW METER RADIO")
                                            {
                                                newMeterRadioNo = item.fieldValue;
                                            }
                                        }
                                    }



                                    if (masterFieldId != 0 && !string.IsNullOrEmpty(mapFilePath))
                                    {
                                        TblProjectFieldDataAttachment obj = new TblProjectFieldDataAttachment();
                                        obj.Active = 1;
                                        obj.CreatedOn = new CommonFunctions().ServerDate();
                                        obj.CreatedBy = int.Parse(objForm.userId.ToString());
                                        obj.tblProjectFieldDataMaster_ID = masterFieldId;
                                        obj.IsAudioVideoImage = 3;
                                        obj.strFilePath = tempPath1;
                                        obj.isServiceData = false;
                                        objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                                        objCompassEntities.SaveChanges();
                                    }

                                    if (!string.IsNullOrEmpty(CommonMapFilePath))
                                    {
                                        TblProjectFieldDataAttachment obj = new TblProjectFieldDataAttachment();
                                        obj.Active = 1;
                                        obj.CreatedOn = new CommonFunctions().ServerDate();
                                        obj.CreatedBy = int.Parse(objForm.userId.ToString());
                                        obj.ProjectFieldDataId = objTblProjectFieldData.ID;
                                        obj.IsAudioVideoImage = 3;
                                        obj.strFilePath = CommonTempPath;
                                        obj.isServiceData = false;
                                        objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                                        objCompassEntities.SaveChanges();
                                    }

                                    ///Add record to Audit.

                                    var workStatus = objForm.currentAccountData.accountFieldDataList.Where(o => o.fieldName.ToLower() == "WORK COMPLETED".ToLower() && o.fieldValue == "true").Count();
                                    if (workStatus == 1)
                                    {
                                        var excelData = objCompassEntities.UploadedExcelDatas.Where(o => o.Id == objForm.currentAccountData.initialFieldId && o.Active == 1).FirstOrDefault();
                                        excelData.UploadStatus = 2;
                                        AuditData objAudit = new AuditData();

                                        objAudit.Fk_InstallerId = objTblProjectFieldData.InstallerId;
                                        objAudit.FK_FieldDataId = objTblProjectFieldData.ID;
                                        objAudit.FK_ProjectId = objForm.projectId;
                                        objAudit.FK_UploadId = long.Parse(objTblProjectFieldData.FK_UploadedExcelData_Id.ToString());
                                        objAudit.VisitDate = DateTime.Parse(objTblProjectFieldData.visitdatetime.ToString());
                                        objAudit.AddedForAudit = 0;
                                        objAudit.Active = 1;
                                        objAudit.CreatedOn = new CommonFunctions().ServerDate();
                                        objAudit.CreatedBy = (int)objTblProjectFieldData.InstallerId;


                                        objCompassEntities.AuditDatas.Add(objAudit);
                                        objCompassEntities.SaveChanges();
                                    }




                                }

                                projectFieldDataId = objTblProjectFieldData.ID;

                                isRecrodSaved = true;
                                responseMessage = "Record Saved Successfull.";

                                #endregion
                            }

                            scope.Complete();

                            Task.Factory.StartNew(() =>
                                {
                                    if (isRecrodSaved == true && (!string.IsNullOrEmpty(newMeterNo) || !string.IsNullOrEmpty(newMeterRadioNo)))
                                    {
                                        CheckDuplicateMeterOrRadio(objFormDetails.ProjectId, projectFieldDataId, newMeterNo, newMeterRadioNo);
                                    }
                                });



                            #endregion

                        }
                    }
                }
                else if (objFormDetails.FormTypeID == 1)
                {
                    isRecrodSaved = saveDynamicAuditData(objForm, _jsonData, out responseMessage, googleKey);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isRecrodSaved;

        }




        /// <summary>
        /// returns date
        /// </summary>
        /// <param name="strDateTime"></param>
        /// <returns></returns>
        public DateTime getDate(string strDateTime)
        {
            string[] subStr = strDateTime.Split(' ');
            string[] strDate = subStr[0].ToString().Split('-');
            string[] strTimeSplit = subStr[1].Split(':');
            int _year = int.Parse(strDate[2]);
            int _Mon = DateTime.ParseExact(strDate[0], "MMM", CultureInfo.CurrentCulture).Month;
            int _Day = int.Parse(strDate[1]);

            int _hr = subStr[2] == "PM" ? (strTimeSplit[0] == "12" ? int.Parse(strTimeSplit[0]) : (12 + int.Parse(strTimeSplit[0]))) : int.Parse(strTimeSplit[0]);
            int _MN = int.Parse(strTimeSplit[1]);

            DateTime _VisitDatetime = new DateTime(_year, _Mon, _Day, _hr, _MN, 0);
            return _VisitDatetime;
        }


        public List<CaptureGeoLocation> GetGeoCaptureFields(long FormId)
        {

            List<CaptureGeoLocation> objList = new List<CaptureGeoLocation>();
            CompassEntities objCompassEntities1 = new CompassEntities();
            var SectionList = objCompassEntities1.tblFormSectionRelations.Where(o => o.Active == 1 && o.FormID == FormId).OrderBy(o => o.SortOrderNumber).ToList();

            int i = 1;
            foreach (var item in SectionList)
            {
                var FieldList = objCompassEntities1.tblFormSectionFieldRelations.Where(o => o.Active == 1 && o.SectionId == item.SectionId).OrderBy(o => o.FieldOrder).ToList();

                foreach (var itemField in FieldList)
                {
                    if (itemField.FieldName.ToLower() == "Location/Gps".ToLower())
                    {
                        objList.Add(new CaptureGeoLocation { Id = i, FieldName = itemField.FieldName, strImagePath = "images/MapPin/" + i + ".png" });
                        i++;
                    }
                    else
                    {
                        var A = objCompassEntities1.tblProjectFieldProperties.Where(o => o.Active == 1 && o.FormSectionFieldID == itemField.FieldId && o.CaptureGeolocation == true).FirstOrDefault();
                        if (A != null)
                        {
                            objList.Add(new CaptureGeoLocation { Id = i, FieldName = itemField.FieldName, strImagePath = "images/MapPin/" + i + ".png" });
                            i++;
                        }
                    }
                }
            }

            return objList;

        }


        public bool saveDynamicAuditData(FormDataNew objForm, string _jsonData, out string responseMessage, string googleKey)
        {

            bool isRecrodSaved = false;
            responseMessage = "";



            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = new TimeSpan(0, 15, 0);

            //// define our transaction scope
            //var scope = new TransactionScope(
            //    // a new transaction will always be created
            //    TransactionScopeOption.RequiresNew,
            //    // we will allow volatile data to be read during transaction
            //   options
            //);

            try
            {
                // use the scope we just defined
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, options))
                {
                    using (CompassEntities objCompassEntities = new CompassEntities())
                    {

                        string newMeterNo = "";
                        string newMeterRadioNo = "";
                        long projectFieldDataId = 0;
                        //bool saveRecord = false;
                        bool saveRecord = false;

                        var objFormDetails = objCompassEntities.tblFormMasters.Where(o => o.FormId == objForm.formId).FirstOrDefault();

                        var workCompleteFlag = objCompassEntities.PROC_WebService_Audit_CheckRecordStatus_WorkCompleted(objForm.currentAccountData.initialFieldId, objForm.formId).FirstOrDefault();
                        var skipOrRtuFlag = objCompassEntities.PROC_WebService_Audit_CheckRecordStatus_SkipOrRtu(objForm.currentAccountData.initialFieldId, objForm.formId).FirstOrDefault();


                        if (workCompleteFlag != null || skipOrRtuFlag != null)
                        {


                            if (workCompleteFlag != null)
                            {
                                responseMessage = "Failed to save the record. It is already submitted.";
                            }
                            else if (skipOrRtuFlag != null)
                            {
                                long recordId = long.Parse(skipOrRtuFlag.ToString());
                                ProjectAuditData objProjectAuditData = objCompassEntities.ProjectAuditDatas.Where(o => o.ID == recordId).FirstOrDefault();
                                objProjectAuditData.Active = 0;
                                objProjectAuditData.ModifiedOn = new CommonFunctions().ServerDate();
                                objCompassEntities.SaveChanges();
                                #region history inserted
                                ProjectAuditDataHistory objProjectAuditDataHistory = new ProjectAuditDataHistory();
                                objProjectAuditDataHistory.Active = 1;
                                objProjectAuditDataHistory.CreatedBy = 1;
                                objProjectAuditDataHistory.CreatedOn = new CommonFunctions().ServerDate();
                                objProjectAuditDataHistory.FormId = objProjectAuditData.FormId;
                                objProjectAuditDataHistory.FK_AuditorId = objProjectAuditData.FK_AuditorId;
                                objProjectAuditDataHistory.FK_AuditorMapId = objProjectAuditData.FK_AuditorMapId;
                                //objProjectAuditDataHistory.IsReAssign = objProjectAuditData.IsReAssign;
                                objProjectAuditDataHistory.IsSkipped = objProjectAuditData.IsSkipped;
                                objProjectAuditDataHistory.JsonDataVal = objProjectAuditData.JsonDataVal;
                                objProjectAuditDataHistory.SkipComment = objProjectAuditData.SkipComment;
                                objProjectAuditDataHistory.FK_SkipId = objProjectAuditData.FK_SkipId;
                                objProjectAuditDataHistory.SkippedDatetime = objProjectAuditData.SkippedDatetime;
                                objProjectAuditDataHistory.SkippedReason = objProjectAuditData.SkippedReason;
                                // objTblProjectFieldDataHistory.tblUploadedData_Id = objTblProjectFieldData.tblUploadedData_Id;
                                objProjectAuditDataHistory.FK_UploadedExcelData_Id = objProjectAuditData.FK_UploadedExcelData_Id;
                                objProjectAuditDataHistory.VisitDateTime = objProjectAuditData.VisitDateTime;

                                objCompassEntities.ProjectAuditDataHistories.Add(objProjectAuditDataHistory);
                                objCompassEntities.SaveChanges();
                                #endregion

                                saveRecord = true;
                            }

                        }
                        else
                        {
                            saveRecord = true;
                        }


                        if (saveRecord)
                        {




                            ProjectAuditData objProjectAuditData = new ProjectAuditData();
                            objProjectAuditData.Active = 1;
                            objProjectAuditData.CreatedOn = new CommonFunctions().ServerDate();
                            objProjectAuditData.CreatedBy = int.Parse(objForm.userId.ToString());
                            objProjectAuditData.FK_AuditorId = long.Parse(objForm.userId.ToString());
                            objProjectAuditData.FormId = objForm.formId;
                            objProjectAuditData.FK_UploadedExcelData_Id = objForm.currentAccountData.initialFieldId;
                            objProjectAuditData.IsSkipped = objForm.currentAccountData.isAccountSkipped;

                            /* get  */
                            objProjectAuditData.SkipGPSLocation = "";
                            objProjectAuditData.IsMediaPending = objForm.isMediaPending;

                            if (!string.IsNullOrEmpty(objForm.currentAccountData.skipeCapturedDateTime))
                            {
                                objProjectAuditData.SkippedDatetime = getDate(objForm.visitDateTime);
                                objProjectAuditData.SkippedReason = objForm.currentAccountData.skipeReason;
                                objProjectAuditData.SkipComment = objForm.currentAccountData.skipComment;
                                objProjectAuditData.SkipGPSLocation = objForm.currentAccountData.skipGPSLocation;
                                if (objForm.currentAccountData.skipId != null)
                                {
                                    objProjectAuditData.FK_SkipId = long.Parse(objForm.currentAccountData.skipId.ToString());
                                }

                            }


                            if (!string.IsNullOrEmpty(objForm.visitDateTime))
                            {
                                objProjectAuditData.VisitDateTime = getDate(objForm.visitDateTime);
                            }
                            else
                            {
                                // objTblProjectFieldData.VisitDatetime = objTblProjectFieldData.CreatedOn;
                            }
                            //

                            objProjectAuditData.JsonDataVal = _jsonData;
                            objCompassEntities.ProjectAuditDatas.Add(objProjectAuditData);
                            objCompassEntities.SaveChanges();


                            #region skip image capture

                            if (objForm.currentAccountData.skipId != null && objForm.currentAccountData.skipId > 0)
                            {

                                string latlng = objForm.currentAccountData.skipGPSLocation;
                                string urlPath = "https://maps.googleapis.com/maps/api/staticmap?center=" + latlng +
                                   "&zoom=15&size=200x200&maptype=roadmap&markers=color:green%7C" + latlng + "&sensor=false&key=" + googleKey;

                                string ServerUploadFolder = @"~\MobileUploadData\";
                                string ServerUploadFolder1 = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"]);
                                string tempPath = ServerUploadFolder + objForm.projectId + @"\" + objForm.formId + @"\" +
                                objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "MAP" + @"\";
                                string mapFilePath = System.Web.HttpContext.Current.Request.MapPath(tempPath);
                                grantAccess(mapFilePath);

                                mapFilePath = mapFilePath + @"img.png";
                                string tempPath1 = ServerUploadFolder1 + objForm.projectId + @"\" + objForm.formId + @"\" +
                                objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "MAP" + @"\img.png";

                                //String Url to Bytes
                                byte[] bytes = GetBytesFromUrl(urlPath);

                                //Bytes Saved to Specified File
                                WriteBytesToFile(mapFilePath, bytes);



                                ProjectAuditFieldDataAttachment obj = new ProjectAuditFieldDataAttachment();
                                obj.Active = 1;
                                obj.CreatedOn = new CommonFunctions().ServerDate();
                                obj.CreatedBy = 1;
                                obj.FK_ProjectAuditFieldDataId = objProjectAuditData.ID;
                                obj.IsAudioVideoImage = 3;
                                obj.StrFilePath = tempPath1;
                                obj.IsSkipped = true;
                                obj.IsServiceData = false;
                                objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
                                objCompassEntities.SaveChanges();
                            }
                            #endregion

                            if (objForm.currentAccountData.isAccountSkipped == false)
                            {

                                string mapFilePath = string.Empty;
                                string CommonMapFilePath = string.Empty;
                                long masterFieldId = 0;
                                string tempPath1 = string.Empty;
                                string CommonTempPath = string.Empty;

                                var objGeoCaptureList = GetGeoCaptureFields(objForm.formId);

                                string urlPath1 = "https://maps.googleapis.com/maps/api/staticmap?center=";
                                if (objGeoCaptureList != null && objGeoCaptureList.Count > 0)
                                {
                                    #region create gps location map image
                                    int i = 1;
                                    foreach (var item in objGeoCaptureList)
                                    {
                                        var a = objForm.currentAccountData.accountFieldDataList.Where(o => o.fieldName == item.FieldName).FirstOrDefault();

                                        if (a != null && !string.IsNullOrEmpty(a.geolocationLatitudeValue) && !string.IsNullOrEmpty(a.geolocationLongitudeValue))
                                        {
                                            item.strLatLong = a.geolocationLatitudeValue + "," + a.geolocationLongitudeValue;

                                            if (i == 1)
                                            {
                                                urlPath1 = urlPath1 + item.strLatLong + "&zoom=20&size=600x400&maptype=roadmap&";
                                            }

                                            urlPath1 = urlPath1 + "markers=color:blue%7Clabel:" + item.Id.ToString() + "%7C" + item.strLatLong + "&";
                                        }
                                    }


                                    urlPath1 = urlPath1 + "sensor=false&key=" + googleKey;


                                    string ServerUploadFolder = @"~\MobileUploadData\";
                                    string ServerUploadFolder1 = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"]);
                                    string tempPath = ServerUploadFolder + objForm.projectId + @"\" + objForm.formId + @"\" +
                                    objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "CommonMAP" + @"\";
                                    CommonMapFilePath = System.Web.HttpContext.Current.Request.MapPath(tempPath);
                                    grantAccess(CommonMapFilePath);

                                    CommonMapFilePath = CommonMapFilePath + @"img.png";
                                    CommonTempPath = ServerUploadFolder1 + objForm.projectId + @"\" + objForm.formId + @"\" +
                                    objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "CommonMAP" + @"\img.png";

                                    //String Url to Bytes
                                    byte[] bytes = GetBytesFromUrl(urlPath1);

                                    //Bytes Saved to Specified File
                                    WriteBytesToFile(CommonMapFilePath, bytes);

                                    #endregion
                                }




                                #region save

                                foreach (var item in objForm.currentAccountData.accountFieldDataList)
                                {
                                    ProjectAuditFieldDataMaster ObjProjectAuditFieldDataMaster = new ProjectAuditFieldDataMaster();
                                    ObjProjectAuditFieldDataMaster.Active = 1;
                                    ObjProjectAuditFieldDataMaster.Comments = item.commentForServiceField;
                                    ObjProjectAuditFieldDataMaster.CreatedOn = new CommonFunctions().ServerDate();
                                    ObjProjectAuditFieldDataMaster.CreatedBy = int.Parse(objForm.userId.ToString());
                                    ObjProjectAuditFieldDataMaster.FieldLatitude = item.geolocationLatitudeValue;
                                    ObjProjectAuditFieldDataMaster.FieldLongitude = item.geolocationLongitudeValue;
                                    ObjProjectAuditFieldDataMaster.FieldStatus = item.statusForServiceField;
                                    if (!string.IsNullOrEmpty(item.capturedDateTime))
                                    {
                                        // DateTime _captureDateTime = UnixTimeStampToDateTime(Convert.ToDouble(item.capturedDateTime));
                                        string[] subStr = item.capturedDateTime.Split(' ');
                                        string[] strDate = subStr[0].ToString().Split('-');
                                        string[] strTimeSplit = subStr[1].Split(':');
                                        int _year = int.Parse(strDate[2]);
                                        int _Mon = DateTime.ParseExact(strDate[0], "MMM", CultureInfo.CurrentCulture).Month;
                                        int _Day = int.Parse(strDate[1]);

                                        int _hr = subStr[2] == "PM" ? (strTimeSplit[0] == "12" ? int.Parse(strTimeSplit[0]) : (12 + int.Parse(strTimeSplit[0]))) : int.Parse(strTimeSplit[0]);
                                        int _MN = int.Parse(strTimeSplit[1]);

                                        DateTime _captureDateTime = new DateTime(_year, _Mon, _Day, _hr, _MN, 0);
                                        ObjProjectAuditFieldDataMaster.CapturedDateTime = _captureDateTime;

                                    }


                                    //  objtblProjectFieldDataMaster.FieldValue = item.fieldName == "WORK COMPLETED" ? "false" : item.fieldValue;
                                    ObjProjectAuditFieldDataMaster.FieldValue = item.fieldValue;

                                    if (item.fieldName == "LOCATION/GPS")
                                    {
                                        ObjProjectAuditFieldDataMaster.FieldValue = item.geolocationLatitudeValue + ", " + item.geolocationLongitudeValue;

                                        string latlng = item.geolocationLatitudeValue + "," + item.geolocationLongitudeValue;
                                        string urlPath = "https://maps.googleapis.com/maps/api/staticmap?center=" + latlng +
                                           "&zoom=15&size=200x200&maptype=roadmap&markers=color:green%7C" + latlng + "&sensor=false&key=" + googleKey;

                                        string ServerUploadFolder = @"~\MobileUploadData\";
                                        string ServerUploadFolder1 = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"]);
                                        string tempPath = ServerUploadFolder + objForm.projectId + @"\" + objForm.formId + @"\" +
                                        objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "MAP" + @"\";
                                        mapFilePath = System.Web.HttpContext.Current.Request.MapPath(tempPath);
                                        grantAccess(mapFilePath);

                                        mapFilePath = mapFilePath + @"img.png";
                                        tempPath1 = ServerUploadFolder1 + objForm.projectId + @"\" + objForm.formId + @"\" +
                                        objForm.currentAccountData.accountNumber + @"\" + objForm.currentAccountData.initialFieldId + @"\" + "MAP" + @"\img.png";

                                        //String Url to Bytes
                                        byte[] bytes = GetBytesFromUrl(urlPath);

                                        //Bytes Saved to Specified File
                                        WriteBytesToFile(mapFilePath, bytes);

                                    }
                                    ObjProjectAuditFieldDataMaster.InitialFieldId = item.fieldId;
                                    ObjProjectAuditFieldDataMaster.InitialFieldName = item.fieldName.Trim();
                                    ObjProjectAuditFieldDataMaster.FK_ProjectAuditData = objProjectAuditData.ID;
                                    objCompassEntities.ProjectAuditFieldDataMasters.Add(ObjProjectAuditFieldDataMaster);
                                    objCompassEntities.SaveChanges();
                                    if (item.fieldName == "LOCATION/GPS")
                                    {
                                        masterFieldId = ObjProjectAuditFieldDataMaster.ID;
                                    }

                                    if (item.fieldName == "NEW METER NUMBER")
                                    {
                                        newMeterNo = item.fieldValue;
                                    }
                                    if (item.fieldName == "NEW METER RADIO")
                                    {
                                        newMeterRadioNo = item.fieldValue;
                                    }

                                }



                                if (masterFieldId != 0 && !string.IsNullOrEmpty(mapFilePath))
                                {
                                    ProjectAuditFieldDataAttachment obj = new ProjectAuditFieldDataAttachment();
                                    obj.Active = 1;
                                    obj.CreatedOn = new CommonFunctions().ServerDate();
                                    obj.CreatedBy = 1;
                                    obj.FK_ProjectAuditFieldDataMaster_ID = masterFieldId;
                                    obj.IsAudioVideoImage = 3;
                                    obj.StrFilePath = tempPath1;
                                    obj.IsServiceData = false;
                                    objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
                                    objCompassEntities.SaveChanges();
                                }


                                if (!string.IsNullOrEmpty(CommonMapFilePath))
                                {
                                    ProjectAuditFieldDataAttachment obj = new ProjectAuditFieldDataAttachment();
                                    obj.Active = 1;
                                    obj.CreatedOn = new CommonFunctions().ServerDate();
                                    obj.CreatedBy = 1;
                                    obj.FK_ProjectAuditFieldDataId = objProjectAuditData.ID;
                                    obj.IsAudioVideoImage = 3;
                                    obj.StrFilePath = CommonTempPath;
                                    obj.IsServiceData = false;
                                    objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
                                    objCompassEntities.SaveChanges();
                                }




                                #endregion

                            }
                            else
                            {

                                var auditDataRecord = objCompassEntities.AuditDatas.Where(o => o.FK_ProjectId == objForm.projectId
                                    && o.FK_UploadId == objForm.currentAccountData.initialFieldId && o.Active == 1).FirstOrDefault();

                                long userId = long.Parse(objForm.userId);
                                var recordAuditPercent = auditDataRecord.AuditPercent;

                                if (recordAuditPercent < 100)
                                {
                                    var newAuditDataRecord = objCompassEntities.AuditDatas.Where(o => o.FK_ProjectId == objForm.projectId
                                                            && o.AddedForAudit == 0 && o.Active == 1 && o.Fk_InstallerId != userId).FirstOrDefault();

                                    var AuditProject = objCompassEntities.AuditMasters.Where(o => o.FKProjectId == objForm.projectId && o.Active == 1).FirstOrDefault();
                                    if (AuditProject != null && newAuditDataRecord != null)
                                    {
                                        ProjectAuditAllocation obj = new ProjectAuditAllocation();

                                        obj.FK_AuditorId = userId;

                                        obj.Active = 1;
                                        obj.CreatedOn = new CommonFunctions().ServerDate();
                                        obj.Fk_ProjectId = objForm.projectId;
                                        obj.FK_UploadedExcelData_Id = newAuditDataRecord.FK_UploadId;
                                        obj.FromDate = AuditProject.AuditStartDate;
                                        obj.ToDate = AuditProject.AuditEndDate;

                                        objCompassEntities.ProjectAuditAllocations.Add(obj);
                                        objCompassEntities.SaveChanges();


                                        newAuditDataRecord.AddedForAudit = 1;
                                        newAuditDataRecord.AuditPercent = recordAuditPercent;
                                        newAuditDataRecord.ModifiedOn = new CommonFunctions().ServerDate();
                                        objCompassEntities.SaveChanges();
                                    }


                                }
                            }

                            projectFieldDataId = objProjectAuditData.ID;

                            isRecrodSaved = true;
                            responseMessage = "Record Saved Successfull.";

                        }

                        scope.Complete();

                        //Task.Factory.StartNew(() =>
                        //{
                        //    if (isRecrodSaved == true && (!string.IsNullOrEmpty(newMeterNo) || !string.IsNullOrEmpty(newMeterRadioNo)))
                        //    {
                        //        CheckDuplicateMeterOrRadio(objFormDetails.ProjectId, projectFieldDataId, newMeterNo, newMeterRadioNo);
                        //    }
                        //});
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isRecrodSaved;

        }


        public void CheckDuplicateMeterOrRadio(long projectId, long projectFieldDataId, string newMeterNo, string newMeterRadioNo)
        {
            List<DuplicateReportBO> objList = new DuplicateReportDAL().GetDuplicateRecords(projectId, 0, 0);
            // Created Date  21-11-2016 by sominath (1:30)
            //To check the duplicate records. if presents then send email with all details of duplecate records to the admin 
            var duplicatefield = objList.Where(x => x.NEWMETERNUMBER == newMeterNo).FirstOrDefault();

            var duplicatefield1 = objList.Where(x => x.NEWMETERRADIO == newMeterRadioNo).FirstOrDefault();

            CompassEntities objCompassEntities = new CompassEntities();

            if (duplicatefield != null || duplicatefield1 != null)
            {
                var projField = GetListByProjectId(projectFieldDataId).ToList(); //objCompassEntities.tblProjectFieldDataMasters.Where(o => o.TblProjectFieldData_Id == projectFieldDataId && o.Active == 1).ToList();

                var data = GetListByDataID(projectFieldDataId).FirstOrDefault(); //objCompassEntities.TblProjectFieldDatas.Where(x => x.ID == projectFieldDataId ).FirstOrDefault();

                long frmId = data.FormId;

                var SectionDat = GetSectionFieldRel(frmId).ToList();               //objCompassEntities.tblFormSectionRelations.Where(x => x.FormID == frmId && x.Active == 1).ToList();

                List<tblFormSectionFieldRelation> fieldDataList = new List<tblFormSectionFieldRelation>();



                foreach (var item in SectionDat)
                {
                    fieldDataList.AddRange(objCompassEntities.tblFormSectionFieldRelations.Where(x => x.SectionId == item.SectionId && x.Active == 1).ToList());
                }

                if (projField != null)
                {


                    DateTime dtserverdate = new CommonFunctions().ServerDate();
                    StringBuilder sb = new StringBuilder();



                    var ProjectModel = (from b in objCompassEntities.tblProjects
                                        join c in objCompassEntities.tblStateMasters on b.StateId equals c.StateId
                                        join d in objCompassEntities.TblCityMasters on b.CityId equals d.CityId
                                        where b.ProjectId == projectId
                                        select new ProjectModel
                                        {
                                            ProjectName = b.ProjectName,
                                            StateName = c.StateName,
                                            CityName = d.CityName,
                                            Utilitytype = b.Utilitytype
                                        }).FirstOrDefault();


                    var Message = GetMessage();

                    //var installerName = objCompassEntities.tblUsers.Where(x => x.UserID == data.InstallerId && x.Active == 1).FirstOrDefault();

                    sb.Append("<body   style='padding: 10px 0px 0px 10px; background: #f8f8fb; font-family:Times New Roman; font-size:14px;'>");
                    sb.Append("Dear " + " Administrator " + ",");
                    sb.Append("  <p style='font-size:15px;'>Greetings from Data Depot....!!!</p>");

                    sb.Append("<div style='background: #e6e1e1; width:100%;'>" + Message + "</div><br/>");
                    sb.Append("<div style='background: #e6e1e1; width:100%;'><table style='width:100%;background: #e6e1e1;border:none;padding: 0px 0px 0px 10px;'>" +
                                 "<tr>" +
                                      "<td style='border:none;'><b>Project Name </b></td>" +
                                      "<td style='border:none;'><b>:</b></td>" +
                                      "<td style='border:none;' colspan='3'>" + ProjectModel.ProjectName.ToUpper() + "</td>" +
                                  "</tr>" +
                                   "<tr>" +
                                      "<td style='border:none;'><b>Utility Type</b></td>" +
                                      "<td style='border:none;'><b>:</b></td>" +
                                      "<td style='border:none;' >" + ProjectModel.Utilitytype + "</td>" +
                                  "</tr>" +
                                   "<tr>" +
                                      "<td style='border:none;'><b>City:</b></td>" +
                                      "<td style='border:none;'><b>:</b></td>" +
                                     " <td style='border:none;' >" + ProjectModel.CityName.ToUpper() + " &nbsp;&nbsp;</td>" +

                                     " <td style='border:none;'><b>State</b></td>" +
                                     " <td style='border:none;'><b>:</b></td>" +
                                      "<td style='border:none;' >" + ProjectModel.StateName.ToUpper() + "</td>" +
                                 " </tr>" +
                                  "<tr>" +
                                     " <td style='border:none;'><b>Visit Start Date</b></td>" +
                                     " <td style='border:none;'><b>:</b></td>" +
                                      "<td style='border:none;' colspan='3'>" + data.visitdatetime + "   &nbsp;&nbsp;</td>" +
                                 "</tr>" +
                                "</table></div>");

                    sb.Append("<br/><div></div>");
                    sb.Append("<html><head> <meta http-equiv='Content-Type' content='text/html;charset=utf-8' /> <meta http-equiv='X-UA-Compatible' content='IE=8,IE=9' /></head><body style='font-family:calibri;padding:0px; margin:36px auto; '><div style=\"margin:0 auto; z-index:1;\"><div style=\"text-align:center;padding-left:35px;padding-top: 9px; \"></div>");
                    sb.Append("<br/><table style='border:1px solid gray;font-family:calibri; font-size:14px; border-radius:5px;margin-left:34px;width:90%;border-collapse:collapse;'>");
                    foreach (var sectionval in SectionDat.OrderBy(x => x.SortOrderNumber).ToList())
                    {
                        sb.Append("<tr  style=\"border-bottom: 1px solid gray !important;\">");
                        sb.Append("<td width='100%' colspan='2' style=\"background: #e6e1e1;border-bottom: 1px solid gray !important;font-weight: bold;\">" + "Section:-" + sectionval.SectionName.ToUpper() + "</td>");

                        sb.Append("</tr>");


                        foreach (var field in fieldDataList.Where(o => o.SectionId == sectionval.SectionId).OrderBy(o => o.FieldOrder).ToList())
                        {
                            var currentField = projField.Where(o => o.InitialFieldName == field.FieldName).FirstOrDefault();
                            if (currentField.InitialFieldName.ToLower() == "services")
                            {
                                string FieldValue = currentField.FieldValue;
                                if (FieldValue != null)
                                {
                                    string[] result = FieldValue.Split('|');
                                    List<string> strServices = new List<string>();
                                    foreach (var item1 in result)
                                    {
                                        int fieldVal = (int)Convert.ToInt32(item1);
                                        var services = objCompassEntities.tblServices.Where(x => x.ID == fieldVal && x.Active == 1).FirstOrDefault();
                                        if (services != null)
                                        {
                                            string Service = services.Service;
                                            strServices.Add(Service);
                                            if (strServices != null)
                                            {
                                                string str_Services = string.Join(",", strServices.ToArray());
                                                sb.Append("<tr  style=\"border-bottom: 1px solid gray !important;\">");
                                                sb.Append("<td width='40%' style=\"border-bottom: 1px solid gray !important;\">" + currentField.InitialFieldName + "</td>");
                                                sb.Append("<td  width='60%' style=\"padding-left:-5px;border-bottom: 1px solid gray !important;\">:&nbsp;" + str_Services + "</td>");
                                                sb.Append("</tr>");
                                            }


                                        }

                                    }
                                }
                                else
                                {
                                    string Service = "-";
                                    sb.Append("<tr  style=\"border-bottom: 1px solid gray !important;\">");
                                    sb.Append("<td width='40%' style=\"border-bottom: 1px solid gray !important;\">" + currentField.InitialFieldName + "</td>");
                                    sb.Append("<td  width='60%' style=\"padding-left:-5px;border-bottom: 1px solid gray !important;\">:&nbsp;" + Service + "</td>");
                                    sb.Append("</tr>");
                                }



                            }
                            else if (currentField.InitialFieldName.ToLower() == "additional services")
                            {
                                string FieldValue = currentField.FieldValue;
                                if (FieldValue != "")
                                {
                                    string[] result = FieldValue.Split('|');
                                    List<string> strAddServices = new List<string>();
                                    foreach (var item1 in result)
                                    {
                                        int fieldVal1 = (int)Convert.ToInt32(item1);
                                        var services = objCompassEntities.tblServices.Where(x => x.ID == fieldVal1 && x.Active == 1).FirstOrDefault();
                                        if (services != null)
                                        {
                                            string Service = services.Service;
                                            strAddServices.Add(Service);
                                            if (strAddServices != null)
                                            {
                                                string str_AddServices = string.Join(",", strAddServices.ToArray());
                                                sb.Append("<tr  style=\"border-bottom: 1px solid gray !important;\">");
                                                sb.Append("<td width='40%' style=\"border-bottom: 1px solid gray !important;\">" + currentField.InitialFieldName + "</td>");
                                                sb.Append("<td  width='60%' style=\"padding-left:-5px;border-bottom: 1px solid gray !important;\">:&nbsp;" + str_AddServices + "</td>");
                                                sb.Append("</tr>");
                                            }


                                        }

                                    }
                                }
                                else
                                {
                                    string Service = "-";
                                    sb.Append("<tr  style=\"border-bottom: 1px solid gray !important;\">");
                                    sb.Append("<td width='40%' style=\"border-bottom: 1px solid gray !important;\">" + currentField.InitialFieldName + "</td>");
                                    sb.Append("<td  width='60%' style=\"padding-left:-5px;border-bottom: 1px solid gray !important;\">:&nbsp;" + Service + "</td>");
                                    sb.Append("</tr>");
                                }

                            }
                            else
                            {
                                sb.Append("<tr  style=\"border-bottom: 1px solid gray !important;\">");
                                sb.Append("<td width='40%' style=\"border-bottom: 1px solid gray !important;\">" + currentField.InitialFieldName + "</td>");
                                sb.Append("<td  width='60%' style=\"padding-left:-5px;border-bottom: 1px solid gray !important;\">:&nbsp;" + currentField.FieldValue + "</td>");
                                sb.Append("</tr>");
                            }



                        }
                    }
                    sb.Append("</table>");
                    sb.Append("<br/><div></div></div></body></html>");

                    sb.Append("<br/><div></div>");

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

                    var UserData = objCompassEntities.TblEmailConfigureUserDetails.Where(o => o.TblEmailConfigureID == 6).ToList();
                    List<String> mailto = new List<String>();
                    foreach (var item in UserData)
                    {
                        var UserMail = objCompassEntities.tblUsers.Where(o => o.UserID == item.UserID).FirstOrDefault();

                        mailto.Add(UserMail.Email);

                    }

                    string Subject = GetSubject();

                    List<String> CCEmailto = new List<String>();

                    CCEmailto = CCEmailTo();                                       // obj.CCEmailTo();
                    Task.Factory.StartNew(() =>
                    {
                        //string myEmail = UserMail.Email;
                        //mailto.Add(myEmail);

                        new GarronT.CMS.Model.clsMail().SendMail(mailto, CCEmailto, MailBody, Subject);

                    });
                    //}





                }




            }












        }







        // Created by sominath for getting email message on date 22/11/2016
        public string GetMessage()
        {
            string Message = "";
            CompassEntities dbcontext = new CompassEntities();
            var ObjTblEmailConfigure = (from b in dbcontext.TblAlertMasters
                                        join c in dbcontext.TblEmailConfigures on b.AlertId equals c.AlertTableID
                                        where b.AlertId == 9
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

        // Created by sominath  on date 22/11/2016
        public List<tblProjectFieldDataMaster> GetListByProjectId(long ProjectID)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            List<tblProjectFieldDataMaster> list = new List<tblProjectFieldDataMaster>();

            list = objCompassEntities.tblProjectFieldDataMasters.Where(x => x.TblProjectFieldData_Id == ProjectID && x.Active == 1).ToList();

            return list.ToList();
        }
        // Created by sominath  on date 22/11/2016
        public List<TblProjectFieldData> GetListByDataID(long uploadDataId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            List<TblProjectFieldData> list = new List<TblProjectFieldData>();
            list = objCompassEntities.TblProjectFieldDatas.Where(o => o.ID == uploadDataId && o.Active == 1).ToList();

            return list.ToList();

        }
        // Created by sominath  on date 22/11/2016
        public List<tblFormSectionRelation> GetSectionFieldRel(long fromId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            List<tblFormSectionRelation> list = new List<tblFormSectionRelation>();
            list = objCompassEntities.tblFormSectionRelations.Where(x => x.FormID == fromId && x.Active == 1).ToList();

            return list.ToList();


        }
        // Created by sominath  on date 22/11/2016
        public List<String> CCEmailTo()
        {
            List<String> List = new List<String>();
            CompassEntities db = new CompassEntities();
            var CCEmailId = (from b in db.TblAlertMasters
                             join c in db.TblEmailConfigures on b.AlertId equals c.AlertTableID
                             join d in db.TblEmailConfigureUserDetails on c.ID equals d.TblEmailConfigureID
                             where b.AlertId == 9
                             //select new TblEmailConfigureUserDetail
                             //{
                             //    UserID = d.UserID
                             //}
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
        // Created by sominath  on date 22/11/2016
        public string GetSubject()
        {
            string subject = "";
            CompassEntities dbcontext = new CompassEntities();
            var ObjTblEmailConfigure = (from b in dbcontext.TblAlertMasters
                                        join c in dbcontext.TblEmailConfigures on b.AlertId equals c.AlertTableID
                                        where b.AlertId == 9
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

        /// <summary>
        /// Date 2016-10-06 Devloper Bharat Magdum
        /// method to update media upload complete flag  submitted from mobile app.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="formId"></param>
        /// <param name="initialId"></param>
        /// <returns></returns>
        public bool updateMediaFlag(long userId, long formId, long initialId)
        {
            try
            {
                bool a = false;

                CompassEntities objCompassEntities = new CompassEntities();

                var objFormDetails = objCompassEntities.tblFormMasters.Where(o => o.FormId == formId).FirstOrDefault();

                if (objFormDetails.FormTypeID == 2)
                {
                    var objRecord = objCompassEntities.TblProjectFieldDatas.Where(o => o.Active == 1 && o.FormId == formId && o.FK_UploadedExcelData_Id == initialId).FirstOrDefault();

                    if (objRecord != null)
                    {
                        objRecord.IsMediaPending = 0;
                        objCompassEntities.SaveChanges();
                        a = true;
                    }
                }
                else if (objFormDetails.FormTypeID == 1)
                {
                    var objRecord = objCompassEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == formId && o.FK_UploadedExcelData_Id == initialId).FirstOrDefault();

                    if (objRecord != null)
                    {
                        objRecord.IsMediaPending = 0;
                        objCompassEntities.SaveChanges();
                        a = true;
                    }
                }


                return a;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public List<ProcessAccess> checkProcessAccess(long userId, DateTime currentDate)
        {
            try
            {
                List<ProcessAccess> objList = new List<ProcessAccess>();

                CompassEntities objCompassEntities = new CompassEntities();

                var getrecord = objCompassEntities.PROC_WebService_LogIn_Process(userId, 1, currentDate).FirstOrDefault();

                if (getrecord != null && getrecord > 0)
                {
                    objList.Add(new ProcessAccess { processId = 1, processName = "Installation" });
                }

                getrecord = objCompassEntities.PROC_WebService_LogIn_Process(userId, 2, currentDate).FirstOrDefault();

                if (getrecord != null && getrecord > 0)
                {
                    objList.Add(new ProcessAccess { processId = 2, processName = "Audit" });
                }


                return objList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }



    public static class StringExtensions
    {
        public static IEnumerable<string> GetMostCommonSubstrings(this IList<string> strings)
        {
            if (strings == null)
                return Enumerable.Empty<string>();
            if (!strings.Any() || strings.Any(s => string.IsNullOrEmpty(s)))
                return Enumerable.Empty<string>();

            var allSubstrings = new List<List<string>>();
            for (int i = 0; i < strings.Count; i++)
            {
                var substrings = new List<string>();
                string str = strings[i];
                for (int c = 0; c < str.Length - 1; c++)
                {
                    for (int cc = 1; c + cc <= str.Length; cc++)
                    {
                        string substr = str.Substring(c, cc);
                        if (allSubstrings.Count < 1 || allSubstrings.Last().Contains(substr))
                            substrings.Add(substr);
                    }
                }
                allSubstrings.Add(substrings);
            }
            if (allSubstrings.Last().Any())
            {
                var mostCommon = allSubstrings.Last()
                    .GroupBy(str => str)
                    .OrderByDescending(g => g.Key.Length)
                    .ThenByDescending(g => g.Count())
                    .Select(g => g.Key);
                return mostCommon;
            }
            return Enumerable.Empty<string>();
        }
    }





}
