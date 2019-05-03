using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Classes;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel.Messages;
using RedditSharp.Things;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace RedditBrowser.ViewModel
{
    public class ListVM : ViewModelBase, IViewModel
	{
		private AuthenticatedUser   _user;
		private bool                _busy;

		public ObservableCollection<Post>   Posts           { get; set; } = new ObservableCollection<Post>();
        public Post                         MousedOverPost  { get; set; }
		public AuthenticatedUser            User            { get => _user; set { _user = value; RaisePropertyChanged(); } }
		public bool                         Busy            { get => _busy; set { _busy = value; RaisePropertyChanged(); } }

		public ListVM()
        {
			Messenger.Default.Send(new GoToPageMessage(this));
		}

        #region Commands

        public RelayCommand LoadNextPost { get; set; }

        public ICommand ItemHover
        {
            get
            {
                return new DelegateCommand((a) =>
                    {
                        this.MousedOverPost = (Post)a;
                    }
                    , (a) =>
                    {
                        return Posts.Count > 0;
                    });
            }
        }

        public ICommand ItemClick
        {
            get
            {
                return new DelegateCommand((a) =>
                {
                    Messenger.Default.Send(new GoToPageMessage(new PostVM(this.MousedOverPost)));
                }
                , (a) =>
                {
                    return Posts.Count > 0;
                });
            }
        }

		public ICommand UpvoteClick
		{
			get
			{
				return new RelayCommand(() =>
				{
					if(MousedOverPost.Liked.HasValue && MousedOverPost.Liked.Value) MousedOverPost.ClearVote(); else MousedOverPost.Upvote();
				}
				, () =>
				{
					return this.User != null && this.MousedOverPost != null;
				},true);
			}
		}

		public ICommand DownvoteClick
		{
			get
			{
				return new DelegateCommand((a) =>
				{
					if (MousedOverPost.Liked.HasValue && MousedOverPost.Liked.Value) MousedOverPost.ClearVote(); else MousedOverPost.Downvote();
				}
				, (a) =>
				{
					return this.User != null && this.MousedOverPost != null;
				});
			}
		}

        #endregion // Commands
    }
}
