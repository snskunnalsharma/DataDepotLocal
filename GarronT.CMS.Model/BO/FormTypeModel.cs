using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class BaseFormTypeModel
    {
        public long FormTypeID { get; set; }
        public string FormType { get; set; }
        public bool ImpactStatus { get; set; }
    }

    public class FormTypeDOL
    {
        public long ID { get; set; }
        public string strFormType { get; set; }
        public string FormTypeDescription { get; set; }
        public List<BaseFormTypeModel> objBaseFormTypeModelList { get; set; }

    }



}
