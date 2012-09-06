using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace curtmfg.Models {
    public class NewsModel {

        public static List<NewsItem> GetAll() {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                List<NewsItem> news = new List<NewsItem>();

                news = db.NewsItems.Where(x => x.active == true).Where(x => x.publishStart != null).Where(x => x.publishStart < DateTime.Now).Where(x => ((x.publishEnd > DateTime.Now) || (x.publishEnd == null))).OrderByDescending(x => x.publishStart).ToList<NewsItem>();

                return news;
            } catch (Exception e) {
                return new List<NewsItem>();
            }
        }

        public static List<NewsItem> GetArchive(int page = 1, int perpage = 10) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                List<NewsItem> news = new List<NewsItem>();

                int skip = (page - 1) * perpage;

                news = db.NewsItems.Where(x => x.active == true).Where(x => x.publishStart != null).Where(x => x.publishStart < DateTime.Now).Where(x => (x.publishEnd <= DateTime.Now)).OrderByDescending(x => x.publishStart).OrderByDescending(x => x.publishStart).Skip(skip).Take(perpage).ToList<NewsItem>();

                return news;
            } catch (Exception e) {
                return new List<NewsItem>();
            }
        }

        public static List<NewsItem> GetLatest() {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                List<NewsItem> news = new List<NewsItem>();

                news = db.NewsItems.Where(x => x.active == true).Where(x => x.publishStart != null).Where(x => x.publishStart < DateTime.Now).Where(x => ((x.publishEnd > DateTime.Now) || (x.publishEnd == null))).OrderByDescending(x => x.publishStart).Take(5).ToList<NewsItem>();

                return news;
            } catch (Exception e) {
                return new List<NewsItem>();
            }
        }

        public static int GetPages() {
            int pages = 0;
            CurtDevDataContext db = new CurtDevDataContext();
            int articles = db.NewsItems.Where(x => x.active == true).Where(x => x.publishStart != null).Where(x => x.publishStart < DateTime.Now).Where(x => ((x.publishEnd > DateTime.Now) || (x.publishEnd == null))).OrderByDescending(x => x.publishStart).Count();
            pages = (int) Math.Ceiling((decimal)articles / 5);
            return pages;
        }

        public static int GetArchivePages(int perpage = 0) {
            int pages = 0;
            CurtDevDataContext db = new CurtDevDataContext();
            int articles = db.NewsItems.Where(x => x.active == true).Where(x => x.publishStart != null).Where(x => x.publishStart < DateTime.Now).Where(x => (x.publishEnd <= DateTime.Now)).OrderByDescending(x => x.publishStart).Count();
            pages = (int)Math.Ceiling((decimal)articles / perpage);
            return pages;
        }

        public static List<NewsItem> GetPage(int page = 1) {
            try {
                int ResultsPerPage = 5;
                CurtDevDataContext db = new CurtDevDataContext();
                List<NewsItem> news = new List<NewsItem>();

                //number of records to skip
                int skip = (page - 1) * ResultsPerPage;

                //number of results per page. 
                int take = page;

                news = db.NewsItems.Where(x => x.active == true).Where(x => x.publishStart != null).Where(x => x.publishStart < DateTime.Now).Where(x => ((x.publishEnd > DateTime.Now) || (x.publishEnd == null))).OrderByDescending(x => x.publishStart).Skip(skip).Take(ResultsPerPage).ToList<NewsItem>();

                return news;
            } catch (Exception e) {
                return new List<NewsItem>();
            }
        }

        public static NewsItem Get(string date = "", string name = "") {
            try {
                DateTime post_date = Convert.ToDateTime(date);
                CurtDevDataContext db = new CurtDevDataContext();
                NewsItem newsitem = new NewsItem();
                newsitem = db.NewsItems.Where(x => x.active.Equals(true))
                    .Where(x => x.publishStart != null).Where(x => x.publishStart < DateTime.Now)
                    .Where(x => x.slug.ToLower() == name.ToLower().Trim())
                    .Where(x => Convert.ToDateTime(x.publishStart).Day.Equals(post_date.Day))
                    .Where(x => Convert.ToDateTime(x.publishStart).Year.Equals(post_date.Year))
                    .Where(x => Convert.ToDateTime(x.publishStart).Month.Equals(post_date.Month))
                    .FirstOrDefault<NewsItem>();

                return newsitem;
            } catch (Exception e) {
                return new NewsItem();
            }
        }

    }
}