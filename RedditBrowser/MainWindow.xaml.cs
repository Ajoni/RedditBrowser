using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RedditBrowser;
using RedditSharp;
using RedditSharp.Things;

namespace RedditBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string subredditName { get; set; }
        private Subreddit subreddit { get; set; }
        private int postIndex { get; set; } = -1;
        private IEnumerator<Post> newsetPost { get; set; }
        private List<Post> posts { get; set; } = new List<Post>();
        private List<BitmapImage> images { get; set; } = new List<BitmapImage>();
        private List<string> supportedFormats { get; set; } = new List<string>();


        public MainWindow()
        {
            InitializeComponent();

            AddSupportedFormats();

            // TODO: Add pan as well
            AddImageZoom();            
        }        

        private void AddSupportedFormats()
        {
            supportedFormats.Add(".jpg");
            supportedFormats.Add(".png");
        }

        private void AddImageZoom()
        {
            var group = new TransformGroup();
            var st = new ScaleTransform();
            group.Children.Add(st);
            image.RenderTransform = group;
        }

        private void loadPreviousImg()
        {
            postIndex--;
            DisplayCachedImageAtPostIndex();            
        }

        private void DisplayCachedImageAtPostIndex()
        {
            image.Source = images.ElementAt(postIndex);
            titleLabel.Content = posts.ElementAt(postIndex).Title;
        }

        /*
         * If currently displayed post is not the last one (being pointed to by the 'newsetPost.Current').
         */
        private void loadNextImg()
        {
            postIndex++;
            DisplayCachedImageAtPostIndex();
        }

        /*
         * If currently displayed post is the last one (being pointed to by the 'newsetPost.Current').
         */
        private void loadNewImg()
        {            
            FindNextPostWithSupportedFormat();

            AddPostAndImageToCache();

            loadNextImg();
        }        

        private void FindNextPostWithSupportedFormat()
        {
            newsetPost.MoveNext();
            while (!newsetPostHasSupportedFormat())
            {
                newsetPost.MoveNext();
            }
        }

        private void AddPostAndImageToCache()
        {
            posts.Add(newsetPost.Current);

            string source = newsetPost.Current.Url.ToString();
            var img = new BitmapImage(new Uri(source, UriKind.Absolute));
            images.Add(img);
        }

        private bool newsetPostHasSupportedFormat()
        {
            return supportedFormats.Any(s => newsetPost.Current.Url.ToString().Contains(s));
        }

        private void OpenSub_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenSub_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectSubDialog dialog = new SelectSubDialog();
            dialog.ShowDialog();
            if (dialog.DialogResult != true)
            {
                // TODO: Add some sort of msg to gui here.
                return;
            }

            subredditName = dialog.subName;
            subreddit = new Reddit().GetSubreddit($"/r/{subredditName}");
            var posts = subreddit.Posts;
            newsetPost = posts.GetEnumerator();

            loadNewImg();
        }

        private void Download_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (image != null)
            {
                e.CanExecute = image.Source != null;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void Download_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "Image Files (*.jpg)|*.jpg",
                FileName = newsetPost.Current.Id.ToString()
            };
            if (dialog.ShowDialog() == true)
            {
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }
        }

        private void Prev_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = postIndex > 0;
        }

        private void Prev_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            loadPreviousImg();
        }

        private void Next_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = newsetPost != null;
        }

        private void Next_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (postIndex < posts.Count - 1)
            {
                loadNextImg();
            }
            else
            {
                loadNewImg();
            }
        }

        private void ImgLink_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (image != null)
            {
                e.CanExecute = image.Source != null;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ImgLink_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(posts.ElementAt(postIndex).Url.ToString());
        }

        private void ShowButtons_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ShowButtons_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (Button item in ButtonsPanel.Children.OfType<Button>())
            {
                if (item.IsVisible)
                    item.Visibility = Visibility.Collapsed;
                else
                    item.Visibility = Visibility.Visible;
            }
        }

        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var st = (ScaleTransform)((TransformGroup)image.RenderTransform)
                .Children.First(x=>x is ScaleTransform);
            double zoom = e.Delta > 0 ? .1 : -.1;
            st.ScaleX += zoom;
            st.ScaleY += zoom;
        }
    }
}
