using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;

namespace WisDot.Bos.StandardPlans.Data
{
    public class StandardPlanQuery
    {
        private static readonly SqlDatabaseConnection sqlDatabaseConnection;
        private static readonly OracleDatabaseConnection fiipsOracleDatabaseConnection;
        private static readonly string standardBridgeDesignDatabaseName = DataConnector.GetStandardBridgeDesignDatabaseName();
        private static readonly string fiipsDatabaseName = DataConnector.GetFiipsDatabaseName();
        private static readonly List<DatabaseConnection> databaseConnections = DataConnector.GetDatabaseConnections();

        static StandardPlanQuery()
        {
            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "Query Start", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");
            sqlDatabaseConnection = (SqlDatabaseConnection)DataConnector.RequestDatabaseConnection(standardBridgeDesignDatabaseName);
            fiipsOracleDatabaseConnection = (OracleDatabaseConnection)DataConnector.RequestDatabaseConnection(fiipsDatabaseName);
            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "Query End", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");
        }

        public static DataTable GetStandardBridgePlan(int planId)
        {
            DataTable dataTable = null;
            string query =
                @"
                    select *
                    from StandardPlan
                    where PlanId = @planId
                ";
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@planId", SqlDbType.Int);
            parameters[0].Value = planId;
            dataTable = DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection, parameters);
            return dataTable;
        }

