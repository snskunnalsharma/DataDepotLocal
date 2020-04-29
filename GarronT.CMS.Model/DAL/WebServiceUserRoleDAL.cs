using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.DAL
{
    public class WebServiceUserRoleDAL : IDisposable
    {

        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="RoleName"></param>
        /// <param name="CurrentDate"></param>
        /// <returns></returns>
        public List<InventorySourceChildBO> GetAllUsersBasedOnRole(long UserId, string RoleName, DateTime CurrentDate)
        {
            List<InventorySourceChildBO> objInventorySourceChildBOList = new List<InventorySourceChildBO>();
            CompassEntities objCompassEntities = new CompassEntities();


            var manager = new ApplicationDbContext.UserManager();

            var UseNames = manager.GetUsersInRole(RoleName);

            if (UseNames != null)
            {
                foreach (var item in UseNames)
                {
                    var record = objCompassEntities.tblUsers.Where(o => o.Active == 1 && o.aspnet_UserId.ToString() == item.Id.ToString() && o.UserID != UserId).
                        Select(o => new InventorySourceChildBO { ChildId = o.UserID, ChildName = o.FirstName }).FirstOrDefault();
                    if (record != null)
                    {
                        objInventorySourceChildBOList.Add(record);
                    }
                }
                objInventorySourceChildBOList = objInventorySourceChildBOList.OrderBy(o => o.ChildName).ToList();
            }
            return objInventorySourceChildBOList;
        }


        public List<InventorySourceChildBO> GetAllUsersBasedOnRole(long UserId, string RoleName)
        {
            List<InventorySourceChildBO> objInventorySourceChildBOList = new List<InventorySourceChildBO>();
            CompassEntities objCompassEntities = new CompassEntities();


            var manager = new ApplicationDbContext.UserManager();

            var UseNames = manager.GetUsersInRole(RoleName);

            if (UseNames != null)
            {
                foreach (var item in UseNames)
                {
                    var record = objCompassEntities.tblUsers.Where(o => o.Active == 1 && o.aspnet_UserId.ToString() == item.Id.ToString() && o.UserID != UserId).
                        Select(o => new InventorySourceChildBO { ChildId = o.UserID, ChildName = o.FirstName }).FirstOrDefault();
                    if (record != null)
                    {
                        objInventorySourceChildBOList.Add(record);
                    }
                }
                objInventorySourceChildBOList = objInventorySourceChildBOList.OrderBy(o => o.ChildName).ToList();
            }
            return objInventorySourceChildBOList;
        }

        public List<InventorySourceChildBO> GetAllUsersBasedOnRoleWithSelf(string RoleName)
        {
            List<InventorySourceChildBO> objInventorySourceChildBOList = new List<InventorySourceChildBO>();
            CompassEntities objCompassEntities = new CompassEntities();


            var manager = new ApplicationDbContext.UserManager();

            var UseNames = manager.GetUsersInRole(RoleName);

            if (UseNames != null)
            {
                foreach (var item in UseNames)
                {
                    var record = objCompassEntities.tblUsers.Where(o => o.Active == 1 && o.aspnet_UserId.ToString() == item.Id.ToString()).
                        Select(o => new InventorySourceChildBO { ChildId = o.UserID, ChildName = o.FirstName }).FirstOrDefault();
                    if (record != null)
                    {
                        objInventorySourceChildBOList.Add(record);
                    }
                }
                objInventorySourceChildBOList = objInventorySourceChildBOList.OrderBy(o => o.ChildName).ToList();
            }
            return objInventorySourceChildBOList;
        }


    }
}
