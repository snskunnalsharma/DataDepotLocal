namespace GarronT.CMS.Model
{
    using System;

    public partial class PROC_InstallStockDetailReport_Result
    {
        public DateTime? CreatedOn { get; set; }
        /// <summary>Pre-formatted for grid (date on one line, time on next via &lt;br/&gt;).</summary>
        public string DateAndTime { get; set; }
    }
}
