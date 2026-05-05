using System;

namespace GarronT.CMS.Model.BO
{
    public class INVInstallerReportUpsertBO
    {
        public long ProjectId { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string InstallerName { get; set; }
        public string WorkOrderId { get; set; }
        public string SerialNumber { get; set; }
        public string ServiceAddress { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Status { get; set; }
        public string ProjectName { get; set; }
        public DateTime? OperationDate { get; set; }

    }
}

