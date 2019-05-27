using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Ada.Core.Tools;
using Ada.Services.Setting;
using Files.Models;
using Files.Tool;
using Ionic.BZip2;
using Ionic.Zip;
using Thumbnail = Files.Tool.Thumbnail;

namespace Files.Services
{
    public class FileService: IFileService
    {
        private FileConfig _config;
        public FileService(ISettingService settingService)
        {
            _config = settingService.GetSetting<FileConfig>();
        }

        public UploadResult FileSaveAs(UploadView view)
        {
            var result=new UploadResult();
            try
            {
                string fileExt = Path.GetExtension(view.UploadFileName).Trim('.'); //文件扩展名，不含“.”
                string newFileName = GetRamCode() + "." + fileExt; //随机生成新的文件名
                string newThumbnailFileName = "thumb_" + newFileName; //随机生成缩略图文件名

                string upLoadPath = GetUpLoadPath(); //本地上传目录相对路径
                string fullUpLoadPath = Utils.GetMapPath(upLoadPath); //本地上传目录的物理路径
                string newFilePath = upLoadPath + newFileName; //本地上传后的路径
                string newThumbnailPath = upLoadPath + newThumbnailFileName; //本地上传后的缩略图路径

                byte[] thumbData = null; //缩略图文件流

                //检查文件字节数组是否为NULL
                if (view.PostedFileByte == null)
                {
                    result.ErrorMsg = "请选择要上传的文件！";
                    return result;
                }
                //检查文件扩展名是否合法
                if (!CheckFileExt(fileExt))
                {
                    result.ErrorMsg = "不允许上传" + fileExt + "类型的文件！";
                    return result;
                }
                //检查文件大小是否合法
                if (!CheckFileSize(fileExt, view.PostedFileByte.Length))
                {
                    result.ErrorMsg = "文件超过限制的大小！";
                    return result;
                }

                //如果是图片，检查图片是否超出最大尺寸，是则裁剪
                if (IsImage(fileExt) && (_config.ImageHeigth > 0 || _config.ImageWidth > 0))
                {
                    view.PostedFileByte = Thumbnail.MakeThumbnailImage(view.PostedFileByte, fileExt, _config.ImageWidth, _config.ImageHeigth);
                }
                //如果是图片，检查是否需要生成缩略图，是则生成
                if (IsImage(fileExt) && view.IsThumbnail && _config.ThumbnailWidth > 0 && _config.ThumbnailHeigth > 0)
                {
                    thumbData = Thumbnail.MakeThumbnailImage(view.PostedFileByte, fileExt, _config.ThumbnailWidth, _config.ThumbnailHeigth, _config.Thumbnailmode);
                }
                else
                {
                    newThumbnailPath = newFilePath; //不生成缩略图则返回原图
                }
                //如果是图片，检查是否需要打水印
                if (IsWaterMark(fileExt) && view.IsWatermark)
                {
                    switch (_config.Watermarktype)
                    {
                        case 1:
                            view.PostedFileByte = WaterMark.AddImageSignText(view.PostedFileByte, fileExt, _config.Watermarktext, _config.Watermarkposition,
                                _config.Watermarkimgquality, _config.Watermarkfont, _config.Watermarkfontsize);
                            break;
                        case 2:
                            view.PostedFileByte = WaterMark.AddImageSignPic(view.PostedFileByte, fileExt, _config.Watermarkpic, _config.Watermarkposition,
                                _config.Watermarkimgquality, _config.Watermarktransparency);
                            break;
                    }
                }

                //分发不同的上传方式处理
                switch (_config.UploadServer)
                {
                    //case "aliyun": //阿里云OSS对象存储
                    //    //检查配置是否完善
                    //    if (string.IsNullOrEmpty(_config.osssecretid) || string.IsNullOrEmpty(sysConfig.osssecretkey) || string.IsNullOrEmpty(sysConfig.ossendpoint))
                    //    {
                    //        return "{\"status\": 0, \"msg\": \"文件上传配置未完善，无法上传\"}";
                    //    }
                    //    //初始化阿里云配置
                    //    API.Cloud.AliyunOss aliyun = new API.Cloud.AliyunOss(sysConfig.ossendpoint, sysConfig.osssecretid, sysConfig.osssecretkey);
                    //    string result = string.Empty; //返回信息

                    //    //保存主文件
                    //    if (!aliyun.PutObject(byteData, sysConfig.ossbucket, newFilePath, sysConfig.ossdomain, out result))
                    //    {
                    //        return "{\"status\": 0, \"msg\": \"" + result + "\"}";
                    //    }
                    //    newFilePath = result; //将地址赋值给新文件地址

                    //    //保存缩略图文件
                    //    if (thumbData != null)
                    //    {
                    //        aliyun.PutObject(thumbData, sysConfig.ossbucket, newThumbnailPath, sysConfig.ossdomain, out result);
                    //        newThumbnailPath = result; //将缩略图地址赋值
                    //    }
                    //    break;

                    case "qcloud": //腾讯云COS对象存储

                        break;
                    default: //本地服务器
                        //检查本地上传的物理路径是否存在，不存在则创建
                        if (!Directory.Exists(fullUpLoadPath))
                        {
                            Directory.CreateDirectory(fullUpLoadPath);
                        }
                        //保存主文件
                        FileHelper.SaveFile(view.PostedFileByte, fullUpLoadPath + newFileName);
                        //保存缩略图文件
                        if (thumbData != null)
                        {
                            FileHelper.SaveFile(thumbData, fullUpLoadPath + newThumbnailFileName);
                        }
                        break;
                }
                result.FileName = view.UploadFileName;
                result.FilePath = newFilePath;
                result.ThumbnailPath = newThumbnailPath;
                result.FileSize = view.PostedFileByte.Length;
                result.FileExt = fileExt;
                result.IsSuccess = true;
                return result;
            }
            catch(Exception ex)
            {
                result.ErrorMsg = "上传过程中发生意外错误！"+ex.Message;
                return result;
            }
        }


