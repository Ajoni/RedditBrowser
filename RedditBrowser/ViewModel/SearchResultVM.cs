using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RedditBrowser.Classes;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RedditBrowser.ViewModel
{
	public class SearchResultVM : ViewModelBase, IViewModel
	{
		private bool _busy;

		public ListVM ListVM { get; set; }
		public string Query { get; set; }
		public ObservableCollection<Subreddit> Subreddits { get; set; } = new ObservableCollection<Subreddit>();
		public bool Busy
		{
			get => _busy; set
			{
				_busy = value; RaisePropertyChanged();
			}
		}
		public Reddit Reddit { get; set; }
		public Subreddit Subreddit { get; set; }

		public SearchResultVM(ListVM listVM, string query, Reddit reddit, Subreddit subreddit)
		{
			ListVM = listVM;
			ListVM.LoadNextPost = new RelayCommand(() => LoadNextPostMethod(), canExecute: () => Subreddit != null);
			Query = query;
			Reddit = reddit;
			Subreddit = subreddit;

			InitSubs();
			if (Subreddit != null)
				InitPosts();
		}

		private async void InitSubs()
		{
			Subreddits.Clear();
			List<Subreddit> subs = new List<Subreddit>();
			Busy = true;
			await Task.Run(() =>
			{
				subs = this.Reddit.SearchSubreddits(Query).Take(5).ToList();
			});
			IObservable<Subreddit> postsToLoad = subs.ToObservable();
			postsToLoad.Subscribe(p =>
			{
				Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action<Subreddit>((sub) => Subreddits.Add(sub)), p);
			}, () =>
			{
				Busy = false;
			});

		}

		private async void InitPosts()
		{
			ListVM.Posts.Clear();
			List<LoadedPost> posts = new List<LoadedPost>();
			Busy = true;
			await Task.Run(() =>
			{
				posts = LoadPosts(0, 5);
			});
			IObservable<LoadedPost> postsToLoad = posts.ToObservable();
			postsToLoad.Subscribe(p =>
			{
				Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action<LoadedPost>((post) => ListVM.Posts.Add(post)), p);
			}, () =>
			{
				Busy = false;
			});

		}
		private List<LoadedPost> LoadPosts(int from, int amount) =>
		Subreddit.Search(Query)
		.Skip(from).Select(post => new LoadedPost(post)).Take(amount).ToList();
		private async void LoadNextPostMethod()
		{
			if (ListVM.Busy)
				return;
			ListVM.Busy = true;
			{
				var newPosts = await Task.Run(() =>
				{
					var currentPostCount = ListVM.Posts.Count;
					return Subreddit.Search(Query).Skip(currentPostCount).Take(3).Select(post => new LoadedPost(post)).ToList();
				});
				foreach (var post in newPosts)
					ListVM.Posts.Add(post);
				ListVM.RaisePropertyChanged("Posts");
			}
			ListVM.Busy = false;
		}
	}
}
