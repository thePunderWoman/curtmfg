using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;

namespace curtmfg.Controllers
{
    public class NewsController : BaseController
    {
        //
        // GET: /FAQ/

        public ActionResult Index(int page = 1){

            // Get all the News listed by published Start Date
            List<NewsItem> news = NewsModel.GetPage(page);
            int pages = NewsModel.GetPages();
            ViewBag.pages = pages;
            ViewBag.page = page;
            ViewBag.news = news;

            return View();
        }

        public ActionResult Article(string date = "", string title = "") {
            // Get news item by name
            NewsItem item = NewsModel.Get(date,title);
            if (item != null && item.newsItemID != 0) {
                ViewBag.item = item;
                return View();
            } else {
                return RedirectToAction("Index");
            }
        }
        public ActionResult Archive(int page = 1, int perpage = 10) {
            // Get news item by name
            List<NewsItem> news = new List<NewsItem>();
            int pages = 0;
            try {
                news = NewsModel.GetArchive(page, perpage);
                pages = NewsModel.GetArchivePages(perpage);
            } catch { };
            ViewBag.news = news;
            ViewBag.pages = pages;
            ViewBag.page = page;
            return View();
        }
    }
}
