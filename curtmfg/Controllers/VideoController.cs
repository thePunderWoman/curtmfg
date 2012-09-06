using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;
using Google.GData.Client;
using Google.GData.YouTube;
using Google.GData.Extensions.MediaRss;
using Google.YouTube;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace curtmfg.Controllers {
    public class VideoController : BaseController {
        //
        // GET: /Media/

        public ActionResult Index(int page = 1) {

            Feed<Google.YouTube.Video> curt_videos = MediaModel.GetCurtVideos(page);
            ViewBag.curt_videos = curt_videos;
            ViewBag.page = page;
            return View();
        }

        public string getHomePageVideos() {
            List<curtmfg.Video> videos = MediaModel.GetVideos();
            return JsonConvert.SerializeObject(videos);
        }

    }
}
