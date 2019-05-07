using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Classes;
using RedditBrowser.Config;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel.Messages;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RedditBrowser.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private IViewModel _currentPage;
		private bool _busy;

		public IViewModel CurrentPage { get => _currentPage; set { _currentPage = value; RaisePropertyChanged(); } }
		public TopPanelVM TopPanel { get; set; } = new TopPanelVM();
		public ListVM ListVM { get; set; }
		public LoginVM LoginVM { get; set; }
		public Subreddit Subreddit { get; set; }
		public Reddit Reddit { get; set; }
		public bool Busy { get => _busy; set { _busy = value; RaisePropertyChanged(); } }
        private WebAgent WebAgent { get; set; }

		public MainViewModel()
		{
			this.RegisterMessages();
            this.ListVM = new ListVM()
            {
                LoadNextPost = new RelayCommand(() => LoadNextPostMethod(), canExecute: () => Subreddit != null)
            };
			this.LoginVM = new LoginVM();
			this.Reddit = new Reddit();
		}

		public async Task Init()
		{
			this.ListVM.Posts.Clear();
			List<LoadedPost> posts = new List<LoadedPost>();
			this.Busy = true;
			await Task.Run(() =>
			{
				posts = LoadPosts(0, 5);
			});
			IObservable<LoadedPost> postsToLoad = posts.ToObservable();
			postsToLoad.Subscribe(p =>
			{
				Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action<LoadedPost>((post) => this.ListVM.Posts.Add(post)), p);
			}, () =>
			{
				this.Busy = false;
			});
		}

		private List<LoadedPost> LoadPosts(int from, int amount) => Subreddit.Posts.Skip(from).Select(post => new LoadedPost(post)).Take(amount).ToList();

        private async void LoadNextPostMethod()
        {
            if (ListVM.Busy)
                return;
            ListVM.Busy = true;
            {
                var newPosts =  await Task.Run(() =>
                {
                    var currentPostCount = ListVM.Posts.Count;
                    return Subreddit.Posts.Skip(currentPostCount).Take(3).Select( post => new LoadedPost(post)).ToList();
                });
                foreach(var post in newPosts)
                    ListVM.Posts.Add(post);
                ListVM.RaisePropertyChanged("Posts");
            }
            ListVM.Busy = false;
        }

		private void ReceiveMessage(GoToPageMessage message) => this.CurrentPage = message.Page;

		private async void ReceiveMessage(ChangeSubredditMessage message)
		{
			if (string.IsNullOrEmpty(message.Name))
				return;
			this.CurrentPage = this.ListVM;
			if (message.Name == this.Subreddit?.Name)
				return;

			if (message.Name == "all")
				this.Subreddit = this.Reddit.RSlashAll;
			else
				try
				{
					this.Subreddit = await this.Reddit.GetSubredditAsync(message.Name);
				}
				catch (System.Net.WebException)
				{
					this.TopPanel.Subreddits.Remove(message.Name);
					MessageBox.Show($"Subreddit: '{message.Name}' does not exist or there is something wrong with your connection or reddit.");
					return;
				}
			await this.Init();
		}
        
		private async void ReceiveMessage(LoginChangeMessage message)
		{
			if (message.UserLoginResult != null)
				this.Reddit = new Reddit(message.UserLoginResult.WebAgent, true);
			else
				this.Reddit = new Reddit();
			this.ListVM.User = this.Reddit.User;

            if (Reddit.User != null)
            {
                foreach (var item in this.Reddit.User.SubscribedSubreddits)
                    if (!this.TopPanel.Subreddits.Any(s => s == item.Name))
                        this.TopPanel.Subreddits.Add(item.Name);
            }

			this.CurrentPage = this.ListVM;
			if (this.Subreddit != null)
				await this.Init();

            TopPanel.IsUserLoggedIn = Reddit.User != null;
		}
		private void ReceiveMessage(SearchMessage message) => this.CurrentPage = new SearchResultVM(new ListVM(false),message.Query,this.Reddit,this.Subreddit);
        
		private void RegisterMessages()
		{
			Messenger.Default.Register<GoToPageMessage>         (this, (message) => ReceiveMessage(message));
			Messenger.Default.Register<ChangeSubredditMessage>  (this, (message) => ReceiveMessage(message));
			Messenger.Default.Register<LoginChangeMessage>      (this, (message) => ReceiveMessage(message));
			Messenger.Default.Register<SearchMessage>			(this, (message) => ReceiveMessage(message));
		}

	}
}