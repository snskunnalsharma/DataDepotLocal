using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSPL.AIBC.ARM.Model.BO
{
  public  class CountryModel
    {
        public long CountryID { get; set; }
        [Required(ErrorMessage = "Country Name Required")]      
        public string CountryName { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }

    }
}
