using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    /*
    * -------------------------------------------------------------------------------
    * Create By     : Aniket Jadhav 
    * Created On    : 07-Sep-2016 
    * Description   : BO for Role Master 
    *--------------------------------------------------------------------------------
    */
    public class RoleBO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Nullable<bool> IsSysAdmin { get; set; }
        public string IsAdmin { get; set; }
    }
}
