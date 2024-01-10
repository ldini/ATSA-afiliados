using Natom.ATSA.Afiliados.Managers;
using Natom.ATSA.Afiliados.Models;
using Natom.ATSA.Afiliados.Models.ViewModels;
using Natom.ATSA.Afiliados.Reporting;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Natom.ATSA.Afiliados.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Usuario = new UsuarioManager().ObtenerUsuario(this.SesionUsuarioId.Value);
            //ViewBag.Establecimiento = new EstablecimientoManager().ObtenerEstablecimiento(((Usuario)ViewBag.Usuario).EstablecimientoId);

            return View();
        }

        [HttpPost]
        public ActionResult GetAfiliado(string DNI, string Numero)
        {
            try
            {
                List<Persona> datos = new List<Persona>();
                var afiliadoManager = new AfiliadoManager();
                var afiliado = afiliadoManager.ObtenerAfiliado(DNI, Numero, SesionUsuarioId ?? 0);
                var establecimiento = afiliado != null && afiliado.ESTABLECIMIENTO_ID.HasValue ? afiliadoManager.ObtenerEstablecimiento(afiliado.ESTABLECIMIENTO_ID.Value) : new Establecimiento();
                var familiares = afiliado != null ? afiliadoManager.ObtenerFamiliares(afiliado.ID) : new List<Persona>();

                if (afiliado == null)
                {
                    return Json(new
                    {
                        success = true,
                        encontrado = false
                    });
                }
                else
                    return Json(new
                    {
                        success = true,
                        encontrado = true,
                        datos = new
                        {
                            id = afiliado.ID,
                            numeroAfiliado = afiliado.NUMERO_AFILIADO,
                            nombre = afiliado.NOMBRES,
                            apellido = afiliado.APELLIDOS,
                            codpostal = afiliado.CODIGO_POSTAL,
                            dni = afiliado.DOCUMENTO,
                            cuil = afiliado.CUIL,
                            localidad = afiliado.LOCALIDAD,
                            profesion = afiliado.PROFESION,
                            sexo = afiliado.SEXO,
                            telefono = afiliado.TELEFONO,
                            domicilio = afiliado.DOMICILIO,
                            estado_civil = afiliado.ESTADO_CIVIL,
                            email = afiliado.EMAIL,
                            estado = afiliado.ESTADO_ID == 2 ? "Afiliado" : "Jubilado",
                            establecimiento = establecimiento.Nombre,
                            edad = afiliado.FECHA_NACIMIENTO.HasValue ? Convert.ToInt32((DateTime.Now - afiliado.FECHA_NACIMIENTO.Value).TotalDays / 365).ToString() : "",
                            fechaNacimiento = afiliado.FECHA_NACIMIENTO.HasValue ? afiliado.FECHA_NACIMIENTO.Value.ToString("dd/MM/yyyy") : "N / A",
                            familiares = from f in familiares
                                         select new
                                         {
                                             Nombre = f.NOMBRES,
                                             Apellido = f.APELLIDOS,
                                             DNI = f.DOCUMENTO
                                         }
                        }
                    });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public ActionResult Login(string error = "")
        {
            ViewBag.ErrorMessage = error;
            return View();
        }

        //[HttpPost]
        //public ActionResult Grabar(int id)
        //{
        //    try
        //    {
        //        return Json(new { id = 1 });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, error = ex.Message });
        //    }
        //}

        [HttpPost]
        public ActionResult Grabar(string afiliado_id, string afiliado_nombre, string afiliado_apellido, string cupon_cantidad)
        {
            try
            {
                // Crear una instancia del contexto de base de datos
                using (var context = new DbAfiliadosContext())
                {
                    // Crear una entidad de Cupon y establecer sus propiedades con los datos recibidos
                    var cupon = new Cupones
                    {
                        //CuponId = id,
                        CuponAfiliadoNro = afiliado_id,
                        CuponAfiliadoNombre = afiliado_nombre,
                        CuponAfiliadoApellido = afiliado_apellido,
                        CuponCantidad = cupon_cantidad,
                        CuponOrdenNro = "",
                        CuponFechaGeneracion = DateTime.Now.ToString(),
                        CuponCodigoPrestacion = "",
                        CuponCodigoPrestador = ""
                    };

                    // Agregar la entidad al contexto
                    context.Cupones.Add(cupon);

                    // Guardar los cambios en la base de datos
                    context.SaveChanges();

                    // Puedes devolver algún tipo de respuesta al cliente si es necesario
                    return Json(new { success = true, id = cupon.CuponId });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult ImprimirCupon(int id, string tipo,string cantidad,string nroOrden)
        {
            var afiliadoManager = new AfiliadoManager();
            byte[] b = afiliadoManager.GenerarImprimirReciboPDFEnBytes(id,tipo,cantidad,nroOrden);
            //return File(b, "application/pdf");

            if (b != null)
            {
                Response.ClearContent();
                Response.AddHeader("Content-Length", b.Length.ToString());
                Response.AddHeader("Content-Type", "application/pdf");
                Response.BinaryWrite(b);
                Response.Flush();
                return null;
            }
            //NO APLICAR Response.End ni base.File PORQUE ROMPE TODO.
            return null;
            
        }


        [HttpPost]
        public ActionResult Login(LoginView data)
        {
            try
            {
                SesionManager mgr = new SesionManager();
                int usuarioId;
                mgr.ValidarLogin(data.Usuario, data.Clave, out usuarioId);
                HttpCookie myCookie = new HttpCookie("ATSAConafWebApp");
                myCookie.Value = usuarioId.ToString();
                Response.Cookies.Add(myCookie);
                Response.Redirect("/cupones/Home/Index");
                Response.End();
                return null;
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Home", new { @error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpCookie cookie = Request.Cookies["ATSAConafWebApp"];
            Response.Cookies.Remove("ATSAConafWebApp");
            var aCookie = new HttpCookie("ATSAConafWebApp") { Expires = DateTime.Now.AddDays(-1) };
            Response.Cookies.Add(aCookie);
            Response.Redirect("/cupones/Home/Login");
            Response.End();
            return null;
        }
    }
}