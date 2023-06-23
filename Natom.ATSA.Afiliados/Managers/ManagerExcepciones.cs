using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados.Managers
{
    public class ManagerExcepciones
    {
        public static string Procesar(Exception ex)
        {
            string retorno = ex.Message;

            if (ex.InnerException != null)
            {
                retorno = retorno + Procesar(ex.InnerException);
            }

            return retorno;
        }
    }
}