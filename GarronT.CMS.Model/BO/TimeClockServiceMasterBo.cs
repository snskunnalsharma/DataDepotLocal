using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class TimeClockServiceMasterBo
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceDisplayName { get; set; }
        public decimal PayAmount { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public decimal Cost { get; set; }
    }
}
