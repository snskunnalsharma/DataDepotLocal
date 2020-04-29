using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class InstallerAuditCycleBO
    {

        public long Id { get; set; }
        public long FK_ProjectId { get; set; }
        public long FK_CustomerId { get; set; }
        public long FK_InstallerId { get; set; }
        public string InstallerName { get; set; }
        public decimal AuditPercentage { get; set; }
        public int StartDay { get; set; }
        public int EndDay { get; set; }
        public string AuditFailReached { get; set; }
    }
}
