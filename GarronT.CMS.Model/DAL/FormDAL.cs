using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;


namespace GarronT.CMS.Model.DAL
{
    public class FormDAL : IDisposable
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
        /// get list of all form 
        /// </summary>
        /// <param name="ActiveStatus"></param>
        /// <returns></returns>
        /// 

        public List<FormTypeModel> GetFormTypeList()
        {
            CompassEntities CompassEntities = new CompassEntities();
            var data = CompassEntities.tblFormTypes.AsNoTracking().Where(o => o.Active == 1).AsEnumerable().
                Select(o => new FormTypeModel
                {
                    ID = o.ID,
                    FormType = o.FormType,
                }).OrderBy(o => o.FormType).ToList();

            return data;
        }



        public List<FormDOL> GetFormList(bool ActiveStatus)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var data = CompassEntities.VW_GetFormData.AsNoTracking().Where(o => o.Active == (ActiveStatus == true ? 1 : 0)).AsEnumerable().
                Select(o => new FormDOL
                {
                    FormId = o.FormId,
                    FormName = o.FormName,
                    ProjectId = o.ProjectId,
                    ProjectName = o.ProjectName,
                    ClientName = o.ClientName,
                    City = o.City,
                    stringFromDate = o.FromDate != null ? DateTime.Parse(o.FromDate.ToString()).ToString("MMM-dd-yyyy") : "",
                    stringToDate = o.ToDate != null ? DateTime.Parse(o.ToDate.ToString()).ToString("MMM-dd-yyyy") : ""
                }).ToList();

