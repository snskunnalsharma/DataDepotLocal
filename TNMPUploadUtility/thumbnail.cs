//using System;
//using System.Linq;
//using GarronT.CMS.Model;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Drawing.Drawing2D;
//using System.IO;
//using System.Data.Entity;
//using GarronT.CMS.Model;
//using System.Net;
//using RestSharp;
//using Newtonsoft.Json;

namespace TNMPUploadUtility
{
    class thumbnail
    {
        public void getFilesPath()
        {
        //    try
        //    {
        //        DateTime twoDaysAgo = DateTime.Now.AddDays(-6);
        //        ErrorLogFile.writeStatus("Getting files path from DB Table " + twoDaysAgo.ToShortTimeString());
        //        Console.WriteLine("Getting files path from DB Table");
        //        CompassEntities compassEntities = new CompassEntities();
        //        var thumbnailUpdater = new ThumbnailUpdater(compassEntities);

        //        // Retrieve data from the database that meet the initial criteria
        //        var initialFilesPath = compassEntities.TblProjectFieldDataAttachments
        //            .Where(o => o.Active == 1 && o.strFilePath != null)
        //            .ToList();

        //        // Filter the retrieved data based on tmbimg in-memory
        //        var filesPath = initialFilesPath.Where(o => o.tmbimg == null).ToList();

        //        // Now, you can work with the filtered data in 'filesPath'
        //        string _webUrl = "D:\\DataDepot\\GarronT.CMS.UI\\"; // Local path for development purpose.

        //        ErrorLogFile.writeStatus("Number of old files:- " + filesPath.Count() + " : " + twoDaysAgo.ToShortTimeString());
        //        Console.WriteLine("Number of old files:- " + filesPath.Count() + " : " + DateTime.Now.ToShortTimeString());

        //        int count = 0;
        //        string newFileName = "", FolderName = "", fileExtension=""; 

        //        foreach (var objfilespath in filesPath)
        //        {
        //            try
        //            {

        //                string oldFilePath = "";

        //                if (!string.IsNullOrEmpty(objfilespath.strFilePath))
        //                {
        //                    oldFilePath = objfilespath.strFilePath;
        //                    if (oldFilePath.Contains("..\\..\\"))
        //                    {
        //                        oldFilePath = "" + oldFilePath.Replace("..\\..\\", _webUrl);
        //                    }

        //                    if (oldFilePath.Contains("~"))
        //                    {
        //                        oldFilePath = "" + oldFilePath.Replace("~\\", _webUrl);
        //                    }
        //                    oldFilePath = "" + oldFilePath.Replace('\\', '/');
        //                    oldFilePath = oldFilePath.Trim();

        //                    FolderName = Path.GetDirectoryName(oldFilePath);
        //                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(oldFilePath);
        //                    fileExtension = Path.GetExtension(oldFilePath);

        //                    newFileName = fileNameWithoutExtension + "tmb" + fileExtension;

        //                    Console.WriteLine("Original File Name: " + Path.GetFileName(oldFilePath));
        //                    Console.WriteLine("New File Name: " + newFileName);
        //                    Console.WriteLine("Oldfile" + oldFilePath);
        //                }
        //                    if (!string.IsNullOrEmpty(oldFilePath) && File.Exists(oldFilePath))
        //                {
        //                    using (var originalImage = Image.FromFile(oldFilePath))
        //                    {
        //                        int thumbnailWidth = 100;
        //                        int thumbnailHeight = 100;

        //                        using (var thumbnailImage = new Bitmap(thumbnailWidth, thumbnailHeight))
        //                        using (var thumbnailGraphic = Graphics.FromImage(thumbnailImage))
        //                        {
        //                            thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //                            thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality;
        //                            thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality;
        //                            thumbnailGraphic.PixelOffsetMode = PixelOffsetMode.HighQuality;

        //                            thumbnailGraphic.DrawImage(originalImage, 0, 0, thumbnailWidth, thumbnailHeight);
        //                            string thumbnailFilePath = FolderName+"/"+newFileName; // Specify the path where you want to save the thumbnail
        //                                                                                   //   thumbnailImage.Save(thumbnailFilePath, ImageFormat.Jpeg);           
        //                                                                                   // Determine the ImageFormat based on the file extension

        //                            try
        //                            {
        //                                ImageFormat imageFormat = GetImageFormatFromExtension(thumbnailFilePath);
        //                                thumbnailImage.Save(thumbnailFilePath, imageFormat);

        //                                try
        //                                {
        //                                    if (File.Exists(thumbnailFilePath))
        //                                    {
        //                                        objfilespath.tmbimg = thumbnailFilePath;
        //                                        compassEntities.SaveChanges();
        //                                        ErrorLogFile.writeStatus("File successfully uploaded " + DateTime.Now.ToShortTimeString());
        //                                        ErrorLogFile.writeStatus("new file path:- " + thumbnailFilePath + " - " + DateTime.Now.ToShortTimeString());
        //                                    }
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    Console.WriteLine("Error saving file: " + ex.Message);
        //                                }

        //                            }
        //                            catch (Exception e)
        //                            {
        //                                Console.WriteLine("Error processing file: " + objfilespath.strFilePath);
        //                                Console.WriteLine(e.Message);

        //                            }
                                   

        //                            //thumbnailImage.Save(memoryStream, ImageFormat.Jpeg);
        //                            //byte[] thumbnailImageBytes = memoryStream.ToArray();

        //                            // Update the 'tmbimg' column in the database with the thumbnail data
        //                            //  long projectId = objfilespath.ID;
        //                            // thumbnailUpdater.UpdateThumbnailInDatabase(projectId, thumbnailImageBytes);

        //                        }
        //                    }
        //                }

        //                count++;
        //            }
        //            catch (Exception ex)
        //            {
        //                // Handle the exception or log it for debugging
        //                Console.WriteLine("Error processing file: " + objfilespath.strFilePath);
        //                Console.WriteLine(ex.Message);
        //            }
        //        }

        //        ErrorLogFile.writeStatus("Number of uploaded files => " + count + " : " + twoDaysAgo.ToShortTimeString());
        //        Console.WriteLine("Number of uploaded files => " + count + " : " + DateTime.Now.ToShortTimeString());
        //    }
        //    catch (Exception ex)
        //    {
        //        DateTime twoDaysAgo2 = DateTime.Now.AddDays(-6);
        //        ErrorLogFile.writeStatus("getFilesPath have caught an exception " + ex.Message.ToString() + twoDaysAgo2.ToShortTimeString());
        //    }
        //}

        
        //public void CreateThumbnail(string sourceImagePath, string destinationImagePath, int width, int height)
        //{
        //    using (var sourceImage = new Bitmap(sourceImagePath))
        //    {
        //        var thumbnail = sourceImage.GetThumbnailImage(width, height, null, IntPtr.Zero);
        //        thumbnail.Save(destinationImagePath, ImageFormat.Jpeg); // You can change the format as needed.
        //    }
        //}

        //static ImageFormat GetImageFormatFromExtension(string filePath)
        //{
        //    string extension = Path.GetExtension(filePath).ToLower();
        //    switch (extension)
        //    {
        //        case ".jpg":
        //        case ".jpeg":
        //            return ImageFormat.Jpeg;

        //        case ".png":
        //            return ImageFormat.Png;

        //        case ".gif":
        //            return ImageFormat.Gif;

        //        // Add more cases for other supported image formats as needed

        //        default:
        //            throw new NotSupportedException("Image format not supported.");
        //    }
        }
    }
}