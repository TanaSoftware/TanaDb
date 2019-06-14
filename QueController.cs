

using System.Collections.Generic;
using System.Web.Http;

/// <summary>

/// Summary description for QueController

/// </summary>

namespace Tor

{

    public class QueController : ApiController

    {

        [HttpPost]

        [ActionName("AddCustomerQue")]

        public string AddCustomerQue(Que id)

        {

            QueManager userManager = new QueManager();

            return userManager.AddCustomerQue(id);

        }



        [HttpPost]

        [ActionName("DeleteCustomerQue")]

        public string DeleteCustomerQue(Que id)

        {

            QueManager userManager = new QueManager();

            return userManager.DeleteCustomerQue(id);

        }



        [HttpPost]

        [ActionName("GetQue")]

        public List<QueData> GetQue(QueObj id)

        {

            QueManager userManager = new QueManager();

            return userManager.GetQue(id);

        }

    }

}