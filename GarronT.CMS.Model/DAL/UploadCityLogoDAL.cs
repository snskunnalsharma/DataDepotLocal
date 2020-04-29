using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.DAL
{
    public class UploadCityLogoDAL : IDisposable
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
        public UploadCityLogoDAL()
        {


        }

        public UploadedImageData GetData(long cityLogoId)
        {
            UploadedImageData _objUploadedImageData = new UploadedImageData();
            using (CompassEntities _objCompass = new CompassEntities())
            {

                var list = _objCompass.TblStateCityLogoes.Where(o => o.CityLogoID == cityLogoId).FirstOrDefault();

                _objUploadedImageData.CityLogoID = list.CityLogoID;
                _objUploadedImageData.FK_CityId = list.FK_CityId;
                _objUploadedImageData.FK_StateId = list.FK_StateId;
                _objUploadedImageData.FK_CustomerId = list.FK_CustomerId;
                _objUploadedImageData.ImageName = list.ImageName;
                _objUploadedImageData.strImagePath = list.CityLogoImagePath;

            }
            return _objUploadedImageData;
        }


        public bool DeactiveRecord(long cityLogoId, long currentUserId)
        {
            DateTime dt = new CommonFunctions().ServerDate();
            using (CompassEntities _objCompass = new CompassEntities())
            {
                var result = _objCompass.TblStateCityLogoes.Where(o => o.CityLogoID == cityLogoId).FirstOrDefault();
                result.CityLogoID = cityLogoId;
                result.Active = 0;
                result.ModifiedOn = dt;
                result.ModifiedBy = (int)currentUserId;
                _objCompass.SaveChanges();

                return true;

            }
        }

        public bool CheckRecord (long cityId, long customerId)
        {
            using (CompassEntities _objCom = new CompassEntities())
            {
                var count = _objCom.TblStateCityLogoes.Where(o => o.FK_CustomerId==customerId && o.FK_CityId==cityId).Count();
                if (count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public List<StateVM> GetActiveStateRecords()
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<StateVM> objSubMenuesBo = (from b in CompassEntities.tblStateMasters


                                            select new StateVM
                                            {
                                                StateID = b.StateId,
                                                StateName = b.StateName
                                            }).OrderBy(t => t.StateName).ToList();

            return objSubMenuesBo;
        }

        public List<CityVM> GetActiveCityRecords(long stateId)
        {
            var list = new List<CityVM>();
            CompassEntities CompassEntities = new CompassEntities();

            list = CompassEntities.TblCityMasters.AsNoTracking().Where(o => o.StateId == stateId && o.Active == 1).Select(x => new CityVM { CityID = x.CityId, CityName = x.CityName }).ToList();
            return list;
        }


        public List<CustomerVM> GetActiveCustomerRecords()
        {
            var list = new List<CustomerVM>();
            CompassEntities CompassEntities = new CompassEntities();

            list = CompassEntities.tblClients.AsNoTracking().Where(o => o.Active == 1).Select(x => new CustomerVM { CustomerID = x.ClientId, CustomerName = x.ClientName }).ToList();
            return list;
        }

        public bool InsertIntoTblStateCityLogo(string fileStream, long stateid, long cityid, long customerId, int LoginId, string imageLogName, string imageName, long CityLogoImageId)
        {
            DateTime dt = new CommonFunctions().ServerDate();
            CompassEntities objCompassEntities = new CompassEntities();
            bool succes = false;

            try
            {
                var _objTblStateCityLogo = new TblStateCityLogo();

                if (CityLogoImageId != 0)
                {
                    var _objUpdateRecord = objCompassEntities.TblStateCityLogoes.Where(o => o.CityLogoID == CityLogoImageId).FirstOrDefault();

                    _objUpdateRecord.CityLogoID = CityLogoImageId;
                    _objUpdateRecord.FK_StateId = stateid;
                    _objUpdateRecord.FK_CityId = cityid;
                    _objUpdateRecord.FK_CustomerId = customerId;
                    _objUpdateRecord.CityLogoImagePath = imageLogName;
                    _objUpdateRecord.ImageName = imageName;
                    _objUpdateRecord.CreatedBy = LoginId;
                    _objUpdateRecord.CreatedOn = dt;
                    _objUpdateRecord.ModifiedBy = LoginId;
                    _objUpdateRecord.ModifiedOn = dt;
                    _objUpdateRecord.Active = 1;
                    objCompassEntities.SaveChanges();
                }
                else
                {
                    _objTblStateCityLogo.FK_StateId = stateid;
                    _objTblStateCityLogo.FK_CityId = cityid;
                    _objTblStateCityLogo.FK_CustomerId = customerId;
                    _objTblStateCityLogo.CityLogoImagePath = imageLogName;
                    _objTblStateCityLogo.ImageName = imageName;
                    _objTblStateCityLogo.CreatedBy = LoginId;
                    _objTblStateCityLogo.CreatedOn = dt;
                    _objTblStateCityLogo.ModifiedBy = LoginId;
                    _objTblStateCityLogo.ModifiedOn = dt;
                    _objTblStateCityLogo.Active = 1;
                    objCompassEntities.TblStateCityLogoes.Add(_objTblStateCityLogo);

                }

                objCompassEntities.SaveChanges();
                succes = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return succes;
        }

        public List<UploadedImageData> GetUploadedImagRecords()
        {
            List<UploadedImageData> myList = new List<UploadedImageData>();

            using (CompassEntities _objCompass = new CompassEntities())
            {

                var list = _objCompass.TblStateCityLogoes.Where(o => o.Active == 1).ToList();

                foreach (var item in list)
                {
                    var _objUploadedImageData = new UploadedImageData();

                    _objUploadedImageData.CityLogoID = item.CityLogoID;
                    _objUploadedImageData.StateName = _objCompass.tblStateMasters.AsNoTracking().Where(o => o.StateId == item.FK_StateId).Select(o => o.StateName).FirstOrDefault();

                    _objUploadedImageData.CityName = _objCompass.TblCityMasters.AsNoTracking().Where(o => o.CityId == item.FK_CityId && o.Active == 1).Select(o => o.CityName).FirstOrDefault();
                    _objUploadedImageData.CustomerName = _objCompass.tblClients.AsNoTracking().Where(o => o.ClientId == item.FK_CustomerId && o.Active == 1).Select(o => o.ClientName).FirstOrDefault();
                    _objUploadedImageData.ImageName = item.ImageName;
                    _objUploadedImageData.strImagePath = item.CityLogoImagePath;

                    myList.Add(_objUploadedImageData);

                }

            }

            return myList;


        }


    }
}
