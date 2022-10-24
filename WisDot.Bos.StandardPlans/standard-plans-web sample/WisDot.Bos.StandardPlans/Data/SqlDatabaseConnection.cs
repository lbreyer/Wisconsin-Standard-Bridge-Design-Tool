using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace WisDot.Bos.StandardPlans.Data
{
    internal class SqlDatabaseConnection : DatabaseConnection
    {
        public SqlConnection SqlConnection { get; set; }
    }
}