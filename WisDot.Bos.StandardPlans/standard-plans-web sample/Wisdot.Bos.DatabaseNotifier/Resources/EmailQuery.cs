using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;

namespace Wisdot.Bos.DatabaseNotifier.Resources
{
    class EmailQuery
    {
        private static readonly SqlDatabaseConnection sqlDatabaseConnection;
        private static readonly string standardBridgeDesignDatabaseName = "StandardBridgeDesign";
        private static readonly List<string> databaseNames = new List<string>() { standardBridgeDesignDatabaseName };
        private static readonly List<DatabaseConnection> databaseConnections;



        static EmailQuery()
        {
            databaseConnections = new List<DatabaseConnection>();
            databaseConnections = CreateDatabaseConnections();
            sqlDatabaseConnection = (SqlDatabaseConnection)RequestDatabaseConnection(standardBridgeDesignDatabaseName);
        }

        public static DataTable GetTopDownload()
        {
            string query =
                @"
                    select top 1 *
                    from DownloadHistory
                    order by DownloadId desc
                ";

            DataTable dt = ExecuteSelectSql(query, sqlDatabaseConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt;
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetUser(string user)
        {
            string query =
                @"
                    select *
                    from Users
                    where WamsId = @WamsId
                ";

            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@WamsId", SqlDbType.VarChar);
            prms[0].Value = user;

            DataTable dt = ExecuteSelectSql(query, sqlDatabaseConnection, prms);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt;
            }
            else
            {
                return null;
            }
        }

        public static int CountRows()
        {
            string query =
                @"
                    select count(*)
                    from DownloadHistory
                ";
            DataTable dt = ExecuteSelectSql(query, sqlDatabaseConnection);
            return (int)dt.Rows[0].ItemArray[0];
        }

        public static void EmailSent(int downloadId)
        {
            string query =
                @"
                    update DownloadHistory
                    set NotificationSent = 1
                    where DownloadId = @downloadId
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@downloadId", SqlDbType.Int);
            prms[0].Value = downloadId;
            ExecuteInsertUpdateDeleteSql(query, sqlDatabaseConnection, prms);
        }

        public static DatabaseConnection RequestDatabaseConnection(string databaseName)
        {
            if (databaseConnections.Where(el => el.DatabaseName.Equals(databaseName, StringComparison.CurrentCultureIgnoreCase)).Count() > 0)
            {
                var dbConn = databaseConnections.Where(el => el.DatabaseName.Equals(databaseName, StringComparison.CurrentCultureIgnoreCase)).First();
                return dbConn;
            }
            return null;
        }

        private static List<DatabaseConnection> CreateDatabaseConnections()
        {
            var connections = new List<DatabaseConnection>();
            var connectionString = "Data Source=4pyjJD/MxKpcjNAYayRldAZt8F2LBTeF; Database=StandardBridgeDesign; User Id=dot1sama; Password=vY5kliFPkynFTY+dcwrNlA==";
            var databaseName = "StandardBridgeDesign";
            if (!String.IsNullOrEmpty(connectionString) && databaseNames.Any(el => el.Equals(databaseName, StringComparison.CurrentCultureIgnoreCase)))
            {
                //SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(css.ConnectionString);
                SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);
                if (!String.IsNullOrEmpty(csb.Password))
                {
                    string decryptedPassword = CryptorEngine.Decrypt(csb.Password, true);
                    connectionString = connectionString.Replace(csb.Password, decryptedPassword);

                    if (databaseName == "StandardBridgeDesign")
                    {
                        string decryptedDataSource = CryptorEngine.Decrypt(csb.DataSource, true);
                        connectionString = connectionString.Replace(csb.DataSource, decryptedDataSource);
                    }

                    var sqlConn = new SqlDatabaseConnection();
                    try
                    {
                        sqlConn.SqlConnection = new SqlConnection(connectionString);
                        sqlConn.ConnectionString = connectionString;
                        sqlConn.ProviderName = "Sql";
                        sqlConn.DatabaseName = databaseName;
                        connections.Add(sqlConn);
                    }
                    catch (Exception e)
                    {
                        Debug.Print("Exception: {0}", e.StackTrace.ToString());
                        //throw;
                    }
                }
            }
            return connections;
        }

