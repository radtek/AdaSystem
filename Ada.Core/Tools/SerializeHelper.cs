using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Ada.Core.Tools
{
   public class SerializeHelper
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value">对象</param>
        /// <returns></returns>
        public static string SerializeToString(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="str">对象字符串</param>
        /// <returns></returns>
        public static T DeserializeToObject<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="filename">文件路径</param>
        /// <returns></returns>
        public static object Load(Type type, string filename)
        {
            FileStream fs = null;
            try
            {
                // open the stream...
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(fs);
            }
            finally
            {
                fs?.Close();
            }
        }


        /// <summary>
        /// XML序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filename">文件路径</param>
        public static void Save(object obj, string filename)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(fs, obj);
            }
            finally
            {
                fs?.Close();
            }

        }

        /// <summary>
        /// 对象序列化成XMLString
        /// </summary>
        public static string XmlSerialize<T>(T obj)
        {
            string xmlstring;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            using (MemoryStream ms = new MemoryStream())
            {
                xmlSerializer.Serialize(ms, obj, ns);
                xmlstring = Encoding.UTF8.GetString(ms.ToArray());
            }
            return xmlstring;
        }
        /// <summary>
        /// XMLString反序列化成对象
        /// </summary>
        public static T XmlDeserialize<T>(string xmlString) where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (Stream xmlstream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            {
                using (XmlReader xmlreader = XmlReader.Create(xmlstream))
                {
                    var obj = xmlSerializer.Deserialize(xmlreader);
                    return obj as T;
                }

            }
        }
    }
}
