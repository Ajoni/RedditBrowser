using RedditBrowser.ViewModel;
using System.Windows;

namespace RedditBrowser
{
	public partial class MainWindow : Window
	{
		private MainViewModel VM = new MainViewModel();

		public MainWindow()
		{
			InitializeComponent();

			//VM.Subreddit= new Reddit().GetSubreddit($"/r/Animemes");
			DataContext = VM;
		}

		private async void WindowMain_Loaded(object sender, RoutedEventArgs e)
		{
			await VM.Init();
		}
	}

}
