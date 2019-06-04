    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.ViewModel.Messages
{
    public class SubscribeMessage
    {
        public string Name { get; set; }

        public SubscribeMessage(string name)
        {
            Name = name;
        }
    }
}
