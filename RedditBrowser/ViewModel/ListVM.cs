using GalaSoft.MvvmLight.Messaging;
using RedditBrowser.Helpers;
using RedditBrowser.ViewModel.Messages;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RedditBrowser.ViewModel
{
				public class ListVM : IViewModel, INotifyPropertyChanged
				{
								public ObservableCollection<Post> Posts { get; set; } = new ObservableCollection<Post>();
								public Post MousedOverPost { get; set; }

								private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
								{
												PropertyChangedEventHandler handler = PropertyChanged;
												if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
								}
								public event PropertyChangedEventHandler PropertyChanged;

								public ListVM()
								{
												Messenger.Default.Send(new GoToPageMessage(this));
								}

								public ICommand ItemHover
								{
												get
												{
																return new DelegateCommand((a) =>
																		{
																						this.MousedOverPost = (Post)a;
																		}
																		, (a) =>
																		{
																						return Posts.Count > 0;
																		});
																				}
								}

								public ICommand ItemClicked
								{
												get
												{
																return new DelegateCommand((a) =>
																{
																				Messenger.Default.Send(new GoToPageMessage(new PostVM(this.MousedOverPost)));
																}
																		, (a) =>
																		{
																						return Posts.Count > 0;
																		});
												}
								}
				}
}
