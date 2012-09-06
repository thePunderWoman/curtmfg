using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Security.Cryptography;

namespace curtmfg.Models {
    public class APIModels {

        public static string MakeJSONRequest(string url = "") {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream receiveStream = resp.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            return readStream.ReadToEnd();
        }

        public static XDocument MakeXMLRequest(string url = "") {
            XDocument doc = new XDocument(url);
            return doc;
        }

        public static Boolean FileExists(string path = "") {
            if (path == null || path.Trim().Length == 0) {
                return false;
            }
            WebRequest req = WebRequest.Create(path);
            req.Method = "HEAD";

            try {
                WebResponse resp = req.GetResponse();
                return true;
            } catch {
                return false;
            }
        }

        public static List<T> Shuffle<T>(IList<T> list) {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1) {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list.ToList();
        }

        public static List<State> GetStates() {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                List<State> states = new List<State>();
                states = db.States.OrderBy(x => x.abbr).ToList<State>();
                return states;
            } catch {
                return new List<State>();
            }
        }

    }

    public class APIPart {

        private List<APIAttribute> _content = new List<APIAttribute>();
        private List<APIAttribute> _attributes = new List<APIAttribute>();
        private List<APIAttribute> _vehicleattributes = new List<APIAttribute>();
        private List<APIReview> _reviews = new List<APIReview>();
        private List<PartImage> _images = new List<PartImage>();
        private List<APIVideo> _videos = new List<APIVideo>();
        private APIImages _apiimages = new APIImages();
        private int _partID = 0;
        private int _status = 0;
        private string _dateModified = "";
        private string _dateAdded = "";
        private string _shortDesc = "";
        private string _oldPartNumber = "";
        private string _listPrice = "";
        private string _pClass = "";
        private int _relatedCount = 0;
        private double? _averageReview = 0;

        public int partID {
            get {
                return this._partID;
            }
            set {
                if (value != null && value != this._partID) {
                    this._partID = value;
                }
            }
        }
        public int status {
            get {
                return this._status;
            }
            set {
                if (value != null && value != this._status) {
                    this._status = value;
                }
            }
        }
        public string dateModified {
            get {
                return this._dateModified;
            }
            set {
                if (value != null && value != this._dateModified) {
                    this._dateModified = value;
                }
            }
        }
        public string dateAdded {
            get {
                return this._dateAdded;
            }
            set {
                if (value != null && value != this._dateAdded) {
                    this._dateAdded = value;
                }
            }
        }
        public string shortDesc {
            get {
                return this._shortDesc;
            }
            set {
                if (value != null && value != this._shortDesc) {
                    this._shortDesc = value;
                }
            }
        }
        public string oldPartNumber {
            get {
                return this._oldPartNumber;
            }
            set {
                if (value != null && value != this._oldPartNumber) {
                    this._oldPartNumber = value;
                }
            }
        }
        public string listPrice {
            get {
                return this._listPrice;
            }
            set {
                if (value != null && value != this._listPrice) {
                    this._listPrice = value;
                }
            }
        }
        public List<APIAttribute> attributes {
            get {
                return this._attributes;
            }
            set {
                if (value != null && value != this._attributes) {
                    this._attributes = value;
                }
            }
        }
        public List<APIAttribute> vehicleAttributes {
            get {
                return this._vehicleattributes;
            }
            set {
                if (value != null && value != this._vehicleattributes) {
                    this._vehicleattributes = value;
                }
            }
        }
        public List<APIAttribute> content {
            get {
                return this._content;
            }
            set {
                if (value != null && value != this._content) {
                    this._content = value;
                }
            }
        }
        public List<APIReview> reviews {
            get {
                return this._reviews;
            }
            set {
                if (value != null && value != this._reviews) {
                    this._reviews = value;
                }
            }
        }

        public List<PartImage> images {
            get {
                return this._images;
            }
            set {
                if (value != null && value != this._images) {
                    this._images = value;
                }
            }
        }

        public APIImages apiimages {
            get {
                return this.BindPartImages();
            }
        }

        public List<APIVideo> videos {
            get {
                return this._videos;
            }
            set {
                if (value != null && value != this._videos) {
                    this._videos = value;
                }
            }
        }
        
        public string pClass {
            get {
                return this._pClass;
            }
            set {
                if (value != null && value != this._pClass) {
                    this._pClass = value;
                }
            }
        }

        public int relatedCount {
            get {
                return this._relatedCount;
            }
            set {
                if (value != null && value != this._relatedCount) {
                    this._relatedCount = value;
                }
            }
        }

        public double? averageReview {
            get {
                return this._averageReview;
            }
            set {
                if (value != null && value != this._averageReview) {
                    this._averageReview = value;
                }
            }
        }

        public int installTime { get; set; }
        public int? vehicleID { get; set; }
        public int? priceCode { get; set; }
        public string drilling { get; set; }
        public string exposed { get; set; }

        private APIImages BindPartImages() {
            List<string> chars = this.images.Select(x => x.sort).Distinct().ToList();
            List<PartImageIndex> indexes = new List<PartImageIndex>();
            foreach(string sort in chars) {
                PartImageIndex i = new PartImageIndex {
                    name = sort,
                    images = (from img in this.images
                              where img.sort == sort
                              orderby img.sort
                              select img).ToDictionary(item => item.size)
                };
                indexes.Add(i);
            }
            APIImages imgs = new APIImages {
                images = indexes.ToDictionary(x => x.name)
            };
            return imgs;
        }
    }

    public class APIImages {
        public Dictionary<string, PartImageIndex> images { get; set; }

        public PartImageIndex getIndex(string name = "") {
            PartImageIndex pii = new PartImageIndex();
            if(this.images.ContainsKey(name)) {
                pii = this.images[name];
            } else {
                try {
                    pii = this.images[this.images.Keys.First()];
                } catch { }
            }
            return pii;
        }
    }

    public class PartImageIndex {
        public string name { get; set; }
        public Dictionary<string, PartImage> images { get; set; }

        public PartImage getImage(string size = "") {
            if (this.images != null && this.images.ContainsKey(size)) {
                return this.images[size];
            } else {
                try {
                    return this.images[this.images.Keys.First()];
                } catch { };
            };
            return new PartImage();
        }
    }

    public class PartImage {
        public string size { get; set; }
        public string path { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string sort { get; set; }

        public string getPath() {
            if (this.path == null || this.path == "") {
                return "/Content/img/noimage.png";
            }
            return this.path;
        }
    }

    public class APIVideo {
        public int videoID { get; set; }
        public bool isPrimary { get; set; }
        public int typeID { get; set; }
        public string type { get; set; }
        public string typeicon { get; set; }
        public string youTubeVideoID { get; set; }
    }

    public class APIAttribute {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class APIColorCode {
        public int codeID { get; set; }
        public string code { get; set; }
        public string font { get; set; }
    }

    public class APIReview {
        public int reviewID { get; set; }
        public int partID { get; set; }
        public int rating { get; set; }
        public string subject { get; set; }
        public string review_text { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public DateTime createdDate { get; set; }
    }

    public class APIContent {
        public bool isHTML { get; set; }
        public string type { get; set; }
        public string content { get; set; }
    }

    public class CityState_Location {
        public int locationID { get; set; }
        public string city { get; set; }
        public string state { get; set; }
    }

    public class LocationWithState {
        public int locationID { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public int stateID { get; set; }
        public string state { get; set; }
        public int zip { get; set; }
        public int isPrimary { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        public string places_id { get; set; }
        public string places_reference { get; set; }
        public string places_status { get; set; }
    }

    /// <summary>
    /// This object will hold strings containing the actual values of the year, make, model, and style for a vehicle.
    /// </summary>
    public class FullVehicle {
        public int vehicleID { get; set; }
        public int yearID { get; set; }
        public int makeID { get; set; }
        public int modelID { get; set; }
        public int styleID { get; set; }
        public int aaiaID { get; set; }
        public double year { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string style { get; set; }
        public int installTime { get; set; }
        public string drilling { get; set; }
        public string exposed { get; set; }
        public List<APIAttribute> attributes { get; set; }
    }

    /// <summary>
    /// This object will hold the part or category data from the API search method.
    /// </summary>
    public class APISearchResult {
        public int? categoryID { get; set; }
        public int? partID { get; set; }
        public string type { get; set; }
        public bool isHitch { get; set; }
        public string short_desc { get; set; }
        public string description { get; set; }
        public decimal relevance { get; set; }
    }

    public class APITowable {
        public int trailerID { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public APIFileInfo imageinfo { get; set; }
        public string hitchClass { get; set; }
        public int TW { get; set; }
        public int GTW { get; set; }
        public string shortDesc { get; set; }
        public string message { get; set; }
    }

    public class APIFileInfo {
        public int fileID { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string name { get; set; }
        public string created { get; set; }
        public string path { get; set; }
        public string extension { get; set; }
    }

    public class APICategoryParts {
        public int total { get; set; }
        public int page { get; set; }
        public int perpage { get; set; }
        public List<APIPart> parts { get; set; }
    }
}