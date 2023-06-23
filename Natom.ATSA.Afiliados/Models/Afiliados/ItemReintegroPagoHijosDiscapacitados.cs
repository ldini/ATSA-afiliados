using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Natom.ATSA.Afiliados.Models.Discapacitados
{
    public class ItemReintegroPagoHijosDiscapacitados
    {
        [Key]
        public int ItemReintegroPagoHijosDiscapacitadosId { get; set; }

        public int ReintegroPagoHijosDiscapacitadosId { get; set; }
        public ReintegroPagoHijosDiscapacitados ReintegroPagoHijosDiscapacitados { get; set; }

        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal Monto { get; set; }
        public string CBU { get; set; }

        public int? TipoDoc { get; set; }
        public string CUIL { get; set; }

        public string Email { get; set; }

        public int BancoId { get; set; }
        public Banco Banco { get; set; }

        public int? TipoCuentaBancariaId { get; set; }
        public TipoCuentaBancaria TipoCuentaBancaria { get; set; }

        public int AfiliadoId { get; set; }
        public string Afiliado { get; set; }
        
        public string Observaciones { get; set; }

        public bool? TransferidoSinErrores { get; set; }

        public int? TransferidoSinErroresUsuarioId { get; set; }
        [ForeignKey("TransferidoSinErroresUsuarioId")]
        public Usuario TransferidoSinErroresUsuario { get; set; }

        public DateTime? FechaHoraTransferidoSinErrores { get; set; }
        public string DetalleError { get; set; }
    }
}