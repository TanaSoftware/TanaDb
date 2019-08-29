
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Tor.Controllers

{

    public class UserController : ApiController

    {

        [HttpGet]

        [ActionName("ConfirmRegister")]

        public string ConfirmRegister(string id)

        {

            UserManager userManager = new UserManager();

            return userManager.ActivateUser(id);

        }

        [HttpGet]

        [ActionName("ConfirmCustomeRegister")]

        public string ConfirmCustomeRegister(string id)

        {

            UserManager userManager = new UserManager();

            return userManager.ActivateCustomer(id);

        }
        [HttpGet]
        [ActionName("ResetPasswordPhase1")]
        public string ResetPasswordPhase1(string id)
        {
            UserManager userManager = new UserManager();

            return userManager.ResetPasswordPhase1(id);
        }

        [HttpGet]

        [ActionName("ResetPassword")]

        public HttpResponseMessage ResetPassword(string id)

        {
            var response = new HttpResponseMessage();

            UserManager userManager = new UserManager();
            if(userManager.IsCustomerGuidExists(id) || userManager.IsUserGuidExists(id))
            {
                
                response = Request.CreateResponse(HttpStatusCode.Moved);
                string fullyQualifiedUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/ResetPass.html?v=" + ConfigManager.version + "&id=" + id;
                response.Headers.Location = new Uri(fullyQualifiedUrl);
                return response;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            return response;
        }

        [HttpGet]
        [ActionName("GetUserByGuid")]
        public CustomerObj GetUserByGuid(string id)
        {
            UserManager userManager = new UserManager();

            return userManager.GetUserByGuid(id);
        }

        [HttpGet]
        [ActionName("ConfirmAddedCustomer")]

        public HttpResponseMessage ConfirmAddedCustomer(string id, string id2)

        {

            var response = new HttpResponseMessage();           

            response = Request.CreateResponse(HttpStatusCode.Moved);
            string fullyQualifiedUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/TorApp/CustomerBiz.html?v=" + ConfigManager.version + "&id=" + id + "&user="+id2;
            response.Headers.Location = new Uri(fullyQualifiedUrl);
            return response;

        }

        [HttpPost]

        [ActionName("PostEmail")]
        public bool PostEmail(MailObj id)
        {
            UserManager userManager = new UserManager();

            return userManager.ContanctUs(id);
        }

        [HttpPost]

        [ActionName("ConfirmResetPassword")]

        public string ConfirmResetPassword(ResetPasswordObj id)
        {
            if(id.pass1.Length<8)
            {
                return "נא להזין לפחות 8 תווים";
            }
            if (id.pass2.Length < 8)
            {
                return "נא להזין לפחות 8 תווים";
            }
            if (id.pass1 != id.pass2)
            {

                return "סיסמה לא זהה";
            }
            UserManager userManager = new UserManager();

            return userManager.ResetPasswordPhase2(id.guid,id.pass1,id.pass2);

        }

        [HttpPost]

        [ActionName("Register")]

        public UserDetails Register(UserObj id)

        {

            UserManager userManager = new UserManager();

            return userManager.RegisterUser(id);

        }

        [HttpPost]

        [ActionName("RegisterCustmer")]

        public UserDetails RegisterCustmer(CustomerObj id)

        {

            UserManager userManager = new UserManager();

            return userManager.RegisterCustmer(id);

        }


        [HttpPost]

        [ActionName("AddUserExtraActivity")]

        public string AddUserExtraActivity(UserExtraActivity id)

        {

            UserManager userManager = new UserManager();

            return userManager.AddUserExtraActivity(id);

        }

        [HttpPost]

        [ActionName("Login")]

        public UserDetails Login(user id)

        {

            UserManager userManager = new UserManager();

            return userManager.Login(id);

        }



        [HttpPost]

        [ActionName("GetCustomers")]

        public CustomerObjWrapper GetCustomers(CustomerUser id)

        {

            UserManager userManager = new UserManager();

            return userManager.GetCustomersPerUser(id);

        }



        [HttpPost]

        [ActionName("GetUsers")]

        public UsersObjWrapper GetUsers(UserSearch id)

        {

            UserManager userManager = new UserManager();

            return userManager.GetUsers(id);

        }



        [HttpPost]

        [ActionName("GetUserById")]

        public UserDetails GetUserById(UserSearch id)

        {

            UserManager userManager = new UserManager();

            return userManager.GetUserById(id);

        }

        [HttpPost]

        [ActionName("SearchCustomer")]

        public CustomerObjBase SearchCustomer(CustomerSearch id)

        {

            UserManager userManager = new UserManager();

            return userManager.SearchCustomer(id);

        }

        [HttpPost]

        [ActionName("AddGropWithFriends")]

        public string AddGropWithFriends(Groups id)

        {

            UserManager userManager = new UserManager();

            return userManager.AddGropWithFriends(id);

        }

        [HttpPost]

        [ActionName("GetGroupsPerCustomer")]

        public List<Groups> GetGroupsPerCustomer(Groups id)
        {

            UserManager userManager = new UserManager();

            return userManager.GetGroupsPerCustomer(id.guid,id.groupId);

        }

        [HttpPost]

        [ActionName("AddCustomerToGroup")]

        public string AddCustomerToGroup(AddCustomerGroup id)
        {

            UserManager userManager = new UserManager();

            return userManager.AddCustomerToGroup(id.guid, id.GroupId,id.CustomerId);

        }

        [HttpGet]

        [ActionName("GetUserBiz")]

        public UserSearch GetUserBiz(string id)

        {
            UserManager u = new UserManager();
            return u.GetUserByBizName(id);
        }

        [HttpPost]
        [ActionName("GetUserDetails")]
        public UserDetailsWrapper GetUserDetails(UserSearch id)
        {
            UserManager u = new UserManager();
            return  u.GetUserDetails(id);
        }

        [HttpPost]
        [ActionName("DeleteUserActivity")]
        public string DeleteUserActivity(UserActivity id)
        {
            UserManager u = new UserManager();
            return u.DeleteUserActivity(id);
        }

        [HttpPost]
        [ActionName("UpdateEmployee")]
        public string UpdateEmployee(EmployeeDataWrapper id)
        {
            UserManager u = new UserManager();
            return u.UpdateEmployee(id);

        }
        [HttpPost]
        [ActionName("DeleteEmployee")]
        public string DeleteEmployee(BizTypeObj id)
        {
            UserManager u = new UserManager();
            return u.DeleteEmployee(id);

        }

        [HttpPost]
        [ActionName("AddUserActivity")]
        public int AddUserActivity(UserActivity id)
        {
            UserManager u = new UserManager();
            return u.AddUserActivity(id);

        }

        [HttpPost]
        [ActionName("AddEmployee")]
        public object AddEmployee(List<BizTypeObj> id)
        {
            UserManager u = new UserManager();
            return u.AddEmployee(id);

        }

        [HttpPost]
        [ActionName("UserAddCustmer")]
        public string UserAddCustmer(AddedCustomerByUser id)

        {

            UserManager userManager = new UserManager();

            return userManager.UserAddHisCustomer(id);

        }



        

        [HttpPost]
        [ActionName("GetCustomerDetails")]

        public IEnumerable<CustomerObj>  GetCustomerDetails(UserSearch id)

        {

            UserManager userManager = new UserManager();

            return userManager.GetCustomerDetails(id);

        }

        [HttpPost]
        [ActionName("SaveUser")]

        public string SaveUser(UserObj id)

        {

            UserManager userManager = new UserManager();

            return userManager.SaveUser(id);

        }

        [HttpPost]

        [ActionName("SaveCustomer")]

        public string SaveCustomer(CustomerObj id)

        {

            UserManager userManager = new UserManager();

            return userManager.SaveCustomer(id);

        }

        [HttpGet]

        [ActionName("GetCity")]

        public IEnumerable<City> GetCity()

        {

            UserManager userManager = new UserManager();

            return userManager.GetCities();

        }

        [HttpPost]

        [ActionName("SearchBiz")]

        public IEnumerable<SearchUser> SearchBiz(SearchUser search)
        {
            UserManager userManager = new UserManager();

            return userManager.SearchBiz(search);
        }
    }

}
