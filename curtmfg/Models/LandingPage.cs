using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace curtmfg {
    partial class LandingPage {
        public LandingPage Get(int id) {
            CurtDevDataContext db = new CurtDevDataContext();
            LandingPage landingPage = new LandingPage();
            try {
                landingPage = db.LandingPages.Where(x => x.id.Equals(id) && x.startDate <= DateTime.Now && x.endDate >= DateTime.Now).First();
            } catch { }
            return landingPage;
        }
    }
}