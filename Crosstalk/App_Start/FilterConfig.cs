using System.Web;
using System.Web.Mvc;
using Crosstalk.Handlers;

namespace Crosstalk
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}