using System.Configuration;
using System.Web.Configuration;

namespace Tor
{
    public class ConfigManager
    {
        public static string ConnectionString
        {
            get
            {                
                return WebConfigurationManager.ConnectionStrings["MyTorConn"].ConnectionString;                
            }
        }

        public static string MailServer
        {
            get
            {
                
                return ConfigurationManager.AppSettings["MailServer"];
            }
        }

        public static string BaseUrl
        {
            get
            {

                return ConfigurationManager.AppSettings["BaseUrl"];
            }
        }

        public static string MailFrom
        {
            get
            {

                return ConfigurationManager.AppSettings["MailFrom"];
            }
        }

        public static string MailTo
        {
            get
            {

                return ConfigurationManager.AppSettings["MailTo"];
            }
        }

        public static string version
        {

            get

            {

                return ConfigurationManager.AppSettings["ver"];

            }

        }
    }
}
