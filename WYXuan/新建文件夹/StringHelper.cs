using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Web;
using System.Xml;
using YoHao.Lib.Security;
using Microsoft.Security.Application;


namespace YoHao.Lib
{
    public static class StringHelper
    {
        /// <summary>
        /// 将传入的字符串中间部分字符替换成特殊字符  适合手机号码等截取
        /// </summary>
        /// <param name="value">需要替换的字符串</param>
        /// <param name="startLen">前保留长度</param>
        /// <param name="endLen">尾保留长度</param>
        /// <param name="specialChar"></param>
        /// <returns>被特殊字符替换的字符串</returns>
        public static string ReplaceWithSpecialChar(string value, int startLen = 4, int endLen = 4, char specialChar = '*')
        {
            try
            {
                var lenth = value.Length - startLen - endLen;
                var replaceStr = value.Substring(startLen, lenth);
                var specialStr = string.Empty;
                for (var i = 0; i < replaceStr.Length; i++)
                {
                    specialStr += specialChar;
                }
                value = value.Replace(replaceStr, specialStr);
            }
            catch (Exception)
            {
            }
            return value;
        }


        // Methods
        public static void AppendString(StringBuilder sb, string append)
        {
            AppendString(sb, append, ",");
        }

        public static void AppendString(StringBuilder sb, string append, string split)
        {
            if (sb.Length == 0)
            {
                sb.Append(append);
            }
            else
            {
                sb.Append(split);
                sb.Append(append);
            }
        }

