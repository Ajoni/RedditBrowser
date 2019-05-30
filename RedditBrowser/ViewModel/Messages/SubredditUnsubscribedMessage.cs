using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.ViewModel.Messages
{
    public class SubredditUnsubscribedMessage
    {
        public string Name { get; set; }

        public SubredditUnsubscribedMessage(string name)
        {
            Name = name;
        }
    }
}
