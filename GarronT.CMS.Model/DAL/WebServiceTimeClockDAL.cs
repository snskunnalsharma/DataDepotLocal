using GarronT.CMS.Model.BO;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.DAL
{
    public class WebServiceTimeClockDAL : IDisposable
    {

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~WebServiceTimeClockDAL() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

        public List<WebServiceTimeClockBO> getTimeClockData()
        {
            List<WebServiceTimeClockBO> timeClockData = new List<WebServiceTimeClockBO>();
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();
                var clockDataList = objCompassEntities.TimeClockMasters.Where(o => o.Active == 1).Select(o =>
                    new WebServiceTimeClockBO
                    {
                        Id = o.Id,
                        Description = o.Description,
                        Active = o.Active,
                        CreatedBy = o.CreatedBy,
                        ModifiedBy = o.ModifiedBy,
                        CreatedOn = o.CreatedOn,
                        ModifiedOn = o.ModifiedOn
                    }
                ).ToList();
                if(clockDataList != null)
                {
                    timeClockData.AddRange(clockDataList);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return timeClockData;
        }

        // Add Code by Deepak date 14 Oct 2020 
        public String InsertOrUpdate(long TimeClockId, long InstallerId, long UploadedDataExcelId, long ProductId, long PK_UTCD)
        {
            String json = string.Empty;
            try
            {                                                               // info UTCD = UserTimeClockDetails
                CompassEntities objCompassEntities = new CompassEntities();
                CommonFunctions _datetime = new CommonFunctions();
                var dbdate = _datetime.ServerDate();
                UserTimeClockDetail objUserTimeClockDetail = new UserTimeClockDetail();
                List<WebServiceIimeClockDetailBO> UTCDId = new List<WebServiceIimeClockDetailBO>();
                if (PK_UTCD == 0)
                {
                    #region Insert
                    objUserTimeClockDetail.FKTimeClockId = TimeClockId;
                    objUserTimeClockDetail.FKInstallerId = InstallerId;
                    objUserTimeClockDetail.FKUploadedDataExcelId = UploadedDataExcelId;
                    objUserTimeClockDetail.FKProductId = ProductId;
                    objUserTimeClockDetail.InTime = dbdate;
                    objUserTimeClockDetail.OutTime = null;
                    objUserTimeClockDetail.Active = 1;          // 1 for Active or 2 for InActive
                    objUserTimeClockDetail.CreatedOn = dbdate;
                    objUserTimeClockDetail.CreatedBy = InstallerId;
                    objUserTimeClockDetail.ModifiedOn = dbdate;
                    objUserTimeClockDetail.ModifiedBy = InstallerId;
                    objCompassEntities.UserTimeClockDetails.Add(objUserTimeClockDetail);
                    objCompassEntities.SaveChanges();

                    long insertedcurrentid = objUserTimeClockDetail.Id; // inserted current id
                    json = "[{\"Id\":\"" + insertedcurrentid + "\"}]";
                    //json = @"{""Id"":""" + insertedcurrentid + "\"}";
                    /* var getcurrentinsertrecordsid = objCompassEntities.UserTimeClockDetails.Where(o => o.Id == insertedcurrentid).Select(o =>
                        new WebServiceIimeClockDetailBO
                        {
                            Id = o.Id
                        }
                        ).ToList();
                     if (getcurrentinsertrecordsid != null)
                     {
                         UTCDId.AddRange(getcurrentinsertrecordsid);
                     }*/
                    #endregion
                }
                else
                {
                    #region Update
                    var exist = objCompassEntities.UserTimeClockDetails.Where(o => o.Id == PK_UTCD).FirstOrDefault();
                    if (exist != null)
                    {
                        exist.OutTime = dbdate;
                        exist.ModifiedOn = dbdate;
                        exist.ModifiedBy = InstallerId;
                        objCompassEntities.SaveChanges();
                        json = "[{\"Id\":\"" + PK_UTCD + "\"}]";
                        //json = @"{""Id"":""" + PK_UTCD + "}";
                    }
                    else
                    {
                        json = "[{\"error\":\"error\"}]";
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return json;
        }
    }
}
