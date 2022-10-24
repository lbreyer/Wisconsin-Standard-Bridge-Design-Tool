using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WisDot.Bos.StandardPlans.Data;

namespace WisDot.Bos.StandardPlans
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "RegisterRoutes Start", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Search", id = UrlParameter.Optional }
            );
            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "RegisterRoutes End", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");
        }
    }
}
