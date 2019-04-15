using Logic;
using RedditBrowser.Classes;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.VM
{
				public class MainWindowVM
				{
								public Subreddit Subreddit { get; set; }
								public bool Busy { get; set; }
								public Person Person { get; set; }
								private Manager Manager { get; set; }
				}
}
