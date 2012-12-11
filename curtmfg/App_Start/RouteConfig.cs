using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace curtmfg {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "LegecyCatch",
                url: "index.cfm",
                defaults: new { controller = "Legacy", action = "legacyRedirect" }
            );

            routes.MapRoute(
                name: "LandingPage",
                url: "lp/{id}",
                defaults: new { controller = "LandingPage", action = "Index", id = 0 }
            );

            routes.MapRoute(
                name: "LandingPageWithName",
                url: "lp/{id}/{name}",
                defaults: new { controller = "LandingPage", action = "Index", id = 0, name = "" }
            );

            routes.MapRoute(
                name: "TechSupportContact",
                url: "technical_service",
                defaults: new { controller = "Contact", action = "Index", page = "technical_service", defaultvalue = "Tech Services" }
            );

            routes.MapRoute(
                name: "ForumTopic",
                url: "Forum/Topic/{id}/{name}",
                defaults: new { controller = "Forum", action = "Topic", id = UrlParameter.Optional, name = "" }
            );

            routes.MapRoute(
                name: "ForumDiscussion",
                url: "Forum/Discussion/{id}/{name}",
                defaults: new { controller = "Forum", action = "Discussion", id = UrlParameter.Optional, name = "" }
            );

            // http://localhost/Blog/Archive/January/2011, page is optional
            routes.MapRoute(
                name: "BlogArchive",
                url: "Blog/Archive/{month}/{year}",
                defaults: new { controller = "Blog", action = "ViewArchive", month = "", year = "" }
            );

            // http://localhost/Blog/Author/Test_Testerson, page is optional
            routes.MapRoute(
                name: "BlogAuthor",
                url: "Blog/Author/{name}",
                defaults: new { controller = "Blog", action = "Author", name = "" }
            );

            // http://localhost/Blog/Category/Hitches/page/1, page is optional
            routes.MapRoute(
                name: "BlogFeed",
                url: "Blog/Feed/{type}",
                defaults: new { controller = "Blog", action = "Feed", feed = "rss" }
            );

            // http://localhost/Blog/Category/Hitches, page is optional
            routes.MapRoute(
                name: "BlogCategory",
                url: "Blog/Category/{name}",
                defaults: new { controller = "Blog", action = "ViewCategory", name = "" }
            );

            // http://localhost/Blog/Category/Hitches/page/1, page is optional
            routes.MapRoute(
                name: "BlogCategoryFeed",
                url: "Blog/Category/{name}/Feed/{type}",
                defaults: new { controller = "Blog", action = "CategoryFeed", name = "", type = "rss" }
            );

            // http://localhost/Blog/Post/Comment/1, the number is the post id
            routes.MapRoute(
                name: "BlogPostComment",
                url: "Blog/Post/Comment/{id}",
                defaults: new { controller = "Blog", action = "Comment", id = "", message = UrlParameter.Optional }
            );

            // http://localhost/Blog/8-24-2011/This+is+a+blog+post+title
            routes.MapRoute(
                name: "BlogPost",
                url: "Blog/Post/{date}/{title}",
                defaults: new { controller = "Blog", action = "ViewPost", date = "", title = "" }
            );

            routes.MapRoute(
                name: "NewsByName",
                url: "News/article/{date}/{title}",
                defaults: new { controller = "News", action = "Article", date = "", title = "" }
            );

            routes.MapRoute(
                name: "CategoryGetPage",
                url: "Category/GetNextPage",
                defaults: new { controller = "Category", action = "GetNextPage" }
            );

            routes.MapRoute(
                name: "CategoryGetInstallSheet",
                url: "Category/GetInstallSheet/{id}",
                defaults: new { controller = "Category", action = "GetInstallSheet", id = 0 }
            );

            routes.MapRoute(
                name: "CategoryGrid",
                url: "Category/Grid/{id}/{name}",
                defaults: new { controller = "Category", action = "Grid", id = UrlParameter.Optional, name = "" }
            );

            routes.MapRoute(
                name: "CategoryWithID",
                url: "Category/{id}/{name}",
                defaults: new { controller = "Category", action = "Index", id = UrlParameter.Optional, name = "" }
            );

            routes.MapRoute(
                name: "Lifestyles",
                url: "Lifestyles",
                defaults: new { controller = "Lifestyle", action = "index" }
            );

            routes.MapRoute(
                name: "LifestyleWithID",
                url: "Lifestyle/{id}/{name}",
                defaults: new { controller = "Lifestyle", action = "lifestyle", id = 0, name = "" }
            );

            routes.MapRoute(
                name: "Mount",
                url: "HitchLookup/Mount/{mount}",
                defaults: new { controller = "HitchLookup", action = "Mount", mount = "" }
            );

            routes.MapRoute(
                name: "Year",
                url: "HitchLookup/Mount/{mount}/Year/{year}",
                defaults: new { controller = "HitchLookup", action = "Year", year = "", mount = "" }
            );

            routes.MapRoute(
                name: "Make",
                url: "HitchLookup/Mount/{mount}/Year/{year}/Make/{make}",
                defaults: new { controller = "HitchLookup", action = "Make", year = "", make = "", mount = "" }
            );

            routes.MapRoute(
                name: "Model",
                url: "HitchLookup/Mount/{mount}/Year/{year}/Make/{make}/Model/{model}",
                defaults: new { controller = "HitchLookup", action = "Model", year = "", make = "", model = "", mount = "" }
            );

            routes.MapRoute(
                name: "Page",
                url: "page/{name}",
                defaults: new { controller = "Content", action = "Index", name = "" }
            );

            routes.MapRoute(
                name: "PageWithMenu",
                url: "page/{menuid}/{name}",
                defaults: new { controller = "Content", action = "Index", name = "", menuid = 0 }
            );

            routes.MapRoute(
                name: "DealerPage",
                url: "dealer/page/{name}",
                defaults: new { controller = "Dealer", action = "Page", name = "" }
            );

            routes.MapRoute(
                name: "LatestPartAjax",
                url: "part/getLatestPartsAjax",
                defaults: new { controller = "Part", action = "getLatestPartsAjax" }
            );

            routes.MapRoute(
                name: "PartReview",
                url: "part/AddReview/{id}",
                defaults: new { controller = "Part", action = "AddReview", id = 0 }
            );

            routes.MapRoute(
                name: "Part",
                url: "part/{id}",
                defaults: new { controller = "Part", action = "Index", id = 0 }
            );

            routes.MapRoute(
                name: "Sheet",
                url: "sheet/{id}",
                defaults: new { controller = "Sheet", action = "Index", id = 0 }
            );

            routes.MapRoute(
                name: "Hitch",
                url: "hitch/{id}",
                defaults: new { controller = "Part", action = "Index", id = 0 }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Index", action = "Index", id = UrlParameter.Optional }
            ); 
            
        }
    }
}