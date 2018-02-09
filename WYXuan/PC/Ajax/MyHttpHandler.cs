using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Yohao.Common.MVC;
using YoHao.Libs.Security;

namespace PC.Ajax
{
    public class MyHttpHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public YoHao.Model.User.User User;
        public bool IsValidateMember = false;//是否验证用户的登录信息
        public SqlSugarClient DB { get; } = DBbase.GetInstance();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public virtual void ProcessRequest(HttpContext context)  //验证
        {
            context.Request.ContentEncoding = Encoding.UTF8;
            var resContent = new StringBuilder("{");
            if (RequestAuth(context, resContent))
            {
                ExecuteHandle(context, resContent);
            }
            resContent.Append("}");
            context.Response.ContentType = "application/json";
            context.Response.Write(resContent.ToString());
            context.Response.End();
        }

        /// <summary>
        ///  请求验证，如果返回true则会执行处理函数ExecuteHandle，可在继承类中重写以改变验证方式
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resContent"></param>
        /// <returns></returns>
        private bool RequestAuth(HttpContext context, StringBuilder resContent)
        {
            if (IsValidateMember)  //验证用户是否登录
            {
                var currentUser = YoHao.Core.Auth.AuthUser.GetMiniMember(System.Web.HttpContext.Current);
                if (currentUser == null)
                {
                    resContent.Append("\"ret\":-1, \"msg\":\"您还未登录……请先登录！\"");
                    return false;
                }
                else
                {
                    User = DB.Queryable<YoHao.Model.User.User>().InSingle(currentUser.Id);//转换 得到完整的member 信息
                    if (User == null)
                    {
                        resContent.Append("\"ret\":-1, \"msg\":\"您还未登录……请先登录！\"");
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 请求验证，如果返回true则会执行处理函数ExecuteHandle，可在继承类中重写以改变验证方式
        /// </summary>
        public virtual void ExecuteHandle(HttpContext context, StringBuilder resContent)
        { }
        #region 公用方法
        /// <summary>
        /// 防止Sql注入
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public string Check(string parameter)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                return "";
            }
            return DataSecurity.ReplaceSql(parameter);
        }
        #endregion
        public bool IsReusable => false;
    }
}