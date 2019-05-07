using Newtonsoft.Json;
using RedditBrowser.Config;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace RedditBrowser
{
    public partial class MainWindow : Window
    {
        MainViewModel VM;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowMain_Loaded(object sender, RoutedEventArgs e)
        {
            InitConfig();

            VM = new MainViewModel();
            this.DataContext = VM;
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
