using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel.Messages;
using System.Windows.Input;

namespace RedditBrowser.ViewModel
{
	public class TopPanelVM : IViewModel
	{
		public string Header { get; set; }
		public string SubredditName { get; set; }
		public string Search { get; set; }

		public ICommand GoToListView => new DelegateCommand((a) =>
			{
				Messenger.Default.Send(new GoToListViewMessage());
			});

		public ICommand LoginClick => new DelegateCommand((a) =>
		{
			Messenger.Default.Send(new ShowLoginMessage());
		});
	}
}
