using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Molit_IN.API.Data;
using Molit_IN.API.Models;
using Molit_IN.Library.Menu;
using Molit_IN.Library.Pilot;
using System.Transactions;

namespace Molit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2")]
    public class PilotController : ControllerBase
    {
        private readonly ApplicationDbContext _bd;
        public PilotController(ApplicationDbContext bd)
        {
            _bd = bd;
        }

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var lista = (from pilot in _bd.Pilots
                             where pilot.IsActive == true
                             select new PilotListCLS
                             {
                                 PilotId = pilot.PilotId,
                                 BranchCode = pilot.BranchCode,
                                 FullName = pilot.FullName,
                                 Age = pilot.Age,
                                 LicenseType = pilot.LicenseType,
                                 LicenseNumber = pilot.LicenseNumber,
                                 LicensePhoto = pilot.LicensePhoto,
                                 NamePhoto = pilot.NamePhoto

                             }).ToList();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("getPilotByBranchCode/{branchcode}")]
        public ActionResult Get(string branchcode)
        {
            try
            {
                var lista = (from pilot in _bd.Pilots
                             where pilot.IsActive == true
                             && pilot.BranchCode == branchcode
                             select new PilotListCLS
                             {
                                 PilotId = pilot.PilotId,
                                 BranchCode = pilot.BranchCode,
                                 FullName = pilot.FullName,
                                 Age = pilot.Age,
                                 LicenseType = pilot.LicenseType,
                                 LicenseNumber = pilot.LicenseNumber,
                                 LicensePhoto = pilot.LicensePhoto,
                                 NamePhoto = pilot.NamePhoto

                             }).ToList();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("{idpilot}")]
        public ActionResult Get(int idpilot)
        {
            try
            {
                var pilot = _bd.Pilots.Where(p => p.PilotId == idpilot).
                    Select(p => new PilotListCLS
                    {
                        PilotId = p.PilotId,
                        BranchCode = p.BranchCode,
                        FullName = p.FullName,
                        Age = p.Age,
                        LicenseType = p.LicenseType,
                        LicenseNumber = p.LicenseNumber,
                        LicensePhoto = p.LicensePhoto,
                        NamePhoto = p.NamePhoto
                    }).FirstOrDefault();
                if (pilot == null)
                {
                    return NotFound();
                }
                else
                    return Ok(pilot);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete("{idpilot}")]
        public IActionResult Delete(int idpilot)
        {
            try
            {
                var pilot = _bd.Pilots.Where(p => p.PilotId == idpilot && p.IsActive == true).
              FirstOrDefault();
                if (pilot == null)
                {
                    return NotFound();
                }
                pilot.IsActive = false;
                _bd.SaveChanges();
                return Ok("Se elimino correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        public ActionResult Post([FromBody] PilotFormAddCLS oPilotFormAddCLS)
        {
            try
            {
                if (oPilotFormAddCLS.PilotId == 0)
                {
                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        Pilots oPilot = new Pilots();
                        oPilot.BranchCode = oPilotFormAddCLS.BranchCode;
                        oPilot.FullName = oPilotFormAddCLS.FullName;
                        oPilot.Age = oPilotFormAddCLS.Age;
                        oPilot.LicenseType = oPilotFormAddCLS.LicenseType;
                        oPilot.LicenseNumber = oPilotFormAddCLS.LicenseNumber;
                        oPilot.LicensePhoto = oPilotFormAddCLS.LicensePhoto;
                        oPilot.NamePhoto = oPilotFormAddCLS.NamePhoto;
                        oPilot.IsActive = true;
                        _bd.Pilots.Add(oPilot);
                        _bd.SaveChanges();
                        transaccion.Complete();
                    }

                    return Ok("Se guardo el Piloto correctamente");
                }
                else
                {
                    Pilots oPilot = _bd.Pilots.Find(oPilotFormAddCLS.PilotId);
                    if (oPilot == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        oPilot.BranchCode = oPilotFormAddCLS.BranchCode;
                        oPilot.FullName = oPilotFormAddCLS.FullName;
                        oPilot.Age = oPilotFormAddCLS.Age;
                        oPilot.LicenseType = oPilotFormAddCLS.LicenseType;
                        oPilot.LicenseNumber = oPilotFormAddCLS.LicenseNumber;
                        oPilot.LicensePhoto = oPilotFormAddCLS.LicensePhoto;
                        oPilot.NamePhoto = oPilotFormAddCLS.NamePhoto;
                        _bd.SaveChanges();
                        return Ok("Se actualizó el Piloto correctamente");
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
