using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;
using System.Diagnostics;
using Tor.Dal;
using System.Threading.Tasks;

namespace Tor

{
    public class QueDataMessage
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Email { get; set; }

        public string ActivityName { get; set; }

        public string CustomerName { get; set; }

        public string userName { get; set; }

        public string UserMail { get; set; }

        public int QueId { get; set; }

        public string CustId { get; set; }
    }
    public class MessageObserver
    {

        private static Timer TraceTimer = new Timer(600000 );
        private static object _sync = new object();
        static string baseUerl = ConfigManager.BaseUrl;
        public MessageObserver()
        {

        }



        public void Run()
        {

            TraceTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            TraceTimer.Enabled = true;
        }

        private void Check()
        {

            lock (_sync)
            {
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
                DateTime d = DateTime.Today.AddDays(1);
                DateTime FromDate = d + new TimeSpan(0, 0, 0); ;
                DateTime ToDate = d + new TimeSpan(23, 59, 0);

                var sql = @"SELECT top 100 A.FromDate,A.ToDate,B.Email,B.tel,U.Name as [ActivityName],
                            B.Name as [CustomerName],Us.BizName as [userName],Us.Email as [UserMail],A.id as QueId,B.Id As CustId
                            FROM Que A INNER JOIN Customer B on A.CustomerId = B.Id                       
                                 JOIN UsersActivitiesTypes U ON U.Id = A.QueType  
	                             JOIN Users Us On Us.Id = A.UserId 
                        WHERE (A.IsMessageSent=0 Or A.IsMessageSent=Null) And A.FromDate>=@FromDate And A.ToDate<=@ToDate
                        ORDER BY A.FromDate asc";

                SecureData sec = new SecureData();
                try
                {
                    IEnumerable<QueDataMessage> QueDataRows = db.QueryData<QueDataMessage>(sql, 1, new { FromDate = FromDate, ToDate = ToDate });
                    foreach (QueDataMessage que in QueDataRows)
                    {
                        string subject = "הודעה מ" + que.userName;
                        string content = "שלום" + "<BR><BR>" + "מחר יש לך תור ל" + que.ActivityName + " בתאריך " + que.FromDate.ToShortDateString() + " בשעה " + que.FromDate.ToShortTimeString() + " עד " + que.ToDate.ToShortTimeString();
                        content += "<BR><BR>";
                        string encriptedQueId = sec.EncryptTextWithNoSpecialCharecters(que.QueId.ToString(), "shalomHa!@");
                        content += "לאישור הגעתך אנא " + " <a href='" + baseUerl + "/Que/Approve/" + encriptedQueId + "'>לחץ כאן לאישור</a>";
                        content += "<BR><BR>";
                        content += "לביטול התור" + " <a href='" + baseUerl + "/Que/Cancel/" + encriptedQueId + "'>לחץ כאן לביטול</a>";
                        string[] arr = new string[1];
                        arr[0] = que.Email;
                        bool isSent = MailSender.sendMail(subject, content, arr, que.UserMail);
                        if (isSent)
                            Task.Run(() => { updateMessageStatus(que.QueId); });
                    }
                }
                catch(Exception ex)
                {
                    Logger.Write(ex);
                }
            }
        }
        private void updateMessageStatus(int queId)
        {
            try
            {
                string sqlUpdate = "UPDATE Que SET [IsMessageSent]=1 where [id]=@QueId;";
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
                var affectedRows = db.Execute(sqlUpdate, 1, new { QueId = queId });
            }
            catch(Exception ex)
            {
                Logger.Write(ex);
            }
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Check();
        }

    }
}