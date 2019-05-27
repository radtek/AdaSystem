using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ada.Framework.Filter;
using Files.Models;
using Files.Services;
using Files.Tool;

namespace Files.Controllers
{
    public class UploadController : BaseController
    {
        private readonly IFileService _service;

        public UploadController(IFileService service)
        {
            _service = service;
        }
        [HttpPost]
        public ActionResult Ajax(PostView postView)
        {
            var file = Request.Files[postView.input];
            if (file == null)
            {
                return Json(new { State = 0, Msg = "请选择要上传的文件" });
            }
            //获取文件信息
            string fileName = file.FileName;
            byte[] byteData = FileHelper.ConvertStreamToByteBuffer(file.InputStream); //获取文件流
            var result = _service.FileSaveAs(new UploadView
            {
                PostedFileByte = byteData,
                UploadFileName = fileName,
                IsThumbnail = postView.thumbnail,
                IsWatermark = postView.water
            });
            return Json(new { State = result.IsSuccess ? 1 : 0, Data = result, Msg = result.ErrorMsg });
        }
    }
}