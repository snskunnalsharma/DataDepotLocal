using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using GarronT.CMS.Model;
using System.Data.SqlClient;
using System.Data;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.Model.BO;
using System.Transactions;
using System.Reflection;
using System.Xml.Linq;
using System.Configuration;

namespace CMSAuditThread
{
    class MobileSyncUtil
    {
        public void triggerScheduler()
        {
            DateTime dt = DateTime.Now;
            clsErrorLogFile.writeStatus("Thread Started at -" + DateTime.Now.ToShortTimeString());
            clsErrorLogFile.writeStatus("Dates -" + dt.Date.ToString() + " / " + DateTime.Now.Date.ToString());
            Console.WriteLine("Thread Started at -" + DateTime.Now.ToShortTimeString());
            try
            {
                CompleteTask completeTask = new CompleteTask();
                completeTask.setMobileSyncStatus(25);
                // Add 06 oct 2020
                //completeTask.PendingAllCrossesDateAddress_AssignToUnallocateaddres();     // Same old mobile sync utility used 

                // Add 09 oct 2020
                //CalPendingLatLongAllAddress calPendingLatLongAddress = new CalPendingLatLongAllAddress();
                //calPendingLatLongAddress.CalPendingLatLongForProject();

            }
            catch (Exception ex)
            {
                clsErrorLogFile.writeStatus("OnStart Method exception occured at -" + DateTime.Now.ToShortTimeString());
                clsErrorLogFile.writeStatus("OnStart Method exception ex message-" + ex.Message);
                clsErrorLogFile.writeStatus("OnStart Method exception ex inner exception message-" + Convert.ToString(ex.InnerException.Message));
                Console.WriteLine("OnStart Method exception ex message-" + ex.Message);
            }
            clsErrorLogFile.writeStatus("Thread End at -" + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Thread End at -" + DateTime.Now.ToShortTimeString());
        }
    }

