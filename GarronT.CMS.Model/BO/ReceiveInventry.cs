using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class ReceiveInvoiceSavedRecords
    {
        public long HeaderId { get; set; }
        public long ParentId { get; set; }
        public Nullable<long> ChildParentId { get; set; }
        public Nullable<int> GenerateBarcode { get; set; }
        public string SystemGenBarcode { get; set; }
        public string BarCodeFilePath { get; set; }
        public Nullable<int> IsSerialNumber { get; set; }
        public string SerialNumber { get; set; }
        public string Location { get; set; }
        public int UploadQty { get; set; }
        public Nullable<long> FKWarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public Nullable<long> FKProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string PartNumber { get; set; }
        public string UPCCode { get; set; }
        public string UtilityName { get; set; }
        public string CategoryName { get; set; }
        public string Make { get; set; }
        public string MeterSize { get; set; }
        public string MeterType { get; set; }
    }

    public class ViewUploadInventryVM
    {
        public List<ReceiveInvoiceSavedRecords> ReceiveInvoiceSavedRecordList { get; set; }
        public long RequestId { get; set; }
        public string uploadType { get; set; }
        public DateTime? DeviceDate { get; set; }
        public string DeviceDateTime { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
       
        public string ProjectName { get; set; }
        public string WarehouseName { get; set; }
        public long? ProjectId { get; set; }
    }
}
