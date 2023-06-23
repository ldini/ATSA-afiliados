using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados.Models.ViewModels
{
    public class LoginView
    {
        public int UsuarioId { get; set; }
        public string Usuario { get; set; }
        public string Clave { get; set; }
    }
}