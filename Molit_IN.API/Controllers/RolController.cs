using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Molit_IN.API.Data;
using Molit_IN.API.Models;          // Role
using Molit_IN.Library.Rol;         // RolListCLS, RolFormAdd
using System.Linq;

namespace Molit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1")]
    public class RolController : ControllerBase
    {
        private readonly ApplicationDbContext _bd;

        public RolController(ApplicationDbContext bd)
        {
            _bd = bd;
        }

        // GET: api/Rol
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var lista = (from rol in _bd.Roles
                             where rol.IsActive == true
                             select new RolListCLS
                             {
                                 RoleId = rol.RoleId,
                                 RoleName = rol.RoleName
                             }).ToList();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Rol/5
        [HttpGet("{idrol:int}")]
        public ActionResult Get(int idrol)
        {
            try
            {
                var rol = _bd.Roles
                    .Where(p => p.RoleId == idrol)
                    .Select(p => new RolListCLS
                    {
                        RoleId = p.RoleId,
                        RoleName = p.RoleName
                    })
                    .FirstOrDefault();

                if (rol is null) return NotFound();
                return Ok(rol);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Rol  (Crear)
        [HttpPost]
        public ActionResult Post([FromBody] RolFormAdd oRolFormAdd)
        {
            try
            {
                if (oRolFormAdd is null) return BadRequest("Datos inválidos.");

                if (oRolFormAdd.RoleId != 0)
                    return BadRequest("RoleId debe ser 0 para crear.");

                var entity = new Role
                {
                    RoleName = oRolFormAdd.RoleName,
                    IsActive = true
                };

                _bd.Roles.Add(entity);
                _bd.SaveChanges();

                return Ok("Se guardó correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Rol  (Editar)
        [HttpPut]
        public ActionResult Put([FromBody] RolFormAdd oRolFormAdd)
        {
            try
            {
                if (oRolFormAdd is null) return BadRequest("Datos inválidos.");
                if (oRolFormAdd.RoleId == 0)
                    return BadRequest("RoleId requerido para actualizar.");

                var entity = _bd.Roles.Find(oRolFormAdd.RoleId);
                if (entity is null) return NotFound();

                entity.RoleName = oRolFormAdd.RoleName;
                _bd.SaveChanges();

                return Ok("Se actualizó correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/Rol/5 (Borrado lógico)
        [HttpDelete("{idrol:int}")]
        public IActionResult Delete(int idrol)
        {
            try
            {
                var entity = _bd.Roles
                    .FirstOrDefault(p => p.RoleId == idrol && p.IsActive == true);

                if (entity is null) return NotFound();

                entity.IsActive = false;
                _bd.SaveChanges();

                return Ok("Se eliminó correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //// DEBUG opcional: ver claims del usuario autenticado
        //// GET: api/Rol/whoami
        //[HttpGet("whoami")]
        //public IActionResult WhoAmI()
        //{
        //    var claims = User?.Claims.Select(c => new { c.Type, c.Value }).ToList();
        //    return Ok(new
        //    {
        //        IsAuth = User?.Identity?.IsAuthenticated ?? false,
        //        Name = User?.Identity?.Name,
        //        Claims = claims
        //    });
        //}
    }
}
