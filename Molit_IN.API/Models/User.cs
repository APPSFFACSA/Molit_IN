using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molit_IN.API.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUser { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? UserName { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? Email { get; set; }
        [Column(TypeName = "nvarchar(256)")]
        public string? Password { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string FullName { get; set; }
        public string TypeUser { get; set; }

        //si el tipo es externo estos campos se activan en la creación del usuario.
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public int RoleId { get; set; }
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

        public enum UserType
        {
            Int,
            Ext
        }
    }
}
