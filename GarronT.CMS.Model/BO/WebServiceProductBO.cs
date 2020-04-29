using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class WebServiceProductBO
    {
        public List<WebServiceUtilityBO> UtilityList { get; set; }
        public List<WebServiceCategoryBO> CategoryList { get; set; }
        public List<WebServiceMakeBO> MakeList { get; set; }
        public List<WebServiceTypeBO> TypeList { get; set; }
        public List<WebServiceSizeBO> SizeList { get; set; }
        public List<WebUserListBO> ManagerList { get; set; }
    }

    public class WebUserListBO
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
    }

    public class WebServiceUtilityBO
    {
        public long UtilityId { get; set; }
        public string UtilityName { get; set; }
    }


    public class WebServiceCategoryBO
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class WebServiceMakeBO
    {
        public long MakeId { get; set; }
        public string MakeName { get; set; }
    }

    public class WebServiceTypeBO
    {
        public long TypeId { get; set; }
        public string TypeName { get; set; }
    }

    public class WebServiceSizeBO
    {
        public long SizeId { get; set; }
        public string SizeName { get; set; }
    }


    public class WebServiceProductReceiveBO
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string PartNumber { get; set; }
        public string UPCCode { get; set; }
        public bool IsSerialNumber { get; set; }
        public bool GenerateBarcode { get; set; }
        public long CategoryId { get; set; }
        public long MakeId { get; set; }
        public long TypeId { get; set; }
        public long SizeId { get; set; }
        public long UtilityId { get; set; }
        public long ProjectId { get; set; }
        public long UserId { get; set; }
        public int? WarehouseMinimumStock { get; set; }
        public int? UserMinimumStock { get; set; }
        public List<long> ManagerList { get; set; }
        public bool AlertTypeEmail { get; set; }

    }

}
