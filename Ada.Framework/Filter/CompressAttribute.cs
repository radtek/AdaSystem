using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ada.Framework.Filter
{
   public class CompressAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string format = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
            if (!string.IsNullOrWhiteSpace(format))
            {
                if (format.Contains("gzip"))
                {
                    filterContext.HttpContext.Response.AddHeader("Content-Encoding", "GZIP");
                    filterContext.HttpContext.Response.Filter = new GZipStream(filterContext.HttpContext.Response.Filter, CompressionMode.Compress);
                }
            }
        }
    }
}
