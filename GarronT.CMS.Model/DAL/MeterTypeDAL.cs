using AutoMapper;
using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GarronT.CMS.Model.DAL
{
   public class MeterTypeDAL
    {

        #region GetMethods

        public MeterTypeModel GetByMakeMeterId(long Id)
        {
            MeterTypeModel objMeterTypeModel = new MeterTypeModel();
            CompassEntities objCompassEntities = new CompassEntities();
            tblMeterType objtblMeterType = (from b in objCompassEntities.tblMeterTypes
                                            where b.ID == Id && b.Active == 1
                                            select b).FirstOrDefault();
            Mapper.DynamicMap<tblMeterType, MeterTypeModel>(objtblMeterType, objMeterTypeModel);
            return objMeterTypeModel;
        }
        public MeterTypeModel GetAllMakeMeterId(long Id)
        {
            MeterTypeModel objMeterTypeModel = new MeterTypeModel();
            CompassEntities objCompassEntities = new CompassEntities();
            tblMeterType objtblMeterType = (from b in objCompassEntities.tblMeterTypes
                                            where b.ID == Id
                                            select b).FirstOrDefault();
            Mapper.DynamicMap<tblMeterType, MeterTypeModel>(objtblMeterType, objMeterTypeModel);
            return objMeterTypeModel;
        }


        public List<MeterTypeModel> GetMakeMeters()
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var objMeterTypeModel = new List<MeterTypeModel>();
            foreach (tblMeterType obj in objCompassEntities.tblMeterTypes.Where(A => A.Active == 1).OrderBy(t => t.MeterType))
            {
                MeterTypeModel myobj = new MeterTypeModel();
                Mapper.DynamicMap<tblMeterType, MeterTypeModel>(obj, myobj);
                objMeterTypeModel.Add(myobj);
            }
            return objMeterTypeModel;
        }


        public List<MeterTypeModel> GetAllMakeMeters()
        {
            CompassEntities objCompassEntities = new CompassEntities();

            var objMeterTypeModel = new List<MeterTypeModel>();
            foreach (tblMeterType obj in objCompassEntities.tblMeterTypes.OrderBy(t => t.MeterType))
            {
                MeterTypeModel myobj = new MeterTypeModel();
                Mapper.DynamicMap<tblMeterType, MeterTypeModel>(obj, myobj);
                objMeterTypeModel.Add(myobj);
            }
            return objMeterTypeModel;
        }

        #endregion

        #region Insert,Update,Delete
        public bool CreateOrUpdate(MeterTypeModel ObjectToCreaetOrUpdate, out string Message)
        {
            bool result = false;
            Message = "Unable to Create / Update Meter Type.";
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                //here check the duplicate Menu name
                // According to user requirement changes condition here for b.Menu.ToLower() == ObjectToCreaetOrUpdate.Menu.ToLower()
                tblMeterType objChk = (from b in objCompassEntities.tblMeterTypes
                                       where b.MeterType.ToLower().Trim() == ObjectToCreaetOrUpdate.MeterType.ToLower().Trim()
                                       && b.ID != ObjectToCreaetOrUpdate.ID
                                       select b).FirstOrDefault();

                //using (TransactionScope ts = new TransactionScope())
                //{
                if (objChk == null)//No duplicate records
                {
                    if (ObjectToCreaetOrUpdate.ID == 0) //new record for insert
                    {
                        tblMeterType createOrUpdate = new tblMeterType();

                        Mapper.DynamicMap<MeterTypeModel, tblMeterType>(ObjectToCreaetOrUpdate, createOrUpdate);
                        objCompassEntities.tblMeterTypes.Add(createOrUpdate);
                        objCompassEntities.SaveChanges();

                        //ts.Complete();
                        //change message accroding to your requirement

                        Message = CommonFunctions.strRecordCreated;
                        result = true;
                    }
                    else //update records
                    {
                        tblMeterType createOrUpdate = (from b in objCompassEntities.tblMeterTypes
                                                       where b.Active == 1 && b.ID == ObjectToCreaetOrUpdate.ID
                                                       select b).FirstOrDefault();
                        if (createOrUpdate != null)
                        {
                            Mapper.DynamicMap<MeterTypeModel, tblMeterType>(ObjectToCreaetOrUpdate, createOrUpdate);
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


                tblMeterType ObjMenu = (from b in objCompassEntities.tblMeterTypes
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

        public List<MeterTypeModel> GetDeActiveRecords()
        {
            var list = new List<MeterTypeModel>();
            CompassEntities CompassEntities = new CompassEntities();

            list = CompassEntities.tblMeterTypes.Where(a => a.Active == 0).Select(x => new MeterTypeModel { MeterType = x.MeterType, ID = x.ID }).ToList();
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

                tblMeterType ObjFaq = (from b in objCompassEntities.tblMeterTypes
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
