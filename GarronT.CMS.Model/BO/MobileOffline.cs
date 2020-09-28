using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class MobileOffline
    {
        public long projectId { get; set; }
        public string projectName { get; set; }
        public int AutoDataSyncInterval { get; set; }
        public int DataSyncConnection { get; set; }

        public List<MapVisits> ObjMapVisits { get; set; }
        public List<MenuVisits> ObjMenuVisits { get; set; }

    }

    public class MapVisits
    {
        public int colorFlag { get; set; }
        public int completedVisits { get; set; }
        public string groupId { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int pendingVisits { get; set; }
        public int skippedVisits { get; set; }
        public string street { get; set; }
        public int totalVisits { get; set; }
        public bool isActive { get; set; }
    }

    public class MenuVisits
    {
        public string groupId { get; set; }
        public string visitId { get; set; }
        public string street { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int colorFlag { get; set; }
        public int totalVisits { get; set; }
        public int pendingVisits { get; set; }
        public int skippedVisits { get; set; }
        public int completedVisits { get; set; }

        public List<Accounts> objAccounts { get; set; }
    }

    public class Accounts
    {

        public long uploadId { get; set; }
        public string accountNumber { get; set; }
        public string oldMeterNumber { get; set; }
        public string oldMeterType { get; set; }
        public string oldMeterSize { get; set; }
        public string oldMeterRadioNumber { get; set; }
        public bool isVisited { get; set; }
        public string visitDateTime { get; set; }
        public List<Forms> FormList { get; set; }
    }

    public class Forms
    {
        public long formId { get; set; }
        public string formName { get; set; }
        public bool isVisited { get; set; }
        public bool isSkipped { get; set; }
        public string visitDateTime { get; set; }
        public List<Section> sectionList { get; set; }
    }

    public class Section : IDisposable
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
        public List<Fields> fieldList { get; set; }
        public long sectionId { get; set; }
        public string sectionName { get; set; }
        public int sectionOrder { get; set; }

    }

    public class Fields : IDisposable
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

        public string acknowlegementButtonText { get; set; }
        public string acknowlegementClickedButtonText { get; set; }
        public string acknowlegementText { get; set; }
        public bool allowAnnotation { get; set; }
        public bool allowMultiSelection { get; set; }
        public bool allowNotApplicable { get; set; }
        public bool allowprii { get; set; }
        public string audioQuality { get; set; }

        public bool cameraEnabled { get; set; }
        public bool captureGeolocation { get; set; }
        public string captureMode { get; set; }
        public bool captureTimestamp { get; set; }

        public string decimalPositions { get; set; }
        public string defaultValue { get; set; }
        public string displayMask { get; set; }

        public bool enforceMinMax { get; set; }
        public bool excludeonSync { get; set; }

        public string fieldDataTypeName { get; set; }
        public string fieldFilterkey { get; set; }
        public long fieldId { get; set; }
        public List<FieldInitialDataO> fieldInitialDataList { get; set; }
        public string fieldLabel { get; set; }
        public string formatMask { get; set; }
        public string fieldName { get; set; }
        public int fieldOrder { get; set; }


        public bool gpsTagging { get; set; }


        public bool hasAlert { get; set; }
        public bool hasFile { get; set; }
        public bool hasValues { get; set; }
        public bool hideFieldLabel { get; set; }
        public string hintText { get; set; }


        public bool isEnabled { get; set; }
        public bool isRequired { get; set; }


        public bool keyboardType { get; set; }

        public int maximumHeight { get; set; }
        public int maximumWidth { get; set; }
        public int maxLengthValue { get; set; }
        public int maxrecordabletimeInSeconds { get; set; }
        public int minLengthValue { get; set; }



        public int numberOfColumnsForPhones { get; set; }
        public int numberOfColumnsForTablets { get; set; }

        public bool photoFile { get; set; }
        public bool photoLibraryEnabled { get; set; }
        public string photoQuality { get; set; }

        public bool ratingLabels { get; set; }
        public bool ratingMax { get; set; }


        public long sectionId { get; set; }
        public bool secure { get; set; }
        public bool showMultiselect { get; set; }
        public bool showRatingLabels { get; set; }
       
        public bool trackInInventory { get; set; }
        public string videoQuality { get; set; }

        
    }

    public class FieldInitialDataO : IDisposable
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
        public bool flagCaptureAudio { get; set; }
        public bool flagCaptureVideo { get; set; }
        public bool flagIsImageCaptureRequired { get; set; }
        public List<PictureList> imageList { get; set; }
        public long initialFieldId { get; set; }
        public string initialFieldValue { get; set; }
        public bool isAdditionalService { get; set; }

    }

    public class PictureList : IDisposable
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
        public long imageId { get; set; }
        public string imageName { get; set; }
    }

    public class MobileOfflineResult
    {

        public bool batchIndicator { get; set; }
        public long batchNumber { get; set; }
        public int chunkSize { get; set; }
        public string checkSum { get; set; }
        public List<MobileOffline1> data { get; set; }

    }

    public class MobileOffline1
    {
        public int autoDataSyncInterval { get; set; }
        public int dataSyncConnection { get; set; }
        public List<Forms1> objFormStructure { get; set; }
        public List<MapVisits> objMapVisits { get; set; }
        public List<MenuVisits1> objMenuVisits { get; set; }
        public long projectId { get; set; }
        public string projectName { get; set; }
        public List<SkippReason> skipReasonList { get; set; }
    }

    public class NewMobileOfflineResult
    {

        public bool batchIndicator { get; set; }
        public long batchNumber { get; set; }
        public int chunkSize { get; set; }
        public string checkSum { get; set; }
        public List<newMobileOffline> data { get; set; }
        // public int totalRecord { get; set; }

    }

    public class newMobileOffline
    {
        public int autoDataSyncInterval { get; set; }
        public int dataSyncConnection { get; set; }
        public List<Forms1> objFormStructure { get; set; }
        public List<MapVisits> objMapVisits { get; set; }
        public List<MenuVisits1> objMenuVisits { get; set; }
        public long projectId { get; set; }
        public string projectName { get; set; }
        public List<RTUReason> rtuReasonList { get; set; }
        public List<SkippReason> skipReasonList { get; set; }

    }


    public class MobileOfflineResult1
    {
        public bool batchIndicator { get; set; }
        public long batchNumber { get; set; }
        public int chunkSize { get; set; }
        public string checkSum { get; set; }
        public List<newMobileOffline1> data { get; set; }
        public int totalRecord { get; set; }
    }



    public class newMobileOffline1
    {
        public int autoDataSyncInterval { get; set; }
        public int dataSyncConnection { get; set; }
        public List<Forms1> objFormStructure { get; set; }
        public List<MapVisits> objMapVisits { get; set; }
        public List<MenuVisits2> objMenuVisits { get; set; }
        public long projectId { get; set; }
        public string projectName { get; set; }
        public List<RTUReason> rtuReasonList { get; set; }
        public List<SkippReason> skipReasonList { get; set; }

    }

    public class MenuVisits2
    {
        public int colorFlag { get; set; }
        public int completedVisits { get; set; }
        public string groupId { get; set; }
        public List<Accounts2> objAccounts { get; set; }
        public int pendingVisits { get; set; }
        public int skippedVisits { get; set; }
        public string street { get; set; }
        public int totalVisits { get; set; }
        public string visitDateTime { get; set; }
        public string visitId { get; set; }
        public bool isActive { get; set; }
    }




    public class Forms1 : IDisposable
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
        public long formId { get; set; }
        public string formName { get; set; }
        public List<Section> sectionList { get; set; }
        public bool syncFlag { get; set; }
    }

    public class SectionData
    {
        public long FormId { get; set; }
        public string FormName { get; set; }
        public long SectionId { get; set; }
        public string SectionName { get; set; }
        public int SortOrderNumber { get; set; }

    }

    public class MenuVisits1
    {
        public int colorFlag { get; set; }
        public int completedVisits { get; set; }
        public string groupId { get; set; }
        public List<Accounts1> objAccounts { get; set; }
        public int pendingVisits { get; set; }
        public int skippedVisits { get; set; }
        public string street { get; set; }
        public int totalVisits { get; set; }
        public string visitDateTime { get; set; }
        public string visitId { get; set; }
        public bool isActive { get; set; }
    }

    public class Accounts1
    {

        public string accountNumber { get; set; }
        public int colorFlag { get; set; }
        public List<Forms2> formList { get; set; }
        public bool isActive { get; set; }
        public bool isReOpen { get; set; }
        public bool isVisited { get; set; }
        public string oldMeterNumber { get; set; }
        public string oldMeterType { get; set; }
        public string oldMeterSize { get; set; }
        //public string oldMeterRadioNumber { get; set; }
        public long uploadId { get; set; }
        public string visitDateTime { get; set; }

    }

    public class Accounts2
    {

        public string accountNumber { get; set; }
        public int colorFlag { get; set; }
        public List<Forms3> formList { get; set; }
        public bool isActive { get; set; }
        public bool isReOpen { get; set; }
        public bool isVisited { get; set; }
        public string oldMeterNumber { get; set; }
        public string oldMeterType { get; set; }
        public string oldMeterSize { get; set; }
        public string oldMeterRadioNumber { get; set; }
        public long uploadId { get; set; }
        public string visitDateTime { get; set; }

    }

    public class Forms2
    {
        public List<Fields1> fieldList { get; set; }
        public long formId { get; set; }
        public string formName { get; set; }
        public bool isSkipped { get; set; }
        public bool isVisited { get; set; }
        //  public string skipReason { get; set; }
        // public string reasonType { get; set; }
        public string visitDateTime { get; set; }

    }


    public class Forms3
    {
        public List<Fields1> fieldList { get; set; }
        public long formId { get; set; }
        public string formName { get; set; }
        public bool isSkipped { get; set; }
        public bool isVisited { get; set; }
        public string skipReason { get; set; }
        public string reasonType { get; set; }
        public string visitDateTime { get; set; }

    }
    public class Fields1
    {
        public long fieldId { get; set; }
        public List<FieldInitialDataO> fieldInitialDataList { get; set; }
        public string fieldName { get; set; }
        public long sectionId { get; set; }
    }

    public class FieldInitialDataO1
    {
        public long initialFieldId { get; set; }
        public string initialFieldValue { get; set; }
    }

    public class SkippReason
    {
        public long skipId { get; set; }
        public string skipReason { get; set; }
        public bool isImageRequired { get; set; }
        public bool isAudioRequired { get; set; }
        public bool isVideoRequired { get; set; }
    }
    public class RTUReason
    {
        public long rtuId { get; set; }
        public string rtuReason { get; set; }
        public bool isImageRequired { get; set; }
        public bool isAudioRequired { get; set; }
        public bool isVideoRequired { get; set; }
    }


    public class ServiceData
    {
        public bool IsAdditionalService { get; set; }
        public List<ProductData> productList { get; set; }
        public long serviceId { get; set; }
    }



    public class ProductData
    {
        public int productAlertQty { get; set; }
        public long productId { get; set; }
        public string productName { get; set; }
        public int productQty { get; set; }
        public bool serialNumberRequired { get; set; }
    }

    public class ServiceDataSubmit
    {
        public List<ProductDataSubmit> produtDetails { get; set; }
        public long serviceId { get; set; }
    }

    public class ProductDataSubmit
    {
        public long productId { get; set; }
        public string serialNumber { get; set; }
        public int qty { get; set; }
    }

    #region OfflineData class

    public class MobileOfflineResult2
    {
        public string appointmentURL { get; set; }
        public bool batchIndicator { get; set; }
        public long batchNumber { get; set; }
        public int chunkSize { get; set; }
        public string checkSum { get; set; }
        public List<newMobileOffline2> data { get; set; }
        public int totalRecord { get; set; }
        public List<LastCompleteLocation> projectLastLocation { get; set; }
        public TimeSpan totalTime { get; set; }
    }
     
    public class LastCompleteLocation
    {
        public long projectId { get; set; }
        public string lastCompleteLat { get; set; }
        public string lastCompleteLong { get; set; }
    }

    public class LocationCycleRoute
    {
        public string cycle { get; set; }
        public string route { get; set; }
    }

    public class newMobileOffline2
    {
        public string appointmentDates { get; set; }
        public int autoDataSyncInterval { get; set; }
        public int dataSyncConnection { get; set; }

        public List<Forms1> objFormStructure { get; set; }
        public List<MapVisits1> objMapVisits { get; set; }
        public List<MenuVisits3> objMenuVisits { get; set; }
        public long processId { get; set; }
        public long projectId { get; set; }
        public string projectName { get; set; }
        public string lastCompleteLat { get; set; }
        public string lastCompleteLong { get; set; }
        public List<RTUReason> rtuReasonList { get; set; }

        public List<ServiceData> servicesProductList { get; set; }
        public List<SkippReason> skipReasonList { get; set; }


        public bool synchBasicInfoOnly { get; set; }
        public bool trackInventoryForProject { get; set; }
    }

    public class MenuVisits3
    {

        public int colorFlag { get; set; }
        public int completedVisits { get; set; }
        public string groupId { get; set; }
        public bool hasAppointment { get; set; }
        public bool isActive { get; set; }
        public List<Accounts3> objAccounts { get; set; }
        public int pendingVisits { get; set; }
        public int rtuVisits { get; set; }
        public int skippedVisits { get; set; }
        public string street { get; set; }
        public int totalVisits { get; set; }
        public string visitDateTime { get; set; }
        public string visitId { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }

    }


    public class MapVisits1 : IDisposable
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
        public List<AppointmentDetails> appointmentList { get; set; }
        public int colorFlag { get; set; }
        public int completedVisits { get; set; }
        public string groupId { get; set; }
        public bool hasAppointment { get; set; }
        public bool isActive { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int pendingVisits { get; set; }
        public int rtuVisits { get; set; }
        public int skippedVisits { get; set; }
        public string street { get; set; }
        public int totalVisits { get; set; }
        public string UpdatedDate { get; set; }
        public string cycle { get; set; }
        public string route { get; set; }

    }

    public class AppointmentDetails
    {
        public string accountNumber { get; set; }
        public string appointmentContactName { get; set; }
        public string appointmentContactNumber { get; set; }
        public string appointmentDate { get; set; }
        public string appointmentTime { get; set; }
        public long id { get; set; }
    }



    public class Accounts3 : IDisposable
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


        public string accountNumber { get; set; }

        public string appointmentContactName { get; set; }
        public string appointmentContactNumber { get; set; }
        public string appointmentDate { get; set; }
        public double appointmentEpochTime { get; set; }
        public string appointmentTime { get; set; }


        public int colorFlag { get; set; }

        public List<Forms3> formList { get; set; }
        public bool hasAppointment { get; set; }
        public long id { get; set; }
        public bool isActive { get; set; }
        public bool isReOpen { get; set; }
        public bool isVisited { get; set; }
        public double notificationFireDate { get; set; }
        public string oldMeterNumber { get; set; }
        public string oldMeterType { get; set; }
        public string oldMeterSize { get; set; }
        public string oldMeterRadioNumber { get; set; }
        public long uploadId { get; set; }
        public string visitDateTime { get; set; }
        public string visitExpiryDateTime { get; set; }





    }
    #endregion
}
