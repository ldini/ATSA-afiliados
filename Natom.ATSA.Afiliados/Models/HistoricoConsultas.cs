using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados.Models
{
    public class HistoricoConsultas
    {
        public long HistoricoConsultasId { get; set; }
        public int UsuarioId { get; set; }
        public string NumeroAfiliadoConsultado { get; set; }
        public DateTime FechaHora { get; set; }
    }
}