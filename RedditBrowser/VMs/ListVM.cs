using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.VMs
{
				public class ListVM : IViewModel
				{
								public ObservableCollection<Post> Posts { get; set; } = new ObservableCollection<Post>();
				}
}
