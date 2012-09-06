using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace curtmfg.Models {
    public class SearchModel {
        public static List<APIPart> Search(string term = "") {
            List<APIPart> parts = Hitch_API.PowerSearch(term);
            return parts;
        }
    }

    public class SearchResult {
        public string link { get; set; }
        public string title { get; set; }
        public string long_description { get; set; }
    }
}