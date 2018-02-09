using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PC.Controllers
{
    public class AuthController : YoHao.Common.MVC.ClientController
    {
        // GET: Auth
        public ActionResult Login()
        {
            if (LoginInfo != null)
            {
                return Redirect("/");
            }
            return View();
           
        }

        public ActionResult Register()
        {
            if (LoginInfo != null)
            {
                return Redirect("/");
            }
            return View();
        }
    }
}