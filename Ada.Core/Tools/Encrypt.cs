using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.Tools
{
    public class Encrypt
    {
        //密钥
        private static readonly byte[] ArrDesKey = { 42, 16, 93, 156, 78, 4, 218, 32 };
        private static readonly byte[] ArrDesiv = { 55, 103, 246, 79, 36, 99, 167, 3 };

        /// <summary>
        /// 加密。
        /// </summary>
        /// <param name="mNeedEncodeString"></param>
        /// <returns></returns>
        public static string Encode(string mNeedEncodeString)
        {
            if (mNeedEncodeString == null)
            {
                throw new Exception("Error: \n源字符串为空！！");
            }
            DESCryptoServiceProvider objDes = new DESCryptoServiceProvider();
            MemoryStream objMemoryStream = new MemoryStream();
            CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDes.CreateEncryptor(ArrDesKey, ArrDesiv), CryptoStreamMode.Write);
            StreamWriter objStreamWriter = new StreamWriter(objCryptoStream);
            objStreamWriter.Write(mNeedEncodeString);
            objStreamWriter.Flush();
            objCryptoStream.FlushFinalBlock();
            objMemoryStream.Flush();
            return Convert.ToBase64String(objMemoryStream.GetBuffer(), 0, (int)objMemoryStream.Length);
        }

        /// <summary>
        /// 解密。
        /// </summary>
        /// <param name="mNeedEncodeString"></param>
        /// <returns></returns>
        public static string Decode(string mNeedEncodeString)
        {
            if (mNeedEncodeString == null)
            {
                throw new Exception("Error: \n源字符串为空！！");
            }
            DESCryptoServiceProvider objDes = new DESCryptoServiceProvider();
            byte[] arrInput = Convert.FromBase64String(mNeedEncodeString);
            MemoryStream objMemoryStream = new MemoryStream(arrInput);
            CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDes.CreateDecryptor(ArrDesKey, ArrDesiv), CryptoStreamMode.Read);
            StreamReader objStreamReader = new StreamReader(objCryptoStream);
            return objStreamReader.ReadToEnd();
        }

        /// <summary>
        /// md5
        /// </summary>
        /// <param name="encypStr"></param>
        /// <returns></returns>
        public static string Md5(string encypStr)
        {
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();
            var inputBye = Encoding.ASCII.GetBytes(encypStr);
            var outputBye = m5.ComputeHash(inputBye);
            var retStr = Convert.ToBase64String(outputBye);
            return retStr;
        }
    }
}
