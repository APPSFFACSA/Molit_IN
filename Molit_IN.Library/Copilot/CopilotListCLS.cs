using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molit_IN.Library.Copilots
{
    public class CopilotListCLS
    {
        public int CopilotsId { get; set; }
        public int? PilotId { get; set; }
        public string BranchCode { get; set; }
        public string CodEmpleado { get; set; }
        public string? FullName { get; set; }
        public int Age { get; set; }
        public string LicenseType { get; set; }
        public string? LicenseNumber { get; set; }
        public byte[] LicensePhoto { get; set; }
        public string? NamePhoto { get; set; }
        public bool IsActive { get; set; } = true;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
        public string PilotFullName { get; set; }
    }
}
