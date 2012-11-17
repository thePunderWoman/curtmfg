using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;
using System.Web.Script.Serialization;

namespace curtmfg.Controllers
{
    public class IndexController : BaseController
    {
        //
        // GET: /Index/

        public ActionResult Index()
        {
            ContentPage content = new ContentPage();
            content = new SiteContentModel().GetPrimary();
            ViewBag.content = content;

            List<NewsItem> news = new List<NewsItem>();
            news = NewsModel.GetLatest();
            ViewBag.news = news;

            List<Testimonial> testimonials = new List<Testimonial>();
            testimonials = TestimonialModel.GetRandomFive();
            ViewBag.testimonials = testimonials;

            /*List<APIPart> parts = new List<APIPart>();
            parts = Hitch_API.getLatestParts();
            ViewBag.parts = parts;*/

            return View();
        }

        public string ClearCache(string page = "") {
            if (page.Trim() != "") {
                try {
                    HttpResponse.RemoveOutputCacheItem(page.Trim());
                    return "Cache cleared for page: \"" + page + "\"";
                } catch (Exception e) {
                    return e.Message;
                }
            }
            return "No Cache Cleared";
        }

        [ChildActionOnly]
        public ActionResult Message(string message = "") {
            ViewBag.message = message;
            return PartialView("_Message");
        }

    }

}
