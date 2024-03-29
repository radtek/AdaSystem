﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ada.Framework.UploadFile
{
    public class UEditorModel
    {
        /// <summary>
        /// 文件命名规则
        /// </summary>
        public string PathFormat { get; set; }

        /// <summary>
        /// 上传表单域名称
        /// </summary>
        public string UploadFieldName { get; set; }

        /// <summary>
        /// 上传大小限制
        /// </summary>
        public int SizeLimit { get; set; }

        /// <summary>
        /// 上传允许的文件格式
        /// </summary>
        public string[] AllowExtensions { get; set; }

        /// <summary>
        /// 文件是否以 Base64 的形式上传
        /// </summary>
        public bool Base64 { get; set; }

        /// <summary>
        /// Base64 字符串所表示的文件名
        /// </summary>
        public string Base64Filename { get; set; }
    }
    public class UploadResult
    {
        public UploadState State { get; set; }
        public string Url { get; set; }
        public string OriginFileName { get; set; }

        public string ErrorMessage { get; set; }
    }
    public enum UploadState
    {
        Success = 0,
        SizeLimitExceed = -1,
        TypeNotAllow = -2,
        FileAccessError = -3,
        NetworkError = -4,
        Unknown = 1,
    }
    public enum ResultState
    {
        Success,
        InvalidParam,
        AuthorizError,
        IOError,
        PathNotFound
    }

    public class ListFile
    {
        public int Start { get; set; }
        public int Size { get; set; }
        public int Total { get; set; }
        public ResultState State { get; set; }
        public string PathToList { get; set; }
        public string[] FileList { get; set; }

        public string[] SearchExtensions { get; set; }
    }

}