using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace curtmfg.Models {
    public class FAQModel {

        public static List<FAQ> GetAll(string sorted_column = "", string sort_direction = "") {

            try {
                CurtDevDataContext db = new CurtDevDataContext();
                List<FAQ> faqs = new List<FAQ>();

                switch (sorted_column) {
                    // Sort by question
                    case "question":
                        switch (sort_direction) {
                            case "descending":
                                faqs = db.FAQs.OrderByDescending(x => x.question).ToList<FAQ>();
                                break;
                            default:
                                faqs = db.FAQs.OrderBy(x => x.question).ToList<FAQ>();
                                break;
                        }
                        break;

                    // Sort by answer
                    case "answer":
                        switch (sort_direction) {
                            case "descending":
                                faqs = db.FAQs.OrderByDescending(x => x.answer).ToList<FAQ>();
                                break;
                            default:
                                faqs = db.FAQs.OrderBy(x => x.answer).ToList<FAQ>();
                                break;
                        }
                        break;

                    // Sort by faqID
                    default:
                        switch (sort_direction) {
                            case "descending":
                                faqs = db.FAQs.OrderByDescending(x => x.faqID).ToList<FAQ>();
                                break;
                            default:
                                faqs = db.FAQs.OrderBy(x => x.faqID).ToList<FAQ>();
                                break;
                        }
                        break;

                }

                return faqs;
            } catch (Exception e) {
                return new List<FAQ>();
            }
        }
    }
}