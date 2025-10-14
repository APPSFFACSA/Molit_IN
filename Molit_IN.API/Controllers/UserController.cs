using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Molit_IN.API.Data;
using Molit_IN.API.Functions;
using Molit_IN.API.Models;
using Molit_IN.Library.User;

namespace Molit_IN.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _bd;
        public UserController(ApplicationDbContext bd)
        {
            _bd = bd;
        }

        // GET: api/User
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var usuariosBD = (from usuario in _bd.Users
                                  where usuario.IsActive == true
                                  select new
                                  {
                                      usuario.IdUser,
                                      usuario.UserName,
                                      usuario.FullName,
                                      usuario.Email,
                                      usuario.RoleId,
                                      usuario.BranchName,
                                      usuario.BranchCode,
                                      usuario.TypeUser
                                  }).ToList();

                var lista = usuariosBD.Select(usuario => new UserListCLS
                {
                    IdUser = usuario.IdUser,
                    UserName = usuario.UserName,
                    FullName = usuario.FullName,
                    Email = usuario.Email,
                    RoleId = usuario.RoleId,
                    BranchCode = usuario.BranchCode,
                    BranchName = usuario.BranchName,
                    TypeUser = Enum.TryParse<UserListCLS.UserType>(usuario.TypeUser, out var tipoUsuario)
                               ? tipoUsuario : UserListCLS.UserType.Int
                }).ToList();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/User/{id}
        // Devuelve UserFormAddCLS para alinear con el cliente
        [HttpGet("{idusuario}")]
        public async Task<ActionResult> Get(int idusuario)
        {
            try
            {
                var dto = await _bd.Users
                    .Where(x => x.IsActive && x.IdUser == idusuario)
                    .Select(x => new
                    {
                        x.IdUser,
                        x.UserName,
                        x.FullName,
                        x.Email,
                        x.RoleId,
                        x.BranchCode,
                        x.BranchName,
                        TypeUser = x.TypeUser // string en DB
                    })
                    .FirstOrDefaultAsync();

                if (dto is null) return NotFound();

                var model = new UserFormAddCLS
                {
                    IdUser = dto.IdUser,
                    UserName = dto.UserName,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    RoleId = dto.RoleId,
                    BranchCode = dto.BranchCode,
                    BranchName = dto.BranchName,
                    TypeUser = Enum.TryParse<UserFormAddCLS.UserType>(dto.TypeUser, out var tu)
                                ? tu
                                : UserFormAddCLS.UserType.Int
                };

                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/User/{id}
        [HttpDelete("{idusuario}")]
        public IActionResult Delete(int idusuario)
        {
            try
            {
                var entity = _bd.Users.FirstOrDefault(p => p.IdUser == idusuario && p.IsActive);
                if (entity == null) return NotFound();

                entity.IsActive = false;
                _bd.SaveChanges();
                return Ok("Se eliminó el usuario correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/User  (Crear)
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserFormAddCLS oUsuarioFormAddCLS)
        {
            try
            {
                if (oUsuarioFormAddCLS is null)
                    return BadRequest("Modelo vacío");

                if (oUsuarioFormAddCLS.IdUser != 0)
                    return BadRequest("Para actualizar use PUT /api/User/{id}");

                // Password requerida en alta
                if (string.IsNullOrWhiteSpace(oUsuarioFormAddCLS.Password))
                    return BadRequest("La contraseña es obligatoria para crear el usuario.");

                // Normalización ligera
                oUsuarioFormAddCLS.BranchCode ??= "0";
                oUsuarioFormAddCLS.BranchName ??= "na";

                var clavecifrada = Cifrar.cifrarCadena(oUsuarioFormAddCLS.Password);

                var oUser = new User
                {
                    UserName = oUsuarioFormAddCLS.UserName,
                    Password = clavecifrada,
                    FullName = oUsuarioFormAddCLS.FullName,
                    RoleId = oUsuarioFormAddCLS.RoleId,
                    TypeUser = oUsuarioFormAddCLS.TypeUser.ToString(),
                    Email = oUsuarioFormAddCLS.Email,
                    BranchCode = oUsuarioFormAddCLS.BranchCode,
                    BranchName = oUsuarioFormAddCLS.BranchName,
                    IsActive = true
                };

                _bd.Users.Add(oUser);
                await _bd.SaveChangesAsync();

                // Opcional: devolver ubicación del recurso creado
                return CreatedAtAction(nameof(Get), new { idusuario = oUser.IdUser }, new { message = "Se guardó el usuario correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/User/{id}  (Actualizar)
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserFormAddCLS modelo)
        {
            try
            {
                if (modelo is null) return BadRequest("Modelo vacío.");
                if (id != modelo.IdUser) return BadRequest("El id de la ruta no coincide con el del modelo.");

                var entity = await _bd.Users.FirstOrDefaultAsync(u => u.IdUser == id && u.IsActive);
                if (entity is null) return NotFound();

                // Campos actualizables
                entity.FullName = modelo.FullName;
                entity.Email = modelo.Email;
                entity.RoleId = modelo.RoleId;
                entity.BranchCode = string.IsNullOrWhiteSpace(modelo.BranchCode) ? "0" : modelo.BranchCode;
                entity.BranchName = string.IsNullOrWhiteSpace(modelo.BranchName) ? "na" : modelo.BranchName;
                entity.TypeUser = modelo.TypeUser.ToString();

                // Si viene nueva contraseña desde el formulario (edición con toggle), actualizarla
                if (!string.IsNullOrWhiteSpace(modelo.Password))
                {
                    entity.Password = Cifrar.cifrarCadena(modelo.Password);
                }

                await _bd.SaveChangesAsync();
                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
