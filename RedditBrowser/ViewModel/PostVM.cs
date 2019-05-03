using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Classes;
using RedditBrowser.ViewModel.Messages;
using RedditSharp.Things;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace RedditBrowser.ViewModel
{
	public class PostVM : ViewModelBase, IViewModel
	{
		//private WindowState _windowState;
		private AuthenticatedUser _user;

		public Post Post { get; set; }
		public string Comment { get; set; }
		public Comment MousedOverComment { get; set; }
		public AuthenticatedUser User { get => _user; set { _user = value; RaisePropertyChanged(); } }
		//public WindowState WindowState
		//{
		//	get => _windowState; set { _windowState = value; RaisePropertyChanged(); }
		//}

		public PostVM(Post post, AuthenticatedUser user)
		{
			Post = post;
			User = user;
			//RegisterMessages();
		}

		public ICommand UpvoteClick
		{
			get
			{
				return new RelayCommand(() =>
				{
					if (Post.Liked.HasValue && Post.Liked.Value) Post.ClearVote(); else Post.Upvote();
				}
				, () =>
				{
					return this.Post != null && this.User != null;
				}, true);
			}
		}

		public ICommand DownvoteClick
		{
			get
			{
				return new RelayCommand(() =>
				{
					if (Post.Liked.HasValue && Post.Liked.Value) Post.ClearVote(); else Post.Downvote();
				}
				, () =>
				{
					return this.Post != null && this.User != null;
				}, true);
			}
		}

		//public ICommand CommentHover
		//{
		//	get
		//	{
		//		return new RelayCommand(() =>
		//		{
		//			this.MousedOverComment = (Post)a;
		//		});					
		//	}
		//}
	}
}
