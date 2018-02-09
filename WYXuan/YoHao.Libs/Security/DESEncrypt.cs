using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace YoHao.Libs.Security
{
    public sealed class DesEncrypt
    {
        /// <summary>
        /// 加密URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string EncodeUrl(string url)
        {
            url = string.IsNullOrEmpty(url) ? "-" : url;
            return Encrypt(url, "dictha12", "haisdong").Replace("+", "%2B").Replace("/", "{xg}");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string Encrypt(string encryptString)
        {
            return Encrypt(encryptString, "zgzfclee$");
        }

        /// <summary>
        /// 加密文本
        /// </summary>
        /// <param name="encryptoContext"></param>
        /// <param name="cryptoKey"></param>
        /// <returns></returns>
        public static string Encrypt(string encryptoContext, string cryptoKey)
        {
            var _iv = "67^%*(&(*Ghx7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk"; //iv 向量
            //取 8 位 key
            cryptoKey = cryptoKey.PadLeft(8, '0').Substring(0, 8);
            //设置加密的 key，其值来自参数
            byte[] key = Encoding.UTF8.GetBytes(cryptoKey);
            //设置加密的 iv 向量，这里使用硬编码演示
            byte[] iv = Encoding.UTF8.GetBytes(_iv);
            //将需要加密的正文放进 byte 数组
            byte[] context = Encoding.UTF8.GetBytes(encryptoContext);

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                    {
                        cs.Write(context, 0, context.Length);
                        //将缓冲区数据写入，然后清空缓冲区
                        cs.FlushFinalBlock();
                    }

                    //从内存流返回结果，并编码为 base64string
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }



        /// <summary>
        /// 对字符串进行DES加密  
        /// </summary>
        /// <param name="sourcestring">待加密的字符串</param>
        /// <param name="key">必须长度等于8的字符串</param>
        /// <param name="iv">必须长度大于等于8的字符串</param>
        /// <returns>加密后的BASE64编码的字符串</returns>   
        public static string Encrypt(string sourcestring, string key, string iv)
        {
            byte[] byteKey = Encoding.UTF8.GetBytes(key);
            byte[] byteIv = Encoding.UTF8.GetBytes(iv);
            var des = new DESCryptoServiceProvider();
            using (var ms = new MemoryStream())
            {
                try
                {
                    des.Mode = CipherMode.CBC;
                    des.Padding = PaddingMode.Zeros;
                    byte[] data = Encoding.UTF8.GetBytes(sourcestring);
                    using (var cs = new CryptoStream(ms, des.CreateEncryptor(byteKey, byteIv), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
                catch
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedString"></param>
        /// <returns></returns>
        public static string Decrypt(string encryptedString)
        {
            return Decrypt(encryptedString, "zgzfclee$");
        }

        /// <summary>
        /// 解密文本
        /// </summary>
        /// <param name="decryptoContext"></param>
        /// <returns></returns>
        public static string Decrypt(string decryptoContext, string cryptoKey)
        {
            var _iv = "67^%*(&(*Ghx7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk"; //iv 向量
            //取 8 位 key
            cryptoKey = cryptoKey.PadLeft(8, '0').Substring(0, 8);
            //设置解密的 key，其值来自参数
            byte[] key = Encoding.UTF8.GetBytes(cryptoKey);
            //设置解密的 iv 向量，这里使用硬编码演示
            byte[] iv = Encoding.UTF8.GetBytes(_iv);
            //将解密正文返回到 byte 数组，加密时编码为 base64string ，这里要使用 FromBase64String 直接取回 byte 数组
            byte[] context = Convert.FromBase64String(decryptoContext);

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, iv), CryptoStreamMode.Write))
                    {
                        cs.Write(context, 0, context.Length);
                        //将当前缓冲区写入绑定的内存流，然后清空缓冲区
                        cs.FlushFinalBlock();
                    }

                    //从内存流返回值，并编码到 UTF8 输出原文
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }


        /// <summary>  
        /// 对DES加密后的字符串进行解密     
        /// </summary>     
        /// <param name="encryptedString">待解密的字符串</param>     
        /// <param name="key">同加密时所采用的key</param>
        /// <param name="iv">同加密时所采用的iv</param>
        /// <returns>解密后的字符串</returns>     
        public static string Decrypt(string encryptedString, string key, string iv)
        {
            encryptedString = encryptedString.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
            var byteKey = Encoding.UTF8.GetBytes(key);
            var byteIv = Encoding.UTF8.GetBytes(iv);
            var des = new DESCryptoServiceProvider();
            using (var ms = new MemoryStream())
            {
                try
                {
                    des.Mode = CipherMode.CBC;
                    des.Padding = PaddingMode.Zeros;
                    byte[] data = Convert.FromBase64String(encryptedString);
                    using (var cs = new CryptoStream(ms, des.CreateDecryptor(byteKey, byteIv), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
                catch
                {
                    throw;
                }
            }
        }
        /// <summary>  
        /// 对文件内容进行DES加密     
        /// </summary>     
        /// <param name="sourceFile">待加密的文件绝对路径</param>     
        /// <param name="destFile">加密后的文件保存的绝对路径</param>   
        /// <param name="key">必须长度等于8的字符串</param>
        /// <param name="iv">必须长度大于等于8的字符串</param>
        public static void EncryptFile(string sourceFile, string destFile, string key, string iv)
        {
            if (!File.Exists(sourceFile))
                throw new FileNotFoundException("指定的文件路径不存在！", sourceFile);
            byte[] byteKey = Encoding.Default.GetBytes(key);
            byte[] byteIV = Encoding.Default.GetBytes(iv);
            var des = new DESCryptoServiceProvider();
            byte[] byteFile = File.ReadAllBytes(sourceFile);
            using (var fs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    using (var cs = new CryptoStream(fs, des.CreateEncryptor(byteKey, byteIV), CryptoStreamMode.Write))
                    {
                        cs.Write(byteFile, 0, byteFile.Length);
                        cs.FlushFinalBlock();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    fs.Close();
                }
            }
        }
        /// <summary>     
        /// Encrypts the file.     
        /// 对文件内容进行DES加密，加密后覆盖掉原来的文件     
        /// </summary>     
        /// <param name="sourceFile">The source file.待加密的文件的绝对路径</param>     
        /// <param name="key">必须长度等于8的字符串</param>
        /// <param name="iv">必须长度大于等于8的字符串</param>
        public static void EncryptFile(string sourceFile, string key, string iv)
        {
            EncryptFile(sourceFile, sourceFile, key, iv);
        }
        /// <summary> 
        /// 对文件内容进行DES解密     
        /// </summary>     
        /// <param name="sourceFile">待解密的文件绝对路径</param>     
        /// <param name="destFile">解密后的文件保存的绝对路径</param>
        /// <param name="key">同加密时所采用的key</param>
        /// <param name="iv">同加密时所采用的iv</param>
        public static void DecryptFile(string sourceFile, string destFile, string key, string iv)
        {
            if (!File.Exists(sourceFile)) throw new FileNotFoundException("指定的文件路径不存在！", sourceFile);
            byte[] byteKey = Encoding.Default.GetBytes(key);
            byte[] byteIV = Encoding.Default.GetBytes(iv);
            var des = new DESCryptoServiceProvider();
            byte[] byteFile = File.ReadAllBytes(sourceFile);
            using (var fs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    using (var cs = new CryptoStream(fs, des.CreateDecryptor(byteKey, byteIV), CryptoStreamMode.Write))
                    {
                        cs.Write(byteFile, 0, byteFile.Length);
                        cs.FlushFinalBlock();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    fs.Close();
                }
            }
        }
        /// <summary> 
        /// 对文件内容进行DES解密，加密后覆盖掉原来的文件     
        /// </summary>     
        /// <param name="sourceFile">待解密的文件的绝对路径</param>     
        /// <param name="key">同加密时所采用的key</param>
        /// <param name="iv">同加密时所采用的iv</param>
        public static void DecryptFile(string sourceFile, string key, string iv)
        {
            DecryptFile(sourceFile, sourceFile, key, iv);
        }
        /// <summary>
        /// MD5校验码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMd5Sum(string str)
        {
            Encoder enc = System.Text.Encoding.Unicode.GetEncoder();
            var unicodeText = new byte[str.Length * 2];
            enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(unicodeText);
            var sb = new StringBuilder();
            foreach (var t in result)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
