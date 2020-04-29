using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.DAL
{
    public class CategoryDAL : IDisposable
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



        public List<CategoryBO> GetAllRecord(bool Status)
        {

            CompassEntities objCompassEntities = new CompassEntities();

            var objCategoryBOList = objCompassEntities.Categories.Where(o =>
                       o.Active == (Status ? 1 : 0)).Select(o =>
                           new CategoryBO
                           {
                               CategoryId = o.CategoryId,
                               CategoryName = o.CategoryName
                           }).ToList();

            return objCategoryBOList;
        }



        public CategoryBO GetRecord(long CategoryId)
        {

            CompassEntities objCompassEntities = new CompassEntities();

            var objCategory = objCompassEntities.Categories.Where(o =>
                        o.CategoryId == CategoryId).Select(o => new CategoryBO
                        {
                            CategoryId = o.CategoryId,
                            CategoryName = o.CategoryName
                        }).FirstOrDefault();

            return objCategory;
        }



        public StatusMessage CreateOrUpdate(CategoryBO ObjectToCreateOrUpdate)
        {
            StatusMessage objStatusMessage = new StatusMessage();
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                if (ObjectToCreateOrUpdate.CategoryId == 0)
                {
                    #region Insert

                    var CheckDuplicate = objCompassEntities.Categories.Where(o => o.CategoryName == ObjectToCreateOrUpdate.CategoryName).FirstOrDefault();
                    if (CheckDuplicate != null)
                    {
                        objStatusMessage.Success = false;
                        objStatusMessage.Message = "Category creation failed. Category name already in use.";
                    }
                    else
                    {
                        Category obj = new Category();
                        obj.CategoryName = ObjectToCreateOrUpdate.CategoryName;

                        obj.Active = 1;
                        obj.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                        obj.CreatedOn = DateTime.Now;
                        objCompassEntities.Categories.Add(obj);
                        objCompassEntities.SaveChanges();
                        objStatusMessage.Success = true;
                        objStatusMessage.Message = "Record created successfully.";
                    }
                    #endregion

                }
                else
                {
                    #region Update


                    var CheckDuplicate = objCompassEntities.Categories.Where(o => o.CategoryName == ObjectToCreateOrUpdate.CategoryName &&
                         o.CategoryId != ObjectToCreateOrUpdate.CategoryId).FirstOrDefault();

                    if (CheckDuplicate != null)
                    {
                        objStatusMessage.Success = false;
                        objStatusMessage.Message = "Update failed. Category name already in use.";
                    }
                    else
                    {
                        CheckDuplicate = objCompassEntities.Categories.Where(o => o.CategoryId == ObjectToCreateOrUpdate.CategoryId).FirstOrDefault();
                        CheckDuplicate.CategoryName = ObjectToCreateOrUpdate.CategoryName;

                        CheckDuplicate.ModifiedBy = ObjectToCreateOrUpdate.CurrentUserId;
                        CheckDuplicate.ModifiedOn = DateTime.Now;

                        objCompassEntities.SaveChanges();

                        objStatusMessage.Success = true;
                        objStatusMessage.Message = "Record updated successfully.";
                    }

                    #endregion
                }


            }
            catch (Exception)
            {

                throw;
            }

            return objStatusMessage;

        }

        public StatusMessage ActivateOrDeactivate(long Id, bool Status, long CurrentUserId)
        {
            StatusMessage objStatusMessage = new StatusMessage();
            try
            {

                CompassEntities objCompassEntities = new CompassEntities();
                var Obj = objCompassEntities.Categories.Where(o => o.CategoryId == Id).FirstOrDefault();

                Obj.Active = Status == true ? 1 : 0;
                Obj.ModifiedBy = CurrentUserId;
                Obj.ModifiedOn = DateTime.Now;
                objCompassEntities.SaveChanges();
                objStatusMessage.Success = true;
                objStatusMessage.Message = Status == true ? "Record activated successfully." : "Record deactivated successfully.";

            }
            catch (Exception)
            {
                throw;
            }

            return objStatusMessage;
        }

    }
}
