﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace RedditBrowser.Converters
{
    public class ImgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                try
                {
                    bi.UriSource = (Uri)value;
                }
                catch (UriFormatException)
                {
                    return null;
                }
                bi.EndInit();
                return bi;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
