using RedditBrowser.Classes;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.ViewModel
{
				public class PostVM : IViewModel
				{
								public SimplfiedPost Post { get; set; }
								public string Comment { get; set; }
								public PostVM(SimplfiedPost post)
								{
												Post = post;
								}
				}
}
