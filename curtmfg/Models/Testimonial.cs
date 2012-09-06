using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace curtmfg.Models {
    public class TestimonialModel {

        public static List<Testimonial> GetAll(int page = 1, int pageSize = 10) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                return db.Testimonials.Where(x => x.active == true).Where(x => x.approved == true).OrderByDescending(x => x.dateAdded).Skip((page - 1) * pageSize).Take(pageSize).ToList<Testimonial>();
            } catch (Exception e) {
                return new List<Testimonial>();
            }
        }

        public static List<Testimonial> GetRandomFive() {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                List<Testimonial> testimonials = UDF.ShuffleTestimonials(db.Testimonials.Where(x => x.active == true).Where(x => x.approved == true).ToList<Testimonial>()).AsQueryable().Take(5).ToList<Testimonial>();
                return testimonials;
            } catch (Exception e) {
                return new List<Testimonial>();
            }
        }

        public static int CountAll() {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                return db.Testimonials.Where(x => x.active == true).Where(x => x.approved == true).Count();
            } catch (Exception e) {
                return 0;
            }
        }

        public static Testimonial Add(string first_name = "", string last_name = "", string location = "", string title = "", string testimonial = "", double rating = 0) {
            Testimonial t = new Testimonial();
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                t = new Testimonial {
                    testimonial1 = testimonial,
                    first_name = first_name,
                    last_name = last_name,
                    location = location,
                    title = title,
                    rating = rating,
                    dateAdded = DateTime.Now,
                    approved = false,
                    active = true,
                };
                db.Testimonials.InsertOnSubmit(t);
                db.SubmitChanges();
                return t;

            } catch (Exception e) {
                return t;
            }
        }
    }
}