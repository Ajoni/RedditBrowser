using Logic;
using RedditBrowser.Helpers;
using RedditBrowser.VMs;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RedditBrowser
{
    public partial class MainWindow : Window
    {
								MainVM VM = new MainVM();

        public MainWindow()
        {
            InitializeComponent();

												VM.Subreddit= new Reddit().GetSubreddit($"/r/Animemes");
												this.DataContext = VM;
        }

								private async void WindowMain_Loaded(object sender, RoutedEventArgs e)
								{
												List<Post> a = new List<Post>();
												await Task.Run(async () =>
												{
																a = this.VM.Subreddit.Posts.Take(10).ToList();
												});
												IObservable<Post> postsToLoad = a.ToObservable<Post>();
												postsToLoad.Subscribe<Post>(p =>
												{
																Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action<Post>((post) => this.VM.ListVM.Posts.Add(post)), p);
												}, () => { 
																//tutaj kiedys busyIndicator na notBusy ustawic (kiedy sie go doda)
												}
												);
								}
				}

}
