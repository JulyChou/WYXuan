using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace YoHao.Core.Auth
{
    public class AuthUser
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PassWord { get; set; }
        public string Sessionid { get; set; }

        public bool remember = false;
        /// <summary>
        /// cookie 名称
        /// </summary>
        private const string AuthorizationTicketName = "showfcmember";
        public int timeout = 60 * 24 * 7;


        /// <summary>
        /// 登录 写入cookie信息
        /// </summary>
        /// <param name="context"></param>
        public void SetTicket(HttpContext context)
        {
            var encode = GetTicket();
            var cookie = context.Request.Cookies[AuthorizationTicketName] ?? new HttpCookie(AuthorizationTicketName);
            cookie.Value = encode;
            //cookie.Domain = "ejiadg.cn";//主站登录 其他子站共享登录cookie
            if (remember)
            {
                cookie.Expires = DateTime.Now.AddMinutes(timeout);
                cookie.HttpOnly = true;
            }
            context.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="context"></param>
        public static void LogOut(HttpContext context)
        {
            var cookie = context.Request.Cookies[AuthorizationTicketName] ?? new HttpCookie(AuthorizationTicketName);
            cookie.Value = "";
            //cookie.Domain = "ejiadg.cn";//注销主站登录cookie
            cookie.Expires = DateTime.Now.AddMinutes(-1);
            context.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 得到不完整的 登录后会员信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static AuthUser GetMiniMember(HttpContext context)
        {
            var cookie = context.Request.Cookies[AuthorizationTicketName];
            var ticket = cookie == null ? context.Request.Params[AuthorizationTicketName] : context.Request.Cookies[AuthorizationTicketName].Value;
            if (ticket == null | ticket == "")
                return null;
            var arguments = Cryptography.Decrypt(ticket).Split('|');
            if (arguments.Length < 4)
            {
                return null;
            }
            var m = new AuthUser
            {
                Id = int.Parse(arguments[0]),
                Sessionid = arguments[1],
                Name = arguments[2],
            };
            return m;
        }
        /// <summary>
        /// 解码得到 登录后的用户信息
        /// </summary>
        /// <returns></returns>
        public string GetTicket()
        {
            return YoHao.Core.Cryptography.Encrypt(Id.ToString() + "|" + Sessionid + "|" + Name + "|" + Name);
        }
    }
}
