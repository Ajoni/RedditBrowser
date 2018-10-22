﻿using Logic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RedditBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region fields
        //handles reddit api and image cache
        private Manager manager = new Manager(new string[]{".jpg", ".png"});
        
        //needed for image zoom and pan
        private Point origin;
        private Point start;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            AddImageZoomAndPan();            
        }

        private void loadAndShow()
        {
            image.Source = ((MediaBitmapImage)manager.GetCurrentMedia()).Image;
            titleLabel.Content = manager.GetTitle().ToString();
        }

        private void AddImageZoomAndPan()
        {
            var group = new TransformGroup();
            var st = new ScaleTransform();
            group.Children.Add(st);
            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);
            image.RenderTransform = group;
            image.RenderTransformOrigin = new Point(0.0, 0.0);
        }

        private void OpenSub_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenSub_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectSubDialog dialog = new SelectSubDialog();
            dialog.ShowDialog();
            if (dialog.DialogResult != true)
            {
                // TODO: Add some sort of msg to gui here.
                return;
            }

            string subredditName = dialog.subName;
            var result = manager.SetSubreddit(subredditName);
            if (!result)
            {
                MessageBox.Show($"Couldnt load selected reddit: {subredditName}.");
                return;
            }
            manager.Next();
            loadAndShow();
        }

        private void Download_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (image != null)
            {
                e.CanExecute = image.Source != null;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void Download_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "Image Files (*.jpg)|*.jpg",
                FileName = manager.GetCurrentPost().Id.ToString()
            };
            if (dialog.ShowDialog() == true)
            {
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }
        }

        private void Prev_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = manager.canGetPrevious();
        }

        private void Prev_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            manager.Previous();
            loadAndShow();
        }

        private void Next_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = manager.canGetNext();
        }

        private void Next_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            manager.Next();
            loadAndShow();
        }

        private void ImgLink_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (image != null)
            {
                e.CanExecute = image.Source != null;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ImgLink_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(manager.GetUri().ToString());
        }

        private void ShowButtons_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ShowButtons_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (Button item in ButtonsPanel.Children.OfType<Button>())
            {
                if (item.IsVisible)
                    item.Visibility = Visibility.Collapsed;
                else
                    item.Visibility = Visibility.Visible;
            }
        }

        private void ResetImg_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (image != null)
            {
                e.CanExecute = image.Source != null;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ResetImg_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ResetImgTransform();
        }

        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var st = (ScaleTransform)((TransformGroup)image.RenderTransform)
                .Children.First(x => x is ScaleTransform);
            var tt = (TranslateTransform)((TransformGroup)image.RenderTransform)
                .Children.First(x => x is TranslateTransform);

            double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;

                Point relative = e.GetPosition(image);
                double abosuluteX;
                double abosuluteY;

                abosuluteX = relative.X * st.ScaleX + tt.X;
                abosuluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += zoom;
                st.ScaleY += zoom;

                tt.X = abosuluteX - relative.X * st.ScaleX;
                tt.Y = abosuluteY - relative.Y * st.ScaleY;
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (image != null)
            {
                var tt = (TranslateTransform)((TransformGroup)image.RenderTransform)
                    .Children.First(x => x is TranslateTransform);
                start = e.GetPosition(this);
                origin = new Point(tt.X, tt.Y);
                this.Cursor = Cursors.Hand;
                image.CaptureMouse();
            }
        }

        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (image != null)
            {
                image.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }
        }

        void image_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ResetImgTransform();
        }

        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            if (image != null)
            {
                if (image.IsMouseCaptured)
                {
                    var tt = (TranslateTransform)((TransformGroup)image.RenderTransform)
                        .Children.First(x => x is TranslateTransform);
                    Vector v = start - e.GetPosition(this);
                    tt.X = origin.X - v.X;
                    tt.Y = origin.Y - v.Y;
                }
            }
        }

        private void ResetImgTransform()
        {
            if (image != null)
            {
                // reset zoom
                var st = (ScaleTransform)((TransformGroup)image.RenderTransform)
                .Children.First(x => x is ScaleTransform);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                // reset pan
                var tt = (TranslateTransform)((TransformGroup)image.RenderTransform)
                .Children.First(x => x is TranslateTransform);
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }
    }
}
