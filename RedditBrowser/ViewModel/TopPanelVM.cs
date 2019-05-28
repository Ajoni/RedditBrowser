using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel.Messages;
using RedditSharp.Things;
using System.Collections.ObjectModel;
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
		public ObservableCollection<string> Subreddits { get; set; }
        

        public bool IsUserLoggedIn
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
			RegisterMessages();
		}

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
			if (!Subreddits.Contains(this.SelectedSubreddit))
				Subreddits.Add(this.SelectedSubreddit);
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
			if(!string.IsNullOrEmpty(Query))
				Messenger.Default.Send(new SearchMessage(Query));
		});

        public ICommand SubredditUnsubscribeClick
        {
            get
            {
                return new RelayCommand<Subreddit>((sub) =>
                {
                    this.MousedOverSubreddit.Unsubscribe();
                    this.SubscribedSubreddits.Remove(sub.Name);
                }, (sub) => this.Reddit.User != null && !isSubscribed(sub));
            }
        }

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

			if (!Subreddits.Contains(this.SelectedSubreddit))
				Subreddits.Add(this.SelectedSubreddit);

		}
		private void ReceiveMessage(SubredditSubscribedMessage message)
		{
			if (!Subreddits.Contains(message.Name))
				Subreddits.Add(message.Name);
		}

		private void RegisterMessages()
		{
			Messenger.Default.Register<ChangeSubredditMessage>(this, (message) => ReceiveMessage(message));
			Messenger.Default.Register<SubredditSubscribedMessage>(this, (message) => ReceiveMessage(message));
		}
	}
}
