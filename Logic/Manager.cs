using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;
using RedditSharp.Things;

namespace Logic
{
    public class Manager
    {
        private Cache cache { get; } = Cache.Instance;
        private Subreddit subreddit;

        public Manager(List<string> supportedFormats) { }
        public Media getMedia() { }
        public List<Comment> getTopComments() { }
        public string getTitle() { }
        public string getUri() { }
        public string getSelfText() { }
        public string getSubreddit() { return subreddit.Name; }
        public void loadPrevContent() { }
        public void loadNextContent() { }
        public void setSubreddit(string subredditName)
        {
            subreddit = new Reddit().GetSubreddit($"/r/{subredditName}");
        }
    }
}
