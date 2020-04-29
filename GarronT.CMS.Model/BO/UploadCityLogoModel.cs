using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
   public class UploadCityLogoModel
    {



    }
    public class StateVM
    {
        public long StateID { get; set; }
        public string StateName { get; set; }
    }

    public class CustomerVM
    {
        public long CustomerID { get; set; }
        public string CustomerName { get; set; }
    }

    public class CityVM
    {
        public long CityID { get; set; }
        public long StateID { get; set; }
        public string CityName { get; set; }
    }

    public class MyFileModel
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileSize { get; set; }
        public long StateID { get; set; }
        public long CityID { get; set; }

    }

    public class UploadedImageData
    {
       
        public long CityLogoID { get; set; }
        public long FK_StateId { get; set; }
        public long FK_CityId { get; set; }
        public long FK_CustomerId { get; set; }      
        public string ImageName { get; set; }  
        public string strImagePath { get; set; }

        public string StateName { get; set; }
        public string CityName { get; set; }  
         public string CustomerName { get; set; }  
        
    }
}
