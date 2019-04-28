using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Classes;
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
		public LoginVM LoginVM { get; set; }
		public Subreddit Subreddit { get; set; }
		public Reddit Reddit { get; set; }
		public User User { get; set; }
		public bool Busy { get; set; }


		public MainViewModel()
		{
			RegisterMessages();
			ListVM = new ListVM();
			LoginVM = new LoginVM();

			Reddit = new Reddit();
			Subreddit = Reddit.RSlashAll;

			TopPanel.Header = Subreddit.HeaderImage;
			TopPanel.SubredditName = Subreddit.Name;

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
				Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action<SimplfiedPost>((post) => ListVM.Posts.Add(post)), p);
			}, () =>
			{
				Busy = false;
			});
		}

		private List<SimplfiedPost> LoadPosts(int from, int amount)
		{
			return Subreddit.Posts.Skip(from).Take(amount).Select(post => new SimplfiedPost(post)).ToList();
		}

		private void ReceiveMessage(GoToPageMessage message) => CurrentPage = message.Page;
		private void ReceiveMessage(GoToListViewMessage message) => CurrentPage = ListVM;

		private void RegisterMessages()
		{
			Messenger.Default.Register<GoToPageMessage>(this, (action) => ReceiveMessage(action));
			Messenger.Default.Register<GoToListViewMessage>(this, (action) => ReceiveMessage(action));
		}

	}
}