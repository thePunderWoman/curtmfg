using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace curtmfg.Models {
    public class SiteContentModel {
        public int websiteID {
            get {
                return 1;
            }
        }

        public SiteContent GetByID(int id = 0) {
            SiteContent content = new SiteContent();
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                content = db.SiteContents.Where(x => x.contentID == id && x.websiteID.Equals(this.websiteID)).First<SiteContent>();
            } catch {}
            return content;
        }

        public ContentPage Get(string name = "", int menuid = 0, bool authenticated = false) {
            ContentPage content = new ContentPage();
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                content = (from s in db.SiteContents
                           where s.slug == name && s.published == true && s.active == true && s.websiteID.Equals(this.websiteID)
                           select new ContentPage {
                               contentID = s.contentID,
                               page_title = s.page_title,
                               content_type = s.content_type,
                               lastModified = s.lastModified,
                               createdDate = s.createdDate,
                               published = s.published,
                               meta_title = s.meta_title,
                               meta_description = s.meta_description,
                               keywords = s.keywords,
                               canonical = s.canonical,
                               active = s.active,
                               isPrimary = s.isPrimary,
                               slug = s.slug,
                               requireAuthentication = s.requireAuthentication,
                               revision = (db.SiteContentRevisions.Where(x => x.contentID == s.contentID).Where(x => x.active == true).First<SiteContentRevision>()),
                               menu = new MenuModel().GetByContentID(s.contentID, menuid, authenticated)
                           }).First<ContentPage>();
                return content;
            } catch (Exception e) { return content; }
        }

        public ContentPage GetPrimary() {
            ContentPage content = new ContentPage();
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                content = (from s in db.SiteContents
                           where s.isPrimary == true && s.websiteID.Equals(this.websiteID)
                           select new ContentPage {
                               contentID = s.contentID,
                               page_title = s.page_title,
                               content_type = s.content_type,
                               lastModified = s.lastModified,
                               createdDate = s.createdDate,
                               published = s.published,
                               meta_title = s.meta_title,
                               meta_description = s.meta_description,
                               canonical = s.canonical,
                               keywords = s.keywords,
                               active = s.active,
                               isPrimary = s.isPrimary,
                               slug = s.slug,
                               requireAuthentication = s.requireAuthentication,
                               revision = (db.SiteContentRevisions.Where(x => x.contentID == s.contentID).Where(x => x.active == true).First<SiteContentRevision>())
                           }).First<ContentPage>();
                return content;
            } catch { return content; }
        }

        public List<ContentPage> GetSitemap() {
            List<ContentPage> contents = new List<ContentPage>();
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                contents = (from s in db.SiteContents
                            where !(from msc in db.Menu_SiteContents select msc.contentID).Contains(s.contentID) && s.websiteID.Equals(this.websiteID)
                           select new ContentPage {
                               contentID = s.contentID,
                               page_title = s.page_title,
                               content_type = s.content_type,
                               lastModified = s.lastModified,
                               createdDate = s.createdDate,
                               published = s.published,
                               meta_title = s.meta_title,
                               meta_description = s.meta_description,
                               canonical = s.canonical,
                               keywords = s.keywords,
                               active = s.active,
                               isPrimary = s.isPrimary,
                               slug = s.slug,
                               requireAuthentication = s.requireAuthentication,
                               revision = (db.SiteContentRevisions.Where(x => x.contentID == s.contentID).Where(x => x.active == true).First<SiteContentRevision>())
                           }).ToList<ContentPage>();
                return contents;
            } catch { return contents; }
        }
    }

    public class ContentPage : SiteContent {
        public SiteContentRevision revision { get; set; }
        public menuWithContent menu { get; set; }
    }
}