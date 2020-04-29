using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class ProjectStandardFieldReletionBO
    {
        public long? id { get; set; }
        public int filedid { get; set; }
        public bool Acyive { get; set; }
        public Int64 ProjectID { get; set; }
        public long StandarFiledName { get; set; }
    }
}
