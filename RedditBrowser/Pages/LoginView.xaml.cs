using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel;
using RedditBrowser.ViewModel.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RedditBrowser.Pages
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
		public LoginVM LoginVM{ get; set; }

		public LoginView()
        {
            InitializeComponent();

			LoginVM = new LoginVM();
			DataContext = LoginVM;
		}

		private async void ButtonLogin_Click(object sender, RoutedEventArgs e)
		{
			this.LoginVM.Busy = true;
			var user = await LoginHelper.LoginUser(this.LoginVM.Username, this.watermarkpasswordboxPasswordd.Password);
			this.LoginVM.Busy = false;
			if (user == null)
				MessageBox.Show("Could not login");
			else
			{
				Messenger.Default.Send(new LoginChangeMessage(user));
				this.LoginVM.WindowState = Xceed.Wpf.Toolkit.WindowState.Closed;
			}
		}
	}
}
