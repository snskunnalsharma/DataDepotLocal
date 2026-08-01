using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class WarehouseBO
    {
        public long WarehouseId { get; set; }

        [Required(ErrorMessage = "Warehouse Name is required")]
        [StringLength(250, ErrorMessage = "Warehouse Name must be at least {2} and max {1} characters long.", MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string WarehouseName { get; set; }

        [Required(ErrorMessage = "Warehouse Location is required")]
        [StringLength(250, ErrorMessage = "Warehouse Location must be at least {2} and max {1} characters long.", MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string WarehouseLocation { get; set; }

        [Required(ErrorMessage = "State is required")]
        public long StateId { get; set; }

        public string StateName { get; set; }

        [Required(ErrorMessage = "City is required")]
        public long CityId { get; set; }

        public string CityName { get; set; }

        public long CurrentUserId { get; set; }

        public List<long> userList { get; set; }
    }


    public class MobileUtilityType
    {
        public long Id { get; set; }
        public string UtilityName { get; set; }
    }


    public class MobileWarehouseBO
    {
        public long Id { get; set; }

        public string WarehouseName { get; set; }
    }



    public class WarehouseStockReport
    {
        public long StockDetailId { get; set; }
        public long WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string PartNumber { get; set; }
        public string UPCCode { get; set; }
        public string UtilityName { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Make { get; set; }
        public string MeterType { get; set; }
        public string MeterSize { get; set; }
        public string serialnumber { get; set; }
        public Nullable<int> BalanceQty { get; set; }

        public Nullable<int> TotalQty { get; set; }

        public bool SerialNumberRequired { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        /// <summary>Pre-formatted for grid/export display (MM/dd/yyyy HH:mm:ss).</summary>
        public string DateAndTime { get; set; }
    }
    public class WarehouseStockReportExport
    {

        public string WarehouseName { get; set; }
        public string Location { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string PartNumber { get; set; }
        public string UPCCode { get; set; }
        public string UtilityName { get; set; }
        public string Make { get; set; }
        public string MeterType { get; set; }
        public string MeterSize { get; set; }
        public string serialnumber { get; set; }
        public Nullable<int> BalanceQty { get; set; }
        public string DateAndTime { get; set; }
    }


    public class WarehouseStockReportSummeryExport
    {

        public string WarehouseName { get; set; }
        public string Location { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string PartNumber { get; set; }
        public string UPCCode { get; set; }
        public string UtilityName { get; set; }
        public string Make { get; set; }
        public string MeterType { get; set; }
        public string MeterSize { get; set; }
        public Nullable<int> BalanceQty { get; set; }
        public string DateAndTime { get; set; }
    }
}
