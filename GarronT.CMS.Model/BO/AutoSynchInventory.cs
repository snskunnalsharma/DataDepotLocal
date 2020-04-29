using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class AutoSynchInventory
    {

        public long TransferRequestId { get; set; }
        public List<TransferredSerialStock> SerialNumberList { get; set; }

        public List<TransferredStock> ProductQuantityList { get; set; }

    }

    public class TransferredStock
    {
        public long ProjectId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductQuantity { get; set; }
    }
    public class TransferredSerialStock
    {
        public long ProjectId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string SerialNumber { get; set; }
        public int ProductQuantity { get; set; }
    }
}
