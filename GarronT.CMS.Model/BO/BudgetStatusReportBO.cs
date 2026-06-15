using System.Collections.Generic;

namespace GarronT.CMS.Model.BO
{
    public class BudgetStatusReportBO
    {
        public string ServiceDescription { get; set; }

        public decimal UnitCost { get; set; }

        public int TotalCount { get; set; }

        public decimal TotalCost { get; set; }
    }

    public class BudgetStatusReportResultBO
    {
        public decimal ApprovedProjectBudget { get; set; }

        public decimal UsedBudget { get; set; }

        public decimal BalanceBudget { get; set; }

        public List<BudgetStatusReportBO> ReportRows { get; set; }

        public BudgetStatusReportResultBO()
        {
            ReportRows = new List<BudgetStatusReportBO>();
        }
    }

    public class BudgetUtilityBO
    {
        public long Id { get; set; }

        public string UtilityName { get; set; }
    }

    public class BudgetProjectBO
    {
        public long ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectCityState { get; set; }
    }

    public class BudgetInstallerBO
    {
        public long UserId { get; set; }

        public string UserName { get; set; }
    }
}