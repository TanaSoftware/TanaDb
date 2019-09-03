using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.RegularExpressions;
using Tor.Dal;


namespace Tor

{
    public class UserActivity
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int ActiveDuration { get; set; }

        public string color { get; set; }
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

    public class CustomerSearch

    {      
        public string searchText { get; set; }

      
        public string guid { get; set; }

    }
    public class Groups

    {
        public Dictionary<int,string> friends { get; set; }

        public int groupId { get; set; }
        public string groupName { get; set; }

        public string guid { get; set; }

    }
    public class AddCustomerGroup

    {
      
        public int GroupId { get; set; }
        public int CustomerId { get; set; }

        public string guid { get; set; }

    }

    public class Group

    {
        public int groupId { get; set; }
        public string groupName { get; set; }       

    }

    public class CustomerGroup

    {
        public string customerName { get; set; }
        public int customerId { get; set; }

    }

    public class AddedCustomerByUser

    {

        public int Id { get; set; }



        [Required]

        public string Name { get; set; }



        [Required]

        public string tel { get; set; }

        public string mail { get; set; }

        [Required]
        public string guid { get; set; }

        public int City { get; set; }



    }

    public class CustomerObj : CustomerObjBase

    {


        [Required]

        public string Password { get; set; }

        [Required]

        public string Email { get; set; }



        public string Active { get; set; }

        public int City { get; set; }

    }

    public class SearchUser
    {
        public string Id { get; set; }
        public string guid { get; set; }
        public string City { get; set; }
        public string BizName { get; set; }
        public string Adrress { get; set; }


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

        public bool IsGroup { get; set; }
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

        public int City { get; set; }

        public int CustomerId { get; set; }

        public int day { get; set; }
        public bool isGroup { get; set; }
        public Dictionary<string, List<BizTypeObj>> dicBizType { get; set; }
        public IEnumerable<UserActivities> Activities { get; set; }

        public Dictionary<int, List<string>> dicEmployeeActivities { get; set; }

        public IEnumerable<UserActivity> CustomersUsers { get; set; }
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

        public bool IsGroup { get; set; }
        public string guid { get; set; }

        [Required]
        public Dictionary<string, string> UserActivities { get; set; }

        public Dictionary<string, string> UserActivitiesColor { get; set; }

        [Required]
        public Dictionary<string, List<BizTypeObj>> dicBizType { get; set; }

        [Required]
        public Dictionary<int, List<string>> dicEmployeeActivities { get; set; }

    }


    public class ResetPasswordObj
    {
        public string guid { get; set; }
        public string pass1 { get; set; }
        public string pass2 { get; set; }
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

    public class EmployeeDataWrapper
    {
        public List<BizTypeObj> ActivitiesData { get; set; }
        public List<CheckedActivities> checkedActivities { get; set; }
    }
    public class CheckedActivities
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public int ActivityId { get; set; }
        public bool IsChecked { get; set; }
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

        public string color { get; set; }
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

            var regexUser = new Regex(@"^[א-תA-Za-z\ \']+$");
            var regexEngText = new Regex(@"^[א-תA-Za-z]+$");
            var regexTextAndNums = new Regex(@"^[א-תA-Za-z0-9\ ]+$");
            bool isMatch = regexUser.IsMatch(user.User);
            bool isMatchBizName = regexUser.IsMatch(user.BizName);
            bool isMatchEng = regexEngText.IsMatch(user.BizNameEng);

            //bool isMatchTextNum = regexTextAndNums.IsMatch(user.e);

            if (!isMatch || !isMatchBizName || !isMatchEng)
            {
                userObjWrapper.ErrorMsg = "ארעה שגיאה";
                return userObjWrapper;
            }
           



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
                userDetails.City = string.IsNullOrEmpty(user.City) ? 0 : Convert.ToInt32(user.City);
                return userDetails;

            }



            foreach (CustomerObj cust in customers)
            {

                userDetails.CustomerId = cust.Id;

                userDetails.guid = cust.guid;

                userDetails.UserName = cust.Name;

                userDetails.isUser = false;
                userDetails.City = cust.City;
                userDetails.CustomersUsers = getUsersForCustomers(cust.Id, userDetails.City);
                return userDetails;

            }



