using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Media.Imaging;

namespace Logic
{
    public class Manager
    {
        private Cache cache { get; } = Cache.Instance;
        private Subreddit subreddit;
        private List<Post> posts = new List<Post>();
        private int postIndex = -1;
        private string[] supportedFormats;
        private IEnumerator<Post> newsetPost { get; set; }

        public Manager(string[] supportedFormats) { this.supportedFormats = supportedFormats; }
        public List<Comment> GetTopComments(int amount) { return posts[postIndex].Comments.Take(amount).ToList(); }
        public Media GetCurrent() { return cache.GetCurrent(); }
        public string GetTitle() { return posts[postIndex].Title; }
        public string GetUri() { return posts[postIndex].Url.ToString(); }
        public string GetSelfText() { return posts[postIndex].SelfText; }
        public string GetSubreddit() { return subreddit.Name; }
        public bool canGetNext() { return newsetPost != null; }
        public bool canGetPrevious() { return postIndex >0; }

        public bool LoadNewPost() {
            if(!FindNextPostWithSupportedFormat()) return false;
            AddPostAndImageToCache();
            return true;
        }

        public bool SetSubreddit(string subredditName)
        {
            try {
                subreddit = new Reddit().GetSubreddit($"/r/{subredditName}");
                if(subreddit == null) { return false; }
                var posts = subreddit.Posts;
                newsetPost = posts.GetEnumerator();
            }
            catch(WebException e) { return false; }
            
            return true;
        }

        public bool Next()
        {
            postIndex++;
            //we are not at the end of loaded posts
            if(postIndex < posts.Count)
            {                
                if (!cache.hasNext())//cache has been invalidated so we have to load uri again
                {
                    loadCurrentPostMedia();
                }
                    cache.Next();
                return true;
            }
            else
            {
                var result = LoadNewPost();
                if(result)
                    cache.Next();
                return result;
            }
        }

        private void loadCurrentPostMedia()
        {
            var img = new BitmapImage(new Uri(GetUri(), UriKind.Absolute));
            cache.addElem(new MediaBitmapImage(img));
        }

        public void Previous()
        {
            postIndex--;
            if (!cache.hasPrevious())
            {
                cache.invalidateCache(); //index -1
                loadCurrentPostMedia(); 
                cache.Next(); //index 0
            }
            else
            {
                cache.Previous();
            }

        }

        private bool FindNextPostWithSupportedFormat()
        {
            newsetPost.MoveNext();
            int times = 1;

            while (!newsetPostHasSupportedFormat())
            {
                newsetPost.MoveNext();
                times++;
                if (times > 10)
                    return false;
            }
            return true;
        }

        private bool newsetPostHasSupportedFormat()
        {
            return supportedFormats.Any(s => newsetPost.Current.Url.ToString().Contains(s));
        }

        private void AddPostAndImageToCache()
        {
            posts.Add(newsetPost.Current);

            string source = newsetPost.Current.Url.ToString();
            var img = new BitmapImage(new Uri(source, UriKind.Absolute));
            cache.addElem(new MediaBitmapImage(img));
        }

    }
}
