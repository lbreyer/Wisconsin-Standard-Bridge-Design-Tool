using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oracle.DataAccess.Client;

namespace WisDot.Bos.StandardPlans.Data
{
    internal class OracleDatabaseConnection : DatabaseConnection
    {
        public OracleConnection OracleConnection { get; set; }
    }
}