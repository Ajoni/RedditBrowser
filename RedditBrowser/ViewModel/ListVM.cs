using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Classes;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel.Messages;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace RedditBrowser.ViewModel
{
	public class ListVM : IViewModel, INotifyPropertyChanged
	{
		public ObservableCollection<SimplfiedPost> Posts { get; set; } = new ObservableCollection<SimplfiedPost>();
		public SimplfiedPost MousedOverPost { get; set; }

		private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public ListVM()
		{
			Messenger.Default.Send(new GoToPageMessage(this));
		}

		public ICommand ItemHover
		{
			get
			{
				return new DelegateCommand((a) =>
						{
							MousedOverPost = (SimplfiedPost)a;
						}
						, (a) =>
						{
							return Posts.Count > 0;
						});
			}
		}

		public ICommand ItemClicked
		{
			get
			{
				return new DelegateCommand((a) =>
				{
					Messenger.Default.Send(new GoToPageMessage(new PostVM(MousedOverPost)));
				}
						, (a) =>
						{
							return Posts.Count > 0;
						});
			}
		}
	}
}
