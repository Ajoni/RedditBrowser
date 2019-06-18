using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Classes;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel.Messages;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace RedditBrowser.ViewModel
{
    public class TopPanelVM : ViewModelBase, IViewModel
    {
        private string _SubredditName = "";
        private string _query;

        public string Header { get; set; }
        public string Query
        {
            get => _query; set
            {
                _query = value; RaisePropertyChanged();
            }
        }
        public ObservableCollection<SubredditComboboxLayout> Subreddits { get; set; }
        public ObservableCollection<string> SubscribedSubreddits { get; set; }
        public string SelectedSubreddit
        {
            get { return _SubredditName; }
            set
            {
                _SubredditName = value;
                RaisePropertyChanged();
            }
        }

        public TopPanelVM()
        {
            Subreddits = new ObservableCollection<SubredditComboboxLayout> { new SubredditComboboxLayout("all") };
            RegisterMessages();
        }

        #region Commands
        public RelayCommand GoToRAll => new RelayCommand(() =>
        {
            this.SelectedSubreddit = "all";
        });

        public ICommand ChangeSubreddit => new RelayCommand(() =>
{
    ChangeSubredditExec();
});

        private void ChangeSubredditExec()
        {
            if (Subreddits.Where(s => s.Name == this.SelectedSubreddit).ToList().Count == 0)
                Subreddits.Add(new SubredditComboboxLayout(this.SelectedSubreddit));
            Messenger.Default.Send(new ChangeSubredditMessage(SelectedSubreddit));
        }

        public ICommand LoginClick => new RelayCommand(() =>
        {
            Messenger.Default.Send(new ShowLoginMessage());
        });

        public ICommand LogoutClick => new RelayCommand(() =>
        {
            Messenger.Default.Send(new LoginChangeMessage(null));
        });

        public ICommand Search => new RelayCommand(() =>
        {
            if (!string.IsNullOrEmpty(Query))
                Messenger.Default.Send(new SearchMessage(Query));
        });
        #endregion
        public void PopulateCombobox(IEnumerable<Subreddit> subscribedSubreddits)
        {
            foreach (var item in subscribedSubreddits)
            {
                if (!Subreddits.Any(s => s.Name == item.Name))
                    Subreddits.Add(new SubredditComboboxLayout(item.Name));
                SubredditComboboxLayout.SubscribedSubreddits.Add(item.Name);
            }
        }

        public ICommand SubredditSelectionChange => new RelayCommand<System.Windows.Controls.SelectionChangedEventArgs>((e) =>
        {
            if (e.AddedItems.Count != 1)
                return;
            var subreddit = e.AddedItems[0] as SubredditComboboxLayout;
            if (subreddit == null)
                return;

            SelectedSubreddit = subreddit.Name;
            Messenger.Default.Send(new ChangeSubredditMessage(subreddit.Name));
        }, true);

        /// <summary>
        /// update selected sub when user clicks on subreddit link
        /// </summary>
        /// <param name="message"></param>
        private void ReceiveMessage(ChangeSubredditMessage message)
        {
            if (string.IsNullOrEmpty(message.Name))
                return;

            if (message.Name != this.SelectedSubreddit)
                this.SelectedSubreddit = message.Name;

            if (Subreddits.Where(s => s.Name == this.SelectedSubreddit).ToList().Count == 0)
            {
                Subreddits.Add(new SubredditComboboxLayout(this.SelectedSubreddit));
                updateComboboxLayouts();
            }

        }
        private void ReceiveMessage(SubredditSubscribedMessage message)
        {
            if (Subreddits.Where(s => s.Name == message.Name).ToList().Count == 0)
                Subreddits.Add(new SubredditComboboxLayout(this.SelectedSubreddit));
            if (!SubredditComboboxLayout.SubscribedSubreddits.Contains(message.Name))
                SubredditComboboxLayout.SubscribedSubreddits.Add(message.Name);
            updateComboboxLayouts();
        }
        private void ReceiveMessage(SubredditUnsubscribedMessage message)
        {
            SubredditComboboxLayout.SubscribedSubreddits.Remove(message.Name);
            updateComboboxLayouts();
        }
        private void ReceiveMessage(SessionContextUpdatedMessage message)
        {
            updateComboboxLayouts();
        }

        private void updateComboboxLayouts()
        {
            foreach (var sub in Subreddits)
                sub.NeedsUpdate = true;
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<ChangeSubredditMessage>(this, (message) => ReceiveMessage(message));
            Messenger.Default.Register<SubredditSubscribedMessage>(this, (message) => ReceiveMessage(message));
            Messenger.Default.Register<SubredditUnsubscribedMessage>(this, (message) => ReceiveMessage(message));
            Messenger.Default.Register<SessionContextUpdatedMessage>(this, (message) => ReceiveMessage(message));
        }

        public class SubredditComboboxLayout : INotifyPropertyChanged
        {
            public static ObservableCollection<string> SubscribedSubreddits { get; private set; } = new ObservableCollection<string>();
            public bool NeedsUpdate
            {
                set
                {
                    if (value)
                        SubUnsubButtonPropsChanged();
                }
            }

            private void SubUnsubButtonPropsChanged()
            {
                OnPropertyChanged(nameof(SubredditUnsubscribeClick)); OnPropertyChanged(nameof(SubredditSubscribeClick));
                OnPropertyChanged(nameof(IsSubscribed)); OnPropertyChanged(nameof(CanSubscribe));
            }

            public string Name { get; set; }

            public SubredditComboboxLayout(string name)
            {
                Name = name;
            }

            public ICommand SubredditUnsubscribeClick => new RelayCommand<string>((sub) =>
                                                                           {
                                                                               SubscribedSubreddits.Remove(sub);
                                                                               Messenger.Default.Send(new UnsubscribeMessage(sub));
                                                                               SubUnsubButtonPropsChanged();
                                                                           }, (sub) => IsSubscribed, true);

            public ICommand SubredditSubscribeClick => new RelayCommand<string>((sub) =>
                                                                         {
                                                                             SubscribedSubreddits.Add(sub);
                                                                             Messenger.Default.Send(new SubscribeMessage(sub));
                                                                             SubUnsubButtonPropsChanged();
                                                                         }, (sub) => CanSubscribe, true);

            public bool IsSubscribed { get => SubscribedSubreddits.Contains(Name) && SessionContext.Context.IsUserLoggedIn; }
            public bool CanSubscribe { get => !SubscribedSubreddits.Contains(Name) && Name != "all" && SessionContext.Context.IsUserLoggedIn; }

            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
