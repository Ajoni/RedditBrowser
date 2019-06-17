using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.ViewModel.Messages;
using RedditSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.Classes
{
    public class SessionContext : INotifyPropertyChanged
    {
        public static SessionContext Context { get; } = new SessionContext();
        public Reddit Reddit { get; set; } = new Reddit();
        public bool IsUserLoggedIn { get => Reddit.User != null; }

        public SessionContext()
        {
            RegisterMesssages();
        }

        private void ReceiveMessage(LoginChangeMessage message)
        {
            Update(message.UserLoginResult);
            Messenger.Default.Send(new SessionContextUpdatedMessage());
        }

        private void RegisterMesssages()
        {
            Messenger.Default.Register<LoginChangeMessage>(this, (message) => ReceiveMessage(message));
        }

        internal void Update(UserLoginResult userLoginResult)
        {
            if (userLoginResult != null)
                Reddit = new Reddit(userLoginResult.WebAgent, true);
            else
                Reddit = new Reddit();
            OnPropertyChanged(nameof(Reddit));
            OnPropertyChanged(nameof(IsUserLoggedIn));
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
