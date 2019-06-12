using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;


namespace Tor

{
    public class HebrewCalendarManager
    {
        static Dictionary<int, string> dicHolidays = new Dictionary<int, string>();
        static Dictionary<string, string> dicHolidaysDesc = new Dictionary<string, string>();
        public static void init()
        {
            dicHolidays[1] = ",1,2,10,15,22,";
            dicHolidays[8] = ",15,21,";
            dicHolidays[9] = ",5,";
            dicHolidays[10] = ",6,";

            dicHolidaysDesc["11"] = "ראש השנה";
            dicHolidaysDesc["12"] = "ראש השנה";
            dicHolidaysDesc["110"] = "יום הכיפורים";
            dicHolidaysDesc["115"] = "סוכות";
            dicHolidaysDesc["122"] = "שמחת תורה";
            dicHolidaysDesc["815"] = "פסח";
            dicHolidaysDesc["821"] = "שביעי של פסח";
            dicHolidaysDesc["95"] = "יום עצמאות";
            dicHolidaysDesc["106"] = "שבועות";
        }

        public static bool IsDateInHoliday(DateTime date)
        {
            Calendar HebCal = new HebrewCalendar();
            int curMonth = HebCal.GetMonth(date);
            if (dicHolidays.ContainsKey(curMonth))
            {
                string hol = dicHolidays[curMonth];
                int curDay = HebCal.GetDayOfMonth(date);
                string currentDay = curDay.ToString();

                if (curMonth == 9)//עצמאות
                {
                    
                    string azmautDay = HebCal.GetDayOfMonth(date.AddDays(1)).ToString();
                    if (hol.Contains("," + azmautDay + ",") && date.AddDays(1).DayOfWeek == DayOfWeek.Friday)
                    {
                        return true;
                    }
                    string azmautDay2 = HebCal.GetDayOfMonth(date.AddDays(2)).ToString();
                    if (hol.Contains("," + azmautDay2 + ",") && date.AddDays(2).DayOfWeek == DayOfWeek.Saturday)
                    {
                        return true;
                    }
                    string azmautDay3 = HebCal.GetDayOfMonth(date.AddDays(-1)).ToString();
                    if (hol.Contains("," + azmautDay3 + ",") && date.AddDays(-1).DayOfWeek == DayOfWeek.Monday)
                    {
                        return true;
                    }
                }
                

                if (hol.Contains("," + currentDay + ","))
                {
                    if(curMonth == 9)//עצמאות
                    {
                        if (date.DayOfWeek == DayOfWeek.Monday || date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public static List<QueData> GetHeb(DateTime from, DateTime to)
        {

            Calendar HebCal = new HebrewCalendar();



            int days = (to - from).Days;
            List<QueData> lst = new List<QueData>();

            for (int i = 0; i <= days; i++)
            {
                int curMonth = HebCal.GetMonth(from);
                if (dicHolidays.ContainsKey(curMonth))
                {
                    int curDay = HebCal.GetDayOfMonth(from);
                    string hol = dicHolidays[curMonth];
                    string currentDay = curDay.ToString();
                    if (hol.Contains("," + currentDay + ","))
                    {
                        QueData qData = new QueData();
                        if (curMonth == 9 && from.DayOfWeek == DayOfWeek.Monday)//יום עצמאות ביום שני
                        {
                            qData = getQue(from.AddDays(1), dicHolidaysDesc[curMonth.ToString() + currentDay]);
                        }
                        else if (curMonth == 9 && from.DayOfWeek == DayOfWeek.Friday)//יום עצמאות ביום שישי  
                        {
                            qData = getQue(from.AddDays(-1), dicHolidaysDesc[curMonth.ToString() + currentDay]);
                        }
                        else if (curMonth == 9 && from.DayOfWeek == DayOfWeek.Saturday)//יום עצמאות ביום שבת
                        {
                            qData = getQue(from.AddDays(-2), dicHolidaysDesc[curMonth.ToString() + currentDay]);
                        }
                        else
                            qData = getQue(from, dicHolidaysDesc[curMonth.ToString() + currentDay]);
                        
                        lst.Add(qData);
                    }

                }
                from = from.AddDays(1);
            }

            //int curYear = HebCal.GetYear(Today);    

            //int curDay = HebCal.GetDayOfMonth(Today);
            //DateTime d = HebCal.ToDateTime(curYear, curMonth, curDay, 0, 0, 0, 0);


            return lst;
        }

        private static QueData getQue(DateTime date,string title)
        {
            QueData qData = new QueData();
            qData.start = date + new TimeSpan(0, 0, 0); ;
            qData.end = date + new TimeSpan(23, 59, 59); ;
            qData.title = title;// dicHolidaysDesc[curMonth.ToString() + currentDay];
            qData.backgroundColor = "#f00";
            return qData;

        }
    }
}