using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace YoHao.Lib.Security
{
    public class DataValidate
    {
        public static bool IsAreaCode(string input)
        {
            return ((IsNumber(input) && (input.Length >= 3)) && (input.Length <= 5));
        }

        public static bool IsDecimal(string input)
        {
            return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^[0-9]+[.]?[0-9]+$");
        }

        public static bool IsDecimalSign(string input)
        {
            return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^[+-]?[0-9]+[.]?[0-9]+$");
        }


        public static bool SafeCodeIsTrue(string str)
        {
            if (System.Web.HttpContext.Current.Session["safeCode"] == null || str.ToLower() != System.Web.HttpContext.Current.Session["safeCode"].ToString().ToLower())
            {
                System.Web.HttpContext.Current.Session["safeCode"] = null;
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsImage(string filepath)
        {
            return !string.IsNullOrEmpty(filepath) && Regex.IsMatch(filepath, "^.+?\\.([jJ][pP][Gg]|[jJ][pP][eE][Gg]|[Gg][iI][fF]|[pP][nN][Gg]|[bB][mM][pP])$");
        }

        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsEmail(string strEmail)
        {
            return !string.IsNullOrEmpty(strEmail) && Regex.IsMatch(strEmail, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }


        public static bool IsIP(string strIp)
        {
            return (!string.IsNullOrEmpty(strIp) && Regex.IsMatch(strIp.Trim(), @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$"));
        }

        /// <summary>
        /// 判断给定的字符串(strNumber)是否是数值型
        /// </summary>
        /// <param name="strNumber">要确认的字符串</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsNumber(string strNumber)
        {
            return Regex.IsMatch(strNumber, "^[0-9]+$");
        }

        public static bool IsNumberSign(string input)
        {
            return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^[+-]?[0-9]+$");
        }

        /// <summary>
        /// 检测是否符合邮编格式
        /// </summary>
        /// <param name="postCode"></param>
        /// <returns></returns>
        public static bool IsPostCode(string postCode)
        {
            return Regex.IsMatch(postCode, @"^\d{6}$");
        }


        /// <summary>
        /// 检测是否符合身份证号码格式
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool IsIdentityNumber(string num)
        {
            return Regex.IsMatch(num, @"^\d{17}[\d|X]|\d{15}$");
        }
        /// <summary>
        /// 检测是否符合时间格式
        /// </summary>
        /// <returns></returns>
        public static bool IsTime(string timeval)
        {
            return Regex.IsMatch(timeval, @"20\d{2}\-[0-1]{1,2}\-[0-3]?[0-9]?(\s*((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?))?");
        }
        /// <summary>
        /// 检测是否符合url格式,前面必需含有http://
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsUrl(string url)
        {
            return !string.IsNullOrEmpty(url) && Regex.IsMatch(url, @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$");
        }

        /// <summary>
        /// 检测是否符合电话格式
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static bool IsPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^(\(\d{3}\)|\d{3}-)?\d{7,8}$|^((\(\d{3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$");
        }

        public static bool IsValidId(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            input = input.Replace("|", "").Replace(",", "").Replace("-", "").Replace(" ", "").Trim();
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            return IsNumber(input);
        }

        public static bool IsLenEnough(string input, int len)
        {
            return !string.IsNullOrEmpty(input) && input.Length >= len;
        }


        public static bool IsChinaName(string input)
        {
            return !string.IsNullOrEmpty(input) && Regex.IsMatch(input, "^[\\u0391-\\uFFE5]{2,4}$");
        }

        public static bool IsValidUserName(string userName)
        {
            return !string.IsNullOrEmpty(userName);
        }

        public static bool IsValidUserName(string userName, out string msg)
        {
            userName = userName.Trim();
            msg = "";
            if (string.IsNullOrEmpty(userName))
            {
                msg = "用户名不能为空";
                return false;
            }
            if (StringHelper.GetStringLength(userName) > 16)
            {
                msg = "用户名长度不能大于16";
                return false;
            }
            if (StringHelper.GetStringLength(userName) < 2)
            {
                msg = "用户名长度不能小于2";
                return false;
            }
            const string str = "\\/\"[]:|<>+=;,?*@.'";
            if (userName.Any(t => str.IndexOf(t) >= 0))
            {
                msg = "用户名含有非法字符";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断是否为纯数字
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool IsNumberic(string msg)
        {
            var rex =new Regex(@"^\d+$");
            if (rex.IsMatch(msg))
            {
                return true;
            }
            else
                return false;
        }


        public static bool IsValidPassword(string userName, out string msg)
        {
            userName = userName.Trim();
            msg = "";
            if (string.IsNullOrEmpty(userName))
            {
                msg = "密码不能为空";
                return false;
            }
            if (StringHelper.GetStringLength(userName) > 16)
            {
                msg = "密码长度不能大于16";
                return false;
            }
            if (StringHelper.GetStringLength(userName) < 3)
            {
                msg = "密码长度不能小于6";
                return false;
            }
            const string str = "\\/\"[]:|<>+=;,?*@.'";
            if (!userName.Any(t => str.IndexOf(t) >= 0)) return true;
            msg = "密码含有非法字符";
            return false;
        }

    }
}
