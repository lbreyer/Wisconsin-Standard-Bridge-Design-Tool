using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WisDot.Bos.StandardPlans.Repositories;
using WisDot.Bos.StandardPlans.Models;

using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using WisDot.Bos.StandardPlans.Data;
using System.Windows.Forms;
using NLog;
using System.Web.UI;

namespace WisDot.Bos.StandardPlans.Controllers
{
    public class HomeController : Controller
    {
        private IStandardPlanRepository standardPlanRepository;
        private static StandardPlan plan;
        private static User currUser;
        //Session["UserName"] = "";
        private static readonly string Username = DataConnector.GetUsername();
        private static readonly string Password = DataConnector.GetPassword();

        public HomeController()
        {
            if (currUser == null)
            {
                currUser = new User("BOS", "BOS", "BOS", "", "");
            }
            this.standardPlanRepository = new StandardPlanRepository();
        }

        public JsonResult GetTermsOfAgreement()
        {
            var agreementUrl = standardPlanRepository.GetTermsOfAgreement();
            return Json(new { success = true, url = agreementUrl });
        }

        public async Task<ActionResult> GetReport()
        {
            try
            {
                plan = new StandardPlan();

                plan.PlanFilePath = "docs/StandardBridgeDesignToolAgreement.pdf";

                plan.PlanRelPath = String.Format("{0}/{1}", plan.appContext, plan.PlanFilePath);
                plan.uriBuilder.Path = plan.PlanRelPath;

                plan.PlanFileName = Path.GetFileName(plan.PlanRelPath);

                plan.contentType = MimeMapping.GetMimeMapping(plan.PlanRelPath);

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(plan.uriBuilder.ToString());
                    client.DefaultRequestHeaders.Accept.Clear();

                    // For Basic Authentication http header - "username:password"
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", Username, CryptorEngine.Decrypt(Password, true)));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    plan.response = await client.GetAsync(plan.uriBuilder.ToString());

                    DateTimeOffset DTO;

                    try
                    {
                        DTO = (DateTimeOffset)plan.response.Content.Headers.LastModified;
                        plan.FileDate = DTO.DateTime;
                    }
                    catch (Exception e)
                    {
                        plan.FileDate = DateTime.MaxValue;
                    }
                }
                HttpContent content = plan.response.Content;

                var contentStream = await content.ReadAsStreamAsync(); // get the actual content stream

                if (plan.contentType.StartsWith("image") || plan.contentType.Contains("pdf"))
                    return File(contentStream, plan.contentType);

                return File(contentStream, plan.contentType, plan.PlanFileName);
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public JsonResult ValidateFiipsConstructionId(string fiipsConstructionId)
        {
            var fiipsProject = standardPlanRepository.GetFiipsProject(fiipsConstructionId);
            if (fiipsProject != null)
            {
                return Json(new { success = true, project = fiipsProject });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public JsonResult EnableLocalLet()
        {
            if (DataConnector.GetLocalLetEnabled() == "true")
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        public ActionResult GetPlan(string spanLength, string substructureSkew, string clearRoadwayWidth, string barrierType,
            string pavingNotch, string abutmentHeight, string pilingType)
        {
            float length = Convert.ToSingle(spanLength);
            float skew = Convert.ToSingle(substructureSkew.Split(' ')[0]);
            float width = Convert.ToSingle(clearRoadwayWidth);
            bool hasPavingNotch = pavingNotch != null ? true : false;
            float height = Convert.ToSingle(abutmentHeight);
            var standardPlan = standardPlanRepository.GetStandardPlan(length, skew, width, barrierType, hasPavingNotch, height, pilingType);
            ViewBag.StandardPlan = null;
            if (standardPlan != null)
            {
                ViewBag.StandardPlan = standardPlan;
            }
            return PartialView("_Downloads");
        }

        [HttpPost]
        public async Task<ActionResult> GetStandardPlan(string spanLength, string substructureSkew, string clearRoadwayWidth, string barrierType,
            string pavingNotch, string abutmentHeight, string pilingType, string fiipsConstructionId, string fiipsDesignId, string fiipsStructureId)
        {
            try
            {
                plan = new StandardPlan();

                float length = Convert.ToSingle(spanLength);
                float skew = Convert.ToSingle(substructureSkew.Split(' ')[0]);
                float width = Convert.ToSingle(clearRoadwayWidth);
                bool hasPavingNotch = pavingNotch.Equals("true") ? true : false;
                float height = Convert.ToSingle(abutmentHeight);

                // NOTE: Currently set to pull from renamed folder [SKEW]R 
                plan.PlanFilePath = String.Format("{0}R/{1}_{2}_{3}_{4}", spanLength, spanLength, skew.ToString(), clearRoadwayWidth, barrierType);
                plan.PlanFilePath += String.Format("_{0}notch_{1}_{2}.zip", hasPavingNotch ? "yes" : "no", abutmentHeight, pilingType);

                plan.PlanRelPath = String.Format("{0}/{1}", plan.appContext, plan.PlanFilePath);
                plan.uriBuilder.Path = plan.PlanRelPath;

                plan.PlanFileName = Path.GetFileName(plan.PlanRelPath);

                plan.contentType = MimeMapping.GetMimeMapping(plan.PlanRelPath);

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(plan.uriBuilder.ToString());
                    client.DefaultRequestHeaders.Accept.Clear();

                    // For Basic Authentication http header - "username:password"
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", Username, CryptorEngine.Decrypt(Password, true)));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    plan.response = await client.GetAsync(plan.uriBuilder.ToString());

                    DateTimeOffset DTO;

                    try
                    {
                        DTO = (DateTimeOffset)plan.response.Content.Headers.LastModified;
                        plan.FileDate = DTO.DateTime;
                    }
                    catch (Exception e)
                    {
                        plan.FileDate = DateTime.MaxValue;
                    }

                    if (plan.response.IsSuccessStatusCode || (skew > 0 && plan.response.Content != null))
                    {
                        // currUser.wamsID != null ? currUser.wamsID : "XXX"
                       standardPlanRepository.WriteDownloadHistory(plan.PlanFileName, true, plan.FileDate, DateTime.Now, comments: "", wamsId: Session["wamsID"].ToString(), fiipsConstructionId, fiipsDesignId, fiipsStructureId);
                        
                        return Json(new { success = true });
                    }
                    else
                    {
                        standardPlanRepository.WriteDownloadHistory(plan.PlanFileName, false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "Plan not found", wamsId: currUser.wamsID, fiipsConstructionId, fiipsDesignId, fiipsStructureId);
                        return Json(new { success = false });
                    }
                }
            }
            catch (Exception e)
            {
                standardPlanRepository.WriteDownloadHistory(plan.PlanFileName, false, fileDate: new DateTime(1, 1, 1), DateTime.Now, comments: "Failed due to Error", wamsId: currUser.wamsID, fiipsConstructionId, fiipsDesignId, fiipsStructureId);
                return Json(new { success = false });
            }
        }

        public FileResult GetPlanZip(string filePath)
        {
            //var sampleStandardSlabPlanUrl = standardPlanRepository.GetSampleStandardSlabPlan();
            //var agreementUrl = standardPlanRepository.GetTermsOfAgreement();
            int fail = 10;
            read:
            try
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/zip");
            }
            catch (Exception e)
            {
                System.Threading.Thread.Sleep(100);
                if (fail > 0)
                {
                    fail--;
                    goto read;
                }
            }
            return null;
        }

        public async Task<ActionResult> DownloadPlan(string spanLength, string substructureSkew, string clearRoadwayWidth, string barrierType,
            string pavingNotch, string abutmentHeight, string pilingType, string fiipsConstructionId)
        {
            StandardPlan plan = new StandardPlan();
            float length = Convert.ToSingle(spanLength);
            float skew = Convert.ToSingle(substructureSkew.Split(' ')[0]);
            float width = Convert.ToSingle(clearRoadwayWidth);
            bool hasPavingNotch = pavingNotch.Equals("true") ? true : false;
            float height = Convert.ToSingle(abutmentHeight);

            string path = String.Format("{0}/{1}_{2}_{3}_{4}", spanLength, spanLength, skew > 0 ? "+" + skew.ToString() : skew.ToString(), clearRoadwayWidth, barrierType);
            path += String.Format("_{0}notch_{1}_{2}.zip", hasPavingNotch ? "yes" : "no", abutmentHeight, pilingType);

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = "https";
            uriBuilder.Host = "iisgtwyt.wi.gov";
            string appContext = "/wisp";
            string relPath = String.Format("{0}/{1}", appContext, path);
            uriBuilder.Path = relPath;

            string fileName = Path.GetFileName(relPath);
            string contentType = MimeMapping.GetMimeMapping(relPath);

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
                    HttpContent content = response.Content;
                    var contentStream = await content.ReadAsStreamAsync(); // get the actual content stream

                    if (contentType.StartsWith("image") || contentType.Contains("pdf"))
                        return File(contentStream, contentType);

                    return File(contentStream, contentType, fileName);
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
                //return await Download(path);


                /*UriBuilder uriBuilder = new UriBuilder();
                uriBuilder.Scheme = "https";
                uriBuilder.Host = "iisgtwyt.wi.gov";
                string appContext = "/wisp";
                string relPath = String.Format("{0}/{1}/{2}", appContext, spanLength, path);
                uriBuilder.Path = relPath;

                string fileName = Path.GetFileName(relPath);
                string contentType = MimeMapping.GetMimeMapping(relPath);

                bool isFound = false;
                //uriBuilder.Path = relPath;

                //https://iisgtwyt.wi.gov/wisp/24_-20_24_42SS_yesnotch_7_CIP1034-219.zip
                //https://iisgtwyt.wi.gov/wisp/Prelim1.zip

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(uriBuilder.ToString());
                    client.DefaultRequestHeaders.Accept.Clear();
                    // For Basic Authentication http header - "username:password"
                    var byteArray = Encoding.ASCII.GetBytes("S_DOT8WISP:Kpr4kgmT8wpnFg@z");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    HttpResponseMessage response = await client.GetAsync(uriBuilder.ToString());

                    //plan.PlanFileName = fileName;
                    //plan.PlanFilePath = string.Format("{0}://{1}{2}", uriBuilder.Scheme, uriBuilder.Host, relPath);

                    if (response.IsSuccessStatusCode)
                    {
                        isFound = true;
                        //GetPlanZip(plan.PlanFilePath);
                        //FileInfo fileInfo = new FileInfo(plan.PlanFilePath);
                        //plan.FileDate = fileInfo.LastWriteTime;
                        //plan.FileFound = true;

                        //var task = Task.Run(async () => await new DownloadController().Download(relPath));
                        HttpContent content = response.Content;
                        var contentStream = await content.ReadAsStreamAsync(); // get the actual content stream
                        var temp = contentStream.ToString();
                        if (contentType.StartsWith("image") || contentType.Contains("pdf"))
                            return File(contentStream, contentType);

                        return File(contentStream, contentType, fileName);
                    }
                    else
                    {
                        throw new FileNotFoundException();
                    }
                }*/
                //return Json(new { success = plan.FileFound, plan = plan });
            }

        public async Task<ActionResult> Download()
        {
            try
            {
                HttpContent content = plan.response.Content;
                var contentStream = await content.ReadAsStreamAsync(); // get the actual content stream

                if (plan.contentType.StartsWith("image") || plan.contentType.Contains("pdf"))
                    return File(contentStream, plan.contentType);

                return File(contentStream, plan.contentType, plan.PlanFileName);
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }
        }

        public ActionResult Search()
        {
            string wamsID = this.HttpContext.Request.Headers["Authorization"];

            if (wamsID != null)
            {
                try
                {
                    byte[] data = Convert.FromBase64String(wamsID.Replace("Basic ", ""));
                    Session["wamsID"] = Encoding.UTF8.GetString(data).Split(':')[0];
                }
                catch
                {
                    Session["wamsID"] = "TMP";
                }
            }
            else
            {
                Session["wamsID"] = "TMP";
            }

            string firstName = this.HttpContext.Request.Headers["x-firstname"];
            string lastName = this.HttpContext.Request.Headers["x-lastname"];
            string email = this.HttpContext.Request.Headers["X-Mail"];

            Session["firstName"] = firstName;
            Session["lastName"] = lastName;
            Session["email"] = email;

            currUser = new User(Session["wamsID"].ToString(), firstName, lastName, email, "");
            StandardPlanQuery.ValidateUser(Session["wamsID"].ToString(), currUser.firstName, currUser.lastName, currUser.email, "", currUser.isInactive, currUser.inactiveDate);

            //var spanLengths = new List<double>();
            var spanLengths = standardPlanRepository.GetSpanLengths();
            var substructureSkews = standardPlanRepository.GetSubstructureSkews();
            var clearRoadwayWidths = standardPlanRepository.GetClearRoadwayWidths();
            var barrierTypes = standardPlanRepository.GetBarrierTypes();
            /*
            barrierTypes.Add(new BarrierType("42SS", "42SS Parapet"));
            barrierTypes.Add(new BarrierType("TypeM", "Type M Rail"));
            barrierTypes.Add(new BarrierType("TypeNY4", "Type NY4 Rail"));*/
            var pilingTypes = standardPlanRepository.GetPilingTypes();
            /*
            pilingTypes.Add(new PilingType("CIP", "CIP"));
            pilingTypes.Add(new PilingType("HP", "HP"));*/
            var abutmentHeights = standardPlanRepository.GetAbutmentHeights();

            if (DataConnector.DBfail != 0)
            {
                switch (DataConnector.DBfail)
                {
                    case 1:
                        //Response.Write("<script>alert('Error: Failed to create SQL Database Connection')</script>");
                        break;
                    case 2:
                        Response.Write("<script>alert('Error: Failed to create Oracle Database Connection " + DataConnector.EString + DataConnector.Stack + "')</script>");
                        break;
                    case 3:
                        //Response.Write("<script>alert('Error: Failed to open SQL Database Connection')</script>");
                        break;
                    case 4:
                        Response.Write("<script>alert('Error: Failed to open Oracle Database Connection " + DataConnector.EString + DataConnector.Stack + "')</script>");
                        break;
                    case 5:
                        Response.Write("<script>alert('Error: Failed to access Oracle Database Values " + DataConnector.EString + DataConnector.Stack + "')</script>");
                        break;
                    default:
                        break;
                }
            }
            else if (spanLengths.Count == 0 || substructureSkews.Count == 0 || clearRoadwayWidths.Count == 0 
                || barrierTypes.Count == 0 || pilingTypes.Count == 0 || abutmentHeights.Count == 0)
            {
                Response.Write("<script>alert('Error: Database response failed')</script>");
            }

            

            /*
           for (double i = 6.0; i <= 10.0; i++)
           {
               abutmentHeights.Add(i);
           }

           var spanLengths = new List<SelectListItem>();
           for (int i = 24; i <= 48; i += 4)
           {
               spanLengths.Add(new SelectListItem
               {
                   Value = i.ToString(),
                   Text = i.ToString()
               });
           }*/
            ViewBag.SpanLengths = spanLengths;
            ViewBag.SubstructureSkews = substructureSkews;
            ViewBag.ClearRoadwayWidths = clearRoadwayWidths;
            ViewBag.BarrierTypes = barrierTypes;
            ViewBag.AbutmentHeights = abutmentHeights;
            ViewBag.PilingTypes = pilingTypes;
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Application description and contacts info page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}