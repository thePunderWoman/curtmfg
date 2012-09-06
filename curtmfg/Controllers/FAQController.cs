using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;

namespace curtmfg.Controllers
{
    public class FAQController : BaseController
    {
        //
        // GET: /FAQ/

        public ActionResult Index(){

            // Get all the FAQs listed alphabetically by question
            List<FAQ> faqs = FAQModel.GetAll("question", "ascending");
            ViewBag.faqs = faqs;

            return View();
        }

    }
}
