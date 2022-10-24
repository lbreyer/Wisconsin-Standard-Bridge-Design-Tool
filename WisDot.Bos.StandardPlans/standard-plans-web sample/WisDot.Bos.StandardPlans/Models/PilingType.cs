using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WisDot.Bos.StandardPlans.Models
{
    public class PilingType
    {
        public string PilingTypeCode { get; set; }
        public string PilingTypeDescription { get; set; }
        public DateTime InactiveDate { get; set; }

        public PilingType() { }

        public PilingType(string pilingTypeCode, string pilingTypeDescription)
        {
            this.PilingTypeCode = pilingTypeCode;
            this.PilingTypeDescription = pilingTypeDescription;
        }
    }
}