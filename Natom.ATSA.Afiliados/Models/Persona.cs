using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados.Models
{
    [Table("persona")]
    public class Persona
    {
        [Key]
        public long ID { get; set; }
        public bool? ACTIVO { get; set; }
        public string APELLIDOS { get; set; }
        public string CODIGO_POSTAL { get; set; }
        public string CUIL { get; set; }
        public string DOCUMENTO { get; set; }
        public string DOCUMENTO_TIPO { get; set; }
        public DateTime? FECHA_AFILIACION { get; set; }
        public DateTime? FECHA_INGRESO { get; set; }
        public DateTime? FECHA_NACIMIENTO { get; set; }
        public string LOCALIDAD { get; set; }
        public string NOMBRES { get; set; }
        public string PROFESION { get; set; }
        public string SEXO { get; set; }
        public string TELEFONO { get; set; }
        public string TIPO_PERSONA { get; set; }

        public long? ESTADO_ID { get; set; }
        //[ForeignKey("ESTADO_ID")]
        //public Estado Estado { get; set; }

        public long? ESTABLECIMIENTO_ID { get; set; }

        public long? NACIONALIDAD { get; set; }
        [ForeignKey("NACIONALIDAD")]
        public Pais Pais { get; set; }

        public string NUMERO_AFILIADO { get; set; }
        public string DOMICILIO { get; set; }

        public string EMAIL { get; set; }

        public DateTime? FECHA_BAJA { get; set; }
        public string ESTADO_CIVIL { get; set; }
    }
}