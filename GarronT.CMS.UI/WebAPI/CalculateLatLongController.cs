using GarronT.CMS.Model;
using GarronT.CMS.Model.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.WebPages;
using System.Xml.Linq;

namespace GarronT.CMS.UI.WebAPI
{
    public class CalculateLatLongController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projetID"></param>
        //public void CalculateNullLatLongbyProjectID(long projetID)
        //{
        //    CompassEntities CompassEntities = new CompassEntities();
        //    JavaScriptSerializer s = new JavaScriptSerializer();
        //    s.MaxJsonLength = 2147483644;
        //    bool result = false;

        //    var project = CompassEntities.tblProjects.Where(a => a.ProjectId == projetID && a.Active == 1).FirstOrDefault();

        //    // var city = (from c in CompassEntities.TblCityMasters where c.CityId == project.CityId select c).FirstOrDefault();
        //    var state = (from st in CompassEntities.tblStateMasters where st.StateId == project.StateId select st).FirstOrDefault();
        //    var cityname = CompassEntities.TblCityMasters.Where(o => o.CityId == project.CityId).Select(o => o.CityName).FirstOrDefault();
        //    try
        //    {
        //        string address, lat, lng;
        //        string Citylat, Citylng;
        //        var projectDataList = CompassEntities.tblUploadedDatas.Where(o => o.Active == 1 && o.ProjectId == projetID).ToList();

        //        if (projectDataList != null && projectDataList.Count > 0)
        //        {

        //            if (project != null)
        //            {
        //                project.IsLatLongCalculated = 2;  //Pending
        //                CompassEntities.SaveChanges();
        //            }

        //            //Added by Sanjiv to get City and state Lat Long
        //            FindCityCoordinates(Convert.ToString(cityname) + ", " + state.StateName + ", USA", out Citylat, out Citylng);

        //            foreach (var item in projectDataList)
        //            {
        //                if (!string.IsNullOrEmpty(item.Latitude) && item.Latitude.Trim().Length > 0 && item.Latitude != "0"
        //                    && !string.IsNullOrEmpty(item.Longitude) && item.Longitude.Trim().Length > 0 && item.Longitude != "0")
        //                {
        //                    #region check lat long is correct
        //                    /// check the lat long uploaded from excel is correct & comes
        //                    /// under current city & state
        //                    try
        //                    {
        //                        bool a = FindCoordinates(item.Latitude.ToString(), item.Longitude.ToString(), Citylat, Citylng);
        //                        //if lat lon correct then no change but if lat long is out of city then null will updated
        //                        // in the database.
        //                        item.Latitude = a ? item.Latitude : null;
        //                        item.Longitude = a ? item.Longitude : null;
        //                        item.AddressLatitude = a ? item.Latitude : null;
        //                        item.AddressLongitude = a ? item.Longitude : null;
        //                        CompassEntities.SaveChanges();

        //                    }
        //                    catch
        //                    {
        //                    }
        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region
        //                    //Changed by Sanjiv to remove # from address

        //                    if (Convert.ToString(item.Street).Trim().Contains('#'))
        //                    {
        //                        int startIndex = 0;
        //                        int endIndex = item.Street.IndexOf('#'); ;
        //                        int length = endIndex - startIndex - 1;
        //                        String computeddress = item.Street.Substring(0, length);
        //                        address = computeddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";
        //                    }
        //                    else
        //                    {
        //                        address = item.Street + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";

        //                    }
        //                    if (!string.IsNullOrEmpty(address))
        //                    {
        //                        System.Threading.Thread.Sleep(100);

        //                        FindCoordinates(address, out lat, out lng, Citylat, Citylng);
        //                        if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
        //                        {
        //                            item.Latitude = lat;
        //                            item.Longitude = lng;
        //                            item.AddressLatitude = lat;
        //                            item.AddressLongitude = lng;
        //                        }

        //                    }
        //                    CompassEntities.SaveChanges();

        //                    #endregion
        //                }
        //            }
        //            result = true;
        //        }


        //        if (project != null)
        //        {
        //            //project.IsLatLongCalculated = true;
        //            project.IsLatLongCalculated = 1; //  complete
        //            CompassEntities.Database.CommandTimeout = 180;
        //            CompassEntities.STP_SetGroupIDVisitId(projetID);
        //            CompassEntities.SaveChanges();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        if (project != null)
        //        {
        //            project.IsLatLongCalculated = 3; //Error
        //            CompassEntities.SaveChanges();
        //        }
        //        List<String> mailto = new List<String>();
        //        mailto.Add("aniket.jadhav@kairee.in");
        //        mailto.Add("sanjiv.shinde@kairee.in");
        //        mailto.Add("bharat.magdum@kairee.in");

