using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class ProjectCustomeFieldReletion
    {
        public long? ID { get; set; }
        public int MasterTableField { get; set; }
        public Int64 TableID { get; set; }
        public Int64 ProjectID { get; set; }
    }
}
