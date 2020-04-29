using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class DeviceMasterModel
    {
        public long DeviceId { get; set; }
        [Required(ErrorMessage = "Device Required")]
        public string Device { get; set; }
        public string QRCode { get; set; }
        public long DeviceTypeId { get; set; }
        public long DeviceSizeId { get; set; }
        public string Description { get; set; }

        public Nullable<int> Active { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}
