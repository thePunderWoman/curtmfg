using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Globalization;
using Newtonsoft.Json;

namespace curtmfg.Models {
    public class Twitter {
        private static string twitterapi = "https://api.twitter.com";
        private static string oauth_consumer_key = "nN0BFcaIVP2ucY5y1qn4w";
        private static string oauth_consumer_secret = "CErsPEh4wXnfujGUPceRQ8zlWZC9GO1QcYenwkISg";
        private static string oauth_version = "1.0a";
        private static string oauth_signature_method = "HMAC-SHA1";
        private static string oauth_token = "15837162-BZaAMIgmabS9WAiNWI9iUdG6QycwWbDnjM9Q8zmPR";
        private static string oauth_token_secret = "ITIP3oAIN5b9PUUnTEZPB9niHIQ9sO5W04VFb76k";

        public static string getBaseString(string apicall, string method, string nonce, string timestamp) {
            string baseString = String.Empty;
            baseString += method + "&";

            SortedDictionary<string, string> sd = new SortedDictionary<string, string>();
            sd.Add("oauth_version", oauth_version);
            sd.Add("oauth_consumer_key", oauth_consumer_key);
            sd.Add("oauth_nonce", nonce);
            sd.Add("oauth_signature_method", oauth_signature_method);
            sd.Add("oauth_timestamp", timestamp);
            sd.Add("oauth_token", oauth_token);
            baseString += Uri.EscapeDataString(twitterapi + apicall);
            foreach (KeyValuePair<string, string> entry in sd) {
                baseString += Uri.EscapeDataString("&" + entry.Key + "=" + entry.Value);
            }

            return baseString;
        }
        
        public static string signRequest(string apicall,string method,string nonce, string timestamp) {
            
            string signingKey = Uri.EscapeDataString(oauth_consumer_secret) + "&" + Uri.EscapeDataString(oauth_token_secret);

            HMACSHA1 hasher = new HMACSHA1( new ASCIIEncoding().GetBytes(signingKey));
            string signatureString = Convert.ToBase64String(hasher.ComputeHash(new ASCIIEncoding().GetBytes(getBaseString(apicall,method,nonce,timestamp))));

            string authParams = String.Empty;
            authParams += "Oauth ";
            authParams += "oauth_nonce=" + "\"" + Uri.EscapeDataString(nonce) + "\",";
            authParams += "oauth_timestamp=" + "\"" + Uri.EscapeDataString(timestamp) + "\",";
            authParams += "oauth_consumer_key=" + "\"" + Uri.EscapeDataString(oauth_consumer_key) + "\",";
            authParams += "oauth_token=" + "\"" + Uri.EscapeDataString(oauth_token) + "\",";
            authParams += "oauth_signature=" + "\"" + Uri.EscapeDataString(signatureString) + "\",";
            authParams += "oauth_version=" + "\"" + Uri.EscapeDataString(oauth_version) + "\"";

            return authParams;
        }

        private static string makeAPIRequest(string apicall, string method = "GET") {
            string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            string oauth_timestamp = Convert.ToInt64(ts.TotalSeconds).ToString();

            System.Net.ServicePointManager.Expect100Continue = false;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(twitterapi + apicall);
            request.Method = method;

            request.Headers.Add("Authorization", signRequest(apicall,method,oauth_nonce,oauth_timestamp));
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 3 * 60 * 1000;
            string jsonresponse = "";

            try {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                jsonresponse = sr.ReadToEnd();

            } catch (WebException e) {
                jsonresponse = "{\"error\":\"" + e.Message + "\"}";
            }

            return jsonresponse;
        }

        public static int getTimeline() {
            CurtDevDataContext db = new CurtDevDataContext();
            string latesttweet = "156823928013197312";
            try {
                latesttweet = db.Tweets.OrderByDescending(x => x.postDate).Select(x => x.twitterTweetID).First();
            } catch { };

            string jsonresult = makeAPIRequest("/1/statuses/user_timeline.json?include_entities=true&exclude_replies=false&include_rts=false&screen_name=curtmfg&since_id=" + latesttweet);
            List<APITweet> tweets = JsonConvert.DeserializeObject<List<APITweet>>(jsonresult);
            try {
                List<Tweet> new_tweets = new List<Tweet>();
                foreach (APITweet tweet in tweets) {
                    if (db.Tweets.Where(x => x.twitterTweetID == tweet.id_str).Count() == 0) {
                        Tweet t = new Tweet {
                            postDate = ParseTwitterTime(tweet.created_at),
                            tweet1 = tweet.text,
                            twitterTweetID = tweet.id_str,
                            twitterUserID = tweet.user.id_str,
                            screenName = tweet.user.screen_name,
                            profilePhoto = tweet.user.profile_image_url_https
                        };
                        new_tweets.Add(t);
                    }
                }
                db.Tweets.InsertAllOnSubmit(new_tweets);
                db.SubmitChanges();
            } catch { };
            return tweets.Count;
        }

