using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static RedditSharp.Things.VotableThing;

namespace RedditBrowser.Classes
{
	public class LoadedComment : INotifyPropertyChanged
	{
		private bool? _liked;
		private int _score;

		public string AuthorName { get; set; }
		public string AuthorFlairText { get; set; }
		public string Body { get; set; }
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
		public Comment Comment { get; set; }

		public void Downvote()
		{
			Comment.Downvote();
			UpdateVoteProps();
		}
		public void Upvote()
		{
			Comment.Upvote();
			UpdateVoteProps();
		}
		public void SetVote(VoteType type)
		{
			Comment.SetVote(type);
			UpdateVoteProps();
		}
		public void ClearVote()
		{
			Comment.ClearVote();
			UpdateVoteProps();
		}
		private void UpdateVoteProps()
		{
			Score = Comment.Score;
			Liked = Comment.Liked;
		}

		public void Reply(string message)
		{
			this.Comment.Reply(message);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
