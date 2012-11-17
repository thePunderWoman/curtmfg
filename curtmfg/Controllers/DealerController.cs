using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using curtmfg.Models;
using System.Web.Script.Serialization;

namespace curtmfg.Controllers
{
    public class DealerController : BaseController
    {
        //
        // GET: /Dealer/

        public ActionResult Index() {
            if (!AuthenticateModel.checkAuth(Request.Cookies.Get("customerID"))) {
                string message = "Authentication is required to view that content. Please login using the form below.";
                return RedirectToAction("Login", new { message = message });
            }

            menuWithContent dealermenu = new MenuModel().Get("dealerarea");
            ViewBag.dealermenu = dealermenu;
            ContentPage content = new ContentPage();
            foreach (menuItem item in dealermenu.getChildren()) {
                if (item.hasContent()) {
                    content = item.content;
                    break;
                }
            }
            ViewBag.content = content;

            return View();
        }

        public string getWhereToBuyDealers() {
            List<Customer> dealers = DealerModel.GetWhereToBuyDealers();
            return Newtonsoft.Json.JsonConvert.SerializeObject(dealers);
        }

        public ActionResult BecomeADealer() {
            string error = "";
            CurtDevDataContext db = new CurtDevDataContext();
            bool success = false;

            #region Form Submission
            try {
                if (Request.Form.Count > 0) {
                    // Save form values
                    string first_name = (Request.Form["first_name"] != null) ? Request.Form["first_name"] : "";
                    string last_name = (Request.Form["last_name"] != null) ? Request.Form["last_name"] : "";
                    string business_name = (Request.Form["business_name"] != null) ? Request.Form["business_name"] : "";
                    string email = (Request.Form["email"] != null) ? Request.Form["email"] : "";
                    string phone = (Request.Form["phone"] != null) ? Request.Form["phone"] : "";
                    string type = (Request.Form["type"] != null) ? Request.Form["type"] : "";
                    string street1 = (Request.Form["street1"] != null) ? Request.Form["street1"] : "";
                    string street2 = (Request.Form["street2"] != null) ? Request.Form["street2"] : "";
                    string city = (Request.Form["city"] != null) ? Request.Form["city"] : "";
                    string state = (Request.Form["state"] != null) ? Request.Form["state"] : "";
                    string postalCode = (Request.Form["postalCode"] != null) ? Request.Form["postalCode"] : "";
                    string country = (Request.Form["country"] != null) ? Request.Form["country"] : "";
                    string formmessage = (Request.Form["message"] != null) ? Request.Form["message"] : "";
                    
                    // Validate the form fields
                    if (first_name.Length == 0) throw new Exception("First Name is required.");
                    if (last_name.Length == 0) throw new Exception("Last Name is required.");
                    if (business_name.Length == 0) throw new Exception("Business Name is required.");
                    if (email.Length == 0) throw new Exception("Email is required.");
                    if (phone.Length == 0) throw new Exception("Phone is required.");
                    if (type.Length == 0) throw new Exception("Business Type is required.");
                    if (street1.Length == 0) throw new Exception("Address is required.");
                    if (city.Length == 0) throw new Exception("City is required.");
                    if (state.Length == 0) throw new Exception("State / Province is required.");
                    if (postalCode.Length == 0) throw new Exception("Postal Code is required.");
                    if (country.Length == 0) throw new Exception("Country is required.");

                    string message = "This contact is interested in becoming a CURT Dealer.<br>";
                    message += "<strong>Name:</strong> " + first_name + " " + last_name + "<br>";
                    message += "<strong>Business:</strong> " + business_name + "<br>";
                    message += "<strong>Business Type:</strong> " + type + "<br>";
                    message += "<strong>Email:</strong> " + email + "<br>";
                    message += "<strong>Phone:</strong> " + phone + "<br>";
                    message += "<strong>Street 1:</strong> " + street1 + "<br>";
                    message += "<strong>Street 2:</strong> " + street2 + "<br>";
                    message += "<strong>City, State, Zip:</strong> " + city + ", " + state + " " + postalCode + "<br>";
                    message += "<strong>Country:</strong> " + country + "<br>";
                    message += "<strong>Message:</strong> " + formmessage + "<br>";

                    // Create the new customer and save
                    Contact contact = new Contact {
                        first_name = first_name,
                        last_name = last_name,
                        email = email,
                        phone = phone,
                        type = "New Customer",
                        createdDate = DateTime.Now,
                        subject = "New Customer Request",
                        message = message
                    };

                    success = ContactModel.Send(contact);
                    if (!success) {
                        throw new Exception("There was an error sending your email. Try again later.");
                    }
                }
            } catch (Exception e) {
                error = e.Message;
            }
            #endregion

            List<BusinessClass> businessclasses = db.BusinessClasses.OrderBy(x => x.sort).ToList<BusinessClass>();
            ViewBag.businessclasses = businessclasses;
            ViewBag.success = success;

            ViewBag.error = error;
            return View();
        }

