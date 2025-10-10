using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Molit_IN.API.Data;
using Molit_IN.API.Functions;
using Molit_IN.Library.Login;
using Molit_IN.Library.Menu;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Molit_IN.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _bd;
        public LoginController(ApplicationDbContext bd)
        {
            _bd = bd;
        }

        [HttpPost]
        public ActionResult Post([FromBody] LoginCLS oLoginCLS)
        {
            try
            {
                // Normalizar entradas
                var username = oLoginCLS?.username?.Trim();
                var password = oLoginCLS?.password?.Trim();

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return Unauthorized("Usuario o contraseña incorrectos");

                // Cifrar con el mismo método usado al crear usuarios
                string clavecifrada = Cifrar.cifrarCadena(password);

                // Buscar usuario (case-insensitive de forma segura)
                string usernameUpper = username.ToUpperInvariant();
                var usuario = _bd.Users
                    .FirstOrDefault(p => p.UserName.ToUpper() == usernameUpper && p.Password == clavecifrada);

                if (usuario == null)
                {
                    return Unauthorized("Usuario o contraseña incorrectos");
                }

                // Construcción de claims evitando valores null
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(ClaimTypes.Name, usuario.UserName ?? string.Empty),
                    new Claim(ClaimTypes.Role, usuario.RoleId.ToString()),
                    new Claim("FullName", usuario.FullName ?? string.Empty),
                    new Claim("RoleId", usuario.RoleId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                if (!string.IsNullOrWhiteSpace(usuario.CardCode))
                {
                    claims.Add(new Claim("CardCode", usuario.CardCode));
                }

                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("BO7YThbqh51BmXcAyReF806mMjHgMDik"));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(5),
                    Issuer = "Molit_IN.API",
                    Audience = "Molit_IN.Client",
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new { token = tokenHandler.WriteToken(token) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
