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

        public IViewModel CurrentPage { get => _currentPage; set { _currentPage = value; RaisePropertyChanged(); } }
        public TopPanelVM TopPanel { get; set; } = new TopPanelVM();
        public ListVM ListVM { get; set; }
        public Subreddit Subreddit { get; set; }
        public Reddit Reddit { get; set; }
        public User User { get; set; }
        public bool Busy { get; set; }


        public MainViewModel()
        {
            this.RegisterMessages();
            this.ListVM = new ListVM();

            string keyPath = GlobalConfig.Get<string>(GlobalKeys.KeyConfigPath);

            string[] keys = System.IO.File.ReadAllText(keyPath).Split(',');
            AuthProvider auth = new AuthProvider(keys[0], keys[1], keys[2]);
            var accessToken = auth.GetOAuthToken(keys[3], keys[4]);
            var agent = new WebAgent() { AccessToken = accessToken };
			WebAgent.RootDomain = "oauth.reddit.com";
			this.Reddit = new Reddit(agent,true);
			this.Subreddit = this.Reddit.RSlashAll;

            this.TopPanel.Header = Subreddit.HeaderImage;
            this.TopPanel.SubredditName = Subreddit.Name;

        }

        public async Task Init()
        {
            List<SimplfiedPost> posts = new List<SimplfiedPost>();
            await Task.Run(() =>
            {
                posts = LoadPosts(0, 1);
            });
            IObservable<SimplfiedPost> postsToLoad = posts.ToObservable();
            postsToLoad.Subscribe(p =>
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action<SimplfiedPost>((post) => this.ListVM.Posts.Add(post)), p);
            }, () =>
            {
                this.Busy = false;
            });
        }

        private List<SimplfiedPost> LoadPosts(int from, int amount)
        {
            return Subreddit.Posts.Skip(from).Take(amount).Select(post => new SimplfiedPost(post)).ToList();
        }

        private void ReceiveMessage(GoToPageMessage message) => this.CurrentPage = message.Page;
        private void ReceiveMessage(GoToListViewMessage message) => this.CurrentPage = this.ListVM;

        private void RegisterMessages()
        {
            Messenger.Default.Register<GoToPageMessage>(this, (action) => ReceiveMessage(action));
            Messenger.Default.Register<GoToListViewMessage>(this, (action) => ReceiveMessage(action));
        }

    }
}