using System.Web.Routing;
using System.Web.Http;

namespace Tor
{
    public class RouteConfig
    {
        /// <summary>
        /// routing
        /// </summary>
        public static void RegisterRoutes()
        {
            
            RouteTable.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new
                {
                    id = System.Web.Http.RouteParameter.Optional,
                    action = System.Web.Http.RouteParameter.Optional
                });

            RouteTable.Routes.MapHttpRoute(
                name: "DefaultApi2",
                routeTemplate: "{controller}/{action}/{id}/{id2}",
                defaults: new
                {
                    id = System.Web.Http.RouteParameter.Optional,
                    action = System.Web.Http.RouteParameter.Optional
                });
        }
       
    }
}
