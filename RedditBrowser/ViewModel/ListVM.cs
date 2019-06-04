using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Classes;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel.Messages;
using RedditSharp.Things;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

		public ListVM(AuthenticatedUser user = null, bool goTo = true)
        {
			User = user;
			if(goTo)
				Messenger.Default.Send(new GoToPageMessage(this));

            RegisterMessages();
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

        

		#endregion // Commands

		private void ReceiveMessage(ChangeSubredditMessage message)
		{
            if (message.Reload)
            {
                this.MousedOverPost = null;
                this.Posts.Clear(); 
            }
		}

        private void RegisterMessages()
        {
            Messenger.Default.Register<ChangeSubredditMessage>(this, (message) => ReceiveMessage(message));
        }
    }
}
