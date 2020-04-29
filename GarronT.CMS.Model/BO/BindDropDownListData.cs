using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class BindDropDownListData
    {

        public List<VW_GETTab6DropdownData> objData { get; set; }

        public List<string> headerlist { get; set; }
        public long ProjectID { get; set; }
    }
}
