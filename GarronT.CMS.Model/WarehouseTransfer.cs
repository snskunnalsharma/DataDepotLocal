using System;

namespace GarronT.CMS.Model
{
    public class WarehouseTransfer
    {
        public long Id { get; set; }
        public string SerialNumber { get; set; }
        public long FromWarehouseId { get; set; }
        public long ToWarehouseId { get; set; }
        public DateTime TransferDate { get; set; }
        public long TransferredByUserId { get; set; }
        public long ProjectId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}