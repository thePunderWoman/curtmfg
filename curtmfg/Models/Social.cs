using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace curtmfg.Models
{
    public class Social{


        public List<TwitterFeed> getTweets(string user, int count = 0) {

            XElement twitter = XElement.Load("http://www.twitter.com/statuses/user_timeline/"+user+".xml");

            var tweets = from t in twitter.Elements("status")
                         select new TwitterFeed( 
                             t.Element("text"), 
                             t.Element("id"), 
                             t.Element("source"),
                             t.Element("created_at"),
                             t.Element("truncated"),
                             t.Element("favorited"),
                             t.Element("retweeted"),
                             t.Element("retweet_count")
                         );
            if (count == 0) {
                return tweets.ToList();
            } else {
                return tweets.Take(count).ToList();
            }

        }
    }


    public class TwitterFeed {
        public string text;
        public string id;
        public string source;
        public string created;
        public DateTime createdDate;
        public string retweetCount;
        public Boolean truncated;
        public Boolean favorited;
        public Boolean retweeted;
        

        public TwitterFeed(XElement text, XElement id, XElement source, XElement created, XElement truncate, XElement favorite, XElement retweet, XElement rC) {
            this.text = text.Value.ToString();
            this.id = id.Value.ToString();
            this.source = source.Value.ToString();
            this.source = this.source.Replace("href", "target='_blank' href");
            this.created = created.Value.ToString().Replace("+0000","");
            this.createdDate = Convert.ToDateTime(this.created.Split(' ')[1] + " " + this.created.Split(' ')[2] + ", " + this.created.Split(' ')[4]);
            this.truncated = (bool)truncate;
            this.favorited = (bool)favorite;
            this.retweeted = (bool)retweet;
            this.retweetCount = rC.ToString();
        }


    }
}
