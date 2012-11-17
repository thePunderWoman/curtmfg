using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;

namespace curtmfg.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/

        protected override void Initialize(System.Web.Routing.RequestContext requestContext) {
            base.Initialize(requestContext);

            try {
                try {
                    List<curtmfg.Models.Category> parentCatSession = (List<curtmfg.Models.Category>)HttpContext.Session["parentCategories"];
                    if (parentCatSession == null) {
                        throw new Exception("No Categories");
                    }
                    ViewBag.parentcategories = parentCatSession;
                } catch {
                    List<curtmfg.Models.Category> parentcategories = curtmfg.Models.Category.GetParentCategories();
                    HttpContext.Session.Add("parentCategories", parentcategories);
                    ViewBag.parentcategories = parentcategories;
                }

                SimpleMenu mainmenu = new MenuModel().GetPrimary();
                ViewBag.mainmenu = mainmenu;

                List<SimpleMenu> sitemapmenus = new MenuModel().GetFooterSitemap();
                ViewBag.sitemapmenus = sitemapmenus;

                //List<string> years = Hitch_API.getYears();
                //ViewBag.years = years;

                // Get all the cookies
                HttpCookie vehicleYear = Request.Cookies.Get("vehicle_year");
                ViewBag.year = (vehicleYear != null && vehicleYear.Value != null) ? vehicleYear.Value.ToString() : "";

                HttpCookie vehicleMake = Request.Cookies.Get("vehicle_make");
                ViewBag.make = (vehicleMake != null && vehicleMake.Value != null) ? vehicleMake.Value.ToString() : "";

                HttpCookie vehicleModel = Request.Cookies.Get("vehicle_model");
                ViewBag.model = (vehicleModel != null && vehicleModel.Value != null) ? vehicleModel.Value.ToString() : "";

                HttpCookie vehicleStyle = Request.Cookies.Get("vehicle_style");
                ViewBag.style = (vehicleStyle != null && vehicleStyle.Value != null) ? vehicleStyle.Value.ToString() : "";

                HttpCookie vehicleID = Request.Cookies.Get("vehicle_id");
                ViewBag.vehicleID = (vehicleID != null && vehicleID.Value != null) ? vehicleID.Value.ToString() : "";

                HttpCookie curtCustomerID = Request.Cookies.Get("customerID");
                ViewBag.curtCustomerID = (curtCustomerID != null && curtCustomerID.Value != null) ? curtCustomerID.Value.ToString() : "";

                HttpCookie curtCustomerName = Request.Cookies.Get("customerName");
                ViewBag.curtCustomerName = (curtCustomerName != null && curtCustomerName.Value != null) ? curtCustomerName.Value.ToString() : "";

                HttpCookie lastCategory = Request.Cookies.Get("last_category");
                ViewBag.lastVisitedCategory = (lastCategory != null && lastCategory.Value != null) ? Convert.ToInt32(lastCategory.Value.ToString()) : 0;

            } catch (Exception e) {
                ViewBag.error = e.Message;
            }
        }

    }
}
