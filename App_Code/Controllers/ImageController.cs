

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

/// <summary>

/// Summary description for QueController

/// </summary>

namespace Tor

{

    public class ImageController : ApiController

    {
        [HttpPost]
        public HttpResponseMessage UploadImage()
        {
            var exMessage = string.Empty;
            try
            {
                string uploadPath = "//img/";
                HttpPostedFile file = null;
                HttpPostedFile file2 = null;
                if (HttpContext.Current.Request.Files.Count > 2)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        error = true,
                        message = "נא לבחור 2 תמונות"
                    });
                }

                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    file = HttpContext.Current.Request.Files.Get("file");
                    file2 = HttpContext.Current.Request.Files.Get("file2");
                }
                // Check if we have a file
                if (null == file && file2==null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        error = true,
                        message = "ארעה שגיאה"
                    });

                // Make sure the file has content
                
                string exttension = System.IO.Path.GetExtension(file.FileName);
                string exttension2 = file2!=null? System.IO.Path.GetExtension(file2.FileName):"";
                List<string> _validExtensions = new List<string> { ".jpg", ".bmp", ".gif", ".png" };
                bool isImage = _validExtensions.Contains(exttension);
                bool isImage2 = _validExtensions.Contains(exttension2);
                if (isImage || isImage2)
                {
                    int userId = int.Parse(HttpContext.Current.Request.Params.Get("userId"));
                    string guid = HttpContext.Current.Request.Params.Get("guid");

                    UserManager um = new UserManager();

                    if (!um.IsUserGuidExists(guid))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new
                        {
                            error = true,
                            message = "ארעה שגיאה"
                        });
                    }

                    uploadPath += "User_" + userId;

                    if (!Directory.Exists(HttpContext.Current.Server.MapPath(uploadPath)))
                    {
                        // If it doesn't exist, create the directory
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(uploadPath));
                    }

                    //Upload File
                    if (exttension.Length > 0)
                        file.SaveAs(HttpContext.Current.Server.MapPath(uploadPath + "/Logo_" + userId+ exttension));
                    if(exttension2.Length>0)
                        file2.SaveAs(HttpContext.Current.Server.MapPath(uploadPath + "/back_" + userId + exttension2));
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        error = true,
                        message = "ארעה שגיאה"
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);

                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = true,
                    message = "ארעה שגיאה"
                });
            }
            return Request.CreateResponse(HttpStatusCode.OK,
                new { error = false, message = "" });
        }
    }

}
