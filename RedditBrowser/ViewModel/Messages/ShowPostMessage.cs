using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.ViewModel.Messages
{
	public class ShowPostMessage
	{
		public Post Post { get; set; }

		public ShowPostMessage(Post post)
		{
			Post = post;
		}
	}
}
