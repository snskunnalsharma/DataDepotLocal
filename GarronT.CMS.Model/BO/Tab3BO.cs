using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class Tab3BO
    {
        public long projectId { get; set; }
        public List<ProjectServiceBO> Servicedata { get; set; }
    }

    public class ProjectServiceBO
    {
        public long serviceId { get; set; }
        public decimal servicePrice { get; set; }
        public string imageCaptureRequired { get; set; }
        public long[] AssignedProductList { get; set; }
    }

    public class Tab3BOMeterPayRate
    {
        public long projectId { get; set; }
        public List<PayRateBO> newMeterPayRate { get; set; }
    }

    public class PayRateBO
    {
        public long Id { get; set; }
        public decimal Pay { get; set; }
    }
}
