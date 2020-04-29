using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class ExportReportBO : IDisposable
    {

        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            //if (!disposed)
            //{
            //    if (disposing)
            //    {
            //        //dispose managed ressources

            //    }
            //}
            //dispose unmanaged ressources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public long Id { get; set; }
        public long FieldId { get; set; }
        public long TableId { get; set; }
        public string FieldName { get; set; }
        public int SortOrder { get; set; }
        public int Flag { get; set; }
        public int IsExcelMapFieldFlag { get; set; }
    }
}
