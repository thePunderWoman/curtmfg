using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Net.Mail;

namespace curtmfg.Controllers
{
    public class ForumController : BaseController
    {
        //
        // GET: /FAQ/

        public ActionResult Index(){

            List<FullGroup> groups = ForumModel.GetAllGroups();
            ViewBag.groups = groups;

            return View();

        }

        public ActionResult Topic(int id = 0, int page = 1, int perpage = 10) {

            FullTopic topic = ForumModel.GetTopic(id,page,perpage);
            ViewBag.topic = topic;
            ViewBag.page = page;
            ViewBag.perpage = perpage;
            decimal pagecount = Math.Ceiling(Convert.ToDecimal(ForumModel.GetTopicDiscussionCount(id)) / Convert.ToDecimal(perpage));
            ViewBag.pagecount = Convert.ToInt32(pagecount);

            return View();
        }

        public ActionResult Discussion(int id = 0, int page = 1, int perpage = 10) {

            Thread thread = ForumModel.GetThread(id, page, perpage);
            ViewBag.thread = thread;
            ViewBag.page = page;
            ViewBag.perpage = perpage;
            decimal pagecount = Math.Ceiling(Convert.ToDecimal(ForumModel.GetDiscussionPostCount(id)) / Convert.ToDecimal(perpage));
            ViewBag.pagecount = Convert.ToInt32(pagecount);

            FullTopic topic = ForumModel.GetTopic(thread.topicID);
            ViewBag.topic = topic;

            return View();
        }

        public string AddDiscussionAjax(int topicid = 0, string addtitle = "", string post = "", string name = "", string email = "", string company = "", bool notify = false, string recaptcha_challenge_field = "", string recaptcha_response_field = "") {
            CurtDevDataContext db = new CurtDevDataContext();
            JavaScriptSerializer js = new JavaScriptSerializer();
            #region trycatch
            try {
                string remoteip = (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null) ? Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : Request.ServerVariables["REMOTE_ADDR"];
                if (!(Models.ReCaptcha.ValidateCaptcha(recaptcha_challenge_field, recaptcha_response_field, remoteip))) {
                    throw new Exception("The Captcha was incorrect. Try again please!");
                }
                if (topicid == 0) { throw new Exception("The topic was invalid."); }
                if (addtitle.Trim() == "") { throw new Exception("Title is required."); }
                if (post.Trim() == "") { throw new Exception("Post is required"); }
                if (email.Trim() != "" && (!IsValidEmail(email.Trim()))) { throw new Exception("Your email address was not a valid address."); }
                if (notify == true && email.Trim() == "") { throw new Exception("You cannot be notified by email without an email address."); }
                if(checkIPAddress(remoteip)) { throw new Exception("You cannot post because your IP Address is blocked by our server."); }

                bool moderation = Convert.ToBoolean(ConfigurationManager.AppSettings["ForumModeration"]);

                ForumThread t = new ForumThread {
                    topicID = topicid,
                    active = true,
                    closed = false,
                    createdDate = DateTime.Now,
                };

                db.ForumThreads.InsertOnSubmit(t);
                db.SubmitChanges();

                ForumPost p = new ForumPost {
                    threadID = t.threadID,
                    title = addtitle,
                    post = post,
                    name = name,
                    email = email,
                    notify = notify,
                    approved = !moderation,
                    company = company,
                    createdDate = DateTime.Now,
                    flag = false,
                    active = true,
                    IPAddress = remoteip,
                    parentID = 0,
                    sticky = false
                };

                db.ForumPosts.InsertOnSubmit(p);
                db.SubmitChanges();

                Post newpost = ForumModel.GetPost(p.postID);
                return js.Serialize(newpost);

            } catch (Exception e) {
                return "{\"error\": \"" + e.Message + "\"}";
            }
            #endregion
        }

