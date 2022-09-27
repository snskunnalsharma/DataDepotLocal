using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using Goheer.EXIF;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Script.Serialization;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;



namespace GarronT.CMS.UI.WebAPI
{
    public class FileResult
    {
        public IEnumerable<string> FileNames { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateTime UpdatedTimestamp { get; set; }
        // public string DownloadLink { get; set; }
        public IEnumerable<string> ContentTypes { get; set; }
        public IEnumerable<string> Names { get; set; }
    }




    //  [RoutePrefix("api/UploadFile")]
    public class UploadFileController : ApiController
    {

        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(CMS_MobileService));
        #endregion

        private static readonly string ServerUploadFolder = Convert.ToString(ConfigurationManager.AppSettings["UploadFilePath"]);
        //@"~\MobileUploadData\"; //Path.GetTempPath();

        //return JsonErrorResponse(s, "fail", "no attachment found for the request.");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="status"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private HttpResponseMessage JsonErrorResponse(JavaScriptSerializer s, string status, string errorMessage)
        {
            ErrorModelStatus em = new ErrorModelStatus();
            em.status = status;
            em.errorMessage = errorMessage;
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
            return response;
        }

        #region SERVICE IMAGE CAPTURE
        [System.Web.Http.HttpPost]
        [ActionName("UploadServiceImageFiles")]
        public HttpResponseMessage UploadServiceImageFiles()
        {
            log.Info("WEB API Service  UploadServiceImageFiles Started at - " + DateTime.Now.ToString());
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            try
            {


                string projectId = ""; string AccountNumber = "";
                string formId = ""; string accountInitialFieldId = "";
                string imageName = ""; int iUploadedCnt = 0;
                string serviceinitialId = ""; string userId = "";
                string isAdditionalService = "";
                HttpFileCollection hfc = HttpContext.Current.Request.Files;

                userId = HttpContext.Current.Request.Form["userId"];
                formId = HttpContext.Current.Request.Form["formId"];
                accountInitialFieldId = HttpContext.Current.Request.Form["accountInitialFieldId"];
                serviceinitialId = HttpContext.Current.Request.Form["serviceinitialId"];
                isAdditionalService = HttpContext.Current.Request.Form["isAdditionalService"];
                imageName = HttpContext.Current.Request.Form["imageName"];
                log.Info("WEB API Service UploadServiceImageFiles Requested by  - " + userId);
                long _formId = 0; long _serviceInitialId = 0;
                int _userId = 0; long _accountInitialFieldId = 0; bool _isAdditionalServices = false;
                bool validParameters = true;

                #region validation
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out _userId))
                {
                    return JsonErrorResponse(s, "fail", "userId field not valid.");
                }
                else if (string.IsNullOrEmpty(formId) || !long.TryParse(formId, out _formId))
                {
                    return JsonErrorResponse(s, "fail", "formId field not valid.");
                }
                else if (string.IsNullOrEmpty(accountInitialFieldId) || !long.TryParse(accountInitialFieldId, out _accountInitialFieldId))
                {
                    return JsonErrorResponse(s, "fail", "accountInitialFieldId field not valid.");
                }
                else if (string.IsNullOrEmpty(serviceinitialId) || !long.TryParse(serviceinitialId, out _serviceInitialId))
                {
                    return JsonErrorResponse(s, "fail", "serviceinitialId field not valid.");
                }
                else if (string.IsNullOrEmpty(isAdditionalService) || !bool.TryParse(isAdditionalService, out _isAdditionalServices))
                {
                    return JsonErrorResponse(s, "fail", "isAdditionalService field not valid.");
                }
                else if (string.IsNullOrEmpty(imageName))
                {
                    return JsonErrorResponse(s, "fail", "imageName field empty.");
                }
                #endregion

                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    string[] extension = hpf.FileName.Split('.');
                    if (extension.Count() < 2)
                    {
                        validParameters = false;
                    }
                    else
                    {
                        if (extension[1].ToLower() != "jpeg" && extension[1].ToLower() != "jpg")
                        {
                            validParameters = false;
                        }
                    }
                }

                if (!validParameters)
                {
                    return JsonErrorResponse(s, "fail", "only .jpeg/.jpg images allowed.");
                }
                else
                {

                    CompassEntities objCompassEntities = new CompassEntities();

                    projectId = Convert.ToString(objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).Select(o => o.ProjectId).FirstOrDefault());

                    if (string.IsNullOrEmpty(projectId))
                    {
                        return JsonErrorResponse(s, "fail", "formId not present in the database.");
                    }

                    AccountNumber = Convert.ToString(objCompassEntities.UploadedExcelDatas.
                       Where(o => o.Id == _accountInitialFieldId).Select(o => o.Account12).FirstOrDefault());
                    if (string.IsNullOrEmpty(AccountNumber))
                    {
                        return JsonErrorResponse(s, "fail", "accountInitialFieldId not present in the database.");
                    }

                    string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
                        AccountNumber + @"\" + accountInitialFieldId + @"\" + imageName + @"\";
                    string filePath = HttpContext.Current.Request.MapPath(tempPath);
                    grantAccess(filePath);

                    List<string> imageFileNameList = new List<string>();
                    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                    {
                        System.Web.HttpPostedFile hpf = hfc[iCnt];

                        if (hpf.ContentLength > 0)
                        {
                            // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)


                            if (System.IO.File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                            {
                                System.IO.File.Delete(filePath + Path.GetFileName(hpf.FileName));
                            }
                            hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                            string _imagePath = filePath + Path.GetFileName(hpf.FileName);
                            // Rotate the image according to EXIF data
                            var bmp = new Bitmap(_imagePath);
                            //if (System.IO.File.Exists(_imagePath))
                            //{
                            //    System.IO.File.Delete(_imagePath);
                            //}

                            var exif = new EXIFextractor(ref bmp, "n");

                            if (exif["Orientation"] != null)
                            {
                                RotateFlipType flip = OrientationToFlipType(exif["Orientation"].ToString());

                                if (flip != RotateFlipType.RotateNoneFlipNone) // don't flip of orientation is correct
                                {
                                    bmp.RotateFlip(flip);
                                    exif.setTag(0x112, "1"); // Optional: reset orientation tag
                                    bmp.Save(_imagePath, ImageFormat.Jpeg);
                                }
                            }
                            bmp.Dispose();

                            iUploadedCnt = iUploadedCnt + 1;
                            imageFileNameList.Add(tempPath + Path.GetFileName(hpf.FileName));
                        }
                    }




                    var pictureRecord = objCompassEntities.tblPictures.Where(o => o.Picture.ToLower() == imageName.ToLower()).Select(o => o).FirstOrDefault();

                    if (pictureRecord == null)
                    {
                        return JsonErrorResponse(s, "fail", "Image data not found for the current imageName.");
                    }
                    long masterId = 0;
                    if (_isAdditionalServices)
                    {
                        var objmasterFieldRelation = objCompassEntities.TblProjectMasterFieldRelations.Where(o => o.MasterFieldID == _serviceInitialId).Select(o => o).FirstOrDefault();
                        _serviceInitialId = objmasterFieldRelation.MasterFieldID;

                        masterId = objmasterFieldRelation.ID;
                    }
                    var rv = objCompassEntities.TblServicePictures.Where(o => o.tblPicture_PictureId == pictureRecord.ID && o.tblService_ServiceId == _serviceInitialId).Select(o => o).FirstOrDefault();

                    if (rv == null)
                    {
                        return JsonErrorResponse(s, "fail", "No data not found for the current serviceinitialId.");
                    }

                    var objForm = objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).FirstOrDefault();

                    if (objForm.FormTypeID == 2)
                    {

                        #region Installation process


                        var projectFieldData = objCompassEntities.TblProjectFieldDatas.Where(o => o.Active == 1 && o.FormId == _formId
                            && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                        if (projectFieldData == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic info first for the image.");
                        }


                        tblProjectFieldDataMaster fielddata;
                        if (!_isAdditionalServices)
                        {
                            fielddata = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1 && o.TblProjectFieldData_Id == projectFieldData.ID &&
                                        o.InitialFieldName.ToLower() == "SERVICES".ToLower()).FirstOrDefault();

                            if (fielddata == null)
                            {
                                return JsonErrorResponse(s, "fail", "Please save the basic service info first for the image.");
                            }
                        }
                        else
                        {
                            fielddata = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1 && o.TblProjectFieldData_Id == projectFieldData.ID &&
                                        o.InitialFieldName.ToLower() == "ADDITIONAL SERVICES".ToLower()).FirstOrDefault();
                            if (fielddata == null)
                            {
                                return JsonErrorResponse(s, "fail", "Please save the basic service info first for the image.");
                            }
                        }


                        foreach (var item in imageFileNameList)
                        {

                            var obj = objCompassEntities.TblProjectFieldDataAttachments.Where(o => o.Active == 1 &&
                                o.tblProjectFieldDataMaster_ID == fielddata.ID && o.IsAudioVideoImage == 3 &&
                                o.strFilePath == item && o.tblService_Id == _serviceInitialId && o.TblServicePicture_ID == rv.ID
                                && o.isServiceData == true).FirstOrDefault();
                            if (obj == null)
                            {
                                obj = new TblProjectFieldDataAttachment();
                                obj.Active = 1;
                                obj.CreatedOn = new CommonFunctions().ServerDate();
                                obj.CreatedBy = _userId;
                                obj.tblProjectFieldDataMaster_ID = fielddata.ID;
                                obj.IsAudioVideoImage = 3;
                                obj.IsSkipped = false;
                                obj.strFilePath = item;

                                //obj.tblService_Id = !_isAdditionalServices ? _serviceInitialId : masterId;
                                obj.tblService_Id = _serviceInitialId;
                                obj.TblServicePicture_ID = rv.ID;
                                obj.isServiceData = true;
                                objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                                objCompassEntities.SaveChanges();
                            }
                        }

                        if (imageFileNameList.Count > 0)
                        {
                            log.Info("WEB API UploadServiceImageFiles Save Completed at - " + DateTime.Now.ToString());

                            ErrorModelOnlyStatus em = new ErrorModelOnlyStatus();
                            em.status = "Success";
                            var response = this.Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                            return response;

                        }
                        else
                        {
                            log.Info("WEB API UploadServiceImageFiles Save ERROR at - " + DateTime.Now.ToString() + "-Failed to save. Image with same name already exist");
                            return JsonErrorResponse(s, "fail", "Failed to save. Image with same name already exist.");

                        }


