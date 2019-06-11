
using System;

using System.ComponentModel.DataAnnotations;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using Tor.Dal;
using System.Globalization;

namespace Tor

{

    public class QueObj

    {

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        public string guid { get; set; }

        [Required]
        public bool isUser { get; set; }

        public int CustomerId { get; set; }

        public int EmployeeId { get; set; }
    }

    public class Que : QueObj
    {
        [Required]
        public string QueType { get; set; }


    }

    public class QueData

    {

        public int Id { get; set; }

        public string title { get; set; }

        public DateTime start { get; set; }

        public DateTime end { get; set; }

        public string backgroundColor { get; set; }

    }

    public class QueActiveData

    {

        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public string EmployeeName { get; set; }
        public int ActiveDay { get; set; }
        public DateTime ActiveHourFrom { get; set; }

        public DateTime ActiveHourTo { get; set; }

        public DateTime ActiveHourFromNone { get; set; }

        public DateTime ActiveHourToNone { get; set; }

    }

    public class QueDataWrapper

    {

        public IEnumerable<QueData> queData { get; set; }

        public string Message { get; set; }







    }

    public class QueManager

    {

        public List<QueData> GetQue(QueObj que)

        {
            List<QueData> lst = new List<QueData>();
            //QueDataWrapper queWrapper = new QueDataWrapper();



            if (que.isUser)

            {

                if (!IsUserExist(que.guid))

                {

                    //Logger.Write("Error user doesnt exists");

                    //queWrapper.Message = "ארעה שגיאה";

                    return lst;

                }

            }

            else

            {

                if (!IsCustomerExist(que.guid))

                {

                    //Logger.Write("Error customer doesnt exists");

                    //queWrapper.Message = "ארעה שגיאה";

                    return lst;

                }

            }

            try

            {

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                string select = que.isUser ? "U.Name + ' ' + B.Name" : "CASE When B.Id=@CustomerId THEN U.Name ELSE N'תפוס' END";
                //string select = que.isUser ? "": "";
                var sql = @"SELECT A.id," + select + @" As [title],A.FromDate As [start],A.ToDate As [end]

                        FROM Que A INNER JOIN Customer B on A.CustomerId = B.Id
                        JOIN UsersActivitiesTypes U ON U.Id = A.QueType
                        WHERE A.UserId = @UserId And EmployeeId = @EmployeeId And FromDate>=@FromDate And ToDate<=@ToDate

                        ORDER BY start asc";



                IEnumerable<QueData> QueDataRows = db.QueryData<QueData>(sql, 1, new { UserId = que.UserId, FromDate = que.FromDate, ToDate = que.ToDate, CustomerId = que.CustomerId, EmployeeId = que.EmployeeId });

                var sqlEmp = "Select * FROM UsersActivity WHERE EmployeeId=@EmployeeId And UserId=@UserId";// And ActiveHourFromNone is not null

                IEnumerable<QueActiveData> BusyQue = db.QueryData<QueActiveData>(sqlEmp, 1, new { EmployeeId = que.EmployeeId, UserId = que.UserId });
                if (BusyQue.Count() > 0)
                {
                    var totalDays = (que.ToDate - que.FromDate).TotalDays;
                    Dictionary<int, QueActiveData> dicQ = new Dictionary<int, QueActiveData>();
                    foreach (QueActiveData q in BusyQue)
                    {
                        if (!dicQ.ContainsKey(q.ActiveDay))
                            dicQ.Add(q.ActiveDay, q);

                    }

                    if (totalDays <= 1)
                    {
                        int currentDay = GetCurrentDay(que.FromDate);
                        if (dicQ.ContainsKey(currentDay))
                        {
                            if (dicQ[currentDay].ActiveHourFromNone.Year != 1)
                            {
                                QueData qData = new QueData();
                                qData.start = dicQ[currentDay].ActiveHourFromNone;
                                qData.end = dicQ[currentDay].ActiveHourToNone;
                                qData.title = "תפוס";
                                qData.backgroundColor = "#f00";
                                lst.Add(qData);
                            }
                        }
                        else
                        {
                            QueData qData = new QueData();
                            qData.start = que.FromDate.Date + new TimeSpan(0, 0, 0);
                            qData.end = que.FromDate.Date + new TimeSpan(23, 59, 0);
                            qData.title = "תפוס";
                            qData.backgroundColor = "#f00";
                            lst.Add(qData);
                        }
                    }
                    if (totalDays > 6 && totalDays <= 7)
                    {
                        DateTime dt = StartOfWeek(que.FromDate.Date, DayOfWeek.Sunday);
                        for (int i = 1; i <= 7; i++)
                        {
                            if (dicQ.ContainsKey(i))
                            {
                                if (dicQ[i].ActiveHourFromNone.Year != 1)
                                {
                                    QueData qData = new QueData();

                                    TimeSpan tsFrom = new TimeSpan(dicQ[i].ActiveHourFromNone.Hour, dicQ[i].ActiveHourFromNone.Minute, 0);
                                    qData.start = (dt + tsFrom);

                                    TimeSpan tsTo = new TimeSpan(dicQ[i].ActiveHourToNone.Hour, dicQ[i].ActiveHourToNone.Minute, 0);
                                    qData.end = (dt + tsTo);
                                    qData.title = "תפוס";
                                    qData.backgroundColor = "#f00";
                                    lst.Add(qData);
                                }
                            }
                            else
                            {
                                QueData qData = new QueData();
                                qData.start = dt + new TimeSpan(0, 0, 0);
                                qData.end = dt + new TimeSpan(23, 59, 0);
                                qData.title = "תפוס";
                                qData.backgroundColor = "#f00";
                                lst.Add(qData);
                            }
                            dt = dt.AddDays(1);

                        }
                    }
                    if (totalDays > 7)
                    {

                        var startDate = new DateTime(que.FromDate.Year, que.FromDate.Month, 1);
                        //var endDate = startDate.AddMonths(1).AddDays(-1);
                        for (int i = 0; i < totalDays; i++)
                        {
                            int currentDay = GetCurrentDay(startDate);
                            if (dicQ.ContainsKey(currentDay))
                            {
                                if (dicQ[currentDay].ActiveHourFromNone.Year != 1)
                                {
                                    QueData qData = new QueData();

                                    TimeSpan tsFrom = new TimeSpan(dicQ[currentDay].ActiveHourFromNone.Hour, dicQ[currentDay].ActiveHourFromNone.Minute, 0);
                                    qData.start = (startDate + tsFrom);

                                    TimeSpan tsTo = new TimeSpan(dicQ[currentDay].ActiveHourToNone.Hour, dicQ[currentDay].ActiveHourToNone.Minute, 0);
                                    qData.end = (startDate + tsTo);
                                    string fromHour = dicQ[currentDay].ActiveHourFromNone.Hour.ToString();
                                    string fromMin = dicQ[currentDay].ActiveHourFromNone.Minute.ToString();
                                    string toHour = dicQ[currentDay].ActiveHourToNone.Hour.ToString();
                                    string toMin = dicQ[currentDay].ActiveHourToNone.Minute.ToString();

                                    //fromMin = fromMin=="0" ? fromMin += "00" : fromMin;
                                    //toHour = toHour.EndsWith(":0") ? toHour += "00" : toHour;
                                    //toMin = toMin == "0" ? toMin += "0" : toMin;
                                    qData.title = "תפוס" + " מ " + fromHour + ":" + fromMin + " עד " + toHour + ":" + toMin;
                                    qData.backgroundColor = "#f00";
                                    lst.Add(qData);
                                }
                            }
                            else
                            {
                                QueData qData = new QueData();
                                qData.start = startDate + new TimeSpan(0, 0, 0);
                                qData.end = startDate + new TimeSpan(23, 59, 0);
                                qData.title = "תפוס";
                                qData.backgroundColor = "#f00";
                                lst.Add(qData);
                            }
                            startDate = startDate.AddDays(1);
                        }
                    }
                }

                if (QueDataRows.Count() > 0)
                {
                    lst.AddRange(QueDataRows);
                }


            }

            catch (Exception ex)

            {

                Logger.Write("QueManager:GetQue:" + ex);

                //queWrapper.Message = "ארעה שגיאה";

            }



            return lst;

        }

