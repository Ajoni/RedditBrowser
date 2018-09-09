using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public string subredditName { get; set; }
        public Subreddit subreddit { get; set; }
        int postNr { get; set; } = -1;
        public IEnumerator<Post> it { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectSubDialog dialog = new SelectSubDialog();
            dialog.ShowDialog();
            if (dialog.DialogResult != true)
            {
                //add some sort of msg to gui here
                return;
            }
            subredditName = dialog.subName;
            subreddit = new Reddit().GetSubreddit($"/r/{subredditName}");
            var posts = subreddit.Posts;
            it = posts.GetEnumerator();
            loadNextImg();
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            loadNextImg();
        }

        private void loadNextImg()
        {
            it.MoveNext(); postNr++;
            statusLabel.Content = "Skipping non jpg posts";
            while (it.Current.Url.ToString().Contains(".jpg") != true)
            {
                it.MoveNext(); postNr++;
            }
            statusLabel.Content = "Loading img";
            string source = it.Current.Url.ToString();
            meme.Source = new BitmapImage(new Uri(source, UriKind.Absolute));
            statusLabel.Content = "";
        }
    }
}
