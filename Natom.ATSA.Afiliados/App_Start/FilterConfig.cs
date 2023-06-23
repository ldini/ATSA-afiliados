using System.Web;
using System.Web.Mvc;

namespace Natom.ATSA.Afiliados
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
