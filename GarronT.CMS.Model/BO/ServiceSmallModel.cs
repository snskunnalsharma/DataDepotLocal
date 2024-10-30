using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class ServiceSmallModel : IDisposable
    {

        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            //dispose unmanaged ressources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public long ServiceId { get; set; }
        public long MasterFieldId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceBasePrice { get; set; }
        public bool isImageMapped { get; set; }
        public Nullable<bool> ImageFlag { get; set; }
        public long[] productArray { get; set; }
    }
    public class ServiceNotSelectedModel : IDisposable
    {

        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            //dispose unmanaged ressources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public long ServiceId { get; set; }
        public long MasterFieldId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceBasePrice { get; set; }
        public bool isImageMapped { get; set; }
        public Nullable<bool> ImageFlag { get; set; }
        public long[] productArray { get; set; }
    }
    public class ServiceModel : IDisposable
    {
        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            //dispose unmanaged ressources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public List<ServiceSmallModel> objList1 { get; set; }
        public List<ServiceNotSelectedModel> objList2 { get; set; }
        public List<ProductServiceBo> objList3 { get; set; }
        public List<MeterSizePayRateBo> objList4 { get; set; }
    }

    public class ProductServiceBo
    {
        public long productId { get; set; }

        public long ProjectServiceId { get; set; }
        public string productName { get; set; }
        public string Make { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string UtilityName { get; set; }
        public bool SerialNumberRequired { get; set; }
        public int Quantity { get; set; }
    }

    // Add new 
    public class MeterSizePayRateBo : IDisposable
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
        public long Id { get; set; }
        public string MeterSize { get; set; }
        public string Pay { get; set; }
        public long[] MeterPayRateArray { get; set; }
    }

    public class Meterpayratedetail
    {
        public long MeterSizeId { get; set; }
        public string MeterSize { get; set; }
        public long MeterTypeId { get; set; }
        public string MeterType { get; set; }
    }

    public class RadioMeterpayratedetail
    {
        public string ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal BasePay { get; set; }
        public long MeterSizeId { get; set; }
        public string MeterSize { get; set; }
        public long MeterTypeId { get; set; }
        public string MeterType { get; set; }
    }

}
