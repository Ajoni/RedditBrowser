using GalaSoft.MvvmLight.CommandWpf;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static RedditSharp.Things.VotableThing;

namespace RedditBrowser.Classes
{
	public class LoadedComment : INotifyPropertyChanged
	{
		private bool? _liked;
		private int _score;

        private string _replyText;


        public string ReplyText
        {
            get => _replyText; set
            {
                _replyText = value; OnPropertyChanged();
            }
        }
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
        public ObservableCollection<LoadedComment> Comments { get; } = new ObservableCollection<LoadedComment>();
        public Comment Comment { get; set; }

        public LoadedComment(Comment comment)
        {
            this.Comment = comment;
            this.Body = comment.Body;
            this.AuthorName = comment.AuthorName;
            this.AuthorFlairText = comment.AuthorFlairText;
            this.Score = comment.Score;
            this.Liked = comment.Liked;
            this.Created = comment.Created;
            foreach (var reply in comment.Comments)
                this.Comments.Add(new LoadedComment(reply));
        }

        public ICommand UpvoteCommentClick
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (Liked.HasValue && Liked.Value) ClearVote(); else Upvote();
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
                    if (Liked.HasValue && !Liked.Value) ClearVote(); else Downvote();
                });
            }
        }
        public ICommand PostComment
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Reply(ReplyText);
                    ReplyText = "";
                });
            }
        }

        public ICommand AuthorNameLinkClick
        {
            get
            {
                return new RelayCommand<MouseButtonEventArgs>((args) =>
                {
                    var comment = (LoadedComment)((System.Windows.Controls.TextBlock)args.Source).DataContext;
                    System.Diagnostics.Process.Start($"https://www.reddit.com/user/{comment.Comment.AuthorName}/");
                    args.Handled = true;
                });
            }
        }

        private void Downvote()
		{
			Comment.Downvote();
			UpdateVoteProps();
		}
        private void Upvote()
		{
			Comment.Upvote();
			UpdateVoteProps();
		}
        private void SetVote(VoteType type)
		{
			Comment.SetVote(type);
			UpdateVoteProps();
		}
        private void ClearVote()
		{
			Comment.ClearVote();
			UpdateVoteProps();
		}
        private void UpdateVoteProps()
		{
			Score = Comment.Score;
			Liked = Comment.Liked;
		}
        private void Reply(string message)
		{
			var comment = this.Comment.Reply(message);
            Comments.Add(new LoadedComment(comment));
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
