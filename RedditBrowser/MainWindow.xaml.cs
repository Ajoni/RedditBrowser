using Logic;
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

												VM.Subreddit= new Reddit().GetSubreddit($"/r/ProgrammerHumor");
												this.DataContext = VM;
        }

								private void WindowMain_Loaded(object sender, RoutedEventArgs e)
								{
												this.VM.Init();
								}
				}

}
