using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WisDot.Bos.DatabaseMonitor.Resources
{
    public class EmailRepository
    {
        private static string[] ToList = { "DOTDLStructuresPreliminary@dot.state.wi.us" };
        //private static string[] ToList = { "luke.breyer@dot.wi.gov" };

        public static string[] parseFileName(string FileName)
        {
            return FileName.Split("_");
        }

        public static string EmailMessage(string wamsID, string StructureId, string DesignID, string ConstructionID, string FileName, string FirstName, string LastName, string Email, DateTime DownloadDate)
        {
            string message = "";
            
            string subject = String.Format("Standardized Slab Plan downloaded by {0}", wamsID);

            //string[] ccList = new string[] { "DOTDTSDStructuresProgram@dot.wi.gov" };
            //string[] bccList = new string[] { "joseph.barut@dot.wi.gov; scot.becker@dot.wi.gov" };
            //string cc = "";

            string[] FileInfo = parseFileName(FileName);

            #region Message Construction
            // Name, Wams, Email
            message = String.Format("User Name:&emsp;&emsp;&ensp;&nbsp;{0} {1}</br>", FirstName, LastName);
            message += String.Format("WamsID:&emsp;&emsp;&emsp;&emsp;{0}</br>", wamsID);
            message += String.Format("Email Address:&emsp;&ensp;{0}</br>", Email);

            message += String.Format("ConstructionID:&emsp;{0}</br>", ConstructionID);
            message += String.Format("DesignID:&emsp;&emsp;&ensp;&ensp;&nbsp;&nbsp;&nbsp;{0}</br>", DesignID);
            message += String.Format("StructureId:&emsp;&emsp;&ensp;&nbsp;{0}</br>", StructureId);

            message += String.Format("SpanLength:&emsp;&emsp;&nbsp;&nbsp;&nbsp;{0}</br>", FileInfo[0]);
            message += String.Format("Skew:&emsp;&emsp;&emsp;&emsp;&ensp;&nbsp;&nbsp;&ensp;{0}</br>", FileInfo[1].Contains('-') ? FileInfo[1] : String.Format("+{0}", FileInfo[1]));
            message += String.Format("Width:&emsp;&emsp;&emsp;&ensp;&ensp;&ensp;&ensp;{0}</br>", FileInfo[2]);
            message += String.Format("Barrier:&emsp;&emsp;&emsp;&ensp;&nbsp;&ensp;&ensp;{0}</br>", FileInfo[3]);
            message += String.Format("PavingNotch:&emsp;&emsp;&nbsp;{0}</br>", FileInfo[4].Contains("yes") ? "Yes" : "No");
            message += String.Format("AbutHeight:&emsp;&emsp;&nbsp;&nbsp;&nbsp;{0}</br>", FileInfo[5]);
            message += String.Format("Piles:&emsp;&emsp;&emsp;&emsp;&ensp;&emsp;&nbsp;{0}</br>", FileInfo[6]);
            message += String.Format("FileID:&emsp;&emsp;&emsp;&emsp;&nbsp;&emsp;&nbsp;{0}</br>", FileName);

            message += String.Format("Download Date:&emsp;{0}</br>", DownloadDate.Date.ToShortDateString());
            #endregion

            ComposeMessage(subject, message, ToList);
            return message;
        }

        public static bool ComposeMessage(string subject, string message, string[] to, [Optional] string[] attachments, [Optional] string[] cc, [Optional] string[] bcc, [Optional] MailPriority mailPriority)
        {
            /* Default is a successful sending of the email, but might want to change default logic to be false (instead of true) */
            bool successfulSend = true;

            /* In App.Config files (there are 3 app.config files in this entire project) */
            //string from = ConfigurationManager.AppSettings.GetValues("EmailAddress")[0];
            string from = "DOTDTSDStructuresStandardPlans@dot.wi.gov";

            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();

            /* The following lines are requirements either from BITS or SMTP */
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mailx.dot.state.wi.us";
            mail.From = new MailAddress(from);

            if (!IsNullOrEmpty(to) && !to[0].Equals(""))
            {
                foreach (string recipient in to)
                {
                    mail.To.Add(recipient);
                }
            }
            else
            {
                successfulSend = false;
            }

            if (!IsNullOrEmpty(attachments) && !attachments[0].Equals(""))
            {
                foreach (string file in attachments)
                {
                    mail.Attachments.Add(new Attachment(file));
                }
            }

            mail.CC.Add(from);
            if (!IsNullOrEmpty(cc) && !cc[0].Equals(""))
            {
                foreach (string person in cc)
                {
                    mail.CC.Add(person);
                }
            }

            if (!IsNullOrEmpty(bcc) && !bcc[0].Equals(""))
            {
                foreach (string person in bcc)
                {
                    mail.Bcc.Add(person);
                }
            }

            if (!string.IsNullOrWhiteSpace(subject) && !string.IsNullOrWhiteSpace(message))
            {
                mail.Subject = subject;
                mail.Body = message;
            }
            else
            {
                successfulSend = false;
            }

            mail.IsBodyHtml = true;

            /* Making sure that email has all the parameters needed to send correctly, required params are To, Subject, Body */
            try
            {
                if (successfulSend)
                    client.Send(mail);
            }
            catch (Exception e)
            {
                successfulSend = false;
            }

            mail.Dispose();
            client.Dispose();

            /* Return true on successful sending of email */
            return successfulSend;
        }

        public static bool IsNullOrEmpty(Array array)
        {
            return (array == null || array.Length == 0);
        }
    }
}
