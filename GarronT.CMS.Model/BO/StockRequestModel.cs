using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class StockRequestModel
    {
        public long RequestId { get; set; }
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string RequestIdWeb { get; set; }
        public long FKInventorySourceMasterId { get; set; }
        public long FKRequestedFromUserId { get; set; }
        public System.DateTime DeviceRequestDateTime { get; set; }

        public string DeviceDateTime { get; set; }

        public string DeviceTime { get; set; }
        public long FKInventoryRequestStatusId { get; set; }
        public Nullable<long> FKApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDateTime { get; set; }
        public string ApprovalComment { get; set; }
        public string DeviceGpsLocation { get; set; }
        public string DeviceIPAddress { get; set; }
        public string UserComment { get; set; }
        public string JsonRequestData { get; set; }
        public int Active { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string RequestFrom { get; set; }
     
        public string RequestTo { get; set; }
       
        public string StockRequestStatus { get; set; }

        
    }

    public class StockRequestDetailModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
    public class StockRequestDetailVM
    {
        public StockRequestModel StockRequestModel { get; set; }
        public List<StockRequestDetailModel> StockRequestDetailModelList { get; set; }
    }
}
