using Logic;
using RedditBrowser.VM;
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
        MainWindowVM VM = new MainWindowVM();

        public MainWindow()
        {
            InitializeComponent();

												this.DataContext = VM;
        }

    }

}
