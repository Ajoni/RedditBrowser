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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedditBrowser.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
        private const int AmountForInitialLoad = 3;
        private IViewModel _currentPage;
		private bool _busy;

		public IViewModel CurrentPage { get => _currentPage; set { _currentPage = value; RaisePropertyChanged(); } }
		public TopPanelVM TopPanel { get; set; } = new TopPanelVM();
		public ListVM ListVM { get; set; }
		public LoginVM LoginVM { get; set; }
		public Subreddit Subreddit { get; set; }
		public bool Busy { get => _busy; set { _busy = value; RaisePropertyChanged(); } }
        public ICommand ChangeToViewCommand
        {
            get => new RelayCommand<IViewModel>((page) => this.CurrentPage = page);
        }

        private CancellationTokenSource TokenSource = new CancellationTokenSource();
        private CancellationToken LoadNextPostToken;
        private CancellationToken InitToken;
        private bool firtsLoad = true;

        public MainViewModel()
		{
			this.RegisterMessages();
            this.ListVM = new ListVM()
            {
                LoadNextPost = new RelayCommand(() => LoadNextPostMethod(LoadNextPostToken), canExecute: () => Subreddit != null && !ListVM.Busy)
            };
			this.LoginVM = new LoginVM();
		}

		public async Task Init(CancellationToken cancellationToken)
		{
            this.ListVM.Posts.Clear();
			List<LoadedPost> posts = new List<LoadedPost>();
            await Task.Factory.StartNew(stateObject =>
            {
                var castedToken = (CancellationToken)stateObject;
                posts = LoadPosts(0, AmountForInitialLoad);
                if (castedToken.IsCancellationRequested)
                    posts.Clear();
            }, cancellationToken);
            IObservable<LoadedPost> postsToLoad = posts.ToObservable();
			postsToLoad.Subscribe(p =>
			{
				Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action<LoadedPost>((post) => this.ListVM.Posts.Add(post)), p);
			});
            if (cancellationToken.IsCancellationRequested)
                foreach (var item in posts)
                    this.ListVM.Posts.Remove(item);
            var a = cancellationToken.Equals(this.InitToken);
        }

		private List<LoadedPost> LoadPosts(int from, int amount) => Subreddit.Posts.Skip(from).Select(
                post => {
                    var p = new LoadedPost(post);
                    p.ItemClickAction = () => ChangeToViewCommand.Execute(new PostVM(p) { ReturnToPreviousViewAction = () => ChangeToViewCommand.Execute(ListVM) });
                    return p;
                }
            ).Take(amount).ToList();

        private async void LoadNextPostMethod(CancellationToken cancellationToken)
            //wyœcig tu jest chwilowo
        {
            if (ListVM.Busy || 
                ListVM.Posts.Count < 3) // chwilowe rozwiazanie które dzia³a tylko dla pierwszego load
                return;
            ListVM.Busy = true;
            {
                LoadNextPostToken = TokenSource.Token;
                var newPosts =  await Task.Run(() =>
                {
                    if (LoadNextPostToken.IsCancellationRequested)
                        return new List<LoadedPost>();
                    var currentPostCount = ListVM.Posts.Count;
                    return LoadPosts(currentPostCount, 3);
                });

                if (LoadNextPostToken.IsCancellationRequested)
                {
                    ListVM.Busy = false;
                    return;
                }
                foreach (var post in newPosts)
                    ListVM.Posts.Add(post);
                ListVM.RaisePropertyChanged("Posts");

                if (LoadNextPostToken.IsCancellationRequested)
                    foreach (var post in newPosts)
                        ListVM.Posts.Remove(post);
            }
            ListVM.Busy = false;
        }

		private void ReceiveMessage(GoToPageMessage message) => this.CurrentPage = message.Page;
		private async void ReceiveMessage(ChangeSubredditMessage message)
        {
            if (string.IsNullOrEmpty(message.Name))
                return;
            this.CurrentPage = this.ListVM;
            if (!message.Reload)
                return;
            this.ListVM.Busy = true;

            changeTokenSource();

            if (message.Name == "all")
                this.Subreddit = SessionContext.Context.Reddit.RSlashAll;
            else
                try
                {
                    this.Subreddit = await SessionContext.Context.Reddit.GetSubredditAsync(message.Name);
                    if (this.Subreddit == null)
                        throw new Exception();
                }
                catch (Exception)
                {
                    var sub = this.TopPanel.Subreddits.SingleOrDefault(s => s.Name == message.Name);
                    this.TopPanel.Subreddits.Remove(sub);
                    MessageBox.Show($"Subreddit: '{message.Name}' does not exist or there is something wrong with your connection or reddit.");
                    return;
                }
            await this.Init(InitToken);
            this.ListVM.Busy = false;
        }

        private void changeTokenSource()
        {
            if (!firtsLoad)
            {
                TokenSource.Cancel();
                TokenSource.Dispose();
            }
            firtsLoad = false;
            TokenSource = new CancellationTokenSource();
            InitToken = TokenSource.Token;
        }

        private async void ReceiveMessage(LoginChangeMessage message)
		{
            SessionContext.Context.Update(message.UserLoginResult);

            if (SessionContext.Context.IsUserLoggedIn)
                this.TopPanel.PopulateCombobox(SessionContext.Context.Reddit.User.SubscribedSubreddits);

			this.CurrentPage = this.ListVM;
			if (this.Subreddit != null)
                Messenger.Default.Send(new ChangeSubredditMessage(this.Subreddit.Name)); //reload needed to update webAgents with user acces token
		}

        private void ReceiveMessage(SearchMessage message) => this.CurrentPage = new SearchResultVM(new ListVM(false), message.Query, this.Subreddit)
        {
            ChangeToViewCommand = this.ChangeToViewCommand
        };

        private async void ReceiveMessage(SubscribeMessage message)
        {
            var sub = await SessionContext.Context.Reddit.GetSubredditAsync(message.Name);
            await Task.Run(()=>sub.Subscribe());
        }
        private async void ReceiveMessage(UnsubscribeMessage message)
        {
            var sub = await SessionContext.Context.Reddit.GetSubredditAsync(message.Name);
            await Task.Run(() => sub.Unsubscribe());
        }
        
		private void RegisterMessages()
		{
			Messenger.Default.Register<GoToPageMessage>         (this, (message) => ReceiveMessage(message));
			Messenger.Default.Register<ChangeSubredditMessage>  (this, (message) => ReceiveMessage(message));
			Messenger.Default.Register<LoginChangeMessage>      (this, (message) => ReceiveMessage(message));
			Messenger.Default.Register<SearchMessage>			(this, (message) => ReceiveMessage(message));
			Messenger.Default.Register<SubscribeMessage>			(this, (message) => ReceiveMessage(message));
			Messenger.Default.Register<UnsubscribeMessage>			(this, (message) => ReceiveMessage(message));
		}
	}
}