        //        //mailto.Add(UserName);
        //        new GarronT.CMS.Model.clsMail().SendMail(mailto, null, GetRegisterMailBody(projetID, ex.Message), "Error in Latitude,Longitude calculation");
        //    }


        //    //var response = this.Request.CreateResponse(HttpStatusCode.OK);
        //    //response.Content = new StringContent(s.Serialize(result), Encoding.UTF8, "application/json");
        //    //return response;
        //}

        #region calculate lat long
        //[System.Web.Http.HttpPost]
        //[ActionName("CalculateLatLongbyProjectID")]
        //public void CalculateLatLongbyProjectID(long projetID)
        //{
        //    CompassEntities CompassEntities = new CompassEntities();
        //    JavaScriptSerializer s = new JavaScriptSerializer();
        //    s.MaxJsonLength = 2147483644;
        //    bool result = false;

        //    var project = CompassEntities.tblProjects.Where(a => a.ProjectId == projetID && a.Active == 1).FirstOrDefault();

        //    // var city = (from c in CompassEntities.TblCityMasters where c.CityId == project.CityId select c).FirstOrDefault();
        //    var state = (from st in CompassEntities.tblStateMasters where st.StateId == project.StateId select st).FirstOrDefault();
        //    var cityname = CompassEntities.TblCityMasters.Where(o => o.CityId == project.CityId).Select(o => o.CityName).FirstOrDefault();
        //    try
        //    {
        //        string address, lat, lng;
        //        string Citylat, Citylng;
        //        var projectDataList = (from m in CompassEntities.tblUploadedDatas where m.Active == 1 && m.ProjectId == projetID select m).ToList();

        //        if (projectDataList != null && projectDataList.Count > 0)
        //        {

        //            if (project != null)
        //            {
        //                // project.IsLatLongCalculated = true;
        //                project.IsLatLongCalculated = 2;  //Pending
        //                CompassEntities.SaveChanges();
        //            }
        //            //Added by Sanjiv to get City and state Lat Long
        //            FindCityCoordinates(Convert.ToString(cityname) + ", " + state.StateName + ", USA", out Citylat, out Citylng);

        //            foreach (var item in projectDataList)
        //            {
        //                //Changed by Sanjiv to remove # from address

        //                if (Convert.ToString(item.Street).Trim().Contains('#'))
        //                {
        //                    int startIndex = 0;
        //                    int endIndex = item.Street.IndexOf('#'); ;
        //                    int length = endIndex - startIndex - 1;
        //                    String computeddress = item.Street.Substring(0, length);
        //                    address = computeddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";
        //                }
        //                else
        //                {
        //                    address = item.Street + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";

        //                }
        //                if (!string.IsNullOrEmpty(address))
        //                {
        //                    System.Threading.Thread.Sleep(100);

        //                    FindCoordinates(address, out lat, out lng, Citylat, Citylng);
        //                    if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
        //                    {
        //                        item.Latitude = lat;
        //                        item.Longitude = lng;
        //                    }
        //                    //if (string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(lng))
        //                    //{
        //                    //    dtRow["Status"] = "Lat Long Not Found For Current Address.";
        //                    //}
        //                    //else
        //                    //{
        //                    //    dtRow["Status"] = "Lat Long Updated Sucessfully.";
        //                    //}
        //                }
        //                CompassEntities.SaveChanges();
        //            }
        //            result = true;
        //        }

        //        if (project != null)
        //        {
        //            //project.IsLatLongCalculated = true;
        //            project.IsLatLongCalculated = 1; //  complete
        //            CompassEntities.Database.CommandTimeout = 180;
        //            CompassEntities.STP_SetGroupIDVisitId(projetID);
        //            CompassEntities.SaveChanges();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        if (project != null)
        //        {
        //            project.IsLatLongCalculated = 3; //Error
        //            CompassEntities.SaveChanges();
        //        }
        //        List<String> mailto = new List<String>();
        //        mailto.Add("aniket.jadhav@kairee.in");
        //        mailto.Add("sanjiv.shinde@kairee.in");
        //        mailto.Add("bharat.magdum@kairee.in");
        //        //mailto.Add("khan.imran685@gmail.com");
        //        //mailto.Add("imran.khan@kairee.in");
        //        //mailto.Add(UserName);
        //        new GarronT.CMS.Model.clsMail().SendMail(mailto, null, GetRegisterMailBody(projetID, ex.Message), "Error in Latitude,Longitude calculation");
        //    }


