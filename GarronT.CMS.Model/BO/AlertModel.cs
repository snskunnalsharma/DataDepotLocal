using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class AlertModel
    {
        public long AlertId { get; set; }
        public long ProjectID { get; set; }
        public long AdminEmailIdid { get; set; }
        public string AlertName { get; set; }
        public string AdminEmailId { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public List<string> Emailidlist { get; set; }
    }
}
