using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class InwardInventoryBo : IDisposable
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


        public List<BarcodeData> BarcodeDataList { get; set; }

        public double DeviceDateTimeOfRequest { get; set; }

        public int InventorySourceId { get; set; }

        public int InventorySubSourceId { get; set; }

        public bool IsRecordProcessed { get; set; }

        public List<QRcodeData> QRcodeDataList { get; set; }

        public string UserId { get; set; }



    }



    public class BarcodeData
    {
        public string BarcodeSerialNumber { get; set; }
        public string InventoryStatus { get; set; }

    }

    public class QRcodeData
    {
        public bool GenerateBarcode { get; set; }
        public string InventoryName { get; set; }
        public string InventoryStatus { get; set; }
        public int InventoryQty { get; set; }
        public string WebUrlOfBarcodeFile { get; set; }
    }




    public class RequestNewInventoryBo : IDisposable
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


        public double DeviceRequestDate { get; set; }

        public int InventorySourceId { get; set; }

        public int InventorySubSourceId { get; set; }

        public long UtilityId { get; set; }

        public List<QRcodeData> InventoryRequestNameBOList { get; set; }

        public string UserId { get; set; }



    }









    public class ViewInventoryRequestBo : IDisposable
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

        public long RequestId { get; set; }

        public string SubmittedDateTime { get; set; }

        public string UserName { get; set; }

        public string ApprovedUserName { get; set; }

        public string ApprovedDateTime { get; set; }

        public List<QRcodeDataItems> QRcodeDataList { get; set; }

    }


    public class QRcodeDataItems
    {
        public string InventoryName { get; set; }
        public int InventoryQty { get; set; }
    }









    public class MobileWarehouseReceiveBo : IDisposable
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


        public List<MobileSerialNumberData> SerialNumberList { get; set; }

        public double DeviceDateTimeOfRequest { get; set; }

        public long WarehouseId { get; set; }

        //public bool IsRecordProcessed { get; set; }

        public List<MobileProductQuantity> ProductQuantityList { get; set; }
        public long ProjectId { get; set; }
        public long UserId { get; set; }
        public string GPSLocation { get; set; }
        public string DeviceIp { get; set; }
        
    }


    public class MobileSerialNumberData
    {
        // public long RecordId { get; set; }
        public long ProductId { get; set; }
        public long UtilityId { get; set; }
        public string SerialNumber { get; set; }
        public string ServerReponseStatus { get; set; }
        public long ParentId { get; set; }
    }

    public class MobileProductQuantity
    {
        //public long RecordId { get; set; }

        public long UtilityId { get; set; }

        public bool GenerateBarcode { get; set; }

        public long ProductId { get; set; }

        public string ServerReponseStatus { get; set; }

        public int ProductQuantity { get; set; }

        public string WebUrlOfBarcodeFile { get; set; }

        public long ParentId { get; set; }


    }






    public class MobileWarehouseResponseBo : IDisposable
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


        public double DeviceDateTimeOfRequest { get; set; }

        public List<MobileWarehouseResponseProductData> objData { get; set; }

        public long WarehouseId { get; set; }

        public long UserId { get; set; }



    }


    public class MobileWarehouseResponseProductData
    {
        //public long RecordId { get; set; }
        //public long ProductId { get; set; }
        //public long UtilityId { get; set; }
        public string ProductName { get; set; }
        public bool GenerateBarcode { get; set; }
        public string UtilityName { get; set; }
        // public string WarehouseName { get; set; }
        public int Quantity { get; set; }
        public string SerialNumber { get; set; }
        public string ServerReponseStatus { get; set; }
        public string WebUrlOfBarcodeFile { get; set; }

    }





    public class MobileUserReceiveBo : IDisposable
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


        public List<MobileUserSerialNumberData> SerialNumberList { get; set; }

        public double DeviceDateTimeOfRequest { get; set; }

        //public bool IsRecordProcessed { get; set; }

        public List<MobileUserProductQuantity> ProductQuantityList { get; set; }
        public long ProjectId { get; set; }
        public long UserId { get; set; }
        public string GPSLocation { get; set; }
        public string DeviceIp { get; set; }


    }


    public class MobileUserSerialNumberData
    {

        public long ProductId { get; set; }
        public string ProductMake { get; set; }
        public string ProductName { get; set; }
        public string ProductSize { get; set; }
        public string ProductType { get; set; }
        public string PartNumber { get; set; }
        public string Base { get; set; }
        public string ServerReponseStatus { get; set; }
        public string SerialNumber { get; set; }
        public long ServerRequestId { get; set; }
    }

    public class MobileUserProductQuantity
    {

        public long ReceiveFromId { get; set; }
        public long ReceiveFromSubId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ServerReponseStatus { get; set; }
        public int ProductQuantity { get; set; }
        public long ServerRequestId { get; set; }
    }








}
