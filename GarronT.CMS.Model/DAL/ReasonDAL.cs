using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.DAL
{
    public class ReasonDAL
    {

        public List<ReasonBO> GetReasonList(bool _val)
        {
            int id = 0;
            if (_val)
            {
                id = 1;
            }
            else
                id = 0;
            CompassEntities objCompassEntities = new CompassEntities();

            List<ReasonBO> objReasonBOList = objCompassEntities.tblSkipReasonMasters.Where(o => o.Active == id).
                Select(o => new ReasonBO { SkipId = o.SkipId, CategoryType = o.CategoryType.ToUpper(), SkipReason = o.SkipReason }).ToList();

            return objReasonBOList;
        }



        /// <summary>
        /// Inserts the records
        /// </summary>
        /// <param name="objReasonBO"></param>
        /// <param name="_result"></param>
        /// <returns></returns>
        public bool InsertRecord(ReasonBO objReasonBO, out string _result)
        {
            CompassEntities objCompassEntities = new CompassEntities();


            var a = objCompassEntities.tblSkipReasonMasters.Where(o => o.SkipReason.ToUpper() == objReasonBO.SkipReason.ToUpper()
                && o.CategoryType.ToUpper() == objReasonBO.CategoryType.ToUpper()).FirstOrDefault();

            if (a == null)
            {

                tblSkipReasonMaster obj1 = new tblSkipReasonMaster();
                obj1.Active = 1;
                obj1.CategoryType = objReasonBO.CategoryType;
                obj1.CreatedBy = int.Parse(objReasonBO.CreatedBy.ToString());
                obj1.CreatedOn = new CommonFunctions().ServerDate();
                obj1.SkipReason = objReasonBO.SkipReason;
                //obj1.IsMediaRequired = objReasonBO.IsMediaRequired;
                obj1.IsAudioRequired = objReasonBO.IsAudioRequired;
                obj1.IsVideoRequired = objReasonBO.IsVideoRequired;
                obj1.IsImageRequired = objReasonBO.IsImageRequired;
                objCompassEntities.tblSkipReasonMasters.Add(obj1);
                objCompassEntities.SaveChanges();

                foreach (var item in objReasonBO.FormTypeList)
                {
                    tblSkipReasonMaster_FormType obj = new tblSkipReasonMaster_FormType();
                    obj.CreatedBy = int.Parse(objReasonBO.CreatedBy.ToString());
                    obj.CreatedOn = new CommonFunctions().ServerDate();
                    obj.Active = 1;
                    obj.FKFormTypeId = item;
                    obj.FKReasonId = obj1.SkipId;
                    objCompassEntities.tblSkipReasonMaster_FormType.Add(obj);
                    objCompassEntities.SaveChanges();
                }

                _result = "Success";
                return true;
            }
            else
            {
                _result = "Reason already in use";
                return false;
            }

        }



        /// <summary>
        /// Updates the records
        /// </summary>
        /// <param name="objReasonBO"></param>
        /// <param name="_result"></param>
        /// <returns></returns>
        public bool UpdateRecord(ReasonBO objReasonBO, out string _result)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            var a = objCompassEntities.tblSkipReasonMasters.Where(o => o.SkipReason.ToUpper() == objReasonBO.SkipReason.ToUpper()
                && o.CategoryType.ToUpper() == objReasonBO.CategoryType.ToUpper() && o.SkipId != objReasonBO.SkipId).FirstOrDefault();

            if (a == null)
            {
                tblSkipReasonMaster obj1 = objCompassEntities.tblSkipReasonMasters.Where(o => o.SkipId == objReasonBO.SkipId).FirstOrDefault();
                obj1.Active = 1;
                obj1.CategoryType = objReasonBO.CategoryType;
                obj1.CreatedBy = int.Parse(objReasonBO.CreatedBy.ToString());
                obj1.CreatedOn = new CommonFunctions().ServerDate();
                obj1.SkipReason = objReasonBO.SkipReason;
                // obj1.IsMediaRequired = objReasonBO.IsMediaRequired;
                obj1.IsAudioRequired = objReasonBO.IsAudioRequired;
                obj1.IsVideoRequired = objReasonBO.IsVideoRequired;
                obj1.IsImageRequired = objReasonBO.IsImageRequired;
                objCompassEntities.SaveChanges();



                var oldData = objCompassEntities.tblSkipReasonMaster_FormType.Where(o => o.Active == 1 && o.FKReasonId == obj1.SkipId).ToList();

                foreach (var item in oldData)
                {
                    item.Active = 0;
                    item.ModifiedBy = int.Parse(objReasonBO.CreatedBy.ToString());
                    item.ModifiedOn = new CommonFunctions().ServerDate();
                    objCompassEntities.SaveChanges();
                }

                foreach (var item in objReasonBO.FormTypeList)
                {


                    tblSkipReasonMaster_FormType obj = oldData.Where(o => o.FKFormTypeId == item).FirstOrDefault();
                    if (obj == null)
                    {
                        obj = new tblSkipReasonMaster_FormType();
                        obj.CreatedBy = int.Parse(objReasonBO.CreatedBy.ToString());
                        obj.CreatedOn = new CommonFunctions().ServerDate();
                        obj.Active = 1;
                        obj.FKFormTypeId = item;
                        obj.FKReasonId = obj1.SkipId;
                        objCompassEntities.tblSkipReasonMaster_FormType.Add(obj);
                        objCompassEntities.SaveChanges();
                    }
                    else
                    {
                        obj.ModifiedBy = int.Parse(objReasonBO.CreatedBy.ToString());
                        obj.ModifiedOn = new CommonFunctions().ServerDate();
                        obj.Active = 1;
                        objCompassEntities.SaveChanges();
                    }
                }


                _result = "Success";
                return true;
            }
            else
            {
                _result = "Reason already in use";
                return false;
            }
        }



        public bool ActivateRecord(long Id)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            var obj1 = objCompassEntities.tblSkipReasonMasters.Where(o => o.SkipId == Id).FirstOrDefault();
            obj1.Active = 1;
            obj1.ModifiedOn = new CommonFunctions().ServerDate();
            objCompassEntities.SaveChanges();
            return true;

        }



        public ReasonBO GetEditRecord(long Id)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            var obj1 = objCompassEntities.tblSkipReasonMasters.Where(o => o.SkipId == Id).FirstOrDefault();
            obj1.Active = 1;
            obj1.IsMediaRequired = (obj1.IsMediaRequired == null ? false : bool.Parse(obj1.IsMediaRequired.ToString()));
            obj1.ModifiedOn = new CommonFunctions().ServerDate();
            objCompassEntities.SaveChanges();

            var formList = objCompassEntities.tblSkipReasonMaster_FormType.Where(o => o.Active == 1 && o.FKReasonId == obj1.SkipId).Select(o => o.FKFormTypeId).ToList();
            return new ReasonBO
            {
                CategoryType = obj1.CategoryType,
                Comment = obj1.Comment,
                FormTypeList = formList,
                IsAudioRequired = obj1.IsAudioRequired,
                IsImageRequired = obj1.IsImageRequired,
                IsMediaRequired = obj1.IsMediaRequired,
                IsVideoRequired = obj1.IsVideoRequired,
                SkipId = obj1.SkipId,
                SkipReason = obj1.SkipReason
            };

        }

        public bool DeactivateRecord(long Id)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            var obj1 = objCompassEntities.tblSkipReasonMasters.Where(o => o.SkipId == Id).FirstOrDefault();
            obj1.Active = 0;
            obj1.ModifiedOn = new CommonFunctions().ServerDate();
            objCompassEntities.SaveChanges();
            return true;
        }




        public List<BaseFormTypeModel> GetFormTypeRecord()
        {
            CompassEntities objCompassEntities = new CompassEntities();

            var obj1 = objCompassEntities.tblFormTypes.Where(o => o.Active == 1).
                Select(o => new BaseFormTypeModel { FormTypeID = o.ID, FormType = o.FormType }).ToList();

            return obj1;
        }
    }
}
