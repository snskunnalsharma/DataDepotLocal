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
using System.Data.Entity.Core.Objects;
using KSPL.AIBC.ARM.Model.BO;

namespace GarronT.CMS.Model.DAL
{
    public class VehicleDAL
    {

        public VehicleModel GetVehicleById(long Id)
        {

            CompassEntities objCompassEntities = new CompassEntities();
            VehicleModel objClientModel = (from b in objCompassEntities.tblVehicleMasters
                                           where b.UserId == Id
                                           select new VehicleModel
                                           {
                                               MakeID = b.MakeID,
                                               ModelID = b.ModelID,
                                               Color = b.Color,
                                               LicensePlate = b.LicensePlate,
                                               Year = b.Year,
                                               UserId = b.UserId,
                                               ImageofVehicle=b.ImageofVehicle,
                                               Active = b.Active

                                           }).FirstOrDefault();

            return objClientModel;

        }

        public List<VehicleMakeModel> GetVehicleMakeList()
        {
            try
            {
                CompassEntities objTollFreeWidgetEntities = new CompassEntities();
                List<VehicleMakeModel> objVehicleMakeModel = (from b in objTollFreeWidgetEntities.tblVehicleMakeMasters
                                                              where b.Active == 1
                                                              select new VehicleMakeModel
                                                           {
                                                               MakeId = b.MakeId,
                                                               Make = b.Make
                                                           }).OrderBy(t => t.Make).ToList();

                return objVehicleMakeModel;
            }
            catch (Exception)
            {

                throw;
            }

        }


        public List<VehicleModelVM> GetVehicleModelList(long VehicleMakeID)
        {
            try
            {
                CompassEntities objTollFreeWidgetEntities = new CompassEntities();
                List<VehicleModelVM> objVehicleModelVM = (from b in objTollFreeWidgetEntities.tblVehicleModelMasters
                                                          where b.Active == 1 && b.tblMakeMasterId == VehicleMakeID
                                                          select new VehicleModelVM
                                                            {
                                                                ID = b.ID,
                                                                Model = b.Model,
                                                                tblMakeMasterId = b.tblMakeMasterId
                                                            }).OrderBy(t => t.Model).ToList();

                return objVehicleModelVM;
            }
            catch (Exception)
            {

                throw;
            }

        }







    }



}
