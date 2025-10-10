using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Molit_IN.API.Data;
using Molit_IN.API.Functions;
using Molit_IN.API.Models;
using Molit_IN.Library.User;
using System.Transactions;

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


        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                // 1. Primero traer datos simples desde la base
                var usuariosBD = (from usuario in _bd.Users
                                  where usuario.IsActive == true
                                  select new
                                  {
                                      usuario.IdUser,
                                      usuario.UserName,
                                      usuario.FullName,
                                      usuario.Email,
                                      usuario.RoleId,
                                      usuario.CardName,
                                      usuario.CardCode,
                                      usuario.TypeUser
                                  }).ToList(); // <- ejecuta en SQL aquí, SIN TryParse todavía

                // 2. Ahora en memoria, mapear al modelo UserListCLS
                var lista = usuariosBD.Select(usuario => new UserListCLS
                {
                    IdUser = usuario.IdUser,
                    UserName = usuario.UserName,
                    FullName = usuario.FullName,
                    Email = usuario.Email,
                    RoleId = usuario.RoleId,
                    CardCode = usuario.CardCode,
                    CardName = usuario.CardName,
                    TypeUser = Enum.TryParse<UserListCLS.UserType>(usuario.TypeUser, out var tipoUsuario) ? tipoUsuario : UserListCLS.UserType.Int
                }).ToList();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{idusuario}")]
        public ActionResult Get(int idusuario)
        {
            try
            {
                // 1. Primero traer datos simples desde la base
                var usuariosBD = (from usuario in _bd.Users
                                  where usuario.IsActive == true
                                  && usuario.IdUser == idusuario
                                  select new
                                  {
                                      usuario.IdUser,
                                      usuario.UserName,
                                      usuario.FullName,
                                      usuario.Email,
                                      usuario.RoleId,
                                      usuario.CardName,
                                      usuario.CardCode,
                                      usuario.TypeUser
                                  }).ToList(); // <- ejecuta en SQL aquí, SIN TryParse todavía

                // 2. Ahora en memoria, mapear al modelo UserListCLS
                var lista = usuariosBD.Select(usuario => new UserListCLS
                {
                    IdUser = usuario.IdUser,
                    UserName = usuario.UserName,
                    FullName = usuario.FullName,
                    Email = usuario.Email,
                    RoleId = usuario.RoleId,
                    CardCode = usuario.CardCode,
                    CardName = usuario.CardName,
                    TypeUser = Enum.TryParse<UserListCLS.UserType>(usuario.TypeUser, out var tipoUsuario) ? tipoUsuario : UserListCLS.UserType.Int
                }).FirstOrDefault();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //[HttpGet("{idusuario}")]
        //public ActionResult Get(int idusuario)
        //{
        //    try
        //    {
        //        var usuario = _bd.User.Where(p => p.IdUser == idusuario).
        //            Select(p => new UserListCLS
        //            {
        //                IdUser = p.IdUser,
        //                UserName = p.UserName,
        //                FullName = p.FullName,
        //                Email = p.Email,
        //                RoleId = p.RoleId,
        //                IdSupplier = p.IdSupplier,
        //                TypeUser = p.TypeUser
        //            }).
        //            FirstOrDefault();
        //        if (usuario == null)
        //        {
        //            return NotFound();
        //        }
        //        else
        //            return Ok(usuario);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }

        //}

        [HttpDelete("{idusuario}")]
        public IActionResult Delete(int idusuario)
        {
            try
            {
                var carrera = _bd.Users.Where(p => p.IdUser == idusuario && p.IsActive == true).
              FirstOrDefault();
                if (carrera == null)
                {
                    return NotFound();
                }
                carrera.IsActive = false;
                _bd.SaveChanges();
                return Ok("Se elimino el usuario correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        public ActionResult Post([FromBody] UserFormAddCLS oUsuarioFormAddCLS)
        {
            try
            {
                if (oUsuarioFormAddCLS.IdUser == 0)
                {
                    string clavecifrada = Cifrar.cifrarCadena(oUsuarioFormAddCLS.Password);
                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        User oUser = new User();
                        oUser.UserName = oUsuarioFormAddCLS.UserName;
                        oUser.Password = clavecifrada;
                        oUser.FullName = oUsuarioFormAddCLS.FullName;
                        oUser.RoleId = oUsuarioFormAddCLS.RoleId;
                        oUser.TypeUser = oUsuarioFormAddCLS.TypeUser.ToString();
                        //oUser.AlmacenDefaul = oUsuarioFormAddCLS.AlmacenDefaul;
                        oUser.Email = oUsuarioFormAddCLS.Email;
                        oUser.CardCode = oUsuarioFormAddCLS.CardCode;
                        oUser.CardName = oUsuarioFormAddCLS.CardName;
                        oUser.IsActive = true;
                        _bd.Users.Add(oUser);
                        _bd.SaveChanges();
                        _bd.SaveChanges();
                        transaccion.Complete();
                    }
                    return Ok("Se guardo el usuario correctamente");
                }
                else
                {
                    User oUser = _bd.Users.Find(oUsuarioFormAddCLS.IdUser);
                    if (oUser == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        oUser.FullName = oUsuarioFormAddCLS.FullName;
                        oUser.Email = oUsuarioFormAddCLS.Email;
                        oUser.RoleId = oUsuarioFormAddCLS.RoleId;
                        oUser.CardCode = oUsuarioFormAddCLS.CardCode;
                        oUser.CardName = oUsuarioFormAddCLS.CardName;
                        oUser.TypeUser = oUsuarioFormAddCLS.TypeUser.ToString();
                        _bd.SaveChanges();
                        return Ok("Se actualizó la carrera correctamente");
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
