using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml;

namespace curtmfg.Models {
    public class StateRegion {
        public int stateID { get; set; }
        public string name { get; set; }
        public string abbr { get; set; }
        public int count { get; set; }
        public List<MapPolygon> polygons { get; set; }
    }
}