        //    //var response = this.Request.CreateResponse(HttpStatusCode.OK);
        //    //response.Content = new StringContent(s.Serialize(result), Encoding.UTF8, "application/json");
        //    //return response;
        //}
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //:::                                                                         :::
        //:::  This routine calculates the distance between two points (given the     :::
        //:::  latitude/longitude of those points). It is being used to calculate     :::
        //:::  the distance between two locations using GeoDataSource(TM) products    :::
        //:::                                                                         :::
        //:::  Definitions:                                                           :::
        //:::    South latitudes are negative, east longitudes are positive           :::
        //:::                                                                         :::
        //:::  Passed to function:                                                    :::
        //:::    lat1, lon1 = Latitude and Longitude of point 1 (in decimal degrees)  :::
        //:::    lat2, lon2 = Latitude and Longitude of point 2 (in decimal degrees)  :::
        //:::    unit = the unit you desire for results                               :::
        //:::           where: 'M' is statute miles (default)                         :::
        //:::                  'K' is kilometers                                      :::
        //:::                  'N' is nautical miles                                  :::
        //:::                                                                         :::
        //:::  Worldwide cities and other features databases with latitude longitude  :::
        //:::  are available at https://www.geodatasource.com                          :::
        //:::                                                                         :::
        //:::  For enquiries, please contact sales@geodatasource.com                  :::
        //:::                                                                         :::
        //:::  Official Web site: https://www.geodatasource.com                        :::
        //:::                                                                         :::
        //:::           GeoDataSource.com (C) All Rights Reserved 2015                :::
        //:::                                                                         :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

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

            // XDocument xDoc = XDocument.Load("https://maps.googleapis.com/maps/api/geocode/xml?address=" + Address + "&sensor=false");
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
            //var result = new System.Net.WebClient().DownloadString(Address);
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(result);
            //XmlNodeList parentNode = doc.GetElementsByTagName("location");

            //foreach (XmlNode childrenNode in parentNode)
            //{
            //    Lat = childrenNode.SelectSingleNode("lat").InnerText;
            //    lng = childrenNode.SelectSingleNode("lng").InnerText;
            //}

            //var address = Address;

            //var locationService = new GoogleLocationService();
            //var point = locationService.GetLatLongFromAddress(Address);

            //if (point != null)
            //{
            //    Lat = Convert.ToString(point.Latitude);
            //    lng = Convert.ToString(point.Longitude);
            //}
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


            // XDocument xDoc = XDocument.Load("https://maps.googleapis.com/maps/api/geocode/xml?address=" + Address + "&sensor=false");
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
            //var result = new System.Net.WebClient().DownloadString(Address);
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(result);
            //XmlNodeList parentNode = doc.GetElementsByTagName("location");

            //foreach (XmlNode childrenNode in parentNode)
            //{
            //    Lat = childrenNode.SelectSingleNode("lat").InnerText;
            //    lng = childrenNode.SelectSingleNode("lng").InnerText;
            //}

            //var address = Address;

            //var locationService = new GoogleLocationService();
            //var point = locationService.GetLatLongFromAddress(Address);

            //if (point != null)
            //{
            //    Lat = Convert.ToString(point.Latitude);
            //    lng = Convert.ToString(point.Longitude);
            //}
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

        #endregion




        //public void CalculatPendingLatLongbyProjectID(long projetID)
        //{
        //    CompassEntities CompassEntities = new CompassEntities();
        //    JavaScriptSerializer s = new JavaScriptSerializer();
        //    s.MaxJsonLength = 2147483644;
        //    // bool result = false;

        //    var project = CompassEntities.tblProjects.Where(a => a.ProjectId == projetID && a.Active == 1).FirstOrDefault();

        //    var state = (from st in CompassEntities.tblStateMasters where st.StateId == project.StateId select st).FirstOrDefault();
        //    var cityname = CompassEntities.TblCityMasters.Where(o => o.CityId == project.CityId).Select(o => o.CityName).FirstOrDefault();
        //    try
        //    {
        //        string address, lat, lng;
        //        string Citylat, Citylng;
        //        var projectDataList = CompassEntities.tblUploadedDatas.Where(o => o.Active == 1 && o.ProjectId == projetID
        //            && (o.Latitude == string.Empty || o.Latitude == null || o.Longitude == string.Empty || o.Longitude == null)).ToList();

        //        if (projectDataList != null && projectDataList.Count > 0)
        //        {

        //            //if (project != null)
        //            //{
        //            //    project.IsLatLongCalculated = 2;  //Pending
        //            //    CompassEntities.SaveChanges();
        //            //}

        //            //Added by Sanjiv to get City and state Lat Long
        //            FindCityCoordinates(Convert.ToString(cityname) + ", " + state.StateName + ", USA", out Citylat, out Citylng);

        //            foreach (var item in projectDataList)
        //            {

        //                #region
        //                //Changed by Sanjiv to remove # from address

