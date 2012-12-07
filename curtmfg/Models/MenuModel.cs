using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace curtmfg.Models {
    public class MenuModel {
        public int websiteID {
            get {
                return 1;
            }
        }

        public Menu GetMenu(int id = 0) {
            Menu menu = new Menu();
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                menu = db.Menus.Where(x => x.menuID == id && x.websiteID.Equals(this.websiteID)).First<Menu>();
            } catch { }
            return menu;
        }

        public SimpleMenu GetPrimary() {
            SimpleMenu menu = new SimpleMenu();
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                menu = (from m in db.Menus
                        where m.isPrimary == true && m.websiteID.Equals(this.websiteID)
                        select new SimpleMenu {
                            menuID = m.menuID,
                            menu_name = m.menu_name,
                            display_name = m.display_name,
                            requireAuthentication = m.requireAuthentication,
                            isPrimary = m.isPrimary,
                            active = m.active,
                            contents = (from msc in db.Menu_SiteContents
                                           where msc.parentID == null && msc.menuID.Equals(m.menuID) && ((msc.menuTitle != null && msc.menuLink != null) ||
                                           (msc.contentID != null && (db.SiteContents.Where(x => x.contentID == msc.contentID).Where(x => x.published == true)).Single() != null))
                                           orderby msc.menuSort
                                           select new menuItem {
                                               menuContentID = msc.menuContentID,
                                               menuID = msc.menuID,
                                               menuSort = msc.menuSort,
                                               menuTitle = msc.menuTitle,
                                               menuLink = msc.menuLink,
                                               linkTarget = msc.linkTarget,
                                               parentID = msc.parentID,
                                               contentID = msc.contentID,
                                               content = (from sc in db.SiteContents
                                                          where sc.contentID.Equals(msc.contentID)
                                                          select new ContentPage {
                                                              contentID = sc.contentID,
                                                              page_title = sc.page_title,
                                                              content_type = sc.content_type,
                                                              lastModified = sc.lastModified,
                                                              createdDate = sc.createdDate,
                                                              published = sc.published,
                                                              meta_title = sc.meta_title,
                                                              meta_description = sc.meta_description,
                                                              keywords = sc.keywords,
                                                              active = sc.active,
                                                              isPrimary = sc.isPrimary,
                                                              slug = sc.slug,
                                                              revision = (db.SiteContentRevisions.Where(x => x.contentID == sc.contentID).Where(x => x.active == true).First<SiteContentRevision>())
                                                          }).FirstOrDefault<ContentPage>()
                                           }).ToList<menuItem>()
                        }).First<SimpleMenu>();

                return menu;
            } catch { return menu; }
        }

        public List<SimpleMenu> GetFooterSitemap() {
            List<SimpleMenu> menus = new List<SimpleMenu>();
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                menus = (from m in db.Menus
                         where m.showOnSitemap == true && m.websiteID.Equals(this.websiteID)
                        orderby m.sort
                        select new SimpleMenu {
                            menuID = m.menuID,
                            menu_name = m.menu_name,
                            display_name = m.display_name,
                            requireAuthentication = m.requireAuthentication,
                            isPrimary = m.isPrimary,
                            active = m.active,
                            contents = (from msc in db.Menu_SiteContents
                                        where msc.parentID == null && msc.menuID.Equals(m.menuID) && ((msc.menuTitle != null && msc.menuLink != null) ||
                                        (msc.contentID != null && (db.SiteContents.Where(x => x.contentID == msc.contentID).Where(x => x.published == true)).Single() != null))
                                        orderby msc.menuSort
                                        select new menuItem {
                                            menuContentID = msc.menuContentID,
                                            menuID = msc.menuID,
                                            menuSort = msc.menuSort,
                                            menuTitle = msc.menuTitle,
                                            menuLink = msc.menuLink,
                                            linkTarget = msc.linkTarget,
                                            parentID = msc.parentID,
                                            contentID = msc.contentID,
                                            content = (from sc in db.SiteContents
                                                       where sc.contentID.Equals(msc.contentID)
                                                       select new ContentPage {
                                                           contentID = sc.contentID,
                                                           page_title = sc.page_title,
                                                           content_type = sc.content_type,
                                                           lastModified = sc.lastModified,
                                                           createdDate = sc.createdDate,
                                                           published = sc.published,
                                                           meta_title = sc.meta_title,
                                                           meta_description = sc.meta_description,
                                                           keywords = sc.keywords,
                                                           active = sc.active,
                                                           isPrimary = sc.isPrimary,
                                                           slug = sc.slug,
                                                           revision = (db.SiteContentRevisions.Where(x => x.contentID == sc.contentID).Where(x => x.active == true).First<SiteContentRevision>())
                                                       }).FirstOrDefault<ContentPage>()
                                        }).ToList<menuItem>()
                        }).ToList<SimpleMenu>();
            } catch {}
            return menus;
        }

        public menuWithContent Get(string name = "") {
            menuWithContent menu = new menuWithContent();
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                menu = (from m in db.Menus
                        where m.menu_name.ToLower() == name.ToLower() && m.websiteID.Equals(this.websiteID)
                        select new menuWithContent {
                            menuID = m.menuID,
                            menu_name = m.menu_name,
                            display_name = m.display_name,
                            requireAuthentication = m.requireAuthentication,
                            isPrimary = m.isPrimary,
                            active = m.active
                        }).First<menuWithContent>();

                List<menuItem> contents = (from msc in db.Menu_SiteContents
                                           where msc.menuID.Equals(menu.menuID) && ((msc.menuTitle != null && msc.menuLink != null) ||
                                           (msc.contentID != null && (db.SiteContents.Where(x => x.contentID == msc.contentID).Where(x => x.published == true)).Single() != null))
                                           orderby msc.parentID, msc.menuSort
                                           select new menuItem {
                                               menuContentID = msc.menuContentID,
                                               menuID = msc.menuID,
                                               menuSort = msc.menuSort,
                                               menuTitle = msc.menuTitle,
                                               menuLink = msc.menuLink,
                                               linkTarget = msc.linkTarget,
                                               parentID = msc.parentID,
                                               contentID = msc.contentID,
                                               content = (from sc in db.SiteContents
                                                          where sc.contentID.Equals(msc.contentID)
                                                          select new ContentPage {
                                                              contentID = sc.contentID,
                                                              page_title = sc.page_title,
                                                              content_type = sc.content_type,
                                                              lastModified = sc.lastModified,
                                                              createdDate = sc.createdDate,
                                                              published = sc.published,
                                                              meta_title = sc.meta_title,
                                                              meta_description = sc.meta_description,
                                                              keywords = sc.keywords,
                                                              active = sc.active,
                                                              isPrimary = sc.isPrimary,
                                                              slug = sc.slug,
                                                              revision = (db.SiteContentRevisions.Where(x => x.contentID == sc.contentID).Where(x => x.active == true).First<SiteContentRevision>())
                                                          }).FirstOrDefault<ContentPage>()
                                           }).ToList<menuItem>();
                menu.contents = contents.ToLookup(k => (k.parentID == null) ? 0 : k.parentID);
                return menu;
            } catch { return menu; }
        }

        public List<menuWithContent> GetSitemap() {
            List<menuWithContent> menus = new List<menuWithContent>();
            List<menuWithContent> remove = new List<menuWithContent>();
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                menus = (from m in db.Menus
                         where m.websiteID.Equals(this.websiteID)
                         orderby m.isPrimary descending
                         select new menuWithContent {
                             menuID = m.menuID,
                             menu_name = m.menu_name,
                             display_name = m.display_name,
                             requireAuthentication = m.requireAuthentication,
                             isPrimary = m.isPrimary,
                             active = m.active
                         }).ToList<menuWithContent>();

                foreach (menuWithContent menu in menus) {
                    List<menuItem> contents = (from msc in db.Menu_SiteContents
                                               where msc.menuID.Equals(menu.menuID) && (msc.contentID != null && (db.SiteContents.Where(x => x.contentID == msc.contentID).Where(x => x.published == true).Where(x => x.requireAuthentication == false).Single() != null))
                                               orderby msc.parentID, msc.menuSort
                                               select new menuItem {
                                                   menuContentID = msc.menuContentID,
                                                   menuID = msc.menuID,
                                                   menuSort = msc.menuSort,
                                                   menuTitle = msc.menuTitle,
                                                   menuLink = msc.menuLink,
                                                   linkTarget = msc.linkTarget,
                                                   parentID = msc.parentID,
                                                   contentID = msc.contentID,
                                                   content = (from sc in db.SiteContents
                                                              where sc.contentID.Equals(msc.contentID)
                                                              select new ContentPage {
                                                                  contentID = sc.contentID,
                                                                  page_title = sc.page_title,
                                                                  content_type = sc.content_type,
                                                                  lastModified = sc.lastModified,
                                                                  createdDate = sc.createdDate,
                                                                  published = sc.published,
                                                                  meta_title = sc.meta_title,
                                                                  meta_description = sc.meta_description,
                                                                  keywords = sc.keywords,
                                                                  active = sc.active,
                                                                  isPrimary = sc.isPrimary,
                                                                  slug = sc.slug,
                                                                  revision = (db.SiteContentRevisions.Where(x => x.contentID == sc.contentID).Where(x => x.active == true).First<SiteContentRevision>())
                                                              }).FirstOrDefault<ContentPage>()
                                               }).ToList<menuItem>();
                    if (contents.Count > 0) {
                        menu.contents = contents.ToLookup(k => (k.parentID == null) ? 0 : k.parentID);
                    } else {
                        remove.Add(menu);
                    }
                }
                foreach (menuWithContent m in remove) {
                    menus.Remove(m);
                }
            } catch { }
            return menus;
        }
        
        public menuWithContent GetByContentID(int contentID = 0, int id = 0, bool authenticated = false) {
            menuWithContent menu = new menuWithContent();
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                if (id == 0) {
                    id = db.Menu_SiteContents.Where(x => x.contentID == contentID).Select(x => x.menuID).FirstOrDefault();
                }
                menu = (from m in db.Menus
                        where m.menuID == id && m.websiteID.Equals(this.websiteID) && m.isPrimary == false && (!m.requireAuthentication || (m.requireAuthentication && authenticated))
                        select new menuWithContent {
                            menuID = m.menuID,
                            menu_name = m.menu_name,
                            display_name = m.display_name,
                            requireAuthentication = m.requireAuthentication,
                            isPrimary = m.isPrimary,
                            active = m.active
                        }).First<menuWithContent>();

                List<menuItem> contents = (from msc in db.Menu_SiteContents
                                            where msc.menuID.Equals(menu.menuID) && ((msc.menuTitle != null && msc.menuLink != null) ||
                                            (msc.contentID != null && (db.SiteContents.Where(x => x.contentID == msc.contentID).Where(x => x.published == true).Where(x => x.active == true)).Single() != null))
                                            orderby msc.parentID, msc.menuSort
                                            select new menuItem {
                                                menuContentID = msc.menuContentID,
                                                menuID = msc.menuID,
                                                menuSort = msc.menuSort,
                                                menuTitle = msc.menuTitle,
                                                menuLink = msc.menuLink,
                                                linkTarget = msc.linkTarget,
                                                parentID = msc.parentID,
                                                contentID = msc.contentID,
                                                content = (from sc in db.SiteContents
                                                            where sc.contentID.Equals(msc.contentID)
                                                            select new ContentPage {
                                                                contentID = sc.contentID,
                                                                page_title = sc.page_title,
                                                                content_type = sc.content_type,
                                                                lastModified = sc.lastModified,
                                                                createdDate = sc.createdDate,
                                                                published = sc.published,
                                                                meta_title = sc.meta_title,
                                                                meta_description = sc.meta_description,
                                                                keywords = sc.keywords,
                                                                active = sc.active,
                                                                isPrimary = sc.isPrimary,
                                                                slug = sc.slug,
                                                                revision = (db.SiteContentRevisions.Where(x => x.contentID == sc.contentID).Where(x => x.active == true).First<SiteContentRevision>())
                                                            }).FirstOrDefault<ContentPage>()
                                            }).ToList<menuItem>();
                menu.contents = contents.ToLookup(k => (k.parentID == null) ? 0 : k.parentID);
                return menu;
            } catch { return menu; }
        }
    }

    public class SimpleMenu : Menu {
        public List<menuItem> contents { get; set; }
    }

    public class menuWithContent : Menu {
        public ILookup<int?, menuItem> contents { get; set; }

        public bool hasChildren(int parentID = 0) {
            if (this.contents != null && this.contents.Contains(parentID)) {
                return true;
            }
            return false;
        }
        public int getChildrenCount(int parentID = 0) {
            if (this.contents != null && this.contents.Contains(parentID)) {
                return this.contents[parentID].Count();
            }
            return 0;
        }
        public string getChildrenIDs(int parentID = 0) {
            if (this.contents != null && this.contents.Contains(parentID)) {
                string childlist = "";
                List<menuItem> children = getChildren(parentID);
                for (int i = 0; i < children.Count(); i++) {
                    if (i != 0) {
                        childlist += ",";
                    }
                    childlist += children[i].menuContentID;
                }
                return childlist;
            }
            return "";
        }
        public List<menuItem> getChildren(int parentID = 0) {
            if (hasChildren()) {
                return this.contents[parentID].ToList<menuItem>();
            }
            return new List<menuItem>();
        }
    }

    public class menuItem : Menu_SiteContent {
        public ContentPage content { get; set; }

        public bool hasContent() {
            if (this.content != null) {
                return true;
            }
            return false;
        }
    }
}