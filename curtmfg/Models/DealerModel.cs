using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq.Mapping;

namespace curtmfg.Models
{
    public class DealerModel {
        public static List<Customer> GetDealers() {
            CurtDevDataContext db = new CurtDevDataContext();
            List<Customer> customers = db.Customers.Join(db.DealerTypes, c => c.dealer_type, dt => dt.dealer_type, (c, dt) => new { Customer = c, DealerType = dt })
                .Where(x => x.Customer.isDummy.Equals(false)).OrderBy(x => x.DealerType.online).Select(x => x.Customer).ToList<Customer>();
            return customers;
        }

        public static DealerLocation GetDealerLocation(int locationID = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            DealerLocation location = new DealerLocation();
            try {
                location = (from cl in db.CustomerLocations
                            join c in db.Customers on cl.cust_id equals c.cust_id
                            join dt in db.DealerTypes on c.dealer_type equals dt.dealer_type
                            join dtr in db.DealerTiers on c.tier equals dtr.ID
                            where cl.locationID.Equals(locationID)
                            select new DealerLocation {
                                State = cl.State,
                                customername = c.name,
                                dealerType = dt,
                                dealerTier = dtr,
                                locationID = cl.locationID,
                                name = cl.name,
                                address = cl.address,
                                city = cl.city,
                                stateID = cl.stateID,
                                postalCode = cl.postalCode,
                                email = cl.email,
                                phone = cl.phone,
                                fax = cl.fax,
                                latitude = cl.latitude,
                                longitude = cl.longitude,
                                cust_id = cl.cust_id,
                                isprimary = cl.isprimary,
                                contact_person = cl.contact_person,
                                websiteurl = (c.eLocalURL == null) ? "" : c.eLocalURL,
                                showWebsite = c.showWebsite
                            }).FirstOrDefault<DealerLocation>();
            } catch { };
            return location;
        }

        public static List<MapGraphics> GetLocalDealerTypes() {
            CurtDevDataContext db = new CurtDevDataContext();
            List<MapGraphics> graphics = (from m in db.MapIcons
                                          join dt in db.DealerTypes on m.dealer_type equals dt.dealer_type
                                          join dtr in db.DealerTiers on m.tier equals dtr.ID
                                          where dt.show.Equals(true)
                                          orderby dtr.sort descending
                                          select new MapGraphics {
                                              ID = m.ID,
                                              mapicon1 = m.mapicon1,
                                              mapiconshadow = m.mapiconshadow,
                                              dealer_type = m.dealer_type,
                                              tier = m.tier,
                                              dealertier = dtr,
                                              dealertype = dt
                                          }).ToList<MapGraphics>();
            return graphics;
        }

        public static List<DealerTier> GetLocalDealerTiers() {
            CurtDevDataContext db = new CurtDevDataContext();
            List<DealerTier> tiers = (from dtr in db.DealerTiers
                                      join c in db.Customers on dtr.ID equals c.tier
                                      join dt in db.DealerTypes on c.dealer_type equals dt.dealer_type
                                      where dt.online.Equals(false) && dt.show.Equals(true)
                                      select dtr).Distinct().OrderByDescending(x => x.sort).ToList<DealerTier>();
            return tiers;
        }

        public static List<Customer> GetEtailers() {
            CurtDevDataContext db = new CurtDevDataContext();
            List<Customer> customers = (from c in db.Customers
                                        join dt in db.DealerTypes on c.dealer_type equals dt.dealer_type
                                        join dtr in db.DealerTiers on c.tier equals dtr.ID
                                        where dt.online.Equals(true) && c.isDummy.Equals(false)
                                        orderby dtr.sort
                                        select c).ToList<Customer>();

            return customers;
        }

        public static List<Customer> GetWhereToBuyDealers() {
            CurtDevDataContext db = new CurtDevDataContext();
            List<Customer> dealers = (from c in db.Customers
                                        join dty in db.DealerTypes on c.dealer_type equals dty.dealer_type
                                        join dtr in db.DealerTiers on c.tier equals dtr.ID
                                        where dty.dealer_type.Equals(1) && dtr.ID.Equals(4) && c.isDummy == false && c.searchURL != "" && c.searchURL != null
                                        select c).ToList<Customer>();
            return dealers;
        }

