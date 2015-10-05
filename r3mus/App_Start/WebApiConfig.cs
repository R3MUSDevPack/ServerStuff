using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace r3mus
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
			config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "SlackRegistration",
                routeTemplate: "api/SlackRegistry/{group}/{emailAddress}/{token}",
                defaults: new { controller = "ExternalServices", action = "RegisterForSlack", group = UrlParameter.Optional, emailAddress = UrlParameter.Optional, token = UrlParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
