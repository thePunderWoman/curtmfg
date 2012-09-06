using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using curtmfg.Models;
using System.Net.Mail;

namespace curtmfg.Models {
    public class ForumModel {
        public static List<FullGroup> GetAllGroups() {
            List<FullGroup> groups = new List<FullGroup>();
            try {
                CurtDevDataContext db = new CurtDevDataContext();

                groups = (from fg in db.ForumGroups
                          orderby fg.name
                          select new FullGroup {
                              forumGroupID = fg.forumGroupID,
                              name = fg.name,
                              description = fg.description,
                              createdDate = fg.createdDate,
                              topics = GetAllTopics(fg.forumGroupID)
                          }).ToList<FullGroup>();
            } catch (Exception e) { }

            return groups;
        }

        public static FullGroup GetGroup(int groupID = 0) {
            FullGroup group = new FullGroup();
            try {
                CurtDevDataContext db = new CurtDevDataContext();

                group = (from fg in db.ForumGroups
                          where fg.forumGroupID.Equals(groupID)
                          orderby fg.name
                          select new FullGroup {
                              forumGroupID = fg.forumGroupID,
                              name = fg.name,
                              description = fg.description,
                              createdDate = fg.createdDate,
                              topics = GetAllTopics(fg.forumGroupID)
                          }).First<FullGroup>();
            } catch (Exception e) { }

            return group;
        }

        public static List<FullTopic> GetAllTopics(int groupID = 0) {
            List<FullTopic> topics = new List<FullTopic>();
            try {
                CurtDevDataContext db = new CurtDevDataContext();

                topics = (from ft in db.ForumTopics
                          where ft.TopicGroupID.Equals(groupID) && ft.active == true
                            orderby ft.name
                            select new FullTopic {
                                topicID = ft.topicID,
                                TopicGroupID = ft.TopicGroupID,
                                name = ft.name,
                                description = ft.description,
                                image = ft.image,
                                createdDate = ft.createdDate,
                                active = ft.active,
                                closed = ft.closed,
                                count = db.ForumThreads.Where(x => x.topicID == ft.topicID).Count()
                            }).ToList<FullTopic>();
            } catch (Exception e) { }

            return topics;
        }

        public static FullTopic GetTopic(int topicID = 0, int page = 1, int perpage = 10) {
            FullTopic topic = new FullTopic();
            try {
                CurtDevDataContext db = new CurtDevDataContext();

                topic = (from ft in db.ForumTopics
                         where ft.topicID.Equals(topicID) && ft.active == true
                          orderby ft.name
                          select new FullTopic {
                              topicID = ft.topicID,
                              TopicGroupID = ft.TopicGroupID,
                              name = ft.name,
                              description = ft.description,
                              image = ft.image,
                              createdDate = ft.createdDate,
                              active = ft.active,
                              closed = ft.closed,
                              threads = GetAllThreads(ft.topicID,page,perpage)
                          }).First<FullTopic>();
            } catch (Exception e) { }

            return topic;
        }

        public static int GetTopicDiscussionCount(int topicID = 0) {
            CurtDevDataContext db = new CurtDevDataContext();

            int count = db.ForumThreads.Where(x => x.topicID == topicID).Where(x => x.active == true).Count();

            return count;
        }

        public static List<Thread> GetAllThreads(int topicID = 0, int page = 1, int perpage = 10) {
            List<Thread> threads = new List<Thread>();
            try {
                CurtDevDataContext db = new CurtDevDataContext();

                int skip = (page - 1) * perpage;

                threads = (from ft in db.ForumThreads
                           where ft.topicID.Equals(topicID) && ft.active == true
                           select new Thread {
                               threadID = ft.threadID,
                               topicID = ft.topicID,
                               createdDate = ft.createdDate,
                               active = ft.active,
                               closed = ft.closed,
                               count = db.ForumPosts.Where(x => x.threadID == ft.threadID).Where(x => x.active == true).Where(x => x.approved == true).Where(x => x.flag == false).Count(),
                               latestPost = db.ForumPosts.Where(x => x.threadID == ft.threadID).Where(x => x.active == true).Where(x => x.approved == true).Where(x => x.flag == false).OrderByDescending(x => x.createdDate).FirstOrDefault<ForumPost>(),
                               firstPost = db.ForumPosts.Where(x => x.threadID == ft.threadID).Where(x => x.active == true).Where(x => x.approved == true).Where(x => x.flag == false).OrderBy(x => x.createdDate).FirstOrDefault<ForumPost>()
                           }).OrderByDescending(x => x.latestPost.createdDate).Skip(skip).Take(perpage).ToList<Thread>();
            } catch (Exception e) { }

            return threads;
        }

