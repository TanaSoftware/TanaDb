using System.Configuration;

namespace Tor
{
    public class ConfigManager

    {

        public static string ConnectionString
        {

            get

            {

                return ConfigurationManager.ConnectionStrings["MyTorConn"].ConnectionString;

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
