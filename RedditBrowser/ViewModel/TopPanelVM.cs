using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel.Messages;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RedditBrowser.ViewModel
{
	public class TopPanelVM : ViewModelBase, IViewModel
	{
		private string  _SubredditName = "";
        private bool    _IsUserLoggedIn = false;

		public string                       Header          { get; set; }
		public string                       Search          { get; set; }
		public ObservableCollection<string> Subreddits      { get; set; }
        public bool                         IsUserLoggedIn
        {
            get { return _IsUserLoggedIn; }
            set { _IsUserLoggedIn = value; RaisePropertyChanged(); }
        }

        public string SelectedSubreddit
		{
			get { return _SubredditName; }
			set
			{
				_SubredditName = value;
				RaisePropertyChanged();
			}
		}

		public TopPanelVM()
		{
			Subreddits = new ObservableCollection<string> { "all" };
		}

        public RelayCommand GoToRAll => new RelayCommand(() =>
        {
            if (this.SelectedSubreddit == "all")
                return;
            this.SelectedSubreddit = "all";
			ChangeSubredditExec();
		});

        public ICommand ChangeSubreddit => new RelayCommand(() =>
		{
			ChangeSubredditExec();
		});

		private void ChangeSubredditExec()
		{
			if (!Subreddits.Contains(this.SelectedSubreddit))
				Subreddits.Add(this.SelectedSubreddit);
			Messenger.Default.Send(new ChangeSubredditMessage(SelectedSubreddit));
		}

		public ICommand LoginClick => new DelegateCommand((a) =>
		{
			Messenger.Default.Send(new ShowLoginMessage());
		});

		public ICommand LogoutClick => new DelegateCommand((a) =>
		{
			Messenger.Default.Send(new LoginChangeMessage(null));
		});
	}
}
