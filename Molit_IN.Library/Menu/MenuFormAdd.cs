using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molit_IN.Library.Menu
{
    public class MenuFormAdd
    {
        public int MenuId { get; set; }

        [Required]
        [StringLength(50)]
        public string? OpcionName { get; set; }
        public string? Icon { get; set; }
        public string? Url { get; set; }
    }
}
