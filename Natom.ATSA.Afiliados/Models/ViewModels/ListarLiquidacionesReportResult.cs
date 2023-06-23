using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados.Models.ViewModels
{
    public class ListarLiquidacionesReportResult
    {
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal Total { get; set; }
    }
}
