using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.VMs
{
				public class ListVM : IViewModel, INotifyPropertyChanged
				{
								public ObservableCollection<Post> Posts { get; set; } = new ObservableCollection<Post>();

								private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
								{
												PropertyChangedEventHandler handler = PropertyChanged;
												if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
								}
								public event PropertyChangedEventHandler PropertyChanged;
				}
}
