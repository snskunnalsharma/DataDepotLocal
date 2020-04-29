using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class ServicesPicture : IDisposable
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

        public long Id { get; set; }
        //[Required(ErrorMessage = "Service Type Required")]
        public long ServiceId { get; set; }
        public long PictureId { get; set; }
        public bool isSelected { get; set; }
        public string ServiceName { get; set; }
        public string PictureName { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }


    }
}
