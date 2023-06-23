using Natom.ATSA.Afiliados.Models.Afiliados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Clave { get; set; }

        public DateTime FechaHoraAlta { get; set; }
        public DateTime? FechaHoraBaja { get; set; }
        public string Token { get; set; }
        
        public bool EsAdmin { get; set; }

        [NotMapped]
        public string NombreApellido
        {
            get
            {
                return String.Concat(this.Nombre, " ", this.Apellido);
            }
        }
    }
}