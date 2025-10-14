using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molit_IN.Library.User
{
    public class UserListCLS
    {
        public int IdUser { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? UserName { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? Email { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string FullName { get; set; }
        public UserType TypeUser { get; set; }
        public int IdBranch { get; set; }
        public string? nameSupplier { get; set; }
        public int  RoleId { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public enum UserType
        {
            Int,
            Ext
        }
    }

   
}
