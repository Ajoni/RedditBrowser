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
												BitmapImage bi = new BitmapImage((Uri)value);
												return bi;
								}

								public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
								{
												throw new NotImplementedException();
								}
				}
}
