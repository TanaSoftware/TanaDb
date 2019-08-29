

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

        [ActionName("IsQueUpdated")]
        public bool IsQueUpdated(Que id)

        {

            QueManager userManager = new QueManager();

            return userManager.IsQueUpdated(id.UserId,id.EmployeeId,id.CustomerId);

        }

        [HttpPost]

        [ActionName("DeleteCustomerQue")]

        public string DeleteCustomerQue(Que id)

        {

            QueManager userManager = new QueManager();

            return userManager.DeleteCustomerQue(id);

        }

        [HttpPost]

        [ActionName("DeleteGroup")]

        public string DeleteGroup(DeleteGroupCustomer id)

        {

            QueManager userManager = new QueManager();

            return userManager.DeleteGroup(id.guid,id.GroupId);

        }

        [HttpPost]

        [ActionName("DeleteCustomerFromGroup")]

        public string DeleteCustomerFromGroup(DeleteGroupCustomer id)

        {

            QueManager userManager = new QueManager();

            return userManager.DeleteCustomerFromGroup(id.guid, id.GroupId,id.CustomerId);

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
