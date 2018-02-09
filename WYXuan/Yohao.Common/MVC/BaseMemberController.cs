using System.Web.Mvc;

namespace YoHao.Common.MVC
{
   public class BaseMemberController:ClientController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (LoginInfo == null)//判断用户登录
            {
                filterContext.Result = RedirectToAction("login", "auth", new { area = "" });
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
