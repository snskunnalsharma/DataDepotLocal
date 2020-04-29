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

namespace GarronT.CMS.Model.DAL
{
   public  class CityDAL
    {



       public List<CityModel> GetCityByID()
       {
           CompassEntities CompassEntities = new CompassEntities();
           List<CityModel> objCityModel = (from b in CompassEntities.TblCityMasters
                                           where b.Active == 1
                                           select new CityModel
                                           {
                                               CityID = b.CityId,
                                               CityName = b.CityName,
                                              // StateID = b.StateId,

                                               Active = b.Active
                                           }).OrderBy(t => t.CityName).ToList();

           List<CityModel> uniqueIDs = objCityModel.GroupBy(x=> x.CityName).Select(x=> x.First()).ToList();
           return uniqueIDs;

        
       }

       public List<CityModel> GetCityByIDs(int id)
       {
           CompassEntities CompassEntities = new CompassEntities();
           List<CityModel> objCityModel = (from b in CompassEntities.TblCityMasters
                                           where b.StateId==id && b.Active == 1 
                                           select new CityModel
                                           {
                                                 CityID = b.CityId,
                                               CityName = b.CityName,
                                               // StateID = b.StateId,

                                               Active = b.Active
                                           }).OrderBy(t => t.CityName).ToList();

           List<CityModel> uniqueIDs = objCityModel.GroupBy(x => x.CityName).Select(x => x.First()).ToList();
           return uniqueIDs;


       }


     
      

    }


   
}
