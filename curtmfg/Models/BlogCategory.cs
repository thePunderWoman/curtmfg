﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml;

namespace curtmfg.Models {
    public class BlogCategoryModel {

        public static List<BlogCategory> GetCategories() {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                List<BlogCategory> categories = new List<BlogCategory>();

                categories = db.BlogCategories.Where(x => x.active == true).OrderBy(x => x.name).ToList<BlogCategory>();

                return categories;
            } catch {
                return new List<BlogCategory>();
            }
        }

        public static BlogCategory GetCategory(int id = 0) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                BlogCategory category = new BlogCategory();

                category = db.BlogCategories.Where(x => x.blogCategoryID == id).FirstOrDefault<BlogCategory>();

                return category;
            } catch {
                return new BlogCategory();
            }
        }

        public static BlogCategory GetCategoryByName(string name = "") {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                BlogCategory category = new BlogCategory();

                category = db.BlogCategories.Where(x => x.slug == name).FirstOrDefault<BlogCategory>();

                return category;
            } catch {
                return new BlogCategory();
            }
        }

    }
}