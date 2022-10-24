using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WisDot.Bos.StandardPlans.Models;

namespace WisDot.Bos.StandardPlans.Controllers
{
    public class DownloadController : Controller
    {
        public async Task<ActionResult> Download(string path)
        {
            using (var memory = new MemoryStream())
            {
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                var exit = Path.GetExtension(path).ToLowerInvariant();
                // return File(new FileStream(path, FileMode.Open), MimeMapping.GetMimeMapping(path), Path.GetFileName(path));
                return File(memory, MimeMapping.GetMimeMapping(path), Path.GetFileName(path));
            }
        }

        /*public StandardPlan GetRemoteStandardPlan(float spanLength, float substructureSkew, float clearRoadwayWidth,
            string barrierTypeCode, bool pavingNotch, float abutmentHeight, string pilingTypeCode)
        {
            StandardPlan plan = new StandardPlan();
            string path = String.Format("{0}_{1}_{2}_{3}", spanLength, skew > 0 ? "+" + skew.ToString() : skew.ToString(), clearRoadwayWidth, barrierType);
            path += String.Format("_{0}notch_{1}_{2}.zip", hasPavingNotch ? "yes" : "no", abutmentHeight, pilingType);


        }*/

    }
}