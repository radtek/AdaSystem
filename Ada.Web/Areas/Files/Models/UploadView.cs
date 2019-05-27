using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Files.Models
{
    public class UploadView
    {
        public UploadView()
        {
            IsThumbnail = false;
            IsWatermark = false;
        }
        /// <summary>
        /// 文件流
        /// </summary>
        public byte[] PostedFileByte { get; set; }
        /// <summary>
        /// 是否缩略图
        /// </summary>
        public bool IsThumbnail { get; set; }
        /// <summary>
        /// 是否水印
        /// </summary>
        public bool IsWatermark { get; set; }
        /// <summary>
        /// 上传文件名
        /// </summary>
        public string UploadFileName { get; set; }
    }

    public class UploadResult
    {
        public UploadResult()
        {
            IsSuccess = false;
            FileSize = 0;
        }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 缩略图路径
        /// </summary>
        public string ThumbnailPath { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public int FileSize { get; set; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FileExt { get; set; }
    }
}