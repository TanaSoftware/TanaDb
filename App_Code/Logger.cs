using System;
using System.IO;

namespace Tor
{
    public class Logger
    {
        static char DelimeterChar = ',';
        public static void Write(string msg)
        {
            WriteToFile(" Message: "+ msg);
        }

        public static void Write(Exception ex)
        {
            WriteToFile(" Exception: " + ex.Message);
        }
        private static void WriteToFile(string text)
        {
            string date = DateTime.Now.ToShortDateString();
            string time = DateTime.Now.ToLongTimeString();
            string str = time + DelimeterChar + text;

            string fileName = "ErrLog_" + date;
            fileName = fileName.Replace(':', '_');
            fileName = fileName.Replace('/', '_');

            string path = GetPath() + "\\App_Data\\"+ fileName;
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(str);
            }
        }

        private static string GetPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
