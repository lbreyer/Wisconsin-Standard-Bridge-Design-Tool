using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Wisdot.Bos.DatabaseNotifier.Resources;

namespace Wisdot.Bos.DatabaseNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable top = EmailQuery.GetTopDownload();
            DataTable User;
            int downloadID = -1;
            int sentCount = 0;

            if (top != null)
            {
                int rowCount = EmailQuery.CountRows();
                downloadID = (int)top.Rows[0]["DownloadId"];
                while (top != null && (top.Rows[0]["NotificationSent"] == System.DBNull.Value || (bool)top.Rows[0]["NotificationSent"] != true))
                {
                    if (top.Rows[0]["WamsId"].ToString() != "TMP")
                    {
                        User = EmailQuery.GetUser(top.Rows[0]["WamsId"].ToString());
                        EmailRepository.EmailMessage(top.Rows[0]["WamsId"].ToString(), top.Rows[0]["StructureId"].ToString(),
                            top.Rows[0]["DesignId"].ToString(), top.Rows[0]["FiipsConstructionId"].ToString(), top.Rows[0]["FileName"].ToString(),
                            User.Rows[0]["FirstName"].ToString(), User.Rows[0]["LastName"].ToString(), User.Rows[0]["EmailAddress"].ToString(),
                            DateTime.Parse(top.Rows[0]["DownloadDate"].ToString()).Date);
                    }
                    EmailQuery.EmailSent((int)top.Rows[0]["DownloadId"]);
                    sentCount++;

                    do
                    {
                        downloadID--;

                        top = EmailQuery.GetDownloadHistory(downloadID);
                    }
                    while (top == null && sentCount != rowCount);
                }
            }
        }
    }
}
