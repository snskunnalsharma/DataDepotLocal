using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
  public  class CityModel
    {
        public long CityID { get; set; }
        public long StateID { get; set; }
        public string StateName { get; set; }

        public Nullable<long> CountryID { get; set; }
        public string CountryName { get; set; }

        public Nullable<int> Active { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string CityName { get; set; }
    }
}
