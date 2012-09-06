using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using curtmfg.Models;
using System.Net.Mail;

namespace curtmfg.Models {
    public class ContactModel {
        public static bool Send(Contact contact) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                SmtpClient SmtpServer = new SmtpClient();

                List<ContactReceiver> receivers = (from cr in db.ContactReceivers
                                                   join crt in db.ContactReceiver_ContactTypes on cr.contactReceiverID equals crt.contactReceiverID
                                                   join ct in db.ContactTypes on crt.contactTypeID equals ct.contactTypeID
                                                   where ct.name.ToLower().Trim().Equals(contact.type.ToLower().Trim())
                                                   select cr).ToList<ContactReceiver>();
                if (receivers.Count() == 0) {
                    receivers = db.ContactReceivers.ToList<ContactReceiver>();
                }

                db.Contacts.InsertOnSubmit(contact);
                db.SubmitChanges();

                foreach (ContactReceiver receiver in receivers) {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(receiver.email);
                    mail.Subject = "New Website Contact: " + contact.subject;
                    mail.IsBodyHtml = true;
                    string mailhtml = contact.first_name + " " + contact.last_name + " said: " + contact.message;
                    string mailto = "mailto" + ":" + contact.email + "?subject=Re: " + contact.subject + "&body=" + mailhtml;
                    string htmlBody = "<p>Hi " + receiver.first_name + ",</p>";
                    htmlBody += "<p>We've received a new web contact.<br /><a style=\"border: 2px solid #e64d2d; text-transform: uppercase; color: #e64d2d; font-weight: bold; padding: 2px 6px; font-size: 14px; text-decoration: none;\" href=\"" + mailto + "\">Click to Reply</a></p>";
                    htmlBody += "<table><tr><td style=\"vertical-align: top;\"><strong>Name: </strong></td><td>" + contact.first_name + " " + contact.last_name + "</td></tr>";
                    htmlBody += "<tr><td style=\"vertical-align: top;\"><strong>Reason: </strong></td><td>" + contact.type + "</td></tr>";
                    htmlBody += "<tr><td style=\"vertical-align: top;\"><strong>Email: </strong></td><td>" + contact.email + "</td></tr>";
                    if (contact.phone != "") {
                        htmlBody += "<tr><td style=\"vertical-align: top;\"><strong>Phone: </strong></td><td>" + contact.phone + "</td></tr>";
                    }
                    if (contact.address1 != "") {
                        htmlBody += "<tr><td style=\"vertical-align: top;\"><strong>Address1: </strong></td><td>" + contact.address1 + "</td></tr>";
                    }
                    if (contact.address2 != "") {
                        htmlBody += "<tr><td style=\"vertical-align: top;\"><strong>Address2: </strong></td><td>" + contact.address2 + "</td></tr>";
                    }
                    if (contact.city != "") {
                        htmlBody += "<tr><td style=\"vertical-align: top;\"><strong>City: </strong></td><td>" + contact.city + "</td></tr>";
                    }
                    if (contact.state != "") {
                        htmlBody += "<tr><td style=\"vertical-align: top;\"><strong>State / Province: </strong></td><td>" + contact.state + "</td></tr>";
                    }
                    if (contact.postalcode != "") {
                        htmlBody += "<tr><td style=\"vertical-align: top;\"><strong>Postal Code: </strong></td><td>" + contact.postalcode + "</td></tr>";
                    }
                    if (contact.country != "") {
                        htmlBody += "<tr><td style=\"vertical-align: top;\"><strong>Country: </strong></td><td>" + contact.country + "</td></tr>";
                    }
                    htmlBody += "<tr><td style=\"vertical-align: top;\"><strong>Subject: </strong></td><td>" + contact.subject + "</td></tr>";
                    htmlBody += "<tr><td style=\"vertical-align: top;\"><strong>Message: </strong></td><td>" + contact.message + "</td></tr></table>";
                    mail.Body = htmlBody;
                    SmtpServer.Send(mail);
                }



                return true;
            } catch { return false; }
        }

        public static bool ForgotPassword(Customer customer) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                SmtpClient SmtpServer = new SmtpClient("mail.curtmfg.com");
                SmtpServer.Port = 25;
                string contactuslink = "http://www.curtmfg.com/Contact";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("no-reply@curthitch.biz");
                mail.To.Add(customer.email);
                mail.Subject = "CURT Manufacturing - Account Password";
                mail.IsBodyHtml = true;
                string htmlBody = "<p>Hi there,</p>";
                htmlBody += "<p>Here is the password for your CURT Manufacturing Dealer Account.</p>";
                htmlBody += "<div style='border:2px solid #ccc;display:inline-block;padding:10px;padding-top:0px'>";
                htmlBody += "<p><strong>Password:</strong> " + customer.password + "</p></div>";
                htmlBody += "<p>Please <a href='" + contactuslink + "'>contact us</a> if you have further questions or problems logging in. Thanks.</p>";
                htmlBody += "<p>Sincerely,<br />CURT Support Staff</p>";
                mail.Body = htmlBody;
                SmtpServer.Send(mail);

                return true;
            } catch { return false; }
        }
    }
}