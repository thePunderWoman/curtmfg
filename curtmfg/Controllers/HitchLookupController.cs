using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;
using System.Collections;
using System.Data;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Net;
using System.Xml.Linq;
using System.Web.Script.Serialization;


namespace curtmfg.Controllers
{
    public class HitchLookupController : BaseController
    {
        //
        // GET: /HitchLookup/
        

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Mount(string mount = "rear mount") {

            ViewBag.mount = mount;

            List<string> years = new List<string>();
            years = Hitch_API.getYears(mount);
            ViewBag.years = years;

            return View();
        }

        public ActionResult Year(string mount = "", string year = "") {

            ViewBag.mount = mount;
            ViewBag.year = year;

            List<string> makes = new List<string>();
            makes = Hitch_API.getMakes(mount, year);
            ViewBag.makes = makes;

            return View();
        }

        public ActionResult Make(string mount = "", string year = "", string make = "") {
            ViewBag.mount = mount;
            ViewBag.year = year;
            ViewBag.make = make.Replace('|', '/');

            List<string> models = new List<string>();
            models = Hitch_API.getModels(mount, year, make.Replace('|', '/'));
            ViewBag.models = models;

            return View();
        }

        public ActionResult Model(string mount = "", string year = "", string make = "", string model = "") {
            ViewBag.mount = mount;
            ViewBag.year = year;
            ViewBag.make = make.Replace('|', '/');
            ViewBag.model = model.Replace('|', '/');

            List<string> styles = new List<string>();
            styles = Hitch_API.getStyles(mount, year, make.Replace('|', '/'), model.Replace('|', '/'));
            ViewBag.styles = styles;
            return View();
        }

        //[OutputCache(CacheProfile = "30mins")]
        public ActionResult find(string year = "", string make = "", string model = "", string style = "") {

            // Store the vehicle fields in cookies
            HttpCookie year_cookie = new HttpCookie("vehicle_year");
            year_cookie.Value = year;
            year_cookie.Expires = DateTime.Now.AddDays(14);
            Response.Cookies.Add(year_cookie);
            ViewBag.year = year;

            HttpCookie make_cookie = new HttpCookie("vehicle_make");
            make_cookie.Value = make;
            make_cookie.Expires = DateTime.Now.AddDays(14);
            Response.Cookies.Add(make_cookie);
            ViewBag.make = make;

            HttpCookie model_cookie = new HttpCookie("vehicle_model");
            model_cookie.Value = model;
            model_cookie.Expires = DateTime.Now.AddDays(14);
            Response.Cookies.Add(model_cookie);
            ViewBag.model = model;

            HttpCookie style_cookie = new HttpCookie("vehicle_style");
            style_cookie.Value = style;
            style_cookie.Expires = DateTime.Now.AddDays(14);
            Response.Cookies.Add(style_cookie);
            ViewBag.style = style;

            FullVehicle vehicle = Hitch_API.getVehicle(year, make, model, style);
            HttpCookie id_cookie = new HttpCookie("vehicle_id");
            id_cookie.Value = vehicle.vehicleID.ToString();
            id_cookie.Expires = DateTime.Now.AddDays(14);
            Response.Cookies.Add(id_cookie);
            ViewBag.vehicleID = vehicle.vehicleID;

            List<APIPart> parts = Hitch_API.find(year, make, model, style);

            Dictionary<string, List<APIPart>> classes = new Dictionary<string, List<APIPart>>();
            Dictionary<string, APIColorCode> colors = new Dictionary<string, APIColorCode>();

            foreach(APIPart part in parts) {
                if(part.pClass != "") {
                    if (!classes.ContainsKey(part.pClass)) {
                        classes.Add(part.pClass, new List<APIPart> { part });
                        colors.Add(part.pClass, Hitch_API.GetColorCode( part.partID ));
                    } else {
                        classes[part.pClass].Add(part);
                    }
                } else {
                    if (!classes.ContainsKey("Wiring")) {
                        classes.Add("Wiring", new List<APIPart> { part });
                        colors.Add("Wiring", Hitch_API.GetColorCode(part.partID));
                    } else {
                        classes["Wiring"].Add(part);
                    }
                }
            }

            ViewBag.classes = classes;
            ViewBag.colors = colors;

            return View();
        }

        public ActionResult Compare(String[] compare) {
            if (compare.Length > 0) {
                Response.Write("<h3 style='text-decoration:underline'>You would like to compare the following hitch IDs</h3><br />");

                foreach (string prodID in compare) {
                    Response.Write("<p>Product ID: " + prodID + "</p><br />");
                }
            } else {
                Response.Write("<h3 style='text-decoration:underline;color:red'>You did not select any hitches to compare.</h3><br />");
            }
            
            Response.End();
            return (new EmptyResult());
        }

        public void Clear() {
            HttpCookie year_cookie = Request.Cookies["vehicle_year"];
            year_cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(year_cookie);

            HttpCookie make_cookie = Request.Cookies["vehicle_make"];
            make_cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(make_cookie);

            HttpCookie model_cookie = Request.Cookies["vehicle_model"];
            model_cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(model_cookie);

            HttpCookie style_cookie = Request.Cookies["vehicle_style"];
            style_cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(style_cookie);

            HttpCookie id_cookie = Request.Cookies["vehicle_id"];
            id_cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(id_cookie);
        }
    }
}
