using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using WisDot.Bos.StandardPlans.Data;

namespace WisDot.Bos.StandardPlans.Models
{
    public class StandardPlan
    {
        public int PlanId { get; set; }
        public string PlanFileName { get; set; }
        public string PlanFilePath { get; set; }
        public string PlanRelPath { get; set; }
        public float SpanLength { get; set; }
        public float SubstructureSkew { get; set; }
        public float ClearRoadwayWidth { get; set; }
        public string BarrierTypeCode { get; set; }
        //public string BarrierTypeDescription { get; set; }
        public BarrierType BarrierType { get; internal set; }
        public bool PavingNotch { get; set; }
        public float AbutmentHeight { get; set; }
        public string PilingTypeCode { get; set; }
        //public string PilingTypeDescription { get; set; }
        public PilingType PilingType { get; internal set; }
        public DateTime InactiveDate { get; internal set; }
        public string Comments { get; set; }
        public DateTime FileDate { get; set; }
        public bool FileFound { get; set; }
        public string FiipsConstructionId { get; set; }
        public UriBuilder uriBuilder { get; set; }
        public string appContext { get; set; }
        public string contentType { get; set; }
        public HttpResponseMessage response { get; set; }

        public StandardPlan()
        {
            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "Standard Plan Start", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");
            Initialize();
            //StandardPlanQuery.WriteDownloadHistory("Test", false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "Standard Plan End", "TST", "XXXXXXXX", "XXXXXXXX", "XXXXXXX");
        }

        private void Initialize()
        {
            this.PlanFileName = "";
            this.PlanFilePath = "";
            this.BarrierTypeCode = "";
            //this.BarrierTypeDescription = "";
            this.PilingTypeCode = "";
            this.Comments = "";
            this.FiipsConstructionId = "";
            this.PlanRelPath = "";
            this.appContext = "";
            this.contentType = "";
            //this.PilingTypeDescription = "";
            this.uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "iisgtwyp.wi.gov";
            appContext = "/wisp";
            //https://iisgtwyt.wi.gov/wisp
        }
    }
}