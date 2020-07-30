using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{

    /// <summary>
    /// 
    /// </summary>
    public class AuditBO : IDisposable
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

        //  A.Id,P.ProjectName, P.Utilitytype, A.AuditStartDate, A.AuditEndDate,
        //S.StateName, C.CityName,

        public long Id { get; set; }
        public string ProjectName { get; set; }
        public string Utilitytype { get; set; }
        public string AuditStartDate { get; set; }
        public string AuditEndDate { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string ClientName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AuditEntityBo : IDisposable
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


        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string AuditStartDate { get; set; }
        public string AuditEndDate { get; set; }
        public List<long> AuditorId { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class AuditorList
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AuditUserList
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long AuditorId { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class AuditDetailBO : IDisposable
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

        public List<AuditUserList> UserList { get; set; }
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string UtilityType { get; set; }
        public string AuditStartDate { get; set; }
        public string AuditEndDate { get; set; }
        public string ProjectStartDate { get; set; }
        public string ProjectEndDate { get; set; }

        public List<AuditorList> AuditorList { get; set; }


    }








    public class MobileOfflineAuditResult : IDisposable
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
        public bool batchIndicator { get; set; }
        public long batchNumber { get; set; }
        public int chunkSize { get; set; }
        public string checkSum { get; set; }
        public List<AuditMobile> data { get; set; }
        public int totalRecord { get; set; }
    }


    public class AuditMobile : IDisposable
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

        public int autoDataSyncInterval { get; set; }
        public int dataSyncConnection { get; set; }
        public List<AuditForms> objFormStructure { get; set; }
        public List<AuditMapVisits> objMapVisits { get; set; }
        public List<AuditMenuVisit> objMenuVisits { get; set; }

        public long processId { get; set; }
        public long projectId { get; set; }
        public string projectName { get; set; }
        public List<SkippReason> skipReasonList { get; set; }
        public bool synchBasicInfoOnly { get; set; }


    }

    public class AuditMapVisits : IDisposable
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

        public int colorFlag { get; set; }
        public int completedVisits { get; set; }
        public string groupId { get; set; }
        public bool isActive { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int pendingVisits { get; set; }
        public int skippedVisits { get; set; }
        public string street { get; set; }
        public int totalVisits { get; set; }

    }


    public class AuditMenuVisit : IDisposable
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

        public int colorFlag { get; set; }
        public int completedVisits { get; set; }
        public string groupId { get; set; }
        public bool isActive { get; set; }
        public List<AuditAccounts> objAccounts { get; set; }
        public int pendingVisits { get; set; }
        public int skippedVisits { get; set; }
        public string street { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int totalVisits { get; set; }
        public string visitDateTime { get; set; }
        public string visitId { get; set; }

    }


    public class AuditAccounts : IDisposable
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
        public int colorFlag { get; set; }
        public List<AccountMenuForms> formList { get; set; }
        public bool isActive { get; set; }
        public bool isReOpen { get; set; }
        public bool isVisited { get; set; }
        public string oldMeterNumber { get; set; }
        public string oldMeterType { get; set; }
        public string oldMeterSize { get; set; }
        public string oldMeterRadioNumber { get; set; }
        public long uploadId { get; set; }
        public string visitDateTime { get; set; }
        public string visitExpiryDateTime { get; set; }

    }


    public class AuditForms : IDisposable
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

        public bool syncFlag { get; set; }
        public List<AuditSection> sectionList { get; set; }

    }

    public class AuditSection : IDisposable
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
        public List<AuditFields> fieldList { get; set; }
        public long sectionId { get; set; }
        public string sectionName { get; set; }
        public int sectionOrder { get; set; }

    }


    public class AuditFields : IDisposable
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
        public List<AuditFieldInitialData> fieldInitialDataList { get; set; }
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

        public string videoQuality { get; set; }

    }


    public class AuditFieldInitialData : IDisposable
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
        public List<AuditPictureList> imageList { get; set; }
        public long initialFieldId { get; set; }
        public string initialFieldValue { get; set; }
        public bool isAdditionalService { get; set; }

    }


    public class AuditPictureList : IDisposable
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

    public class ProcessAccess : IDisposable
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

        public int processId { get; set; }
        public string processName { get; set; }
    }

    public class AccountMenuForms : IDisposable
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
        public List<AccountMenuFields> fieldList { get; set; }
        public long formId { get; set; }
        public string formName { get; set; }
        public bool isSkipped { get; set; }
        public bool isVisited { get; set; }
        public string skipReason { get; set; }
       // public string reasonType { get; set; }
        public string visitDateTime { get; set; }

    }


    public class AccountMenuFields : IDisposable
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
        public long fieldId { get; set; }
        public List<AccountFieldInitialData> fieldInitialDataList { get; set; }
        public string fieldName { get; set; }
        public long sectionId { get; set; }
    }


    public class AccountFieldInitialData : IDisposable
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
        public List<AuditPictureList> imageList { get; set; }
        public long initialFieldId { get; set; }
        public string initialFieldValue { get; set; }
        public bool isAdditionalService { get; set; }

    }






    public class ExportAuditDataComplete
    {
        public Nullable<long> ID { get; set; }
        public Nullable<long> FK_ProjectAuditData { get; set; }
        public Nullable<long> ProjectFieldDataMasterID { get; set; }
        public string InitialFieldName { get; set; }
        public string FieldValue { get; set; }
        public string FieldLatitude { get; set; }
        public string FieldLongitude { get; set; }
        public Nullable<System.DateTime> CapturedDateTime { get; set; }
        public string Cycle { get; set; }
        public string Route { get; set; }
        public Nullable<System.DateTime> visitdatetime { get; set; }
        public string Installer_Name { get; set; }

        public string Auditor_Name { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
    }
}
