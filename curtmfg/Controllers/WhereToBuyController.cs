using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace curtmfg.Controllers
{
    public class WhereToBuyController : BaseController {
        //
        // GET: /WhereToBuy/

        public ActionResult Index() {
            string dealersearch = Request.QueryString["search"];
            ViewBag.dealersearch = dealersearch;

            if (dealersearch != null && dealersearch.Trim() != "") {
                List<DealerLocation> localdealers = DealerModel.SearchLocationsByType(dealersearch);
                int loccount = localdealers.Count;
                if (loccount == 0) {
                    // Query Google Maps API for a geolocation
                    LatLong location = new LatLong();
                    try {
                        string apiurl = "http://maps.googleapis.com/maps/api/geocode/xml?sensor=false&address=" + dealersearch;
                        XDocument xdoc = XDocument.Load(apiurl);
                        location = (from latlng in xdoc.Descendants("result").Descendants("geometry").Descendants("location")
                                   select new LatLong {
                                       latitude = Convert.ToDouble(latlng.Element("lat").Value),
                                       longitude = Convert.ToDouble(latlng.Element("lng").Value)
                                   }).First<LatLong>();
                    } catch { };
                    if (location.latitude != null && location.longitude != null) {
                        localdealers = DealerModel.SearchLocationsByLatLong(location);
                    }
                }
                ViewBag.localdealers = localdealers;
            }

            List<Customer> onlinedealers = DealerModel.GetEtailers();
            ViewBag.onlinedealers = onlinedealers;

            List<MapGraphics> localtiers = DealerModel.GetLocalDealerTypes();
            ViewBag.localtiers = localtiers;

            return View();
        }

        public string Directions(int id = 0) {
            DealerLocation location = DealerModel.GetDealerLocation(id);
            return Newtonsoft.Json.JsonConvert.SerializeObject(location);
        }

        public string getLocalDealersJSON(string latlng = "", string center = "") {
            List<DealerLocation> locations = DealerModel.GetLocalDealerLocations(latlng,center);
            return Newtonsoft.Json.JsonConvert.SerializeObject(locations);
        }

        [OutputCache(CacheProfile = "24Hours")]
        public string getLocalRegionsJSON() {
            List<StateRegion> regions = DealerModel.GetLocalRegions();
            return Newtonsoft.Json.JsonConvert.SerializeObject(regions);
        }

        public string searchLocations(string search = "") {
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<DealerLocation> locations = DealerModel.SearchLocations(search);
            return js.Serialize(locations);
        }

    }
}
