

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
            
            var response = new HttpResponseMessage();
            
            response = Request.CreateResponse(HttpStatusCode.Moved);
            string fullyQualifiedUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/Biz.html?ver="+ConfigManager.version + "&id=" + id;
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