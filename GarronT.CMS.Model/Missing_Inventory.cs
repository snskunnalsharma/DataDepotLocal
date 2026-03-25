using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GarronT.CMS.Model
{
    public partial class Missing_Inventory
    {
        public long Id { get; set; }

        public long? FKUserId { get; set; }

        public string ProductName { get; set; }

        public string SerialNumber { get; set; }

        public int Qty { get; set; }

        public string UserEmail { get; set; }   // ✅ NEW FIELD

        public DateTime CreatedOn { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public long? ModifiedBy { get; set; }

        public virtual tblUser tblUser { get; set; }
    }
}