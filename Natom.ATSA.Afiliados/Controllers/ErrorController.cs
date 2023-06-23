using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Natom.ATSA.Afiliados.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult NoTienePermiso()
        {
            return View();
        }
    }
}