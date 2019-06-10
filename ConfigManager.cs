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
                //tode - encrypt decrypt
                return WebConfigurationManager.ConnectionStrings["ConnStringDb"].ConnectionString;                
            }
        }

        public static string MailServer
        {
            get
            {
                
                return ConfigurationManager.AppSettings["MailServer"];
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
