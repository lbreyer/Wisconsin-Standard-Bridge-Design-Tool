using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WisDot.Bos.StandardPlans.Models
{
    public class FiipsProject
    {
        public string ConstructionId { get; set; }
        public string DesignId { get; set; }
        public List<string> TiedConstructionIds { get; set; }
        public string County { get; set; }
        public string Region { get; set; }
        public string PrimaryRoute { get; set; }
        public string Title { get; set; }
        public string Limit { get; set; }
        public string PrimaryOrganizationName { get; set; }
        public string ResponsibleOrganizationCode { get; set; }
        public string ResponsibleOrganizationName { get; set; }
        public string Supervisor { get; set; }
        public string Manager { get; set; }
        public string ProjectLeader { get; set; }
        public DateTime LetDate { get; set; }
        public DateTime PseDate { get; set; }
        public DateTime EarliestPseDate { get; set; }
        public string PlanningProjectConceptCode { get; set; }
        public string PlanningProjectConceptDescription { get; set; }
        public string LifecycleStageCode { get; set; }
        public string LifecycleStageDescription { get; set; }

        public FiipsProject()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.ConstructionId = "";
            this.DesignId = "";
            this.TiedConstructionIds = new List<string>();
            this.County = "";
            this.Region = "";
            this.PrimaryRoute = "";
            this.Title = "";
            this.Limit = "";
            this.PrimaryOrganizationName = "";
            this.ResponsibleOrganizationCode = "";
            this.Supervisor = "";
            this.Manager = "";
            this.ProjectLeader = "";
        }
    }
}