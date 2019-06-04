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
        public bool Reload { get; set; }
        public ChangeSubredditMessage(string name, bool reload = false)
		{
			Name = name;
            Reload = reload;
		}
	}
}
