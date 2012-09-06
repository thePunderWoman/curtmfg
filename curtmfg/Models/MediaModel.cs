using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.YouTube;
using Google.GData.Extensions.MediaRss;
using Google.YouTube;

namespace curtmfg.Models {
    public class MediaModel {

        public static Feed<Google.YouTube.Video> GetCurtVideos(int page = 1) {
            try {
                YouTubeRequestSettings settings = new YouTubeRequestSettings("curtmfg", "AI39si6iCFZ_NutrvZe04i9_m7gFhgmPK1e7LF6-yHMAwB-GDO3vC3eD0R-5lberMQLdglNjH3IWUMe3tJXe9qrFe44n2jAUyg");
                YouTubeRequest req = new YouTubeRequest(settings);

                YouTubeQuery query = new YouTubeQuery(YouTubeQuery.DefaultVideoUri);
                query.Author = "curtmfg";
                query.Formats.Add(YouTubeQuery.VideoFormat.Embeddable);
                query.OrderBy = "published";
                query.StartIndex = ((page - 1) * 25) + 1;

                // We need to load the feed data for the CURTMfg Youtube Channel
                Feed<Google.YouTube.Video> video_feed = req.Get<Google.YouTube.Video>(query);
                return video_feed;
            } catch (Exception) {
                return null;
            }
        }

        public static AtomFeed GetFeed(string url = "", int start = 0, int num_results = 0) {
            FeedQuery query = new FeedQuery("");
            Google.GData.Client.Service service = new Google.GData.Client.Service();
            query.Uri = new Uri(url);
            if (start > 0) {
                query.StartIndex = start;
            }
            if (num_results > 0) {
                query.NumberToRetrieve = num_results;
            }

            AtomFeed feed = service.Query(query);
            return feed;
        }

        public static List<Video> GetVideos() {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                List<Video> videolist = db.Videos.OrderBy(x => x.sort).ToList<Video>();
                return videolist;
            } catch (Exception) {
                return new List<Video>();
            }
        }

        public static Google.YouTube.Video GetYouTubeVideo(string id = "") {
            try {
                // Initiate video object
                Google.YouTube.Video video = new Google.YouTube.Video();

                // Initiate YouTube request object
                YouTubeRequestSettings settings = new YouTubeRequestSettings("curtmfg", "AI39si6iCFZ_NutrvZe04i9_m7gFhgmPK1e7LF6-yHMAwB-GDO3vC3eD0R-5lberMQLdglNjH3IWUMe3tJXe9qrFe44n2jAUyg");
                YouTubeRequest req = new YouTubeRequest(settings);
                req.Proxy = null;

                // Create the URI and make request to YouTube
                Uri video_url = new Uri("http://gdata.youtube.com/feeds/api/videos/" + id);
                video = req.Retrieve<Google.YouTube.Video>(video_url);

                return video;
            } catch (Exception) {
                return new Google.YouTube.Video();
            }
        }

    }

    public class YouTubeVideo {
        public string VideoId { get; set; }
        public string Title { get; set; }
        public string WatchPage { get; set; }
        public string Description { get; set; }
    }
}
