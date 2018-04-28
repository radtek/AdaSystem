using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ada.Framework.Filter
{
  public  class DeleteFileAttribute: ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Flush();
            //將當前filter context轉換成具體操作的文件并獲取文件路徑
            string filePath = (filterContext.Result as FilePathResult)?.FileName;
            //有文件路徑后就可以直接刪除相關文件了
            System.IO.File.Delete(filePath ?? throw new InvalidOperationException());
        }
    }
}
