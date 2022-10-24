using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace WisDot.Bos.DatabaseMonitor.Resources
{
    internal class SqlDatabaseConnection : DatabaseConnection
    {
        public SqlConnection SqlConnection { get; set; }
    }
}
