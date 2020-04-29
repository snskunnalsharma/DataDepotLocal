using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSPL.AIBC.ARM.Model.BO
{
  public class VehicleModel
    {
        public long VehicleID { get; set; }
        public Nullable<long> UserId { get; set; }
        public Nullable<long> ModelID { get; set; }
        public string LicensePlate { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<System.DateTime> CreateOn { get; set; }
        public Nullable<int> Createdby { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> Modifiedby { get; set; }
        public string Vehicle { get; set; }
        public Nullable<long> MakeID { get; set; }
        public string Color { get; set; }
        public Nullable<int> Year { get; set; }
        public string ImageofVehicle { get; set; }

    }

  public class VehicleMakeModel
  {
      public long MakeId { get; set; }
      public string Make { get; set; }
      public Nullable<int> Active { get; set; }
      public Nullable<int> CreatedBy { get; set; }
      public Nullable<System.DateTime> CreatedOn { get; set; }
      public Nullable<int> ModifiedBy { get; set; }
      public Nullable<System.DateTime> ModifiedOn { get; set; }

  }
  public class VehicleModelVM
  {
      public long ID { get; set; }
      public long tblMakeMasterId { get; set; }
      public string Model { get; set; }
      public Nullable<int> Active { get; set; }
      public Nullable<int> CreatedBy { get; set; }
      public Nullable<System.DateTime> CreatedOn { get; set; }
      public Nullable<int> ModifiedBy { get; set; }
      public Nullable<System.DateTime> ModifiedOn { get; set; }

  }
}