        //                if (Convert.ToString(item.Street).Trim().Contains('#'))
        //                {
        //                    int startIndex = 0;
        //                    int endIndex = item.Street.IndexOf('#'); ;
        //                    int length = endIndex - startIndex - 1;
        //                    String computeddress = item.Street.Substring(0, length);
        //                    address = computeddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";
        //                }
        //                else
        //                {
        //                    address = item.Street + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";

        //                }
        //                if (!string.IsNullOrEmpty(address))
        //                {
        //                    System.Threading.Thread.Sleep(100);

        //                    FindCoordinates(address, out lat, out lng, Citylat, Citylng);
        //                    if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
        //                    {
        //                        item.Latitude = lat;
        //                        item.Longitude = lng;
        //                        item.AddressLatitude = lat;
        //                        item.AddressLongitude = lng;
        //                    }

        //                }
        //                CompassEntities.SaveChanges();

        //                #endregion

        //            }
        //            // result = true;
        //        }


        //        if (project != null)
        //        {
        //            //project.IsLatLongCalculated = true;
        //            project.IsLatLongCalculated = 1; //  complete
        //            CompassEntities.Database.CommandTimeout = 180;
        //            CompassEntities.STP_SetGroupIDVisitId(projetID);
        //            CompassEntities.SaveChanges();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        if (project != null)
        //        {
        //            //project.IsLatLongCalculated = 3; //Error
        //            //CompassEntities.SaveChanges();
        //        }
        //        List<String> mailto = new List<String>();
        //        mailto.Add("aniket.jadhav@kairee.in");
        //        mailto.Add("sanjiv.shinde@kairee.in");
        //        mailto.Add("bharat.magdum@kairee.in");
        //        //mailto.Add("khan.imran685@gmail.com");
        //        //mailto.Add("imran.khan@kairee.in");
        //        //mailto.Add(UserName);
        //        new GarronT.CMS.Model.clsMail().SendMail(mailto, null, GetRegisterMailBody(projetID, ex.Message), "Error in Latitude,Longitude calculation");
        //    }


        //    //var response = this.Request.CreateResponse(HttpStatusCode.OK);
        //    //response.Content = new StringContent(s.Serialize(result), Encoding.UTF8, "application/json");
        //    //return response;
        //}