        /// <summary>
        /// 裁剪图片并保存
        /// </summary>
        public UploadResult CropSaveAs(string fileUri, int maxWidth, int maxHeight, int cropWidth, int cropHeight, int X, int Y)
        {
            var result = new UploadResult();
            string fileExt = Path.GetExtension(fileUri).Trim('.'); //文件扩展名，不含“.”
            if (string.IsNullOrEmpty(fileExt) || !IsImage(fileExt))
            {
                result.ErrorMsg = "该文件不是图片！";
                return result;
            }

            byte[] byteData = null;
            //判断是否远程文件
            if (fileUri.ToLower().StartsWith("http://") || fileUri.ToLower().StartsWith("https://"))
            {
                WebClient client = new WebClient();
                byteData = client.DownloadData(fileUri);
                client.Dispose();
            }
            else //本地源文件
            {
                string fullName = Utils.GetMapPath(fileUri);
                if (File.Exists(fullName))
                {
                    FileStream fs = File.OpenRead(fullName);
                    BinaryReader br = new BinaryReader(fs);
                    br.BaseStream.Seek(0, SeekOrigin.Begin);
                    byteData = br.ReadBytes((int)br.BaseStream.Length);
                    fs.Close();
                }
            }
            //裁剪后得到文件流
            byteData = Thumbnail.MakeThumbnailImage(byteData, fileExt, maxWidth, maxHeight, cropWidth, cropHeight, X, Y);
            //删除原图
            DeleteFile(fileUri);
            //保存制作好的缩略图
            return FileSaveAs(new UploadView(){PostedFileByte = byteData,UploadFileName = fileUri});
        }

