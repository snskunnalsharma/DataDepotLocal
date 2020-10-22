using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class WebServiceTimeClockBO
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
    public class WebServiceIimeClockDetailBO
    {
        public long Id { get; set; }
        public Nullable<long> FKTimeClockId { get; set; }
        public Nullable<long> FKInstallerId { get; set; }
        public Nullable<long> FKUploadedDataExcelId { get; set; }
        public Nullable<long> FKProductId { get; set; }
        public Nullable<System.DateTime> InTime { get; set; }
        public Nullable<System.DateTime> OutTime { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
    }
}
