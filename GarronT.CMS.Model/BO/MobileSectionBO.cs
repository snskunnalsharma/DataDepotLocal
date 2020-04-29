using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{

    public class MobileFormBO
    {
        public long formId { get; set; }
        public string formName { get; set; }
        public List<MobileSectionBO> SectionList { get; set; }
    }


    public class MobileSectionBO
    {
        public long sectionId { get; set; }
        public string sectionName { get; set; }
        public string sectionOrder { get; set; }
        public List<SectionFieldBO> FieldList { get; set; }
    }


    public class SectionFieldBO
    {
        public long sectionId { get; set; }
        public long fieldId { get; set; }
        public string fieldName { get; set; }
        public int fieldOrder { get; set; }
        public bool hideFieldLabel { get; set; }
        public string fieldDataTypeName { get; set; }
        public List<FieldInitialData> fieldInitialDataList { get; set; }

        public string fieldLabel { get; set; }
        public string defaultValue { get; set; }
        public string decimalPositions { get; set; }
        public string hintText { get; set; }
        public bool enforceMinMax { get; set; }

        public int maxLengthValue { get; set; }
        public int minLengthValue { get; set; }

        public bool isRequired { get; set; }
        public bool isEnabled { get; set; }
        public bool captureGeolocation { get; set; }
        public bool captureTimestamp { get; set; }
        public bool hasAlert { get; set; }
        public bool secure { get; set; }
        public bool allowprii { get; set; }
        public bool keyboardType { get; set; }

        public string formatMask { get; set; }
        public string displayMask { get; set; }
        public string captureMode { get; set; }

        public bool photoFile { get; set; }
        public bool allowAnnotation { get; set; }
        public string photoQuality { get; set; }


        public string maximumHeight { get; set; }
        public string maximumWidth { get; set; }
        public bool excludeonSync { get; set; }


        public bool photoLibraryEnabled { get; set; }
        public bool cameraEnabled { get; set; }
        public bool gpsTagging { get; set; }

        public bool allowNotApplicable { get; set; }
        public bool ratingMax { get; set; }
        public bool ratingLabels { get; set; }

        public bool showMultiselect { get; set; }
        public bool showRatingLabels { get; set; }
        public bool allowMultiSelection { get; set; }

        public string numberOfColumnsForPhones { get; set; }
        public string numberOfColumnsForTablets { get; set; }
        public bool hasValues { get; set; }

        public string fieldFilterkey { get; set; }
        public string acknowlegementText { get; set; }
        public string acknowlegementButtonText { get; set; }
        public string acknowlegementClickedButtonText { get; set; }
        public bool hasFile { get; set; }
        public string videoQuality { get; set; }
        public string maxrecordabletimeInSeconds { get; set; }
        public string audioQuality { get; set; }

    }


    public class FieldInitialData
    {
        public long initialFieldId { get; set; }
        public string initialFieldValue { get; set; }
        public Nullable<bool> isAdditionalService { get; set; }
        public bool initialFieldActive { get; set; }
        public bool isVisitedForForm { get; set; }
        public bool isPreviousSkipped { get; set; }
        public serviceInitialData objserviceInitialData { get; set; }
    }

    public class serviceInitialData
    {
        public bool flagCaptureAudio { get; set; }
        public bool flagCaptureVideo { get; set; }
        public List<string> imageList { get; set; }
    }


    public class SectioFieldDataBO
    {

        public long fieldId { get; set; }
        public string fieldValue { get; set; }
        public string fieldGeolocationValue { get; set; }
        public string fieldTimestampValue { get; set; }
        public List<byte[]> photoFile { get; set; }
        public List<byte[]> videoFile { get; set; }
        public List<byte[]> audioFile { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class ProjectFormBO
    {
        public long projectId { get; set; }
        public string projectName { get; set; }
        public List<FormSectionBO> FormNameList { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class FormSectionBO
    {
        public long formId { get; set; }
        public string formName { get; set; }
        public List<MobileSectionBO> SectionList { get; set; }
    }


    public class FormData
    {
        public string userId { get; set; }
        public long projectId { get; set; }
        public long formId { get; set; }
        public AccountData currentAccountData { get; set; }
        public string visitDateTime { get; set; }

    }

    public class AccountData
    {
        public List<fieldData> accountFieldDataList { get; set; }
        
        public string accountNumber { get; set; }
        public long initialFieldId { get; set; }
        public bool isAccountSkipped { get; set; }
        public string skipeReason { get; set; }
        public string skipGPSLocation { get; set; }
        public string skipeCapturedDateTime { get; set; }
        public List<byte[]> skipedReasonPhotoProof { get; set; }
        public Nullable<long> skipId { get; set; }
        public string skipComment { get; set; }

        public List<ServiceDataSubmit> inventoryDetails { get; set; }

    }


    public class fieldData
    {
        public long fieldId { get; set; }
        public string fieldName { get; set; }
        public string capturedDateTime { get; set; }
        public string fieldValue { get; set; }
        public string geolocationLatitudeValue { get; set; }
        public string geolocationLongitudeValue { get; set; }
        public string capturedTimeStamp { get; set; }
        public string commentForServiceField { get; set; }
        public string statusForServiceField { get; set; }
        public List<byte[]> capturedPhotos { get; set; }
        public List<byte[]> capturedAudios { get; set; }
        public List<byte[]> capturedVideos { get; set; }
    }




    public class FormDataNew
    {

        public AccountData currentAccountData { get; set; }
        public long formId { get; set; }
        public int isMediaPending { get; set; }
        public long projectId { get; set; }
        public string userId { get; set; }
        public string visitDateTime { get; set; }
       
    }


    public class CaptureGeoLocation
    {

        public long Id { get; set; }
        public string FieldName { get; set; }
        public string strImagePath { get; set; }
        public string strLatLong { get; set; }

    }
}
