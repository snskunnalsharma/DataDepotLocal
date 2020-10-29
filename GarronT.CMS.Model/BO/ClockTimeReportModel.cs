using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GarronT.CMS.Model.BO
{
    class ClockTimeReportModel
    {

    }
    public class WeeksModel
    {
        public string WeekRangeDate { get; set; }
        //public DateTime WeekFromDate { get; set; }
        //public DateTime WeekToDate { get; set; }
        public string DisplayWeekRangeDate { get; set; }
    }

    public class Get_CTR        // CTR = Clock Time Report
    {
        public string Description { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string TotalWorkInWeek { get; set; }

    }
    public class Get_CTR_Temp        // CTR = Clock Time Report
    {
        public string Description { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public TimeSpan TotalWeekedInaDay { get; set; }

    }

    public class DetailDayTemp
    {
        public string Description { get; set; }
        public string Day{ get; set; }
        public string ClockTime { get; set; }
        public TimeSpan TotalWorkedTime { get; set; }
    }
    public class DetailDay
    {
        public string Description { get; set; }
        public string Day { get; set; }
        public string ClockTime { get; set; }
        public TimeSpan TotalWorkedTime { get; set; }
    }
    public class HeaderDates
    {
        public string DayName { get; set; }
        public string DynamicDate { get; set; }
    }
}
