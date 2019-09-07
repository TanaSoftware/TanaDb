using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Tor

{
    public class MailSender
    {
        public static bool sendMail(string subject, string content, string[] recipients, string from)
        {
            if (recipients == null || recipients.Length == 0)
                throw new ArgumentException("recipients");

            var gmailClient = new System.Net.Mail.SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("shilacoh3@gmail.com", "honey3moon"),
                TargetName = "STARTTLS/smtp.gmail.com",     
                           
        };

            //byte[] bytes = Encoding.Default.GetBytes(content);
            //string EncContent = Encoding.UTF8.GetString(bytes);

            //byte[] bytesX = Encoding.Default.GetBytes(subject);
            //string EncSubject = Encoding.UTF8.GetString(bytesX);
            using (var msg = new System.Net.Mail.MailMessage(from, recipients[0], subject , content))
            {
                //msg.HeadersEncoding = Encoding.UTF8;
                //msg.BodyEncoding = Encoding.UTF8;
                for (int i = 1; i < recipients.Length; i++)
                    msg.To.Add(recipients[i]);

                try
                {
                    msg.IsBodyHtml = true;
                    gmailClient.Send(msg);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                    return false;
                }
            }
        }

        private static string BuildMessageBody(string content)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http-equiv='Content-Type' content='text/html; charset=windows-1255'>");
            sb.AppendLine("</head>");

            sb.AppendLine("<body>");
            sb.AppendLine("<div dir='rtl' style='font-family:arial;'> ");
            sb.AppendLine(content);
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("<html>");

            return sb.ToString();
        }
        public static bool sendMail2(string mailTo,string guid,string title,string body,string url)

        {

            try

            {

                MailMessage mail = new MailMessage(ConfigManager.MailFrom, mailTo);

                SmtpClient client = new SmtpClient();

                client.UseDefaultCredentials = true;

                client.Host = ConfigManager.MailServer;

                mail.Subject = title;
                string baseUerl = ConfigManager.BaseUrl;
                mail.Body = body + baseUerl + url + guid;
                mail.BodyEncoding = Encoding.UTF8;
                client.Send(mail);

            }

            catch (Exception ex)

            {

                Logger.Write(ex);

                return false;

            }

            return true;

        }
    }
}