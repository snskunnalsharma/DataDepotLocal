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
    public class ClientContactDetailsDAL
    {


        #region GetMenthods

        public List<ClientContactDetailsModel> GetClientContactDeatils(int id)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var list = new List<ClientContactDetailsModel>();
            int i = 1;
            List<ClientContactDetailsModel> objSubMenuesBo = (from b in CompassEntities.tblClientContactDetails
                                                              where b.Active == 1 && b.tblClient_ClientId == id
                                                              select new ClientContactDetailsModel
                                                              {

                                                                  ID = b.ID,
                                                                  FirstName = b.FirstName,
                                                                  LastName = b.LastName,
                                                                  MobileNumber = b.MobileNumber,
                                                                  EmailId = b.EmailId,
                                                                  Active = b.Active
                                                                  // S_VIP = b.VIP == 1 ? "Yes" : "No",


                                                              }).ToList();

            foreach (var obj in objSubMenuesBo)
            {

                ClientContactDetailsModel myobj = new ClientContactDetailsModel();
                myobj.TempID = i;
                myobj.FirstName = obj.FirstName;
                myobj.LastName = obj.LastName;
                myobj.MobileNumber = obj.MobileNumber;
                myobj.EmailId = obj.EmailId;
                i = i + 1;
                list.Add(myobj);
            }
            return list;
        }


        #endregion


        #region Insert,Update & Delete

        public bool CreateOrUpdate(ClientContactDetailsModel ObjectToCreaetOrUpdate, out string Message)
        {
            bool result = false;
            Message = "Unable to Create / Update QuestionAttr Action Relation.";
            try
            {
                CompassEntities objTollFreeWidgetEntities = new CompassEntities();

                //here check the duplicate Menu ActionID
                // According to user requirement changes condition here for b.Menu.ToLower() == ObjectToCreaetOrUpdate.Menu.ToLower()
                tblClientContactDetail objChk = (from b in objTollFreeWidgetEntities.tblClientContactDetails
                                                 where b.tblClient_ClientId == ObjectToCreaetOrUpdate.tblClient_ClientId &&
                                              b.MobileNumber == ObjectToCreaetOrUpdate.MobileNumber && b.FirstName == ObjectToCreaetOrUpdate.FirstName && b.Active == 1
                                                 select b).FirstOrDefault();

                //using (TransactionScope ts = new TransactionScope())
                //{
                //if (objChk != null)//No duplicate records
                //{
                if (ObjectToCreaetOrUpdate.ID == 0) //new record for insert
                {
                    tblClientContactDetail createOrUpdate = new tblClientContactDetail();
                    Mapper.DynamicMap<ClientContactDetailsModel, tblClientContactDetail>(ObjectToCreaetOrUpdate, createOrUpdate);
                    createOrUpdate.Active = 1;
                    objTollFreeWidgetEntities.tblClientContactDetails.Add(createOrUpdate);
                    objTollFreeWidgetEntities.SaveChanges();

                    //ts.Complete();
                    //change message accroding to your requirement

                    Message = CommonFunctions.strRecordCreated;
                    result = true;
                }
                else if (ObjectToCreaetOrUpdate.ID != 0) //update records
                {
                    tblClientContactDetail createOrUpdate = (from b in objTollFreeWidgetEntities.tblClientContactDetails
                                                             where b.Active == 1 && b.ID == ObjectToCreaetOrUpdate.ID
                                                             select b).FirstOrDefault();
                    if (createOrUpdate != null)
                    {
                        Mapper.DynamicMap<ClientContactDetailsModel, tblClientContactDetail>(ObjectToCreaetOrUpdate, createOrUpdate);
                        createOrUpdate.Active = 1;
                    }
                    objTollFreeWidgetEntities.SaveChanges();
                    // ts.Complete();
                    //change message accroding to your requirement
                    Message = CommonFunctions.strRecordUpdated;
                    result = true;
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
                // }

            }

            catch (Exception ex)
            {

                Message += "<br/>" + ex.Message;
            }

            return result;

        }

        #endregion
    }
}
