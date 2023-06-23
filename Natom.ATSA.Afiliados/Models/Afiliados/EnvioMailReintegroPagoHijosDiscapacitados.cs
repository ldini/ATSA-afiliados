using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Natom.ATSA.Afiliados.Models.Discapacitados
{
    public class EnvioMailReintegroPagoHijosDiscapacitados
    {
        [Key]
        public int EnvioMailReintegroPagoHijosDiscapacitadosId { get; set; }

        public int ReintegroPagoHijosDiscapacitadosId { get; set; }
        public ReintegroPagoHijosDiscapacitados ReintegroPagoHijosDiscapacitados { get; set; }

        public string Destinatario { get; set; }
        public DateTime FechaHora { get; set; }

        public int? EnviadoPorUsuarioId { get; set; }
        [ForeignKey("EnviadoPorUsuarioId")]
        public Usuario EnviadoPorUsuario { get; set; }
    }
}