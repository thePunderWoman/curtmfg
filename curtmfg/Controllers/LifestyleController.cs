using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;

namespace curtmfg.Controllers
{
    public class LifestyleController : BaseController
    {
        //
        // GET: /Index/

        [OutputCache(CacheProfile = "30mins")]
        public ActionResult Index() {
            List<curtmfg.Models.Category> lifestyles = Hitch_API.GetLifestyles();
            ViewBag.lifestyles = lifestyles;
            return View();
        }

        [OutputCache(CacheProfile = "30mins")]
        public ActionResult Lifestyle(int id = 0, string name = "") {
            if (id == 0) {
                return RedirectToAction("Index");
            }
            curtmfg.Models.Category lifestyle = Hitch_API.GetLifestyle(id);
            foreach (APITowable t in lifestyle.towables) {
                t.imageinfo = Hitch_API.getImageInfo(t.image);
            }
            ViewBag.lifestyle = lifestyle;
            curtmfg.Models.APICategoryParts catparts = lifestyle.GetParts(0, 1, 5);
            ViewBag.parts = catparts.parts;
            return View();
        }

    }
}