        public static void getTweetsFromTwitter() {
            CurtDevDataContext db = new CurtDevDataContext();
            double CallsPerHour = 12;
            double interval = (60 * 60) / CallsPerHour;
            DateTime lastCheck;
            try {
                lastCheck = db.TwitterLogs.OrderByDescending(x => x.lastUpdated).Select(x => x.lastUpdated).FirstOrDefault();
            } catch { lastCheck = DateTime.Now.AddSeconds(-(interval * 2)); };
            if (DateTime.Now.CompareTo(lastCheck.AddSeconds(interval)) >= 0) {

                TwitterLog log = new TwitterLog {
                    lastUpdated = DateTime.Now,
                    tweets = 0
                };
                db.TwitterLogs.InsertOnSubmit(log);
                db.SubmitChanges();
                log.tweets = getTimeline();
                db.SubmitChanges();
            }
        }

        public static string getTweets() {
            CurtDevDataContext db = new CurtDevDataContext();
            JavaScriptSerializer js = new JavaScriptSerializer();
            Twitter.getTweetsFromTwitter();
            List<FullTweet> tweets = new List<FullTweet>();
            try {
                tweets = (from t in db.Tweets
                          orderby t.postDate descending
                          select new FullTweet {
                              tweetID = t.tweetID,
                              tweet1 = t.tweet1,
                              twitterTweetID = t.twitterTweetID,
                              twitterUserID = t.twitterUserID,
                              postDate = t.postDate,
                              profilePhoto = t.profilePhoto,
                              screenName = t.screenName,
                              date = String.Format("{0:MM/dd/yyyy}",t.postDate)
                          }).Take(10).ToList<FullTweet>();
                            
            } catch { };
            return js.Serialize(tweets);
        }
        
        public static DateTime ParseTwitterTime(string date) {
            //Format: Sat Sep 10 22:23:38 +0000 2011
            const string format = "ddd MMM dd HH:mm:ss zzzz yyyy";
            return DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
        }
    }

    public class FullTweet : Tweet {
        public string date { get; set; }
    }

    public class APITweet {
        public object in_reply_to_status_id { get; set; }
        public object in_reply_to_status_id_str { get; set; }
        public Entities entities { get; set; }
        public object in_reply_to_user_id { get; set; }
        public bool favorited { get; set; }
        public bool possibly_sensitive { get; set; }
        public bool truncated { get; set; }
        public object in_reply_to_user_id_str { get; set; }
        public string id_str { get; set; }
        public bool retweeted { get; set; }
        public object in_reply_to_screen_name { get; set; }
        public object geo { get; set; }
        public User user { get; set; }
        public object contributors { get; set; }
        public string created_at { get; set; }
        public string source { get; set; }
        public int retweet_count { get; set; }
        public object id { get; set; }
        public string text { get; set; }
    }

    public class User {
        public string profile_background_image_url_https { get; set; }
        public string profile_sidebar_fill_color { get; set; }
        public string id_str { get; set; }
        public bool default_profile_image { get; set; }
        public bool contributors_enabled { get; set; }
        public string lang { get; set; }
        public bool verified { get; set; }
        public int friends_count { get; set; }
        public string location { get; set; }
        public string name { get; set; }
        public int favourites_count { get; set; }
        public string profile_sidebar_border_color { get; set; }
        public bool is_translator { get; set; }
        public int? utc_offset { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public bool show_all_inline_media { get; set; }
        public bool profile_use_background_image { get; set; }
        public int statuses_count { get; set; }
        public string profile_text_color { get; set; }
        public int listed_count { get; set; }
        public object following { get; set; }
        public string profile_background_image_url { get; set; }
        public string time_zone { get; set; }
        public bool geo_enabled { get; set; }
        public string created_at { get; set; }
        public string profile_link_color { get; set; }
        public int followers_count { get; set; }
        public string profile_image_url { get; set; }
        public bool default_profile { get; set; }
        public string profile_background_color { get; set; }
        public int id { get; set; }
        public object follow_request_sent { get; set; }
        public object notifications { get; set; }
        public bool profile_background_tile { get; set; }
        public string profile_image_url_https { get; set; }
        public string screen_name { get; set; }
    }

    public class Url {
        public string display_url { get; set; }
        public string url { get; set; }
        public string expanded_url { get; set; }
        public List<int> indices { get; set; }
    }

    public class Entities {
        public List<object> user_mentions { get; set; }
        public List<object> hashtags { get; set; }
        public List<Url> urls { get; set; }
    }
}