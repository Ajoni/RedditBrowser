using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel.Messages;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace RedditBrowser.ViewModel
{
	public class LoginVM : ViewModelBase
	{
		private WindowState _windowState;
		private bool _busy;

		public string Password { get; set; }
		public string Username { get; set; }
		public bool Busy { get => _busy; set { _busy = value; RaisePropertyChanged(); } }
		public WindowState WindowState
		{
			get => _windowState; set { _windowState = value; RaisePropertyChanged(); }
		}

		public LoginVM()
		{
			this.Busy = false;
			WindowState = WindowState.Closed;
			RegisterMessages();
		}

		public ICommand LoginClick => new DelegateCommand(async (a) =>
		{
			this.Busy = true;
			var user = await LoginHelper.LoginUser(this.Username, this.Password);
			Messenger.Default.Send(new LoginChangeMessage(user));
			this.Busy = false;
			this.WindowState = WindowState.Closed;
		});

		private void ReceiveMessage(ShowLoginMessage message) => this.WindowState = WindowState.Open;

		private void RegisterMessages()
		{
			Messenger.Default.Register<ShowLoginMessage>(this, (action) => ReceiveMessage(action));
		}
	}
}