        private List<QueData> GetHeb(DateTime from, DateTime to)
        {
            Dictionary<int, int> di = new Dictionary<int, int>();
            DateTime Today = new DateTime(2019,6,9);
            List<QueData> lst = new List<QueData>();
            Calendar HebCal = new HebrewCalendar();
            int curYear = HebCal.GetYear(Today);    //current numeric hebrew year
            int curMonth = HebCal.GetMonth(Today);
            int curDay = HebCal.GetDayOfMonth(Today);
            DateTime d = HebCal.ToDateTime(curYear, curMonth, curDay, 0, 0, 0, 0);
            QueData qData = new QueData();
            //qData.start = startDate + new TimeSpan(0, 0, 0);
            //qData.end = startDate + new TimeSpan(23, 59, 0);
            qData.title = "תפוס";
            qData.backgroundColor = "#f00";
            lst.Add(qData);
            return lst;
        }
        public int GetCurrentDay(DateTime dt)
        {
            int currentDay = (int)dt.DayOfWeek;

            return currentDay + 1;

        }
        public DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public string AddCustomerQue(Que que)

        {

            if (que.isUser)

            {

                if (!IsUserExist(que.guid))

                {

                    return "ארעה שגיאה";

                }

            }

            else

            {

                if (!IsCustomerExist(que.guid))

                {

                    //Logger.Write("Error customer doesnt exists");

                    return "ארעה שגיאה";

                }
                if (!IsCustomerQueAvailable(que))
                    return "לא ניתן להזמין תור בזמן הזה";

            }

            if (IsCustomerQueExist(que))
            {
                return "תור תפוס, אנא קבע במועד אחר";
            }

            if (!AddQueToDb(que))
            {
                Logger.Write("Error in insert data to Que table, CustomerId: " + que.CustomerId + ", userId: " + que.UserId);

                return "ארעה שגיאה";



            }

            return "";
        }


