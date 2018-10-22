using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using RedditSharp.Things;

namespace RedditBrowserLogic.Mock
{
    public class Manager
    {
        private Cache cache = new Cache();
        private string[] supportedFormats;
        private int current = 0;

        public Manager(string[] supportedFormats)
        {
            this.supportedFormats = supportedFormats;
        }

        public void Next()
        {
            if (current < cache.medias.Count - 1)
            {
                current++;
            }
        }

        public void Previous()
        {
            if (current > 0)
            {
                current--;
            }
        }

        public Media GetCurrent()
        {
            return cache.GetCurrent(current);
        }

        public List<Comment> GetTopComments(int count)
        {
            List<Comment> comments = new List<Comment>();
            comments.Add(new Comment() { AuthorName = "Johnny", Body = "I eat paaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaint" });
            comments.Add(new Comment() { AuthorName = "Bonnie", Body = "I eat bananas" });
            comments.Add(new Comment() { AuthorName = "Sonny", Body = "I eat darling's cake" });
            return comments;
        }

    }

    class Cache
    {
        public List<Media> medias;

        public Cache()
        {
            medias = new List<Media>() { 
                new MediaBitmapImage(new BitmapImage( new Uri(Environment.CurrentDirectory + @"/sample1.png"))),
                new MediaBitmapImage(new BitmapImage( new Uri(Environment.CurrentDirectory + @"/sample2.jpg"))),
                new MediaBitmapImage(new BitmapImage( new Uri(Environment.CurrentDirectory + @"/sample4.jpg"))),
                new MediaGIF(Image.FromFile(Environment.CurrentDirectory.ToString() + @"/sample3.gif"))
                };
        }

        public Media GetCurrent(int index)
        {
            return medias[index];
        }
    }

    public abstract class Media
    {
        
    }

    public class MediaBitmapImage : Media
    {
        public BitmapImage image;

        public MediaBitmapImage(BitmapImage image)
        {
            this.image = image;
        }
    }

    public class MediaGIF : Media
    {
        public Image image;

        public MediaGIF(Image image)
        {
            this.image = image;
        }
    }    
}
