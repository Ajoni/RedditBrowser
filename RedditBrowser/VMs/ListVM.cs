using RedditBrowser.Helpers;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RedditBrowser.VMs
{
				public class ListVM : IViewModel, INotifyPropertyChanged
				{
								public ObservableCollection<Post> Posts { get; set; } = new ObservableCollection<Post>();
								public Post MouvedOverPost { get; set; }

								private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
								{
												PropertyChangedEventHandler handler = PropertyChanged;
												if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
								}
								public event PropertyChangedEventHandler PropertyChanged;

								//public ICommand ItemClicked {get { return new DelegateCommand<object>(FuncToCall, FuncToEvaluate); }
				}
}
