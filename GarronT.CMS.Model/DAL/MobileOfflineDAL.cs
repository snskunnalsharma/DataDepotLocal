using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace GarronT.CMS.Model.DAL
{
    public class MobileOfflineDAL : IDisposable
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

        public List<MobileOffline> getUserVisitData(long userId, DateTime strCurrentDate)
        {
            List<MobileOffline> objListMobileOffline = new List<MobileOffline>();

            CompassEntities objCompassEntities = new CompassEntities();

            var ProjectVisitData = objCompassEntities.STP_OfflineDataSP(userId, strCurrentDate).ToList();
            var FormDataList = objCompassEntities.STP_OfflineDataSP1(userId, strCurrentDate).ToList();

            var ImageList = objCompassEntities.tblPictures.ToList();
            if (ProjectVisitData != null)
            {
                var distPro = ProjectVisitData.Select(o => new { o.ProjectId, o.ProjectName }).Distinct().ToList();

                foreach (var item in distPro)
                {

                    var projectFormData = FormDataList.Where(o => o.ProjectID == item.ProjectId).ToList();
                    var projectDistinctFormList = projectFormData.Select(o => new { o.FormId, o.FormName }).Distinct().ToList();
                    var projectDistinctSectionList = projectFormData.Select(o =>
                        new { o.FormId, o.FormName, o.SectionId, o.SectionName, o.SortOrderNumber }).Distinct().OrderBy(o => o.SortOrderNumber).ToList();

                    var resNewMeterSizeList = objCompassEntities.STP_OfflineDataSP2(item.ProjectId, "tblMeterSize").ToList();
                    var resNewMeterTypeList = objCompassEntities.STP_OfflineDataSP3(item.ProjectId, "tblMeterType").ToList();
                    var resServiceList = objCompassEntities.STP_OfflineDataSP4(item.ProjectId, "tblService").ToList();

                    var checkMappedColumnsToExcel = objCompassEntities.SP_GetFieldInfoForProject(item.ProjectId).ToList();

                    MobileOffline objMobileOffline = new MobileOffline();
                    objMobileOffline.projectId = item.ProjectId;
                    objMobileOffline.projectName = item.ProjectName;
                    objMobileOffline.DataSyncConnection = 1;
                    objMobileOffline.AutoDataSyncInterval = 5;

                    #region MenuVisit
                    List<MenuVisits> objMenuVisitsList = new List<MenuVisits>();
                    var currentProData = ProjectVisitData.Where(o => o.ProjectId == objMobileOffline.projectId).ToList();

                    foreach (var item1 in currentProData)
                    {
                        var getStreetNameData = currentProData.Where(o => o.Street == item1.Street).ToList();

                        MenuVisits objMenuVisits = new MenuVisits();
                        objMenuVisits.latitude = Convert.ToString(item1.Latitude);
                        objMenuVisits.longitude = Convert.ToString(item1.Longitude);
                        objMenuVisits.pendingVisits = 0;
                        objMenuVisits.completedVisits = 0;
                        objMenuVisits.skippedVisits = 0;
                        objMenuVisits.totalVisits = 0;
                        objMenuVisits.colorFlag = 0;
                        objMenuVisits.street = item1.Street;
                        objMenuVisits.visitId = item1.VisitId;
                        objMenuVisits.groupId = item1.GroupId;
                        List<Accounts> objListAccounts = new List<Accounts>();
                        foreach (var item2 in getStreetNameData)
                        {
                            #region Account
                            Accounts objAccounts = new Accounts();
                            objAccounts.accountNumber = item2.Account;
                            objAccounts.isVisited = false;
                            objAccounts.oldMeterNumber = item2.OldMeterNo;
                            objAccounts.oldMeterSize = item2.OldMeterSize;
                            objAccounts.oldMeterType = item2.OldMeterType;
                            objAccounts.uploadId = item2.UploadId;

                            // objAccounts.visitDateTime=item2.
                            List<Forms> objListForms = new List<Forms>();

                            foreach (var item3 in projectDistinctFormList)
                            {
                                #region Form
                                Forms objForms = new Forms();
                                objForms.formId = item3.FormId;
                                objForms.formName = item3.FormName;
                                objForms.isSkipped = false;
                                objForms.isVisited = false;
                                //objForms.visitDateTime

                                List<Section> objListSection = new List<Section>();

                                foreach (var item4 in projectDistinctSectionList)
                                {
                                    var sectionFields = projectFormData.Where(o => o.SectionId == item4.SectionId).Select(o => o).OrderBy(o => o.FieldOrder).ToList();
                                    Section objSection = new Section();
                                    objSection.sectionId = item4.SectionId;
                                    objSection.sectionName = item4.SectionName;
                                    objSection.sectionOrder = item4.SortOrderNumber;

                                    List<Fields> objListFields = new List<Fields>();

                                    foreach (var item5 in sectionFields)
                                    {
                                        #region
                                        var aCurrentField = projectFormData.Where(o => o.FieldId == item5.FieldId).FirstOrDefault();
                                        Fields objField = new Fields();
                                        objField.acknowlegementButtonText = aCurrentField.AcknowlegementButtonText;
                                        objField.acknowlegementClickedButtonText = aCurrentField.AcknowlegementClickedButtonText;
                                        objField.acknowlegementText = aCurrentField.AcknowlegementText;
                                        objField.allowAnnotation = aCurrentField.AllowAnnotation != null ? bool.Parse(aCurrentField.AllowAnnotation.ToString()) : false;
                                        objField.allowMultiSelection = aCurrentField.AllowMultiSelection != null ? bool.Parse(aCurrentField.AllowMultiSelection.ToString()) : false;
                                        objField.allowNotApplicable = aCurrentField.AllowNotApplicable != null ? bool.Parse(aCurrentField.AllowNotApplicable.ToString()) : false;
                                        objField.allowprii = aCurrentField.allowprii != null ? bool.Parse(aCurrentField.allowprii.ToString()) : false;
                                        objField.audioQuality = aCurrentField.AudioQuality;
                                        objField.cameraEnabled = aCurrentField.CameraEnabled != null ? bool.Parse(aCurrentField.CameraEnabled.ToString()) : false;
                                        objField.captureGeolocation = aCurrentField.CaptureGeolocation != null ? bool.Parse(aCurrentField.CaptureGeolocation.ToString()) : false;
                                        objField.captureMode = aCurrentField.CaptureMode;
                                        objField.captureTimestamp = aCurrentField.CaptureTimestamp != null ? bool.Parse(aCurrentField.CaptureTimestamp.ToString()) : false;
                                        objField.decimalPositions = aCurrentField.DecimalPositions;
                                        objField.defaultValue = aCurrentField.DefaultValue;
                                        objField.hideFieldLabel = aCurrentField.HideFieldLabel != null ? bool.Parse(aCurrentField.HideFieldLabel.ToString()) : false;
                                        objField.displayMask = aCurrentField.DisplayMask;
                                        objField.enforceMinMax = aCurrentField.EnforceMinMax != null ? bool.Parse(aCurrentField.EnforceMinMax.ToString()) : false;
                                        objField.excludeonSync = aCurrentField.ExcludeonSync != null ? bool.Parse(aCurrentField.ExcludeonSync.ToString()) : false;



                                        objField.fieldDataTypeName = aCurrentField.FieldDataTypeName;
                                        objField.fieldFilterkey = aCurrentField.FieldFilterkey;
                                        objField.fieldId = aCurrentField.FieldId;
                                        objField.fieldLabel = aCurrentField.FieldLabel;
                                        objField.fieldName = aCurrentField.FieldName;
                                        objField.fieldOrder = aCurrentField.FieldOrder;

                                        objField.formatMask = aCurrentField.FormatMask;
                                        objField.gpsTagging = aCurrentField.GPSTagging != null ? bool.Parse(aCurrentField.GPSTagging.ToString()) : false;
                                        objField.hasAlert = aCurrentField.HasAlert != null ? bool.Parse(aCurrentField.HasAlert.ToString()) : false;
                                        objField.hasFile = aCurrentField.HasFile != null ? bool.Parse(aCurrentField.HasFile.ToString()) : false;
                                        objField.hasValues = aCurrentField.HasValues != null ? bool.Parse(aCurrentField.HasValues.ToString()) : false;
                                        objField.hintText = aCurrentField.HintText;
                                        objField.isEnabled = aCurrentField.IsEnabled != null ? bool.Parse(aCurrentField.IsEnabled.ToString()) : false;
                                        objField.isRequired = aCurrentField.IsRequired != null ? bool.Parse(aCurrentField.IsRequired.ToString()) : false;
                                        objField.keyboardType = aCurrentField.KeyboardType != null ? bool.Parse(aCurrentField.KeyboardType.ToString()) : false;
                                        if (!string.IsNullOrEmpty(aCurrentField.MaximumHeight))
                                        {
                                            objField.maximumHeight = int.Parse(aCurrentField.MaximumHeight);
                                        }
                                        if (!string.IsNullOrEmpty(aCurrentField.MaximumWidth))
                                        {
                                            objField.maximumWidth = int.Parse(aCurrentField.MaximumWidth);
                                        }
                                        objField.maxLengthValue = !string.IsNullOrEmpty(aCurrentField.MaxLengthValue) ? int.Parse(aCurrentField.MaxLengthValue) : 250;
                                        objField.minLengthValue = !string.IsNullOrEmpty(aCurrentField.MinLengthValue) ? int.Parse(aCurrentField.MinLengthValue) : 250;

                                        if (!string.IsNullOrEmpty(aCurrentField.NumberOfColumnsForPhones))
                                        {
                                            objField.numberOfColumnsForPhones = int.Parse(aCurrentField.NumberOfColumnsForPhones);
                                        }
                                        if (!string.IsNullOrEmpty(aCurrentField.NumberOfColumnsForTablets))
                                        {
                                            objField.numberOfColumnsForTablets = int.Parse(aCurrentField.NumberOfColumnsForTablets);
                                        }
                                        objField.photoFile = aCurrentField.PhotoFile != null ? bool.Parse(aCurrentField.PhotoFile.ToString()) : false;
                                        objField.photoLibraryEnabled = aCurrentField.PhotoLibraryEnabled != null ? bool.Parse(aCurrentField.PhotoLibraryEnabled.ToString()) : false;
                                        objField.photoQuality = aCurrentField.PhotoQuality;


                                        objField.ratingLabels = aCurrentField.RatingLabels != null ? bool.Parse(aCurrentField.RatingLabels.ToString()) : false;
                                        objField.ratingMax = aCurrentField.RatingMax != null ? bool.Parse(aCurrentField.RatingMax.ToString()) : false;
                                        objField.sectionId = aCurrentField.SectionId;
                                        objField.secure = aCurrentField.Secure != null ? bool.Parse(aCurrentField.Secure.ToString()) : false;
                                        objField.showMultiselect = aCurrentField.ShowMultiselect != null ? bool.Parse(aCurrentField.ShowMultiselect.ToString()) : false;
                                        objField.showRatingLabels = aCurrentField.ShowRatingLabels != null ? bool.Parse(aCurrentField.ShowRatingLabels.ToString()) : false;
                                        if (!string.IsNullOrEmpty(aCurrentField.MaxrecordabletimeInSeconds))
                                        {
                                            objField.maxrecordabletimeInSeconds = int.Parse(aCurrentField.MaxrecordabletimeInSeconds);
                                        }
                                        objField.videoQuality = aCurrentField.VideoQuality;


                                        List<FieldInitialDataO> FieldInitialDataList = new List<FieldInitialDataO>();
                                        if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER SIZE".ToLower())
                                        {
                                            FieldInitialDataList = resNewMeterSizeList.Select(o => new FieldInitialDataO { initialFieldId = o.initialFieldId, initialFieldValue = o.initialFieldValue }).ToList();
                                        }
                                        else if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER TYPE".ToLower())
                                        {
                                            FieldInitialDataList = resNewMeterTypeList.Select(o => new FieldInitialDataO { initialFieldId = o.initialFieldId, initialFieldValue = o.initialFieldValue }).ToList();
                                        }
                                        else if (aCurrentField.FieldName.ToLower().Trim() == "SERVICES".ToLower())
                                        {

                                            FieldInitialDataList = resServiceList.Where(o => o.isAdditionalService == false).Select(o =>
                                                new FieldInitialDataO
                                                {
                                                    initialFieldId = o.initialFieldId,
                                                    initialFieldValue = o.initialFieldValue,
                                                    flagCaptureAudio = o.flagCaptureAudio,
                                                    flagCaptureVideo = o.flagCaptureVideo,
                                                    flagIsImageCaptureRequired = bool.Parse(o.flagIsImageCaptureRequired.ToString()),
                                                    isAdditionalService = bool.Parse(o.isAdditionalService.ToString()),
                                                    imageList = string.IsNullOrEmpty((o.imageList)) ? null : (o.imageList).Split(',').ToList()
                                                    .Select(o2 => new PictureList
                                                    {
                                                        imageId = (ImageList.Where(o1 => o1.Picture.Trim() == o2.Trim()).Select(o1 => o1.ID).FirstOrDefault()),
                                                        imageName = o2.Trim()
                                                    }).ToList()
                                                }).ToList();
                                        }
                                        else if (aCurrentField.FieldName.ToLower().Trim() == "ADDITIONAL SERVICES".ToLower())
                                        {

                                            FieldInitialDataList = resServiceList.Where(o => o.isAdditionalService == true).Select(o =>
                                                new FieldInitialDataO
                                                {
                                                    initialFieldId = o.initialFieldId,
                                                    initialFieldValue = o.initialFieldValue,
                                                    flagCaptureAudio = o.flagCaptureAudio,
                                                    flagCaptureVideo = o.flagCaptureVideo,
                                                    flagIsImageCaptureRequired = bool.Parse(o.flagIsImageCaptureRequired.ToString()),
                                                    isAdditionalService = bool.Parse(o.isAdditionalService.ToString()),
                                                    //imageList = string.IsNullOrEmpty((o.imageList)) ? "".Split(',').ToList() : (o.imageList).Split(',').ToList()
                                                    imageList = string.IsNullOrEmpty((o.imageList)) ? null : (o.imageList).Split(',').ToList()
                                                   .Select(o2 => new PictureList
                                                   {
                                                       imageId = (ImageList.Where(o1 => o1.Picture.Trim() == o2.Trim()).Select(o1 => o1.ID).FirstOrDefault()),
                                                       imageName = o2.Trim()
                                                   }).ToList()
                                                }).ToList();
                                        }
                                        else
                                        {
                                            var checkfield = checkMappedColumnsToExcel.Where(o => o.FieldName == aCurrentField.FieldName).FirstOrDefault();

                                            if (checkfield != null && checkfield.isMappedToExcel == 1 && checkfield.FieldName != "LOCATION/GPS")
                                            {
                                                FieldInitialDataO objFieldInitialData = new FieldInitialDataO();
                                                objFieldInitialData.initialFieldId = item2.UploadId;
                                                if (checkfield.FieldName.ToLower() == "account")
                                                {
                                                    objFieldInitialData.initialFieldValue = item2.Account;
                                                }
                                                else if (checkfield.FieldName.ToLower() == "STREET".ToLower())
                                                {
                                                    objFieldInitialData.initialFieldValue = item2.Street;
                                                }
                                                else if (checkfield.FieldName.ToLower() == "OLD METER NUMBER".ToLower())
                                                {
                                                    objFieldInitialData.initialFieldValue = item2.OldMeterNo;
                                                }
                                                else if (checkfield.FieldName.ToLower() == "OLD METER SIZE".ToLower())
                                                {
                                                    objFieldInitialData.initialFieldValue = item2.OldMeterSize;
                                                }
                                                else if (checkfield.FieldName.ToLower() == "OLD METER TYPE".ToLower())
                                                {
                                                    objFieldInitialData.initialFieldValue = item2.OldMeterType;
                                                }
                                                else if (checkfield.FieldName.ToLower() == "OLD METER RADIO NUMBER".ToLower())
                                                {
                                                    objFieldInitialData.initialFieldValue = item2.OldMeterRadioNo;
                                                }
                                                else if (checkfield.FieldName.ToLower() == "OLD METER LAST READING".ToLower())
                                                {
                                                    objFieldInitialData.initialFieldValue = item2.OldMeterLastReading;
                                                }
                                                else if (checkfield.FieldName.ToLower() == "CUSTOMER".ToLower())
                                                {
                                                    objFieldInitialData.initialFieldValue = item2.customer;
                                                }
                                                FieldInitialDataList.Add(objFieldInitialData);
                                            }
                                            else
                                            {
                                                var res = objCompassEntities.tblProjectFieldDDLMasters.Where(o => o.FormSectionFieldID == aCurrentField.FieldId && o.Active == 1).ToList();
                                                FieldInitialDataList = res.Select(o => new FieldInitialDataO { initialFieldId = o.ID, initialFieldValue = o.FieldValue }).ToList();
                                            }
                                        }



                                        objField.fieldInitialDataList = FieldInitialDataList;

                                        objListFields.Add(objField);

                                        #endregion
                                    }
                                    objSection.fieldList = objListFields;
                                    objListSection.Add(objSection);
                                }
                                objForms.sectionList = objListSection;
                                objListForms.Add(objForms);


                                #endregion
                            }
                            objAccounts.FormList = objListForms;
                            objListAccounts.Add(objAccounts);

                            #endregion
                        }

                        objMenuVisits.objAccounts = objListAccounts;
                        objMenuVisitsList.Add(objMenuVisits);
                    }
                    #endregion

                    #region Map Visit
                    List<MapVisits> objListMapVisits = new List<MapVisits>();

                    foreach (var item1 in currentProData)
                    {
                        var checkPresent = objListMapVisits.Where(o => o.latitude == Convert.ToString(item1.Latitude) &&
                            o.longitude == Convert.ToString(item1.Longitude)).FirstOrDefault();
                        if (checkPresent == null)
                        {
                            var allMatchingLatLonList = currentProData.Where(o => o.Latitude == item1.Latitude && o.Longitude == item1.Longitude).ToList();
                            if (allMatchingLatLonList.Count == 1)
                            {
                                MapVisits objMapVisits = new MapVisits();
                                objMapVisits.groupId = item1.GroupId;
                                //objMapVisits.visitId = 0;
                                objMapVisits.colorFlag = 1;
                                objMapVisits.street = item1.Street;
                                objMapVisits.latitude = Convert.ToString(item1.Latitude);
                                objMapVisits.longitude = Convert.ToString(item1.Longitude);
                                objMapVisits.completedVisits = 0;
                                objMapVisits.pendingVisits = 0;
                                objMapVisits.skippedVisits = 0;
                                objMapVisits.totalVisits = 0;
                                objListMapVisits.Add(objMapVisits);
                            }
                            else
                            {

                                var checkAddressList = allMatchingLatLonList.Select(o => o.Street).Distinct().ToList();

                                IEnumerable<string> s1 = StringExtensions.GetMostCommonSubstrings(checkAddressList);

                                string swd = s1.FirstOrDefault();

                                MapVisits objMapVisits = new MapVisits();
                                objMapVisits.groupId = item1.GroupId;
                                //objMapVisits.visitId = 0;
                                objMapVisits.colorFlag = 1;
                                objMapVisits.street = swd;
                                objMapVisits.latitude = Convert.ToString(item1.Latitude);
                                objMapVisits.longitude = Convert.ToString(item1.Longitude);
                                objMapVisits.completedVisits = 0;
                                objMapVisits.pendingVisits = 0;
                                objMapVisits.skippedVisits = 0;
                                objMapVisits.totalVisits = 0;
                                objListMapVisits.Add(objMapVisits);
                            }
                        }

                    }


                    #endregion



                    objMobileOffline.ObjMenuVisits = objMenuVisitsList;
                    objMobileOffline.ObjMapVisits = objListMapVisits;
                    objListMobileOffline.Add(objMobileOffline);
                }

            }



            return objListMobileOffline;
        }

        public MobileOfflineResult getUserVisitDataNewFormat(long userId, DateTime strCurrentDate)
        {

            MobileOfflineResult mobileOfflineResult = new MobileOfflineResult();

            List<MobileOffline1> objListMobileOffline = new List<MobileOffline1>();



            CompassEntities objCompassEntities = new CompassEntities();
            var ImageList = objCompassEntities.tblPictures.ToList();
            var skipList = objCompassEntities.tblSkipReasonMasters.Where(o => o.Active == 1).ToList();
            var ProjectVisitData = objCompassEntities.STP_OfflineDataSP(userId, strCurrentDate).ToList();
            ProjectVisitData = ProjectVisitData.Where(o => o.SynchWithMobileFlag != 1).Select(o => o).ToList();
            var FormDataList = objCompassEntities.STP_OfflineDataSP1(userId, strCurrentDate).ToList();

            if (ProjectVisitData != null)
            {
                var distPro = ProjectVisitData.Select(o => new { o.ProjectId, o.ProjectName }).Distinct().ToList();

                foreach (var item in distPro)
                {

                    var projectFormData = FormDataList.Where(o => o.ProjectID == item.ProjectId).ToList();
                    var projectDistinctFormList = projectFormData.Select(o => new { o.FormId, o.FormName }).Distinct().ToList();
                    var projectDistinctSectionList = projectFormData.Select(o =>
                        new { o.FormId, o.FormName, o.SectionId, o.SectionName, o.SortOrderNumber }).Distinct().OrderBy(o => o.SortOrderNumber).ToList();

                    var resNewMeterSizeList = objCompassEntities.STP_OfflineDataSP2(item.ProjectId, "tblMeterSize").ToList();
                    var resNewMeterTypeList = objCompassEntities.STP_OfflineDataSP3(item.ProjectId, "tblMeterType").ToList();
                    var resServiceList = objCompassEntities.STP_OfflineDataSP4(item.ProjectId, "tblService").ToList();

                    var checkMappedColumnsToExcel = objCompassEntities.SP_GetFieldInfoForProject(item.ProjectId).ToList();

                    MobileOffline1 objMobileOffline = new MobileOffline1();
                    objMobileOffline.projectId = item.ProjectId;
                    objMobileOffline.projectName = item.ProjectName;
                    objMobileOffline.dataSyncConnection = 1;
                    objMobileOffline.autoDataSyncInterval = 5;

                    #region Form Visits
                    List<Forms1> objListFormsNew = new List<Forms1>();

                    foreach (var item3 in projectDistinctFormList)
                    {
                        #region Form
                        Forms1 objForms = new Forms1();
                        objForms.formId = item3.FormId;
                        objForms.formName = item3.FormName;

                        List<Section> objListSection = new List<Section>();

                        foreach (var item4 in projectDistinctSectionList)
                        {
                            var sectionFields = projectFormData.Where(o => o.SectionId == item4.SectionId).Select(o => o).OrderBy(o => o.FieldOrder).ToList();
                            Section objSection = new Section();
                            objSection.sectionId = item4.SectionId;
                            objSection.sectionName = item4.SectionName;
                            objSection.sectionOrder = item4.SortOrderNumber;

                            List<Fields> objListFields = new List<Fields>();

                            foreach (var item5 in sectionFields)
                            {
                                #region
                                var aCurrentField = projectFormData.Where(o => o.FieldId == item5.FieldId).FirstOrDefault();
                                Fields objField = new Fields();
                                objField.acknowlegementButtonText = aCurrentField.AcknowlegementButtonText;
                                objField.acknowlegementClickedButtonText = aCurrentField.AcknowlegementClickedButtonText;
                                objField.acknowlegementText = aCurrentField.AcknowlegementText;
                                objField.allowAnnotation = aCurrentField.AllowAnnotation != null ? bool.Parse(aCurrentField.AllowAnnotation.ToString()) : false;
                                objField.allowMultiSelection = aCurrentField.AllowMultiSelection != null ? bool.Parse(aCurrentField.AllowMultiSelection.ToString()) : false;
                                objField.allowNotApplicable = aCurrentField.AllowNotApplicable != null ? bool.Parse(aCurrentField.AllowNotApplicable.ToString()) : false;
                                objField.allowprii = aCurrentField.allowprii != null ? bool.Parse(aCurrentField.allowprii.ToString()) : false;
                                objField.audioQuality = aCurrentField.AudioQuality;
                                objField.cameraEnabled = aCurrentField.CameraEnabled != null ? bool.Parse(aCurrentField.CameraEnabled.ToString()) : false;
                                objField.captureGeolocation = aCurrentField.CaptureGeolocation != null ? bool.Parse(aCurrentField.CaptureGeolocation.ToString()) : false;
                                objField.captureMode = aCurrentField.CaptureMode;
                                objField.captureTimestamp = aCurrentField.CaptureTimestamp != null ? bool.Parse(aCurrentField.CaptureTimestamp.ToString()) : false;
                                objField.decimalPositions = aCurrentField.DecimalPositions;
                                objField.defaultValue = aCurrentField.DefaultValue;
                                objField.hideFieldLabel = aCurrentField.HideFieldLabel != null ? bool.Parse(aCurrentField.HideFieldLabel.ToString()) : false;
                                objField.displayMask = aCurrentField.DisplayMask;
                                objField.enforceMinMax = aCurrentField.EnforceMinMax != null ? bool.Parse(aCurrentField.EnforceMinMax.ToString()) : false;
                                objField.excludeonSync = aCurrentField.ExcludeonSync != null ? bool.Parse(aCurrentField.ExcludeonSync.ToString()) : false;



                                objField.fieldDataTypeName = aCurrentField.FieldDataTypeName;
                                objField.fieldFilterkey = aCurrentField.FieldFilterkey;
                                objField.fieldId = aCurrentField.FieldId;
                                objField.fieldLabel = aCurrentField.FieldLabel;
                                objField.fieldName = aCurrentField.FieldName;
                                objField.fieldOrder = aCurrentField.FieldOrder;

                                objField.formatMask = aCurrentField.FormatMask;
                                objField.gpsTagging = aCurrentField.GPSTagging != null ? bool.Parse(aCurrentField.GPSTagging.ToString()) : false;
                                objField.hasAlert = aCurrentField.HasAlert != null ? bool.Parse(aCurrentField.HasAlert.ToString()) : false;
                                objField.hasFile = aCurrentField.HasFile != null ? bool.Parse(aCurrentField.HasFile.ToString()) : false;
                                objField.hasValues = aCurrentField.HasValues != null ? bool.Parse(aCurrentField.HasValues.ToString()) : false;
                                objField.hintText = aCurrentField.HintText;
                                objField.isEnabled = aCurrentField.IsEnabled != null ? bool.Parse(aCurrentField.IsEnabled.ToString()) : false;
                                objField.isRequired = aCurrentField.IsRequired != null ? bool.Parse(aCurrentField.IsRequired.ToString()) : false;
                                objField.keyboardType = aCurrentField.KeyboardType != null ? bool.Parse(aCurrentField.KeyboardType.ToString()) : false;
                                if (!string.IsNullOrEmpty(aCurrentField.MaximumHeight))
                                {
                                    objField.maximumHeight = int.Parse(aCurrentField.MaximumHeight);
                                }
                                if (!string.IsNullOrEmpty(aCurrentField.MaximumWidth))
                                {
                                    objField.maximumWidth = int.Parse(aCurrentField.MaximumWidth);
                                }
                                objField.maxLengthValue = !string.IsNullOrEmpty(aCurrentField.MaxLengthValue) ? int.Parse(aCurrentField.MaxLengthValue) : 250;
                                objField.minLengthValue = !string.IsNullOrEmpty(aCurrentField.MinLengthValue) ? int.Parse(aCurrentField.MinLengthValue) : 250;

                                if (!string.IsNullOrEmpty(aCurrentField.NumberOfColumnsForPhones))
                                {
                                    objField.numberOfColumnsForPhones = int.Parse(aCurrentField.NumberOfColumnsForPhones);
                                }
                                if (!string.IsNullOrEmpty(aCurrentField.NumberOfColumnsForTablets))
                                {
                                    objField.numberOfColumnsForTablets = int.Parse(aCurrentField.NumberOfColumnsForTablets);
                                }
                                objField.photoFile = aCurrentField.PhotoFile != null ? bool.Parse(aCurrentField.PhotoFile.ToString()) : false;
                                objField.photoLibraryEnabled = aCurrentField.PhotoLibraryEnabled != null ? bool.Parse(aCurrentField.PhotoLibraryEnabled.ToString()) : false;
                                objField.photoQuality = aCurrentField.PhotoQuality;


                                objField.ratingLabels = aCurrentField.RatingLabels != null ? bool.Parse(aCurrentField.RatingLabels.ToString()) : false;
                                objField.ratingMax = aCurrentField.RatingMax != null ? bool.Parse(aCurrentField.RatingMax.ToString()) : false;
                                objField.sectionId = aCurrentField.SectionId;
                                objField.secure = aCurrentField.Secure != null ? bool.Parse(aCurrentField.Secure.ToString()) : false;
                                objField.showMultiselect = aCurrentField.ShowMultiselect != null ? bool.Parse(aCurrentField.ShowMultiselect.ToString()) : false;
                                objField.showRatingLabels = aCurrentField.ShowRatingLabels != null ? bool.Parse(aCurrentField.ShowRatingLabels.ToString()) : false;
                                if (!string.IsNullOrEmpty(aCurrentField.MaxrecordabletimeInSeconds))
                                {
                                    objField.maxrecordabletimeInSeconds = int.Parse(aCurrentField.MaxrecordabletimeInSeconds);
                                }
                                objField.videoQuality = aCurrentField.VideoQuality;

                                List<FieldInitialDataO> FieldInitialDataList = new List<FieldInitialDataO>();
                                objField.fieldInitialDataList = FieldInitialDataList;

                                objListFields.Add(objField);

                                #endregion
                            }
                            objSection.fieldList = objListFields;
                            objListSection.Add(objSection);
                        }
                        objForms.sectionList = objListSection;
                        objListFormsNew.Add(objForms);


                        #endregion
                    }



                    #endregion

                    #region MenuVisit
                    List<MenuVisits1> objMenuVisitsList = new List<MenuVisits1>();
                    var currentProData = ProjectVisitData.Where(o => o.ProjectId == objMobileOffline.projectId).ToList();

                    foreach (var item1 in currentProData)
                    {

                        int streetCount = objMenuVisitsList.Where(o => o.street == item1.Street).Count();

                        if (streetCount == 0)
                        {

                            var getStreetNameData = currentProData.Where(o => o.Street == item1.Street).ToList();

                            MenuVisits1 objMenuVisits = new MenuVisits1();
                            //objMenuVisits.latitude = Convert.ToString(item1.Latitude);
                            //objMenuVisits.longitude = Convert.ToString(item1.Longitude);
                            objMenuVisits.pendingVisits = 0;
                            objMenuVisits.completedVisits = 0;
                            objMenuVisits.skippedVisits = 0;
                            objMenuVisits.totalVisits = 0;
                            objMenuVisits.colorFlag = 0;
                            objMenuVisits.street = item1.Street;
                            objMenuVisits.visitId = item1.VisitId;
                            objMenuVisits.groupId = item1.GroupId;
                            List<Accounts1> objListAccounts = new List<Accounts1>();
                            foreach (var item2 in getStreetNameData)
                            {
                                #region Account
                                Accounts1 objAccounts = new Accounts1();
                                objAccounts.accountNumber = item2.Account;
                                objAccounts.isVisited = false;
                                objAccounts.oldMeterNumber = item2.OldMeterNo;
                                objAccounts.oldMeterSize = item2.OldMeterSize;
                                objAccounts.oldMeterType = item2.OldMeterType;
                                objAccounts.uploadId = item2.UploadId;

                                // objAccounts.visitDateTime=item2.
                                List<Forms2> objListForms = new List<Forms2>();

                                foreach (var item3 in projectDistinctFormList)
                                {
                                    #region Form
                                    Forms2 objForms = new Forms2();
                                    objForms.formId = item3.FormId;
                                    objForms.formName = item3.FormName;
                                    objForms.isSkipped = false;
                                    objForms.isVisited = false;
                                    //objForms.visitDateTime

                                    List<Fields1> objListFields = new List<Fields1>();
                                    foreach (var item4 in projectDistinctSectionList)
                                    {
                                        var sectionFields = projectFormData.Where(o => o.SectionId == item4.SectionId).Select(o => o).OrderBy(o => o.FieldOrder).ToList();

                                        foreach (var item5 in sectionFields)
                                        {
                                            #region
                                            var aCurrentField = projectFormData.Where(o => o.FieldId == item5.FieldId).FirstOrDefault();
                                            Fields1 objField = new Fields1();
                                            objField.fieldId = item5.FieldId;
                                            objField.sectionId = item4.SectionId;
                                            objField.fieldName = item5.FieldName;

                                            List<FieldInitialDataO> FieldInitialDataList = new List<FieldInitialDataO>();

                                            if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER SIZE".ToLower())
                                            {
                                                FieldInitialDataList = resNewMeterSizeList.Select(o => new FieldInitialDataO { initialFieldId = o.initialFieldId, initialFieldValue = o.initialFieldValue }).ToList();
                                            }
                                            else if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER TYPE".ToLower())
                                            {
                                                FieldInitialDataList = resNewMeterTypeList.Select(o => new FieldInitialDataO { initialFieldId = o.initialFieldId, initialFieldValue = o.initialFieldValue }).ToList();
                                            }
                                            else if (aCurrentField.FieldName.ToLower().Trim() == "SERVICES".ToLower())
                                            {

                                                FieldInitialDataList = resServiceList.Where(o => o.isAdditionalService == false).Select(o =>
                                                    new FieldInitialDataO
                                                    {
                                                        initialFieldId = o.initialFieldId,
                                                        initialFieldValue = o.initialFieldValue,
                                                        flagCaptureAudio = o.flagCaptureAudio,
                                                        flagCaptureVideo = o.flagCaptureVideo,
                                                        flagIsImageCaptureRequired = bool.Parse(o.flagIsImageCaptureRequired.ToString()),
                                                        isAdditionalService = bool.Parse(o.isAdditionalService.ToString()),
                                                        //imageList = string.IsNullOrEmpty((o.imageList)) ? "".Split(',').ToList() : (o.imageList).Split(',').ToList()
                                                        imageList = string.IsNullOrEmpty((o.imageList)) ? null : (o.imageList).Split(',').ToList()
                                                       .Select(o2 => new PictureList
                                                       {
                                                           imageId = (ImageList.Where(o1 => o1.Picture.Trim() == o2.Trim()).Select(o1 => o1.ID).FirstOrDefault()),
                                                           imageName = o2.Trim()
                                                       }).ToList()
                                                    }).ToList();
                                            }
                                            else if (aCurrentField.FieldName.ToLower().Trim() == "ADDITIONAL SERVICES".ToLower())
                                            {

                                                FieldInitialDataList = resServiceList.Where(o => o.isAdditionalService == true).Select(o =>
                                                    new FieldInitialDataO
                                                    {
                                                        initialFieldId = o.initialFieldId,
                                                        initialFieldValue = o.initialFieldValue,
                                                        flagCaptureAudio = o.flagCaptureAudio,
                                                        flagCaptureVideo = o.flagCaptureVideo,
                                                        flagIsImageCaptureRequired = bool.Parse(o.flagIsImageCaptureRequired.ToString()),
                                                        isAdditionalService = bool.Parse(o.isAdditionalService.ToString()),
                                                        //imageList = string.IsNullOrEmpty((o.imageList)) ? "".Split(',').ToList() : (o.imageList).Split(',').ToList()
                                                        imageList = string.IsNullOrEmpty((o.imageList)) ? null : (o.imageList).Split(',').ToList()
                                                       .Select(o2 => new PictureList
                                                       {
                                                           imageId = (ImageList.Where(o1 => o1.Picture.Trim() == o2.Trim()).Select(o1 => o1.ID).FirstOrDefault()),
                                                           imageName = o2.Trim()
                                                       }).ToList()
                                                    }).ToList();
                                            }
                                            else
                                            {
                                                var checkfield = checkMappedColumnsToExcel.Where(o => o.FieldName == aCurrentField.FieldName).FirstOrDefault();

                                                if (checkfield != null && checkfield.isMappedToExcel == 1 && checkfield.FieldName != "LOCATION/GPS")
                                                {
                                                    FieldInitialDataO objFieldInitialData = new FieldInitialDataO();

                                                    objFieldInitialData.initialFieldId = item5.FieldId;
                                                    if (checkfield.FieldName.ToLower() == "account")
                                                    {
                                                        objFieldInitialData.initialFieldValue = item2.Street;
                                                    }
                                                    else if (checkfield.FieldName.ToLower() == "STREET".ToLower())
                                                    {
                                                        objFieldInitialData.initialFieldValue = item2.Street;
                                                    }
                                                    else if (checkfield.FieldName.ToLower() == "OLD METER NUMBER".ToLower())
                                                    {
                                                        objFieldInitialData.initialFieldValue = item2.OldMeterNo;
                                                    }
                                                    else if (checkfield.FieldName.ToLower() == "OLD METER SIZE".ToLower())
                                                    {
                                                        objFieldInitialData.initialFieldValue = item2.OldMeterSize;
                                                    }
                                                    else if (checkfield.FieldName.ToLower() == "OLD METER TYPE".ToLower())
                                                    {
                                                        objFieldInitialData.initialFieldValue = item2.OldMeterType;
                                                    }
                                                    else if (checkfield.FieldName.ToLower() == "OLD METER RADIO NUMBER".ToLower())
                                                    {
                                                        objFieldInitialData.initialFieldValue = item2.OldMeterRadioNo;
                                                    }
                                                    else if (checkfield.FieldName.ToLower() == "OLD METER LAST READING".ToLower())
                                                    {
                                                        objFieldInitialData.initialFieldValue = item2.OldMeterLastReading;
                                                    }
                                                    else if (checkfield.FieldName.ToLower() == "CUSTOMER".ToLower())
                                                    {
                                                        objFieldInitialData.initialFieldValue = item2.customer;
                                                    }

                                                    FieldInitialDataList.Add(objFieldInitialData);
                                                }
                                                else
                                                {
                                                    var res = objCompassEntities.tblProjectFieldDDLMasters.
                                                        Where(o => o.FormSectionFieldID == aCurrentField.FieldId && o.Active == 1).ToList();
                                                    FieldInitialDataList = res.Select(o => new FieldInitialDataO { initialFieldId = o.ID, initialFieldValue = o.FieldValue }).ToList();
                                                }

                                            }


                                            objField.fieldInitialDataList = FieldInitialDataList;

                                            objListFields.Add(objField);

                                            #endregion
                                        }
                                    }

                                    objForms.fieldList = objListFields;
                                    objListForms.Add(objForms);


                                    #endregion
                                }
                                objAccounts.formList = objListForms;
                                objListAccounts.Add(objAccounts);

                                #endregion
                            }

                            objMenuVisits.objAccounts = objListAccounts;
                            objMenuVisitsList.Add(objMenuVisits);
                        }
                    }
                    #endregion

                    #region Map Visit
                    List<MapVisits> objListMapVisits = new List<MapVisits>();

                    foreach (var item1 in currentProData)
                    {
                        var checkPresent = objListMapVisits.Where(o => o.latitude == Convert.ToString(item1.Latitude) &&
                            o.longitude == Convert.ToString(item1.Longitude)).FirstOrDefault();
                        if (checkPresent == null)
                        {
                            var allMatchingLatLonList = currentProData.Where(o => o.Latitude == item1.Latitude && o.Longitude == item1.Longitude).ToList();
                            if (allMatchingLatLonList.Count == 1)
                            {
                                MapVisits objMapVisits = new MapVisits();
                                objMapVisits.groupId = item1.GroupId;
                                objMapVisits.colorFlag = 1;
                                objMapVisits.street = item1.Street;
                                objMapVisits.latitude = Convert.ToString(item1.Latitude);
                                objMapVisits.longitude = Convert.ToString(item1.Longitude);
                                objMapVisits.completedVisits = 0;
                                objMapVisits.pendingVisits = 0;
                                objMapVisits.skippedVisits = 0;
                                objMapVisits.totalVisits = 0;
                                objListMapVisits.Add(objMapVisits);
                            }
                            else
                            {

                                var checkAddressList = allMatchingLatLonList.Select(o => o.Street).Distinct().ToList();

                                IEnumerable<string> s1 = StringExtensions.GetMostCommonSubstrings(checkAddressList);

                                string swd = s1.FirstOrDefault();

                                MapVisits objMapVisits = new MapVisits();
                                objMapVisits.groupId = item1.GroupId;
                                objMapVisits.colorFlag = 1;
                                objMapVisits.street = swd;
                                objMapVisits.latitude = Convert.ToString(item1.Latitude);
                                objMapVisits.longitude = Convert.ToString(item1.Longitude);
                                objMapVisits.completedVisits = 0;
                                objMapVisits.pendingVisits = 0;
                                objMapVisits.skippedVisits = 0;
                                objMapVisits.totalVisits = 0;
                                objListMapVisits.Add(objMapVisits);
                            }
                        }

                    }


                    #endregion


                    objMobileOffline.objFormStructure = objListFormsNew;
                    objMobileOffline.objMenuVisits = objMenuVisitsList;
                    objMobileOffline.objMapVisits = objListMapVisits;
                    objMobileOffline.skipReasonList = skipList.Select(o => new SkippReason { skipId = o.SkipId, skipReason = o.SkipReason }).ToList();
                    objListMobileOffline.Add(objMobileOffline);
                }

            }




            mobileOfflineResult.data = objListMobileOffline;
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            s.Serialize(mobileOfflineResult.data);
            string m = GetMd5Sum(s.ToString());
            mobileOfflineResult.checkSum = m;
            return mobileOfflineResult;
        }

        public bool setLastSynch(long userId, int batchNumber)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            var objInstallerMapList = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId
                                    && o.BatchNumber == batchNumber && o.SynchWithMobileFlag == 1 && o.MobileResponse != 1).ToList();

            foreach (var instItemList in objInstallerMapList)
            {
                instItemList.MobileResponse = 1;
                instItemList.MobileResponseDate = new CommonFunctions().ServerDate();
                objCompassEntities.SaveChanges();
            }

            var userLastVisit = objCompassEntities.tblInstallerSynchHistories.Where(o => o.UserId == userId).FirstOrDefault();

            if (userLastVisit == null)
            {
                userLastVisit = new tblInstallerSynchHistory();
                userLastVisit.Active = 1;
                userLastVisit.UserId = userId;
                userLastVisit.LastVisitDate = new CommonFunctions().ServerDate();
                objCompassEntities.tblInstallerSynchHistories.Add(userLastVisit);
                objCompassEntities.SaveChanges();
            }
            else
            {
                userLastVisit.Active = 1;
                userLastVisit.LastVisitDate = new CommonFunctions().ServerDate();
                objCompassEntities.SaveChanges();
            }

            return true;
        }


        public bool setLastSynchAudit(long userId, int batchNumber)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var objAuditMapList = objCompassEntities.ProjectAuditAllocations.Where(o => o.FK_AuditorId == userId
                                    && o.BatchNumber == batchNumber && o.SynchWithMobileFlag == 1 && o.MobileResponse != 1).ToList();

            foreach (var instItemList in objAuditMapList)
            {
                instItemList.MobileResponse = 1;
                instItemList.MobileResponseDate = new CommonFunctions().ServerDate();
                objCompassEntities.SaveChanges();
            }

            //var userLastVisit = objCompassEntities.tblInstallerSynchHistories.Where(o => o.UserId == userId).FirstOrDefault();

            //if (userLastVisit == null)
            //{
            //    userLastVisit = new tblInstallerSynchHistory();
            //    userLastVisit.Active = 1;
            //    userLastVisit.UserId = userId;
            //    userLastVisit.LastVisitDate = new CommonFunctions().ServerDate();
            //    objCompassEntities.tblInstallerSynchHistories.Add(userLastVisit);
            //    objCompassEntities.SaveChanges();
            //}
            //else
            //{
            //    userLastVisit.Active = 1;
            //    userLastVisit.LastVisitDate = new CommonFunctions().ServerDate();
            //    objCompassEntities.SaveChanges();
            //}

            return true;
        }



        // Create an md5 sum string of this string
        public string GetMd5Sum(string str)
        {
            //// First we need to convert the string into bytes, which
            //// means using a text encoder.
            //Encoder enc = System.Text.Encoding.Unicode.GetEncoder();
            HashAlgorithm algorithm = MD5.Create();
            Encoding encoding = new UTF8Encoding();
            var sb = new StringBuilder();
            foreach (var element in algorithm.ComputeHash(encoding.GetBytes(str)))
            {
                sb.Append(element.ToString("x2"));
            }
            return sb.ToString();
        }

        public MobileOfflineResult1 getOfflineData(long userId, DateTime strCurrentDate, long batchNumber)
        {

            MobileOfflineResult1 mobileOfflineResult = new MobileOfflineResult1();

            List<newMobileOffline1> objListMobileOffline = new List<newMobileOffline1>();


            long BatchNumber = 0;
            bool sendAlreadySentRecord = false;
            bool checkRecordAvilableForNextTrip = false;
            CompassEntities objCompassEntities = new CompassEntities();
            //var userLastVisitDetail = objCompassEntities.tblInstallerSynchHistories.Where(o => o.UserId == userId).FirstOrDefault();

            bool WrongBatchNumber = false;

            #region Batch number logic
            //send all data avilable 
            if (batchNumber == 0)
            {
                BatchNumber = 0;

                // send the incremental data for the installer

                //first check if any data is in queue to synch with mobile
                var checkB = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId
                    && o.BatchNumber != null).Select(o => o.BatchNumber).FirstOrDefault();

                if (checkB == null)
                {
                    //if no data found for condition - data sent from server but no response updated from mobile for the installer.                    
                    //send fresh new batch data                    
                    BatchNumber = 1;
                }
                else
                {


                    //checkB = objCompassEntities.tblInstallerMapDatas.Where(o => o.SynchWithMobileFlag == 1 && o.MobileResponse != 1 && o.InstallerId == userId
                    //    && o.Active == 1).Select(o => o.BatchNumber).FirstOrDefault();

                    var ProVisitData = objCompassEntities.STP_OfflineDataSP(userId, strCurrentDate).ToList();
                    checkB = ProVisitData.Where(o => o.SynchWithMobileFlag == 1 && o.MobileResponse != 1).
                        Select(o => o.BatchNumber).FirstOrDefault();
                    if (checkB != null)
                    {
                        // if data is present which is already sent to mobile for the installer but don't got any response 
                        // then resend this data to mobile for the installer
                        sendAlreadySentRecord = true;
                        BatchNumber = long.Parse(checkB.ToString());

                    }
                    else
                    {
                        // send fresh new batch data
                        checkB = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId).Select(o => o.BatchNumber).Max();
                        BatchNumber = long.Parse(checkB.ToString()) + 1;
                    }
                }


            }
            else
            {
                if (batchNumber > 1)
                {
                    /// Update mobile response from batchnumber  
                    /// send fresh new batch data



                    var objInstallerMapList = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId
                        && o.BatchNumber == (batchNumber - 1) && o.SynchWithMobileFlag == 1 && o.MobileResponse != 1).ToList();

                    if (objInstallerMapList != null && objInstallerMapList.Count > 0)
                    {
                        foreach (var instItemList in objInstallerMapList)
                        {
                            Nullable<int> MR = null;
                            // instItemList.MobileResponse = instItemList.Active == 1 ? 1 : MR;
                            instItemList.MobileResponse = 1;
                            instItemList.MobileResponseDate = new CommonFunctions().ServerDate();
                            objCompassEntities.SaveChanges();
                        }
                        BatchNumber = batchNumber;
                    }
                    else
                    {
                        objInstallerMapList = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId
                        && o.BatchNumber == batchNumber && o.SynchWithMobileFlag == 1 && o.MobileResponse != 1).ToList();

                        if (objInstallerMapList != null && objInstallerMapList.Count > 0)
                        {
                            sendAlreadySentRecord = true;
                            BatchNumber = batchNumber;
                        }
                        else
                        {
                            var checkB = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId
                           && o.Active == 1).Select(o => o.BatchNumber).Max();

                            if ((checkB + 1) != batchNumber)
                            {
                                WrongBatchNumber = true;
                            }
                            else
                            {
                                BatchNumber = long.Parse(checkB.ToString()) + 1;
                            }
                        }
                    }
                }
                else
                {
                    // send all avilable data 

                    var checkB = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId
                       && o.Active == 1).Select(o => o.BatchNumber).Max();

                    // if batch no > 1 means already send data present for the installer

                    if (checkB >= 1)
                    {
                        // First update the old resend data flag for the current time period.                    

                        var ProVisitData = objCompassEntities.STP_OfflineDataSP(userId, strCurrentDate).ToList();

                        // CompassEntities objCompassEntities1 = new CompassEntities();
                        foreach (var item in ProVisitData)
                        {
                            var objInstallerMap = objCompassEntities.tblInstallerMapDatas.Where(o => o.tblUploadedData_ID == item.UploadId &&
                                o.InstallerId == userId).FirstOrDefault();
                            if (objInstallerMap != null)
                            {
                                objInstallerMap.BatchNumber = null;
                                objInstallerMap.SynchCheckSumString = null;
                                objInstallerMap.SynchWithMobileFlag = null;
                                objInstallerMap.SynchWithMobileDate = null;
                                objInstallerMap.MobileResponse = null;
                                objInstallerMap.MobileResponseDate = null;
                                objCompassEntities.SaveChanges();
                            }
                        }


                        // get the new batchnumber for the installer 
                        checkB = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId && o.Active == 1).Select(o => o.BatchNumber).Max();
                        BatchNumber = checkB != null ? long.Parse(checkB.ToString()) + 1 : 1;
                    }
                    else
                    {
                        BatchNumber = 1;
                    }

                }



            }
            #endregion

            if (!WrongBatchNumber)
            {
                #region  send records
                var skipReasonTable = objCompassEntities.tblSkipReasonMasters.Where(o => o.Active == 1).ToList();
                var ImageList = objCompassEntities.tblPictures.ToList();
                var skipList = skipReasonTable.Where(o => o.CategoryType.ToLower() == "skip").ToList();
                var rtuList = skipReasonTable.Where(o => o.CategoryType.ToLower() == "rtu").ToList();
                var ProjectVisitData = objCompassEntities.STP_OfflineDataSP(userId, strCurrentDate).ToList();
                var projectCountLogic = objCompassEntities.STP_OfflineCountSP(userId, strCurrentDate).ToList();






                List<STP_OfflineDataSP_Result> ProjectVisitDataChunk = new List<STP_OfflineDataSP_Result>();
                int totalCount = ProjectVisitData.Where(o => (o.Active == 1 || o.Active == 0) && o.MobileResponse == null).Count();
                int chunkRecordCount = ProjectVisitData.Where(o => (o.Active == 1 || o.Active == 0) && o.MobileResponse == null && o.Reopen == 0).Select(o => o).ToList().Count();
                //int currentRecords = 0;
                if (sendAlreadySentRecord == true)
                {
                    //totalCount = ProjectVisitData.Where(o => o.MobileResponse != 1).Count();
                    ProjectVisitData = ProjectVisitData.Where(o => o.BatchNumber == BatchNumber).ToList();
                    ProjectVisitDataChunk = ProjectVisitData.Where(o => o.MobileResponse != 1).Select(o => o).Take(50).ToList();
                }
                else
                {
                    //totalCount = ProjectVisitData.Where(o => (o.SynchWithMobileFlag != 1 && o.Active == 1)
                    //    //|| (o.SynchWithMobileFlag == 1 && o.MobileResponse != 1 && o.Active == 1)
                    //    || (o.SynchWithMobileFlag == 1 && o.MobileResponse == null && o.Active == 0)).Count();

                    //ProjectVisitDataChunk = ProjectVisitData.Where(o => o.SynchWithMobileFlag != 1
                    //    || (o.SynchWithMobileFlag == 1 && o.MobileResponse == null && o.Active == 0)).Select(o => o).Take(50).ToList();
                    ProjectVisitDataChunk = ProjectVisitData.Where(o => (o.Active == 1 || o.Active == 0) && o.MobileResponse == null).Select(o => o).Take(50).ToList();
                }


                checkRecordAvilableForNextTrip = (ProjectVisitDataChunk == null || ProjectVisitDataChunk.Count == 0) ? false : ((totalCount - 50) > 0 ? true : false);

                var FormDataList = objCompassEntities.STP_OfflineDataSP1(userId, strCurrentDate).ToList();

                if (ProjectVisitData != null)
                {
                    var distPro = ProjectVisitDataChunk.Select(o => new { o.ProjectId, o.ProjectName, o.AutoDataSyncInterval, o.DataSyncConnection }).Distinct().ToList();


                    foreach (var item in distPro)
                    {
                        var distCountPro = projectCountLogic.Where(o => o.ProjectId == item.ProjectId).ToList();
                        var projectFormData = FormDataList.Where(o => o.ProjectID == item.ProjectId).ToList();
                        var projectDistinctFormList = projectFormData.Select(o => new { o.FormId, o.FormName }).Distinct().ToList();
                        var projectDistinctSectionList = projectFormData.Select(o =>
                            new { o.FormId, o.FormName, o.SectionId, o.SectionName, o.SortOrderNumber }).Distinct().OrderBy(o => o.SortOrderNumber).ToList();

                        var resNewMeterSizeList = objCompassEntities.STP_OfflineDataSP2(item.ProjectId, "tblMeterSize").ToList();
                        var resNewMeterTypeList = objCompassEntities.STP_OfflineDataSP3(item.ProjectId, "tblMeterType").ToList();
                        var resServiceList = objCompassEntities.STP_OfflineDataSP4(item.ProjectId, "tblService").ToList();
                        var resMeterMakeList = objCompassEntities.STP_OfflineDataSP5(item.ProjectId, "tblMeterMake").ToList();
                        var checkMappedColumnsToExcel = objCompassEntities.SP_GetFieldInfoForProject(item.ProjectId).ToList();

                        newMobileOffline1 objNewMobileOffline = new newMobileOffline1();
                        objNewMobileOffline.projectId = item.ProjectId;
                        objNewMobileOffline.projectName = item.ProjectName;
                        objNewMobileOffline.dataSyncConnection = !string.IsNullOrEmpty(Convert.ToString(item.DataSyncConnection)) ? int.Parse(item.DataSyncConnection.ToString()) : 3;
                        objNewMobileOffline.autoDataSyncInterval = !string.IsNullOrEmpty(Convert.ToString(item.AutoDataSyncInterval)) ? int.Parse(item.AutoDataSyncInterval.ToString()) : 5;

                        #region Form Visits


                        List<Forms1> objListFormsNew = new List<Forms1>();

                        foreach (var item3 in projectDistinctFormList)
                        {
                            #region Form
                            Forms1 objForms = new Forms1();
                            objForms.formId = item3.FormId;
                            objForms.formName = item3.FormName;

                            List<Section> objListSection = new List<Section>();

                            foreach (var item4 in projectDistinctSectionList.Where(o => o.FormId == item3.FormId))
                            {
                                #region section
                                var sectionFields = projectFormData.Where(o => o.SectionId == item4.SectionId).Select(o => o).OrderBy(o => o.FieldOrder).ToList();
                                Section objSection = new Section();
                                objSection.sectionId = item4.SectionId;
                                objSection.sectionName = item4.SectionName;
                                objSection.sectionOrder = item4.SortOrderNumber;

                                List<Fields> objListFields = new List<Fields>();

                                foreach (var item5 in sectionFields)
                                {
                                    #region Field
                                    var aCurrentField = projectFormData.Where(o => o.FieldId == item5.FieldId).FirstOrDefault();
                                    Fields objField = new Fields();
                                    objField.acknowlegementButtonText = aCurrentField.AcknowlegementButtonText;
                                    objField.acknowlegementClickedButtonText = aCurrentField.AcknowlegementClickedButtonText;
                                    objField.acknowlegementText = aCurrentField.AcknowlegementText;
                                    objField.allowAnnotation = aCurrentField.AllowAnnotation != null ? bool.Parse(aCurrentField.AllowAnnotation.ToString()) : false;
                                    objField.allowMultiSelection = aCurrentField.AllowMultiSelection != null ? bool.Parse(aCurrentField.AllowMultiSelection.ToString()) : false;
                                    objField.allowNotApplicable = aCurrentField.AllowNotApplicable != null ? bool.Parse(aCurrentField.AllowNotApplicable.ToString()) : false;
                                    objField.allowprii = aCurrentField.allowprii != null ? bool.Parse(aCurrentField.allowprii.ToString()) : false;
                                    objField.audioQuality = aCurrentField.AudioQuality;
                                    objField.cameraEnabled = aCurrentField.CameraEnabled != null ? bool.Parse(aCurrentField.CameraEnabled.ToString()) : false;
                                    objField.captureGeolocation = aCurrentField.CaptureGeolocation != null ? bool.Parse(aCurrentField.CaptureGeolocation.ToString()) : false;
                                    objField.captureMode = aCurrentField.CaptureMode;
                                    objField.captureTimestamp = aCurrentField.CaptureTimestamp != null ? bool.Parse(aCurrentField.CaptureTimestamp.ToString()) : false;
                                    objField.decimalPositions = aCurrentField.DecimalPositions;
                                    objField.defaultValue = aCurrentField.DefaultValue;
                                    objField.hideFieldLabel = aCurrentField.HideFieldLabel != null ? bool.Parse(aCurrentField.HideFieldLabel.ToString()) : false;
                                    objField.displayMask = aCurrentField.DisplayMask;
                                    objField.enforceMinMax = aCurrentField.EnforceMinMax != null ? bool.Parse(aCurrentField.EnforceMinMax.ToString()) : false;
                                    objField.excludeonSync = aCurrentField.ExcludeonSync != null ? bool.Parse(aCurrentField.ExcludeonSync.ToString()) : false;



                                    objField.fieldDataTypeName = aCurrentField.FieldDataTypeName;
                                    objField.fieldFilterkey = aCurrentField.FieldFilterkey;
                                    objField.fieldId = aCurrentField.FieldId;
                                    objField.fieldLabel = aCurrentField.FieldLabel;
                                    objField.fieldName = aCurrentField.FieldName;
                                    objField.fieldOrder = aCurrentField.FieldOrder;

                                    objField.formatMask = aCurrentField.FormatMask;
                                    objField.gpsTagging = aCurrentField.GPSTagging != null ? bool.Parse(aCurrentField.GPSTagging.ToString()) : false;
                                    objField.hasAlert = aCurrentField.HasAlert != null ? bool.Parse(aCurrentField.HasAlert.ToString()) : false;
                                    objField.hasFile = aCurrentField.HasFile != null ? bool.Parse(aCurrentField.HasFile.ToString()) : false;
                                    objField.hasValues = aCurrentField.HasValues != null ? bool.Parse(aCurrentField.HasValues.ToString()) : false;
                                    objField.hintText = aCurrentField.HintText;
                                    objField.isEnabled = aCurrentField.IsEnabled != null ? bool.Parse(aCurrentField.IsEnabled.ToString()) : false;
                                    objField.isRequired = aCurrentField.IsRequired != null ? bool.Parse(aCurrentField.IsRequired.ToString()) : false;
                                    objField.keyboardType = aCurrentField.KeyboardType != null ? bool.Parse(aCurrentField.KeyboardType.ToString()) : false;
                                    if (!string.IsNullOrEmpty(aCurrentField.MaximumHeight))
                                    {
                                        objField.maximumHeight = int.Parse(aCurrentField.MaximumHeight);
                                    }
                                    if (!string.IsNullOrEmpty(aCurrentField.MaximumWidth))
                                    {
                                        objField.maximumWidth = int.Parse(aCurrentField.MaximumWidth);
                                    }
                                    objField.maxLengthValue = !string.IsNullOrEmpty(aCurrentField.MaxLengthValue) ? int.Parse(aCurrentField.MaxLengthValue) : 250;
                                    objField.minLengthValue = !string.IsNullOrEmpty(aCurrentField.MinLengthValue) ? int.Parse(aCurrentField.MinLengthValue) : 250;

                                    if (!string.IsNullOrEmpty(aCurrentField.NumberOfColumnsForPhones))
                                    {
                                        objField.numberOfColumnsForPhones = int.Parse(aCurrentField.NumberOfColumnsForPhones);
                                    }
                                    if (!string.IsNullOrEmpty(aCurrentField.NumberOfColumnsForTablets))
                                    {
                                        objField.numberOfColumnsForTablets = int.Parse(aCurrentField.NumberOfColumnsForTablets);
                                    }
                                    objField.photoFile = aCurrentField.PhotoFile != null ? bool.Parse(aCurrentField.PhotoFile.ToString()) : false;
                                    objField.photoLibraryEnabled = aCurrentField.PhotoLibraryEnabled != null ? bool.Parse(aCurrentField.PhotoLibraryEnabled.ToString()) : false;
                                    objField.photoQuality = aCurrentField.PhotoQuality;


                                    objField.ratingLabels = aCurrentField.RatingLabels != null ? bool.Parse(aCurrentField.RatingLabels.ToString()) : false;
                                    objField.ratingMax = aCurrentField.RatingMax != null ? bool.Parse(aCurrentField.RatingMax.ToString()) : false;
                                    objField.sectionId = aCurrentField.SectionId;
                                    objField.secure = aCurrentField.Secure != null ? bool.Parse(aCurrentField.Secure.ToString()) : false;
                                    objField.showMultiselect = aCurrentField.ShowMultiselect != null ? bool.Parse(aCurrentField.ShowMultiselect.ToString()) : false;
                                    objField.showRatingLabels = aCurrentField.ShowRatingLabels != null ? bool.Parse(aCurrentField.ShowRatingLabels.ToString()) : false;
                                    if (!string.IsNullOrEmpty(aCurrentField.MaxrecordabletimeInSeconds))
                                    {
                                        objField.maxrecordabletimeInSeconds = int.Parse(aCurrentField.MaxrecordabletimeInSeconds);
                                    }
                                    objField.videoQuality = aCurrentField.VideoQuality;

                                    List<FieldInitialDataO> FieldInitialDataList = new List<FieldInitialDataO>();
                                    objField.fieldInitialDataList = FieldInitialDataList;

                                    objListFields.Add(objField);

                                    #endregion
                                }
                                objSection.fieldList = objListFields;
                                objListSection.Add(objSection);

                                #endregion
                            }
                            objForms.sectionList = objListSection;
                            objListFormsNew.Add(objForms);


                            #endregion
                        }



                        #endregion

                        #region MenuVisit
                        List<MenuVisits2> objMenuVisitsList = new List<MenuVisits2>();
                        var currentProData = ProjectVisitDataChunk.Where(o => o.ProjectId == objNewMobileOffline.projectId).ToList();

                        foreach (var item1 in currentProData)
                        {

                            int streetCount = objMenuVisitsList.Where(o => o.street == item1.Street).Count();

                            if (streetCount == 0)
                            {

                                var adata = distCountPro.Where(o => o.Street == item1.Street).FirstOrDefault();
                                int completed = 0;
                                int skipped = 0;
                                int totalVisits = 0;
                                int pendingVisits = 0;
                                if (adata != null)
                                {
                                    completed = adata.TotalCountWS != null ? int.Parse(adata.TotalCountWS.ToString()) : 0;
                                    skipped = adata.TotalCountSS != null ? int.Parse(adata.TotalCountSS.ToString()) : 0;
                                    totalVisits = adata.TotalCountS != null ? int.Parse(adata.TotalCountS.ToString()) : 0;
                                    pendingVisits = totalVisits - (completed + skipped);
                                }


                                var getStreetNameData = currentProData.Where(o => o.Street == item1.Street).ToList();

                                MenuVisits2 objMenuVisits = new MenuVisits2();
                                //objMenuVisits.latitude = Convert.ToString(item1.Latitude);
                                //objMenuVisits.longitude = Convert.ToString(item1.Longitude);
                                objMenuVisits.pendingVisits = pendingVisits;
                                objMenuVisits.completedVisits = completed;
                                objMenuVisits.skippedVisits = skipped;
                                objMenuVisits.totalVisits = totalVisits;
                                if (completed == totalVisits)
                                {
                                    objMenuVisits.colorFlag = 4;
                                }
                                else if (totalVisits == pendingVisits)
                                {
                                    objMenuVisits.colorFlag = 1;
                                }
                                else if (skipped > 0 && pendingVisits == 0)
                                {
                                    objMenuVisits.colorFlag = 3;
                                }
                                else if (pendingVisits > 0)
                                {
                                    objMenuVisits.colorFlag = 2;
                                }

                                objMenuVisits.street = item1.Street;
                                objMenuVisits.visitId = item1.VisitId;
                                objMenuVisits.groupId = item1.GroupId;



                                List<Accounts2> objListAccounts = new List<Accounts2>();
                                foreach (var item2 in getStreetNameData)
                                {
                                    #region Account
                                    bool isReopen = false;
                                    var listReopen = getStreetNameData.Where(o => o.UploadId == item2.UploadId).ToList();

                                    if (listReopen.Count > 1)
                                    {
                                        var checkReopen = listReopen.Where(o => o.Reopen == 1).FirstOrDefault();
                                        if (checkReopen != null)
                                        {
                                            if (item2.Reopen == 0)
                                            {
                                                isReopen = true;
                                            }
                                        }

                                    }

                                    if (!isReopen)
                                    {
                                        #region Account


                                        var accountFlag = distCountPro.Where(o => o.Account == item2.Account && o.oldmeterno == item2.OldMeterNo).FirstOrDefault();

                                        int completedA = 0;
                                        int skippedA = 0;
                                        int totalVisitsA = 0;
                                        int pendingVisitsA = 0;
                                        if (accountFlag != null)
                                        {
                                            completedA = accountFlag.TotalCountWA != null ? int.Parse(accountFlag.TotalCountWA.ToString()) : 0;
                                            skippedA = accountFlag.TotalCountSA != null ? int.Parse(accountFlag.TotalCountSA.ToString()) : 0;
                                            totalVisitsA = accountFlag.TotalCountA != null ? int.Parse(accountFlag.TotalCountA.ToString()) : 0;
                                            pendingVisitsA = totalVisitsA - (completedA + skippedA);
                                        }


                                        string visitDate = accountFlag != null ? (!string.IsNullOrEmpty(Convert.ToString(accountFlag.CreatedOnA))
                                            ? DateTime.Parse(Convert.ToString(accountFlag.CreatedOnA)).ToString("MMM-dd-yyyy hh:mm tt") : "") : "";

                                        Accounts2 objAccounts = new Accounts2();
                                        objAccounts.accountNumber = item2.Account;
                                        objAccounts.isActive = item2.Active == 0 ? false : true;
                                        objAccounts.isReOpen = item2.Reopen == 0 ? false : true;


                                        if (completedA == totalVisitsA)
                                        {
                                            objAccounts.colorFlag = 4;
                                        }
                                        else if (totalVisitsA == pendingVisitsA)
                                        {
                                            objAccounts.colorFlag = 1;
                                        }
                                        else if (skippedA > 0 && pendingVisitsA == 0)
                                        {
                                            objAccounts.colorFlag = 3;
                                        }
                                        else if (pendingVisitsA > 0)
                                        {
                                            objAccounts.colorFlag = 2;
                                        }
                                        objAccounts.isVisited = item2.Active == 0 ? false : (objAccounts.colorFlag == 4 ? true : false);

                                        objAccounts.oldMeterNumber = item2.OldMeterNo;
                                        objAccounts.oldMeterSize = item2.OldMeterSize;
                                        objAccounts.oldMeterType = item2.OldMeterType;
                                        objAccounts.oldMeterRadioNumber = item2.OldMeterRadioNo;
                                        objAccounts.uploadId = item2.UploadId;

                                        objAccounts.visitDateTime = visitDate;
                                        List<Forms3> objListForms = new List<Forms3>();

                                        foreach (var item3 in projectDistinctFormList)
                                        {
                                            var adata1 = distCountPro.Where(o => o.UploadId == item2.UploadId && o.formId == item3.FormId).FirstOrDefault();

                                            #region Form


                                            Forms3 objForms = new Forms3();
                                            objForms.formId = item3.FormId;
                                            objForms.formName = item3.FormName;


                                            objForms.isSkipped = adata1 != null ? (adata1.FormSkipped == 1 ? true : false) : false;
                                            objForms.isVisited = adata1 != null ? (adata1.FormWorkCompleted == 1 ? true : false) : false;
                                            objForms.visitDateTime = adata1 != null ? (!string.IsNullOrEmpty(Convert.ToString(adata1.CreatedOnU)) ?
                                                DateTime.Parse(Convert.ToString(adata1.CreatedOnU)).ToString("MMM-dd-yyyy hh:mm tt") : "") : "";
                                            //DateTime.Parse(Convert.ToString(adata1.CreatedOnU)).ToString("dd-MMM-yyyy HH:MM T")

                                            if (objForms.isSkipped)
                                            {
                                                //objForms.
                                                var aFieldDataRecord = objCompassEntities.TblProjectFieldDatas.Where(o => o.FormId == item3.FormId
                                                    && o.InstallerId == userId && o.tblUploadedData_Id == adata1.UploadId && o.Active == 1 && o.IsSkipped == true).FirstOrDefault();
                                                if (aFieldDataRecord != null)
                                                {
                                                    var m1 = skipReasonTable.Where(o => o.SkipId == aFieldDataRecord.SkipId).FirstOrDefault();
                                                    if (m1 != null)
                                                    {
                                                        objForms.reasonType = m1.CategoryType;
                                                        objForms.skipReason = m1.SkipReason;
                                                    }
                                                }

                                            }
                                            List<Fields1> objListFields = new List<Fields1>();
                                            foreach (var item4 in projectDistinctSectionList.Where(o => o.FormId == item3.FormId))
                                            {
                                                var sectionFields = projectFormData.Where(o => o.SectionId == item4.SectionId).Select(o => o).OrderBy(o => o.FieldOrder).ToList();

                                                foreach (var item5 in sectionFields)
                                                {
                                                    #region Fields
                                                    var aCurrentField = projectFormData.Where(o => o.FieldId == item5.FieldId).FirstOrDefault();
                                                    Fields1 objField = new Fields1();
                                                    objField.fieldId = item5.FieldId;
                                                    objField.sectionId = item4.SectionId;
                                                    objField.fieldName = item5.FieldName;

                                                    List<FieldInitialDataO> FieldInitialDataList = new List<FieldInitialDataO>();

                                                    if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER SIZE".ToLower())
                                                    {
                                                        FieldInitialDataList = resNewMeterSizeList.Select(o => new FieldInitialDataO { initialFieldId = o.initialFieldId, initialFieldValue = o.initialFieldValue }).ToList();
                                                    }
                                                    else if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER TYPE".ToLower())
                                                    {
                                                        FieldInitialDataList = resNewMeterTypeList.Select(o => new FieldInitialDataO { initialFieldId = o.initialFieldId, initialFieldValue = o.initialFieldValue }).ToList();
                                                    }
                                                    else if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER MAKE".ToLower())
                                                    {
                                                        FieldInitialDataList = resMeterMakeList.Select(o => new FieldInitialDataO { initialFieldId = o.initialFieldId, initialFieldValue = o.initialFieldValue }).ToList();
                                                    }
                                                    else if (aCurrentField.FieldName.ToLower().Trim() == "SERVICES".ToLower())
                                                    {

                                                        FieldInitialDataList = resServiceList.Where(o => o.isAdditionalService == false).Select(o =>
                                                            new FieldInitialDataO
                                                            {
                                                                initialFieldId = o.initialFieldId,
                                                                initialFieldValue = o.initialFieldValue,
                                                                flagCaptureAudio = o.flagCaptureAudio,
                                                                flagCaptureVideo = o.flagCaptureVideo,
                                                                flagIsImageCaptureRequired = bool.Parse(o.flagIsImageCaptureRequired.ToString()),
                                                                isAdditionalService = bool.Parse(o.isAdditionalService.ToString()),
                                                                //imageList = string.IsNullOrEmpty((o.imageList)) ? "".Split(',').ToList() : (o.imageList).Split(',').ToList()
                                                                imageList = string.IsNullOrEmpty((o.imageList)) ? null : (o.imageList).Split(',').ToList()
                                                               .Select(o2 => new PictureList
                                                               {
                                                                   imageId = (ImageList.Where(o1 => o1.Picture.Trim() == o2.Trim()).Select(o1 => o1.ID).FirstOrDefault()),
                                                                   imageName = o2.Trim()
                                                               }).ToList()
                                                            }).ToList();
                                                    }
                                                    else if (aCurrentField.FieldName.ToLower().Trim() == "ADDITIONAL SERVICES".ToLower())
                                                    {

                                                        FieldInitialDataList = resServiceList.Where(o => o.isAdditionalService == true).Select(o =>
                                                            new FieldInitialDataO
                                                            {
                                                                initialFieldId = o.initialFieldId,
                                                                initialFieldValue = o.initialFieldValue,
                                                                flagCaptureAudio = o.flagCaptureAudio,
                                                                flagCaptureVideo = o.flagCaptureVideo,
                                                                flagIsImageCaptureRequired = bool.Parse(o.flagIsImageCaptureRequired.ToString()),
                                                                isAdditionalService = bool.Parse(o.isAdditionalService.ToString()),
                                                                //imageList = string.IsNullOrEmpty((o.imageList)) ? "".Split(',').ToList() : (o.imageList).Split(',').ToList()
                                                                imageList = string.IsNullOrEmpty((o.imageList)) ? null : (o.imageList).Split(',').ToList()
                                                               .Select(o2 => new PictureList
                                                               {
                                                                   imageId = (ImageList.Where(o1 => o1.Picture.Trim() == o2.Trim()).Select(o1 => o1.ID).FirstOrDefault()),
                                                                   imageName = o2.Trim()
                                                               }).ToList()
                                                            }).ToList();
                                                    }
                                                    else
                                                    {
                                                        var checkfield = checkMappedColumnsToExcel.Where(o => o.FieldName == aCurrentField.FieldName).FirstOrDefault();

                                                        if (checkfield != null && checkfield.isMappedToExcel == 1 && checkfield.FieldName != "LOCATION/GPS")
                                                        {
                                                            FieldInitialDataO objFieldInitialData = new FieldInitialDataO();

                                                            objFieldInitialData.initialFieldId = item5.FieldId;
                                                            if (checkfield.FieldName.ToLower() == "account")
                                                            {
                                                                objFieldInitialData.initialFieldValue = item2.Account;
                                                            }
                                                            else if (checkfield.FieldName.ToLower() == "STREET".ToLower())
                                                            {
                                                                objFieldInitialData.initialFieldValue = item2.Street;
                                                            }
                                                            else if (checkfield.FieldName.ToLower() == "OLD METER NUMBER".ToLower())
                                                            {
                                                                objFieldInitialData.initialFieldValue = item2.OldMeterNo;
                                                            }
                                                            else if (checkfield.FieldName.ToLower() == "OLD METER SIZE".ToLower())
                                                            {
                                                                objFieldInitialData.initialFieldValue = item2.OldMeterSize;
                                                            }
                                                            else if (checkfield.FieldName.ToLower() == "OLD METER TYPE".ToLower())
                                                            {
                                                                objFieldInitialData.initialFieldValue = item2.OldMeterType;
                                                            }
                                                            else if (checkfield.FieldName.ToLower() == "OLD METER RADIO NUMBER".ToLower())
                                                            {
                                                                objFieldInitialData.initialFieldValue = item2.OldMeterRadioNo;
                                                            }
                                                            else if (checkfield.FieldName.ToLower() == "OLD METER LAST READING".ToLower())
                                                            {
                                                                objFieldInitialData.initialFieldValue = item2.OldMeterLastReading;
                                                            }
                                                            else if (checkfield.FieldName.ToLower() == "CUSTOMER".ToLower())
                                                            {
                                                                objFieldInitialData.initialFieldValue = item2.customer;
                                                            }
                                                            else if (checkfield.FieldName.ToLower() == "VOLTS".ToLower())
                                                            {
                                                                objFieldInitialData.initialFieldValue = item2.ElectricVOLTS;
                                                            }
                                                            else if (checkfield.FieldName.ToLower() == "PHASE".ToLower())
                                                            {
                                                                objFieldInitialData.initialFieldValue = item2.ElectricPHASE;
                                                            }
                                                            else if (checkfield.FieldName.ToLower() == "WIRE".ToLower())
                                                            {
                                                                objFieldInitialData.initialFieldValue = item2.ElectricWIRE;
                                                            }
                                                            else if (checkfield.FieldName.ToLower() == "MULTIPLIER".ToLower())
                                                            {
                                                                objFieldInitialData.initialFieldValue = item2.ElectricMULTIPLIER;
                                                            }
                                                            else if (checkfield.FieldName.ToLower() == "AMPS".ToLower())
                                                            {
                                                                objFieldInitialData.initialFieldValue = item2.ElectricAMPS;
                                                            }
                                                            else if (checkfield.FieldName.ToLower() == "MODEL".ToLower())
                                                            {
                                                                objFieldInitialData.initialFieldValue = item2.ElectricMODEL;
                                                            }
                                                            FieldInitialDataList.Add(objFieldInitialData);
                                                        }
                                                        else
                                                        {
                                                            var res = objCompassEntities.tblProjectFieldDDLMasters.
                                                                Where(o => o.FormSectionFieldID == aCurrentField.FieldId && o.Active == 1).ToList();
                                                            FieldInitialDataList = res.Select(o => new FieldInitialDataO { initialFieldId = o.ID, initialFieldValue = o.FieldValue }).ToList();
                                                        }

                                                    }


                                                    objField.fieldInitialDataList = FieldInitialDataList;

                                                    objListFields.Add(objField);

                                                    #endregion
                                                }
                                            }

                                            objForms.fieldList = objListFields;
                                            objListForms.Add(objForms);


                                            #endregion
                                        }
                                        objAccounts.formList = objListForms;
                                        objListAccounts.Add(objAccounts);

                                        #endregion
                                    }

                                    #endregion
                                }

                                objMenuVisits.objAccounts = objListAccounts;

                                var _totalAccount = objListAccounts.Count;
                                var _deactivateAccount = objListAccounts.Where(o => o.isActive == false).Count();
                                objMenuVisits.isActive = (_deactivateAccount > 0 && _deactivateAccount == _totalAccount) ? false : true;

                                objMenuVisitsList.Add(objMenuVisits);


                            }

                        }
                        #endregion

                        #region Map Visit
                        List<MapVisits> objListMapVisits = new List<MapVisits>();

                        foreach (var item1 in currentProData)
                        {
                            var checkPresent = objListMapVisits.Where(o => o.latitude == Convert.ToString(item1.Latitude) &&
                                o.longitude == Convert.ToString(item1.Longitude)).FirstOrDefault();
                            if (checkPresent == null)
                            {
                                var allMatchingLatLonList = currentProData.Where(o => o.Latitude == item1.Latitude && o.Longitude == item1.Longitude
                                    && o.ProjectId == item1.ProjectId).ToList();
                                if (allMatchingLatLonList.Count == 1)
                                {
                                    var adata = distCountPro.Where(o => o.Latitude == item1.Latitude && o.Longitude == item1.Longitude
                                        && o.ProjectId == item1.ProjectId).FirstOrDefault();

                                    int completed = 0;
                                    int skipped = 0;
                                    int totalVisits = 0;
                                    int pendingVisits = 0;

                                    if (adata != null)
                                    {
                                        completed = adata.TotalCountWL != null ? int.Parse(adata.TotalCountWL.ToString()) : 0;
                                        skipped = adata.TotalCountSL != null ? int.Parse(adata.TotalCountSL.ToString()) : 0;
                                        totalVisits = adata.TotalCountL != null ? int.Parse(adata.TotalCountL.ToString()) : 0;
                                        pendingVisits = totalVisits - (completed + skipped);
                                    }
                                    MapVisits objMapVisits = new MapVisits();
                                    objMapVisits.groupId = item1.GroupId;

                                    if (completed == totalVisits)
                                    {
                                        objMapVisits.colorFlag = 4;
                                    }
                                    else if (totalVisits == pendingVisits)
                                    {
                                        objMapVisits.colorFlag = 1;
                                    }
                                    else if (skipped > 0 && pendingVisits == 0)
                                    {
                                        objMapVisits.colorFlag = 3;
                                    }
                                    else if (pendingVisits > 0)
                                    {
                                        objMapVisits.colorFlag = 2;
                                    }



                                    objMapVisits.street = item1.Street;
                                    objMapVisits.latitude = Convert.ToString(item1.Latitude);
                                    objMapVisits.longitude = Convert.ToString(item1.Longitude);
                                    objMapVisits.completedVisits = completed;
                                    objMapVisits.pendingVisits = pendingVisits;
                                    objMapVisits.skippedVisits = skipped;
                                    objMapVisits.totalVisits = totalVisits;
                                    objMapVisits.isActive = item1.Active == 1 ? true : false;
                                    objListMapVisits.Add(objMapVisits);
                                }
                                else
                                {
                                    var adata = distCountPro.Where(o => o.Latitude == item1.Latitude && o.Longitude == item1.Longitude
                                        && o.ProjectId == item1.ProjectId).FirstOrDefault();
                                    int completed = 0;
                                    int skipped = 0;
                                    int totalVisits = 0;
                                    int pendingVisits = 0;

                                    if (adata != null)
                                    {
                                        completed = adata.TotalCountWL != null ? int.Parse(adata.TotalCountWL.ToString()) : 0;
                                        skipped = adata.TotalCountSL != null ? int.Parse(adata.TotalCountSL.ToString()) : 0;
                                        totalVisits = adata.TotalCountL != null ? int.Parse(adata.TotalCountL.ToString()) : 0;
                                        pendingVisits = totalVisits - (completed + skipped);
                                    }
                                    //  

                                    var checkAddressList = allMatchingLatLonList.Select(o => o.Street).Distinct().ToList();

                                    IEnumerable<string> s1 = StringExtensions.GetMostCommonSubstrings(checkAddressList);

                                    string swd = s1.FirstOrDefault();

                                    MapVisits objMapVisits = new MapVisits();
                                    objMapVisits.groupId = item1.GroupId;
                                    if (completed == totalVisits)
                                    {
                                        objMapVisits.colorFlag = 4;
                                    }
                                    else if (totalVisits == pendingVisits)
                                    {
                                        objMapVisits.colorFlag = 1;
                                    }
                                    else if (skipped > 0 && pendingVisits == 0)
                                    {
                                        objMapVisits.colorFlag = 3;
                                    }
                                    else if (pendingVisits > 0)
                                    {
                                        objMapVisits.colorFlag = 2;
                                    }
                                    objMapVisits.street = swd;
                                    objMapVisits.latitude = Convert.ToString(item1.Latitude);
                                    objMapVisits.longitude = Convert.ToString(item1.Longitude);
                                    objMapVisits.completedVisits = completed;
                                    objMapVisits.pendingVisits = pendingVisits;
                                    objMapVisits.skippedVisits = skipped;
                                    objMapVisits.totalVisits = totalVisits;


                                    var _totalAccount = currentProData.Where(o => o.Latitude == item1.Latitude && o.Longitude == item1.Longitude
                                        && o.ProjectId == item1.ProjectId).Count();
                                    var _deactivateAccount = currentProData.Where(o => o.Latitude == item1.Latitude && o.Longitude == item1.Longitude && o.Active == 0
                                        && o.ProjectId == item1.ProjectId).Count();

                                    objMapVisits.isActive = (_deactivateAccount == _totalAccount && _deactivateAccount > 0) ? false : true;

                                    objListMapVisits.Add(objMapVisits);
                                }
                            }

                        }


                        #endregion


                        objNewMobileOffline.objFormStructure = objListFormsNew;
                        objNewMobileOffline.objMenuVisits = objMenuVisitsList;
                        objNewMobileOffline.objMapVisits = objListMapVisits;
                        objNewMobileOffline.skipReasonList = skipList.Select(o => new SkippReason { skipId = o.SkipId, skipReason = o.SkipReason }).ToList();
                        objNewMobileOffline.rtuReasonList = rtuList.Select(o => new RTUReason { rtuId = o.SkipId, rtuReason = o.SkipReason }).ToList();
                        objListMobileOffline.Add(objNewMobileOffline);
                    }

                }

                JavaScriptSerializer s = new JavaScriptSerializer();
                s.MaxJsonLength = 2147483644;
                // s.Serialize(objListMobileOffline);
                int currentRecords = 0;
                if (objListMobileOffline != null)
                {
                    foreach (var item in objListMobileOffline)
                    {
                        foreach (var item1 in item.objMenuVisits)
                        {
                            currentRecords = currentRecords + item1.objAccounts.Count();
                        }
                    }
                }
                string json = s.Serialize(objListMobileOffline);

                string m = GetMd5Sum(json);
                mobileOfflineResult.checkSum = m;
                mobileOfflineResult.data = objListMobileOffline;
                mobileOfflineResult.batchNumber = BatchNumber;
                mobileOfflineResult.batchIndicator = checkRecordAvilableForNextTrip;
                mobileOfflineResult.chunkSize = currentRecords;
                mobileOfflineResult.totalRecord = chunkRecordCount;



                #region set checksum & BatchNumber
                objCompassEntities = new CompassEntities();
                foreach (var item in ProjectVisitDataChunk)
                {
                    var objInstallerMap = objCompassEntities.tblInstallerMapDatas.
                        Where(o => o.tblUploadedData_ID == item.UploadId && o.InstallerId == userId).FirstOrDefault();
                    objInstallerMap.BatchNumber = BatchNumber;
                    objInstallerMap.SynchCheckSumString = m;
                    objInstallerMap.SynchWithMobileFlag = 1;
                    objInstallerMap.SynchWithMobileDate = new CommonFunctions().ServerDate();
                    objCompassEntities.SaveChanges();
                }



                #endregion

                return mobileOfflineResult;


                #endregion
            }
            else
            {
                return null;
            }

        }




        /// <summary>
        /// 2016-10-03 Devloper- Bharat
        /// Returns offline data for given user.
        /// </summary>
        /// <param name="userId">Current userId</param>
        /// <param name="strCurrentDate">Mobile current date</param>
        /// <param name="batchNumber">batch number</param>
        /// <returns>List </returns>
        public MobileOfflineResult2 OfflineData(long userId, DateTime strCurrentDate, long batchNumber)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                MobileOfflineResult2 mobileOfflineResult = new MobileOfflineResult2();

                List<newMobileOffline2> objListMobileOffline = new List<newMobileOffline2>();

                string procName = "";
                int? excludeSkip = 0;
                long BatchNumber = 0;
                bool sendAlreadySentRecord = false;
                bool checkRecordAvilableForNextTrip = false;
                CompassEntities objCompassEntities = new CompassEntities();

                DataTable dt = new DataTable();


                DataTable countDT = new DataTable();
                CompassEntities db = new CompassEntities();
                var UserObj = objCompassEntities.tblUsers.Where(n => n.UserID == userId).FirstOrDefault();
                excludeSkip = UserObj.ExcludeSkip;
                if (excludeSkip == 1)
                {
                    procName = "PROC_WebService_Installer_OfflineDataExcludeSkip";
                }
                else
                {
                    procName = "PROC_WebService_Installer_OfflineData";
                }

                using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(procName, sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@installerId", userId);
                        cmd.Parameters.Add("@strCurrentDate", strCurrentDate);
                        cmd.CommandTimeout = 600;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("PROC_WebService_Installer_OfflineCountSP_Appointment", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@installerId", userId);
                        cmd.Parameters.Add("@strCurrentDate", strCurrentDate);
                        cmd.CommandTimeout = 600;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(countDT);
                        }
                    }
                }



                //returns current batch number for the user.
                bool WrongBatchNumber = GetBatchNumber(userId, dt, strCurrentDate, batchNumber, ref BatchNumber, ref sendAlreadySentRecord);



                dt = new DataTable();
                using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(procName, sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@installerId", userId);
                        cmd.Parameters.Add("@strCurrentDate", strCurrentDate);
                        cmd.CommandTimeout = 600;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }


                if (!WrongBatchNumber)
                {
                    #region  send records



                    // get all the image list, skip reasons list, rtu reason list & user data. 
                    var skipReasonTable = objCompassEntities.tblSkipReasonMasters.Where(o => o.Active == 1).ToList();
                    var imageList = objCompassEntities.tblPictures.ToList();
                    var skipList = skipReasonTable.Where(o => o.CategoryType.ToLower() == "skip").ToList();
                    var rtuList = skipReasonTable.Where(o => o.CategoryType.ToLower() == "rtu").ToList();
                    //var ProjectVisitData = objCompassEntities.STP_OfflineDataSP(userId, strCurrentDate).ToList();
                    //var projectCountLogic = objCompassEntities.PROC_OfflineCountSP(userId, strCurrentDate).ToList();
                    int totalCount = dt.AsEnumerable().Where(o => (o.Field<int>("Active1") == 1 || o.Field<int>("Active1") == 0) && (o.Field<int?>("MobileResponse") == null || o.Field<int?>("MobileResponse") == 0)).Count();
                    int chunkRecordCount = dt.AsEnumerable().Where(o => (o.Field<int>("Active1") == 1 || o.Field<int>("Active1") == 0) && (o.Field<int?>("MobileResponse") == null || o.Field<int?>("MobileResponse") == 0) && o.Field<int?>("Reopen") == 0).Select(o => o).ToList().Count();



                    DataTable ProjectVisitDataChunk = new DataTable();

                    if (sendAlreadySentRecord == true)
                    {
                        var dt1 = dt.AsEnumerable().Where(o => o.Field<Int64?>("BatchNumber") == BatchNumber).ToList();
                        ProjectVisitDataChunk = dt1.Where(o => o.Field<Int32?>("MobileResponse") != 1).Select(o => o).Take(50).CopyToDataTable();
                    }
                    else
                    {
                        var _data = dt.AsEnumerable().Where(o => (o.Field<int>("Active1") == 1 || o.Field<int>("Active1") == 0)
                            && (o.Field<int?>("MobileResponse") == null || o.Field<int?>("MobileResponse") == 0)).Select(o => o).Take(50).ToList();


                        if (_data == null || _data.Count == 0)
                        {
                            JavaScriptSerializer s1 = new JavaScriptSerializer();
                            s1.MaxJsonLength = 2147483644;
                            string json = s1.Serialize(objListMobileOffline);
                            string m = GetMd5Sum(json);
                            mobileOfflineResult.checkSum = m;
                            mobileOfflineResult.data = objListMobileOffline;
                            mobileOfflineResult.batchNumber = BatchNumber;
                            mobileOfflineResult.batchIndicator = false;
                            mobileOfflineResult.chunkSize = 0;
                            mobileOfflineResult.totalRecord = 0;
                            return mobileOfflineResult;
                        }
                        else
                        {
                            ProjectVisitDataChunk = _data.CopyToDataTable();
                        }
                    }

                    checkRecordAvilableForNextTrip = (ProjectVisitDataChunk == null || ProjectVisitDataChunk.Rows.Count == 0) ? false : ((totalCount - 50) > 0 ? true : false);

                    var FormDataList = GetDynamicFormDataList(userId, strCurrentDate);

                    if (dt != null)
                    {
                        var distPro = ProjectVisitDataChunk.AsEnumerable().
                            Select(o => new
                            {
                                ProjectId = o.Field<long>("ProjectId"),
                                ProjectName = o.Field<string>("ProjectName"),
                                AutoDataSyncInterval = o.Field<int?>("AutoDataSyncInterval"),
                                DataSyncConnection = o.Field<int?>("DataSyncConnection")
                            }).Distinct().ToList();


                        foreach (var item in distPro)
                        {

                            string strAppointmentDateList = "";

                            var AppointmentList = objCompassEntities.PROC_WebService_Installer_OfflineData_AppoinmentDetails(item.ProjectId, userId, strCurrentDate).ToList();

                            if (AppointmentList != null)
                            {
                                var dateList = AppointmentList.Select(o => Convert.ToDateTime(o.FirstPreferedDate).ToString("MMM-dd-yyyy")).Distinct().ToList();

                                strAppointmentDateList = String.Join(",", dateList);
                            }



                            var projectData = objCompassEntities.tblProjects.Where(o => o.ProjectId == item.ProjectId).FirstOrDefault();
                            newMobileOffline2 objNewMobileOffline = new newMobileOffline2();

                            objNewMobileOffline.trackInventoryForProject = (string.IsNullOrEmpty(projectData.ManageInventory) ? false : (projectData.ManageInventory == "No" ? false : true));
                            objNewMobileOffline.appointmentDates = strAppointmentDateList;
                            objNewMobileOffline.processId = 1;
                            objNewMobileOffline.projectId = item.ProjectId;
                            objNewMobileOffline.projectName = item.ProjectName;
                            objNewMobileOffline.dataSyncConnection = !string.IsNullOrEmpty(Convert.ToString(item.DataSyncConnection)) ? int.Parse(item.DataSyncConnection.ToString()) : 3;
                            objNewMobileOffline.autoDataSyncInterval = !string.IsNullOrEmpty(Convert.ToString(item.AutoDataSyncInterval)) ? int.Parse(item.AutoDataSyncInterval.ToString()) : 5;
                            objNewMobileOffline.synchBasicInfoOnly = !string.IsNullOrEmpty(Convert.ToString(projectData.PartialSynch)) ?
                                (int.Parse(projectData.PartialSynch.ToString()) == 1 ? true : false) : false;

                            /*if (!checkRecordAvilableForNextTrip)
                            {
                                var lastLocation = getLastCompleteLocation(userId, item.ProjectId);
                                if(lastLocation.Count > 0)
                                {
                                    objNewMobileOffline.lastCompleteLat = lastLocation["Latitude"];
                                    objNewMobileOffline.lastCompleteLong = lastLocation["Longitude"];
                                }
                                else
                                {
                                    objNewMobileOffline.lastCompleteLat = "";
                                    objNewMobileOffline.lastCompleteLong = "";
                                }
                                
                            }
                            else
                            {
                                objNewMobileOffline.lastCompleteLat = "";
                                objNewMobileOffline.lastCompleteLong = "";
                            }*/

                            long projectId = item.ProjectId;

                            var distCountPro = countDT.AsEnumerable().Where(o => o.Field<long>("FKProjectId") == projectId).CopyToDataTable();
                            var projectFormData = FormDataList.Where(o => o.ProjectID == projectId).ToList();
                            var projectDistinctFormList = projectFormData.Select(o => new { o.FormId, o.FormName }).Distinct().ToList();
                            var projectDistinctSectionList = projectFormData.Select(o =>
                                new
                                {
                                    FormId = o.FormId,
                                    FormName = o.FormName,
                                    SectionId = o.SectionId,
                                    SectionName = o.SectionName,
                                    SortOrderNumber = o.SortOrderNumber
                                }
                                    ).Distinct().OrderBy(o => o.SortOrderNumber).ToList();


                            var resNewMeterSizeList = objCompassEntities.STP_OfflineDataSP2(projectId, "tblMeterSize").ToList();
                            var resNewMeterTypeList = objCompassEntities.STP_OfflineDataSP3(projectId, "tblMeterType").ToList();
                            var resServiceList = objCompassEntities.STP_OfflineDataSP4(projectId, "tblService").ToList();
                            var resMeterMakeList = objCompassEntities.STP_OfflineDataSP5(projectId, "tblMeterMake").ToList();
                            var checkMappedColumnsToExcel = objCompassEntities.PROC_WebService_GetFieldInfoForProject(projectId).ToList();

                            var GetServiceProductData = objCompassEntities.PROC_WebService_GetFieldInfoForProject(projectId).ToList();
                            //Proc_ProjectServiceProductlist

                            var ServiceProductList = objCompassEntities.PROC_ProjectServiceProductlist(projectId).ToList();
                            var distinctserviceList = ServiceProductList.Select(o => o.FKServiceId).Distinct().ToList();

                            objNewMobileOffline.servicesProductList = new List<ServiceData>();


                            if (projectData.ManageInventory == "Yes")
                            {
                                foreach (var service in distinctserviceList)
                                {

                                    var PList = ServiceProductList.Where(o => o.FKServiceId == service).
                                        Select(o =>
                                            new ProductData
                                            {
                                                productAlertQty = o.productAlertQty,
                                                productId = o.Id,
                                                productName = o.ProductName,
                                                serialNumberRequired = o.SerialNumberRequired,
                                                productQty = int.Parse(o.Quantity.ToString())

                                            }).ToList();

                                    //objNewMobileOffline.servicesProductList.Add(new ServiceData { productList = PList, serviceId = service });
                                    var aIsAdditionalService = ServiceProductList.Where(o => o.FKServiceId == service).Select(o => o.IsAdditionalService).FirstOrDefault();
                                    if (aIsAdditionalService == null)
                                    {
                                        aIsAdditionalService = true;
                                    }
                                    objNewMobileOffline.servicesProductList.Add(new ServiceData { productList = PList, serviceId = service, IsAdditionalService = aIsAdditionalService.Value });

                                }
                            }



                            #region Form Visits

                            List<Forms1> objListFormsNew = new List<Forms1>();

                            foreach (var item3 in projectDistinctFormList)
                            {
                                #region Form
                                Forms1 objForms = new Forms1();
                                objForms.formId = item3.FormId;
                                objForms.formName = item3.FormName;

                                List<Section> objListSection = new List<Section>();

                                foreach (var item4 in projectDistinctSectionList.Where(o => o.FormId == item3.FormId))
                                {
                                    #region section
                                    var sectionFields = projectFormData.Where(o => o.SectionId == item4.SectionId).Select(o => o).OrderBy(o => o.FieldOrder).ToList();
                                    Section objSection = new Section();
                                    objSection.sectionId = item4.SectionId;
                                    objSection.sectionName = item4.SectionName;
                                    objSection.sectionOrder = item4.SortOrderNumber;

                                    List<Fields> objListFields = new List<Fields>();

                                    foreach (var item5 in sectionFields)
                                    {
                                        #region Field
                                        var aCurrentField = projectFormData.Where(o => o.FieldId == item5.FieldId).FirstOrDefault();
                                        Fields objField = new Fields();
                                        objField.acknowlegementButtonText = aCurrentField.AcknowlegementButtonText;
                                        objField.acknowlegementClickedButtonText = aCurrentField.AcknowlegementClickedButtonText;
                                        objField.acknowlegementText = aCurrentField.AcknowlegementText;
                                        objField.allowAnnotation = aCurrentField.AllowAnnotation != null ? bool.Parse(aCurrentField.AllowAnnotation.ToString()) : false;
                                        objField.allowMultiSelection = aCurrentField.AllowMultiSelection != null ? bool.Parse(aCurrentField.AllowMultiSelection.ToString()) : false;
                                        objField.allowNotApplicable = aCurrentField.AllowNotApplicable != null ? bool.Parse(aCurrentField.AllowNotApplicable.ToString()) : false;
                                        objField.allowprii = aCurrentField.allowprii != null ? bool.Parse(aCurrentField.allowprii.ToString()) : false;
                                        objField.audioQuality = aCurrentField.AudioQuality;
                                        objField.cameraEnabled = aCurrentField.CameraEnabled != null ? bool.Parse(aCurrentField.CameraEnabled.ToString()) : false;
                                        objField.captureGeolocation = aCurrentField.CaptureGeolocation != null ? bool.Parse(aCurrentField.CaptureGeolocation.ToString()) : false;
                                        objField.captureMode = aCurrentField.CaptureMode;
                                        objField.captureTimestamp = aCurrentField.CaptureTimestamp != null ? bool.Parse(aCurrentField.CaptureTimestamp.ToString()) : false;
                                        objField.decimalPositions = aCurrentField.DecimalPositions;
                                        objField.defaultValue = aCurrentField.DefaultValue;
                                        objField.hideFieldLabel = aCurrentField.HideFieldLabel != null ? bool.Parse(aCurrentField.HideFieldLabel.ToString()) : false;
                                        objField.displayMask = aCurrentField.DisplayMask;
                                        objField.enforceMinMax = aCurrentField.EnforceMinMax != null ? bool.Parse(aCurrentField.EnforceMinMax.ToString()) : false;
                                        objField.excludeonSync = aCurrentField.ExcludeonSync != null ? bool.Parse(aCurrentField.ExcludeonSync.ToString()) : false;



                                        objField.fieldDataTypeName = aCurrentField.FieldDataTypeName;
                                        objField.fieldFilterkey = aCurrentField.FieldFilterkey;
                                        objField.fieldId = aCurrentField.FieldId;
                                        objField.fieldLabel = aCurrentField.FieldLabel;
                                        objField.fieldName = aCurrentField.FieldName;
                                        objField.fieldOrder = aCurrentField.FieldOrder;

                                        objField.formatMask = aCurrentField.FormatMask;
                                        objField.gpsTagging = aCurrentField.GPSTagging != null ? bool.Parse(aCurrentField.GPSTagging.ToString()) : false;
                                        objField.hasAlert = aCurrentField.HasAlert != null ? bool.Parse(aCurrentField.HasAlert.ToString()) : false;
                                        objField.hasFile = aCurrentField.HasFile != null ? bool.Parse(aCurrentField.HasFile.ToString()) : false;
                                        objField.hasValues = aCurrentField.HasValues != null ? bool.Parse(aCurrentField.HasValues.ToString()) : false;
                                        objField.hintText = aCurrentField.HintText;
                                        objField.isEnabled = aCurrentField.IsEnabled != null ? bool.Parse(aCurrentField.IsEnabled.ToString()) : false;
                                        objField.isRequired = aCurrentField.IsRequired != null ? bool.Parse(aCurrentField.IsRequired.ToString()) : false;
                                        objField.keyboardType = aCurrentField.KeyboardType != null ? bool.Parse(aCurrentField.KeyboardType.ToString()) : false;
                                        if (!string.IsNullOrEmpty(aCurrentField.MaximumHeight))
                                        {
                                            objField.maximumHeight = int.Parse(aCurrentField.MaximumHeight);
                                        }
                                        if (!string.IsNullOrEmpty(aCurrentField.MaximumWidth))
                                        {
                                            objField.maximumWidth = int.Parse(aCurrentField.MaximumWidth);
                                        }
                                        objField.maxLengthValue = !string.IsNullOrEmpty(aCurrentField.MaxLengthValue) ? int.Parse(aCurrentField.MaxLengthValue) : 250;
                                        objField.minLengthValue = !string.IsNullOrEmpty(aCurrentField.MinLengthValue) ? int.Parse(aCurrentField.MinLengthValue) : 250;

                                        if (!string.IsNullOrEmpty(aCurrentField.NumberOfColumnsForPhones))
                                        {
                                            objField.numberOfColumnsForPhones = int.Parse(aCurrentField.NumberOfColumnsForPhones);
                                        }
                                        if (!string.IsNullOrEmpty(aCurrentField.NumberOfColumnsForTablets))
                                        {
                                            objField.numberOfColumnsForTablets = int.Parse(aCurrentField.NumberOfColumnsForTablets);
                                        }
                                        objField.photoFile = aCurrentField.PhotoFile != null ? bool.Parse(aCurrentField.PhotoFile.ToString()) : false;
                                        objField.photoLibraryEnabled = aCurrentField.PhotoLibraryEnabled != null ? bool.Parse(aCurrentField.PhotoLibraryEnabled.ToString()) : false;
                                        objField.photoQuality = aCurrentField.PhotoQuality;


                                        objField.ratingLabels = aCurrentField.RatingLabels != null ? bool.Parse(aCurrentField.RatingLabels.ToString()) : false;
                                        objField.ratingMax = aCurrentField.RatingMax != null ? bool.Parse(aCurrentField.RatingMax.ToString()) : false;
                                        objField.sectionId = aCurrentField.SectionId;
                                        objField.secure = aCurrentField.Secure != null ? bool.Parse(aCurrentField.Secure.ToString()) : false;
                                        objField.showMultiselect = aCurrentField.ShowMultiselect != null ? bool.Parse(aCurrentField.ShowMultiselect.ToString()) : false;
                                        objField.showRatingLabels = aCurrentField.ShowRatingLabels != null ? bool.Parse(aCurrentField.ShowRatingLabels.ToString()) : false;

                                        if (!string.IsNullOrEmpty(aCurrentField.MaxrecordabletimeInSeconds))
                                        {
                                            objField.maxrecordabletimeInSeconds = int.Parse(aCurrentField.MaxrecordabletimeInSeconds);
                                        }
                                        //fieldName
                                        if (objField.fieldName == "NEW METER NUMBER" || objField.fieldName == "NEW METER RADIO")
                                        {
                                            objField.trackInInventory = true;
                                        }
                                        else
                                        {
                                            objField.trackInInventory = false;
                                        }

                                        //if (objField.fieldName == "WORK COMPLETED" && aCurrentField.SynchFlag != null)
                                        //{
                                        //    // aCurrentField.synchFlag = false;
                                        //   // objField.syncFlag = (bool)aCurrentField.SynchFlag;
                                        //    objForms.syncFlag = (bool)aCurrentField.SynchFlag;
                                        //}
                                        //else
                                        //{
                                        //    //objField.syncFlag = false;
                                        //    objForms.syncFlag = false;
                                        //}

                                        if (objField.fieldName == "WORK COMPLETED")
                                        {
                                            if (aCurrentField.SynchFlag != null)
                                            {
                                                objForms.syncFlag = (bool)aCurrentField.SynchFlag;
                                            }
                                            else
                                            {
                                                objForms.syncFlag = false;
                                            }
                                        }



                                        objField.videoQuality = aCurrentField.VideoQuality;

                                        List<FieldInitialDataO> FieldInitialDataList = new List<FieldInitialDataO>();
                                        objField.fieldInitialDataList = FieldInitialDataList;

                                        objListFields.Add(objField);

                                        #endregion
                                    }
                                    objSection.fieldList = objListFields;
                                    objListSection.Add(objSection);

                                    #endregion
                                }
                                objForms.sectionList = objListSection;
                                objListFormsNew.Add(objForms);


                                #endregion
                            }

                            #endregion

                            #region MenuVisit
                            List<MenuVisits3> objMenuVisitsList = new List<MenuVisits3>();

                            var currentProData = ProjectVisitDataChunk.AsEnumerable().Where(o => o.Field<long>("FKProjectId") == objNewMobileOffline.projectId).CopyToDataTable();



                            foreach (DataRow row in currentProData.Rows)
                            {

                                string StreetDynamicColumnName = checkMappedColumnsToExcel.Where(o => o.FieldName == "STREET").Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                string AccountDynamicColumnName = checkMappedColumnsToExcel.Where(o => o.FieldName == "ACCOUNT").Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                string OldMeterNoDynamicColumnName = checkMappedColumnsToExcel.Where(o => o.FieldName == "OLD METER NUMBER").Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                string OldRadioNoDynamicColumnName = checkMappedColumnsToExcel.Where(o => o.FieldName == "OLD METER RADIO NUMBER").Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                string OldMeterSizeDynamicColumnName = checkMappedColumnsToExcel.Where(o => o.FieldName == "OLD METER SIZE").Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                string OldMeterTypeDynamicColumnName = checkMappedColumnsToExcel.Where(o => o.FieldName == "OLD METER TYPE").Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();

                                int streetCount = objMenuVisitsList.Where(o => o.street == row[StreetDynamicColumnName].ToString()).Count();

                                if (streetCount == 0)
                                {

                                    var adata = distCountPro.AsEnumerable().Where(o => o.Field<string>(StreetDynamicColumnName) == row[StreetDynamicColumnName].ToString()).FirstOrDefault();

                                    if (adata != null)
                                    {

                                        int completed = adata["TotalCountWS"] != null ? int.Parse(adata["TotalCountWS"].ToString()) : 0;
                                        int skipped = adata["TotalCountSS"] != null ? int.Parse(adata["TotalCountSS"].ToString()) : 0;
                                        int rtu = adata["TotalCountRS"] != null ? int.Parse(adata["TotalCountRS"].ToString()) : 0;
                                        int totalVisits = adata["TotalCountS"] != null ? int.Parse(adata["TotalCountS"].ToString()) : 0;
                                        int appointmentVisits = adata["TotalCountAppS"] != null ? int.Parse(adata["TotalCountAppS"].ToString()) : 0;

                                        totalVisits = totalVisits - appointmentVisits;

                                        int pendingVisits = adata["TotalCountRS"] != null ? totalVisits - (completed + skipped + rtu) : 0;

                                        var getStreetNameData = currentProData.AsEnumerable().Where(o => o.Field<string>(StreetDynamicColumnName) == row[StreetDynamicColumnName].ToString()).CopyToDataTable();

                                        MenuVisits3 objMenuVisits = new MenuVisits3();
                                        objMenuVisits.pendingVisits = pendingVisits;
                                        objMenuVisits.completedVisits = completed;
                                        objMenuVisits.skippedVisits = skipped;
                                        objMenuVisits.rtuVisits = rtu;
                                        objMenuVisits.totalVisits = totalVisits;



                                        if (totalVisits == pendingVisits)
                                        {
                                            objMenuVisits.colorFlag = 1;
                                        }
                                        else if (pendingVisits > 0)
                                        {
                                            objMenuVisits.colorFlag = 2;
                                        }
                                        else if (skipped > 0 && pendingVisits == 0)
                                        {
                                            objMenuVisits.colorFlag = 3;
                                        }
                                        else if (rtu > 0 && pendingVisits == 0 && skipped == 0)
                                        {
                                            objMenuVisits.colorFlag = 4;
                                        }
                                        else if (completed == totalVisits)
                                        {
                                            objMenuVisits.colorFlag = 5;
                                        }

                                        if (objMenuVisits.colorFlag == 0)
                                        {
                                            objMenuVisits.colorFlag = 1;
                                        }

                                        objMenuVisits.street = row[StreetDynamicColumnName].ToString();
                                        objMenuVisits.visitId = row["VisitId"].ToString();
                                        objMenuVisits.groupId = row["GroupId"].ToString();
                                        objMenuVisits.hasAppointment = false;
                                        objMenuVisits.latitude = Convert.ToString(row["AddressLatitude"]);
                                        objMenuVisits.longitude = Convert.ToString(row["AddressLongitude"]);

                                        List<Accounts3> objListAccounts = new List<Accounts3>();
                                        foreach (DataRow row1 in getStreetNameData.Rows)
                                        {
                                            #region Account

                                            bool isReopen = false;
                                            var uid = (long)row1["Id"];
                                            if (uid == 137332)
                                            {
                                                var found = true;
                                            }
                                            var listReopen = getStreetNameData.AsEnumerable().Where(o => o.Field<long>("Id") == (long)row1["Id"]).ToList();

                                            if (listReopen.Count > 1)
                                            {
                                                var checkReopen = listReopen.Where(o => o.Field<int>("Reopen") == 1).FirstOrDefault();
                                                if (checkReopen != null && (int)row1["Reopen"] == 0)
                                                {
                                                    isReopen = true;
                                                }
                                            }

                                            if (!isReopen)
                                            {
                                                #region Account





                                                var accountFlag = distCountPro.AsEnumerable().Where(o => o.Field<string>(AccountDynamicColumnName) == row1[AccountDynamicColumnName].ToString() &&
                                                     o.Field<string>(OldMeterNoDynamicColumnName) == row1[OldMeterNoDynamicColumnName].ToString()).FirstOrDefault();

                                                int completedA = accountFlag != null && accountFlag["TotalCountWA"] != DBNull.Value ? int.Parse(accountFlag["TotalCountWA"].ToString()) : 0;
                                                int skippedA = accountFlag != null && accountFlag["TotalCountSA"] != DBNull.Value ? int.Parse(accountFlag["TotalCountSA"].ToString()) : 0;
                                                int rtuA = accountFlag != null && accountFlag["TotalCountRA"] != DBNull.Value ? int.Parse(accountFlag["TotalCountRA"].ToString()) : 0;
                                                int totalVisitsA = accountFlag != null && accountFlag["TotalCountA"] != DBNull.Value ? int.Parse(accountFlag["TotalCountA"].ToString()) : 0;
                                                int appointmentVisitsA = accountFlag != null && accountFlag["TotalCountAppA"] != DBNull.Value ? int.Parse(accountFlag["TotalCountAppA"].ToString()) : 0;

                                                totalVisitsA = totalVisitsA - appointmentVisitsA;

                                                int pendingVisitsA = accountFlag != null ? totalVisitsA - (completedA + skippedA + rtuA) : 0;



                                                string visitDate = accountFlag != null ? (!string.IsNullOrEmpty(Convert.ToString(accountFlag["CreatedOnA"]))
                                                    ? DateTime.Parse(Convert.ToString(accountFlag["CreatedOnA"])).ToString("MMM-dd-yyyy hh:mm tt") : "") : "";



                                                Accounts3 objAccounts = new Accounts3();
                                                objAccounts.id = (long)row1["Id"];
                                                objAccounts.accountNumber = row1[AccountDynamicColumnName].ToString();
                                                objAccounts.isActive = (int)row1["Active1"] == 0 ? false : true;
                                                objAccounts.isReOpen = (int)row1["Reopen"] == 0 ? false : true;

                                                if (!string.IsNullOrEmpty(Convert.ToString(row1["FirstPreferedDate"])))
                                                {
                                                    objAccounts.appointmentContactName = Convert.ToString(row1["FirstName"]) + " " + Convert.ToString(row1["LastName"]);
                                                    objAccounts.appointmentContactNumber = Convert.ToString(row1["DayTimePhone"]);
                                                    objAccounts.appointmentDate = DateTime.Parse(Convert.ToString(row1["FirstPreferedDate"])).ToString("MMM-dd-yyyy");
                                                    objAccounts.appointmentTime = Convert.ToString(row1["FirstTime"]);
                                                    objAccounts.hasAppointment = true;

                                                    string strinApp = objAccounts.appointmentDate + " " + objAccounts.appointmentTime;

                                                    DateTime appdt = DateTime.Parse(strinApp);

                                                    objAccounts.appointmentEpochTime = ConvertToUnixTimestamp(appdt);

                                                    appdt = appdt.AddHours(-1);

                                                    objAccounts.notificationFireDate = ConvertToUnixTimestamp(appdt);


                                                    objMenuVisits.hasAppointment = true;
                                                    objAccounts.visitExpiryDateTime = objAccounts.appointmentDate;
                                                }
                                                else
                                                {
                                                    objAccounts.hasAppointment = false;
                                                    objAccounts.visitExpiryDateTime = ((DateTime)accountFlag["ToDate"]).ToString("MMM-dd-yyyy");
                                                }



                                                if (totalVisitsA == pendingVisitsA)
                                                {
                                                    objAccounts.colorFlag = 1;
                                                }
                                                else if (pendingVisitsA > 0)
                                                {
                                                    objAccounts.colorFlag = 2;
                                                }
                                                else if (skippedA > 0 && pendingVisitsA == 0)
                                                {
                                                    objAccounts.colorFlag = 3;
                                                }
                                                else if (rtuA > 0 && pendingVisitsA == 0 && skippedA == 0)
                                                {
                                                    objAccounts.colorFlag = 4;
                                                }
                                                else if (completedA == totalVisitsA)
                                                {
                                                    objAccounts.colorFlag = 5;
                                                }

                                                if (objAccounts.colorFlag == 0)
                                                {
                                                    objAccounts.colorFlag = 1;
                                                }

                                                if (objAccounts.colorFlag == 4)
                                                {
                                                    objAccounts.isVisited = true;
                                                }
                                                else
                                                {
                                                    objAccounts.isVisited = (int)row1["Active1"] == 0 ? false : true;
                                                }



                                                objAccounts.oldMeterNumber = Convert.ToString(row1[OldMeterNoDynamicColumnName]);
                                                objAccounts.oldMeterSize = OldMeterSizeDynamicColumnName == null ? "" : Convert.ToString(row1[OldMeterSizeDynamicColumnName]);

                                                objAccounts.oldMeterType = OldMeterTypeDynamicColumnName == null ? "" : Convert.ToString(row1[OldMeterTypeDynamicColumnName]);
                                                objAccounts.oldMeterRadioNumber = OldRadioNoDynamicColumnName == null ? "" : Convert.ToString(row1[OldRadioNoDynamicColumnName]);
                                                objAccounts.uploadId = (long)row1["Id"];

                                                objAccounts.visitDateTime = visitDate;

                                                List<Forms3> objListForms = new List<Forms3>();

                                                foreach (var item3 in projectDistinctFormList)
                                                {
                                                    var adata1 = distCountPro.AsEnumerable().
                                                        Where(o => o.Field<long>("Id") == (long)row1["Id"] && o.Field<long>("formId") == item3.FormId).FirstOrDefault();

                                                    #region Form


                                                    Forms3 objForms = new Forms3();
                                                    objForms.formId = item3.FormId;
                                                    objForms.formName = item3.FormName;

                                                    bool _isSkip = false;
                                                    if (adata1 != null && (int)adata1["FormSkip"] == 1)
                                                    {
                                                        //if form is skip
                                                        objForms.isSkipped = adata1 != null ? ((int)adata1["FormSkip"] == 1 ? true : false) : false;
                                                        _isSkip = true;
                                                    }
                                                    bool _isRTU = false;
                                                    if (adata1 != null && (int)adata1["FormRTU"] == 1)
                                                    {
                                                        //if form is rtu
                                                        objForms.isSkipped = adata1 != null ? ((int)adata1["FormRTU"] == 1 ? true : false) : false;
                                                        _isRTU = true;
                                                    }

                                                    if (_isSkip == false && _isRTU == false)
                                                    {
                                                        objForms.isSkipped = false;
                                                    }

                                                    objForms.isVisited = adata1 != null ? ((int)adata1["FormComplete"] == 1 ? true : false) : false;
                                                    objForms.visitDateTime = adata1 != null ? (!string.IsNullOrEmpty(Convert.ToString(adata1["CreatedOnU"])) ?
                                                        DateTime.Parse(Convert.ToString(adata1["CreatedOnU"])).ToString("MMM-dd-yyyy hh:mm tt") : "") : "";
                                                    //DateTime.Parse(Convert.ToString(adata1.CreatedOnU)).ToString("dd-MMM-yyyy HH:MM T")

                                                    if (objForms.isSkipped)
                                                    {
                                                        long _iddd = (long)adata1["Id"];
                                                        if (_iddd == 114004)
                                                        {
                                                            var FK_UploadedExcelData_Id = _iddd;
                                                        }
                                                        long _formidd = (long)item3.FormId;
                                                        var skipOrRtuFlag = objCompassEntities.PROC_WebService_CheckRecordStatus_SkipOrRtu(_iddd, _formidd).FirstOrDefault();
                                                        //var aFieldDataRecord = objCompassEntities.TblProjectFieldDatas.Where(o => o.FormId == _formidd
                                                        //    && o.InstallerId == userId && o.FK_UploadedExcelData_Id == _iddd
                                                        //    && o.Active == 1 && o.IsSkipped == true).FirstOrDefault();

                                                        long fddId = (long)skipOrRtuFlag;
                                                        var aFieldDataRecord = objCompassEntities.TblProjectFieldDatas.Where(o => o.ID == fddId).FirstOrDefault();

                                                        if (aFieldDataRecord != null)
                                                        {
                                                            var m1 = skipReasonTable.Where(o => o.SkipId == aFieldDataRecord.SkipId).FirstOrDefault();
                                                            if (m1 != null)
                                                            {
                                                                objForms.reasonType = m1.CategoryType;
                                                                objForms.skipReason = m1.SkipReason;
                                                            }
                                                        }
                                                    }

                                                    List<Fields1> objListFields = new List<Fields1>();
                                                    if (objAccounts.colorFlag != 5 && objAccounts.colorFlag != 4)
                                                    {
                                                        var currentFormSections = projectDistinctSectionList.Where(o => o.FormId == item3.FormId).ToList();
                                                        foreach (var item4 in currentFormSections)
                                                        {
                                                            var sectionFields = projectFormData.Where(o => o.SectionId == item4.SectionId).Select(o => o).OrderBy(o => o.FieldOrder).ToList();

                                                            foreach (var item5 in sectionFields)
                                                            {
                                                                #region Fields
                                                                var aCurrentField = projectFormData.Where(o => o.FieldId == item5.FieldId).FirstOrDefault();
                                                                Fields1 objField = new Fields1();
                                                                objField.fieldId = item5.FieldId;
                                                                objField.sectionId = item4.SectionId;
                                                                objField.fieldName = item5.FieldName;

                                                                List<FieldInitialDataO> FieldInitialDataList = new List<FieldInitialDataO>();

                                                                if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER SIZE".ToLower())
                                                                {
                                                                    FieldInitialDataList = resNewMeterSizeList.Select(o => new FieldInitialDataO { initialFieldId = o.initialFieldId, initialFieldValue = o.initialFieldValue }).ToList();
                                                                }
                                                                else if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER TYPE".ToLower())
                                                                {
                                                                    FieldInitialDataList = resNewMeterTypeList.Select(o => new FieldInitialDataO { initialFieldId = o.initialFieldId, initialFieldValue = o.initialFieldValue }).ToList();
                                                                }
                                                                else if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER MAKE".ToLower())
                                                                {
                                                                    FieldInitialDataList = resMeterMakeList.Select(o => new FieldInitialDataO { initialFieldId = o.initialFieldId, initialFieldValue = o.initialFieldValue }).ToList();
                                                                }
                                                                else if (aCurrentField.FieldName.ToLower().Trim() == "SERVICES".ToLower())
                                                                {
                                                                    #region
                                                                    FieldInitialDataList = resServiceList.Where(o => o.isAdditionalService == false).Select(o =>
                                                                        new FieldInitialDataO
                                                                        {
                                                                            initialFieldId = o.initialFieldId,
                                                                            initialFieldValue = o.initialFieldValue,
                                                                            flagCaptureAudio = o.flagCaptureAudio,
                                                                            flagCaptureVideo = o.flagCaptureVideo,
                                                                            flagIsImageCaptureRequired = bool.Parse(o.flagIsImageCaptureRequired.ToString()),
                                                                            isAdditionalService = bool.Parse(o.isAdditionalService.ToString()),
                                                                            //imageList = string.IsNullOrEmpty((o.imageList)) ? "".Split(',').ToList() : (o.imageList).Split(',').ToList()
                                                                            imageList = string.IsNullOrEmpty((o.imageList)) ? null : (o.imageList).Split(',').ToList()
                                                                           .Select(o2 => new PictureList
                                                                           {
                                                                               imageId = (imageList.Where(o1 => o1.Picture.Trim() == o2.Trim()).Select(o1 => o1.ID).FirstOrDefault()),
                                                                               imageName = o2.Trim()
                                                                           }).ToList()
                                                                        }).ToList();
                                                                    #endregion
                                                                }
                                                                else if (aCurrentField.FieldName.ToLower().Trim() == "ADDITIONAL SERVICES".ToLower())
                                                                {
                                                                    #region
                                                                    FieldInitialDataList = resServiceList.Where(o => o.isAdditionalService == true).Select(o =>
                                                                        new FieldInitialDataO
                                                                        {
                                                                            initialFieldId = o.initialFieldId,
                                                                            initialFieldValue = o.initialFieldValue,
                                                                            flagCaptureAudio = o.flagCaptureAudio,
                                                                            flagCaptureVideo = o.flagCaptureVideo,
                                                                            flagIsImageCaptureRequired = bool.Parse(o.flagIsImageCaptureRequired.ToString()),
                                                                            isAdditionalService = bool.Parse(o.isAdditionalService.ToString()),
                                                                            //imageList = string.IsNullOrEmpty((o.imageList)) ? "".Split(',').ToList() : (o.imageList).Split(',').ToList()
                                                                            imageList = string.IsNullOrEmpty((o.imageList)) ? null : (o.imageList).Split(',').ToList()
                                                                           .Select(o2 => new PictureList
                                                                           {
                                                                               imageId = (imageList.Where(o1 => o1.Picture.Trim() == o2.Trim()).Select(o1 => o1.ID).FirstOrDefault()),
                                                                               imageName = o2.Trim()
                                                                           }).ToList()
                                                                        }).ToList();
                                                                    #endregion
                                                                }
                                                                else
                                                                {
                                                                    #region other fields
                                                                    var checkfield = checkMappedColumnsToExcel.Where(o => o.FieldName == aCurrentField.FieldName).FirstOrDefault();

                                                                    if (checkfield != null && checkfield.isMappedToExcel == 1 && checkfield.FieldName != "LOCATION/GPS")
                                                                    {
                                                                        FieldInitialDataO objFieldInitialData = new FieldInitialDataO();

                                                                        objFieldInitialData.initialFieldId = item5.FieldId;

                                                                        var _columnName = checkMappedColumnsToExcel.Where(o => o.FieldName.ToLower() == checkfield.FieldName.ToLower()).FirstOrDefault();

                                                                        if (_columnName != null)
                                                                        {
                                                                            objFieldInitialData.initialFieldValue = Convert.ToString(row1[_columnName.FKUploadedExcelDataDynamicColumn]);
                                                                        }


                                                                        FieldInitialDataList.Add(objFieldInitialData);
                                                                    }
                                                                    else
                                                                    {
                                                                        var res = objCompassEntities.tblProjectFieldDDLMasters.
                                                                            Where(o => o.FormSectionFieldID == aCurrentField.FieldId && o.Active == 1).ToList();
                                                                        FieldInitialDataList = res.Select(o => new FieldInitialDataO { initialFieldId = o.ID, initialFieldValue = o.FieldValue }).ToList();
                                                                    }
                                                                    #endregion
                                                                }


                                                                objField.fieldInitialDataList = FieldInitialDataList;

                                                                objListFields.Add(objField);

                                                                #endregion
                                                            }
                                                        }
                                                    }

                                                    objForms.fieldList = objListFields;
                                                    objListForms.Add(objForms);


                                                    #endregion
                                                }
                                                objAccounts.formList = objListForms;
                                                objListAccounts.Add(objAccounts);

                                                #endregion
                                            }

                                            #endregion
                                        }

                                        objMenuVisits.objAccounts = objListAccounts;

                                        var _totalAccount = objListAccounts.Count;
                                        var _deactivateAccount = objListAccounts.Where(o => o.isActive == false).Count();
                                        objMenuVisits.isActive = (_deactivateAccount > 0 && _deactivateAccount == _totalAccount) ? false : true;

                                        objMenuVisitsList.Add(objMenuVisits);

                                    }
                                }

                            }


                            #endregion

                            #region Map Visit
                            List<MapVisits1> objListMapVisits = new List<MapVisits1>();

                            foreach (DataRow item1 in currentProData.Rows)
                            {
                                var checkPresent = objListMapVisits.Where(o => o.latitude == Convert.ToString(item1["AddressLatitude"]) &&
                                    o.longitude == Convert.ToString(item1["AddressLongitude"])).FirstOrDefault();

                                if (checkPresent == null)
                                {

                                    var allMatchingLatLonList = currentProData.AsEnumerable().
                                        Where(o => o.Field<string>("AddressLatitude") == item1["AddressLatitude"].ToString() &&
                                        o.Field<string>("AddressLongitude") == item1["AddressLongitude"].ToString()
                                        && o.Field<long>("FKProjectId") == (long)item1["FKProjectId"]).ToList();


                                    var allAppointmentList = currentProData.AsEnumerable().
                                       Where(o => o.Field<string>("AddressLatitude") == item1["AddressLatitude"].ToString() &&
                                       o.Field<string>("AddressLongitude") == item1["AddressLongitude"].ToString()
                                       && o.Field<long>("FKProjectId") == (long)item1["FKProjectId"] &&
                                       o.Field<DateTime?>("FirstPreferedDate") != null).ToList();

                                    string StreetDynamicColumnName = checkMappedColumnsToExcel.Where(o => o.FieldName == "STREET").
                                        Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();


                                    List<AppointmentDetails> objList = new List<AppointmentDetails>();
                                    if (allAppointmentList != null && allAppointmentList.Count > 0)
                                    {

                                        foreach (var itemApp in allAppointmentList)
                                        {

                                            AppointmentDetails obj = new AppointmentDetails();
                                            obj.accountNumber = Convert.ToString(itemApp.Field<string>("Account12"));
                                            obj.appointmentDate = DateTime.Parse(Convert.ToString(itemApp.Field<DateTime?>("FirstPreferedDate"))).ToString("MMM-dd-yyyy");
                                            obj.appointmentTime = Convert.ToString(itemApp.Field<string>("FirstTime"));
                                            obj.appointmentContactNumber = Convert.ToString(itemApp.Field<string>("DayTimePhone"));
                                            obj.appointmentContactName = Convert.ToString(itemApp.Field<string>("FirstName")) + " " + Convert.ToString(itemApp.Field<string>("LastName"));
                                            obj.id = Convert.ToInt64(itemApp.Field<long>("Id"));
                                            objList.Add(obj);
                                        }

                                    }



                                    if (allMatchingLatLonList.Count == 1)
                                    {
                                        #region single record

                                        var adata = distCountPro.AsEnumerable().Where(o => o.Field<string>("AddressLatitude") == item1["AddressLatitude"].ToString() &&
                                            o.Field<string>("AddressLongitude") == item1["AddressLongitude"].ToString()
                                            && o.Field<long>("FKProjectId") == (long)item1["FKProjectId"]).FirstOrDefault();

                                        int completed = adata != null && adata["TotalCountWL"] != null ? int.Parse(adata["TotalCountWL"].ToString()) : 0;
                                        int skipped = adata != null && adata["TotalCountSL"] != null ? int.Parse(adata["TotalCountSL"].ToString()) : 0;
                                        int rtu = adata != null && adata["TotalCountRL"] != null ? int.Parse(adata["TotalCountRL"].ToString()) : 0;
                                        int totalVisits = adata != null && adata["TotalCountL"] != null ? int.Parse(adata["TotalCountL"].ToString()) : 0;
                                        int AppointmentVisits = adata != null && adata["TotalCountAppL"] != null ? int.Parse(adata["TotalCountAppL"].ToString()) : 0;

                                        totalVisits = totalVisits - AppointmentVisits;
                                        int pendingVisits = totalVisits - (completed + skipped + rtu);


                                        MapVisits1 objMapVisits = new MapVisits1();
                                        objMapVisits.groupId = item1["GroupId"].ToString();


                                        if (totalVisits == pendingVisits)
                                        {
                                            objMapVisits.colorFlag = 1;
                                        }
                                        else if (pendingVisits > 0)
                                        {
                                            objMapVisits.colorFlag = 2;
                                        }
                                        else if (skipped > 0 && pendingVisits == 0)
                                        {
                                            objMapVisits.colorFlag = 3;
                                        }
                                        else if (rtu > 0 && pendingVisits == 0 && skipped == 0)
                                        {
                                            objMapVisits.colorFlag = 4;
                                        }

                                        else if (completed == totalVisits)
                                        {
                                            objMapVisits.colorFlag = 5;
                                        }


                                        if (AppointmentVisits > 0)
                                        {
                                            objMapVisits.hasAppointment = true;
                                            objMapVisits.appointmentList = objList;
                                            objMapVisits.colorFlag = 1;
                                        }
                                        else
                                        {
                                            objMapVisits.hasAppointment = false;
                                        }

                                        LocationCycleRoute cycleRoute = getCycleRoute((long)item1["Id"], (long)item1["FKProjectId"]);

                                        objMapVisits.street = item1[StreetDynamicColumnName].ToString();
                                        objMapVisits.latitude = Convert.ToString(item1["AddressLatitude"]);
                                        objMapVisits.longitude = Convert.ToString(item1["AddressLongitude"]);
                                        objMapVisits.completedVisits = completed;
                                        objMapVisits.pendingVisits = pendingVisits;
                                        objMapVisits.skippedVisits = skipped;
                                        objMapVisits.totalVisits = totalVisits;
                                        objMapVisits.isActive = (int)item1["Active"] == 1 ? true : false;
                                        objMapVisits.UpdatedDate = Convert.ToString(item1["ModifiedOn"]);
                                        objMapVisits.cycle = cycleRoute.cycle;
                                        objMapVisits.route = cycleRoute.route;
                                        objListMapVisits.Add(objMapVisits);

                                        #endregion
                                    }
                                    else
                                    {
                                        #region multiple records
                                        var adata = distCountPro.AsEnumerable().Where(o => o.Field<string>("AddressLatitude") == item1["AddressLatitude"].ToString() &&
                                              o.Field<string>("AddressLongitude") == item1["AddressLongitude"].ToString()
                                            && o.Field<long>("FKProjectId") == (long)item1["FKProjectId"]).FirstOrDefault();

                                        int completed = adata != null && adata["TotalCountWL"] != null ? int.Parse(adata["TotalCountWL"].ToString()) : 0;
                                        int skipped = adata != null && adata["TotalCountSL"] != null ? int.Parse(adata["TotalCountSL"].ToString()) : 0;
                                        int rtu = adata != null && adata["TotalCountRL"] != null ? int.Parse(adata["TotalCountRL"].ToString()) : 0;
                                        int totalVisits = adata != null && adata["TotalCountL"] != null ? int.Parse(adata["TotalCountL"].ToString()) : 0;
                                        int AppointmentVisits = adata != null && adata["TotalCountAppL"] != null ? int.Parse(adata["TotalCountAppL"].ToString()) : 0;

                                        totalVisits = totalVisits - AppointmentVisits;
                                        int pendingVisits = totalVisits - (completed + skipped + rtu);

                                        var checkAddressList = allMatchingLatLonList.AsEnumerable().Select(o => o[StreetDynamicColumnName].ToString()).Distinct().ToList();

                                        IEnumerable<string> s1 = StringExtensions.GetMostCommonSubstrings(checkAddressList);

                                        string swd = s1.FirstOrDefault();

                                        MapVisits1 objMapVisits = new MapVisits1();
                                        objMapVisits.groupId = item1["GroupId"].ToString();

                                        if (totalVisits == pendingVisits)
                                        {
                                            objMapVisits.colorFlag = 1;
                                        }
                                        else if (pendingVisits > 0)
                                        {
                                            objMapVisits.colorFlag = 2;
                                        }
                                        else if (skipped > 0 && pendingVisits == 0)
                                        {
                                            objMapVisits.colorFlag = 3;
                                        }
                                        else if (rtu > 0 && pendingVisits == 0 && skipped == 0)
                                        {
                                            objMapVisits.colorFlag = 4;
                                        }
                                        else if (completed == totalVisits)
                                        {
                                            objMapVisits.colorFlag = 5;
                                        }

                                        if (AppointmentVisits > 0)
                                        {
                                            objMapVisits.hasAppointment = true;
                                            objMapVisits.appointmentList = objList;
                                            objMapVisits.colorFlag = objMapVisits.colorFlag == 0 ? 1 : objMapVisits.colorFlag;
                                        }
                                        else
                                        {
                                            objMapVisits.hasAppointment = false;
                                        }

                                        LocationCycleRoute cycleRoute = getCycleRoute((long)item1["Id"], (long)item1["FKProjectId"]);
                                        objMapVisits.street = swd;
                                        objMapVisits.latitude = Convert.ToString(item1["AddressLatitude"]);
                                        objMapVisits.longitude = Convert.ToString(item1["AddressLongitude"]);
                                        objMapVisits.completedVisits = completed;
                                        objMapVisits.pendingVisits = pendingVisits;
                                        objMapVisits.rtuVisits = rtu;
                                        objMapVisits.skippedVisits = skipped;
                                        objMapVisits.totalVisits = totalVisits;
                                        objMapVisits.cycle = cycleRoute.cycle;
                                        objMapVisits.route = cycleRoute.route;


                                        var _totalAccount = currentProData.AsEnumerable().Where(o => o.Field<string>("AddressLatitude") == item1["AddressLatitude"].ToString() &&
                                              o.Field<string>("AddressLongitude") == item1["AddressLongitude"].ToString()
                                            && o.Field<long>("FKProjectId") == (long)item1["FKProjectId"]).Count();
                                        var _deactivateAccount = currentProData.AsEnumerable().Where(o => o.Field<string>("AddressLatitude") == item1["AddressLatitude"].ToString() &&
                                              o.Field<string>("AddressLongitude") == item1["AddressLongitude"].ToString()
                                            && o.Field<long>("FKProjectId") == (long)item1["FKProjectId"]
                                        && o.Field<int>("Active1") == 0).Count();

                                        objMapVisits.isActive = (_deactivateAccount == _totalAccount && _deactivateAccount > 0) ? false : true;

                                        objListMapVisits.Add(objMapVisits);


                                        #endregion
                                    }
                                }

                            }


                            #endregion


                            objNewMobileOffline.objFormStructure = objListFormsNew;
                            objNewMobileOffline.objMenuVisits = objMenuVisitsList;
                            objNewMobileOffline.objMapVisits = objListMapVisits;
                            objNewMobileOffline.skipReasonList = skipList.Select(o => new SkippReason
                            {
                                skipId = o.SkipId,
                                skipReason = o.SkipReason,
                                isAudioRequired = (o.IsAudioRequired == null ? false : bool.Parse(o.IsAudioRequired.ToString())),
                                isImageRequired = (o.IsImageRequired == null ? false : bool.Parse(o.IsImageRequired.ToString())),
                                isVideoRequired = (o.IsVideoRequired == null ? false : bool.Parse(o.IsVideoRequired.ToString()))
                            }).ToList();
                            objNewMobileOffline.rtuReasonList = rtuList.Select(o => new RTUReason
                            {
                                rtuId = o.SkipId,
                                rtuReason = o.SkipReason,
                                isAudioRequired = (o.IsAudioRequired == null ? false : bool.Parse(o.IsAudioRequired.ToString())),
                                isImageRequired = (o.IsImageRequired == null ? false : bool.Parse(o.IsImageRequired.ToString())),
                                isVideoRequired = (o.IsVideoRequired == null ? false : bool.Parse(o.IsVideoRequired.ToString()))

                            }).ToList();
                            objListMobileOffline.Add(objNewMobileOffline);
                        }

                    }

                    JavaScriptSerializer s = new JavaScriptSerializer();
                    s.MaxJsonLength = 2147483644;
                    // s.Serialize(objListMobileOffline);
                    int currentRecords = 0;
                    if (objListMobileOffline != null)
                    {
                        foreach (var item in objListMobileOffline)
                        {
                            foreach (var item1 in item.objMenuVisits)
                            {
                                currentRecords = currentRecords + item1.objAccounts.Count();
                            }
                        }
                    }
                    string json1 = s.Serialize(objListMobileOffline);

                    string m2 = GetMd5Sum(json1);
                    mobileOfflineResult.checkSum = m2;
                    mobileOfflineResult.data = objListMobileOffline;
                    mobileOfflineResult.batchNumber = BatchNumber;
                    mobileOfflineResult.batchIndicator = checkRecordAvilableForNextTrip;
                    mobileOfflineResult.chunkSize = currentRecords;
                    mobileOfflineResult.totalRecord = chunkRecordCount;
                    mobileOfflineResult.appointmentURL = "https://www.datadepotonline.com/Admin/Appointment";


                    #region set checksum & BatchNumber
                    objCompassEntities = new CompassEntities();
                    foreach (DataRow item in ProjectVisitDataChunk.Rows)
                    {
                        long id = (long)item["Id"];
                        var objInstallerMap = objCompassEntities.tblInstallerMapDatas.
                            Where(o => o.FK_UploadedExcelData_Id == id && o.InstallerId == userId && o.Active == 1).FirstOrDefault();
                        if (objInstallerMap != null)
                        {
                            objInstallerMap.BatchNumber = BatchNumber;
                            objInstallerMap.SynchCheckSumString = m2;
                            objInstallerMap.SynchWithMobileFlag = 1;
                            objInstallerMap.SynchWithMobileDate = new CommonFunctions().ServerDate();
                            //objCompassEntities.SaveChanges();
                        }
                    }
                    if (ProjectVisitDataChunk.Rows.Count > 0)
                    {
                        objCompassEntities.SaveChanges();
                    }
                    #endregion
                    //get last complete position of user project's
                    var timeElapsed = sw.Elapsed;
                    if (!checkRecordAvilableForNextTrip)
                    {
                        var projectLastLocation = getLastCompleteLocation(userId);
                        if (projectLastLocation.Count > 0)
                        {
                            mobileOfflineResult.projectLastLocation = projectLastLocation;
                        }
                    }
                    mobileOfflineResult.totalTime = sw.Elapsed;

                    return mobileOfflineResult;


                    #endregion
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


        private double ConvertToUnixTimestamp(DateTime date)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(date) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;

        }


        private static List<STP_OfflineDataSP1_Result> GetFormDataList(long userId, DateTime strCurrentDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var FormDataList = objCompassEntities.STP_OfflineDataSP1(userId, strCurrentDate).ToList();
            return FormDataList;
        }

        private static List<PROC_WebService_Installer_OfflineDataSP1_Result> GetDynamicFormDataList(long userId, DateTime strCurrentDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var FormDataList = objCompassEntities.PROC_WebService_Installer_OfflineDataSP1(userId, strCurrentDate).ToList();
            return FormDataList;
        }


        private static List<PROC_WebService_Auditor_OfflineDataSP1_Result> GetAuditDynamicFormDataList(long userId, DateTime strCurrentDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var FormDataList = objCompassEntities.PROC_WebService_Auditor_OfflineDataSP1(userId, strCurrentDate).ToList();
            return FormDataList;
        }


        /// <summary>
        /// 2016-10-3 Devloper- Bharat
        /// returns batchnumber for the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="strCurrentDate"></param>
        /// <param name="batchNumber"></param>
        /// <param name="BatchNumber"></param>
        /// <param name="sendAlreadySentRecord"></param>
        /// <returns></returns>
        private bool GetBatchNumber(long userId, DataTable dt, DateTime strCurrentDate, long batchNumber, ref long BatchNumber, ref bool sendAlreadySentRecord)
        {
            bool WrongBatchNumber = false;

            #region Batch number logic

            if (batchNumber == 0)
            {
                ///if batch number=0
                GetBatchNo(userId, dt, strCurrentDate, ref BatchNumber, ref sendAlreadySentRecord);
            }
            else
            {
                if (batchNumber > 1)
                {
                    ///if batch number greater than 1
                    GetBatchNo1(userId, batchNumber, ref BatchNumber, ref sendAlreadySentRecord, ref WrongBatchNumber);
                }
                else
                {
                    ///if batch number =1
                    BatchNumber = GetBatchNo2(userId, dt, strCurrentDate, ref BatchNumber);
                }
            }
            #endregion
            return WrongBatchNumber;
        }

        /// <summary>
        /// 2016-10-3 Devloper- Bharat
        /// Returns the batch number when request batch number is equal to 1
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="strCurrentDate"></param>
        /// <param name="BatchNumber"></param>
        /// <returns></returns>
        private long GetBatchNo2(long userId, DataTable dt, DateTime strCurrentDate, ref long BatchNumber)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            var checkB = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId
               && o.Active == 1).Select(o => o.BatchNumber).Max();

            // if batch no > 1 means already send data present for the installer

            if (checkB >= 1)
            {
                // First update the old resend data flag for the current time period.                    

                //var ProVisitData = objCompassEntities.STP_OfflineDataSP(userId, strCurrentDate).ToList();

                // CompassEntities objCompassEntities1 = new CompassEntities();
                foreach (DataRow row in dt.Rows)
                {
                    long UploadId = long.Parse(row["Id"].ToString());
                    var objInstallerMap = objCompassEntities.tblInstallerMapDatas.Where(o => o.FK_UploadedExcelData_Id == UploadId &&
                         o.InstallerId == userId && o.Active == 1).FirstOrDefault();
                    if (objInstallerMap != null)
                    {
                        objInstallerMap.BatchNumber = null;
                        objInstallerMap.SynchCheckSumString = null;
                        objInstallerMap.SynchWithMobileFlag = null;
                        objInstallerMap.SynchWithMobileDate = null;
                        objInstallerMap.MobileResponse = null;
                        objInstallerMap.MobileResponseDate = null;
                        //objCompassEntities.SaveChanges();
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    objCompassEntities.SaveChanges();
                }


                // get the new batchnumber for the installer 
                checkB = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId && o.Active == 1).Select(o => o.BatchNumber).Max();
                BatchNumber = checkB != null ? long.Parse(checkB.ToString()) + 1 : 1;
            }
            else
            {
                BatchNumber = 1;
            }
            return BatchNumber;
        }

        /// <summary>
        /// 2016-10-3 Devloper- Bharat
        /// Returns the batch number when request batch number is > 1
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="batchNumber"></param>
        /// <param name="BatchNumber"></param>
        /// <param name="sendAlreadySentRecord"></param>
        /// <param name="WrongBatchNumber"></param>
        private void GetBatchNo1(long userId, long batchNumber, ref long BatchNumber, ref bool sendAlreadySentRecord, ref bool WrongBatchNumber)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            /// Check current batch number-1 present in database for the installer.
            if (batchNumber == 15)
            {
                var debug = true;
            }
            var objInstallerMapList = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId
                && o.BatchNumber == (batchNumber - 1) && o.SynchWithMobileFlag == 1 && o.MobileResponse != 1 && o.Active == 1).ToList();

            if (objInstallerMapList != null && objInstallerMapList.Count > 0)
            {
                /// if  record present & it's mobileresponse flag is not true 
                /// then get these record & set the flag to true.    
                foreach (var instItemList in objInstallerMapList)
                {
                    instItemList.MobileResponse = 1;
                    instItemList.MobileResponseDate = new CommonFunctions().ServerDate();
                    //objCompassEntities.SaveChanges();
                }
                objCompassEntities.SaveChanges();
                // set new batch number value.
                BatchNumber = batchNumber;
            }
            else
            {
                /// if  record not present then check the current batch present with mobile response false for the user 
                objInstallerMapList = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId
                && o.BatchNumber == batchNumber && o.SynchWithMobileFlag == 1 && o.MobileResponse != 1 && o.Active == 1).ToList();

                if (objInstallerMapList != null && objInstallerMapList.Count > 0)
                {
                    ///if record found then send already sent batch for user.
                    sendAlreadySentRecord = true;
                    BatchNumber = batchNumber;
                }
                else
                {
                    ///if not found any record then select the max batch number from the datbase
                    ///for the user & check (max+1)!= requested batch number     
                    var checkB = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId
                   && o.Active == 1).Select(o => o.BatchNumber).Max();

                    if ((checkB + 1) != batchNumber)
                    {
                        // then rejwct the request for the user
                        WrongBatchNumber = true;
                    }
                    else
                    {
                        //set the new batch number as requested batch number is equal to max +1 
                        BatchNumber = long.Parse(checkB.ToString()) + 1;
                    }
                }
            }
        }

        /// <summary>
        /// 2016-10-3 Devloper- Bharat
        /// Returns the batch number when request batch number is equal to 0
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="strCurrentDate"></param>
        /// <param name="BatchNumber"></param>
        /// <param name="sendAlreadySentRecord"></param>
        private void GetBatchNo(long userId, DataTable dt, DateTime strCurrentDate, ref long BatchNumber, ref bool sendAlreadySentRecord)
        {
            ///get incremental data or data from begining

            CompassEntities objCompassEntities = new CompassEntities();
            // send the incremental data for the installer
            BatchNumber = 0;

            //first check any batch number present for the user
            var checkB = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId && o.BatchNumber != null && o.Active ==1).Select(o => o.BatchNumber).FirstOrDefault();


            if (checkB == null)
            {
                //user first time log in no batch number present in the database for user.                    
                //send fresh new batch data                    
                BatchNumber = 1;
            }
            else
            {
                //get all data for given date so we can check if any data pending in partially synch
                //i.e. data is sent to mobile & no response got from mobile.

                //checkB = dt.AsEnumerable().Where(o => o.Field<int?>("SynchWithMobileFlag") == 1 && o.Field<int?>("MobileResponse") != 1).
                //    Select(o => o.Field<int?>("BatchNumber")).FirstOrDefault();


                var a = dt.Columns["MobileResponse"].DataType;
                var b = dt.Columns["SynchWithMobileFlag"].DataType;
                var c = dt.Columns["BatchNumber"].DataType;
                checkB = dt.AsEnumerable().Where(o => o.Field<Int32?>("SynchWithMobileFlag") == 1 && o.Field<Int32?>("MobileResponse") != 1).
                 Select(o => o.Field<Int64>("BatchNumber")).FirstOrDefault();
                if (checkB != null && checkB != 0)
                {
                    //data is sent to mobile & no response got from mobile.
                    // then resend this data to mobile for the installer.
                    sendAlreadySentRecord = true;
                    BatchNumber = long.Parse(checkB.ToString());

                }
                else
                {

                    checkB = objCompassEntities.tblInstallerMapDatas.Where(o => o.InstallerId == userId && o.Active==1).Select(o => o.BatchNumber).Max();
                    BatchNumber = long.Parse(checkB.ToString()) + 1;
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public List<PROC_UserNotification_Result> UserNotificationData(long userId, DateTime currentDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();


            var notificationData = objCompassEntities.PROC_UserNotification(userId, currentDate).ToList();


            return notificationData;
        }






        /// <summary>
        /// 2016-12-22 Devloper- Bharat
        /// Returns offline data for given user.
        /// </summary>
        /// <param name="userId">Current userId</param>
        /// <param name="strCurrentDate">Mobile current date</param>
        /// <param name="batchNumber">batch number</param>
        /// <returns>List </returns>
        public MobileOfflineAuditResult OfflineAuditData(long userId, DateTime strCurrentDate, long batchNumber)
        {
            try
            {

                MobileOfflineAuditResult mobileOfflineResult = new MobileOfflineAuditResult();

                List<AuditMobile> objListMobileOffline = new List<AuditMobile>();


                long BatchNumber = 0;
                bool sendAlreadySentRecord = false;
                bool checkRecordAvilableForNextTrip = false;
                CompassEntities objCompassEntities = new CompassEntities();

                DataTable dt = new DataTable();


                DataTable countDT = new DataTable();


                using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("PROC_WebService_Auditor_OfflineData", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@AuditorId", userId);
                        cmd.Parameters.Add("@strCurrentDate", strCurrentDate);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("PROC_WebService_Auditor_OfflineCountSP", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@AuditorId", userId);
                        cmd.Parameters.Add("@strCurrentDate", strCurrentDate);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(countDT);
                        }
                    }
                }



                //returns current batch number for the user.
                bool WrongBatchNumber = GetAuditBatchNumber(userId, dt, strCurrentDate, batchNumber, ref BatchNumber, ref sendAlreadySentRecord);



                dt = new DataTable();
                using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("PROC_WebService_Auditor_OfflineData", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@AuditorId", userId);
                        cmd.Parameters.Add("@strCurrentDate", strCurrentDate);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }


                if (!WrongBatchNumber)
                {
                    #region  send records



                    // get all the image list, skip reasons list, rtu reason list & user data. 
                    var skipReasonTable = objCompassEntities.tblSkipReasonMasters.Where(o => o.Active == 1).ToList();
                    var imageList = objCompassEntities.tblPictures.ToList();
                    var skipList = skipReasonTable.Where(o => o.CategoryType.ToLower() == "skip").ToList();
                    //  var rtuList = skipReasonTable.Where(o => o.CategoryType.ToLower() == "rtu").ToList();
                    //var ProjectVisitData = objCompassEntities.STP_OfflineDataSP(userId, strCurrentDate).ToList();
                    //var projectCountLogic = objCompassEntities.PROC_OfflineCountSP(userId, strCurrentDate).ToList();
                    int totalCount = dt.AsEnumerable().Where(o => (o.Field<int>("Active1") == 1 || o.Field<int>("Active1") == 0)
                        && (o.Field<int?>("MobileResponse") == null || o.Field<int?>("MobileResponse") == 0)).Count();
                    int chunkRecordCount = dt.AsEnumerable().Where(o => (o.Field<int>("Active1") == 1 || o.Field<int>("Active1") == 0)
                        && (o.Field<int?>("MobileResponse") == null || o.Field<int?>("MobileResponse") == 0) && o.Field<int?>("Reopen") == 0).Select(o => o).ToList().Count();



                    DataTable ProjectVisitDataChunk = new DataTable();

                    if (sendAlreadySentRecord == true)
                    {
                        var dt1 = dt.AsEnumerable().Where(o => o.Field<Int64?>("BatchNumber") == BatchNumber).ToList();
                        ProjectVisitDataChunk = dt1.Where(o => o.Field<Int32?>("MobileResponse") != 1).Select(o => o).Take(50).CopyToDataTable();
                    }
                    else
                    {
                        var _data = dt.AsEnumerable().Where(o => (o.Field<int>("Active1") == 1 || o.Field<int>("Active1") == 0)
                            && (o.Field<int?>("MobileResponse") == null || o.Field<int?>("MobileResponse") == 0)).Select(o => o).Take(50).ToList();
                        if (_data == null || _data.Count == 0)
                        {
                            JavaScriptSerializer s1 = new JavaScriptSerializer();
                            s1.MaxJsonLength = 2147483644;
                            string json = s1.Serialize(objListMobileOffline);

                            string m = GetMd5Sum(json);
                            mobileOfflineResult.checkSum = m;
                            mobileOfflineResult.data = objListMobileOffline;
                            mobileOfflineResult.batchNumber = BatchNumber;
                            mobileOfflineResult.batchIndicator = false;
                            mobileOfflineResult.chunkSize = 0;
                            mobileOfflineResult.totalRecord = 0;
                            return mobileOfflineResult;
                        }
                        else
                        {
                            ProjectVisitDataChunk = _data.CopyToDataTable();
                        }
                    }

                    checkRecordAvilableForNextTrip = (ProjectVisitDataChunk == null || ProjectVisitDataChunk.Rows.Count == 0) ? false : ((totalCount - 50) > 0 ? true : false);

                    var FormDataList = GetAuditDynamicFormDataList(userId, strCurrentDate);

                    if (dt != null)
                    {
                        var distPro = ProjectVisitDataChunk.AsEnumerable().
                            Select(o => new
                            {
                                ProjectId = o.Field<long>("ProjectId"),
                                ProjectName = o.Field<string>("ProjectName"),
                                AutoDataSyncInterval = o.Field<int?>("AutoDataSyncInterval"),
                                DataSyncConnection = o.Field<int?>("DataSyncConnection")
                            }).Distinct().ToList();


                        foreach (var item in distPro)
                        {
                            var checkRecordPresent = countDT.AsEnumerable().Where(o => o.Field<long>("FKProjectId") == item.ProjectId).Count();

                            if (checkRecordPresent > 0)
                            {
                                var projectData = objCompassEntities.tblProjects.Where(o => o.ProjectId == item.ProjectId).FirstOrDefault();

                                var ProjectNewDataInfo = objCompassEntities.PROC_Audit_GetInfo(item.ProjectId).ToList();


                                AuditMobile objNewMobileOffline = new AuditMobile();
                                objNewMobileOffline.processId = 2;
                                objNewMobileOffline.projectId = item.ProjectId;
                                objNewMobileOffline.projectName = item.ProjectName;
                                objNewMobileOffline.dataSyncConnection = !string.IsNullOrEmpty(Convert.ToString(item.DataSyncConnection)) ? int.Parse(item.DataSyncConnection.ToString()) : 3;
                                objNewMobileOffline.autoDataSyncInterval = !string.IsNullOrEmpty(Convert.ToString(item.AutoDataSyncInterval)) ? int.Parse(item.AutoDataSyncInterval.ToString()) : 5;
                                objNewMobileOffline.synchBasicInfoOnly = !string.IsNullOrEmpty(Convert.ToString(projectData.PartialSynch)) ?
                                    (int.Parse(projectData.PartialSynch.ToString()) == 1 ? true : false) : false;

                                long projectId = item.ProjectId;

                                var distCountPro = countDT.AsEnumerable().Where(o => o.Field<long>("FKProjectId") == projectId).CopyToDataTable();
                                var projectFormData = FormDataList.Where(o => o.ProjectID == projectId).ToList();
                                var projectDistinctFormList = projectFormData.Select(o => new { o.FormId, o.FormName }).Distinct().ToList();
                                var projectDistinctSectionList = projectFormData.Select(o =>
                                    new
                                    {
                                        FormId = o.FormId,
                                        FormName = o.FormName,
                                        SectionId = o.SectionId,
                                        SectionName = o.SectionName,
                                        SortOrderNumber = o.SortOrderNumber
                                    }
                                        ).Distinct().OrderBy(o => o.SortOrderNumber).ToList();


                                var resNewMeterSizeList = objCompassEntities.STP_OfflineDataSP2(projectId, "tblMeterSize").ToList();
                                var resNewMeterTypeList = objCompassEntities.STP_OfflineDataSP3(projectId, "tblMeterType").ToList();
                                var resServiceList = objCompassEntities.STP_OfflineDataSP4(projectId, "tblService").ToList();
                                var resMeterMakeList = objCompassEntities.STP_OfflineDataSP5(projectId, "tblMeterMake").ToList();
                                var checkMappedColumnsToExcel = objCompassEntities.PROC_WebService_GetFieldInfoForProject(projectId).ToList();


                                #region Form Visits

                                List<AuditForms> objListFormsNew = new List<AuditForms>();

                                foreach (var item3 in projectDistinctFormList)
                                {
                                    #region Form
                                    AuditForms objForms = new AuditForms();
                                    objForms.formId = item3.FormId;
                                    objForms.formName = item3.FormName;

                                    List<AuditSection> objListSection = new List<AuditSection>();

                                    foreach (var item4 in projectDistinctSectionList.Where(o => o.FormId == item3.FormId))
                                    {
                                        #region section
                                        var sectionFields = projectFormData.Where(o => o.SectionId == item4.SectionId).Select(o => o).OrderBy(o => o.FieldOrder).ToList();
                                        AuditSection objSection = new AuditSection();
                                        objSection.sectionId = item4.SectionId;
                                        objSection.sectionName = item4.SectionName;
                                        objSection.sectionOrder = item4.SortOrderNumber;

                                        List<AuditFields> objListFields = new List<AuditFields>();

                                        foreach (var item5 in sectionFields)
                                        {
                                            #region Field
                                            var aCurrentField = projectFormData.Where(o => o.FieldId == item5.FieldId).FirstOrDefault();
                                            AuditFields objField = new AuditFields();
                                            objField.acknowlegementButtonText = aCurrentField.AcknowlegementButtonText;
                                            objField.acknowlegementClickedButtonText = aCurrentField.AcknowlegementClickedButtonText;
                                            objField.acknowlegementText = aCurrentField.AcknowlegementText;
                                            objField.allowAnnotation = aCurrentField.AllowAnnotation != null ? bool.Parse(aCurrentField.AllowAnnotation.ToString()) : false;
                                            objField.allowMultiSelection = aCurrentField.AllowMultiSelection != null ? bool.Parse(aCurrentField.AllowMultiSelection.ToString()) : false;
                                            objField.allowNotApplicable = aCurrentField.AllowNotApplicable != null ? bool.Parse(aCurrentField.AllowNotApplicable.ToString()) : false;
                                            objField.allowprii = aCurrentField.allowprii != null ? bool.Parse(aCurrentField.allowprii.ToString()) : false;
                                            objField.audioQuality = aCurrentField.AudioQuality;
                                            objField.cameraEnabled = aCurrentField.CameraEnabled != null ? bool.Parse(aCurrentField.CameraEnabled.ToString()) : false;
                                            objField.captureGeolocation = aCurrentField.CaptureGeolocation != null ? bool.Parse(aCurrentField.CaptureGeolocation.ToString()) : false;
                                            objField.captureMode = aCurrentField.CaptureMode;
                                            objField.captureTimestamp = aCurrentField.CaptureTimestamp != null ? bool.Parse(aCurrentField.CaptureTimestamp.ToString()) : false;
                                            objField.decimalPositions = aCurrentField.DecimalPositions;
                                            objField.defaultValue = aCurrentField.DefaultValue;
                                            objField.hideFieldLabel = aCurrentField.HideFieldLabel != null ? bool.Parse(aCurrentField.HideFieldLabel.ToString()) : false;
                                            objField.displayMask = aCurrentField.DisplayMask;
                                            objField.enforceMinMax = aCurrentField.EnforceMinMax != null ? bool.Parse(aCurrentField.EnforceMinMax.ToString()) : false;
                                            objField.excludeonSync = aCurrentField.ExcludeonSync != null ? bool.Parse(aCurrentField.ExcludeonSync.ToString()) : false;



                                            objField.fieldDataTypeName = aCurrentField.FieldDataTypeName;
                                            objField.fieldFilterkey = aCurrentField.FieldFilterkey;
                                            objField.fieldId = aCurrentField.FieldId;
                                            objField.fieldLabel = aCurrentField.FieldLabel;
                                            objField.fieldName = aCurrentField.FieldName;
                                            objField.fieldOrder = aCurrentField.FieldOrder;

                                            objField.formatMask = aCurrentField.FormatMask;
                                            objField.gpsTagging = aCurrentField.GPSTagging != null ? bool.Parse(aCurrentField.GPSTagging.ToString()) : false;
                                            objField.hasAlert = aCurrentField.HasAlert != null ? bool.Parse(aCurrentField.HasAlert.ToString()) : false;
                                            objField.hasFile = aCurrentField.HasFile != null ? bool.Parse(aCurrentField.HasFile.ToString()) : false;
                                            objField.hasValues = aCurrentField.HasValues != null ? bool.Parse(aCurrentField.HasValues.ToString()) : false;
                                            objField.hintText = aCurrentField.HintText;
                                            objField.isEnabled = aCurrentField.IsEnabled != null ? bool.Parse(aCurrentField.IsEnabled.ToString()) : false;
                                            objField.isRequired = aCurrentField.IsRequired != null ? bool.Parse(aCurrentField.IsRequired.ToString()) : false;
                                            objField.keyboardType = aCurrentField.KeyboardType != null ? bool.Parse(aCurrentField.KeyboardType.ToString()) : false;
                                            if (!string.IsNullOrEmpty(aCurrentField.MaximumHeight))
                                            {
                                                objField.maximumHeight = int.Parse(aCurrentField.MaximumHeight);
                                            }
                                            if (!string.IsNullOrEmpty(aCurrentField.MaximumWidth))
                                            {
                                                objField.maximumWidth = int.Parse(aCurrentField.MaximumWidth);
                                            }
                                            objField.maxLengthValue = !string.IsNullOrEmpty(aCurrentField.MaxLengthValue) ? int.Parse(aCurrentField.MaxLengthValue) : 250;
                                            objField.minLengthValue = !string.IsNullOrEmpty(aCurrentField.MinLengthValue) ? int.Parse(aCurrentField.MinLengthValue) : 250;

                                            if (!string.IsNullOrEmpty(aCurrentField.NumberOfColumnsForPhones))
                                            {
                                                objField.numberOfColumnsForPhones = int.Parse(aCurrentField.NumberOfColumnsForPhones);
                                            }
                                            if (!string.IsNullOrEmpty(aCurrentField.NumberOfColumnsForTablets))
                                            {
                                                objField.numberOfColumnsForTablets = int.Parse(aCurrentField.NumberOfColumnsForTablets);
                                            }
                                            objField.photoFile = aCurrentField.PhotoFile != null ? bool.Parse(aCurrentField.PhotoFile.ToString()) : false;
                                            objField.photoLibraryEnabled = aCurrentField.PhotoLibraryEnabled != null ? bool.Parse(aCurrentField.PhotoLibraryEnabled.ToString()) : false;
                                            objField.photoQuality = aCurrentField.PhotoQuality;


                                            objField.ratingLabels = aCurrentField.RatingLabels != null ? bool.Parse(aCurrentField.RatingLabels.ToString()) : false;
                                            objField.ratingMax = aCurrentField.RatingMax != null ? bool.Parse(aCurrentField.RatingMax.ToString()) : false;
                                            objField.sectionId = aCurrentField.SectionId;
                                            objField.secure = aCurrentField.Secure != null ? bool.Parse(aCurrentField.Secure.ToString()) : false;
                                            objField.showMultiselect = aCurrentField.ShowMultiselect != null ? bool.Parse(aCurrentField.ShowMultiselect.ToString()) : false;
                                            objField.showRatingLabels = aCurrentField.ShowRatingLabels != null ? bool.Parse(aCurrentField.ShowRatingLabels.ToString()) : false;
                                            if (!string.IsNullOrEmpty(aCurrentField.MaxrecordabletimeInSeconds))
                                            {
                                                objField.maxrecordabletimeInSeconds = int.Parse(aCurrentField.MaxrecordabletimeInSeconds);
                                            }
                                            objField.videoQuality = aCurrentField.VideoQuality;


                                            if (objField.fieldName == "WORK COMPLETED")
                                            {
                                                if (aCurrentField.SynchFlag != null)
                                                {
                                                    objForms.syncFlag = (bool)aCurrentField.SynchFlag;
                                                }
                                                else
                                                {
                                                    objForms.syncFlag = false;
                                                }
                                            }

                                            List<AuditFieldInitialData> FieldInitialDataList = new List<AuditFieldInitialData>();
                                            objField.fieldInitialDataList = FieldInitialDataList;

                                            objListFields.Add(objField);

                                            #endregion
                                        }
                                        objSection.fieldList = objListFields;
                                        objListSection.Add(objSection);

                                        #endregion
                                    }
                                    objForms.sectionList = objListSection;
                                    objListFormsNew.Add(objForms);


                                    #endregion
                                }

                                #endregion

                                #region MenuVisit
                                List<AuditMenuVisit> objMenuVisitsList = new List<AuditMenuVisit>();

                                var currentProData = ProjectVisitDataChunk.AsEnumerable().Where(o => o.Field<long>("FKProjectId") == objNewMobileOffline.projectId).CopyToDataTable();

                                foreach (DataRow row in currentProData.Rows)
                                {

                                    string StreetDynamicColumnName = checkMappedColumnsToExcel.Where(o => o.FieldName == "STREET").Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                    string AccountDynamicColumnName = checkMappedColumnsToExcel.Where(o => o.FieldName == "ACCOUNT").Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                    string OldMeterNoDynamicColumnName = checkMappedColumnsToExcel.Where(o => o.FieldName == "OLD METER NUMBER").Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                    string OldRadioNoDynamicColumnName = checkMappedColumnsToExcel.Where(o => o.FieldName == "OLD METER RADIO NUMBER").Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                    string OldMeterSizeDynamicColumnName = checkMappedColumnsToExcel.Where(o => o.FieldName == "OLD METER SIZE").Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                    string OldMeterTypeDynamicColumnName = checkMappedColumnsToExcel.Where(o => o.FieldName == "OLD METER TYPE").Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();

                                    int streetCount = objMenuVisitsList.Where(o => o.street == row[StreetDynamicColumnName].ToString()).Count();

                                    if (streetCount == 0)
                                    {

                                        var adata = distCountPro.AsEnumerable().Where(o => o.Field<string>(StreetDynamicColumnName) == row[StreetDynamicColumnName].ToString()).FirstOrDefault();
                                        int completed = 0;
                                        int skipped = 0;
                                        //int rtu = 0;
                                        int totalVisits = 0;
                                        int pendingVisits = 0;
                                        if (adata != null)
                                        {
                                            completed = adata["TotalCountWS"] != null ? int.Parse(adata["TotalCountWS"].ToString()) : 0;
                                            skipped = adata["TotalCountSS"] != null ? int.Parse(adata["TotalCountSS"].ToString()) : 0;
                                            //rtu = adata["TotalCountRS"] != null ? int.Parse(adata["TotalCountRS"].ToString()) : 0;
                                            totalVisits = adata["TotalCountS"] != null ? int.Parse(adata["TotalCountS"].ToString()) : 0;
                                            pendingVisits = totalVisits - (completed + skipped);
                                        }


                                        var getStreetNameData = currentProData.AsEnumerable().Where(o => o.Field<string>(StreetDynamicColumnName) == row[StreetDynamicColumnName].ToString()).CopyToDataTable();

                                        AuditMenuVisit objMenuVisits = new AuditMenuVisit();
                                        objMenuVisits.pendingVisits = pendingVisits;
                                        objMenuVisits.completedVisits = completed;
                                        objMenuVisits.skippedVisits = skipped;
                                        // objMenuVisits.rtuVisits = rtu;
                                        objMenuVisits.totalVisits = totalVisits;

                                        if (totalVisits == pendingVisits)
                                        {
                                            objMenuVisits.colorFlag = 1;
                                        }
                                        else if (pendingVisits > 0)
                                        {
                                            objMenuVisits.colorFlag = 2;
                                        }
                                        else if (skipped > 0 && pendingVisits == 0)
                                        {
                                            objMenuVisits.colorFlag = 3;
                                        }
                                        //else if (rtu > 0 && pendingVisits == 0 && skipped == 0)
                                        //{
                                        //    objMenuVisits.colorFlag = 4;
                                        //}
                                        else if (completed == totalVisits)
                                        {
                                            objMenuVisits.colorFlag = 5;
                                        }


                                        objMenuVisits.street = row[StreetDynamicColumnName].ToString();
                                        objMenuVisits.visitId = row["VisitId"].ToString();
                                        objMenuVisits.groupId = row["GroupId"].ToString();
                                        //added by sns for get latitude and longitude
                                        objMenuVisits.latitude = Convert.ToString(row["AddressLatitude"]);
                                        objMenuVisits.longitude = Convert.ToString(row["AddressLongitude"]);



                                        List<AuditAccounts> objListAccounts = new List<AuditAccounts>();
                                        foreach (DataRow row1 in getStreetNameData.Rows)
                                        {
                                            #region Account
                                            string visitDate1 = Convert.ToString(row1["VisitDate"]);
                                            string InstallerName = Convert.ToString(row1["FirstName"]);
                                            bool isReopen = false;

                                            var listReopen = getStreetNameData.AsEnumerable().Where(o => o.Field<long>("Id") == (long)row1["Id"]).ToList();

                                            if (listReopen.Count > 1)
                                            {
                                                var checkReopen = listReopen.Where(o => o.Field<int>("Reopen") == 1).FirstOrDefault();
                                                if (checkReopen != null && (int)row1["Reopen"] == 0)
                                                {
                                                    isReopen = true;
                                                }
                                            }

                                            if (!isReopen)
                                            {
                                                #region Account

                                                var accountFlag = distCountPro.AsEnumerable().Where(o => o.Field<string>(AccountDynamicColumnName) == row1[AccountDynamicColumnName].ToString() &&
                                                     o.Field<string>(OldMeterNoDynamicColumnName) == row1[OldMeterNoDynamicColumnName].ToString()).FirstOrDefault();

                                                int completedA = 0;
                                                int skippedA = 0;
                                                //  int rtuA = 0;
                                                int totalVisitsA = 0;
                                                int pendingVisitsA = 0;
                                                if (accountFlag != null)
                                                {
                                                    completedA = accountFlag["TotalCountWA"] != DBNull.Value ? int.Parse(accountFlag["TotalCountWA"].ToString()) : 0;
                                                    skippedA = accountFlag["TotalCountSA"] != DBNull.Value ? int.Parse(accountFlag["TotalCountSA"].ToString()) : 0;
                                                    //   rtuA = accountFlag["TotalCountRA"] != DBNull.Value ? int.Parse(accountFlag["TotalCountRA"].ToString()) : 0;
                                                    totalVisitsA = accountFlag["TotalCountA"] != DBNull.Value ? int.Parse(accountFlag["TotalCountA"].ToString()) : 0;
                                                    pendingVisitsA = totalVisitsA - (completedA + skippedA);
                                                }


                                                string visitDate = accountFlag != null ? (!string.IsNullOrEmpty(Convert.ToString(accountFlag["CreatedOnA"]))
                                                    ? DateTime.Parse(Convert.ToString(accountFlag["CreatedOnA"])).ToString("MMM-dd-yyyy hh:mm tt") : "") : "";

                                                AuditAccounts objAccounts = new AuditAccounts();
                                                objAccounts.accountNumber = row1[AccountDynamicColumnName].ToString();
                                                objAccounts.isActive = (int)row1["Active1"] == 0 ? false : true;
                                                objAccounts.isReOpen = (int)row1["Reopen"] == 0 ? false : true;
                                                if (totalVisitsA == pendingVisitsA)
                                                {
                                                    objAccounts.colorFlag = 1;
                                                }
                                                else if (pendingVisitsA > 0)
                                                {
                                                    objAccounts.colorFlag = 2;
                                                }
                                                else if (skippedA > 0 && pendingVisitsA == 0)
                                                {
                                                    objAccounts.colorFlag = 3;
                                                }
                                                //else if (rtuA > 0 && pendingVisitsA == 0 && skippedA == 0)
                                                //{
                                                //    objAccounts.colorFlag = 4;
                                                //}
                                                else if (completedA == totalVisitsA)
                                                {
                                                    objAccounts.colorFlag = 5;
                                                }



                                                objAccounts.isVisited = (int)row1["Active1"] == 0 ? false : true;

                                                objAccounts.oldMeterNumber = Convert.ToString(row1[OldMeterNoDynamicColumnName]);
                                                objAccounts.oldMeterSize = OldMeterSizeDynamicColumnName == null ? "" : Convert.ToString(row1[OldMeterSizeDynamicColumnName]);

                                                objAccounts.oldMeterType = OldMeterTypeDynamicColumnName == null ? "" : Convert.ToString(row1[OldMeterTypeDynamicColumnName]);
                                                objAccounts.oldMeterRadioNumber = OldRadioNoDynamicColumnName == null ? "" : Convert.ToString(row1[OldRadioNoDynamicColumnName]);
                                                objAccounts.uploadId = (long)row1["Id"];

                                                objAccounts.visitDateTime = visitDate;
                                                objAccounts.visitExpiryDateTime = ((DateTime)accountFlag["ToDate"]).ToString("MMM-dd-yyyy");
                                                List<AccountMenuForms> objListForms = new List<AccountMenuForms>();

                                                foreach (var item3 in projectDistinctFormList)
                                                {
                                                    var adata1 = distCountPro.AsEnumerable().
                                                        Where(o => o.Field<long>("Id") == (long)row1["Id"] && o.Field<long>("formId") == item3.FormId).FirstOrDefault();

                                                    #region Form


                                                    AccountMenuForms objForms = new AccountMenuForms();
                                                    objForms.formId = item3.FormId;
                                                    objForms.formName = item3.FormName;

                                                    bool _isSkip = false;
                                                    if (adata1 != null && (int)adata1["FormSkip"] == 1)
                                                    {
                                                        //if form is skip
                                                        objForms.isSkipped = adata1 != null ? ((int)adata1["FormSkip"] == 1 ? true : false) : false;
                                                        _isSkip = true;
                                                    }
                                                    //bool _isRTU = false;
                                                    //if (adata1 != null && (int)adata1["FormRTU"] == 1)
                                                    //{
                                                    //    //if form is rtu
                                                    //    objForms.isSkipped = adata1 != null ? ((int)adata1["FormRTU"] == 1 ? true : false) : false;
                                                    //    _isRTU = true;
                                                    //}

                                                    if (_isSkip == false)
                                                    {
                                                        objForms.isSkipped = false;
                                                    }

                                                    objForms.isVisited = adata1 != null ? ((int)adata1["FormComplete"] == 1 ? true : false) : false;
                                                    objForms.visitDateTime = adata1 != null ? (!string.IsNullOrEmpty(Convert.ToString(adata1["CreatedOnU"])) ?
                                                        DateTime.Parse(Convert.ToString(adata1["CreatedOnU"])).ToString("MMM-dd-yyyy hh:mm tt") : "") : "";
                                                    //DateTime.Parse(Convert.ToString(adata1.CreatedOnU)).ToString("dd-MMM-yyyy HH:MM T")

                                                    if (objForms.isSkipped)
                                                    {
                                                        long _iddd = (long)adata1["Id"];
                                                        //  long _formidd = (long)item3.FormId;
                                                        //var skipOrRtuFlag = objCompassEntities.PROC_WebService_CheckRecordStatus_SkipOrRtu(_iddd, _formidd).FirstOrDefault();
                                                        //var aFieldDataRecord = objCompassEntities.TblProjectFieldDatas.Where(o => o.FormId == _formidd
                                                        //    && o.InstallerId == userId && o.FK_UploadedExcelData_Id == _iddd
                                                        //    && o.Active == 1 && o.IsSkipped == true).FirstOrDefault();

                                                        // long fddId = (long)skipOrRtuFlag;
                                                        var aFieldDataRecord = objCompassEntities.ProjectAuditDatas.Where(o => o.ID == _iddd && o.Active == 1).FirstOrDefault();

                                                        if (aFieldDataRecord != null)
                                                        {
                                                            var m1 = skipReasonTable.Where(o => o.SkipId == aFieldDataRecord.FK_SkipId).FirstOrDefault();
                                                            if (m1 != null)
                                                            {
                                                                //objForms.reasonType = m1.CategoryType;
                                                                objForms.skipReason = m1.SkipReason;
                                                            }
                                                        }
                                                    }

                                                    List<AccountMenuFields> objListFields = new List<AccountMenuFields>();

                                                    var currentFormSections = projectDistinctSectionList.Where(o => o.FormId == item3.FormId).ToList();
                                                    foreach (var item4 in currentFormSections)
                                                    {
                                                        var sectionFields = projectFormData.Where(o => o.SectionId == item4.SectionId).Select(o => o).OrderBy(o => o.FieldOrder).ToList();

                                                        foreach (var item5 in sectionFields)
                                                        {
                                                            #region Fields
                                                            var aCurrentField = projectFormData.Where(o => o.FieldId == item5.FieldId).FirstOrDefault();
                                                            AccountMenuFields objField = new AccountMenuFields();
                                                            objField.fieldId = item5.FieldId;
                                                            objField.sectionId = item4.SectionId;
                                                            objField.fieldName = item5.FieldName;
                                                            List<AccountFieldInitialData> FieldInitialDataList = new List<AccountFieldInitialData>();
                                                            if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER NUMBER".ToLower())
                                                            {
                                                                if (ProjectNewDataInfo != null)
                                                                {

                                                                    var objTest = ProjectNewDataInfo.Where(o => o.Id == objAccounts.uploadId).FirstOrDefault();
                                                                    if (objTest != null && objTest.NEWMETERNUMBER != null)
                                                                    {
                                                                        FieldInitialDataList.Add(new AccountFieldInitialData { initialFieldId = 1, initialFieldValue = objTest.NEWMETERNUMBER });
                                                                    }

                                                                    objField.fieldInitialDataList = FieldInitialDataList;
                                                                }
                                                            }
                                                            else if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER RADIO".ToLower())
                                                            {

                                                                if (ProjectNewDataInfo != null)
                                                                {

                                                                    var objTest = ProjectNewDataInfo.Where(o => o.Id == objAccounts.uploadId).FirstOrDefault();
                                                                    if (objTest != null && objTest.NEWMETERRADIO != null)
                                                                    {

                                                                        FieldInitialDataList.Add(new AccountFieldInitialData { initialFieldId = 1, initialFieldValue = objTest.NEWMETERRADIO });
                                                                    }

                                                                    objField.fieldInitialDataList = FieldInitialDataList;
                                                                }
                                                            }
                                                            else if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER SIZE".ToLower())
                                                            {
                                                                FieldInitialDataList = resNewMeterSizeList.Select(o => new AccountFieldInitialData { initialFieldId = o.initialFieldId, initialFieldValue = o.initialFieldValue }).ToList();
                                                            }
                                                            else if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER TYPE".ToLower())
                                                            {
                                                                FieldInitialDataList = resNewMeterTypeList.Select(o => new AccountFieldInitialData { initialFieldId = o.initialFieldId, initialFieldValue = o.initialFieldValue }).ToList();
                                                            }
                                                            else if (aCurrentField.FieldName.ToLower().Trim() == "NEW METER MAKE".ToLower())
                                                            {
                                                                FieldInitialDataList = resMeterMakeList.Select(o => new AccountFieldInitialData { initialFieldId = o.initialFieldId, initialFieldValue = o.initialFieldValue }).ToList();
                                                            }
                                                            else if (aCurrentField.FieldName.ToLower().Trim() == "Visit Datetime".ToLower())
                                                            {
                                                                FieldInitialDataList.Add(new AccountFieldInitialData { initialFieldId = item5.FieldId, initialFieldValue = visitDate1 });
                                                            }
                                                            else if (aCurrentField.FieldName.ToLower().Trim() == "Installer Name".ToLower())
                                                            {
                                                                FieldInitialDataList.Add(new AccountFieldInitialData { initialFieldId = item5.FieldId, initialFieldValue = InstallerName });
                                                            }
                                                            else if (aCurrentField.FieldName.ToLower().Trim() == "SERVICES".ToLower())
                                                            {
                                                                #region
                                                                FieldInitialDataList = resServiceList.Where(o => o.isAdditionalService == false).Select(o =>
                                                                    new AccountFieldInitialData
                                                                    {
                                                                        initialFieldId = o.initialFieldId,
                                                                        initialFieldValue = o.initialFieldValue,
                                                                        flagCaptureAudio = o.flagCaptureAudio,
                                                                        flagCaptureVideo = o.flagCaptureVideo,
                                                                        flagIsImageCaptureRequired = bool.Parse(o.flagIsImageCaptureRequired.ToString()),
                                                                        isAdditionalService = bool.Parse(o.isAdditionalService.ToString()),
                                                                        //imageList = string.IsNullOrEmpty((o.imageList)) ? "".Split(',').ToList() : (o.imageList).Split(',').ToList()
                                                                        imageList = string.IsNullOrEmpty((o.imageList)) ? null : (o.imageList).Split(',').ToList()
                                                                       .Select(o2 => new AuditPictureList
                                                                       {
                                                                           imageId = (imageList.Where(o1 => o1.Picture.Trim() == o2.Trim()).Select(o1 => o1.ID).FirstOrDefault()),
                                                                           imageName = o2.Trim()
                                                                       }).ToList()
                                                                    }).ToList();
                                                                #endregion
                                                            }
                                                            else if (aCurrentField.FieldName.ToLower().Trim() == "ADDITIONAL SERVICES".ToLower())
                                                            {
                                                                #region
                                                                FieldInitialDataList = resServiceList.Where(o => o.isAdditionalService == true).Select(o =>
                                                                    new AccountFieldInitialData
                                                                    {
                                                                        initialFieldId = o.initialFieldId,
                                                                        initialFieldValue = o.initialFieldValue,
                                                                        flagCaptureAudio = o.flagCaptureAudio,
                                                                        flagCaptureVideo = o.flagCaptureVideo,
                                                                        flagIsImageCaptureRequired = bool.Parse(o.flagIsImageCaptureRequired.ToString()),
                                                                        isAdditionalService = bool.Parse(o.isAdditionalService.ToString()),
                                                                        //imageList = string.IsNullOrEmpty((o.imageList)) ? "".Split(',').ToList() : (o.imageList).Split(',').ToList()
                                                                        imageList = string.IsNullOrEmpty((o.imageList)) ? null : (o.imageList).Split(',').ToList()
                                                                       .Select(o2 => new AuditPictureList
                                                                       {
                                                                           imageId = (imageList.Where(o1 => o1.Picture.Trim() == o2.Trim()).Select(o1 => o1.ID).FirstOrDefault()),
                                                                           imageName = o2.Trim()
                                                                       }).ToList()
                                                                    }).ToList();
                                                                #endregion
                                                            }
                                                            else
                                                            {
                                                                #region other fields
                                                                var checkfield = checkMappedColumnsToExcel.Where(o => o.FieldName == aCurrentField.FieldName).FirstOrDefault();

                                                                if (checkfield != null && checkfield.isMappedToExcel == 1 && checkfield.FieldName != "LOCATION/GPS")
                                                                {
                                                                    AccountFieldInitialData objFieldInitialData = new AccountFieldInitialData();

                                                                    objFieldInitialData.initialFieldId = item5.FieldId;

                                                                    var _columnName = checkMappedColumnsToExcel.Where(o => o.FieldName.ToLower() == checkfield.FieldName.ToLower()).FirstOrDefault();

                                                                    if (_columnName != null)
                                                                    {
                                                                        objFieldInitialData.initialFieldValue = Convert.ToString(row1[_columnName.FKUploadedExcelDataDynamicColumn]);
                                                                    }
                                                                    FieldInitialDataList.Add(objFieldInitialData);
                                                                }
                                                                else
                                                                {
                                                                    var res = objCompassEntities.tblProjectFieldDDLMasters.Where(o => o.FormSectionFieldID == aCurrentField.FieldId && o.Active == 1).ToList();
                                                                    FieldInitialDataList = res.Select(o => new AccountFieldInitialData { initialFieldId = o.ID, initialFieldValue = o.FieldValue }).ToList();
                                                                }
                                                                #endregion
                                                            }


                                                            objField.fieldInitialDataList = FieldInitialDataList;

                                                            objListFields.Add(objField);

                                                            #endregion
                                                        }
                                                    }

                                                    objForms.fieldList = objListFields;
                                                    objListForms.Add(objForms);


                                                    #endregion
                                                }
                                                objAccounts.formList = objListForms;
                                                objListAccounts.Add(objAccounts);

                                                #endregion
                                            }

                                            #endregion
                                        }

                                        objMenuVisits.objAccounts = objListAccounts;

                                        var _totalAccount = objListAccounts.Count;
                                        var _deactivateAccount = objListAccounts.Where(o => o.isActive == false).Count();
                                        objMenuVisits.isActive = (_deactivateAccount > 0 && _deactivateAccount == _totalAccount) ? false : true;

                                        objMenuVisitsList.Add(objMenuVisits);


                                    }

                                }


                                #endregion

                                #region Map Visit
                                List<AuditMapVisits> objListMapVisits = new List<AuditMapVisits>();

                                foreach (DataRow item1 in currentProData.Rows)
                                {
                                    var checkPresent = objListMapVisits.Where(o => o.latitude == Convert.ToString(item1["AddressLatitude"]) &&
                                        o.longitude == Convert.ToString(item1["AddressLongitude"])).FirstOrDefault();
                                    if (checkPresent == null)
                                    {
                                        var allMatchingLatLonList = currentProData.AsEnumerable().Where(o => o.Field<string>("AddressLatitude") == item1["AddressLatitude"].ToString() &&
                                            o.Field<string>("AddressLongitude") == item1["AddressLongitude"].ToString()
                                            && o.Field<long>("FKProjectId") == (long)item1["FKProjectId"]).ToList();
                                        string StreetDynamicColumnName = checkMappedColumnsToExcel.Where(o => o.FieldName == "STREET").Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                        if (allMatchingLatLonList.Count == 1)
                                        {
                                            #region single record
                                            var adata = distCountPro.AsEnumerable().Where(o => o.Field<string>("AddressLatitude") == item1["AddressLatitude"].ToString() &&
                                                o.Field<string>("AddressLongitude") == item1["AddressLongitude"].ToString()
                                                && o.Field<long>("FKProjectId") == (long)item1["FKProjectId"]).FirstOrDefault();

                                            int completed = 0;
                                            int skipped = 0;
                                            //int rtu = 0;
                                            int totalVisits = 0;
                                            int pendingVisits = 0;

                                            if (adata != null)
                                            {
                                                completed = adata["TotalCountWL"] != null ? int.Parse(adata["TotalCountWL"].ToString()) : 0;
                                                skipped = adata["TotalCountSL"] != null ? int.Parse(adata["TotalCountSL"].ToString()) : 0;
                                                // rtu = adata["TotalCountRL"] != null ? int.Parse(adata["TotalCountRL"].ToString()) : 0;
                                                totalVisits = adata["TotalCountL"] != null ? int.Parse(adata["TotalCountL"].ToString()) : 0;
                                                pendingVisits = totalVisits - (completed + skipped);
                                            }
                                            AuditMapVisits objMapVisits = new AuditMapVisits();
                                            objMapVisits.groupId = item1["GroupId"].ToString();


                                            if (totalVisits == pendingVisits)
                                            {
                                                objMapVisits.colorFlag = 1;
                                            }
                                            else if (pendingVisits > 0)
                                            {
                                                objMapVisits.colorFlag = 2;
                                            }
                                            else if (skipped > 0 && pendingVisits == 0)
                                            {
                                                objMapVisits.colorFlag = 3;
                                            }
                                            //else if (rtu > 0 && pendingVisits == 0 && skipped == 0)
                                            //{
                                            //    objMapVisits.colorFlag = 4;
                                            //}
                                            else if (completed == totalVisits)
                                            {
                                                objMapVisits.colorFlag = 5;
                                            }


                                            objMapVisits.street = item1[StreetDynamicColumnName].ToString();
                                            objMapVisits.latitude = Convert.ToString(item1["AddressLatitude"]);
                                            objMapVisits.longitude = Convert.ToString(item1["AddressLongitude"]);
                                            objMapVisits.completedVisits = completed;
                                            objMapVisits.pendingVisits = pendingVisits;
                                            objMapVisits.skippedVisits = skipped;
                                            objMapVisits.totalVisits = totalVisits;
                                            objMapVisits.isActive = (int)item1["Active"] == 1 ? true : false;
                                            objListMapVisits.Add(objMapVisits);

                                            #endregion
                                        }
                                        else
                                        {
                                            #region multiple records
                                            var adata = distCountPro.AsEnumerable().Where(o => o.Field<string>("AddressLatitude") == item1["AddressLatitude"].ToString() &&
                                                  o.Field<string>("AddressLongitude") == item1["AddressLongitude"].ToString()
                                                && o.Field<long>("FKProjectId") == (long)item1["FKProjectId"]).FirstOrDefault();
                                            int completed = 0;
                                            int skipped = 0;
                                            // int rtu = 0;
                                            int totalVisits = 0;
                                            int pendingVisits = 0;

                                            if (adata != null)
                                            {
                                                completed = adata["TotalCountWL"] != null ? int.Parse(adata["TotalCountWL"].ToString()) : 0;
                                                skipped = adata["TotalCountSL"] != null ? int.Parse(adata["TotalCountSL"].ToString()) : 0;
                                                // rtu = adata["TotalCountRL"] != null ? int.Parse(adata["TotalCountRL"].ToString()) : 0;
                                                totalVisits = adata["TotalCountL"] != null ? int.Parse(adata["TotalCountL"].ToString()) : 0;
                                                pendingVisits = totalVisits - (completed + skipped);
                                            }
                                            //  

                                            var checkAddressList = allMatchingLatLonList.AsEnumerable().Select(o => o[StreetDynamicColumnName].ToString()).Distinct().ToList();

                                            IEnumerable<string> s1 = StringExtensions.GetMostCommonSubstrings(checkAddressList);

                                            string swd = s1.FirstOrDefault();

                                            AuditMapVisits objMapVisits = new AuditMapVisits();
                                            objMapVisits.groupId = item1["GroupId"].ToString();

                                            if (totalVisits == pendingVisits)
                                            {
                                                objMapVisits.colorFlag = 1;
                                            }
                                            else if (pendingVisits > 0)
                                            {
                                                objMapVisits.colorFlag = 2;
                                            }
                                            else if (skipped > 0 && pendingVisits == 0)
                                            {
                                                objMapVisits.colorFlag = 3;
                                            }
                                            //else if (rtu > 0 && pendingVisits == 0 && skipped == 0)
                                            //{
                                            //    objMapVisits.colorFlag = 4;
                                            //}
                                            else if (completed == totalVisits)
                                            {
                                                objMapVisits.colorFlag = 5;
                                            }

                                            objMapVisits.street = swd;
                                            objMapVisits.latitude = Convert.ToString(item1["AddressLatitude"]);
                                            objMapVisits.longitude = Convert.ToString(item1["AddressLongitude"]);
                                            objMapVisits.completedVisits = completed;
                                            objMapVisits.pendingVisits = pendingVisits;
                                            //objMapVisits.rtuVisits = rtu;
                                            objMapVisits.skippedVisits = skipped;
                                            objMapVisits.totalVisits = totalVisits;


                                            var _totalAccount = currentProData.AsEnumerable().Where(o => o.Field<string>("AddressLatitude") == item1["AddressLatitude"].ToString() &&
                                                  o.Field<string>("AddressLongitude") == item1["AddressLongitude"].ToString()
                                                && o.Field<long>("FKProjectId") == (long)item1["FKProjectId"]).Count();
                                            var _deactivateAccount = currentProData.AsEnumerable().Where(o => o.Field<string>("AddressLatitude") == item1["AddressLatitude"].ToString() &&
                                                  o.Field<string>("AddressLongitude") == item1["AddressLongitude"].ToString()
                                                && o.Field<long>("FKProjectId") == (long)item1["FKProjectId"]
                                            && o.Field<int>("Active1") == 0).Count();

                                            objMapVisits.isActive = (_deactivateAccount == _totalAccount && _deactivateAccount > 0) ? false : true;

                                            objListMapVisits.Add(objMapVisits);


                                            #endregion
                                        }
                                    }

                                }


                                #endregion


                                objNewMobileOffline.objFormStructure = objListFormsNew;
                                objNewMobileOffline.objMenuVisits = objMenuVisitsList;
                                objNewMobileOffline.objMapVisits = objListMapVisits;
                                objNewMobileOffline.skipReasonList = skipList.Select(o => new SkippReason
                                {
                                    skipId = o.SkipId,
                                    skipReason = o.SkipReason,
                                    isAudioRequired = (o.IsAudioRequired == null ? false : bool.Parse(o.IsAudioRequired.ToString())),
                                    isImageRequired = (o.IsImageRequired == null ? false : bool.Parse(o.IsImageRequired.ToString())),
                                    isVideoRequired = (o.IsVideoRequired == null ? false : bool.Parse(o.IsVideoRequired.ToString()))
                                }).ToList();

                                objListMobileOffline.Add(objNewMobileOffline);

                            }

                        }

                    }

                    JavaScriptSerializer s = new JavaScriptSerializer();
                    s.MaxJsonLength = 2147483644;
                    // s.Serialize(objListMobileOffline);
                    int currentRecords = 0;
                    if (objListMobileOffline != null)
                    {
                        foreach (var item in objListMobileOffline)
                        {
                            foreach (var item1 in item.objMenuVisits)
                            {
                                currentRecords = currentRecords + item1.objAccounts.Count();
                            }
                        }
                    }
                    string json1 = s.Serialize(objListMobileOffline);

                    string m2 = GetMd5Sum(json1);
                    mobileOfflineResult.checkSum = m2;
                    mobileOfflineResult.data = objListMobileOffline;
                    mobileOfflineResult.batchNumber = BatchNumber;
                    mobileOfflineResult.batchIndicator = checkRecordAvilableForNextTrip;
                    mobileOfflineResult.chunkSize = currentRecords;
                    mobileOfflineResult.totalRecord = chunkRecordCount;



                    #region set checksum & BatchNumber
                    objCompassEntities = new CompassEntities();
                    foreach (DataRow item in ProjectVisitDataChunk.Rows)
                    {
                        long id = (long)item["Id"];
                        var objInstallerMap = objCompassEntities.ProjectAuditAllocations.
                            Where(o => o.FK_UploadedExcelData_Id == id && o.FK_AuditorId == userId).FirstOrDefault();
                        objInstallerMap.BatchNumber = BatchNumber;
                        objInstallerMap.SynchCheckSumString = m2;
                        objInstallerMap.SynchWithMobileFlag = 1;
                        objInstallerMap.SynchWithMobileDate = new CommonFunctions().ServerDate();
                        objCompassEntities.SaveChanges();
                    }



                    #endregion

                    return mobileOfflineResult;


                    #endregion
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }




        /// <summary>
        /// 2016-10-3 Devloper- Bharat
        /// returns batchnumber for the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="strCurrentDate"></param>
        /// <param name="batchNumber"></param>
        /// <param name="BatchNumber"></param>
        /// <param name="sendAlreadySentRecord"></param>
        /// <returns></returns>
        private bool GetAuditBatchNumber(long userId, DataTable dt, DateTime strCurrentDate, long batchNumber, ref long BatchNumber, ref bool sendAlreadySentRecord)
        {
            bool WrongBatchNumber = false;

            #region Batch number logic

            if (batchNumber == 0)
            {
                ///if batch number=0
                GetAuditBatchNo(userId, dt, strCurrentDate, ref BatchNumber, ref sendAlreadySentRecord);
            }
            else
            {
                if (batchNumber > 1)
                {
                    ///if batch number greater than 1
                    GetAuditBatchNo1(userId, batchNumber, ref BatchNumber, ref sendAlreadySentRecord, ref WrongBatchNumber);
                }
                else
                {
                    ///if batch number =1
                    BatchNumber = GetAuditBatchNo2(userId, dt, strCurrentDate, ref BatchNumber);
                }
            }
            #endregion
            return WrongBatchNumber;
        }

        /// <summary>
        /// 2016-10-3 Devloper- Bharat
        /// Returns the batch number when request batch number is equal to 1
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="strCurrentDate"></param>
        /// <param name="BatchNumber"></param>
        /// <returns></returns>
        private long GetAuditBatchNo2(long userId, DataTable dt, DateTime strCurrentDate, ref long BatchNumber)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            var checkB = objCompassEntities.ProjectAuditAllocations.Where(o => o.FK_AuditorId == userId
               && o.Active == 1).Select(o => o.BatchNumber).Max();

            // if batch no > 1 means already send data present for the installer

            if (checkB >= 1)
            {
                // First update the old resend data flag for the current time period.                    

                //var ProVisitData = objCompassEntities.STP_OfflineDataSP(userId, strCurrentDate).ToList();

                // CompassEntities objCompassEntities1 = new CompassEntities();
                foreach (DataRow row in dt.Rows)
                {
                    long UploadId = long.Parse(row["Id"].ToString());
                    var objInstallerMap = objCompassEntities.ProjectAuditAllocations.Where(o => o.FK_UploadedExcelData_Id == UploadId &&
                         o.FK_AuditorId == userId).FirstOrDefault();
                    if (objInstallerMap != null)
                    {
                        objInstallerMap.BatchNumber = null;
                        objInstallerMap.SynchCheckSumString = null;
                        objInstallerMap.SynchWithMobileFlag = null;
                        objInstallerMap.SynchWithMobileDate = null;
                        objInstallerMap.MobileResponse = null;
                        objInstallerMap.MobileResponseDate = null;
                        objCompassEntities.SaveChanges();
                    }
                }


                // get the new batchnumber for the installer 
                checkB = objCompassEntities.ProjectAuditAllocations.Where(o => o.FK_AuditorId == userId && o.Active == 1).Select(o => o.BatchNumber).Max();
                BatchNumber = checkB != null ? long.Parse(checkB.ToString()) + 1 : 1;
            }
            else
            {
                BatchNumber = 1;
            }
            return BatchNumber;
        }

        /// <summary>
        /// 2016-10-3 Devloper- Bharat
        /// Returns the batch number when request batch number is > 1
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="batchNumber"></param>
        /// <param name="BatchNumber"></param>
        /// <param name="sendAlreadySentRecord"></param>
        /// <param name="WrongBatchNumber"></param>
        private void GetAuditBatchNo1(long userId, long batchNumber, ref long BatchNumber, ref bool sendAlreadySentRecord, ref bool WrongBatchNumber)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            /// Check current batch number-1 present in database for the installer.                        
            var objInstallerMapList = objCompassEntities.ProjectAuditAllocations.Where(o => o.FK_AuditorId == userId
                && o.BatchNumber == (batchNumber - 1) && o.SynchWithMobileFlag == 1 && o.MobileResponse != 1).ToList();

            if (objInstallerMapList != null && objInstallerMapList.Count > 0)
            {
                /// if  record present & it's mobileresponse flag is not true 
                /// then get these record & set the flag to true.    
                foreach (var instItemList in objInstallerMapList)
                {
                    instItemList.MobileResponse = 1;
                    instItemList.MobileResponseDate = new CommonFunctions().ServerDate();
                    objCompassEntities.SaveChanges();
                }
                // set new batch number value.
                BatchNumber = batchNumber;
            }
            else
            {
                /// if  record not present then check the current batch present with mobile response false for the user 
                objInstallerMapList = objCompassEntities.ProjectAuditAllocations.Where(o => o.FK_AuditorId == userId
                && o.BatchNumber == batchNumber && o.SynchWithMobileFlag == 1 && o.MobileResponse != 1).ToList();

                if (objInstallerMapList != null && objInstallerMapList.Count > 0)
                {
                    ///if record found then send already sent batch for user.
                    sendAlreadySentRecord = true;
                    BatchNumber = batchNumber;
                }
                else
                {
                    ///if not found any record then select the max batch number from the datbase
                    ///for the user & check (max+1)!= requested batch number     
                    var checkB = objCompassEntities.ProjectAuditAllocations.Where(o => o.FK_AuditorId == userId
                   && o.Active == 1).Select(o => o.BatchNumber).Max();

                    if ((checkB + 1) != batchNumber)
                    {
                        // then rejwct the request for the user
                        WrongBatchNumber = true;
                    }
                    else
                    {
                        //set the new batch number as requested batch number is equal to max +1 
                        BatchNumber = long.Parse(checkB.ToString()) + 1;
                    }
                }
            }
        }

        /// <summary>
        /// 2016-10-3 Devloper- Bharat
        /// Returns the batch number when request batch number is equal to 0
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="strCurrentDate"></param>
        /// <param name="BatchNumber"></param>
        /// <param name="sendAlreadySentRecord"></param>
        private void GetAuditBatchNo(long userId, DataTable dt, DateTime strCurrentDate, ref long BatchNumber, ref bool sendAlreadySentRecord)
        {
            ///get incremental data or data from begining

            CompassEntities objCompassEntities = new CompassEntities();
            // send the incremental data for the installer
            BatchNumber = 0;

            //first check any batch number present for the user
            var checkB = objCompassEntities.ProjectAuditAllocations.Where(o => o.FK_AuditorId == userId
                && o.BatchNumber != null).Select(o => o.BatchNumber).FirstOrDefault();

            if (checkB == null)
            {
                //user first time log in no batch number present in the database for user.                    
                //send fresh new batch data                    
                BatchNumber = 1;
            }
            else
            {
                //get all data for given date so we can check if any data pending in partially synch
                //i.e. data is sent to mobile & no response got from mobile.

                //checkB = dt.AsEnumerable().Where(o => o.Field<int?>("SynchWithMobileFlag") == 1 && o.Field<int?>("MobileResponse") != 1).
                //    Select(o => o.Field<int?>("BatchNumber")).FirstOrDefault();


                var a = dt.Columns["MobileResponse"].DataType;
                var b = dt.Columns["SynchWithMobileFlag"].DataType;
                var c = dt.Columns["BatchNumber"].DataType;
                checkB = dt.AsEnumerable().Where(o => o.Field<Int32?>("SynchWithMobileFlag") == 1 && o.Field<Int32?>("MobileResponse") != 1).
                 Select(o => o.Field<Int64>("BatchNumber")).FirstOrDefault();
                if (checkB != null && checkB != 0)
                {
                    //data is sent to mobile & no response got from mobile.
                    // then resend this data to mobile for the installer.
                    sendAlreadySentRecord = true;
                    BatchNumber = long.Parse(checkB.ToString());

                }
                else
                {
                    // send fresh new batch data for the current user.
                    checkB = objCompassEntities.ProjectAuditAllocations.Where(o => o.FK_AuditorId == userId).Select(o => o.BatchNumber).Max();
                    BatchNumber = long.Parse(checkB.ToString()) + 1;
                }
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public List<ProjectMobileBO> ProjectData(long userId, DateTime currentDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            List<ProjectMobileBO> objList = new List<ProjectMobileBO>();

            var CurrentProjectList = objCompassEntities.PROC_GetCurrentProject(userId, currentDate.Date).ToList();

            if (CurrentProjectList != null && CurrentProjectList.Count > 0)
            {

                objList = objCompassEntities.STP_GetProjectCityState().
                    Where(a => a.Active == 1 && CurrentProjectList.Contains(a.ProjectId)).Select(o =>
                                           new ProjectMobileBO
                                           {
                                               ProjectId = o.ProjectId,
                                               ProjectName = o.ProjectName
                                               // ProjectCityState = o.ProjectCityName,                                              
                                           }).OrderBy(a => a.ProjectName).ToList();

                if (objList == null)
                {
                    objList = new List<ProjectMobileBO>();
                }
            }

            return objList;

        }

        public List<LastCompleteLocation> getLastCompleteLocation(long installerId)
        {
            List<LastCompleteLocation> resultDict = new List<LastCompleteLocation>();
            CompassEntities objCompassEntities = new CompassEntities();
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PROC_GetLatestCompletPosition", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@InstallerId", installerId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    if (dt != null)
                    {
                        resultDict = dt.AsEnumerable().
                        Select(o => new LastCompleteLocation
                        {
                            projectId = o.Field<long>("ProjectId"),
                            lastCompleteLat = o.Field<string>("AddressLatitude"),
                            lastCompleteLong = o.Field<string>("AddressLongitude")
                        }).Distinct().ToList();

                        
                    }
                    cmd.Clone();
                }
                sqlcon.Close();
            }

            return resultDict;
        }
        public LocationCycleRoute getCycleRoute(long recordId, long projectId)
        {
            LocationCycleRoute result = new LocationCycleRoute();
            CompassEntities objCompassEntities = new CompassEntities();
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PROC_GetLocationCycleRoute", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@uploadedExcelDataId", recordId);
                    cmd.Parameters.Add("@projectId", projectId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    if (dt != null)
                    {
                        var resultDict = dt.AsEnumerable().
                        Select(o => new
                        {
                            cycle = o.Field<string>("cycle"),
                            route = o.Field<string>("route")
                        }).Distinct().ToList();
                        foreach (var dataId in resultDict)
                        {
                            result.cycle = dataId.cycle;
                            result.route = dataId.route;
                        }
                    }
                    cmd.Clone();
                }
                sqlcon.Close();
            }

            return result;
        }

    }
}
