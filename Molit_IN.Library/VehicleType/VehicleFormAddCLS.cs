using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molit_IN.Library.VehicleType
{
    public class VehicleFormAddCLS
    {
        public int IdVehicleType { get; set; }
        public string? DscriptionVehicle { get; set; }
        public string? ItemCode { get; set; }
    }
}
