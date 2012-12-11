using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace curtmfg.Controllers {
    public class LandingPageController : BaseController {
        //

        public ActionResult Index(int id = 0) {

            LandingPage page = new LandingPage().Get(id);
            if (page == null || (page != null && page.id == 0)) {
                return RedirectToAction("Index", "Index");
            }

            ViewBag.landingPage = page;

            return View();
        }


    }
}
