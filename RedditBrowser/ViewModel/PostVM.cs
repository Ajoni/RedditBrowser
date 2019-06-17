using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Classes;
using RedditBrowser.ViewModel.Messages;
using RedditSharp.Things;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace RedditBrowser.ViewModel
{
    public class PostVM : ViewModelBase, IViewModel
    {
        private string _comment;


        public LoadedPost Post { get; set; }
        public string Comment
        {
            get => _comment; set
            {
                _comment = value; RaisePropertyChanged(); RaisePropertyChanged("CommentButtonEnabled");
            }
        }
        public bool CommentButtonEnabled { get { return !string.IsNullOrEmpty(this.Comment) && SessionContext.Context.IsUserLoggedIn; } }

        public PostVM(LoadedPost post)
        {
            Post = post;
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
                    Post.Comment(Comment); this.Comment = "";
                }, () =>
                 {
                     return !string.IsNullOrEmpty(this.Comment) && SessionContext.Context.IsUserLoggedIn;
                 });
            }
        }

        public ICommand ReturnClick
        {
            get => new RelayCommand(Return);
        }

        public ICommand SaveImageCommand
        {
            get
            {
                return Post.SaveImageCommand;
            }
        }

        public Action ReturnToPreviousViewAction { get; set; }

        private void Return()
        {
            ReturnToPreviousViewAction?.Invoke();
        }
    }
}
