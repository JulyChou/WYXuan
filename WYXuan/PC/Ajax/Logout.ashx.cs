using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PC.Ajax
{
    /// <summary>
    /// Logout 的摘要说明
    /// </summary>
    public class Logout : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var backUrl = "/auth/login/";
            YoHao.Core.Auth.AuthUser.LogOut(HttpContext.Current);
            HttpContext.Current.Response.Redirect(backUrl);
            HttpContext.Current.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}