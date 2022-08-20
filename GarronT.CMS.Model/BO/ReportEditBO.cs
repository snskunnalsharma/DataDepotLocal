using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{

    public class ImageSliderList
    {
        public long ImageId { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
    }

    public class ReportEditBO
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
    public class DetailsAll
    {
        public string ProjectName { get; set; }
        public string FromName { get; set; }
        public string InstallerName { get; set; }
        public string AuditorName { get; set; }
        public string CustomerAddress { get; set; }
        public string UtilityType { get; set; }

    }
    public class ImageandServicesName
    {
        public string ImageName { get; set; }
        public string SeravicesName { get; set; }
    }

    public class UploadFilesResult
    {
        public string Name { get; set; }
        public int Length { get; set; }
        public string Type { get; set; }
    }

    public class newReportEditBO
    {
        public long userId1 { get; set; }
        public int projectId1 { get; set; }
        public long accountinifiledId { get; set; }
        public long reportFormId { get; set; }
    }

    // Added Class by vishal to get audio or video path nd service name
    public class GetAttachmentBo
    {
        public long AttachmentID { get; set; }
        public string strPath { get; set; }
        public string strServiceName { get; set; }

    }

    //Added by Sominath to display report data on popup
    public class AuditorAllocationData
    {
        public DateTime ServiceDateTime { get; set; }
        public string VisitDateTime { get; set; }
        public string Account { get; set; }
        public string Cycle { get; set; }
        public string Route { get; set; }
        public string Address { get; set; }
        public string FormName { get; set; }
        public string InstallerName { get; set; }
        public string Status { get; set; }
        public string ProjectName { get; set; }
        public long AuditorID { get; set; }
        public string AuditorName { get; set; }
        public long ProjectId { get; set; }
        public long UploadId { get; set; }
    }
}
namespace GarronT.CMS.Model
{
    public partial class STP_GetReportTableNew_bak_Result
    {
        public bool IsDuplicate { get; set; }

        public Nullable<long> AuditorId { get; set; }

        public List<SkipRtuAttachmentModel> SkipRtuAttachmentModel { get; set; }

        public int AuditStaus { get; set; }

        public string mapImagePath { get; set; }
        public string skipGPSLocation { get; set; }
        public string OLDMETERFINALREADING { get; set; }
        public string NEWMETERREADING { get; set; }
        public string NEWRADIONUMBER { get; set; }
        //OLDMETERFINALREADING, NEWMETERREADING,NEWMETERSIZE1

    }
}

namespace GarronT.CMS.Model
{
    using System;
    using System.Collections.Generic;

    public partial class TblProjectFieldDataAttachment
    {
        public string ImageName { get; set; }
    }

    public partial class ProjectAuditFieldDataAttachment
    {
        public string ImageName { get; set; }
    }
}
