using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using YoHao.Core.Auth;
using YoHao.Libs;

namespace PC.Ajax
{
    /// <summary>
    /// Register 的摘要说明
    /// </summary>
    public class Register : MyHttpHandler
    {
        public Register() { this.IsValidateMember = false; }

        public override void ExecuteHandle(HttpContext context, StringBuilder resContent)
        {
            var txtmobile = Check(context.Request["Mobile"]);
            var txtpwd = context.Request["PassWord"];
            var txtName = context.Request["Name"];
            var mobileAny= DB.Queryable<YoHao.Model.User.User>().Where(it => it.Mobile == txtmobile).Any();
            if (mobileAny)
            {
                 resContent.Append("\"status\":\"n\", \"info\":\"注册失败手机号已存在！\"");
                base.ExecuteHandle(context, resContent);
                return;
            }
            var user = new YoHao.Model.User.User
            {
                PassWord = StringHelper.MD5(txtpwd),
                Name = txtName,
                Mobile = txtmobile,
                CreateTime = DateTime.Now,
                Info="",
                Sessionid = context.Session.SessionID
            };
            var t3 = DB.Insertable(user).ExecuteReturnEntity();
            var authuer = new AuthUser()
            {
                Id = t3.Id,
                Name = t3.Name,
                Sessionid = t3.Sessionid
            };
            authuer.SetTicket(HttpContext.Current);
            resContent.Append("\"status\":\"y\", \"info\":\"注册成功！\"");
            base.ExecuteHandle(context, resContent);
        }
    }
}