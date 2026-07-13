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
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string ReportPeriodType { get; set; }

        public decimal TotalConfiguredProjectBudget { get; set; }

        public decimal PriorProjectUsedBudget { get; set; }

        public decimal ApprovedProjectBudget { get; set; }

        public decimal UsedBudget { get; set; }

        public decimal BalanceBudget { get; set; }

        public List<BudgetStatusReportBO> ReportRows { get; set; }

        public decimal TotalConfiguredServiceLineBudget { get; set; }

        public decimal PriorServiceLineUsedBudget { get; set; }

        public decimal ServiceLineBudget { get; set; }

        public decimal ServiceLineUsedBudget { get; set; }

        public decimal ServiceLineBalanceBudget { get; set; }

        public List<BudgetStatusReportBO> ServiceLineReportRows { get; set; }

        public BudgetStatusReportResultBO()
        {
            ReportRows = new List<BudgetStatusReportBO>();
            ServiceLineReportRows = new List<BudgetStatusReportBO>();
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