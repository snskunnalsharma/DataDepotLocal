using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{



    public class ErrorModel
    {
        public string status { get; set; }
        public string userId { get; set; }
    }
    public class ErrorModelOnlyStatus
    {
        public string status { get; set; }
    }
    public class ErrorModelStatus
    {
        public string status { get; set; }
        public string errorMessage { get; set; }
    }

    public class ErrorModel_AddAppointment
    {
        public string status { get; set; }
        public string userId { get; set; }
        public string typeofOperation { get; set; }
    }
    public class TodaysDateModel
    {
        public double todaysDate { get; set; }
    }
    public class ErrorModelLogin
    {
        public string status { get; set; }
        public string userId { get; set; }
        public string isFirstLogin { get; set; }
    }
    public class MobileInputModel
    {
        public string userId { get; set; }
        public string accountNumber { get; set; }
    }
    public class AccountListModel
    {
        public List<AccountMobileModel> accountList { get; set; }
        public List<WorkTypeModel> workTypeList { get; set; }
    }
    public class AccountMobileModel
    {
        public Nullable<long> PJPID { get; set; }
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public string address { get; set; }
        public string mobileNo { get; set; }
        public string remark { get; set; }
        public Nullable<long> billDueDate { get; set; }
        public Nullable<decimal> latitude { get; set; }
        public Nullable<decimal> longitude { get; set; }
        public string territory { get; set; }
        public int newAppointment { get; set; }
        public int visitDone { get; set; }
        public List<ContactModel> contactPersonsDetails { get; set; }
        public double visitDate { get; set; }
    }
    public class ContactModel
    {
        public string name { get; set; }
        public string contactNumber { get; set; }
        public string emailId { get; set; }
    }

    public class TodaysVisitMobileModel
    {
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public Nullable<long> pjpId { get; set; }
        public string workType { get; set; }

    }
    public class VisitMobileModel
    {
        public Nullable<double> date { get; set; }
        public string workType { get; set; }
        public string number { get; set; }
        public string remark { get; set; }
    }
    public class WorkTypeModel
    {
        public Nullable<long> workTypeID { get; set; }
        public string workType { get; set; }
    }

    public class AccountList
    {
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public Nullable<decimal> latitude { get; set; }
        public Nullable<decimal> longitude { get; set; }

    }

    public class AccountHistory
    {
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public Nullable<double> date { get; set; }
        public string workType { get; set; }
        public string number { get; set; }
        public string remark { get; set; }
    }


    public class TodaysVisitInfo
    {

        public string projectId { get; set; }
        public string projectName { get; set; }
        public List<visitInfo> visits { get; set; }

    }

    public class ProjectVisitInfo
    {

        public string projectId { get; set; }
        public string projectName { get; set; }
        public string InstallerId { get; set; }
        public List<visitInfo> visits { get; set; }

    }

    public class visitInfo
    {
        public string Id { get; set; }
        public string account { get; set; }
        public string street { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool isVisited { get; set; }
    }


    public class ProjectMapVisitInfo
    {

        public string projectId { get; set; }
        public string projectName { get; set; }
        public List<visitMapInfo> visits { get; set; }

    }
    public class visitMapInfo
    {
        public string street { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public bool isVisited { get; set; }
        public int colorFlag { get; set; }
    }

    public class ProMapVisitInfo
    {

        public string projectId { get; set; }
        public string projectName { get; set; }
        public List<addressVisitInfo> visits { get; set; }

    }
    public class addressVisitInfo
    {
        public string street { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public bool isVisited { get; set; }
        public string totalVisit { get; set; }
        public string completedVisit { get; set; }
        public string skippedVisit { get; set; }
        public string rtuVisit { get; set; }
        public string pendingVisit { get; set; }
        public string lastVisitDatetime { get; set; }
        public int colorFlag { get; set; }
    }


    public class uploadFiles
    {
        //string projectId, string FormId, string AccountNumber, string tblUploadId, byte[] item
        public string formId { get; set; }
        public string accountInitalFieldId { get; set; }
        public string accountNumber { get; set; }
        public string initialFieldId { get; set; }
        public string fieldName { get; set; }
        public byte[] fileData { get; set; }

    }

    public class AccountsByStreet
    {
        public string projectId { get; set; }
        public string projectName { get; set; }
        public string accountNumber { get; set; }
        public string oldMeterNumber { get; set; }
        public string oldMeterType { get; set; }
        public string oldMeterSize { get; set; }
        public string accountInitalFieldId { get; set; }
        public bool isVisited { get; set; }
        public string visitDateTime { get; set; }
    }


    public class ProjectInfo
    {
        public long project_Id { get; set; }
        public string projectId { get; set; }
        public string projectName { get; set; }
        public List<AccountInfo> AccountInfo { get; set; }
    }
    public class AccountInfo
    {
        public string accountNumber { get; set; }
        public string oldMeterNumber { get; set; }
        public string oldMeterType { get; set; }
        public string oldMeterSize { get; set; }
        public string accountInitalFieldId { get; set; }
        public bool isVisited { get; set; }
        public string visitDateTime { get; set; }
    }


    public class AddressByLatLon
    {
        public string projectId { get; set; }
        public string projectName { get; set; }
        public string userId { get; set; }
        public List<AddressStatus> status { get; set; }
    }

    public class AddressStatus
    {
        public string street { get; set; }
        public bool isVisited { get; set; }
        public string visitCompleteStatus { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
    }



    public class FormListAgainstProject
    {
        public string formId { get; set; }
        public string formName { get; set; }
        public bool isVisited { get; set; }
        public bool isSkipped { get; set; }

    }
}