        public static DataTable GetDownloadHistory(int DownloadID)
        {
            string query =
                @"
                    select *
                    from DownloadHistory
                    where DownloadId = @DownloadId
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@DownloadId", SqlDbType.Int);
            prms[0].Value = DownloadID;

            DataTable dt = ExecuteSelectSql(query, sqlDatabaseConnection, prms);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt;
            }
            else
            {
                return null;
            }
        }
        public static DataTable ExecuteSelectSql(string query, SqlDatabaseConnection sqlDatabaseConnection, SqlParameter[] parameters)
        {
            OpenDatabaseConnection(sqlDatabaseConnection);
            DataTable dataTable = null;
            DataSet dataSet = new DataSet();
            try
            {
                SqlCommand sqlCommand = new SqlCommand(query, sqlDatabaseConnection.SqlConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.AddRange(parameters);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dataSet);
                sqlDataAdapter.Dispose();
                dataTable = dataSet.Tables[0];
            }
            catch (SqlException e)
            {
                Debug.Print("Query: {0} \nSqlException: {1}", query, e.StackTrace.ToString());
            }
            catch (Exception e)
            {
                Debug.Print("Query: {0} \nException: {1}", query, e.StackTrace.ToString());
            }
            return dataTable;
        }

        public static DataTable ExecuteSelectSql(string query, SqlDatabaseConnection sqlDatabaseConnection)
        {
            OpenDatabaseConnection(sqlDatabaseConnection);
            DataSet dataSet = new DataSet();
            DataTable dataTable = null;
            try
            {
                SqlCommand sqlCommand = new SqlCommand(query, sqlDatabaseConnection.SqlConnection);
                sqlCommand.CommandType = CommandType.Text;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dataSet);
                sqlDataAdapter.Dispose();
                dataTable = dataSet.Tables[0];
            }
            catch (SqlException e)
            {
                Debug.Print("Query: {0} \nSqlException: {1}", query, e.StackTrace.ToString());
                //throw;
            }
            catch (Exception e)
            {
                Debug.Print("Query: {0} \nException: {1}", query, e.StackTrace.ToString());
                //throw;
            }
            return dataTable;
        }

        public static void ExecuteInsertUpdateDeleteSql(string query, SqlDatabaseConnection sqlDatabaseConnection, SqlParameter[] parameters)
        {
            OpenDatabaseConnection(sqlDatabaseConnection);
            try
            {
                SqlCommand sqlCommand = new SqlCommand(query, sqlDatabaseConnection.SqlConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.AddRange(parameters);
                sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Debug.Print("Query: {0} \nSqlException: {1}", query, e.StackTrace.ToString());
            }
            catch (Exception e)
            {
                Debug.Print("Query: {0} \nException: {1}", query, e.StackTrace.ToString());
            }
        }

        private static void OpenDatabaseConnection(DatabaseConnection dbConn)
        {
            if (dbConn is SqlDatabaseConnection sqlDbConn)
            {
                if (sqlDbConn.SqlConnection == null)
                {
                    sqlDbConn.SqlConnection = new SqlConnection(sqlDbConn.ConnectionString);
                }
                if (sqlDbConn.SqlConnection.State == ConnectionState.Closed || sqlDbConn.SqlConnection.State == ConnectionState.Broken)
                {
                    try
                    {
                        sqlDbConn.SqlConnection.Open();
                    }
                    catch (SqlException e)
                    {

                        Debug.Print("Exception: {0}", e.StackTrace.ToString());
                        //throw;
                    }
                    catch (Exception e)
                    {
                        Debug.Print("Exception: {0}", e.StackTrace.ToString());
                        //throw;
                    }
                }
            }

        }


    }
}
