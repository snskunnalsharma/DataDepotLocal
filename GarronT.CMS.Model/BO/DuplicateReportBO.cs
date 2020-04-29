using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class DuplicateReportBO
    {

        public long ID { get; set; }
        public string Account { get; set; }
        public string Street { get; set; }
        public string Customer { get; set; }
        public string OldMeterNo { get; set; }
        public string OldMeterRadioNo { get; set; }
        public long UserID { get; set; }
        public string FirstName { get; set; }
        public string visitdatetime { get; set; }
        public string NEWMETERRADIO { get; set; }
        public string NEWMETERNUMBER { get; set; }
    }
}
