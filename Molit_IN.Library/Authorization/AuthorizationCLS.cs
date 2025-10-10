using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molit_IN.Library.Authorization
{
    public class AuthorizationCLS
    {
        public int IdAuthorization { get; set; }
        public int LevelAutho { get; set; }
        public int? IdUser { get; set; }
        public string? Status { get; set; }
        public DateTime DateRevision { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? Comments { get; set; }
        public int IdSettlement { get; set; }
        public  string? CardCode { get; set; }
        public string? CardName { get; set; }
        public decimal Total { get; set; }
        public decimal TotalKM { get; set; }
    }
}
