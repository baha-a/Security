using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using ITech.Security.Business;
using ITech.Security.Models;

namespace ITech.Security.Business
{
    public abstract class BaseController : Controller
    {
        protected DbSearcher ask;

        protected BaseController()
        {
            var db = new SecurityDB();
            ask = new DbSearcher(db);
        }


        protected string UserName
        {
            get { return User.Identity.Name.FixUserName(); }
        }


        protected bool IsAllowed(string operation)
        {
            return ask.IsAllow(UserName, operation, AppSettings.ApplicationId).IsAllowed;
        }
        protected bool IsNotAllowed(string operation)
        {
            return IsAllowed(operation) == false;
        }
        protected void HasPermission(string operation)
        {
            if (IsNotAllowed(operation))
                throw new HttpException(401, "access denied for '" + operation + "'");
        }


        protected JsonResult JsonX(object x)
        {
            return Json(x, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult JsonAccessDenied(string msg = "access deiend")
        {
            Response.StatusCode = 401;
            return JsonX(msg);
        }

        protected JsonResult JsonNotFound(string msg = "not found")
        {
            Response.StatusCode = 404;
            return JsonX(msg);
        }
    }
}