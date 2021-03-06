﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Classes;
using RedditBrowser.ViewModel.Messages;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedditBrowser.ViewModel
{
	public class SearchResultVM : ViewModelBase, IViewModel
	{
		private bool _busy;

		public ListVM ListVM { get; set; }
		public string Query { get; set; }
		public ObservableCollection<Subreddit> Subreddits { get; set; } = new ObservableCollection<Subreddit>();
		public ObservableCollection<string> SubscribedSubreddits { get; set; } = new ObservableCollection<string>();
		public Subreddit MousedOverSubreddit { get; private set; }
		public bool Busy
		{
			get => _busy; set
			{
				_busy = value; RaisePropertyChanged();
			}
		}
		/// <summary>
		/// currenlty loaded subbreddit, instance needed to search posts
		/// </summary>
		public Subreddit Subreddit { get; set; }

		public SearchResultVM(ListVM listVM, string query, Subreddit subreddit)
		{
			ListVM = listVM;
			ListVM.LoadNextPost = new RelayCommand(() => LoadNextPostMethod(), canExecute: () => Subreddit != null);
			Query = query;
			Subreddit = subreddit;

            if(SessionContext.Context.Reddit.User != null)
                foreach (var sub in SessionContext.Context.Reddit.User.SubscribedSubreddits)
                    SubscribedSubreddits.Add(sub.Name);

			Task.Run(() =>InitSubs());
			if (Subreddit != null)
				InitPosts();
		}

		private async Task InitSubs()
		{
			Subreddits.Clear();
			await LoadSubs(0, 5);
		}

		private async Task LoadSubs(int toSkip, int toTake)
		{
			List<Subreddit> subs = new List<Subreddit>();
            Busy = true;

            await Task.Run(() =>
			{
				try
				{
                    subs = SessionContext.Context.Reddit.SearchSubreddits(Query).Skip(toSkip).Take(toTake).ToList();
				}
				catch (Exception)
				{
                    // oof
                }
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

		public ICommand SubredditHover
		{
			get
			{
				return new RelayCommand<Subreddit>((sub) =>
				{
					this.MousedOverSubreddit = sub;
				});
			}
		}

		public ICommand SubredditLinkClick
		{
			get
			{
				return new RelayCommand(() =>
				{
					Messenger.Default.Send(new ChangeSubredditMessage(this.MousedOverSubreddit.Name));
				});
			}
		}

		public ICommand SubredditSubscribeClick
		{
			get
			{
                return new RelayCommand<Subreddit>((sub) =>
                {
                    this.MousedOverSubreddit.Subscribe();
                    this.SubscribedSubreddits.Add(sub.Name);
					Messenger.Default.Send(new SubredditSubscribedMessage(sub.Name));
				},(sub) => SessionContext.Context.Reddit.User != null && !IsSubscribed(sub));
			}
		}
        private bool IsSubscribed(Subreddit subreddit) => SubscribedSubreddits.Contains(subreddit.Name);
       
        public ICommand SubredditUnsubscribeClick
        {
            get
            {
                return new RelayCommand<Subreddit>((sub) =>
                {
                    this.MousedOverSubreddit.Unsubscribe();
                    this.SubscribedSubreddits.Remove(sub.Name);
                }, (sub) => SessionContext.Context.Reddit.User != null && !IsSubscribed(sub));
            }
        }
        public ICommand LoadNextSubreddit
		{

			get
			{
				return new RelayCommand(async () =>
				{
					await LoadSubs(this.Subreddits.Count, 5);
				});
			}
		}

        public ICommand ChangeToViewCommand { get; set; }

        private async void InitPosts()
		{
			ListVM.Posts.Clear();
			await LoadPosts(ListVM.Posts.Count,3);
		}

		private async Task LoadPosts(int toSkip, int toTake)
		{
			List<LoadedPost> posts = new List<LoadedPost>();
			this.ListVM.Busy = true;
			await Task.Run(() =>
			{
				posts = Subreddit.Search(Query).Skip(toSkip).Select(post => {
                    var p = new LoadedPost(post);
                    p.ItemClickAction = () => ChangeToViewCommand.Execute(new PostVM(p) { ReturnToPreviousViewAction = () => ChangeToViewCommand.Execute(this) });
                    return p;
                }).Take(toTake).ToList();
			});
			IObservable<LoadedPost> postsToLoad = posts.ToObservable();
			postsToLoad.Subscribe(p =>
			{
				Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action<LoadedPost>((post) => ListVM.Posts.Add(post)), p);
			}, () =>
			{
                this.ListVM.Busy = false;
			});
		}

		private async void LoadNextPostMethod()
		{
			if (ListVM.Busy)
				return;
			ListVM.Busy = true;
			{
				var newPosts = await Task.Run(() =>
				{
					var currentPostCount = ListVM.Posts.Count;
					return Subreddit.Search(Query).Skip(currentPostCount).Take(3).Select(post => {
                        var p = new LoadedPost(post);
                        p.ItemClickAction = () => ChangeToViewCommand.Execute(new PostVM(p) { ReturnToPreviousViewAction = () => ChangeToViewCommand.Execute(this) });
                        return p;
                    }).ToList();
				});
				foreach (var post in newPosts)
					ListVM.Posts.Add(post);
				ListVM.RaisePropertyChanged("Posts");
			}
			ListVM.Busy = false;
		}
	}
}
