using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class StatusMessage
    {
        public StatusMessage()
        {
            this.Success = false;
            this.Message = "";
        }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
