using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.DAL
{
    public class FormTypeDAL
    {
        #region GetMethod



        public List<FormTypeDOL> GetFormTypeList(bool isActive)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            List<FormTypeDOL> objFormTypeModel = new List<FormTypeDOL>();


            objFormTypeModel = objCompassEntities.tblFormTypes.Where(o => o.Active == (isActive ? 1 : 0)).Select(o => new FormTypeDOL { ID = o.ID, strFormType = o.FormType }).ToList();

            return objFormTypeModel;
        }


        public FormTypeDOL GetFormType(long id)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            FormTypeDOL objFormTypeDOL = new FormTypeDOL();
            objFormTypeDOL = objCompassEntities.tblFormTypes.Where(o => o.Active == 1 && o.ID == id).Select(o =>
                new FormTypeDOL { ID = o.ID, strFormType = o.FormType, FormTypeDescription = o.FormDescription }).FirstOrDefault();

            List<BaseFormTypeModel> objBaseList = objCompassEntities.tblFormTypes.Where(o => o.ID != id && o.Active == 1)
                .Select(o => new BaseFormTypeModel { FormTypeID = o.ID, FormType = o.FormType }).ToList();

            foreach (var item in objBaseList)
            {
                var a = objCompassEntities.tblFormTypeMaps.Where(o => o.FormTypeId == id && o.AffectingFormTypeId == item.FormTypeID && o.Active == 1).FirstOrDefault();
                if (a == null)
                {
                    item.ImpactStatus = false;
                }
                else
                {
                    item.ImpactStatus = a.IsAffect == 1 ? true : false;
                }

            }

            objFormTypeDOL.objBaseFormTypeModelList = objBaseList;


            return objFormTypeDOL;
        }

        public List<BaseFormTypeModel> GetBaseFormType()
        {
            CompassEntities objCompassEntities = new CompassEntities();


            List<BaseFormTypeModel> objBaseList = objCompassEntities.tblFormTypes.Where(o => o.Active == 1)
                .Select(o => new BaseFormTypeModel { FormTypeID = o.ID, FormType = o.FormType, ImpactStatus = false }).ToList();

            ;


            return objBaseList;
        }


        #endregion


        #region Insert,Update,Delete


        public bool CreateOrUpdate(FormTypeDOL ObjectToCreaetOrUpdate, string[] FType, string[] FtypeBool, out string Message)
        {
            bool result = false;
            Message = "Unable to Create / Update Form Type.";
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                //here check the Form type name is already present

                var objChk = (from b in objCompassEntities.tblFormTypes
                              where b.FormType.ToLower().Trim() == ObjectToCreaetOrUpdate.strFormType.ToLower().Trim()
                              && b.ID != ObjectToCreaetOrUpdate.ID
                              select b).FirstOrDefault();

                long id = 0;
                if (objChk == null)
                {
                    if (ObjectToCreaetOrUpdate.ID == 0) //new record for insert
                    {
                        tblFormType createOrUpdate = new tblFormType();
                        createOrUpdate.FormType = ObjectToCreaetOrUpdate.strFormType;
                        createOrUpdate.FormDescription = ObjectToCreaetOrUpdate.FormTypeDescription;
                        createOrUpdate.Active = 1;
                        createOrUpdate.CreatedOn = new CommonFunctions().ServerDate();
                        objCompassEntities.tblFormTypes.Add(createOrUpdate);
                        objCompassEntities.SaveChanges();

                        id = createOrUpdate.ID;
                        Message = CommonFunctions.strRecordCreated;
                        result = true;
                    }
                    else //update records
                    {
                        tblFormType createOrUpdate = (from b in objCompassEntities.tblFormTypes
                                                      where b.Active == 1 && b.ID == ObjectToCreaetOrUpdate.ID
                                                      select b).FirstOrDefault();

                        createOrUpdate.FormType = ObjectToCreaetOrUpdate.strFormType;
                        createOrUpdate.FormDescription = ObjectToCreaetOrUpdate.FormTypeDescription;
                        createOrUpdate.Active = 1;
                        createOrUpdate.CreatedOn = new CommonFunctions().ServerDate();
                        objCompassEntities.SaveChanges();
                        id = createOrUpdate.ID;
                        Message = CommonFunctions.strRecordUpdated;
                        result = true;
                    }

                    if (result)
                    {
                        int i = 0;
                        foreach (var item in FType)
                        {
                            string idval = item.Remove(0, 3);
                            long affectingId = long.Parse(idval);
                            var objtblFormTypeMap = objCompassEntities.tblFormTypeMaps.Where(o => o.FormTypeId == id && o.AffectingFormTypeId == affectingId).FirstOrDefault();

                            if (objtblFormTypeMap == null)
                            {
                                objtblFormTypeMap = new tblFormTypeMap();
                                objtblFormTypeMap.Active = 1;
                                objtblFormTypeMap.AffectingFormTypeId = affectingId;
                                objtblFormTypeMap.FormTypeId = id;
                                objtblFormTypeMap.IsAffect = FtypeBool[i] == "true" ? 1 : 0;
                                objtblFormTypeMap.CreatedOn = new CommonFunctions().ServerDate();
                                objCompassEntities.tblFormTypeMaps.Add(objtblFormTypeMap);
                                objCompassEntities.SaveChanges();
                            }
                            else
                            {
                                objtblFormTypeMap.Active = 1;
                                objtblFormTypeMap.AffectingFormTypeId = affectingId;
                                objtblFormTypeMap.FormTypeId = id;
                                objtblFormTypeMap.IsAffect = FtypeBool[i] == "true" ? 1 : 0;
                                objtblFormTypeMap.ModifiedOn = new CommonFunctions().ServerDate();
                                objCompassEntities.SaveChanges();
                            }

                            i++;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Message += "<br/>" + ex.Message;
            }

            return result;

        }


        /// <summary>
        /// Deactivate the active record
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool Deactivate(long Id)
        {
            try
            {

                CompassEntities objCompassEntities = new CompassEntities();
                tblFormType ObjMenu = objCompassEntities.tblFormTypes.Where(o => o.Active == 1 && o.ID == Id).FirstOrDefault();
                ObjMenu.Active = 0;
                objCompassEntities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion




        public bool ActivateRecord(long Id, out string msg)
        {
            try
            {
                msg = "";


                CompassEntities objCompassEntities = new CompassEntities();

                tblFormType ObjFaq = objCompassEntities.tblFormTypes.Where(o => o.ID == Id).FirstOrDefault();

                if (ObjFaq != null)
                {
                    ObjFaq.Active = 1;
                    //ObjFaq.ModifiedBy = Convert.ToInt64(HttpContext.Current.Session["UserId"].ToString());
                    ObjFaq.ModifiedOn = new CommonFunctions().ServerDate();
                    objCompassEntities.SaveChanges();

                    msg = "Record activated.";
                    return true;

                }
                else
                {
                    msg = "Unable to Activate Record.";
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
