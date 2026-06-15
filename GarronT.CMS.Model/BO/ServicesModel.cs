using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class ServicesModel : IDisposable
    {
        bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            //dispose unmanaged ressources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // public long ServiceId { get; set; }
        // [Required(ErrorMessage = "Service Type Required.")]
        // public string ServiceName { get; set; }
        // public string ServiceDescription { get; set; }
        // public List<ServicesPicture> objServicesPictureList { get; set; }
        // public int MaxAudio { get; set; }
        // public int MaxVideo { get; set; }

        // Added Vishal
        public long ID { get; set; }
        public string Service { get; set; }
        public string Description { get; set; }
        public Nullable<bool> AudioRequired { get; set; }
        public Nullable<int> MaxAudio { get; set; }
        public Nullable<bool> VideoRequired { get; set; }
        public Nullable<int> MaxVideo { get; set; }

        public Nullable<decimal> ServiceBasePrice { get; set; }

        // New: Default/master service cost rate
        public Nullable<decimal> CostRate { get; set; }

        public int Active { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public List<ServiceAttachmentModel> lisServiceAttachment { get; set; }
        public long tblProjectFieldDataMaster_ID { get; set; }
    }
}