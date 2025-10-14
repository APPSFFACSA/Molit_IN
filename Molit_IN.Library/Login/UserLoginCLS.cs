
using Molit_IN.Library.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molit_IN.Library.Login
{
    public class UserLoginCLS
    {
        public int iduusario { get; set; }
        public string FullName { get; set; }
        public UserType TypeUser { get; set; }
        public int IdBranch { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public string? nameSupplier { get; set; }
        public int RoleId { get; set; }

        public List<MenuListCLS> listamenu { get; set; }

        public enum UserType
        {
            Int,
            Ext
        }
    }
}