        /// <summary>
        /// 保存远程文件到本地
        /// </summary>
        /// <param name="sourceUri">URI地址</param>
        /// <returns>上传后的路径</returns>
        public UploadResult RemoteSaveAs(string sourceUri)
        {
            var result = new UploadResult();
            if (!IsExternalIPAddress(sourceUri))
            {
                result.ErrorMsg= "URL地址有误";
                return result;
            }
            var request = HttpWebRequest.Create(sourceUri) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    result.ErrorMsg = "返回结果： " + response.StatusCode + ", " + response.StatusDescription ;
                    return result;
                }
                if (response.ContentType.IndexOf("image") == -1)
                {
                    result.ErrorMsg = "此地址不是图片";
                    return result;
                }
                try
                {
                    byte[] byteData = FileHelper.ConvertStreamToByteBuffer(response.GetResponseStream());
                    return FileSaveAs(new UploadView(){PostedFileByte = byteData,UploadFileName = sourceUri});
                }
                catch (Exception e)
                {
                    result.ErrorMsg = "抓取错误：" + e.Message;
                    return result;
                }
            }
        }


        /// <summary>
        /// 删除上传文件
        /// </summary>
        /// <param name="fileUri">相对地址或网址</param>
        public void DeleteFile(string fileUri)
        {
            //分发不同的上传方式处理
            switch (_config.UploadServer)
            {
                //case "aliyun": //阿里云OSS对象存储
                //    //检查配置是否完善
                //    if (string.IsNullOrEmpty(sysConfig.osssecretid) || string.IsNullOrEmpty(sysConfig.osssecretkey) || string.IsNullOrEmpty(sysConfig.ossendpoint))
                //    {
                //        return;
                //    }
                //    //初始化配置
                //    API.Cloud.AliyunOss aliyun = new API.Cloud.AliyunOss(sysConfig.ossendpoint, sysConfig.osssecretid, sysConfig.osssecretkey);
                //    string result = string.Empty; //返回信息
                //    aliyun.DeleteObject(sysConfig.ossbucket, fileUri, sysConfig.ossdomain, out result);
                //    break;
                case "qcloud": //腾讯云COS对象存储

                    break;
                default: //本地服务器
                    //文件不应是上传文件，防止跨目录删除
                    if (fileUri.IndexOf("..") == -1 && fileUri.ToLower().StartsWith("/" + _config.UploadDirectory.ToLower()))
                    {
                        FileHelper.DeleteUpFile(fileUri);
                    }
                    break;
            }
        }



        public byte[] ZipFiles(List<string> files,string dicName=null)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = null;
                using (ZipFile zip = new ZipFile(Encoding.Default))
                {
                    if (string.IsNullOrWhiteSpace(dicName))
                    {
                        zip.AddFiles(files);
                    }
                    else
                    {
                        zip.AddFiles(files,dicName);
                    }
                    
                    zip.Save(ms);
                    buffer = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(buffer, 0, buffer.Length);
                }
                return buffer;
            }
        }
        public byte[] ZipFiles(List<SelectListItem> files)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = null;
                using (ZipFile zip = new ZipFile(Encoding.Default))
                {
                    foreach (var item in files)
                    {
                        zip.AddFile(item.Value, item.Text);
                    }

                    zip.Save(ms);
                    buffer = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(buffer, 0, buffer.Length);
                }
                return buffer;
            }
        }













        /// <summary>
        /// 返回上传目录相对路径
        /// </summary>
        private string GetUpLoadPath()
        {
            string path = "/"+_config.UploadDirectory + "/"; //站点目录+上传目录
            switch (_config.SaveDirectoryType)
            {
                case 1: //按年月日每天一个文件夹
                    path += DateTime.Now.ToString("yyyyMMdd");
                    break;
                default: //按年月/日存入不同的文件夹
                    path += DateTime.Now.ToString("yyyyMM") + "/" + DateTime.Now.ToString("dd");
                    break;
            }
            return path + "/";
        }

        /// <summary>
        /// 是否需要打水印
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        private bool IsWaterMark(string _fileExt)
        {
            //判断是否开启水印
            if (_config.Watermarktype > 0)
            {
                //判断是否可以打水印的图片类型
                ArrayList al = new ArrayList();
                al.Add("bmp");
                al.Add("jpeg");
                al.Add("jpg");
                al.Add("png");
                if (al.Contains(_fileExt.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否为图片文件
        /// </summary>
        /// <param name="fileExt">文件扩展名，不含“.”</param>
        private bool IsImage(string fileExt)
        {
            ArrayList al = new ArrayList();
            al.Add("bmp");
            al.Add("jpeg");
            al.Add("jpg");
            al.Add("gif");
            al.Add("png");
            if (al.Contains(fileExt.ToLower()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查是否为合法的上传文件
        /// </summary>
        private bool CheckFileExt(string fileExt)
        {
            //检查危险文件
            string[] excExt = { "asp", "aspx", "ashx", "asa", "asmx", "asax", "php", "jsp", "htm", "html" };
            for (int i = 0; i < excExt.Length; i++)
            {
                if (excExt[i].ToLower() == fileExt.ToLower())
                {
                    return false;
                }
            }
            //检查合法文件
            string[] allowExt = (_config.FileExt + "," + _config.VideoExt).Split(',');
            for (int i = 0; i < allowExt.Length; i++)
            {
                if (allowExt[i].ToLower() == fileExt.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查文件大小是否合法
        /// </summary>
        /// <param name="fileExt">文件扩展名，不含“.”</param>
        /// <param name="fileSize">文件大小(B)</param>
        private bool CheckFileSize(string fileExt, int fileSize)
        {
            //将视频扩展名转换成ArrayList
            var lsVideoExt = new ArrayList(_config.VideoExt.ToLower().Split(','));
            //判断是否为图片文件
            if (IsImage(fileExt))
            {
                if (_config.ImageMaxSize > 0 && fileSize > _config.ImageMaxSize * 1024)
                {
                    return false;
                }
            }
            else if (lsVideoExt.Contains(fileExt.ToLower()))
            {
                if (_config.VideoMaxSize > 0 && fileSize > _config.VideoMaxSize * 1024)
                {
                    return false;
                }
            }
            else
            {
                if (_config.FileMaxSize > 0 && fileSize > _config.FileMaxSize * 1024)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查文件地址是否文件服务器地址
        /// </summary>
        /// <param name="url">文件地址</param>
        private bool IsExternalIPAddress(string url)
        {
            var uri = new Uri(url);
            switch (uri.HostNameType)
            {
                case UriHostNameType.Dns:
                    var ipHostEntry = Dns.GetHostEntry(uri.DnsSafeHost);
                    foreach (IPAddress ipAddress in ipHostEntry.AddressList)
                    {
                        byte[] ipBytes = ipAddress.GetAddressBytes();
                        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            if (!IsPrivateIP(ipAddress))
                            {
                                return true;
                            }
                        }
                    }
                    break;

                case UriHostNameType.IPv4:
                    return !IsPrivateIP(IPAddress.Parse(uri.DnsSafeHost));
            }
            return false;
        }

        /// <summary>
        /// 检查IP地址是否本地服务器地址
        /// </summary>
        /// <param name="myIPAddress">IP地址</param>
        private bool IsPrivateIP(IPAddress myIPAddress)
        {
            if (IPAddress.IsLoopback(myIPAddress)) return true;
            if (myIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                byte[] ipBytes = myIPAddress.GetAddressBytes();
                // 10.0.0.0/24 
                if (ipBytes[0] == 10)
                {
                    return true;
                }
                // 172.16.0.0/16
                else if (ipBytes[0] == 172 && ipBytes[1] == 16)
                {
                    return true;
                }
                // 192.168.0.0/16
                else if (ipBytes[0] == 192 && ipBytes[1] == 168)
                {
                    return true;
                }
                // 169.254.0.0/16
                else if (ipBytes[0] == 169 && ipBytes[1] == 254)
                {
                    return true;
                }
            }
            return false;
        }
        private string GetRamCode()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        }

        
    }
}