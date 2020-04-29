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

namespace GarronT.CMS.Model.DAL
{
  public  class StateDAL
    {
      public List<StateModel> GetStatesByCityID(string city)
      {
          CompassEntities CompassEntities = new CompassEntities();
          List<StateModel> objSubMenuesBo = (from b in CompassEntities.tblStateMasters
                                            join c in CompassEntities.TblCityMasters on b.StateId equals c.StateId

                                             where c.CityName==city
                                            select new StateModel
                                            {
                                                StateID = b.StateId,
                                                StateName=b.StateName
                                            }).OrderBy(t => t.StateName).ToList();

          return objSubMenuesBo;
      }

      public List<StateModel> GetStates()
      {
          CompassEntities CompassEntities = new CompassEntities();
          List<StateModel> objSubMenuesBo = (from b in CompassEntities.tblStateMasters
                                             select new StateModel
                                             {
                                                 StateID = b.StateId,
                                                 StateName = b.StateName
                                             }).OrderBy(t => t.StateName).ToList();

          return objSubMenuesBo;
      }

    }
}
