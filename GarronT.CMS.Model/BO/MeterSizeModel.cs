using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class MeterSizeModel
    {
        public long ID { get; set; }

        public string MeterSize { get; set; }

        public string Description { get; set; }

        public int Active { get; set; }

        public Nullable<int> CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public Nullable<int> ModifiedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        // Optional. This is not used by current meter pay rate save flow.
        public Nullable<decimal> CostRate { get; set; }
    }
}