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
        public string DisplayWeekRangeDate { get; set; }
        public int SerialNo { set; get; }
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
        public string Pay { get; set; }
        public string Total { get; set; }

    }
    public class Get_CTR_New        // CTR = Clock Time Report
    {
        public string Description { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string TotalWorkInWeek { get; set; }
        public string Pay { get; set; }
        public string Total { get; set; }

    }
    public class Model_FinalResult
    {
        public int TotalInstalledMeterInWeek { set; get; }
        public decimal TotalWorkingHoursInWeek { set; get; }
        public decimal AvgPerHours { set; get; }
        public List<Get_CTR> _GetCTR { get; set; }
        public string filePath { get; set; }
    }

    
    public class HeaderDates
    {
        public string DayName { get; set; }
        public string DynamicDate { get; set; }
    }


    public class Model_UserWorkedTime       
    {
        //public string Dates { get; set; }
        public string Day { get; set; }
        public string StartTime { get; set; }
        public string LunchBreak { get; set; }
        public string EndTime { get; set; }
        public string TotalHours { get; set; }
    }

    public class Model_MeterSizeDetail
    {
        public string Day { get; set; }
        public string MeterSize { get; set; }
        public string MeterDisplayName { get; set; }
        public string MeterCount { get; set; }
        public int TotalInstaledMeter { get; set; }
        public string Pay { get; set; }
    }

    public class Model_InstalledMeterDetail
    {
        public long ProjectId { get; set; }
        public string MeterSize { get; set; }
        public string MeterType { get; set; }
        public string CreatedOn { get; set; }
        public string DayName { get; set; }
        public string Time { get; set; }
        public long ProjectFieldDataId { get; set; }
    }

    public class Model_InstalledMeterDetailPayRate
    {
        public long ProjectId { get; set; }
        public string MeterSize { get; set; }
        public string MeterType { get; set; }
        public string MeterDisplayName { get; set; }
        public string CreatedOn { get; set; }
        public string DayName { get; set; }
        public string Time { get; set; }
        public string Pay { get; set; }
    }

    public class YearsListModel
    {
        public long YearId { get; set; }
        public string YearName { get; set; }
    }
}
