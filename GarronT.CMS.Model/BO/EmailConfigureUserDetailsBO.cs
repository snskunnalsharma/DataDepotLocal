using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class EmailConfigureUserDetailsBO
    {
        public long ID { get; set; }
        public Nullable<long> TblEmailConfigureID { get; set; }
        public Nullable<long> UserID { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}
