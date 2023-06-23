using Natom.ATSA.Afiliados.Managers;
using Natom.ATSA.Afiliados.Models.Afiliados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Natom.ATSA.Afiliados.Controllers
{
    public class BaseController : Controller
    {
        public int? SesionUsuarioId
        {
            get
            {
                HttpCookie cookie = Request.Cookies["ATSAConafWebApp"];
                if (cookie == null)
                {
                    return null;
                }
                else
                {
                    return Convert.ToInt32(cookie.Value);
                }
            }
        }
    }
}