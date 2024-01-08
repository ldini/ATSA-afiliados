using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados.Models.ViewModels
{
    public class CuponesResult
    {
        public int CuponId { get; set; }
        public string CuponAfiliadoNombre { get; set; }
        public string CuponAfiliadoApellido { get; set; }
        public string CuponAfiliadoNro { get; set; }
        public string CuponOrdenNro { get; set; }
        public string CuponFechaGeneracion { get; set; }
        public string CuponCodigoPrestacion { get; set; }
        public string CuponCodigoPrestador { get; set; }
        public string CuponCantidad { get; set; }
    }
}