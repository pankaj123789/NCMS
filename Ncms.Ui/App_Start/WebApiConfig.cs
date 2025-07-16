using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using F1Solutions.Global.Common.Logging;

namespace Ncms.Ui
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.Services.Add(typeof(IExceptionLogger), new HttpUnhandledExceptionLogger { DefaultSource = "WebAPI" });
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
