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
								public Post Post { get; set; }
								public string Comment { get; set; }
								public PostVM(Post post)
								{
												Post = post;
								}
				}
}