        public static List<Customer> GetLocalDealers() {
            CurtDevDataContext db = new CurtDevDataContext();
            List<Customer> customers = (from c in db.Customers
                                          join dt in db.DealerTypes on c.dealer_type equals dt.dealer_type
                                          join dtr in db.DealerTiers on c.tier equals dtr.ID
                                          where dt.dealer_type.Equals(2) && c.isDummy == false
                                          orderby dtr.sort
                                          select c).ToList<Customer>();
            return customers;
        }
        
        public static List<DealerLocation> GetLocalDealerLocations(string latlng = "", string center = "") {
            double earth = 3963.1676; // radius of Earth in miles
            double swlat = -90;
            double swlong = -180;
            double swlong2 = -180;
            double nelat = 90;
            double nelong = 180;
            double nelong2 = 180;
            double clat = 44.79300;
            double clong = -91.41048;
            string[] latlngs;
            string[] centerlatlngs;
            if (center != "") {
                centerlatlngs = center.Split(',');
                clat = Convert.ToDouble(centerlatlngs[0]);
                clong = Convert.ToDouble(centerlatlngs[1]);
            }
            if (latlng != "") {
                latlngs = latlng.Split(',');
                swlat = Convert.ToDouble(latlngs[0]);
                swlong = Convert.ToDouble(latlngs[1]);
                nelat = Convert.ToDouble(latlngs[2]);
                nelong = Convert.ToDouble(latlngs[3]);
                swlong2 = swlong;
                nelong2 = nelong;
            }
            if(swlong > nelong) {
                swlong = -180;
                nelong2 = 180;
            }
            double distancea = getViewPortWidth(swlat, swlong, clat, clong);
            double distanceb = getViewPortWidth(nelat, nelong2, clat, clong);
            double viewdistance = (distancea > distanceb) ? distancea : distanceb;
            double texswlat = 26.08;
            double texswlong = -106.64;
            double texnelat = 36.90;
            double texnelong = -93.50;
            CurtDevDataContext db = new CurtDevDataContext();
            List<DealerLocation> locationlist = (from cl in db.CustomerLocations
                                              join c in db.Customers on cl.cust_id equals c.cust_id
                                              join dt in db.DealerTypes on c.dealer_type equals dt.dealer_type
                                              join dtr in db.DealerTiers on c.tier equals dtr.ID
                                              where dt.online == false && c.isDummy == false && dt.show == true &&
                                              ( earth * (
                                                            2 * Math.Atan2(
                                                                Math.Sqrt((Math.Sin(((cl.latitude - clat) * (Math.PI / 180)) / 2) * Math.Sin(((cl.latitude - clat) * (Math.PI / 180)) / 2)) + ((Math.Sin(((cl.longitude - clong) * (Math.PI / 180)) / 2)) * (Math.Sin(((cl.longitude - clong) * (Math.PI / 180)) / 2))) * Math.Cos(clat * (Math.PI / 180)) * Math.Cos(cl.latitude * (Math.PI / 180))),
                                                                Math.Sqrt(1 - ((Math.Sin(((cl.latitude - clat) * (Math.PI / 180)) / 2) * Math.Sin(((cl.latitude - clat) * (Math.PI / 180)) / 2)) + ((Math.Sin(((cl.longitude - clong) * (Math.PI / 180)) / 2)) * (Math.Sin(((cl.longitude - clong) * (Math.PI / 180)) / 2))) * Math.Cos(clat * (Math.PI / 180)) * Math.Cos(cl.latitude * (Math.PI / 180))))
                                                            )
                                                        ) < viewdistance) ||
                                              ((c.name == "Discount Hitch & Truck Accessories") && ((swlat > texswlat && swlong2 > texswlong && swlat < texnelat && swlong2 < texnelong) || (nelat < texnelat && nelong < texnelong && nelat > texswlat && nelong > texswlong) ) )
                                              select new DealerLocation {
                                                  distance = earth * (
                                                            2 * Math.Atan2(
                                                                Math.Sqrt((Math.Sin(((cl.latitude - clat) * (Math.PI / 180)) / 2) * Math.Sin(((cl.latitude - clat) * (Math.PI / 180)) / 2)) + ((Math.Sin(((cl.longitude - clong) * (Math.PI / 180)) / 2)) * (Math.Sin(((cl.longitude - clong) * (Math.PI / 180)) / 2))) * Math.Cos(clat * (Math.PI / 180)) * Math.Cos(cl.latitude * (Math.PI / 180))),
                                                                Math.Sqrt(1 - ((Math.Sin(((cl.latitude - clat) * (Math.PI / 180)) / 2) * Math.Sin(((cl.latitude - clat) * (Math.PI / 180)) / 2)) + ((Math.Sin(((cl.longitude - clong) * (Math.PI / 180)) / 2)) * (Math.Sin(((cl.longitude - clong) * (Math.PI / 180)) / 2))) * Math.Cos(clat * (Math.PI / 180)) * Math.Cos(cl.latitude * (Math.PI / 180))))
                                                            )
                                                        ),
                                                  State = cl.State,
                                                  customername = c.name,
                                                  dealerType = dt,
                                                  dealerTier = dtr,
                                                  locationID = cl.locationID,
                                                  name = cl.name,
                                                  address = cl.address,
                                                  city = cl.city,
                                                  stateID = cl.stateID,
                                                  postalCode = cl.postalCode,
                                                  email = cl.email,
                                                  phone = cl.phone,
                                                  fax = cl.fax,
                                                  latitude = cl.latitude,
                                                  longitude = cl.longitude,
                                                  cust_id = cl.cust_id,
                                                  isprimary = cl.isprimary,
                                                  contact_person = cl.contact_person,
                                                  websiteurl = (c.eLocalURL == null) ? "" : c.eLocalURL,
                                                  showWebsite = c.showWebsite
                                              }).OrderByDescending(x => x.dealerTier.sort).ThenBy(x => x.distance).ToList<DealerLocation>();
            IQueryable<DealerLocation> locationqry = locationlist.AsQueryable();

            List<DealerLocation> locations = (from l in locationqry
                                              where ((Convert.ToDouble(l.latitude) >= swlat && Convert.ToDouble(l.latitude) <= nelat) &&
                                              (Convert.ToDouble(l.longitude) >= swlong && Convert.ToDouble(l.longitude) <= nelong) ||
                                              (Convert.ToDouble(l.longitude) >= swlong2 && Convert.ToDouble(l.longitude) <= nelong2)) ||
                                              (l.customername == "Discount Hitch & Truck Accessories")
                                              select l).OrderBy(x => x.dealerTier.sort).ThenBy(x => x.distance).ToList<DealerLocation>();

            return locations;
        }

