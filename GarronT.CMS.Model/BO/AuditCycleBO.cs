using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class AuditCycleBO
    {
        public long Id { get; set; }

        public long ProjectId { get; set; }


        public int AuditStartDate { get; set; }


        public int AuditEndDate { get; set; }


        public int UserId { get; set; }

        public decimal AuditPercentage { get; set; }


        public long InstallerId { get; set; }

        public string InstallerName { get; set; }

        public string AuditFailed { get; set; }

        // public List<InventorySourceChildBO> objList { get; set; }
        public int Active { get; set; }
    }

    public class AuditFailCycleBO
    {
        public long Id { get; set; }

        public long ProjectId { get; set; }

        public decimal FailPercentage { get; set; }

        public int IncreaseByDays { get; set; }
    }



    public class AuditCreateBO
    {
        public long auditProjectId { get; set; }
        public long projectId { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string[] auditorList { get; set; }

        public List<AuditCycleBO> objList { get; set; }
        public List<AuditProjectFailBO> objAuditProjectFailBOList { get; set; }
    }


    public class AuditProjectFailBO
    {
        public long auditProjectId { get; set; }
        public long Id { get; set; }
        public decimal FailPercentageFrom { get; set; }
        public decimal FailPercentageTo { get; set; }
        public int IncreaseByDays { get; set; }
        public int Active { get; set; }
        //public string startDate { get; set; }
        //public string endDate { get; set; }
    }

}