            return data;


        }

        public List<FieldDataTypeMasterModel> GetFieldDataType()
        {
            CompassEntities CompassEntities = new CompassEntities();
            var data = CompassEntities.TblFieldDataTypeMasters.AsNoTracking().Where(o => o.Active == 1).AsEnumerable().
                Select(o => new FieldDataTypeMasterModel
                {
                    DataTypeId = o.DataTypeId,
                    FieldDataTypeName = o.FieldDataTypeName,
                    FieldDataTypeDescription = o.FieldDataTypeDescription
                }).ToList();

            return data;


        }

        public bool A_Or_D_FormRecord(bool status, long formId, int userId, out string returnMessage)
        {
            CompassEntities CompassEntities = new CompassEntities();
            tblFormMaster obj = CompassEntities.tblFormMasters.Where(o => o.FormId == formId).FirstOrDefault();
            obj.Active = status == true ? 1 : 0;
            obj.ModifiedBy = userId;
            obj.ModifiedOn = new CommonFunctions().ServerDate();
            CompassEntities.SaveChanges();
            returnMessage = status == true ? CommonFunctions.strRecordActivated :
               CommonFunctions.strRecordDeactivated;
            return true;
        }

        public FormMasterFields GetFormmasterFields(long Formid)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var data = CompassEntities.tblFormMasters.AsNoTracking().Where(o => o.Active == 1 && o.FormId == Formid).AsEnumerable().
                Select(o => new FormMasterFields
                {
                    FormId = o.FormId,
                    FormName = o.FormName,
                    ProjectId = o.ProjectId,
                    FromDate = o.FromDate.ToString("MMM-dd-yyyy"),
                    ToDate = o.ToDate.ToString("MMM-dd-yyyy"),
                    FormTypeID = o.FormTypeID,
                }).FirstOrDefault();

            return data;

        }


        public List<FormManagementViewModel> GetFormSectionFieldsClone(long Formid, List<SP_GetFieldInfoForProject_Result> SourcefieldsData)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<FormManagementViewModel> FormManagementViewModel = new List<FormManagementViewModel>();
            var FormSectionRelationdata = CompassEntities.tblFormSectionRelations.AsNoTracking().Where(o => o.Active == 1 && o.FormID == Formid).ToList();
            foreach (var item in FormSectionRelationdata)
            {
                FormManagementViewModel viewmodel = new FormManagementViewModel();
                List<FormManagementFields> SectionFieldList = new List<FormManagementFields>();


                viewmodel.SectionID = item.SectionId;
                viewmodel.SectionName = item.SectionName;
                viewmodel.SectionOrder = item.SortOrderNumber;

                var FormSectionFieldRelation = CompassEntities.tblFormSectionFieldRelations.AsNoTracking().Where(o => o.Active == 1 && o.SectionId == viewmodel.SectionID).ToList();
                if (FormSectionFieldRelation.Count > 0)
                {
                    foreach (var FormSectionRelation in FormSectionFieldRelation)
                    {
                        if (SourcefieldsData.Any(prod => prod.FieldName == FormSectionRelation.FieldName))
                        {



                            FormManagementFields SectionField = new FormManagementFields();
                            var ProjectFieldProperty = CompassEntities.tblProjectFieldProperties.AsNoTracking().Where(o => o.Active == 1 && o.FormSectionFieldID == FormSectionRelation.FieldId).FirstOrDefault();
                            if (ProjectFieldProperty != null)
                            {
                                SectionField.field_ProjectID = ProjectFieldProperty.ProjectID;
                                SectionField.field_SectionID = FormSectionRelation.SectionId.ToString();
                                SectionField.field_FieldID = ProjectFieldProperty.FDID.Value;
                                SectionField.field_FieldName = FormSectionRelation.FieldName;
                                SectionField.field_FieldOrder = FormSectionRelation.FieldOrder;
                                SectionField.field_DatatypeID = FormSectionRelation.TblFieldDataTypeMaster_Id.Value;
                                // SectionField.field_DatatypeName=


                                SectionField.field_FieldLabel = ProjectFieldProperty.FieldLabel;
                                SectionField.field_HideFieldLabel = ProjectFieldProperty.HideFieldLabel;
                                SectionField.field_DefaultValue = ProjectFieldProperty.DefaultValue;
                                SectionField.field_HintText = ProjectFieldProperty.HintText;
                                SectionField.field_FieldFilterkey = ProjectFieldProperty.FieldFilterkey;
                                SectionField.field_EnforceMinMax = Convert.ToBoolean(ProjectFieldProperty.EnforceMinMax);
                                SectionField.field_AllowMultiSelection = ProjectFieldProperty.AllowMultiSelection;
                                SectionField.field_MaxLengthValue = ProjectFieldProperty.MaxLengthValue;
                                SectionField.field_MinLengthValue = ProjectFieldProperty.MinLengthValue;
                                SectionField.field_IsRequired = Convert.ToBoolean(ProjectFieldProperty.IsRequired);
                                SectionField.field_IsEnabled = Convert.ToBoolean(ProjectFieldProperty.IsEnabled);
                                SectionField.field_CaptureGeolocation = Convert.ToBoolean(ProjectFieldProperty.CaptureGeolocation);
                                SectionField.field_CaptureTimestamp = Convert.ToBoolean(ProjectFieldProperty.CaptureTimestamp);
                                SectionField.field_HasAlert = ProjectFieldProperty.HasAlert;



                                SectionField.field_PhotoQuality = ProjectFieldProperty.PhotoQuality;
                                SectionField.field_PhotoLibraryEnabled = Convert.ToBoolean(ProjectFieldProperty.PhotoLibraryEnabled);
                                SectionField.field_AllowAnnotation = Convert.ToBoolean(ProjectFieldProperty.AllowAnnotation);
                                SectionField.field_CameraEnabled = Convert.ToBoolean(ProjectFieldProperty.CameraEnabled);
                                SectionField.field_GPSTagging = Convert.ToBoolean(ProjectFieldProperty.GPSTagging);
                                SectionField.field_ExcludeonSync = Convert.ToBoolean(ProjectFieldProperty.ExcludeonSync);
                                SectionField.field_MaximumHeight = ProjectFieldProperty.MaximumHeight;
                                SectionField.field_MaximumWidth = ProjectFieldProperty.MaximumWidth;
                                SectionField.field_AcknowlegementText = ProjectFieldProperty.AcknowlegementText;
                                SectionField.field_AcknowlegementButtonText = ProjectFieldProperty.AcknowlegementButtonText;
                                SectionField.field_AcknowlegementClickedButtonText = ProjectFieldProperty.AcknowlegementClickedButtonText;
                                SectionField.field_PhotoLibraryEnabled = Convert.ToBoolean(ProjectFieldProperty.PhotoLibraryEnabled);
                                SectionField.field_MaxrecordabletimeInSeconds = ProjectFieldProperty.MaxrecordabletimeInSeconds;
                                SectionField.field_VideoQuality = ProjectFieldProperty.VideoQuality;

                                //AddedBy AniketJ on 03-Mar-2016
                                SectionField.field_FieldCalculation = ProjectFieldProperty.FieldCalculation;
                                SectionField.field_AllowRepeat = ProjectFieldProperty.AllowRepeat;

                                //AddedBy AniketJ on 15-Feb-2018
                                SectionField.field_SynchFlag = ProjectFieldProperty.SynchFlag;


                                SectionFieldList.Add(SectionField);
                            }
                        }
                    }
                }

                viewmodel.SectionFields = SectionFieldList;
                FormManagementViewModel.Add(viewmodel);
            }
            FormManagementViewModel = FormManagementViewModel.OrderByDescending(a => a.SectionOrder).ToList();
            return FormManagementViewModel;

        }


        public List<FormManagementViewModel> GetFormSectionFields(long Formid)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<FormManagementViewModel> FormManagementViewModel = new List<FormManagementViewModel>();
            var FormSectionRelationdata = CompassEntities.tblFormSectionRelations.AsNoTracking().Where(o => o.Active == 1 && o.FormID == Formid).ToList();
            foreach (var item in FormSectionRelationdata)
            {
                FormManagementViewModel viewmodel = new FormManagementViewModel();
                List<FormManagementFields> SectionFieldList = new List<FormManagementFields>();


                viewmodel.SectionID = item.SectionId;
                viewmodel.SectionName = item.SectionName;
                viewmodel.SectionOrder = item.SortOrderNumber;

                var FormSectionFieldRelation = CompassEntities.tblFormSectionFieldRelations.AsNoTracking().Where(o => o.Active == 1 && o.SectionId == viewmodel.SectionID).ToList();
                if (FormSectionFieldRelation.Count > 0)
                {
                    foreach (var FormSectionRelation in FormSectionFieldRelation)
                    {
                        FormManagementFields SectionField = new FormManagementFields();
                        var ProjectFieldProperty = CompassEntities.tblProjectFieldProperties.AsNoTracking().Where(o => o.Active == 1 && o.FormSectionFieldID == FormSectionRelation.FieldId).FirstOrDefault();
                        if (ProjectFieldProperty != null)
                        {
                            SectionField.field_ProjectID = ProjectFieldProperty.ProjectID;
                            SectionField.field_SectionID = FormSectionRelation.SectionId.ToString();
                            SectionField.field_FieldID = ProjectFieldProperty.FDID.Value;
                            SectionField.field_FieldName = FormSectionRelation.FieldName;
                            SectionField.field_FieldOrder = FormSectionRelation.FieldOrder;
                            SectionField.field_DatatypeID = FormSectionRelation.TblFieldDataTypeMaster_Id.Value;
                            // SectionField.field_DatatypeName=


                            SectionField.field_FieldLabel = ProjectFieldProperty.FieldLabel;
                            SectionField.field_HideFieldLabel = ProjectFieldProperty.HideFieldLabel;
                            SectionField.field_DefaultValue = ProjectFieldProperty.DefaultValue;
                            SectionField.field_HintText = ProjectFieldProperty.HintText;
                            SectionField.field_FieldFilterkey = ProjectFieldProperty.FieldFilterkey;
                            SectionField.field_EnforceMinMax = Convert.ToBoolean(ProjectFieldProperty.EnforceMinMax);
                            SectionField.field_AllowMultiSelection = ProjectFieldProperty.AllowMultiSelection;
                            SectionField.field_MaxLengthValue = ProjectFieldProperty.MaxLengthValue;
                            SectionField.field_MinLengthValue = ProjectFieldProperty.MinLengthValue;
                            SectionField.field_IsRequired = Convert.ToBoolean(ProjectFieldProperty.IsRequired);
                            SectionField.field_IsEnabled = Convert.ToBoolean(ProjectFieldProperty.IsEnabled);
                            SectionField.field_CaptureGeolocation = Convert.ToBoolean(ProjectFieldProperty.CaptureGeolocation);
                            SectionField.field_CaptureTimestamp = Convert.ToBoolean(ProjectFieldProperty.CaptureTimestamp);
                            SectionField.field_HasAlert = ProjectFieldProperty.HasAlert;



                            SectionField.field_PhotoQuality = ProjectFieldProperty.PhotoQuality;
                            SectionField.field_PhotoLibraryEnabled = Convert.ToBoolean(ProjectFieldProperty.PhotoLibraryEnabled);
                            SectionField.field_AllowAnnotation = Convert.ToBoolean(ProjectFieldProperty.AllowAnnotation);
                            SectionField.field_CameraEnabled = Convert.ToBoolean(ProjectFieldProperty.CameraEnabled);
                            SectionField.field_GPSTagging = Convert.ToBoolean(ProjectFieldProperty.GPSTagging);
                            SectionField.field_ExcludeonSync = Convert.ToBoolean(ProjectFieldProperty.ExcludeonSync);
                            SectionField.field_MaximumHeight = ProjectFieldProperty.MaximumHeight;
                            SectionField.field_MaximumWidth = ProjectFieldProperty.MaximumWidth;
                            SectionField.field_AcknowlegementText = ProjectFieldProperty.AcknowlegementText;
                            SectionField.field_AcknowlegementButtonText = ProjectFieldProperty.AcknowlegementButtonText;
                            SectionField.field_AcknowlegementClickedButtonText = ProjectFieldProperty.AcknowlegementClickedButtonText;
                            SectionField.field_PhotoLibraryEnabled = Convert.ToBoolean(ProjectFieldProperty.PhotoLibraryEnabled);
                            SectionField.field_MaxrecordabletimeInSeconds = ProjectFieldProperty.MaxrecordabletimeInSeconds;
                            SectionField.field_VideoQuality = ProjectFieldProperty.VideoQuality;

                            //AddedBy AniketJ on 03-Mar-2016
                            SectionField.field_FieldCalculation = ProjectFieldProperty.FieldCalculation;
                            SectionField.field_AllowRepeat = ProjectFieldProperty.AllowRepeat;


                            //AddedBy AniketJ on 15-Feb-2018
                            SectionField.field_SynchFlag = ProjectFieldProperty.SynchFlag;

                            SectionFieldList.Add(SectionField);
                        }
                    }
                }

                viewmodel.SectionFields = SectionFieldList;
                FormManagementViewModel.Add(viewmodel);
            }
            FormManagementViewModel = FormManagementViewModel.OrderByDescending(a => a.SectionOrder).ToList();
            return FormManagementViewModel;

        }


        public List<ListOptionViewModel> GetFormOptions(long Formid)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<ListOptionViewModel> ListOptionViewModel = new List<ListOptionViewModel>();
            var FormSectionRelationdata = CompassEntities.tblFormSectionRelations.AsNoTracking().Where(o => o.Active == 1 && o.FormID == Formid).ToList();
            foreach (var item in FormSectionRelationdata)
            {





                var FormSectionFieldRelation = CompassEntities.tblFormSectionFieldRelations.AsNoTracking().Where(o => o.Active == 1 && o.SectionId == item.SectionId).ToList();

                if (FormSectionFieldRelation.Count > 0)
                {

                    foreach (var FormSectionRelation in FormSectionFieldRelation)
                    {


                        var ProjectFieldDDLMastersList = CompassEntities.tblProjectFieldDDLMasters.AsNoTracking().Where(o => o.Active == 1 && o.FormSectionFieldID == FormSectionRelation.FieldId).ToList();
                        if (ProjectFieldDDLMastersList != null)
                        {
                            ListOptionViewModel viewmodel = new ListOptionViewModel();
                            List<ListOptions> optionListList = new List<ListOptions>();

                            viewmodel.SectionID = item.SectionId;
                            viewmodel.SectionName = item.SectionName;
                            foreach (var ProjectFieldDDLMasters in ProjectFieldDDLMastersList)
                            {
                                ListOptions ListOption = new ListOptions();
                                viewmodel.FieldName = FormSectionRelation.FieldName;
                                ListOption.FieldName = FormSectionRelation.FieldName;
                                ListOption.optionID = ProjectFieldDDLMasters.ID;
                                ListOption.FieldText = ProjectFieldDDLMasters.FieldText;
                                ListOption.FieldValue = ProjectFieldDDLMasters.FieldValue;
                                ListOption.FieldFilterkey = ProjectFieldDDLMasters.FieldFilterkey;
                                ListOption.OrderNo = ProjectFieldDDLMasters.SortOrderNumber;

                                optionListList.Add(ListOption);
                            }

                            viewmodel.OptionList = optionListList;
                            ListOptionViewModel.Add(viewmodel);
                        }
                    }
                }


            }
            //ListOptionViewModel = ListOptionViewModel.OrderByDescending(a => a.).ToList();
            return ListOptionViewModel;

        }


        public bool SaveUpdateMasterFields(FormMasterFields ObjectToCreaetOrUpdate, List<FormManagementViewModel> formdata, List<ListOptionViewModel> optionList, int? loggeduserid, out string Message, ref long SavedFormID)
        {
            DateTime currentDate = new CommonFunctions().ServerDate();
            bool result = false;
            //SavedFormID = 0;
            Message = "Unable to Create / Update Form.";
            try
            {
                CompassEntities CompassEntities = new CompassEntities();

                //here check the duplicate Menu name
                // According to user requirement changes condition here for b.Menu.ToLower() == ObjectToCreaetOrUpdate.Menu.ToLower()
                tblFormMaster objChk = (from b in CompassEntities.tblFormMasters.AsNoTracking()
                                        where b.FormName.ToLower().Trim() == ObjectToCreaetOrUpdate.FormName.ToLower().Trim()
                                        && b.ProjectId == ObjectToCreaetOrUpdate.ProjectId
                                       && b.FormId != ObjectToCreaetOrUpdate.FormId
                                        select b).FirstOrDefault();

                using (TransactionScope ts = new TransactionScope())
                {
                    if (objChk == null)//No duplicate records
                    {
                        if (ObjectToCreaetOrUpdate.FormId == 0) //new record for insert
                        {
                            #region Add Master Fields in FormMaster Table
                            tblFormMaster createOrUpdate = new tblFormMaster();
                            createOrUpdate.FormName = ObjectToCreaetOrUpdate.FormName.Trim();
                            createOrUpdate.ProjectId = ObjectToCreaetOrUpdate.ProjectId;

                            createOrUpdate.FormTypeID = ObjectToCreaetOrUpdate.FormTypeID;

                            createOrUpdate.FromDate = Convert.ToDateTime(ObjectToCreaetOrUpdate.FromDate);
                            createOrUpdate.ToDate = Convert.ToDateTime(ObjectToCreaetOrUpdate.ToDate);
                            createOrUpdate.Active = 1;
                            createOrUpdate.CreatedOn = currentDate;
                            createOrUpdate.CreatedBy = loggeduserid;
                            CompassEntities.tblFormMasters.Add(createOrUpdate);
                            CompassEntities.SaveChanges();
                            SavedFormID = createOrUpdate.FormId;
                            #endregion

                            #region Save Fields in FormSection Relation Table

                            foreach (var itemsection in formdata)
                            {
                                tblFormSectionRelation tblFormSectionRelation = new tblFormSectionRelation();
                                tblFormSectionRelation.FormID = createOrUpdate.FormId;
                                tblFormSectionRelation.SectionName = itemsection.SectionName.Trim();
                                tblFormSectionRelation.SortOrderNumber = itemsection.SectionOrder;
                                tblFormSectionRelation.Active = 1;
                                tblFormSectionRelation.CreatedOn = currentDate;
                                tblFormSectionRelation.CreatedBy = loggeduserid;
                                CompassEntities.tblFormSectionRelations.Add(tblFormSectionRelation);

                                CompassEntities.SaveChanges();


                                #region Save Fields in FormSection Relation Table
                                if (itemsection.SectionFields != null)
                                {
                                    foreach (var itemFields in itemsection.SectionFields)
                                    {
                                        tblFormSectionFieldRelation tblFormSectionFieldRelation = new tblFormSectionFieldRelation();
                                        tblFormSectionFieldRelation.SectionId = tblFormSectionRelation.SectionId;
                                        tblFormSectionFieldRelation.FieldName = itemFields.field_FieldName;
                                        tblFormSectionFieldRelation.FieldOrder = itemFields.field_FieldOrder;
                                        tblFormSectionFieldRelation.TblFieldDataTypeMaster_Id = itemFields.field_DatatypeID;
                                        tblFormSectionFieldRelation.CreatedOn = currentDate;
                                        tblFormSectionFieldRelation.CreatedBy = loggeduserid;
                                        tblFormSectionFieldRelation.Active = 1;

                                        CompassEntities.tblFormSectionFieldRelations.Add(tblFormSectionFieldRelation);
                                        CompassEntities.SaveChanges();

                                        tblProjectFieldProperty tfp = new tblProjectFieldProperty();


                                        tfp.ProjectID = itemFields.field_ProjectID;
                                        tfp.FormSectionFieldID = tblFormSectionFieldRelation.FieldId;
                                        tfp.FDID = itemFields.field_DatatypeID;

                                        tfp.FieldLabel = itemFields.field_FieldLabel;
                                        tfp.DefaultValue = itemFields.field_DefaultValue;
                                        tfp.HintText = itemFields.field_HintText;
                                        tfp.FieldFilterkey = itemFields.field_FieldFilterkey;
                                        tfp.HideFieldLabel = itemFields.field_HideFieldLabel;
                                        tfp.AllowMultiSelection = itemFields.field_AllowMultiSelection;
                                        tfp.EnforceMinMax = itemFields.field_EnforceMinMax.ToString();
                                        tfp.MaxLengthValue = itemFields.field_MaxLengthValue;
                                        tfp.MinLengthValue = itemFields.field_MinLengthValue;
                                        tfp.IsRequired = itemFields.field_IsRequired != null ? bool.Parse(itemFields.field_IsRequired.ToString()) : false;
                                        tfp.IsEnabled = itemFields.field_IsEnabled != null ? bool.Parse(itemFields.field_IsEnabled.ToString()) : false;
                                        tfp.CaptureGeolocation = itemFields.field_CaptureGeolocation != null ? bool.Parse(itemFields.field_CaptureGeolocation.ToString()) : false;
                                        tfp.CaptureTimestamp = itemFields.field_CaptureTimestamp != null ? bool.Parse(itemFields.field_CaptureTimestamp.ToString()) : false;
                                        tfp.CreatedOn = currentDate;
                                        tfp.CreatedBy = loggeduserid;
                                        tfp.Active = 1;



                                        tfp.DecimalPositions = itemFields.field_DecimalPositions;
                                        tfp.HasAlert = itemFields.field_HasAlert != null ? bool.Parse(itemFields.field_HasAlert.ToString()) : false;
                                        tfp.Secure = itemFields.field_Secure != null ? bool.Parse(itemFields.field_Secure.ToString()) : false;
                                        tfp.allowprii = itemFields.field_allowprii != null ? bool.Parse(itemFields.field_allowprii.ToString()) : false;
                                        tfp.KeyboardType = itemFields.field_KeyboardType != null ? bool.Parse(itemFields.field_KeyboardType.ToString()) : false;
                                        tfp.FormatMask = itemFields.field_FormatMask;
                                        tfp.DisplayMask = itemFields.field_DisplayMask;
                                        tfp.CaptureMode = itemFields.field_CaptureMode;
                                        tfp.PhotoFile = itemFields.field_PhotoFile != null ? bool.Parse(itemFields.field_PhotoFile.ToString()) : false;
                                        tfp.AllowAnnotation = itemFields.field_AllowAnnotation != null ? bool.Parse(itemFields.field_AllowAnnotation.ToString()) : false;
                                        tfp.PhotoQuality = itemFields.field_PhotoQuality;
                                        tfp.MaximumHeight = itemFields.field_MaximumHeight;
                                        tfp.MaximumWidth = itemFields.field_MaximumWidth;
                                        tfp.ExcludeonSync = itemFields.field_ExcludeonSync != null ? bool.Parse(itemFields.field_ExcludeonSync.ToString()) : false;
                                        tfp.PhotoLibraryEnabled = itemFields.field_PhotoLibraryEnabled != null ? bool.Parse(itemFields.field_PhotoLibraryEnabled.ToString()) : false;
                                        tfp.CameraEnabled = itemFields.field_CameraEnabled != null ? bool.Parse(itemFields.field_CameraEnabled.ToString()) : false;
                                        tfp.GPSTagging = itemFields.field_GPSTagging != null ? bool.Parse(itemFields.field_GPSTagging.ToString()) : false;
                                        tfp.AllowNotApplicable = itemFields.field_AllowNotApplicable != null ? bool.Parse(itemFields.field_AllowNotApplicable.ToString()) : false;
                                        tfp.RatingMax = itemFields.field_RatingMax != null ? bool.Parse(itemFields.field_RatingMax.ToString()) : false;
                                        tfp.RatingLabels = itemFields.field_RatingLabels != null ? bool.Parse(itemFields.field_RatingLabels.ToString()) : false;
                                        tfp.ShowMultiselect = itemFields.field_ShowMultiselect != null ? bool.Parse(itemFields.field_ShowMultiselect.ToString()) : false;
                                        tfp.ShowRatingLabels = itemFields.field_ShowRatingLabels != null ? bool.Parse(itemFields.field_ShowRatingLabels.ToString()) : false;
                                        tfp.AllowMultiSelection = itemFields.field_AllowMultiSelection != null ? bool.Parse(itemFields.field_AllowMultiSelection.ToString()) : false;
                                        tfp.NumberOfColumnsForPhones = itemFields.field_NumberOfColumnsForPhones;
                                        tfp.NumberOfColumnsForTablets = itemFields.field_NumberOfColumnsForTablets;
                                        tfp.HasValues = itemFields.field_HasValues != null ? bool.Parse(itemFields.field_HasValues.ToString()) : false;
                                        tfp.FieldFilterkey = itemFields.field_FieldFilterkey;
                                        tfp.AcknowlegementText = itemFields.field_AcknowlegementText;
                                        tfp.AcknowlegementButtonText = itemFields.field_AcknowlegementButtonText;
                                        tfp.AcknowlegementClickedButtonText = itemFields.field_AcknowlegementClickedButtonText;
                                        tfp.HasFile = itemFields.field_HasFile != null ? bool.Parse(itemFields.field_HasFile.ToString()) : false;
                                        tfp.VideoQuality = itemFields.field_VideoQuality;
                                        tfp.MaxrecordabletimeInSeconds = itemFields.field_MaxrecordabletimeInSeconds;
                                        tfp.AudioQuality = itemFields.field_AudioQuality;

                                        //AddedBy AniketJ on 03-Mar-2017

                                        tfp.FieldCalculation = itemFields.field_FieldCalculation;
                                        tfp.AllowRepeat = itemFields.field_AllowRepeat;


                                        //AddedBy AniketJ on 15-Feb-2018
                                        tfp.SynchFlag = itemFields.field_SynchFlag;



                                        //  tfp.PhotoLibraryEnabled = itemFields.field_PhotoLibraryEnabled != null ? bool.Parse(itemFields.field_PhotoLibraryEnabled.ToString()) : false;
                                        //  tfp.AllowAnnotation = itemFields.field_AllowAnnotation != null ? bool.Parse(itemFields.field_AllowAnnotation.ToString()) : false; 
                                        // tfp.CameraEnabled = itemFields.field_CameraEnabled != null ? bool.Parse(itemFields.field_CameraEnabled.ToString()) : false; 
                                        // tfp.ExcludeonSync = itemFields.field_ExcludeonSync != null ? bool.Parse(itemFields.field_ExcludeonSync.ToString()) : false;
                                        //  tfp.MaximumHeight = itemFields.field_MaximumHeight;
                                        // tfp.MaximumWidth = itemFields.field_MaximumWidth;
                                        // tfp.AcknowlegementText = itemFields.field_AcknowlegementText;
                                        // tfp.AcknowlegementButtonText = itemFields.field_AcknowlegementButtonText;
                                        // tfp.AcknowlegementClickedButtonText = itemFields.field_AcknowlegementClickedButtonText;
                                        // tfp.PhotoLibraryEnabled = itemFields.field_PhotoLibraryEnabled != null ? bool.Parse(itemFields.field_PhotoLibraryEnabled.ToString()) : false;
                                        // tfp.MaxrecordabletimeInSeconds = itemFields.field_MaxrecordabletimeInSeconds;
                                        //  tfp.VideoQuality = itemFields.field_VideoQuality;



                                        CompassEntities.tblProjectFieldProperties.Add(tfp);
                                        CompassEntities.SaveChanges();



                                        if (itemFields.field_DatatypeID == 10 && itemFields.field_MappedtoExcel == "0")
                                        {
                                            var sectionname = itemsection.SectionName.Trim();
                                            var FildNamename = itemFields.field_FieldName;

                                            foreach (var options in optionList)
                                            {
                                                //ModifiedBy AniketJ on 01-Mar-2017
                                                //if (options.SectionName == sectionname && options.FieldName == FildNamename)
                                                if (options.FieldName == FildNamename)
                                                {
                                                    if (options.OptionList != null)
                                                    {
                                                        foreach (var optionitem in options.OptionList)
                                                        {
                                                            tblProjectFieldDDLMaster tblProjectFieldDDLMaster = new tblProjectFieldDDLMaster();
                                                            tblProjectFieldDDLMaster.FormSectionFieldID = tblFormSectionFieldRelation.FieldId;
                                                            tblProjectFieldDDLMaster.FormID = createOrUpdate.FormId;
                                                            tblProjectFieldDDLMaster.FieldText = optionitem.FieldText.Trim();
                                                            tblProjectFieldDDLMaster.FieldValue = optionitem.FieldValue.Trim();
                                                            tblProjectFieldDDLMaster.FieldFilterkey = optionitem.FieldFilterkey != null ? optionitem.FieldFilterkey.Trim() : "";
                                                            tblProjectFieldDDLMaster.SortOrderNumber = optionitem.OrderNo;

                                                            tblProjectFieldDDLMaster.Active = 1;
                                                            tblProjectFieldDDLMaster.CreatedOn = currentDate;
                                                            tblProjectFieldDDLMaster.CreatedBy = loggeduserid;
                                                            CompassEntities.tblProjectFieldDDLMasters.Add(tblProjectFieldDDLMaster);
                                                            CompassEntities.SaveChanges();
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }

                                #endregion
                            }


                            #endregion

                            ts.Complete();
                            //change message accroding to your requirement

                            Message = CommonFunctions.strRecordCreated;
                            result = true;
                        }
                        else //update records
                        {

                            #region Deactivate old record
                            var FormMasters = (from b in CompassEntities.tblFormMasters
                                               where b.FormId == ObjectToCreaetOrUpdate.FormId && b.Active == 1
                                               select b).FirstOrDefault();
                            List<ProjectFormCustomeListBO> objProjectFormCustomeListBO = new List<ProjectFormCustomeListBO>();
                            if (FormMasters != null)
                            {

                                #region Deactivate FormSectionRelationList
                                var FormSectionRelationList = (from c in CompassEntities.tblFormSectionRelations
                                                               where c.FormID == FormMasters.FormId && c.Active == 1
                                                               select c).ToList();

                                if (FormSectionRelationList != null)
                                {
                                    foreach (var FormSectionRelation in FormSectionRelationList)
                                    {
                                        var FormSectionFieldRelationList = (from d in CompassEntities.tblFormSectionFieldRelations
                                                                            where d.SectionId == FormSectionRelation.SectionId && d.Active == 1
                                                                            select d).ToList();
                                        if (FormSectionFieldRelationList != null)
                                        {
                                            foreach (var FormSectionFieldRelation in FormSectionFieldRelationList)
                                            {
                                                var ProjectFieldPropertyList = (from e in CompassEntities.tblProjectFieldProperties
                                                                                where e.FormSectionFieldID == FormSectionFieldRelation.FieldId && e.Active == 1
                                                                                select e).ToList();
                                                if (ProjectFieldPropertyList != null)
                                                {
                                                    foreach (var ProjectFieldProperty in ProjectFieldPropertyList)
                                                    {
                                                        ProjectFieldProperty.Active = 0;
                                                        CompassEntities.SaveChanges();
                                                    }
                                                }

                                                var ProjectFieldDDLMasterList = (from f in CompassEntities.tblProjectFieldDDLMasters
                                                                                 where f.FormSectionFieldID == FormSectionFieldRelation.FieldId && f.Active == 1
                                                                                 select f).ToList();

                                                if (ProjectFieldDDLMasterList != null)
                                                {
                                                    ProjectFormCustomeListBO objold = new ProjectFormCustomeListBO();
                                                    objold.fieldId = FormSectionFieldRelation.FieldId;
                                                    objold.fieldName = FormSectionFieldRelation.FieldName;
                                                    objold.objList = new List<tblProjectFieldDDLMaster>();
                                                    foreach (var ProjectFieldDDLMaster in ProjectFieldDDLMasterList)
                                                    {
                                                        objold.objList.Add(new tblProjectFieldDDLMaster
                                                        {
                                                            FieldValue = ProjectFieldDDLMaster.FieldValue,
                                                            FieldText = ProjectFieldDDLMaster.FieldText,
                                                            FieldFilterkey = ProjectFieldDDLMaster.FieldFilterkey,
                                                            SortOrderNumber = ProjectFieldDDLMaster.SortOrderNumber
                                                        });

                                                        ProjectFieldDDLMaster.Active = 0;
                                                        CompassEntities.SaveChanges();
                                                    }
                                                    objProjectFormCustomeListBO.Add(objold);
                                                }
                                                FormSectionFieldRelation.Active = 0;
                                                CompassEntities.SaveChanges();
                                            }

                                        }
                                        FormSectionRelation.Active = 0;
                                        CompassEntities.SaveChanges();
                                    }
                                }
                                #endregion

                                FormMasters.FormName = ObjectToCreaetOrUpdate.FormName;
                                FormMasters.FromDate = Convert.ToDateTime(ObjectToCreaetOrUpdate.FromDate);
                                FormMasters.FormTypeID = ObjectToCreaetOrUpdate.FormTypeID;
                                FormMasters.ToDate = Convert.ToDateTime(ObjectToCreaetOrUpdate.ToDate);
                                CompassEntities.SaveChanges();

                                #region Save Fields in FormSection Relation Table

                                foreach (var itemsection in formdata)
                                {
                                    tblFormSectionRelation tblFormSectionRelation = new tblFormSectionRelation();
                                    tblFormSectionRelation.FormID = FormMasters.FormId;
                                    tblFormSectionRelation.SectionName = itemsection.SectionName.Trim();
                                    tblFormSectionRelation.SortOrderNumber = itemsection.SectionOrder;
                                    tblFormSectionRelation.Active = 1;
                                    tblFormSectionRelation.CreatedOn = currentDate;
                                    tblFormSectionRelation.CreatedBy = loggeduserid;
                                    CompassEntities.tblFormSectionRelations.Add(tblFormSectionRelation);

                                    CompassEntities.SaveChanges();


                                    #region Save Fields in FormSection Relation Table
                                    if (itemsection.SectionFields != null)
                                    {
                                        foreach (var itemFields in itemsection.SectionFields)
                                        {
                                            tblFormSectionFieldRelation tblFormSectionFieldRelation = new tblFormSectionFieldRelation();
                                            tblFormSectionFieldRelation.SectionId = tblFormSectionRelation.SectionId;
                                            tblFormSectionFieldRelation.FieldName = itemFields.field_FieldName;
                                            tblFormSectionFieldRelation.FieldOrder = itemFields.field_FieldOrder;
                                            tblFormSectionFieldRelation.TblFieldDataTypeMaster_Id = itemFields.field_DatatypeID;
                                            tblFormSectionFieldRelation.CreatedOn = currentDate;
                                            tblFormSectionFieldRelation.CreatedBy = loggeduserid;
                                            tblFormSectionFieldRelation.Active = 1;

                                            CompassEntities.tblFormSectionFieldRelations.Add(tblFormSectionFieldRelation);
                                            CompassEntities.SaveChanges();

                                            tblProjectFieldProperty tfp = new tblProjectFieldProperty();


                                            tfp.ProjectID = itemFields.field_ProjectID;
                                            tfp.FormSectionFieldID = tblFormSectionFieldRelation.FieldId;
                                            tfp.FDID = itemFields.field_DatatypeID;

                                            tfp.FieldLabel = itemFields.field_FieldLabel;
                                            tfp.DefaultValue = itemFields.field_DefaultValue;
                                            tfp.HintText = itemFields.field_HintText;
                                            tfp.FieldFilterkey = itemFields.field_FieldFilterkey;
                                            tfp.EnforceMinMax = itemFields.field_EnforceMinMax.ToString();
                                            tfp.HideFieldLabel = itemFields.field_HideFieldLabel;
                                            tfp.AllowMultiSelection = itemFields.field_AllowMultiSelection;
                                            tfp.MaxLengthValue = itemFields.field_MaxLengthValue;
                                            tfp.MinLengthValue = itemFields.field_MinLengthValue;
                                            tfp.IsRequired = itemFields.field_IsRequired != null ? bool.Parse(itemFields.field_IsRequired.ToString()) : false;
                                            tfp.IsEnabled = itemFields.field_IsEnabled != null ? bool.Parse(itemFields.field_IsEnabled.ToString()) : false;
                                            tfp.CaptureGeolocation = itemFields.field_CaptureGeolocation != null ? bool.Parse(itemFields.field_CaptureGeolocation.ToString()) : false;
                                            tfp.CaptureTimestamp = itemFields.field_CaptureTimestamp != null ? bool.Parse(itemFields.field_CaptureTimestamp.ToString()) : false;
                                            tfp.CreatedOn = currentDate;
                                            tfp.CreatedBy = loggeduserid;
                                            tfp.Active = 1;


                                            tfp.DecimalPositions = itemFields.field_DecimalPositions;
                                            tfp.HasAlert = itemFields.field_HasAlert != null ? bool.Parse(itemFields.field_HasAlert.ToString()) : false;
                                            tfp.Secure = itemFields.field_Secure != null ? bool.Parse(itemFields.field_Secure.ToString()) : false;
                                            tfp.allowprii = itemFields.field_allowprii != null ? bool.Parse(itemFields.field_allowprii.ToString()) : false;
                                            tfp.KeyboardType = itemFields.field_KeyboardType != null ? bool.Parse(itemFields.field_KeyboardType.ToString()) : false;
                                            tfp.FormatMask = itemFields.field_FormatMask;
                                            tfp.DisplayMask = itemFields.field_DisplayMask;
                                            tfp.CaptureMode = itemFields.field_CaptureMode;
                                            tfp.PhotoFile = itemFields.field_PhotoFile != null ? bool.Parse(itemFields.field_PhotoFile.ToString()) : false;
                                            tfp.AllowAnnotation = itemFields.field_AllowAnnotation != null ? bool.Parse(itemFields.field_AllowAnnotation.ToString()) : false;
                                            tfp.PhotoQuality = itemFields.field_PhotoQuality;
                                            tfp.MaximumHeight = itemFields.field_MaximumHeight;
                                            tfp.MaximumWidth = itemFields.field_MaximumWidth;
                                            tfp.ExcludeonSync = itemFields.field_ExcludeonSync != null ? bool.Parse(itemFields.field_ExcludeonSync.ToString()) : false;
                                            tfp.PhotoLibraryEnabled = itemFields.field_PhotoLibraryEnabled != null ? bool.Parse(itemFields.field_PhotoLibraryEnabled.ToString()) : false;
                                            tfp.CameraEnabled = itemFields.field_CameraEnabled != null ? bool.Parse(itemFields.field_CameraEnabled.ToString()) : false;
                                            tfp.GPSTagging = itemFields.field_GPSTagging != null ? bool.Parse(itemFields.field_GPSTagging.ToString()) : false;
                                            tfp.AllowNotApplicable = itemFields.field_AllowNotApplicable != null ? bool.Parse(itemFields.field_AllowNotApplicable.ToString()) : false;
                                            tfp.RatingMax = itemFields.field_RatingMax != null ? bool.Parse(itemFields.field_RatingMax.ToString()) : false;
                                            tfp.RatingLabels = itemFields.field_RatingLabels != null ? bool.Parse(itemFields.field_RatingLabels.ToString()) : false;
                                            tfp.ShowMultiselect = itemFields.field_ShowMultiselect != null ? bool.Parse(itemFields.field_ShowMultiselect.ToString()) : false;
                                            tfp.ShowRatingLabels = itemFields.field_ShowRatingLabels != null ? bool.Parse(itemFields.field_ShowRatingLabels.ToString()) : false;
                                            tfp.AllowMultiSelection = itemFields.field_AllowMultiSelection != null ? bool.Parse(itemFields.field_AllowMultiSelection.ToString()) : false;
                                            tfp.NumberOfColumnsForPhones = itemFields.field_NumberOfColumnsForPhones;
                                            tfp.NumberOfColumnsForTablets = itemFields.field_NumberOfColumnsForTablets;
                                            tfp.HasValues = itemFields.field_HasValues != null ? bool.Parse(itemFields.field_HasValues.ToString()) : false;
                                            tfp.FieldFilterkey = itemFields.field_FieldFilterkey;
                                            tfp.AcknowlegementText = itemFields.field_AcknowlegementText;
                                            tfp.AcknowlegementButtonText = itemFields.field_AcknowlegementButtonText;
                                            tfp.AcknowlegementClickedButtonText = itemFields.field_AcknowlegementClickedButtonText;
                                            tfp.HasFile = itemFields.field_HasFile != null ? bool.Parse(itemFields.field_HasFile.ToString()) : false;
                                            tfp.VideoQuality = itemFields.field_VideoQuality;
                                            tfp.MaxrecordabletimeInSeconds = itemFields.field_MaxrecordabletimeInSeconds;
                                            tfp.AudioQuality = itemFields.field_AudioQuality;

                                            //Added By AniketJ on 03-Mar-2017
                                            tfp.FieldCalculation = itemFields.field_FieldCalculation;
                                            tfp.AllowRepeat = itemFields.field_AllowRepeat;

                                            //AddedBy AniketJ on 15-Feb-2018
                                            tfp.SynchFlag = itemFields.field_SynchFlag;


                                            //tfp.PhotoLibraryEnabled = itemFields.field_PhotoLibraryEnabled != null ? bool.Parse(itemFields.field_PhotoLibraryEnabled.ToString()) : false;
                                            //tfp.AllowAnnotation = itemFields.field_AllowAnnotation != null ? bool.Parse(itemFields.field_AllowAnnotation.ToString()) : false;
                                            //tfp.CameraEnabled = itemFields.field_CameraEnabled != null ? bool.Parse(itemFields.field_CameraEnabled.ToString()) : false;
                                            //tfp.ExcludeonSync = itemFields.field_ExcludeonSync != null ? bool.Parse(itemFields.field_ExcludeonSync.ToString()) : false;
                                            //tfp.MaximumHeight = itemFields.field_MaximumHeight;
                                            //tfp.MaximumWidth = itemFields.field_MaximumWidth;
                                            //tfp.AcknowlegementText = itemFields.field_AcknowlegementText;
                                            //tfp.AcknowlegementButtonText = itemFields.field_AcknowlegementButtonText;
                                            //tfp.AcknowlegementClickedButtonText = itemFields.field_AcknowlegementClickedButtonText;
                                            //tfp.PhotoLibraryEnabled = itemFields.field_PhotoLibraryEnabled != null ? bool.Parse(itemFields.field_PhotoLibraryEnabled.ToString()) : false;
                                            //tfp.MaxrecordabletimeInSeconds = itemFields.field_MaxrecordabletimeInSeconds;
                                            //tfp.VideoQuality = itemFields.field_VideoQuality;



                                            CompassEntities.tblProjectFieldProperties.Add(tfp);
                                            CompassEntities.SaveChanges();


                                            if (itemFields.field_DatatypeID == 10 && (itemFields.field_MappedtoExcel == "0" || itemFields.field_MappedtoExcel == null))
                                            {
                                                var sectionname = itemsection.SectionName.Trim();
                                                var FildNamename = itemFields.field_FieldName;

                                                foreach (var options in optionList)
                                                {
                                                    //Modifiedby AniketJ on 01-Mar-2017
                                                    //if (options.SectionName == sectionname && options.FieldName == FildNamename)
                                                    if (options.FieldName == FildNamename)
                                                    {
                                                        if (options.OptionList != null)
                                                        {

                                                            foreach (var optionitem in options.OptionList)
                                                            {
                                                                tblProjectFieldDDLMaster tblProjectFieldDDLMaster = new tblProjectFieldDDLMaster();
                                                                tblProjectFieldDDLMaster.FormSectionFieldID = tblFormSectionFieldRelation.FieldId;
                                                                tblProjectFieldDDLMaster.FormID = FormMasters.FormId;
                                                                tblProjectFieldDDLMaster.FieldText = optionitem.FieldText.Trim();
                                                                tblProjectFieldDDLMaster.FieldValue = optionitem.FieldValue.Trim();
                                                                tblProjectFieldDDLMaster.FieldFilterkey = optionitem.FieldFilterkey != null ? optionitem.FieldFilterkey.Trim() : "";
                                                                tblProjectFieldDDLMaster.SortOrderNumber = optionitem.OrderNo;

                                                                tblProjectFieldDDLMaster.Active = 1;
                                                                tblProjectFieldDDLMaster.CreatedOn = currentDate;
                                                                tblProjectFieldDDLMaster.CreatedBy = loggeduserid;
                                                                CompassEntities.tblProjectFieldDDLMasters.Add(tblProjectFieldDDLMaster);
                                                                CompassEntities.SaveChanges();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var objOldRecordCustomList = objProjectFormCustomeListBO.Where(o => o.fieldName == FildNamename).FirstOrDefault();

                                                            if (objOldRecordCustomList.objList != null)
                                                            {
                                                                foreach (var optionitem in objOldRecordCustomList.objList)
                                                                {
                                                                    tblProjectFieldDDLMaster tblProjectFieldDDLMaster = new tblProjectFieldDDLMaster();
                                                                    tblProjectFieldDDLMaster.FormSectionFieldID = tblFormSectionFieldRelation.FieldId;
                                                                    tblProjectFieldDDLMaster.FormID = FormMasters.FormId;
                                                                    tblProjectFieldDDLMaster.FieldText = optionitem.FieldText.Trim();
                                                                    tblProjectFieldDDLMaster.FieldValue = optionitem.FieldValue.Trim();
                                                                    tblProjectFieldDDLMaster.FieldFilterkey = optionitem.FieldFilterkey != null ? optionitem.FieldFilterkey.Trim() : "";
                                                                    tblProjectFieldDDLMaster.SortOrderNumber = optionitem.SortOrderNumber;

                                                                    tblProjectFieldDDLMaster.Active = 1;
                                                                    tblProjectFieldDDLMaster.CreatedOn = currentDate;
                                                                    tblProjectFieldDDLMaster.CreatedBy = loggeduserid;
                                                                    CompassEntities.tblProjectFieldDDLMasters.Add(tblProjectFieldDDLMaster);
                                                                    CompassEntities.SaveChanges();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    #endregion
                                }


                                #endregion

                            }
                            #endregion

                            ts.Complete();
                            Message = CommonFunctions.strRecordUpdated;
                            result = true;

                            SavedFormID = ObjectToCreaetOrUpdate.FormId;
                        }
                    }

                    else
                    {
                        if (objChk.Active == 1)
                        {
                            Message = CommonFunctions.strActiveRecordExists;
                            return false;
                        }
                        else if (objChk.Active == 0)
                        {
                            Message = CommonFunctions.strDeactiveRecordExists;
                            return false;
                        }
                        else
                        {
                            Message = CommonFunctions.strDefaultAdd;
                            return false;
                        }

                        //change message accroding to your requirement
                        // Message = "Sub Menu already exists.";
                        result = false;
                    }
                }

            }

            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    Message += "<br/>" + ex.Message;
                }
                else
                {
                    Message += "<br/>" + ((ex.InnerException).InnerException).Message;
                }
                //Message += "<br/>" + ex.Message;
            }

            return result;

        }


        public bool DeactivateRecord(long formid, out string Message)
        {
            CompassEntities CompassEntities = new CompassEntities();
            bool result = false;
            Message = "Unable to delete record.";
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var FormMaster = (from c in CompassEntities.tblFormMasters
                                      where c.FormId == formid && c.Active == 1
                                      select c).FirstOrDefault();

                    if (FormMaster != null)
                    {
                        //#region Deactivate FormSectionRelationList

                        //var FormSectionRelationList = (from c in CompassEntities.tblFormSectionRelations
                        //                               where c.FormID == FormMaster.FormId && c.Active == 1
                        //                               select c).ToList();

                        //if (FormSectionRelationList != null)
                        //{
                        //    foreach (var FormSectionRelation in FormSectionRelationList)
                        //    {
                        //var FormSectionFieldRelationList = (from d in CompassEntities.tblFormSectionFieldRelations
                        //                                    where d.SectionId == FormSectionRelation.SectionId && d.Active == 1
                        //                                    select d).ToList();
                        //if (FormSectionFieldRelationList != null)
                        //{
                        //    foreach (var FormSectionFieldRelation in FormSectionFieldRelationList)
                        //    {
                        //        var ProjectFieldPropertyList = (from e in CompassEntities.tblProjectFieldProperties
                        //                                        where e.FormSectionFieldID == FormSectionFieldRelation.FieldId && e.Active == 1
                        //                                        select e).ToList();
                        //        if (ProjectFieldPropertyList != null)
                        //        {
                        //            foreach (var ProjectFieldProperty in ProjectFieldPropertyList)
                        //            {
                        //                ProjectFieldProperty.Active = 0;
                        //                CompassEntities.SaveChanges();
                        //            }
                        //        }

                        //        var ProjectFieldDDLMasterList = (from f in CompassEntities.tblProjectFieldDDLMasters
                        //                                         where f.FormSectionFieldID == FormSectionFieldRelation.FieldId && f.Active == 1
                        //                                         select f).ToList();
                        //        if (ProjectFieldDDLMasterList != null)
                        //        {
                        //            foreach (var ProjectFieldDDLMaster in ProjectFieldDDLMasterList)
                        //            {
                        //                ProjectFieldDDLMaster.Active = 0;
                        //                CompassEntities.SaveChanges();
                        //            }
                        //        }


                        //        FormSectionFieldRelation.Active = 0;
                        //        CompassEntities.SaveChanges();
                        //    }

                        //}
                        //        FormSectionRelation.Active = 0;
                        //        CompassEntities.SaveChanges();
                        //    }
                        //}

                        //#endregion
                        FormMaster.Active = 0;
                        CompassEntities.SaveChanges();

                        ts.Complete();
                        Message = CommonFunctions.strRecordDeactivated;
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    Message += "<br/>" + ex.Message;
                }
                else
                {
                    Message += "<br/>" + ((ex.InnerException).InnerException).Message;
                }
            }

            return result;


        }


        public bool ActivateRecord(long formid, out string Message)
        {
            CompassEntities CompassEntities = new CompassEntities();
            bool result = false;
            Message = "Unable to delete record.";
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    var FormMaster = (from c in CompassEntities.tblFormMasters
                                      where c.FormId == formid && c.Active == 0
                                      select c).FirstOrDefault();

                    if (FormMaster != null)
                    {
                        //    #region Deactivate FormSectionRelationList

                        //    var FormSectionRelationList = (from c in CompassEntities.tblFormSectionRelations
                        //                                   where c.FormID == FormMaster.FormId && c.Active == 0
                        //                                   select c).ToList();

                        //    if (FormSectionRelationList != null)
                        //    {
                        //        foreach (var FormSectionRelation in FormSectionRelationList)
                        //        {
                        //var FormSectionFieldRelationList = (from d in CompassEntities.tblFormSectionFieldRelations
                        //                                    where d.SectionId == FormSectionRelation.SectionId && d.Active == 0
                        //                                    select d).ToList();
                        //if (FormSectionFieldRelationList != null)
                        //{
                        //    foreach (var FormSectionFieldRelation in FormSectionFieldRelationList)
                        //    {
                        //        var ProjectFieldPropertyList = (from e in CompassEntities.tblProjectFieldProperties
                        //                                        where e.FormSectionFieldID == FormSectionFieldRelation.FieldId && e.Active == 0
                        //                                        select e).ToList();
                        //        if (ProjectFieldPropertyList != null)
                        //        {
                        //            foreach (var ProjectFieldProperty in ProjectFieldPropertyList)
                        //            {
                        //                ProjectFieldProperty.Active = 1;
                        //                CompassEntities.SaveChanges();
                        //            }
                        //        }


                        //        var ProjectFieldDDLMasterList = (from f in CompassEntities.tblProjectFieldDDLMasters
                        //                                         where f.FormSectionFieldID == FormSectionFieldRelation.FieldId && f.Active == 0
                        //                                         select f).ToList();
                        //        if (ProjectFieldDDLMasterList != null)
                        //        {
                        //            foreach (var ProjectFieldDDLMaster in ProjectFieldDDLMasterList)
                        //            {
                        //                ProjectFieldDDLMaster.Active = 1;
                        //                CompassEntities.SaveChanges();
                        //            }
                        //        }

                        //        FormSectionFieldRelation.Active = 1;
                        //        CompassEntities.SaveChanges();
                        //    }

                        //}
                        //        FormSectionRelation.Active = 1;
                        //        CompassEntities.SaveChanges();
                        //    }
                        //}

                        //#endregion
                        FormMaster.Active = 1;
                        CompassEntities.SaveChanges();

                        ts.Complete();
                        Message = CommonFunctions.strRecordActivated;
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                {
                    Message += "<br/>" + ex.Message;
                }
                else
                {
                    Message += "<br/>" + ((ex.InnerException).InnerException).Message;
                }
            }

            return result;


        }



        public List<FormMasterFields> GetFormsbyProjectID(long projectID)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var data = CompassEntities.tblFormMasters.AsNoTracking().Where(o => o.Active == 1 && o.ProjectId == projectID).AsEnumerable().
                Select(o => new FormMasterFields
                {
                    FormId = o.FormId,
                    FormName = o.FormName,
                    ProjectId = o.ProjectId,
                    FromDate = o.FromDate.ToString("MMM-dd-yyyy"),
                    ToDate = o.ToDate.ToString("MMM-dd-yyyy"),

                }).ToList();

            return data;

        }

    }
}
