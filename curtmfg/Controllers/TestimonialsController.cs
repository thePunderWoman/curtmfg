using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;
using System.Web.Script.Serialization;

namespace curtmfg.Controllers
{
    public class TestimonialsController : BaseController
    {
        //
        // GET: /FAQ/

        public ActionResult Index(string message = "", int page = 1, int pageSize = 10) {

            List<string> errors = (List<string>)TempData["errors"];
            ViewBag.first_name = ((string)TempData["first_name"] != null) ? (string)TempData["first_name"] : "";
            ViewBag.last_name = ((string)TempData["last_name"] != null) ? (string)TempData["last_name"] : "";
            ViewBag.location = ((string)TempData["location"] != null) ? (string)TempData["location"] : "";
            ViewBag.addtitle = ((string)TempData["addtitle"] != null) ? (string)TempData["addtitle"] : "";
            ViewBag.testimonial = ((string)TempData["testimonial"] != null) ? (string)TempData["testimonial"] : "";
            ViewBag.rating = (TempData["rating"] != null) ? (double)TempData["rating"] : 0;
            ViewBag.errors = (errors == null) ? new List<string>() : errors;
            ViewBag.message = message;

            // Get all the FAQs listed alphabetically by question
            List<Testimonial> testimonials = TestimonialModel.GetAll(page, pageSize);
            ViewBag.testimonials = testimonials;

            int total = TestimonialModel.CountAll();

            decimal pagecount = Math.Ceiling(Convert.ToDecimal(total) / Convert.ToDecimal(pageSize));
            ViewBag.pagecount = Convert.ToInt32(pagecount);
            ViewBag.page = page;

            return View();
        }

        public string AddTestimonialAjax(string first_name = "", string last_name = "", string location = "", double rating = 0, string addtitle = "", string testimonial = "") {
            Testimonial t = new Testimonial();
            JavaScriptSerializer js = new JavaScriptSerializer();
            try {
                if (testimonial.Trim() == "" || addtitle.Trim() == "" || rating == 0) {
                    throw new Exception();
                }
                t = TestimonialModel.Add(first_name, last_name, location, addtitle, testimonial, rating);
            } catch {}
            return js.Serialize(t);
        }

        public ActionResult AddTestimonial(string first_name = "", string last_name = "", string location = "", double rating = 0, string addtitle = "", string testimonial = "") {
            Testimonial t = new Testimonial();
            List<string> errors = new List<string>();
            try {
                string remoteip = (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null) ? Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : Request.ServerVariables["REMOTE_ADDR"];
                bool recaptchavalid = ReCaptcha.ValidateCaptcha(Request.Form["recaptcha_challenge_field"], Request.Form["recaptcha_response_field"], remoteip);
                if (!recaptchavalid) errors.Add("Captcha was incorrect.");
                if (rating == 0) errors.Add("Rating is required.");
                if (addtitle.Trim() == "") errors.Add("A Title is required.");
                if (testimonial.Trim() == "") errors.Add("A Testimonial is required.");
                if (errors.Count() > 0) throw new Exception();
                
                t = TestimonialModel.Add(first_name, last_name, location, addtitle, testimonial, rating);

                string message = "Your Testimonial has been submitted successfully. Thank you!";
                return RedirectToAction("Index", "Testimonials", new { message = message });
            } catch {
                TempData["first_name"] = first_name;
                TempData["last_name"] = last_name;
                TempData["location"] = location;
                TempData["addtitle"] = addtitle;
                TempData["testimonial"] = testimonial;
                TempData["rating"] = rating;
                return RedirectToAction("Index", "Testimonials");
            }
        }
    }
}
