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
		private AuthenticatedUser _user;

		public IViewModel CurrentPage { get => _currentPage; set { _currentPage = value; RaisePropertyChanged(); } }
		public TopPanelVM TopPanel { get; set; } = new TopPanelVM();
		public ListVM ListVM { get; set; }
		public LoginVM LoginVM { get; set; }
		//public PostVM PostVM { get; set; }
		public Subreddit Subreddit { get; set; }
		public Reddit Reddit { get; set; }
		public bool Busy { get => _busy; set { _busy = value; RaisePropertyChanged(); } }
		public AuthenticatedUser User { get => _user; set { _user = value; this.ListVM.User = value; RaisePropertyChanged(); } }
		private WebAgent WebAgent { get; set; }

		public MainViewModel()
		{
			this.RegisterMessages();
            this.ListVM = new ListVM()
            {
                LoadNextPost = new RelayCommand(() => LoadNextPostMethod())
            };
			this.LoginVM = new LoginVM();
			//this.PostVM = new PostVM();

			this.WebAgent = this.LoginApp();
			this.Reddit = new Reddit(WebAgent, true);
			//this.Reddit = new Reddit();
			//this.Subreddit = this.Reddit.RSlashAll;

			//this.TopPanel.Header = Subreddit.HeaderImage;
			//this.TopPanel.SelectedSubreddit = Subreddit.Name;

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
			this.ListVM.Posts.Clear();
			List<Post> posts = new List<Post>();
			this.Busy = true;
			await Task.Run(() =>
			{
				posts = LoadPosts(0, 1);
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

		private List<Post> LoadPosts(int from, int amount) => Subreddit.Posts.Skip(from).Take(amount).ToList();

        private async void LoadNextPostMethod()
        {
            if (ListVM.Busy)
                return;
            ListVM.Busy = true;
            {
                var newPosts =  await Task.Run<List<Post>>(() =>
                {
                    var currentPostCount = ListVM.Posts.Count;
                    return Subreddit.Posts.Skip(currentPostCount).Take(3).ToList();
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
			this.CurrentPage = this.ListVM;
			if (message.Name == "r/all")
				this.Subreddit = this.Reddit.RSlashAll;
			else
				this.Subreddit = await this.Reddit.GetSubredditAsync(message.Name);
			await this.Init();

		}
		private async void ReceiveMessage(LoginChangeMessage message)
		{
			this.Reddit = new Reddit(message.UserLoginResult.WebAgent, true);
			this.Reddit.User = this.ListVM.User = message.UserLoginResult.AuthenticatedUser;
			await this.Init();
		}

		private void RegisterMessages()
		{
			Messenger.Default.Register<GoToPageMessage>(this, (action) => ReceiveMessage(action));
			Messenger.Default.Register<ChangeSubredditMessage>(this, (action) => ReceiveMessage(action));
			Messenger.Default.Register<LoginChangeMessage>(this, (action) => ReceiveMessage(action));
		}

	}
}