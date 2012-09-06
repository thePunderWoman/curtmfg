using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;
using System.Drawing;
using System.Web.Script.Serialization;
using System.IO;
using System.Configuration;

namespace curtmfg.Controllers
{
    public class CategoryController : BaseController
    {
        //
        // GET: /Accessories/

        //[DonutOutputCache(CacheProfile = "1Hours")]
        public ActionResult Index(string name = "", int id = 0, int page = 1, int perpage = 10) {

            curtmfg.Models.Category category = new curtmfg.Models.Category();
            if (id == 0) {
                category = curtmfg.Models.Category.GetByName(HttpUtility.UrlEncode(name.Replace('|', '/')));
            } else {
                category = curtmfg.Models.Category.Get(id);
            }
             
            if(category.catID > 0) {
                HttpCookie category_cookie = new HttpCookie("last_category");
                category_cookie.Value = category.catID.ToString();
                category_cookie.Expires = DateTime.Now.AddDays(14);
                Response.Cookies.Add(category_cookie);
                List<string> attributes = category.GetAttributes();
                ViewBag.attributes = attributes;
                int vehicleID = (ViewBag.vehicleID != "") ? Convert.ToInt32(ViewBag.vehicleID) : 0;
                ViewBag.category = category;
                curtmfg.Models.APICategoryParts catparts = category.GetParts(0, vehicleID, page, perpage);
                
                ViewBag.page = page;
                ViewBag.perpage = perpage;
                ViewBag.pagecount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(catparts.total) / Convert.ToDecimal(perpage)));
                ViewBag.parts = catparts.parts;
                List<curtmfg.Models.Category> breadcrumbs = curtmfg.Models.Category.GetBreadcrumbs(category.catID);
			    curtmfg.Models.Category parent = category;
			    if (breadcrumbs.Count() > 0) {
				    parent = breadcrumbs[breadcrumbs.Count() - 1];
			    }
                ViewBag.parent = parent;
                ViewBag.breadcrumbs = breadcrumbs;
			    CurtDevDataContext db = new CurtDevDataContext();
			    string colorcode = (from c in db.ColorCodes
								    join ct in db.Categories on c.codeID equals ct.codeID
								    where ct.catID.Equals(parent.catID)
								    select c.code).FirstOrDefault<string>();
			    if (colorcode != null && colorcode != "") {
				    int redval = Convert.ToInt32(colorcode.Substring(0, 3));
				    int greenval = Convert.ToInt32(colorcode.Substring(3, 3));
				    int blueval = Convert.ToInt32(colorcode.Substring(6, 3));
				    ViewBag.colorcode = redval + "," + greenval + "," + blueval;
				    ViewBag.hexcode = ColorTranslator.ToHtml(Color.FromArgb(redval, greenval, blueval));
			    }
    			return View();
            } else {
                return RedirectToAction("index","_404");
            }
        }

        [OutputCache(CacheProfile = "1Hours")]
        public ActionResult Grid(string name = "", int id = 0, int page = 1, int perpage = 50) {

            curtmfg.Models.Category category = new curtmfg.Models.Category();
            if (id == 0) {
                category = curtmfg.Models.Category.GetByName(HttpUtility.UrlEncode(name.Replace('|', '/')));
            } else {
                category = curtmfg.Models.Category.Get(id);
            }

            if (category.catID > 0) {
                HttpCookie category_cookie = new HttpCookie("last_category");
                category_cookie.Value = category.catID.ToString();
                category_cookie.Expires = DateTime.Now.AddDays(14);
                Response.Cookies.Add(category_cookie);
                List<string> attributes = category.GetAttributes();
                ViewBag.attributes = attributes;

                ViewBag.category = category;
                curtmfg.Models.APICategoryParts catparts = category.GetParts(0, page, perpage);

                ViewBag.page = page;
                ViewBag.perpage = perpage;
                ViewBag.pagecount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(catparts.total) / Convert.ToDecimal(perpage)));
                ViewBag.parts = catparts.parts;
                List<curtmfg.Models.Category> breadcrumbs = curtmfg.Models.Category.GetBreadcrumbs(category.catID);
                curtmfg.Models.Category parent = category;
                if (breadcrumbs.Count() > 0) {
                    parent = breadcrumbs[breadcrumbs.Count() - 1];
                }
                ViewBag.parent = parent;
                ViewBag.breadcrumbs = breadcrumbs;
                CurtDevDataContext db = new CurtDevDataContext();
                string colorcode = (from c in db.ColorCodes
                                    join ct in db.Categories on c.codeID equals ct.codeID
                                    where ct.catID.Equals(parent.catID)
                                    select c.code).FirstOrDefault<string>();
                if (colorcode != null && colorcode != "") {
                    int redval = Convert.ToInt32(colorcode.Substring(0, 3));
                    int greenval = Convert.ToInt32(colorcode.Substring(3, 3));
                    int blueval = Convert.ToInt32(colorcode.Substring(6, 3));
                    ViewBag.colorcode = redval + "," + greenval + "," + blueval;
                    ViewBag.hexcode = ColorTranslator.ToHtml(Color.FromArgb(redval, greenval, blueval));
                }
                return View();
            } else {
                return RedirectToAction("index", "_404");
            }
        }

        /*[OutputCache(CacheProfile = "30mins")]
        public string GetNextPage(int categoryID = 0, int page = 1) {
            string json = curtmfg.Models.APIModels.MakeJSONRequest(ConfigurationManager.AppSettings["APIURL"] + "/GetCategoryParts?catID=" + categoryID + "&page=" + page + "&perpage=5&dataType=JSON");
            return json;
        }*/
    }
}
