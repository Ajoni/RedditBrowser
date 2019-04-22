using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Classes;
using RedditBrowser.ViewModel.Messages;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
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
								public User User { get; set; }
								public bool Busy { get; set; }


								public MainViewModel()
								{
												Messenger.Default.Register<GoToPageMessage>(this, (action) => ReceiveMessage(action));
												this.ListVM = new ListVM();

												Subreddit = new Reddit().RSlashAll;

												TopPanel.Header = Subreddit.HeaderImage;
												TopPanel.SubredditName = Subreddit.Name;

								}

								public async Task Init()
								{
												List<Post> posts = new List<Post>();
												await Task.Run(() =>
												{
																posts = LoadPosts(0, 2);
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

								private object ReceiveMessage(GoToPageMessage message)
								{
												this.CurrentPage = message.Page;
												return null;
								}

				}
}