        public static List<StateRegion> GetLocalRegions() {
            CurtDevDataContext db = new CurtDevDataContext();
            List<StateRegion> regions = new List<StateRegion>();

            regions = (from s in db.States
                       where (from c in db.Customers
                           join cl in db.CustomerLocations on c.cust_id equals cl.cust_id
                           where c.DealerType.online.Equals(false) && cl.stateID.Equals(s.stateID)
                           select cl.locationID).Count() > 0
                       select new StateRegion {
                           stateID = s.stateID,
                           name = s.state1,
                           abbr = s.abbr,
                           count = (from c in db.Customers
                                    join cl in db.CustomerLocations on c.cust_id equals cl.cust_id
                                    where c.DealerType.online.Equals(false) && cl.stateID.Equals(s.stateID)
                                    select cl.locationID).Count(),
                           polygons = (from mp in db.MapPolygons
                                       where mp.stateID.Equals(s.stateID)
                                       select mp).ToList<MapPolygon>()
                       }).ToList<StateRegion>();

            return regions;
        }

        private static double getViewPortWidth(double lat1, double lon1,double lat2,double lon2) {
            double earth = 3963.1676; // radius of Earth in miles
            double dlat = (lat2 - lat1) * (Math.PI / 180);
            double dlon = (lon2 - lon1) * (Math.PI / 180);
            lat1 = lat1 * (Math.PI / 180);
            lat2 = lat2 * (Math.PI / 180);

            double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + ((Math.Sin(dlon / 2)) * (Math.Sin(dlon / 2))) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = earth * c;
            return distance;
        }

