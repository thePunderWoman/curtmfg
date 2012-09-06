using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;

namespace curtmfg.Controllers
{
    public class ContentController : BaseController
    {
        //
        // GET: /Content/

        //[OutputCache(CacheProfile="5mins")]
        public ActionResult Index(string name = "", int menuid = 0)
        {
            bool authenticated = false;
            if (AuthenticateModel.checkAuth(Request.Cookies.Get("customerID"))) {
                authenticated = true;
            }
            ContentPage content = SiteContentModel.Get(name, menuid, authenticated);
            if (content.contentID > 0 && content.published) {
                ViewBag.content = content;
                if (content.requireAuthentication && !authenticated) {
                    string message = "Authentication is required to view that content. Please login using the form below.";
                    return RedirectToAction("Login", "Dealer", new { message = message });
                }
                return View();
            } else {
                return RedirectToAction("Index","_404");
            }
        }

    }
}
