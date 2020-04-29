using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.DAL
{
    public class VersionDAL
    {

        public string getVersionDetials()
        {
            // DateTime selDatenew = Convert.ToDateTime(selDate);
            CompassEntities CompassEntities = new CompassEntities();
            int a1=CompassEntities.tblVersionMasters.Where(o=>o.Active==1).Max(o=>o.VersionId);
            var a = CompassEntities.tblVersionMasters.Where(o => o.Active==1 && o.VersionId == a1).Select(o=>o.VersionNumber).FirstOrDefault();
            return "Version - "+Convert.ToString(a);            
        }
    }
}
