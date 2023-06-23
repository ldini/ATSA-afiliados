using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados.Models.ViewModels
{
    public class ListarReintegrosResult
    {
        public int ReintegroPagoHijosDiscapacitadosId { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public bool Anulado { get; set; }
        public bool TransferenciaConfirmada { get; set; }
        public bool LiquidacionCerrada { get; set; }
    }
}