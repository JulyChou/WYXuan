using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using YoHao.Libs.Security;
using YoHao.Model.User;
using  YoHao.Core.Auth;
using YoHao.Libs;

namespace PC.Ajax
{
    /// <summary>
    /// Login 的摘要说明
    /// </summary>
    public class Login : MyHttpHandler
    {
        public Login() { this.IsValidateMember = false; }
        public override void ExecuteHandle(HttpContext context, StringBuilder resContent)
        {
            var txtval = context.Request["Mobile"].Trim();
            var txtpwd = context.Request["PassWord"].Trim();
            if (!string.IsNullOrEmpty(txtval) && !string.IsNullOrEmpty(txtpwd))
            {
                
                string sqlWhere;
                if (txtval.Length == 11 && DataValidate.IsNumberic(txtval))
                {
                    sqlWhere = $" Mobile='{txtval}'";
                }
                else
                {
                    sqlWhere = $" PassWord='{txtval}' ";
                }
                sqlWhere += $" and password='{StringHelper.MD5(txtpwd)}'";
                User = DB.Queryable<User>().Single(p=>p.Mobile==txtval && p.PassWord== StringHelper.MD5(txtpwd));

                if (User == null)
                {
                    resContent.Append("\"status\":\"n\", \"info\":\"该登录账号或密码错误！\"");
                    return;
                }
                //普通用户登录
                User.Sessionid = context.Session.SessionID;
                DB.Updateable(User);
                var authuer = new AuthUser()
                {
                    Id = User.Id,
                    Name = User.Name,
                    Sessionid = User.Sessionid
                };
                authuer.SetTicket(HttpContext.Current);
                resContent.Append("\"status\":\"y\", \"info\":\"登录成功！\",\"type\":" +"1");
            }
            else
            {
                resContent.Append("\"status\":\"n\", \"info\":\"请输入登录信息！\"");
            }
            base.ExecuteHandle(context, resContent);
        }
    }
}