using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WisDot.Bos.StandardPlans.Data
{
    internal class DatabaseConnection
    {
        public string DatabaseName { get; set; }
        //public Constants.Database Database { get; set; }
        public string ProviderName { get; set; } // Oracle, SQL, Access, etc.
        public string ConnectionString { get; set; }
    }
}