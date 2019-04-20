using RedditBrowser.Classes;
using RedditBrowser.Helpers;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RedditBrowser.VMs
{
				public class MainVM
				{
								public IViewModel CurrentPage { get; set; }
								public TopPanelVM TopPanel { get; set; } = new TopPanelVM();
								public ListVM ListVM { get; set; } = new ListVM();
								public Subreddit Subreddit { get; set; }
								public User User { get; set; }
								public bool Busy { get; set; }


								public MainVM()
								{
												Subreddit = new Reddit().RSlashAll;
												CurrentPage = ListVM;

												TopPanel.Header = Subreddit.HeaderImage;
												TopPanel.SubredditName = Subreddit.Name;

								}

								public async Task Init()
								{
												await Task.Run(async () =>
												{
																var posts = LoadPosts(0, 10);
																await Application.Current.Dispatcher.BeginInvoke(new Action(() => this.ListVM.Posts.AddRange(posts)));
												});
								}

								private List<Post> LoadPosts(int from, int amount)
								{
												return Subreddit.Posts.Skip(from).Take(amount).ToList();
								}
				}
}
