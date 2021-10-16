using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class ProjectModel : IDisposable
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

        public long ProjectId { get; set; }
        public long CloneId { get; set; }
        public bool IsCloneProject { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCityState { get; set; }
        public string ProjectDescription { get; set; }
        public long TotalRecords { get; set; }
        public string LatLongStatus { get; set; }
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public long StateId { get; set; }
        public string StateName { get; set; }
        public long CityId { get; set; }
        public string CityName { get; set; }
        public Nullable<long> ContactId { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }

        public string stringFromDate { get; set; }
        public string stringToDate { get; set; }

        public string Utilitytype { get; set; }


        public int Active { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }

        public string ProjectStatus { get; set; }
        public int ConnectionType { get; set; }
        public int dataSynch { get; set; }
        public int SyncType { get; set; }
        public string ProjectAppointmentUrl { get; set; }

        public string ManageInventory { get; set; }


        public List<StateModel> objStateList { get; set; }
        public List<CityModel> objCityList { get; set; }

        public List<ClientModel> objClientModelList { get; set; }

        public List<ContactModelDDL> objContactModelDDLList { get; set; }

        public List<MobileWarehouseBO> objMobileWarehouseBOList { get; set; }

        public long[] objWarehouseArray { get; set; }


        public List<MeterMakeModel> objMeterMakeModelList { get; set; }
        public List<MeterSizeModel> objMeterSizeList { get; set; }
        public List<MeterTypeModel> objMeterTypeList { get; set; }
        public List<ServicesModel> objServiceList { get; set; }
        public List<UsersSmallModel> objUsersSmallModelList { get; set; }
        public List<string> CycleList { get; set; }


        public long[] objMakeArray { get; set; }
        public long[] objSizeArray { get; set; }
        public long[] objTypeArray { get; set; }
        public long[] objServiceArray { get; set; }
        public long[] objUsersArray { get; set; }

    }

    

    public class ProjectEditModel
    {
        public long ID { get; set; }
        public string Cycle { get; set; }
        public string Route { get; set; }
        public string Street { get; set; }
        public string Account { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CreatedOn { get; set; }
        public string OldMeterType { get; set; }
        public string OldMeterMake { get; set; }
        public string OldMeterNumber { get; set; }
        public string OldMeterSize { get; set; }
        public string OldMeterRadioNumber { get; set; }
        public string Customer { get; set; }
    }

    public class ProjectSmallModel
    {
        public string CustomerName { get; set; }
        public string stringFromDate { get; set; }
        public string stringToDate { get; set; }
        public string ProjectName { get; set; }
        public string Utility { get; set; }
        public long ProjectId { get; set; }
        public List<UsersSmallModel> objPresentList { get; set; }
        public List<long> objLongList { get; set; }

        public List<AuditCycleBO> objList { get; set; }
        public List<AuditProjectFailBO> objAuditProjectFailBOList { get; set; }
    }


    public class ProjectMobileBO
    {
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
    }



    public class ReportHomePageImages
    {
        public string FieldName { get; set; }
        // public bool isSkipped { get; set; }
        public bool isServiceData { get; set; }
        public string ServiceName { get; set; }
        public string strFilePath { get; set; }
    }
}


namespace GarronT.CMS.Model
{
    using System;
    using System.Collections.Generic;

    public partial class tblUploadedData
    {
        public string Street_route { get; set; }
        public string street_number { get; set; }
        public string status { get; set; }
        public string FirstName { get; set; }

    }
}