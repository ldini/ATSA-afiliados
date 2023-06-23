using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados.Models.ViewModels
{
    public class ObtenerLiquidacionesResult
    {
        public long Numero { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public DateTime FechaHora { get; set; }
        public DateTime FechaHoraUltimaGrabacion { get; set; }
        public DateTime? FechaHoraConfirmacion { get; set; }
        public long EstablecimientoId { get; set; }
        public long LiquidacionId { get; set; }
    }
}