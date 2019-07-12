using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Tor.Dal;


namespace Tor

{
    public class UserActivity
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int ActiveDuration { get; set; }
        public string guid { get; set; }
    }
    public class UserExtraActivity
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public string guid { get; set; }
    }
    public class City
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }

    public class user

    {

        [Required]

        public string Mail { get; set; }

        [Required]

        public string Password { get; set; }



    }



    public class CustomerObjBase

    {

        public int Id { get; set; }



        [Required]

        public string Name { get; set; }



        [Required]

        public string tel { get; set; }



        public string guid { get; set; }





    }

    public class CustomerObj : CustomerObjBase

    {


        [Required]

        public string Password { get; set; }

        [Required]

        public string Email { get; set; }



        public string Active { get; set; }



    }



    public class CustomerObjWrapper

    {

        public IEnumerable<CustomerObj> customersList { get; set; }



        public string message { get; set; }



    }



    public class UsersObjWrapper

    {

        public IEnumerable<UserSearch> usersList { get; set; }



        public string message { get; set; }



    }

    public class CustomerUser

    {

        [Required]

        public int UserId { get; set; }



        [Required]

        public string guid { get; set; }

    }

    public class UserDetailsData
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string BizName { get; set; }


        public string BizNameEng { get; set; }
        public string Adrress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
    public class UserDetailsWrapper
    {
        public string ErrorMsg { get; set; }

        public UserDetailsData userDetailsData { get; set; }
    }
    public class UserDetails

    {

        public string ErrorMsg { get; set; }

        public string UserName { get; set; }

        public int UserId { get; set; }

        public string guid { get; set; }

        public bool isUser { get; set; }



        public int CustomerId { get; set; }

        public int day { get; set; }

        public Dictionary<string, List<BizTypeObj>> dicBizType { get; set; }
        public IEnumerable<UserActivities> Activities { get; set; }

        public Dictionary<int, List<string>> dicEmployeeActivities { get; set; }
    }

    public class UserObj

    {

        public int Id { get; set; }

        [Required]

        public string User { get; set; }



        [Required]

        public string Password { get; set; }

        [Required]

        public string tel { get; set; }

        [Required]

        public string Email { get; set; }

        [Required]

        public string BizName { get; set; }

        [Required]

        public string BizNameEng { get; set; }

        [Required]

        public string Adrress { get; set; }

        [Required]

        public string City { get; set; }

        public string Country { get; set; }

        [Required]

        public string CreditCard { get; set; }

        public string guid { get; set; }

        [Required]
        public Dictionary<string, string> UserActivities { get; set; }

        [Required]
        public Dictionary<string, List<BizTypeObj>> dicBizType { get; set; }

        [Required]
        public Dictionary<int, List<string>> dicEmployeeActivities { get; set; }

    }



    public class UserSearch

    {

        public int Id { get; set; }

        public string User { get; set; }



        public string tel { get; set; }



        public string Adrress { get; set; }



        public string City { get; set; }

        public string version { get; set; }

        [Required]

        public string guid { get; set; }

        public string logo { get; set; }

        public string img { get; set; }

    }



    public class BizTypeObj
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public string EmployeeName { get; set; }

        public int ActiveDay { get; set; }

        [Required]

        public string ActiveHourFrom { get; set; }

        [Required]
        public string ActiveHourTo { get; set; }

        public string ActiveHourFromNone { get; set; }

        public string ActiveHourToNone { get; set; }

        public int UserId { get; set; }

        public string guid { get; set; }
    }



    public class UserObjWrapper

    {

        public string ErrorMsg { get; set; }

        public string guid { get; set; }

    }

    public class UserActivities
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int ActiveDuration { get; set; }
    }

    public class EmployeeActivities
    {
        public int EmployeeId { get; set; }

        public int ActivityId { get; set; }
    }

    public class MailObj
    {
        public string name { get; set; }
        public string Msg { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class UserManager

    {

        private UserDetails CheckIsValidUserRegister(UserObj user)

        {

            UserDetails userObjWrapper = new UserDetails();

            //if (user.ActiveDays == "")

            //{

            // userObjWrapper.ErrorMsg = "נא לבחור לפחות יום אחד בשבוע";

            //}

            //string[] arrActiveDays = user.ActiveDays.Split(',');

            //if (arrActiveDays.Length <= 0)

            //{

            // userObjWrapper.ErrorMsg = "נא לבחור לפחות יום אחד בשבוע";

            //}





            userObjWrapper.guid = Guid.NewGuid().ToString();

            user.guid = userObjWrapper.guid;



            return userObjWrapper;

        }

        public UserDetails Login(user u)

        {

            string sql = "SELECT * FROM Users WHERE [Email] = @Email And [Password]= @Password And [Active] is not null ;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            IEnumerable<UserObj> users = db.QueryData<UserObj>(sql, 1, new { Email = u.Mail, Password = u.Password });

            string sqlCust = "SELECT * FROM Customer WHERE [Email] = @Email And [Password]= @Password And [Active] is not null ;";

            IEnumerable<CustomerObj> customers = db.QueryData<CustomerObj>(sqlCust, 1, new { Email = u.Mail, Password = u.Password });

            UserDetails userDetails = new UserDetails();

            userDetails.day = (int)new DateTime().DayOfWeek;

            foreach (UserObj user in users)
            {

                userDetails.guid = user.guid;

                userDetails.UserName = user.User;

                userDetails.isUser = true;

                userDetails.dicBizType = getBizTypeDictionary(user.Id);
                userDetails.Activities = getUserActivities(user.Id);
                userDetails.dicEmployeeActivities = getEmployeeActivities(user.Id);
                userDetails.UserId = user.Id;

                return userDetails;

            }



            foreach (CustomerObj cust in customers)
            {

                userDetails.CustomerId = cust.Id;

                userDetails.guid = cust.guid;

                userDetails.UserName = cust.Name;

                userDetails.isUser = false;

                return userDetails;

            }



            return null;

        }

        private Dictionary<int, List<string>> getEmployeeActivities(int userId)
        {
            Dictionary<int, List<string>> dic = new Dictionary<int, List<string>>();
            string sqlCust = "SELECT [EmployeeId],[ActivityId] FROM [EmployeesActivities] WHERE [UserId] = @UserId;";
            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            IEnumerable<EmployeeActivities> userAct = db.QueryData<EmployeeActivities>(sqlCust, 1, new { UserId = userId });
            List<string> lst = new List<string>();
            int empId = 0;
            foreach (EmployeeActivities u in userAct)
            {
                lst.Add(u.ActivityId.ToString());
                if (empId != u.EmployeeId)
                {
                    lst = new List<string>();
                    dic.Add(u.EmployeeId, lst);
                    empId = u.EmployeeId;
                }
                else
                {
                    dic[empId] = lst;
                }

            }
            return dic;
        }
        private IEnumerable<UserActivities> getUserActivities(int userId)
        {
            string sqlCust = "SELECT [Id],[Name],[ActiveDuration] FROM UsersActivitiesTypes WHERE [UserId] = @UserId;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            IEnumerable<UserActivities> userAct = db.QueryData<UserActivities>(sqlCust, 1, new { UserId = userId });

            return userAct;
        }
        private Dictionary<string, List<BizTypeObj>> getBizTypeDictionary(int userId)

        {

            Dictionary<string, List<BizTypeObj>> dic = new Dictionary<string, List<BizTypeObj>>();

            string sqlCust = "SELECT * FROM UsersActivity WHERE [UserId] = @Id;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            IEnumerable<BizTypeObj> userAct = db.QueryData<BizTypeObj>(sqlCust, 1, new { Id = userId });

            string biz = "";

            List<BizTypeObj> lst = new List<BizTypeObj>();

            foreach (BizTypeObj user in userAct)
            {

                if (user.EmployeeName != biz)

                {

                    biz = user.EmployeeName;

                    lst = new List<BizTypeObj>();

                    lst.Add(user);

                    dic.Add(user.EmployeeName, lst);

                }

                else

                    lst.Add(user);

            }

            return dic;

        }

        public UserDetails RegisterCustmer(CustomerObj customer)

        {

            UserDetails userDetails = new UserDetails();

            if (IsCustomerMailExist(customer.Email))

            {

                userDetails.ErrorMsg = "מייל קיים במערכת";

                return userDetails;

            }
            if (IsMailExist(customer.Email))

            {

                userDetails.ErrorMsg = "מייל קיים במערכת";

                return userDetails;

            }
            if (IsCustomerTelExist(customer.tel))

            {

                userDetails.ErrorMsg = "מספר טלפון קיים במערכת";

                return userDetails;

            }


            string guid = Guid.NewGuid().ToString();

            customer.guid = guid;

            if (!AddCustomerToDb(customer, 0))

            {

                userDetails.ErrorMsg = "ארעה שגיאה";

                return userDetails;

            }


            if (!sendCustomerMail(customer))

            {

                userDetails.ErrorMsg = "ארעה שגיאה";

                return userDetails;

            }

            //userDetails.guid = guid;

            userDetails.UserName = customer.Name;

            userDetails.isUser = false;

            return userDetails;

        }

        private bool sendCustomerMail(CustomerObj user)
        {
            string baseUerl = ConfigManager.BaseUrl;
            string content = " : שלום - להמשך רישום למערכת תורים נא ללחוץ על הקישור הבא" + " <a href='" + baseUerl + "/user/ConfirmCustomeRegister/" + user.guid + "'>לחץ כאן לאישור</a>";
            string[] arr = new string[1];
            arr[0] = user.Email;
            return MailSender.sendMail("הרשמה למערכת תורים", content, arr, "tana@TanaSoftware.com");

        }

        private bool AddCustomerToDb(CustomerObj user, int userId)

        {


            try

            {

                string sqlCustomerInsert = @"INSERT INTO Customer ([Name],[Email],[Password],[tel],[guid]) Values

(@Name,@Email,@Password,@tel,@guid);";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                var affectedRows = db.Execute(sqlCustomerInsert, 1, user);



                if (userId > 0)

                {

                    string sql = "select [Id] From Customer where [Name]=@Name And [tel]=@tel";

                    IEnumerable<int> CustomerId = db.QueryData<int>(sql, 1, new { Name = user.Name, tel = user.tel });

                    int customerId = 0;



                    foreach (int u in CustomerId)

                    {

                        customerId = u;

                    }

                    if (customerId > 0)

                    {

                        string sqlUserCustomerInsert = @"INSERT INTO UsersToCusomers ([UserId],[CustomerId]) Values

                        (@UserId,@CustomerId);";

                        var Rows = db.Execute(sqlUserCustomerInsert, 1, new { UserId = userId, CustomerId = customerId });

                    }

                }

            }

            catch (Exception ex)

            {

                Logger.Write(ex);

                return false;

            }



            return true;

        }



        public IEnumerable<City> GetCities()

        {

            var sql = @"SELECT [Id],[Name] FROM City;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            IEnumerable<City> cities = db.QueryData<City>(sql, 1, null);

            return cities;

        }



        public string UserAddHisCustomer(CustomerObjBase customer)

        {

            if (customer.Name == "")

            {

                return "נא להזין שם";

            }

            if (customer.tel == "")

            {

                return "נא להזין טלפון";

            }

            if (!IsUserGuidExists(customer.guid))

            {

                return "אין באפשרותך להוסיף משתמש";

            }



            if (IsCustomerTelExist(customer.tel))

            {

                return "מספר טלפון קיים במערכת";

            }



            CustomerObj user = new CustomerObj();

            user.Email = "";

            user.guid = Guid.NewGuid().ToString();

            user.Name = customer.Name;

            user.Password = "";

            user.tel = customer.tel;

            user.Active = "";



            if (!AddCustomerToDb(user, customer.Id))

            {

                return "ארעה שגיאה";

            }



            return "";

        }

        private bool IsCustomerMailExist(string mail)

        {

            string sql = "SELECT count(1) FROM Customer WHERE [Email] = @Email;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            return db.IsExist(sql, new { Email = mail }, 1);

        }

        private bool IsCustomerTelExist(string tel)

        {

            string sql = "SELECT count(1) FROM Customer WHERE [tel] = @tel;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            return db.IsExist(sql, new { tel = tel }, 1);

        }

        public string SaveUser(UserObj userObj)
        {
            if (!IsUserGuidExists(userObj.guid))
            {
                return "ארעה שגיאה";
            }

            try
            {
                string sql = "";
                if (string.IsNullOrEmpty(userObj.Password))//without password
                    sql = @"Update Users Set [User]=@User,[Email]=@Email,[Tel]=@tel,[BizName]=@BizName,[BizNameEng]=@BizNameEng,
                        [Adrress]=@Adrress,[City]=@City where [Id]=@Id And [Active]=@guid;";
                else//with password
                    sql = @"Update Users Set [User]=@User,[Email]=@Email,[Tel]=@tel,[BizName]=@BizName,[Password]=@Password,[BizNameEng]=@BizNameEng,
                        [Adrress]=@Adrress,[City]=@City where [Id]=@Id And [Active]=@guid;";
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
                var affectedRows = db.Execute(sql, 1, userObj);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return "ארעה שגיאה";
            }
            return "נתוני משתמש נשמרו בהצלחה";
        }
        public UserDetails RegisterUser(UserObj user)

        {

            UserDetails userObjWrapper = CheckIsValidUserRegister(user);

            if (userObjWrapper.ErrorMsg != "")

            {

                //if (IsUserExist(user.User))

                //{

                //    userObjWrapper.ErrorMsg = "שם משתמש קיים במערכת";

                //    userObjWrapper.guid = "";

                //    return userObjWrapper;

                //}

                if (IsMailExist(user.Email))

                {

                    userObjWrapper.ErrorMsg = "מייל קיים במערכת";

                    userObjWrapper.guid = "";

                    return userObjWrapper;

                }
                if (IsCustomerMailExist(user.Email))

                {

                    userObjWrapper.ErrorMsg = "מייל קיים במערכת";

                    userObjWrapper.guid = "";

                    return userObjWrapper;

                }

                if (IsBizNameExist(user.BizNameEng))

                {

                    userObjWrapper.ErrorMsg = "שם משתמש באנגלית קיים במערכת";

                    userObjWrapper.guid = "";

                    return userObjWrapper;

                }
                if (user.UserActivities == null)
                {
                    userObjWrapper.ErrorMsg = "נא להזין פעילות";

                    userObjWrapper.guid = "";

                    return userObjWrapper;
                }
                foreach (var item in user.UserActivities)
                {

                    if (item.Value.Length <= 0)
                    {
                        userObjWrapper.ErrorMsg = "נא להזין משך פעילות";

                        userObjWrapper.guid = "";

                        return userObjWrapper;
                    }
                    if (item.Value.Length > 3)
                    {
                        userObjWrapper.ErrorMsg = "נא להזין משך פעילות עד 720 דקות";

                        userObjWrapper.guid = "";

                        return userObjWrapper;
                    }
                }
                if (!AddUserToDb(user))

                {

                    userObjWrapper.ErrorMsg = "ארעה שגיאה";

                    userObjWrapper.guid = "";

                    return userObjWrapper;

                }

                if (!sendMail(user))

                {

                    userObjWrapper.ErrorMsg = "ארעה שגיאה";

                    userObjWrapper.guid = "";

                    return userObjWrapper;

                }

            }

            //userObjWrapper.guid = user.guid;

            userObjWrapper.isUser = true;

            userObjWrapper.UserName = user.User;

            return userObjWrapper;

        }

        private bool sendMail(UserObj user)
        {
            string baseUerl = ConfigManager.BaseUrl;
            string content = " : שלום - להמשך רישום למערכת תורים נא ללחוץ על הקישור הבא" + " <a href='" + baseUerl + "/user/ConfirmRegister/" + user.guid + "'>לחץ כאן לאישור</a>";
            string[] arr = new string[1];
            arr[0] = user.Email;
            return MailSender.sendMail("הרשמה למערכת תורים", content, arr, "tana@TanaSoftware.com");
            //return MailSender.sendMail(user.Email, user.guid, "הרשמה למערכת תורים", " שלום - להמשך רישום למערכת תורים נא ללחוץ על הקישור הבא - ", "/user/ConfirmRegister/");               

        }

        public bool ContanctUs(MailObj mail)
        {
            string[] arr = new string[1];
            arr[0] = ConfigManager.MailTo;
            return MailSender.sendMail("מייל נשלח מטלפון :  - " + mail.phone + ", מייל :  " + mail.email
                + ", שם :" + mail.name, mail.Msg, arr, mail.email);
        }

        public string ActivateCustomer(string guid)

        {
            string sql = "SELECT count(1) FROM Customer WHERE [guid] = @guid;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            bool isExist = db.IsExist(sql, new { guid = guid }, 1); ;
            if (isExist)

            {

                try

                {

                    string sqlUsersUpdate = "UPDATE Customer SET [Active]=@Active where [guid]=@guid;";

                    var affectedRows = db.Execute(sqlUsersUpdate, 1, new { Active = guid, guid = guid });

                    return "Ok";

                }

                catch (Exception ex)

                {

                    Logger.Write("UserManager:ActivateCustomer:" + ex);



                }

            }

            return "";

        }

        public string ActivateUser(string guid)

        {

            if (IsUserGuidExists(guid, true))

            {

                try

                {
                    DateTime d = DateTime.Now;
                    string sqlUsersUpdate = "UPDATE Users SET [Active]=@Active,[StartDate]=@StartDate where [guid]=@guid;";

                    DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                    var affectedRows = db.Execute(sqlUsersUpdate, 1, new { Active = guid, guid = guid, StartDate=d });

                    return "Ok";

                }

                catch (Exception ex)

                {

                    Logger.Write("UserManager:ActivateUser:" + ex);



                }

            }

            return "";

        }

        private bool AddUserToDb(UserObj user)

        {

            bool Issuccess = false;

            try

            {

                string sqlUsersInsert = @"INSERT INTO Users ([User],[Email],[Password],[BizName],[tel],[BizNameEng],[Adrress],[City],[Country],[CreditCard],[guid]) Values

(@User,@Email,@Password,@BizName,@tel,@BizNameEng,@Adrress,@City,@Country,@CreditCard,@guid);";



                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                var affectedRows = db.Execute(sqlUsersInsert, 1, user);



                string sql = "select [Id] From Users where [User]=@User And [Email]=@Email";

                IEnumerable<int> UserId = db.QueryData<int>(sql, 1, new { User = user.User, Email = user.Email });

                int userId = 0;



                foreach (int u in UserId)

                {

                    userId = u;

                }

                if (userId > 0)

                {

                    string sqlAct = "INSERT INTO UsersActivitiesTypes ([UserId],[Name],[ActiveDuration]) Values(@UserId,@Name,@ActiveDuration)";
                    foreach (var item in user.UserActivities)
                    {

                        var affectedA = db.Execute(sqlAct, 1, new { UserId = userId, Name = item.Key, ActiveDuration = item.Value });
                    }



                    string sqlUserActivity = @"INSERT INTO UsersActivity ([EmployeeId],[UserId],[ActiveDay],[EmployeeName],[ActiveHourFrom],[ActiveHourTo],[ActiveHourFromNone],[ActiveHourToNone]) Values
                        (@EmployeeId,@UserId,@ActiveDay,@EmployeeName,@ActiveHourFrom,@ActiveHourTo,@ActiveHourFromNone,@ActiveHourToNone);";


                    foreach (var item in user.dicBizType)
                    {

                        for (int i = 0; i < item.Value.Count; i++)

                        {

                            item.Value[i].UserId = userId;
                            if (item.Value[i].ActiveHourFromNone == "")
                                item.Value[i].ActiveHourFromNone = null;

                            if (item.Value[i].ActiveHourToNone == "")
                                item.Value[i].ActiveHourToNone = null;

                            db.Execute(sqlUserActivity, 1, item.Value[i]);

                        }

                    }
                    string sqlActEmp = "select [Id],[Name],[ActiveDuration] From [UsersActivitiesTypes] where [UserId]=@UserId";

                    IEnumerable<UserActivities> ActEmployyees = db.QueryData<UserActivities>(sqlActEmp, 1, new { UserId = userId });


                    string sqlEmployeesActivities = "INSERT INTO EmployeesActivities ([UserId],[EmployeeId],[ActivityId]) Values(@UserId,@EmployeeId,@ActivityID)";
                    foreach (var item in user.dicEmployeeActivities)
                    {
                        foreach (UserActivities u in ActEmployyees)
                        {
                            for (int i = 0; i < item.Value.Count; i++)
                            {
                                if (item.Value[i] == u.Name)
                                {
                                    var affectedA = db.Execute(sqlEmployeesActivities, 1, new { UserId = userId, EmployeeId = item.Key, ActivityID = u.Id });
                                }
                            }

                        }

                    }

                }

                else

                {

                    Logger.Write("UserManager:AddUserToDb: cant find user Id in Users table");

                    return false;

                }



            }

            catch (Exception ex)

            {

                Logger.Write("UserManager:AddUserToDb:" + ex);

                Issuccess = false;

            }



            Issuccess = true;

            return Issuccess;

        }

        private void DeleteUser(string user, string email)

        {

            try

            {



                string sqlUsersDelete = "Delete from Users where User=@User And Email=@Email;";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                var affectedRows = db.Execute(sqlUsersDelete, 1, new { User = user, Email = email, });

            }

            catch (Exception ex)

            {

                Logger.Write("UserManager:DeleteUser:" + ex);

            }

        }

        private bool IsCustomerGuidExists(string guid)

        {

            try

            {

                string sql = "SELECT count(1) FROM Customer WHERE [Active] = @guid;";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);



                return db.IsExist(sql, new { guid = guid }, 1);

            }

            catch (Exception ex)

            {

                Logger.Write("UserManager:IsCustomerGuidExists:" + ex);

                return false;

            }

        }

        public bool IsUserGuidExists(string guid, bool isUseGuid = false)

        {

            try

            {
                string w = isUseGuid ? "guid" : "Active";
                string sql = "SELECT count(1) FROM Users WHERE [" + w + "] = @guid;";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                return db.IsExist(sql, new { guid = guid }, 1);

            }

            catch (Exception ex)

            {

                Logger.Write("UserManager:IsUserGuidExists:" + ex);

                return false;

            }

        }

        private bool IsUserExist(string user)

        {

            try

            {

                string sql = "SELECT count(1) FROM Users WHERE [User] = @User;";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                return db.IsExist(sql, new { User = user }, 1);

            }

            catch (Exception ex)

            {

                Logger.Write("UserManager:IsUserExist:" + ex);

                return true;

            }

        }

        private bool IsMailExist(string mail)

        {

            try

            {

                string sql = "SELECT count(1) FROM Users WHERE [Email] = @Email;";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                return db.IsExist(sql, new { Email = mail }, 1);

            }

            catch (Exception ex)

            {

                Logger.Write("UserManager:IsMailExist:" + ex);

                return false;

            }

        }

        private bool IsBizNameExist(string bizName)

        {

            try

            {

                string sql = "SELECT count(1) FROM Users WHERE BizNameEng = @BizNameEng;";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                return db.IsExist(sql, new { BizNameEng = bizName }, 1);

            }

            catch (Exception ex)

            {

                Logger.Write("UserManager:IsBizNameExist:" + ex);

                return false;

            }

        }

        private void UpdateUser(UserObj user)

        {

            try

            {

                string sqlUsersUpdate = "UPDATE Users SET User=@User,=Email=@Email,BizName=@BizNamem,Adrress=@Adrress,City=@City,Country=@Country,CreditCard=@CreditCard where User=@User And Email=@Email;";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                var affectedRows = db.Execute(sqlUsersUpdate, 1, user);

            }

            catch (Exception ex)

            {

                Logger.Write("UserManager:IsBizNameExist:" + ex);

            }

        }



        public CustomerObjWrapper GetCustomersPerUser(CustomerUser user)

        {

            CustomerObjWrapper customerObjWrapper = new CustomerObjWrapper();

            customerObjWrapper.message = "";



            if (!IsUserGuidExists(user.guid))

            {

                customerObjWrapper.message = "ארעה שגיאה";

                return customerObjWrapper;

            }



            try

            {

                var sql = "SELECT A.Id,A.Name,A.tel,A.Email FROM Customer A INNER JOIN UsersToCusomers B on A.Id = B.CustomerId WHERE B.UserId = @UserId ";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                IEnumerable<CustomerObj> customers = db.QueryData<CustomerObj>(sql, 1, new { UserId = user.UserId });

                customerObjWrapper.customersList = customers;

            }

            catch (Exception ex)

            {

                Logger.Write(ex);

                customerObjWrapper.message = "ארעה שגיאה";

                customerObjWrapper.customersList = null;

            }

            return customerObjWrapper;



        }

        public string UpdateEmployee(List<BizTypeObj> user)
        {
            try
            {
                if (!IsUserGuidExists(user[0].guid) || user.Count <= 0)
                {

                    return "ארעה שגיאה בעדכון עובד";
                }
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                string sqlDel = "Delete from UsersActivity where [UserId]=@UserId And [EmployeeId]=@EmployeeId;";
                var Rows = db.Execute(sqlDel, 1, user[0]);

                string sqlUserActivity = @"INSERT INTO UsersActivity ([EmployeeId],[UserId],[ActiveDay],[EmployeeName],[ActiveHourFrom],[ActiveHourTo],[ActiveHourFromNone],[ActiveHourToNone]) Values
                        (@EmployeeId,@UserId,@ActiveDay,@EmployeeName,@ActiveHourFrom,@ActiveHourTo,@ActiveHourFromNone,@ActiveHourToNone);";

                foreach (BizTypeObj biz in user)
                {
                    if (biz.ActiveHourFromNone == "")
                        biz.ActiveHourFromNone = null;

                    if (biz.ActiveHourToNone == "")
                        biz.ActiveHourToNone = null;

                    var affectedRows = db.Execute(sqlUserActivity, 1, biz);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return "ארעה שגיאה בעדכון עובד";

            }
            return "";
        }

        public string DeleteEmployee(BizTypeObj user)
        {
            try
            {
                if (!IsUserGuidExists(user.guid))
                {

                    return "ארעה שגיאה במחיקת עובד";
                }
                string sql = "Delete From UsersActivity where [UserId]=@UserId And [EmployeeId]=@EmployeeId";
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                var affectedRows = db.Execute(sql, 1, new { EmployeeName = user.EmployeeName, UserId = user.UserId, EmployeeId = user.EmployeeId });
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return "ארעה שגיאה במחיקת עובד";

            }
            return "";
        }

        public string DeleteUserActivity(UserActivity userActivity)
        {
            if (!IsUserGuidExists(userActivity.guid))
            {
                return "משתמש לא קיים";
            }
            string sqlAct = "DELETE FROM UsersActivitiesTypes Where Id=@UserId;";
            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
            var affectedRows = db.Execute(sqlAct, 1, userActivity);
            return "";
        }
        public int AddUserActivity(UserActivity userActivity)
        {
            if (!IsUserGuidExists(userActivity.guid))
            {
                return 0;
            }


            if (userActivity.ActiveDuration <= 0)
            {
                return 0;
            }
            if (userActivity.ActiveDuration > 720)
            {
                return 0;
            }

            string sqlAct = "INSERT INTO UsersActivitiesTypes ([UserId],[Name],[ActiveDuration]) Values(@UserId,@Name,@ActiveDuration)";
            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
            var affectedRows = db.Execute(sqlAct, 1, userActivity);
            string sql = "select max(Id) as id From UsersActivitiesTypes where UserId=@UserId";
            IEnumerable<int> activityId = db.QueryData<int>(sql, 1, new { UserId = userActivity.UserId });
            int id = 0;
            foreach (int u in activityId)
            {
                id = u;
            }

            return id;
        }

        public object AddEmployee(List<BizTypeObj> user)
        {
            try
            {
                if (user != null && user.Count > 0)
                {

                    if (!IsUserGuidExists(user[0].guid))
                    {

                        return new { msg = "ארעה שגיאה בהוספת עובד", EmployeeId = 0 };
                    }

                    DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
                    string sqlCheck = "Select max([EmployeeId]) as EmployeeId from UsersActivity where [UserId]=@UserId";
                    IEnumerable<int> maxEmployeeId = db.QueryData<int>(sqlCheck, 1, new { UserId = user[0].UserId });
                    int EmployeeId = 0;
                    foreach (int u in maxEmployeeId)
                    {
                        EmployeeId = u + 1;
                    }
                    for (int i = 0; i < user.Count; i++)
                    {
                        user[i].EmployeeId = EmployeeId;
                        string sql = @"Insert into UsersActivity ([EmployeeId],[UserId],[EmployeeName],[ActiveDay],[ActiveHourFrom],[ActiveHourTo],[ActiveHourFromNone],[ActiveHourToNone])
                            values(@EmployeeId,@UserId,@EmployeeName,@ActiveDay,@ActiveHourFrom,@ActiveHourTo,@ActiveHourFromNone,@ActiveHourToNone)";
                        var affectedRows = db.Execute(sql, 1, user[i]);

                    }
                    return new { msg = "", EmployeeId = EmployeeId };
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return new { msg = "ארעה שגיאה בהוספת עובד", EmployeeId = 0 };

            }
            return new { msg = "ארעה שגיאה בהוספת עובד", EmployeeId = 0 };
        }
        public UserDetails GetUserById(UserSearch user)

        {

            UserDetails userDetails = new UserDetails();

            if (!IsCustomerGuidExists(user.guid))

            {

                userDetails.ErrorMsg = "משתמש לא קיים";

                return userDetails;

            }

            userDetails.day = (int)new DateTime().DayOfWeek;

            userDetails.dicBizType = getBizTypeDictionary(user.Id);
            userDetails.Activities = getUserActivities(user.Id);
            return userDetails;

        }

        public UserDetailsWrapper GetUserDetails(UserSearch user)

        {
            UserDetailsWrapper wrapper = new UserDetailsWrapper();
            //UserDetails userDetails = new UserDetails();

            string sql = "SELECT * FROM Users WHERE [Active] = @guid;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            bool isUserExists = false;

            IEnumerable<UserDetailsData> UserX = db.QueryData<UserDetailsData>(sql, 1, new { guid = user.guid });
            int userId = 0;

            foreach (UserDetailsData u in UserX)
            {
                UserDetailsData ud = new UserDetailsData();
                ud.Adrress = u.Adrress;
                ud.BizName = u.BizName;
                ud.BizNameEng = u.BizNameEng;
                ud.City = u.City;
                ud.Email = u.Email;
                ud.Tel = u.Tel;
                ud.User = u.User;
                userId = u.Id;
                wrapper.userDetailsData = ud;
                isUserExists = true;
            }

            if (!isUserExists)
            {
                wrapper.ErrorMsg = "משתמש לא קיים";
                return wrapper;
            }
            //userDetails.dicBizType = getBizTypeDictionary(userId);
            //userDetails.Activities = getUserActivities(userId);
            //wrapper.userDetails = userDetails;

            return wrapper;

        }

        public UsersObjWrapper GetUsers(UserSearch user)

        {

            UsersObjWrapper customerObjWrapper = new UsersObjWrapper();



            if (user.City == null || user.City == "")

            {

                customerObjWrapper.message = "נא להזין עיר";

                return customerObjWrapper;

            }

            if (user.User == null || user.User == "")

            {

                customerObjWrapper.message = "נא להזין שם";

                return customerObjWrapper;

            }



            customerObjWrapper.message = "";



            if (!IsCustomerGuidExists(user.guid))

            {

                customerObjWrapper.message = "ארעה שגיאה";

                return customerObjWrapper;

            }



            try

            {

                var sql = @"SELECT [Id],[User],[Tel],[Adrress],[City] FROM Users WHERE [Active] is not null And

                        [User] LIKE @user And City = @City; ";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                IEnumerable<UserSearch> userSearch = db.QueryData<UserSearch>(sql, 1, new { City = user.City, user = "%" + user.User + "%" });

                customerObjWrapper.usersList = userSearch;

            }

            catch (Exception ex)

            {

                Logger.Write(ex);

                customerObjWrapper.message = "ארעה שגיאה";

                customerObjWrapper.usersList = null;

            }

            return customerObjWrapper;



        }
        public UserSearch GetUserByBizName(string bizName)
        {
            UserSearch userSearch = new UserSearch();

            string sql = "select * From Users where [BizNameEng]=@bizName";
            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
            IEnumerable<UserObj> UserX = db.QueryData<UserObj>(sql, 1, new { bizName = bizName });
            if (UserX == null)
                return null;




            //string cityName = "";
            foreach (UserObj u in UserX)
            {
                //string sqlCity = "select name From City where [Id]=@Id";
                //IEnumerable<string> city = db.QueryData<string>(sqlCity, 1, new { Id = u.City });
                //foreach (string c in city)
                //{
                //    cityName = c;
                //}
                userSearch.City = u.City;
                userSearch.Adrress = u.Adrress;
                userSearch.Id = u.Id;
                userSearch.tel = u.tel;
                userSearch.User = u.User;
                userSearch.version = ConfigManager.version;
                DirectoryInfo d = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\\img\");
                FileInfo[] Files = d.GetFiles("*.*");

                foreach (FileInfo file in Files)
                {
                    if (file.Name.Contains("LOGO"))
                        userSearch.logo = file.FullName;
                    else
                        userSearch.img = file.FullName;

                }
                return userSearch;
            }
            return null;
        }

        public string AddUserExtraActivity(UserExtraActivity user)
        {
            try
            {
                if (!IsUserGuidExists(user.guid))
                {
                    return "ארעה שגיאה";
                }
                string sql = @"INSERT INTO UserExtraActivity ([UserId],[EmployeeId],[From],[To]) Values

                        (@UserId,@EmployeeId,@From,@To);";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                var affectedRows = db.Execute(sql, 1, user);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return "ארעה שגיאה";
            }
            return "";
        }
    }

}