        public static string Base64StringDecode(string input)
        {
            var bytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string Base64StringEncode(string input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public static bool CheckNodePurview(string arrstr1, string arrstr2)
        {
            if (!string.IsNullOrEmpty(arrstr1) && !string.IsNullOrEmpty(arrstr2))
            {
                var strArray = arrstr1.Split(new char[] { Convert.ToChar(",") });
                var strArray2 = arrstr2.Split(new char[] { Convert.ToChar(",") });
                return strArray.Any(str => strArray2.Any(str2 => !string.IsNullOrEmpty(str2.Trim()) && (str2.Trim() == str.Trim())));
            }
            return false;
        }

        public static string CollectionFilter(string conStr, string tagName, int fType)
        {
            string input = conStr;
            switch (fType)
            {
                case 1:
                    return Regex.Replace(input, "<" + tagName + "([^>])*>", "", RegexOptions.IgnoreCase);

                case 2:
                    return Regex.Replace(input, "<" + tagName + "([^>])*>.*?</" + tagName + "([^>])*>", "",
                        RegexOptions.IgnoreCase);

                case 3:
                    return Regex.Replace(Regex.Replace(input, "<" + tagName + "([^>])*>", "", RegexOptions.IgnoreCase),
                        "</" + tagName + "([^>])*>", "", RegexOptions.IgnoreCase);
            }
            return input;
        }

        public static string DecodeIp(long ip)
        {
            var strArray = new string[]
            {
                ((ip >> 0x18) & 0xffL).ToString(), ".", ((ip >> 0x10) & 0xffL).ToString(), ".",
                ((ip >> 8) & 0xffL).ToString(), ".", (ip & 0xffL).ToString()
            };
            return string.Concat(strArray);
        }

        public static string DecodeLockIp(string lockIp)
        {
            var builder = new StringBuilder(0x100);
            if (!string.IsNullOrEmpty(lockIp))
            {
                try
                {
                    var strArray = lockIp.Split(new string[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        string[] strArray2 = strArray[i].Split(new string[] { "----" },
                            StringSplitOptions.RemoveEmptyEntries);
                        builder.Append(DecodeIp(Convert.ToInt64(strArray2[0])) + "----" +
                                       DecodeIp(Convert.ToInt64(strArray2[1])) + "\n");
                    }
                    return builder.ToString().TrimEnd(new char[] { '\n' });
                }
                catch (IndexOutOfRangeException)
                {
                    return builder.ToString();
                }
            }
            return builder.ToString();
        }

        public static double EncodeIp(string sip)
        {
            if (string.IsNullOrEmpty(sip))
            {
                return 0.0;
            }
            var strArray = sip.Split(new char[] { '.' });
            var num = 0L;
            foreach (var str in strArray)
            {
                byte num2;
                if (byte.TryParse(str, out num2))
                {
                    num = (num << 8) | num2;
                }
                else
                {
                    return 0.0;
                }
            }
            return (double)num;
        }



        public static string FilterScript(string conStr, string filterItem)
        {
            var str = conStr.Replace("\r", "{$Chr13}").Replace("\n", "{$Chr10}");
            foreach (var str2 in filterItem.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                switch (str2)
                {
                    case "Iframe":
                        str = CollectionFilter(str, str2, 2);
                        break;

                    case "Object":
                        str = CollectionFilter(str, str2, 2);
                        break;

                    case "Script":
                        str = CollectionFilter(str, str2, 2);
                        break;

                    case "Style":
                        str = CollectionFilter(str, str2, 2);
                        break;

                    case "Div":
                        str = CollectionFilter(str, str2, 3);
                        break;

                    case "Span":
                        str = CollectionFilter(str, str2, 3);
                        break;

                    case "Table":
                        str =
                            CollectionFilter(
                                CollectionFilter(
                                    CollectionFilter(CollectionFilter(CollectionFilter(str, str2, 3), "Tbody", 3), "Tr",
                                        3), "Td", 3), "Th", 3);
                        break;

                    case "Img":
                        str = CollectionFilter(str, str2, 1);
                        break;

                    case "Font":
                        str = CollectionFilter(str, str2, 3);
                        break;

                    case "A":
                        str = CollectionFilter(str, str2, 3);
                        break;

                    case "Html":
                        str = StripTags(str);
                        break;
                }
            }
            return str.Replace("{$Chr13}", "\r").Replace("{$Chr10}", "\n");
        }

        public static bool FoundCharInArr(string checkStr, string findStr)
        {
            return FoundCharInArr(checkStr, findStr, ",");
        }

        public static bool FoundCharInArr(string checkStr, string findStr, string split)
        {
            bool flag = false;
            if (string.IsNullOrEmpty(split))
            {
                split = ",";
            }
            if (string.IsNullOrEmpty(checkStr))
            {
                return false;
            }
            if (checkStr.IndexOf(split, System.StringComparison.Ordinal) != -1)
            {
                if (findStr.IndexOf(split, System.StringComparison.Ordinal) != -1)
                {
                    var strArray = checkStr.Split(new char[] { Convert.ToChar(split) });
                    var strArray2 = findStr.Split(new char[] { Convert.ToChar(split) });
                    foreach (string str in strArray)
                    {
                        if (strArray2.Any(str2 => System.String.CompareOrdinal(str, str2) == 0))
                        {
                            flag = true;
                        }
                        if (flag)
                        {
                            return flag;
                        }
                    }
                    return flag;
                }
                if (checkStr.Split(new char[] { Convert.ToChar(split) }).Any(str3 => System.String.CompareOrdinal(str3, findStr) == 0))
                {
                    return true;
                }
                return flag;
            }
            if (System.String.CompareOrdinal(checkStr, findStr) == 0)
            {
                flag = true;
            }
            return flag;
        }



        /// <summary>
        /// 检查一个数组中所有的元素是否有包含于指定字符串的元素
        /// </summary>
        /// <param name="arr">存储数据数据的字串</param>
        /// <param name="toFind">要查找的字符串</param>
        /// <param name="separator">数组的分隔符</param>
        /// <returns></returns>
        public static bool FoundStringInArr(string arr, string toFind, char separator)
        {
            if (arr.IndexOf(separator) >= 0)
            {
                var arrTemp = arr.Split('|');
                for (var i = 0; i < arrTemp.Length; i++)
                {
                    if ((toFind.ToLower().IndexOf(arrTemp[i].ToLower(), System.StringComparison.Ordinal) >= 0) && (arrTemp[i].ToLower() != ""))
                        return true;
                }
            }
            else
            {
                if ((toFind.ToLower().IndexOf(arr.ToLower(), System.StringComparison.Ordinal)) >= 0 && (arr.ToLower() != ""))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            int i = strContent.IndexOf(strSplit, System.StringComparison.Ordinal);
            if (strContent.IndexOf(strSplit, System.StringComparison.Ordinal) < 0)
            {
                string[] tmp = { strContent };
                return tmp;
            }
            return Regex.Split(strContent, @strSplit.Replace(".", @"\."));
        }

        private static string GetGbkX(string testTxt)
        {
            if (testTxt.CompareTo("吖") >= 0)
            {
                if (testTxt.CompareTo("八") < 0)
                {
                    return "A";
                }
                if (testTxt.CompareTo("嚓") < 0)
                {
                    return "B";
                }
                if (testTxt.CompareTo("咑") < 0)
                {
                    return "C";
                }
                if (testTxt.CompareTo("妸") < 0)
                {
                    return "D";
                }
                if (testTxt.CompareTo("发") < 0)
                {
                    return "E";
                }
                if (testTxt.CompareTo("旮") < 0)
                {
                    return "F";
                }
                if (testTxt.CompareTo("铪") < 0)
                {
                    return "G";
                }
                if (testTxt.CompareTo("讥") < 0)
                {
                    return "H";
                }
                if (testTxt.CompareTo("咔") < 0)
                {
                    return "J";
                }
                if (testTxt.CompareTo("垃") < 0)
                {
                    return "K";
                }
                if (testTxt.CompareTo("嘸") < 0)
                {
                    return "L";
                }
                if (testTxt.CompareTo("拏") < 0)
                {
                    return "M";
                }
                if (testTxt.CompareTo("噢") < 0)
                {
                    return "N";
                }
                if (testTxt.CompareTo("妑") < 0)
                {
                    return "O";
                }
                if (testTxt.CompareTo("七") < 0)
                {
                    return "P";
                }
                if (testTxt.CompareTo("亽") < 0)
                {
                    return "Q";
                }
                if (testTxt.CompareTo("仨") < 0)
                {
                    return "R";
                }
                if (testTxt.CompareTo("他") < 0)
                {
                    return "S";
                }
                if (testTxt.CompareTo("哇") < 0)
                {
                    return "T";
                }
                if (testTxt.CompareTo("夕") < 0)
                {
                    return "W";
                }
                if (testTxt.CompareTo("丫") < 0)
                {
                    return "X";
                }
                if (testTxt.CompareTo("帀") < 0)
                {
                    return "Y";
                }
                if (testTxt.CompareTo("咗") < 0)
                {
                    return "Z";
                }
            }
            return testTxt;
        }

        public static string GetInitial(string str)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                builder.Append(GetOneIndex(str.Substring(i, 1)));
            }
            return builder.ToString();
        }

        private static string GetOneIndex(string testTxt)
        {
            if ((Convert.ToChar(testTxt) >= '\0') && (Convert.ToChar(testTxt) < 'Ā'))
            {
                return testTxt;
            }
            return GetGbkX(testTxt);
        }

        public static bool IsIncludeChinese(string inputData)
        {
            var regex = new Regex("[一-龥]");
            return regex.Match(inputData).Success;
        }


        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            byte[] b = Encoding.Default.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }

        public static string Md5Gb2312(string input)
        {
            using (var provider = new MD5CryptoServiceProvider())
            {
                return
                    BitConverter.ToString(provider.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(input)))
                        .Replace("-", "")
                        .ToLower();
            }
        }

