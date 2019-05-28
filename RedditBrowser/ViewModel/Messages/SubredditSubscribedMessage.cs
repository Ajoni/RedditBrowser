using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.ViewModel.Messages
{
	class SubredditSubscribedMessage
	{
		public string Name { get; set; }

		public SubredditSubscribedMessage(string name)
		{
			this.Name = name;
		}
	}
}
