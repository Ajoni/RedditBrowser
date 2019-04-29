using RedditSharp;
using RedditSharp.Things;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RedditBrowser.Classes
{
	public class SimplfiedPost : Post
	{
		public new Comment[] Comments { get; }
		public new bool IsSpoiler { get; set; }
		public new string Domain { get; set; }
		public new bool IsSelfPost { get; set; }
		public new string LinkFlairCssClass { get; set; }
		public new string LinkFlairText { get; set; }
		public new RedditUser Author { get; }
		public new int CommentCount { get; set; }
		public new Uri Permalink { get; set; }
		public new string SelfText { get; set; }
		public new string SelfTextHtml { get; set; }
		public new Uri Thumbnail { get; set; }
		public new string Title { get; set; }
		public new string SubredditName { get; set; }
		public new bool NSFW { get; set; }
		public new Subreddit Subreddit { get; }
		public new Uri Url { get; set; }
		public new DateTimeOffset Created { get; set; }

		public SimplfiedPost(Post post, WebAgent webAgent, Reddit reddit)
		{
			Id = post.Id;
			FullName = post.FullName;
			Kind = post.Kind;
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
			WebAgent = webAgent;
			Reddit = reddit;
		}


	}
}
