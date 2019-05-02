using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.ViewModel.Messages
{
    public class ChangeSubredditMessage
    {
		public string Name { get; set; }

		public ChangeSubredditMessage(string name)
		{
			Name = name;
		}
	}
}
