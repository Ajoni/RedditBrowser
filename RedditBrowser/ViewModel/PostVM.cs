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
		private AuthenticatedUser _user;
		private string _comment;

		public LoadedPost Post { get; set; }
		public string Comment
		{
			get => _comment; set
			{
				_comment = value; RaisePropertyChanged(); RaisePropertyChanged("CommentButtonEnabled");
			}
		}
		public Comment MousedOverComment { get; set; }
		public AuthenticatedUser User { get => _user; set { _user = value; RaisePropertyChanged(); RaisePropertyChanged("CommentButtonEnabled"); } }
		public bool CommentButtonEnabled { get { return !string.IsNullOrEmpty(this.Comment) && this.User != null; } }

		public PostVM(LoadedPost post, AuthenticatedUser user)
		{
			Post = post;
			User = user;
		}

		public ICommand SubredditNameClick
		{
			get
			{
				return new RelayCommand(() =>
				{
					Messenger.Default.Send(new ChangeSubredditMessage(this.Post.Post.SubredditName));
				});
			}
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
					if (Post.Liked.HasValue && !Post.Liked.Value) Post.ClearVote(); else Post.Downvote();
				}
				, () =>
				{
					return this.Post != null && this.User != null;
				}, true);
			}
		}

		public ICommand PostComment
		{
			get
			{
				return new RelayCommand(() =>
				{
					this.Post.Comment(this.Comment);
				});
			}
		}

		public ICommand CommentHover
		{
			get
			{
				return new RelayCommand<Comment>((comment) =>
				{
					this.MousedOverComment = comment;
				});
			}
		}

		public ICommand UpvoteCommentClick
		{
			get
			{
				return new RelayCommand(() =>
				{
					if (MousedOverComment.Liked.HasValue && MousedOverComment.Liked.Value) MousedOverComment.ClearVote(); else MousedOverComment.Upvote();
				}
				, true);
			}
		}

		public ICommand DownvoteCommentClick
		{
			get
			{
				return new RelayCommand(() =>
				{
					if (MousedOverComment.Liked.HasValue && !MousedOverComment.Liked.Value) MousedOverComment.ClearVote(); else MousedOverComment.Downvote();
				}
				, true);
			}
		}
	}
}
