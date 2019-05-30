
using System.Collections.Generic;

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

        [HttpGet]

        [ActionName("GetUserBiz")]

        public UserSearch GetUserBiz(string id)

        {
            UserManager u = new UserManager();
            return u.GetUserByBizName(id);
        }
        [HttpPost]

        [ActionName("UserAddCustmer")]

        public string UserAddCustmer(CustomerObjBase id)

        {

            UserManager userManager = new UserManager();

            return userManager.UserAddHisCustomer(id);

        }



        [HttpGet]

        [ActionName("GetCity")]

        public IEnumerable<City> GetCity()

        {

            UserManager userManager = new UserManager();

            return userManager.GetCities();

        }

    }

}
