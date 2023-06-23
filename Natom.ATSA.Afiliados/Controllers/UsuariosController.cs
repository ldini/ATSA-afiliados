using Natom.ATSA.Afiliados.Managers;
using Natom.ATSA.Afiliados.Models;
using Natom.ATSA.Afiliados.Models.DataTable;
using Natom.ATSA.Afiliados.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Natom.ATSA.Afiliados.Controllers
{
    public class UsuariosController : BaseController
    {
        private UsuarioManager manager = new UsuarioManager();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            return View(this.manager.ObtenerUsuario(id));
        }

        [HttpPost]
        public ActionResult Grabar(Usuario usuario)
        {
            try
            {
                this.manager.Grabar(usuario, this.SesionUsuarioId.Value);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public ActionResult RecuperoDeClave(string u1du_22m2dl)
        {
            Usuario e = this.manager.ObtenerUsuarioPorToken(u1du_22m2dl);
            LoginView data = new LoginView();
            data.UsuarioId = e.UsuarioId;
            data.Usuario = e.Email;
            data.Clave = "";
            return View(data);
        }

        [HttpPost]
        public ActionResult RecuperoDeClave(LoginView loginData)
        {
            this.manager.GrabarPassword(loginData.UsuarioId, loginData.Clave);
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public ActionResult EnviarMailRecupero(string email)
        {
            try
            {
                this.manager.EnviarMailRecuperoDeClave(email);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public ActionResult Eliminar(int id)
        {
            try
            {
                this.manager.Eliminar(id, this.SesionUsuarioId.Value);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public ActionResult ObtenerListadoIndex(JQueryDataTableParamModel param)
        {
            DataTableParams dtParams = new DataTableParams(Request);
            int cantidadRegistros = this.manager.ObtenerCantidadUsuarios();

            IEnumerable<Usuario> cargosFiltrados = this.manager.ObtenerUsuariosConFiltros(dtParams.Search);

            Func<Usuario, string> orderingFunction =
                (c => dtParams.SortByColumnIndex == 0 ? c.Nombre :
                    dtParams.SortByColumnIndex == 1 ? c.Apellido :
                    dtParams.SortByColumnIndex == 2 ? c.Email :
                "");

            if (dtParams.SortingDirection == eSortingDirection.ASC)
            {
                cargosFiltrados = cargosFiltrados.OrderBy(orderingFunction);
            }
            else
            {
                cargosFiltrados = cargosFiltrados.OrderByDescending(orderingFunction);
            }

            List<Usuario> displayedCargos = cargosFiltrados
                .Skip(param.start).Take(param.length).ToList();


            var result = from c in displayedCargos
                         select new object[] {
                             c.Nombre,
                             c.Apellido,
                             c.Email,
                             c.UsuarioId
                            };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = cantidadRegistros,
                iTotalDisplayRecords = cargosFiltrados.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);

        }
    }
}