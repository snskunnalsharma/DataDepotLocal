using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
   public class ExcelSheetBO
    {
        public long FieldID { get; set; }
        public long FieldType { get; set; }
        public string FieldName { get; set; }
    }

    public class officeCollection
    {
        public string accountNo { set; get; }
        public string officeName { set; get; }
        public string type { set; get; }
    }
}
