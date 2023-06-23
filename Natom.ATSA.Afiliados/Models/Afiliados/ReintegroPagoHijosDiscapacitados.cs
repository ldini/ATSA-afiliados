using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados.Models.Discapacitados
{
    public class ReintegroPagoHijosDiscapacitados
    {
        [Key]
        public int ReintegroPagoHijosDiscapacitadosId { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal Total { get; set; }
        public string CerradoPor { get; set; }
        public DateTime? FechaHoraCerrado { get; set; }
        public string ConfirmaTransferencia { get; set; }
        public DateTime? FechaHoraConfirmaTransferencia { get; set; }
        public bool Anulado { get; set; }
        public bool Automatica { get; set; }

        public List<ItemReintegroPagoHijosDiscapacitados> Items { get; set; }
        public List<EnvioMailReintegroPagoHijosDiscapacitados> Emails { get; set; }
    }
}