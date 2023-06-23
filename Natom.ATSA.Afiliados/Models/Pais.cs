using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados.Models
{
    [Table("pais")]
    public class Pais
    {
        [Key]
        public long ID { get; set; }
        public string DESCRIPCION { get; set; }
    }
}