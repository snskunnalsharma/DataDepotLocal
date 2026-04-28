using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.UI.Models;
using GarronT.CMS.UI.WebAPI;
using log4net;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services; 

namespace GarronT.CMS.UI
{
    /// <summary>
    /// Summary description for CMS_MobileService
    /// </summary>
    [WebService(Namespace = "https://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CMS_MobileService : System.Web.Services.WebService
    {
        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(CMS_MobileService));
        #endregion

        public static class MemberInfoGetting
        {
            public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
            {
                MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
                return expressionBody.Member.Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">
        /// 1=>is null or empty
        /// 2=>is invalid data in field
        /// 3=> only sucess
        /// 4=> exception / error message
        /// </param>
        /// <param name="status"></param>
        /// <param name="_fieldName"></param>
        void setJsonReponse(int id, string status, string _fieldName)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            Context.Response.ContentType = "application/json";

            if (id == 1)
            {
                //setJsonForIsNullOrEmptyField
                ErrorModelStatus obj1 = new ErrorModelStatus();
                obj1.status = status;
                obj1.errorMessage = "Missing parameter " + _fieldName + ".";
                Context.Response.Write(s.Serialize(obj1));
            }
            else if (id == 2)
            {
                //setJsonForInvalidInputField

                ErrorModelStatus obj1 = new ErrorModelStatus();
                obj1.status = status;
                obj1.errorMessage = "Parameter " + _fieldName + " is not valid.";
                Context.Response.Write(s.Serialize(obj1));
            }
            else if (id == 3)
            {
                //setJsonForSuccess
                ErrorModelOnlyStatus obj1 = new ErrorModelOnlyStatus();
                obj1.status = status;
                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(obj1));
            }
            else if (id == 4)
            {
                //setJsonForException
                ErrorModelStatus obj1 = new ErrorModelStatus();
                obj1.status = status;
                obj1.errorMessage = _fieldName;
                Context.Response.Write(s.Serialize(obj1));
            }
        }




        #region Forget Password Method
        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public void ForgotPassword(string userName)
        //{


        //    JavaScriptSerializer s = new JavaScriptSerializer();
        //    s.MaxJsonLength = 2147483644;
        //    if (string.IsNullOrEmpty(userName))
        //    {
        //        setJsonReponse(1, "Fail", MemberInfoGetting.GetMemberName(() => userName));
        //    }
        //    else
        //    {
        //        bool isFound = false;

        //        var result = new LoginDAL().ForgotPassword(userName, out isFound);
        //        if (result == true)
        //        {
        //            setJsonReponse(3, "Success", string.Empty);
        //        }
        //        else
        //        {
        //            if (isFound == true)
        //            { setJsonReponse(4, "Fail", "User is not active."); }
        //            else { setJsonReponse(4, "Fail", "User not exist."); }
        //        }
        //    }


        //}
        #endregion

        #region Change Password
        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public void ChangePassword(string userName, string oldPassword, string newPassword)
        //{
        //    JavaScriptSerializer s = new JavaScriptSerializer();
        //    s.MaxJsonLength = 2147483644;

        //    if (string.IsNullOrEmpty(userName))
        //    {
        //        setJsonReponse(1, "Fail", MemberInfoGetting.GetMemberName(() => userName));
        //    }
        //    else if (string.IsNullOrEmpty(newPassword))
        //    {
        //        setJsonReponse(1, "Fail", MemberInfoGetting.GetMemberName(() => newPassword));
        //    }
        //    else if (string.IsNullOrEmpty(oldPassword))
        //    {
        //        setJsonReponse(1, "Fail", MemberInfoGetting.GetMemberName(() => oldPassword));
        //    }
        //    else if (newPassword.Trim().Length < 8)
        //    {
        //        setJsonReponse(4, "Fail", "newPassword parameter must be minimum 8 character long.");
        //    }
        //    else if (newPassword.Trim().Length > 50)
        //    {
        //        setJsonReponse(4, "Fail", "newPassword parameter must be maximum 50 character long.");
        //    }
        //    else if (!newPassword.Any(char.IsUpper))
        //    {
        //        setJsonReponse(4, "Fail", "newPassword parameter must contain atleast 1 Uppercase character.");
        //    }
        //    else if (!newPassword.Any(char.IsDigit))
        //    {
        //        setJsonReponse(4, "Fail", "newPassword parameter must contain atleast 1 number.");
        //    }
        //    else if (!Regex.Match(newPassword, "[^a-z0-9]", RegexOptions.IgnoreCase).Success)
        //    {
        //        setJsonReponse(4, "Fail", "newPassword parameter must contain atleast 1 special character.");
        //    }
        //    else
        //    {
        //        bool containsAtLeastOneUppercase = newPassword.Any(char.IsUpper);

        //        var result = new LoginDAL().ChangePasswordforMobile(userName, oldPassword, newPassword);

        //        if (Convert.ToBoolean(result.Result.Item1) == true)
        //        {
        //            setJsonReponse(3, "Success", string.Empty);
        //        }
        //        else
        //        {
        //            ErrorModelStatus obj1 = new ErrorModelStatus();
        //            obj1.status = "Error";
        //            if (result.Result.Item1 == false && result.Result.Item2 == true && result.Result.Item3 == true)
        //            {
        //                obj1.errorMessage = "Old password is incorrect";
        //            }
        //            else if (result.Result.Item1 == false && result.Result.Item2 == true && result.Result.Item3 == false)
        //            {
        //                obj1.errorMessage = "User is not active";
        //            }
        //            else
        //            {
        //                obj1.errorMessage = "User not exist";
        //            }
        //            Context.Response.ContentType = "application/json";
        //            Context.Response.Write(s.Serialize(obj1));
        //        }
        //    }

        //}
        #endregion


        #region date time
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getServerDate()
        {


            DateTime currentdate = new CommonFunctions().ServerDate(); ;
            double todaysDate = ConvertToUnixTimestamp(currentdate);

            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            Context.Response.ContentType = "application/json";
            Context.Response.Write(s.Serialize(todaysDate.ToString()));
        }



        private double ConvertToUnixTimestamp(DateTime? date)
        {
            double returnvalue = 0;
            if (date != null)
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                TimeSpan diff = Convert.ToDateTime(date).ToUniversalTime() - origin;
                return Math.Floor(diff.TotalMilliseconds);
            }
            else return returnvalue;
        }

        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        static readonly double MaxUnixSeconds = (DateTime.MaxValue - UnixEpoch).TotalSeconds;
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            return unixTimeStamp > MaxUnixSeconds
               ? UnixEpoch.AddMilliseconds(unixTimeStamp)
               : UnixEpoch.AddSeconds(unixTimeStamp);
        }

