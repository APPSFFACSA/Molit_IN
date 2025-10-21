using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Molit_IN.API.Data;
using Molit_IN.API.Models;
using Molit_IN.Library.Menu;
using Molit_IN.Library.VehicleType;
using System.Transactions;

namespace Molit.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1, 2, 3")]
    public class VehicleTypeController : ControllerBase
    {
        private readonly ApplicationDbContext _bd;
        public VehicleTypeController(ApplicationDbContext bd)
        {
            _bd = bd;
        }

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var lista = (from tipovehiculo in _bd.VehicleType
                             where tipovehiculo.IsActive == true
                             select new VehicleListCLS
                             {
                                 IdVehicleType = tipovehiculo.IdVehicleType,
                                 DscriptionVehicle = tipovehiculo.DscriptionVehicle,
                                 ItemCode = tipovehiculo.ItemCode

                             }).ToList();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("{idtipovehiculo}")]
        public ActionResult Get(int idtipovehiculo)
        {
            try
            {
                var tipovehiculo = _bd.VehicleType.Where(p => p.IdVehicleType == idtipovehiculo).
                    Select(p => new VehicleListCLS
                    {
                        IdVehicleType = p.IdVehicleType,
                        DscriptionVehicle = p.DscriptionVehicle,
                        ItemCode = p.ItemCode

                    }).
                    FirstOrDefault();
                if (tipovehiculo == null)
                {
                    return NotFound();
                }
                else
                    return Ok(tipovehiculo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete("{idtipovehiculo}")]
        public IActionResult Delete(int idtipovehiculo)
        {
            try
            {
                var tipovehiculo = _bd.VehicleType.Where(p => p.IdVehicleType == idtipovehiculo && p.IsActive == true).
              FirstOrDefault();
                if (tipovehiculo == null)
                {
                    return NotFound();
                }
                tipovehiculo.IsActive = false;
                _bd.SaveChanges();
                return Ok("Se elimino correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]

        public ActionResult Post([FromBody] VehicleFormAddCLS oVehicleFormAddCLS)
        {
            try
            {
                if (oVehicleFormAddCLS.IdVehicleType == 0)
                {
                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        VehicleType oVehicleType = new VehicleType();
                        oVehicleType.DscriptionVehicle = oVehicleFormAddCLS.DscriptionVehicle;
                        oVehicleType.ItemCode = oVehicleFormAddCLS.ItemCode;
                        oVehicleType.IsActive = true;
                        _bd.VehicleType.Add(oVehicleType);
                        _bd.SaveChanges();
                        transaccion.Complete();
                    }

                    return Ok("Se guardo correctamente");
                }
                else
                {
                    VehicleType oVehicleType = _bd.VehicleType.Find(oVehicleFormAddCLS.IdVehicleType);
                    if (oVehicleType == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        oVehicleType.DscriptionVehicle = oVehicleFormAddCLS.DscriptionVehicle;
                        oVehicleType.ItemCode = oVehicleFormAddCLS.ItemCode;
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