        public static DataTable GetStandardBridgePlan(float spanLength, float substructureSkew, float clearRoadwayWidth, string barrierTypeCode, bool pavingNotch,
            float abutmentHeight, string pilingTypeCode)
        {
            DataTable dataTable = null;
            string query =
                @"
                    select *
                    from StandardPlan
                    where SpanLength = @spanLength
                        and SubstructureSkew = @substructureSkew
                        and ClearRoadwayWidth = @clearRoadwayWidth
                        and BarrierTypeCode = @barrierTypeCode
                        and PavingNotch = @pavingNotch
                        and AbutmentHeight = @abutmentHeight
                        and PilingTypeCode = @pilingTypeCode
                ";
            SqlParameter[] parameters = new SqlParameter[7];
            parameters[0] = new SqlParameter("@spanLength", SqlDbType.Decimal);
            parameters[0].Value = spanLength;
            parameters[1] = new SqlParameter("@substructureSkew", SqlDbType.Decimal);
            parameters[1].Value = substructureSkew;
            parameters[2] = new SqlParameter("@clearRoadwayWidth", SqlDbType.Decimal);
            parameters[2].Value = clearRoadwayWidth;
            parameters[3] = new SqlParameter("@barrierTypeCode", SqlDbType.VarChar);
            parameters[3].Value = barrierTypeCode;
            parameters[4] = new SqlParameter("@pavingNotch", SqlDbType.Bit);
            parameters[4].Value = pavingNotch ? 1 : 0;
            parameters[5] = new SqlParameter("@abutmentHeight", SqlDbType.Decimal);
            parameters[5].Value = abutmentHeight;
            parameters[6] = new SqlParameter("@pilingTypeCode", SqlDbType.VarChar);
            parameters[6].Value = pilingTypeCode;
            dataTable = DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection, parameters);
            return dataTable;
        }

        public static DataTable GetBarrierTypes()
        {
            DataTable dataTable = null;
            string query =
                @"
                    select *
                    from BarrierType
                    where InactiveDate is null
                    order by BarrierTypeCode
                ";
            dataTable = DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection);
            return dataTable;
        }

        public static DataTable GetBarrierType(string barrierTypeCode)
        {
            DataTable dataTable = null;
            string query =
                @"
                    select *
                    from BarrierType
                    where BarrierTypeCode = @barrierTypeCode
                ";
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@barrierTypeCode", SqlDbType.VarChar);
            parameters[0].Value = barrierTypeCode;
            dataTable = DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection, parameters);
            return dataTable;
        }

        public static DataTable GetSpanLengths()
        {
            DataTable dataTable = null;
            string query =
                @"
                    select *
                    from SpanLength
                    where InactiveDate is null
                ";
            dataTable = DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection);
            return dataTable;
        }

        public static DataTable GetSubstructureSkews()
        {
            DataTable dataTable = null;
            string query =
                @"
                    select *
                    from SubstructureSkew
                    where InactiveDate is null
                    order by substructureSkew
                ";
            dataTable = DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection);
            return dataTable;
        }

        public static DataTable GetClearRoadwayWidths()
        {
            DataTable dataTable = null;
            string query =
                @"
                    select *
                    from ClearRoadwayWidth
                    where InactiveDate is null
                    order by ClearRoadwayWidth
                ";
            dataTable = DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection);
            return dataTable;
        }

        public static DataTable GetAbutmentHeights()
        {
            DataTable dataTable = null;
            string query =
                @"
                    select *
                    from AbutmentHeight
                    where InactiveDate is null
                    order by AbutmentHeight
                ";
            dataTable = DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection);
            return dataTable;
        }

        public static DataTable GetPilingTypes()
        {
            DataTable dataTable = null;
            string query =
                @"
                    select *
                    from PilingType
                    where InactiveDate is null
                    order by PilingTypeCode
                ";
            dataTable = DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection);
            return dataTable;
        }

        public static DataTable GetPilingType(string pilingTypeCode)
        {
            DataTable dataTable = null;
            string query =
                @"
                    select *
                    from PilingType
                    where PilingTypeCode = @pilingTypeCode
                ";
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@pilingTypeCode", SqlDbType.VarChar);
            parameters[0].Value = pilingTypeCode;
            dataTable = DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection, parameters);
            return dataTable;
        }

        public static DataTable GetFiipsProject(string fiipsConstructionId)
        {
            DataTable dataTable = null;
            string query =
               @"
                    select project.fos_proj_id,
                        grp.grp_fos_proj_id as designid,
                        project.proj_cnty_nm,
                        county.dot_rgn_nm,
                        project.hwy_jrdn_cd,
                        project.hwy_rte_nb,
                        project.pproj_fos_titl_txt,
                        project.pproj_fos_lmt_txt,
                        project.prm_org_nm,
                        project.rpsb_org_cd,
                        project.rpsb_org_nm,
                        project.supr_pproj_ptcp_nm,
                        project.mgr_pproj_ptcp_nm,
                        project.ldr_pproj_ptcp_nm,
                        project.estcp_schd_dt,
                        project.pproj_epsbl_pse_dt,
                        project.lfcy_stg_cd,
                        project.lfcy_stg_desc,
                        project.pproj_cncp_cd,
                        project.pproj_cncp_desc,
                        pmpAssociatedProject.pproj_antd_pse_dt
                    from dot1pmic.dw_pmic_fiip_proj project, 
                        dot1pmic.dw_pmic_fiip_grp grp,
                        dot1pmic.dw_pmic_fiip_cnty county,
                        dot1pmic.dw_pmp_assd_proj pmpAssociatedProject
                    where project.fos_proj_id = :constructionId
                        and project.pproj_id = county.pproj_id
                        and project.fos_proj_id = grp.fos_proj_id(+)
                        and grp.pproj_grp_ty_cd = 'DES'
                        and project.fos_proj_id = pmpAssociatedProject.fos_proj_id(+)
                ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("constructionId", OracleDbType.Varchar2);
            prms[0].Value = fiipsConstructionId;
            dataTable = DataConnector.ExecuteSelectOracle(query, fiipsOracleDatabaseConnection, prms);
            return dataTable;
        }

        public static DataTable GetPilotProject(string fiipsConstructionId)
        {
            string query =
                @"
                    select *
                    from Pilot
                    where ConstructionId = @constructionId
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@constructionId", SqlDbType.VarChar);
            prms[0].Value = fiipsConstructionId;

            return DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection, prms);
        }

        public static void WriteDownloadHistory(string fileName, bool fileFound, DateTime fileDate, DateTime downloadDate, string comments, string wamsId, string fiipsConstructionId, string fiipsDesignId, string fiipsStructureId)
        {
            string query =
                @"
                    insert into DownloadHistory(FileName, FileFound, FileDate, DownloadDate, WamsId, FiipsConstructionId, DesignId, StructureId, Comments, NotificationSent)
                    values(@fileName, @fileFound, @fileDate, @downloadDate, @wamsId, @fiipsConstructionId, @designId, @structureId, @comments, @notificationSent)
                ";
            SqlParameter[] parameters = new SqlParameter[10];
            parameters[0] = new SqlParameter("@fileName", SqlDbType.VarChar);
            parameters[0].Value = fileName;
            parameters[1] = new SqlParameter("@fileFound", SqlDbType.Bit);
            parameters[1].Value = fileFound ? 1 : 0;
            parameters[2] = new SqlParameter("@fileDate", SqlDbType.DateTime);
            if (fileFound)
            {
                parameters[2].Value = fileDate;
                //parameters[2].Value = DBNull.Value;
            }
            else
            {
                parameters[2].Value = DBNull.Value;
            }
            parameters[3] = new SqlParameter("@downloadDate", SqlDbType.DateTime);
            parameters[3].Value = downloadDate;
            //parameters[3].Value = DBNull.Value;
            parameters[4] = new SqlParameter("@wamsId", SqlDbType.VarChar);
            if (!String.IsNullOrEmpty(wamsId))
            {
                parameters[4].Value = wamsId;
            }
            else
            {
                parameters[4].Value = DBNull.Value;
            }
            parameters[5] = new SqlParameter("@fiipsConstructionId", SqlDbType.VarChar);
            parameters[5].Value = fiipsConstructionId;
            parameters[6] = new SqlParameter("@designId", SqlDbType.VarChar);
            if (!String.IsNullOrEmpty(fiipsDesignId))
            {
                parameters[6].Value = fiipsDesignId;
            }
            else
            {
                parameters[6].Value = DBNull.Value;
            }
            parameters[7] = new SqlParameter("@structureId", SqlDbType.VarChar);
            if (!String.IsNullOrEmpty(fiipsStructureId))
            {
                parameters[7].Value = fiipsStructureId;
            }
            else
            {
                parameters[7].Value = DBNull.Value;
            }
            parameters[8] = new SqlParameter("@comments", SqlDbType.VarChar);
            if (!String.IsNullOrEmpty(comments))
            {
                parameters[8].Value = comments;
            }
            else
            {
                parameters[8].Value = DBNull.Value;
            }
            parameters[9] = new SqlParameter("@notificationSent", SqlDbType.Bit);
            parameters[9].Value = 0;
            //.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            DataConnector.ExecuteInsertUpdateDeleteSql(query, sqlDatabaseConnection, parameters);
        }

        public static void ValidateUser(string WamsID, string FirstName, string LastName, string Email, string PhoneNum, bool IsInactive, DateTime InactiveDate)
        {
            try
            {
                //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "Validate Start", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");

                string query =
                @"
                    select *
                    from Users
                    where WamsId = @wamsId
                ";
                SqlParameter[] prms = new SqlParameter[1];
                prms[0] = new SqlParameter("@wamsId", SqlDbType.VarChar);
                prms[0].Value = WamsID;
                DataTable dt = DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection, prms);

                if (dt != null && dt.Rows.Count == 0)
                {
                    AddUser(WamsID, FirstName, LastName, Email, PhoneNum, IsInactive, InactiveDate);
                }

                //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "Validate End", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");

            }
            catch (Exception e)
            {
                // In case user spams input faster than Database can respond to queries
                //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "Validate User Failed", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");

            }
        }

        public static void AddUser(string WamsID, string FirstName, string LastName, string Email, string PhoneNum, bool IsInactive, DateTime InactiveDate)
        {
            string query =
                @"
                    insert into Users(WamsId, FirstName, LastName, EmailAddress, PhoneNumber, InactiveDate)
                    values(@wamsId, @firstName, @lastName, @emailAddress, @phoneNumber, @inactiveDate)
                ";
            SqlParameter[] prms = new SqlParameter[6];
            prms[0] = new SqlParameter("@wamsId", SqlDbType.VarChar);
            prms[0].Value = WamsID;
            prms[1] = new SqlParameter("@firstName", SqlDbType.VarChar);
            prms[1].Value = FirstName;
            prms[2] = new SqlParameter("@lastName", SqlDbType.VarChar);
            prms[2].Value = LastName;
            prms[3] = new SqlParameter("@emailAddress", SqlDbType.VarChar);
            prms[3].Value = Email;
            prms[4] = new SqlParameter("@phoneNumber", SqlDbType.VarChar);
            prms[4].Value = PhoneNum;
            prms[5] = new SqlParameter("@inactiveDate", SqlDbType.VarChar);
            if (IsInactive)
            {
                prms[5].Value = InactiveDate;
            }
            else
            {
                prms[5].Value = DBNull.Value;
            }
            DataConnector.ExecuteInsertUpdateDeleteSql(query, sqlDatabaseConnection, prms);
        }

        public static void WriteError(string error)
        {
            string query =
                @"
                    insert into Errors(ErrorType)
                    values(@errorType)
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@errorType", SqlDbType.NChar);
            prms[0].Value = error;
            DataConnector.ExecuteInsertUpdateDeleteSql(query, sqlDatabaseConnection, prms);
        }

        public static bool ValidatePilotRestriction()
        {
            string query =
                @"
                    select *
                    from ApplicationSetting
                    where SettingName = @pilotRestricted
                        and SettingValue = @yes
                ";
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@pilotRestricted", SqlDbType.VarChar);
            prms[0].Value = "PilotRestricted";
            prms[1] = new SqlParameter("@yes", SqlDbType.VarChar);
            prms[1].Value = "yes";
            DataTable dt = DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection, prms);
            if (dt != null && dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static DataTable GetTopDownload()
        {
            string query =
                @"
                    select top 1 *
                    from DownloadHistory
                    order by @DownloadId desc
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@DownloadId", SqlDbType.VarChar);
            prms[0].Value = "DownloadId";

            DataTable dt = DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection, prms);
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt;
            }
            else
            {
                return null;
            }
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

            DataTable dt = DataConnector.ExecuteSelectSql(query, sqlDatabaseConnection, prms);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt;
            }
            else
            {
                return null;
            }
        }
    }
}