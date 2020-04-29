using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class ReOpneMail
    {
        public DateTime Dates { get; set; }
        public string Account { get; set; }
        public string Cycle { get; set; }
        public string Route { get; set; }

        public string Street { get; set; }
        public string FirstName { get; set; }

        public string FormName { get; set; }
        public string OldMeterNo { get; set;  }
        public string OldMeterRadioNo { get; set; }
        public string CustomerName { get; set; }
    }
}
