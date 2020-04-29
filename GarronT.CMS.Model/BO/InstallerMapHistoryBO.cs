using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
   /*
   * -------------------------------------------------------------------------------
   * Create By     : Aniket Jadhav 
   * Created On    : 07-Sep-2016 
   * Description   : BO for Installer Map Data History
   *--------------------------------------------------------------------------------
   */
    public class InstallerMapHistoryBO
    {
        public long ID { get; set; }
        public long InstallerId { get; set; }
        public long tblUploadedData_ID { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> SynchWithMobileFlag { get; set; }
        public Nullable<System.DateTime> SynchWithMobileDate { get; set; }
        public Nullable<int> MobileResponse { get; set; }
        public Nullable<System.DateTime> MobileResponseDate { get; set; }
        public string SynchCheckSumString { get; set; }
        public Nullable<long> BatchNumber { get; set; }
        public int Active { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<long> ProjectID { get; set; }
        public Nullable<long> SkipReasonMasterId { get; set; }
        public string SkipComment { get; set; }
    }

    public class InstallerMapHistoryVM
    {
        public long ProjectFieldDataId { get; set; }
        public long FormId { get; set; }
        public string FormName { get; set; }
        public string InstallerName { get; set; }
        public long InstallerId { get; set; }
        public string projectname { get; set; }
        public Nullable<bool> IsSkipped { get; set; }
        public string SkippedReason { get; set; }
        public string SkipComment { get; set; }
        public long ID { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public Nullable<long> StateId { get; set; }
        public Nullable<long> CityId { get; set; }
        public string Cycle { get; set; }
        public string Route { get; set; }
        public string Account { get; set; }
        public string Customer { get; set; }
        public string Street { get; set; }
        public string OldMeterNo { get; set; }
        public string OldMeterSize { get; set; }
        public string OldMeterType { get; set; }
        public string OldMeterRadioNo { get; set; }
        public string OldMeterLastReading { get; set; }
        public string OldMeterFinalReading { get; set; }
        public string NewMeterNo { get; set; }
        public string NewMeterResgisterNo { get; set; }
        public string NewMeterRadio { get; set; }
        public string NewMeterSize { get; set; }
        public string NewMeterType { get; set; }
        public string NewMeterReading { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string MeterInstallNotation { get; set; }
        public string MeterLatitude { get; set; }
        public string MeterLongitude { get; set; }
        public string AddressLatitude { get; set; }
        public string AddressLongitude { get; set; }
        public Nullable<bool> IsVisisted { get; set; }
        public Nullable<System.DateTime> VisitedOn { get; set; }
        public string VisitId { get; set; }
        public string OldVisitId { get; set; }
        public string GroupId { get; set; }
        public string OldGroupId { get; set; }
        public int Active { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<long> VisitedBy { get; set; }
        public string VisitedDate { get; set; }
    }


    public class SkippedPDFVM
    {
         public long ProjectFieldDataId { get; set; }
         public long ID { get; set; }
        public string ProjectName { get; set; }
        public string FormName { get; set; }
        public string InstallerName { get; set; }
        public string VisitedDate { get; set; }
        public long FormId { get; set; }
        public InstallerMapHistoryVM skipRecordModel { get; set; }
    }


    //Createdby AniketJ on 16-Sep-2016
    public class InstallerAddressDetails
    {
        public long id { get; set; }
        public string ProjectName { get; set; }
        public string Utilitytype { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public string firstname { get; set; }
        public string Email { get; set; }
        public string cycle { get; set; }
        public string Route { get; set; }
        public string Account { get; set; }
        public string Customer { get; set; }
        public string OldMeterNumber { get; set; }
        public string OldMeterRadioNumber { get; set; }
        public string Street { get; set; }
    }
}
