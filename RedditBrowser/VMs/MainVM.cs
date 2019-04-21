using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Classes;
using RedditBrowser.Helpers;
using RedditBrowser.VMs.Messages;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RedditBrowser.VMs
{
				public class MainVM : ViewModelBase
				{
								public IViewModel CurrentPage { get; set; }
								public TopPanelVM TopPanel { get; set; } = new TopPanelVM();
								public ListVM ListVM { get; set; } = new ListVM();
								public Subreddit Subreddit { get; set; }
								public User User { get; set; }
								public bool Busy { get; set; }


								public MainVM()
								{
												Messenger.Default.Register<GoToPageMessage>(this,(action) => ReceiveMessage(action));

												Subreddit = new Reddit().RSlashAll;
												//CurrentPage = ListVM;

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

								private object ReceiveMessage(GoToPageMessage action)
								{
												this.CurrentPage = action.Page;
												return null;
								}
				}
}
