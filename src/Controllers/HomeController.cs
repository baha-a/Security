using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITech.Security.Business;
using ITech.Security.Models;

namespace ITech.Security.Controllers
{

    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View(ask.GetAllUserViews(UserName).GroupBySection().Order());
        }

        public ActionResult CacheReset(string returnUrl = "")
        {
            Cacher.BuildCache();

            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index");
            return Redirect(returnUrl);
        }

        public ActionResult Refresh(string returnUrl = "")
        {
            new SecurityDB().AddTheEssentialDataIfDeleteFromDB();
            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index");
            return Redirect(returnUrl);
        }
    }
}