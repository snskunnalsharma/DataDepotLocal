using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class InstallerStockReportSummaryBO
    {

        public long UserId { get; set; }
        public string UserName { get; set; }
        public long FKProductId { get; set; }
        public long ProductCategoryId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public Nullable<int> OpeningBalance { get; set; }
        public int TotalRelasedQty { get; set; }
        public int InstalledQty { get; set; }
        public int ReturnedQty { get; set; }
        public int TransferredQty { get; set; }
        public Nullable<int> InHandInventory { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string DateAndTime { get; set; }
    }
    public class OpeningInventoryStockReportSummaryBO
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long FKProductId { get; set; }
        public long ProductCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> OpeningBalance { get; set; }

    }
    public class TempInstallerStockReportSummaryBO
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long FKProductId { get; set; }
        public long ProductCategoryId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        //public int OpeningBalance { get; set; }
        public int TotalRelasedQty { get; set; }
        public int InstalledQty { get; set; }
        public int ReturnedQty { get; set; }
        public int TransferredQty { get; set; }
        public Nullable<int> InHandInventory { get; set; }
    }
}
