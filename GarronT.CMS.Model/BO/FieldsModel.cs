using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class FieldsModel
    {

        public int FieldId { get; set; }        
        public string FieldName { get; set; }
        public string Name { get; set; }       

    }


    public class FieldDataTypeMasterModel
    {

        public long DataTypeId { get; set; }
        public string FieldDataTypeName { get; set; }
        public string FieldDataTypeDescription { get; set; }
        public int Active { get; set; }

    }
}
