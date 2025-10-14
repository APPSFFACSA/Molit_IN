using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molit_IN.Library.Pilot
{
    public class PilotListCLS
    {
        public int PilotId { get; set; }
        public string BranchCode { get; set; }

        public string? FullName { get; set; }
        public int Age { get; set; }
        public string LicenseType { get; set; }
        public string? LicenseNumber { get; set; }
        public byte[] LicensePhoto { get; set; }
        public string? NamePhoto { get; set; }
    }
}
