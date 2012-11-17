using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;
using System.Web.Script.Serialization;
using System.Net.Mail;
using System.Net;

namespace curtmfg.Controllers
{
    public class ContactController : BaseController
    {
        //
        // GET: /Contact/

        public ActionResult Index(string page = "contact", string defaultvalue = "")
        {
            CurtDevDataContext db = new CurtDevDataContext();
            List<string> errors = (List<string>)TempData["errors"];
            ViewBag.first_name = ((string)TempData["first_name"] != null) ? (string)TempData["first_name"] : "";
            ViewBag.last_name = ((string)TempData["last_name"] != null) ? (string)TempData["last_name"] : "";
            ViewBag.email = ((string)TempData["email"] != null) ? (string)TempData["email"] : "";
            ViewBag.type = ((string)TempData["type"] != null) ? (string)TempData["type"] : "";
            ViewBag.phone = ((string)TempData["phone"] != null) ? (string)TempData["phone"] : "";
            ViewBag.address1 = ((string)TempData["address1"] != null) ? (string)TempData["address1"] : "";
            ViewBag.address2 = ((string)TempData["address2"] != null) ? (string)TempData["address2"] : "";
            ViewBag.city = ((string)TempData["city"] != null) ? (string)TempData["city"] : "";
            ViewBag.state = (TempData["state"] != null) ? (int)TempData["state"] : 0;
            ViewBag.postalcode = ((string)TempData["postalcode"] != null) ? (string)TempData["postalcode"] : "";
            ViewBag.subject = ((string)TempData["subject"] != null) ? (string)TempData["subject"] : "";
            ViewBag.contactmessage = ((string)TempData["contactmessage"] != null) ? (string)TempData["contactmessage"] : "";
            ViewBag.errors = (errors == null) ? new List<string>() : errors;
            
            try {
                ContentPage content = new SiteContentModel().Get(page);
                ViewBag.content = content;
            } catch { ViewBag.content = null; }

            List<FullCountry> countries = DealerModel.GetCountries();
            ViewBag.countries = countries;

            List<ContactType> types = db.ContactTypes.OrderBy(x => x.name).ToList<ContactType>();
            ViewBag.types = types;

            if(ViewBag.type == "") ViewBag.type = defaultvalue;

            return View();
        }

        public ActionResult Thank() {
            ViewBag.contact = (TempData["contact"] != null) ? (Contact)TempData["contact"] : new Contact();

            return View();
        }

        public dynamic Send(string first_name = "", string last_name = "", string type = "", string email = "", string address1 = "", string address2 = "", string phone = "", string city = "", int state = 0, string postalcode = "", string subject = "", string contactmessage = "", string page = "contact") {
            List<string> errors = new List<string>();
            CurtDevDataContext db = new CurtDevDataContext();
            try {
                string remoteip = (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null) ? Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : Request.ServerVariables["REMOTE_ADDR"];

                bool recaptchavalid = ReCaptcha.ValidateCaptcha(Request.Form["recaptcha_challenge_field"], Request.Form["recaptcha_response_field"], remoteip);
                if (!recaptchavalid) errors.Add("Captcha was incorrect.");
                if (!UDF.IsEmail(email)) errors.Add("Email is not a valid address");
                if (email.Trim() == "") errors.Add("Email is required.");
                if (first_name.Trim() == "Enter your first name.") first_name = "";
                if (last_name.Trim() == "Enter your last name.") last_name = "";
                if (first_name.Trim() == "" && last_name.Trim() == "") errors.Add("Your Name is required.");
                if (phone.Trim() == "Enter your phone number. (optional)") phone = "";
                if (address1.Trim() == "Enter your street address. (optional)") address1 = "";
                if (address2.Trim() == "Address line 2. (optional)") address2 = "";
                if (city.Trim() == "Enter your city. (optional)") city = "";
                if (postalcode.Trim() == "Enter your postal code. (optional)") postalcode = "";
                if (subject.Trim() == "Enter the subject.") subject = "";
                if (subject.Trim() == "") errors.Add("A Subject is required.");
                if (contactmessage.Trim() == "") errors.Add("A Message is required.");
                if (type.Trim() == "") errors.Add("A Reason is required.");
                if (errors.Count() > 0) throw new Exception();

                string stateabbr = "";
                string countryname = "";
                if (state > 0) {
                    try {
                        State s = db.States.Where(x => x.stateID == state).First<State>();
                        stateabbr = s.abbr;
                        Country c = db.Countries.Where(x => x.countryID == s.countryID).First<Country>();
                        countryname = c.name;
                    } catch { };
                }

                Contact contact = new Contact() {
                    first_name = first_name.Trim(),
                    last_name = last_name.Trim(),
                    email = email.Trim(),
                    phone = phone.Trim(),
                    address1 = address1.Trim(),
                    address2 = address2.Trim(),
                    city = city.Trim(),
                    state = stateabbr.Trim(),
                    country = countryname.Trim(),
                    postalcode = postalcode.Trim(),
                    subject = subject.Trim(),
                    message = contactmessage.Trim(),
                    createdDate = DateTime.Now,
                    type = type.Trim()
                };

                bool success = ContactModel.Send(contact);
                if (!success) {
                    errors.Add("Message failed to send. Try again later.");
                    throw new Exception();
                }
                TempData["contact"] = contact;
                return RedirectToAction("Thank", "Contact");
            } catch (Exception e) {
                TempData["errors"] = errors;
                TempData["first_name"] = first_name;
                TempData["last_name"] = last_name;
                TempData["email"] = email;
                TempData["phone"] = phone;
                TempData["address1"] = address1;
                TempData["address2"] = address2;
                TempData["city"] = city;
                TempData["state"] = state;
                TempData["postalcode"] = postalcode;
                TempData["subject"] = subject;
                TempData["contactmessage"] = contactmessage;
                TempData["type"] = type;
                return RedirectToAction("Index", "Contact", new { page = page });
            }
        }

    }

}