    class CompleteTask
    {
        public void setMobileSyncStatus(int topActive)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            try
            {
                ProjectMasterDAL projectMasterDAL = new ProjectMasterDAL();
                IEnumerable<ProjectModel> data = (from a in projectMasterDAL.GetProjectName()
                                                  orderby a.ProjectName
                                                  select a).Distinct<ProjectModel>();

                foreach (var proj in data)
                {
                    var projectId = proj.ProjectId;
                    var fromDate = proj.stringFromDate;
                    var toDate = proj.stringToDate;
                    var formId = objCompassEntities.tblFormMasters.AsNoTracking().Where(o => o.Active == 1 && o.FormTypeID == 2
                       && o.ProjectId == projectId).Select(o => o.FormId).FirstOrDefault();
                    List<UsersSmallModel> userObj = new List<UsersSmallModel>();
                    if (projectId > 0)
                    {
                        using (UserRoleDAL obj = new UserRoleDAL())
                        {
                            userObj = obj.GeInstallerListCurrentProject(projectId).OrderBy(a => a.UserName).ToList();
                        }
                        foreach (var user in userObj)
                        {

                            List<CycleMaster> cycleList = GetExistingCycles((long)projectId, user.UserId, fromDate, toDate);

                            foreach (var cycle in cycleList)
                            {
                                if (user.UserId == 14 && projectId == 10075 && cycle.CycleId == "16")
                                {
                                    Console.WriteLine("For user and project - 14,10075");
                                }
                                var cycleId = cycle.CycleId;
                                clsErrorLogFile.writeStatus("For Cycle -" + cycleId);
                                Console.WriteLine("For Cycle -" + cycleId);
                                DataTable dt = new DataTable();
                                using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
                                {
                                    using (SqlCommand cmd = new SqlCommand("PROC_MobileSync_GetAfterTop", sqlcon))
                                    {
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.Add("@formId", formId);
                                        cmd.Parameters.Add("@InstallerId", user.UserId);
                                        cmd.Parameters.Add("@cycle", cycleId);
                                        cmd.Parameters.Add("@FromDate", DateTime.Parse(fromDate));
                                        cmd.Parameters.Add("@ToDate", DateTime.Parse(toDate));
                                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                                        {
                                            da.Fill(dt);
                                        }
                                    }
                                }
                                if (dt != null)
                                {
                                    var distExcelData = dt.AsEnumerable().
                                    Select(o => new
                                    {
                                        DataId = o.Field<long>("ID")
                                    }).Distinct().ToList();

                                    foreach (var dataId in distExcelData)
                                    {
                                        var installerData = objCompassEntities.tblInstallerMapDatas.Where(x => x.ID == dataId.DataId).Select(x => x.FK_UploadedExcelData_Id).FirstOrDefault();
                                        var excelData = objCompassEntities.UploadedExcelDatas.Where(o => o.Id == installerData).FirstOrDefault();
                                        clsErrorLogFile.writeStatus("For excelData Id -" + excelData.Id);
                                        Console.WriteLine("For excelData Id -" + excelData.Id);
                                        excelData.SyncEnable = 0;

                                    }
                                    objCompassEntities.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsErrorLogFile.writeStatus("SetMobileSyncStatus Method exception ex message-" + ex.Message);
                clsErrorLogFile.writeStatus("SetMobileSyncStatus Method exception ex inner exception message-" + Convert.ToString(ex.InnerException.Message));
            }



            /*using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
            {
                var userData = objCompassEntities.tblUsers.Where(o => o.Active == 1).ToList();
                foreach (var item in userData)
                {
                    var userId = item.UserID;
                    var projectList = new ProjectMasterDAL().GetUserProjectList(userId);

                    foreach(var project in projectList)
                    {
                        int active = 1;
                        GetExistingCycles((long)project, userId, "08-01-2017", "08-01-2020");
                        /*string cycleColumn = Convert.ToString((from m in objCompassEntities.TblProjectExcelMappings
                                                         join c in objCompassEntities.VW_GETTab6DropdownData on m.FieldType equals c.FieldType
                                                         where m.ProjectID == project && c.ProjectID == project && m.Active == active && c.FieldName == "CYCLE"
                                                         select m.FKUploadedExcelDataDynamicColumn).FirstOrDefault());
                        var cycleList = (from a in objCompassEntities.UploadedExcelDatas where a.Active == 1 &&
                                         a.FKProjectId == project group a by cycleColumn into b select cycleColumn).ToList();
                        DataTable dt = new DataTable();
                        var projectId = project;
                        //var cycleList = new InstallerDAL().GetCycleByProjectID((long)project);
                        foreach(var cycle in cycleList)
                        {

                        }*/
            //}


            //}
            /*using (SqlCommand cmd = new SqlCommand("FN_GetColumnName", sqlcon))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@input", "CYCLE");
                cmd.Parameters.Add("@projectId", projectId);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }*/
            //}
        }

        public List<CycleMaster> GetExistingCycles(long projectID, long installerId, string fromDate, string toDate)
        {
            List<string> cyclelist1 = new List<string>();

            cyclelist1 = new InstallerDAL().GetDynamicFreshCycleByProjectID(projectID);
            var cyclelist2 = new InstallerDAL().GetDynamicExistingMappedCycle(projectID, installerId, DateTime.Parse(fromDate), DateTime.Parse(toDate));
            List<CycleMaster> OrignalCycleList = new List<CycleMaster>();
            //List<CycleMaster> cyclelist = new List<CycleMaster>();
            foreach (var item in cyclelist1)
            {
                OrignalCycleList.Add(new CycleMaster { CycleId = item, CycleDisabled = false });
            }


            foreach (var item in cyclelist2)
            {
                var ObjCycle = OrignalCycleList.Where(o => o.CycleId == item.Cycle).FirstOrDefault();
                if (ObjCycle == null)
                {
                    OrignalCycleList.Add(new CycleMaster { CycleId = item.Cycle, CycleDisabled = item.Disabled == 0 ? false : true });
                }
                else
                {

                    if (ObjCycle.CycleDisabled == false)
                    {
                        ObjCycle.CycleDisabled = item.Disabled == 0 ? false : true;
                    }
                }
            }

            /*foreach (var item in cyclelist2)
            {
                var ObjCycle = cyclelist.Where(o => o.CycleId == item.Cycle).FirstOrDefault();
                if (ObjCycle == null)
                {
                    cyclelist.Add(new CycleMaster { CycleId = item.Cycle, CycleDisabled = item.Disabled == 0 ? false : true });
                }
                else
                {

                    if (ObjCycle.CycleDisabled == false)
                    {
                        ObjCycle.CycleDisabled = item.Disabled == 0 ? false : true;
                    }
                }
            }

            cyclelist = cyclelist.OrderBy(o => o.CycleId).Distinct().ToList();*/
            OrignalCycleList = OrignalCycleList.OrderBy(o => o.CycleId).Distinct().ToList();
            return OrignalCycleList;
            //return base.Json(new { OrignalCycleList = OrignalCycleList, CycleList = cyclelist }, JsonRequestBehavior.AllowGet);
        }

        // Created by date 06-Oct-2020
        // Pending All crosses date records return to unallocate entry
        public void PendingAllCrossesDateAddress_AssignToUnallocateaddres()
        {
            try
            {
                // connection
                CompassEntities dbcontext = new CompassEntities();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlDataReader reader;
                sqlConnection1.Open();

                // Get All Ative Project
                List<ProjectModel> GetAllActiveList = new List<ProjectModel>();
                List<string> ProjectStatusList = new List<string>();
                ProjectStatusList.Add("Active");
                GetAllActiveList = new ReportDAL().GetProjectNamebyProjectStatus(ProjectStatusList);
                foreach (var item in GetAllActiveList)
                {
                    // Get all installer project wise
                    List<UsersSmallModel> InstallerList = new List<UsersSmallModel>();
                    if (item.ProjectId > 0)
                    {
                        InstallerList = new ReportDAL().GeInstallerListbyProjectID(item.ProjectId);
                        foreach (var insID in InstallerList)
                        {
                            if (insID.UserId > 0)
                            {
                                string uid = Convert.ToString(insID.UserId);
                                SqlCommand cmd = new SqlCommand();
                                cmd.CommandText = "PROC_GetAllCrossedDateAddress";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Connection = sqlConnection1;
                                cmd.Parameters.Add("@InstallerId", uid);

                                reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    long Pendingid = Convert.ToInt64(reader["FK_UploadedExcelData_Id"]);

                                    if (Pendingid > 0)
                                    {
                                        SqlCommand cmdup = new SqlCommand("Update tblInstallerMapData set Active=0, ModifiedOn=getdate(),BatchNumber=null, MobileResponse =null where FK_UploadedExcelData_Id='" + Pendingid + "'", sqlConnection1);
                                        cmdup.ExecuteNonQuery();
                                    }
                                }
                                reader.Close();
                                reader.Dispose();
                                cmd.Dispose();
                            }
                        }
                    }
                }
                sqlConnection1.Close();
                sqlConnection1.Dispose();
            }
            catch (Exception ex)
            {
            }
        }
    }
    //ends hare

    // Created by date 06-Oct-2020
    class CalPendingLatLongAllAddress
    {
        public void CalPendingLatLongForProject()
        {
            // Get All Ative Project
            List<ProjectModel> GetAllActiveList = new List<ProjectModel>();
            List<string> ProjectStatusList = new List<string>();
            ProjectStatusList.Add("Active");
            GetAllActiveList = new ReportDAL().GetProjectNamebyProjectStatus(ProjectStatusList);
            foreach (var Pid in GetAllActiveList)
            {
                if (Pid.ProjectId > 0)
                {
                    string id = "";
                    CompassEntities objCompassEntities = new CompassEntities();
                    var projetID = Pid.ProjectId;
                    var project = objCompassEntities.tblProjects.Where(a => a.ProjectId == projetID && a.Active == 1).FirstOrDefault();

                    var state = (from st in objCompassEntities.tblStateMasters where st.StateId == project.StateId select st).FirstOrDefault();
                    var cityname = objCompassEntities.TblCityMasters.Where(o => o.CityId == project.CityId).Select(o => o.CityName).FirstOrDefault();
                    try
                    {
                        string address = string.Empty;
                        string lat = string.Empty;
                        string lng = string.Empty;
                        string Citylat, Citylng;
                        var projectDataList = objCompassEntities.UploadedExcelDatas.Where(o => o.Active == 1 && o.FKProjectId == projetID
                            && (o.AddressLatitude == string.Empty || o.AddressLatitude == null ||
                            o.AddressLongitude == string.Empty || o.AddressLongitude == null)).ToList();



                        string streetColumn = null;
                        string latColumn = null;
                        string longColumn = null;
                        GetDynamicColumnName(projetID, out streetColumn, out latColumn, out longColumn);

                        if (projectDataList != null && projectDataList.Count > 0)
                        {

                            FindCityCoordinates(Convert.ToString(cityname) + ", " + state.StateName + ", USA", out Citylat, out Citylng);
                            int i = 1;
                            foreach (var item in projectDataList)
                            {
                                id = Convert.ToString(item.Id.ToString());

                                if (!string.IsNullOrEmpty(latColumn) && !string.IsNullOrEmpty(longColumn))
                                {
                                    Type tClass = item.GetType();
                                    PropertyInfo[] pClass = tClass.GetProperties();
                                    PropertyInfo currentProperty = pClass.Where(o => o.Name == latColumn).FirstOrDefault();
                                    string latValue = Convert.ToString(currentProperty.GetValue(item));
                                    PropertyInfo currentProperty1 = pClass.Where(o => o.Name == longColumn).FirstOrDefault();
                                    string longValue = Convert.ToString(currentProperty1.GetValue(item));


                                    if (!string.IsNullOrEmpty(latValue) && latValue.Length > 0 && latValue != "0"
                              && !string.IsNullOrEmpty(longValue) && longValue.Trim().Length > 0 && longValue != "0")
                                    {
                                        bool a = FindCoordinates(latValue, longValue, Citylat, Citylng);
                                        //if lat lon correct then no change but if lat long is out of city then null will updated
                                        // in the database.
                                        item.AddressLatitude = a ? Convert.ToString(currentProperty.GetValue(item)) : null;
                                        item.AddressLongitude = a ? Convert.ToString(currentProperty1.GetValue(item)) : null;
                                        //objCompassEntities.SaveChanges();  // one by one not save save all record in one time see in below
                                    }
                                    else
                                    {
                                        //Changed by  remove # from address
                                        string _streetAddress = item.GetType().GetProperty(streetColumn).GetValue(item, null) as string;

                                        if (Convert.ToString(_streetAddress).Trim().Contains('#'))
                                        {
                                            int startIndex = 0;
                                            int endIndex = _streetAddress.IndexOf('#'); ;
                                            int length = endIndex - startIndex - 1;
                                            String computeddress = _streetAddress.Substring(0, length);
                                            address = computeddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";
                                        }
                                        else
                                        {
                                            address = _streetAddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";

                                        }
                                        if (!string.IsNullOrEmpty(address))
                                        {
                                            System.Threading.Thread.Sleep(100);

                                            FindCoordinates(address, out lat, out lng, Citylat, Citylng);
                                            if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
                                            {
                                                item.AddressLatitude = lat;
                                                item.AddressLongitude = lng;
                                            }
                                        }
                                        //objCompassEntities.SaveChanges(); // one by one not save save all record in one time see in below
                                    }
                                }
                                else
                                {
                                    //Changed by Sanjiv to remove # from address
                                    string _streetAddress = item.GetType().GetProperty(streetColumn).GetValue(item, null) as string;

                                    if (Convert.ToString(_streetAddress).Trim().Contains('#'))
                                    {
                                        int startIndex = 0;
                                        int endIndex = _streetAddress.IndexOf('#'); ;
                                        int length = endIndex - startIndex - 1;
                                        String computeddress = _streetAddress.Substring(0, length);
                                        address = computeddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";
                                    }
                                    else
                                    {
                                        address = _streetAddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";

                                    }
                                    if (!string.IsNullOrEmpty(address))
                                    {
                                        System.Threading.Thread.Sleep(100);

                                        FindCoordinates(address, out lat, out lng, Citylat, Citylng);
                                        if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
                                        {
                                            item.AddressLatitude = lat;
                                            item.AddressLongitude = lng;
                                        }
                                    }
                                    //objCompassEntities.SaveChanges(); // one by one not save save all record in one time see in below
                                }
                                objCompassEntities.SaveChanges();     // one by one records save
                                if (lat != string.Empty && lng != string.Empty)
                                {
                                    String DynamicColumnName = Get_DynamicColumnName(Convert.ToString(projetID));
                                    String StreetValue = Get_StreetValue(Convert.ToString(projetID), id, DynamicColumnName);
                                    String VisitId = Get_Create_VisitId(Convert.ToString(projetID), StreetValue, DynamicColumnName);
                                    String GroupId = Get_Create_GroupId(Convert.ToString(projetID), lat, lng);
                                    item.VisitId = VisitId;
                                    item.GroupId = GroupId;
                                    objCompassEntities.SaveChanges();     // one by one records save
                                }

                            }
                        }

                        if (project != null)
                        {
                            project.IsLatLongCalculated = 1; //  complete
                            objCompassEntities.SaveChanges();
                        }

                    }
                    catch (Exception ex)
                    {
                        List<String> mailto = new List<String>();
                        //mailto.Add("yogendra.rajpoot@snssystem.com");
                        new GarronT.CMS.Model.clsMail().SendMail(mailto, null, GetRegisterMailBody(projetID, ex.Message + "\n Uploaded excel table not save Lat or Long Records id = " + id), "Error in Latitude,Longitude calculation");
                    }
                }
            }
        }


        public void GetDynamicColumnName(long projectId, out string streetColumn, out string latColumn, out string longColumn)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var viewData = objCompassEntities.VW_GETTab6DropdownData.Where(o => o.ProjectID == projectId && o.FieldName == "STREET").FirstOrDefault();

            var dynamicColName = objCompassEntities.TblProjectExcelMappings.Where(o => o.ProjectID == projectId && o.FieldID == viewData.tabelId
                && o.FieldType == viewData.FieldType && o.Active == 1).FirstOrDefault();

            streetColumn = dynamicColName.FKUploadedExcelDataDynamicColumn;

            var viewData1 = objCompassEntities.VW_GETTab6DropdownData.Where(o => o.ProjectID == projectId && o.FieldName == "ADDRESS LATITUDE").FirstOrDefault();
            if (viewData1 == null)
            {
                latColumn = "";
            }
            else
            {
                var dynamicColName1 = objCompassEntities.TblProjectExcelMappings.Where(o => o.ProjectID == projectId && o.FieldID == viewData1.tabelId
                    && o.FieldType == viewData1.FieldType && o.Active == 1).FirstOrDefault();

                latColumn = dynamicColName1 == null ? "" : dynamicColName1.FKUploadedExcelDataDynamicColumn;
            }

            var viewData2 = objCompassEntities.VW_GETTab6DropdownData.Where(o => o.ProjectID == projectId && o.FieldName == "ADDRESS LONGITUDE").FirstOrDefault();
            if (viewData2 == null)
            {
                longColumn = "";
            }
            else
            {
                var dynamicColName2 = objCompassEntities.TblProjectExcelMappings.Where(o => o.ProjectID == projectId && o.FieldID == viewData2.tabelId
                    && o.FieldType == viewData2.FieldType && o.Active == 1).FirstOrDefault();

                longColumn = dynamicColName2 == null ? "" : dynamicColName2.FKUploadedExcelDataDynamicColumn;
            }
        }

        private double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        protected void FindCoordinates(string Address, out string Lat, out string lng, String Citylat, String Citylng)
        {
            Lat = lng = "";
            double calculatedDistance = 0;
            XDocument xDoc = XDocument.Load("https://maps.googleapis.com/maps/api/geocode/xml?key=" + Convert.ToString(ConfigurationManager.AppSettings["googleapiKey"]) + " &address=" + Address + "&sensor=false");
            var resultElements = (from result in xDoc.Descendants("GeocodeResponse").Elements("result")
                                  select new
                                  {
                                      TrunkPointName = result.Elements("address_component").FirstOrDefault().Element("long_name").Value,
                                      Address = result.Element("formatted_address").Value,
                                      Lat = Convert.ToDecimal(result.Element("geometry").Element("location").Element("lat").Value),
                                      lng = Convert.ToDecimal(result.Element("geometry").Element("location").Element("lng").Value)
                                  }).ToList();
            if (resultElements != null)
            {
                if (resultElements.Count > 0)
                {
                    //added by sanjiv to get lat long within 15vmiles of city
                    foreach (var result in resultElements)
                    {
                        Lat = Convert.ToString(result.Lat);
                        lng = Convert.ToString(result.lng);
                        if (Lat == null || Lat == "")
                        {

                        }
                        else
                        {
                            calculatedDistance = distance(Convert.ToDouble(result.Lat), Convert.ToDouble(result.lng), Convert.ToDouble(Citylat), Convert.ToDouble(Citylng), 'M');
                            var distanceFromCityCenterInKM = int.Parse(System.Configuration.ConfigurationManager.AppSettings["distanceFromCityCenterInKM"].ToString());
                            if (calculatedDistance < distanceFromCityCenterInKM)
                            {
                                Lat = Convert.ToString(result.Lat);
                                lng = Convert.ToString(result.lng);
                                break;
                            }
                            else
                            {
                                Lat = "";
                                lng = "";
                            }

                        }
                    }
                }
            }
        }

        protected bool FindCoordinates(string Lat, string lng, String Citylat, String Citylng)
        {

            try
            {
                double calculatedDistance = 0;
                double _lat = Convert.ToDouble(Lat, System.Globalization.CultureInfo.InvariantCulture);
                double _lon = Convert.ToDouble(lng, System.Globalization.CultureInfo.InvariantCulture);
                double _Citylat = Convert.ToDouble(Citylat, System.Globalization.CultureInfo.InvariantCulture);
                double _Citylng = Convert.ToDouble(Citylng, System.Globalization.CultureInfo.InvariantCulture);
                calculatedDistance = distance(_lat, _lon, _Citylat, _Citylng, 'M');

                var distanceFromCityCenterInKM = int.Parse(System.Configuration.ConfigurationManager.AppSettings["distanceFromCityCenterInKM"].ToString());
                if (calculatedDistance < distanceFromCityCenterInKM)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void FindCityCoordinates(string Address, out string Lat, out string lng)
        {
            Lat = lng = "";
            XDocument xDoc = XDocument.Load("https://maps.googleapis.com/maps/api/geocode/xml?key=" + Convert.ToString(ConfigurationManager.AppSettings["googleapiKey"]) + "&address=" + Address + "&sensor=true");
            var resultElements = (from result in xDoc.Descendants("GeocodeResponse").Elements("result")
                                  select new
                                  {
                                      TrunkPointName = result.Elements("address_component").FirstOrDefault().Element("long_name").Value,
                                      Address = result.Element("formatted_address").Value,
                                      Lat = Convert.ToDecimal(result.Element("geometry").Element("location").Element("lat").Value),
                                      lng = Convert.ToDecimal(result.Element("geometry").Element("location").Element("lng").Value)
                                  }).ToList();
            if (resultElements != null)
            {
                if (resultElements.Count > 0)
                {

                    Lat = Convert.ToString(resultElements.FirstOrDefault().Lat);
                    lng = Convert.ToString(resultElements.FirstOrDefault().lng);
                    if (Lat == null || Lat == "")
                    {

                    }
                }
            }
        }

        public string GetRegisterMailBody(long ProjectId, string Message)
        {

            StringBuilder sb = new StringBuilder();



            DateTime dtserverdate = new CommonFunctions().ServerDate();

            sb.Append("Hi,");
            //   string UserEmail = HttpUtility.UrlEncode(CommonFunctions.Encrypt(username.Trim()));
            sb.Append("<br /><br />The following errors occured when calculating Lat,Long for ProjectID-" + ProjectId + ". <br /><br /><b>Error :</b> " + Message + "<br />");

            sb.Append("<br /><br />Thanks and Regards,<br />Team CMS");


            // string MailFormat = new CommonFunctions().GetFileContents((Server.MapPath("~/EmailTemplate/RegisterMailTemplate.txt")));
            // string MailBody = String.Format(MailFormat, Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["WebURL"]), sb.ToString());

            return sb.ToString();
        }

        public string Get_DynamicColumnName(String ProjectID)
        {
            try
            {
                String DynamicColumnName = "";
                CompassEntities dbcontext = new CompassEntities();
                SqlConnection _sqlCon = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                _sqlCon.Open();
                SqlDataAdapter sda = new SqlDataAdapter("Select DynamicColumnName=[dbo].[FN_GetColumnName]('STREET','" + ProjectID + "')", _sqlCon);
                DataTable _dt = new DataTable();
                sda.Fill(_dt);
                if (_dt.Rows.Count > 0)
                {
                    DynamicColumnName = _dt.Rows[0]["DynamicColumnName"].ToString();
                }
                _sqlCon.Close();
                return DynamicColumnName;
            }
            catch
            {
                return null;
            }
        }
        public string Get_StreetValue(String ProjectID, String id, String DynamicColumnName)
        {
            try
            {
                string StreetValue = "";
                CompassEntities dbcontext = new CompassEntities();
                SqlConnection _sqlCon = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                _sqlCon.Open();
                SqlCommand _cmdstreetname = new SqlCommand("Select " + DynamicColumnName + " from UploadedExcelData where id='" + id + "'", _sqlCon);
                _cmdstreetname.CommandType = CommandType.Text;
                SqlDataAdapter _sda = new SqlDataAdapter(_cmdstreetname);
                DataTable _dts = new DataTable();
                _sda.Fill(_dts);
                if (_dts.Rows.Count > 0)
                {
                    StreetValue = _dts.Rows[0][0].ToString();
                }
                _sqlCon.Close();
                return StreetValue;
            }
            catch
            {
                return "";
            }

        }
        public string Get_Create_VisitId(String ProjectID, String Street, String DynamicColumnName)
        {
            try
            {
                string VisitId = "";
                CompassEntities dbcontext = new CompassEntities();
                SqlConnection _sqlCon = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                _sqlCon.Open();
                SqlCommand _cmd = new SqlCommand("Select top 1 VisitId from UploadedExcelData where FKProjectId='" + ProjectID + "' and " + DynamicColumnName + "='" + Street + "' and [VisitId] is not null ", _sqlCon);
                SqlDataAdapter sda = new SqlDataAdapter(_cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    // availabe VisitId id
                    VisitId = dt.Rows[0]["VisitId"].ToString();
                }
                else
                {
                    // not availabe VisitId then generate new id
                    Guid objVisitId = Guid.NewGuid();
                    VisitId = objVisitId.ToString();
                }
                _sqlCon.Close();
                return VisitId;
            }
            catch
            {
                return "";
            }
        }
        public string Get_Create_GroupId(String ProjectID, String let, String lng)
        {
            try
            {
                string GroupId = "";
                CompassEntities dbcontext = new CompassEntities();
                SqlConnection _sqlCon = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                _sqlCon.Open();
                SqlCommand _cmd = new SqlCommand("Select top 1 GroupId from UploadedExcelData where FKProjectId='" + ProjectID + "' and AddressLatitude='" + let + "' and AddressLongitude='" + lng + "' and GroupId is not null", _sqlCon);
                SqlDataAdapter sda = new SqlDataAdapter(_cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    // availabe VisitId id
                    GroupId = dt.Rows[0]["GroupId"].ToString();
                }
                else
                {
                    // not availabe VisitId then generate new id
                    Guid objGroupId = Guid.NewGuid();
                    GroupId = objGroupId.ToString();
                }
                _sqlCon.Close();
                return GroupId;
            }
            catch
            {
                return "";
            }

        }
    }
    /*
    class SkippedTask
    {
        public void changeSkipToNotAllocated()
        {
            DateTime dt = DateTime.Now;
            clsErrorLogFile.writeStatus("Thread changeSkipToNotAllocated Started at -" + DateTime.Now.ToShortTimeString());
            clsErrorLogFile.writeStatus("Dates -" + dt.Date.ToString() + " / " + DateTime.Now.Date.ToString());
            CompassEntities objCompassEntities = new CompassEntities();
            try
            {
                DataTable dtable = new DataTable();
                using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("PROC_GetSkippedAllBefore72h", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtable);
                        }
                    }
                    sqlcon.Close();
                    
                }
                if (dtable != null)
                {
                    var distSkippedData = dtable.AsEnumerable().
                    Select(o => new
                    {
                        ExcelId = o.Field<long>("ID"),
                        ProjectId = o.Field<long>("ProjectId"),
                        InstallerId = o.Field<long>("InstallerId"),
                        FormId = o.Field<long>("FormId")
                    }).Distinct().ToList();

                    foreach (var skipData in distSkippedData)
                    {
                        var projectId = objCompassEntities.UploadedExcelDatas.AsNoTracking().Where(o => o.Id == skipData.ExcelId && o.Active == 1).Select(o => o.FKProjectId).FirstOrDefault();
                        var formObject = objCompassEntities.tblFormMasters.AsNoTracking().Where(o => o.FormId == skipData.FormId && o.ProjectId == projectId).FirstOrDefault();
                        var objDependantFormList = objCompassEntities.tblFormTypeMaps.Where(o => o.FormTypeId == formObject.FormId && o.Active == 1).Select(o => o).ToList();

                    }
                    objCompassEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                clsErrorLogFile.writeStatus("changeSkipToNotAllocated Method exception occured at -" + DateTime.Now.ToShortTimeString());
                clsErrorLogFile.writeStatus("changeSkipToNotAllocated Method exception ex message-" + ex.Message);
                clsErrorLogFile.writeStatus("changeSkipToNotAllocated Method exception ex inner exception message-" + Convert.ToString(ex.InnerException.Message));
                Console.WriteLine("changeSkipToNotAllocated Method exception ex message-" + ex.Message);
            }
            clsErrorLogFile .writeStatus("Thread changeSkipToNotAllocated End at -" + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Thread changeSkipToNotAllocated End at -" + DateTime.Now.ToShortTimeString());
        }
    }*/
}