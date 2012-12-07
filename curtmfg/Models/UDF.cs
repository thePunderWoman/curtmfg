using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace curtmfg.Models {
    public class UDF {
        public static bool fileExists(string location) {
            WebRequest request = WebRequest.Create(location);
            request.Method = "HEAD";
            try {
                WebResponse response = request.GetResponse();
                return true;
            } catch {
                return false;
            }
        }

        public static string GenerateSlug(string phrase = "") {
            string str = RemoveAccent(phrase).ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars           
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            str = str.Trim(); // cut and trim it   
            str = Regex.Replace(str, @"\s", "_"); // underscores

            return str;
        }

        public static string RemoveAccent(string txt = "") {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static List<Customer> ShuffleList(List<Customer> list) {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                Customer value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public static List<DealerLocation> ShuffleLocations(List<DealerLocation> list) {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                DealerLocation value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public static List<Testimonial> ShuffleTestimonials(List<Testimonial> list) {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                Testimonial value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public static string generateMenu(menuWithContent menu, int parentid, int contentID) {
            string pagecontent = "<ul>";
            List<menuItem> menuitems = menu.getChildren(parentid);
            foreach (menuItem menuitem in menuitems) {
                pagecontent += "<li>";
                if (menuitem.hasContent()) {
                    pagecontent += "<a href='/page/" + menu.menuID + "/" + menuitem.content.slug + "' " + ((contentID == menuitem.contentID) ? "class='active'" : "") + " >" + menuitem.content.page_title + "</a>";
                } else {
                    pagecontent += "<a " + ((menuitem.linkTarget) ? "target=_blank" : "" ) + " href='" + menuitem.menuLink + "'>" + menuitem.menuTitle + "</a>";
                }
                if (menu.hasChildren(menuitem.menuContentID)) {
                    pagecontent += generateMenu(menu, menuitem.menuContentID, contentID);
                }
                pagecontent += "</li>";
            }
            pagecontent += "</ul>";
            return pagecontent;
        }

        public static string generateMenu(menuWithContent menu, int parentid) {
            string pagecontent = "<ul>";
            List<menuItem> menuitems = menu.getChildren(parentid);
            foreach (menuItem menuitem in menuitems) {
                pagecontent += "<li>";
                if (menuitem.hasContent()) {
                    pagecontent += "<a href='/page/" + menu.menuID + "/" + menuitem.content.slug + "' >" + menuitem.content.page_title + "</a>";
                } else {
                    pagecontent += "<a " + ((menuitem.linkTarget) ? "target=_blank" : "") + " href='" + menuitem.menuLink + "'>" + menuitem.menuTitle + "</a>";
                }
                if (menu.hasChildren(menuitem.menuContentID)) {
                    pagecontent += generateMenu(menu, menuitem.menuContentID);
                }
                pagecontent += "</li>";
            }
            pagecontent += "</ul>";
            return pagecontent;
        }

        public static string generateCategorySitemap(List<Category> categories, int parentID = 0) {
            string cats = "<ul>";
            List<Category> parentcats = categories.Where(x => x.parentID == parentID).OrderBy(x => x.sort).ToList<Category>();
            foreach(Category cat in parentcats) {
                cats += "<li><a href=\"/Category/" + cat.catID + "/" + cat.catTitle.Replace("/","|") + "\">" + cat.catTitle + "</a>";
                cats += generateCategorySitemap(categories, cat.catID);
                cats += "</li>";
            }
            cats += "</ul>";
            return cats;
        }

        public const string MatchEmailPattern =
                    @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
             + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
             + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
             + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        /// <summary>
        /// Checks whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="email">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and 
        /// contains a valid E-Mail address;
        /// otherwise false.</returns>
        public static bool IsEmail(string email) {
            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }
    }
}