using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{



    public class InventorySourceBO
    {
        public long Id { get; set; }
        public string InventorySourceName { get; set; }
        public List<InventorySourceChildBO> ObjInventorySourceChildBOList { get; set; }
    }

    public class InventorySourceChildBO
    {
        public long ChildId { get; set; }
        public string ChildName { get; set; }
    }



    public class InventoryNewRequestSourceBO
    {
        public List<InventorySourceBO> ObjInventorySourceBOList { get; set; }

        public List<InventoryUtilityTypeBO> ObjInventoryUtilityTypeBOList { get; set; }

        public List<InventoryRequestNameBO> ObjInventoryRequestNameBOList { get; set; }
    }


    public class InventoryUtilityTypeBO
    {
        public long Id { get; set; }
        public string UtilityName { get; set; }
    }

    public class InventoryRequestNameBO
    {
        public string InventoryName { get; set; }
    }


    public class MobileRecieveInventoryBO
    {
        List<InventorySourceBO> objBOList { get; set; }

    }


    public class MobileStockRequest
    {
        public long UserId { get; set; }
        public long ProjectId { get; set; }
        public long SourceId { get; set; }
        public long SubSourceId { get; set; }
        public double DeviceRequestDateTime { get; set; }
        public string DeviceIPAddress { get; set; }
        public string DeviceGpsLocation { get; set; }
        public string UserComment { get; set; }
        public List<RequestedStock> ProductRequestList { get; set; }
    }

    public class RequestedStock
    {
        public long ProductId { get; set; }
        public int Qty { get; set; }
    }


    public class MobileViewRequestBo
    {

        public long ProjectId { get; set; }
        public long UserId { get; set; }
        public long Id { get; set; }
        public long ChildId { get; set; }
        public string FromDate { get; set; }
        public string Todate { get; set; }

    }


    public class MobileViewResponseBo
    {
        public long Id { get; set; }
        public string RequestUId { get; set; }
        public string RequestedBy { get; set; }
        public string RequestedTo { get; set; }
        public string RequestStatus { get; set; }
        public string RequestDate { get; set; }
        public string RequestTime { get; set; }
    }


    public class MobileStockRequestDetails
    {
        public string ProjectName { get; set; }
        public long Id { get; set; }
        public string RequestUId { get; set; }
        public string RequestedBy { get; set; }
        public string RequestedTo { get; set; }
        public string RequestStatus { get; set; }
        public string DeviceRequestDate { get; set; }
        public string DeviceRequestTime { get; set; }
        public string Comment { get; set; }
        public List<RequestedStockDetails> ProductRequestList { get; set; }

    }


    public class RequestedStockDetails
    {
        public string ProductName { get; set; }
        public int Qty { get; set; }
    }

    public class MobileUpdateStockRequestBO
    {
        public long userId { get; set; }
        public long id { get; set; }
        public string status { get; set; }
        public double approvedDatetime { get; set; }
        public string comment { get; set; }
    }




    public class MobileRunningInventoryBO
    {
        public long userId { get; set; }
        public int allocatedInventory { get; set; }
        public int installedInventory { get; set; }
        public int transferredInventory { get; set; }
        public int returnedInventory { get; set; }
        //public int balanceInventory { get; set; }

    }



    public class MobileUserSignOffBO
    {
        public long userId { get; set; }
        public double DeviceRequestDateTime { get; set; }
        public string DeviceIPAddress { get; set; }
        public string DeviceGpsLocation { get; set; }

        public int allocatedInventory { get; set; }
        public int installedInventory { get; set; }
        public int transferredInventory { get; set; }
        public int returnedInventory { get; set; }
        public int balanceInventory { get; set; }


        public List<UserProjectStockDetails> InventoryList { get; set; }


    }




    public class UserProjectStockDetails
    {
        public List<string> SerialNumberList { get; set; }
        public List<UserStockDetails> ProductQuantityList { get; set; }
    }


    public class UserStockDetails
    {
        public long ProductId { get; set; }
        public int Qty { get; set; }
    }




    public class MobileReturnStockBO
    {
        public long UserId { get; set; }
        public long ProjectId { get; set; }
        public double DeviceRequestDateTime { get; set; }
        public string DeviceIPAddress { get; set; }
        public string DeviceGpsLocation { get; set; }

        public long Id { get; set; }
        public long ChildId { get; set; }



        public List<MobileReturnStockSerialChildBO> SerialNumberList { get; set; }

        public List<MobileReturnStockNonSerialChildBO> ProductQuantityList { get; set; }

        public List<MobileReturnStockOldChildBO> OldProductQuantityList { get; set; }

        public string RequestDateTime { get; set; }
        public string ReturnedTo { get; set; }
        public long? ReturnRequestId { get; set; }


    }


    public class MobileReturnStockSerialChildBO
    {
        public string SerialNumber { get; set; }
        public int? TotalQty { get; set; }
        public string ServerReponseStatus { get; set; }
    }

    public class MobileReturnStockNonSerialChildBO
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalQty { get; set; }
        public string ServerReponseStatus { get; set; }
    }

    public class MobileReturnStockOldChildBO
    {
        public long? ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalQty { get; set; }
        public string ServerReponseStatus { get; set; }
    }





}
