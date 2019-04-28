using RedditBrowser.Classes;

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