        public string AddReplyAjax(int threadid = 0, string addtitle = "", string post = "", string name = "", string email = "", string company = "", bool notify = false, string recaptcha_challenge_field = "", string recaptcha_response_field = "") {
            CurtDevDataContext db = new CurtDevDataContext();
            JavaScriptSerializer js = new JavaScriptSerializer();
            #region trycatch
            try {
                string remoteip = (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null) ? Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : Request.ServerVariables["REMOTE_ADDR"];
                if (!(Models.ReCaptcha.ValidateCaptcha(recaptcha_challenge_field, recaptcha_response_field, remoteip))) {
                    throw new Exception("The Captcha was incorrect. Try again please!");
                }
                if (threadid == 0) { throw new Exception("The thread was invalid."); }
                if (addtitle.Trim() == "") { throw new Exception("Title is required."); }
                if (post.Trim() == "") { throw new Exception("Post is required"); }
                if (email.Trim() != "" && (!IsValidEmail(email.Trim()))) { throw new Exception("Your email address was not a valid address."); }
                if (notify == true && email.Trim() == "") { throw new Exception("You cannot be notified by email without an email address."); }
                if (checkIPAddress(remoteip)) { throw new Exception("You cannot post because your IP Address is blocked by our server."); }

                bool moderation = Convert.ToBoolean(ConfigurationManager.AppSettings["ForumModeration"]);

                ForumPost p = new ForumPost {
                    threadID = threadid,
                    title = addtitle,
                    post = post,
                    name = name,
                    email = email,
                    notify = notify,
                    approved = !moderation,
                    company = company,
                    createdDate = DateTime.Now,
                    flag = false,
                    active = true,
                    IPAddress = remoteip,
                    parentID = 0,
                    sticky = false
                };

                db.ForumPosts.InsertOnSubmit(p);
                db.SubmitChanges();
                sendNotifications(p.threadID,email);
                Post newpost = ForumModel.GetPost(p.postID);
                return js.Serialize(newpost);

            } catch (Exception e) {
                return "{\"error\": \"" + e.Message + "\"}";
            }
            #endregion
        }

        public string Flag(int postID = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            try {
                ForumPost p = db.ForumPosts.Where(x => x.postID == postID).First<ForumPost>();
                p.flag = true;
                db.SubmitChanges();
                return "true";
            } catch { return "false"; };
        }

        public ActionResult FlagAsSpam(int id = 0) {
            CurtDevDataContext db = new CurtDevDataContext();
            try {
                ForumPost p = db.ForumPosts.Where(x => x.postID == id).First<ForumPost>();
                p.flag = true;
                db.SubmitChanges();
                return RedirectToAction("Discussion", new { id = p.threadID });
            } catch {
                return RedirectToAction("Index");
            };
        }

        public ActionResult Unsubscribe(int id = 0, string email = "") {
            CurtDevDataContext db = new CurtDevDataContext();
            ViewBag.thread = ForumModel.GetThread(id);
            try {
                List<ForumPost> posts = db.ForumPosts.Where(x => x.threadID == id).Where(x => x.email.Trim() == email.Trim()).Where(x => x.notify == true).ToList<ForumPost>();
                foreach (ForumPost p in posts) {
                    p.notify = false;
                }
                db.SubmitChanges();
                ViewBag.unsubscribed = true;
            } catch {
                ViewBag.unsubscribed = false;
            };
            return View();
        }

        public static bool checkIPAddress(string ipaddress) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                IPBlock block = db.IPBlocks.Where(x => x.IPAddress == ipaddress).First<IPBlock>();
                return true;
            } catch { }
            return false;
        }
        
        public static bool IsValidEmail(string strIn) {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

        /* Start Email Methods */
        private void sendNotifications(int threadid = 0, string originalemail = "") {
            CurtDevDataContext db = new CurtDevDataContext();
            SmtpClient SmtpServer = new SmtpClient();
            List<string> emails = new List<string>();
            Thread t = ForumModel.GetThread(threadid);
            emails = db.ForumPosts.Where(x => x.threadID == threadid).Where(x => x.notify == true).Select(x => x.email).Distinct().ToList();
            foreach (string email in emails) {
                try {
                    MailMessage mail = new MailMessage();

                    mail.To.Add(email);
                    mail.Subject = "CURT Manufacturing Forum Reply Notification";

                    mail.IsBodyHtml = true;
                    string htmlBody;

                    htmlBody = "<div style='margin-top: 15px;font-family: Arial;font-size: 10pt;'>";
                    htmlBody += "<h4>Hi There!</h4>";
                    htmlBody += "<p>Someone replied to your post on the CURT Manufacturing forum. Visit the following link to see the reply:</p>";
                    htmlBody += "<p style='margin:2px 0px'><a href='http://" + HttpContext.Request.Url.Host + "/Forum/Discussion/" + threadid + "/" + UDF.GenerateSlug(t.firstPost.title) + "'>" + t.firstPost.title + "</a></p>";
                    htmlBody += "______________________________________________________________________";
                    htmlBody += "<br /><span style='color:#999'>Thank you,</span>";
                    htmlBody += "<br /><br /><br />";
                    htmlBody += "<span style='line-height:75px;color:#999'>CURT Manufacturing Forums</span>";
                    htmlBody += "<p style='font-size: 11px;'>To unsubscribe from future notifications, click the unsubscribe link. <a href='http://" + HttpContext.Request.Url.Host + "/Forum/Unsubscribe/" + threadid + "?email=" + email + "'>Unsubscribe</a></p>";
                    htmlBody += "</div>";

                    mail.Body = htmlBody;

                    if (email.Trim() != originalemail.Trim()) {
                        SmtpServer.Send(mail);
                    }
                } catch { };
            }
        }
        /* End Email Methods */
    }
}
