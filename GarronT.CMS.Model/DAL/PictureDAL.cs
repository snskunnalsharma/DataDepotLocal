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
   public class PictureDAL
   {


       #region GetMethods

       public PictureModel GetByPictureId(long Id)
       {
           PictureModel objPictureModel = new PictureModel();
           CompassEntities objCompassEntities = new CompassEntities();
           tblPicture objtblPicture = (from b in objCompassEntities.tblPictures
                                           where b.ID == Id && b.Active == 1
                                           select b).FirstOrDefault();
           Mapper.DynamicMap<tblPicture, PictureModel>(objtblPicture, objPictureModel);
           return objPictureModel;
       }
       public PictureModel GetAllPictureId(long Id)
       {
           PictureModel objPictureModel = new PictureModel();
           CompassEntities objCompassEntities = new CompassEntities();
           tblPicture objtblPicture = (from b in objCompassEntities.tblPictures
                                           where b.ID == Id
                                           select b).FirstOrDefault();
           Mapper.DynamicMap<tblPicture, PictureModel>(objtblPicture, objPictureModel);
           return objPictureModel;
       }


       public List<PictureModel> GetPictures()
       {
           CompassEntities objCompassEntities = new CompassEntities();
           var objPictureModel = new List<PictureModel>();
           foreach (tblPicture obj in objCompassEntities.tblPictures.Where(A => A.Active == 1).OrderBy(t => t.Picture))
           {
               PictureModel myobj = new PictureModel();
               Mapper.DynamicMap<tblPicture, PictureModel>(obj, myobj);
               objPictureModel.Add(myobj);
           }
           return objPictureModel;
       }


       public List<PictureModel> GetAllPictures()
       {
           CompassEntities objCompassEntities = new CompassEntities();

           var objPictureModel = new List<PictureModel>();
           foreach (tblPicture obj in objCompassEntities.tblPictures.OrderBy(t => t.Picture))
           {
               PictureModel myobj = new PictureModel();
               Mapper.DynamicMap<tblPicture, PictureModel>(obj, myobj);
               objPictureModel.Add(myobj);
           }
           return objPictureModel;
       }
       #endregion


       #region Insert,Update,Delete
       public bool CreateOrUpdate(PictureModel ObjectToCreaetOrUpdate, out string Message)
       {
           bool result = false;
           Message = "Unable to Create / Update Meter Make.";
           try
           {
               CompassEntities objCompassEntities = new CompassEntities();

               //here check the duplicate Menu name
               // According to user requirement changes condition here for b.Menu.ToLower() == ObjectToCreaetOrUpdate.Menu.ToLower()
               tblPicture objChk = (from b in objCompassEntities.tblPictures
                                    where b.Picture.ToLower().Trim() == ObjectToCreaetOrUpdate.Picture.ToLower().Trim()
                                      && b.ID != ObjectToCreaetOrUpdate.ID
                                      select b).FirstOrDefault();

               //using (TransactionScope ts = new TransactionScope())
               //{
               if (objChk == null)//No duplicate records
               {
                   if (ObjectToCreaetOrUpdate.ID == 0) //new record for insert
                   {
                       tblPicture createOrUpdate = new tblPicture();

                       Mapper.DynamicMap<PictureModel, tblPicture>(ObjectToCreaetOrUpdate, createOrUpdate);
                       objCompassEntities.tblPictures.Add(createOrUpdate);
                       objCompassEntities.SaveChanges();

                       //ts.Complete();
                       //change message accroding to your requirement

                       Message = CommonFunctions.strRecordCreated;
                       result = true;
                   }
                   else //update records
                   {
                       tblPicture createOrUpdate = (from b in objCompassEntities.tblPictures
                                                      where b.Active == 1 && b.ID == ObjectToCreaetOrUpdate.ID
                                                      select b).FirstOrDefault();
                       if (createOrUpdate != null)
                       {
                           Mapper.DynamicMap<PictureModel, tblPicture>(ObjectToCreaetOrUpdate, createOrUpdate);
                       }
                       objCompassEntities.SaveChanges();
                       // ts.Complete();
                       //change message accroding to your requirement
                       Message = CommonFunctions.strRecordUpdated;
                       result = true;
                   }
               }

               else
               {
                   if (objChk.Active == 1)
                   {
                       Message = CommonFunctions.strActiveRecordExists;
                       return false;
                   }
                   else if (objChk.Active == 0)
                   {
                       Message = CommonFunctions.strDeactiveRecordExists;
                       return false;
                   }
                   else
                   {
                       Message = CommonFunctions.strDefaultAdd;
                       return false;
                   }

                   //change message accroding to your requirement
                   // Message = "Sub Menu already exists.";
                   result = false;
               }
               //}

           }

           catch (Exception ex)
           {

               Message += "<br/>" + ex.Message;
           }

           return result;

       }

       public bool Delete(int Id)
       {
           try
           {

               //using (TransactionScope ts = new TransactionScope())
               //{
               CompassEntities objCompassEntities = new CompassEntities();


               tblPicture ObjMenu = (from b in objCompassEntities.tblPictures
                                       where b.Active == 1 && b.ID == Id
                                       select b).FirstOrDefault();

               ObjMenu.Active = 0;
               // ObjMenu.ModifiedBy = Convert.ToInt64(HttpContext.Current.Session["UserId"].ToString());
               //ObjMenu.ModifiedOn = new CommonFunctions().ServerDate();               
               objCompassEntities.SaveChanges();

               return true;
           }
           catch (Exception)
           {

               throw;
           }
       }
       #endregion


       public List<PictureModel> GetDeActiveRecords()
       {
           var list = new List<PictureModel>();
           CompassEntities CompassEntities = new CompassEntities();

           list = CompassEntities.tblPictures.Where(a => a.Active == 0).Select(x => new PictureModel { Picture = x.Picture, ID = x.ID }).ToList();
           return list;
       }


       public bool ActivateRecord(int Id, out string msg)
       {
           try
           {
               msg = "";

               //  using (TransactionScope ts = new TransactionScope())
               // {
               CompassEntities objCompassEntities = new CompassEntities();

               tblPicture ObjFaq = (from b in objCompassEntities.tblPictures
                                      where b.ID == Id
                                      select b).FirstOrDefault();


               if (ObjFaq != null)
               {
                   ObjFaq.Active = 1;
                   //ObjFaq.ModifiedBy = Convert.ToInt64(HttpContext.Current.Session["UserId"].ToString());
                   ObjFaq.ModifiedOn = new CommonFunctions().ServerDate();
                   objCompassEntities.SaveChanges();

                   msg = "Record activated.";
                   return true;
                   // ts.Complete();
               }
               else
               {
                   msg = "Unable to Activate Record.";
                   return false;
               }
               //}


           }
           catch (Exception)
           {

               throw;
           }
       }

   }
}
