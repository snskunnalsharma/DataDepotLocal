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
    public class ServiceDAL : IDisposable
    {

        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            //if (!disposed)
            //{
            //    if (disposing)
            //    {
            //        //dispose managed ressources

            //    }
            //}
            //dispose unmanaged ressources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //public List<ServicesModel> getServicesList()
        //{
        //    CompassEntities objCompassEntities = new CompassEntities();
        //    List<ServicesModel> objList = objCompassEntities.tblServices.Where(o => o.Active == 1).
        //        Select(o => new ServicesModel { 
        //            ServiceId = o.ID,
        //            ServiceName = o.Service,
        //            ServiceDescription=o.Description,
        //            TakeAudio = o.AudioRequired != null ? bool.Parse(o.AudioRequired.ToString()) : false,
        //            TakeVideo = o.VideoRequired != null ? bool.Parse(o.VideoRequired.ToString()) : false,
        //            MaxAudio = o.MaxAudio != null ? int.Parse(o.MaxAudio.ToString()) : 0,
        //            MaxVideo=o.MaxVideo!=null?int.Parse(o.MaxVideo.ToString()):0                    
        //        }).ToList();

        //    objCompassEntities.Dispose();
        //    return objList;
        //}



        //public void insertServicesDetails()
        //{
        //    CompassEntities objCompassEntities = new CompassEntities();

        //}

        //public void updateServicesDetails()
        //{
        //    CompassEntities objCompassEntities = new CompassEntities();

        //}

        //public void deactivateServicesDetails()
        //{
        //    CompassEntities objCompassEntities = new CompassEntities();

        //}


        //Added Vishal

        #region GetMethods

        public ServicesModel GetServiceById(long Id)
        {

            CompassEntities objCompassEntities = new CompassEntities();
            ServicesModel objServicesModel = (from b in objCompassEntities.tblServices
                                          where b.ID == Id
                                          select new ServicesModel
                                          {
                                              ID=b.ID,
                                              Service=b.Service,
                                              Description = b.Description,
                                              AudioRequired=b.AudioRequired,
                                              VideoRequired=b.VideoRequired,
                                              ServiceBasePrice=b.ServiceBasePrice,
                                              MaxAudio=b.MaxAudio,
                                              MaxVideo=b.MaxVideo,                                      
                                              CreatedBy = b.CreatedBy,
                                              CreatedOn = b.CreatedOn,
                                              ModifiedBy = b.ModifiedBy,
                                              ModifiedOn = b.ModifiedOn,
                                              Active = b.Active

                                          }).FirstOrDefault();
            return objServicesModel;

        }
        public ServicesModel GetAllServiceID()
        {
            CompassEntities objCompassEntities = new CompassEntities();
            ServicesModel objServicesModel = (from b in objCompassEntities.tblServices

                                          select new ServicesModel
                                          {
                                              ID = b.ID,
                                              Service = b.Service,
                                              Description = b.Description,
                                              AudioRequired = b.AudioRequired,
                                              VideoRequired = b.VideoRequired,
                                              ServiceBasePrice = b.ServiceBasePrice,
                                              MaxAudio = b.MaxAudio,
                                              MaxVideo = b.MaxVideo,
                                              CreatedBy = b.CreatedBy,
                                              CreatedOn = b.CreatedOn,
                                              ModifiedBy = b.ModifiedBy,
                                              ModifiedOn = b.ModifiedOn,
                                              Active = b.Active

                                          }).FirstOrDefault();
            return objServicesModel;
        }

        public ServicesModel GetServices()
        {
            CompassEntities objCompassEntities = new CompassEntities();
            ServicesModel objServicesModel = (from b in objCompassEntities.tblServices
                                 
                                          select new ServicesModel
                                          {
                                              ID = b.ID,
                                              Service = b.Service,
                                              Description = b.Description,
                                              AudioRequired = b.AudioRequired,
                                              VideoRequired = b.VideoRequired,
                                              ServiceBasePrice = b.ServiceBasePrice,
                                              MaxAudio = b.MaxAudio,
                                              MaxVideo = b.MaxVideo,
                                              CreatedBy = b.CreatedBy,
                                              CreatedOn = b.CreatedOn,
                                              ModifiedBy = b.ModifiedBy,
                                              ModifiedOn = b.ModifiedOn,
                                              Active = b.Active

                                          }).FirstOrDefault();
            return objServicesModel;
        }

        public List<ServicesModel> GetActiveServiceRecords()
        {
            var list = new List<ServicesModel>();
            CompassEntities CompassEntities = new CompassEntities();
            //  var tblStateList = CompassEntities.tblStateMasters.Include("tblCountryMaster").Where(a => a.Active == 1 && a.CountryID == Id).ToList();
            list = (from b in CompassEntities.tblServices
                    where b.Active == 1
                    select new ServicesModel
                    {
                        ID = b.ID,
                        Service = b.Service,
                        Description = b.Description,
                        AudioRequired = b.AudioRequired,
                        VideoRequired = b.VideoRequired,
                        ServiceBasePrice = b.ServiceBasePrice,
                        MaxAudio = b.MaxAudio,
                        MaxVideo = b.MaxVideo,
                        CreatedBy = b.CreatedBy,
                        CreatedOn = b.CreatedOn,
                        ModifiedBy = b.ModifiedBy,
                        ModifiedOn = b.ModifiedOn,
                        Active = b.Active
                    }).ToList();
            return list;
        }

        public List<ServicesModel> GetDeActiveRecords()
        {
            var list = new List<ServicesModel>();
            CompassEntities CompassEntities = new CompassEntities();
            //  var tblStateList = CompassEntities.tblStateMasters.Include("tblCountryMaster").Where(a => a.Active == 1 && a.CountryID == Id).ToList();

            list = (from b in CompassEntities.tblServices
                    where b.Active == 0
                    select new ServicesModel
                    {
                        ID = b.ID,
                        Service = b.Service,
                        Description = b.Description,
                        AudioRequired = b.AudioRequired,
                        VideoRequired = b.VideoRequired,
                        ServiceBasePrice = b.ServiceBasePrice,
                        MaxAudio = b.MaxAudio,
                        MaxVideo = b.MaxVideo,
                        CreatedBy = b.CreatedBy,
                        CreatedOn = b.CreatedOn,
                        ModifiedBy = b.ModifiedBy,
                        ModifiedOn = b.ModifiedOn,
                        Active = b.Active
                    }).ToList();
            return list;
        }


        public bool ActivateRecord(int Id, out string msg)
        {
            try
            {
                msg = "";
                CompassEntities CompassEntities = new CompassEntities();

                tblService ObjFaq = (from b in CompassEntities.tblServices
                                    
                                    where b.ID == Id 

                                    select b).FirstOrDefault();


                if (ObjFaq != null)
                {
                    ObjFaq.Active = 1;
                    //ObjFaq.ModifiedBy = Convert.ToInt64(HttpContext.Current.Session["UserId"].ToString());
                    ObjFaq.ModifiedOn = new CommonFunctions().ServerDate();
                    CompassEntities.SaveChanges();

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
        #endregion


        #region Insert,Update & Delete

        public bool CreateOrUpdate(ServicesModel ObjectToCreaetOrUpdate, out string Message, out long ID)
        {
            bool result = false;
            Message = "Unable to Create / Update State.";
            ID = 0;
            try
            {
                CompassEntities CompassEntities = new CompassEntities();

                tblService objChk = (from b in CompassEntities.tblServices
                                     where b.Service.ToLower().Trim() == ObjectToCreaetOrUpdate.Service.ToLower().Trim()
                                    && b.ID != ObjectToCreaetOrUpdate.ID
                                    select b).FirstOrDefault();

                if (objChk == null)//No duplicate records
                {
                    if (ObjectToCreaetOrUpdate.ID == 0) //new record for insert
                    {
                        tblService createOrUpdate = new tblService();

                        createOrUpdate.Service=ObjectToCreaetOrUpdate.Service;
                        createOrUpdate.Description = ObjectToCreaetOrUpdate.Description;
                        createOrUpdate.MaxAudio = ObjectToCreaetOrUpdate.MaxAudio;
                        createOrUpdate.MaxVideo=ObjectToCreaetOrUpdate.MaxVideo;
                        createOrUpdate.AudioRequired = ObjectToCreaetOrUpdate.AudioRequired;
                        createOrUpdate.VideoRequired = ObjectToCreaetOrUpdate.VideoRequired;
                        createOrUpdate.ServiceBasePrice = ObjectToCreaetOrUpdate.ServiceBasePrice;
                        createOrUpdate.CreatedBy = ObjectToCreaetOrUpdate.CreatedBy;
                        createOrUpdate.CreatedOn = ObjectToCreaetOrUpdate.CreatedOn;
                        createOrUpdate.Active = ObjectToCreaetOrUpdate.Active;



                     //  Mapper.DynamicMap<ServicesModel, tblService>(ObjectToCreaetOrUpdate, createOrUpdate);
                        CompassEntities.tblServices.Add(createOrUpdate);
                        CompassEntities.SaveChanges();


                        //ts.Complete();
                        //change message accroding to your requirement
                        ID = createOrUpdate.ID;
                        Message = CommonFunctions.strRecordCreated;

                        result = true;
                    }
                    else //update records
                    {
                        tblService createOrUpdate = (from b in CompassEntities.tblServices
                                                    where b.Active == 1 && b.ID == ObjectToCreaetOrUpdate.ID
                                                    select b).FirstOrDefault();
                        if (createOrUpdate != null)
                        {
                            Mapper.DynamicMap<ServicesModel, tblService>(ObjectToCreaetOrUpdate, createOrUpdate);
                        }

                        // Commenet 18-01-2016 check Client use another table
                        //if (ObjectToCreaetOrUpdate.Active == 0)
                        //{
                        //    tblServiceContactDetail tblARMTerritoryRelation = (from b in CompassEntities.tblServiceContactDetails
                        //                                                       where b.tblService_ID == ObjectToCreaetOrUpdate.ID && b.Active == 1
                        //                                                       select b).FirstOrDefault();
                        //    if (tblARMTerritoryRelation != null)
                        //    {
                        //        Message = CommonFunctions.strRecordDeactivatingExist;
                        //        return false;
                        //    }
                        //}
                        CompassEntities.SaveChanges();
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
                CompassEntities CompassEntities = new CompassEntities();

                tblService ObjMenu = (from b in CompassEntities.tblServices
                                     where b.Active == 1 && b.ID == Id
                                     select b).FirstOrDefault();


                //if (ObjMenu != null)
                //{
                ObjMenu.Active = 0;
                // ObjMenu.ModifiedBy = Convert.ToInt64(HttpContext.Current.Session["UserId"].ToString());
                // ObjMenu.ModifiedOn = new CommonFunctions().ServerDate();

                //List<tblThirdLevelMenuMaster> listThirdmenu = (from a in CompassEntities.tblThirdLevelMenuMasters
                //                                               where a.Active == 1 && a.StateID == ObjMenu.StateID
                //                                               select a).ToList();
                //foreach (var a in listThirdmenu)
                //{
                //    a.Active = 0;
                //}
                CompassEntities.SaveChanges();
                // ts.Complete();
                //}
                //}

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

    }
}
