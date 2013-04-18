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
using Newtonsoft.Json;


namespace curtmfg.Controllers {
    public class PartController : BaseController {
        //
        // GET: /HitchLookup/

        public ActionResult Index(int id, Review review = null) {
            if (id.ToString().Length == 6) {
                int pid = Convert.ToInt32(id.ToString().Substring(0, 5));
                int lid = Convert.ToInt32(id.ToString().Substring(5, 1));
                if (lid == 0) {
                    int nid = Convert.ToInt32(pid.ToString() + "3");
                    return RedirectToRoutePermanent("Part", new { id = nid });
                }
            }
            if (review != null) {
                ViewBag.name = review.name;
                ViewBag.email = review.email;
                ViewBag.subject = review.subject;
                ViewBag.rating = review.rating;
                ViewBag.review_text = review.review_text;
                ViewBag.errors = review.errors;
            }

            int vehicleID = (ViewBag.vehicleID != null && ViewBag.vehicleID != "") ? Convert.ToInt32(ViewBag.vehicleID) : 0;

            APIPart part = Hitch_API.getPart(id, vehicleID);
            if(part.partID > 0) {
                ViewBag.part = part;

                List<curtmfg.Models.Category> breadcrumbs = Hitch_API.GetPartBreadcrumbs(part.partID, ViewBag.lastVisitedCategory);
                ViewBag.breadcrumbs = breadcrumbs;

                List<APIPart> accessories = Hitch_API.getRelatedParts(id);
                ViewBag.accessories = accessories;

                List<APIPart> connectors = new List<APIPart>();
                if (part.vehicleID != 0 && part.pClass != "" && part.pClass.ToLower() != "wiring") {
                    connectors = Hitch_API.getConnectors(vehicleID);
                }
                ViewBag.connectors = connectors;

                List<FullVehicle> vehicles = Hitch_API.getVehiclesByPart(part.partID);
                ViewBag.vehicles = vehicles;

                string colorcode = Hitch_API.GetColorCode(part.partID).code;
                if (colorcode != null && colorcode != "") {
                    int redval = Convert.ToInt32(colorcode.Substring(0, 3));
                    int greenval = Convert.ToInt32(colorcode.Substring(3, 3));
                    int blueval = Convert.ToInt32(colorcode.Substring(6, 3));
                    ViewBag.colorcode = redval + "," + greenval + "," + blueval;
                    ViewBag.hexcode = ColorTranslator.ToHtml(Color.FromArgb(redval, greenval, blueval));
                }
                return View();
            } else {
                return RedirectToAction("index", "_404");
            }

        }

        public ActionResult AddReview(int id, int rating = 0, string subject = "", string review_text = "", string name = "", string email = "") {
            List<string> errors = new List<string>();
            try {
                string message = "Review Submitted Successfully";
                string remoteip = (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null) ? Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : Request.ServerVariables["REMOTE_ADDR"];

                bool recaptchavalid = ReCaptcha.ValidateCaptcha(Request.Form["recaptcha_challenge_field"], Request.Form["recaptcha_response_field"], remoteip);
                if (!recaptchavalid) errors.Add("Captcha was incorrect.");
                if (subject.Trim() == "") errors.Add("A Subject is required.");
                if (review_text.Trim() == "") errors.Add("A Review is required.");
                if (rating == 0) errors.Add("A Rating is required.");
                if (id == 0) errors.Add("A Part Number is required.");
                if (errors.Count() > 0) throw new Exception();

                string response = Hitch_API.submitReview(id, rating, name, email, subject, review_text);
                if (response != "success") {
                    errors.Add(response);
                    throw new Exception();
                }

                return RedirectToAction("index", "part", new { id = id, message = message });
            } catch {
                Review tempreview = new Review {
                    name = name,
                    email = email,
                    rating = rating,
                    review_text = review_text,
                    subject = subject,
                    errors = errors
                };
                return RedirectToAction("index", "part", new { id = id,  });
            }
        }

        public string AddReviewAjax(int id = 0, int rating = 0, string subject = "", string review_text = "", string name = "", string email = "") {
            List<string> errors = new List<string>();
            string reviewresponse = "fail";
            try {
                if (subject.Trim() == "" || review_text.Trim() == "" || rating == 0 || id == 0) {
                    throw new Exception();
                }

                reviewresponse = Hitch_API.submitReview(id, rating, name, email, subject, review_text);
                if (reviewresponse != "success") {
                    throw new Exception();
                }
            } catch { }
            var response = new { result = reviewresponse };
            return JsonConvert.SerializeObject(response);
        }

    }

    public class Review {
        public string name { get; set; }
        public string subject { get; set; }
        public int rating { get; set; }
        public string review_text { get; set; }
        public string email { get; set; }
        public List<string> errors { get; set; }
    }
}
