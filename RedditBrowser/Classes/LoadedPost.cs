using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static RedditSharp.Things.VotableThing;

namespace RedditBrowser.Classes
{
	public class LoadedPost : INotifyPropertyChanged
	{
		private bool? _liked;
		private int _score;

		public ObservableCollection<Comment> Comments { get; } = new ObservableCollection<Comment>();
		//public bool IsSpoiler { get; set; }
		//public string Domain { get; set; }
		//public bool IsSelfPost { get; set; }
		//public string LinkFlairCssClass { get; set; }
		public string LinkFlairText { get; set; }
		public RedditUser Author { get; }
		//public int CommentCount { get; set; }
		public Uri Permalink { get; set; }
		public string SelfText { get; set; }
		//public string SelfTextHtml { get; set; }
		public Uri Thumbnail { get; set; }
		public string Title { get; set; }
		//public string SubredditName { get; set; }
		//public bool NSFW { get; set; }
		//public Subreddit Subreddit { get; }
		public Uri Url { get; set; }
		public int Score
		{
			get => _score; set
			{
				_score = value; OnPropertyChanged();
			}
		}
		public DateTimeOffset Created { get; set; }
		public bool? Liked
		{
			get => _liked; set
			{
				_liked = value; OnPropertyChanged();
			}
		}
		public Post Post { get; set; }

		public LoadedPost(Post post)
		{
			//Id = post.Id;
			//FullName = post.FullName;
			//Kind = post.Kind;
			foreach (var comment in post.Comments)
				Comments.Add(comment);
			//IsSpoiler = post.IsSpoiler;
			//Domain = post.Domain;
			//IsSelfPost = post.IsSelfPost;
			//LinkFlairCssClass = post.LinkFlairCssClass;
			LinkFlairText = post.LinkFlairText;
			Author = post.Author;
			//CommentCount = post.CommentCount;
			Permalink = post.Permalink;
			SelfText = post.SelfText;
			//SelfTextHtml = post.SelfTextHtml;
			Thumbnail = post.Thumbnail;
			Title = post.Title;
			//SubredditName = post.SubredditName;
			//NSFW = post.NSFW;
			//Subreddit = post.Subreddit;
			Url = post.Url;
			Score = post.Score;
			Created = post.Created;
			Liked = post.Liked;
			Post = post;
			//WebAgent = webAgent;
			//Reddit = reddit;
		}

		public void Downvote()
		{
			Post.Downvote();
			UpdateVoteProps();
		}
		public void Upvote()
		{
			Post.Upvote();
			UpdateVoteProps();
		}
		public void SetVote(VoteType type)
		{
			Post.SetVote(type);
			UpdateVoteProps();
		}
		public void ClearVote()
		{
			Post.ClearVote();
			UpdateVoteProps();
		}
		private void UpdateVoteProps()
		{
			Score = Post.Score;
			Liked = Post.Liked;
		}

		public void Comment(string message)
		{
			this.Post.Comment(message);
			foreach (var comment in this.Post.Comments)
				Comments.Add(comment);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