        public static string RemoveXss(string input)
        {
            string str;
            string str2;
            do
            {
                str = input;
                input = Regex.Replace(input, @"(&#*\w+)[\x00-\x20]+;", "$1;");
                input = Regex.Replace(input, "(&#x*[0-9A-F]+);*", "$1;", RegexOptions.IgnoreCase);
                input = Regex.Replace(input, "&(amp|lt|gt|nbsp|quot);", "&amp;$1;");
                input = HttpUtility.HtmlDecode(input);
            } while (str != input);
            do
            {
                str = input;
                input = Regex.Replace(input, @"(<[^>]+style[\x00-\x20]*=[\x00-\x20]*[^>]*?)\\([^>]*>)", "$1/$2",
                    RegexOptions.IgnoreCase);
            } while (str != input);
            input = Regex.Replace(input, @"[\x00-\x08\x0b-\x0c\x0e-\x19]", "");
            input = Regex.Replace(input, "(<[^>]+[\\x00-\\x20\"'/])(on|xmlns)[^>]*>", "$1>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input,
                "([a-z]*)[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*)[\\x00-\\x20]*j[\\x00-\\x20]*a[\\x00-\\x20]*v[\\x00-\\x20]*a[\\x00-\\x20]*s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*:",
                "$1=$2nojavascript...", RegexOptions.IgnoreCase);
            input = Regex.Replace(input,
                "([a-z]*)[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*)[\\x00-\\x20]*v[\\x00-\\x20]*b[\\x00-\\x20]*s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*:",
                "$1=$2novbscript...", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"(<[^>]+style[\x00-\x20]*=[\x00-\x20]*[^>]*?)/\*[^>]*\*/([^>]*>)", "$1$2",
                RegexOptions.IgnoreCase);
            input = Regex.Replace(input,
                "(<[^>]+)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*expression[\\x00-\\x20]*\\([^>]*>", "$1>",
                RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "(<[^>]+)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*behaviour[^>]*>", "$1>",
                RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "(<[^>]+)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*behavior[^>]*>", "$1>",
                RegexOptions.IgnoreCase);
            input = Regex.Replace(input,
                "(<[^>]+)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*:*[^>]*>",
                "$1>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"</*\w+:\w[^>]*>", "noxss");
            do
            {
                str2 = input;
                input = Regex.Replace(input,
                    "</*(applet|meta|xml|blink|link|style|script|embed|object|iframe|frame|frameset|ilayer|layer|bgsound|title|base)[^>]*>?",
                    "no$1", RegexOptions.IgnoreCase);
            } while (str2 != input);
            return input;
        }


        /// <summary>
        /// 自定义的替换字符串函数
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="searchString"></param>
        /// <param name="replaceString"></param>
        /// <param name="isCaseInsensetive"></param>
        /// <returns></returns>
        public static string ReplaceString(string sourceString, string searchString, string replaceString,
            bool isCaseInsensetive)
        {
            return Regex.Replace(sourceString, Regex.Escape(searchString), replaceString,
                isCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }


        public static string Sha1(string input)
        {
            using (var provider = new SHA1CryptoServiceProvider())
            {
                return
                    BitConverter.ToString(provider.ComputeHash(Encoding.UTF8.GetBytes(input)))
                        .Replace("-", "")
                        .ToLower();
            }
        }

        public static string StripTags(string input)
        {
            var regex = new Regex("<([^<]|\n)+?>");
            return regex.Replace(input, "");
        }

        public static string SubString(string demand, int length, string substitute)
        {
            if (Encoding.Default.GetBytes(demand).Length <= length)
            {
                return demand;
            }
            var encoding = new ASCIIEncoding();
            length -= Encoding.Default.GetBytes(substitute).Length;
            var num = 0;
            var builder = new StringBuilder();
            var bytes = encoding.GetBytes(demand);
            for (var i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0x3f)
                {
                    num += 2;
                }
                else
                {
                    num++;
                }
                if (num > length)
                {
                    break;
                }
                builder.Append(demand.Substring(i, 1));
            }
            builder.Append(substitute);
            return builder.ToString();
        }

        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns></returns>
        public static int GetStringLength(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }

        public static string Trim(string returnStr)
        {
            if (!string.IsNullOrEmpty(returnStr))
            {
                return returnStr.Trim();
            }
            return string.Empty;
        }

        public static bool ValidateMd5(string password, string md5Value)
        {
            if (System.String.CompareOrdinal(password, md5Value) != 0)
            {
                return (System.String.CompareOrdinal(password, md5Value.Substring(8, 0x10)) == 0);
            }
            return true;
        }


        #region DateTime 方法
        /// <summary>
        /// 返回标准日期格式string
        /// </summary>
        public static string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 返回指定日期格式
        /// </summary>
        public static string GetDate(string datetimestr, string replacestr)
        {
            if (datetimestr == null)
            {
                return replacestr;
            }
            if (datetimestr.Equals(""))
            {
                return replacestr;
            }
            try
            {
                datetimestr = Convert.ToDateTime(datetimestr).ToString("yyyy-MM-dd").Replace("1900-01-01", replacestr);
            }
            catch
            {
                return replacestr;
            }
            return datetimestr;
        }


        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回相对于当前时间的相对天数
        /// </summary>
        public static string GetDateTime(int relativeday)
        {
            return DateTime.Now.AddDays(relativeday).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTimeF()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }

        /// <summary>
        /// 返回标准时间 
        /// </summary>
        /// <param name="fDateTime"></param>
        /// <param name="formatStr"></param>
        /// <returns></returns>
        public static string GetStandardDateTime(string fDateTime, string formatStr)
        {
            var s = Convert.ToDateTime(fDateTime);
            return s.ToString(formatStr);
        }

        /// <summary>
        /// 返回标准时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="fDateTime"></param>
        /// <returns></returns>
        public static string GetStandardDateTime(string fDateTime)
        {
            return GetStandardDateTime(fDateTime, "yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回相差的秒数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static int StrDateDiffSeconds(string time, int sec)
        {
            var ts = DateTime.Now - DateTime.Parse(time).AddSeconds(sec);
            if (ts.TotalSeconds > int.MaxValue)
            {
                return int.MaxValue;
            }
            else if (ts.TotalSeconds < int.MinValue)
            {
                return int.MinValue;
            }
            return (int)ts.TotalSeconds;
        }

        /// <summary>
        /// 返回相差的分钟数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static int StrDateDiffMinutes(string time, int minutes)
        {
            if (string.IsNullOrEmpty(time))
                return 1;
            var ts = DateTime.Now - DateTime.Parse(time).AddMinutes(minutes);
            if (ts.TotalMinutes > int.MaxValue)
            {
                return int.MaxValue;
            }
            else if (ts.TotalMinutes < int.MinValue)
            {
                return int.MinValue;
            }
            return (int)ts.TotalMinutes;
        }

        /// <summary>
        /// 返回相差的小时数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static int StrDateDiffHours(string time, int hours)
        {
            if (string.IsNullOrEmpty(time))
                return 1;
            var ts = DateTime.Now - DateTime.Parse(time).AddHours(hours);
            if (ts.TotalHours > int.MaxValue)
            {
                return int.MaxValue;
            }
            else if (ts.TotalHours < int.MinValue)
            {
                return int.MinValue;
            }
            return (int)ts.TotalHours;
        }

        public static string GetDayOfWeek(DateTime dt)
        {
            var weekstr = dt.DayOfWeek.ToString();
            switch (weekstr)
            {
                case "Monday": weekstr = "星期一"; break;
                case "Tuesday": weekstr = "星期二"; break;
                case "Wednesday": weekstr = "星期三"; break;
                case "Thursday": weekstr = "星期四"; break;
                case "Friday": weekstr = "星期五"; break;
                case "Saturday": weekstr = "星期六"; break;
                case "Sunday": weekstr = "星期日"; break;
            }
            return weekstr;
        }

        #endregion


        /// <summary>
        /// 替换回车换行符为html换行符
        /// </summary>
        public static string StrFormat(string str)
        {
            string str2;
            if (str == null)
            {
                str2 = "";
            }
            else
            {
                str = str.Replace("\r\n", "<br />");
                str = str.Replace("\n", "<br />");
                str2 = str;
            }
            return str2;
        }

        public static string MaxLength(object obj, int length)
        {
            if (obj == null) return "";
            var str = obj.ToString().Trim();
            if (GetStringLength(str) <= length) return str;
            var sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                sb.Append(str.Substring(i, 1));
                if (GetStringLength((string)(sb.ToString() + "...")) >= length) return sb.ToString() + "...";
            }
            return str;
        }

        public static string SubLength(string str, int length)
        {
            str = str.Trim();
            if (GetStringLength(str) <= length) return str;
            var sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                sb.Append(str.Substring(i, 1));
                if (GetStringLength((string)(sb.ToString())) >= length) return sb.ToString();
            }
            return str;
        }

        private static string GetDate(object o)
        {
            string str = o.ToString();
            return str.Split(' ')[0];
        }

        public static string NullToEmpty(object o)
        {
            if (o == null) return "";
            else
            {
                return o.ToString();
            }
        }

        /// <summary>
        /// 从字符串中移除html标签
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string Removehtml(object o)
        {
            if (o == null)
            {
                return "";
            }
            if (string.IsNullOrEmpty(o.ToString()))
            {
                return "";
            }
            var reg = new System.Text.RegularExpressions.Regex("<.*?>",
                  System.Text.RegularExpressions.RegexOptions.IgnoreCase |
                  System.Text.RegularExpressions.RegexOptions.Singleline |
                  System.Text.RegularExpressions.RegexOptions.Multiline);
            return new System.Text.RegularExpressions.Regex("\\s+").Replace(
                reg.Replace(o.ToString(), "")
                    .Replace(System.Environment.NewLine, "")
                    .Replace((char)10, (char)0)
                    .Replace((char)13, (char)0)
                , " ").Replace("&nbsp;", "").Replace("\0", "");
        }

        /// <summary>
        /// 从字符串中移除 图片
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string RemoveImg(object o)
        {
            if (o == null)
            {
                return "";
            }
            if (string.IsNullOrEmpty(o.ToString()))
            {
                return "";
            }
            var input = o.ToString();
            string str;
            string str2;
            do
            {
                str = input;
                input = Regex.Replace(input, @"(&#*\w+)[\x00-\x20]+;", "$1;");
                input = Regex.Replace(input, "(&#x*[0-9A-F]+);*", "$1;", RegexOptions.IgnoreCase);
                input = Regex.Replace(input, "&(amp|lt|gt|nbsp|quot);", "&amp;$1;");
                input = HttpUtility.HtmlDecode(input);
            } while (str != input);
            do
            {
                str = input;
                input = Regex.Replace(input, @"(<[^>]+style[\x00-\x20]*=[\x00-\x20]*[^>]*?)\\([^>]*>)", "$1/$2",
                    RegexOptions.IgnoreCase);
            } while (str != input);
            input = Regex.Replace(input, @"[\x00-\x08\x0b-\x0c\x0e-\x19]", "");
            input = Regex.Replace(input, "(<[^>]+[\\x00-\\x20\"'/])(on|xmlns)[^>]*>", "$1>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input,
                "([a-z]*)[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*)[\\x00-\\x20]*j[\\x00-\\x20]*a[\\x00-\\x20]*v[\\x00-\\x20]*a[\\x00-\\x20]*s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*:",
                "$1=$2nojavascript...", RegexOptions.IgnoreCase);
            input = Regex.Replace(input,
                "([a-z]*)[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*)[\\x00-\\x20]*v[\\x00-\\x20]*b[\\x00-\\x20]*s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*:",
                "$1=$2novbscript...", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"(<[^>]+style[\x00-\x20]*=[\x00-\x20]*[^>]*?)/\*[^>]*\*/([^>]*>)", "$1$2",
                RegexOptions.IgnoreCase);
            input = Regex.Replace(input,
                "(<[^>]+)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*expression[\\x00-\\x20]*\\([^>]*>", "$1>",
                RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "(<[^>]+)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*behaviour[^>]*>", "$1>",
                RegexOptions.IgnoreCase);
            input = Regex.Replace(input, "(<[^>]+)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*behavior[^>]*>", "$1>",
                RegexOptions.IgnoreCase);
            input = Regex.Replace(input,
                "(<[^>]+)style[\\x00-\\x20]*=[\\x00-\\x20]*([`'\"]*).*s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*:*[^>]*>",
                "$1>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"</*\w+:\w[^>]*>", "noxss");
            do
            {
                str2 = input;
                input = Regex.Replace(input,
                    "</*(img|input|select|applet|meta|xml|blink|link|style|script|embed|object|iframe|frame|frameset|ilayer|layer|bgsound|title|base)[^>]*>?",
                    "", RegexOptions.IgnoreCase);
            } while (str2 != input);
            return input;
        }

        /// <summary>
        /// 从字符串当中取得所有图片
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static List<string> GetImgsFromString(string info)
        {
            var result = new List<string>();
            if (!string.IsNullOrEmpty(info))
            {
                var reg =
                    new System.Text.RegularExpressions.Regex("<img[^>]+src=\"?([^>]*?)\"[^>]+/>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.MatchCollection coll = reg.Matches(info);
                result.AddRange(from Match m in coll select m.Groups[1].Value);
            }
            return result;
        }

        /// <summary>
        /// 从字符串当中取得图片的 title，src
        /// </summary>
        /// <param name="info"></param>
        /// <returns>返回list集合 </returns>
        public static List<KeyValuePair<string, string>> GetTitleImgsFromString(string info)
        {
            var result = new List<KeyValuePair<string, string>>();
            if (!string.IsNullOrEmpty(info))
            {
                var reg = new Regex("<img[^>]+src=\"?([^>]*?)\"[^>]+/>", RegexOptions.IgnoreCase);
                var regtitle = new Regex("title=\"?([^>]*?)\"[^>]", RegexOptions.IgnoreCase);
                var coll = reg.Matches(info);
                foreach (Match m in coll)
                {
                    var key = m.Groups[1].Value;
                    var colltitle = regtitle.Matches(m.Groups[0].Value);
                    if (colltitle.Count > 0)
                    {
                        result.Add(new KeyValuePair<string, string>(key, colltitle[0].Groups[1].Value));
                    }
                    else
                    {
                        result.Add(new KeyValuePair<string, string>(key, ""));
                    }
                }
            }
            return result;
        }

        public static List<string> GetImgsFromString(string info, int length, string defaultpath)
        {
            var result = new List<string>();
            if (!string.IsNullOrEmpty(info))
            {
                var reg = new Regex("<img[^>]+src=\"?([^>]*?)\"[^>]+/>", RegexOptions.IgnoreCase);
                var coll = reg.Matches(info);
                result.AddRange(from Match m in coll select m.Groups[1].Value);
            }
            if (result.Count < 9)
            {
                int time = 9 - result.Count;
                for (int i = 0; i < time; i++)
                {
                    result.Add(defaultpath);
                }
            }
            if (result.Count > 9)
            {
                result.RemoveRange(9, result.Count - 9);
            }
            return result;
        }

        /// <summary>
        /// 从后台多图片上传的节点中取得图片路径
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static List<string> GetImgsFromXmlString(string info)
        {
            var result = new List<string>();
            if (!string.IsNullOrEmpty(info))
            {
                var doc = new System.Xml.XmlDocument();
                try
                {
                    doc.LoadXml(info);
                    System.Xml.XmlNodeList nodes = doc.SelectNodes("/files/file");
                    result.AddRange(from XmlNode node in nodes select node.Attributes["path"].Value);
                }
                catch
                {

                }
            }
            return result;
        }

        public static List<KeyValuePair<string, string>> GetImgDecsFromXmlString(string info)
        {
            var result = new List<KeyValuePair<string, string>>();
            if (!string.IsNullOrEmpty(info))
            {
                var doc = new System.Xml.XmlDocument();
                try
                {
                    doc.LoadXml(info);
                    var nodes = doc.SelectNodes("/files/file");
                    foreach (System.Xml.XmlNode node in nodes)
                    {
                        result.Add(new KeyValuePair<string, string>(node.Attributes["name"].Value,
                            node.Attributes["path"].Value));
                    }
                }
                catch
                {

                }

            }
            return result;
        }

        /// <summary>
        /// 字符串截取 --获得摘要数据
        /// </summary>
        /// <param name="o"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetSummary(object o, int length)
        {
            if (o == null)
            {
                return "";
            }
            else
            {
                return MaxLength(Removehtml(o.ToString()), length);
            }
        }
        public static string GetSafeHtml(this string input)
        {
            return Sanitizer.GetSafeHtmlFragment(input);
        }

        public static string GetSafeSql(this string input)
        {
            return DataSecurity.ReplaceBadChar(input);
        }

        /// <summary>
        /// 扩展判断字符串是否为空
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string input)
        {
            return string.IsNullOrEmpty(input);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kvp"></param>
        /// <returns></returns>
        public static String BuildQueryString(Dictionary<String, String> kvp)
        {
            return String.Join("&", (from item in kvp
                                     let value = String.Format("{0}", item.Value)
                                     select String.Format("{0}={1}", Uri.EscapeDataString(item.Key), Uri.EscapeDataString(value))).ToArray());
        }

        /// <summary>
        /// 参数构造排序 按照参数首字母
        /// </summary>
        /// <param name="kvp"></param>
        /// <returns></returns>
        public static String BuildQuerySortedString(SortedList<String, String> kvp)
        {
            return String.Join("&", kvp.Select(item => String.Format("{0}={1}", item.Key.Trim(), item.Value)).ToArray());
        }

    }

}
