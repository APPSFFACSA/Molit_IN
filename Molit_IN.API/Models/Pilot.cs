using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Molit_IN.API.Models
{
    public class Pilots
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Autoincremental (IDENTITY)
        public int PilotId { get; set; }
        public string BranchCode { get; set; }

        [Column(TypeName = "nvarchar(254)")]
        public string? FullName { get; set; }
        public int Age { get; set; }
        public string LicenseType { get; set; }
        public string? LicenseNumber { get; set; }

        [Column(TypeName = "varbinary(max)")]
        public byte[] LicensePhoto { get; set; }
        public string? NamePhoto { get; set; }  

        [Comment("Registro activo")]
        public bool IsActive { get; set; } = true;

        [Comment("Usuario que creo el registro")]
        public string CreatedBy { get; set; } = string.Empty;
        [Comment("Fecha y hora de creación del registro")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [StringLength(256)]
        [Comment("Ultimo usuario que modificó el registro")]
        public string UpdatedBy { get; set; } = string.Empty;
        [Comment("Ultima fecha y hora de actualización del registro")]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }

   

}
