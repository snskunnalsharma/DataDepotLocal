using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class InventoryUploadBO
    {

        public long Id { get; set; }

        //public long FK_InventoryUpload_Project { get; set; }

        public long UploadNumber { get; set; }

        public string SerialNo { get; set; }

        public string MeterSize { get; set; }

        public string MeterType { get; set; }

        public string MeterMake { get; set; }

        public string IsDuplicate { get; set; }

        public int Quantity { get; set; }

        public string Description { get; set; }


    }

    public partial class InventryStockReportExport
    {
       
        public string InstallerName { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string SerialNumber { get; set; }
        public int ScannedInventry { get; set; }
        public int InstalledInventry { get; set; }
        public int TransferredInventry { get; set; }
        public int ReturnedInventry { get; set; }
        public Nullable<int> InventryInHand { get; set; }
        public string DateAndTime { get; set; }

    }

    public partial class InventryStockSummaryReportExport
    {

        public string InstallerName { get; set; }

        public string CategoryName { get; set; }
       
        public int? ScannedInventry { get; set; }
        public int? InstalledInventry { get; set; }
        public int? TransferredInventry { get; set; }
        public int? ReturnedInventry { get; set; }
        public Nullable<int> InventryInHand { get; set; }

    }

}