            return null;

        }

        private IEnumerable<UserActivity> getUsersForCustomers(int CustomerId,int cityId)
        {
            try
            {
                List<UserActivity> lst = new List<UserActivity>();
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
                string sqlCust = @"SELECT distinct q.UserId,u.BizName as Name
                              FROM [Que] q INNER JOIN Users u on q.UserId = u.Id
                              where q.customerId=@CustomerId;";
                IEnumerable<UserActivity> userAct = db.QueryData<UserActivity>(sqlCust, 1, new { CustomerId = CustomerId });

                string sqlCust2 = @"SELECT [Id] as UserId,BizName as Name
                              FROM Users
                              where [City]=@cityId;";
                IEnumerable<UserActivity> userAct2 = db.QueryData<UserActivity>(sqlCust2, 1, new { cityId = cityId });

                foreach (UserActivity u in userAct)
                    lst.Add(u);

                foreach (UserActivity u in userAct2)
                    lst.Add(u);
                
                return lst;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
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
                lst.Add(u.ActivityId.ToString());
            }
            return dic;
        }
        private IEnumerable<UserActivities> getUserActivities(int userId)
        {
            string sqlCust = "SELECT [Id],[Name],[ActiveDuration],[color] FROM UsersActivitiesTypes WHERE [UserId] = @UserId;";

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

                string sqlCustomerInsert = @"INSERT INTO Customer ([Name],[Email],[Password],[tel],[guid],[City]) Values

(@Name,@Email,@Password,@tel,@guid,@City);";

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

        public IEnumerable<CustomerObj> GetCustomerDetails(UserSearch search)
        {
            string sql = "select [Name],[Password],[tel],[Email],[City] from [Customer] where guid=@guid";
            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            IEnumerable<CustomerObj> cust = db.QueryData<CustomerObj>(sql, 1, search);

            return cust;
        }

        public IEnumerable<City> GetCities()

        {

            var sql = @"SELECT [Id],[Name] FROM City;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            IEnumerable<City> cities = db.QueryData<City>(sql, 1, null);

            return cities;

        }



        public string UserAddHisCustomer(AddedCustomerByUser customer)

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

            if (customer.mail != null && customer.mail != "")
            {
                if (IsMailExist(customer.mail) || IsCustomerMailExist(customer.mail))
                {
                    return "מייל קיים במערכת";
                }
            }

            CustomerObj user = new CustomerObj();

            user.Email = customer.mail != null ? customer.mail : "";

            user.guid = Guid.NewGuid().ToString();

            user.Name = customer.Name;

            user.Password = "";

            user.tel = customer.tel;

            user.Active = "";
            user.City = customer.City;

            if (!AddCustomerToDb(user, customer.Id))

            {

                return "ארעה שגיאה";

            }

            if (customer.mail != null && customer.mail != "")
            {
                string sql = "select [BizName],[Tel],[Email],[BizNameEng] from [Users] where Id=@Id and guid=@guid";
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
                try
                {
                    IEnumerable<UserDetailsData> userDetalis = db.QueryData<UserDetailsData>(sql, 1, customer);
                    if (userDetalis != null)
                    {
                        foreach (UserDetailsData u in userDetalis)
                        {
                            string[] arr = new string[1];
                            arr[0] = u.Email;
                            string baseUerl = ConfigManager.BaseUrl;
                            string content = " שלום " + "<br><br>" + u.BizName + " : הוסיף אותך למערכת זימון תורים - להמשך רישום למערכת תורים נא ללחוץ על הקישור הבא" + " <a href='" + baseUerl + "/user/ConfirmAddedCustomer/" + user.guid + "/" + u.BizNameEng + "'>לחץ כאן לאישור</a>";
                            MailSender.sendMail("הרשמה למערכת תורים", content, arr, "tana@TanaSoftware.com");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }

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

        public string SaveCustomer(CustomerObj cust)
        {
            string sql = "Update [Customer] set [Name]=@Name, [Password]=@Password, [tel]=@tel, [Email]=@Email,[City]=@City,[Active]=@guid where [guid]=@guid";
            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
            var affectedRows = db.Execute(sql, 1, cust);
            if (affectedRows == 0)
                return "ארעה שגיאה";

            return "";
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


            if (String.IsNullOrEmpty(userObjWrapper.guid))
            {
                return userObjWrapper;
            }
            else
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

                    var affectedRows = db.Execute(sqlUsersUpdate, 1, new { Active = guid, guid = guid, StartDate = d });

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

                string sqlUsersInsert = @"INSERT INTO Users ([User],[Email],[Password],[BizName],[tel],
[BizNameEng],[Adrress],[City],[Country],[CreditCard],[guid],[IsGroup]) Values

(@User,@Email,@Password,@BizName,@tel,@BizNameEng,@Adrress,@City,@Country,@CreditCard,@guid,@IsGroup);";



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

                    string sqlAct = "INSERT INTO UsersActivitiesTypes ([UserId],[Name],[ActiveDuration],[color]) Values(@UserId,@Name,@ActiveDuration,@color)";
                    foreach (var item in user.UserActivities)
                    {

                        var affectedA = db.Execute(sqlAct, 1, new { UserId = userId, Name = item.Key, ActiveDuration = item.Value,color = user.UserActivitiesColor[item.Key] });
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

        public bool IsCustomerGuidExists(string guid)

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

        public string UpdateEmployee(EmployeeDataWrapper user)
        {
            try
            {
                if (!IsUserGuidExists(user.ActivitiesData[0].guid) || user.ActivitiesData.Count <= 0)
                {

                    return "ארעה שגיאה בעדכון עובד";
                }
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                string sqlDel = "Delete from UsersActivity where [UserId]=@UserId And [EmployeeId]=@EmployeeId;";
                var Rows = db.Execute(sqlDel, 1, user.ActivitiesData[0]);

                string sqlUserActivity = @"INSERT INTO UsersActivity ([EmployeeId],[UserId],[ActiveDay],[EmployeeName],[ActiveHourFrom],[ActiveHourTo],[ActiveHourFromNone],[ActiveHourToNone]) Values
                        (@EmployeeId,@UserId,@ActiveDay,@EmployeeName,@ActiveHourFrom,@ActiveHourTo,@ActiveHourFromNone,@ActiveHourToNone);";

                foreach (BizTypeObj biz in user.ActivitiesData)
                {
                    if (biz.ActiveHourFromNone == "")
                        biz.ActiveHourFromNone = null;

                    if (biz.ActiveHourToNone == "")
                        biz.ActiveHourToNone = null;

                    var affectedRows = db.Execute(sqlUserActivity, 1, biz);
                }

                string sqlDelEmployeesActivities = "Delete from [EmployeesActivities] where [UserId]=@UserId And [EmployeeId]=@EmployeeId;";
                var Rows2 = db.Execute(sqlDelEmployeesActivities, 1, user.ActivitiesData[0]);

                string sqlEmployeesActivities = @"INSERT INTO EmployeesActivities ([UserId],[EmployeeId],[ActivityId]) Values
                        (@UserId,@EmployeeId,@ActivityId);";

                foreach (CheckedActivities CheckedAct in user.checkedActivities)
                {
                    if (CheckedAct.IsChecked)
                    {
                        var affectedRows = db.Execute(sqlEmployeesActivities, 1, CheckedAct);
                    }
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

            string sqlAct = "INSERT INTO UsersActivitiesTypes ([UserId],[Name],[ActiveDuration],[color]) Values(@UserId,@Name,@ActiveDuration,@color)";
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
            userDetails.dicEmployeeActivities = getEmployeeActivities(user.Id);

            string sql = "SELECT * FROM Users WHERE [Id] = @id;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
            
            IEnumerable<UserDetailsData> UserX = db.QueryData<UserDetailsData>(sql, 1, new { id = user.Id });           

            foreach (UserDetailsData u in UserX)
            {
                userDetails.isGroup = u.IsGroup;
            }
                
                
            return userDetails;

        }

        public List<Groups> GetGroupsPerCustomer(string guid, int CustomerId)
        {
            int isok = 0;
            if (!IsCustomerGuidExists(guid))
            {
                isok++;
            }
            if (!IsUserExist(guid))
            {
                isok++;
            }
            if (isok == 0)
                return null;
            List<Groups> lstGroups = new List<Tor.Groups>();
            string sql = @"SELECT G.Id as groupId,G.Name as groupName
                          FROM [Groups] G Inner join GroupsCustomers GC On G.Id=Gc.GroupId
                          where Gc.CustomerId=@CustomerId";

            
            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            IEnumerable<Group> Groups = db.QueryData<Group>(sql, 1, new { CustomerId = CustomerId });
            foreach (Group u in Groups)
            {
                Groups gr = new Groups();
                gr.groupName = u.groupName;
                gr.groupId = u.groupId;
                gr.friends = GetCustomersByGroup(u.groupId);
                lstGroups.Add(gr);
            }
            
            return lstGroups;
        }

        private Dictionary<int,string> GetCustomersByGroup(int groupId)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
              string sql = @"SELECT c.[Id] AS customerId,c.Name AS customerName FROM 
                            [GroupsCustomers] GC Inner join Customer c On c.[Id]=Gc.[CustomerId]
                            where GC.[GroupId]=@groupId";
            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            IEnumerable<CustomerGroup> Groups = db.QueryData<CustomerGroup>(sql, 1, new { groupId = groupId });
            foreach (CustomerGroup u in Groups)
            {
                if (!dic.ContainsKey(u.customerId))
                    dic.Add(u.customerId, u.customerName);

            }
            return dic;
        }

        public string AddCustomerToGroup(string guid,int groupId,int customerId)
        {
            if (!IsCustomerGuidExists(guid))
            {
                return "ארעה שגיאה";
            }
            try
            {
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                string sqlInsert = "INSERT INTO GroupsCustomers ([GroupId],[CustomerId]) Values (@GroupId,@CustomerId);";

                var affectedRows2 = db.Execute(sqlInsert, 1, new { GroupId = groupId, CustomerId = customerId });
            }
            catch(Exception ex)
            {
                Logger.Write(ex);
                return "ארעה שגיאה";
            }
            return "";
        }
        public string AddGropWithFriends(Groups group)
        {
            if(group.friends.Count<=1)
            {
                return "נא להזין לפחות 2 חברים";
            }
            if (!IsCustomerGuidExists(group.guid))
            {
                return "ארעה שגיאה";
            }

            try
            {
                string sqlCustomerInsert = @"INSERT INTO Groups ([Name]) Values (@Name);";

                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                var affectedRows = db.Execute(sqlCustomerInsert, 1, new { Name = group.groupName });

                if (affectedRows > 0)

                {

                    string sql = "select [Id] From Groups where [Name]=@Name";

                    IEnumerable<int> GroupId = db.QueryData<int>(sql, 1, new { Name = group.groupName });

                    int grpId = 0;

                    foreach (int u in GroupId)
                    {
                        grpId = u;

                    }

                    if (grpId > 0)
                    {
                        string sqlInsert = "INSERT INTO GroupsCustomers ([GroupId],[CustomerId]) Values (@GroupId,@CustomerId);";
                        foreach (var cust in group.friends)
                        {
                            var affectedRows2 = db.Execute(sqlInsert, 1, new { GroupId = grpId, CustomerId = cust.Key });
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Write(ex);
                return "ארעה שגיאה";
            }
            return "";
        }

        public CustomerObjBase SearchCustomer(CustomerSearch cust)
        {
            if(!IsCustomerGuidExists(cust.guid))
            {
                return null;
            }

            CustomerObjBase customer = new CustomerObjBase();

            string sql = "SELECT Id,Name FROM Customer WHERE [Active] is not null and ([tel]=@searchText or [Email]=@searchText);";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);           

            IEnumerable<CustomerObjBase> custX = db.QueryData<CustomerObjBase>(sql, 1, new { searchText = cust.searchText});
            foreach (CustomerObjBase u in custX)
            {
                customer.Id = u.Id;
                customer.Name = u.Name;
                break;
            }
            return customer;
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

                var sql = @"SELECT [Id],[BizName] as [User],[Tel],[Adrress],[City] FROM Users WHERE [Active] is not null And

                        [BizName] LIKE @user And City = @City; ";

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
                userSearch.User = u.BizName;
                userSearch.version = ConfigManager.version;
                string PhisicalbasePath = "User_" + u.Id + "\\";
                DirectoryInfo d = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\\img\" + PhisicalbasePath);
                if (d.Exists)
                {
                    FileInfo[] Files = d.GetFiles("*.*");
                    string basePath = "User_" + u.Id + "/";

                    foreach (FileInfo file in Files)
                    {
                        if (file.Name.Contains("Logo_" + u.Id))
                            userSearch.logo = basePath + file.Name;
                        else if (file.Name.Contains("back_" + u.Id))
                            userSearch.img = basePath + file.Name;

                    }
                }
                return userSearch;
            }
            return null;
        }
        public CustomerObj GetUserByGuid(string guid)
        {
            CustomerObj userSearch = new CustomerObj();

            string sql = "select * From [Customer] where [guid]=@guid";
            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
            IEnumerable<CustomerObj> UserX = db.QueryData<CustomerObj>(sql, 1, new { guid = guid });
            if (UserX == null)
                return null;


            //string cityName = "";
            foreach (CustomerObj u in UserX)
            {
                userSearch.Email = u.Email;
                userSearch.Name = u.Name;
                userSearch.Id = u.Id;
                userSearch.tel = u.tel;

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

        public string ResetPasswordPhase1(string mail)
        {
            string guid = "";
            string sql = "SELECT * FROM Users WHERE [Email] = @Email And [Active] is not null ;";
            mail = mail.Replace("!!", ".");
            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

            IEnumerable<UserObj> users = db.QueryData<UserObj>(sql, 1, new { Email = mail });



            foreach (UserObj user in users)
            {

                guid = user.guid;

                break;

            }

            if (guid == "")
            {
                string sqlCust = "SELECT * FROM Customer WHERE [Email] = @Email And [Active] is not null ;";

                IEnumerable<CustomerObj> customers = db.QueryData<CustomerObj>(sqlCust, 1, new { Email = mail });


                foreach (CustomerObj cust in customers)
                {

                    guid = cust.guid;

                    break;

                }
            }

            if (guid == "")
            {
                return "ארעה שגיאה";
            }

            string baseUerl = ConfigManager.BaseUrl;

            string content = " : שלום - לאיפוס סיסמה נא ללחוץ על הקישור הבא" + " <a href='" + baseUerl + "/user/ResetPassword/" + guid + "'>לחץ כאן </a>";
            string[] arr = new string[1];
            arr[0] = mail;
            bool isOk = MailSender.sendMail("מערכת תורים", content, arr, "tana@TanaSoftware.com");
            if (!isOk)
                return "ארעה שגיאה";

            return "";
        }
        public string ResetPasswordPhase2(string guid, string pass, string pass2)
        {
            bool isFaild = true;
            string NewGuid = Guid.NewGuid().ToString();
            if (IsCustomerGuidExists(guid))
            {
                string sql = "Update [Customer] set [Password]=@Password, [Active]=@NewGuid,[guid]=@NewGuid where [guid]=@guid";
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
                var affectedRows = db.Execute(sql, 1, new { Password = pass, guid = guid, NewGuid = NewGuid });
                if (affectedRows <= 0)
                    return "ארעה שגיאה";
                isFaild = false;
            }

            if (IsUserGuidExists(guid))
            {
                string sql = "Update [Users] set [Password]=@Password, [Active]=@NewGuid,[guid]=@NewGuid where [guid]=@guid";
                DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
                var affectedRows = db.Execute(sql, 1, new { Password = pass, guid = guid, NewGuid = NewGuid });
                if (affectedRows <= 0)
                    return "ארעה שגיאה";

                isFaild = false;
            }

            if (isFaild)
                return "ארעה שגיאה";

            return "";
        }


        public IEnumerable<SearchUser> SearchBiz(SearchUser search)
        {
            string sWhere = "";
            bool isAddress = string.IsNullOrEmpty(search.Adrress);
            bool isName = string.IsNullOrEmpty(search.BizName);
            bool isCity = string.IsNullOrEmpty(search.City);

            if (!isAddress && !isName && !isCity)
                sWhere = "u.[BizName] like @name  And u.[Adrress] like @Adrress And u.[City]=@city";

            else if (!isAddress && !isName && isCity)
                sWhere = "u.[BizName] like @name  And u.[Adrress] like @Adrress";

            else if(!isAddress && isName && !isCity)
                sWhere = "u.[Adrress] like @Adrress And u.[City]=@city";

            else if (isAddress && !isName && !isCity)
                sWhere = "u.[BizName] like @name And u.[City]=@city";


            else if (!isAddress && isName && isCity)
                sWhere = "u.[Adrress] like @Adrress";

            else if (isAddress && !isName && isCity)
                sWhere = "u.[BizName] like @name";

            else if (isAddress && isName && !isCity)
                sWhere = "u.[City]=@city";

            string sqlCust = @"SELECT u.[Id], u.BizName,u.Adrress,c.Name as City
                               FROM [Users] u INNER JOIN City c on u.City = c.Id
                               WHERE  " + sWhere + " And u.[Active] is not null ;";

            DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);
            IEnumerable<SearchUser> users = db.QueryData<SearchUser>(sqlCust, 1,
                new { name = "%" + search.BizName + "%" , Adrress = "%" + search.Adrress + "%", city= search .City});


            return users;

        }
    }

}