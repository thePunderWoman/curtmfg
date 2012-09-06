using System;
using System.Data;
using System.Data.Odbc;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.IO;
using System.Collections;
using System.Net;
using curtmfg.Models;
using System.Text;
using System.Xml;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace curtmfg.Models {
    public class Hitch_API {
        private static string APIURL = ConfigurationManager.AppSettings["APIURL"];

        public static List<string> getYears(string mount = "") {
            try {
                string year_json = "";
                WebClient wc = new WebClient();
                wc.Proxy = null;

                year_json = wc.DownloadString(getAPIPath() + "getyear?dataType=JSON");
                List<string> years = JsonConvert.DeserializeObject<List<string>>(year_json);
                return years;
            } catch (Exception) {
                return new List<string>();
            }
        }

        public static List<string> getMakes(string mount = "", string year = "") {
            try {
                string make_json = "";
                WebClient wc = new WebClient();
                wc.Proxy = null;

                make_json = wc.DownloadString(getAPIPath() + "GetMake?mount=" + mount + "&year=" + year + "dataType=JSON");
                List<string> makes = JsonConvert.DeserializeObject<List<string>>(make_json);
                return makes;
            } catch {
                return new List<string>();
            }
        }

        public static List<string> getModels(string mount = "", string year = "", string make = "") {
            try {
                string model_json = "";
                WebClient wc = new WebClient();
                wc.Proxy = null;

                model_json = wc.DownloadString(getAPIPath() + "/GetModel?mount=" + mount + "&year=" + year + "&make=" + make + "dataType=JSON");
                List<string> models = JsonConvert.DeserializeObject<List<string>>(model_json);
                return models;
            } catch {
                return new List<string>();
            }
        }

        public static List<string> getStyles(string mount = "", string year = "", string make = "", string model = "") {
            try {
                string style_json = "";
                WebClient wc = new WebClient();
                wc.Proxy = null;

                style_json = wc.DownloadString(getAPIPath() + "/GetStyle?mount=" + mount + "&year=" + year + "&make=" + make + "&model=" + model + "dataType=JSON");
                List<string> styles = JsonConvert.DeserializeObject<List<string>>(style_json);
                return styles;
            } catch {
                return new List<string>();
            }
        }
        
        public static FullVehicle getVehicle(string year, string make, string model, string style) {
            try {
                string vehicle_json = "";
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath() + "/GetVehicle?year=" + year;
                url += "&make=" + make + "&model=" + model + "&style=" + HttpUtility.UrlEncode(style);
                url += "&dataType=JSON";
                vehicle_json = wc.DownloadString(url);

                FullVehicle vehicle = JsonConvert.DeserializeObject<List<FullVehicle>>(vehicle_json).FirstOrDefault<FullVehicle>();
                return vehicle;
            } catch {
                return new FullVehicle();
            };
        }

        public static List<FullVehicle> getVehiclesByPart(int partID) {
            try {
                string vehicles_json = "";
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath() + "/GetPartVehicles?partID=" + partID;
                url += "&dataType=JSON";
                vehicles_json = wc.DownloadString(url);

                List<FullVehicle> vehicles = JsonConvert.DeserializeObject<List<FullVehicle>>(vehicles_json);
                return vehicles;
            } catch {
                return new List<FullVehicle>();
            };
        }
        
        public static List<APIPart> find(string year, string make, string model, string style) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath() + "/GetParts?year=" + HttpUtility.UrlEncode(year);
                url += "&make=" + HttpUtility.UrlEncode(make) + "&model=" + HttpUtility.UrlEncode(model) + "&style=" + HttpUtility.UrlEncode(style);
                url += "&dataType=JSON";

                string part_json = wc.DownloadString(url);
                List<APIPart> parts = JsonConvert.DeserializeObject<List<APIPart>>(part_json).OrderBy(x => x.pClass).OrderBy(x => x.partID).ToList<APIPart>();
                return parts;
            } catch {
                return new List<APIPart>();
            };
        }

        public static APIPart getPart(int partID, int vehicleID = 0) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath() + "/GetPart?partID=" + partID;
                url += "&vehicleID=" + vehicleID;
                url += "&dataType=JSON";

                string part_json = wc.DownloadString(url);
                APIPart part = JsonConvert.DeserializeObject<APIPart>(part_json);
                return part;
            } catch {
                return new APIPart();
            };
        }

        public static APIContent getInstallSheet(int partID) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string apiurl = getAPIPath() + "/GetInstallSheet?partID=" + partID + "&dataType=JSON";

                string jsonresp = wc.DownloadString(apiurl);
                APIContent installsheet = JsonConvert.DeserializeObject<APIContent>(jsonresp);
                return installsheet;
            } catch {
                return new APIContent();
            };
        }
        
        public static List<Category> GetPartBreadcrumbs(int partID = 0, int catId = 0) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = curtmfg.Models.Hitch_API.getAPIPath();
                url += "/GetPartBreadCrumbs?partID=" + partID;
                url += "&catId=" + catId + "&dataType=JSON";

                List<Category> cats = new List<Category>();

                string cat_json = wc.DownloadString(url);
                cats = JsonConvert.DeserializeObject<List<Category>>(cat_json);

                return cats;
            } catch (Exception) {
                return new List<Category>();
            }
        }

        public static List<Category> GetPartCategories(int partID = 0) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = curtmfg.Models.Hitch_API.getAPIPath();
                url += "/GetPartCategories?partID=" + partID;
                url += "&dataType=JSON";

                List<Category> cats = new List<Category>();

                string cat_json = wc.DownloadString(url);
                cats = JsonConvert.DeserializeObject<List<Category>>(cat_json);

                return cats;
            } catch (Exception) {
                return new List<Category>();
            }
        }

        public static List<APIPart> getRelatedParts(int partID) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath() + "/GetRelatedParts?partID=" + partID + "&dataType=JSON";
                string parts_json = wc.DownloadString(url);
                List<APIPart> parts = JsonConvert.DeserializeObject<List<APIPart>>(parts_json);
                return parts;
            } catch {
                return new List<APIPart>();
            };
        }

        public static List<APIPart> getLatestParts(int count = 6) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath() + "/GetLatestParts?count=" + count + "&dataType=JSON";
                string part_json = wc.DownloadString(url);
                List<APIPart> parts = JsonConvert.DeserializeObject<List<APIPart>>(part_json);
                return parts;
            } catch {
                return new List<APIPart>();
            };
        }
        
        public static List<APIPart> getConnectors(int vehicleID = 0) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath() + "/GetConnector?vehicleID=" + vehicleID + "&dataType=JSON";
                string part_json = wc.DownloadString(url);
                List<APIPart> parts = JsonConvert.DeserializeObject<List<APIPart>>(part_json);
                return parts;
            } catch {
                return new List<APIPart>();
            };
        }

        public static string submitReview(int partID, int rating = 0, string name = "", string email = "", string subject = "", string review_text = "") {
            string postdata = "partID=" + partID +
                                "&cust_id=1" +
                                "&name=" + HttpUtility.UrlEncode(name) +
                                "&subject=" + HttpUtility.UrlEncode(subject) +
                                "&rating=" + rating +
                                "&review_text=" + HttpUtility.UrlEncode(review_text) +
                                "&email=" + email;

            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIURL + "/SubmitReview");
                request.Method = "POST";
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] postbytes = encoding.GetBytes(postdata);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postdata.Length;
                Stream postStream = request.GetRequestStream();
                postStream.Write(postbytes, 0, postbytes.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string responsetext = reader.ReadToEnd();
                postStream.Close();
                responseStream.Close();
                return responsetext;
            } catch {
                return "";
            }
        }

        public static List<APIPart> PowerSearch(string term = "") {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath() + "/PowerSearch?search_term=" + term + "&dataType=JSON";
                string part_json = wc.DownloadString(url);
                List<APIPart> parts = JsonConvert.DeserializeObject<List<APIPart>>(part_json);
                return parts;
            } catch {
                return new List<APIPart>();
            };
        }

        public static List<Category> GetLifestyles() {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = curtmfg.Models.Hitch_API.getAPIPath();
                url += "/GetLifestyles?dataType=JSON";

                return JsonConvert.DeserializeObject<List<Category>>(wc.DownloadString(url));
            } catch (Exception) {
                return new List<Category>();
            }
        }

        public static Category GetLifestyle(int lifestyleID = 0) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;

                string url = getAPIPath();
                url += "/GetLifeStyle?lifestyleID=" + lifestyleID;
                url += "&dataType=JSON";

                return JsonConvert.DeserializeObject<Category>(wc.DownloadString(url));
            } catch (Exception) {
                return new Category();
            }
        }

        public static APIFileInfo getImageInfo(string path = "") {
            APIFileInfo fi = new APIFileInfo();
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;
                
                string url = getAPIPath() + "/GetFileInfo?path=" + path;
                MemoryStream ms = new MemoryStream(wc.DownloadData(url));
                XmlTextReader rdr = new XmlTextReader(ms);
                XDocument xml = XDocument.Load(rdr);
                fi = (from f in xml.Descendants("File")
                      select new APIFileInfo {
                          fileID = Convert.ToInt32(f.Attribute("fileID").Value),
                          height = Convert.ToInt32(f.Attribute("height").Value),
                          width = Convert.ToInt32(f.Attribute("width").Value),
                          name = f.Attribute("name").Value,
                          created = f.Attribute("createdDate").Value,
                          path = f.Attribute("path").Value,
                          extension = f.Attribute("extension").Value
                      }).First();
            } catch { };
            return fi;
        }

        public static APIColorCode GetColorCode(int p) {
            try {
                WebClient wc = new WebClient();
                wc.Proxy = null;
                string url = getAPIPath() + "/GetColor?partID=" + p;

                APIColorCode code = JsonConvert.DeserializeObject<APIColorCode>(wc.DownloadString(url));
                return code;
            } catch (Exception) {
                return new APIColorCode();
            }
        }

        public static string getAPIPath() {
            if (isSecure()) {
                return APIURL.Replace("http:", "https:");
            }
            return APIURL;
        }
        private static bool isSecure() {
            return HttpContext.Current.Request.IsSecureConnection;
        }

    }

    public class KeyValueWithKids {
        public string key { get; set; }
        public string value { get; set; }
        public List<KeyValueWithKids> attributes { get; set; }
    }
}