        public DateTime? ConvertFromUnixTimestamp(double timestamp)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(timestamp);
        }

        public DateTime ConvertToLocalTimestamp(double timestamp)
        {
            DateTime localEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            double localSeconds = (DateTime.MaxValue - localEpoch).TotalSeconds;
            if(timestamp > localSeconds)
            {
                return localEpoch.AddMilliseconds(timestamp);
            }
            return localEpoch.AddSeconds(timestamp);
        }


        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public void getCurrentTime(string timeZoneName)
        //{
        //    var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);//("Pacific Standard Time");
        //    DateTime time2 = DateTime.UtcNow.ToTimeZoneTime(tz);

        //    JavaScriptSerializer s = new JavaScriptSerializer();
        //    s.MaxJsonLength = 2147483644;
        //    Context.Response.ContentType = "application/json";
        //    Context.Response.Write(s.Serialize(time2.ToString()));
        //}

        #endregion


        /// <summary>
        /// Date - 2016-10-03 Devloper Bharat
        /// Created web method for enhanced changes for offline data in mobile application
        /// returns current notification in jsonformat to mobile app.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentDate"></param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UserNotificationData(string userId, double currentDate)
        {
            long _userId = 0;
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            if (string.IsNullOrEmpty(userId))
            {
                setJsonReponse(1, "Fail", MemberInfoGetting.GetMemberName(() => userId));
            }
            else if (!long.TryParse(userId, out _userId))
            {
                setJsonReponse(2, "Fail", MemberInfoGetting.GetMemberName(() => userId));
            }
            else
            {

                try
                {
                    //var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);//("Pacific Standard Time");
                    //DateTime time2 = DateTime.UtcNow.ToTimeZoneTime(tz);

                    DateTime currentDate1 = DateTime.Parse(UnixTimeStampToDateTime(currentDate).ToString());
                    MobileOfflineDAL objMobile = new MobileOfflineDAL();


                    //get current avilable notification for the user.
                    var notificationData = objMobile.UserNotificationData(_userId, currentDate1);

                    if (notificationData != null && notificationData.Count > 0)
                    {
                        var stringInNotification = notificationData.Select(o => "Visit for Project - " + o.ProjectName.ToUpper()
                            + " for date range " + o.FromDate.ToString("MMM-dd-yyyy") + " to " + o.ToDate.ToString("MMM-dd-yyyy") + " going to expire."
                            + " Please, make sure all the information sent to server and all accounts are completed.").ToList();
                        var result = new { status = true, notificationInfo = stringInNotification };


                        Context.Response.ContentType = "application/json";
                        Context.Response.Write(s.Serialize(result));
                        objMobile.Dispose();
                    }
                    else
                    {
                        var result = new { status = false, notificationInfo = "" };
                        Context.Response.ContentType = "application/json";
                        Context.Response.Write(s.Serialize(result));
                    }
                }
                catch (Exception ex)
                {
                    log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                    log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                    log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                    setJsonReponse(4, "Fail", ex.Message);
                }

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_time"></param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void checkDate(string _time)
        {
            long _userId = 0;
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            try
            {

                DateTime clientsideProfileSyncStamp = DateTime.Parse(_time, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal);
                string[] subStr = _time.Split(' ');
                string[] strDate = subStr[0].ToString().Split('-');
                string[] strTimeSplit = subStr[1].Split(':');
                int _year = int.Parse(strDate[2]);
                int _Mon = DateTime.ParseExact(strDate[0], "MMM", CultureInfo.CurrentCulture).Month;
                int _Day = int.Parse(strDate[1]);

                int _hr = subStr[2] == "PM" ? (strTimeSplit[0] == "12" ? int.Parse(strTimeSplit[0]) : (12 + int.Parse(strTimeSplit[0]))) : int.Parse(strTimeSplit[0]);
                int _MN = int.Parse(strTimeSplit[1]);
                string str = _year + "," + _Mon + "," + _Day + "," + _hr + "," + _MN;
                DateTime VisitDatetime = new DateTime(_year, _Mon, _Day, _hr, _MN, 0);
                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(str + "|" + VisitDatetime.ToString() + "|" + clientsideProfileSyncStamp.ToString()));

            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
            }


        }



        /// <summary>
        /// Date 2016-10-06 Devloper Bharat Magdum
        /// method to save information submitted from mobile app.
        /// </summary>
        /// <param name="jsonDataParam"></param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveData(string jsonDataParam)
        {

            log.Info("WEB Service Save Started at - " + DateTime.Now.ToString());

            log.Info("WEB Service Save Input Json - " + jsonDataParam);
            //testing the web service of sending the field initial data for excel mapped data.
            //jsonDataParam = getjsonData1();
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            // long _userId = 0;
            ErrorModelStatus obj1 = new ErrorModelStatus();


            if (string.IsNullOrEmpty(jsonDataParam))
            {
                obj1.status = "Error";
                obj1.errorMessage = "Missing parameter jsonDataParam.";
                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(obj1));
            }
            else
            {
                try
                {

                    FormDataNew objFormData = new FormDataNew();
                    bool isJsonDataCorrect = false;
                    try
                    {
                        objFormData = s.Deserialize<FormDataNew>(jsonDataParam);
                        isJsonDataCorrect = true;
                    }
                    catch (Exception ex)
                    {
                        ErrorModelStatus obj2 = new ErrorModelStatus();
                        obj2.status = "Error";
                        obj2.errorMessage = ex.Message;
                        Context.Response.ContentType = "application/json";
                        Context.Response.Write(s.Serialize(obj2));

                    }

                    if (isJsonDataCorrect)
                    {
                        MobileDAL objMobile = new MobileDAL();
                        string outputString = "";

                        string googleKey = System.Configuration.ConfigurationManager.AppSettings["googleapiKey"].ToString();
                        //var result = objMobile.saveData(objFormData, jsonDataParam, out outputString);
                        var result = objMobile.saveDynamicData(objFormData, jsonDataParam, out outputString, googleKey);
                        if (result)
                        {
                            ErrorModelOnlyStatus obj3 = new ErrorModelOnlyStatus();
                            obj3.status = "Inserted";
                            Context.Response.ContentType = "application/json";
                            Context.Response.Write(s.Serialize(obj3));
                            objMobile.Dispose();
                        }
                        else
                        {
                            ErrorModelOnlyStatus obj3 = new ErrorModelOnlyStatus();
                            obj3.status = outputString;
                            Context.Response.ContentType = "application/json";
                            Context.Response.Write(s.Serialize(obj3));
                            objMobile.Dispose();
                        }
                    }

                    log.Info("WEB Service Save Ended at - " + DateTime.Now.ToString());
                }
                catch (Exception ex)
                {

                    log.Info("WEB Service Save ERROR at - " + DateTime.Now.ToString());
                    log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                    log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                    log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));

                    ErrorModelStatus obj2 = new ErrorModelStatus();
                    obj2.status = "Error";
                    obj2.errorMessage = ex.Message;
                    Context.Response.ContentType = "application/json";
                    Context.Response.Write(s.Serialize(obj2));

                }

                log.Info("WEB Service Save Method Exit - " + DateTime.Now.ToString());
            }
        }


        /// <summary>
        /// Date 2016-10-06 Devloper Bharat Magdum
        /// method to update media upload complete flag  submitted from mobile app.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="formId"></param>
        /// <param name="accountInitalFieldId"></param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UpdateMediaFlag(string userId, string formId, string accountInitalFieldId)
        {
            long _userId = 0; long _formId = 0; long _InitalFieldId = 0;
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            ErrorModelStatus obj1 = new ErrorModelStatus();
            if (string.IsNullOrEmpty(userId))
            {
                setJsonReponse(1, "Fail", MemberInfoGetting.GetMemberName(() => userId));
            }
            else if (!long.TryParse(userId, out _userId))
            {
                setJsonReponse(2, "Fail", MemberInfoGetting.GetMemberName(() => userId));
            }
            else if (string.IsNullOrEmpty(formId))
            {
                setJsonReponse(1, "Fail", MemberInfoGetting.GetMemberName(() => formId));
            }
            else if (!long.TryParse(formId, out _formId))
            {
                setJsonReponse(2, "Fail", MemberInfoGetting.GetMemberName(() => formId));
            }
            else if (string.IsNullOrEmpty(accountInitalFieldId))
            {
                setJsonReponse(1, "Fail", MemberInfoGetting.GetMemberName(() => accountInitalFieldId));
            }
            else if (!long.TryParse(accountInitalFieldId, out _InitalFieldId))
            {
                setJsonReponse(2, "Fail", MemberInfoGetting.GetMemberName(() => accountInitalFieldId));
            }
            else
            {

                try
                {
                    MobileDAL objMobile = new MobileDAL();
                    var result = objMobile.updateMediaFlag(_userId, _formId, _InitalFieldId);
                    if (result)
                    {
                        ErrorModelOnlyStatus obj3 = new ErrorModelOnlyStatus();
                        obj3.status = "Success";
                        Context.Response.ContentType = "application/json";
                        Context.Response.Write(s.Serialize(obj3));

                    }
                    else
                    {
                        ErrorModelOnlyStatus obj3 = new ErrorModelOnlyStatus();
                        obj3.status = "Fail";
                        Context.Response.ContentType = "application/json";
                        Context.Response.Write(s.Serialize(obj3));

                    }
                    objMobile.Dispose();
                }
                catch (Exception ex)
                {
                    setJsonReponse(4, "Fail", ex.Message);
                    log.Info("WEB Service  UpdateMediaFlag ERROR at " + DateTime.Now.ToString());
                    log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                    log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                    log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="processId"></param>
        /// <param name="currentDate"></param>
        /// <param name="batchNumber"></param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void OfflineUserDataAll(string userId, int processId, double currentDate, long batchNumber)
        {
            long _userId = 0;
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            if (string.IsNullOrEmpty(userId))
            {
                setJsonReponse(1, "Fail", MemberInfoGetting.GetMemberName(() => userId));
            }
            else if (!long.TryParse(userId, out _userId))
            {
                setJsonReponse(2, "Fail", MemberInfoGetting.GetMemberName(() => userId));
            }
            else if (batchNumber < 0)
            {
                setJsonReponse(2, "Fail", MemberInfoGetting.GetMemberName(() => userId));
            }
            else
            {

                try
                {
                    //var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);//("Pacific Standard Time");
                    //DateTime time2 = DateTime.UtcNow.ToTimeZoneTime(tz);
                    DateTime currDate = DateTime.Parse(UnixTimeStampToDateTime(currentDate).ToString()); 
                    DateTime currentDate1 = currDate.Date;

                    MobileOfflineDAL objMobile = new MobileOfflineDAL();

                    if (processId == 1)
                    {

                        //if (batchNumber == 0 || batchNumber == 1)
                        //{
                        //    AuditAllocationController objAuditAllocationController = new AuditAllocationController();
                        //    objAuditAllocationController.AllocateAuditVisit(_userId);
                        //}


                        var result = objMobile.OfflineData(_userId, currentDate1, batchNumber);
                        if (result != null)
                        {
                            Context.Response.ContentType = "application/json";
                            Context.Response.Write(s.Serialize(result));
                            objMobile.Dispose();
                        }
                        else
                        {
                            ErrorModelOnlyStatus obj = new ErrorModelOnlyStatus();
                            obj.status = "Wrong Batch Number";
                            Context.Response.ContentType = "application/json";
                            Context.Response.Write(s.Serialize(obj));
                        }
                    }
                    else if (processId == 2)
                    {
                        var result = objMobile.OfflineAuditData(_userId, currentDate1, batchNumber);
                        if (result != null)
                        {
                            Context.Response.ContentType = "application/json";
                            Context.Response.Write(s.Serialize(result));
                            objMobile.Dispose();
                        }
                        else
                        {
                            ErrorModelOnlyStatus obj = new ErrorModelOnlyStatus();
                            obj.status = "Wrong Batch Number";
                            Context.Response.ContentType = "application/json";
                            Context.Response.Write(s.Serialize(obj));
                        }
                    }
                }
                catch (Exception ex)
                {
                    setJsonReponse(4, "Fail", ex.Message);
                    log.Info("WEB Service  OfflineUserDataAll ERROR at " + DateTime.Now.ToString());
                    log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                    log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                    log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                }

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="batchNumber"></param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void setLastSynchFlag(string userId, int processId, int batchNumber)
        {

            log.Info("WEB Service  setLastSynchFlag Started at - " + DateTime.Now.ToString());
            long _userId = 0;
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            if (string.IsNullOrEmpty(userId))
            {
                setJsonReponse(1, "Fail", MemberInfoGetting.GetMemberName(() => userId));
            }
            else if (!long.TryParse(userId, out _userId))
            {
                setJsonReponse(2, "Fail", MemberInfoGetting.GetMemberName(() => userId));
            }
            else
            {

                try
                {
                    //var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);//("Pacific Standard Time");
                    //DateTime time2 = DateTime.UtcNow.ToTimeZoneTime(tz);
                    if (processId == 1)
                    {
                        MobileOfflineDAL objMobile = new MobileOfflineDAL();
                        var result = objMobile.setLastSynch(_userId, batchNumber);

                        ErrorModelOnlyStatus obj = new ErrorModelOnlyStatus();
                        obj.status = result.ToString();

                        Context.Response.ContentType = "application/json";
                        Context.Response.Write(s.Serialize(obj));
                        objMobile.Dispose();
                    }
                    else if (processId == 2)
                    {
                        MobileOfflineDAL objMobile = new MobileOfflineDAL();
                        var result = objMobile.setLastSynchAudit(_userId, batchNumber);

                        ErrorModelOnlyStatus obj = new ErrorModelOnlyStatus();
                        obj.status = result.ToString();

                        Context.Response.ContentType = "application/json";
                        Context.Response.Write(s.Serialize(obj));
                        objMobile.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    setJsonReponse(4, "Fail", ex.Message);
                    log.Info("WEB Service setLastSynchFlag ERROR at " + DateTime.Now.ToString());
                    log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                    log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                    log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                }

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="currentDate"></param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UserLogin(string userName, string password, double currentDate)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;



            if (string.IsNullOrEmpty(userName))
            {
                setJsonReponse(1, "Fail", MemberInfoGetting.GetMemberName(() => userName));
            }
            else if (string.IsNullOrEmpty(password))
            {
                setJsonReponse(1, "Fail", MemberInfoGetting.GetMemberName(() => password));
            }
            else
            {

                try
                {
                    long userId = 0; int isFirstLogin = 0;
                    bool statusActive = false; string message;
                    string uName;
                    bool result = new LoginDAL().Login(userName, password, out userId, out isFirstLogin, out statusActive, out message, out uName);
                    if (result == true)
                    {
                        isFirstLogin = 0;
                        if (statusActive)
                        {

                            DateTime currentDate1 = DateTime.Parse(UnixTimeStampToDateTime(currentDate).ToString());
                            MobileDAL objMobile = new MobileDAL();
                            var result1 = objMobile.checkProcessAccess(userId, currentDate1);

                            string RoleNames = GetUserRoles(int.Parse(userId.ToString()));

                            Context.Response.ContentType = "application/json";
                            Context.Response.Write(s.Serialize(new
                            {
                                status = "Success",
                                userId = Convert.ToString(userId),
                                isFirstLogin = Convert.ToString(isFirstLogin),
                                processes = result1,
                                roleId = RoleNames,
                                UserName = uName
                            }
                            ));

                            objMobile.Dispose();
                        }
                        else
                        {
                            setJsonReponse(4, "Fail", "User is not active.");
                        }

                    }
                    else
                    {
                        setJsonReponse(4, "Fail", message);
                    }
                }
                catch (Exception ex)
                {
                    setJsonReponse(4, "Fail", ex.Message);
                    log.Info("WEB Service  UserLogin ERROR at " + DateTime.Now.ToString());
                    log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                    log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                    log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                }
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public string GetUserRoles(int UserId)
        {
            try
            {

                string RoleNames = "";

                var list1 = new UserDAL().GetUserByID(UserId).FirstOrDefault();

                var manager = new ApplicationDbContext.UserManager();
                manager.UserValidator = new UserValidator<ApplicationUser>(manager) { AllowOnlyAlphanumericUserNames = false };


                CompassEntities et = new CompassEntities();
                var Roleresult = manager.FindById(list1.aspnet_UserId.ToString());
                var roleId = Roleresult.Roles.Select(a => a.RoleId).ToList();


                for (int i = 0; i < roleId.Count(); i++)
                {
                    var rols = et.AspNetRoles.ToList();

                    var droles = rols.Where(a => Convert.ToString(a.Id) == Convert.ToString(roleId[i])).FirstOrDefault();
                    if (droles != null)
                    {
                        if (droles.Name == "Installer" || droles.Name == "Manager" || droles.Name == "Field Supervisor" || droles.Name == "Auditor")
                        {
                            var tempNumber = droles.Name == "Installer" ? 1 : droles.Name == "Manager" ? 2 : droles.Name == "Field Supervisor" ? 3 : 4;
                            if (RoleNames == "")
                            {
                                RoleNames = tempNumber.ToString();//droles.Name;
                            }
                            else
                            {
                                RoleNames = RoleNames + "|" + tempNumber.ToString();//droles.Name;
                            }
                        }
                    }
                }


                return RoleNames;

            }
            catch (Exception ex)
            {
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                throw ex;
            }
        }

        /*[WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetUserSkippedStatus(string userid)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
        }*/

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public void GetInventorySourceInfo(string userId, long sourceNumber, double currentDate)
        //{
        //    JavaScriptSerializer s = new JavaScriptSerializer();
        //    s.MaxJsonLength = 2147483644;


            //    log.Info("WEB Service GetInventorySourceInfo Started at - " + DateTime.Now.ToString());
            //    long _userId = 0;


            //    if (string.IsNullOrEmpty(userId))
            //    {
            //        setJsonReponse(1, "Fail", MemberInfoGetting.GetMemberName(() => userId));
            //    }
            //    else if (!long.TryParse(userId, out _userId))
            //    {
            //        setJsonReponse(2, "Fail", MemberInfoGetting.GetMemberName(() => userId));
            //    }
            //    else
            //    {

            //        try
            //        {
            //            DateTime currentDate1 = DateTime.Parse(UnixTimeStampToDateTime(currentDate).ToString());

            //            if (sourceNumber == 1)
            //            {

            //                List<InventorySourceBO> objInventorySourceBOList = new List<InventorySourceBO>();
            //                InventoryWebServiceDAL objInventoryWebServiceDAL = new InventoryWebServiceDAL();
            //                objInventorySourceBOList = objInventoryWebServiceDAL.GetInventorySourceData(_userId, currentDate1);

            //                Context.Response.ContentType = "application/json";
            //                Context.Response.Write(s.Serialize(objInventorySourceBOList));
            //                objInventoryWebServiceDAL.Dispose();

            //            }
            //            else if (sourceNumber == 2)
            //            {

            //                InventoryNewRequestSourceBO objInventoryNewRequestSourceBO = new InventoryNewRequestSourceBO();
            //                InventoryWebServiceDAL objInventoryWebServiceDAL = new InventoryWebServiceDAL();
            //                //objInventoryNewRequestSourceBO = objInventoryWebServiceDAL.GetInventoryNewRequestSourceData(_userId, currentDate1);

            //                Context.Response.ContentType = "application/json";
            //                Context.Response.Write(s.Serialize(objInventoryNewRequestSourceBO));
            //                objInventoryWebServiceDAL.Dispose();

            //            }
            //            else if (sourceNumber == 3)
            //            {

            //                List<InventorySourceBO> objInventorySourceBOList = new List<InventorySourceBO>();
            //                InventoryWebServiceDAL objInventoryWebServiceDAL = new InventoryWebServiceDAL();
            //                objInventorySourceBOList = objInventoryWebServiceDAL.GetInventorySourceData(_userId, currentDate1);

            //                Context.Response.ContentType = "application/json";
            //                Context.Response.Write(s.Serialize(objInventorySourceBOList));
            //                objInventoryWebServiceDAL.Dispose();

            //            }
            //            else
            //            {
            //                setJsonReponse(4, "Fail", "Wrong Source Number");
            //            }



            //        }
            //        catch (Exception ex)
            //        {
            //            setJsonReponse(4, "Fail", ex.Message);
            //            log.Info("WEB Service  GetInventorySourceInfo ERROR at " + DateTime.Now.ToString());
            //            log.Info("WEB Service  GetInventorySourceInfo ERROR  ex.Message - " + ex.Message);

            //            log.Info("WEB Service  GetInventorySourceInfo ERROR  ex.InnerException.Message - " + (ex.InnerException != null ? Convert.ToString(ex.InnerException.Message) : ""));
            //        }

            //    }


            //}



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ValidateInventoryData(string jsonDataParam)
        {
            log.Info("WEB Service ValidateInventoryData Started at - " + DateTime.Now.ToString());

            log.Info("WEB Service ValidateInventoryData Input Json - " + jsonDataParam);

            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            ErrorModelStatus obj1 = new ErrorModelStatus();


            if (string.IsNullOrEmpty(jsonDataParam))
            {
                obj1.status = "Error";
                obj1.errorMessage = "Missing parameter jsonDataParam.";
                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(obj1));
            }
            else
            {
                try
                {

                    InwardInventoryBo objInwardInventoryBo = new InwardInventoryBo();
                    bool isJsonDataCorrect = false;
                    try
                    {
                        objInwardInventoryBo = s.Deserialize<InwardInventoryBo>(jsonDataParam);
                        isJsonDataCorrect = true;
                    }
                    catch (Exception ex)
                    {
                        log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                        log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                        log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                        ErrorModelStatus obj2 = new ErrorModelStatus();
                        obj2.status = "Error";
                        obj2.errorMessage = ex.Message;
                        Context.Response.ContentType = "application/json";
                        Context.Response.Write(s.Serialize(obj2));

                    }

                    if (isJsonDataCorrect)
                    {
                        DateTime currentDate1 = DateTime.Parse(UnixTimeStampToDateTime(objInwardInventoryBo.DeviceDateTimeOfRequest).ToString());

                        InventoryWebServiceDAL objInventoryWebServiceDAL = new InventoryWebServiceDAL();
                        string outputString = "";

                        //var result = objMobile.saveData(objFormData, jsonDataParam, out outputString);
                        var result = objInventoryWebServiceDAL.ValidateInventoryData(ref objInwardInventoryBo, jsonDataParam, currentDate1);

                        if (result)
                        {

                            Context.Response.ContentType = "application/json";
                            Context.Response.Write(s.Serialize(objInwardInventoryBo));
                            objInventoryWebServiceDAL.Dispose();
                        }
                        else
                        {
                            ErrorModelOnlyStatus obj3 = new ErrorModelOnlyStatus();
                            obj3.status = outputString;
                            Context.Response.ContentType = "application/json";
                            Context.Response.Write(s.Serialize(obj3));
                            objInventoryWebServiceDAL.Dispose();
                        }

                    }

                    log.Info("WEB Service Save Ended at - " + DateTime.Now.ToString());
                }
                catch (Exception ex)
                {

                    log.Info("WEB Service ValidateInventoryData ERROR at - " + DateTime.Now.ToString());
                    log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                    log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                    log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));

                    ErrorModelStatus obj2 = new ErrorModelStatus();
                    obj2.status = "Error";
                    obj2.errorMessage = ex.Message;
                    Context.Response.ContentType = "application/json";
                    Context.Response.Write(s.Serialize(obj2));

                }

                log.Info("WEB Service ValidateInventoryData Method Exit - " + DateTime.Now.ToString());
            }
        }



        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public void GetWarehouseUtilityList()
        //{
        //    JavaScriptSerializer s = new JavaScriptSerializer();
        //    s.MaxJsonLength = 2147483644;


        //    log.Info("WEB Service GetWarehouseUtilityList Started at - " + DateTime.Now.ToString());


        //    try
        //    {

        //        WebServiceProductDAL objWebServiceProductDAL = new WebServiceProductDAL();

        //        List<MobileWarehouseBO> ObjWarehouseList = new WareHouseDAL().GetWarehouseListNew();


        //        var ObjUtiltyList = objWebServiceProductDAL.GetUtilityTypeList();

        //        var obj = new { ObjWarehouseList, ObjUtiltyList };

        //        Context.Response.ContentType = "application/json";
        //        Context.Response.Write(s.Serialize(obj));
        //        objWebServiceProductDAL.Dispose();




        //    }
        //    catch (Exception ex)
        //    {
        //        setJsonReponse(4, "Fail", ex.Message);
        //        log.Info("WEB Service  GetWarehouseUtilityList ERROR at " + DateTime.Now.ToString());
        //        log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
        //        log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
        //        log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
        //    }
        //}


        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public void GetUtilityList()
        //{
        //    JavaScriptSerializer s = new JavaScriptSerializer();
        //    s.MaxJsonLength = 2147483644;


        //    log.Info("WEB Service GetUtilityList Started at - " + DateTime.Now.ToString());


        //    try
        //    {

        //        WebServiceProductDAL objWebServiceProductDAL = new WebServiceProductDAL();

        //        var ObjUtiltyList = objWebServiceProductDAL.GetUtilityTypeList();

        //        var obj = new { ObjUtiltyList };

        //        Context.Response.ContentType = "application/json";
        //        Context.Response.Write(s.Serialize(obj));
        //        objWebServiceProductDAL.Dispose();




        //    }
        //    catch (Exception ex)
        //    {
        //        setJsonReponse(4, "Fail", ex.Message);
        //        log.Info("WEB Service  GetUtilityList ERROR at " + DateTime.Now.ToString());
        //        log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
        //        log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
        //        log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
        //    }
        //}

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public void GetProductList(long utilityId)
        //{
        //    JavaScriptSerializer s = new JavaScriptSerializer();
        //    s.MaxJsonLength = 2147483644;


        //    log.Info("WEB Service GetProductList Started at - " + DateTime.Now.ToString());

        //    if (utilityId < 1 || utilityId > 3)
        //    {

        //        setJsonReponse(4, "Fail", "Invalid input.");
        //    }
        //    else
        //    {
        //        try
        //        {


        //            List<MobileProductBO> objMobileProductBOList = new List<MobileProductBO>();

        //            WebServiceProductDAL objWebServiceProductDAL = new WebServiceProductDAL();

        //            objMobileProductBOList = objWebServiceProductDAL.GetProductList(utilityId);

        //            foreach (var item in objMobileProductBOList)
        //            {
        //                item.ProductName = item.ProductName + "-" + item.Make + (string.IsNullOrEmpty(item.Type) ? "" : ("-" + item.Type))
        //                    + (string.IsNullOrEmpty(item.Size) ? "" : ("-" + item.Size));
        //            }

        //            Context.Response.ContentType = "application/json";
        //            Context.Response.Write(s.Serialize(objMobileProductBOList));
        //            objWebServiceProductDAL.Dispose();




        //        }
        //        catch (Exception ex)
        //        {
        //            setJsonReponse(4, "Fail", ex.Message);
        //            log.Info("WEB Service  GetProductList ERROR at " + DateTime.Now.ToString());
        //            log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
        //            log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
        //            log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
        //        }
        //    }
        //}


        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public void AddNewProductSourceData(long utilityId)
        //{
        //    JavaScriptSerializer s = new JavaScriptSerializer();
        //    s.MaxJsonLength = 2147483644;


        //    log.Info("WEB Service AddNewProductSourceData Started at - " + DateTime.Now.ToString());

        //    if (utilityId < 1 || utilityId > 3)
        //    {

        //        setJsonReponse(4, "Fail", "Invalid input.");
        //    }
        //    else
        //    {
        //        try
        //        {




        //            WebServiceProductDAL objWebServiceProductDAL = new WebServiceProductDAL();

        //            WebServiceProductBO ObjWebServiceProductBO = objWebServiceProductDAL.GetAllSourceInfoForProduct(utilityId);

        //            Context.Response.ContentType = "application/json";
        //            Context.Response.Write(s.Serialize(ObjWebServiceProductBO));
        //            objWebServiceProductDAL.Dispose();




        //        }
        //        catch (Exception ex)
        //        {
        //            setJsonReponse(4, "Fail", ex.Message);
        //            log.Info("WEB Service  AddNewProductSourceData ERROR at " + DateTime.Now.ToString());
        //            log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
        //            log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
        //            log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
        //        }
        //    }
        //}


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void AddNewProductSaveData(string jsonDataParam)
        {
            //
            log.Info("WEB Service AddNewProductSaveData Started at - " + DateTime.Now.ToString());

            log.Info("WEB Service AddNewProductSaveData Input Json - " + jsonDataParam);

            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            ErrorModelStatus obj1 = new ErrorModelStatus();


            if (string.IsNullOrEmpty(jsonDataParam))
            {
                setJsonReponse(4, "Error", "Missing parameter jsonDataParam.");
            }
            else
            {
                try
                {

                    WebServiceProductReceiveBO objWebServiceProductReceiveBO = new WebServiceProductReceiveBO();
                    bool isJsonDataCorrect = false;
                    try
                    {
                        objWebServiceProductReceiveBO = s.Deserialize<WebServiceProductReceiveBO>(jsonDataParam);
                        isJsonDataCorrect = true;



                        if (string.IsNullOrEmpty(objWebServiceProductReceiveBO.ProductName))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing ProductName field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(Convert.ToString(objWebServiceProductReceiveBO.CategoryId)) || objWebServiceProductReceiveBO.CategoryId == 0)
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing CategoryId field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(Convert.ToString(objWebServiceProductReceiveBO.MakeId)) || objWebServiceProductReceiveBO.MakeId == 0)
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing MakeId field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(objWebServiceProductReceiveBO.PartNumber))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing PartNumber field in request string");
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        setJsonReponse(4, "Error", "Json submitted is not in correct format.");
                        log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                        log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                        log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                    }

                    if (isJsonDataCorrect)
                    {

                        StatusMessage statusMessage = new StatusMessage();
                        InventoryDAL objInventoryDAL = new InventoryDAL();
                        long utilityId = objInventoryDAL.GetUtilityForProject(objWebServiceProductReceiveBO.ProjectId);
                        ProductMasterBO objProductMasterBO =
                            new ProductMasterBO
                            {
                                //UtilityId = objWebServiceProductReceiveBO.UtilityId,
                                UtilityId = utilityId,
                                CategoryId = objWebServiceProductReceiveBO.CategoryId,
                                MeterMakeId = objWebServiceProductReceiveBO.MakeId,
                                MeterTypeId = objWebServiceProductReceiveBO.TypeId,
                                MeterSizeId = objWebServiceProductReceiveBO.SizeId,
                                ProductName = objWebServiceProductReceiveBO.ProductName,
                                ProductPartNumber = objWebServiceProductReceiveBO.PartNumber,
                                ProductUPCCode = objWebServiceProductReceiveBO.UPCCode,
                                ProductDescription = objWebServiceProductReceiveBO.ProductDescription,
                                GenerateBarcode = objWebServiceProductReceiveBO.GenerateBarcode,
                                CurrentUserId = objWebServiceProductReceiveBO.UserId,
                                AlertTypeEmail = objWebServiceProductReceiveBO.AlertTypeEmail,
                                WarehouseMinimumStock = objWebServiceProductReceiveBO.WarehouseMinimumStock,
                                UserMinimumStock = objWebServiceProductReceiveBO.UserMinimumStock,
                                ManagerList = objWebServiceProductReceiveBO.ManagerList

                            };
                        InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();

                        long productId = 0;
                        statusMessage = objInventoryMasterDAL.CreateOrUpdateProduct(objProductMasterBO, ref productId);


                        if (statusMessage.Success == true)
                        {
                            //setJsonReponse(3, "Success", "");
                            Context.Response.ContentType = "application/json";
                            Context.Response.Write(s.Serialize(new { status = "Success", productId = productId }));


                        }
                        else
                        {
                            setJsonReponse(4, "Error", statusMessage.Message);
                        }

                    }


                    log.Info("WEB Service AddNewProductSaveData Ended at - " + DateTime.Now.ToString());
                }
                catch (Exception ex)
                {

                    log.Info("WEB Service AddNewProductSaveData ERROR at - " + DateTime.Now.ToString());
                    log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                    log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                    log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));

                    setJsonReponse(4, "Error", ex.Message);
                }

                log.Info("WEB Service AddNewProductSaveData Method Exit - " + DateTime.Now.ToString());
            }
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ReceiveWarehouseInventory(string jsonDataParam)
        {
            log.Info("WEB Service ReceiveWarehouseInventory Started at - " + DateTime.Now.ToString());

            log.Info("WEB Service ReceiveWarehouseInventory Input Json - " + jsonDataParam);

            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            ErrorModelStatus obj1 = new ErrorModelStatus();


            if (string.IsNullOrEmpty(jsonDataParam))
            {
                setJsonReponse(4, "Error", "Missing parameter jsonDataParam.");
            }
            else
            {
                try
                {

                    MobileWarehouseReceiveBo objMobileWarehouseRecieveBo = new MobileWarehouseReceiveBo();
                    bool isJsonDataCorrect = false;
                    try
                    {
                        objMobileWarehouseRecieveBo = s.Deserialize<MobileWarehouseReceiveBo>(jsonDataParam);
                        isJsonDataCorrect = true;

                        if (objMobileWarehouseRecieveBo.SerialNumberList != null && objMobileWarehouseRecieveBo.SerialNumberList.Count > 0)
                        {
                            var checkProductIdNotPresent = objMobileWarehouseRecieveBo.SerialNumberList.Where(o => o.ProductId == 0).FirstOrDefault();
                            if (checkProductIdNotPresent != null)
                            {
                                isJsonDataCorrect = false;
                                setJsonReponse(4, "Error", "SerialNumberList does not hold productid");

                                return;
                            }
                        }

                        if (objMobileWarehouseRecieveBo.ProductQuantityList != null && objMobileWarehouseRecieveBo.ProductQuantityList.Count > 0)
                        {
                            var checkProductIdNotPresent = objMobileWarehouseRecieveBo.ProductQuantityList.
                                Where(o => o.ProductId == 0 || o.ProductQuantity == 0).FirstOrDefault();
                            if (checkProductIdNotPresent != null)
                            {
                                isJsonDataCorrect = false;
                                setJsonReponse(4, "Error", "ProductQuantityList does not hold productid or productquantity is 0");
                                return;
                            }
                        }

                        if (string.IsNullOrEmpty(objMobileWarehouseRecieveBo.DeviceIp))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing device ip field in request string");
                            return;
                        }

                        if (string.IsNullOrEmpty(objMobileWarehouseRecieveBo.GPSLocation))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing GPS Location field in request string");
                            return;
                        }

                        if (string.IsNullOrEmpty(Convert.ToString(objMobileWarehouseRecieveBo.ProjectId)))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing ProjectId field in request string");
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        setJsonReponse(4, "Error", "Json submitted is not in correct format.");
                        log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                        log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                        log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                    }

                    if (isJsonDataCorrect)
                    {
                        DateTime currentDate1 = DateTime.Parse(UnixTimeStampToDateTime(objMobileWarehouseRecieveBo.DeviceDateTimeOfRequest).ToString());

                        InventoryWebServiceDAL objInventoryWebServiceDAL = new InventoryWebServiceDAL();

                        MobileWarehouseResponseBo objMobileWarehouseResponseBo;
                        //log.Info("STEP 1 - Before WarehouseInsertMobileData Call");
                        string message;
                        var result = objInventoryWebServiceDAL.WarehouseInsertMobileData(ref objMobileWarehouseRecieveBo, jsonDataParam, currentDate1, out objMobileWarehouseResponseBo, out message);
                        //log.Info("STEP 2 - After WarehouseInsertMobileData Call");
                        objInventoryWebServiceDAL.Dispose();
                        //log.Info("STEP 3 - After WarehouseInsertMobileData Call");


                        if (result)
                        {
                            Context.Response.ContentType = "application/json";
                            Context.Response.Write(s.Serialize(objMobileWarehouseResponseBo));
                        }
                        else
                        {
                            setJsonReponse(4, "Error", message);
                        }

                    }


                    log.Info("WEB Service Save Ended at - " + DateTime.Now.ToString());
                }
                catch (Exception ex)
                {

                    log.Info("WEB Service ReceiveWarehouseInventory ERROR at - " + DateTime.Now.ToString());
                    log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                    log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                    log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));

                    setJsonReponse(4, "Error", ex.Message);
                }

                log.Info("WEB Service ReceiveWarehouseInventory Method Exit - " + DateTime.Now.ToString());
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ReceiveInventorySourceInfo(long userId, long projectId)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;


            log.Info("WEB Service GetInventorySourceInfo Started at - " + DateTime.Now.ToString());


            try
            {


                List<InventorySourceBO> ObjInventorySourceBOList = new List<InventorySourceBO>();
                InventoryWebServiceDAL objInventoryWebServiceDAL = new InventoryWebServiceDAL();
                WebServiceProductDAL objWebServiceProductDAL = new WebServiceProductDAL();
                string userRoles = GetUserRoles(int.Parse(userId.ToString()));

                ObjInventorySourceBOList = objInventoryWebServiceDAL.GetInventorySourceData(projectId, userId, userRoles);
                //var ObjUtiltyList = new WebServiceProductDAL().GetUtilityTypeList();
                List<MobileProductBO> ProductList = objWebServiceProductDAL.GetProductList(projectId);
                if (ProductList != null)
                {
                    ProductList = ProductList.Where(o => o.ProductHasSerialNumber == false).ToList();
                }

                var obj = new { DropDownList = ObjInventorySourceBOList, ProductList = ProductList };


                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(obj));
                objInventoryWebServiceDAL.Dispose();


            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  GetInventorySourceInfo ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ReceiveUserInventory(string jsonDataParam)
        {
            log.Info("WEB Service ReceiveUserInventory Started at - " + DateTime.Now.ToString());

            log.Info("WEB Service ReceiveUserInventory Input Json - " + jsonDataParam);

            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            ErrorModelStatus obj1 = new ErrorModelStatus();


            if (string.IsNullOrEmpty(jsonDataParam))
            {
                setJsonReponse(4, "Error", "Missing parameter jsonDataParam.");
            }
            else
            {
                try
                {

                    MobileUserReceiveBo objMobileUserReceiveBo = new MobileUserReceiveBo();
                    bool isJsonDataCorrect = false;
                    try
                    {
                        objMobileUserReceiveBo = s.Deserialize<MobileUserReceiveBo>(jsonDataParam);
                        isJsonDataCorrect = true;

                        if (string.IsNullOrEmpty(objMobileUserReceiveBo.DeviceIp))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing device ip field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(objMobileUserReceiveBo.GPSLocation))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing GPS Location field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(Convert.ToString(objMobileUserReceiveBo.ProjectId)))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing ProjectId field in request string");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        setJsonReponse(4, "Error", "Json submitted is not in correct format. - " + ex.Message);
                        log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                        log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                        log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                    }

                    if (isJsonDataCorrect)
                    {
                        //check first warehouseid present for user recievie from warehouse to enter product quantity information
                        // if product quantity list present then warehouseid required  & if not present warehouse id not required
                        //reject if product quantity present & warehouse id not present.

                        DateTime currentDate1 = DateTime.Parse(UnixTimeStampToDateTime(objMobileUserReceiveBo.DeviceDateTimeOfRequest).ToString());

                        InventoryWebServiceDAL objInventoryWebServiceDAL = new InventoryWebServiceDAL();

                        string message;

                        bool warehouseData = false;




                        if (objMobileUserReceiveBo.ProductQuantityList != null && objMobileUserReceiveBo.ProductQuantityList.Count > 0)
                        {
                            if (objMobileUserReceiveBo.ProductQuantityList[0].ReceiveFromId == 1)
                            {
                                warehouseData = true;
                            }
                        }

                        var result = objInventoryWebServiceDAL.ManageStock(ref objMobileUserReceiveBo, jsonDataParam, currentDate1, out message);

                        objInventoryWebServiceDAL.Dispose();


                        if (result)
                        {
                            Context.Response.ContentType = "application/json";
                            Context.Response.Write(s.Serialize(objMobileUserReceiveBo));
                        }
                        else
                        {
                            setJsonReponse(4, "Error", message);
                        }



                    }

                    log.Info("WEB Service Save Ended at - " + DateTime.Now.ToString());
                }
                catch (Exception ex)
                {

                    log.Info("WEB Service ReceiveUserInventory ERROR at - " + DateTime.Now.ToString());
                    log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                    log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                    log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));

                    setJsonReponse(4, "Error", ex.Message);
                }

                log.Info("WEB Service ReceiveUserInventory Method Exit - " + DateTime.Now.ToString());
            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void RequestNewInventorySourceInfo(long userId, long projectId)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            log.Info("WEB Service RequestNewInventorySourceInfo Started at - " + DateTime.Now.ToString());

            try
            {
                List<InventorySourceBO> ObjInventorySourceBOList = new List<InventorySourceBO>();
                WebServiceProductDAL objWebServiceProductDAL = new WebServiceProductDAL();
                WebServiceRequestStockDal objWebServiceRequestStockDal = new WebServiceRequestStockDal();
                string userRoles = GetUserRoles(int.Parse(userId.ToString()));

                ObjInventorySourceBOList = objWebServiceRequestStockDal.GetRequestNewInventorySourceData(userId, projectId, userRoles);
                //var ObjUtiltyList = new WebServiceProductDAL().GetUtilityTypeList();
                List<MobileProductBO> ProductList = objWebServiceProductDAL.GetProductList(projectId);

                var obj = new { ObjInventorySourceBOList, ProductList };

                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(obj));
                objWebServiceRequestStockDal.Dispose();

            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  RequestNewInventorySourceInfo ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SubmitNewInventoryRequest(string jsonDataParam)
        {

            //
            log.Info("WEB Service SubmitNewInventoryRequest Started at - " + DateTime.Now.ToString());

            log.Info("WEB Service SubmitNewInventoryRequest Input Json - " + jsonDataParam);

            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            ErrorModelStatus obj1 = new ErrorModelStatus();


            if (string.IsNullOrEmpty(jsonDataParam))
            {
                setJsonReponse(4, "Error", "Missing parameter jsonDataParam.");
            }
            else
            {
                try
                {

                    MobileStockRequest objMobileStockRequest = new MobileStockRequest();
                    bool isJsonDataCorrect = false;
                    try
                    {
                        objMobileStockRequest = s.Deserialize<MobileStockRequest>(jsonDataParam);
                        isJsonDataCorrect = true;

                        if (objMobileStockRequest != null && objMobileStockRequest.ProductRequestList.Count > 0)
                        {
                            var checkProductIdNotPresent = objMobileStockRequest.ProductRequestList.Where(o => o.ProductId == 0).FirstOrDefault();
                            if (checkProductIdNotPresent != null)
                            {
                                isJsonDataCorrect = false;
                                setJsonReponse(4, "Error", "ProductRequestList does not hold productid");

                                return;
                            }

                        }


                        if (string.IsNullOrEmpty(objMobileStockRequest.DeviceIPAddress))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing device ip field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(objMobileStockRequest.DeviceGpsLocation))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing GPS Location field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(Convert.ToString(objMobileStockRequest.UserId)))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing UserId field in request string");
                            return;
                        }

                        if (string.IsNullOrEmpty(Convert.ToString(objMobileStockRequest.ProjectId)) || objMobileStockRequest.ProjectId == 0)
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing ProjectId field in request string");
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        setJsonReponse(4, "Error", "Json submitted is not in correct format.");
                    }

                    if (isJsonDataCorrect)
                    {
                        DateTime currentDate1 = DateTime.Parse(UnixTimeStampToDateTime(objMobileStockRequest.DeviceRequestDateTime).ToString());

                        WebServiceRequestStockDal objWebServiceRequestStockDal = new WebServiceRequestStockDal();

                        //MobileWarehouseResponseBo objMobileWarehouseResponseBo;

                        string message;
                        var result = objWebServiceRequestStockDal.SaveStockNewRequest(ref objMobileStockRequest, jsonDataParam, currentDate1, out message);

                        objWebServiceRequestStockDal.Dispose();


                        if (result)
                        {
                            setJsonReponse(3, "Success", message);
                        }
                        else
                        {
                            setJsonReponse(4, "Error", message);
                        }

                    }


                    log.Info("WEB Service Save Ended at - " + DateTime.Now.ToString());
                }
                catch (Exception ex)
                {

                    log.Info("WEB Service ReceiveWarehouseInventory ERROR at - " + DateTime.Now.ToString());
                    log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                    log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                    log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));

                    setJsonReponse(4, "Error", ex.Message);
                }

                log.Info("WEB Service ReceiveWarehouseInventory Method Exit - " + DateTime.Now.ToString());
            }



        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ViewNewInventoryRequestSourceData(long UserId, long projectId)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;


            log.Info("WEB Service ViewNewInventoryRequestFieldData Started at - " + DateTime.Now.ToString());


            try
            {


                List<InventorySourceBO> ObjInventorySourceBOList = new List<InventorySourceBO>();
                WebServiceRequestStockDal objWebServiceRequestStockDal = new WebServiceRequestStockDal();


                string userRole = GetUserRoles(int.Parse(UserId.ToString()));

                ObjInventorySourceBOList = objWebServiceRequestStockDal.GetStockRequestSourceData(UserId, userRole, projectId);


                var obj = new { ObjInventorySourceBOList };

                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(obj));
                objWebServiceRequestStockDal.Dispose();


            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  ViewNewInventoryRequestFieldData ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }


        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ViewNewInventoryRequest(string jsonDataParam)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;


            log.Info("WEB Service ViewNewInventoryRequestFieldData Started at - " + DateTime.Now.ToString());

            log.Info("WEB Service ViewNewInventoryRequestFieldData Input json - " + jsonDataParam);

            try
            {
                if (string.IsNullOrEmpty(jsonDataParam))
                {
                    setJsonReponse(4, "Error", "Missing parameter jsonDataParam.");
                }
                else
                {

                    MobileViewRequestBo objMobileViewRequestBo = new MobileViewRequestBo();
                    bool isJsonDataCorrect = false;
                    try
                    {
                        objMobileViewRequestBo = s.Deserialize<MobileViewRequestBo>(jsonDataParam);
                        isJsonDataCorrect = true;


                        if (string.IsNullOrEmpty(objMobileViewRequestBo.FromDate))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing FromDate field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(objMobileViewRequestBo.Todate))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing Todate field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(Convert.ToString(objMobileViewRequestBo.Id)))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing Id field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(Convert.ToString(objMobileViewRequestBo.UserId)))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing UserId field in request string");
                            return;
                        }

                        if (string.IsNullOrEmpty(Convert.ToString(objMobileViewRequestBo.ProjectId)))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing ProjectId field in request string");
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        setJsonReponse(4, "Error", "Json submitted is not in correct format.");
                        log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                        log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                        log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                    }

                    if (isJsonDataCorrect)
                    {
                        List<MobileViewResponseBo> viewRequestList = new List<MobileViewResponseBo>();
                        WebServiceRequestStockDal objWebServiceRequestStockDal = new WebServiceRequestStockDal();

                        string RoleNames = GetUserRoles(int.Parse(objMobileViewRequestBo.UserId.ToString()));

                        viewRequestList = objWebServiceRequestStockDal.GetRequestList(objMobileViewRequestBo, RoleNames);

                        Context.Response.ContentType = "application/json";
                        Context.Response.Write(s.Serialize(viewRequestList));
                        objWebServiceRequestStockDal.Dispose();
                    }

                }


            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  ViewNewInventoryRequestFieldData ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ViewDetailedRequest(long RequestId)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;


            log.Info("WEB Service ViewDetailedRequest Started at - " + DateTime.Now.ToString());

            log.Info("WEB Service ViewDetailedRequest Input  - " + RequestId.ToString());

            try
            {
             
                if (RequestId == 0)
                {
                    setJsonReponse(4, "Error", "Invalid input.");
                }
                else
                {
                    WebServiceRequestStockDal objWebServiceRequestStockDal = new WebServiceRequestStockDal();
                    var viewRequestList = objWebServiceRequestStockDal.GetRequestDetails(RequestId);
                    Context.Response.ContentType = "application/json";
                    Context.Response.Write(s.Serialize(viewRequestList));
                    objWebServiceRequestStockDal.Dispose();
                }


            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  ViewDetailedRequest ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UpdateNewRequest(string jsonDataParam)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;


            log.Info("WEB Service UpdateNewRequest Started at - " + DateTime.Now.ToString());

            log.Info("WEB Service UpdateNewRequest Input  - " + jsonDataParam);

            try
            {

                if (string.IsNullOrEmpty(jsonDataParam))
                {
                    setJsonReponse(4, "Error", "Missing parameter jsonDataParam.");
                }
                else
                {


                    MobileUpdateStockRequestBO objMobileUpdateStockRequestBO = new MobileUpdateStockRequestBO();
                    bool isJsonDataCorrect = false;
                    try
                    {
                        objMobileUpdateStockRequestBO = s.Deserialize<MobileUpdateStockRequestBO>(jsonDataParam);
                        isJsonDataCorrect = true;


                        if (string.IsNullOrEmpty(objMobileUpdateStockRequestBO.approvedDatetime.ToString()))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing approvedDatetime field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(objMobileUpdateStockRequestBO.status))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing Status field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(Convert.ToString(objMobileUpdateStockRequestBO.id)))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing Id field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(Convert.ToString(objMobileUpdateStockRequestBO.userId)))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing userId field in request string");
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        setJsonReponse(4, "Error", "Json submitted is not in correct format.");
                    }

                    if (isJsonDataCorrect)
                    {
                        DateTime currentDate1 = DateTime.Parse(UnixTimeStampToDateTime(objMobileUpdateStockRequestBO.approvedDatetime).ToString());

                        WebServiceRequestStockDal objWebServiceRequestStockDal = new WebServiceRequestStockDal();

                        string message = "";
                        var result = objWebServiceRequestStockDal.UpdateNewRequest(objMobileUpdateStockRequestBO, currentDate1, out message);

                        objWebServiceRequestStockDal.Dispose();


                        if (result)
                        {
                            setJsonReponse(3, "Success", message);
                        }
                        else
                        {
                            setJsonReponse(4, "Error", message);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  UpdateNewRequest ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ViewRunningInventory(long UserId, long ProjectId, string FromDate, string Todate)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            log.Info("WEB Service ViewRunningInventory Started at - " + DateTime.Now.ToString());

            try
            {
                if (UserId == 0)
                {
                    setJsonReponse(4, "Error", "Missing UserId field in request string.");
                }
                else if (string.IsNullOrEmpty(FromDate))
                {

                    setJsonReponse(4, "Error", "Missing FromDate field in request string");
                    return;
                }
                else if (string.IsNullOrEmpty(Todate))
                {

                    setJsonReponse(4, "Error", "Missing Todate field in request string");
                    return;
                }
                else
                {
                    WebServiceRunningInventoryDAL objWebServiceRunningInventoryDAL = new WebServiceRunningInventoryDAL();
                    var viewRequestList = objWebServiceRunningInventoryDAL.GetRunningInventory(UserId, ProjectId, FromDate, Todate);
                    Context.Response.ContentType = "application/json";
                    Context.Response.Write(s.Serialize(viewRequestList));
                    objWebServiceRunningInventoryDAL.Dispose();
                }


            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  ViewRunningInventory ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ViewBalanceDetailInventory(long UserId, long ProjectId, string FromDate, string Todate)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            log.Info("WEB Service ViewRunningInventory Started at - " + DateTime.Now.ToString());

            try
            {
                if (UserId == 0)
                {
                    setJsonReponse(4, "Error", "Missing UserId field in request string.");
                }
                else if (string.IsNullOrEmpty(FromDate))
                {

                    setJsonReponse(4, "Error", "Missing FromDate field in request string");
                    return;
                }
                else if (string.IsNullOrEmpty(Todate))
                {

                    setJsonReponse(4, "Error", "Missing Todate field in request string");
                    return;
                }
                else
                {
                    /*WebServiceRunningInventoryDAL objWebServiceRunningInventoryDAL = new WebServiceRunningInventoryDAL();
                    var viewRequestList = objWebServiceRunningInventoryDAL.GetRunningInventory(UserId, ProjectId, FromDate, Todate);
                    Context.Response.ContentType = "application/json";
                    Context.Response.Write(s.Serialize(viewRequestList));
                    objWebServiceRunningInventoryDAL.Dispose();*/
                    List<PROC_InstallStockDetailReport_Result> ReportData = new List<PROC_InstallStockDetailReport_Result>();
                    //List<InstallerStockReportSummaryBO> SummeryReportData = new List<InstallerStockReportSummaryBO>();
                    List<BalanceDetail> inventoryDetail = new List<BalanceDetail>();
                    InventoryDAL inventoryDal = new InventoryDAL();
                    //SummeryReportData = inventoryDal.GetInventryStockReportSummery(ProjectId, UserId, 0, FromDate, Todate);
                    ReportData = inventoryDal.GetInventryStockReport(ProjectId, UserId, 0, FromDate, Todate);
                    List<long> productList = new List<long>();
                    inventoryDetail = ReportData.Where(o => o.InHandQty > 0).GroupBy(o => new { o.FKProductId, o.ProductName }).Select(o => new BalanceDetail { ProductName = o.Key.ProductName, Quantity = o.Sum(x => x.InHandQty )}).ToList();
                    /*foreach(PROC_InstallStockDetailReport_Result stock in ReportData)
                    {
                        
                        //stock.FKProductId
                    }
                    foreach (InstallerStockReportSummaryBO summaryInfo in SummeryReportData)
                    {
                        if(summaryInfo.InHandInventory > 0)
                        {
                            BalanceDetail detail = new BalanceDetail();
                            detail.ProductName = summaryInfo.ProductName;
                            detail.Quantity = summaryInfo.InHandInventory;
                            inventoryDetail.Add(detail);
                        }
                        
                    }*/
                    Context.Response.ContentType = "application/json";
                    Context.Response.Write(s.Serialize(inventoryDetail));
                    inventoryDal.Dispose();
                }


            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  ViewRunningInventory ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UserSignOffRequest(string jsonDataParam)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;


            log.Info("WEB Service UserSignOffRequest Started at - " + DateTime.Now.ToString());



            try
            {
                if (string.IsNullOrEmpty(jsonDataParam))
                {
                    setJsonReponse(4, "Error", "Missing parameter jsonDataParam.");
                }
                else
                {

                    MobileUserSignOffBO objMobileUserSignOffBO = new MobileUserSignOffBO();
                    bool isJsonDataCorrect = false;
                    try
                    {
                        objMobileUserSignOffBO = s.Deserialize<MobileUserSignOffBO>(jsonDataParam);
                        isJsonDataCorrect = true;


                        if (string.IsNullOrEmpty(objMobileUserSignOffBO.DeviceGpsLocation))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing DeviceGpsLocation field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(objMobileUserSignOffBO.DeviceIPAddress))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing DeviceIPAddress field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(Convert.ToString(objMobileUserSignOffBO.DeviceRequestDateTime)))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing DeviceRequestDateTime field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(Convert.ToString(objMobileUserSignOffBO.userId)))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing UserId field in request string");
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        setJsonReponse(4, "Error", "Json submitted is not in correct format.");
                    }

                    if (isJsonDataCorrect)
                    {

                        WebServiceRunningInventoryDAL objWebServiceRunningInventoryDAL = new WebServiceRunningInventoryDAL();

                        string message;

                        DateTime currentDate1 = DateTime.Parse(UnixTimeStampToDateTime(objMobileUserSignOffBO.DeviceRequestDateTime).ToString());

                        bool result = objWebServiceRunningInventoryDAL.MobileSignOffRequest(objMobileUserSignOffBO, currentDate1, jsonDataParam, out message);

                        objWebServiceRunningInventoryDAL.Dispose();
                        if (result)
                        {
                            setJsonReponse(3, "Success", message);
                        }
                        else
                        {
                            setJsonReponse(4, "Error", message);
                        }

                    }

                }


            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  UserSignOffRequest ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }




        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ReturnInventorySourceData(long UserId, long ProjectId)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            log.Info("WEB Service ReturnInventorySourceData Started at - " + DateTime.Now.ToString());
            try
            {
                if (UserId == 0)
                {
                    setJsonReponse(4, "Error", "Invalid input.");
                }
                else
                {
                    WebServiceReturnInventoryDAL objWebServiceReturnInventoryDAL = new WebServiceReturnInventoryDAL();

                    //string userRoles = GetUserRoles(int.Parse(UserId.ToString()));

                    var ObjInventorySourceBOList = objWebServiceReturnInventoryDAL.GetReturnInventorySourceData(UserId, ProjectId);

                    // var ObjUtiltyList = new WebServiceProductDAL().GetUtilityTypeList();
                    var obj = new { ObjInventorySourceBOList };

                    Context.Response.ContentType = "application/json";
                    Context.Response.Write(s.Serialize(obj));
                    objWebServiceReturnInventoryDAL.Dispose();
                }
            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  ReturnInventorySourceData ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ReturnInventorySubmitData(string jsonDataParam)
        {

            //
            log.Info("WEB Service ReturnInventorySubmitData Started at - " + DateTime.Now.ToString());

            log.Info("WEB Service ReturnInventorySubmitData Input Json - " + jsonDataParam);

            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            ErrorModelStatus obj1 = new ErrorModelStatus();


            if (string.IsNullOrEmpty(jsonDataParam))
            {
                setJsonReponse(4, "Error", "Missing parameter jsonDataParam.");
            }
            else
            {
                try
                {

                    MobileReturnStockBO objMobileReturnStockBO = new MobileReturnStockBO();
                    bool isJsonDataCorrect = false;
                    try
                    {
                        objMobileReturnStockBO = s.Deserialize<MobileReturnStockBO>(jsonDataParam);
                        isJsonDataCorrect = true;



                        if (string.IsNullOrEmpty(Convert.ToString(objMobileReturnStockBO.DeviceRequestDateTime)))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing DeviceRequestDateTime field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(objMobileReturnStockBO.DeviceIPAddress))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing device ip field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(objMobileReturnStockBO.DeviceGpsLocation))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing GPS Location field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(Convert.ToString(objMobileReturnStockBO.UserId)))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing UserId field in request string");
                            return;
                        }
                        if (string.IsNullOrEmpty(Convert.ToString(objMobileReturnStockBO.ProjectId)))
                        {
                            isJsonDataCorrect = false;
                            setJsonReponse(4, "Error", "Missing ProjectId field in request string");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        setJsonReponse(4, "Error", "Json submitted is not in correct format.");
                    }

                    if (isJsonDataCorrect)
                    {
                        DateTime currentDate1 = DateTime.Parse(UnixTimeStampToDateTime(objMobileReturnStockBO.DeviceRequestDateTime).ToString());

                        WebServiceReturnInventoryDAL objWebServiceReturnInventoryDAL = new WebServiceReturnInventoryDAL();

                        //MobileWarehouseResponseBo objMobileWarehouseResponseBo;

                        string message;
                        var result = objWebServiceReturnInventoryDAL.ManageReturnStock(ref objMobileReturnStockBO, jsonDataParam, currentDate1, out message);

                        objWebServiceReturnInventoryDAL.Dispose();


                        if (result)
                        {
                            Context.Response.ContentType = "application/json";
                            Context.Response.Write(s.Serialize(objMobileReturnStockBO));
                        }
                        else
                        {
                            setJsonReponse(4, "Error", message);
                        }

                    }


                    log.Info("WEB Service ReturnInventorySubmitData Ended at - " + DateTime.Now.ToString());
                }
                catch (Exception ex)
                {

                    log.Info("WEB Service ReturnInventorySubmitData ERROR at - " + DateTime.Now.ToString());
                    log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                    log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                    log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));

                    setJsonReponse(4, "Error", ex.Message);
                }

                log.Info("WEB Service ReturnInventorySubmitData Method Exit - " + DateTime.Now.ToString());
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ReceiveNotInInventoryToInventory(long UserId, long ServerRequestId, long ProductId,long WarehouseId)
        {


            //
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;


            log.Info("WEB Service AddToInventory Started at - " + DateTime.Now.ToString());



            try
            {
                if (UserId == 0 || ServerRequestId == 0 || ProductId == 0)
                {
                    setJsonReponse(4, "Error", "Invalid input.");
                }
                else
                {

                    string message = "";

                    var result = new InventoryWebServiceDAL().AddNotInInventoryInStock(UserId, ServerRequestId, ProductId,WarehouseId, out message);

                    if (result)
                    {
                        setJsonReponse(3, "Success", message);
                    }
                    else
                    {
                        setJsonReponse(4, "Error", message);
                    }

                }


            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  AddToInventory ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void InventoryAutoSynch(long UserId, bool IsLogIn, double currentDate)
        {

            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            log.Info("WEB Service AddToInventory Started at - " + DateTime.Now.ToString());

            try
            {
                if (UserId == 0)
                {
                    setJsonReponse(4, "Error", "Invalid input.");
                }
                else
                {
                    DateTime currentDate1 = DateTime.Parse(UnixTimeStampToDateTime(currentDate).ToString());

                    var result1 = new WebServiceRunningInventoryDAL().GetAutoSynchExistingAllocatedInventory(UserId, currentDate1);

                    var result = new WebServiceRunningInventoryDAL().GetAutoSynchInventory(UserId, currentDate1);

                    if (IsLogIn == true && result != null)
                    {
                        //if login then do not send negetive inventory as this is fresh login


                        new WebServiceRunningInventoryDAL().UpdateAutoSynchInventoryFlag(UserId, result.TransferRequestId);


                        result = new WebServiceRunningInventoryDAL().GetAutoSynchInventory(UserId, currentDate1);

                    }

                    if (result.TransferRequestId == 0)
                    {
                        if (IsLogIn == true)
                        {
                            if (result1.SerialNumberList != null && result1.SerialNumberList.Count > 0)
                            {
                                foreach (var item in result1.SerialNumberList)
                                {
                                    result.SerialNumberList.Insert(0, item);
                                }
                            }

                            if (result1.ProductQuantityList != null && result1.ProductQuantityList.Count > 0)
                            {
                                foreach (var item in result1.ProductQuantityList)
                                {
                                    result.ProductQuantityList.Insert(0, item);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (IsLogIn == true)
                        {
                            if (result1.SerialNumberList != null && result1.SerialNumberList.Count > 0)
                            {
                                foreach (var item in result1.SerialNumberList)
                                {
                                    result.SerialNumberList.Insert(0, item);
                                }
                            }

                            if (result1.ProductQuantityList != null && result1.ProductQuantityList.Count > 0)
                            {
                                foreach (var item in result1.ProductQuantityList)
                                {
                                    result.ProductQuantityList.Insert(0, item);
                                }
                            }
                        }

                    }

                    Context.Response.ContentType = "application/json";
                    Context.Response.Write(s.Serialize(result));
                }

            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  AddToInventory ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void InventoryAutoSynchSetFlag(long UserId, long TransferRequestId)
        {


            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;


            log.Info("WEB Service AddToInventory Started at - " + DateTime.Now.ToString());



            try
            {
                if (UserId == 0)
                {
                    setJsonReponse(4, "Error", "Invalid input.");
                }
                else
                {

                    var result = new WebServiceRunningInventoryDAL().UpdateAutoSynchInventoryFlag(UserId, TransferRequestId);

                    if (result)
                    {
                        setJsonReponse(3, "Success", "");
                    }
                    else
                    {
                        setJsonReponse(4, "Error", "");
                    }
                }


            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  AddToInventory ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }

        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentDate"></param>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UserProjectData(long userId, double currentDate)
        {

            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            try
            {
                DateTime currentDate1 = DateTime.Parse(UnixTimeStampToDateTime(currentDate).ToString());
                MobileOfflineDAL objMobile = new MobileOfflineDAL();

                var obj = objMobile.ProjectData(userId, currentDate1);
                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(obj));
                objMobile.Dispose();
            }
            catch (Exception ex)
            {
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                setJsonReponse(4, "Fail", ex.Message);
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetWarehouseProductList(long projectId)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;


            log.Info("WEB Service GetWarehouseProductList Started at - " + DateTime.Now.ToString());


            try
            {

                WebServiceProductDAL objWebServiceProductDAL = new WebServiceProductDAL();

                List<MobileWarehouseBO> ObjWarehouseList = objWebServiceProductDAL.GetWarehouseListNew(projectId);


                List<MobileProductBO> ProductList = objWebServiceProductDAL.GetProductList(projectId);

                var obj = new { ObjWarehouseList, ProductList };

                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(obj));
                objWebServiceProductDAL.Dispose();




            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  GetWarehouseUtilityList ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetProductListForProject(long projectId)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;


            log.Info("WEB Service GetProductList Started at - " + DateTime.Now.ToString());


            try
            {


                List<MobileProductBO> objMobileProductBOList = new List<MobileProductBO>();

                WebServiceProductDAL objWebServiceProductDAL = new WebServiceProductDAL();

                objMobileProductBOList = objWebServiceProductDAL.GetProductList(projectId);

                foreach (var item in objMobileProductBOList)
                {
                    item.ProductName = item.ProductName + "-" + item.Make + (string.IsNullOrEmpty(item.Type) ? "" : ("-" + item.Type))
                        + (string.IsNullOrEmpty(item.Size) ? "" : ("-" + item.Size));
                }

                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(objMobileProductBOList));
                objWebServiceProductDAL.Dispose();




            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  GetProductList ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }

        }

        
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ClearInventory()
        {
            try
            {
                string ConnectionString = "Data Source=198.57.200.76;Initial Catalog=CMSMeterTesting2;Integrated Security=false;User Id=CMSMeterUser;password=2017@@#CMS;Connection Timeout=0;";
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    //Create the SqlCommand object
                    SqlCommand cmd = new SqlCommand("STP_ClearInventory", con);

                    //Specify that the SqlCommand is a stored procedure
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;


                    //Open the connection and execute the query
                    con.Open();
                    cmd.ExecuteNonQuery();


                }
            }
            catch (Exception ex)
            {
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }

            setJsonReponse(3, "Success", "");
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetClockTimeMasterRecord(long projectId)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            log.Info("WEB Service GetProductList Started at - " + DateTime.Now.ToString());
            try
            {
                List<WebServiceTimeClockBO> objTimeClockBOList = new List<WebServiceTimeClockBO>();
                WebServiceTimeClockDAL objWebServiceClockDAL = new WebServiceTimeClockDAL();

                objTimeClockBOList = objWebServiceClockDAL.getTimeClockData();
              
                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(objTimeClockBOList));
                objWebServiceClockDAL.Dispose();
            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  GetClockTimeMasterRecord ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }
        }

        // Add Code by Deepak date 13 Oct 2020 
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]                                                 // UserTimeClockDetail = UTCD
        public void Save_Update_UserClockTimeRecord(long TimeClockId,long InstallerId,long UploadedDataExcelId,long ProductId, long PK_UTCD)
        {                                                                                  
            try
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                s.MaxJsonLength = 2147483644;
                log.Info("WEB Service Save_Update_UserClockTimeRecord Started at - " + DateTime.Now.ToString());
                List<WebServiceIimeClockDetailBO> objWebServiceIimeClockDetailBO = new List<WebServiceIimeClockDetailBO>();
                WebServiceTimeClockDAL objWebServiceClockDAL = new WebServiceTimeClockDAL();
                String returnid = objWebServiceClockDAL.InsertOrUpdate(TimeClockId, InstallerId, UploadedDataExcelId, ProductId, PK_UTCD);
                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(returnid));
                objWebServiceClockDAL.Dispose();

                log.Info("WEB Service Save_Update_UserClockTimeRecord Ended at - " + DateTime.Now.ToString());
            }
            catch(Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service Save_Update_UserClockTimeRecord ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }
        }

        // Add Code by Deepak date 13 Oct 2020 
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]                                                
        public void get_AccountSummaryReport(long ProjectId, long InstallerId, double summaryDate, string selectDate)
        {
            try
            {
                TimeSpan interval = TimeSpan.FromMilliseconds(summaryDate);
                DateTime dt = DateTime.ParseExact(selectDate,"MM-dd-yyyy",null);
                DateTime currentDate = DateTime.Parse(UnixTimeStampToDateTime(summaryDate).ToString());
                //DateTime currentDate = new DateTime(Convert.ToInt64(summaryDate));
                JavaScriptSerializer s = new JavaScriptSerializer();
                s.MaxJsonLength = 2147483644;
                log.Info("WEB Service get Account Summary Report Started at - " + DateTime.Now.ToString());
                ReportDAL objReportDAL = new ReportDAL();
                var list = objReportDAL.getAccountSummaryReport(ProjectId,InstallerId, currentDate, dt);
                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(list));
                
                log.Info("WEB Service Account Summary Report Started Ended at - " + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service Account Summary Report Started ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }
        }

        // Add Code by SNS date 13 Sep 2022 
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetAccountAssignedStatus(long InstallerId, long UploadExcelId)
        {
            try
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                s.MaxJsonLength = 2147483644;
                log.Info("WEB Service Get Account Assigned Status Started at - " + DateTime.Now.ToString());
                MobileDAL mobileDal = new MobileDAL();
                var result = mobileDal.getAccountAssignedStatus(InstallerId, UploadExcelId);
                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(new { status = "Success", assignedStatus = result }));

                log.Info("WEB Service Get Account Assigned Status Started Ended at - " + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service Get Account  ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetProjectUserList(long ProjectId)
        {
            try
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                s.MaxJsonLength = 2147483644;
                log.Info("WEB Service Get Project User List Started at - " + DateTime.Now.ToString());
                MobileDAL mobileDal = new MobileDAL();
                /*var result = mobileDal.getAccountAssignedStatus(InstallerId, UploadExcelId);
                Context.Response.ContentType = "application/json";
                Context.Response.Write(s.Serialize(new { status = "Success", assignedStatus = result }));*/

                List<UsersSmallModel> userList = new List<UsersSmallModel>();
                if (ProjectId > 0)
                {
                    using (UserRoleDAL obj = new UserRoleDAL())
                    {
                        userList = obj.GeInstallerListCurrentProject(ProjectId).OrderBy(a => a.UserName).ToList();
                    }
                }
                Context.Response.ContentType = "application/json";
                //Context.Response.Write(s.Serialize(new { status = "Success", projectUserList = userList }));
                Context.Response.Write(s.Serialize(userList));
                log.Info("WEB Service Get Project User List Ended at - " + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service Get Account  ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ViewReconcileDetailInventory(long UserId, long ProjectId)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;

            log.Info("WEB Service ViewReconcileDetailInventory Started at - " + DateTime.Now.ToString());

            try
            {
                if (UserId == 0)
                {
                    setJsonReponse(4, "Error", "Missing UserId field in request string.");
                }
                else if (ProjectId == 0)
                {

                    setJsonReponse(4, "Error", "Missing Project Id field in request string");
                    return;
                }
                else
                {
                    List<PROC_InstallStockDetailReport_Result> ReportData = new List<PROC_InstallStockDetailReport_Result>();
                    //List<InstallerStockReportSummaryBO> SummeryReportData = new List<InstallerStockReportSummaryBO>();
                    List<ReconcileDetail> inventoryDetail = new List<ReconcileDetail>();
                    InventoryDAL inventoryDal = new InventoryDAL();
                    ReportData = inventoryDal.GetReconcileInventoryReport(ProjectId, UserId);
                    List<long> productList = new List<long>();
                    inventoryDetail = ReportData.Where(o => o.InHandQty > 0).Select(o => new ReconcileDetail { ProductName = o.ProductName, AMP = o.SerialNumber, Quantity = o.InHandQty }).ToList();

                    Context.Response.ContentType = "application/json";
                    Context.Response.Write(s.Serialize(inventoryDetail));
                    inventoryDal.Dispose();
                }


            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service  ViewReconcileDetailInventory ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SendMailForMissingInventory(string jsonDataParam)
        {
            try
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                s.MaxJsonLength = 2147483644;
                log.Info("WEB Service SendMailForMissingInventory Started at - " + DateTime.Now.ToString());
                if (string.IsNullOrEmpty(jsonDataParam))
                {
                    setJsonReponse(4, "Error", "Missing parameter jsonDataParam.");
                }
                else
                {
                    MissingInventoryBO objMissingInventoryBO = new MissingInventoryBO();
                    objMissingInventoryBO = s.Deserialize<MissingInventoryBO>(jsonDataParam);
                    long projectId = objMissingInventoryBO.ProjectId;
                    long userId = objMissingInventoryBO.UserId;
                    List<String> mailto = new List<String>();
                    List<String> mailcc = new List<String>(); // ✅ yaha declare karo

                    if (projectId == 10135 || projectId == 10136)
                    {
                        mailto.Add("jpicard@scope-services.com");
                        mailto.Add("mhopkins@scope-services.com");
                        // mailto.Add("bjaime@scope-services.com");
                        mailto.Add("jchrist@scope-services.com");

                        mailcc.Add("garron@cmsmeter.com");
                        mailcc.Add("kunal.sharma@snssystem.com");
                        mailcc.Add("yogendra.rajpoot@snssystem.com");
                    }
                    else
                    {
                        mailto.Add("garron@cmsmeter.com");
                    }
                    /*CompassEntities db = new CompassEntities();
                    //  string Subject = (db.tblSkipReasonMasters.Where(n => n.SkipId == reOpenId).FirstOrDefault().SkipReason);
                    var UserFirstName = (db.tblUsers.Where(n => n.UserID == userId).FirstOrDefault().FirstName);
                    var UserLastName = (db.tblUsers.Where(n => n.UserID == userId).FirstOrDefault().LastName);
                    var UserName = UserFirstName + " " + UserLastName;
                    var ProjectName = (db.tblProjects.Where(n => n.ProjectId == projectId).FirstOrDefault().ProjectName);
                    //List<String> CCEmailTo = objReportDAL.CCEmailTo();
                    if (objMissingInventoryBO.MissingInventoryList != null)
                    {
                        foreach (var item in objMissingInventoryBO.MissingInventoryList)
                        {
                            var productName = item.ProductName;
                            var serialNum = item.SerialNumber;
                            var quantity = item.Quantity;
                        }
                    }*/
                    Task.Factory.StartNew(() =>
                    {
                        //mailto.Add(UserEmailId);
                        // mailto.Add("vishal.takawle@kairee.in");
                        // CCEmailTo.Add("bharat.magdum@kairee.in");
                        new GarronT.CMS.Model.clsMail().SendMail(mailto, mailcc, getMaliBody(objMissingInventoryBO), "Missing Inventory Alert!");
                    });
                    Context.Response.ContentType = "application/json";
                    Context.Response.Write(s.Serialize(new { status = "Success", sendMailResult = "Mail sent successfully." }));

                }
            }
            catch (Exception ex)
            {
                setJsonReponse(4, "Fail", ex.Message);
                log.Info("WEB Service Get Account  ERROR at " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
            }
            log.Info("WEB Service SendMailForMissingInventory Method Exit - " + DateTime.Now.ToString());
        }

        public string getMaliBody(MissingInventoryBO missingInventoryBO)
        {
            DateTime dtserverdate = new CommonFunctions().ServerDate();
            StringBuilder sb = new StringBuilder();
            CompassEntities db = new CompassEntities();
            long projectId = missingInventoryBO.ProjectId;
            long userId = missingInventoryBO.UserId;

            var UserFirstName = (db.tblUsers.Where(n => n.UserID == userId).FirstOrDefault().FirstName);
            var UserLastName = (db.tblUsers.Where(n => n.UserID == userId).FirstOrDefault().LastName);
            var UserName = UserFirstName + " " + UserLastName;
            var ProjectName = (db.tblProjects.Where(n => n.ProjectId == projectId).FirstOrDefault().ProjectName);

            sb.Append("<body  style='padding: 10px 0px 0px 10px;background: #f8f8fb; font-family:Times New Roman; font-size:14px;'>");

            /*sb.Append("Dear ");
            if (InstallerName != null)
            {
                sb.Append(" " + InstallerName.FirstName + " ,<br/>");
            }
            else
            {
                sb.Append(",<br/>");
            }*/
            //sb.Append("<br/><br/>");
            sb.Append("  <p style='font-size:15px;'>Missing Inventory report from Data Depot....!!!</p>");
            sb.Append("<br/> <p >This is to inform you that user " + UserName + " have missing inventory in project " + ProjectName + ". Please check below details.</p><br/>");
            sb.Append("<div style='background: #e6e1e1; width:30%;'><table style='width:100%; border:2px; padding: 0px 0px 0px 0px; border-collapse: collapse;'>" +
                            "<thead><tr>" +
                                "<td style='border:1px solid black;'><b>Product Name </b></td>" +
                                "<td style='border:1px solid black; text-align:center;'><b>Serial Number</b></td>" +
                                "<td style='border:1px solid black; text-align:center;'><b>Quantity</b></td>" +
                            "</tr></thead>");
            if (missingInventoryBO.MissingInventoryList != null)
            {
                sb.Append("<tbody>");
                foreach (var item in missingInventoryBO.MissingInventoryList)
                {
                    var productName = item.ProductName;
                    var serialNum = item.SerialNumber;
                    var quantity = item.Quantity;
                    sb.Append("<tr>" +
                              "<td style='border:1px solid black;'>" + productName + "</td>" +
                              "<td style='border:1px solid black; text-align:center;'>" + serialNum + "</td>" +
                              "<td style='border:1px solid black; text-align:center;'>" + quantity + "</td>" +
                              "</tr>");
                }
                sb.Append("</tbody>");
            }
            sb.Append("</table></div><br /><br/>");
            sb.Append("Thank You,<br/>Data Depot Team.<br>" +
              "<p style='text-align:center;'> <a href='https://www.datadepotonline.com/' title='' rel='home'>" +
              "<img src='https://www.datadepotonline.com/images/logo_03_2.png' title='Data Depot' alt='' style='height:100px; width:200px;'>" +
              "</a> </p>" +
              "<p style='text-align:center; font-size:13px;'>You are receiving this notification because you are registered with Data Depot ©"
              + DateTime.Now.ToString("yyyy") + " Data Depot </p> <br>");
            sb.Append("<p style='text-align:center; color:red; font-size:15px;'>*** This Email is system generated. Please do not reply to this email ID." +
                "For any queries please contact Data Depot Team. ***</p>");

            sb.Append("<p style='text-align:center; color:red; font-size:13px;'>This message (including any attachments) contains confidential information" +
                "intended for a specific individual and purpose and is protected by law." +
            "If you are not the intended recipient, you should delete this message." +
            " Any disclosure, copying or distribution of this message, or the taking of any action based on it, is strictly prohibited.</p>");


            sb.Append("</body>");
            string MailBody = sb.ToString();
            return MailBody;
        }
    }
}
