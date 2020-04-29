using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class UsersModel
    {
        public long UserId { get; set; }

        [Required(ErrorMessage = "Login Id Required")]
        public string LogInId { get; set; }

        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password Required")]
        public string Confirmpassword { get; set; }
        


        //[Required(ErrorMessage = "Name  Required")]
        //[RegularExpression(@"^[a-zA-Z\S]+$", ErrorMessage = "Use letters only please")]
        public string UserFullName { get; set; }

        //[Required(ErrorMessage = "Email is required")]
       // [RegularExpression("^[A-Za-z0-9._%+-]*@[A-Za-z0-9.-]*\\.[A-Za-z0-9-]{2,}$", ErrorMessage = "Email is required and must be properly formatted.")]
       // [StringLength(50, ErrorMessage = "Email must be at least {2} and max {1} characters long.", MinimumLength = 1)]
        public string Email { get; set; }


       // [Required(ErrorMessage = "State is required")]
        public long? StateID { get; set; }

      //  [Required(ErrorMessage = "City is required")]
        public long? CityID { get; set; }



        public string FirstName { get; set; }




        public long? MobileNo { get; set; }

        public Nullable<System.Guid> aspnet_UserId { get; set; }

        public string UserName { get; set; }
        public string Address { get; set; }
        public string RoleID { get; set; }

        public Nullable<long> ManagerId { get; set; }
        public Nullable<long> FieldSupervisorId { get; set; }
        public Nullable<int> ManagerApprovalRequired { get; set; }

        public string PrimaryRoleId { get; set; }
        public string PrimaryRole { get; set; }
        public bool AdditionalRole1 { get; set; }
        public bool AdditionalRole2 { get; set; }
        public bool AdditionalRole3 { get; set; }
        public bool AdditionalRole4 { get; set; }
        public int IsFirstLogin { get; set; }
        public int PasswordExpiryDays { get; set; }

        public Nullable<int> Active { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }

        //new fields added by aniket 7-5-2016


        public string ZipCode { get; set; }
        public string Gender { get; set; }
        public string HomePhoneNumber { get; set; }

        //vehicle
        public long? VehicleMake { get; set; }
        public long? VehicleModel { get; set; }
        public int? VehicleYear { get; set; }
        public string VehicleLicensePlateNumber { get; set; }
        public string Vehicle { get; set; }
        public string VehicleColor { get; set; }
        public string ImageofVehicle { get; set; }
        public string TemplateURL { get; set; }


        //Employment Details

        public string JobType { get; set; }
        public string InstallerImage { get; set; }
        public long? FormType { get; set; }

        public string profilepicfolder { get; set; }
        public string vehiclepicfolder { get; set; }

        public List<WarehouseUserBO> objWarehouseUserList { get; set; }

    }


    public class WarehouseUserBO
    {
        public long Id { get; set; }
        public long FKUserId { get; set; }
        public long FKWareHouseId { get; set; }
    }


    public class VehicleModelData
    {
        public long? VehicleMake { get; set; }
        public long? VehicleModel { get; set; }
        public int? VehicleYear { get; set; }
        public string VehicleLicensePlateNumber { get; set; }
        public string Vehicle { get; set; }
        public string VehicleColor { get; set; }
        public string ImageofVehicle { get; set; }
        public string TemplateURL { get; set; }

        public string JobType { get; set; }


    }

    public class UsersSmallModel
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
    }

    public class UsersServiceModel
    {
        public Nullable<long> ID { get; set; }
        public Nullable<long> UserID { get; set; }
        public string ServiceName { get; set; }
        public Nullable<long> ServiceID { get; set; }
        public string RatesUnit { get; set; }
        public Nullable<decimal> ServiceRates { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }

    public class UserViewModel
    {
        public List<UsersServiceModel> ServiceRatesList { get; set; }
        public List<string> objroleList { get; set; }
        public List<int> objFormTypeList { get; set; }
        public UsersModel UserModel { get; set; }
        public VehicleModelData VehicleModelData { get; set; }

        public string profilepicfolder { get; set; }
        public string vehiclepicfolder { get; set; }
    }


    public class FormTypeModel
    {
        public long ID { get; set; }
        public string FormType { get; set; }
    }

    public class CityImageNameList
    {
        public long CityId { get; set; }
        public long StateId { get; set; }
        public string ImageName { get; set; }

    }

}
