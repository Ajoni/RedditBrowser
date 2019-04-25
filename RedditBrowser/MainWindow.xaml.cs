using Logic;
using Newtonsoft.Json;
using RedditBrowser.Config;
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
        MainViewModel VM;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void WindowMain_Loaded(object sender, RoutedEventArgs e)
        {
            InitConfig();

            VM = new MainViewModel();
            this.DataContext = VM;
            await this.VM.Init();
        }

        private void InitConfig()
        {
            var configFile = GlobalPaths.Config;
            if (!File.Exists(configFile))
                return;

            string json = File.ReadAllText(configFile);
            var config = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            GlobalConfig.Set(config);
        }
    }

}
