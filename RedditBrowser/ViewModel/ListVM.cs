using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Classes;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel.Messages;
using RedditSharp.Things;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace RedditBrowser.ViewModel
{
    public class ListVM : ViewModelBase, IViewModel
	{
		private bool                _busy;

		public ObservableCollection<LoadedPost>   Posts           { get; set; } = new ObservableCollection<LoadedPost>();
		public bool                         Busy            { get => _busy; set { _busy = value; RaisePropertyChanged(); } }
		public PostVM PostVM { get; private set; }

		public ListVM(bool goTo = true)
        {
			if(goTo)
				Messenger.Default.Send(new GoToPageMessage(this));
        }

        public RelayCommand LoadNextPost { get; set; }

    }
}
