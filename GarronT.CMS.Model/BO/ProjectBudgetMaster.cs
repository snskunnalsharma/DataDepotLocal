namespace GarronT.CMS.Model
{
    using System;

    public partial class ProjectBudgetMaster
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string BudgetCode { get; set; }
        public string BudgetName { get; set; }
        public decimal BudgetAmount { get; set; }
        public string BudgetType { get; set; }
        public Nullable<long> ServiceId { get; set; }
        public string ServiceCode { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
    }
}