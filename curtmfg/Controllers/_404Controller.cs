using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;

namespace curtmfg.Controllers
{
    public class _404Controller : BaseController
    {
        //
        // GET: /Error/

        [OutputCache(CacheProfile = "24Hours")]
        public ActionResult Index()
        {
            try {
                string errorpath = Request.QueryString["aspxerrorpath"];
                if (errorpath.LastIndexOf("/") != -1 && errorpath.LastIndexOf("/") == 0) {
                    string name = errorpath.Substring(1);
                    CurtDevDataContext db = new CurtDevDataContext();
                    if (db.SiteContents.Where(x => x.page_title.ToLower() == name.ToLower()).Where(x => x.published == true).Where(x => x.active == true).SingleOrDefault() != null) {
                        return RedirectToRoute("Page", new { name = name });
                    }
                }
            } catch (Exception e){
                string message = e.Message;
            }

            return View();
        }

    }
}
