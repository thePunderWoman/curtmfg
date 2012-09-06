using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml;
using System.Configuration;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace curtmfg.Models {
    public class Category {
        private static string APIURL = ConfigurationManager.AppSettings["APIURL"];
        private List<Category> _subs = new List<Category>();

        public int catID { get; set; }
        public int parentID { get; set; }
        public DateTime dateAdded { get; set; }
        public string catTitle { get; set; }
        public string shortDesc { get; set; }
        public string longDesc { get; set; }
        public string image { get; set; }
        public int isLifestyle { get; set; }
        public int sort { get; set; }
        public bool vehicleSpecific { get; set; }
        public List<APIContent> content { get; set; }
        public List<APITowable> towables { get; set; }
        public List<Category> sub_categories {
            get {
                return this._subs;
            }
            set {
                this._subs = value;
            }
        }

        // not presently in use
        public static List<Category> GetCategories(int catID = 0) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = curtmfg.Models.Hitch_API.getAPIPath();
                url += "GetCategories?dataType=JSON";
                url += "&parentID=" + catID;

                return JsonConvert.DeserializeObject<List<Category>>(wc.DownloadString(url));
            } catch (Exception) {
                return new List<Category>();
            }
        }

        // used only in sitemap
        public static List<Category> GetAllCategories() {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = curtmfg.Models.Hitch_API.getAPIPath();
                url += "/GetCategories";
                url += "?dataType=JSON";

                List<Category> cats = new List<Category>();

                string cat_json = wc.DownloadString(url);
                cats = JsonConvert.DeserializeObject<List<Category>>(cat_json);

                return cats.OrderBy(x => x.parentID).ThenBy(x => x.sort).ToList<Category>();
            } catch (Exception) {
                return new List<Category>();
            }
        }
        
        public static List<Category> GetParentCategories() {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = curtmfg.Models.Hitch_API.getAPIPath();
                url += "/GetParentCategories";
                url += "?dataType=JSON";

                List<Category> cats = new List<Category>();

                string cat_json = wc.DownloadString(url);
                cats = JsonConvert.DeserializeObject<List<Category>>(cat_json);

                return cats;
            } catch (Exception) {
                return new List<Category>();
            }
            
        }

		public static Category GetByName(string name = "") {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;
                string url = curtmfg.Models.Hitch_API.getAPIPath();
                url += "/GetCategoryByName?dataType=JSON";
                url += "&catName=" + name;
                string cat_json = wc.DownloadString(url);
                JSONAPICategory ugly_cat = JsonConvert.DeserializeObject<JSONAPICategory>(cat_json);
                Category api_cat = ugly_cat.parent;
                api_cat.content = ugly_cat.content;
                api_cat.sub_categories = ugly_cat.sub_categories;

                return api_cat;
            } catch (Exception e) {
                return new Category();
            }
		}

        public static Category Get(int id = 0) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = curtmfg.Models.Hitch_API.getAPIPath();
                url += "/GetCategory?dataType=JSON";
                url += "&catID=" + id;

                string cat_json = wc.DownloadString(url);
                JSONAPICategory ugly_cat = JsonConvert.DeserializeObject<JSONAPICategory>(cat_json);
                Category api_cat = ugly_cat.parent;
                api_cat.content = ugly_cat.content;
                api_cat.sub_categories = ugly_cat.sub_categories;

                return api_cat;
            } catch (Exception) {
                return new Category();
            }

        }

        public static List<Category> GetBreadcrumbs(int catId = 0) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = curtmfg.Models.Hitch_API.getAPIPath();
                url += "/GetCategoryBreadCrumbs";
                url += "?dataType=JSON";
                url += "&catId=" + catId;

                List<Category> cats = new List<Category>();

                string cat_json = wc.DownloadString(url);
                cats = JsonConvert.DeserializeObject<List<Category>>(cat_json);

                return cats;
            } catch (Exception) {
                return new List<Category>();
            }

        }
        
        public List<Category> GetCategories() {
            try {
                List<Category> cats = new List<Category>();
                if (this.catID != 0) {

                    WebClient wc = new WebClient();
                    wc.Proxy = null;

                    string url = curtmfg.Models.Hitch_API.getAPIPath();
                    url += "/GetCategories?parentID=" + this.catID;
                    url += "&dataType=JSON";


                    string cat_json = wc.DownloadString(url);
                    cats = JsonConvert.DeserializeObject<List<Category>>(cat_json);
                }
                return cats;
            } catch (Exception) {
                return new List<Category>();
            }


        }

        public Category GetByName() {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;
                string url = curtmfg.Models.Hitch_API.getAPIPath();
                url += "/GetCategoryByName?dataType=JSON";
                url += "&catName=" + this.catTitle;
                string cat_json = wc.DownloadString(url);
                JSONAPICategory ugly_cat = JsonConvert.DeserializeObject<JSONAPICategory>(cat_json);
                Category api_cat = ugly_cat.parent;
                api_cat.content = ugly_cat.content;
                api_cat.sub_categories = ugly_cat.sub_categories;

                return api_cat;
            } catch (Exception e) {
                return new Category();
            }
        }

        public Category Get() {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = curtmfg.Models.Hitch_API.getAPIPath();
                url += "/GetCategory?dataType=JSON";
                url += "&catID=" + this.catID;

                string cat_json = wc.DownloadString(url);
                JSONAPICategory ugly_cat = JsonConvert.DeserializeObject<JSONAPICategory>(cat_json);
                Category api_cat = ugly_cat.parent;
                api_cat.content = ugly_cat.content;
                api_cat.sub_categories = ugly_cat.sub_categories;

                return api_cat;
            } catch (Exception) {
                return new Category();
            }
        }

        public List<string> GetAttributes() {
            try {
                string attribute_json = "";
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = curtmfg.Models.Hitch_API.getAPIPath();
                url += "/GetCategoryAttributes?catID=" + this.catID + "&dataType=JSON";

                attribute_json = wc.DownloadString(url);
                List<string> attributes = JsonConvert.DeserializeObject<List<string>>(attribute_json);
                return attributes;
            } catch (Exception) {
                return new List<string>();
            }
        }

        public APICategoryParts GetParts(int acctID = 0, int vehicleID = 0, int page = 1, int perpage = 5) {
            APICategoryParts catparts = new APICategoryParts {
                page = page,
                perpage = perpage,
                total = 0,
                parts = new List<APIPart>()
            };
            List<APIPart> parts = new List<APIPart>();
            string url = APIURL + "/GetCategoryParts?catID=" + this.catID + "&vehicleID=" + vehicleID + "&cust_id=" + acctID + "&page=" + page + "&perpage=" + perpage + "&dataType=JSON";
            string url2 = APIURL + "/GetCategoryPartsCount?catID=" + this.catID;
            try {

                WebClient wc = new WebClient();
                wc.Proxy = null;

                string part_json = wc.DownloadString(url);
                string count_json = wc.DownloadString(url2);

                parts = JsonConvert.DeserializeObject<List<APIPart>>(part_json);

                int count = Convert.ToInt32(count_json);


                catparts.total = count;
                catparts.parts = parts;
            } catch (Exception e) {
                string err = e.Message;
            }
            return catparts;
        }

    }

    public class JSONAPICategory {
        public Category parent { get; set; }
        public List<Category> sub_categories { get; set; }
        public List<APIContent> content { get; set; }
    }
}
