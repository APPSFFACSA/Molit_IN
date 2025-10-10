using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molit_IN.Library.Login
{
    public class LoginCLS
    {
        [Required(ErrorMessage = "Debe ingresar el usuario")]
        public string username { get; set; }

        [Required(ErrorMessage = "Debe ingresar la contraseña")]
        public string password { get; set; }
    }
}
