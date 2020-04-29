using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class MovementReport
    {
        public string SerialNumber { get; set; }
        public string StockStatus { get; set; }
        public string Location { get; set; }
        public string StockMovementDateString { get; set; }
        public Nullable<System.DateTime> StockMovementDate { get; set; }
        public string UserName { get; set; }

        public string LatLong { get; set; }
        public string Account { get; set; }
        public string Street { get; set; }
        public string ProjectName { get; set; }

    }
}
