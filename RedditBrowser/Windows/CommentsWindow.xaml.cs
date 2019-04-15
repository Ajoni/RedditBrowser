using RedditSharp.Things;
using System.Collections.Generic;
using System.Windows;

namespace RedditBrowser
{
    /// <summary>
    /// Interaction logic for CommentsWindow.xaml
    /// </summary>
    public partial class CommentsWindow : Window
    {
        private List<Comment> mComments;
        public CommentsWindow(List<Comment> comments)
        {
            InitializeComponent();

            mComments = comments;

            CommnetsTrV.ItemsSource = mComments;
        }

        public CommentsWindow()
        {
            InitializeComponent();
        }

        public void SetData(List<Comment> comments)
        {
            mComments = comments;
            CommnetsTrV.ItemsSource = mComments;
        }

        public void exit(bool exit)
        {
            if (exit)
                this.Close();
            else
                this.Hide();

        }

    }
}
