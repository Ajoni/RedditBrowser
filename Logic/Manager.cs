using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Logic
{
    public class Manager
    {
        private Cache cache { get; } = Cache.Instance;
        private Subreddit subreddit;
        private List<Post> posts = new List<Post>();
        private int postIndex = -1;
        private bool lateMediaInit = false;
        private string[] supportedFormats;
        private IEnumerator<Post> newsetPost { get; set; }

        public Manager(string[] supportedFormats) { this.supportedFormats = supportedFormats; }
        public Manager(string[] supportedFormats, bool lateMediaInit) { this.supportedFormats = supportedFormats; this.lateMediaInit = lateMediaInit; }
        public List<Comment> GetTopComments(int amount) { return posts[postIndex].Comments.OrderByDescending(x => x.Score).Take(amount).ToList(); }
        public List<Comment> GetComments() { return posts[postIndex].Comments.OrderByDescending(x => x.Score).ToList(); }
        public Media GetCurrentMedia() { return cache.GetCurrent(); }
        public Post GetCurrentPost() { return posts[postIndex]; }
        public string GetTitle() { return posts[postIndex].Title; }
        public string GetUri() { return posts[postIndex].Url.ToString(); }
        public string GetSelfText() { return posts[postIndex].SelfText; }
        public string GetSubreddit() { return subreddit.Name; }
        public bool CanGetNext() { return newsetPost != null; }
        public bool CanGetPrevious() { return postIndex >0; }
        public bool SubredditIsOpen() { return subreddit != null; }

        public bool LoadNewPost() {
            if(!FindNextPostWithSupportedFormat()) return false;
            AddPostAndImageToCache();
            return true;
        }

        public bool SetSubreddit(string subredditName)
        {
            try
            {
                cache.invalidateCache();
                InvalidatePostsCache();

                subreddit = new Reddit().GetSubreddit($"/r/{subredditName}");
                if (subreddit == null) { return false; }
                var posts = subreddit.Posts;
                newsetPost = posts.GetEnumerator();
            }
            catch (WebException ) { return false; }
            
            return true;
        }

        public async Task<bool> SetSubredditAsync(string subredditName)
        {
            bool res;
            res =  await Task.Run(() =>
            {
                try
                {
                    bool result = true;
                    cache.invalidateCache();
                    InvalidatePostsCache();

                    subreddit = new Reddit().GetSubreddit($"/r/{subredditName}");
                    if (subreddit == null) { result =  false; }
                    newsetPost = subreddit.Posts.GetEnumerator();

                    return result;
                }
                    catch (WebException) { return false; }
            });

            return res;
        }

        public void UnsetSubreddit()
        {
            subreddit = null;
            newsetPost = null;
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
        /// <summary>
        /// If the media cache is invalidated and we're loading posts which exist in cache
        /// </summary>
        private void loadCurrentPostMedia()
        {
            var img = downloadImage(GetUri());
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
            // TODO: Different behaviours for different formats, e.g. .png vs .gif
            BitmapImage img;
            img = downloadImage(source);
            cache.addElem(new MediaBitmapImage(img));
        }

        private BitmapImage downloadImage(string source)
        {
            // let user handle init
            if (this.lateMediaInit)
                return new BitmapImage(new Uri(source, UriKind.Absolute));

            BitmapImage bitmap = new BitmapImage();

            using (var wc = new WebClient())
            {
                byte[] bytes = wc.DownloadData(source);
                using (var ms = new MemoryStream(bytes))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
            }

            return bitmap;
        }

        private void InvalidatePostsCache()
        {
            this.posts.Clear();
            postIndex = -1;
        }
    }
}
