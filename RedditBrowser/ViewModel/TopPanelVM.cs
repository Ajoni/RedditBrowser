using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel.Messages;
using System.Windows.Input;

namespace RedditBrowser.ViewModel
{
	public class TopPanelVM : IViewModel
	{
        private string _SubredditName;

		public string Header { get; set; }
		public string SubredditName { get { return "r/" + _SubredditName; } set { _SubredditName = value; } }
		public string Search { get; set; }

		public ICommand GoToListView => new DelegateCommand((a) =>
		{
			Messenger.Default.Send(new GoToListViewMessage());
		});
	}
}
