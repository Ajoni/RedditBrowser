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
using System.Windows.Shapes;

namespace RedditBrowser
{
    /// <summary>
    /// Interaction logic for SelectSubDialog.xaml
    /// </summary>
    public partial class SelectSubDialog : Window
    {
        public string subName{
            get { return txtAnswer.Text; }
        }

        public SelectSubDialog()
        {
            InitializeComponent();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
