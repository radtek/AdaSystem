using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;
using Ada.Framework.UploadFile;

namespace Admin.Controllers
{
    public class CommonController : BaseController
    {
        [HttpPost]
        public ActionResult UploadImage()
        {
            UEditorModel uploadConfig = new UEditorModel()
            {
                AllowExtensions = UEditorConfig.GetStringList("imageAllowFiles"),
                PathFormat = UEditorConfig.GetString("imagePathFormat"),
                SizeLimit = UEditorConfig.GetInt("imageMaxSize"),
                UploadFieldName = UEditorConfig.GetString("imageFieldName")
            };
            var file = Request.Files[uploadConfig.UploadFieldName];
            var uploadFileName = file.FileName;
            var fileExtension = Path.GetExtension(uploadFileName).ToLower();
            if (!uploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension))
            {
                return Json(new { State = 0, Msg = "图片类型不匹配" });
            }
            if (!(file.ContentLength < uploadConfig.SizeLimit))
            {
                return Json(new { State = 0, Msg = "上传的图片最大只能为：" + uploadConfig.SizeLimit + "B" });
            }
            var uploadFileBytes = new byte[file.ContentLength];
            try
            {
                file.InputStream.Read(uploadFileBytes, 0, file.ContentLength);
            }
            catch (Exception)
            {
                return Json(new { State = 0, Msg = "上传异常，系统错误或者网络异常" });
            }
            var savePath = PathFormatter.Format(uploadFileName, uploadConfig.PathFormat);
            var localPath = Server.MapPath(savePath);
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                }
                System.IO.File.WriteAllBytes(localPath, uploadFileBytes);
            }
            catch (Exception e)
            {
                return Json(new { State = 0, Msg = "上传异常，异常信息：" + e.Message });
            }
            return Json(new { State = 1, Msg = savePath });
        }
    }
}