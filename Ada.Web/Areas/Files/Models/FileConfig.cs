using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Files.Models
{
    public class FileConfig
    {
        public FileConfig()
        {
            UploadDirectory = "upload";
            FileExt = "rar,zip,jpg,jpeg,png,gif,bmp,mp3";
            VideoExt = "mp4,flv";
            FileMaxSize = 51200;
            VideoMaxSize = 102400;
            ImageMaxSize = 10240;
            ImageWidth = 1600;
            ImageHeigth = 1600;
            ThumbnailWidth = 300;
            ThumbnailHeigth = 300;
            Watermarkimgquality = 80;
            Watermarktransparency = 5;
            Watermarkfontsize = 12;
            SaveDirectoryType = 2;
            Watermarkposition = 9;
            Watermarktype = 2;
            Watermarkpic = "watermark.png";
        }
        /// <summary>
        /// 上传云端 aliyun
        /// </summary>
        [Display(Name = "上传云端")]
        public string UploadServer { get; set; }
        /// <summary>
        /// 上传目录
        /// </summary>
        [Display(Name = "上传目录")]
        public string UploadDirectory { get; set; }
        /// <summary>
        /// 保存方式
        /// </summary>
        [Display(Name = "保存方式")]
        public int SaveDirectoryType { get; set; }
        /// <summary>
        /// 附件类型
        /// </summary>
        [Display(Name = "附件类型")]
        public string FileExt { get; set; }
        /// <summary>
        /// 视频类型
        /// </summary>
        [Display(Name = "视频类型")]
        public string VideoExt { get; set; }
        /// <summary>
        /// 文件上传最大容量
        /// </summary>
        [Display(Name = "文件上传最大容量")]
        public int FileMaxSize { get; set; }
        /// <summary>
        /// 视频上传最大容量
        /// </summary>
        [Display(Name = "视频上传最大容量")]
        public int VideoMaxSize { get; set; }
        /// <summary>
        /// 图片上传最大容量
        /// </summary>
        [Display(Name = "图片上传最大容量")]
        public int ImageMaxSize { get; set; }
        /// <summary>
        /// 图片上传最大宽度
        /// </summary>
        [Display(Name = "宽度")]
        public int ImageWidth { get; set; }
        /// <summary>
        /// 图片上传最大高度
        /// </summary>
        [Display(Name = "高度")]
        public int ImageHeigth { get; set; }
        /// <summary>
        /// 图片上传最大宽度
        /// </summary>
        [Display(Name = "宽度")]
        public int ThumbnailWidth { get; set; }
        /// <summary>
        /// 图片上传最大高度
        /// </summary>
        [Display(Name = "高度")]
        public int ThumbnailHeigth { get; set; }
        /// <summary>
        /// 缩略图生成方式
        /// </summary>
        [Display(Name = "缩略图生成方式")]
        public string Thumbnailmode { get; set; }
        /// <summary>
        /// 图片水印类型
        /// </summary>
        [Display(Name = "图片水印类型")]
        public int Watermarktype
        {
            get;
            set;
        }
        /// <summary>
        /// 图片水印位置
        /// </summary>
        [Display(Name = "图片水印位置")]
        public int Watermarkposition
        {
            get;
            set;
        }
        /// <summary>
        /// 图片生成质量
        /// </summary>
        [Display(Name = "图片生成质量")]
        public int Watermarkimgquality
        {
            get;
            set;
        }
        /// <summary>
        /// 图片水印文件
        /// </summary>
        [Display(Name = "图片水印文件")]
        public string Watermarkpic
        {
            get;
            set;
        }
        /// <summary>
        /// 水印透明度
        /// </summary>
        [Display(Name = "水印透明度")]
        public int Watermarktransparency
        {
            get;
            set;
        }
        /// <summary>
        /// 水印文字
        /// </summary>
        [Display(Name = "水印文字")]
        public string Watermarktext
        {
            get;
            set;
        }
        /// <summary>
        /// 文字字体
        /// </summary>
        [Display(Name = "文字字体")]
        public string Watermarkfont
        {
            get;
            set;
        }
        /// <summary>
        /// 文字大小(像素)
        /// </summary>
        [Display(Name = "文字大小")]
        public int Watermarkfontsize
        {
            get;
            set;
        }
    }
}