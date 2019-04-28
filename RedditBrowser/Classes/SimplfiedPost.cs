using RedditSharp.Things;
using System;

namespace RedditBrowser.Classes
{
	public class SimplfiedPost
	{
		public Comment[] Comments { get; }
		public bool IsSpoiler { get; set; }
		public string Domain { get; set; }
		public bool IsSelfPost { get; set; }
		public string LinkFlairCssClass { get; set; }
		public string LinkFlairText { get; set; }
		public RedditUser Author { get; }
		public int CommentCount { get; set; }
		public Uri Permalink { get; set; }
		public string SelfText { get; set; }
		public string SelfTextHtml { get; set; }
		public Uri Thumbnail { get; set; }
		public string Title { get; set; }
		public string SubredditName { get; set; }
		public bool NSFW { get; set; }
		public Subreddit Subreddit { get; }
		public Uri Url { get; set; }
		public int Score{ get; set; }
		public DateTimeOffset Created { get; set; }

		public SimplfiedPost(Post post)
		{
			Comments = post.Comments;
			IsSpoiler = post.IsSpoiler;
			Domain = post.Domain;
			IsSelfPost = post.IsSelfPost;
			LinkFlairCssClass = post.LinkFlairCssClass;
			LinkFlairText = post.LinkFlairText;
			Author = post.Author;
			CommentCount = post.CommentCount;
			Permalink = post.Permalink;
			SelfText = post.SelfText;
			SelfTextHtml = post.SelfTextHtml;
			Thumbnail = post.Thumbnail;
			Title = post.Title;
			SubredditName = post.SubredditName;
			NSFW = post.NSFW;
			Subreddit = post.Subreddit;
			Url = post.Url;
			Score = post.Score;
			Created = post.Created;
		}
	}
}
