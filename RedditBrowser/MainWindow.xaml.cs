using Logic;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel;
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
								MainViewModel VM = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();

												//VM.Subreddit= new Reddit().GetSubreddit($"/r/Animemes");
												this.DataContext = VM;
        }

								private async void WindowMain_Loaded(object sender, RoutedEventArgs e)
								{
												await this.VM.Init();
								}
				}

}
