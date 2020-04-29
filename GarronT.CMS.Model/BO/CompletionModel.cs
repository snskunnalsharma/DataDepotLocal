using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class CompletionModel
    {
        public int Installations { get; set; }
        public int percentage { get; set; }
        public string Status { get; set; }
        public long projectId { get; set; }
        public string projectName { get; set; }
        public string projectDisplayName { get; set; }
    }

    public class InstallationModel
    {
        public decimal Value { get; set; }
        public string Month { get; set; }
        public long InstallerID { get; set; }
        public string Installer { get; set; }

        public decimal Total { get; set; }
        public decimal Complete { get; set; }
        public decimal Pending { get; set; }
        public decimal Pending1 { get; set; }
        public decimal Skipped { get; set; }
        public decimal RTU { get; set; }
    }

    //Added by sominath To display the Installer- Audite Chart
    //public class DashboardAuditChartModel
    //{
    //    public long ProjectID { get; set; }
    //    public long InstallerID { get; set; }      

    //    public string Installer { get; set; }


    //    public decimal TotalInstallation { get; set; }
    //    public decimal TotalAssignedAudit { get; set; }
    //    public decimal TotalCompleteAudit { get; set; }
    //    public decimal TotalPassedAudit { get; set; }
    //    public decimal TotalFailedAudit { get; set; }

    //    public  List<DashboardAuditChartModel> fullList { get; set; }




    //}


    public class AuditChartModel
    {
        public long ProjectID { get; set; }
        public long InstallerID { get; set; }
        public string Installer { get; set; }
        public int TotalInstallation { get; set; }
        public int TotalAssignedAudit { get; set; }
        public int TotalCompleteAudit { get; set; }
        public int TotalPassedAudit { get; set; }
        public int TotalFailedAudit { get; set; }

    }

    public class PerformanceModel
    {
        public decimal Complete { get; set; }
        public decimal Pending { get; set; }
        public string Category { get; set; }
    }
    public class ProjectInstallationModel
    {
        public decimal Sales { get; set; }
        public string Project { get; set; }
    }


    public class DashboardHeader
    {
        public long CityID { get; set; }
        public string CityName { get; set; }

        public long ProjectID { get; set; }
        public string ProjectName { get; set; }
    }
}