        public static List<DealerLocation> SearchLocations(string search = "") {
            CurtDevDataContext db = new CurtDevDataContext();
            List<DealerLocation> locations = (from cl in db.CustomerLocations
                                              join c in db.Customers on cl.cust_id equals c.cust_id
                                              join dt in db.DealerTypes on c.dealer_type equals dt.dealer_type
                                              join dtr in db.DealerTiers on c.tier equals dtr.ID
                                              where (dt.dealer_type.Equals(2) || dt.dealer_type.Equals(3)) && c.isDummy == false && dt.show == true && 
                                              (cl.name.ToLower().Contains(search.ToLower().Trim()) || c.name.ToLower().Contains(search.ToLower().Trim()))
                                              orderby cl.name
                                              select new DealerLocation {
                                                  State = cl.State,
                                                  dealerType = dt,
                                                  dealerTier = dtr,
                                                  locationID = cl.locationID,
                                                  name = cl.name,
                                                  address = cl.address,
                                                  city = cl.city,
                                                  stateID = cl.stateID,
                                                  postalCode = cl.postalCode,
                                                  email = cl.email,
                                                  phone = cl.phone,
                                                  fax = cl.fax,
                                                  latitude = cl.latitude,
                                                  longitude = cl.longitude,
                                                  cust_id = cl.cust_id,
                                                  isprimary = cl.isprimary,
                                                  contact_person = cl.contact_person,
                                                  websiteurl = (c.eLocalURL == null) ? "" : c.eLocalURL,
                                                  showWebsite = c.showWebsite
                                              }).ToList<DealerLocation>();
            return locations;
        }

        public static List<DealerLocation> SearchLocationsByType(string search = "") {
            CurtDevDataContext db = new CurtDevDataContext();
            List<DealerLocation> locations = (from cl in db.CustomerLocations
                                          join c in db.Customers on cl.cust_id equals c.cust_id
                                          join dt in db.DealerTypes on c.dealer_type equals dt.dealer_type
                                          join dtr in db.DealerTiers on c.tier equals dtr.ID
                                          where dt.online == false && c.isDummy == false && dt.show == true &&
                                          (cl.name.ToLower().Contains(search.ToLower().Trim()) || c.name.ToLower().Contains(search.ToLower().Trim()))
                                          orderby cl.name
                                          select new DealerLocation {
                                              State = cl.State,
                                              dealerType = dt,
                                              dealerTier = dtr,
                                              locationID = cl.locationID,
                                              name = cl.name,
                                              address = cl.address,
                                              city = cl.city,
                                              stateID = cl.stateID,
                                              postalCode = cl.postalCode,
                                              email = cl.email,
                                              phone = cl.phone,
                                              fax = cl.fax,
                                              latitude = cl.latitude,
                                              longitude = cl.longitude,
                                              cust_id = cl.cust_id,
                                              isprimary = cl.isprimary,
                                              contact_person = cl.contact_person,
                                              websiteurl = (c.eLocalURL == null) ? "" : c.eLocalURL,
                                              showWebsite = c.showWebsite
                                          }).ToList<DealerLocation>();
            return locations;
        }

