using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;

namespace curtmfg.Controllers
{
    public class SitemapController : BaseController
    {
        //
        // GET: /FAQ/

        public ActionResult Index(){

            List<menuWithContent> sitemap = MenuModel.GetSitemap();
            ViewBag.sitemap = sitemap;

            List<ContentPage> contents = SiteContentModel.GetSitemap();
            ViewBag.contents = contents;

            List<NewsItem> news = NewsModel.GetAll();
            ViewBag.news = news;

            List<PostWithCategories> posts = PostModel.GetSitemap();
            ViewBag.posts = posts;

            List<curtmfg.Models.Category> categories = curtmfg.Models.Category.GetAllCategories();
            ViewBag.categories = UDF.generateCategorySitemap(categories);

            List<curtmfg.Models.Category> lifestyles = Hitch_API.GetLifestyles();
            ViewBag.lifestyles = lifestyles;

            return View();

        }

    }
}
