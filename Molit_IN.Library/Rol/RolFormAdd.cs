using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molit_IN.Library.Rol
{
    public class RolFormAdd
    {
        public int RoleId { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? RoleName { get; set; }
    }
}
