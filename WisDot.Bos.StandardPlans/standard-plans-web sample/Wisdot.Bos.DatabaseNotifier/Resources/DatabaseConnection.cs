using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.DatabaseNotifier.Resources
{
    internal class DatabaseConnection
    {
        public string DatabaseName { get; set; }
        //public Constants.Database Database { get; set; }
        public string ProviderName { get; set; } // Oracle, SQL, Access, etc.
        public string ConnectionString { get; set; }
    }
}
