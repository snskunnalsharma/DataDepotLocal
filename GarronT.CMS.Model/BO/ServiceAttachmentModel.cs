using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class ServiceAttachmentModel
    {
        public long id { get; set; }
        public long tblProjectFieldDataMaster_ID { get; set; }
        public int IsAudioVideoImage { get; set; }
        public string strFilePath { get; set; }
        public bool? isServiceData { get; set; }
        public long? TblServicePicture_ID { get; set; }
        public int Active { get; set; }
    }



    public class SkipRtuAttachmentModel
    {
        public long id { get; set; }
        public long ProjectFieldDataID { get; set; }
        public int IsAudioVideoImage { get; set; }
        public string strFilePath { get; set; }
    }
}
