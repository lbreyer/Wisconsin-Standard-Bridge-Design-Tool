using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Oracle.DataAccess.Client;
using System.Data.OleDb;

namespace WisDot.Bos.StandardPlans.Data
{
    internal class DataConnector
    {
        // Fields are static so there's only one copy
        // "name" of the database to get the connection string
        private static readonly string fiipsDatabaseName = ConfigurationManager.AppSettings["FiipsDatabaseName"].ToString();
        //private static readonly string hsisDatabaseName = ConfigurationManager.AppSettings["HsisDatabaseName"].ToString();
        private static readonly string standardBridgeDesignDatabaseName = ConfigurationManager.AppSettings["StandardBridgeDesignDatabaseName"].ToString();
        private static readonly List<string> databaseNames = new List<string>() { fiipsDatabaseName, standardBridgeDesignDatabaseName };
        private static readonly List<DatabaseConnection> databaseConnections;
        public static int DBfail = 0;
        public static string EString = "";
        public static string Stack = "";
        //private static readonly string AuthUsername = ConfigurationManager.AppSettings["AuthUsername"].ToString();
        //private static readonly string AuthPassword = ConfigurationManager.AppSettings["AuthPassword"].ToString();

        private static readonly string LocalLetEnabled = ConfigurationManager.AppSettings["LocalLetEnabled"].ToString();

        static DataConnector()
        {
            databaseConnections = new List<DatabaseConnection>();
            databaseConnections = CreateDatabaseConnections();
        }

        public static List<DatabaseConnection> GetDatabaseConnections()
        {
            return databaseConnections;
        }

        public static string GetLocalLetEnabled()
        {
            return LocalLetEnabled;
        }

        public static string GetFiipsDatabaseName()
        {
            return fiipsDatabaseName;
        }

        public static string GetStandardBridgeDesignDatabaseName()
        {
            return standardBridgeDesignDatabaseName;
        }

        public static string GetUsername()
        {
            return ConfigurationManager.AppSettings["AuthUsername"].ToString();
        }

