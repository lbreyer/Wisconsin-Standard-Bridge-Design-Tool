using System;
using System.Web;
using System.Web.Mvc;
using WisDot.Bos.StandardPlans.Data;

namespace WisDot.Bos.StandardPlans
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "RegisterGlobalFilters Start", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");
            filters.Add(new HandleErrorAttribute());
            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "RegisterGlobalFilters End", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");
        }
    }
}
