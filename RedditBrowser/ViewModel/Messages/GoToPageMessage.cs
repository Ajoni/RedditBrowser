namespace RedditBrowser.ViewModel.Messages
{
	public class GoToPageMessage
	{
		public IViewModel Page { get; set; }

		public GoToPageMessage(IViewModel page)
		{
			Page = page;
		}
	}
}
