using System;
using System.Web;
using System.Web.Mvc;

namespace r3mus
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            try
            {
                filters.Add(new ClacksOverheadHeaderFilter());
            }
            catch (Exception ex) { }
        }
    }
    public class ClacksOverheadHeaderFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Headers.Add("X-Clacks-Overhead", "GNU Terry Pratchett");
        }
    } 
}
