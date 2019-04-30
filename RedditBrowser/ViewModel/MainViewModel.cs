using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Classes;
using RedditBrowser.Config;
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
		public PostVM PostVM { get; set; }
		public Subreddit Subreddit { get; set; }
		public Reddit Reddit { get; set; }
		public bool Busy { get => _busy; set { _busy = value; RaisePropertyChanged(); } }

		private WebAgent WebAgent { get; set; }


		public MainViewModel()
		{
			this.RegisterMessages();
			this.ListVM = new ListVM();
			this.LoginVM = new LoginVM();
			//this.PostVM = new PostVM();

			//this.WebAgent = this.LoginApp();
			//this.Reddit = new Reddit(WebAgent, true);
			this.Reddit = new Reddit();
			this.Subreddit = this.Reddit.RSlashAll;

			this.TopPanel.Header = Subreddit.HeaderImage;
			this.TopPanel.SubredditName = Subreddit.Name;

		}

		private WebAgent LoginApp()
		{
			string keyPath = GlobalConfig.Get<string>(GlobalKeys.KeyConfigPath);

			string[] keys = System.IO.File.ReadAllText(keyPath).Split(',');
			AuthProvider auth = new AuthProvider(keys[0], keys[1], keys[2]);
			var accessToken = auth.GetOAuthToken(keys[3], keys[4]);
			var agent = new WebAgent() { AccessToken = accessToken };
			WebAgent.RootDomain = "oauth.reddit.com";
			return agent;
		}

		public async Task Init()
		{
			List<Post> posts = new List<Post>();
			this.Busy = true;
			await Task.Run(() =>
			{
				posts = LoadPosts(0, 3);
			});
			IObservable<Post> postsToLoad = posts.ToObservable();
			postsToLoad.Subscribe(p =>
			{
				Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action<Post>((post) => this.ListVM.Posts.Add(post)), p);
			}, () =>
			{
				this.Busy = false;
			});
		}

		private List<Post> LoadPosts(int from, int amount)
		{
			return Subreddit.Posts.Skip(from).Take(amount).ToList();
		}

		private void ReceiveMessage(GoToPageMessage message) => this.CurrentPage = message.Page;
		private void ReceiveMessage(GoToListViewMessage message) => this.CurrentPage = this.ListVM;
		private void ReceiveMessage(LoginChangeMessage message) { this.Reddit.User = message.UserLoginResult.AuthenticatedUser; this.ListVM.User = message.UserLoginResult.AuthenticatedUser; }

		private void RegisterMessages()
		{
			Messenger.Default.Register<GoToPageMessage>(this, (action) => ReceiveMessage(action));
			Messenger.Default.Register<GoToListViewMessage>(this, (action) => ReceiveMessage(action));
			Messenger.Default.Register<LoginChangeMessage>(this, (action) => ReceiveMessage(action));
		}

	}
}