using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.ViewModel.Messages
{
    public class UnsubscribeMessage
    {
        public string Name { get; set; }

        public UnsubscribeMessage(string name)
        {
            Name = name;
        }
    }
}
