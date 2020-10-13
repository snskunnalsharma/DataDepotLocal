using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
