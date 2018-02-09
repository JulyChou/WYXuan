using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using SqlSugar;
using Yohao.Common.MVC;
using System.Web.Routing;

namespace YoHao.Common.MVC
{
    public class ClientController : Controller
    {
        public SqlSugarClient DB { get; } = DBbase.GetInstance();
        public YoHao.Model.User.User LoginInfo = null;
        protected override void Initialize(RequestContext requestContext)
        {
            var currentUser = YoHao.Core.Auth.AuthUser.GetMiniMember(System.Web.HttpContext.Current);
            if (currentUser != null)
            {
                LoginInfo = DB.Queryable<YoHao.Model.User.User>().InSingle(currentUser.Id);
            }
            ViewBag.LoginInfo = LoginInfo;//创建登录后视图信息
            base.Initialize(requestContext);
        }

    }
}
