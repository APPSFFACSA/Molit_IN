using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molit_IN.Library.Menu
{
    public class MenuRolListCLS
    {
        public int RoleMenuId { get; set; }
        public int MenuId { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [NotMapped]
        public string OpcionName { get; set; }
        public string RoleName { get; set; }
    }
}
