using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{
    public class CategoryBO
    {
        public long CategoryId { get; set; }

        [Required(ErrorMessage = "Warehouse Name is required")]
        [StringLength(250, ErrorMessage = "Warehouse Name must be at least {2} and max {1} characters long.", MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string CategoryName { get; set; }

        public long CurrentUserId { get; set; }
    }
}
