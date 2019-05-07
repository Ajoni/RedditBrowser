using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBrowser.ViewModel.Messages
{
	class SearchMessage
	{
		public string Query { get; set; }

		public SearchMessage(string query)
		{
			Query = query;
		}
	}
}
