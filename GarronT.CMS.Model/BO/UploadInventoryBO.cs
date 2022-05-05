using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class UploadInventoryBO
    {
        public long  UniqueId { get; set; }
        public long Id { get; set; }

        public long InwardHeaderId { get; set; }

        //public long FK_InventoryUpload_Project { get; set; }

        public long UploadNumber { get; set; }
        [Required]
        public string UtilityType { get; set; }
        [Required]
        public string ProductName { get; set; }

        public string ProductDescription { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public string WarehouseName { get; set; }
        [Required]
        public string PartNumber { get; set; }
       
        public string UPCCode { get; set; }

        public string Make { get; set; }

        public string Type { get; set; }

        public string Size { get; set; }

        public string SerialNumber { get; set; }

        public string IsDuplicate { get; set; }
        [Required]
        public int Quantity { get; set; }


        public long ProductId { get; set; }

        public long UtilityId { get; set; }
        public long WarehouseId { get; set; }
        public string Location { get; set; }

        public bool SerialNumberAllowded { get; set; }

        public int InventryStatus { get; set; }
        public bool IsSerialNumber { get; set; }
    }



    public class DownloadInventory
    {

        public string UtilityType { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }

        public string PartNumber { get; set; }

        public string UPCCode { get; set; }

        public string Make { get; set; }

        public string Type { get; set; }

        public string Size { get; set; }

        public string SerialNumber { get; set; }
        public int Quantity { get; set; }

        public string Status { get; set; }

    }


    public class InwardHeaderBO
    {
        public long Id { get; set; }
        public int UploadType { get; set; }
        public Nullable<long> FKWarehouseId { get; set; }
        public string ExcelFilePath { get; set; }
        public Nullable<System.DateTime> DeviceDateTime { get; set; }
        public string JsonRequestData { get; set; }
        public int Active { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string GPSLocation { get; set; }
        public string DeviceIP { get; set; }
    }

    public class UploadInventryVM
    {

        public List<UploadInventoryBO> UploadInventoryList { get; set; }

        public List<ProductMasterBO> ProductMasterList { get; set; }

        public List<ProductMasterBO> BarcodeProductList { get; set; }

        public long UserId { get; set; }

        public long ProjectId { get; set; }

        public long WarehouseId { get; set; }

        public string StrFilePath { get; set; }

        public long UtilityId { get; set; }

        public long UploadType { get; set; }

        public DateTime? DeviceDateTime { get; set; }

    }

}
