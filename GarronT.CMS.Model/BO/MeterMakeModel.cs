using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class MeterMakeModel
    {


        public long ID { get; set; }
        [Required(ErrorMessage = "Meter Size Required")]
        public string Make { get; set; }
        public string Description { get; set; }
        public int Active { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }

    public class MasterListModel
    {
        public List<MeterMakeModel> objMeterMakeModelList { get; set; }
        public List<MeterSizeModel> objMeterSizeList { get; set; }
        public List<MeterTypeModel> objMeterTypeList { get; set; }
        public List<ServicesModel> objServiceList { get; set; }
        public List<UsersSmallModel> objUsersSmallModelList { get; set; }
    }

}
