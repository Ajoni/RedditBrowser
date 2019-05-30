using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel.Messages;
using RedditSharp.Things;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RedditBrowser.ViewModel
{
	public class TopPanelVM : ViewModelBase, IViewModel
	{
		private string _SubredditName = "";
		private bool _IsUserLoggedIn = false;
		private string _query;

		public string Header { get; set; }
		public string Query
		{
			get => _query; set
			{
				_query = value; RaisePropertyChanged();
			}
		}
		public ObservableCollection<SubredditComboboxLayout> Subreddits { get; set; }
        public ObservableCollection<string> SubscribedSubreddits { get; set; }

        public bool IsUserLoggedIn
		{
			get { return _IsUserLoggedIn; }
			set { _IsUserLoggedIn = SubredditComboboxLayout.IsUserLoggedIn = value; RaisePropertyChanged(); }
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
			Subreddits = new ObservableCollection<SubredditComboboxLayout> { new SubredditComboboxLayout("all") };
			RegisterMessages();
		}

        #region Commands
		public RelayCommand GoToRAll => new RelayCommand(() =>
		{
			this.SelectedSubreddit = "all";
			ChangeSubredditExec();
		});

        public ICommand ChangeSubreddit => new RelayCommand(() =>
{
    ChangeSubredditExec();
});

        private void ChangeSubredditExec()
        {
            if (Subreddits.Where(s => s.Name == this.SelectedSubreddit).ToList().Count == 0)
                Subreddits.Add(new SubredditComboboxLayout(this.SelectedSubreddit));
            Messenger.Default.Send(new ChangeSubredditMessage(SelectedSubreddit));
        }

        public ICommand LoginClick => new RelayCommand(() =>
        {
            Messenger.Default.Send(new ShowLoginMessage());
        });

        public ICommand LogoutClick => new RelayCommand(() =>
        {
            Messenger.Default.Send(new LoginChangeMessage(null));
        });

        public ICommand Search => new RelayCommand(() =>
        {
            if (!string.IsNullOrEmpty(Query))
                Messenger.Default.Send(new SearchMessage(Query));
        }); 
        #endregion

        /// <summary>
        /// update selected sub when user clicks on subreddit link
        /// </summary>
        /// <param name="message"></param>
        private void ReceiveMessage(ChangeSubredditMessage message)
		{
			if (string.IsNullOrEmpty(message.Name))
				return;

			if (message.Name != this.SelectedSubreddit)
				this.SelectedSubreddit = message.Name;

            if (Subreddits.Where(s => s.Name == this.SelectedSubreddit).ToList().Count == 0)
                Subreddits.Add(new SubredditComboboxLayout(this.SelectedSubreddit));

        }
		private void ReceiveMessage(SubredditSubscribedMessage message)
		{
            if (Subreddits.Where(s => s.Name == this.SelectedSubreddit).ToList().Count == 0)
            {
                Subreddits.Add(new SubredditComboboxLayout(this.SelectedSubreddit));
                SubredditComboboxLayout.SubscribedSubreddits.Add(message.Name);
            }
        }
        private void ReceiveMessage(SubredditUnsubscribedMessage message)
        {
            SubredditComboboxLayout.SubscribedSubreddits.Remove(message.Name);
        }

        private void RegisterMessages()
		{
			Messenger.Default.Register<ChangeSubredditMessage>(this, (message) => ReceiveMessage(message));
			Messenger.Default.Register<SubredditSubscribedMessage>(this, (message) => ReceiveMessage(message));
			Messenger.Default.Register<SubredditUnsubscribedMessage>(this, (message) => ReceiveMessage(message));
		}

        public class SubredditComboboxLayout
        {
            public static ObservableCollection<string> SubscribedSubreddits { get; set; } = new ObservableCollection<string>();
            public static bool IsUserLoggedIn { get; set; }
            public string Name { get; set; }

            public SubredditComboboxLayout(string name)
            {
                Name = name;
            }

            public ICommand SubredditUnsubscribeClick
            {
                get
                {
                    return new RelayCommand<string>((sub) =>
                    {
                        Messenger.Default.Send(new UnsubscribeMessage(sub));
                    }, (sub) => IsSubscribed);
                }
            }

            public ICommand SubredditSubscribeClick
            {
                get
                {
                    return new RelayCommand<string>((sub) =>
                    {
                        Messenger.Default.Send(new SubscribeMessage(sub));
                    }, (sub) => CanSubscribe, true);
                }
            }

            public bool IsSubscribed { get => SubscribedSubreddits.Contains(Name) && IsUserLoggedIn; }
            public bool CanSubscribe { get => !SubscribedSubreddits.Contains(Name) && Name != "all" && IsUserLoggedIn; }
        }
	}
}