        public ActionResult ForgotPassword() {
            CurtDevDataContext db = new CurtDevDataContext();
            string error = "";
            bool success = false;

            #region Form Submission
            try {
                if (Request.Form.Count > 0) {
                    // Save form values
                    int customerID = (Request.Form["customerID"] != null) ? Convert.ToInt32(Request.Form["customerID"].Trim()) : 0;

                    // Validate the form fields
                    if (customerID == 0) throw new Exception("Please enter a customer ID.");

                    Customer customer = db.Customers.Where(x => x.customerID == customerID).FirstOrDefault<Customer>();

                    if (customer != null) {
                        if (customer.email != null && customer.email != "") {
                            // customer email exists
                            if (customer.password == null || customer.password == "") {
                                // no password for the account
                                PasswordGenerator pw = new PasswordGenerator();
                                customer.password = pw.Generate();
                                db.SubmitChanges();
                            }

                            // Mail the customer their password
                            success = ContactModel.ForgotPassword(customer);
                            if (!success) {
                                throw new Exception("An error ocurred sending you your password. Try again later or contact us for further assistance.");
                            }
                        } else {
                            // no customer email
                            throw new Exception("No Email Address is set up with your account. Please Contact Us to set up your account's email address.");
                        }
                    } else {
                        // No customer associated with this number.
                        throw new Exception("No Customer is associated with that number.");
                    }
                }
            } catch (Exception e) {
                error = e.Message;
            }
            #endregion

            ViewBag.error = error;
            ViewBag.success = success;

            return View();
        }

        public ActionResult page(string name = "") {
            if (!AuthenticateModel.checkAuth(Request.Cookies.Get("customerID"))) {
                string message = "You are not logged in!";
                return RedirectToAction("Login", new { message = message });
            }
            ContentPage content = new SiteContentModel().Get(name);
            ViewBag.content = content;

            menuWithContent dealermenu = new MenuModel().Get("dealerarea");
            ViewBag.dealermenu = dealermenu;

            return View("index");
        }

        public ActionResult Logout() {
            HttpCookie customerID = new HttpCookie("customerID");
            customerID.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(customerID);

            HttpCookie curtCustomerName = new HttpCookie("customerName");
            curtCustomerName.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(curtCustomerName);

            return RedirectToAction("Index", "Index");
        }

        public ActionResult Login(string message = "") {
            ViewBag.message = message;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Login(string curtcustid = "", string password = "", string message = "") {
            CurtDevDataContext db = new CurtDevDataContext();
            if (password.Trim().Length == 0) {
                ViewBag.message = "You must enter a password.";
                return View();
            }
            Customer login_cust = new Customer();

            try {
                login_cust = (from c in db.Customers
                              where c.customerID.Equals(Convert.ToInt32(curtcustid))
                              select c).FirstOrDefault();
            } catch { };
            if (login_cust == null || login_cust.cust_id == 0) {
                try {
                    login_cust = (from c in db.Customers
                                  where c.email.ToLower().Equals(curtcustid.ToLower().Trim())
                                  select c).FirstOrDefault();
                } catch { };
            }
            if (login_cust == null || login_cust.cust_id == 0) {
                try {
                    login_cust = (from c in db.Customers
                                  where c.customerID.ToString().Remove(c.customerID.ToString().Length - 2, 2).Equals(Convert.ToInt32(curtcustid))
                                  select c).FirstOrDefault();
                } catch { };
            }

            if (login_cust == null || login_cust.cust_id == 0) {
                ViewBag.message = "Customer ID / Email was not found.";
                return View();
            }
            bool authpass = false;

            /*if (password.Trim() == login_cust.customerID.ToString() && (login_cust.password == null || login_cust.password == "")) {
                return RedirectToAction("setpassword");
            }*/

            if (password.Trim() == login_cust.password) {
                authpass = true;
            } else if (password.Trim() == login_cust.customerID.ToString()) {
                authpass = true;
            }

            if (!authpass) {
                ViewBag.message = "Password was incorrect.";
                ViewBag.username = curtcustid;
                return View();
            } else {
                // User login successful: assign Session data and redirect.

                HttpCookie customerID = new HttpCookie("customerID");
                customerID.Value = login_cust.customerID + "";
                customerID.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(customerID);

                HttpCookie name = new HttpCookie("customerName");
                name.Value = login_cust.name;
                name.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(name);
                return RedirectToAction("Index", "Dealer");
            }

            ViewBag.message = "We're sorry, but there was error while logging you in. Try again later.";
            return View();
        }

    }
}
