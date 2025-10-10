using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Molit_IN.API.Models
{
    public class Menu
    {
        [Key]
        public int MenuId { get; set; }

        [Required]
        [StringLength(50)]

        public string OpcionName { get; set; } = string.Empty;

        public string? Icon { get; set; }
        public string? Url { get; set; }

        [Comment("Registro activo")]
        public bool IsActive { get; set; } = true;
        [StringLength(256)]
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
