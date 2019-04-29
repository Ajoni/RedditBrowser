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

		public Post Post { get; set; }
		public string Comment { get; set; }
		//public WindowState WindowState
		//{
		//	get => _windowState; set { _windowState = value; RaisePropertyChanged(); }
		//}

		public PostVM(Post post)
		{
			Post = post;
			//RegisterMessages();
		}

		//private void RegisterMessages()
		//{
		//	Messenger.Default.Register<ShowPostMessage>(this, (action) => ReceiveMessage(action));
		//}

		//private void ReceiveMessage(ShowPostMessage message) { this.WindowState = WindowState.Open; this.Post = message.Post; }

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
					return this.Post != null;
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
					return this.Post != null;
				},true);
			}
		}
	}
}
