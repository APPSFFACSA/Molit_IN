using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Molit_IN.API.Data;
using Molit_IN.API.Functions;
using Molit_IN.API.Models;
using Molit_IN.Library.Rol;
using System.Transactions;

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

        [HttpGet("{idrol}")]
        public ActionResult Get(int idrol)
        {
            try
            {
                var usuario = _bd.Roles.Where(p => p.RoleId == idrol).
                    Select(p => new RolListCLS
                    {
                        RoleId = p.RoleId,
                        RoleName = p.RoleName
                    }).
                    FirstOrDefault();
                if (usuario == null)
                {
                    return NotFound();
                }
                else
                    return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete("{idrol}")]
        public IActionResult Delete(int idrol)
        {
            try
            {
                var carrera = _bd.Roles.Where(p => p.RoleId == idrol && p.IsActive == true).
              FirstOrDefault();
                if (carrera == null)
                {
                    return NotFound();
                }
                carrera.IsActive = false;
                _bd.SaveChanges();
                return Ok("Se elimino correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        public ActionResult Post([FromBody] RolFormAdd oRolFormAdd)
        {
            try
            {
                if (oRolFormAdd.RoleId == 0)
                {

                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        Role oRole = new Role();
                        oRole.RoleName = oRolFormAdd.RoleName;
                        _bd.Roles.Add(oRole);
                        _bd.SaveChanges();
                        _bd.SaveChanges();
                        transaccion.Complete();
                    }
                    return Ok("Se guardo correctamente");
                }
                else
                {
                    Role oRole = _bd.Roles.Find(oRolFormAdd.RoleId);
                    if (oRole == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        oRole.RoleName = oRolFormAdd.RoleName;
                        _bd.SaveChanges();
                        return Ok("Se actualizó correctamente");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
