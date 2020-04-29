using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class ReasonBO
    {
        public long SkipId { get; set; }
        public string SkipReason { get; set; }
        public string CategoryType { get; set; }
        public string Comment { get; set; }
        public bool? IsMediaRequired { get; set; }
        public bool? IsImageRequired { get; set; }
        public bool? IsAudioRequired { get; set; }
        public bool? IsVideoRequired { get; set; }
        public List<long> FormTypeList { get; set; }       
        public Nullable<int> Active { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}
