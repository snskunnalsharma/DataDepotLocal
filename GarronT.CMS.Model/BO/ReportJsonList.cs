using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class SectionList
    {
        public long SectionId { get; set; }
        public string SectionName { get; set; }
    }

    public class projectFiledData
    {
        public long ID { get; set; }
        public long tblUploadID { get; set; }
    }

    public class ProjectDate
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string frmStartDate { get; set; }
        public string frmEndDate { get; set; }
    }

    public class FromDetails
    {
        public long FormId { get; set; }
        public string FormName { get; set; }
    }

    public class FieldNames
    {
        public long? SectionId { get; set; }
        public string SectionName { get; set; }
        public long FieldId { get; set; }
        public string FieldName { get; set; }
        public long? TblFieldDataTypeId { get; set; }
        public string FielddatatypeNames { get; set; }
        public string FileddataTypedescription { get; set; }
        public long Id { get; set; }
        public long? ProjectID { get; set; }
        public long? FormSectionFieldID { get; set; }
        public long? FDID { get; set; }
        public string FieldLabel { get; set; }
        public string DefaultValue { get; set; }
        public string DecimalPositions { get; set; }
        public string HintText { get; set; }
        public bool? EnforceMinMax { get; set; }
        public string MaxLengthValue { get; set; }
        public string MinLengthValue { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? CaptureGeolocation { get; set; }
        public bool? CaptureTimestamp { get; set; }
        public bool? HasAlert { get; set; }
        public bool? Secure { get; set; }
        public bool? allowprii { get; set; }
        public bool? KeyboardType { get; set; }
        public string FormatMask { get; set; }
        public string DisplayMask { get; set; }
        public string CaptureMode { get; set; }
        public bool? PhotoFile { get; set; }
        public bool? AllowAnnotation { get; set; }
        public string PhotoQuality { get; set; }
        public string MaximumHeight { get; set; }
        public string MaximumWidth { get; set; }
        public bool? ExcludeonSync { get; set; }
        public bool? PhotoLibraryEnabled { get; set; }
        public bool? CameraEnabled { get; set; }
        public bool? GPSTagging { get; set; }
        public bool? AllowNotApplicable { get; set; }
        public bool? RatingMax { get; set; }
        public bool? RatingLabels { get; set; }
        public bool? ShowMultiselect { get; set; }
        public bool? ShowRatingLabels { get; set; }
        public bool? AllowMultiSelection { get; set; }
        public string NumberOfColumnsForPhones { get; set; }
        public string NumberOfColumnsForTablets { get; set; }
        public bool? HasValues { get; set; }
        public string FieldFilterkey { get; set; }
        public string AcknowlegementText { get; set; }
        public string AcknowlegementButtonText { get; set; }
        public string AcknowlegementClickedButtonText { get; set; }
        public bool? HasFile { get; set; }
        public string VideoQuality { get; set; }
        public string MaxrecordabletimeInSeconds { get; set; }
        public string AudioQuality { get; set; }
        public string FieldText { get; set; }
        public string Fieldvalue { get; set; }
        public long? initialId { get; set; }
        public int Count { get; set; }

        public string CurrentLat { get; set; }
        public string CurrentLong { get; set; }
        public long tblProjectFieldDataMasterId { get; set; }
    }
    public class FieldProperty
    {
        public long Id { get; set; }
        public long? ProjectID { get; set; }
        public long? FormSectionFieldID { get; set; }
        public long? FDID { get; set; }
        public string FieldLabel { get; set; }
        public string DefaultValue { get; set; }
        public string DecimalPositions { get; set; }
        public string HintText { get; set; }
        public string EnforceMinMax { get; set; }
        public string MaxLengthValue { get; set; }
        public string MinLengthValue { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? CaptureGeolocation { get; set; }
        public bool? CaptureTimestamp { get; set; }
        public bool? HasAlert { get; set; }
        public bool? Secure { get; set; }
        public bool? allowprii { get; set; }
        public bool? KeyboardType { get; set; }
        public string FormatMask { get; set; }
        public string DisplayMask { get; set; }
        public string CaptureMode { get; set; }
        public bool? PhotoFile { get; set; }
        public bool? AllowAnnotation { get; set; }
        public string PhotoQuality { get; set; }
        public string MaximumHeight { get; set; }
        public string MaximumWidth { get; set; }
        public bool? ExcludeonSync { get; set; }
        public bool? PhotoLibraryEnabled { get; set; }
        public bool? CameraEnabled { get; set; }
        public bool? GPSTagging { get; set; }
        public bool? AllowNotApplicable { get; set; }
        public bool? RatingMax { get; set; }
        public bool? RatingLabels { get; set; }
        public bool? ShowMultiselect { get; set; }
        public bool? ShowRatingLabels { get; set; }
        public bool? AllowMultiSelection { get; set; }
        public string NumberOfColumnsForPhones { get; set; }
        public string NumberOfColumnsForTablets { get; set; }
        public bool? HasValues { get; set; }
        public string FieldFilterkey { get; set; }
        public string AcknowlegementText { get; set; }
        public string AcknowlegementButtonText { get; set; }
        public string AcknowlegementClickedButtonText { get; set; }
        public bool? HasFile { get; set; }
        public string VideoQuality { get; set; }
        public string MaxrecordabletimeInSeconds { get; set; }
        public string AudioQuality { get; set; }
        public string FieldDatatypeName { get; set; }

    }

    public class FieldDDLMaster
    {
        public long Id { get; set; }
        public string FieldText { get; set; }
        public string Fieldvalue { get; set; }


    }

    public class ProjectFildDataTypeMaster
    {
        public long ID { get; set; }
        public long? InitialFieldId { get; set; }
        public string InitialFieldName { get; set; }
        public string FieldValue { get; set; }
        public string FieldStatus { get; set; }
        public long? ProjectAttachmentId { get; set; }
        public long? InitialFieldIdAttach { get; set; }
        public int IsAudioVideoImage { get; set; }
        public string strFilePath { get; set; }
        public bool? isservicesData { get; set; }
        public long? tblservices_Id { get; set; }
        public long? tblservicePic_Id { get; set; }
        public int Active { get; set; }
        public string ThumbnailPath { get; set; }
    }

    public class sectionfiledval
    {
        public long? IDS { get; set; }
        public string sectionName { get; set; }
        public long? FieldId { get; set; }
        public string FieldName { get; set; }
        public string FieldValues { get; set; }
        public string Fieldlatitude { get; set; }
        public string Fieldlongitude { get; set; }
        public int? IsAudioVideoImage { get; set; }

        public int? FieldOrder { get; set; }
        public int? SortOrderNumber { get; set; }
        public string strFilePath { get; set; }
        public string strCloudUrl { get; set; }
        public bool isLaLongCaptureRequired { get; set; }
        public bool isTimeStampCaptureRequired { get; set; }
        public string captureTimeStamp { get; set; }
    }
    public class ReportPDFVM
    {
        public string ProjectName { get; set; }
        public string FormName { get; set; }
        public string InstallerName { get; set; }
        public string AuditorName { get; set; }
        public string VisitedDate { get; set; }
        public long? FormID { get; set; }
        public long? UploadID { get; set; }
        public string CategoryType { get; set; }
        public List<sectionfiledval> sectionfiledval { get; set; }
        public ReportSkipRTUModel skipRtuRecordModel { get; set; }
        public long ProjectId { get; set; }
        public string MeterAfterBeforeBoxDiffTime { get; set; }
        public string CustomerAddress { get; set; }
        public string UtilityType { get; set; }
    }


    public class ReportRow
    {
        public long UploadedID { get; set; }
        public long FormID { get; set; }
        public string CategoryType { get; set; }
    }


    public class ReportHeaderModel
    {
        public string FormName { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDate { get; set; }
        public string InstallerName { get; set; }
        public string AuditorName { get; set; }

    }

    public class AccountSummaryReportModel
    {
        public string Dates { get; set; }
        public string CategoryType { get; set; }
        public string MeterSize { get; set; }
        public string NewMeterName { get; set; }
        public string OldMeterRadioNumber { get; set; } //for Electric project AMP
        public string Cnfg { get; set; } //for Electric project Configration (Demand or Non-Demand)
        public string ToDate { get; set; }
    }

    public class AccountSummaryReportList
    {
        public int COMPLETED { get; set; }
        public int PENDING { get; set; }
        public int RTU { get; set; }
        public int SKIP { get; set; }
        public int NOT_ALLOCATED { get; set; }
        public List<SummaryDetail> SummaryDetail { get; set; }
    }

    public class SummaryDetail
    {
        public string METERSIZE { get; set; }
        public string AMP { get; set; }
        public int TOTALCOUNT { get; set; }
    }

    public class AccountSummaryReportJson
    {
        public string summaryType { get; set; }
        public AccountSummaryReportList records { get; set; }
    }
    public class AccountSummaryReport
    {
        public Dictionary<string, AccountSummaryReportList> dict { get; set; }
    }

    public class AccountHistoryDetailModel
    {
        public string AccountNo { get; set; }
        public string CustomerAddress { get; set; }
        public string Cycle { get; set; }
        public string Route { get; set; }
        public string InstallerName { get; set; }
        public string AssignedUser { get; set; }
        public string ActionDate { get; set; }
        public string Status { get; set; }
        public string SkippedReason { get; set; }
        public string SkipComment { get; set; }
    }
}
