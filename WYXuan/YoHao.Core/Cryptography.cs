using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace YoHao.Core
{
    public class Cryptography
    {
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string Encrypt(string encryptString, string encryptKey, bool compress)
        {
            try
            {
                string rnd = encryptKey.Substring(0, 8);
                byte[] rgbKey = Encoding.UTF8.GetBytes(rnd);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(compress ? Compress.EnCompress(encryptString) : encryptString);
                var dCSP = new DESCryptoServiceProvider();
                var mStream = new MemoryStream();
                var cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                byte[] msbty;
                msbty = mStream.ToArray();
                var ret = new StringBuilder();
                foreach (var b in msbty)
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                ret.ToString();
                return rnd + (compress ? "1" : "0") + ret.ToString();
            }

            catch
            {
                return encryptString;
            }
        }

        public static string Encrypt(string encryptString, bool compress)
        {
            return Encrypt(encryptString, Random(8), compress);
        }

        public static string Encrypt(string encryptString)
        {
            return Encrypt(encryptString, false);
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string Decrypt(string decryptString, string decryptKey, bool compress)
        {
            if (compress)
            {
                decryptString = Compress.DeCompress(decryptString);
            }
            var inputByteArray = new byte[decryptString.Length / 2];
            for (int x = 0; x < decryptString.Length / 2; x++)
            {
                int i = (Convert.ToInt32(decryptString.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            var btys = inputByteArray;
            var rgbKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
            var rgbIV = Keys;
            var DCSP = new DESCryptoServiceProvider();
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(btys, 0, btys.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        public static string Decrypt(string decryptString)
        {
            var key = decryptString.Substring(0, 8);
            var compress = decryptString.Substring(8, 1);
            decryptString = decryptString.Substring(9);
            return Decrypt(decryptString, key, compress == "1");
        }

        private static string Random(int codeCount)
        {
            string allChar = "1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,J,K,M,N,P,Q,R,S,T,U,W,X,Y,Z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;
            var rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(31);
                if (temp == t)
                {
                    return Random(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }
    }
}
