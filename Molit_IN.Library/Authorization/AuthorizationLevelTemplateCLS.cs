using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molit_IN.Library.Authorization
{
    public class AuthorizationLevelTemplateCLS
    {
        public int Id { get; set; }

        /// <summary>
        /// Número de nivel (1 = primer nivel, 2 = segundo, etc.)
        /// </summary>
        public int LevelAutho { get; set; }

        /// <summary>
        /// Descripción opcional (p.e. “Jefe de Compras”)
        /// </summary>
        [StringLength(100)]
        public string? Description { get; set; }
        public int? IdUser { get; set; }
    }
}
