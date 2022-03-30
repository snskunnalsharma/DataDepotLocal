using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class ProductMasterBO
    {
        public long Id { get; set; }
        public Guid UniqueId { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(250, ErrorMessage = "Product Name must be at least {2} and max {1} characters long.", MinimumLength = 1)]
        public string ProductName { get; set; }

        public long WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        public string SerialNumber { get; set; }
        [StringLength(250, ErrorMessage = "Product Description must be at least {2} and max {1} characters long.", MinimumLength = 1)]
        public string ProductDescription { get; set; }

        public int Quantity { get; set; }

        [Required(ErrorMessage = "Part Number is required")]
        [StringLength(50, ErrorMessage = "Part Number must be at least {2} and max {1} characters long.", MinimumLength = 1)]
        //[RegularExpression(@"[A-Z0-9-]{3,25}$", ErrorMessage = "Part Number must hold numbers & capital letters only")] // Allow Special Charectors as per clent request
        public string ProductPartNumber { get; set; }

        //

        public bool IncludePartNoInBarcode { get; set; }

        public bool IncludeUPCInBarcode { get; set; }


        [StringLength(13, ErrorMessage = "UPC Code must be at {2} digit long.", MinimumLength = 12)]
        [RegularExpression(@"^[0-9]{12,13}$", ErrorMessage = "UPC Code must hold numbers only")]
        public string ProductUPCCode { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public long CategoryId { get; set; }

        public bool SerialNumberRequired { get; set; }
        public bool GenerateBarcode { get; set; }
        public bool Consumables { get; set; }

        public string CategoryName { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Base { get; set; }
        public string Make { get; set; }
        public string Utility { get; set; }
        public string strSerialNumberTracking { get; set; }
        public string strConsumableProduct { get; set; }


        [Required(ErrorMessage = "Utility is required")]
        public long UtilityId { get; set; }

        public long ProjectId { get; set; }

        [Required(ErrorMessage = "Make is required")]
        public long MeterMakeId { get; set; }

        public long? MeterSizeId { get; set; }


        public long? MeterTypeId { get; set; }

        public List<CommonDropDownClass> UtilityTypeList { get; set; }

        public List<CommonDropDownClass> MeterSizeList { get; set; }

        public List<CommonDropDownClass> MeterMakeList { get; set; }

        public List<CommonDropDownClass> MeterTypeList { get; set; }


        public List<long> SelectedUtilityTypeList { get; set; }

        public List<long> SelectedMeterSizeList { get; set; }

        public List<long> SelectedMeterMakeList { get; set; }

        public List<long> SelectedMeterTypeList { get; set; }

        //[Required(ErrorMessage = "Barcode information is required")]
        //public bool BarcodeRequired { get; set; }

        public string UtilityName { get; set; }

        public bool SerialNumberTracking { get; set; }

        public bool ConsumableProduct { get; set; }

        public long CurrentUserId { get; set; }

        public long ParentId { get; set; }
        public string WebUrlOfBarcodeFile { get; set; }

        public int? WarehouseMinimumStock { get; set; }
        public int? UserMinimumStock { get; set; }

        public bool AlertTypeEmail { get; set; }
        public bool AlertTypeSMS { get; set; }

        public List<long> ManagerList { get; set; }

        public string RecordStatus { get; set; }
        public int ValidRecords { get; set; }
    }
    //ObjMeterSizeList

    public class CommonDropDownClass
    {
        public long Id { get; set; }

        public string FieldValue { get; set; }

    }





    public class MobileProductBO
    {
        public long Id { get; set; }

        public string PartNumber { get; set; }

        public bool ProductHasSerialNumber { get; set; }

        public string ProductName { get; set; }

        public string UPCCode { get; set; }

        public string Make { get; set; }

        public string Type { get; set; }

        public string Size { get; set; }
    }

}
