using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados.Models
{
    [Table("establecimiento")]
    public class Establecimiento
    {
        [Key]
        public long ID { get; set; }
        public string Nombre { get; set; }
    }
}