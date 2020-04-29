using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GarronT.CMS.Model.BO
{
   public class ClientModel
    {
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public Nullable<long> City { get; set; }
        public Nullable<long> State { get; set; }
        public Nullable<long> Country { get; set; }
        public string Pincode { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string ClientEmailId { get; set; }
        public int Active { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }


        public string CityName { get; set; }
        public string StateName { get; set; }

    }

   public class ContactModelDDL
   {
       public long ContactId { get; set; }
       public string ContactName { get; set; }
   }

   public partial class tblUploadedDataBO
   {
       public long ID { get; set; }
       public Nullable<long> ProjectId { get; set; }
       public Nullable<long> StateId { get; set; }
       public Nullable<long> CityId { get; set; }
       public string Cycle { get; set; }
       public string Route { get; set; }
       public string Account { get; set; }
       public string Street { get; set; }
       public string Street_route { get; set; }
       public string street_number { get; set; }
       public string OldMeterNo { get; set; }
       public string OldMeterSize { get; set; }
       public string OldMeterType { get; set; }
       public string OldMeterRadioNo { get; set; }
       public string OldMeterLastReading { get; set; }
       public string NewMeterNo { get; set; }
       public string NewMeterResgisterNo { get; set; }
       public string NewMeterRadio { get; set; }
       public string NewMeterSize { get; set; }
       public string NewMeterType { get; set; }
       public string Latitude { get; set; }
       public string Longitude { get; set; }
       public int Active { get; set; }
       public Nullable<int> CreatedBy { get; set; }
       public Nullable<System.DateTime> CreatedOn { get; set; }
       public Nullable<int> ModifiedBy { get; set; }
       public Nullable<System.DateTime> ModifiedOn { get; set; }

       public string AdressStatus { get; set; }
       public string InstallerName { get; set; }

       public bool? IsVisisted { get; set; }
   }
}