        private bool IsCustomerQueExist(Que que)
        {

            string sql = "SELECT count(1) FROM Que WHERE [UserId] = @UserId And EmployeeId = @EmployeeId And [FromDate]>=@FromDate And [ToDate]<=@ToDate;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            return db.IsExist(sql, new { UserId = que.UserId, FromDate = que.FromDate, ToDate = que.ToDate, EmployeeId = que.EmployeeId }, 1);

        }



        private bool IsUserExist(string guid)

        {

            string sql = "SELECT count(1) FROM Users WHERE [Active] = @guid;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            return db.IsExist(sql, new { guid = guid }, 1);

        }

        private bool IsCustomerExist(string guid)

        {

            string sql = "SELECT count(1) FROM Customer WHERE [Active] = @guid;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            return db.IsExist(sql, new { guid = guid }, 1);

        }

        private bool IsCustomerQueAvailable(Que que)

        {

            int currentDay = GetCurrentDay(que.FromDate);
            var sql = @"Select * FROM UsersActivity WHERE EmployeeId = @EmployeeId And UserId = @UserId And ActiveDay=@ActiveDay";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            IEnumerable<QueActiveData> BusyQue =
                db.QueryData<QueActiveData>(sql, 1, new { EmployeeId = que.EmployeeId, UserId = que.UserId, ActiveDay = currentDay });

            if (BusyQue.Count() <= 0)
                return false;

            foreach (QueActiveData q in BusyQue)
            {
                TimeSpan tsFr = new TimeSpan(q.ActiveHourFrom.Hour, q.ActiveHourFrom.Minute, 0);
                DateTime ActiveHourFrom = (que.FromDate.Date + tsFr);

                TimeSpan tsT = new TimeSpan(q.ActiveHourTo.Hour, q.ActiveHourTo.Minute, 0);
                DateTime ActiveHourTo = (que.ToDate.Date + tsT);

                if (que.FromDate < ActiveHourFrom)
                    return false;

                if (que.FromDate >= ActiveHourTo)
                    return false;

                if (que.ToDate < ActiveHourFrom)
                    return false;

                if (que.ToDate > ActiveHourTo)
                    return false;

                if (q.ActiveHourFromNone.Year != 1)//there is hour in table
                {
                    TimeSpan tsFrom = new TimeSpan(q.ActiveHourFromNone.Hour, q.ActiveHourFromNone.Minute, 0);
                    DateTime ActiveHourFromNone = (que.FromDate.Date + tsFrom);
                    TimeSpan tsTo = new TimeSpan(q.ActiveHourToNone.Hour, q.ActiveHourToNone.Minute, 0);
                    DateTime ActiveHourToNone = (que.ToDate.Date + tsTo);

                    if (que.FromDate >= ActiveHourFromNone && que.FromDate < ActiveHourToNone)
                        return false;
                    if (que.ToDate >= ActiveHourFromNone && que.ToDate <= ActiveHourToNone)
                        return false;
                }
            }
            return true;

        }

