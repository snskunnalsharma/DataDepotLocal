using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
   public class ErrorUserModel
    {
        public long UserID { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public Nullable<int> IsFirstLogin { get; set; }
        public Nullable<System.Guid> aspnet_UserId { get; set; }
        public Nullable<long> CompanyID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<long> RoleID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public Nullable<long> CityID { get; set; }
        public Nullable<long> CountryID { get; set; }
        public Nullable<long> StateID { get; set; }
        public Nullable<int> ISARM { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public Nullable<decimal> HomeLatitude { get; set; }
        public Nullable<decimal> HomeLongitude { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string Pincode { get; set; }

        public string Role { get; set; }
        public string S_ISARM { get; set; }
        public string EmailIDHidden { get; set; }
        public string MobileNoHidden { get; set; }

        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string TerritoryName { get; set; }

        public string Description { get; set; }
    }
}
