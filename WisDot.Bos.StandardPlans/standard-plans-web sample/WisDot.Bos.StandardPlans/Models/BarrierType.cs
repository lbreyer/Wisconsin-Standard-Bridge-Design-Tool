using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WisDot.Bos.StandardPlans.Models
{
    public class BarrierType
    {
        public string BarrierTypeCode { get; set; }
        public string BarrierTypeDescription { get; set; }
        public DateTime InactiveDate { get; set; }

        public BarrierType() { }
        
        public BarrierType(string barrierTypeCode, string barrierTypeDescription)
        {
            this.BarrierTypeCode = barrierTypeCode;
            this.BarrierTypeDescription = barrierTypeDescription;
        }
    }
}