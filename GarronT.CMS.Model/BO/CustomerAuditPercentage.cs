using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class CustomerAuditPercentage
    {
        public long AuditId { get; set; }
        public decimal AuditPercentage { get; set; }
        public int StartDay { get; set; }
        public int EndDay { get; set; }
        public int userId { get; set; }
        public long FK_CustomerId { get; set; }
        
    }


    public class CustomerFailPercentage
    {
        public long Id { get; set; }
        public decimal FailPercentageStart { get; set; }
        public decimal FailPercentageEnd { get; set; }
        public int IncreaseByDays { get; set; }
        public int userId { get; set; }
        public long FK_CustomerId { get; set; }
        

    }
}