        private bool AddQueToDb(Que que)

        {

            bool Issuccess = false;

            try

            {

                string sqlQueInsert = @"INSERT INTO Que ([UserId],[FromDate],[ToDate],[QueType],[EmployeeId],[CustomerId]) Values

(@UserId,@FromDate,@ToDate,@QueType,@EmployeeId,@CustomerId);";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                var affectedRows = db.Execute(sqlQueInsert, 1, que);

            }

            catch (Exception ex)

            {

                Logger.Write(ex);

                Issuccess = false;

            }



            Issuccess = true;

            return Issuccess;

        }





        public string DeleteCustomerQue(Que que)

        {

            if (que.isUser)

            {

                if (!IsUserExist(que.guid))

                {

                    return "ארעה שגיאה";

                }

            }

            else

            {

                if (!isCustomerAllowDeleteHisQue(que))

                {

                    //Logger.Write("Error customer doesnt exists");

                    return "ארעה שגיאה";

                }

            }

            if (!DeleteQueFromDb(que))

            {

                return "ארעה שגיאה";

            }

            return "";

        }



        private bool isCustomerAllowDeleteHisQue(Que que)

        {

            var sql = @"SELECT count(1)

                        FROM Que A INNER JOIN Customer B on A.CustomerId = B.Id

                        WHERE A.UserId = @UserId And A.FromDate=@FromDate And A.ToDate=@ToDate And
                        A.QueType = @QueType And

                        A.CustomerId = @CustomerId And A.EmployeeId=@EmployeeId And B.Active=@guid";

            //string sql = "SELECT count(1) FROM Customer WHERE [guid] = @guid;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            return db.IsExist(sql, que, 1);

        }

        private bool DeleteQueFromDb(Que que)

        {

            bool Issuccess = false;

            try

            {

                string sqlQueDelter = @"DELETE From Que Where [UserId]=@UserId And [CustomerId] = @CustomerId And [EmployeeId] = @EmployeeId And [FromDate]=@FromDate And [ToDate]=@ToDate And [QueType]=@QueType";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                var affectedRows = db.Execute(sqlQueDelter, 1, que);

            }

            catch (Exception ex)

            {

                Logger.Write(ex);

                Issuccess = false;

            }



            Issuccess = true;

            return Issuccess;

        }

    }

}


