using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class UserDetailsModel
    {
        public String ID { get; set; }
        public long TempID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
   
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string FullName { get; set; }
        public long UserID { get; set; }
    }
}