        public static List<DealerLocation> SearchLocationsByLatLong(LatLong location) {
            // Use Haversine formula to find nearest dealers within 100 miles
            CurtDevDataContext db = new CurtDevDataContext();
            double earth = 3963.1676; // radius of Earth in miles
            double lat1 = location.latitude;
            double lon1 = location.longitude;
            List<DealerLocation> locations = (from cl in db.CustomerLocations
                                              join c in db.Customers on cl.cust_id equals c.cust_id
                                              join dt in db.DealerTypes on c.dealer_type equals dt.dealer_type
                                              join dtr in db.DealerTiers on c.tier equals dtr.ID
                                              where dt.online == false && dt.show == true && c.isDummy == false &&
                                              (earth * (
                                                2 * Math.Atan2(
                                                        Math.Sqrt((Math.Sin(((cl.latitude - lat1) * (Math.PI / 180)) / 2) * Math.Sin(((cl.latitude - lat1) * (Math.PI / 180)) / 2)) + ((Math.Sin(((cl.longitude - lon1) * (Math.PI / 180)) / 2)) * (Math.Sin(((cl.longitude - lon1) * (Math.PI / 180)) / 2))) * Math.Cos(location.latitudeRadians) * Math.Cos(cl.latitude * (Math.PI / 180))),
                                                        Math.Sqrt(1 - ((Math.Sin(((cl.latitude - lat1) * (Math.PI / 180)) / 2) * Math.Sin(((cl.latitude - lat1) * (Math.PI / 180)) / 2)) + ((Math.Sin(((cl.longitude - lon1) * (Math.PI / 180)) / 2)) * (Math.Sin(((cl.longitude - lon1) * (Math.PI / 180)) / 2))) * Math.Cos(location.latitudeRadians) * Math.Cos(cl.latitude * (Math.PI / 180))))
                                                    )
                                              ) < 100.0)
                                              select new DealerLocation {
                                                  distance = earth * (
                                                                2 * Math.Atan2(
                                                                    Math.Sqrt((Math.Sin(((cl.latitude - lat1) * (Math.PI / 180)) / 2) * Math.Sin(((cl.latitude - lat1) * (Math.PI / 180)) / 2)) + ((Math.Sin(((cl.longitude - lon1) * (Math.PI / 180)) / 2)) * (Math.Sin(((cl.longitude - lon1) * (Math.PI / 180)) / 2))) * Math.Cos(location.latitudeRadians) * Math.Cos(cl.latitude * (Math.PI / 180))),
                                                                    Math.Sqrt(1 - ((Math.Sin(((cl.latitude - lat1) * (Math.PI / 180)) / 2) * Math.Sin(((cl.latitude - lat1) * (Math.PI / 180)) / 2)) + ((Math.Sin(((cl.longitude - lon1) * (Math.PI / 180)) / 2)) * (Math.Sin(((cl.longitude - lon1) * (Math.PI / 180)) / 2))) * Math.Cos(location.latitudeRadians) * Math.Cos(cl.latitude * (Math.PI / 180))))
                                                                )
                                                             ),
                                                  State = cl.State,
                                                  dealerType = dt,
                                                  dealerTier = dtr,
                                                  locationID = cl.locationID,
                                                  name = cl.name,
                                                  address = cl.address,
                                                  city = cl.city,
                                                  stateID = cl.stateID,
                                                  postalCode = cl.postalCode,
                                                  email = cl.email,
                                                  phone = cl.phone,
                                                  fax = cl.fax,
                                                  latitude = cl.latitude,
                                                  longitude = cl.longitude,
                                                  cust_id = cl.cust_id,
                                                  isprimary = cl.isprimary,
                                                  contact_person = cl.contact_person,
                                                  websiteurl = (c.eLocalURL == null) ? "" : c.eLocalURL,
                                                  showWebsite = c.showWebsite
                                              }).OrderBy(x => x.distance).ToList<DealerLocation>();
            return locations;
        }

        public static List<FullCountry> GetCountries() {
            CurtDevDataContext db = new CurtDevDataContext();
            List<FullCountry> countries = new List<FullCountry>();

            countries = (from c in db.Countries
                         select new FullCountry {
                             countryID = c.countryID,
                             name = c.name,
                             abbr = c.abbr,
                             states = db.States.Where(x => x.countryID == c.countryID).OrderBy(x => x.state1).ToList<State>()
                         }).ToList<FullCountry>();

            return countries;

        }

    }

    public class FullType : DealerType {
        public List<FullDealer> dealers { get; set; }
    }

    public class TypeWithLocations : DealerType {
        public DealerTier dealer_teir { get; set; }
        public List<DealerLocation> locations { get; set; }
    }

    public class FullDealer : Customer {
        public int randomizer { get; set; }
    }

    public class MapGraphics : MapIcon {
        public DealerTier dealertier { get; set; }
        public DealerType dealertype { get; set; }
    }

    public class DealerLocation : CustomerLocation {
        public DealerType dealerType { get; set; }
        public DealerTier dealerTier { get; set; }
        public string customername { get; set; }
        public string websiteurl { get; set; }
        public bool showWebsite { get; set; }
        public double distance { get; set; }
    }
    public class FullCountry : Country {
        public List<State> states { get; set; }
    }
    public class LatLong {
        private double _latitude { get; set; }
        private double _longitude { get; set; }
        public double latitude {
            get {
                return this._latitude;
            }
            set {
                if (value != null && value != this._latitude) {
                    this._latitude = value;
                }
            }
        }
        public double longitude {
            get {
                return this._longitude;
            }
            set {
                if (value != null && value != this._longitude) {
                    this._longitude = value;
                }
            }
        }
        public double latitudeRadians {
            get {
                return (this._latitude * (Math.PI / 180));
            }
        }
        public double longitudeRadians {
            get {
                return (this._longitude * (Math.PI / 180));
            }
        }

    }
}
