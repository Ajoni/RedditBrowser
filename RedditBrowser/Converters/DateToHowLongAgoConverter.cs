using System;
using System.Globalization;
using System.Windows.Data;

namespace RedditBrowser.Converters
{
	public class DateToHowLongAgoConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var val = ((DateTimeOffset)value).UtcDateTime.AddHours(-6);
				var now = DateTime.Now;
				if (now.Year - val.Year > 0) return $"{now.Year - val.Year} years ago";
				else if (now.Month - val.Month > 0) return $"{now.Month - val.Month} months ago";
				else if (now.Day - val.Day > 0) return $"{now.Day - val.Day} days ago";
				else if (now.Hour - val.Hour > 0) return $"{now.Hour - val.Hour} hours ago";
				else if (now.Minute - val.Minute > 0) return $"{now.Minute - val.Minute} minutes ago";
				else return $" {now.Second - val.Second} seconds ago";
			}
			catch { return null; }
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
