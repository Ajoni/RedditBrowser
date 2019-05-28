using GalaSoft.MvvmLight.Command;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using static RedditSharp.Things.VotableThing;

namespace RedditBrowser.Classes
{
    public class LoadedPost : INotifyPropertyChanged
    {
        private bool? _liked;
        private int _score;
        private bool showFullImage;

        public bool ShowFullResolutionImage
        {
            get => showFullImage; set
            {
                showFullImage = value; OnPropertyChanged();
            }
        }

        public ObservableCollection<Comment> Comments { get; } = new ObservableCollection<Comment>();
        public string LinkFlairText { get; set; }
        public RedditUser Author { get; }
        public Uri Permalink { get; set; }
        public string SelfText { get; set; }
        public Uri Thumbnail { get; set; }
        public string Title { get; set; }
        public Uri Url { get; set; }
        public int Score
        {
            get => _score; set
            {
                _score = value; OnPropertyChanged();
            }
        }
        public DateTimeOffset Created { get; set; }
        public bool? Liked
        {
            get => _liked; set
            {
                _liked = value; OnPropertyChanged();
            }
        }
        public Post Post { get; set; }

        public LoadedPost(Post post)
        {
            ShowFullResolutionImage = false;
            foreach (var comment in post.Comments)
                Comments.Add(comment);
            LinkFlairText = post.LinkFlairText;
            Author = post.Author;
            Permalink = post.Permalink;
            SelfText = post.SelfText;
            Thumbnail = post.Thumbnail;
            Title = post.Title;
            Url = post.Url;
            Score = post.Score;
            Created = post.Created;
            Liked = post.Liked;
            Post = post;
        }

        public ICommand SaveImageCommand
        {
            get
            {
                return new RelayCommand(() => {
                    var split = Url.LocalPath.Split('.');
                    string extension = split[split.Length - 1];

                    var dialog = new SaveFileDialog();
                    dialog.Filter = "A media file | *." + extension;
                    dialog.ShowDialog();
                    var fileName = dialog.FileName;
                    if (fileName != null)
                    {
                        Task.Run(() => { Save(Url, fileName); });
                    }
                });
            }
        }


        public void Downvote()
        {
            Post.Downvote();
            UpdateVoteProps();
        }
        public void Upvote()
        {
            Post.Upvote();
            UpdateVoteProps();
        }
        public void SetVote(VoteType type)
        {
            Post.SetVote(type);
            UpdateVoteProps();
        }
        public void ClearVote()
        {
            Post.ClearVote();
            UpdateVoteProps();
        }
        private void UpdateVoteProps()
        {
            Score = Post.Score;
            Liked = Post.Liked;
        }

        public void Comment(string message)
        {
            this.Post.Comment(message);
            this.Comments.Clear();
            foreach (var comment in this.Post.Comments)
                Comments.Add(comment);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Save(Uri uri, string fileName)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.DownloadFile(Url, fileName);
                }
                catch
                {
                    MessageBox.Show("There was an error while downloading file.",
                        "Please try again.",
                        MessageBoxButtons.OK);
                }
            }
        }
    }
}
