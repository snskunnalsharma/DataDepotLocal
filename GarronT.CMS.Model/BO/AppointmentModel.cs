using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
   public class AppointmentModel
    {
        public long AppointmentID { get; set; }
        public string AccountNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string DayTimePhone { get; set; }
        public Nullable<long> FKProjectId { get; set; }
        public string Address { get; set; }
        public Nullable<long> FKCityId { get; set; }
        public Nullable<long> FKStateId { get; set; }
        public string ZipCode { get; set; }
        public Nullable<System.DateTime> FirstPreferedDate { get; set; }
        public string FirstTime { get; set; }
        public Nullable<System.DateTime> SecondPreferedDate { get; set; }
        public string SecondTime { get; set; }
        public Nullable<System.DateTime> ThirdPreferedDate { get; set; }
        public string ThirdTime { get; set; }
        public string MessageBody { get; set; }
        public Nullable<int> MayContactByEmail { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public int Active { get; set; }
        public Nullable<int> PassedDateFlag { get; set; }
        public string City { get; set; }     // Both added on 28/12/2016
        public string State { get; set; }


        // For Setting the flag=0 first time and second time flag=1 for second time
        public string DateForApp { get; set; }
        public long FK_UploadedId { get; set; }
        public long InstallerID { get; set; }
        public long ProjectId { get; set; }
        public string InstallerName { get; set; }
        public string ProjectName { get; set; }
        public string UtilityType { get; set; }
       // public string State { get; set; }
       // public string CityName { get; set; }
        public long Returnflag { get; set; } 
        public string DateOnTable { get; set; }
        public string Route { get; set; }
        public string Cycle { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; }
        public long FK_AppointmentID { get; set; }
    }

    public class AppointmentList
    {
        public string AccountNumber { get; set; }
        public string Address { get; set; }     
        public Nullable<System.DateTime> AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
    }




    public class BookingSuccessModel
    {
        // For displaying the Appointment Booked Successfully on new page
        public string ProjectName { get; set; }
        public string UserName { get; set; }
        public string AccountNumber { get; set; }
        public string AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string Email { get; set; }
        public string UserPhone { get; set; }
        public string StateName {get;set;}
        public string Street { get; set; }
        public string CityName { get; set; }

        
       

    }

    public partial class ExportReport_CompletedRecords
    {
        public Nullable<long> ID { get; set; }
        public Nullable<long> TblProjectFieldData_Id { get; set; }
        public Nullable<long> ProjectFieldDataMasterID { get; set; }
        public string InitialFieldName { get; set; }
        public string FieldValue { get; set; }
        public string FieldLatitude { get; set; }
        public string FieldLongitude { get; set; }
        public Nullable<System.DateTime> CapturedDateTime { get; set; }
        public string Cycle { get; set; }
        public string Route { get; set; }
        public Nullable<System.DateTime> visitdatetime { get; set; }
        public string Installer_Name { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string FormName { get; set; }
    }

}
namespace GarronT.CMS.Model
{

    public partial class Proc_GetInstallerFormStatusData_Result
    {
        public string Appointmentflag { get; set; }
    }
}