        public static Thread GetThread(int threadID = 0,int page = 1, int perpage = 10) {
            Thread thread = new Thread();
            try {
                CurtDevDataContext db = new CurtDevDataContext();

                thread = (from ft in db.ForumThreads
                          where ft.threadID.Equals(threadID) && ft.active == true
                           select new Thread {
                               threadID = ft.threadID,
                               topicID = ft.topicID,
                               createdDate = ft.createdDate,
                               active = ft.active,
                               closed = ft.closed,
                               latestPost = db.ForumPosts.Where(x => x.threadID == ft.threadID).Where(x => x.active == true).Where(x => x.approved == true).Where(x => x.flag == false).OrderByDescending(x => x.createdDate).FirstOrDefault<ForumPost>(),
                               firstPost = db.ForumPosts.Where(x => x.threadID == ft.threadID).Where(x => x.active == true).Where(x => x.approved == true).Where(x => x.flag == false).OrderBy(x => x.createdDate).FirstOrDefault<ForumPost>(),
                               posts = GetPostsByThread(ft.threadID, page, perpage),
                           }).First<Thread>();
            } catch (Exception e) { }

            return thread;
        }
        
        public static List<Post> GetPostsByThread(int threadID = 0, int page = 1, int perpage = 10) {
            List<Post> threads = new List<Post>();
            try {
                CurtDevDataContext db = new CurtDevDataContext();

                int skip = (page - 1) * perpage;

                threads = (from p in db.ForumPosts
                           where p.threadID.Equals(threadID) && p.active == true && p.flag == false && p.approved == true
                           orderby p.sticky descending, p.createdDate
                           select new Post {
                               postID = p.postID,
                               parentID = p.parentID,
                               createdDate = p.createdDate,
                               title = p.title,
                               slug = UDF.GenerateSlug(p.title),
                               post = p.post,
                               name = p.name,
                               company = p.company,
                               email = p.email,
                               IPAddress = p.IPAddress,
                               notify = p.notify,
                               active = p.active,
                               approved = p.approved,
                               flag = p.flag,
                               sticky = p.sticky,
                               posts = GetPostsByPost(p.postID)
                           }).Skip(skip).Take(perpage).ToList<Post>();
            } catch (Exception e) { }

            return threads;
        }

        public static int GetDiscussionPostCount(int threadID = 0) {
            CurtDevDataContext db = new CurtDevDataContext();

            int count = db.ForumPosts.Where(x => x.threadID == threadID).Where(x => x.active == true).Where(x => x.flag == false).Where(x => x.approved == true).Count();

            return count;
        }

        public static List<Post> GetPostsByPost(int postID = 0) {
            List<Post> threads = new List<Post>();
            try {
                CurtDevDataContext db = new CurtDevDataContext();

                threads = (from p in db.ForumPosts
                           where p.parentID.Equals(postID) && p.active == true && p.approved == true && p.flag == false
                           orderby p.sticky descending, p.createdDate descending
                           select new Post {
                               postID = p.postID,
                               parentID = p.parentID,
                               createdDate = p.createdDate,
                               title = p.title,
                               slug = UDF.GenerateSlug(p.title),
                               post = p.post,
                               name = p.name,
                               company = p.company,
                               email = p.email,
                               IPAddress = p.IPAddress,
                               notify = p.notify,
                               active = p.active,
                               approved = p.approved,
                               flag = p.flag,
                               sticky = p.sticky,
                               posts = GetPostsByPost(p.postID)
                           }).ToList<Post>();
            } catch (Exception e) { }

            return threads;
        }

        public static Post GetPost(int postID = 0) {
            Post post = new Post();
            try {
                CurtDevDataContext db = new CurtDevDataContext();

                post = (from p in db.ForumPosts
                        where p.postID.Equals(postID) && p.active == true && p.approved == true && p.flag == false
                           orderby p.createdDate descending
                           select new Post {
                               postID = p.postID,
                               threadID = p.threadID,
                               parentID = p.parentID,
                               createdDate = p.createdDate,
                               date = String.Format("{0:dddd, MMMM d, yyyy} at {0: h:mm tt}", p.createdDate),
                               title = p.title,
                               slug = UDF.GenerateSlug(p.title),
                               post = p.post,
                               name = p.name,
                               company = p.company,
                               email = p.email,
                               IPAddress = p.IPAddress,
                               notify = p.notify,
                               active = p.active,
                               approved = p.approved,
                               flag = p.flag,
                               sticky = p.sticky,
                               posts = GetPostsByPost(p.postID)
                           }).First<Post>();
            } catch (Exception e) { }

            return post;
        }

    }

    public class FullGroup : ForumGroup {
        public List<FullTopic> topics { get; set; }
    }

    public class FullTopic : ForumTopic {
        public List<Thread> threads { get; set; }
        public int count { get; set; }
    }

    public class Thread : ForumThread {
        public List<Post> posts { get; set; }
        public int count { get; set; }
        public ForumPost firstPost { get; set; }
        public ForumPost latestPost { get; set; }
    }

    public class Post : ForumPost {
        public List<Post> posts { get; set; }
        public string date { get; set; }
        public string slug { get; set; }

        public string getName() {
            if (this.name.Trim() == "") {
                return "Anonymous";
            }
            return this.name;
        }
    }
}