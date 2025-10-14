using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Molit_IN.API.Data;
using Molit_IN.Library.Branch;
using Sap.Data.Hana;
using System.Data;

namespace Molit_IN.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2")]
    public class StoreController : ControllerBase
    {
        private readonly ApplicationDbContext _bd;   // Por si luego persistes algo en SQL Server
        private readonly IDbConnection _connection;  // HANA inyectado

        // DESARROLLO | cambia por el schema de producción cuando corresponda
        private readonly string bdHana = "P4_GT_FFACSA";
        // private readonly string bdHana = "SBO_GT_FFACSA"; // PRODUCCIÓN

        public StoreController(ApplicationDbContext bd, IDbConnection connection)
        {
            _bd = bd;
            _connection = connection;
        }

        /// <summary>Lista de tiendas (SAP HANA).</summary>
        [HttpGet("tiendas")]
        public IActionResult GetTiendas()
        {
            var result = new List<TiendasCLS>();
            try
            {
                string sql = $@"
                    select ""Code"",
                    (COALESCE(""Code"", '') || ' - ' || COALESCE(""Name"", '')) as ""Name"",
                    ""U_Direccion"", ""U_Departamento"", ""U_Municipio""
                    from {bdHana}.""@TIENDAS""
                    where ""U_CodContado"" is not null
                    order by ""Code"" asc";

                using var cmd = new HanaCommand(sql, (HanaConnection)_connection);
                _connection.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new TiendasCLS
                    {
                        Code = reader["Code"]?.ToString(),
                        Name = reader["Name"]?.ToString(),
                        U_Direccion = reader["U_Direccion"]?.ToString(),
                        U_Departamento = reader["U_Departamento"]?.ToString(),
                        U_Municipio = reader["U_Municipio"]?.ToString()
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                    _connection.Close();
            }
        }

        /// <summary>Detalle de tienda por código (SAP HANA).</summary>
        [HttpGet("tiendas/{code}")]
        public IActionResult GetTiendaByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest("El código es requerido.");

            TiendasCLS? tienda = null;

            try
            {
                string sql = $@"
            select ""Code"", ""Name"", ""U_Direccion"", ""U_Departamento"", ""U_Municipio""
            from {bdHana}.""@TIENDAS""
            where ""U_CodContado"" is not null
              and ""Code"" = :p_code
            limit 1";

                using var cmd = new HanaCommand(sql, (HanaConnection)_connection);

                cmd.Parameters.Add(new HanaParameter
                {
                    ParameterName = "p_code",
                    Value = code.Trim()
                });

                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    tienda = new TiendasCLS
                    {
                        Code = reader["Code"]?.ToString(),
                        Name = reader["Name"]?.ToString(),
                        U_Direccion = reader["U_Direccion"]?.ToString(),
                        U_Departamento = reader["U_Departamento"]?.ToString(),
                        U_Municipio = reader["U_Municipio"]?.ToString()
                    };
                }

                return tienda is null ? NotFound("No se encontró la tienda.") : Ok(tienda);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error consultando HANA: {ex.Message}");
            }
            finally
            {
                if (_connection.State != ConnectionState.Closed)
                    _connection.Close();
            }
        }

    }
}
