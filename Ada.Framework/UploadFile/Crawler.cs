﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Ada.Framework.UploadFile
{
    public class Crawler
    {
        public string SourceUrl { get; set; }
        public string ServerUrl { get; set; }
        public string State { get; set; }

        //private HttpServerUtility Server { get; set; }


        public Crawler(string sourceUrl)
        {
            this.SourceUrl = sourceUrl;
            //this.Server = server;
        }

        public Crawler Fetch()
        {
            if (!IsExternalIpAddress(this.SourceUrl))
            {
                State = "INVALID_URL";
                return this;
            }
            var request = HttpWebRequest.Create(this.SourceUrl) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    State = "Url returns " + response.StatusCode + ", " + response.StatusDescription;
                    return this;
                }
                if (response.ContentType.IndexOf("image") == -1)
                {
                    State = "Url is not an image";
                    return this;
                }
                ServerUrl = PathFormatter.Format(Path.GetFileName(this.SourceUrl), UEditorConfig.GetString("catcherPathFormat"));
                var savePath = HttpContext.Current.Server.MapPath(ServerUrl);
                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                }
                try
                {
                    var stream = response.GetResponseStream();
                    var reader = new BinaryReader(stream);
                    byte[] bytes;
                    using (var ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[4096];
                        int count;
                        while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            ms.Write(buffer, 0, count);
                        }
                        bytes = ms.ToArray();
                    }
                    File.WriteAllBytes(savePath, bytes);
                    State = "SUCCESS";
                }
                catch (Exception e)
                {
                    State = "抓取错误：" + e.Message;
                }
                return this;
            }
        }

        private bool IsExternalIpAddress(string url)
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

        private bool IsPrivateIP(IPAddress myIpAddress)
        {
            if (IPAddress.IsLoopback(myIpAddress)) return true;
            if (myIpAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) return false;
            byte[] ipBytes = myIpAddress.GetAddressBytes();
            // 10.0.0.0/24 
            if (ipBytes[0] == 10)
            {
                return true;
            }
            // 172.16.0.0/16
            if (ipBytes[0] == 172 && ipBytes[1] == 16)
            {
                return true;
            }
            // 192.168.0.0/16
            if (ipBytes[0] == 192 && ipBytes[1] == 168)
            {
                return true;
            }
            // 169.254.0.0/16
            return ipBytes[0] == 169 && ipBytes[1] == 254;
        }
    }
}