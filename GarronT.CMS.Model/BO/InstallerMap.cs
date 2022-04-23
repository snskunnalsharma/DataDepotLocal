using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class InstallerMap
    {
        public List<string> AddressID { get; set; }
        public List<string> CycleIList { get; set; }
        public List<string> RouteList { get; set; }

        public long cycleID { get; set; }
        public long RouteID { get; set; }
        public long InstallerID { get; set; }
        public long ProjectID { get; set; }
        public long FKAppointmentID { get; set; }
        public Nullable<int> AppFlag { get; set; }
        public long FK_UploadedExcelData_Id { get; set; }
        //  public string Date { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public string OldFromDate { get; set; }
        public string OldToDate { get; set; }

        public List<LatLong> LatLongList { get; set; }
        public List<string> RouteValues { get; set; }
        public List<string> CycleValues { get; set; }
    }

    public class LatLong
    {
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
        public string Street { get; set; }
    }

    public class projectStatus
    {
        public long UploadID { get; set; }
        public string ClientName { get; set; }
        public string ProjectName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string InstallerName { get; set; }
        public string Cycle { get; set; }
        public string Route { get; set; }
        public string City { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Street { get; set; }
        public string Status { get; set; }
    }


    /// <summary>
    /// Added by Aniket on 25-Aug-2016
    /// </summary>
    public class AddressInstallerMap
    {
        public List<long> AddressList { get; set; }
        public long oldInstallerID { get; set; }
        public long newInstallerID { get; set; }
        public long ProjectID { get; set; }
        public List<long> CycleList { get; set; }
        public List<long> RouteList { get; set; }

    }


    /// <summary>
    /// 
    /// </summary>
    public class InstallerDataStatus
    {
        public long UploadID { get; set; }
        public string CustomerName { get; set; }
        public string ProjectName { get; set; }
        public string City { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string InstallerName { get; set; }
        public string Cycle { get; set; }
        public string Route { get; set; }
        public string Street { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string AccountNumber { get; set; }
        public string OldMeterNumber { get; set; }
        public string OldRadioNumber { get; set; }
        public string VisitedDate { get; set; }
        public string VisitStatus { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
    }



}
