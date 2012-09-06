using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace curtmfg.Models {
    public class ReCaptcha {

        public static string GenerateCaptcha(string theme = "clean") {
            string captcha = "";
            string publickey = "6Levd8cSAAAAAGJDbZzk9hRRqm0ltuS8-TgwadQS";
            captcha += "<script type=\"text/javascript\">" +
                        "var RecaptchaOptions = { theme : '" + theme + "' };</script>";
            captcha += "<script type=\"text/javascript\"" +
                        " src=\"http://www.google.com/recaptcha/api/challenge?k=" + publickey + "\">" +
                        "</script>" +
                          "<noscript>" +
                             "<iframe src=\"http://www.google.com/recaptcha/api/noscript?k=" + publickey + "\"" +
                                 " height=\"300\" width=\"500\" frameborder=\"0\"></iframe><br>" +
                             "<textarea name=\"recaptcha_challenge_field\" rows=\"3\" cols=\"40\">" +
                             "</textarea>" +
                             "<input type=\"hidden\" name=\"recaptcha_response_field\"" +
                                 " value=\"manual_challenge\">" +
                          "</noscript>";
            return captcha;
        }

        public static bool ValidateCaptcha(string challenge = "", string apiresponse = "", string remoteip = "") {
            bool valid = false;
            string privatekey = "6Levd8cSAAAAAO_tjAPFuXbfzj6l5viTEaz5YjVv";

            string postdata = "privatekey=" + privatekey +
                                "&challenge=" + challenge +
                                "&response=" + apiresponse + 
                                "&remoteip=" + remoteip;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.google.com/recaptcha/api/verify");
            request.Method = "POST";
            ASCIIEncoding encoding=new ASCIIEncoding();
            byte[] postbytes = encoding.GetBytes(postdata);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postdata.Length;
            Stream postStream = request.GetRequestStream();
            postStream.Write(postbytes, 0, postbytes.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string responsestring = reader.ReadToEnd();
            postStream.Close();
            responseStream.Close();
            string[] responselines = responsestring.Split('\n');
            valid = Convert.ToBoolean(responselines[0]);

            return valid;
        }
    }
}