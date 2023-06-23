using Microsoft.Reporting.WebForms;
using Natom.ATSA.Afiliados.Managers;
using Natom.ATSA.Afiliados.Models.DataTable;
using Natom.ATSA.Afiliados.Models.Afiliados;
using Natom.ATSA.Afiliados.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Natom.ATSA.Afiliados.Controllers
{
    public class LiquidacionController : BaseController
    {
        private LiquidacionManager manager = new LiquidacionManager();

        public ActionResult Index()
        {
            return View();
        }

    }
}