        public static string GetPassword()
        {
            return ConfigurationManager.AppSettings["AuthPassword"].ToString();
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
            foreach (ConnectionStringSettings css in ConfigurationManager.ConnectionStrings)
            {
                var connectionString = css.ConnectionString.Trim();
                var databaseName = css.Name.Trim();
                if (!String.IsNullOrEmpty(connectionString) && databaseNames.Any(el => el.Equals(databaseName, StringComparison.CurrentCultureIgnoreCase)))
                {
                    //SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(css.ConnectionString);
                    SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);
                    if (!String.IsNullOrEmpty(csb.Password))
                    {
                        string decryptedPassword = CryptorEngine.Decrypt(csb.Password, true);
                        connectionString = connectionString.Replace(csb.Password, decryptedPassword);

                        if (css.Name == "StandardBridgeDesign")
                        {
                            string decryptedDataSource = CryptorEngine.Decrypt(csb.DataSource, true);
                            connectionString = connectionString.Replace(csb.DataSource, decryptedDataSource);
                        }
                        
                        switch (css.ProviderName.ToLower())
                        {
                            case "sql":
                                var sqlConn = new SqlDatabaseConnection();
                                try
                                {
                                    sqlConn.SqlConnection = new SqlConnection(connectionString);
                                    sqlConn.ConnectionString = connectionString;
                                    sqlConn.ProviderName = css.ProviderName;
                                    sqlConn.DatabaseName = css.Name;
                                    connections.Add(sqlConn);
                                }
                                catch (Exception e)
                                {
                                    Debug.Print("Exception: {0}", e.StackTrace.ToString());
                                    //DBfail = 1;
                                    //throw;
                                }
                                break;
                            case "oracle":
                                var oraConn = new OracleDatabaseConnection();
                                string flag = "";
                                try
                                {
                                    flag = "A";
                                    oraConn.OracleConnection = new OracleConnection(connectionString);
                                    flag = "B";
                                    oraConn.ConnectionString = connectionString;
                                    flag = "C";
                                    oraConn.ProviderName = css.ProviderName;
                                    flag = "D";
                                    oraConn.DatabaseName = css.Name;
                                    flag = "E";
                                    connections.Add(oraConn);
                                }
                                catch (Exception e)
                                {
                                    Debug.Print("Exception: {0}", e.StackTrace.ToString());
                                    DBfail = 2;
                                    EString = "Error at: CreateDatabaseConnections " + flag + e.InnerException.Message;
                                    Stack = e.StackTrace;
                                    //throw;
                                }
                                break;
                        }
                    }
                }
            }
            return connections;
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
                        //DBfail = 3;
                        //throw;
                    }
                    catch (Exception e)
                    {
                        Debug.Print("Exception: {0}", e.StackTrace.ToString());
                        //DBfail = 3;
                        //throw;
                    }
                }
            }
            else if (dbConn is OracleDatabaseConnection oraDbConn)
            {
                if (oraDbConn.OracleConnection == null)
                {
                    oraDbConn.OracleConnection = new OracleConnection(oraDbConn.ConnectionString);
                }
                if (oraDbConn.OracleConnection.State == ConnectionState.Closed || oraDbConn.OracleConnection.State == ConnectionState.Broken)
                {
                    try
                    {
                        oraDbConn.OracleConnection.Open();
                    }
                    catch (OracleException e)
                    {
                        Debug.Print("Exception: {0}", e.StackTrace.ToString());
                        DBfail = 4;
                        EString = "Error at: OpenDatabaseConnection  " + e.Message;
                        throw;
                    }
                    catch (Exception e)
                    {
                        Debug.Print("Exception: {0}", e.StackTrace.ToString());
                        DBfail = 4;
                        EString = "Error at: OpenDatabaseConnection  " + e.Message;
                        throw;
                    }
                }
            }
        }

        #region SQL Methods
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
        #endregion SQL Methods

        #region Oracle Methods
        public static DataTable ExecuteSelectOracle(string query, OracleDatabaseConnection oracleDatabaseConnection)
        {
            OpenDatabaseConnection(oracleDatabaseConnection);
            DataSet dataSet = new DataSet();
            DataTable dataTable = null;
            string flag = "";
            try
            {
                OracleCommand oracleCommand = new OracleCommand(query, oracleDatabaseConnection.OracleConnection);
                flag = "A";
                oracleCommand.CommandType = CommandType.Text;
                flag = "B";
                OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(oracleCommand);
                flag = "C";
                oracleDataAdapter.Fill(dataSet);
                flag = "D";
                oracleDataAdapter.Dispose();
                flag = "E";
                dataTable = dataSet.Tables[0];
            }
            catch (OracleException e)
            {
                Debug.Print("Query: {0} \nOracleException: {1}", query, e.StackTrace.ToString());
                DBfail = 5;
                EString = "Error at: ExecuteSelectOracle A " + flag + e.InnerException.Message;
                Stack = e.StackTrace;
            }
            catch (Exception e)
            {
                Debug.Print("Query: {0} \nException: {1}", query, e.StackTrace.ToString());
                DBfail = 5;
                EString = "Error at: ExecuteSelectOracle A " + flag + e.InnerException.Message;
                Stack = e.StackTrace;
            }
            return dataTable;
        }

        public static DataTable ExecuteSelectOracle(string query, OracleDatabaseConnection oracleDatabaseConnection, OracleParameter[] parameters)
        {
            OpenDatabaseConnection(oracleDatabaseConnection);
            DataSet dataSet = new DataSet();
            DataTable dataTable = null;
            string flag = "";
            try
            {
                OracleCommand oracleCommand = new OracleCommand(query, oracleDatabaseConnection.OracleConnection);
                flag = "A";
                oracleCommand.CommandType = CommandType.Text;
                flag = "B";
                oracleCommand.Parameters.AddRange(parameters);
                flag = "C";
                OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(oracleCommand);
                flag = "D";
                oracleDataAdapter.Fill(dataSet);
                flag = "E";
                oracleDataAdapter.Dispose();
                flag = "F";
                dataTable = dataSet.Tables[0];
            }
            catch (OracleException e)
            {
                Debug.Print("Query: {0} \nOracleException: {1}", query, e.StackTrace.ToString());
                DBfail = 5;
                EString = "Error at: ExecuteSelectOracle B " + flag + e.InnerException.Message;
                Stack = e.StackTrace;
            }
            catch (Exception e)
            {
                Debug.Print("Query: {0} \nException: {1}", query, e.StackTrace.ToString());
                DBfail = 5;
                EString = "Error at: ExecuteSelectOracle B " + flag + e.InnerException.Message;
                Stack = e.StackTrace;
            }
            return dataTable;
        }
        #endregion Oracle Methods
    }
}