using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    class FormManagementModel
    {
    }

    public class FormManagementFields
    {
        public long field_ProjectID { get; set; }
        public string field_SectionID { get; set; }
        public long field_FieldID { get; set; }
        public string field_FieldName { get; set; }
        public int field_FieldOrder { get; set; }
        public string field_DatatypeName { get; set; }
        public long field_DatatypeID { get; set; }
        //public string field_title { get; set; }
        //public string field_value { get; set; }
        public string field_HintText { get; set; }
        public Nullable<bool> field_HideFieldLabel { get; set; }
        public string field_FieldFilterkey { get; set; }
        public Nullable<bool> field_EnforceMinMax { get; set; }
        public string field_MaxLengthValue { get; set; }
        public string field_MinLengthValue { get; set; }
        public Nullable<bool> field_IsRequired { get; set; }
        public Nullable<bool> field_IsEnabled { get; set; }
        public Nullable<bool> field_CaptureGeolocation { get; set; }
        public Nullable<bool> field_CaptureTimestamp { get; set; }
        public Nullable<bool> field_HasAlert { get; set; }




        public Nullable<long> field_FormSectionFieldID { get; set; }
        public Nullable<long> field_FDID { get; set; }
        public string field_FieldLabel { get; set; }
        public string field_DefaultValue { get; set; }
        public string field_DecimalPositions { get; set; }

        public Nullable<bool> field_Secure { get; set; }
        public Nullable<bool> field_allowprii { get; set; }
        public Nullable<bool> field_KeyboardType { get; set; }
        public string field_FormatMask { get; set; }
        public string field_DisplayMask { get; set; }
        public string field_CaptureMode { get; set; }
        public Nullable<bool> field_PhotoFile { get; set; }
        public Nullable<bool> field_AllowAnnotation { get; set; }
        public string field_PhotoQuality { get; set; }
        public string field_MaximumHeight { get; set; }
        public string field_MaximumWidth { get; set; }
        public Nullable<bool> field_ExcludeonSync { get; set; }
        public Nullable<bool> field_PhotoLibraryEnabled { get; set; }
        public Nullable<bool> field_CameraEnabled { get; set; }
        public Nullable<bool> field_GPSTagging { get; set; }
        public Nullable<bool> field_AllowNotApplicable { get; set; }
        public Nullable<bool> field_RatingMax { get; set; }
        public Nullable<bool> field_RatingLabels { get; set; }
        public Nullable<bool> field_ShowMultiselect { get; set; }
        public Nullable<bool> field_ShowRatingLabels { get; set; }
        public Nullable<bool> field_AllowMultiSelection { get; set; }
        public string field_NumberOfColumnsForPhones { get; set; }
        public string field_NumberOfColumnsForTablets { get; set; }
        public Nullable<bool> field_HasValues { get; set; }

        public string field_AcknowlegementText { get; set; }
        public string field_AcknowlegementButtonText { get; set; }
        public string field_AcknowlegementClickedButtonText { get; set; }
        public Nullable<bool> field_HasFile { get; set; }
        public string field_VideoQuality { get; set; }
        public string field_MaxrecordabletimeInSeconds { get; set; }
        public string field_AudioQuality { get; set; }


        public string field_MappedtoExcel { get; set; }
        public string field_FieldCalculation { get; set; }
        public Nullable<bool> field_AllowRepeat { get; set; }

        public Nullable<bool> field_SynchFlag { get; set; }

    }

    public class FormManagementViewModel
    {
        public long SectionID { get; set; }
        public string SectionName { get; set; }
        public int SectionOrder { get; set; }

        public List<FormManagementFields> SectionFields { get; set; }
    }


    public class ListOptionViewModel
    {
        public long SectionID { get; set; }
        public string SectionName { get; set; }
        public string FieldName { get; set; }

        public List<ListOptions> OptionList { get; set; }
    }

    public class ListOptions
    {
        public long optionID { get; set; }
        public int? OrderNo { get; set; }
        public string FieldText { get; set; }
        public string FieldValue { get; set; }
        public string FieldFilterkey { get; set; }
        public string FieldName { get; set; }
    }


    public class FormMasterFields
    {
        public long FormId { get; set; }
        public long ProjectId { get; set; }
        public string FormName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public long? FormTypeID { get; set; }
    }


    public class ProjectFormCustomeListBO
    {
        public long fieldId { get; set; }
        public string fieldName { get; set; }
        public List<tblProjectFieldDDLMaster> objList { get; set; }
    }
}
