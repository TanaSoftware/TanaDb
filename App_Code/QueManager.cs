
using System;

using System.ComponentModel.DataAnnotations;

using System.Collections.Generic;

using System.Linq;

using Tor.Dal;
using System.Threading.Tasks;

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

        public int GroupId { get; set; }
    }

    public class Que : QueObj
    {
        [Required]
        public string QueType { get; set; }


    }

    public class QueData

    {

        public int id { get; set; }
        public int CustomerId { get; set; }
        public string title { get; set; }

        public DateTime start { get; set; }

        public DateTime end { get; set; }

        public string backgroundColor { get; set; }

        public int QueType { get; set; }

    }

    public class QueActiveData

    {
        public int ActiveDuration { get; set; }
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

    public class DeleteGroupCustomer
    {
        public string guid { get; set; }

        public int CustomerId { get; set; }

        public int GroupId { get; set; }
    }

    public class QueManager

    {

        private bool IsDateInDateList(DateTime date, List<UserExtraActivity> extra)
        {
            return extra.Exists(x => x.From == date);
        }
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

                string select = "";
                if (que.isUser)
                    select = "Case when g.Name is not null then U.Name + ' ' +g.Name Else U.Name + ' ' + B.Name End ";
                else
                    select = "CASE When B.Id=@CustomerId THEN Case when U.Name + ' ' +g.Name is not null then g.Name Else U.Name End ELSE N'תפוס' END ";

                var sql = @"SELECT A.[CustomerId],A.id,A.[QueType], " + select + @" As [title],A.FromDate As [start],A.ToDate As [end],U.[color] as backgroundColor

                        FROM Que A INNER JOIN Customer B on A.CustomerId = B.Id
                        JOIN UsersActivitiesTypes U ON U.Id = A.QueType
                        Left join [Groups] g on A.GroupId = g.Id
                        WHERE A.UserId = @UserId And EmployeeId = @EmployeeId And FromDate>=@FromDate And ToDate<=@ToDate

                        ORDER BY start asc";



                IEnumerable<QueData> QueDataRows = db.QueryData<QueData>(sql, 1, new { UserId = que.UserId, FromDate = que.FromDate, ToDate = que.ToDate, CustomerId = que.CustomerId, EmployeeId = que.EmployeeId });

                var sqlEmp = "Select * FROM UsersActivity WHERE EmployeeId=@EmployeeId And UserId=@UserId";// And ActiveHourFromNone is not null

                IEnumerable<QueActiveData> BusyQue = db.QueryData<QueActiveData>(sqlEmp, 1, new { EmployeeId = que.EmployeeId, UserId = que.UserId });
                if (BusyQue.Count() > 0)
                {

                    var sqlExtraEmp = "Select * FROM [UserExtraActivity] WHERE EmployeeId=@EmployeeId And UserId=@UserId";// And ActiveHourFromNone is not null
                    IEnumerable<UserExtraActivity> extra = db.QueryData<UserExtraActivity>(sqlExtraEmp, 1, new { EmployeeId = que.EmployeeId, UserId = que.UserId });
                    List<UserExtraActivity> userExtraActivity = new List<UserExtraActivity>();
                    foreach (UserExtraActivity extraAct in extra)
                    {
                        UserExtraActivity userExtra = new UserExtraActivity();
                        userExtra = extraAct;
                        userExtraActivity.Add(userExtra);
                    }

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
                            if (dicQ[currentDay].ActiveHourFromNone.Year != 1 && !HebrewCalendarManager.IsDateInHoliday(que.FromDate) && !IsDateInDateList(que.FromDate, userExtraActivity))
                            {
                                QueData qData = new QueData();
                                qData.start = dicQ[currentDay].ActiveHourFromNone;
                                qData.end = dicQ[currentDay].ActiveHourToNone;
                                qData.title = "תפוס";
                                qData.backgroundColor = "#870A30";
                                lst.Add(qData);
                            }
                        }
                        else
                        {
                            if (!HebrewCalendarManager.IsDateInHoliday(que.FromDate))
                            {
                                QueData qData = new QueData();
                                qData.start = que.FromDate.Date + new TimeSpan(0, 0, 0);
                                qData.end = que.FromDate.Date + new TimeSpan(23, 59, 0);
                                qData.title = "תפוס";
                                qData.backgroundColor = "#870A30";
                                lst.Add(qData);
                            }
                        }
                    }
                    if (totalDays > 6 && totalDays <= 7)
                    {
                        DateTime dt = StartOfWeek(que.FromDate.Date, DayOfWeek.Sunday);
                        for (int i = 1; i <= 7; i++)
                        {
                            if (dicQ.ContainsKey(i))
                            {
                                if (dicQ[i].ActiveHourFromNone.Year != 1 && !HebrewCalendarManager.IsDateInHoliday(dt) && !IsDateInDateList(dt, userExtraActivity))
                                {
                                    QueData qData = new QueData();

                                    TimeSpan tsFrom = new TimeSpan(dicQ[i].ActiveHourFromNone.Hour, dicQ[i].ActiveHourFromNone.Minute, 0);
                                    qData.start = (dt + tsFrom);

                                    TimeSpan tsTo = new TimeSpan(dicQ[i].ActiveHourToNone.Hour, dicQ[i].ActiveHourToNone.Minute, 0);
                                    qData.end = (dt + tsTo);
                                    qData.title = "תפוס";
                                    qData.backgroundColor = "#870A30";
                                    lst.Add(qData);
                                }
                            }
                            else
                            {
                                if (!HebrewCalendarManager.IsDateInHoliday(dt) && !IsDateInDateList(dt, userExtraActivity))
                                {
                                    QueData qData = new QueData();
                                    qData.start = dt + new TimeSpan(0, 0, 0);
                                    qData.end = dt + new TimeSpan(23, 59, 0);
                                    qData.title = "תפוס";
                                    qData.backgroundColor = "#870A30";
                                    lst.Add(qData);
                                }
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
                                if (dicQ[currentDay].ActiveHourFromNone.Year != 1 && !HebrewCalendarManager.IsDateInHoliday(startDate))
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
                                    qData.backgroundColor = "#870A30";
                                    lst.Add(qData);
                                }
                            }
                            else
                            {
                                if (!HebrewCalendarManager.IsDateInHoliday(startDate) && !IsDateInDateList(startDate, userExtraActivity))
                                {
                                    QueData qData = new QueData();
                                    qData.start = startDate + new TimeSpan(0, 0, 0);
                                    qData.end = startDate + new TimeSpan(23, 59, 0);
                                    qData.title = "תפוס";
                                    qData.backgroundColor = "#870A30";
                                    lst.Add(qData);
                                }
                            }
                            startDate = startDate.AddDays(1);
                        }
                    }
                }

                if (QueDataRows.Count() > 0)
                {
                    lst.AddRange(QueDataRows);
                }

                List<QueData> hebList = HebrewCalendarManager.GetHeb(que.FromDate, que.ToDate);
                if (hebList.Count > 0)
                    lst.AddRange(hebList);
            }

            catch (Exception ex)

            {

                Logger.Write("QueManager:GetQue:" + ex);

                //queWrapper.Message = "ארעה שגיאה";

            }



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
                //if(que.CustomerId==0)
                //{
                //    int custId = IsCustomerHolidayExist("חופש");
                //    if(custId==0)
                //    {
                //        string sqlCustomerInsert = @"INSERT INTO Customer ([Name],[Email],[Password],[tel],[guid],[City]) Values

                //                ('חופש','','','','','');";

                //        DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                //        var affectedRows = db.Execute(sqlCustomerInsert, 1, null);
                //        custId=IsCustomerHolidayExist("חופש");
                //    }

                //    que.CustomerId = custId;
                    
                //}
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
            DateTime from = que.FromDate.AddMilliseconds(10);
            DateTime to = que.ToDate.AddMilliseconds(-10);
            string sql = @"SELECT count(1) FROM Que 
                           WHERE [UserId] = @UserId And EmployeeId = @EmployeeId And 
                             (([FromDate] between @FromDate 
	                            And @ToDate) Or
	                            ([ToDate] between @FromDate
	                            And @ToDate ))";                           


            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            return db.IsExist(sql, new { UserId = que.UserId, FromDate = from, ToDate = to, EmployeeId = que.EmployeeId }, 1);
            //IEnumerable<int> data =
            //     db.QueryData<int>(sql, 1, new { UserId = que.UserId, FromDate = que.FromDate, ToDate = que.ToDate, EmployeeId = que.EmployeeId });

            //bool isExist = false;
            //foreach(int i in data)
            //{
            //    isExist = true;
            //}
            //return isExist;
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

        private int IsCustomerHolidayExist(string name)

        {
            int CustId = 0;
            string sql = "SELECT Id FROM Customer WHERE [Name] = @name;";
            try
            {
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                IEnumerable<int> customers = db.QueryData<int>(sql, 1, new { name = name });
                foreach (int x in customers)
                {
                    CustId = x;
                    break;
                }
            }
            catch
            {
                CustId = 0;
            }
            return CustId;
        }

        private bool IsCustomerQueAvailable(Que que)

        {

            int currentDay = GetCurrentDay(que.FromDate);
            //var sql = @"Select * FROM UsersActivity WHERE EmployeeId = @EmployeeId And UserId = @UserId And ActiveDay=@ActiveDay";
            string sql = @"SELECT UA.EmployeeId
                          ,UAT.[ActiveDuration]
	                      ,UA.UserId
	                      ,UA.EmployeeName
	                      ,UA.ActiveDay
	                      ,UA.ActiveHourFrom
	                      ,UA.ActiveHourTo
	                      ,UA.ActiveHourFromNone
	                      ,UA.ActiveHourToNone
                      FROM [UsersActivitiesTypes] UAT
                      INNER JOIN UsersActivity UA
                      on UAT.UserId = UA.UserId
                      where UA.EmployeeId = @EmployeeId And UA.UserId = @UserId And ActiveDay=@ActiveDay and UAT.Id=@QueType";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            IEnumerable<QueActiveData> BusyQue =
                db.QueryData<QueActiveData>(sql, 1, new { EmployeeId = que.EmployeeId, UserId = que.UserId, ActiveDay = currentDay, QueType= que.QueType });

            if (BusyQue.Count() <= 0)
                return false;


            TimeSpan iDuration = que.ToDate - que.FromDate;

            foreach (QueActiveData q in BusyQue)
            {
                if (iDuration.Minutes > q.ActiveDuration)
                    return false;

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

        private void AddCusomersToUsers(int userId,int customerId)
        {

            string sql = "Select count(1) from UsersToCusomers Where [UserId]=@UserId And [CustomerId]=@CustomerId;";           

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            bool isExist = db.IsExist(sql, new { UserId = userId, CustomerId= customerId }, 1);
            if (!isExist)
            {
                string sqlUserCustomerInsert = @"INSERT INTO UsersToCusomers ([UserId],[CustomerId]) Values

                        (@UserId,@CustomerId);";
                
                var Rows = db.Execute(sqlUserCustomerInsert, 1, new { UserId = userId, CustomerId = customerId });
            }
        }
        private bool AddQueToDb(Que que)

        {

            bool Issuccess = false;

            try

            {

                string sqlQueInsert = @"INSERT INTO Que ([UserId],[FromDate],[ToDate],[QueType],[EmployeeId],[CustomerId],[GroupId]) Values

                (@UserId,@FromDate,@ToDate,@QueType,@EmployeeId,@CustomerId,@GroupId);";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                var affectedRows = db.Execute(sqlQueInsert, 1, que);
                if (affectedRows > 0)
                {
                    Task.Factory.StartNew(() => AddCusomersToUsers(que.UserId,que.CustomerId));
                }
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
        public string ApproveQue(string decryptedQueId)
        {
            try
            {
                SecureData sec = new SecureData();
                string queId = sec.DecryptTextWithNoSpecialCharecters(decryptedQueId, "shalomHa!@");
                //IsApproved field
                string sqlUpdate = "UPDATE Que SET [IsApproved]=1 where [id]=@QueId;";
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
                var affectedRows = db.Execute(sqlUpdate, 1, new { QueId = queId });
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return "ארעה שגיאה";
            }
            return "";
        }
        public string DeleteQue(string decryptedQueId)
        {
            try
            {
                SecureData sec = new SecureData();
                string queId = sec.DecryptTextWithNoSpecialCharecters(decryptedQueId, "shalomHa!@");
                //IsApproved field
                string sqlUpdate = "Delete FROM Que where [id]=@QueId;";
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
                var affectedRows = db.Execute(sqlUpdate, 1, new { QueId = queId });
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return "ארעה שגיאה";
            }
            return "";
        }

        public string DeleteGroup(string guid, int groupId)
        {
            if (!IsCustomerExist(guid))
            {
                return "ארעה שגיאה";
            }
            try
            {

                string sqlUpdate = "Delete FROM GroupsCustomers where [GroupId]=@GroupId;";
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
                var affectedRows = db.Execute(sqlUpdate, 1, new { GroupId = groupId });

                string sqlUpdate2 = "Delete FROM Groups where [GroupId]=@GroupId;";
                var affectedRows2 = db.Execute(sqlUpdate2, 1, new { GroupId = groupId });
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return "ארעה שגיאה";
            }
            return "";
        }

        public string DeleteCustomerFromGroup(string guid, int groupId,int customerId)
        {
            if(!IsCustomerExist(guid))
            {
                return "ארעה שגיאה";
            }
            try
            {
                
                string sqlUpdate = "Delete FROM GroupsCustomers where [GroupId]=@GroupId And [CustomerId]=@CustomerId;";
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
                var affectedRows = db.Execute(sqlUpdate, 1, new { GroupId = groupId, CustomerId= customerId });
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return "ארעה שגיאה";
            }
            return "";
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
