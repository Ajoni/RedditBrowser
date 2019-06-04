using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.ViewModel;
using RedditBrowser.ViewModel.Messages;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        public ObservableCollection<LoadedComment> Comments { get; } = new ObservableCollection<LoadedComment>();
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
                Comments.Add(new LoadedComment(comment));
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
        public ICommand ItemClick
        {
            get
            {
                return new RelayCommand(() =>
                {
                    throw new NotImplementedException();
                    //Messenger.Default.Send(new GoToPageMessage(new PostVM(this));
                    //ItemClickAction.Invoke();
                });
            }
        }

        public ICommand LinkClick
        {
            get
            {
                return new RelayCommand<MouseButtonEventArgs>((args) =>
                {
                    var post = (LoadedPost)((System.Windows.Controls.TextBlock)args.Source).DataContext;
                    System.Diagnostics.Process.Start(post.Url.ToString());
                    args.Handled = true;
                });
            }
        }

        public ICommand UpvoteClick
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (Liked.HasValue && Liked.Value) ClearVote(); else Upvote();
                }
                , () =>
                {
                    return SessionContext.Reddit.User != null;
                }, true);
            }
        }

        public ICommand DownvoteClick
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (Liked.HasValue && !Liked.Value) ClearVote(); else Downvote();
                }
                , () =>
                {
                    return SessionContext.Reddit.User != null;
                });
            }
        }

        public ICommand ShowFullResolutionImageChange
        {
            get
            {
                return new RelayCommand<LoadedPost>((post) =>
                {
                    post.ShowFullResolutionImage = !post.ShowFullResolutionImage;
                }
                , (post) =>
                {
                    return post.CanShowFullResolutionImage;
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

        public bool CanShowFullResolutionImage
        {
            get
            {
                var formats = new List<string>() { ".png", ".jpg", ".jpeg", ".gif" };
                return formats.Any(f => this.Url.ToString().Contains(f));
            }
        }

        public void Comment(string message)
        {
            var comment = this.Post.Comment(message);
            Comments.Add(new LoadedComment(comment));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateVoteProps()
        {
            Score = Post.Score;
            Liked = Post.Liked;
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
