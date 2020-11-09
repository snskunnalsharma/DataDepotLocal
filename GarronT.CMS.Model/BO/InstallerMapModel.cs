using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class InstallerMapModel
    {
        public long ID { get; set; }
        public long InstallerId { get; set; }
        public long tblUploadedData_ID { get; set; }
        public System.DateTime Date { get; set; }
        public int Active { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<long> ProjectID { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class CycleMaster
    {
        public string CycleId { get; set; }
        public bool CycleDisabled { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class RouteMaster
    {
        public string RouteId { get; set; }
        public bool RouteDisabled { get; set; }
    }


    public class AddressMaster
    {
        public long ID { get; set; }
        public string Street { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool StreetDisabled { get; set; }
        public string Cycle { get; set; }
        public string Route { get; set; }

        public string Appointmentflag { get; set; }
        public string MeterSize { get; set; }
    }



    public class CurrentVisitRecord
    {
        public long ID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long InstallerId { get; set; }
        public long FK_UploadedExcelData_Id { get; set; }
        public string Account { get; set; }
        public string Customer { get; set; }
        public string OldMeterNo { get; set; }
        public string OldMeterRadioNo { get; set; }
    }


    public class CurrentVisitRecordRow
    {
        public long ID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long InstallerId { get; set; }
        public long FK_UploadedExcelData_Id { get; set; }
        public string Account { get; set; }
        public string Customer { get; set; }
        public string OldMeterNo { get; set; }
        public string OldMeterRadioNo { get; set; }
        public string Cycle { get; set; }
        public string Route { get; set; }
        public string Street { get; set; }

        // Added by sominath 
        public string City { get; set; }
        public string State { get; set; }

        // Added by Deepak 02 Nov 2020
        public string OldMeterSize { get; set; }
    }
}
