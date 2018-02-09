using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace YoHao.Lib.Security
{
    public class DataSecurity
    {
        public static bool IsUsername(string str)
        {
            return new Regex("[\u0391-\uFFE5a-zA-Z0-9]{2,20}").IsMatch(str);
        }

        public static string ConvertToJavaScript(string str)
        {
            str = str.Replace(@"\", @"\\");
            str = str.Replace("\n", @"\n");
            str = str.Replace("\r", @"\r");
            str = str.Replace("\"", "\\\"");
            return str;
        }

        /// <summary>
        /// 检查是否含有非法字符
        /// </summary>
        /// <param name="str">要检查的字符串</param>
        /// <returns></returns>
        public static bool ChkBadChar(string str)
        {
            var result = false;
            if (string.IsNullOrEmpty(str))
                return false;
            const string strBadChar = "@@,+,',--,%,^,&,?,(,),<,>,[,],{,},/,\\,;,:,\",\"\"";
            var arrBadChar = StringHelper.SplitString(strBadChar, ",");
            var tempChar = str;
            foreach (var t in arrBadChar.Where(t => tempChar.IndexOf(t, System.StringComparison.Ordinal) >= 0))
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 过滤非法字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceBadChar(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            const string strBadChar = "@@,+,',--,%,^,&,?,(,),<,>,[,],{,},/,\\,;,:,\",\"\"";
            var arrBadChar = StringHelper.SplitString(strBadChar, ",");
            return arrBadChar.Where(t => t.Length > 0).Aggregate(str, (current, t) => current.Replace(t, ""));
        }

        /// <summary>
        /// 替换sql语句中的有问题符号
        /// </summary>
        public static string ReplaceBadSql(string str)
        {
            var str2 = "";
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            var str1 = str;
            var strArray = new string[] { "or", "ＯＲ", "and", "ＡＮＤ", "'", "''", ";", "；", "(", "（", ")", "）", "<", "&lt;", ">", "&gt;", "--", "——", "@@", "@" };
            var builder = new StringBuilder(str1);
            for (int i = 0; i < strArray.Length; i = i + 2)
            {
                str2 = builder.Replace(strArray[i], strArray[i + 1]).ToString();
            }
            return str2;
        }

        public static string ReplaceSql(object o)
        {
            var s = o.ToString().ToLower();
            var badword = new string[] { "'|''", ";--|；--", ";<|＜", ";>|＞", "wscript.|wscript。", "update|ｕpdate", "insert|ｉnsert", "select|ｓelect", "union|ｕnion", ".dbo.|。dbo。", "declare|ｄeclare", "exec|ｅxec", "drop|ｄrop", "create|ｃreate", "backup|ｂackup" };
            return badword.Aggregate(s, (current, word) => current.Replace(word.Split('|')[0], word.Split('|')[1]));
        }

        public static string GetArrayValue(int index, string[] field)
        {
            if ((field != null) && ((index >= 0) && (index < field.Length)))
            {
                return field[index];
            }
            return string.Empty;
        }

        public static string GetArrayValue(int index, Collection<string> field)
        {
            if ((index >= 0) && (index < field.Count))
            {
                return field[index];
            }
            return string.Empty;
        }

        public static string HtmlDecode(object o)
        {
            return o == null ? null : HtmlDecode(o.ToString());
        }


        /// <summary>
        /// 返回 HTML 字符串的解码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string HtmlDecode(string str)
        {
            return HttpUtility.HtmlDecode(str);
        }


        public static string HtmlEncode(object o)
        {
            return o == null ? null : HtmlEncode(o.ToString());
        }


        /// <summary>
        /// 返回  字符串的HTML编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string HtmlEncode(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Replace("<", "&lt;");
                str = str.Replace(">", "&gt;");
                str = str.Replace(" ", "&nbsp;");
                str = str.Replace("'", "&#39;");
                str = str.Replace("\"", "&quot;");
                str = str.Replace("\r\n", "<br>");
                str = str.Replace("\n", "<br>");
            }
            return str;
        }


        public static string MakeFileRndName()
        {
            return (DateTime.Now.ToString("yyyyMMddHHmmss") + MakeRandomString("0123456789", 4));
        }

        public static string MakeFolderName()
        {
            return DateTime.Now.ToString("yyyyMM");
        }

        public static string MakeRandomString(int pwdlen)
        {
            return MakeRandomString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_*", pwdlen);
        }

        public static string MakeRandomString(string pwdchars, int pwdlen)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < pwdlen; i++)
            {
                var num = random.Next(pwdchars.Length);
                builder.Append(pwdchars[num]);
            }
            return builder.ToString();
        }

        public static string RandomNum()
        {
            return RandomNum(4);
        }

        public static string RandomNum(int intlong)
        {
            var random = new Random();
            var builder = new StringBuilder("");
            for (int i = 0; i < intlong; i++)
            {
                builder.Append(random.Next(10));
            }
            return builder.ToString();
        }

        public static string RestrictedUrl(Uri url)
        {
            Uri uri;
            if (url == null)
            {
                return null;
            }
            Uri.TryCreate(url.AbsolutePath, UriKind.Absolute, out uri);
            return (RestrictedUrl(uri) + url.Query);
        }

        public static string RngCspNum(int strLength)
        {
            if (strLength > 0)
            {
                strLength--;
            }
            else
            {
                strLength = 5;
            }
            var data = new byte[strLength];
            new RNGCryptoServiceProvider().GetBytes(data);
            return BitConverter.ToInt32(data, 0).ToString();
        }

        public static string Strings(string ichar, int i)
        {
            var builder = new StringBuilder("");
            for (int j = 0; j < i; j++)
            {
                builder.Append(ichar);
            }
            return builder.ToString();
        }

        public static string UnrestrictedUrl(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }
            if (VirtualPathUtility.IsAppRelative(path))
            {
                path = VirtualPathUtility.ToAbsolute(path);
            }
            const int num = 80;
            var host = HttpContext.Current.Request.Url.Host;
            var str2 = "";
            var baseUri = new Uri(string.Format("http://{0}{1}", host, str2));
            return new Uri(baseUri, path).ToString();
        }


        /// <summary>
        /// 检测是否有危险的可能用于链接的字符串
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            return !Regex.IsMatch(str, @"/^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|$guestexp/is");
        }
        public static string UrlEncode(object urlObj)
        {
            if (urlObj == null)
            {
                return null;
            }
            return UrlEncode(urlObj.ToString());
        }

        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 改正sql语句中的转义字符
        /// </summary>
        public static string MashSql(string str)
        {
            string str2;
            if (str == null)
            {
                str2 = "";
            }
            else
            {
                str = str.Replace("\'", "'");
                str2 = str;
            }
            return str2;
        }


        public static string UrlEncode(string urlStr)
        {
            if (string.IsNullOrEmpty(urlStr))
            {
                return null;
            }
            return Regex.Replace(urlStr, @"[^a-zA-Z0-9,-_\.]+", new MatchEvaluator(DataSecurity.UrlEncodeMatch));
        }

        private static string UrlEncodeMatch(Match match)
        {
            var str = match.ToString();
            if (str.Length < 1)
            {
                return str;
            }
            var builder = new StringBuilder();
            foreach (char ch in str)
            {
                if (ch > '\x007f')
                {
                    builder.AppendFormat("%u{0:X4}", (int)ch);
                }
                else
                {
                    builder.AppendFormat("%{0:X2}", (int)ch);
                }
            }
            return builder.ToString();
        }

        public static string XmlEncode(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Replace("&", "&amp;");
                str = str.Replace("<", "&lt;");
                str = str.Replace(">", "&gt;");
                str = str.Replace("'", "&apos;");
                str = str.Replace("\"", "&quot;");
            }
            return str;
        }
    }

}
