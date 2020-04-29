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
    public class MeterSizeDAL
    {
        #region GetMethod
        public MeterSizeModel GetByMeterSizeId(long Id)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            MeterSizeModel objMeterSizeModel = new MeterSizeModel();
            tblMeterSize objtblMeterSize = (from b in objCompassEntities.tblMeterSizes
                                            where b.Active == 1 && b.ID == Id
                                            select b).FirstOrDefault();
            Mapper.DynamicMap<tblMeterSize, MeterSizeModel>(objtblMeterSize, objMeterSizeModel);
            return objMeterSizeModel;
        }

        public MeterSizeModel GetByAllMeterSizeID(long Id)
        {
            CompassEntities objCompassEntities= new CompassEntities();
            MeterSizeModel objMeterSizeModel = new MeterSizeModel();
            tblMeterSize objtblMeterSize = (from b in objCompassEntities.tblMeterSizes
                                            where b.ID == Id
                                            select b).FirstOrDefault();
            Mapper.DynamicMap<tblMeterSize, MeterSizeModel>(objtblMeterSize, objMeterSizeModel);
            return objMeterSizeModel;
        }

        public List<MeterSizeModel> GetMeterSize()
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var objMeterSizeModel = new List<MeterSizeModel>();

            foreach (tblMeterSize obj in objCompassEntities.tblMeterSizes.Where(x=>x.Active==1).OrderBy(t=>t.MeterSize))
	        {
		       MeterSizeModel myobj=new MeterSizeModel();
                Mapper.DynamicMap<tblMeterSize,MeterSizeModel>(obj,myobj);
                objMeterSizeModel.Add(myobj);
	        }

            return objMeterSizeModel;
        }

        public List<MeterSizeModel> GetAllMeterSize()
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var objMeterSizeModel = new List<MeterSizeModel>();

            foreach (tblMeterSize obj  in objCompassEntities.tblMeterSizes.OrderBy(t=>t.MeterSize))
            {
                MeterSizeModel myobj = new MeterSizeModel();
                Mapper.DynamicMap<tblMeterSize,MeterSizeModel>(obj,myobj);
                objMeterSizeModel.Add(myobj);
            }
            return objMeterSizeModel;
        }
        #endregion


        #region Insert,Update,Delete
        public bool CreateOrUpdate(MeterSizeModel ObjectToCreaetOrUpdate, out string Message)
        {
            bool result = false;
            Message = "Unable to Create / Update Meter Make.";
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                //here check the duplicate Menu name
                // According to user requirement changes condition here for b.Menu.ToLower() == ObjectToCreaetOrUpdate.Menu.ToLower()
                tblMeterSize objChk = (from b in objCompassEntities.tblMeterSizes
                                       where b.MeterSize.ToLower().Trim() == ObjectToCreaetOrUpdate.MeterSize.ToLower().Trim()
                                       && b.ID != ObjectToCreaetOrUpdate.ID
                                       select b).FirstOrDefault();

                //using (TransactionScope ts = new TransactionScope())
                //{
                if (objChk == null)//No duplicate records
                {
                    if (ObjectToCreaetOrUpdate.ID == 0) //new record for insert
                    {
                        tblMeterSize createOrUpdate = new tblMeterSize();

                        Mapper.DynamicMap<MeterSizeModel, tblMeterSize>(ObjectToCreaetOrUpdate, createOrUpdate);
                        objCompassEntities.tblMeterSizes.Add(createOrUpdate);
                        objCompassEntities.SaveChanges();

                        //ts.Complete();
                        //change message accroding to your requirement

                        Message = CommonFunctions.strRecordCreated;
                        result = true;
                    }
                    else //update records
                    {
                        tblMeterSize createOrUpdate = (from b in objCompassEntities.tblMeterSizes
                                                       where b.Active == 1 && b.ID == ObjectToCreaetOrUpdate.ID
                                                       select b).FirstOrDefault();
                        if (createOrUpdate != null)
                        {
                            Mapper.DynamicMap<MeterSizeModel, tblMeterSize>(ObjectToCreaetOrUpdate, createOrUpdate);
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


                tblMeterSize ObjMenu = (from b in objCompassEntities.tblMeterSizes
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

        public List<MeterSizeModel> GetDeActiveRecords()
        {
            var list = new List<MeterSizeModel>();
            CompassEntities CompassEntities = new CompassEntities();
            list = CompassEntities.tblMeterSizes.Where(a => a.Active == 0).Select(x => new MeterSizeModel { MeterSize = x.MeterSize, ID = x.ID }).ToList();
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

                tblMeterSize ObjFaq = (from b in objCompassEntities.tblMeterSizes
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
