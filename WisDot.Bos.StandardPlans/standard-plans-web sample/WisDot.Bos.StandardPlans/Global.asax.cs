using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WisDot.Bos.StandardPlans.Data;

namespace WisDot.Bos.StandardPlans
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "Application_Start Start", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

           //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "Application_Start End", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");
        }
    }
}
