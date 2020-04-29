using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class EmailConfigureBO
    {
        public long ID { get; set; }
        public string AdminEmailID { get; set; }
        public Nullable<long> AlertTableID { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public List<string> Userid { get; set; }

        public List<UsersModel> UserModel { get; set; }
        public string UserEmailID { get; set; }
    }
}
