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
   public class MeterMakeDAL
   {


       #region GetMethods

       public MeterMakeModel GetByMakeMeterId(long Id)
       {
           MeterMakeModel objMeterMakeModel=new MeterMakeModel();
           CompassEntities objCompassEntities = new CompassEntities();
           tblMeterMake objtblMeterMake = (from b in objCompassEntities.tblMeterMakes
                                               where b.ID == Id && b.Active == 1
                                               select b).FirstOrDefault();
           Mapper.DynamicMap<tblMeterMake, MeterMakeModel>(objtblMeterMake, objMeterMakeModel);
           return objMeterMakeModel;
       }
       public MeterMakeModel GetAllMakeMeterId(long Id)
       {
           MeterMakeModel objMeterMakeModel = new MeterMakeModel();
           CompassEntities objCompassEntities = new CompassEntities();
           tblMeterMake objtblMeterMake = (from b in objCompassEntities.tblMeterMakes
                                           where b.ID == Id  
                                           select b).FirstOrDefault();
           Mapper.DynamicMap<tblMeterMake, MeterMakeModel>(objtblMeterMake, objMeterMakeModel);
           return objMeterMakeModel;
       }


       public List<MeterMakeModel> GetMakeMeters()
       {
           CompassEntities objCompassEntities = new CompassEntities();
           var objMeterMakeModel = new List<MeterMakeModel>();
           foreach (tblMeterMake obj in objCompassEntities.tblMeterMakes.Where(A => A.Active == 1).OrderBy(t => t.Make))
           {
               MeterMakeModel myobj = new MeterMakeModel();
               Mapper.DynamicMap<tblMeterMake, MeterMakeModel>(obj, myobj);
               objMeterMakeModel.Add(myobj);
           }
           return objMeterMakeModel;
       }


       public List<MeterMakeModel> GetAllMakeMeters()
       {
           CompassEntities objCompassEntities = new CompassEntities();

           var objMeterMakeModel = new List<MeterMakeModel>();
           foreach (tblMeterMake obj in objCompassEntities.tblMeterMakes.OrderBy(t => t.Make))
           {
               MeterMakeModel myobj = new MeterMakeModel();
               Mapper.DynamicMap<tblMeterMake, MeterMakeModel>(obj, myobj);
               objMeterMakeModel.Add(myobj);
           }
           return objMeterMakeModel;
       }

       #endregion

       #region Insert,Update,Delete
       public bool CreateOrUpdate(MeterMakeModel ObjectToCreaetOrUpdate, out string Message)
       {
           bool result = false;
           Message = "Unable to Create / Update Meter Make.";
           try
           {
               CompassEntities objCompassEntities = new CompassEntities();

               //here check the duplicate Menu name
               // According to user requirement changes condition here for b.Menu.ToLower() == ObjectToCreaetOrUpdate.Menu.ToLower()
               tblMeterMake objChk = (from b in objCompassEntities.tblMeterMakes
                                      where b.Make.ToLower().Trim() == ObjectToCreaetOrUpdate.Make.ToLower().Trim()
                                      && b.ID != ObjectToCreaetOrUpdate.ID
                                          select b).FirstOrDefault();

               //using (TransactionScope ts = new TransactionScope())
               //{
               if (objChk == null)//No duplicate records
               {
                   if (ObjectToCreaetOrUpdate.ID == 0) //new record for insert
                   {
                       tblMeterMake createOrUpdate = new tblMeterMake();

                       Mapper.DynamicMap<MeterMakeModel, tblMeterMake>(ObjectToCreaetOrUpdate, createOrUpdate);
                       objCompassEntities.tblMeterMakes.Add(createOrUpdate);
                       objCompassEntities.SaveChanges();

                       //ts.Complete();
                       //change message accroding to your requirement

                       Message = CommonFunctions.strRecordCreated;
                       result = true;
                   }
                   else //update records
                   {
                       tblMeterMake createOrUpdate = (from b in objCompassEntities.tblMeterMakes
                                                      where b.Active == 1 && b.ID == ObjectToCreaetOrUpdate.ID
                                                          select b).FirstOrDefault();
                       if (createOrUpdate != null)
                       {
                           Mapper.DynamicMap<MeterMakeModel, tblMeterMake>(ObjectToCreaetOrUpdate, createOrUpdate);
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


               tblMeterMake ObjMenu = (from b in objCompassEntities.tblMeterMakes
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

       public List<MeterMakeModel> GetDeActiveRecords()
       {
           var list = new List<MeterMakeModel>();
           CompassEntities CompassEntities = new CompassEntities();

           list = CompassEntities.tblMeterMakes.Where(a => a.Active == 0).Select(x => new MeterMakeModel { Make = x.Make, ID = x.ID }).ToList();
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

               tblMeterMake ObjFaq = (from b in objCompassEntities.tblMeterMakes
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
