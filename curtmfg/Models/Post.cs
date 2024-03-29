﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace curtmfg.Models {
    public class PostModel {
        public static List<BlogPost> GetAll() {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                List<BlogPost> posts = new List<BlogPost>();

                posts = db.BlogPosts.Where(x => x.active == true).OrderBy(x => x.publishedDate).OrderBy(x => x.createdDate).ToList<BlogPost>();

                return posts;
            } catch (Exception e) {
                return new List<BlogPost>();
            }
        }

        public static List<PostWithCategories> GetAllPublished(int page = 1, int pageSize = 5) {
            CurtDevDataContext db = new CurtDevDataContext();
            List<PostWithCategories> posts = new List<PostWithCategories>();

            try {
                posts = (from p in db.BlogPosts
                     where p.publishedDate.Value <= DateTime.Now && p.active.Equals(true)
                     orderby p.publishedDate descending
                     select new PostWithCategories {
                         blogPostID = p.blogPostID,
                         post_title = p.post_title,
                         post_text = p.post_text,
                         slug = p.slug,
                         publishedDate = p.publishedDate,
                         createdDate = p.createdDate,
                         lastModified = p.lastModified,
                         keywords = p.keywords,
                         meta_title = p.meta_title,
                         meta_description = p.meta_description,
                         active = p.active,
                         author = GetAuthor(p.userID),
                         categories = p.BlogPost_BlogCategories.Select(x => x.BlogCategory).Distinct().ToList<BlogCategory>(),
                         comments = p.Comments.Where(x => x.active.Equals(true) && x.approved.Equals(true)).ToList<Comment>()
                     }).Skip((page - 1) * pageSize).Take(pageSize).ToList<PostWithCategories>();

            } catch { }
            return posts;
        }

        public static List<PostWithCategories> GetSitemap() {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                List<PostWithCategories> posts = new List<PostWithCategories>();

                posts = (from p in db.BlogPosts
                         where p.publishedDate.Value <= DateTime.Now && p.active.Equals(true)
                         orderby p.publishedDate descending
                         select new PostWithCategories {
                             blogPostID = p.blogPostID,
                             post_title = p.post_title,
                             post_text = p.post_text,
                             slug = p.slug,
                             publishedDate = p.publishedDate,
                             createdDate = p.createdDate,
                             lastModified = p.lastModified,
                             keywords = p.keywords,
                             meta_title = p.meta_title,
                             meta_description = p.meta_description,
                             active = p.active,
                             author = GetAuthor(p.userID),
                             categories = (from c in db.BlogCategories join pc in db.BlogPost_BlogCategories on c.blogCategoryID equals pc.blogCategoryID where pc.blogPostID.Equals(p.blogPostID) select c).ToList<BlogCategory>(),
                             comments = (from cm in db.Comments where cm.blogPostID.Equals(p.blogPostID) && cm.active.Equals(true) && cm.approved.Equals(true) select cm).ToList<Comment>()
                         }).ToList();

                return posts;
            } catch (Exception e) {
                return new List<PostWithCategories>();
            }
        }

        public static List<PostWithCategories> GetAllPublishedByCategory(string name = "", int page = 1, int pageSize = 5) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                List<PostWithCategories> posts = new List<PostWithCategories>();

                posts = (from p in db.BlogPosts
                         join pca in db.BlogPost_BlogCategories on p.blogPostID equals pca.blogPostID
                         join ca in db.BlogCategories on pca.blogCategoryID equals ca.blogCategoryID
                         where p.publishedDate.Value <= DateTime.Now && p.active.Equals(true) && ca.slug.Equals(name)
                         orderby p.publishedDate descending
                         select new PostWithCategories {
                             blogPostID = p.blogPostID,
                             post_title = p.post_title,
                             post_text = p.post_text,
                             slug = p.slug,
                             publishedDate = p.publishedDate,
                             createdDate = p.createdDate,
                             lastModified = p.lastModified,
                             keywords = p.keywords,
                             meta_title = p.meta_title,
                             meta_description = p.meta_description,
                             active = p.active,
                             author = GetAuthor(p.userID),
                             categories = (from c in db.BlogCategories join pc in db.BlogPost_BlogCategories on c.blogCategoryID equals pc.blogCategoryID where pc.blogPostID.Equals(p.blogPostID) select c).ToList<BlogCategory>(),
                             comments = (from cm in db.Comments where cm.blogPostID.Equals(p.blogPostID) && cm.active.Equals(true) && cm.approved.Equals(true) select cm).ToList<Comment>()
                         }).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return posts;
            } catch (Exception e) {
                return new List<PostWithCategories>();
            }
        }

        public static List<PostWithCategories> GetAllPublishedByDate(string month = "", string year = "", int page = 1, int pageSize = 5) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                List<PostWithCategories> posts = new List<PostWithCategories>();
                DateTime startDate = Convert.ToDateTime(month + " 1, " + year);
                DateTime endDate = startDate.AddMonths(1);

                posts = (from p in db.BlogPosts
                         where p.publishedDate.Value <= endDate && p.publishedDate.Value >= startDate && p.active.Equals(true)
                         orderby p.publishedDate descending
                         select new PostWithCategories {
                             blogPostID = p.blogPostID,
                             post_title = p.post_title,
                             post_text = p.post_text,
                             slug = p.slug,
                             publishedDate = p.publishedDate,
                             createdDate = p.createdDate,
                             lastModified = p.lastModified,
                             keywords = p.keywords,
                             meta_title = p.meta_title,
                             meta_description = p.meta_description,
                             active = p.active,
                             author = GetAuthor(p.userID),
                             categories = (from c in db.BlogCategories join pc in db.BlogPost_BlogCategories on c.blogCategoryID equals pc.blogCategoryID where pc.blogPostID.Equals(p.blogPostID) select c).ToList<BlogCategory>(),
                             comments = (from cm in db.Comments where cm.blogPostID.Equals(p.blogPostID) && cm.active.Equals(true) && cm.approved.Equals(true) select cm).ToList<Comment>()
                         }).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return posts;
            } catch (Exception e) {
                return new List<PostWithCategories>();
            }
        }

        public static int CountAllPublished() {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                int count = 0;
                count = (from p in db.BlogPosts
                         where p.publishedDate.Value <= DateTime.Now && p.active.Equals(true)
                         select p.blogPostID
                         ).Count();
                return count;
            } catch (Exception e) {
                return 0;
            }
        }

        public static int CountAllPublishedByDate(string month = "", string year = "") {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                DateTime startDate = Convert.ToDateTime(month + " 1, " + year);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                int count = 0;
                count = (from p in db.BlogPosts
                         where p.publishedDate.Value <= endDate && p.publishedDate.Value >= startDate && p.active.Equals(true)
                         select p.blogPostID
                         ).Count();
                return count;
            } catch (Exception e) {
                return 0;
            }
        }

        public static DateTime GetLatestPublishedDate() {
            DateTime latest = DateTime.Now;
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                latest = (from p in db.BlogPosts
                          where p.publishedDate.Value != null && p.active.Equals(true)
                          orderby p.publishedDate descending
                          select (DateTime)p.publishedDate).Single();
            } catch { }
            return latest;
        }

        public static DateTime GetLatestPublishedDateByCategory(string name = "") {
            DateTime latest = DateTime.Now;
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                latest = (from p in db.BlogPosts
                          join pca in db.BlogPost_BlogCategories on p.blogPostID equals pca.blogPostID
                          join c in db.BlogCategories on pca.blogCategoryID equals c.blogCategoryID
                          where p.publishedDate.Value != null && p.active.Equals(true) && c.slug.Equals(name)
                          orderby p.publishedDate descending
                          select (DateTime)p.publishedDate).Single();
            } catch { }
            return latest;
        }

        public static int CountAllPublishedByCategory(string name = "") {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                int count = 0;
                count = (from p in db.BlogPosts
                         join pca in db.BlogPost_BlogCategories on p.blogPostID equals pca.blogPostID
                         join c in db.BlogCategories on pca.blogCategoryID equals c.blogCategoryID
                         where p.publishedDate.Value <= DateTime.Now && p.active.Equals(true) && c.slug.Equals(name)
                         select p.blogPostID
                         ).Count();
                return count;
            } catch (Exception e) {
                return 0;
            }
        }

        public static PostWithCategories Get(string date = "", string title = "") {
            try {
                DateTime post_date = Convert.ToDateTime(date);
                CurtDevDataContext db = new CurtDevDataContext();
                PostWithCategories post = new PostWithCategories();
                post = (from p in db.BlogPosts
                        where p.slug.Equals(title) && Convert.ToDateTime(p.publishedDate).Day.Equals(post_date.Day)
                        && Convert.ToDateTime(p.publishedDate).Year.Equals(post_date.Year) && Convert.ToDateTime(p.publishedDate).Month.Equals(post_date.Month)
                        select new PostWithCategories {
                            blogPostID = p.blogPostID,
                            post_title = p.post_title,
                            post_text = p.post_text,
                            slug = p.slug,
                            publishedDate = p.publishedDate,
                            createdDate = p.createdDate,
                            lastModified = p.lastModified,
                            meta_title = p.meta_title,
                            meta_description = p.meta_description,
                            active = p.active,
                            author = GetAuthor(p.userID),
                            categories = (from c in db.BlogCategories join pc in db.BlogPost_BlogCategories on c.blogCategoryID equals pc.blogCategoryID where pc.blogPostID.Equals(p.blogPostID) select c).ToList<BlogCategory>(),
                            comments = (from cm in db.Comments where cm.blogPostID.Equals(p.blogPostID) && cm.active.Equals(true) && cm.approved.Equals(true) select cm).ToList<Comment>()
                        }).First<PostWithCategories>();

                return post;
            } catch (Exception e) {
                return new PostWithCategories();
            }
        }

        public static BlogPost GetById(int id = 0) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                BlogPost post = new BlogPost();
                post = (from p in db.BlogPosts
                        where p.blogPostID.Equals(id)
                        select p).First<BlogPost>();

                return post;
            } catch {
                return new BlogPost();
            }
        }

        public static void Delete(int id = 0) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                BlogPost p = db.BlogPosts.Where(x => x.blogPostID == id).FirstOrDefault<BlogPost>();
                p.active = false;
                db.SubmitChanges();
            } catch (Exception e) {
                throw e;
            }
        }

        private static user GetAuthor(int id = 0) {
            docsDataContext doc_db = new docsDataContext();
            return (from u in doc_db.users where u.userID.Equals(id) select u).First<user>();
        }
    }

    public class PostWithCategories : BlogPost {
        public user author { get; set; }
        public List<curtmfg.BlogCategory> categories { get; set; }
        public List<Comment> comments { get; set; }
    }

}