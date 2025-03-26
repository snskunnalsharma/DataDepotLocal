using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model;
using Newtonsoft.Json;
using System.Net;
using RestSharp;
using System.IO;

namespace TNMPUploadUtility
{
    internal class UploadFilesS3Bucekt
    {
        public void getFilesPath()
        {
            try
            {
                ErrorLogFile.writeStatus("Getting files path from DB Table" + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Getting files path from DB Table");
                CompassEntities compassEntities = new CompassEntities();
                var filesPath = compassEntities.TblProjectFieldDataAttachments.Where(o => o.Active == 1 && o.strFilePath!= null && (o.strCloudUrl == null || o.strCloudUrl == "") && o.CreatedOn.Value.Year == 2025).ToList();
                //var filesPath = compassEntities.TblProjectFieldDataAttachments.Where(o => o.CreatedOn.Value.Year == 2023).ToList();
                //string _webUrl = "C:\\inetpub\\vhosts\\datadepotonline.com\\httpdocs\\"; // Server Image defualt file path
                string _webUrl = "C:\\inetpub\\vhosts\\datadepotonline.com\\ts2.datadepotonline.com\\"; // Server Image defualt file path

                //string _webUrl = "D:\\DataDepot\\GarronT.CMS.UI\\"; //Local path for development purpose.
                ErrorLogFile.writeStatus("Number of old files:- " + filesPath.Count() + " : " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Number of old files:- " + filesPath.Count() + " : " + DateTime.Now.ToShortTimeString());
                int count = 0;
                foreach (var objfilespath in filesPath)
                {
                    try
                    {
                        string oldFilePath = string.Empty;
                        if (!string.IsNullOrEmpty(objfilespath.strFilePath))
                        {
                            oldFilePath = objfilespath.strFilePath;
                            if (oldFilePath.Contains("..\\..\\"))
                            {
                                oldFilePath = "" + oldFilePath.Replace("..\\..\\", _webUrl);
                            }

                            if (oldFilePath.Contains("~"))
                            {
                                oldFilePath = "" + oldFilePath.Replace("~\\", _webUrl);
                            }
                            oldFilePath = "" + oldFilePath.Replace('\\', '/');
                            oldFilePath = oldFilePath.Trim();
                            Console.WriteLine("Oldfile" + oldFilePath);

                            string cluodFilePath = UploadFileS3Bucket(oldFilePath);
  
                            if (!string.IsNullOrEmpty(cluodFilePath))
                            {
                                objfilespath.strCloudUrl = cluodFilePath;
                                compassEntities.SaveChanges();
                                ErrorLogFile.writeStatus("File successfully uploaded " + DateTime.Now.ToShortTimeString());
                                ErrorLogFile.writeStatus("Old file path:- " + oldFilePath + " - " + DateTime.Now.ToShortTimeString());
                                ErrorLogFile.writeStatus("New file path:- " + cluodFilePath + " - " + DateTime.Now.ToShortTimeString());
                                cluodFilePath = null;
                                count++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogFile.writeStatus("getFilesPath have caught an exception " + ex.Message.ToString() + DateTime.Now.ToShortTimeString());
                        //Console.WriteLine("getFilesPath have caught an exception " + ex.Message.ToString());
                    }
                }
                ErrorLogFile.writeStatus("Number of uploaded files => " + count + " : " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Number of uploaded files => " + count + " : " + DateTime.Now.ToShortTimeString());
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("getFilesPath have caught an exception " + ex.Message.ToString() + DateTime.Now.ToShortTimeString());
                //Console.WriteLine("getFilesPath have caught an exception " + ex.Message.ToString());
            }
        }

        public static string UploadFileS3Bucket(string filePath)
        {
            try
            {
                string path = @"" + filePath + "";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.DefaultConnectionLimit = 9999;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                var client = new RestClient("https://services.snssystem.com/api/v1/");
                var request = new RestRequest("file-upload", Method.POST);
                request.AddParameter("token", "RcAxZ2ncVHmT5m9y4wClek");
                request.AddParameter("type", "public");
                request.AddFile("attachments", path);
                IRestResponse response = client.Execute(request);
                var jsonData = JsonConvert.DeserializeObject<dynamic>(response.Content);
                var imgUrl = (string)jsonData.data;
                return imgUrl;
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("UploadFileS3Bucket have caught an exception  " + ex.Message.ToString() + DateTime.Now.ToShortTimeString());
                return null;
            }
        }
    }
}
