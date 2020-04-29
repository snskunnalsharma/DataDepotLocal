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
    public class ClientDAL
    {

        #region GetMethods

        public ClientModel GetClientById(long Id)
        {

            CompassEntities objCompassEntities = new CompassEntities();
            ClientModel objClientModel = (from b in objCompassEntities.STP_GetClient()
                                          where b.ClientId == Id
                                          select new ClientModel
                                          {
                                              ClientId = b.ClientId,
                                              ClientName = b.ClientName,
                                              Address1 = b.Address1,
                                              Address2 = b.Address2,
                                              Address3 = b.Address3,
                                              City = b.City,
                                              CityName = b.CityName,
                                              State = b.State,
                                              StateName = b.StateName,
                                              Pincode = b.Pincode,
                                              MobileNumber = b.MobileNumber,
                                              PhoneNumber = b.PhoneNumber,
                                              ClientEmailId = b.ClientEmailId,
                                              CreatedBy = b.CreatedBy,
                                              CreatedOn = b.CreatedOn,
                                              ModifiedBy = b.ModifiedBy,
                                              ModifiedOn = b.ModifiedOn,
                                              Active = b.Active

                                          }).FirstOrDefault();
            return objClientModel;

        }
        public ClientModel GetAllGetClientId()
        {
            CompassEntities objCompassEntities = new CompassEntities();
            ClientModel objClientModel = (from b in objCompassEntities.tblClients

                                          select new ClientModel
                                          {
                                              ClientId = b.ClientId,
                                              ClientName = b.ClientName,
                                              Address1 = b.Address1,
                                              Address2 = b.Address2,
                                              Address3 = b.Address3,
                                              City = b.City,
                                              State = b.State,
                                              Pincode = b.Pincode,
                                              MobileNumber = b.MobileNumber,
                                              PhoneNumber = b.PhoneNumber,
                                              ClientEmailId = b.ClientEmailId,
                                              CreatedBy = b.CreatedBy,
                                              CreatedOn = b.CreatedOn,
                                              ModifiedBy = b.ModifiedBy,
                                              ModifiedOn = b.ModifiedOn,
                                              Active = b.Active

                                          }).FirstOrDefault();
            return objClientModel;
        }

        public ClientModel GetClients()
        {
            CompassEntities objCompassEntities = new CompassEntities();
            ClientModel objClientModel = (from b in objCompassEntities.tblClients
                                          join c in objCompassEntities.TblCityMasters on b.City equals c.CityId
                                          join d in objCompassEntities.tblStateMasters on b.State equals d.StateId
                                          select new ClientModel
                                          {
                                              ClientId = b.ClientId,
                                              ClientName = b.ClientName,
                                              Address1 = b.Address1,
                                              Address2 = b.Address2,
                                              Address3 = b.Address3,
                                              City = b.City,
                                              State = b.State,
                                              CityName = c.CityName,
                                              StateName = d.StateName,
                                              Pincode = b.Pincode,
                                              MobileNumber = b.MobileNumber,
                                              PhoneNumber = b.PhoneNumber,
                                              ClientEmailId = b.ClientEmailId,
                                              CreatedBy = b.CreatedBy,
                                              CreatedOn = b.CreatedOn,
                                              ModifiedBy = b.ModifiedBy,
                                              ModifiedOn = b.ModifiedOn,
                                              Active = b.Active

                                          }).FirstOrDefault();
            return objClientModel;
        }

        public List<ClientModel> GetActiveClientRecords()
        {
            var list = new List<ClientModel>();
            CompassEntities CompassEntities = new CompassEntities();
            //  var tblStateList = CompassEntities.tblStateMasters.Include("tblCountryMaster").Where(a => a.Active == 1 && a.CountryID == Id).ToList();
            //list = (from b in CompassEntities.tblClients
            //        join c in CompassEntities.TblCityMasters on b.City equals c.CityId
            //        join d in CompassEntities.tblStateMasters on b.State equals d.StateId
            //        where b.Active == 1
            //        select new ClientModel
            //        {
            //            ClientId = b.ClientId,
            //            ClientName = b.ClientName,
            //            Address1 = b.Address1,
            //            Address2 = b.Address2,
            //            Address3 = b.Address3,
            //            City = b.City,
            //            State = b.State,
            //            CityName = c.CityName,
            //            StateName = d.StateName,
            //            Pincode = b.Pincode,
            //            MobileNumber = b.MobileNumber,
            //            PhoneNumber = b.PhoneNumber
            //        }).ToList();

            list = (from b in CompassEntities.STP_GetClient().Where(x => x.Active == 1)
                    select new ClientModel
                    {
                        ClientId = b.ClientId,
                        ClientName = b.ClientName,
                        Address1 = b.Address1,
                        Address2 = b.Address2,
                        Address3 = b.Address3,
                        //  City = b.City,
                        // State = b.State,
                        CityName = b.CityName,
                        StateName = b.StateName,
                        Pincode = b.Pincode,
                        MobileNumber = b.MobileNumber,
                        PhoneNumber = b.PhoneNumber,
                        ClientEmailId = b.ClientEmailId
                    }).OrderBy(t => t.ClientName).ToList();
            return list;
        }

        public List<ClientModel> GetDeActiveRecords()
        {
            var list = new List<ClientModel>();
            CompassEntities CompassEntities = new CompassEntities();
            //  var tblStateList = CompassEntities.tblStateMasters.Include("tblCountryMaster").Where(a => a.Active == 1 && a.CountryID == Id).ToList();

            list = (from b in CompassEntities.STP_GetClient().Where(x => x.Active == 0)
                    select new ClientModel
                    {
                        ClientId = b.ClientId,
                        ClientName = b.ClientName,
                        Address1 = b.Address1,
                        Address2 = b.Address2,
                        Address3 = b.Address3,
                        //  City = b.City,
                        // State = b.State,
                        CityName = b.CityName,
                        StateName = b.StateName,
                        Pincode = b.Pincode,
                        MobileNumber = b.MobileNumber,
                        PhoneNumber = b.PhoneNumber,
                        ClientEmailId = b.ClientEmailId
                    }).OrderBy(t => t.ClientName).ToList();
            return list;
        }


        public bool ActivateRecord(int Id, out string msg)
        {
            try
            {
                msg = "";
                CompassEntities CompassEntities = new CompassEntities();

                tblClient ObjFaq = (from b in CompassEntities.tblClients
                                    where b.ClientId == Id
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

        public bool CreateOrUpdate(ClientModel ObjectToCreaetOrUpdate, out string Message, out long ClientId)
        {
            bool result = false;
            Message = "Unable to Create / Update State.";
            ClientId = 0;
            try
            {
                CompassEntities CompassEntities = new CompassEntities();

                tblClient objChk = (from b in CompassEntities.tblClients
                                    join c in CompassEntities.TblCityMasters on b.City equals c.CityId
                                    join d in CompassEntities.tblStateMasters on b.State equals d.StateId
                                    where b.ClientName.ToLower().Trim() == ObjectToCreaetOrUpdate.ClientName.ToLower().Trim()
                                    && b.ClientId != ObjectToCreaetOrUpdate.ClientId
                                    select b).FirstOrDefault();

                if (objChk == null)//No duplicate records
                {
                    if (ObjectToCreaetOrUpdate.ClientId == 0) //new record for insert
                    {
                        tblClient createOrUpdate = new tblClient();
                        Mapper.DynamicMap<ClientModel, tblClient>(ObjectToCreaetOrUpdate, createOrUpdate);
                        CompassEntities.tblClients.Add(createOrUpdate);
                        CompassEntities.SaveChanges();


                        //ts.Complete();
                        //change message accroding to your requirement
                        ClientId = createOrUpdate.ClientId;
                        Message = CommonFunctions.strRecordCreated;

                        result = true;
                    }
                    else //update records
                    {
                        tblClient createOrUpdate = (from b in CompassEntities.tblClients
                                                    where b.Active == 1 && b.ClientId == ObjectToCreaetOrUpdate.ClientId
                                                    select b).FirstOrDefault();
                        if (createOrUpdate != null)
                        {
                            Mapper.DynamicMap<ClientModel, tblClient>(ObjectToCreaetOrUpdate, createOrUpdate);
                        }

                        // Commenet 18-01-2016 check Client use another table
                        //if (ObjectToCreaetOrUpdate.Active == 0)
                        //{
                        //    tblClientContactDetail tblARMTerritoryRelation = (from b in CompassEntities.tblClientContactDetails
                        //                                                       where b.tblClient_ClientId == ObjectToCreaetOrUpdate.ClientId && b.Active == 1
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

                tblClient ObjMenu = (from b in CompassEntities.tblClients
                                     where b.Active == 1 && b.ClientId == Id
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<CustomerAuditPercentage> GetCustomerAuditSettings(long customerId, int activeDeactive)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<CustomerAuditPercentage> objList = new List<CustomerAuditPercentage>();


                objList = CompassEntities.CustomerAuditSettings.Where(o => o.FK_CustomerId == customerId && o.Active == activeDeactive).
                    Select(o =>
                        new CustomerAuditPercentage
                        {
                            AuditId = o.AuditId,
                            AuditPercentage = o.AuditPercentage,
                            EndDay = o.EndDay,
                            StartDay = o.StartDay
                        }).ToList();

                return objList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<CustomerFailPercentage> GetCustomerFailSettings(long customerId, int activeDeactive)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<CustomerFailPercentage> objList = new List<CustomerFailPercentage>();

                objList = CompassEntities.CustomerAuditFailureSettings.Where(o => o.FK_CustomerId == customerId && o.Active == activeDeactive).
                 Select(o =>
                     new CustomerFailPercentage
                     {
                         Id = o.Id,
                         FailPercentageStart = o.FailPercentageStart,
                         FailPercentageEnd=o.FailPercentageEnd,
                         IncreaseByDays = o.IncreaseByDays
                     }).ToList();

                return objList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="auditId"></param>
        /// <param name="auditPercent"></param>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <param name="resultMessage"></param>
        /// <returns></returns>
        public bool SaveAuditPercentage(long customerId, long auditId, decimal auditPercent, int startDay, int endDay, out string resultMessage)
        {
            bool valInsert = false;
            resultMessage = "";
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();
                if (auditId == 0)
                {
                    #region insert
                    //var checkPercentageAlreadyPresent = objCompassEntities.CustomerAuditSettings.Where(o => o.FK_CustomerId == customerId && o.AuditPercentage == auditPercent).FirstOrDefault();
                    //if (checkPercentageAlreadyPresent != null)
                    //{
                    //    resultMessage = checkPercentageAlreadyPresent.Active == 0 ? "Save failed. Record already present in deactive record."
                    //        : "Save failed. Record already present in active record.";
                    //}
                    //else
                    //{
                    var checkDayRangeIsUnique = objCompassEntities.CustomerAuditSettings.Where(o => o.FK_CustomerId == customerId
                        && (o.StartDay < startDay && o.EndDay > startDay ||
                        o.StartDay < endDay && o.EndDay > endDay)).FirstOrDefault();

                    if (checkDayRangeIsUnique != null)
                    {
                        resultMessage = checkDayRangeIsUnique.Active == 0 ? "Save failed because of start day or end day conflict with other deactive record."
                            : "Save failed because of start day or end day conflict with other active record.";
                    }
                    else
                    {
                        CustomerAuditSetting obj = new CustomerAuditSetting();
                        obj.Active = 1;
                        obj.AuditPercentage = auditPercent;
                        obj.CreatedOn = new CommonFunctions().ServerDate();
                        obj.CreatedBy = 1;
                        obj.EndDay = endDay;
                        obj.FK_CustomerId = customerId;
                        obj.StartDay = startDay;
                        objCompassEntities.CustomerAuditSettings.Add(obj);
                        objCompassEntities.SaveChanges();

                        resultMessage = "Record successfully saved...!";
                        valInsert = true;
                    }
                    // }
                    #endregion
                }
                else
                {
                    #region update

                    //var checkPercentageAlreadyPresent = objCompassEntities.CustomerAuditSettings.Where(o => o.FK_CustomerId == customerId && o.AuditPercentage == auditPercent && o.AuditId != auditId).FirstOrDefault();
                    //if (checkPercentageAlreadyPresent != null)
                    //{
                    //    resultMessage = checkPercentageAlreadyPresent.Active == 0 ? "Save failed. Record already present in deactive record."
                    //        : "Save failed. Record already present in active record.";
                    //}
                    //else
                    //{
                    var checkDayRangeIsUnique = objCompassEntities.CustomerAuditSettings.Where(o => o.FK_CustomerId == customerId
                        && (o.StartDay <= startDay && o.EndDay >= startDay ||
                        o.StartDay <= endDay && o.EndDay >= endDay) && o.AuditId != auditId).FirstOrDefault();

                    if (checkDayRangeIsUnique != null)
                    {
                        resultMessage = checkDayRangeIsUnique.Active == 0 ? "Save failed because of start day or end day conflict with other deactive record."
                            : "Save failed because of start day or end day conflict with other active record.";
                    }
                    else
                    {
                        CustomerAuditSetting obj = objCompassEntities.CustomerAuditSettings.Where(o => o.AuditId == auditId).FirstOrDefault();
                        obj.AuditPercentage = auditPercent;
                        obj.ModifiedOn = new CommonFunctions().ServerDate();
                        obj.ModifiedBy = 1;
                        obj.EndDay = endDay;
                        obj.StartDay = startDay;
                        objCompassEntities.SaveChanges();

                        resultMessage = "Record successfully saved...!";
                        valInsert = true;
                    }
                    //}
                    #endregion
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return valInsert;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="Id"></param>
        /// <param name="failPercent"></param>
        /// <param name="increasedDay"></param>
        /// <param name="resultMessage"></param>
        /// <returns></returns>
        public bool SaveFailPercentage(CustomerFailPercentage obj, out string resultMessage)
        {
            bool valInsert = false;
            resultMessage = "";
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();
                if (obj.Id == 0)
                {
                    #region insert
                    var checkPercentageAlreadyPresent = objCompassEntities.CustomerAuditFailureSettings.
                        Where(o => o.FK_CustomerId == obj.FK_CustomerId && ((o.FailPercentageStart <= obj.FailPercentageStart
                        && o.FailPercentageEnd > obj.FailPercentageStart) || (o.FailPercentageStart <= obj.FailPercentageEnd
                        && o.FailPercentageEnd >= obj.FailPercentageEnd))).FirstOrDefault();
                    if (checkPercentageAlreadyPresent != null)
                    {
                        resultMessage = checkPercentageAlreadyPresent.Active == 0 ? "Save failed because of start percent or end percent conflict with other deactive record."
                            : "Save failed because of start percent or end percent conflict with other active record.";
                    }
                    else
                    {
                        CustomerAuditFailureSetting obj1 = new CustomerAuditFailureSetting();
                        obj1.Active = 1;
                        obj1.FailPercentageStart = obj.FailPercentageStart;
                        obj1.FailPercentageEnd = obj.FailPercentageEnd;
                        obj1.CreatedOn = new CommonFunctions().ServerDate();
                        obj1.CreatedBy = obj.userId;
                        obj1.IncreaseByDays = obj.IncreaseByDays;
                        obj1.FK_CustomerId = obj.FK_CustomerId;
                        objCompassEntities.CustomerAuditFailureSettings.Add(obj1);
                        objCompassEntities.SaveChanges();

                        resultMessage = "Record successfully saved...!";
                        valInsert = true;

                    }
                    #endregion
                }
                else
                {
                    #region update

                    var checkPercentageAlreadyPresent = objCompassEntities.CustomerAuditFailureSettings.Where(o => o.FK_CustomerId == obj.FK_CustomerId
                       && ((o.FailPercentageStart <= obj.FailPercentageStart
                        && o.FailPercentageEnd > obj.FailPercentageStart) || (o.FailPercentageStart <= obj.FailPercentageEnd
                        && o.FailPercentageEnd >= obj.FailPercentageEnd)) && o.Id != obj.Id).FirstOrDefault();
                    if (checkPercentageAlreadyPresent != null)
                    {
                        resultMessage = checkPercentageAlreadyPresent.Active == 0 ? "Save failed because of start percent or end percent conflict with other deactive record."
                            : "Save failed because of start percent or end percent conflict with other active record.";
                    }
                    else
                    {

                        CustomerAuditFailureSetting obj1 = objCompassEntities.CustomerAuditFailureSettings.Where(o => o.Id == obj.Id).FirstOrDefault();
                        obj1.FailPercentageStart = obj.FailPercentageStart;
                        obj1.FailPercentageEnd = obj.FailPercentageEnd;
                        obj1.IncreaseByDays = obj.IncreaseByDays;
                        obj1.ModifiedOn = new CommonFunctions().ServerDate();
                        obj1.ModifiedBy = obj.userId;
                        objCompassEntities.SaveChanges();

                        resultMessage = "Record successfully saved...!";
                        valInsert = true;

                    }
                    #endregion
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return valInsert;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditId"></param>
        /// <returns></returns>
        public CustomerAuditPercentage GetAuditPercentageRecord(long auditId)
        {

            CustomerAuditPercentage obj = new CustomerAuditPercentage();
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();
                obj = objCompassEntities.CustomerAuditSettings.Where(o => o.AuditId == auditId).
                    Select(o => new CustomerAuditPercentage { AuditId = o.AuditId, AuditPercentage = o.AuditPercentage, EndDay = o.EndDay, StartDay = o.StartDay }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="failId"></param>
        /// <returns></returns>
        public CustomerFailPercentage GetFailPercentageRecord(long failId)
        {

            CustomerFailPercentage obj = new CustomerFailPercentage();
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();
                obj = objCompassEntities.CustomerAuditFailureSettings.Where(o => o.Id == failId).
                    Select(o => new CustomerFailPercentage { Id = o.Id, FailPercentageStart = o.FailPercentageStart, FailPercentageEnd=o.FailPercentageEnd, IncreaseByDays = o.IncreaseByDays }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="isAuditId"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public bool ActivateDeactivateRecord(long Id, bool isAuditId, bool isActive, out string resultMessage)
        {
            bool result = false;
            resultMessage = "";
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                if (isAuditId)
                {
                    var obj = objCompassEntities.CustomerAuditSettings.Where(o => o.AuditId == Id).FirstOrDefault();
                    obj.Active = isActive == true ? 1 : 0;
                    objCompassEntities.SaveChanges();
                    resultMessage = isActive == true ? "Record activated successfully" : "Record deactivated successfully";
                    result = true;
                }
                else
                {
                    var obj = objCompassEntities.CustomerAuditFailureSettings.Where(o => o.Id == Id).FirstOrDefault();
                    obj.Active = isActive == true ? 1 : 0;
                    objCompassEntities.SaveChanges();
                    resultMessage = isActive == true ? "Record activated successfully" : "Record deactivated successfully";
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }




    }
}