                        #endregion

                    }
                    else if (objForm.FormTypeID == 1)
                    {

                        #region Audit Process




                        var projectAuditFieldData = objCompassEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == _formId
                            && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                        if (projectAuditFieldData == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic info first for the image.");
                        }


                        ProjectAuditFieldDataMaster fielddata;
                        if (!_isAdditionalServices)
                        {
                            fielddata = objCompassEntities.ProjectAuditFieldDataMasters.Where(o => o.Active == 1 && o.FK_ProjectAuditData == projectAuditFieldData.ID &&
                                        o.InitialFieldName.ToLower() == "SERVICES".ToLower()).FirstOrDefault();

                            if (fielddata == null)
                            {
                                return JsonErrorResponse(s, "fail", "Please save the basic service info first for the image.");
                            }
                        }
                        else
                        {
                            fielddata = objCompassEntities.ProjectAuditFieldDataMasters.Where(o => o.Active == 1 && o.FK_ProjectAuditData == projectAuditFieldData.ID &&
                                        o.InitialFieldName.ToLower() == "ADDITIONAL SERVICES".ToLower()).FirstOrDefault();
                            if (fielddata == null)
                            {
                                return JsonErrorResponse(s, "fail", "Please save the basic service info first for the image.");
                            }
                        }


                        foreach (var item in imageFileNameList)
                        {

                            var obj = objCompassEntities.ProjectAuditFieldDataAttachments.Where(o => o.Active == 1 &&
                                o.FK_ProjectAuditFieldDataMaster_ID == fielddata.ID && o.IsAudioVideoImage == 3 &&
                                o.StrFilePath == item && o.Fk_tblService_Id == _serviceInitialId && o.FK_TblServicePicture_ID == rv.ID
                                && o.IsServiceData == true).FirstOrDefault();
                            if (obj == null)
                            {
                                obj = new ProjectAuditFieldDataAttachment();
                                obj.Active = 1;
                                obj.CreatedOn = new CommonFunctions().ServerDate();
                                obj.CreatedBy = _userId;
                                obj.FK_ProjectAuditFieldDataMaster_ID = fielddata.ID;
                                obj.IsAudioVideoImage = 3;
                                obj.IsSkipped = false;
                                obj.StrFilePath = item;

                                //obj.tblService_Id = !_isAdditionalServices ? _serviceInitialId : masterId;
                                obj.Fk_tblService_Id = _serviceInitialId;
                                obj.FK_TblServicePicture_ID = rv.ID;
                                obj.IsServiceData = true;
                                objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
                                objCompassEntities.SaveChanges();
                            }
                        }

                        if (imageFileNameList.Count > 0)
                        {
                            log.Info("WEB API UploadServiceImageFiles Save Completed at - " + DateTime.Now.ToString());

                            ErrorModelOnlyStatus em = new ErrorModelOnlyStatus();
                            em.status = "Success";
                            var response = this.Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                            return response;

                        }
                        else
                        {
                            log.Info("WEB API UploadServiceImageFiles Save ERROR at - " + DateTime.Now.ToString() + "-Failed to save. Image with same name already exist");
                            return JsonErrorResponse(s, "fail", "Failed to save. Image with same name already exist.");

                        }
                        #endregion

                    }
                    else
                    {
                        return JsonErrorResponse(s, "Error", "Error");
                    }

                }


            }
            catch (Exception ex)
            {

                log.Info("WEB API UploadServiceImageFiles Save ERROR at - " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));

                return JsonErrorResponse(s, "Error", ex.Message);

            }
        }

        #endregion

        #region SERVICE VIDEO CAPTURE
        [System.Web.Http.HttpPost]
        [ActionName("UploadServiceVideoFiles")]
        public HttpResponseMessage UploadServiceVideoFiles()
        {
            log.Info("WEB API Service  UploadServiceVideoFiles Started at - " + DateTime.Now.ToString());
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            try
            {
                string projectId = ""; string AccountNumber = "";
                string formId = ""; string accountInitialFieldId = "";
                int iUploadedCnt = 0; string serviceinitialId = "";
                string userId = ""; //string isAdditionalService = "";
                // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
                //string sPath = "";
                // sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");

                HttpFileCollection hfc = HttpContext.Current.Request.Files;

                userId = HttpContext.Current.Request.Form["userId"];
                formId = HttpContext.Current.Request.Form["formId"];
                accountInitialFieldId = HttpContext.Current.Request.Form["accountInitialFieldId"];
                serviceinitialId = HttpContext.Current.Request.Form["serviceInitialId"];
                log.Info("WEB API Service UploadServiceVideoFiles Requested by  - " + userId);
                //isAdditionalService = HttpContext.Current.Request.Form["isAdditionalService"];
                //initialFieldName = HttpContext.Current.Request.Form["initialFieldName"];
                long _formId = 0;// long.Parse(formId);
                long _serviceInitialId = 0;// long.Parse(serviceinitialId);
                int _userId = 0;// int.Parse(userId);
                long _accountInitialFieldId = 0;
                bool validParameters = true;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out _userId))
                {
                    return JsonErrorResponse(s, "fail", "userId field not valid.");
                }
                else if (string.IsNullOrEmpty(formId) || !long.TryParse(formId, out _formId))
                {
                    return JsonErrorResponse(s, "fail", "formId field not valid.");
                }
                else if (string.IsNullOrEmpty(accountInitialFieldId) || !long.TryParse(accountInitialFieldId, out _accountInitialFieldId))
                {
                    return JsonErrorResponse(s, "fail", "accountInitialFieldId field not valid.");
                }
                else if (string.IsNullOrEmpty(serviceinitialId) || !long.TryParse(serviceinitialId, out _serviceInitialId))
                {
                    return JsonErrorResponse(s, "fail", "serviceinitialId field not valid.");
                }

                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    string[] extension = hpf.FileName.Split('.');
                    if (extension.Count() < 2)
                    {
                        validParameters = false;
                    }
                    else
                    {
                        if (extension[1].ToLower() != "mp4")
                        {
                            validParameters = false;
                        }
                    }
                }

                if (!validParameters)
                {
                    return JsonErrorResponse(s, "fail", "only .mp4 files are allowded to upload");
                }
                else
                {
                    CompassEntities objCompassEntities = new CompassEntities();

                    projectId = Convert.ToString(objCompassEntities.tblFormMasters.
                        Where(o => o.FormId == _formId).Select(o => o.ProjectId).FirstOrDefault());


                    if (string.IsNullOrEmpty(projectId))
                    {
                        return JsonErrorResponse(s, "fail", "formId not present in the database.");
                    }


                    AccountNumber = Convert.ToString(objCompassEntities.UploadedExcelDatas.
                        Where(o => o.Id == _accountInitialFieldId).Select(o => o.Account12).FirstOrDefault());

                    if (string.IsNullOrEmpty(AccountNumber))
                    {
                        return JsonErrorResponse(s, "fail", "accountInitialFieldId not present in the database.");
                    }

                    string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
                        AccountNumber + @"\" + accountInitialFieldId + @"\";
                    string filePath = HttpContext.Current.Request.MapPath(tempPath);
                    grantAccess(filePath);

                    List<string> imageFileNameList = new List<string>();
                    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                    {
                        System.Web.HttpPostedFile hpf = hfc[iCnt];

                        if (hpf.ContentLength > 0)
                        {
                            //// CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                            //if (!File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                            //{
                            //    // SAVE THE FILES IN THE FOLDER.
                            //    hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                            //}

                            if (System.IO.File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                            {
                                System.IO.File.Delete(filePath + Path.GetFileName(hpf.FileName));
                            }
                            hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));


                            iUploadedCnt = iUploadedCnt + 1;
                            imageFileNameList.Add(tempPath + Path.GetFileName(hpf.FileName));

                        }
                    }

                    ///


                    var objForm = objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).FirstOrDefault();

                    if (objForm.FormTypeID == 2)
                    {
                        #region installation

                        foreach (var item in imageFileNameList)
                        {


                            var projectFieldData = objCompassEntities.TblProjectFieldDatas.Where(o => o.Active == 1 && o.FormId == _formId
                                && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                            if (projectFieldData == null)
                            {
                                return JsonErrorResponse(s, "fail", "Please save the basic info first for the video.");
                            }


                            var isAdditionalService = objCompassEntities.TblProjectMasterFieldRelations.Where(o => o.MasterTableName == "tblService"
                                && o.MasterFieldID == _serviceInitialId && o.Active == 1 && o.ProjectID.ToString() == projectId).Select(o => o.IsAdditionalService).FirstOrDefault();

                            //SERVICES    ADDITIONAL SERVICES
                            string fieldName = bool.Parse(Convert.ToString(isAdditionalService)) == true ? "ADDITIONAL SERVICES" : "SERVICES";

                            var fielddata = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1
                                && o.TblProjectFieldData_Id == projectFieldData.ID && o.InitialFieldName == fieldName).FirstOrDefault();

                            if (fielddata == null)
                            {
                                return JsonErrorResponse(s, "fail", "Please save the basic service info first for the video.");
                            }


                            var obj = objCompassEntities.TblProjectFieldDataAttachments.Where(o => o.Active == 1 &&
                               o.tblProjectFieldDataMaster_ID == fielddata.ID && o.IsAudioVideoImage == 2 &&
                               o.strFilePath == item && o.tblService_Id == _serviceInitialId
                               && o.isServiceData == true).FirstOrDefault();
                            if (obj == null)
                            {

                                obj = new TblProjectFieldDataAttachment();
                                obj.Active = 1;
                                obj.CreatedOn = new CommonFunctions().ServerDate();
                                obj.CreatedBy = _userId;
                                obj.tblProjectFieldDataMaster_ID = fielddata.ID;
                                obj.IsAudioVideoImage = 2;
                                obj.IsSkipped = false;
                                obj.strFilePath = item;
                                obj.tblService_Id = _serviceInitialId;
                                //obj.TblServicePicture_ID = rv.ID;
                                obj.isServiceData = true;
                                objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                                objCompassEntities.SaveChanges();
                            }

                        }
                        log.Info("WEB API UploadServiceVideoFiles Save completed at - " + DateTime.Now.ToString());

                        ErrorModelOnlyStatus em1 = new ErrorModelOnlyStatus();
                        em1.status = "Success";
                        var response1 = this.Request.CreateResponse(HttpStatusCode.OK);
                        response1.Content = new StringContent(s.Serialize(em1), Encoding.UTF8, "application/json");
                        return response1;

                        #endregion
                    }
                    else if (objForm.FormTypeID == 1)
                    {
                        #region Audit process

                        foreach (var item in imageFileNameList)
                        {


                            var projectAuditFieldData = objCompassEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == _formId
                                && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                            if (projectAuditFieldData == null)
                            {
                                return JsonErrorResponse(s, "fail", "Please save the basic info first for the video.");
                            }


                            var isAdditionalService = objCompassEntities.TblProjectMasterFieldRelations.Where(o => o.MasterTableName == "tblService"
                                && o.MasterFieldID == _serviceInitialId && o.Active == 1 && o.ProjectID.ToString() == projectId).Select(o => o.IsAdditionalService).FirstOrDefault();

                            //SERVICES    ADDITIONAL SERVICES
                            string fieldName = bool.Parse(Convert.ToString(isAdditionalService)) == true ? "ADDITIONAL SERVICES" : "SERVICES";

                            var fielddata = objCompassEntities.ProjectAuditFieldDataMasters.Where(o => o.Active == 1
                                && o.FK_ProjectAuditData == projectAuditFieldData.ID && o.InitialFieldName == fieldName).FirstOrDefault();

                            if (fielddata == null)
                            {
                                return JsonErrorResponse(s, "fail", "Please save the basic service info first for the video.");
                            }


                            var obj = objCompassEntities.ProjectAuditFieldDataAttachments.Where(o => o.Active == 1 &&
                               o.FK_ProjectAuditFieldDataMaster_ID == fielddata.ID && o.IsAudioVideoImage == 2 &&
                               o.StrFilePath == item && o.Fk_tblService_Id == _serviceInitialId
                               && o.IsServiceData == true).FirstOrDefault();
                            if (obj == null)
                            {

                                obj = new ProjectAuditFieldDataAttachment();
                                obj.Active = 1;
                                obj.CreatedOn = new CommonFunctions().ServerDate();
                                obj.CreatedBy = _userId;
                                obj.FK_ProjectAuditFieldDataMaster_ID = fielddata.ID;
                                obj.IsAudioVideoImage = 2;
                                obj.IsSkipped = false;
                                obj.StrFilePath = item;
                                obj.Fk_tblService_Id = _serviceInitialId;
                                //obj.TblServicePicture_ID = rv.ID;
                                obj.IsServiceData = true;
                                objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
                                objCompassEntities.SaveChanges();
                            }

                        }
                        log.Info("WEB API UploadServiceVideoFiles Save completed at - " + DateTime.Now.ToString());

                        ErrorModelOnlyStatus em1 = new ErrorModelOnlyStatus();
                        em1.status = "Success";
                        var response1 = this.Request.CreateResponse(HttpStatusCode.OK);
                        response1.Content = new StringContent(s.Serialize(em1), Encoding.UTF8, "application/json");
                        return response1;

                        #endregion
                    }
                    else
                    {
                        return JsonErrorResponse(s, "Error", "Error");
                    }

                }


            }
            catch (Exception ex)
            {

                log.Info("WEB API UploadServiceVideoFiles Save ERROR at - " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));

                return JsonErrorResponse(s, "Error", ex.Message);
            }
        }

        #endregion

        #region SERVICE AUDIO CAPTURE
        [System.Web.Http.HttpPost]
        [ActionName("UploadServiceAudioFiles")]
        public HttpResponseMessage UploadServiceAudioFiles()
        {
            log.Info("WEB API Service  UploadServiceAudioFiles Started at - " + DateTime.Now.ToString());
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            try
            {
                string projectId = "";
                string AccountNumber = "";
                string formId = "";
                string accountInitialFieldId = "";
                int iUploadedCnt = 0;
                string serviceinitialId = "";
                string userId = "";

                HttpFileCollection hfc = HttpContext.Current.Request.Files;

                userId = HttpContext.Current.Request.Form["userId"];
                formId = HttpContext.Current.Request.Form["formId"];
                accountInitialFieldId = HttpContext.Current.Request.Form["accountInitialFieldId"];
                serviceinitialId = HttpContext.Current.Request.Form["serviceInitialId"];
                log.Info("WEB API Service UploadServiceAudioFiles Requested by  - " + userId);
                //initialFieldName = HttpContext.Current.Request.Form["initialFieldName"];
                long _formId = 0;
                long _serviceInitialId = 0;
                int _userId = 0;
                long _accountInitialFieldId = 0;
                bool validParameters = true;



                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out _userId))
                {
                    return JsonErrorResponse(s, "fail", "userId field not valid.");
                }
                else if (string.IsNullOrEmpty(formId) || !long.TryParse(formId, out _formId))
                {
                    return JsonErrorResponse(s, "fail", "formId field not valid.");
                }
                else if (string.IsNullOrEmpty(accountInitialFieldId) || !long.TryParse(accountInitialFieldId, out _accountInitialFieldId))
                {
                    return JsonErrorResponse(s, "fail", "accountInitialFieldId field not valid.");
                }
                else if (string.IsNullOrEmpty(serviceinitialId) || !long.TryParse(serviceinitialId, out _serviceInitialId))
                {
                    return JsonErrorResponse(s, "fail", "serviceinitialId field not valid.");
                }



                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    string[] extension = hpf.FileName.Split('.');
                    if (extension.Count() < 2)
                    {
                        validParameters = false;
                    }
                    else
                    {
                        if (extension[1].ToLower() != "m4a")
                        {
                            validParameters = false;
                        }
                    }
                }

                if (!validParameters)
                {
                    return JsonErrorResponse(s, "fail", "only .m4a files are allowded to upload");
                }
                else
                {
                    CompassEntities objCompassEntities = new CompassEntities();

                    projectId = Convert.ToString(objCompassEntities.tblFormMasters.
                        Where(o => o.FormId == _formId).Select(o => o.ProjectId).FirstOrDefault());

                    if (string.IsNullOrEmpty(projectId))
                    {
                        return JsonErrorResponse(s, "fail", "formId not present in the database.");
                    }

                    AccountNumber = Convert.ToString(objCompassEntities.UploadedExcelDatas.
                        Where(o => o.Id == _accountInitialFieldId).Select(o => o.Account12).FirstOrDefault());

                    if (string.IsNullOrEmpty(AccountNumber))
                    {
                        return JsonErrorResponse(s, "fail", "accountInitialFieldId not present in the database.");
                    }


                    string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" + AccountNumber + @"\" + accountInitialFieldId + @"\";
                    string filePath = HttpContext.Current.Request.MapPath(tempPath);
                    grantAccess(filePath);

                    List<string> imageFileNameList = new List<string>();
                    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                    {
                        System.Web.HttpPostedFile hpf = hfc[iCnt];

                        if (hpf.ContentLength > 0)
                        {
                            //// CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                            //if (!File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                            //{
                            //    // SAVE THE FILES IN THE FOLDER.
                            //    hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                            //}

                            if (System.IO.File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                            {
                                System.IO.File.Delete(filePath + Path.GetFileName(hpf.FileName));
                            }
                            hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                            iUploadedCnt = iUploadedCnt + 1;
                            imageFileNameList.Add(tempPath + Path.GetFileName(hpf.FileName));

                        }
                    }


                    var objForm = objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).FirstOrDefault();

                    if (objForm.FormTypeID == 2)
                    {
                        #region installation

                        foreach (var item in imageFileNameList)
                        {


                            var projectFieldData = objCompassEntities.TblProjectFieldDatas.Where(o => o.Active == 1 && o.FormId == _formId
                                && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                            if (projectFieldData == null)
                            {
                                return JsonErrorResponse(s, "fail", "Please save the basic info first for the audio.");
                            }

                            var isAdditionalService = objCompassEntities.TblProjectMasterFieldRelations.Where(o => o.MasterTableName == "tblService"
                             && o.MasterFieldID == _serviceInitialId && o.Active == 1 && o.ProjectID.ToString() == projectId).Select(o => o.IsAdditionalService).FirstOrDefault();

                            //SERVICES    ADDITIONAL SERVICES
                            string fieldName = bool.Parse(Convert.ToString(isAdditionalService)) == true ? "ADDITIONAL SERVICES" : "SERVICES";


                            var fielddata = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1
                                && o.TblProjectFieldData_Id == projectFieldData.ID && o.InitialFieldName == fieldName).FirstOrDefault();

                            if (fielddata == null)
                            {
                                return JsonErrorResponse(s, "fail", "Please save the basic service info first for the audio.");
                            }

                            var obj = objCompassEntities.TblProjectFieldDataAttachments.Where(o => o.Active == 1 &&
                               o.tblProjectFieldDataMaster_ID == fielddata.ID && o.IsAudioVideoImage == 1 &&
                               o.strFilePath == item && o.tblService_Id == _serviceInitialId
                               && o.isServiceData == true).FirstOrDefault();
                            if (obj == null)
                            {
                                obj = new TblProjectFieldDataAttachment();
                                obj.Active = 1;
                                obj.CreatedOn = new CommonFunctions().ServerDate();
                                obj.CreatedBy = _userId;
                                obj.tblProjectFieldDataMaster_ID = fielddata.ID;
                                obj.IsAudioVideoImage = 1;
                                obj.IsSkipped = false;
                                obj.strFilePath = item;
                                obj.tblService_Id = _serviceInitialId;
                                //obj.TblServicePicture_ID = rv.ID;
                                obj.isServiceData = true;
                                objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                                objCompassEntities.SaveChanges();
                            }

                        }



                        log.Info("WEB API UploadServiceAudioFiles Save Complete at - " + DateTime.Now.ToString());

                        ErrorModelOnlyStatus em1 = new ErrorModelOnlyStatus();
                        em1.status = "Success";
                        var response1 = this.Request.CreateResponse(HttpStatusCode.OK);
                        response1.Content = new StringContent(s.Serialize(em1), Encoding.UTF8, "application/json");
                        return response1;

                        #endregion
                    }
                    else if (objForm.FormTypeID == 1)
                    {
                        #region Audit process

                        foreach (var item in imageFileNameList)
                        {


                            var projectAuditFieldData = objCompassEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == _formId
                                && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                            if (projectAuditFieldData == null)
                            {
                                return JsonErrorResponse(s, "fail", "Please save the basic info first for the audio.");
                            }

                            var isAdditionalService = objCompassEntities.TblProjectMasterFieldRelations.Where(o => o.MasterTableName == "tblService"
                             && o.MasterFieldID == _serviceInitialId && o.Active == 1 && o.ProjectID.ToString() == projectId).Select(o => o.IsAdditionalService).FirstOrDefault();

                            //SERVICES    ADDITIONAL SERVICES
                            string fieldName = bool.Parse(Convert.ToString(isAdditionalService)) == true ? "ADDITIONAL SERVICES" : "SERVICES";


                            var fielddata = objCompassEntities.ProjectAuditFieldDataMasters.Where(o => o.Active == 1
                                && o.FK_ProjectAuditData == projectAuditFieldData.ID && o.InitialFieldName == fieldName).FirstOrDefault();

                            if (fielddata == null)
                            {
                                return JsonErrorResponse(s, "fail", "Please save the basic service info first for the audio.");
                            }

                            var obj = objCompassEntities.ProjectAuditFieldDataAttachments.Where(o => o.Active == 1 &&
                               o.FK_ProjectAuditFieldDataMaster_ID == fielddata.ID && o.IsAudioVideoImage == 1 &&
                               o.StrFilePath == item && o.Fk_tblService_Id == _serviceInitialId
                               && o.IsServiceData == true).FirstOrDefault();
                            if (obj == null)
                            {
                                obj = new ProjectAuditFieldDataAttachment();
                                obj.Active = 1;
                                obj.CreatedOn = new CommonFunctions().ServerDate();
                                obj.CreatedBy = _userId;
                                obj.FK_ProjectAuditFieldDataMaster_ID = fielddata.ID;
                                obj.IsAudioVideoImage = 1;
                                obj.IsSkipped = false;
                                obj.StrFilePath = item;
                                obj.Fk_tblService_Id = _serviceInitialId;
                                //obj.TblServicePicture_ID = rv.ID;
                                obj.IsServiceData = true;
                                objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
                                objCompassEntities.SaveChanges();
                            }

                        }

                        log.Info("WEB API UploadServiceAudioFiles Save Complete at - " + DateTime.Now.ToString());

                        ErrorModelOnlyStatus em1 = new ErrorModelOnlyStatus();
                        em1.status = "Success";
                        var response1 = this.Request.CreateResponse(HttpStatusCode.OK);
                        response1.Content = new StringContent(s.Serialize(em1), Encoding.UTF8, "application/json");
                        return response1;
                        #endregion
                    }
                    else
                    {
                        return JsonErrorResponse(s, "Error", "Error");
                    }

                }
            }
            catch (Exception ex)
            {

                log.Info("WEB API UploadServiceAudioFiles Save ERROR at - " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));

                return JsonErrorResponse(s, "Error", ex.Message);
            }

        }

        #endregion


        public static bool grantAccess(string _direcotryPath)
        {
            bool isSucessed = false;
            try
            {
                string folderPath = _direcotryPath;
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                DirectoryInfo dInfo = new DirectoryInfo(folderPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule("everyone", FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
                isSucessed = true;
            }

            catch (Exception ex) { throw ex; }
            return isSucessed;
        }



        #region CUSTOM IMAGE CAPTURE
        [System.Web.Http.HttpPost]
        [ActionName("UploadCustomImageFiles")]
        public HttpResponseMessage UploadCustomImageFiles()
        {

            log.Info("WEB API Service  UploadCustomImageFiles Started at - " + DateTime.Now.ToString());

            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            try
            {
                string projectId = ""; string AccountNumber = "";
                string formId = ""; string accountInitialFieldId = "";
                int iUploadedCnt = 0;
                string fieldName = ""; string userId = "";

                HttpFileCollection hfc = HttpContext.Current.Request.Files;
                

                userId = HttpContext.Current.Request.Form["userId"];
                log.Info("WEB API Service UploadCustomImageFiles Requested by  - " + userId);
                formId = HttpContext.Current.Request.Form["formId"];
                accountInitialFieldId = HttpContext.Current.Request.Form["accountInitialFieldId"];
                fieldName = HttpContext.Current.Request.Form["FieldName"];


                long _formId = 0;
                int _userId = 0; long _accountInitialFieldId = 0;
                bool validParameters = true;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out _userId))
                {
                    return JsonErrorResponse(s, "fail", "userId field not valid.");
                }
                else if (string.IsNullOrEmpty(formId) || !long.TryParse(formId, out _formId))
                {
                    return JsonErrorResponse(s, "fail", "formId field not valid.");
                }
                else if (string.IsNullOrEmpty(accountInitialFieldId) || !long.TryParse(accountInitialFieldId, out _accountInitialFieldId))
                {
                    return JsonErrorResponse(s, "fail", "accountInitialFieldId field not valid.");
                }
                else if (string.IsNullOrEmpty(fieldName))
                {
                    return JsonErrorResponse(s, "fail", "fieldName field not valid.");
                }


                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    string[] extension = hpf.FileName.Split('.');
                    if (extension.Count() < 2)
                    {
                        validParameters = false;
                    }
                    else
                    {
                        if (extension[1].ToLower() != "jpeg" && extension[1].ToLower() != "jpg")
                        {
                            validParameters = false;
                        }
                    }
                }

                if (!validParameters)
                {
                    return JsonErrorResponse(s, "fail", "only .jpeg/.jpg images allowed.");
                }
                else
                {

                    CompassEntities objCompassEntities = new CompassEntities();

                    projectId = Convert.ToString(objCompassEntities.tblFormMasters.
                       Where(o => o.FormId == _formId).Select(o => o.ProjectId).FirstOrDefault());

                    if (string.IsNullOrEmpty(projectId))
                    {
                        return JsonErrorResponse(s, "fail", "formId not present in the database.");
                    }

                    AccountNumber = Convert.ToString(objCompassEntities.UploadedExcelDatas.
                       Where(o => o.Id == _accountInitialFieldId).Select(o => o.Account12).FirstOrDefault());
                    if (string.IsNullOrEmpty(AccountNumber))
                    {
                        return JsonErrorResponse(s, "fail", "accountInitialFieldId not present in the database.");
                    }



                    var objForm = objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).FirstOrDefault();

                    if (objForm.FormTypeID == 2)
                    {
                        #region installation

                        var projectFieldData = objCompassEntities.TblProjectFieldDatas.Where(o => o.Active == 1 && o.FormId == _formId
                        && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                        if (projectFieldData == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic info first for the image.");
                        }



                        var fielddata = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1
                            && o.TblProjectFieldData_Id == projectFieldData.ID &&
                            o.InitialFieldName.ToLower() == fieldName.ToLower()).FirstOrDefault();

                        if (fielddata == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic service info first for the image.");
                        }


                        string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
                            AccountNumber + @"\" + accountInitialFieldId + @"\" + fieldName + @"\";
                        string filePath = HttpContext.Current.Request.MapPath(tempPath);
                        grantAccess(filePath);

                        List<string> imageFileNameList = new List<string>();

                        //string keyName = "updatedtestfile.txt";
                        string bucketName = ConfigurationManager.AppSettings["BucketName"];
                        RegionEndpoint bucketRegion = RegionEndpoint.USWest2;
                        string accesskey = ConfigurationManager.AppSettings["AWSAccessKey"];
                        string secretkey = ConfigurationManager.AppSettings["AWSSecretKey"];

                        for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                        {
                            System.Web.HttpPostedFile hpf = hfc[iCnt];

                            var s3Client = new AmazonS3Client(accesskey, secretkey, bucketRegion);
                            var fileTransferUtility = new TransferUtility(s3Client);
                            try
                            {
                                if (hpf.ContentLength > 0)
                                {
                                    //filePath = Path.Combine(Server.MapPath("~/Files"), Path.GetFileName(hpf.FileName));
                                    filePath = System.IO.Path.Combine(tempPath + Path.GetFileName(hpf.FileName));
                                    var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                                    {
                                        BucketName = bucketName,
                                        FilePath = filePath,
                                        StorageClass = S3StorageClass.StandardInfrequentAccess,
                                        //PartSize = 6291456, // 6 MB.  
                                        Key = hpf.FileName,
                                        CannedACL = S3CannedACL.PublicRead
                                    };
                                    //fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                                    //fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
                                    fileTransferUtility.Upload(fileTransferUtilityRequest);
                                    var uploadfileurl = fileTransferUtilityRequest.FilePath;
                                    fileTransferUtility.Dispose();
                                }
                                //ViewBag.Message = "File Uploaded Successfully!!";
                            }
                            catch (AmazonS3Exception amazonS3Exception)
                            {
                                if (amazonS3Exception.ErrorCode != null &&
                                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                                    ||
                                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                                {
                                    //ViewBag.Message = "Check the provided AWS Credentials.";
                                }
                                else
                                {
                                    //ViewBag.Message = "Error occurred: " + amazonS3Exception.Message;
                                }
                            }

                            

                            if (hpf.ContentLength > 0)
                            {
                                //// CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                                //if (!File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                                //{
                                //    // SAVE THE FILES IN THE FOLDER.
                                //    hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                                //}

                                if (System.IO.File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                                {
                                    System.GC.Collect();
                                    System.GC.WaitForPendingFinalizers();

                                    System.IO.File.Delete(filePath + Path.GetFileName(hpf.FileName));

                                }
                                hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));


                                string _imagePath = filePath + Path.GetFileName(hpf.FileName);
                                // Rotate the image according to EXIF data
                                var bmp = new Bitmap(_imagePath);

                                //if (System.IO.File.Exists(_imagePath))
                                //{
                                //    System.IO.File.Delete(_imagePath);
                                //}

                                var exif = new EXIFextractor(ref bmp, "n");

                                if (exif["Orientation"] != null)
                                {
                                    RotateFlipType flip = OrientationToFlipType(exif["Orientation"].ToString());

                                    if (flip != RotateFlipType.RotateNoneFlipNone) // don't flip of orientation is correct
                                    {
                                        bmp.RotateFlip(flip);
                                        exif.setTag(0x112, "1"); // Optional: reset orientation tag
                                        bmp.Save(_imagePath, ImageFormat.Jpeg);
                                    }
                                }


                                iUploadedCnt = iUploadedCnt + 1;
                                imageFileNameList.Add(tempPath + Path.GetFileName(hpf.FileName));

                            }
                        }



                        foreach (var item in imageFileNameList)
                        {
                            var obj = objCompassEntities.TblProjectFieldDataAttachments.Where(o => o.Active == 1 &&
                               o.tblProjectFieldDataMaster_ID == fielddata.ID && o.IsAudioVideoImage == 3 &&
                               o.strFilePath == item && o.isServiceData == false).FirstOrDefault();
                            if (obj == null)
                            {
                                obj = new TblProjectFieldDataAttachment();
                                obj.Active = 1;
                                obj.CreatedOn = new CommonFunctions().ServerDate();
                                obj.CreatedBy = _userId;
                                obj.tblProjectFieldDataMaster_ID = fielddata.ID;
                                obj.IsAudioVideoImage = 3;
                                obj.IsSkipped = false;
                                obj.strFilePath = item;
                                //obj.tblService_Id = _serviceInitialId;
                                //obj.TblServicePicture_ID = rv.ID;
                                obj.isServiceData = false;
                                objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                                objCompassEntities.SaveChanges();
                            }
                        }


                        log.Info("WEB API UploadCustomImageFiles Save Completed at - " + DateTime.Now.ToString());


                        ErrorModelOnlyStatus em1 = new ErrorModelOnlyStatus();
                        em1.status = "Success";
                        var response1 = this.Request.CreateResponse(HttpStatusCode.OK);
                        response1.Content = new StringContent(s.Serialize(em1), Encoding.UTF8, "application/json");
                        return response1;

                        #endregion
                    }
                    else if (objForm.FormTypeID == 1)
                    {
                        #region Audit process

                        var projectAuditFieldData = objCompassEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == _formId
                        && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                        if (projectAuditFieldData == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic info first for the image.");
                        }



                        var fielddata = objCompassEntities.ProjectAuditFieldDataMasters.Where(o => o.Active == 1
                            && o.FK_ProjectAuditData == projectAuditFieldData.ID && o.InitialFieldName.ToLower() == fieldName.ToLower()).FirstOrDefault();

                        if (fielddata == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic service info first for the image.");
                        }


                        string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" + AccountNumber + @"\" + accountInitialFieldId + @"\" + fieldName + @"\";
                        string filePath = HttpContext.Current.Request.MapPath(tempPath);
                        grantAccess(filePath);

                        List<string> imageFileNameList = new List<string>();
                        for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                        {
                            System.Web.HttpPostedFile hpf = hfc[iCnt];

                            if (hpf.ContentLength > 0)
                            {
                                //// CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                                //if (!File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                                //{
                                //    // SAVE THE FILES IN THE FOLDER.
                                //    hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                                //}

                                if (System.IO.File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                                {
                                    System.GC.Collect();
                                    System.GC.WaitForPendingFinalizers();

                                    System.IO.File.Delete(filePath + Path.GetFileName(hpf.FileName));

                                }
                                hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));


                                string _imagePath = filePath + Path.GetFileName(hpf.FileName);
                                // Rotate the image according to EXIF data
                                var bmp = new Bitmap(_imagePath);

                                //if (System.IO.File.Exists(_imagePath))
                                //{
                                //    System.IO.File.Delete(_imagePath);
                                //}

                                var exif = new EXIFextractor(ref bmp, "n");

                                if (exif["Orientation"] != null)
                                {
                                    RotateFlipType flip = OrientationToFlipType(exif["Orientation"].ToString());

                                    if (flip != RotateFlipType.RotateNoneFlipNone) // don't flip of orientation is correct
                                    {
                                        bmp.RotateFlip(flip);
                                        exif.setTag(0x112, "1"); // Optional: reset orientation tag
                                        bmp.Save(_imagePath, ImageFormat.Jpeg);
                                    }
                                }


                                iUploadedCnt = iUploadedCnt + 1;
                                imageFileNameList.Add(tempPath + Path.GetFileName(hpf.FileName));

                            }
                        }



                        foreach (var item in imageFileNameList)
                        {
                            var obj = objCompassEntities.ProjectAuditFieldDataAttachments.Where(o => o.Active == 1 &&
                               o.FK_ProjectAuditFieldDataMaster_ID == fielddata.ID && o.IsAudioVideoImage == 3 &&
                               o.StrFilePath == item && o.IsServiceData == false).FirstOrDefault();
                            if (obj == null)
                            {
                                obj = new ProjectAuditFieldDataAttachment();
                                obj.Active = 1;
                                obj.CreatedOn = new CommonFunctions().ServerDate();
                                obj.CreatedBy = _userId;
                                obj.FK_ProjectAuditFieldDataMaster_ID = fielddata.ID;
                                obj.IsAudioVideoImage = 3;
                                obj.IsSkipped = false;
                                obj.StrFilePath = item;
                                //obj.tblService_Id = _serviceInitialId;
                                //obj.TblServicePicture_ID = rv.ID;
                                obj.IsServiceData = false;
                                objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
                                objCompassEntities.SaveChanges();
                            }
                        }


                        log.Info("WEB API UploadCustomImageFiles Save Completed at - " + DateTime.Now.ToString());


                        ErrorModelOnlyStatus em1 = new ErrorModelOnlyStatus();
                        em1.status = "Success";
                        var response1 = this.Request.CreateResponse(HttpStatusCode.OK);
                        response1.Content = new StringContent(s.Serialize(em1), Encoding.UTF8, "application/json");
                        return response1;
                        #endregion
                    }
                    else
                    {
                        return JsonErrorResponse(s, "Error", "Error");
                    }



                }


            }
            catch (Exception ex)
            {
                log.Info("WEB API UploadCustomImageFiles Save ERROR at - " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));

                return JsonErrorResponse(s, "fail", ex.Message);
            }
        }

        #endregion


        #region CUSTOM AUDIO CAPTURE
        [System.Web.Http.HttpPost]
        [ActionName("UploadCustomAudioFiles")]
        public HttpResponseMessage UploadCustomAudioFiles()
        {

            log.Info("WEB API Service  UploadCustomAudioFiles Started at - " + DateTime.Now.ToString());

            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            try
            {
                string projectId = ""; string AccountNumber = "";
                string formId = ""; string accountInitialFieldId = "";
                int iUploadedCnt = 0;
                string userId = ""; string fieldName = "";

                HttpFileCollection hfc = HttpContext.Current.Request.Files;

                userId = HttpContext.Current.Request.Form["userId"];
                formId = HttpContext.Current.Request.Form["formId"];
                accountInitialFieldId = HttpContext.Current.Request.Form["accountInitialFieldId"];
                fieldName = HttpContext.Current.Request.Form["FieldName"];
                log.Info("WEB API Service UploadCustomAudioFiles Requested by  - " + userId);

                long _formId = 0; int _userId = 0; long _accountInitialFieldId = 0;
                bool validParameters = true;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out _userId))
                {
                    return JsonErrorResponse(s, "fail", "userId field not valid.");
                }
                else if (string.IsNullOrEmpty(formId) || !long.TryParse(formId, out _formId))
                {
                    return JsonErrorResponse(s, "fail", "formId field not valid.");
                }
                else if (string.IsNullOrEmpty(accountInitialFieldId) || !long.TryParse(accountInitialFieldId, out _accountInitialFieldId))
                {
                    return JsonErrorResponse(s, "fail", "accountInitialFieldId field not valid.");
                }
                else if (string.IsNullOrEmpty(fieldName))
                {
                    return JsonErrorResponse(s, "fail", "fieldName field not valid.");
                }


                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    string[] extension = hpf.FileName.Split('.');
                    if (extension.Count() < 2)
                    {
                        validParameters = false;
                    }
                    else
                    {
                        if (extension[1].ToLower() != "m4a")
                        {
                            validParameters = false;
                        }
                    }
                }

                if (!validParameters)
                {
                    return JsonErrorResponse(s, "fail", "only .m4a files are allowded to upload.");
                }
                else
                {
                    CompassEntities objCompassEntities = new CompassEntities();

                    projectId = Convert.ToString(objCompassEntities.tblFormMasters.
                        Where(o => o.FormId == _formId).Select(o => o.ProjectId).FirstOrDefault());

                    if (string.IsNullOrEmpty(projectId))
                    {
                        return JsonErrorResponse(s, "fail", "formId not present in the database.");
                    }

                    AccountNumber = Convert.ToString(objCompassEntities.UploadedExcelDatas.
                        Where(o => o.Id == _accountInitialFieldId).Select(o => o.Account12).FirstOrDefault());

                    if (string.IsNullOrEmpty(AccountNumber))
                    {
                        return JsonErrorResponse(s, "fail", "accountInitialFieldId not present in the database.");
                    }



                    var objForm = objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).FirstOrDefault();

                    if (objForm.FormTypeID == 2)
                    {
                        #region installation


                        var projectFieldData = objCompassEntities.TblProjectFieldDatas.Where(o => o.Active == 1 && o.FormId == _formId
                              && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                        if (projectFieldData == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic info first for the audio.");
                        }


                        var fielddata = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1
                            && o.TblProjectFieldData_Id == projectFieldData.ID).FirstOrDefault();

                        if (fielddata == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic service info first for the audio.");
                        }


                        string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
                            AccountNumber + @"\" + accountInitialFieldId + @"\";
                        string filePath = HttpContext.Current.Request.MapPath(tempPath);
                        grantAccess(filePath);

                        List<string> imageFileNameList = new List<string>();
                        for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                        {
                            System.Web.HttpPostedFile hpf = hfc[iCnt];

                            if (hpf.ContentLength > 0)
                            {
                                //// CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                                //if (!File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                                //{
                                //    // SAVE THE FILES IN THE FOLDER.
                                //    hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                                //}

                                if (System.IO.File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                                {
                                    System.IO.File.Delete(filePath + Path.GetFileName(hpf.FileName));
                                }
                                hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                                iUploadedCnt = iUploadedCnt + 1;
                                imageFileNameList.Add(tempPath + Path.GetFileName(hpf.FileName));

                            }
                        }

                        foreach (var item in imageFileNameList)
                        {

                            var obj = objCompassEntities.TblProjectFieldDataAttachments.Where(o => o.Active == 1 &&
                               o.tblProjectFieldDataMaster_ID == fielddata.ID && o.IsAudioVideoImage == 1 &&
                               o.strFilePath == item && o.isServiceData == false).FirstOrDefault();
                            if (obj == null)
                            {

                                obj = new TblProjectFieldDataAttachment();
                                obj.Active = 1;
                                obj.CreatedOn = new CommonFunctions().ServerDate();
                                obj.CreatedBy = _userId;
                                obj.tblProjectFieldDataMaster_ID = fielddata.ID;
                                obj.IsAudioVideoImage = 1;
                                obj.IsSkipped = false;
                                obj.strFilePath = item;
                                // obj.tblService_Id = _serviceInitialId;
                                //obj.TblServicePicture_ID = rv.ID;
                                obj.isServiceData = false;
                                objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                                objCompassEntities.SaveChanges();
                            }

                        }

                        if (imageFileNameList.Count > 0)
                        {
                            log.Info("WEB API UploadCustomAudioFiles Save Complete at - " + DateTime.Now.ToString());
                            ErrorModelOnlyStatus em = new ErrorModelOnlyStatus();
                            em.status = "Success";
                            var response = this.Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                            return response;
                        }
                        else
                        {
                            log.Info("WEB API UploadCustomAudioFiles Save ERROR at - " + DateTime.Now.ToString() + "- Failed to save. Audio file with same name already exist.");
                            return JsonErrorResponse(s, "fail", "Failed to save. Audio file with same name already exist.");
                        }

                        #endregion
                    }
                    else if (objForm.FormTypeID == 1)
                    {
                        #region Audit process


                        var projectAuditFieldData = objCompassEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == _formId
                              && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                        if (projectAuditFieldData == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic info first for the audio.");
                        }


                        var fielddata = objCompassEntities.ProjectAuditFieldDataMasters.Where(o => o.Active == 1
                            && o.FK_ProjectAuditData == projectAuditFieldData.ID).FirstOrDefault();

                        if (fielddata == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic service info first for the audio.");
                        }


                        string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" + AccountNumber + @"\" + accountInitialFieldId + @"\";
                        string filePath = HttpContext.Current.Request.MapPath(tempPath);
                        grantAccess(filePath);

                        List<string> imageFileNameList = new List<string>();
                        for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                        {
                            System.Web.HttpPostedFile hpf = hfc[iCnt];

                            if (hpf.ContentLength > 0)
                            {
                                //// CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                                //if (!File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                                //{
                                //    // SAVE THE FILES IN THE FOLDER.
                                //    hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                                //}

                                if (System.IO.File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                                {
                                    System.IO.File.Delete(filePath + Path.GetFileName(hpf.FileName));
                                }
                                hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                                iUploadedCnt = iUploadedCnt + 1;
                                imageFileNameList.Add(tempPath + Path.GetFileName(hpf.FileName));

                            }
                        }

                        foreach (var item in imageFileNameList)
                        {

                            var obj = objCompassEntities.ProjectAuditFieldDataAttachments.Where(o => o.Active == 1 &&
                               o.FK_ProjectAuditFieldDataMaster_ID == fielddata.ID && o.IsAudioVideoImage == 1 &&
                               o.StrFilePath == item && o.IsServiceData == false).FirstOrDefault();
                            if (obj == null)
                            {

                                obj = new ProjectAuditFieldDataAttachment();
                                obj.Active = 1;
                                obj.CreatedOn = new CommonFunctions().ServerDate();
                                obj.CreatedBy = _userId;
                                obj.FK_ProjectAuditFieldDataMaster_ID = fielddata.ID;
                                obj.IsAudioVideoImage = 1;
                                obj.IsSkipped = false;
                                obj.StrFilePath = item;
                                // obj.tblService_Id = _serviceInitialId;
                                //obj.TblServicePicture_ID = rv.ID;
                                obj.IsServiceData = false;
                                objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
                                objCompassEntities.SaveChanges();
                            }

                        }

                        if (imageFileNameList.Count > 0)
                        {
                            log.Info("WEB API UploadCustomAudioFiles Save Complete at - " + DateTime.Now.ToString());
                            ErrorModelOnlyStatus em = new ErrorModelOnlyStatus();
                            em.status = "Success";
                            var response = this.Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                            return response;
                        }
                        else
                        {
                            log.Info("WEB API UploadCustomAudioFiles Save ERROR at - " + DateTime.Now.ToString() + "- Failed to save. Audio file with same name already exist.");
                            return JsonErrorResponse(s, "fail", "Failed to save. Audio file with same name already exist.");
                        }
                        #endregion
                    }
                    else
                    {
                        return JsonErrorResponse(s, "Error", "Error");
                    }


                }
            }
            catch (Exception ex)
            {

                log.Info("WEB API UploadCustomAudioFiles Save ERROR at - " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                return JsonErrorResponse(s, "fail", ex.Message);

            }
        }

        #endregion


        #region CUSTOM VIDEO CAPTURE
        [System.Web.Http.HttpPost]
        [ActionName("UploadCustomVideoFiles")]
        public HttpResponseMessage UploadCustomVideoFiles()
        {
            log.Info("WEB API Service  UploadCustomVideoFiles Started at - " + DateTime.Now.ToString());


            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            try
            {
                string projectId = ""; string AccountNumber = "";
                string formId = ""; string accountInitialFieldId = "";
                int iUploadedCnt = 0;
                string userId = ""; string fieldName = "";

                HttpFileCollection hfc = HttpContext.Current.Request.Files;

                userId = HttpContext.Current.Request.Form["userId"];
                formId = HttpContext.Current.Request.Form["formId"];
                accountInitialFieldId = HttpContext.Current.Request.Form["accountInitialFieldId"];
                fieldName = HttpContext.Current.Request.Form["FieldName"];

                log.Info("WEB API Service UploadCustomVideoFiles Requested by  - " + userId);
                long _formId = 0; int _userId = 0; long _accountInitialFieldId = 0;
                bool validParameters = true;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out _userId))
                {
                    return JsonErrorResponse(s, "fail", "userId field not valid.");
                }
                else if (string.IsNullOrEmpty(formId) || !long.TryParse(formId, out _formId))
                {
                    return JsonErrorResponse(s, "fail", "formId field not valid.");
                }
                else if (string.IsNullOrEmpty(accountInitialFieldId) || !long.TryParse(accountInitialFieldId, out _accountInitialFieldId))
                {
                    return JsonErrorResponse(s, "fail", "accountInitialFieldId field not valid.");
                }
                else if (string.IsNullOrEmpty(fieldName))
                {
                    return JsonErrorResponse(s, "fail", "fieldName field not valid.");
                }

                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    string[] extension = hpf.FileName.Split('.');
                    if (extension.Count() < 2)
                    {
                        validParameters = false;
                    }
                    else
                    {
                        if (extension[1].ToLower() != "mp4")
                        {
                            validParameters = false;
                        }
                    }
                }

                if (!validParameters)
                {
                    return JsonErrorResponse(s, "fail", "only .mp4 files are allowded to upload.");
                }
                else
                {
                    CompassEntities objCompassEntities = new CompassEntities();

                    projectId = Convert.ToString(objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).Select(o => o.ProjectId).FirstOrDefault());


                    if (string.IsNullOrEmpty(projectId))
                    {
                        return JsonErrorResponse(s, "fail", "formId not present in the database.");
                    }


                    AccountNumber = Convert.ToString(objCompassEntities.UploadedExcelDatas.Where(o => o.Id == _accountInitialFieldId).Select(o => o.Account12).FirstOrDefault());

                    if (string.IsNullOrEmpty(AccountNumber))
                    {
                        return JsonErrorResponse(s, "fail", "accountInitialFieldId not present in the database.");
                    }


                    var objForm = objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).FirstOrDefault();

                    if (objForm.FormTypeID == 2)
                    {
                        #region installation
                        var projectFieldData = objCompassEntities.TblProjectFieldDatas.Where(o => o.Active == 1 && o.FormId == _formId
                                                   && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                        if (projectFieldData == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic info first for the video.");
                        }

                        var fielddata = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1
                            && o.TblProjectFieldData_Id == projectFieldData.ID).FirstOrDefault();

                        if (fielddata == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic service info first for the video.");
                        }
                        string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
                            AccountNumber + @"\" + accountInitialFieldId + @"\";
                        string filePath = HttpContext.Current.Request.MapPath(tempPath);
                        grantAccess(filePath);

                        List<string> imageFileNameList = new List<string>();
                        for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                        {
                            System.Web.HttpPostedFile hpf = hfc[iCnt];

                            if (hpf.ContentLength > 0)
                            {
                                // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                                //if (!File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                                //{
                                //    // SAVE THE FILES IN THE FOLDER.
                                //    hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                                //}

                                if (System.IO.File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                                {
                                    System.IO.File.Delete(filePath + Path.GetFileName(hpf.FileName));
                                }
                                hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                                iUploadedCnt = iUploadedCnt + 1;
                                imageFileNameList.Add(tempPath + Path.GetFileName(hpf.FileName));
                            }
                        }

                        foreach (var item in imageFileNameList)
                        {
                            var obj = objCompassEntities.TblProjectFieldDataAttachments.Where(o => o.Active == 1 &&
                               o.tblProjectFieldDataMaster_ID == fielddata.ID && o.IsAudioVideoImage == 2 &&
                               o.strFilePath == item && o.isServiceData == false).FirstOrDefault();
                            if (obj == null)
                            {
                                obj = new TblProjectFieldDataAttachment();
                                obj.Active = 1;
                                obj.CreatedOn = new CommonFunctions().ServerDate();
                                obj.CreatedBy = _userId;
                                obj.tblProjectFieldDataMaster_ID = fielddata.ID;
                                obj.IsAudioVideoImage = 2;
                                obj.IsSkipped = false;
                                obj.strFilePath = item;
                                //obj.tblService_Id = _serviceInitialId;
                                //obj.TblServicePicture_ID = rv.ID;
                                obj.isServiceData = false;
                                objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                                objCompassEntities.SaveChanges();
                            }

                        }

                        if (imageFileNameList.Count > 0)
                        {
                            log.Info("WEB API UploadCustomVideoFiles Save Success at - " + DateTime.Now.ToString());

                            ErrorModelOnlyStatus em = new ErrorModelOnlyStatus();
                            em.status = "Success";
                            var response = this.Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                            return response;
                        }
                        else
                        {
                            log.Info("WEB API UploadCustomVideoFiles Save ERROR at - " + DateTime.Now.ToString() + " - Failed to save. Video file with same name already exist.");
                            return JsonErrorResponse(s, "fail", "Failed to save. Video file with same name already exist.");
                        }

                        #endregion
                    }
                    else if (objForm.FormTypeID == 1)
                    {
                        #region Audit process

                        var projectAuditFieldData = objCompassEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == _formId
                           && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                        if (projectAuditFieldData == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic info first for the video.");
                        }

                        var fielddata = objCompassEntities.ProjectAuditFieldDataMasters.Where(o => o.Active == 1
                            && o.FK_ProjectAuditData == projectAuditFieldData.ID).FirstOrDefault();

                        if (fielddata == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic service info first for the video.");
                        }
                        string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
                            AccountNumber + @"\" + accountInitialFieldId + @"\";
                        string filePath = HttpContext.Current.Request.MapPath(tempPath);
                        grantAccess(filePath);

                        List<string> imageFileNameList = new List<string>();
                        for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                        {
                            System.Web.HttpPostedFile hpf = hfc[iCnt];

                            if (hpf.ContentLength > 0)
                            {
                                // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                                //if (!File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                                //{
                                //    // SAVE THE FILES IN THE FOLDER.
                                //    hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                                //}

                                if (System.IO.File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                                {
                                    System.IO.File.Delete(filePath + Path.GetFileName(hpf.FileName));
                                }
                                hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                                iUploadedCnt = iUploadedCnt + 1;
                                imageFileNameList.Add(tempPath + Path.GetFileName(hpf.FileName));
                            }
                        }

                        foreach (var item in imageFileNameList)
                        {
                            var obj = objCompassEntities.ProjectAuditFieldDataAttachments.Where(o => o.Active == 1 &&
                               o.FK_ProjectAuditFieldDataMaster_ID == fielddata.ID && o.IsAudioVideoImage == 2 &&
                               o.StrFilePath == item && o.IsServiceData == false).FirstOrDefault();
                            if (obj == null)
                            {
                                obj = new ProjectAuditFieldDataAttachment();
                                obj.Active = 1;
                                obj.CreatedOn = new CommonFunctions().ServerDate();
                                obj.CreatedBy = _userId;
                                obj.FK_ProjectAuditFieldDataMaster_ID = fielddata.ID;
                                obj.IsAudioVideoImage = 2;
                                obj.IsSkipped = false;
                                obj.StrFilePath = item;
                                //obj.tblService_Id = _serviceInitialId;
                                //obj.TblServicePicture_ID = rv.ID;
                                obj.IsServiceData = false;
                                objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
                                objCompassEntities.SaveChanges();
                            }

                        }

                        if (imageFileNameList.Count > 0)
                        {
                            log.Info("WEB API UploadCustomVideoFiles Save Success at - " + DateTime.Now.ToString());

                            ErrorModelOnlyStatus em = new ErrorModelOnlyStatus();
                            em.status = "Success";
                            var response = this.Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                            return response;
                        }
                        else
                        {
                            log.Info("WEB API UploadCustomVideoFiles Save ERROR at - " + DateTime.Now.ToString() + " - Failed to save. Video file with same name already exist.");
                            return JsonErrorResponse(s, "fail", "Failed to save. Video file with same name already exist.");
                        }
                        #endregion
                    }
                    else
                    {
                        return JsonErrorResponse(s, "Error", "Error");
                    }


                }


            }
            catch (Exception ex)
            {
                log.Info("WEB API UploadCustomVideoFiles Save ERROR at - " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                return JsonErrorResponse(s, "fail", ex.Message);
            }
        }

        #endregion




        #region SKIP IMAGE CAPTURE
        [System.Web.Http.HttpPost]
        [ActionName("UploadSkipImageFiles")]
        public HttpResponseMessage UploadSkipImageFiles()
        {
            log.Info("WEB API Service  UploadSkipImageFiles Started at - " + DateTime.Now.ToString());
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            try
            {
                string projectId = ""; string AccountNumber = "";
                string formId = ""; string accountInitialFieldId = "";
                int iUploadedCnt = 0;
                string userId = "";

                HttpFileCollection hfc = HttpContext.Current.Request.Files;

                if (hfc[0].FileName == "")
                {
                    return JsonErrorResponse(s, "fail", "no attachment found for the request.");
                }



                userId = HttpContext.Current.Request.Form["userId"];
                formId = HttpContext.Current.Request.Form["formId"];
                accountInitialFieldId = HttpContext.Current.Request.Form["accountInitialFieldId"];

                log.Info("WEB API Service UploadSkipImageFiles Requested by  - " + userId);

                long _formId = 0;
                int _userId = 0; long _accountInitialFieldId = 0;
                bool validParameters = true;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out _userId))
                {
                    return JsonErrorResponse(s, "fail", "userId field not valid.");
                }
                else if (string.IsNullOrEmpty(formId) || !long.TryParse(formId, out _formId))
                {
                    return JsonErrorResponse(s, "fail", "formId field not valid.");
                }
                else if (string.IsNullOrEmpty(accountInitialFieldId) || !long.TryParse(accountInitialFieldId, out _accountInitialFieldId))
                {
                    return JsonErrorResponse(s, "fail", "accountInitialFieldId field not valid.");
                }


                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    string[] extension = hpf.FileName.Split('.');
                    if (extension.Count() < 2)
                    {
                        validParameters = false;
                    }
                    else
                    {
                        if (extension[1].ToLower() != "jpeg" && extension[1].ToLower() != "jpg")
                        {
                            validParameters = false;
                        }
                    }
                }

                if (!validParameters)
                {
                    return JsonErrorResponse(s, "fail", "only .jpeg/.jpg images allowed.");
                }
                else
                {

                    CompassEntities objCompassEntities = new CompassEntities();

                    projectId = Convert.ToString(objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).Select(o => o.ProjectId).FirstOrDefault());

                    if (string.IsNullOrEmpty(projectId))
                    {
                        return JsonErrorResponse(s, "fail", "formId not present in the database.");
                    }

                    AccountNumber = Convert.ToString(objCompassEntities.UploadedExcelDatas.Where(o => o.Id == _accountInitialFieldId).Select(o => o.Account12).FirstOrDefault());
                    if (string.IsNullOrEmpty(AccountNumber))
                    {
                        return JsonErrorResponse(s, "fail", "accountInitialFieldId not present in the database.");
                    }


                    var objForm = objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).FirstOrDefault();

                    if (objForm.FormTypeID == 2)
                    {
                        #region installation

                        var projectFieldData = objCompassEntities.TblProjectFieldDatas.Where(o => o.Active == 1 && o.FormId == _formId
                       && o.FK_UploadedExcelData_Id == _accountInitialFieldId && o.InstallerId == _userId && o.IsSkipped == true).FirstOrDefault();

                        if (projectFieldData == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic info first for the image.");
                        }



                        string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
                            AccountNumber + @"\" + accountInitialFieldId + @"\";
                        string filePath = HttpContext.Current.Request.MapPath(tempPath);
                        grantAccess(filePath);

                        List<string> imageFileNameList = new List<string>();
                        for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                        {
                            System.Web.HttpPostedFile hpf = hfc[iCnt];

                            if (hpf.ContentLength > 0)
                            {
                                DateTime DT = new CommonFunctions().ServerDate();
                                //new file path for new file
                                string newFilePath = filePath + DT.ToString("dd_MM_yyyy_hh_mm_ss_tt") + Path.GetExtension(hpf.FileName);

                                hpf.SaveAs(newFilePath);


                                // string _imagePath = filePath + Path.GetFileName(hpf.FileName);
                                // Rotate the image according to EXIF data
                                var bmp = new Bitmap(newFilePath);


                                var exif = new EXIFextractor(ref bmp, "n");

                                if (exif["Orientation"] != null)
                                {
                                    RotateFlipType flip = OrientationToFlipType(exif["Orientation"].ToString());

                                    if (flip != RotateFlipType.RotateNoneFlipNone) // don't flip of orientation is correct
                                    {
                                        bmp.RotateFlip(flip);
                                        exif.setTag(0x112, "1"); // Optional: reset orientation tag
                                        bmp.Save(newFilePath, ImageFormat.Jpeg);
                                    }
                                }


                                iUploadedCnt = iUploadedCnt + 1;
                                imageFileNameList.Add(tempPath + Path.GetFileName(newFilePath));

                            }
                        }



                        foreach (var item in imageFileNameList)
                        {

                            var obj = new TblProjectFieldDataAttachment();
                            obj.Active = 1;
                            obj.CreatedOn = new CommonFunctions().ServerDate();
                            obj.CreatedBy = _userId;
                            obj.tblProjectFieldDataMaster_ID = 0;
                            obj.ProjectFieldDataId = projectFieldData.ID;
                            obj.IsAudioVideoImage = 3;
                            obj.IsSkipped = true;
                            obj.strFilePath = item;
                            obj.isServiceData = false;
                            objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                            objCompassEntities.SaveChanges();

                        }

                        log.Info("WEB API UploadSkipImageFiles Save Complete at - " + DateTime.Now.ToString());
                        ErrorModelOnlyStatus em1 = new ErrorModelOnlyStatus();
                        em1.status = "Success";
                        var response1 = this.Request.CreateResponse(HttpStatusCode.OK);
                        response1.Content = new StringContent(s.Serialize(em1), Encoding.UTF8, "application/json");
                        return response1;
                        #endregion
                    }
                    else if (objForm.FormTypeID == 1)
                    {
                        #region Audit process

                        var projectFieldData = objCompassEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == _formId
                      && o.FK_UploadedExcelData_Id == _accountInitialFieldId && o.FK_AuditorId == _userId && o.IsSkipped == true).FirstOrDefault();

                        if (projectFieldData == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic info first for the image.");
                        }

                        string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" + AccountNumber + @"\" + accountInitialFieldId + @"\";
                        string filePath = HttpContext.Current.Request.MapPath(tempPath);
                        grantAccess(filePath);

                        List<string> imageFileNameList = new List<string>();
                        for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                        {
                            System.Web.HttpPostedFile hpf = hfc[iCnt];

                            if (hpf.ContentLength > 0)
                            {
                                DateTime DT = new CommonFunctions().ServerDate();
                                //new file path for new file
                                string newFilePath = filePath + DT.ToString("dd_MM_yyyy_hh_mm_ss_tt") + Path.GetExtension(hpf.FileName);

                                hpf.SaveAs(newFilePath);


                                // string _imagePath = filePath + Path.GetFileName(hpf.FileName);
                                // Rotate the image according to EXIF data
                                var bmp = new Bitmap(newFilePath);

                                var exif = new EXIFextractor(ref bmp, "n");

                                if (exif["Orientation"] != null)
                                {
                                    RotateFlipType flip = OrientationToFlipType(exif["Orientation"].ToString());

                                    if (flip != RotateFlipType.RotateNoneFlipNone) // don't flip of orientation is correct
                                    {
                                        bmp.RotateFlip(flip);
                                        exif.setTag(0x112, "1"); // Optional: reset orientation tag
                                        bmp.Save(newFilePath, ImageFormat.Jpeg);
                                    }
                                }


                                iUploadedCnt = iUploadedCnt + 1;
                                imageFileNameList.Add(tempPath + Path.GetFileName(newFilePath));

                            }
                        }



                        foreach (var item in imageFileNameList)
                        {
                            var obj = new ProjectAuditFieldDataAttachment();
                            obj.Active = 1;
                            obj.CreatedOn = new CommonFunctions().ServerDate();
                            obj.CreatedBy = _userId;
                            obj.FK_ProjectAuditFieldDataMaster_ID = 0;
                            obj.FK_ProjectAuditFieldDataId = projectFieldData.ID;
                            obj.IsAudioVideoImage = 3;
                            obj.IsSkipped = true;
                            obj.StrFilePath = item;
                            obj.IsServiceData = false;
                            objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
                            objCompassEntities.SaveChanges();

                        }

                        log.Info("WEB API UploadSkipImageFiles Save Complete at - " + DateTime.Now.ToString());
                        ErrorModelOnlyStatus em1 = new ErrorModelOnlyStatus();
                        em1.status = "Success";
                        var response1 = this.Request.CreateResponse(HttpStatusCode.OK);
                        response1.Content = new StringContent(s.Serialize(em1), Encoding.UTF8, "application/json");
                        return response1;


                        #endregion
                    }
                    else
                    {
                        return JsonErrorResponse(s, "Error", "Error");
                    }




                }

            }
            catch (Exception ex)
            {
                log.Info("WEB API UploadSkipImageFiles Save ERROR at - " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                return JsonErrorResponse(s, "Error", ex.Message);
            }
        }



        #endregion


        #region SKIP AUDIO CAPTURE
        [System.Web.Http.HttpPost]
        [ActionName("UploadSkipAudioFiles")]
        public HttpResponseMessage UploadSkipAudioFiles()
        {
            log.Info("WEB API Service  UploadSkipAudioFiles Started at - " + DateTime.Now.ToString());
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            try
            {
                string projectId = ""; string AccountNumber = "";
                string formId = ""; string accountInitialFieldId = "";
                int iUploadedCnt = 0;
                string userId = "";


                HttpFileCollection hfc = HttpContext.Current.Request.Files;

                if (hfc[0].FileName == "")
                {
                    return JsonErrorResponse(s, "fail", "no attachment found for the request.");
                }

                userId = HttpContext.Current.Request.Form["userId"];
                formId = HttpContext.Current.Request.Form["formId"];
                accountInitialFieldId = HttpContext.Current.Request.Form["accountInitialFieldId"];

                log.Info("WEB API Service UploadSkipAudioFiles Requested by  - " + userId);

                long _formId = 0; int _userId = 0; long _accountInitialFieldId = 0;
                bool validParameters = true;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out _userId))
                {
                    return JsonErrorResponse(s, "fail", "userId field not valid.");
                }
                else if (string.IsNullOrEmpty(formId) || !long.TryParse(formId, out _formId))
                {
                    return JsonErrorResponse(s, "fail", "formId field not valid.");
                }
                else if (string.IsNullOrEmpty(accountInitialFieldId) || !long.TryParse(accountInitialFieldId, out _accountInitialFieldId))
                {
                    return JsonErrorResponse(s, "fail", "accountInitialFieldId field not valid.");
                }



                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    string[] extension = hpf.FileName.Split('.');
                    if (extension.Count() < 2)
                    {
                        validParameters = false;
                    }
                    else
                    {
                        if (extension[1].ToLower() != "m4a")
                        {
                            validParameters = false;
                        }
                    }
                }

                if (!validParameters)
                {
                    return JsonErrorResponse(s, "fail", "only .m4a files are allowded to upload");
                }
                else
                {
                    CompassEntities objCompassEntities = new CompassEntities();

                    projectId = Convert.ToString(objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).Select(o => o.ProjectId).FirstOrDefault());

                    if (string.IsNullOrEmpty(projectId))
                    {
                        return JsonErrorResponse(s, "fail", "formId not present in the database.");
                    }

                    AccountNumber = Convert.ToString(objCompassEntities.UploadedExcelDatas.Where(o => o.Id == _accountInitialFieldId).Select(o => o.Account12).FirstOrDefault());

                    if (string.IsNullOrEmpty(AccountNumber))
                    {
                        return JsonErrorResponse(s, "fail", "accountInitialFieldId not present in the database.");
                    }



                    var objForm = objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).FirstOrDefault();

                    if (objForm.FormTypeID == 2)
                    {
                        #region installation

                        var projectFieldData = objCompassEntities.TblProjectFieldDatas.Where(o => o.Active == 1 && o.FormId == _formId
                        && o.FK_UploadedExcelData_Id == _accountInitialFieldId && o.IsSkipped == true && o.InstallerId == _userId).FirstOrDefault();

                        if (projectFieldData == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic info first for the audio.");
                        }


                        string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
                            AccountNumber + @"\" + accountInitialFieldId + @"\";
                        string filePath = HttpContext.Current.Request.MapPath(tempPath);
                        grantAccess(filePath);

                        List<string> imageFileNameList = new List<string>();
                        for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                        {
                            System.Web.HttpPostedFile hpf = hfc[iCnt];

                            if (hpf.ContentLength > 0)
                            {

                                DateTime DT = new CommonFunctions().ServerDate();
                                //new file path for new file
                                string newFilePath = filePath + DT.ToString("dd_MM_yyyy_hh_mm_ss_tt") + Path.GetExtension(hpf.FileName);

                                hpf.SaveAs(newFilePath);

                                iUploadedCnt = iUploadedCnt + 1;
                                imageFileNameList.Add(tempPath + Path.GetFileName(newFilePath));

                            }
                        }

                        foreach (var item in imageFileNameList)
                        {

                            var obj = new TblProjectFieldDataAttachment();
                            obj.Active = 1;
                            obj.CreatedOn = new CommonFunctions().ServerDate();
                            obj.CreatedBy = _userId;
                            obj.ProjectFieldDataId = projectFieldData.ID;
                            obj.tblProjectFieldDataMaster_ID = 0;
                            obj.IsAudioVideoImage = 1;
                            obj.IsSkipped = true;
                            obj.strFilePath = item;
                            obj.isServiceData = false;
                            objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                            objCompassEntities.SaveChanges();


                        }

                        if (imageFileNameList.Count > 0)
                        {

                            log.Info("WEB API UploadSkipAudioFiles Save Complete at - " + DateTime.Now.ToString());
                            ErrorModelOnlyStatus em = new ErrorModelOnlyStatus();
                            em.status = "Success";
                            var response = this.Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                            return response;
                        }
                        else
                        {
                            log.Info("WEB API UploadSkipAudioFiles Save Complete at - " + DateTime.Now.ToString() + " -Failed to save. Audio file with same name already exist");
                            return JsonErrorResponse(s, "fail", "Failed to save. Audio file with same name already exist.");
                        }

                        #endregion
                    }
                    else if (objForm.FormTypeID == 1)
                    {
                        #region Audit process

                        var projectFieldData = objCompassEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == _formId
                         && o.FK_UploadedExcelData_Id == _accountInitialFieldId && o.IsSkipped == true && o.FK_AuditorId == _userId).FirstOrDefault();

                        if (projectFieldData == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic info first for the audio.");
                        }


                        string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" + AccountNumber + @"\" + accountInitialFieldId + @"\";
                        string filePath = HttpContext.Current.Request.MapPath(tempPath);
                        grantAccess(filePath);

                        List<string> imageFileNameList = new List<string>();
                        for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                        {
                            System.Web.HttpPostedFile hpf = hfc[iCnt];

                            if (hpf.ContentLength > 0)
                            {

                                DateTime DT = new CommonFunctions().ServerDate();
                                //new file path for new file
                                string newFilePath = filePath + DT.ToString("dd_MM_yyyy_hh_mm_ss_tt") + Path.GetExtension(hpf.FileName);

                                hpf.SaveAs(newFilePath);

                                iUploadedCnt = iUploadedCnt + 1;
                                imageFileNameList.Add(tempPath + Path.GetFileName(newFilePath));

                            }
                        }

                        foreach (var item in imageFileNameList)
                        {

                            var obj = new ProjectAuditFieldDataAttachment();
                            obj.Active = 1;
                            obj.CreatedOn = new CommonFunctions().ServerDate();
                            obj.CreatedBy = _userId;
                            obj.FK_ProjectAuditFieldDataId = projectFieldData.ID;
                            obj.FK_ProjectAuditFieldDataMaster_ID = 0;
                            obj.IsAudioVideoImage = 1;
                            obj.IsSkipped = true;
                            obj.StrFilePath = item;
                            obj.IsServiceData = false;
                            objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
                            objCompassEntities.SaveChanges();


                        }

                        if (imageFileNameList.Count > 0)
                        {

                            log.Info("WEB API UploadSkipAudioFiles Save Complete at - " + DateTime.Now.ToString());
                            ErrorModelOnlyStatus em = new ErrorModelOnlyStatus();
                            em.status = "Success";
                            var response = this.Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                            return response;
                        }
                        else
                        {
                            log.Info("WEB API UploadSkipAudioFiles Save Complete at - " + DateTime.Now.ToString() + " -Failed to save. Audio file with same name already exist");
                            return JsonErrorResponse(s, "fail", "Failed to save. Audio file with same name already exist.");
                        }


                        #endregion
                    }
                    else
                    {
                        return JsonErrorResponse(s, "Error", "Error");
                    }




                }
            }
            catch (Exception ex)
            {

                log.Info("WEB API UploadSkipAudioFiles Save ERROR at - " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));


                return JsonErrorResponse(s, "Error", ex.Message);
            }
        }

        #endregion


        #region SKIP VIDEO CAPTURE
        [System.Web.Http.HttpPost]
        [ActionName("UploadSkipVideoFiles")]
        public HttpResponseMessage UploadSkipVideoFiles()
        {
            log.Info("WEB API Service  UploadSkipVideoFiles Started at - " + DateTime.Now.ToString());
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            try
            {
                string projectId = ""; string AccountNumber = "";
                string formId = ""; string accountInitialFieldId = "";
                int iUploadedCnt = 0;
                string userId = "";


                HttpFileCollection hfc = HttpContext.Current.Request.Files;

                if (hfc[0].FileName == "")
                {
                    return JsonErrorResponse(s, "fail", "no attachment found for the request.");
                }


                userId = HttpContext.Current.Request.Form["userId"];
                formId = HttpContext.Current.Request.Form["formId"];
                accountInitialFieldId = HttpContext.Current.Request.Form["accountInitialFieldId"];

                log.Info("WEB API Service  UploadSkipVideoFiles Requested by  - " + userId);

                long _formId = 0; int _userId = 0; long _accountInitialFieldId = 0;
                bool validParameters = true;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out _userId))
                {
                    return JsonErrorResponse(s, "fail", "userId field not valid.");
                }
                else if (string.IsNullOrEmpty(formId) || !long.TryParse(formId, out _formId))
                {
                    return JsonErrorResponse(s, "fail", "formId field not valid.");
                }
                else if (string.IsNullOrEmpty(accountInitialFieldId) || !long.TryParse(accountInitialFieldId, out _accountInitialFieldId))
                {
                    return JsonErrorResponse(s, "fail", "accountInitialFieldId field not valid.");
                }


                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    string[] extension = hpf.FileName.Split('.');
                    if (extension.Count() < 2)
                    {
                        validParameters = false;
                    }
                    else
                    {
                        if (extension[1].ToLower() != "mp4")
                        {
                            validParameters = false;
                        }
                    }
                }

                if (!validParameters)
                {
                    return JsonErrorResponse(s, "fail", "only .mp4 files are allowded to upload");
                }
                else
                {
                    CompassEntities objCompassEntities = new CompassEntities();

                    projectId = Convert.ToString(objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).Select(o => o.ProjectId).FirstOrDefault());


                    if (string.IsNullOrEmpty(projectId))
                    {
                        return JsonErrorResponse(s, "fail", "formId not present in the database.");
                    }


                    AccountNumber = Convert.ToString(objCompassEntities.UploadedExcelDatas.Where(o => o.Id == _accountInitialFieldId).Select(o => o.Account12).FirstOrDefault());

                    if (string.IsNullOrEmpty(AccountNumber))
                    {
                        return JsonErrorResponse(s, "fail", "accountInitialFieldId not present in the database.");
                    }


                    var objForm = objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).FirstOrDefault();

                    if (objForm.FormTypeID == 2)
                    {
                        #region installation

                        var projectFieldData = objCompassEntities.TblProjectFieldDatas.Where(o => o.Active == 1 && o.FormId == _formId
                           && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                        if (projectFieldData == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic info first for the video.");
                        }


                        string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
                            AccountNumber + @"\" + accountInitialFieldId + @"\";
                        string filePath = HttpContext.Current.Request.MapPath(tempPath);
                        grantAccess(filePath);

                        List<string> imageFileNameList = new List<string>();
                        for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                        {
                            System.Web.HttpPostedFile hpf = hfc[iCnt];

                            if (hpf.ContentLength > 0)
                            {

                                DateTime DT = new CommonFunctions().ServerDate();
                                //new file path for new file
                                string newFilePath = filePath + DT.ToString("dd_MM_yyyy_hh_mm_ss_tt") + Path.GetExtension(hpf.FileName);

                                hpf.SaveAs(newFilePath);


                                iUploadedCnt = iUploadedCnt + 1;
                                imageFileNameList.Add(tempPath + Path.GetFileName(newFilePath));
                            }
                        }

                        foreach (var item in imageFileNameList)
                        {

                            var obj = new TblProjectFieldDataAttachment();
                            obj.Active = 1;
                            obj.CreatedOn = new CommonFunctions().ServerDate();
                            obj.CreatedBy = _userId;
                            obj.tblProjectFieldDataMaster_ID = 0;
                            obj.ProjectFieldDataId = projectFieldData.ID;
                            obj.IsAudioVideoImage = 2;
                            obj.IsSkipped = true;
                            obj.strFilePath = item;
                            obj.isServiceData = false;
                            objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                            objCompassEntities.SaveChanges();


                        }

                        if (imageFileNameList.Count > 0)
                        {
                            log.Info("WEB API Service  UploadSkipVideoFiles Completed at - " + DateTime.Now.ToString());
                            ErrorModelOnlyStatus em = new ErrorModelOnlyStatus();
                            em.status = "Success";
                            var response = this.Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                            return response;
                        }
                        else
                        {
                            log.Info("WEB API Service  UploadSkipVideoFiles Started at - " + DateTime.Now.ToString() + "- Failed to save. Video file with same name already exist.   ");
                            return JsonErrorResponse(s, "fail", "Failed to save. Video file with same name already exist.");
                        }

                        #endregion
                    }
                    else if (objForm.FormTypeID == 1)
                    {
                        #region Audit process

                        var projectFieldData = objCompassEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == _formId
                           && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                        if (projectFieldData == null)
                        {
                            return JsonErrorResponse(s, "fail", "Please save the basic info first for the video.");
                        }


                        string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" + AccountNumber + @"\" + accountInitialFieldId + @"\";
                        string filePath = HttpContext.Current.Request.MapPath(tempPath);
                        grantAccess(filePath);

                        List<string> imageFileNameList = new List<string>();
                        for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                        {
                            System.Web.HttpPostedFile hpf = hfc[iCnt];

                            if (hpf.ContentLength > 0)
                            {
                                DateTime DT = new CommonFunctions().ServerDate();
                                //new file path for new file
                                string newFilePath = filePath + DT.ToString("dd_MM_yyyy_hh_mm_ss_tt") + Path.GetExtension(hpf.FileName);
                                hpf.SaveAs(newFilePath);
                                iUploadedCnt = iUploadedCnt + 1;
                                imageFileNameList.Add(tempPath + Path.GetFileName(newFilePath));
                            }
                        }

                        foreach (var item in imageFileNameList)
                        {

                            var obj = new ProjectAuditFieldDataAttachment();
                            obj.Active = 1;
                            obj.CreatedOn = new CommonFunctions().ServerDate();
                            obj.CreatedBy = _userId;
                            obj.FK_ProjectAuditFieldDataMaster_ID = 0;
                            obj.FK_ProjectAuditFieldDataId = projectFieldData.ID;
                            obj.IsAudioVideoImage = 2;
                            obj.IsSkipped = true;
                            obj.StrFilePath = item;
                            obj.IsServiceData = false;
                            objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
                            objCompassEntities.SaveChanges();


                        }

                        if (imageFileNameList.Count > 0)
                        {
                            log.Info("WEB API Service  UploadSkipVideoFiles Completed at - " + DateTime.Now.ToString());
                            ErrorModelOnlyStatus em = new ErrorModelOnlyStatus();
                            em.status = "Success";
                            var response = this.Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                            return response;
                        }
                        else
                        {
                            log.Info("WEB API Service  UploadSkipVideoFiles Started at - " + DateTime.Now.ToString() + "- Failed to save. Video file with same name already exist.   ");
                            return JsonErrorResponse(s, "fail", "Failed to save. Video file with same name already exist.");
                        }


                        #endregion
                    }
                    else
                    {
                        return JsonErrorResponse(s, "Error", "Error");
                    }


                }


            }
            catch (Exception ex)
            {
                log.Info("WEB API UploadSkipVideoFiles Save ERROR at - " + DateTime.Now.ToString());
                log.Info("ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));
                log.Info("ERROR  ex.StackTrace - " + (ex.StackTrace != null ? ex.StackTrace : ""));
                log.Info("ERROR  ex.InnerException.Message - " + (ex.InnerException.Message != null ? ex.InnerException.Message : ""));
                return JsonErrorResponse(s, "Error", ex.Message);
            }
        }

        #endregion


        private static RotateFlipType OrientationToFlipType(string orientation)
        {
            switch (int.Parse(orientation))
            {
                case 1:
                    return RotateFlipType.RotateNoneFlipNone;
                    break;
                case 2:
                    return RotateFlipType.RotateNoneFlipX;
                    break;
                case 3:
                    return RotateFlipType.Rotate180FlipNone;
                    break;
                case 4:
                    return RotateFlipType.Rotate180FlipX;
                    break;
                case 5:
                    return RotateFlipType.Rotate90FlipX;
                    break;
                case 6:
                    return RotateFlipType.Rotate90FlipNone;
                    break;
                case 7:
                    return RotateFlipType.Rotate270FlipX;
                    break;
                case 8:
                    return RotateFlipType.Rotate270FlipNone;
                    break;
                default:
                    return RotateFlipType.RotateNoneFlipNone;
            }
        }


        //AddedBy AniketJ on 26-Oct-2016
        #region UploadImage from web
        #region CUSTOM IMAGE CAPTURE
        [System.Web.Http.HttpPost]
        [ActionName("UploadCustomImageFilesFromWeb")]
        public HttpResponseMessage UploadCustomImageFilesFromWeb()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            try
            {
                string projectId = ""; string AccountNumber = "";
                string formId = ""; string accountInitialFieldId = "";
                int iUploadedCnt = 0;
                string fieldName = ""; string userId = "";
                string currentFieldDataMasterId = "";

                HttpFileCollection hfc = HttpContext.Current.Request.Files;

                userId = HttpContext.Current.Request.Form["userId"];
                formId = HttpContext.Current.Request.Form["formId"];
                accountInitialFieldId = HttpContext.Current.Request.Form["accountInitialFieldId"];
                fieldName = HttpContext.Current.Request.Form["FieldName"];
                currentFieldDataMasterId = HttpContext.Current.Request.Form["fieldDataMasterId"];

                long _formId = 0;
                int _userId = 0; long _accountInitialFieldId = 0;
                bool validParameters = true;
                long _currentFieldDataMasterId = 0;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out _userId))
                {
                    return JsonErrorResponse(s, "fail", "userId field not valid.");
                }
                else if (string.IsNullOrEmpty(formId) || !long.TryParse(formId, out _formId))
                {
                    return JsonErrorResponse(s, "fail", "formId field not valid.");
                }
                else if (string.IsNullOrEmpty(accountInitialFieldId) || !long.TryParse(accountInitialFieldId, out _accountInitialFieldId))
                {
                    return JsonErrorResponse(s, "fail", "accountInitialFieldId field not valid.");
                }
                else if (string.IsNullOrEmpty(fieldName))
                {
                    return JsonErrorResponse(s, "fail", "fieldName field not valid.");
                }
                else if (!string.IsNullOrEmpty(currentFieldDataMasterId) && !long.TryParse(currentFieldDataMasterId, out _currentFieldDataMasterId))
                {
                    return JsonErrorResponse(s, "fail", "fieldDataMasterId field not valid.");
                }

                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    string[] extension = hpf.FileName.Split('.');
                    if (extension.Count() < 2)
                    {
                        validParameters = false;
                    }
                    else
                    {
                        if (extension[1].ToLower() != "jpeg" && extension[1].ToLower() != "jpg")
                        {
                            validParameters = false;
                        }
                    }
                }

                if (!validParameters)
                {
                    return JsonErrorResponse(s, "fail", "only .jpeg/.jpg images allowed.");
                }
                else
                {

                    CompassEntities objCompassEntities = new CompassEntities();

                    projectId = Convert.ToString(objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).Select(o => o.ProjectId).FirstOrDefault());

                    if (string.IsNullOrEmpty(projectId))
                    {
                        return JsonErrorResponse(s, "fail", "formId not present in the database.");
                    }

                    AccountNumber = Convert.ToString(objCompassEntities.UploadedExcelDatas.
                       Where(o => o.Id == _accountInitialFieldId).Select(o => o.Account12).FirstOrDefault());
                    if (string.IsNullOrEmpty(AccountNumber))
                    {
                        return JsonErrorResponse(s, "fail", "accountInitialFieldId not present in the database.");
                    }


                    //check single photo field or not.

                    var ImageType = objCompassEntities.PROC_CheckImageSingleMultiple(_formId, fieldName).FirstOrDefault();

                    if (ImageType == "SinglePhoto" && _currentFieldDataMasterId > 0)
                    {
                        var currentActiveRecord = objCompassEntities.TblProjectFieldDataAttachments.Where(o =>
                            o.tblProjectFieldDataMaster_ID == _currentFieldDataMasterId
                            && o.IsAudioVideoImage == 3 && o.Active == 1).FirstOrDefault();
                        if (currentActiveRecord != null)
                        {
                            currentActiveRecord.Active = 0;
                            currentActiveRecord.ModifiedBy = _userId;
                            currentActiveRecord.ModifiedOn = new CommonFunctions().ServerDate();
                            objCompassEntities.SaveChanges();
                        }
                    }


                    string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
                        AccountNumber + @"\" + accountInitialFieldId + @"\" + fieldName + @"\";

                    string filePath = HttpContext.Current.Request.MapPath(tempPath);
                    grantAccess(filePath);

                    List<string> imageFileNameList = new List<string>();
                    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                    {
                        System.Web.HttpPostedFile hpf = hfc[iCnt];

                        if (hpf.ContentLength > 0)
                        {

                            if (System.IO.File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                            {
                                System.GC.Collect();
                                System.GC.WaitForPendingFinalizers();
                                System.IO.File.Delete(filePath + Path.GetFileName(hpf.FileName));

                            }
                            hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));


                            string _imagePath = filePath + Path.GetFileName(hpf.FileName);

                            iUploadedCnt = iUploadedCnt + 1;
                            imageFileNameList.Add(tempPath + Path.GetFileName(hpf.FileName));

                        }
                    }



                    foreach (var item in imageFileNameList)
                    {
                        var obj = objCompassEntities.TblProjectFieldDataAttachments.Where(o => o.Active == 1 &&
                           o.tblProjectFieldDataMaster_ID == _currentFieldDataMasterId && o.IsAudioVideoImage == 3 &&
                           o.strFilePath == item && o.isServiceData == false).FirstOrDefault();
                        if (obj == null)
                        {
                            obj = new TblProjectFieldDataAttachment();
                            obj.Active = 1;
                            obj.CreatedOn = new CommonFunctions().ServerDate();
                            obj.CreatedBy = _userId;
                            obj.tblProjectFieldDataMaster_ID = _currentFieldDataMasterId;
                            obj.IsAudioVideoImage = 3;
                            obj.IsSkipped = false;
                            obj.strFilePath = item;
                            //obj.tblService_Id = _serviceInitialId;
                            //obj.TblServicePicture_ID = rv.ID;
                            obj.isServiceData = false;
                            objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                            objCompassEntities.SaveChanges();
                        }
                    }



                    return JsonErrorResponse(s, "Success", "File Uploaded Sucessfully.");
                }


            }
            catch (Exception ex)
            {
                return JsonErrorResponse(s, "Error", "Error -" + ex.Message);
            }
        }

        #endregion


        #region SERVICE IMAGE CAPTURE
        [System.Web.Http.HttpPost]
        [ActionName("UploadServiceImageFilesFromWeb")]
        public HttpResponseMessage UploadServiceImageFilesFromWeb()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            try
            {

                string strServiceId = ""; string strUserId = "";
                string strFormId = ""; string strMasterFieldId = "";



                string projectId = ""; string AccountNumber = "";
                string imageName = ""; int iUploadedCnt = 0;

                string isAdditionalService = "";
                HttpFileCollection hfc = HttpContext.Current.Request.Files;

                strUserId = HttpContext.Current.Request.Form["userId"];
                strFormId = HttpContext.Current.Request.Form["formId"];
                strMasterFieldId = HttpContext.Current.Request.Form["MasterFieldId"];
                strServiceId = HttpContext.Current.Request.Form["ServiceId"];
                isAdditionalService = HttpContext.Current.Request.Form["isAdditionalService"];
                imageName = HttpContext.Current.Request.Form["imageName"];

                long FormId = 0; long ServiceId = 0;
                int UserId = 0; //long UploadId = 0;
                bool _isAdditionalServices = false;
                bool validParameters = true;
                long MasterFieldId = 0;


                #region validation
                if (string.IsNullOrEmpty(strUserId) || !int.TryParse(strUserId, out UserId))
                {
                    return JsonErrorResponse(s, "fail", "userId field not valid.");
                }
                else if (string.IsNullOrEmpty(strFormId) || !long.TryParse(strFormId, out FormId))
                {
                    return JsonErrorResponse(s, "fail", "formId field not valid.");
                }
                else if (string.IsNullOrEmpty(strMasterFieldId) || !long.TryParse(strMasterFieldId, out MasterFieldId))
                {
                    return JsonErrorResponse(s, "fail", "MasterFieldId field not valid.");
                }
                else if (string.IsNullOrEmpty(strServiceId) || !long.TryParse(strServiceId, out ServiceId))
                {
                    return JsonErrorResponse(s, "fail", "serviceinitialId field not valid.");
                }
                else if (string.IsNullOrEmpty(isAdditionalService) || !bool.TryParse(isAdditionalService, out _isAdditionalServices))
                {
                    return JsonErrorResponse(s, "fail", "isAdditionalService field not valid.");
                }
                else if (string.IsNullOrEmpty(imageName))
                {
                    return JsonErrorResponse(s, "fail", "imageName field empty.");
                }
                #endregion

                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    string[] extension = hpf.FileName.Split('.');
                    if (extension.Count() < 2)
                    {
                        validParameters = false;
                    }
                    else
                    {
                        if (extension[1].ToLower() != "jpeg" && extension[1].ToLower() != "jpg")
                        {
                            validParameters = false;
                        }
                    }
                }

                if (!validParameters)
                {
                    return JsonErrorResponse(s, "fail", "only .jpeg/.jpg images allowed.");
                }
                else
                {

                    CompassEntities objCompassEntities = new CompassEntities();


                    projectId = Convert.ToString(objCompassEntities.tblFormMasters.Where(o => o.FormId == FormId).Select(o => o.ProjectId).FirstOrDefault());

                    if (string.IsNullOrEmpty(projectId))
                    {
                        return JsonErrorResponse(s, "fail", "formId not present in the database.");
                    }



                    tblProjectFieldDataMaster fielddata1 = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1
                                                 && o.ID == MasterFieldId).FirstOrDefault();

                    var projectFieldData1 = objCompassEntities.TblProjectFieldDatas.Where(o => o.Active == 1 && o.ID == fielddata1.TblProjectFieldData_Id).FirstOrDefault();

                    AccountNumber = Convert.ToString(objCompassEntities.UploadedExcelDatas.
                       Where(o => o.Id == projectFieldData1.FK_UploadedExcelData_Id).Select(o => o.Account12).FirstOrDefault());
                    if (string.IsNullOrEmpty(AccountNumber))
                    {
                        return JsonErrorResponse(s, "fail", "accountInitialFieldId not present in the database.");
                    }

                    string tempPath = ServerUploadFolder + projectId + @"\" + strFormId + @"\" +
                        AccountNumber + @"\" + projectFieldData1.FK_UploadedExcelData_Id.ToString() + @"\" + imageName + @"\";
                    string filePath = HttpContext.Current.Request.MapPath(tempPath);
                    grantAccess(filePath);

                    List<string> imageFileNameList = new List<string>();
                    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                    {
                        System.Web.HttpPostedFile hpf = hfc[iCnt];

                        if (hpf.ContentLength > 0)
                        {
                            // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)


                            if (System.IO.File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                            {
                                System.IO.File.Delete(filePath + Path.GetFileName(hpf.FileName));
                            }
                            hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                            string _imagePath = filePath + Path.GetFileName(hpf.FileName);

                            iUploadedCnt = iUploadedCnt + 1;
                            imageFileNameList.Add(tempPath + Path.GetFileName(hpf.FileName));
                        }
                    }




                    var pictureRecord = objCompassEntities.tblPictures.Where(o => o.Picture.ToLower() == imageName.ToLower()).Select(o => o).FirstOrDefault();

                    if (pictureRecord == null)
                    {
                        return JsonErrorResponse(s, "fail", "Image data not found for the current imageName.");
                    }
                    long masterId = 0;
                    if (_isAdditionalServices)
                    {
                        var objmasterFieldRelation = objCompassEntities.TblProjectMasterFieldRelations.
                            Where(o => o.MasterFieldID == ServiceId).Select(o => o).FirstOrDefault();
                        ServiceId = objmasterFieldRelation.MasterFieldID;

                        masterId = objmasterFieldRelation.ID;
                    }
                    var rv = objCompassEntities.TblServicePictures.Where(o => o.tblPicture_PictureId == pictureRecord.ID
                        && o.tblService_ServiceId == ServiceId).Select(o => o).FirstOrDefault();

                    if (rv == null)
                    {
                        return JsonErrorResponse(s, "fail", "No data not found for the current serviceinitialId.");
                    }


                    //var projectFieldData = objCompassEntities.TblProjectFieldDatas.Where(o => o.Active == 1 && o.FormId == FormId
                    //    && o.FK_UploadedExcelData_Id == UploadId).FirstOrDefault();

                    //if (projectFieldData == null)
                    //{
                    //    return JsonErrorResponse(s, "fail", "Please save the basic info first for the image.");
                    //}


                    //tblProjectFieldDataMaster fielddata;
                    //if (!_isAdditionalServices)
                    //{
                    //    fielddata = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1
                    //       && o.TblProjectFieldData_Id == projectFieldData.ID &&
                    //       o.InitialFieldName.ToLower() == "SERVICES".ToLower()).FirstOrDefault();

                    //    if (fielddata == null)
                    //    {
                    //        return JsonErrorResponse(s, "fail", "Please save the basic service info first for the image.");
                    //    }
                    //}
                    //else
                    //{
                    //    fielddata = objCompassEntities.tblProjectFieldDataMasters.Where(o => o.Active == 1
                    //                             && o.TblProjectFieldData_Id == projectFieldData.ID &&
                    //                             o.InitialFieldName.ToLower() == "ADDITIONAL SERVICES".ToLower()).FirstOrDefault();

                    //    if (fielddata == null)
                    //    {
                    //        return JsonErrorResponse(s, "fail", "Please save the basic service info first for the image.");
                    //    }
                    //}


                    foreach (var item in imageFileNameList)
                    {

                        //var obj = objCompassEntities.TblProjectFieldDataAttachments.Where(o => o.Active == 1 &&
                        //    o.tblProjectFieldDataMaster_ID == fielddata1.ID && o.IsAudioVideoImage == 3 &&
                        //    o.strFilePath == item && o.tblService_Id == ServiceId && o.TblServicePicture_ID == rv.ID
                        //    && o.isServiceData == true).FirstOrDefault();
                        //if (obj == null)
                        //{
                        TblProjectFieldDataAttachment obj = new TblProjectFieldDataAttachment();
                        obj.Active = 1;
                        obj.CreatedOn = new CommonFunctions().ServerDate();
                        obj.CreatedBy = UserId;
                        obj.tblProjectFieldDataMaster_ID = fielddata1.ID;
                        obj.IsAudioVideoImage = 3;
                        obj.IsSkipped = false;
                        obj.strFilePath = item;

                        //obj.tblService_Id = !_isAdditionalServices ? _serviceInitialId : masterId;
                        obj.tblService_Id = ServiceId;
                        obj.TblServicePicture_ID = rv.ID;
                        obj.isServiceData = true;
                        objCompassEntities.TblProjectFieldDataAttachments.Add(obj);
                        objCompassEntities.SaveChanges();
                        //}
                    }

                    if (imageFileNameList.Count > 0)
                    {
                        ErrorModelOnlyStatus em = new ErrorModelOnlyStatus();
                        em.status = "Success";
                        var response = this.Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                        return response;
                    }
                    else
                    {
                        return JsonErrorResponse(s, "fail", "Failed to save. Image with same name already exist.");
                    }
                }


            }
            catch (Exception ex)
            {
                return JsonErrorResponse(s, "Error", ex.Message);
            }
        }

        #endregion

        #endregion


        //AddedBy AniketJ on 27-Jan-2016
        #region Audit UploadImage from web
        #region CUSTOM IMAGE CAPTURE
        [System.Web.Http.HttpPost]
        [ActionName("UploadAuditCustomImageFilesFromWeb")]
        public HttpResponseMessage UploadAuditCustomImageFilesFromWeb()
        {
            //JavaScriptSerializer s = new JavaScriptSerializer();
            //s.MaxJsonLength = 2147483644;
            //try
            //{
            //    string projectId = ""; string AccountNumber = "";
            //    string formId = ""; string accountInitialFieldId = "";
            //    int iUploadedCnt = 0;
            //    string fieldName = ""; string userId = "";

            //    HttpFileCollection hfc = HttpContext.Current.Request.Files;

            //    userId = HttpContext.Current.Request.Form["userId"];
            //    formId = HttpContext.Current.Request.Form["formId"];
            //    accountInitialFieldId = HttpContext.Current.Request.Form["accountInitialFieldId"];
            //    fieldName = HttpContext.Current.Request.Form["FieldName"];


            //    long _formId = 0;
            //    int _userId = 0; long _accountInitialFieldId = 0;
            //    bool validParameters = true;

            //    if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out _userId))
            //    {
            //        ErrorModelStatus em = new ErrorModelStatus();
            //        em.status = "fail";
            //        em.errorMessage = "userId field not valid.";
            //        var response = this.Request.CreateResponse(HttpStatusCode.OK);
            //        response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
            //        return response;
            //    }
            //    else if (string.IsNullOrEmpty(formId) || !long.TryParse(formId, out _formId))
            //    {
            //        ErrorModelStatus em = new ErrorModelStatus();
            //        em.status = "fail";
            //        em.errorMessage = "formId field not valid.";
            //        var response = this.Request.CreateResponse(HttpStatusCode.OK);
            //        response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
            //        return response;
            //    }
            //    else if (string.IsNullOrEmpty(accountInitialFieldId) || !long.TryParse(accountInitialFieldId, out _accountInitialFieldId))
            //    {
            //        ErrorModelStatus em = new ErrorModelStatus();
            //        em.status = "fail";
            //        em.errorMessage = "accountInitialFieldId field not valid.";
            //        var response = this.Request.CreateResponse(HttpStatusCode.OK);
            //        response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
            //        return response;
            //    }
            //    else if (string.IsNullOrEmpty(fieldName))
            //    {
            //        ErrorModelStatus em = new ErrorModelStatus();
            //        em.status = "fail";
            //        em.errorMessage = "fieldName field not valid.";
            //        var response = this.Request.CreateResponse(HttpStatusCode.OK);
            //        response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
            //        return response;
            //    }


            //    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            //    {
            //        System.Web.HttpPostedFile hpf = hfc[iCnt];
            //        string[] extension = hpf.FileName.Split('.');
            //        if (extension.Count() < 2)
            //        {
            //            validParameters = false;
            //        }
            //        else
            //        {
            //            if (extension[1].ToLower() != "jpeg" && extension[1].ToLower() != "jpg")
            //            {
            //                validParameters = false;
            //            }
            //        }
            //    }

            //    if (!validParameters)
            //    {
            //        ErrorModelStatus em = new ErrorModelStatus();
            //        em.status = "fail";
            //        em.errorMessage = "only .jpeg/.jpg images allowed.";
            //        var response = this.Request.CreateResponse(HttpStatusCode.OK);
            //        response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
            //        return response;
            //    }
            //    else
            //    {

            //        CompassEntities objCompassEntities = new CompassEntities();

            //        projectId = Convert.ToString(objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).Select(o => o.ProjectId).FirstOrDefault());

            //        if (string.IsNullOrEmpty(projectId))
            //        {
            //            ErrorModelStatus em = new ErrorModelStatus();
            //            em.status = "fail";
            //            em.errorMessage = "formId not present in the database.";
            //            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            //            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
            //            return response;
            //        }

            //        AccountNumber = Convert.ToString(objCompassEntities.UploadedExcelDatas.
            //           Where(o => o.Id == _accountInitialFieldId).Select(o => o.Account12).FirstOrDefault());
            //        if (string.IsNullOrEmpty(AccountNumber))
            //        {
            //            ErrorModelStatus em = new ErrorModelStatus();
            //            em.status = "fail";
            //            em.errorMessage = "accountInitialFieldId not present in the database.";
            //            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            //            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
            //            return response;
            //        }


            //        var projectFieldData = objCompassEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == _formId
            //           && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

            //        if (projectFieldData == null)
            //        {
            //            ErrorModelStatus em = new ErrorModelStatus();
            //            em.status = "fail";
            //            em.errorMessage = "Please save the basic info first for the image.";
            //            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            //            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
            //            return response;
            //        }



            //        var fielddata = objCompassEntities.ProjectAuditFieldDataMasters.Where(o => o.Active == 1
            //            && o.FK_ProjectAuditData == projectFieldData.ID &&
            //            o.InitialFieldName.ToLower() == fieldName.ToLower()).FirstOrDefault();

            //        if (fielddata == null)
            //        {
            //            ErrorModelStatus em = new ErrorModelStatus();
            //            em.status = "fail";
            //            em.errorMessage = "Please save the basic service info first for the image.";
            //            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            //            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
            //            return response;
            //        }


            //        string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
            //            AccountNumber + @"\" + accountInitialFieldId + @"\" + fieldName + @"\";
            //        string filePath = HttpContext.Current.Request.MapPath(tempPath);
            //        grantAccess(filePath);

            //        List<string> imageFileNameList = new List<string>();
            //        for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            //        {
            //            System.Web.HttpPostedFile hpf = hfc[iCnt];

            //            if (hpf.ContentLength > 0)
            //            {
            //                //// CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
            //                //if (!File.Exists(filePath + Path.GetFileName(hpf.FileName)))
            //                //{
            //                //    // SAVE THE FILES IN THE FOLDER.
            //                //    hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
            //                //}

            //                if (System.IO.File.Exists(filePath + Path.GetFileName(hpf.FileName)))
            //                {
            //                    System.GC.Collect();
            //                    System.GC.WaitForPendingFinalizers();

            //                    System.IO.File.Delete(filePath + Path.GetFileName(hpf.FileName));

            //                }
            //                hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));


            //                string _imagePath = filePath + Path.GetFileName(hpf.FileName);
            //                // Rotate the image according to EXIF data
            //                //var bmp = new Bitmap(_imagePath);

            //                //if (System.IO.File.Exists(_imagePath))
            //                //{
            //                //    System.IO.File.Delete(_imagePath);
            //                //}

            //                // var exif = new EXIFextractor(ref bmp, "n");

            //                //if (exif["Orientation"] != null)
            //                //{
            //                //    RotateFlipType flip = OrientationToFlipType(exif["Orientation"].ToString());

            //                //    if (flip != RotateFlipType.RotateNoneFlipNone) // don't flip of orientation is correct
            //                //    {
            //                //        bmp.RotateFlip(flip);
            //                //        exif.setTag(0x112, "1"); // Optional: reset orientation tag
            //                //        bmp.Save(_imagePath, ImageFormat.Jpeg);
            //                //    }
            //                //}


            //                iUploadedCnt = iUploadedCnt + 1;
            //                imageFileNameList.Add(tempPath + Path.GetFileName(hpf.FileName));

            //            }
            //        }



            //        foreach (var item in imageFileNameList)
            //        {
            //            var obj = objCompassEntities.ProjectAuditFieldDataAttachments.Where(o => o.Active == 1 &&
            //               o.FK_ProjectAuditFieldDataMaster_ID == fielddata.ID && o.IsAudioVideoImage == 3 &&
            //               o.StrFilePath == item && o.IsServiceData == false).FirstOrDefault();
            //            if (obj == null)
            //            {
            //                obj = new ProjectAuditFieldDataAttachment();
            //                obj.Active = 1;
            //                obj.CreatedOn = new CommonFunctions().ServerDate();
            //                obj.CreatedBy = _userId;
            //                obj.FK_ProjectAuditFieldDataMaster_ID = fielddata.ID;
            //                obj.IsAudioVideoImage = 3;
            //                obj.IsSkipped = false;
            //                obj.StrFilePath = item;
            //                //obj.tblService_Id = _serviceInitialId;
            //                //obj.TblServicePicture_ID = rv.ID;
            //                obj.IsServiceData = false;
            //                objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
            //                objCompassEntities.SaveChanges();
            //            }
            //        }

            //        //if (imageFileNameList.Count > 0)
            //        //{

            //        ErrorModelOnlyStatus em1 = new ErrorModelOnlyStatus();
            //        em1.status = "Success";
            //        var response1 = this.Request.CreateResponse(HttpStatusCode.OK);
            //        response1.Content = new StringContent(s.Serialize(em1), Encoding.UTF8, "application/json");
            //        return response1;
            //        //}
            //        //else
            //        //{
            //        //    ErrorModelStatus em = new ErrorModelStatus();
            //        //    em.status = "fail";
            //        //    em.errorMessage = "Failed to save. Image with same name already exist.";
            //        //    var response = this.Request.CreateResponse(HttpStatusCode.OK);
            //        //    response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
            //        //    return response;
            //        //}
            //    }


            //}
            //catch (Exception ex)
            //{
            //    ErrorModelStatus em = new ErrorModelStatus();
            //    em.status = "Error";
            //    em.errorMessage = ex.Message;
            //    var response = this.Request.CreateResponse(HttpStatusCode.OK);
            //    response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
            //    return response;

            //}


            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            try
            {
                string projectId = ""; string AccountNumber = "";
                string formId = ""; string accountInitialFieldId = "";
                int iUploadedCnt = 0;
                string fieldName = ""; string userId = "";
                string currentFieldDataMasterId = "";

                HttpFileCollection hfc = HttpContext.Current.Request.Files;

                userId = HttpContext.Current.Request.Form["userId"];
                formId = HttpContext.Current.Request.Form["formId"];
                accountInitialFieldId = HttpContext.Current.Request.Form["accountInitialFieldId"];
                fieldName = HttpContext.Current.Request.Form["FieldName"];
                currentFieldDataMasterId = HttpContext.Current.Request.Form["fieldDataMasterId"];

                long _formId = 0;
                int _userId = 0; long _accountInitialFieldId = 0;
                bool validParameters = true;
                long _currentFieldDataMasterId = 0;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out _userId))
                {
                    return JsonErrorResponse(s, "fail", "userId field not valid.");
                }
                else if (string.IsNullOrEmpty(formId) || !long.TryParse(formId, out _formId))
                {
                    return JsonErrorResponse(s, "fail", "formId field not valid.");
                }
                else if (string.IsNullOrEmpty(accountInitialFieldId) || !long.TryParse(accountInitialFieldId, out _accountInitialFieldId))
                {
                    return JsonErrorResponse(s, "fail", "accountInitialFieldId field not valid.");
                }
                else if (string.IsNullOrEmpty(fieldName))
                {
                    return JsonErrorResponse(s, "fail", "fieldName field not valid.");
                }
                else if (!string.IsNullOrEmpty(currentFieldDataMasterId) && !long.TryParse(currentFieldDataMasterId, out _currentFieldDataMasterId))
                {
                    return JsonErrorResponse(s, "fail", "fieldDataMasterId field not valid.");
                }

                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    string[] extension = hpf.FileName.Split('.');
                    if (extension.Count() < 2)
                    {
                        validParameters = false;
                    }
                    else
                    {
                        if (extension[1].ToLower() != "jpeg" && extension[1].ToLower() != "jpg")
                        {
                            validParameters = false;
                        }
                    }
                }

                if (!validParameters)
                {
                    return JsonErrorResponse(s, "fail", "only .jpeg/.jpg images allowed.");
                }
                else
                {

                    CompassEntities objCompassEntities = new CompassEntities();

                    projectId = Convert.ToString(objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).Select(o => o.ProjectId).FirstOrDefault());

                    if (string.IsNullOrEmpty(projectId))
                    {
                        return JsonErrorResponse(s, "fail", "formId not present in the database.");
                    }

                    AccountNumber = Convert.ToString(objCompassEntities.UploadedExcelDatas.
                       Where(o => o.Id == _accountInitialFieldId).Select(o => o.Account12).FirstOrDefault());
                    if (string.IsNullOrEmpty(AccountNumber))
                    {
                        return JsonErrorResponse(s, "fail", "accountInitialFieldId not present in the database.");
                    }


                    //check single photo field or not.

                    var ImageType = objCompassEntities.PROC_CheckImageSingleMultiple(_formId, fieldName).FirstOrDefault();

                    if (ImageType == "SinglePhoto" && _currentFieldDataMasterId > 0)
                    {
                        var currentActiveRecord = objCompassEntities.ProjectAuditFieldDataAttachments.Where(o =>
                            o.FK_ProjectAuditFieldDataMaster_ID == _currentFieldDataMasterId && o.IsAudioVideoImage == 3 && o.Active == 1).FirstOrDefault();

                        if (currentActiveRecord != null)
                        {
                            currentActiveRecord.Active = 0;
                            currentActiveRecord.ModifiedBy = _userId;
                            currentActiveRecord.ModifiedOn = new CommonFunctions().ServerDate();
                            objCompassEntities.SaveChanges();
                        }
                    }


                    string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
                        AccountNumber + @"\" + accountInitialFieldId + @"\" + fieldName + @"\";

                    string filePath = HttpContext.Current.Request.MapPath(tempPath);
                    grantAccess(filePath);

                    List<string> imageFileNameList = new List<string>();
                    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                    {
                        System.Web.HttpPostedFile hpf = hfc[iCnt];

                        if (hpf.ContentLength > 0)
                        {

                            if (System.IO.File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                            {
                                System.GC.Collect();
                                System.GC.WaitForPendingFinalizers();
                                System.IO.File.Delete(filePath + Path.GetFileName(hpf.FileName));

                            }
                            hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));


                            string _imagePath = filePath + Path.GetFileName(hpf.FileName);

                            iUploadedCnt = iUploadedCnt + 1;
                            imageFileNameList.Add(tempPath + Path.GetFileName(hpf.FileName));

                        }
                    }



                    foreach (var item in imageFileNameList)
                    {
                        var obj = objCompassEntities.ProjectAuditFieldDataAttachments.Where(o => o.Active == 1 &&
                           o.FK_ProjectAuditFieldDataMaster_ID == _currentFieldDataMasterId && o.IsAudioVideoImage == 3 &&
                           o.StrFilePath == item && o.IsServiceData == false).FirstOrDefault();
                        if (obj == null)
                        {
                            obj = new ProjectAuditFieldDataAttachment();
                            obj.Active = 1;
                            obj.CreatedOn = new CommonFunctions().ServerDate();
                            obj.CreatedBy = _userId;
                            obj.FK_ProjectAuditFieldDataMaster_ID = _currentFieldDataMasterId;
                            obj.IsAudioVideoImage = 3;
                            obj.IsSkipped = false;
                            obj.StrFilePath = item;
                            //obj.tblService_Id = _serviceInitialId;
                            //obj.TblServicePicture_ID = rv.ID;
                            obj.IsServiceData = false;
                            objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
                            objCompassEntities.SaveChanges();
                        }
                    }



                    return JsonErrorResponse(s, "Success", "File Uploaded Sucessfully.");
                }


            }
            catch (Exception ex)
            {
                return JsonErrorResponse(s, "Error", "Error -" + ex.Message);
            }
        }

        #endregion


        #region SERVICE IMAGE CAPTURE
        [System.Web.Http.HttpPost]
        [ActionName("UploadAuditServiceImageFilesFromWeb")]
        public HttpResponseMessage UploadAuditServiceImageFilesFromWeb()
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            s.MaxJsonLength = 2147483644;
            try
            {


                string projectId = ""; string AccountNumber = "";
                string formId = ""; string accountInitialFieldId = "";
                string imageName = ""; int iUploadedCnt = 0;
                string serviceinitialId = ""; string userId = "";
                string isAdditionalService = "";
                HttpFileCollection hfc = HttpContext.Current.Request.Files;

                userId = HttpContext.Current.Request.Form["userId"];
                formId = HttpContext.Current.Request.Form["formId"];
                accountInitialFieldId = HttpContext.Current.Request.Form["accountInitialFieldId"];
                serviceinitialId = HttpContext.Current.Request.Form["serviceinitialId"];
                isAdditionalService = HttpContext.Current.Request.Form["isAdditionalService"];
                imageName = HttpContext.Current.Request.Form["imageName"];

                long _formId = 0; long _serviceInitialId = 0;
                int _userId = 0; long _accountInitialFieldId = 0; bool _isAdditionalServices = false;
                bool validParameters = true;

                #region validation
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out _userId))
                {
                    ErrorModelStatus em = new ErrorModelStatus();
                    em.status = "fail";
                    em.errorMessage = "userId field not valid.";
                    var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                    return response;
                }
                else if (string.IsNullOrEmpty(formId) || !long.TryParse(formId, out _formId))
                {
                    ErrorModelStatus em = new ErrorModelStatus();
                    em.status = "fail";
                    em.errorMessage = "formId field not valid.";
                    var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                    return response;
                }
                else if (string.IsNullOrEmpty(accountInitialFieldId) || !long.TryParse(accountInitialFieldId, out _accountInitialFieldId))
                {
                    ErrorModelStatus em = new ErrorModelStatus();
                    em.status = "fail";
                    em.errorMessage = "accountInitialFieldId field not valid.";
                    var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                    return response;
                }
                else if (string.IsNullOrEmpty(serviceinitialId) || !long.TryParse(serviceinitialId, out _serviceInitialId))
                {
                    ErrorModelStatus em = new ErrorModelStatus();
                    em.status = "fail";
                    em.errorMessage = "serviceinitialId field not valid.";
                    var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                    return response;
                }
                else if (string.IsNullOrEmpty(isAdditionalService) || !bool.TryParse(isAdditionalService, out _isAdditionalServices))
                {
                    ErrorModelStatus em = new ErrorModelStatus();
                    em.status = "fail";
                    em.errorMessage = "isAdditionalService field not valid.";
                    var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                    return response;
                }
                else if (string.IsNullOrEmpty(imageName))
                {
                    ErrorModelStatus em = new ErrorModelStatus();
                    em.status = "fail";
                    em.errorMessage = "imageName field empty.";
                    var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                    return response;
                }
                #endregion

                for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                {
                    System.Web.HttpPostedFile hpf = hfc[iCnt];
                    string[] extension = hpf.FileName.Split('.');
                    if (extension.Count() < 2)
                    {
                        validParameters = false;
                    }
                    else
                    {
                        if (extension[1].ToLower() != "jpeg" && extension[1].ToLower() != "jpg")
                        {
                            validParameters = false;
                        }
                    }
                }

                if (!validParameters)
                {
                    ErrorModelStatus em = new ErrorModelStatus();
                    em.status = "fail";
                    em.errorMessage = "only .jpeg/.jpg images allowed.";
                    var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                    return response;
                }
                else
                {

                    CompassEntities objCompassEntities = new CompassEntities();




                    projectId = Convert.ToString(objCompassEntities.tblFormMasters.Where(o => o.FormId == _formId).Select(o => o.ProjectId).FirstOrDefault());

                    if (string.IsNullOrEmpty(projectId))
                    {
                        ErrorModelStatus em = new ErrorModelStatus();
                        em.status = "fail";
                        em.errorMessage = "formId not present in the database.";
                        var response = this.Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                        return response;
                    }

                    AccountNumber = Convert.ToString(objCompassEntities.UploadedExcelDatas.
                       Where(o => o.Id == _accountInitialFieldId).Select(o => o.Account12).FirstOrDefault());
                    if (string.IsNullOrEmpty(AccountNumber))
                    {
                        ErrorModelStatus em = new ErrorModelStatus();
                        em.status = "fail";
                        em.errorMessage = "accountInitialFieldId not present in the database.";
                        var response = this.Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                        return response;
                    }

                    string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
                        AccountNumber + @"\" + accountInitialFieldId + @"\" + imageName + @"\";
                    string filePath = HttpContext.Current.Request.MapPath(tempPath);
                    grantAccess(filePath);

                    List<string> imageFileNameList = new List<string>();
                    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
                    {
                        System.Web.HttpPostedFile hpf = hfc[iCnt];

                        if (hpf.ContentLength > 0)
                        {
                            // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)


                            if (System.IO.File.Exists(filePath + Path.GetFileName(hpf.FileName)))
                            {
                                System.IO.File.Delete(filePath + Path.GetFileName(hpf.FileName));
                            }
                            hpf.SaveAs(filePath + Path.GetFileName(hpf.FileName));
                            string _imagePath = filePath + Path.GetFileName(hpf.FileName);

                            iUploadedCnt = iUploadedCnt + 1;
                            imageFileNameList.Add(tempPath + Path.GetFileName(hpf.FileName));
                        }
                    }




                    var pictureRecord = objCompassEntities.tblPictures.Where(o => o.Picture.ToLower() == imageName.ToLower()).Select(o => o).FirstOrDefault();

                    if (pictureRecord == null)
                    {
                        ErrorModelStatus em = new ErrorModelStatus();
                        em.status = "fail";
                        em.errorMessage = "Image data not found for the current imageName.";
                        var response = this.Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                        return response;
                    }
                    long masterId = 0;
                    if (_isAdditionalServices)
                    {
                        var objmasterFieldRelation = objCompassEntities.TblProjectMasterFieldRelations.Where(o => o.MasterFieldID == _serviceInitialId).Select(o => o).FirstOrDefault();
                        _serviceInitialId = objmasterFieldRelation.MasterFieldID;

                        masterId = objmasterFieldRelation.ID;
                    }
                    var rv = objCompassEntities.TblServicePictures.Where(o => o.tblPicture_PictureId == pictureRecord.ID
                        && o.tblService_ServiceId == _serviceInitialId).Select(o => o).FirstOrDefault();

                    if (rv == null)
                    {
                        ErrorModelStatus em = new ErrorModelStatus();
                        em.status = "fail";
                        em.errorMessage = "No data not found for the current serviceinitialId.";
                        var response = this.Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                        return response;
                    }


                    var projectFieldData = objCompassEntities.ProjectAuditDatas.Where(o => o.Active == 1 && o.FormId == _formId
                        && o.FK_UploadedExcelData_Id == _accountInitialFieldId).FirstOrDefault();

                    if (projectFieldData == null)
                    {
                        ErrorModelStatus em = new ErrorModelStatus();
                        em.status = "fail";
                        em.errorMessage = "Please save the basic info first for the image.";
                        var response = this.Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                        return response;
                    }


                    ProjectAuditFieldDataMaster fielddata;
                    if (!_isAdditionalServices)
                    {
                        fielddata = objCompassEntities.ProjectAuditFieldDataMasters.Where(o => o.Active == 1
                           && o.FK_ProjectAuditData == projectFieldData.ID &&
                           o.InitialFieldName.ToLower() == "SERVICES".ToLower()).FirstOrDefault();

                        if (fielddata == null)
                        {
                            ErrorModelStatus em = new ErrorModelStatus();
                            em.status = "fail";
                            em.errorMessage = "Please save the basic service info first for the image.";
                            var response = this.Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                            return response;
                        }
                    }
                    else
                    {
                        fielddata = objCompassEntities.ProjectAuditFieldDataMasters.Where(o => o.Active == 1
                                                 && o.FK_ProjectAuditData == projectFieldData.ID &&
                                                 o.InitialFieldName.ToLower() == "ADDITIONAL SERVICES".ToLower()).FirstOrDefault();

                        if (fielddata == null)
                        {
                            ErrorModelStatus em = new ErrorModelStatus();
                            em.status = "fail";
                            em.errorMessage = "Please save the basic service info first for the image.";
                            var response = this.Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                            return response;
                        }
                    }


                    foreach (var item in imageFileNameList)
                    {

                        var obj = objCompassEntities.ProjectAuditFieldDataAttachments.Where(o => o.Active == 1 &&
                            o.FK_ProjectAuditFieldDataMaster_ID == fielddata.ID && o.IsAudioVideoImage == 3 &&
                            o.StrFilePath == item && o.Fk_tblService_Id == _serviceInitialId && o.FK_TblServicePicture_ID == rv.ID
                            && o.IsServiceData == true).FirstOrDefault();
                        if (obj == null)
                        {
                            obj = new ProjectAuditFieldDataAttachment();
                            obj.Active = 1;
                            obj.CreatedOn = new CommonFunctions().ServerDate();
                            obj.CreatedBy = _userId;
                            obj.FK_ProjectAuditFieldDataMaster_ID = fielddata.ID;
                            obj.IsAudioVideoImage = 3;
                            obj.IsSkipped = false;
                            obj.StrFilePath = item;

                            //obj.tblService_Id = !_isAdditionalServices ? _serviceInitialId : masterId;
                            obj.Fk_tblService_Id = _serviceInitialId;
                            obj.FK_TblServicePicture_ID = rv.ID;
                            obj.IsServiceData = true;
                            objCompassEntities.ProjectAuditFieldDataAttachments.Add(obj);
                            objCompassEntities.SaveChanges();
                        }
                    }

                    if (imageFileNameList.Count > 0)
                    {

                        ErrorModelOnlyStatus em = new ErrorModelOnlyStatus();
                        em.status = "Success";
                        var response = this.Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                        return response;
                    }
                    else
                    {
                        ErrorModelStatus em = new ErrorModelStatus();
                        em.status = "fail";
                        em.errorMessage = "Failed to save. Image with same name already exist.";
                        var response = this.Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                        return response;
                    }
                }


            }
            catch (Exception ex)
            {
                ErrorModelStatus em = new ErrorModelStatus();
                em.status = "Error";
                em.errorMessage = ex.Message;
                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(s.Serialize(em), Encoding.UTF8, "application/json");
                return response;

            }
        }

        #endregion

        #endregion

    }


}