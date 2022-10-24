using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using WisDot.Bos.StandardPlans.Models;
using WisDot.Bos.StandardPlans.Data;
using System.IO;
using WisDot.Bos.StandardPlans.Controllers;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace WisDot.Bos.StandardPlans.Repositories
{
    public class StandardPlanRepository : IStandardPlanRepository
    {
        public StandardPlan GetStandardPlan(int id)
        {
            StandardPlan standardPlan = null;
            DataTable dataTable = StandardPlanQuery.GetStandardBridgePlan(id);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                DataRow dataRow = dataTable.Rows[0];
                standardPlan = new StandardPlan
                {
                    PlanId = Convert.ToInt32(dataRow["PlanId"]),
                    PlanFileName = dataRow["PlanFileName"].ToString(),
                    SpanLength = Convert.ToSingle(dataRow["SpanLength"]),
                    SubstructureSkew = Convert.ToSingle(dataRow["SubstructureSkew"]),
                    ClearRoadwayWidth = Convert.ToSingle(dataRow["ClearRoadwayWidth"]),
                    BarrierTypeCode = dataRow["BarrierTypeCode"].ToString(),
                    BarrierType = GetBarrierType(dataRow["BarrierTypeCode"].ToString()),
                    PavingNotch = Convert.ToBoolean(dataRow["PavingNotch"]),
                    AbutmentHeight = Convert.ToSingle(dataRow["AbutmentHeight"]),
                    PilingTypeCode = dataRow["PilingTypeCode"].ToString(),
                    PilingType = GetPilingType(dataRow["PilingTypeCode"].ToString()),
                };
                if (dataRow["InactiveDate"] != DBNull.Value)
                {
                    standardPlan.InactiveDate = Convert.ToDateTime(dataRow["InactiveDate"]);
                }
                if (dataRow["Comments"] != DBNull.Value)
                {
                    standardPlan.Comments = dataRow["Comments"].ToString();
                }
            }
            return standardPlan;
        }

        public StandardPlan GetStandardPlan(float spanLength, float substructureSkew, float clearRoadwayWidth, 
            string barrierTypeCode, bool pavingNotch, float abutmentHeight, string pilingTypeCode)
        {
            StandardPlan plan = new StandardPlan();
            string fileName = String.Format("{0}_{1}_{2}_{3}", spanLength, substructureSkew > 0 ? "+" + substructureSkew.ToString() : substructureSkew.ToString(), clearRoadwayWidth, barrierTypeCode);
            fileName += String.Format("_{0}notch_{1}_{2}.zip", pavingNotch ? "yes" : "no", abutmentHeight, pilingTypeCode);            

            string filePath = HttpContext.Current.Server.MapPath(string.Format("~/standardplans/{0}/{1}", spanLength, fileName));
            //bool planExists = File.Exists(HttpContext.Current.Server.MapPath(string.Format("~/standardplans/{0}/{1}", spanLength, fileName)));
            bool planExists = File.Exists(filePath);
            /* If using the database
            DataTable dataTable = StandardPlanQuery.GetStandardBridgePlan(spanLength, substructureSkew, clearRoadwayWidth, barrierTypeCode, pavingNotch, abutmentHeight, pilingTypeCode);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                DataRow dataRow = dataTable.Rows[0];
                standardPlan = GetStandardPlan(Convert.ToInt32(dataRow["PlanId"]));
            }*/
            plan.PlanFileName = fileName;
            plan.PlanFilePath = filePath;
            if (planExists)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                plan.FileDate = fileInfo.LastWriteTime;
                plan.FileFound = true;
                //var task = Task.Run(async () => await new DownloadController().Download(filePath));
                //using (new DownloadController().Download(filePath)) { }
            }
            else
            {
                plan.FileFound = false;
            }
            return plan;
        }

        public async Task<StandardPlan> GetStandardPlanAsync(float spanLength, float substructureSkew, float clearRoadwayWidth, string barrierTypeCode, bool pavingNotch, float abutmentHeight, string pilingTypeCode)
        {
            StandardPlan plan = new StandardPlan();
            string path = String.Format("{0}_{1}_{2}_{3}", spanLength, substructureSkew > 0 ? "+" + substructureSkew.ToString() : substructureSkew.ToString(), clearRoadwayWidth, barrierTypeCode);
            path += String.Format("_{0}notch_{1}_{2}.zip", pavingNotch ? "yes" : "no", abutmentHeight, pilingTypeCode);
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "iisgtwyt.wi.gov";
            string appContext = "/wisp";
            string relPath = String.Format("{0}/{1}", appContext, path);
            string fileName = Path.GetFileName(relPath);
            string contentType = MimeMapping.GetMimeMapping(relPath);
            plan.PlanFileName = path;
            plan.PlanFilePath = relPath;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(uriBuilder.ToString());
                client.DefaultRequestHeaders.Accept.Clear();
                // For Basic Authentication http header - "username:password"
                var byteArray = Encoding.ASCII.GetBytes("S_DOT8WISP:Kpr4kgmT8wpnFg@z");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                HttpResponseMessage response = await client.GetAsync(uriBuilder.ToString());
                if (response.IsSuccessStatusCode)
                {
                    plan.FileFound = true;
                }
                else
                {
                    plan.FileFound = false;
                }
            }
            return plan;
        }

        public void WriteDownloadHistory(string fileName, bool fileFound, DateTime fileDate, DateTime downloadDate, string comments, string wamsId, string fiipsConstructionId, string fiipsDesignId, string fiipsStructureId)
        {
            StandardPlanQuery.WriteDownloadHistory(fileName, fileFound, fileDate, downloadDate, comments, wamsId, fiipsConstructionId, fiipsDesignId, fiipsStructureId);
        }

        public BarrierType GetBarrierType(string barrierTypeCode)
        {
            BarrierType barrierType = null;
            DataTable dataTable = StandardPlanQuery.GetBarrierType(barrierTypeCode);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                DataRow dataRow = dataTable.Rows[0];
                barrierType = new BarrierType
                {
                    BarrierTypeCode = dataRow["BarrierTypeCode"].ToString(),
                    BarrierTypeDescription = dataRow["BarrierTypeDescription"].ToString()
                };
            }
            return barrierType;
        }

        public List<BarrierType> GetBarrierTypes()
        {
            List<BarrierType> barrierTypes = new List<BarrierType>();
            DataTable dataTable = StandardPlanQuery.GetBarrierTypes();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    BarrierType barrierType = new BarrierType
                    {
                        BarrierTypeCode = dataRow["BarrierTypeCode"].ToString(),
                        BarrierTypeDescription = dataRow["BarrierTypeDescription"].ToString()
                    };
                    barrierTypes.Add(barrierType);
                }
            }
            return barrierTypes;
        }

        public PilingType GetPilingType(string pilingTypeCode)
        {
            PilingType pilingType = null;
            DataTable dataTable = StandardPlanQuery.GetPilingType(pilingTypeCode);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                DataRow dataRow = dataTable.Rows[0];
                pilingType = new PilingType
                {
                    PilingTypeCode = dataRow["PilingTypeCode"].ToString(),
                    PilingTypeDescription = dataRow["PilingTypeDescription"].ToString()
                };
            }
            return pilingType;
        }

        public List<PilingType> GetPilingTypes()
        {
            List<PilingType> pilingTypes = new List<PilingType>();
            DataTable dataTable = StandardPlanQuery.GetPilingTypes();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    PilingType pilingType = new PilingType
                    {
                        PilingTypeCode = dataRow["PilingTypeCode"].ToString(),
                        PilingTypeDescription = dataRow["PilingTypeDescription"].ToString()
                    };
                    pilingTypes.Add(pilingType);
                }
            }
            return pilingTypes;
        }

        public List<double> GetSpanLengths()
        {
            List<double> spanLengths = new List<double>();
            DataTable dataTable = StandardPlanQuery.GetSpanLengths();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    double spanLength = Convert.ToDouble(dataRow["SpanLength"]);
                    spanLengths.Add(spanLength);
                }
            }
            return spanLengths;
        }

        public List<string> GetSubstructureSkews()
        {
            List<string> substructureSkews = new List<string>();
            DataTable dataTable = StandardPlanQuery.GetSubstructureSkews();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    float angle = Convert.ToSingle(dataRow["SubstructureSkew"]);
                    string skewAngle = angle % 1 == 0 ? Math.Truncate(angle).ToString() : angle.ToString();
                    string skew = String.Format("{0} ({1})", skewAngle, dataRow["SubstructureSkewDescription"].ToString());
                    substructureSkews.Add(skew);
                }
            }
            return substructureSkews;
        }

        public List<double> GetClearRoadwayWidths()
        {
            List<double> clearRoadwayWidths = new List<double>();
            DataTable dataTable = StandardPlanQuery.GetClearRoadwayWidths();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    float width = Convert.ToSingle(dataRow["ClearRoadwayWidth"]);
                    if (width % 1 == 0)
                    {
                        clearRoadwayWidths.Add(Math.Truncate(width));
                    }
                    else
                    {
                        clearRoadwayWidths.Add(width);
                    }
                }
            }
            return clearRoadwayWidths;
        }

        public List<double> GetAbutmentHeights()
        {
            List<double> abutmentHeights = new List<double>();
            DataTable dataTable = StandardPlanQuery.GetAbutmentHeights();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    float width = Convert.ToSingle(dataRow["AbutmentHeight"]);
                    if (width % 1 == 0)
                    {
                        abutmentHeights.Add(Math.Truncate(width));
                    }
                    else
                    {
                        abutmentHeights.Add(width);
                    }
                }
            }
            return abutmentHeights;
        }

        public FiipsProject GetFiipsProject(string fiipsConstructionId)
        {
            FiipsProject fiipsProject = null;
            DataTable dataTable;
            if (StandardPlanQuery.ValidatePilotRestriction())
            {
                dataTable = StandardPlanQuery.GetPilotProject(fiipsConstructionId);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    fiipsProject = new FiipsProject();
                }
            }
            else
            {
                dataTable = StandardPlanQuery.GetFiipsProject(fiipsConstructionId);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    DataRow dataRow = dataTable.Rows[0];
                    fiipsProject = new FiipsProject
                    {
                        ConstructionId = dataRow["FOS_PROJ_ID"].ToString(),
                        PrimaryRoute = String.Format("{0} {1}", dataRow["hwy_jrdn_cd"] != DBNull.Value ? dataRow["hwy_jrdn_cd"].ToString() : "",
                            dataRow["hwy_rte_nb"] != DBNull.Value ? dataRow["hwy_rte_nb"].ToString() : ""),
                    };
                    if (dataRow["DESIGNID"] != DBNull.Value)
                    {
                        fiipsProject.DesignId = dataRow["DESIGNID"].ToString();
                    }
                    if (dataRow["PROJ_CNTY_NM"] != DBNull.Value)
                    {
                        fiipsProject.County = dataRow["PROJ_CNTY_NM"].ToString();
                    }
                    if (dataRow["DOT_RGN_NM"] != DBNull.Value)
                    {
                        fiipsProject.Region = dataRow["DOT_RGN_NM"].ToString();
                    }
                    if (dataRow["PPROJ_FOS_TITL_TXT"] != DBNull.Value)
                    {
                        fiipsProject.Title = dataRow["PPROJ_FOS_TITL_TXT"].ToString();
                    }
                    if (dataRow["PPROJ_FOS_LMT_TXT"] != DBNull.Value)
                    {
                        fiipsProject.Limit = dataRow["PPROJ_FOS_LMT_TXT"].ToString();
                    }
                    if (dataRow["PRM_ORG_NM"] != DBNull.Value)
                    {
                        fiipsProject.PrimaryOrganizationName = dataRow["PRM_ORG_NM"].ToString();
                    }
                    if (dataRow["RPSB_ORG_CD"] != DBNull.Value)
                    {
                        fiipsProject.ResponsibleOrganizationCode = dataRow["RPSB_ORG_CD"].ToString();
                    }
                    if (dataRow["RPSB_ORG_NM"] != DBNull.Value)
                    {
                        fiipsProject.ResponsibleOrganizationName = dataRow["RPSB_ORG_NM"].ToString();
                    }
                    if (dataRow["SUPR_PPROJ_PTCP_NM"] != DBNull.Value)
                    {
                        fiipsProject.Supervisor = dataRow["SUPR_PPROJ_PTCP_NM"].ToString();
                    }
                    if (dataRow["MGR_PPROJ_PTCP_NM"] != DBNull.Value)
                    {
                        fiipsProject.Manager = dataRow["MGR_PPROJ_PTCP_NM"].ToString();
                    }
                    if (dataRow["LDR_PPROJ_PTCP_NM"] != DBNull.Value)
                    {
                        fiipsProject.ProjectLeader = dataRow["LDR_PPROJ_PTCP_NM"].ToString();
                    }
                    if (dataRow["ESTCP_SCHD_DT"] != DBNull.Value)
                    {
                        fiipsProject.LetDate = Convert.ToDateTime(dataRow["ESTCP_SCHD_DT"]);
                    }
                    if (dataRow["PPROJ_EPSBL_PSE_DT"] != DBNull.Value)
                    {
                        fiipsProject.EarliestPseDate = Convert.ToDateTime(dataRow["PPROJ_EPSBL_PSE_DT"]);
                    }
                    if (dataRow["PPROJ_ANTD_PSE_DT"] != DBNull.Value)
                    {
                        fiipsProject.PseDate = Convert.ToDateTime(dataRow["PPROJ_ANTD_PSE_DT"]);
                    }
                    if (dataRow["LFCY_STG_CD"] != DBNull.Value)
                    {
                        fiipsProject.LifecycleStageCode = dataRow["LFCY_STG_CD"].ToString();
                    }
                    if (dataRow["LFCY_STG_DESC"] != DBNull.Value)
                    {
                        fiipsProject.LifecycleStageDescription = dataRow["LFCY_STG_DESC"].ToString();
                    }
                    if (dataRow["PPROJ_CNCP_CD"] != DBNull.Value)
                    {
                        fiipsProject.PlanningProjectConceptCode = dataRow["PPROJ_CNCP_CD"].ToString();
                    }
                    if (dataRow["PPROJ_CNCP_DESC"] != DBNull.Value)
                    {
                        fiipsProject.PlanningProjectConceptDescription = dataRow["PPROJ_CNCP_DESC"].ToString();
                    }
                }
            }
            return fiipsProject;
        }

        public string GetTermsOfAgreement()
        {

            return HttpContext.Current.Server.MapPath(string.Format("~/standardplans/docs/{0}", "standardbridgedesigntoolagreement.pdf"));
            //return url;
            //DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("/standardplans"));
            //string url = HttpContext.Current.Server.MapPath(string.Format("/splans/docs/{0}", "standardbridgedesigntoolagreement.pdf"));
            //FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath("~/standardplans/docs/standardbridgedesigntoolagreement.pdf"));
           // return fileInfo.FullName;
        }

        public string GetSampleStandardSlabPlan()
        {
            return HttpContext.Current.Server.MapPath(string.Format("~/standardplans/docs/{0}", "standardslabplan.zip"));
        }
    }
}