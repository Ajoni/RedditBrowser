using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Classes;
using RedditBrowser.ViewModel.Messages;
using RedditSharp.Things;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace RedditBrowser.ViewModel
{
    public class PostVM : ViewModelBase, IViewModel
    {
        private AuthenticatedUser _user;
        private string _comment;


        public LoadedPost Post { get; set; }
        public string Comment
        {
            get => _comment; set
            {
                _comment = value; RaisePropertyChanged(); RaisePropertyChanged("CommentButtonEnabled");
            }
        }

        public PostVM(LoadedPost post, AuthenticatedUser user)
        {
            Post = post;
            User = user;
        }

        public ICommand SubredditNameClick
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send(new ChangeSubredditMessage(this.Post.Post.SubredditName));
                });
            }
        }
        public ICommand PostComment
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Post.Comment(Comment);
                }, () =>
                 {
                     return !string.IsNullOrEmpty(this.Comment) && SessionContext.IsUserLoggedIn;
                 });
            }
        }
    }
}
