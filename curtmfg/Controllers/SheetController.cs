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
using System.Drawing;
using System.IO;


namespace curtmfg.Controllers {
    public class SheetController : BaseController {
        //
        // GET: /Install Sheet Controller/
        

        public ActionResult Index(int id = 0) {
            APIContent installsheet = Hitch_API.getInstallSheet(id);
            if (installsheet != null && installsheet.content != null && installsheet.content != "") {
                return Redirect(installsheet.content);
            } else {
                return View();
            }

        }

    }
}
