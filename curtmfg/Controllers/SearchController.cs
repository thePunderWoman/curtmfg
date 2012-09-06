using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;

namespace curtmfg.Controllers
{
    public class SearchController : BaseController
    {
        //
        // GET: /FAQ/

        public ActionResult Index(string term = "") {

            // Get all the FAQs listed alphabetically by question
            List<APIPart> parts = SearchModel.Search(term);
            ViewBag.parts = parts;
            ViewBag.term = term;
            return View();
        }

    }
}
