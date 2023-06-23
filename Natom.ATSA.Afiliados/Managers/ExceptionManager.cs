using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Natom.ATSA.Afiliados.Managers
{
    public class ExceptionManager
    {
        public static string GetMessage(Exception ex)
        {
            return ex.GetBaseException().Message;
        }
    }
}