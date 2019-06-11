

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

/// <summary>

/// Summary description for QueController

/// </summary>

namespace Tor

{

    public class SiteController : ApiController

    {

        [HttpGet]
        [ActionName("Biz")]
        public HttpResponseMessage GetBiz(string id)
        {

            //UserManager u = new UserManager();
            //string res = u.GetUserByBizName(id);
            //if (res == "")
            //    return new HttpResponseMessage(HttpStatusCode.NotFound);

            var response = new HttpResponseMessage();
            //var cookie = new CookieHeaderValue("BizData", res);
            //cookie.Expires = DateTimeOffset.Now.AddDays(1);
            //cookie.Domain = Request.RequestUri.Host;
            //cookie.Path = "/";
            //response.Headers.AddCookies(new CookieHeaderValue[] { cookie });
            
            response = Request.CreateResponse(HttpStatusCode.Moved);
            string fullyQualifiedUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/TorApp/Biz.html?v="+ConfigManager.version  +"&id="+id;
            response.Headers.Location = new Uri(fullyQualifiedUrl);
            return response;
        }

        [HttpGet]
        [ActionName("GetVersion")]
        public string GetVersion()
        {
            return ConfigManager.version;
        }



    }

}
