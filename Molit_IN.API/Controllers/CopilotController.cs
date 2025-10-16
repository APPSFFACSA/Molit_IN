using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Molit_IN.API.Data;
using Molit_IN.API.Models;         // Entidades EF: Copilots, Pilots
using Molit_IN.Library.Copilots;   // DTOs: CopilotListCLS, CopilotFormAddCLS
using System.Transactions;
using System.Linq;

namespace Molit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2")]
    public class CopilotController : ControllerBase
    {
        private readonly ApplicationDbContext _bd;
        public CopilotController(ApplicationDbContext bd)
        {
            _bd = bd;
        }

        // GET: /api/Copilot
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var lista =
                    (from c in _bd.Copilots
                     where c.IsActive == true
                     join p in _bd.Pilots on c.PilotId equals p.PilotId into gp
                     from p in gp.DefaultIfEmpty() // LEFT JOIN (puede no existir piloto)
                     select new CopilotListCLS
                     {
                         CopilotsId = c.CopilotsId,
                         PilotId = c.PilotId,
                         BranchCode = c.BranchCode,
                         CodEmpleado = c.CodEmpleado,
                         FullName = c.FullName,
                         Age = c.Age,
                         LicenseType = c.LicenseType,
                         LicenseNumber = c.LicenseNumber,
                         LicensePhoto = c.LicensePhoto,
                         NamePhoto = c.NamePhoto,
                         IsActive = c.IsActive,
                         CreatedBy = c.CreatedBy,
                         CreatedDate = c.CreatedDate,
                         UpdatedBy = c.UpdatedBy,
                         UpdatedDate = c.UpdatedDate,

                         // Nombre del piloto asociado
                         PilotFullName = p != null ? p.FullName : null
                     })
                    .ToList();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: /api/Copilot/getCopilotByBranchCode/{branchcode}
        [HttpGet("getCopilotByBranchCode/{branchcode}")]
        public ActionResult GetByBranch(string branchcode)
        {
            try
            {
                var lista =
                    (from c in _bd.Copilots
                     where c.IsActive == true && c.BranchCode == branchcode
                     join p in _bd.Pilots on c.PilotId equals p.PilotId into gp
                     from p in gp.DefaultIfEmpty() // LEFT JOIN
                     select new CopilotListCLS
                     {
                         CopilotsId = c.CopilotsId,
                         PilotId = c.PilotId,
                         BranchCode = c.BranchCode,
                         CodEmpleado = c.CodEmpleado,
                         FullName = c.FullName,
                         Age = c.Age,
                         LicenseType = c.LicenseType,
                         LicenseNumber = c.LicenseNumber,
                         LicensePhoto = c.LicensePhoto,
                         NamePhoto = c.NamePhoto,
                         IsActive = c.IsActive,
                         CreatedBy = c.CreatedBy,
                         CreatedDate = c.CreatedDate,
                         UpdatedBy = c.UpdatedBy,
                         UpdatedDate = c.UpdatedDate,

                         // Nombre del piloto asociado
                         PilotFullName = p != null ? p.FullName : null
                     })
                    .ToList();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: /api/Copilot/{idcopilot}
        [HttpGet("{idcopilot:int}")]
        public ActionResult Get(int idcopilot)
        {
            try
            {
                var copilot =
                    (from c in _bd.Copilots
                     where c.CopilotsId == idcopilot
                     join p in _bd.Pilots on c.PilotId equals p.PilotId into gp
                     from p in gp.DefaultIfEmpty() // LEFT JOIN
                     select new CopilotListCLS
                     {
                         CopilotsId = c.CopilotsId,
                         PilotId = c.PilotId,
                         BranchCode = c.BranchCode,
                         CodEmpleado = c.CodEmpleado,
                         FullName = c.FullName,
                         Age = c.Age,
                         LicenseType = c.LicenseType,
                         LicenseNumber = c.LicenseNumber,
                         LicensePhoto = c.LicensePhoto,
                         NamePhoto = c.NamePhoto,
                         IsActive = c.IsActive,
                         CreatedBy = c.CreatedBy,
                         CreatedDate = c.CreatedDate,
                         UpdatedBy = c.UpdatedBy,
                         UpdatedDate = c.UpdatedDate,

                         // Nombre del piloto asociado
                         PilotFullName = p != null ? p.FullName : null
                     })
                    .FirstOrDefault();

                if (copilot == null)
                    return NotFound();

                return Ok(copilot);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE lógico: /api/Copilot/{idcopilot}
        [HttpDelete("{idcopilot:int}")]
        public IActionResult Delete(int idcopilot)
        {
            try
            {
                var copilot = _bd.Copilots
                    .FirstOrDefault(c => c.CopilotsId == idcopilot && c.IsActive == true);

                if (copilot == null)
                    return NotFound();

                copilot.IsActive = false;
                copilot.UpdatedDate = DateTime.Now;
                // copilot.UpdatedBy = User?.Identity?.Name ?? copilot.UpdatedBy;

                _bd.SaveChanges();
                return Ok("Se eliminó correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: crear/actualizar (mixto)
        [HttpPost]
        public ActionResult Post([FromBody] CopilotFormAddCLS oCopilotFormAddCLS)
        {
            try
            {
                if (oCopilotFormAddCLS.CopilotsId == 0)
                {
                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        var oCopilot = new Copilots
                        {
                            PilotId = oCopilotFormAddCLS.PilotId,
                            BranchCode = oCopilotFormAddCLS.BranchCode,
                            CodEmpleado = oCopilotFormAddCLS.CodEmpleado,
                            FullName = oCopilotFormAddCLS.FullName,
                            Age = oCopilotFormAddCLS.Age,
                            LicenseType = oCopilotFormAddCLS.LicenseType,
                            LicenseNumber = oCopilotFormAddCLS.LicenseNumber,
                            LicensePhoto = oCopilotFormAddCLS.LicensePhoto,
                            NamePhoto = oCopilotFormAddCLS.NamePhoto,

                            IsActive = true,
                            CreatedBy = oCopilotFormAddCLS.CreatedBy,
                            CreatedDate = oCopilotFormAddCLS.CreatedDate == default ? DateTime.Now : oCopilotFormAddCLS.CreatedDate,
                            UpdatedBy = oCopilotFormAddCLS.UpdatedBy,
                            UpdatedDate = oCopilotFormAddCLS.UpdatedDate
                        };

                        _bd.Copilots.Add(oCopilot);
                        _bd.SaveChanges();
                        transaccion.Complete();
                    }

                    return Ok("Se guardó el Copiloto correctamente");
                }
                else
                {
                    var oCopilot = _bd.Copilots.Find(oCopilotFormAddCLS.CopilotsId);
                    if (oCopilot == null)
                        return NotFound();

                    oCopilot.PilotId = oCopilotFormAddCLS.PilotId;
                    oCopilot.BranchCode = oCopilotFormAddCLS.BranchCode;
                    oCopilot.CodEmpleado = oCopilotFormAddCLS.CodEmpleado;
                    oCopilot.FullName = oCopilotFormAddCLS.FullName;
                    oCopilot.Age = oCopilotFormAddCLS.Age;
                    oCopilot.LicenseType = oCopilotFormAddCLS.LicenseType;
                    oCopilot.LicenseNumber = oCopilotFormAddCLS.LicenseNumber;
                    oCopilot.LicensePhoto = oCopilotFormAddCLS.LicensePhoto;
                    oCopilot.NamePhoto = oCopilotFormAddCLS.NamePhoto;

                    // Mantiene/actualiza estado
                    oCopilot.IsActive = oCopilotFormAddCLS.IsActive;
                    oCopilot.UpdatedBy = string.IsNullOrWhiteSpace(oCopilotFormAddCLS.UpdatedBy) ? oCopilot.UpdatedBy : oCopilotFormAddCLS.UpdatedBy;
                    oCopilot.UpdatedDate = DateTime.Now;

                    _bd.SaveChanges();
                    return Ok("Se actualizó el Copiloto correctamente");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
