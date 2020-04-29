using System;
using System.Web;
using AutoMapper;
using System.Linq;
using System.Text;
using System.Reflection;
using GarronT.CMS.Model.BO;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GarronT.CMS.Model.DAL
{
   public  class ServicePictureDAL
    {
        #region GetMenthods

        public List<ServicesPicture> GetServicePictureDeatils(int id)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var list = new List<ServicesPicture>();
          
            List<ServicesPicture> objSubMenuesBo = (from b in CompassEntities.TblServicePictures
                                                    join c in CompassEntities.tblPictures on b.tblPicture_PictureId equals c.ID
                                                              where b.Active == 1 && b.tblService_ServiceId == id
                                                              select new ServicesPicture
                                                              {

                                                                  PictureId=b.tblPicture_PictureId,
                                                                  PictureName=c.Picture
                                                                  // S_VIP = b.VIP == 1 ? "Yes" : "No",


                                                              }).ToList();

            foreach (var obj in objSubMenuesBo)
            {

                ServicesPicture myobj = new ServicesPicture();
           //     myobj.TempID = i;
                  myobj.PictureId=obj.PictureId;
                  myobj.PictureName = obj.PictureName;
            
                list.Add(myobj);
            }
            return list;
        }


        #endregion


        #region Insert,Update & Delete

        public bool CreateOrUpdate(long ServiceID, List<string> objServicePictureList, out string returnMessage)
        {
            bool result = false;
            returnMessage = "Unable to Create / Update State.";

            CompassEntities CompassEntities = new CompassEntities();
            try
            {
                foreach (var item in objServicePictureList)
                {
                    TblServicePicture obj = new TblServicePicture();
                    obj.tblPicture_PictureId = long.Parse(item);
                    obj.tblService_ServiceId = ServiceID;
                    obj.CreatedBy = null;
                    obj.Active = 1;
                    obj.CreatedOn = new CommonFunctions().ServerDate();
                    CompassEntities.TblServicePictures.Add(obj);
                }

                CompassEntities.SaveChanges();
                returnMessage = CommonFunctions.strRecordCreated;
                result = true;
            }
            catch (Exception ex)
            {

                returnMessage += "<br/>" + ex.Message;
                result = false;
            }
            return result;

        }

        #endregion
    }
}