        public void GetLatLongForProject(long projetID)
        {
            string id = "";
            CompassEntities objCompassEntities = new CompassEntities();

            var project = objCompassEntities.tblProjects.Where(a => a.ProjectId == projetID && a.Active == 1).FirstOrDefault();

            // var city = (from c in CompassEntities.TblCityMasters where c.CityId == project.CityId select c).FirstOrDefault();
            string objCityName = string.Empty;
            string objStateName = string.Empty;
            if (project.mapBy == 0)
            {
                var state = (from st in objCompassEntities.tblStateMasters where st.StateId == project.StateId select st).FirstOrDefault();
                var cityname = objCompassEntities.TblCityMasters.Where(o => o.CityId == project.CityId).Select(o => o.CityName).FirstOrDefault();
                objStateName = Convert.ToString(state.StateName).ToString();
                objCityName = Convert.ToString(cityname).ToString();

            }
            else
            {
                objStateName = "Texas";
                objCityName = "OMAHA";
            }
            
            try
            {
                string address = string.Empty;
                string lat = string.Empty;
                string lng = string.Empty;
                string Citylat, Citylng;
                //var projectDataList = objCompassEntities.UploadedExcelDatas.Where(o => o.Active == 1 && o.FKProjectId == projetID).ToList();
                var projectDataList = objCompassEntities.UploadedExcelDatas.Where(o => o.Active == 1 && o.FKProjectId == projetID
                   && (o.AddressLatitude == string.Empty || o.AddressLatitude == null ||
                   o.AddressLongitude == string.Empty || o.AddressLongitude == null)).ToList();

                if (projectDataList != null && projectDataList.Count > 0)
                {

                    if (project != null)
                    {
                        project.IsLatLongCalculated = 2;  //Pending
                        objCompassEntities.SaveChanges();
                    }
                    string streetColumn = null;
                    string latColumn = null;
                    string longColumn = null;
                    GetDynamicColumnName(projetID, out streetColumn, out latColumn, out longColumn);


                    //Added by Sanjiv to get City and state Lat Long
                    //FindCityCoordinates(Convert.ToString(cityname) + ", " + state.StateName + ", USA", out Citylat, out Citylng);
                    FindCityCoordinates(Convert.ToString(objCityName) + ", " + objStateName + ", USA", out Citylat, out Citylng);

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


                            if (!string.IsNullOrEmpty(latValue) && latValue.Trim().Length > 0 && latValue != "0"
                           && !string.IsNullOrEmpty(longValue) && longValue.Trim().Length > 0 && longValue != "0")
                            {
                                #region check lat long is correct
                                /// check the lat long uploaded from excel is correct & comes
                                /// under current city & state
                                try
                                {
                                    bool a = FindCoordinates(latValue, longValue, Citylat, Citylng);
                                    //if lat lon correct then no change but if lat long is out of city then null will updated
                                    // in the database.
                                    item.AddressLatitude = a ? Convert.ToString(currentProperty.GetValue(item)) : null;
                                    item.AddressLongitude = a ? Convert.ToString(currentProperty1.GetValue(item)) : null;
                                    //currentProperty.SetValue(item, (a ? item.AddressLatitude : null), null);
                                    // currentProperty1.SetValue(item, (a ? item.AddressLongitude : null), null);

                                    //objCompassEntities.SaveChanges(); // one by one not save save all record in one time see in below

                                }
                                catch
                                {
                                }
                                #endregion
                            }
                            else
                            {
                                #region
                                var PropertyInfos = item.GetType().GetProperties();
                                var s2 = PropertyInfos.Where(o => o.Name == streetColumn).Select(o => o.GetValue(item, null)).FirstOrDefault();
                                string _streetAddress = Convert.ToString(s2);


                                if (Convert.ToString(_streetAddress).Trim().Contains('#'))
                                {
                                    int startIndex = 0;
                                    int endIndex = _streetAddress.IndexOf('#'); ;
                                    int length = endIndex - startIndex - 1;
                                    String computeddress = _streetAddress.Substring(0, length);
                                    //address = computeddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";
                                    address = computeddress + ", " + objCityName + ", " + objStateName + ", USA";
                                }
                                else
                                {
                                    //address = _streetAddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";
                                    address = _streetAddress + ", " + objCityName + ", " + objStateName + ", USA";
                                }
                                if (!string.IsNullOrEmpty(address))
                                {
                                    //System.Threading.Thread.Sleep(100);

                                    FindCoordinates(address, out lat, out lng, Citylat, Citylng);
                                    if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
                                    {
                                        item.AddressLatitude = lat;
                                        item.AddressLongitude = lng;
                                    }

                                }
                                //objCompassEntities.SaveChanges(); // one by one not save save all record in one time see in below
                                #endregion
                            }
                        }
                        else
                        {

                            if (!string.IsNullOrEmpty(item.AddressLatitude) && item.AddressLatitude.Trim().Length > 0 && item.AddressLatitude != "0"
                            && !string.IsNullOrEmpty(item.AddressLongitude) && item.AddressLongitude.Trim().Length > 0 && item.AddressLongitude != "0")
                            {
                                #region check lat long is correct
                                /// check the lat long uploaded from excel is correct & comes
                                /// under current city & state
                                try
                                {
                                    bool a = FindCoordinates(item.AddressLatitude.ToString(), item.AddressLongitude.ToString(), Citylat, Citylng);
                                    //if lat lon correct then no change but if lat long is out of city then null will updated
                                    // in the database.
                                    item.AddressLatitude = a ? item.AddressLatitude : null;
                                    item.AddressLongitude = a ? item.AddressLongitude : null;
                                    //objCompassEntities.SaveChanges(); // one by one not save save all record in one time see in below

                                }
                                catch
                                {
                                }
                                #endregion
                            }
                            else
                            {
                                #region
                                //Changed by Sanjiv to remove # from address

                                var PropertyInfos = item.GetType().GetProperties();
                                //foreach (PropertyInfo pInfo in PropertyInfos)
                                //{
                                //    string propertyName = pInfo.Name; //gets the name of the property
                                //    string s1 = Convert.ToString(pInfo.GetValue(item, null));
                                //}

                                var s2 = PropertyInfos.Where(o => o.Name == streetColumn).Select(o => o.GetValue(item, null)).FirstOrDefault();

                                //var barProperty = item.GetType().GetProperty(dynamicColumnName);
                                // string s = barProperty.GetValue(item, null) as string;

                                // Added new Code as per Project cerate Nation wide State and City Upload by Excel file.
                                InstallerDAL inDal = new InstallerDAL();
                                string dyZipColName = inDal.getDynamicColumnName("ZIP", Convert.ToString(projetID));
                                string dyCityColName = inDal.getDynamicColumnName("CITY", Convert.ToString(projetID));
                                string dyStateColName = inDal.getDynamicColumnName("STATE", Convert.ToString(projetID));

                                if (!string.IsNullOrEmpty(dyZipColName))
                                {
                                    var Zip = PropertyInfos.Where(o => o.Name == dyZipColName).Select(o => o.GetValue(item, null)).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(Zip.ToString()))
                                    {
                                        s2 += ", " + Zip.ToString();
                                    }
                                }
                                if (!string.IsNullOrEmpty(dyCityColName))
                                {
                                    var City = PropertyInfos.Where(o => o.Name == dyCityColName).Select(o => o.GetValue(item, null)).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(City.ToString()))
                                    {
                                        objCityName = City.ToString();
                                    }
                                }
                                if (!string.IsNullOrEmpty(dyStateColName))
                                {
                                    var State = PropertyInfos.Where(o => o.Name == dyStateColName).Select(o => o.GetValue(item, null)).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(State.ToString()))
                                    {
                                        objStateName = State.ToString();
                                    }
                                }
                                // End hare

                                string _streetAddress = Convert.ToString(s2);

                                if (Convert.ToString(_streetAddress).Trim().Contains('#'))
                                {
                                    int startIndex = 0;
                                    int endIndex = _streetAddress.IndexOf('#'); ;
                                    int length = endIndex - startIndex - 1;
                                    String computeddress = _streetAddress.Substring(0, length);
                                    //address = computeddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";
                                    address = computeddress + ", " + objCityName + ", " + objStateName + ", USA";
                                }
                                else
                                {
                                    //address = _streetAddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";
                                    address = _streetAddress + ", " + objCityName + ", " + objStateName + ", USA";

                                }
                                if (!string.IsNullOrEmpty(address))
                                {
                                    //System.Threading.Thread.Sleep(100);

                                    FindCoordinates(address, out lat, out lng, Citylat, Citylng);
                                    if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
                                    {
                                        item.AddressLatitude = lat;
                                        item.AddressLongitude = lng;
                                    }

                                }
                                //objCompassEntities.SaveChanges();   // one by one not save save all record in one time see in below

                                #endregion
                            }
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

