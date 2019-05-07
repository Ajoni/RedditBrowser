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

		public ObservableCollection<LoadedPost>   Posts           { get; set; } = new ObservableCollection<LoadedPost>();
        public LoadedPost MousedOverPost  { get; set; }
		public AuthenticatedUser            User            { get => _user; set { _user = value; if(this.PostVM != null) this.PostVM.User = value; RaisePropertyChanged(); } }
		public bool                         Busy            { get => _busy; set { _busy = value; RaisePropertyChanged(); } }
		public PostVM PostVM { get; private set; }

		public ListVM(bool goTo = true)
        {
			if(goTo)
				Messenger.Default.Send(new GoToPageMessage(this));
		}

        #region Commands

        public RelayCommand LoadNextPost { get; set; }

        public ICommand ItemHover
        {
            get
            {
                return new RelayCommand<LoadedPost>((post) =>
                    {
                        this.MousedOverPost = post;
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
				return new RelayCommand(() =>
				{
					this.PostVM = new PostVM(this.MousedOverPost, this.User);
					Messenger.Default.Send(new GoToPageMessage(this.PostVM));
                }
                , () =>
                {
                    return Posts.Count > 0;
                });
            }
        }

		public ICommand LinkClick
		{
            get
            {
                return new RelayCommand<LoadedPost>((post) =>
                {
					System.Diagnostics.Process.Start(post.Url.ToString());
				});
            }
        }

		public ICommand UpvoteClick
		{
			get
			{
				return new RelayCommand(() =>
				{
					if (MousedOverPost.Liked.HasValue && MousedOverPost.Liked.Value) MousedOverPost.ClearVote(); else MousedOverPost.Upvote();
				}
				, () =>
				{
					return this.User != null;
				}, true);
			}
		}

		public ICommand DownvoteClick
		{
			get
			{
				return new RelayCommand(() =>
				{
					if (MousedOverPost.Liked.HasValue && !MousedOverPost.Liked.Value) MousedOverPost.ClearVote(); else MousedOverPost.Downvote();
				}
				, () =>
				{
					return this.User != null;
				});
			}
		}

		#endregion // Commands
	}
}
