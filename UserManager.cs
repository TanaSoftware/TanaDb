using System;

using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

using System.Net;

using System.Net.Mail;

using Tor.Dal;



namespace Tor

{

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

    }



    public class UserSearch

    {

        public int Id { get; set; }

        public string User { get; set; }



        public string tel { get; set; }



        public string Adrress { get; set; }



        public string City { get; set; }



        [Required]

        public string guid { get; set; }

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

            if (IsCustomerTelExist(customer.tel))

            {

                userDetails.ErrorMsg = "מספר טלפון קיים במערכת";

                return userDetails;

            }

            if (!AddCustomerToDb(customer, 0))

            {

                userDetails.ErrorMsg = "ארעה שגיאה";

                return userDetails;

            }

            string guid = Guid.NewGuid().ToString();

            customer.guid = guid;

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

            try

            {

                MailMessage mail = new MailMessage("shilacohen@walla.com", "shilacohen@walla.com");

                SmtpClient client = new SmtpClient();

                client.UseDefaultCredentials = true;

                client.EnableSsl = true;

                NetworkCredential cred = new NetworkCredential("shilacoh3@gmail.com", "moon5honey");

                client.Credentials = cred;

                client.Port = 587;

                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                client.Host = "smtp.gmail.com";

                mail.Subject = "הרשמה למערכת תורים";

                mail.Body = " שלום - להמשך רישום למערכת תורים נא ללחוץ על הקישור הבא - " + "http://localhost:58055/user/ConfirmCustomeRegister/" + user.guid;

                client.Send(mail);

            }

            catch (Exception ex)

            {

                Logger.Write(ex);

                return false;

            }

            return true;

        }

        private bool AddCustomerToDb(CustomerObj user, int userId)

        {

            bool Issuccess = false;

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

                Issuccess = false;

            }



            Issuccess = true;

            return Issuccess;

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

        public UserDetails RegisterUser(UserObj user)

        {

            UserDetails userObjWrapper = CheckIsValidUserRegister(user);

            if (userObjWrapper.ErrorMsg != "")

            {

                if (IsUserExist(user.User))

                {

                    userObjWrapper.ErrorMsg = "שם משתמש קיים במערכת";

                    userObjWrapper.guid = "";

                    return userObjWrapper;

                }

                if (IsMailExist(user.Email))

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

            try

            {

                //MailMessage mail = new MailMessage("shilacohen@walla.com", "shilacohen@walla.com");

                //SmtpClient client = new SmtpClient();

                //client.UseDefaultCredentials = true;

                //client.EnableSsl = true;

                //NetworkCredential cred = new NetworkCredential("shilacoh3@gmail.com", "moon5honey");

                //client.Credentials = cred;

                //client.Port = 587;

                //client.DeliveryMethod = SmtpDeliveryMethod.Network;



                //client.Host = "smtp.gmail.com";

                //mail.Subject = "הרשמה למערכת תורים";

                //mail.Body = " שלום - להמשך רישום למערכת תורים נא ללחוץ על הקישור הבא - " + "http://localhost:58055/user/ConfirmRegister/" + user.guid;

                //client.Send(mail);

            }

            catch (Exception ex)

            {

                Logger.Write(ex);

                return false;

            }

            return true;

        }

        public string ActivateCustomer(string guid)

        {

            if (IsCustomerGuidExists(guid))

            {

                try

                {

                    string sqlUsersUpdate = "UPDATE Customer SET [Active]=@Active where [guid]=@guid;";

                    DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

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

            if (IsUserGuidExists(guid))

            {

                try

                {

                    string sqlUsersUpdate = "UPDATE Users SET [Active]=@Active where [guid]=@guid;";

                    DataBaseRetriever db = new DataBaseRetriever(ConfigManager.ConnectionString);

                    var affectedRows = db.Execute(sqlUsersUpdate, 1, new { Active = guid, guid = guid });

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

                            db.Execute(sqlUserActivity, 1, item.Value[i]);

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

        private bool IsUserGuidExists(string guid)

        {

            try

            {

                string sql = "SELECT count(1) FROM Users WHERE [Active] = @guid;";

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

    }

}