                    //objCompassEntities.PROC_SetGroupIDVisitId(projetID);
                    //objCompassEntities.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                if (project != null)
                {
                    project.IsLatLongCalculated = 3; //Error
                    objCompassEntities.SaveChanges();
                }
                List<String> mailto = new List<String>();
                //mailto.Add("yogendra.rajpoot@snssystem.com");
                //mailto.Add("deepak.yadav@snssystem.com");

                //mailto.Add(UserName);
                new GarronT.CMS.Model.clsMail().SendMail(mailto, null, GetRegisterMailBody(projetID, ex.Message + "\n Uploaded excel table not save Lat or Long Records id = " + id), "Error in Latitude,Longitude calculation");
            }

        }

        public void updateVisitIdGroupId(long projetID)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            try
            {
                var projectDataList = objCompassEntities.UploadedExcelDatas.Where(o => o.Active == 1 && o.FKProjectId == projetID
                   && (o.VisitId == string.Empty || o.VisitId == null || o.GroupId == string.Empty || o.GroupId == null)).ToList();

                if (projectDataList != null && projectDataList.Count > 0)
                {
                    foreach (var item in projectDataList)
                    {
                        string lat = "";
                        string lng = "";
                        string id = "";

                        id = Convert.ToString(item.Id.ToString());
                        lat = item.AddressLatitude;
                        lng = item.AddressLongitude;
                        if (lat != "" && lng != "")
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
            }
            catch (Exception ex)
            {
            }

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
            catch (Exception ex)
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
            catch (Exception ex)
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
                long _ProjectID = long.Parse(ProjectID);
                //var _objVisitId = dbcontext.UploadedExcelDatas.Where(o => o.FKProjectId == Convert.ToInt64(ProjectID) && (o.GetType().GetProperties(o => o.Name == DynamicColumnName)).ToString() == Street.ToString() && o.VisitId != null && o.VisitId != "").FirstOrDefault();
                var _objVisitId = dbcontext.UploadedExcelDatas.Where(o => o.FKProjectId == _ProjectID && o.Street12 == Street && o.VisitId != null && o.VisitId != "").FirstOrDefault();
                if (_objVisitId != null)
                {
                    VisitId = _objVisitId.VisitId.ToString();       // This code has been chnaged because street address comming With apostrophe s(')
                }
                else
                {
                    // not availabe VisitId then generate new id
                    Guid objVisitId = Guid.NewGuid();
                    VisitId = objVisitId.ToString().ToUpper();
                }

                /*SqlConnection _sqlCon = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                _sqlCon.Open();
                SqlCommand _cmd = new SqlCommand("Select top 1 VisitId from UploadedExcelData where FKProjectId='" + ProjectID + "' and " + DynamicColumnName + "='" + Street + "' and VisitId !='' and VisitId is not null ", _sqlCon);
                SqlDataAdapter sda = new SqlDataAdapter(_cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                //var UpData=dbcontext.UploadedExcelDatas.Where()
                if (dt.Rows.Count > 0)
                {
                    // availabe VisitId id
                    VisitId = dt.Rows[0]["VisitId"].ToString();
                }
                else
                {
                    // not availabe VisitId then generate new id
                    Guid objVisitId = Guid.NewGuid();
                    VisitId = objVisitId.ToString().ToUpper();
                }
                _sqlCon.Close();*/
                return VisitId;
            }
            catch(Exception ex)
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
                SqlCommand _cmd = new SqlCommand("Select top 1 GroupId from UploadedExcelData where FKProjectId='" + ProjectID + "' and AddressLatitude='" + let + "' and AddressLongitude='" + lng + "' and GroupId !='' and GroupId is not null", _sqlCon);
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
                    GroupId = objGroupId.ToString().ToUpper();
                }
                _sqlCon.Close();
                return GroupId;
            }
            catch (Exception ex)
            {
                return "";
            }

        }


        // Old function not a used
        /*public void CalPendingLatLongForProject(long projetID)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            var project = objCompassEntities.tblProjects.Where(a => a.ProjectId == projetID && a.Active == 1).FirstOrDefault();

            var state = (from st in objCompassEntities.tblStateMasters where st.StateId == project.StateId select st).FirstOrDefault();
            var cityname = objCompassEntities.TblCityMasters.Where(o => o.CityId == project.CityId).Select(o => o.CityName).FirstOrDefault();
            try
            {
                string address, lat, lng;
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

                    //Added by Sanjiv to get City and state Lat Long
                    FindCityCoordinates(Convert.ToString(cityname) + ", " + state.StateName + ", USA", out Citylat, out Citylng);
                    int i = 1;
                    foreach (var item in projectDataList)
                    {

                        if (!string.IsNullOrEmpty(latColumn) && !string.IsNullOrEmpty(longColumn))
                        {
                            Type tClass = item.GetType();
                            PropertyInfo[] pClass = tClass.GetProperties();
                            PropertyInfo currentProperty = pClass.Where(o => o.Name == latColumn).FirstOrDefault();
                            string latValue = Convert.ToString(currentProperty.GetValue(item));
                            PropertyInfo currentProperty1 = pClass.Where(o => o.Name == longColumn).FirstOrDefault();
                            string longValue = Convert.ToString(currentProperty1.GetValue(item));

                            #region check lat long is correct
                            /// check the lat long uploaded from excel is correct & comes
                            /// under current city & state
                            try
                            {
                                if (!string.IsNullOrEmpty(latValue) && latValue.Length > 0 && latValue != "0"
                          && !string.IsNullOrEmpty(longValue) && longValue.Trim().Length > 0 && longValue != "0")
                                {
                                    bool a = FindCoordinates(latValue, longValue, Citylat, Citylng);
                                    //if lat lon correct then no change but if lat long is out of city then null will updated
                                    // in the database.
                                    item.AddressLatitude = a ? Convert.ToString(currentProperty.GetValue(item)) : null;
                                    item.AddressLongitude = a ? Convert.ToString(currentProperty1.GetValue(item)) : null;
                                    //currentProperty.SetValue(item, (a ? item.AddressLatitude : null), null);
                                    //  currentProperty1.SetValue(item, (a ? item.AddressLongitude : null), null);
                                    objCompassEntities.SaveChanges();
                                }
                                else
                                {
                                    #region
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
                                            //  currentProperty.SetValue(item, lat, null);
                                            //  currentProperty1.SetValue(item, lng, null);


                                        }
                                    }
                                    objCompassEntities.SaveChanges();

                                    #endregion
                                }

                            }
                            catch (Exception ex)
                            {
                                if (i == 1)
                                {
                                    List<String> mailto = new List<String>();
                                    // mailto.Add("aniket.jadhav@kairee.in");
                                    mailto.Add("yogendra.rajpoot@snssystem.com");
                                    mailto.Add("deepak.yadav@snssystem.com");

                                    new GarronT.CMS.Model.clsMail().SendMail(mailto, null, GetRegisterMailBody(projetID, ex.Message), "Error in Latitude,Longitude calculation");
                                }
                                i++;
                            }
                            #endregion

                        }
                        else
                        {
                            #region
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

                                    //Type tClass = item.GetType();
                                    //PropertyInfo[] pClass = tClass.GetProperties();
                                    //PropertyInfo currentProperty = pClass.Where(o => o.Name == mappedColumnName.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                    //currentProperty.SetValue(objRecord, sCurrentData, null);

                                }
                            }
                            objCompassEntities.SaveChanges();

                            #endregion
                        }

                    }
                    // result = true;
                }

                if (project != null)
                {
                    project.IsLatLongCalculated = 1; //  complete
                    objCompassEntities.SaveChanges();

                    //objCompassEntities.PROC_SetGroupIDVisitId(projetID);
                    //objCompassEntities.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                if (project != null)
                {
                    //project.IsLatLongCalculated = 3; //Error
                    //CompassEntities.SaveChanges();
                }
                List<String> mailto = new List<String>();
                //mailto.Add("aniket.jadhav@kairee.in");
                //mailto.Add("yogendra.rajpoot@snssystem.com");
                //mailto.Add("deepak.yadav@snssystem.com");

                new GarronT.CMS.Model.clsMail().SendMail(mailto, null, GetRegisterMailBody(projetID, ex.Message), "Error in Latitude,Longitude calculation");
            }
        }*/

        // Create new method 09 Oct 2020
        public void CalPendingLatLongForProject(long projetID)
        {
            string id = "";
            CompassEntities objCompassEntities = new CompassEntities();
            var project = objCompassEntities.tblProjects.Where(a => a.ProjectId == projetID && a.Active == 1).FirstOrDefault();

            string objCityName = string.Empty;
            string objStateName = string.Empty;
            if (project.mapBy == 0)
            {
                var state = (from st in objCompassEntities.tblStateMasters where st.StateId == project.StateId select st).FirstOrDefault();
                var cityname = objCompassEntities.TblCityMasters.Where(o => o.CityId == project.CityId).Select(o => o.CityName).FirstOrDefault();
                objStateName = Convert.ToString(state.StateName).ToString();
                objCityName = Convert.ToString(cityname).ToString();

            }
            else
            {
                objStateName = "Texas";
                objCityName = "OMAHA";
            }
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

                    //FindCityCoordinates(Convert.ToString(cityname) + ", " + state.StateName + ", USA", out Citylat, out Citylng);
                    FindCityCoordinates(Convert.ToString(objCityName) + ", " + objStateName + ", USA", out Citylat, out Citylng);
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


                            if (!string.IsNullOrEmpty(latValue) && latValue.Length > 0 && latValue != "0" && !string.IsNullOrEmpty(longValue) && longValue.Trim().Length > 0 && longValue != "0")
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
                                    //address = computeddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";
                                    address = computeddress + ", " + objCityName + ", " + objStateName + ", USA";
                                }
                                else
                                {
                                    //address = _streetAddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";
                                    address = _streetAddress + ", " + objCityName + ", " + objStateName + ", USA";

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
                            var PropertyInfos = item.GetType().GetProperties();
                            // Added new Code as per Project cerate Nation wide State and City Upload by Excel file.
                            InstallerDAL inDal = new InstallerDAL();
                            string dyZipColName = inDal.getDynamicColumnName("ZIP", Convert.ToString(projetID));
                            string dyCityColName = inDal.getDynamicColumnName("CITY", Convert.ToString(projetID));
                            string dyStateColName = inDal.getDynamicColumnName("STATE", Convert.ToString(projetID));

                            if (!string.IsNullOrEmpty(dyZipColName))
                            {
                                var Zip = PropertyInfos.Where(o => o.Name == dyZipColName).Select(o => o.GetValue(item, null)).FirstOrDefault();
                                if (!string.IsNullOrEmpty(Zip.ToString()))
                                {
                                    _streetAddress += ", " + Zip.ToString();
                                }
                            }
                            if (!string.IsNullOrEmpty(dyCityColName))
                            {
                                var City = PropertyInfos.Where(o => o.Name == dyCityColName).Select(o => o.GetValue(item, null)).FirstOrDefault();
                                if (!string.IsNullOrEmpty(City.ToString()))
                                {
                                    objCityName = City.ToString();
                                }
                            }
                            if (!string.IsNullOrEmpty(dyStateColName))
                            {
                                var State = PropertyInfos.Where(o => o.Name == dyStateColName).Select(o => o.GetValue(item, null)).FirstOrDefault();
                                if (!string.IsNullOrEmpty(State.ToString()))
                                {
                                    objStateName = State.ToString();
                                }
                            }
                            // End hare

                            if (Convert.ToString(_streetAddress).Trim().Contains('#'))
                            {
                                int startIndex = 0;
                                int endIndex = _streetAddress.IndexOf('#'); ;
                                int length = endIndex - startIndex - 1;
                                String computeddress = _streetAddress.Substring(0, length);
                                //address = computeddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";
                                address = computeddress + ", " + objCityName + ", " + objStateName + ", USA";
                            }
                            else
                            {
                                //address = _streetAddress + ", " + Convert.ToString(cityname) + ", " + state.StateName + ", USA";
                                address = _streetAddress + ", " + objCityName + ", " + objStateName + ", USA";
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
                mailto.Add("yogendra.rajpoot@snssystem.com");
                mailto.Add("deepak.yadav@snssystem.com");
                new GarronT.CMS.Model.clsMail().SendMail(mailto, null, GetRegisterMailBody(projetID, ex.Message + "\n Uploaded excel table not save Lat or Long Records id = " + id), "Error in Latitude,Longitude calculation");
            }
        }
        //ends hare


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

    }
}
